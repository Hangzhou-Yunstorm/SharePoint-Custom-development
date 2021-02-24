using DocumentsSP.Helper;
using DocumentsSP.Model;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Timers;

namespace DocumentsSP
{
    public class GetDocumentCalculatedTimer
    {
        public static bool isRunning = false;

        public GetDocumentCalculatedTimer()
        {
            if (!isRunning)
            {
                Timer timer = new Timer();
                timer.Enabled = true;
                timer.AutoReset = true;
                timer.Interval = 4 * 60 * 60 * 1000;//执行间隔时间4H,单位为毫秒  
                timer.Start();
                timer.Elapsed += new ElapsedEventHandler(GetDocumentCalculatedData);

                isRunning = true;
            }
        }

        private void GetDocumentCalculatedData(object sender, ElapsedEventArgs e)
        {
            documentCalculatedList = GetDocumentCalculatedList(true);

            mostDownloadList = GetMostDownloadList(true);
        }

        public static List<DocumentCalculatedModel> documentCalculatedList = new List<DocumentCalculatedModel>();

        public static List<FileModel> mostDownloadList = new List<FileModel>();

        /// <summary>
        /// 获取三个属性计算值列表
        /// </summary>
        /// <param name="isTimer">是否定时任务</param>
        /// <returns>列表</returns>
        public List<DocumentCalculatedModel> GetDocumentCalculatedList(bool isTimer)
        {
            List<DocumentCalculatedModel> list = new List<DocumentCalculatedModel>();
            if (documentCalculatedList.Count == 0 || isTimer)
            {
                try
                {
                    var siteId = SPContext.Current.Site.ID;
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        using (SPSite site = new SPSite(siteId))
                        {
                            using (SPWeb web = site.OpenWeb())
                            {
                                var dList = web.Lists.TryGetList(CommonHelper.documentCalculated);
                                var cList = dList.GetItems();

                                if (cList != null && cList.Count > 0)
                                {
                                    foreach (SPListItem item in cList)
                                    {
                                        DocumentCalculatedModel model = new DocumentCalculatedModel();
                                        model.FID = item["FID"].ToString();
                                        model.DownloadCount = Convert.ToInt32(item["DownloadCount"]);
                                        model.ClickCount = Convert.ToInt32(item["ClickCount"]);
                                        model.AveScore = Convert.ToDouble(item["AveScore"]).ToString("f1");
                                        model.AveScoreD = Convert.ToDouble(model.AveScore);

                                        list.Add(model);
                                    }
                                    documentCalculatedList = list;
                                }
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    CommonHelper.SetErrorLog("GetDocumentCalculatedTimer.cs__GetDocumentCalculatedList", ex.Message);
                }
            }
            return documentCalculatedList;

        }

        /// <summary>
        /// 下载最多列表
        /// </summary>
        /// <param name="isTimer">是否定时任务</param>
        /// <returns>列表</returns>
        public List<FileModel> GetMostDownloadList(bool isTimer)
        {
            List<FileModel> list = new List<FileModel>();
            if (mostDownloadList.Count == 0 || isTimer)
            {
                try
                {
                    var siteId = SPContext.Current.Site.ID;
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        using (SPSite site = new SPSite(siteId))
                        {
                            using (SPWeb web = site.OpenWeb())
                            {
                                var fileList = web.Lists.TryGetList(CommonHelper.docListName);
                                var dList = web.Lists.TryGetList(CommonHelper.documentCalculated);
                                var dquery = new SPQuery();
                                dquery.Query = "<OrderBy><FieldRef Name=\"DownloadCount\" Ascending=\"False\"></FieldRef></OrderBy>";
                                var cList = dList.GetItems(dquery);

                                if (cList != null && cList.Count > 0)
                                {
                                    int count = 0;
                                    foreach (SPListItem ditem in cList)
                                    {
                                        FileModel model = new FileModel();
                                        model.FID = ditem["FID"].ToString();
                                        model.DownCount = ditem["DownloadCount"].ToString();
                                        model.ClickCount = ditem["ClickCount"].ToString();
                                        model.AveScore = Convert.ToDouble(ditem["AveScore"]).ToString("f1");

                                        var query = new SPQuery();
                                        query.ViewAttributes = "Scope=\"Recursive\"";
                                        query.Query = "<Where><Eq><FieldRef Name=\"FID\" /><Value Type=\"Text\">" + model.FID + "</Value></Eq></Where>";

                                        var items = fileList.GetItems(query);
                                        if (items != null && items.Count > 0)
                                        {
                                            count++;
                                            if (count > 100)
                                            {
                                                break;
                                            }
                                            SPListItem item = items[0];

                                            model.FileUrl = CommonHelper.EncodeUrl(item.File.ServerRelativeUrl);
                                            model.ID = item.ID;
                                            model.Created = Convert.ToDateTime(item["Modified"]).ToString("yyyy-MM-dd HH:mm:ss");
                                            model.Name = item.Name;
                                            model.Icon = item.File.IconUrl;
                                            model.FileSize = CommonHelper.GetSize(item.File.Length);
                                            model.ParentFolder = item.File.ParentFolder.Url;
                                            model.CreatedBy = item.File.ModifiedBy.Name;
                                            list.Add(model);
                                        }
                                    }
                                    mostDownloadList = list;
                                }
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    CommonHelper.SetErrorLog("GetDocumentCalculatedTimer.cs__GetMostDownloadList", ex.Message);
                }
            }
            return mostDownloadList;

        }
    }
}
