using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.Services;
using DocumentsSP.Helper;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Utilities;

namespace DocumentsSP.Layouts
{
    public partial class WebPartContent : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 获取轮播图
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static string GetCarousels()
        {
            string json = "";
            try
            {
                using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        var query = new SPQuery();
                        query.Query = "<OrderBy><FieldRef Name=\"ItemIndex\"></FieldRef></OrderBy>";
                        var list = web.Lists.TryGetList(CommonHelper.carouselListName).GetItems(query);

                        foreach (SPListItem item in list)
                        {
                            var redirectUrl = item["RedirectUrl"];
                            if (redirectUrl == null || string.IsNullOrEmpty(redirectUrl.ToString()))
                            {
                                json += "<li><a><img src=\"" + item.File.ServerRelativeUrl + "\" /></a></li>";
                            }
                            else
                            {
                                var url = redirectUrl.ToString();
                                if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                                {
                                    url = "https://" + url;
                                }
                                json += "<li><a target=\"_black\" href=\"" + url + "\"><img src=\"" + item.File.ServerRelativeUrl + "\" /></a></li>";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("WebPartContent.ascx__GetCarousels", ex.Message);
            }
            return json;
        }

        /// <summary>
        /// 获取订阅记录
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static string GetSubscribeList()
        {
            string json = "";
            try
            {
                var user = SPContext.Current.Web.CurrentUser;
                using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        #region 获取订阅记录
                        var subscribeList = web.Lists.TryGetList(CommonHelper.subscribeList);
                        var squery = new SPQuery();
                        squery.Query = "<OrderBy><FieldRef Name=\"Time\" Ascending=\"False\"></FieldRef></OrderBy>" +
                                       "<Where><Eq><FieldRef Name=\"UName\" LookupId=\"True\" /><Value Type=\"User\">" + user.ID + "</Value></Eq></Where>";
                        squery.RowLimit = 20;

                        var sList = subscribeList.GetItems(squery);
                        foreach (SPListItem item in sList)
                        {
                            SPFieldUrlValue name = new SPFieldUrlValue(item["FileName"].ToString());
                            SPFieldUrlValue icon = new SPFieldUrlValue(item["IconUrl"].ToString());
                            SPFieldUrlValue folder = new SPFieldUrlValue(item["Folder"].ToString());

                            string itemName = name.Description;
                            string displayName = itemName;
                            int endIndex;
                            bool b = CommonHelper.GetStrIndex(displayName, 40, out endIndex);
                            if (!b)
                            {
                                displayName = displayName.Substring(0, endIndex - 1) + "...";
                            }

                            json += "<div style=\"margin:5px;\">" +
                                    "<a class=\"a_folder\" href=\"" + folder.Url + "\" target=\"_blank\">Folder</a>" +
                                    "<img class=\"image\" src=\"" + icon.Url + "\">" +
                                    "<a href=\"" + name.Url + "\" target=\"_blank\" title=\"" + itemName + "\">" + displayName + "</a></div>";

                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("WebPartContent.ascx__GetSubscribeList", ex.Message);
            }
            return json;
        }

        /// <summary>
        /// 获取订阅信息条数
        /// </summary>
        /// <returns>订阅信息条数</returns>
        [WebMethod]
        public static string GetSubscribeCount()
        {
            int count = 0;
            try
            {
                var user = SPContext.Current.Web.CurrentUser;
                using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        var subscribeList = web.Lists.TryGetList(CommonHelper.subscribeList);
                        var squery = new SPQuery();
                        squery.Query = "<Where><Eq><FieldRef Name=\"UName\" LookupId=\"True\" /><Value Type=\"User\">" + user.ID + "</Value></Eq></Where>";
                        var sList = subscribeList.GetItems(squery);
                        if (sList != null)
                        {
                            count = sList.Count;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("WebPartContent.aspx__GetSubscribeCount", ex.Message);
            }

            return count.ToString();
        }


        /// <summary>
        /// 获取浏览历史
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static string GetHistoryList()
        {
            string json = "";
            try
            {
                var user = SPContext.Current.Web.CurrentUser;
                using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        var historyList = web.Lists.TryGetList(CommonHelper.historyListName);
                        var query = new SPQuery();
                        query.Query = "<OrderBy><FieldRef Name=\"Time\" Ascending=\"False\"></FieldRef></OrderBy>" +
                            "<Where><Eq><FieldRef Name=\"UName\" LookupId=\"True\" /><Value Type=\"User\">" + user.ID + "</Value></Eq></Where>";
                        query.RowLimit = 20;

                        var hList = historyList.GetItems(query);

                        foreach (SPListItem item in hList)
                        {
                            SPFieldUrlValue name = new SPFieldUrlValue(item["FileName"].ToString());
                            SPFieldUrlValue icon = new SPFieldUrlValue(item["IconUrl"].ToString());
                            SPFieldUrlValue folder = new SPFieldUrlValue(item["Folder"].ToString());

                            string itemName = name.Description;
                            string displayName = itemName;
                            int endIndex;
                            bool b = CommonHelper.GetStrIndex(displayName, 40, out endIndex);
                            if (!b)
                            {
                                displayName = displayName.Substring(0, endIndex - 1) + "...";
                            }
                            //string url = Helper.CommonHelper.EncodeUrl(folder.Url);
                            json += "<div style=\"margin:5px;\">" +
                                    "<a class=\"a_folder\" href=\"" + folder.Url + "\" target=\"_blank\">Folder</a>" +
                                    "<img class=\"image\" src=\"" + icon.Url + "\">" +
                                    "<a href=\"" + name.Url + "\" target=\"_blank\" title=\"" + itemName + "\">" + displayName + "</a></div>";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("WebPartContent.ascx__GetHistoryList", ex.Message);
            }
            return json;
        }

        /// <summary>
        /// 获取公共文件列表
        /// </summary>
        [WebMethod]
        public static string GetCommonFileJson()
        {
            string json = "";
            try
            {
                using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        var folder = web.GetFolder(CommonHelper.CommonPath);
                        //var fList = web.Lists.TryGetList(CommonHelper.docListName);

                        if (folder == null || !folder.Exists)
                        {
                            return json;
                        }
                        //var query = new SPQuery();
                        //query.ViewAttributes = "Scope=\"Recursive\"";
                        //query.Query = "<OrderBy><FieldRef Name=\"Modified\" Ascending=\"False\"></FieldRef></OrderBy>";
                        //query.RowLimit = 20;
                        //query.Folder = folder;
                        //var items = fList.GetItems(query);

                        var files = folder.Files;

                        foreach (SPFile file in files)
                        {
                            var time = Convert.ToDateTime(file.Item["Modified"]).ToString("yyyy-MM-dd HH:mm:ss");

                            string itemName = file.Name;
                            string displayName = itemName;
                            int endIndex;
                            bool b = CommonHelper.GetStrIndex(displayName, out endIndex);
                            if (!b)
                            {
                                displayName = displayName.Substring(0, endIndex - 1) + "...";
                            }
                            string url = CommonHelper.EncodeUrl(file.ParentFolder.Url);
                            string fileUrl = CommonHelper.EncodeUrl(file.ServerRelativeUrl);
                            json += "<div style=\"margin:5px;\">" +
                                    "<a class=\"a_folder\" href=\"/" + url + "\" target=\"_blank\">Folder</a>" +
                                    "<img class=\"image\" src=\"/_layouts/15/IMAGES/" + file.IconUrl + "\">" +
                                    "<a href=\"/_layouts/15/FileDetail.aspx?Url=" + fileUrl + "\" target=\"_blank\" title=\"" + itemName + "\">" + displayName + "</a>" +
                                    "<span style=\"float:right;margin-right:20px;\">" + time + "</span></div>";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("WebPartContent.aspx__GetCommonFileJson", ex.Message);
            }
            return json;
        }

        /// <summary>
        /// 获取下载最多文件列表
        /// </summary>
        [WebMethod]
        public static string GetMostDownLoadFileJson()
        {
            string json = "";
            try
            {
                List<string> fIdList = new List<string>();

                GetDocumentCalculatedTimer timer = new GetDocumentCalculatedTimer();
                var dcList = timer.GetMostDownloadList(false);
                dcList = dcList.Take(20).ToList();

                foreach (var mo in dcList)
                {
                    string itemName = mo.Name;
                    string displayName = itemName;
                    int endIndex;
                    bool b = CommonHelper.GetStrIndex(displayName, out endIndex);
                    if (!b)
                    {
                        displayName = displayName.Substring(0, endIndex - 1) + "...";
                    }
                    json += "<div style=\"margin:5px;\">" +
                            "<a class=\"a_folder\" href=\"/" + mo.ParentFolder + "\" target=\"_blank\">Folder</a>" +
                            "<img class=\"image\" src=\"/_layouts/15/IMAGES/" + mo.Icon + "\">" +
                            "<a href=\"/_layouts/15/FileDetail.aspx?Url=" + mo.FileUrl + "\" target=\"_blank\" title=\"" + itemName + "\">" + displayName + "</a>" +
                            "<span style=\"float:right;margin-right:20px;\">" + mo.Created + "</span></div>";
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("WebPartContent.aspx__GetMostDownLoadFileJson", ex.Message);
            }
            return json;
        }

        /// <summary>
        /// 获取最新上传文件列表
        /// </summary>
        [WebMethod]
        public static string GetLatestFileJson()
        {
            string json = "";
            try
            {
                using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        var fList = web.Lists.TryGetList(CommonHelper.docListName);

                        SPView oView = fList.Views["LatestFiles"];
                        var query = new SPQuery(oView);

                        // TODO
                        string dString = SPUtility.CreateISO8601DateTimeFromSystemDateTime(DateTime.Now.AddDays(-CommonHelper.LatestFileDay)); // 转换当前时间
                        //var query = new SPQuery();
                        query.ViewAttributes = "Scope=\"Recursive\"";
                        query.Query = "<OrderBy><FieldRef Name=\"Modified\" Ascending=\"False\"></FieldRef></OrderBy>" +
                                      "<Where><Geq><FieldRef Name='Modified' /><Value Type='DateTime'>" + dString + "</Value></Geq></Where>";
                        query.RowLimit = 20;

                        var myS = fList.GetItems(query);

                        foreach (SPListItem item in myS)
                        {
                            if (item.FileSystemObjectType == SPFileSystemObjectType.File)
                            {
                                var time = Convert.ToDateTime(item["Modified"]).ToString("yyyy-MM-dd HH:mm:ss");

                                string itemName = item.Name;
                                string displayName = itemName;
                                int endIndex;
                                bool b = CommonHelper.GetStrIndex(displayName, out endIndex);
                                if (!b)
                                {
                                    displayName = displayName.Substring(0, endIndex - 1) + "...";
                                }
                                string url = CommonHelper.EncodeUrl(item.File.ParentFolder.Url);
                                string fileUrl = CommonHelper.EncodeUrl(item.File.ServerRelativeUrl);
                                json += "<div style=\"margin:5px;\">" +
                                        "<a class=\"a_folder\" href=\"/" + url + "\" target=\"_blank\">Folder</a>" +
                                        "<img class=\"image\" src=\"/_layouts/15/IMAGES/" + item.File.IconUrl + "\">" +
                                        "<a href=\"/_layouts/15/FileDetail.aspx?Url=" + fileUrl + "\" target=\"_blank\" title=\"" + itemName + "\">" + displayName + "</a>" +
                                        "<span style=\"float:right;margin-right:20px;\">" + time + "</span></div>";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("WebPartContent.aspx__GetLatestFileJson", ex.Message);
            }
            return json;
        }

        /// <summary>
        /// 获取区域文件列表
        /// </summary>
        [WebMethod]
        public static string GetRegionFilesJson()
        {
            string json = "";
            try
            {
                using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        //var fList = web.Lists.TryGetList(CommonHelper.docListName);
                        SPFolder folder = web.GetFolder("/" + CommonHelper.RegionPath);

                        if (folder == null || !folder.Exists)
                        {
                            return json;
                        }

                        //var query = new SPQuery();
                        //query.ViewAttributes = "Scope=\"Recursive\"";
                        //query.Query = "<OrderBy><FieldRef Name=\"Modified\" Ascending=\"False\"></FieldRef></OrderBy>";
                        //query.RowLimit = 20;
                        //query.Folder = folder;
                        //var myS = fList.GetItems(query);
                        var files = CommonHelper.GetRegionFilesByFolder(folder, 20);

                        foreach (SPFile file in files)
                        {
                            var subG = CommonHelper.GetSubGroupName(file.Author);
                            //var time = Convert.ToDateTime(item["Created"]).ToString("yyyy-MM-dd HH:mm:ss");
                            if (subG.Length > 15)
                            {
                                subG = subG.Substring(0, 15) + "...";
                            }
                            string itemName = file.Name;
                            string displayName = itemName;
                            int endIndex;
                            bool b = CommonHelper.GetStrIndex(displayName, out endIndex);
                            if (!b)
                            {
                                displayName = displayName.Substring(0, endIndex - 1) + "...";
                            }
                            string url = CommonHelper.EncodeUrl(file.ParentFolder.Url);
                            string fileUrl = CommonHelper.EncodeUrl(file.ServerRelativeUrl);
                            json += "<div style=\"margin:5px;\">" +
                                    "<a class=\"a_folder\" href=\"/" + url + "\" target=\"_blank\">Folder</a>" +
                                    "<img class=\"image\" src=\"/_layouts/15/IMAGES/" + file.IconUrl + "\">" +
                                    "<a href=\"/_layouts/15/FileDetail.aspx?Url=" + fileUrl + "\" target=\"_blank\" title=\"" + itemName + "\">" + displayName + "</a>" +
                                    "<span style=\"float:right;margin-right:20px;\">" + subG + "</span></div>";
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("WebPartContent.aspx__GetRegionFilesJson", ex.Message);
            }
            return json;
        }

    }
}
