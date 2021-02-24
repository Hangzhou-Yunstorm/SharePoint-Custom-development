using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.Services;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using DocumentsSP.Model;
using DocumentsSP.Helper;
using System.Web.Script.Serialization;
using System.Linq;

namespace DocumentsSP.Layouts
{
    public partial class FileDetail : LayoutsPageBase
    {
        /// <summary>
        /// 文件Guid
        /// </summary>
        public Guid fileUID;
        /// <summary>
        /// 文件FID
        /// </summary>
        public string fileFID;
        /// <summary>
        /// 文件名
        /// </summary>
        public string fileName;
        /// <summary>
        /// 文件ID
        /// </summary>
        public int fileID;
        /// <summary>
        /// 文件URL
        /// </summary>
        public string fileUrl;
        /// <summary>
        /// 文件路径
        /// </summary>
        public string filePath;
        /// <summary>
        /// 文件所属用户
        /// </summary>
        public string fileAuthor;
        /// <summary>
        /// 文件创建时间
        /// </summary>
        public string fileCreatTime;
        /// <summary>
        /// 文件所属用户的邮箱
        /// </summary>
        public string fileAuthorEmail;
        /// <summary>
        /// 文件描述
        /// </summary>
        public string fileDescription;
        /// <summary>
        /// 文件大小
        /// </summary>
        public string fileSize;
        /// <summary>
        /// 文件图标
        /// </summary>
        public string fileIcon;
        /// <summary>
        /// 网站地址
        /// </summary>
        public string webUrl;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            string uid = Request.QueryString["UId"];
            string fId = Request.QueryString["FID"];
            string url = Request.QueryString["Url"];

            string ru = Request.ServerVariables["HTTP_USER_AGENT"];
            bool isMobile = CommonHelper.IsMobile(ru);

            if (string.IsNullOrEmpty(uid) && string.IsNullOrEmpty(fId) && string.IsNullOrEmpty(url))
            {
                if (isMobile)
                {
                    Response.Write("<script>alert('Please enter the correct url !');window.history.back(-1);</script>");
                    return;
                }
                else
                {
                    Response.Write("<script>alert('Please enter the correct url !');window.location.href='/default.aspx'</script>");
                    return;
                }
            }
            if (!string.IsNullOrEmpty(url))
            {
                url = CommonHelper.DecodeUrl(url);
            }

            webUrl = SPContext.Current.Web.Url;
            //获取文件信息
            var item = GetFileInfoByID(uid, fId, url);
            if (item == null)
            {
                if (isMobile)
                {
                    Response.Write("<script>alert('The file was deleted or unauthorized or file is being synchronized from other sites !');window.history.back(-1);</script>");
                    return;
                }
                else
                {
                    Response.Write("<script>alert('The file was deleted or unauthorized or file is being synchronized from other sites !');window.location.href='/default.aspx'</script>");
                    return;
                }
            }
        }

        /// <summary>
        /// 新增当前用户浏览历史及点击次数
        /// </summary>
        /// <param name="uID">文件Guid</param>
        [WebMethod]
        public static string AddHistory(string uId)
        {
            var siteId = SPContext.Current.Site.ID;
            var user = SPContext.Current.Web.CurrentUser;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite site = new SPSite(siteId))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        web.AllowUnsafeUpdates = true;
                        try
                        {
                            var docs = web.Lists.TryGetList(CommonHelper.docListName);
                            var item = docs.GetItemByUniqueId(new Guid(uId));

                            // 添加浏览历史
                            try
                            {
                                var historyList = web.Lists.TryGetList(CommonHelper.historyListName);
                                var query = new SPQuery();
                                query.Query = "<Where><And><Eq><FieldRef Name='FID' /><Value Type='Text'>" + item["FID"].ToString() + "</Value></Eq>" +
                                    "<Eq><FieldRef Name='UName' LookupId='True' /><Value Type='User'>" + user.ID + "</Value></Eq></And></Where>";

                                var hList = historyList.GetItems(query);
                                if (hList != null && hList.Count > 0)
                                {
                                    SPListItem hItem = hList[0];
                                    hItem["Time"] = DateTime.Now.ToLocalTime();
                                    hItem.SystemUpdate(false);
                                }
                                else
                                {
                                    var fId = item["FID"].ToString();

                                    var listItem = historyList.AddItem();
                                    listItem["FileName"] = CommonHelper.GetFileUrl(item.Name, item["FID"].ToString());
                                    listItem["Time"] = DateTime.Now.ToLocalTime();
                                    listItem["UName"] = user;
                                    listItem["Creator"] = item.File.ModifiedBy;
                                    listItem["CreateTime"] = item["Modified"];
                                    listItem["Folder"] = CommonHelper.GetParentFolder(fId);
                                    listItem["FID"] = fId;
                                    listItem["IconUrl"] = CommonHelper.GetImage(item.File.IconUrl);

                                    listItem.Update();
                                }
                            }
                            catch (Exception e)
                            {
                                CommonHelper.SetErrorLog("FileDetail.aspx__AddHistory_AddHistory", e.Message);
                            }

                            // 添加点击次数
                            try
                            {
                                var dsd = web.Lists.TryGetList(CommonHelper.documentStatisticalDetails);
                                var dquery = new SPQuery();
                                dquery.Query = "<Where><And><Eq><FieldRef Name='FID' /><Value Type='Text'>" + item["FID"].ToString() + "</Value></Eq>" +
                                    "<Eq><FieldRef Name='Title' /><Value Type='Text'>" + CommonHelper.GetServerHostName() + "</Value></Eq></And></Where>";
                                var ditems = dsd.GetItems(dquery);
                                if (ditems != null && ditems.Count > 0)
                                {
                                    var ditem = ditems[0];
                                    ditem["ClickCount"] = Convert.ToInt32(ditem["ClickCount"]) + 1;
                                    ditem.Update();
                                }
                                else
                                {
                                    var ditem = dsd.AddItem();
                                    ditem["FID"] = item["FID"];
                                    ditem["Title"] = CommonHelper.GetServerHostName();
                                    ditem["ClickCount"] = 1;
                                    ditem.Update();
                                }
                            }
                            catch (Exception e)
                            {
                                CommonHelper.SetErrorLog("FileDetail.aspx__AddHistory_AddCount", e.Message);
                            }
                        }
                        catch (Exception e)
                        {
                            CommonHelper.SetErrorLog("FileDetail.aspx__AddHistory_GetFile", e.Message);
                        }
                        web.AllowUnsafeUpdates = false;
                    }
                }
            });
            return string.Empty;
        }

        /// <summary>
        /// 新增FID
        /// </summary>
        /// <param name="uId">文件Guid</param>
        private static string AddFID(Guid uId)
        {
            string fId = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
            var siteId = SPContext.Current.Site.ID;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite site = new SPSite(siteId))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        web.AllowUnsafeUpdates = true;
                        try
                        {
                            var docs = web.Lists.TryGetList(CommonHelper.docListName);
                            var item = docs.GetItemByUniqueId(uId);
                            if (item != null)
                            {
                                if (item["FID"] == null)
                                {
                                    item["FID"] = fId;
                                    item["ParentFolder"] = CommonHelper.GetParentFolder(fId);
                                }
                                item.SystemUpdate(false);
                            }
                        }
                        catch (Exception ex)
                        {
                            CommonHelper.SetErrorLog("FileDetail.aspx__AddClickCount", ex.Message);
                        }
                        web.AllowUnsafeUpdates = false;
                    }
                }
            });
            return fId;
        }

        /// <summary>
        /// 通uid获取文件信息
        /// </summary>
        /// <param name="url"></param>
        private SPListItem GetFileInfoByID(string uid, string fId, string url)
        {
            using (SPSite site = new SPSite(SPContext.Current.Site.ID))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    SPListItem item = null;
                    try
                    {
                        var fileList = web.Lists.TryGetList(CommonHelper.docListName);

                        if (!string.IsNullOrEmpty(uid) && CommonHelper.isGuid(uid))
                        {
                            try
                            {
                                item = fileList.GetItemByUniqueId(new Guid(uid));
                            }
                            catch (Exception ex)
                            {
                                CommonHelper.SetErrorLog("FileDetail.aspx__GetFileInfoByID_GetItemByUniqueId", ex.Message + "; Uid=" + uid);
                            }
                        }
                        else if (!string.IsNullOrEmpty(fId))
                        {
                            var query = new SPQuery();
                            query.ViewAttributes = "Scope=\"Recursive\"";
                            query.Query = "<Where><Eq><FieldRef Name='FID' /><Value Type='Text'>" + fId + "</Value></Eq></Where>";

                            var items = fileList.GetItems(query);
                            if (items != null && items.Count > 0)
                            {
                                item = items[0];
                            }
                        }
                        else if (!string.IsNullOrEmpty(url))
                        {
                            try
                            {
                                item = web.GetFile(url).Item;
                            }
                            catch (Exception ex)
                            {
                                CommonHelper.SetErrorLog("FileDetail.aspx__GetFileInfoByID_GetItemByUniqueId", ex.Message + "; Url=" + url);
                            }
                        }

                        if (item != null)
                        {
                            var file = item.File;

                            fileID = item.ID;
                            fileUID = item.UniqueId;
                            fileName = file.Name;
                            fileUrl = file.ServerRelativeUrl.Replace("'", "?");

                            try
                            {
                                string userName = item["Editor"].ToString();
                                SPFieldUser _user = (SPFieldUser)item.Fields["Creator"];
                                SPFieldUserValue userValue = (SPFieldUserValue)_user.GetFieldValue(userName);
                                var user = userValue.User;

                                fileAuthor = user.Name;
                                fileAuthorEmail = user.Email;
                            }
                            catch
                            {
                                fileAuthor = file.ModifiedBy.Name;
                                fileAuthorEmail = file.ModifiedBy.Email;
                            }
                            fileFID = item["FID"] == null ? AddFID(fileUID) : item["FID"].ToString();
                            fileCreatTime = Convert.ToDateTime(item["Modified"]).ToString("yyyy-MM-dd HH:mm:ss");
                            fileSize = CommonHelper.GetSize(item.File.Length);
                            fileIcon = "48_" + item.File.IconUrl.Replace("gif", "png");
                            filePath = file.ParentFolder.ServerRelativeUrl;

                            if (item["FileDescription"] != null)
                            {
                                fileDescription = item["FileDescription"].ToString();
                                if (fileDescription.Length > 100)
                                {
                                    fileDescription = fileDescription.Substring(0, 100) + "...";
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonHelper.SetErrorLog("FileDetail.aspx__GetFileInfoByID", ex.Message);
                    }
                    return item;
                }
            }
        }

        /// <summary>
        /// 获取下载次数/点击次数/综合评分
        /// </summary>
        /// <param name="fId">FID</param>
        /// <returns>下载次数/点击次数/综合评分</returns>
        [WebMethod]
        public static string Get3Count(string fId)
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            DocumentCalculatedModel model = new DocumentCalculatedModel();
            try
            {
                GetDocumentCalculatedTimer timer = new GetDocumentCalculatedTimer();
                var dcList = timer.GetDocumentCalculatedList(false);

                var item = dcList.FirstOrDefault(T => T.FID == fId);
                if (item != null)
                {
                    model = item;
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("FileDetail.aspx__Get3Count", ex.Message);
            }
            return jsonSerializer.Serialize(model); ;
        }

        /// <summary>
        /// 获取我的打分信息
        /// </summary>
        /// <returns>我的打分信息</returns>
        [WebMethod]
        public static string GetMyScore(string fId)
        {
            string myScore = "-1";
            //当前用户名
            var CurrentUser = SPContext.Current.Web.CurrentUser;
            var siteId = SPContext.Current.Site.ID;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite site = new SPSite(siteId))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        try
                        {
                            var scoreList = web.Lists.TryGetList(CommonHelper.scoreListName);
                            var query = new SPQuery();
                            query.Query = "<Where><And>" +
                                          "<Eq><FieldRef Name='FileID' /><Value Type='Text'>" + fId + "</Value></Eq>" +
                                          "<Eq><FieldRef Name='UName' LookupId='True' /><Value Type='User'>" + CurrentUser.ID + "</Value></Eq>" +
                                          "</And></Where>";
                            var myS = scoreList.GetItems(query);
                            if (myS.Count > 0)
                            {
                                myScore = myS[0]["Score"].ToString();
                            }
                        }
                        catch (Exception ex)
                        {
                            CommonHelper.SetErrorLog("FileDetail.aspx__GetMyScore", ex.Message);
                        }
                    }
                }
            });
            return myScore;
        }

        /// <summary>
        /// 获取文件评论信息
        /// </summary>
        /// <param name="fileFId">文件ID</param>
        /// <returns>评论信息</returns>
        [WebMethod]
        public static string GetComments(string fId)
        {
            string commentJson = "[]";
            List<CommentModel> comments = new List<CommentModel>();
            var siteId = SPContext.Current.Site.ID;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite site = new SPSite(siteId))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        try
                        {
                            var commentList = web.Lists.TryGetList(CommonHelper.commentListName);
                            var query = new SPQuery();
                            query.Query = "<OrderBy><FieldRef Name='Created' Ascending='False'></FieldRef></OrderBy>" +
                                                   "<Where>" +
                                                   "<Eq><FieldRef Name='FileID' /><Value Type='Text'>" + fId + "</Value></Eq>" +
                                                   "</Where>";
                            var cList = commentList.GetItems(query);
                            foreach (SPListItem item in cList)
                            {
                                var title = item["Title"];
                                var text = item["CommentText"];

                                string userName = item["UName"].ToString();
                                SPFieldUser _user = (SPFieldUser)item.Fields["UName"];
                                SPFieldUserValue userValue = (SPFieldUserValue)_user.GetFieldValue(userName);
                                var user = userValue.User;

                                var time = Convert.ToDateTime(item["Created"]).ToString(("yyyy-MM-dd HH:mm:ss"));
                                var scoreList = web.Lists.TryGetList(CommonHelper.scoreListName);
                                var squery = new SPQuery();
                                squery.Query = "<Where><And>" +
                                          "<Eq><FieldRef Name='FileID' /><Value Type='Text'>" + fId + "</Value></Eq>" +
                                          "<Eq><FieldRef Name='UName' LookupId='True' /><Value Type='User'>" + user.ID + "</Value></Eq>" +
                                          "</And></Where>";
                                var sitems = scoreList.GetItems(squery);

                                if (sitems != null && sitems.Count > 0)
                                {
                                    var score = Convert.ToDouble(sitems[0]["Score"]).ToString("f1");
                                    CommentModel ct = new CommentModel() { UserName = user.Name, Score = score, CommentText = text == null ? "" : text.ToString(), Time = time };
                                    comments.Add(ct);
                                }
                            }
                            if (comments.Count > 0)
                            {
                                //序列化
                                DataContractJsonSerializer json = new DataContractJsonSerializer(comments.GetType());
                                using (MemoryStream stream = new MemoryStream())
                                {
                                    json.WriteObject(stream, comments);
                                    commentJson = Encoding.UTF8.GetString(stream.ToArray());
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            CommonHelper.SetErrorLog("FileDetail.aspx__GetComments", ex.Message);
                        }
                    }
                }
            });
            return commentJson;
        }

        /// <summary>
        /// 设置评分
        /// </summary>
        /// <param name="score">评分</param>
        /// <returns>返回数据</returns>
        [WebMethod]
        public static string AddScore(string score, string fId)
        {
            var siteId = SPContext.Current.Site.ID;
            var CurrentUser = SPContext.Current.Web.CurrentUser;

            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite site = new SPSite(siteId))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        web.AllowUnsafeUpdates = true;
                        try
                        {
                            var fileList = web.Lists.TryGetList(CommonHelper.docListName);
                            SPListItem fileItemS = null;
                            var fquery = new SPQuery();
                            fquery.ViewAttributes = "Scope=\"Recursive\"";
                            fquery.Query = "<Where><Eq><FieldRef Name='FID' /><Value Type='Text'>" + fId + "</Value></Eq></Where>";

                            var items = fileList.GetItems(fquery);
                            if (items != null && items.Count > 0)
                            {
                                fileItemS = items[0];
                            }

                            // 添加评分
                            var scoreList = web.Lists.TryGetList(CommonHelper.scoreListName);

                            SPQuery aquery = new SPQuery();
                            aquery.Query = "<Where><And>" +
                                          "<Eq><FieldRef Name='FileID' /><Value Type='Text'>" + fId + "</Value></Eq>" +
                                          "<Eq><FieldRef Name='UName' LookupId='True' /><Value Type='User'>" + CurrentUser.ID + "</Value></Eq>" +
                                          "</And></Where>";
                            var myS = scoreList.GetItems(aquery);
                            if (myS == null || myS.Count == 0)
                            {
                                var listItem = scoreList.AddItem();
                                listItem["Title"] = fileItemS.Name;
                                listItem["FileID"] = fId;
                                listItem["Score"] = double.Parse(score);
                                listItem["UName"] = CurrentUser;
                                listItem["ListID"] = CommonHelper.docListName;
                                listItem.Update();
                            }
                        }
                        catch (Exception ex)
                        {
                            CommonHelper.SetErrorLog("FileDetail.aspx__AddScore", ex.Message);
                        }
                        web.AllowUnsafeUpdates = false;
                    }
                }
            });

            return string.Empty;
        }

        /// <summary>
        /// 添加评论
        /// </summary>
        /// <param name="comment">评论信息</param>
        /// <returns>返回数据</returns>
        [WebMethod]
        public static string AddComment(string comment, string fId, string fileName)
        {
            fileName = fileName.Replace("?", "'");

            var CurrentUser = SPContext.Current.Web.CurrentUser;
            var siteId = SPContext.Current.Site.ID;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite site = new SPSite(siteId))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        web.AllowUnsafeUpdates = true;
                        try
                        {
                            // 添加评论
                            var cList = web.Lists.TryGetList(CommonHelper.commentListName);
                            var listItem = cList.AddItem();
                            listItem["Title"] = fileName;
                            listItem["FileID"] = fId;
                            listItem["CommentText"] = comment;
                            listItem["UName"] = CurrentUser;
                            listItem["ListID"] = CommonHelper.docListName;
                            listItem.Update();
                        }
                        catch (Exception ex)
                        {
                            CommonHelper.SetErrorLog("FileDetail.aspx__AddComment", ex.Message);
                        }
                        web.AllowUnsafeUpdates = false;
                    }
                }
            });
            return string.Empty;
        }

        /// <summary>
        /// 发送邮件反馈
        /// </summary>
        /// <param name="content">反馈内容</param>
        /// <param name="uId">文件Guid</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fileAuthorEmail">文件作者邮箱</param>
        /// <returns>邮件发送结果</returns>
        [WebMethod]
        public static string SendEmail(string content, string fId, string fileName, string fileAuthorEmail)
        {
            try
            {
                fileName = fileName.Replace("?", "'");
                content = "Commet: " + content;
                string webUrl = SPContext.Current.Web.Url;
                List<string> list = new List<string>();
                string mailadd = fileAuthorEmail;
                //mailadd = "zhang.chuang@yunstorm.com"; // TODO
                if (string.IsNullOrEmpty(mailadd))
                {
                    return "The author doesn't have an email !";
                }
                string name = SPContext.Current.Web.CurrentUser.Name;
                list.Add(mailadd);
                EmailHelper email = new EmailHelper(CommonHelper.mailUser, CommonHelper.mailPwd, CommonHelper.mailSmtp);
                email.mailSubject = "File feedback";
                string url = webUrl + "/_layouts/15/FileDetail.aspx?FID=" + fId;
                string mailContent = EmailHelper.GeneratorFeedbackMailContent(fileName, name, DateTime.Now.ToString("yyyy-MM-dd HH:mm"), url, content, url);
                email.mailBody = EmailHelper.GeneratorMailHeader(fileName) + mailContent + EmailHelper.GeneratorMailFooter("File feedback");
                email.isbodyHtml = true;    //是否是HTML
                email.mailToArray = list;
                email.Send(SPContext.Current.Site.ID, SPContext.Current.Web.CurrentUser);

                #region 记录日志
                var siteId = SPContext.Current.Site.ID;
                var cUser = SPContext.Current.Web.CurrentUser;
                using (SPSite site = new SPSite(siteId))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        web.AllowUnsafeUpdates = true;
                        try
                        {
                            var logList = web.Lists.TryGetList(CommonHelper.logListName);
                            var listItem = logList.AddItem();
                            listItem["Title"] = "Send feedback mail to :" + mailadd;
                            listItem["Operate"] = "Mail";
                            listItem["Operater"] = cUser;
                            listItem["Operator"] = cUser.Name;
                            listItem["OperatorId"] = cUser.LoginName;
                            listItem["ServerIP"] = CommonHelper.GetServerHostName();
                            listItem["Department"] = CommonHelper.GetSubGroupName(cUser);
                            listItem["DepartmentId"] = CommonHelper.GetSubGroupId(cUser);
                            listItem["ObjectName"] = url;
                            listItem["ObjectType"] = "File";
                            listItem.Update();
                        }
                        catch (Exception ex)
                        {
                            CommonHelper.SetErrorLog("EventReceiverRMSHandle__SendMailApprove__SendMailLog", ex.Message);
                        }
                        web.AllowUnsafeUpdates = false;
                    }
                }
                #endregion

                return "";
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("FileDetail.aspx__SendEmail", ex.Message);
                return ex.ToString();
            }
        }

        /// <summary>
        /// 获取文件预览信息
        /// </summary>
        /// <param name="uId">文件Guid</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fileUrl">文件URL</param>
        /// <returns>预览信息</returns>
        [WebMethod]
        public static string GetView(string uId, string fileName, string fileUrl)
        {
            try
            {
                SPListItem item = null;
                using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        var fileList = web.Lists.TryGetList(CommonHelper.docListName);
                        item = fileList.GetItemByUniqueId(new Guid(uId));
                    }
                }

                if (!IsView(item))
                {
                    return "0";
                }
                fileUrl = fileUrl.Replace("?", "'");
                fileName = fileName.Replace("?", "'");

                string type = GetType(fileName);
                string frame = "";
                if (type == "img")
                {
                    frame = "<div class='watermark_div'><img src = \"" + fileUrl + "\" /></div>";
                }
                else if (type == "office" || type == "xlsx")
                {
                    string src = "/_layouts/15/WopiFrame.aspx?sourcedoc={" + uId + "}&action=interactivepreview&wdSmallView=1";
                    frame = "<iframe name=\"searchiframe\" id=\"searchiframe\" width=\"710px\" height=\"400px\" marginwidth=\"0\" marginheight=\"0\" hspace=\"0\" vspace=\"0\" frameborder=\"0\" scrolling=\"no\" src=\"" + src + "\" ></iframe>";
                }
                else if (type == "video")
                {
                    frame = "<div class='watermark_div'><video autoplay=\"\" controls=\"\"><source src = \"" + fileUrl + "\"></source></video></div>";
                }
                else if (type == "xls")
                {
                    frame = "xls";
                }
                else
                {
                    frame = "1";
                }

                if ((type == "xls" || type == "xlsx") && (item.File.Length / 1024 > 1024 * 50))
                {
                    frame = "2big";
                }
                return frame;
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("FileDetail.aspx__GetView", ex.Message);
                return "";
            }
        }

        /// <summary>
        /// 获取要预览的文件类型
        /// </summary>
        /// <param name="name">文件名</param>
        /// <returns></returns>
        private static string GetType(string name)
        {
            try
            {
                string extension = Path.GetExtension(name).ToLower();

                if (extension == ".png" || extension == ".jpg" || extension == ".gif" || extension == ".bmp" || extension == ".jpeg" || extension == ".tiff" || extension == ".img")
                {
                    return "img";
                }
                else if (extension == ".pdf" || extension == ".docx" || extension == ".doc" || extension == ".pptx" || extension == ".ppt" || extension == ".csv")
                {
                    return "office";
                }
                else if (extension == ".mp4" || extension == ".ogg" || extension == ".mov" || extension == ".webm")
                {
                    return "video";
                }
                else if (extension == ".xlsx")
                {
                    return "xlsx";
                }
                else if (extension == ".xls")
                {
                    return "xls";
                }
                else
                {
                    return "other";
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("FileDetail.aspx__GetType", ex.Message);

                return "other";
            }
        }

        /// <summary>
        /// 查询当前用户是否有权限预览
        /// </summary>
        /// <returns>是否有权限预览</returns>
        private static bool IsView(SPListItem fileItem)
        {
            try
            {
                if (fileItem != null)
                {
                    SPUser cUser = SPContext.Current.Web.CurrentUser;
                    string editor = fileItem["Editor"].ToString();
                    string author = fileItem["Author"].ToString();
                    SPFieldUserValueCollection users = (SPFieldUserValueCollection)fileItem["Users"];
                    bool IsFullControl = CommonHelper.ObjToBool(fileItem["IsFullControl"]);
                    bool IsPrint = CommonHelper.ObjToBool(fileItem["IsPrint"]);
                    bool IsRead = CommonHelper.ObjToBool(fileItem["IsRead"]);
                    bool IsEdit = CommonHelper.ObjToBool(fileItem["IsEdit"]);
                    bool IsSave = CommonHelper.ObjToBool(fileItem["IsSave"]);
                    if (IsFullControl || IsPrint || IsRead || IsEdit || IsSave)
                    {
                        if (users == null)
                        {
                            return true;
                        }
                        foreach (SPFieldUserValue u in users)//加密策略应用用户匹配
                        {
                            if (u.User != null && (u.User.Name == "每个人" || u.User.Name.ToLower() == "everyone" || cUser.ID == u.User.ID))
                            {
                                return true;
                            }
                        }
                        string editorId = editor.Split(';')[0];
                        string authorId = author.Split(';')[0];
                        if (editorId == cUser.ID.ToString() || authorId == cUser.ID.ToString())
                        {
                            return true;
                        }
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("FileDetail.aspx__IsView", ex.Message);
                return false;
            }
        }
    }
}
