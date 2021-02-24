using System;
using Microsoft.SharePoint.WebControls;
using System.Web.Services;
using Microsoft.SharePoint;
using System.Web;
using DocumentsSP.Helper;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using Microsoft.SharePoint.Utilities;

namespace DocumentsSP.Layouts
{
    public partial class GetNav : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///获取当前用户名
        /// </summary>
        /// <returns>当前用户名</returns>
        [WebMethod]
        public static string GetUserName()
        {
            var userName = SPContext.Current.Web.CurrentUser.Name;
            return userName;
        }

        /// <summary>
        ///获取切站点li
        /// </summary>
        /// <returns>切站点li</returns>
        [WebMethod]
        public static string GetOthersCountryLi()
        {
            return CommonHelper.GetOthersCountryLi();
        }

        /// <summary>
        ///获取页脚
        /// </summary>
        /// <returns>页脚</returns>
        [WebMethod]
        public static string GetFooter()
        {
            string footer = string.Empty;
            try
            {
                footer = CommonHelper.ReadFooter();
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("GetNav.aspx__GetFooter", ex.Message);
            }
            return footer;
        }

        /// <summary>
        ///获取中文用户手册路径
        /// </summary>
        /// <returns>用户手册路径</returns>
        [WebMethod]
        public static string GetUserManualPath_CN()
        {
            return CommonHelper.userManualPath_cn;
        }


        /// <summary>
        ///获取英文用户手册路径
        /// </summary>
        /// <returns>用户手册路径</returns>
        [WebMethod]
        public static string GetUserManualPath_EN()
        {
            return CommonHelper.userManualPath_en;
        }

        /// <summary>
        /// 生成登录cookies和日志
        /// </summary>
        /// <returns>生成结果</returns>
        [WebMethod]
        public static string SetLoginLog()
        {
            try
            {
                var user = SPContext.Current.Web.CurrentUser;
                var uid = user.ID.ToString();
                if (!string.IsNullOrEmpty(uid) && HttpContext.Current.Request.Cookies[uid] == null)
                {
                    HttpCookie cookie = new HttpCookie(uid);
                    cookie.Value = uid;
                    cookie.Expires = DateTime.Now.ToLocalTime().AddHours(6);
                    HttpContext.Current.Response.Cookies.Add(cookie);

                    var siteId = SPContext.Current.Site.ID;
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        using (SPSite site = new SPSite(siteId))
                        {
                            using (SPWeb web = site.OpenWeb())
                            {
                                web.AllowUnsafeUpdates = true;
                                var logList = web.Lists.TryGetList(CommonHelper.logListName);

                                var listItem = logList.AddItem();
                                listItem["Title"] = user.Name + " login system";
                                listItem["Operate"] = "Login System";
                                listItem["Operater"] = user;
                                listItem["Operator"] = user.Name;
                                listItem["OperatorId"] = user.LoginName;
                                listItem["ServerIP"] = CommonHelper.GetServerHostName();
                                listItem["Department"] = CommonHelper.GetSubGroupName(user);
                                listItem["DepartmentId"] = CommonHelper.GetSubGroupId(user);
                                listItem["ObjectName"] = user.Name;
                                listItem["ObjectType"] = "Login";
                                listItem.Update();

                                web.AllowUnsafeUpdates = false;
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("GetNav.aspx__SetLoginLog", ex.Message);
            }
            return string.Empty;
        }

        /// <summary>
        /// 设置订阅目录
        /// </summary>
        /// <param name="folderIds">文件夹ID集合</param>
        /// <returns>设置结果</returns>
        [WebMethod]
        public static string SetSubscribeFolder(string folderIds)
        {
            string msg = string.Empty;
            try
            {
                string[] fIds = folderIds.Split(',');
                for (int i = 0; i < fIds.Length - 1; i++)
                {
                    var siteId = SPContext.Current.Site.ID;
                    var cUser = SPContext.Current.Web.CurrentUser;
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        using (SPSite site = new SPSite(siteId))
                        {
                            using (SPWeb web = site.OpenWeb())
                            {
                                web.AllowUnsafeUpdates = true;
                                SPList spList = web.Lists.TryGetList(CommonHelper.docListName);
                                var item = spList.GetItemById(Convert.ToInt32(fIds[i]));

                                SPList sdList = web.Lists.TryGetList(CommonHelper.sDirectoryList);

                                if (item["FID"] == null)
                                {
                                    var fId = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
                                    item["FID"] = fId;
                                    //item.SystemUpdate(false);
                                    item.Update();

                                    item.ModerationInformation.Comment = "Automatic Approval of items";
                                    item.ModerationInformation.Status = SPModerationStatusType.Approved;//自动审批文件夹
                                    item.Update();
                                }

                                var query = new SPQuery();
                                query.Query = "<Where><And><Eq><FieldRef Name=\"FolderId\" /><Value Type=\"Text\">" + item["FID"].ToString() + "</Value></Eq>" +
                                              "<Eq><FieldRef Name=\"Subscriber\" LookupId=\"True\" /><Value Type=\"User\">" + cUser.ID + "</Value></Eq></And></Where>";

                                var items = sdList.GetItems(query);
                                // 如果已经订阅, 无需重复订阅
                                if (items != null && items.Count > 0)
                                {
                                    SPListItem hItem = items[0];
                                    hItem["Time"] = DateTime.Now.ToLocalTime();
                                    hItem["Folder"] = CommonHelper.GetFolderUrl(item["FID"].ToString());
                                    hItem["FolderPath"] = item.Url;
                                    hItem.SystemUpdate(false);
                                }
                                else
                                {
                                    var listItem = sdList.AddItem();
                                    listItem["Folder"] = CommonHelper.GetFolderUrl(item["FID"].ToString());
                                    listItem["Subscriber"] = cUser;
                                    listItem["FolderPath"] = item.Url;
                                    listItem["Time"] = DateTime.Now.ToLocalTime();
                                    listItem["FolderId"] = item["FID"].ToString();
                                    listItem.Update();
                                }

                                web.AllowUnsafeUpdates = false;
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("GetNav.aspx_SetSubscribe", ex.Message);
            }
            return msg;
        }

        /// <summary>
        /// 获取用户是否推送邮件
        /// </summary>
        /// <returns>是否推送邮件(0:推送, 1:不推送)</returns>
        [WebMethod]
        public static string IsMailPush()
        {
            string isPush = "0";
            try
            {
                var currentUser = SPContext.Current.Web.CurrentUser;
                var siteId = SPContext.Current.Site.ID;
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(siteId))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            web.AllowUnsafeUpdates = true;
                            var mailUserList = web.Lists.TryGetList(CommonHelper.userEmaiPushList);
                            var userquery = new SPQuery();
                            userquery.Query = "<Where><Eq><FieldRef Name=\"EmailUser\" LookupId=\"True\" /><Value Type=\"User\">" + currentUser.ID + "</Value></Eq></Where>";
                            var userItems = mailUserList.GetItems(userquery);
                            if (userItems == null || userItems.Count == 0)
                            {
                                var listItem = mailUserList.AddItem();
                                listItem["EmailUser"] = currentUser;
                                listItem["IsPush"] = true;
                                listItem.Update();
                            }
                            else
                            {
                                var item = userItems[0];
                                var isP = Convert.ToBoolean(item["IsPush"]);
                                if (!isP)
                                {
                                    isPush = "1";
                                }
                            }
                            web.AllowUnsafeUpdates = false;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("GetNav.aspx__IsMailPush", ex.Message);
            }
            return isPush;
        }

        /// <summary>
        /// 设置用户推送邮件
        /// </summary>
        /// <returns>设置结果</returns>
        [WebMethod]
        public static string MailPush()
        {
            string msg = string.Empty;
            try
            {
                var currentUser = SPContext.Current.Web.CurrentUser;
                var siteId = SPContext.Current.Site.ID;
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(siteId))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            web.AllowUnsafeUpdates = true;
                            var mailUserList = web.Lists.TryGetList(CommonHelper.userEmaiPushList);
                            var userquery = new SPQuery();
                            userquery.Query = "<Where><Eq><FieldRef Name=\"EmailUser\" LookupId=\"True\" /><Value Type=\"User\">" + currentUser.ID + "</Value></Eq></Where>";
                            var userItems = mailUserList.GetItems(userquery);

                            if (userItems != null && userItems.Count > 0)
                            {
                                var item = userItems[0];
                                item["IsPush"] = true;
                                item.Update();
                            }
                            web.AllowUnsafeUpdates = false;

                        }
                    }
                });
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                CommonHelper.SetErrorLog("GetNav.aspx__IsMailPush", ex.Message);
            }
            return msg;
        }

        /// <summary>
        /// 设置取消用户推送邮件
        /// </summary>
        /// <returns>设置结果</returns>
        [WebMethod]
        public static string CancelMailPush()
        {
            string msg = string.Empty;
            try
            {
                var currentUser = SPContext.Current.Web.CurrentUser;
                var siteId = SPContext.Current.Site.ID;
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(siteId))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            web.AllowUnsafeUpdates = true;
                            var mailUserList = web.Lists.TryGetList(CommonHelper.userEmaiPushList);
                            var userquery = new SPQuery();
                            userquery.Query = "<Where><Eq><FieldRef Name=\"EmailUser\" LookupId=\"True\" /><Value Type=\"User\">" + currentUser.ID + "</Value></Eq></Where>";
                            var userItems = mailUserList.GetItems(userquery);

                            if (userItems != null && userItems.Count > 0)
                            {
                                var item = userItems[0];
                                item["IsPush"] = false;
                                item.Update();
                            }
                            web.AllowUnsafeUpdates = false;

                        }
                    }
                });
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                CommonHelper.SetErrorLog("GetNav.aspx__IsMailPush", ex.Message);
            }
            return msg;
        }

        /// <summary>
        /// 获取用户类型
        /// </summary>
        /// <returns>用户类型</returns>
        [WebMethod]
        public static string GetUserType()
        {
            UserTypeModel mo = new UserTypeModel();
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            try
            {
                var currentUser = SPContext.Current.Web.CurrentUser;
                var siteId = SPContext.Current.Site.ID;
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(siteId))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {

                            // 删除两个月之前的下载历史
                            //new DeleteDownloadTimer(web);

                            var subUser = "GKS Manage";
                            var approverUser = "GKS Approve";
                            var advanceUser = "GKS Advance";
                            if (currentUser.IsSiteAdmin)
                            {
                                mo.UserType = "Administrator";
                                mo.IsUpload = true;
                            }
                            else
                            {
                                var list = web.Lists.TryGetList(CommonHelper.whiteList);
                                var squery = new SPQuery();
                                squery.Query = "<Where><And>" +
                                              "<Eq><FieldRef Name=\"ObjectName\" LookupId=\"True\" /><Value Type=\"User\">" + currentUser.ID + "</Value></Eq>" +
                                              "<Eq><FieldRef Name=\"Permissions\" /><Value Type=\"MultiChoice\">" + subUser + "</Value></Eq>" +
                                              "</And></Where>";
                                var subUsers = list.GetItems(squery);
                                if (subUsers.Count > 0)
                                {
                                    mo.UserType = subUser;
                                    mo.IsUpload = true;
                                }
                                else
                                {
                                    var aquery = new SPQuery();
                                    aquery.Query = "<Where><And>" +
                                                  "<Eq><FieldRef Name=\"ObjectName\" LookupId=\"True\" /><Value Type=\"User\">" + currentUser.ID + "</Value></Eq>" +
                                                  "<Eq><FieldRef Name=\"Permissions\" /><Value Type=\"MultiChoice\">" + approverUser + "</Value></Eq>" +
                                                  "</And></Where>";
                                    var approverUsers = list.GetItems(aquery);
                                    if (approverUsers.Count > 0)
                                    {
                                        mo.UserType = approverUser;
                                        mo.IsUpload = false;
                                    }

                                    var adquery = new SPQuery();
                                    adquery.Query = "<Where><And>" +
                                                  "<Eq><FieldRef Name=\"ObjectName\" LookupId=\"True\" /><Value Type=\"User\">" + currentUser.ID + "</Value></Eq>" +
                                                  "<Eq><FieldRef Name=\"Permissions\" /><Value Type=\"MultiChoice\">" + advanceUser + "</Value></Eq>" +
                                                  "</And></Where>";
                                    var advanceUsers = list.GetItems(adquery);
                                    if (advanceUsers.Count > 0)
                                    {
                                        mo.UserType = "";
                                        mo.IsUpload = true;
                                    }
                                }
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("GetNav.aspx__GetUserType", ex.Message);
            }
            return jsonSerializer.Serialize(mo);
        }

        public class UserTypeModel
        {
            public string UserType { get; set; }
            public bool IsUpload { get; set; }
        }

        /// <summary>
        /// 获取用户类型
        /// </summary>
        /// <returns>用户类型</returns>
        [WebMethod]
        public static string GetReportUserType()
        {
            string msg = string.Empty;
            try
            {
                var currentUser = SPContext.Current.Web.CurrentUser;
                var siteId = SPContext.Current.Site.ID;
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(siteId))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            var admin = "Report Admin";
                            var manage = "Report Manage";
                            if (currentUser.IsSiteAdmin)
                            {
                                msg = "Administrator";
                            }
                            else
                            {
                                var list = web.Lists.TryGetList(CommonHelper.reportManageList);
                                var squery = new SPQuery();
                                squery.Query = "<Where><And>" +
                                              "<Eq><FieldRef Name=\"ObjectName\" LookupId=\"True\" /><Value Type=\"User\">" + currentUser.ID + "</Value></Eq>" +
                                              "<Eq><FieldRef Name=\"Permissions\" /><Value Type=\"MultiChoice\">" + admin + "</Value></Eq>" +
                                              "</And></Where>";
                                var adminUsers = list.GetItems(squery);
                                if (adminUsers.Count > 0)
                                {
                                    msg = "Administrator";
                                }
                                else
                                {
                                    var aquery = new SPQuery();
                                    aquery.Query = "<Where><And>" +
                                                  "<Eq><FieldRef Name=\"ObjectName\" LookupId=\"True\" /><Value Type=\"User\">" + currentUser.ID + "</Value></Eq>" +
                                                  "<Eq><FieldRef Name=\"Permissions\" /><Value Type=\"MultiChoice\">" + manage + "</Value></Eq>" +
                                                  "</And></Where>";
                                    var managerUsers = list.GetItems(aquery);
                                    if (managerUsers.Count > 0)
                                    {
                                        msg = manage;
                                    }
                                }
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("GetNav.aspx__GetUserType", ex.Message);
            }
            return msg;
        }

        /// <summary>
        ///获取列表信息
        /// </summary>
        /// <returns>列表信息</returns>
        [WebMethod]
        public static string GetListInfo()
        {
            string listName = CommonHelper.docListName;
            string listId = string.Empty;
            try
            {
                var siteId = SPContext.Current.Site.ID;
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(siteId))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            SPList list = web.Lists.TryGetList(CommonHelper.docListName);
                            listId = list.ID.ToString();
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("GetNav.aspx__GetListInfo", ex.Message);
            }
            return listName + "," + listId;
        }

        /// <summary>
        /// 获取待我审批的文件数量
        /// </summary>
        /// <returns>待我审批的文件数量</returns>
        [WebMethod]
        public static string GetApprove()
        {
            int count = 0;
            try
            {
                var siteId = SPContext.Current.Site.ID;
                using (SPSite site = new SPSite(siteId))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPList list = web.Lists.TryGetList(CommonHelper.docListName);
                        if (list != null)
                        {
                            // TODO
                            string dString = SPUtility.CreateISO8601DateTimeFromSystemDateTime(DateTime.Now.AddDays(-CommonHelper.PendingRequestDay)); // 转换当前时间

                            SPQuery oQuery = new SPQuery();
                            oQuery.ViewAttributes = "Scope=\"Recursive\"";
                            oQuery.Query = "<Where><And>" +
                                           "<Eq><FieldRef Name=\"_ModerationStatus\"/><Value Type=\"ModStat\">Pending</Value></Eq>" +
                                           "<Geq><FieldRef Name='Modified' /><Value Type='DateTime'>" + dString + "</Value></Geq>" +
                                           "</And></Where>";
                            SPListItemCollection collListItems = list.GetItems(oQuery);

                            if (collListItems != null)
                            {
                                count = collListItems.Count;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("GetNav.aspx__GetApprove", ex.Message);
            }
            return count.ToString();
        }

        /// <summary>
        /// 获取当前用户权限
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static string GetPermission()
        {
            return "0";
        }

        /// <summary>
        /// 根据文件夹id获取文件夹路径
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns>文件夹路径</returns>
        [WebMethod]
        public static string GetPathById(string folderId)
        {
            string path = "";
            try
            {
                if (!string.IsNullOrEmpty(folderId) && folderId != "undefined" && CommonHelper.IsNumeric(folderId))
                {
                    var siteId = SPContext.Current.Site.ID;
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        using (SPSite site = new SPSite(siteId))
                        {
                            using (SPWeb web = site.OpenWeb())
                            {
                                //获取List
                                SPList list = web.Lists.TryGetList(CommonHelper.docListName);
                                var folder = list.GetItemById(Convert.ToInt32(folderId));
                                path = folder.Url;
                                path = CommonHelper.EncodeUrl(path);
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("GetNav.aspx__GetPathById", ex.Message + " FolderId=" + folderId);
            }
            return path;
        }

        /// <summary>
        /// 获取导航内容
        /// </summary>
        /// <returns>导航内容</returns>
        [WebMethod]
        public static string GetNavs()
        {
            string ul = "<li><a href=\"/default.aspx\">HOME</a></li>";
            try
            {
                var siteId = SPContext.Current.Site.ID;

                using (SPSite site = new SPSite(siteId))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        //获取List
                        SPList list = web.Lists.TryGetList(CommonHelper.docListName);
                        var listName = list.Title;
                        var folderList = list.RootFolder.SubFolders;

                        if (folderList != null && folderList.Count > 0)
                        {
                            // 排序导航
                            List<NavLi> sortNavs = new List<NavLi>();
                            // 其他导航
                            List<NavModel> otherNavs = new List<NavModel>();

                            foreach (SPFolder folder in folderList)
                            {
                                if (folder.Name != "Forms" && folder.Name != CommonHelper.Common)
                                {
                                    if (folder.Item["NavOrder"] != null)
                                    {
                                        string folderName = folder.Name;
                                        int endIndex;
                                        bool b = CommonHelper.GetStrIndex(folderName, 25, out endIndex);
                                        if (!b)
                                        {
                                            folderName = folderName.Substring(0, endIndex - 1) + "...";
                                        }
                                        string url1 = CommonHelper.EncodeUrl(folder.ServerRelativeUrl);
                                        var li = "<li><a href=\"/" + listName + "/Forms/AllItems.aspx?RootFolder=" + url1 + "\" title=\"" + folder.Name + "\">" + folderName + "</a>";
                                        if (folder.SubFolders != null && folder.SubFolders.Count > 0)
                                        {
                                            li += "<ul class=\"secondNav\">";
                                            List<NavModel> navs = new List<NavModel>();
                                            foreach (SPFolder subFd in folder.SubFolders)
                                            {
                                                NavModel mo = new NavModel();
                                                mo.Name = subFd.Name;
                                                mo.Order = subFd.Item["Order Index"] == null ? 0 : Convert.ToInt32(subFd.Item["Order Index"]);
                                                mo.ServerRelativeUrl = subFd.ServerRelativeUrl;
                                                navs.Add(mo);
                                            }
                                            navs = navs.OrderBy(T => T.Order).ThenBy(T => T.Name).ToList();
                                            foreach (NavModel subF in navs)
                                            {
                                                string subFName = subF.Name;
                                                int endIndex2;
                                                bool b2 = CommonHelper.GetStrIndex(subFName, 25, out endIndex2);
                                                if (!b2)
                                                {
                                                    subFName = subFName.Substring(0, endIndex2 - 1) + "...";
                                                }
                                                string url2 = CommonHelper.EncodeUrl(subF.ServerRelativeUrl);
                                                li += "<li><a href=\"/" + listName + "/Forms/AllItems.aspx?RootFolder=" + url2 + "\" title=\"" + subF.Name + "\">" + subFName + "</a></li>";
                                            }
                                            li += "</ul>";
                                        }
                                        li += "</li>";

                                        int sm = Convert.ToInt32(folder.Item["NavOrder"]);
                                        sortNavs.Add(new NavLi() { Li = li, NavOrder = sm });
                                    }
                                    else
                                    {
                                        otherNavs.Add(new NavModel { Name = folder.Name, ServerRelativeUrl = folder.ServerRelativeUrl });
                                    }
                                }
                            }
                            if (sortNavs.Count > 0)
                            {
                                sortNavs = sortNavs.OrderBy(t => t.NavOrder).ToList();
                                foreach (var n in sortNavs)
                                {
                                    ul += n.Li;
                                }
                            }

                            if (otherNavs.Count > 0)
                            {
                                otherNavs = otherNavs.OrderBy(T => T.Name).ToList();
                                string otherLi = "<li id=\"other\"><a href=\"#\">Policy</a><ul class=\"secondNav\">";
                                foreach (NavModel folder in otherNavs)
                                {
                                    string folderName = folder.Name;
                                    int endIndex;
                                    bool b = CommonHelper.GetStrIndex(folderName, 25, out endIndex);
                                    if (!b)
                                    {
                                        folderName = folderName.Substring(0, endIndex - 1) + "...";
                                    }
                                    string url3 = CommonHelper.EncodeUrl(folder.ServerRelativeUrl);
                                    var li = "<li class=\"\"><a href=\"/" + listName + "/Forms/AllItems.aspx?RootFolder=" + url3 + "\" title=\"" + folder.Name + "\">" + folderName + "</a></li>";
                                    otherLi += li;
                                }
                                otherLi += "</ul></li>";

                                ul += otherLi;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("GetNav.aspx__GetNavs", ex.Message);
            }
            return ul;
        }

        public class NavLi
        {
            public string Li { get; set; }
            public int NavOrder { get; set; }
        }

        /// <summary>
        /// 获取下载次数/点击次数/综合评分
        /// </summary>
        /// <param name="fIds">fIds</param>
        /// <returns>下载次数/点击次数/综合评分</returns>
        [WebMethod]
        public static string Get3PCount(string fIds)
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            List<DocumentCalculatedModel> models = new List<DocumentCalculatedModel>();
            var fIdList = fIds.Split(',');
            if (fIdList.Length > 0)
            {
                try
                {
                    GetDocumentCalculatedTimer timer = new GetDocumentCalculatedTimer();
                    var dcList = timer.GetDocumentCalculatedList(false);
                    foreach (var fid in fIdList)
                    {
                        var item = dcList.FirstOrDefault(T => T.FID == fid);
                        if (item != null)
                        {
                            models.Add(item);
                        }
                        else
                        {
                            models.Add(new DocumentCalculatedModel { FID = fid, AveScore = "0.0", ClickCount = 0, DownloadCount = 0 });
                        }
                    }
                }
                catch (Exception ex)
                {
                    CommonHelper.SetErrorLog("GetNav.aspx__Get3PCount", ex.Message);
                }
            }
            return jsonSerializer.Serialize(models); ;
        }

    }
}
