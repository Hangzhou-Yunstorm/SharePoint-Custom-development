using DocumentsSP.Helper;
using DocumentsSP.Model;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Generic;
using System.Timers;

namespace DocumentsSP
{
    public class DeleteDownloadTimer
    {
        public static bool isRunning = false;

        public static DateTime runTime = DateTime.MinValue;

        public static SPWeb spWeb = null;

        public DeleteDownloadTimer(SPWeb web)
        {
            spWeb = web;
            if (!isRunning)
            {
                Timer timer = new Timer();
                timer.Enabled = true;
                timer.AutoReset = true;
                //timer.Interval = 1000;
                timer.Start();
                timer.Elapsed += new ElapsedEventHandler(DeleteDownloadHistory);

                isRunning = true;
            }
        }

        private void DeleteDownloadHistory(object sender, ElapsedEventArgs e)
        {
            // 24小时执行一次
            if ((DateTime.Now - runTime).TotalHours > 24)
            {
                runTime = DateTime.Now;
                Timer tt = (Timer)sender;
                tt.Enabled = false;
                spWeb.AllowUnsafeUpdates = true;

                try
                {
                    XMLHelper.SetLog("Deltete start.", "DeleteDownloadHistory");

                    var dList = spWeb.Lists.TryGetList(CommonHelper.downloadListName);

                    string dString = SPUtility.CreateISO8601DateTimeFromSystemDateTime(DateTime.Now.AddMonths(-2).ToLocalTime());
                    var query = new SPQuery();
                    query.Query = "<Where><Lt><FieldRef Name='Modified' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + dString + "</Value></Lt></Where>";

                    var items = dList.GetItems(query);

                    XMLHelper.SetLog("Total count:" + items.Count, "DeleteDownloadHistory");

                    for (int i = 0; i < items.Count; i++)
                    {
                        items.Delete(i);
                        i--;
                    }
                    XMLHelper.SetLog("Delete end.", "DeleteDownloadHistory");
                }
                catch (Exception ex)
                {
                    XMLHelper.SetLog(ex.Message, "DeleteDownloadHistory");
                }

                try
                {
                    XMLHelper.SetLog("Deltete start.", "DeleteBrowsingHistory");

                    var dList = spWeb.Lists.TryGetList(CommonHelper.historyListName);

                    string dString = SPUtility.CreateISO8601DateTimeFromSystemDateTime(DateTime.Now.AddMonths(-2).ToLocalTime());
                    var query = new SPQuery();
                    query.Query = "<Where><Lt><FieldRef Name='Modified' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + dString + "</Value></Lt></Where>";

                    var items = dList.GetItems(query);

                    XMLHelper.SetLog("Total count:" + items.Count, "DeleteBrowsingHistory");

                    for (int i = 0; i < items.Count; i++)
                    {
                        items.Delete(i);
                        i--;
                    }
                    XMLHelper.SetLog("Delete end.", "DeleteBrowsingHistory");
                }
                catch (Exception ex)
                {
                    XMLHelper.SetLog(ex.Message, "DeleteBrowsingHistory");
                }

                spWeb.AllowUnsafeUpdates = false;
                tt.Enabled = true;
            }
        }

    }
}
