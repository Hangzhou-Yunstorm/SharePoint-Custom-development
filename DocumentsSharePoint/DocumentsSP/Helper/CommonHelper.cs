using DocumentsSP.Model;
using Microsoft.SharePoint;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace DocumentsSP.Helper
{
    public static class CommonHelper
    {
        /// <summary>
        /// AD域信息
        /// </summary>
        public static string AdminId = "yc_lt";
        public static string AdminPassword = "Wykl0501";
        public static string LDAPUrl = "LDAP://dahuatech.com/OU=威讯科技,OU=大华技术,DC=dahuatech,DC=com";
        public static string Domain = "dahuatech";

        //  中间页面域名
        public static string MiddleDomain = "http://check.dahuasecurity.com:9090";

        public static string MblDownloadPath = "E:\\MiddleSystem\\MblDocument\\";
        public static string MblDownloadUrl = MiddleDomain + "/MblDocument/";

        /// <summary>
        /// 发送邮件最大人数
        /// </summary>
        public static int mailToMaxCount = 20;
        /// <summary>
        /// 邮件发送人账号（审批/反馈等）
        /// </summary>
        public static string mailUser = "gks@dahuatech.com";
        /// <summary>
        /// 邮件发送人密码
        /// </summary>
        public static string mailPwd = "Voxa5020";
        /// <summary>
        /// 邮箱smtp地址
        /// </summary>
        public static string mailSmtp = "mail.dahuatech.com";

        //public static string mailUser = "457833428@qq.com";
        //public static string mailPwd = "hqiuuklxeailcaej";
        //public static string mailSmtp = "smtp.qq.com";

        /// <summary>
        /// 邮箱smtp端口
        /// </summary>
        public static int mailPort = 25;
        /// <summary>
        /// 批量下载压缩文件路径
        /// </summary>
        public static string ZipFilePath = @"E:\DownLoadFiles\";
        /// <summary>
        /// 页脚文件路径
        /// </summary>
        public static string footerPath = @"E:\Footer\footer.txt";
        /// <summary>
        /// 用户手册路径
        /// </summary>
        public static string userManualPath_cn = "/Documents/Common/User Manual/用户手册.docx";
        public static string userManualPath_en = "/Documents/Common/User Manual/User Manual.docx";

        /// <summary>
        /// 只有管理员可以操作组名
        /// </summary>
        public static string adminGroup1 = "Authorized-External-Group";
        public static string adminGroup2 = "GKS-AllUsers";

        /// <summary>
        /// 海外销售中心-产品部三级部门ID集合
        /// </summary>
        public static string TDGroupIds = ConfigurationManager.AppSettings["ProductSubIds"];

        /// <summary>
        /// 下载大小限制
        /// </summary>
        public static int MaxSize = string.IsNullOrEmpty(ConfigurationManager.AppSettings["DownloadMaxSize"]) ? 2 : Convert.ToInt32(ConfigurationManager.AppSettings["DownloadMaxSize"]);

        /// <summary>
        /// 最新文件期限（-day）
        /// </summary>
        public static int LatestFileDay = string.IsNullOrEmpty(ConfigurationManager.AppSettings["LatestFileDay"]) ? 45 : Convert.ToInt32(ConfigurationManager.AppSettings["LatestFileDay"]);

        /// <summary>
        /// 待审批文件期限（-day）
        /// </summary>
        public static int PendingRequestDay = string.IsNullOrEmpty(ConfigurationManager.AppSettings["PendingRequestDay"]) ? 60 : Convert.ToInt32(ConfigurationManager.AppSettings["PendingRequestDay"]);

        #region 表格名称
        /// <summary>
        /// 评论List
        /// </summary>
        public static string commentListName = "Comments List";
        /// <summary>
        /// 评分List
        /// </summary>
        public static string scoreListName = "Score List";
        /// <summary>
        /// 文档List
        /// </summary>
        public static string docListName = "Documents";
        /// <summary>
        /// 文档统计详情
        /// </summary>
        public static string documentStatisticalDetails = "DocumentStatisticalDetails";
        /// <summary>
        /// 文档计算
        /// </summary>
        public static string documentCalculated = "DocumentCalculated";
        /// <summary>
        /// 文档List
        /// </summary>
        public static string docRMSListName = "RMSDocuments";
        /// <summary>
        /// 日志List
        /// </summary>
        public static string logListName = "SystemLogList";
        /// <summary>
        /// 错误日志List
        /// </summary>
        public static string errorLogList = "ErrorLogList";
        /// <summary>
        /// 轮播图List
        /// </summary>
        public static string carouselListName = "Carousel List";
        /// <summary>
        /// 历史记录List
        /// </summary>
        public static string historyListName = "Browsing History";
        /// <summary>
        /// 下载List
        /// </summary>
        public static string downloadListName = "Download History";
        /// <summary>
        /// 加密任务List
        /// </summary>
        public static string rmsTask = "RMSTask";
        /// <summary>
        /// 加密历史List
        /// </summary>
        public static string rmsHistory = "RMSHistory";
        /// <summary>
        /// 外部日志List
        /// </summary>
        public static string externalLogs = "ExternalLogs";
        /// <summary>
        /// 白名单List
        /// </summary>
        public static string whiteList = "WhiteList";
        /// <summary>
        /// 报表人员List
        /// </summary>
        public static string reportManageList = "ReportManageList";
        /// <summary>
        /// 订阅目录List
        /// </summary>
        public static string sDirectoryList = "SubscribeDirectoryList";
        /// <summary>
        /// 订阅消息List
        /// </summary>
        public static string subscribeList = "SubscribeList";
        /// <summary>
        /// 外部人员List
        /// </summary>
        public static string externalUserList = "ExternalUserList";
        /// <summary>
        /// 分享目录List
        /// </summary>
        public static string shareCatalogList = "ShareCatalogList";
        /// <summary>
        /// 邮件推送List
        /// </summary>
        public static string userEmaiPushList = "UserEmaiPushList";
        /// <summary>
        /// Common
        /// </summary>
        public static string Common = "Common";
        /// <summary>
        /// CommonFile Path
        /// </summary>
        public static string CommonPath = docListName + "/" + Common;
        /// <summary>
        /// Region Path
        /// </summary>
        public static string RegionPath = docListName + "/Technical Academy/Region Share";
        #endregion

        /// <summary>
        /// 获取页脚内容
        /// </summary>
        /// <returns></returns>
        public static string ReadFooter()
        {
            StreamReader sr = new StreamReader(footerPath, Encoding.Default);
            string line = string.Empty;
            string footer = string.Empty;
            while ((line = sr.ReadLine()) != null)
            {
                footer += line;
            }
            return footer;
        }

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static string GetPropertyValueString(DirectoryEntry entry, string propertyName)
        {
            if (entry == null || !entry.Properties.Contains(propertyName)) return "";
            var collection = entry.Properties[propertyName];
            if (collection == null || collection.Count <= 0) return "";
            if (collection[0] == null) return "";
            return collection[0].ToString();
        }

        /// <summary>
        /// 删除文件夹下过去6H的zip文件
        /// </summary>
        /// <param name="folder">文件夹路径</param>
        public static void DeleteZipFile(string folder)
        {
            try
            {
                DirectoryInfo directory = new DirectoryInfo(folder);
                if (directory.Exists)
                {
                    FileInfo[] zipFiles = directory.GetFiles();

                    if (zipFiles != null && zipFiles.Length > 0)
                    {
                        for (int m = 0; m < zipFiles.Length; m++)
                        {
                            FileInfo zFile = zipFiles[m];
                            if (zFile.Exists)
                            {
                                string ext = Path.GetExtension(zFile.Name).ToLower();

                                TimeSpan ts = new TimeSpan(0, 6, 0, 0);
                                if (DateTime.Now.ToLocalTime() - zFile.LastWriteTime > ts && ext == ".zip")
                                {
                                    zFile.Delete();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetErrorLog("CommonHelper.cs__DeleteZipFile", ex.Message);
            }
        }

        /// <summary>
        /// 删除文件夹下过去3H的文件夹
        /// </summary>
        /// <param name="folder">文件夹路径</param>
        public static void DeleteSubFolders(string folder)
        {
            try
            {
                DirectoryInfo directory = new DirectoryInfo(folder);
                if (directory.Exists)
                {
                    DirectoryInfo[] folders = directory.GetDirectories();

                    if (folders != null && folders.Length > 0)
                    {
                        for (int m = 0; m < folders.Length; m++)
                        {
                            DirectoryInfo subFolder = folders[m];
                            if (subFolder.Exists)
                            {
                                TimeSpan ts = new TimeSpan(0, 3, 0, 0);
                                if (DateTime.Now.ToLocalTime() - subFolder.LastWriteTime > ts)
                                {
                                    subFolder.Delete(true);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetErrorLog("CommonHelper.cs__DeleteFolderFile", ex.Message);
            }
        }

        /// <summary>
        /// 判断文件是否是图片
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>是否是图片</returns>
        public static bool IsPicture(string fileName)
        {
            string strFilter = ".jpeg|.gif|.jpg|.png|.bmp|.pic|.tiff|.ico|.iff|.lbm|.mag|.mac|.mpt|.opt|";
            string[] tempFileds = strFilter.Trim().Split('|');
            var fileType = fileName.Substring(fileName.LastIndexOf("."), fileName.Length - fileName.LastIndexOf(".")).ToLower();

            foreach (string str in tempFileds)
            {
                if (str.Equals(fileType))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>  
        /// 判断是否是数字  
        /// </summary>  
        /// <param name="str">字符串</param>  
        /// <returns>bool</returns>  
        public static bool IsNumeric(string str)
        {
            if (str == null || str.Length == 0)
                return false;
            ASCIIEncoding ascii = new ASCIIEncoding();
            byte[] bytestr = ascii.GetBytes(str);
            foreach (byte c in bytestr)
            {
                if (c < 48 || c > 57)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 判断文件是否加密
        /// </summary>
        /// <param name="item">SPListItem</param>
        /// <returns>是否加密</returns>
        public static bool IsRMS(SPListItem item)
        {
            try
            {
                string ext = Path.GetExtension(item.Name).ToLower();
                if (ext == ".docx" || ext == ".pptx" || ext == ".xlsx")
                {
                    var fc = Convert.ToBoolean(item["IsFullControl"]);
                    var or = Convert.ToBoolean(item["IsRead"]);
                    var ass = Convert.ToBoolean(item["IsPrint"]);
                    var ap = Convert.ToBoolean(item["IsSave"]);
                    var ae = Convert.ToBoolean(item["IsEdit"]);
                    //判断是否加密
                    if (fc || or || ass || ap || ae)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                SetErrorLog("SetFileDown__IsRMS", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 根据文件名/uId获取文件详情页路径
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="uId">uId</param>
        /// <returns>文件详情页路径</returns>
        public static SPFieldUrlValue GetFileUrl(string fileName, string fId)
        {
            SPFieldUrlValue value = new SPFieldUrlValue();
            value.Description = fileName;
            value.Url = "/_layouts/15/FileDetail.aspx?FID=" + fId;
            return value;
        }

        /// <summary>
        /// 根据图片文件名获取图片路径
        /// </summary>
        /// <param name="imageName">图片文件名</param>
        /// <returns>图片路径</returns>
        public static SPFieldUrlValue GetImage(string imageName)
        {
            SPFieldUrlValue value = new SPFieldUrlValue();
            //value.Description = "View";
            value.Url = "/_layouts/15/IMAGES/" + imageName;

            return value;
        }

        /// <summary>
        /// 根据文件的SPListItem获取所在文件夹路径
        /// </summary>
        /// <param name="fId">文件的FID</param>
        /// <returns>文件夹路径</returns>
        public static SPFieldUrlValue GetParentFolder(string fId)
        {
            SPFieldUrlValue value = new SPFieldUrlValue();
            value.Description = "Folder";
            value.Url = "/_layouts/15/ReturnFolder.aspx?FID=" + fId;
            return value;
        }

        /// <summary>
        /// 根据文件夹属性获取文件夹路径
        /// </summary>
        /// <param name="fId">文件夹的FID</param>
        /// <returns>文件夹路径</returns>
        public static SPFieldUrlValue GetFolderUrl(string fId)
        {
            SPFieldUrlValue value = new SPFieldUrlValue();
            value.Description = "Folder";
            value.Url = "/_layouts/15/ReturnFolder.aspx?FolderFID=" + fId;
            return value;
        }

        /// <summary>
        /// 设置下载列表
        /// </summary>
        /// <param name="item">item</param>
        private static void AddDownLoadList(SPListItem item)
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
                            var downloadList = web.Lists.TryGetList(downloadListName);

                            var fId = item["FID"].ToString();
                            var listItem = downloadList.AddItem();
                            listItem["FileName"] = GetFileUrl(item.Name, item["FID"].ToString());
                            listItem["Time"] = DateTime.Now.ToLocalTime();
                            listItem["UName"] = user;
                            listItem["Creator"] = item.File.ModifiedBy;
                            listItem["CreateTime"] = item["Modified"];
                            listItem["Folder"] = GetParentFolder(fId);
                            listItem["FID"] = fId;
                            listItem["IconUrl"] = GetImage(item.File.IconUrl);
                            listItem["Department"] = GetSubGroupName(user);
                            listItem["DepartmentId"] = GetSubGroupId(user);
                            listItem.Update();
                        }
                        catch (Exception e)
                        {
                            SetErrorLog("SetFileDown__AddDownLoadList", e.Message);
                        }
                        try
                        {
                            var dsd = web.Lists.TryGetList(documentStatisticalDetails);
                            var dquery = new SPQuery();
                            dquery.Query = "<Where><And><Eq><FieldRef Name='FID' /><Value Type='Text'>" + item["FID"].ToString() + "</Value></Eq>" +
                                "<Eq><FieldRef Name='Title' /><Value Type='Text'>" + GetServerHostName() + "</Value></Eq></And></Where>";
                            var ditems = dsd.GetItems(dquery);
                            if (ditems != null && ditems.Count > 0)
                            {
                                var ditem = ditems[0];
                                ditem["DownloadCount"] = Convert.ToInt32(ditem["DownloadCount"]) + 1;
                                ditem.Update();
                            }
                            else
                            {
                                var ditem = dsd.AddItem();
                                ditem["FID"] = item["FID"];
                                ditem["Title"] = GetServerHostName();
                                ditem["DownloadCount"] = 1;
                                ditem.Update();
                            }
                        }
                        catch (Exception ex)
                        {
                            SetErrorLog("SetFileDown__AddDownLoadList_AddCount", ex.Message);
                        }
                        web.AllowUnsafeUpdates = false;
                    }
                }
            });
        }

        /// <summary>
        /// 文件下载记录/更新下载次数/写入日志
        /// </summary>
        /// <param name="id">文件id</param>
        public static void SetFileDown(int fileId)
        {
            try
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
                            SPList spList = web.Lists.TryGetList(docListName);
                            var item = spList.GetItemById(fileId);

                            if (item["FID"] == null)
                            {
                                var fId = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
                                item["FID"] = fId;
                                item["ParentFolder"] = GetParentFolder(fId);
                                item.SystemUpdate(false);
                            }

                            // 添加下载次数
                            AddDownLoadList(item);

                            // 下载记录
                            var logList = web.Lists.TryGetList(logListName);
                            var listItem = logList.AddItem();
                            listItem["Title"] = "Download file：" + item.Name;
                            listItem["Operate"] = "Download";
                            listItem["Operater"] = user;
                            listItem["Operator"] = user.Name;
                            listItem["OperatorId"] = user.LoginName;
                            listItem["ServerIP"] = CommonHelper.GetServerHostName();
                            listItem["Department"] = CommonHelper.GetSubGroupName(user);
                            listItem["DepartmentId"] = CommonHelper.GetSubGroupId(user);
                            listItem["ObjectName"] = item.Url;
                            listItem["ObjectType"] = "File";
                            listItem["FID"] = item["FID"];
                            listItem.Update();

                            web.AllowUnsafeUpdates = false;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                SetErrorLog("CommonHelper__SetFileDown", ex.Message);
            }
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="title">日志标题</param>
        /// <param name="msg">错误详情</param>
        public static void SetErrorLog(string title, string msg)
        {
            try
            {
                var siteId = SPContext.Current.Site.ID;
                var cUser = SPContext.Current.Web.CurrentUser;
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(siteId))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            var logList = web.Lists.TryGetList(errorLogList);
                            if (logList != null)
                            {
                                web.AllowUnsafeUpdates = true;
                                var listItem = logList.AddItem();
                                listItem["Title"] = title;
                                listItem["Operator"] = cUser;
                                listItem["Message"] = msg;
                                listItem.Update();
                                web.AllowUnsafeUpdates = false;
                            }
                        }
                    }
                });
            }
            catch { }
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="title">日志标题</param>
        /// <param name="msg">错误详情</param>
        public static void SetErrorLog(string title, string msg, Guid siteId, SPUser cUser)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(siteId))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            var logList = web.Lists.TryGetList(errorLogList);
                            if (logList != null)
                            {
                                web.AllowUnsafeUpdates = true;
                                var listItem = logList.AddItem();
                                listItem["Title"] = title;
                                listItem["Operator"] = cUser;
                                listItem["Message"] = msg;
                                listItem.Update();
                                web.AllowUnsafeUpdates = false;
                            }
                        }
                    }
                });
            }
            catch { }
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="web">SPWeb对象</param>
        /// <param name="model">日志详情</param>
        public static void SetLog(LogModel model, Guid siteId)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite site = new SPSite(siteId))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        try
                        {
                            var logList = web.Lists.TryGetList(logListName);
                            if (logList != null)
                            {
                                web.AllowUnsafeUpdates = true;
                                var listItem = logList.AddItem();
                                listItem["Title"] = model.Title;
                                listItem["Operate"] = model.Operate;
                                listItem["Operater"] = model.Operater;
                                listItem["Operator"] = model.Operator;
                                listItem["OperatorId"] = model.OperatorId;
                                listItem["ServerIP"] = model.ServerIP;
                                listItem["Department"] = model.Department;
                                listItem["DepartmentId"] = model.DepartmentId;
                                listItem["ObjectName"] = model.ObjectName;
                                listItem["ObjectType"] = model.ObjectType;
                                listItem.Update();
                                web.AllowUnsafeUpdates = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            SetErrorLog("CommonHelper__SetLog", ex.Message);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 获取字符串是否具有指定长度（58）
        /// </summary>
        /// <param name="strs">字符串</param>
        /// <param name="endIndex">截取长度</param>
        /// <returns>是否需要截取</returns>
        public static bool GetStrIndex(string strs, out int endIndex)
        {
            endIndex = 0;
            int index = 0;
            try
            {
                var rex = @"[\u4e00-\u9fa5]";
                for (int m = 0; m < strs.Length; m++)
                {
                    string str = strs[m].ToString();
                    Match mInfo = Regex.Match(str, rex);
                    if (mInfo.Success)
                    {
                        index = index + 2;
                    }
                    else
                    {
                        index++;
                    }
                    if (index >= 50)
                    {
                        endIndex = m;
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                SetErrorLog("CommonHelper__GetStrIndex", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 获取字符串是否具有指定长度
        /// </summary>
        /// <param name="strs">字符串</param>
        /// <param name="length">指定字符长度</param>
        /// <param name="endIndex">截取长度</param>
        /// <returns>是否需要截取</returns>
        public static bool GetStrIndex(string strs, int length, out int endIndex)
        {
            endIndex = 0;
            int index = 0;
            try
            {
                var rex = @"[\u4e00-\u9fa5]";
                for (int m = 0; m < strs.Length; m++)
                {
                    string str = strs[m].ToString();
                    Match mInfo = Regex.Match(str, rex);
                    if (mInfo.Success)
                    {
                        index = index + 2;
                    }
                    else
                    {
                        index++;
                    }
                    if (index >= length)
                    {
                        endIndex = m;
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                SetErrorLog("CommonHelper__GetStrIndex", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Object转bool
        /// </summary>
        /// <param name="o">Object转bool</param>
        /// <returns>bool</returns>
        public static bool ObjToBool(object o)
        {
            if (o == null)
            {
                return false;
            }
            else
            {
                if (o.ToString().ToLower() == "false")
                {
                    return false;
                }
                else if (o.ToString().ToLower() == "true")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// URL编码
        /// </summary>
        /// <param name="url">URL字符串</param>
        /// <returns>编码后的URL字符串<</returns>
        public static string EncodeUrl(string url)
        {
            //url = System.Web.HttpUtility.UrlEncode(url, Encoding.UTF8);
            url = Microsoft.JScript.GlobalObject.encodeURIComponent(url);
            return url;
        }

        /// <summary>
        /// URL解码
        /// <param name="url">URL字符串</param>
        /// <returns>解码后的URL字符串<</returns>
        public static string DecodeUrl(string url)
        {
            //url = System.Web.HttpUtility.UrlDecode(url, Encoding.UTF8);
            url = Microsoft.JScript.GlobalObject.decodeURIComponent(url);
            return url;
        }

        /// <summary>
        /// 判断字符串是否是GUID
        /// </summary>
        /// <param name="uid">字符串</param>
        /// <returns>是否是GUID</returns>
        public static bool isGuid(string uid)
        {
            Guid g = Guid.Empty;
            return Guid.TryParse(uid, out g);
        }

        /// <summary>
        /// 用户是否有文件夹的添加权限
        /// </summary>
        /// <param name="rootFolder">文件夹</param>
        /// <param name="siteId">siteId</param>
        /// <param name="cUser">用户</param>
        /// <returns></returns>
        public static void IsFolderHadPermissons(string rootFolder, Guid siteId, SPUser cUser, ref bool isPermissionAdd, ref bool isPermissionExport)
        {
            try
            {
                if (cUser.IsSiteAdmin)
                {
                    isPermissionAdd = true;
                    isPermissionExport = true;
                }
                else
                {
                    bool isAdd = false;
                    bool isExport = false;
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        using (SPSite site = new SPSite(siteId))
                        {
                            using (SPWeb web = site.OpenWeb())
                            {
                                SPFolder folder = null;
                                if (string.IsNullOrEmpty(rootFolder) || rootFolder == "/" + CommonHelper.docListName || rootFolder == "/" + CommonHelper.docListName + "/")
                                {
                                    folder = web.Lists.TryGetList(docListName).RootFolder;
                                }
                                else
                                {
                                    folder = web.GetFolder(rootFolder);
                                }

                                if (folder != null && folder.Exists && folder.Item != null)
                                {
                                    try
                                    {
                                        SPRoleAssignmentCollection roles = folder.Item.RoleAssignments;
                                        SPRoleAssignment roleAssignment = roles.GetAssignmentByPrincipal(cUser);
                                        SPRoleDefinitionBindingCollection defColl = roleAssignment.RoleDefinitionBindings;
                                        foreach (SPRoleDefinition roleDef in defColl)
                                        {
                                            string spr = roleDef.BasePermissions.ToString();
                                            if (spr.Contains("AddListItems") || "FullMask".Equals(spr))
                                            {
                                                isAdd = true;
                                            }
                                            if (spr.Contains("ManageLists") || "FullMask".Equals(spr))
                                            {
                                                isExport = true;
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        try
                                        {
                                            SPPermissionInfo permissionInfo = folder.Item.GetUserEffectivePermissionInfo(cUser.LoginName);
                                            var roleAssignments = permissionInfo.RoleAssignments;
                                            foreach (SPRoleAssignment roleAssignment in roleAssignments)
                                            {
                                                SPRoleDefinitionBindingCollection roleDefColl = roleAssignment.RoleDefinitionBindings;
                                                foreach (SPRoleDefinition roleDef in roleDefColl)
                                                {
                                                    string spr = roleDef.BasePermissions.ToString();
                                                    if (spr.Contains("AddListItems") || "FullMask".Equals(spr))
                                                    {
                                                        isAdd = true;
                                                    }
                                                    if (spr.Contains("ManageLists") || "FullMask".Equals(spr))
                                                    {
                                                        isExport = true;
                                                    }
                                                }
                                            }
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                    });
                    isPermissionAdd = isAdd;
                    isPermissionExport = isExport;
                }
            }
            catch (Exception ex)
            {
                SetErrorLog("CommonHelper__IsFolderHadAddPermisson", ex.Message);
            }
        }

        /// <summary>
        /// 获取用户所在二级部门
        /// </summary>
        /// <param name="user">用户</param>
        /// <returns>二级部门名称</returns>
        public static string GetSubGroupName(SPUser user)
        {
            string sub = "";
            var groups = user.Groups;
            foreach (SPGroup g in groups)
            {
                if (g.Name != CommonHelper.adminGroup2)
                {
                    if (g.Description.StartsWith("Overseas_")) // 同步组的说明都是以‘Overseas_’开头
                    {
                        //sub = g.Name;
                        //break;

                        if (string.IsNullOrEmpty(sub))
                        {
                            sub = g.Name;
                        }
                        else
                        {
                            sub += "," + g.Name;
                        }
                    }
                }
            }
            return sub;
        }

        /// <summary>
        /// 获取用户所在二级部门
        /// </summary>
        /// <param name="user">用户</param>
        /// <returns>二级部门名称</returns>
        public static string GetSubGroupId(SPUser user)
        {
            string subId = "";
            var groups = user.Groups;
            foreach (SPGroup g in groups)
            {
                if (g.Name != CommonHelper.adminGroup2)
                {
                    if (g.Description.StartsWith("Overseas_")) // 同步组的说明都是以‘Overseas_’开头
                    {
                        //subId = g.Description.Replace("Overseas_", "");
                        //break;

                        if (string.IsNullOrEmpty(subId))
                        {
                            subId = g.Description.Replace("Overseas_", "");
                        }
                        else
                        {
                            subId += "," + g.Description.Replace("Overseas_", "");
                        }
                    }
                }
            }
            return subId;
        }

        /// <summary>
        /// 获取服务器机器名
        /// </summary>
        /// <returns></returns>
        public static string GetServerHostName()
        {
            string name = "";
            try
            {
                name = Dns.GetHostName();
                #region IpV4
                //IPAddress[] ipadrlist = Dns.GetHostAddresses(name);
                //foreach (IPAddress ipa in ipadrlist)
                //{
                //    if (ipa.AddressFamily == AddressFamily.InterNetwork)
                //        ip = ipa.ToString();
                //}
                #endregion
            }
            catch (Exception ex)
            {
                SetErrorLog("CommonHelper__GetServerHostName", ex.Message);
            }
            return name;
        }

        /// <summary>
        /// 获取国家名
        /// </summary>
        /// <returns>国家名</returns>
        public static string GetCountryName()
        {
            string country = string.Empty;
            string hostName = GetServerHostName();

            var host = hostName.ToUpper();
            switch (host)
            {
                case "GKS-SP05":
                    country = "SoutheastAsia";
                    break;
                case "GKS-SP06":
                    country = "WesternEurope";
                    break;
                case "GKS-SP03":
                    country = "USA";
                    break;
                case "GKS-SP04":
                    country = "Brazil";
                    break;
                default:
                    country = "Ikernal";
                    break;
            }

            return country;
        }

        /// <summary>
        /// 获取国家名
        /// </summary>
        /// <returns>国家名</returns>
        public static string GetOthersCountryLi()
        {
            string li = string.Empty;
            string hostName = GetServerHostName();

            var host = hostName.ToUpper();
            switch (host)
            {
                case "GKS-SP05":
                    li = "<li><a href='https://euapprove.dahuasecurity.com:4431/Documents/Forms/FileApprove.aspx?HostName=WesternEurope' target='_blank' title='WesternEurope'>WesternEurope</a></li>" +
                         "<li><a href='https://usapprove.dahuasecurity.com:4431/Documents/Forms/FileApprove.aspx?HostName=USA' target='_blank' title='USA'>USA</a></li>" +
                         "<li><a href='https://baapprove.dahuasecurity.com:4431/Documents/Forms/FileApprove.aspx?HostName=Brazil' target='_blank' title='Brazil'>Brazil</a></li>";

                    break;
                case "GKS-SP06":
                    li = "<li><a href='https://sgapprove.dahuasecurity.com:4431/Documents/Forms/FileApprove.aspx?HostName=SoutheastAsia' target='_blank' title='SoutheastAsia'>SoutheastAsia</a></li>" +
                         "<li><a href='https://usapprove.dahuasecurity.com:4431/Documents/Forms/FileApprove.aspx?HostName=USA' target='_blank' title='USA'>USA</a></li>" +
                         "<li><a href='https://baapprove.dahuasecurity.com:4431/Documents/Forms/FileApprove.aspx?HostName=Brazil' target='_blank' title='Brazil'>Brazil</a></li>";

                    break;
                case "GKS-SP03":
                    li = "<li><a href='https://sgapprove.dahuasecurity.com:4431/Documents/Forms/FileApprove.aspx?HostName=SoutheastAsia' target='_blank' title='SoutheastAsia'>SoutheastAsia</a></li>" +
                         "<li><a href='https://euapprove.dahuasecurity.com:4431/Documents/Forms/FileApprove.aspx?HostName=WesternEurope' target='_blank' title='WesternEurope'>WesternEurope</a></li>" +
                         "<li><a href='https://baapprove.dahuasecurity.com:4431/Documents/Forms/FileApprove.aspx?HostName=Brazil' target='_blank' title='Brazil'>Brazil</a></li>";

                    break;
                case "GKS-SP04":
                    li = "<li><a href='https://sgapprove.dahuasecurity.com:4431/Documents/Forms/FileApprove.aspx?HostName=SoutheastAsia' target='_blank' title='SoutheastAsia'>SoutheastAsia</a></li>" +
                         "<li><a href='https://euapprove.dahuasecurity.com:4431/Documents/Forms/FileApprove.aspx?HostName=WesternEurope' target='_blank' title='WesternEurope'>WesternEurope</a></li>" +
                         "<li><a href='https://usapprove.dahuasecurity.com:4431/Documents/Forms/FileApprove.aspx?HostName=USA' target='_blank' title='USA'>USA</a></li>";

                    break;
                default:
                    li = "<li><a href='https://sgapprove.dahuasecurity.com:4431/Documents/Forms/FileApprove.aspx?HostName=SoutheastAsia' target='_blank' title='SoutheastAsia'>SoutheastAsia</a></li>" +
                         "<li><a href='https://euapprove.dahuasecurity.com:4431/Documents/Forms/FileApprove.aspx?HostName=WesternEurope' target='_blank' title='WesternEurope'>WesternEurope</a></li>" +
                         "<li><a href='https://usapprove.dahuasecurity.com:4431/Documents/Forms/FileApprove.aspx?HostName=USA' target='_blank' title='USA'>USA</a></li>" +
                         "<li><a href='https://baapprove.dahuasecurity.com:4431/Documents/Forms/FileApprove.aspx?HostName=Brazil' target='_blank' title='Brazil'>Brazil</a></li>";

                    break;
            }

            return li;
        }

        /// <summary>
        /// 是否是手机端
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public static bool IsMobile(string u)
        {
            Regex b = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if ((b.IsMatch(u) || v.IsMatch(u.Substring(0, 4))))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 通过文件名提取文件格式，并判断是否需要加密
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>是否需要加密</returns>
        public static bool IsRMSFile(string fileName)
        {
            try
            {
                string ext = Path.GetExtension(fileName).ToLower();
                if (ext == ".docx" || ext == ".pptx" || ext == ".xlsx")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 根据长度计算文件大小
        /// </summary>
        /// <param name="filebyte">长度</param>
        /// <returns></returns>
        public static string GetSize(long filebyte)
        {
            string size = string.Empty;

            if (filebyte == 0)
            {
                size = "0";
            }
            else if (filebyte < 1024)
            {
                size = "1KB";
            }
            else if (filebyte < 1024 * 1024)
            {
                //size = ((float)filebyte / 1024).ToString("f2") + "KB";
                size = Math.Round((float)filebyte / 1024, 2) + "KB";
            }
            else
            {
                //size = ((float)filebyte / 1024 / 1024).ToString("f2") + "MB";
                size = Math.Round((float)filebyte / 1024 / 1024, 2) + "MB";
            }
            return size;
        }

        /// <summary>
        /// 获取Region文件
        /// </summary>
        /// <param name="folder">文件夹</param>
        /// <param name="length">长度</param>
        /// <returns>Region文件</returns>
        public static List<SPFile> GetRegionFilesByFolder(SPFolder folder, int length)
        {
            List<SPFile> files = new List<SPFile>();

            try
            {
                // 递归获取
                GetRegionFiles(files, folder);

                // 倒序截取
                files = files.OrderByDescending(T => T.TimeLastModified).Take(length).ToList();

            }
            catch (Exception ex)
            {
                SetErrorLog("CommonHelper__GetRegionFilesByFolder", ex.Message);
            }
            return files;
        }

        /// <summary>
        /// 递归获取文件
        /// </summary>
        /// <param name="files">集合</param>
        /// <param name="folder">文件夹</param>
        private static void GetRegionFiles(List<SPFile> files, SPFolder folder)
        {
            if (folder.Files != null && folder.Files.Count > 0)
            {
                foreach (SPFile file in folder.Files)
                {
                    files.Add(file);
                }
            }
            if (folder.SubFolders != null && folder.SubFolders.Count > 0)
            {
                foreach (SPFolder sFolder in folder.SubFolders)
                {
                    GetRegionFiles(files, sFolder);
                }
            }
        }

    }
}
