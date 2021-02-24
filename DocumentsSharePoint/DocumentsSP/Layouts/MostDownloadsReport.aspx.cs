using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using DocumentsSP.Helper;
using Microsoft.SharePoint.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using System.Web.Script.Serialization;

namespace DocumentsSP.Layouts
{
    public partial class MostDownloadsReport : LayoutsPageBase
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 获取指定日期下载最多的20个文件
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns></returns>
        [WebMethod]
        public static string GetDatas(string year, string month)
        {
            ReportModel model = new ReportModel();
            if (string.IsNullOrEmpty(year))
            {
                year = DateTime.Now.Year.ToString();
            }
            if (string.IsNullOrEmpty(month))
            {
                month = "All";
            }

            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            try
            {
                var siteId = SPContext.Current.Site.ID;
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(siteId))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            SPUser user = web.CurrentUser;
                            var dList = web.Lists.TryGetList(CommonHelper.logListName);
                            var fileList = web.Lists.TryGetList(CommonHelper.docListName);

                            DateTime start, end;
                            if (month == "All")
                            {
                                start = new DateTime(Convert.ToInt32(year), 1, 1);
                                end = start.AddMonths(12);
                            }
                            else
                            {
                                start = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1);
                                end = start.AddMonths(1);
                            }

                            string startStr = SPUtility.CreateISO8601DateTimeFromSystemDateTime(start); // 开始时间
                            string endStr = SPUtility.CreateISO8601DateTimeFromSystemDateTime(end); // 结束时间
                            SPQuery dquery = new SPQuery();
                            dquery.Query = "<Where><And><And>" +
                                                      "<Gt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + startStr + "</Value></Gt>" +
                                                      "<Lt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + endStr + "</Value></Lt></And>" +
                                                      "<IsNotNull><FieldRef Name='FID' /></IsNotNull>" +
                                                      "</And></Where>";

                            var downloads = dList.GetItems(dquery);
                            if (downloads != null && downloads.Count > 0)
                            {
                                List<string> files = new List<string>();
                                foreach (SPListItem item in downloads)
                                {
                                    if (item["FID"] != null)
                                    {
                                        files.Add(item["FID"].ToString());
                                    }
                                }
                                List<string> names = new List<string>();
                                List<int> values = new List<int>();

                                var g = files.GroupBy(p => p).OrderByDescending(t => t.Count());

                                int n = 1;
                                foreach (var item in g)
                                {
                                    if (n > 20)
                                    {
                                        break;
                                    }

                                    var query = new SPQuery();
                                    query.ViewAttributes = "Scope=\"Recursive\"";
                                    query.Query = "<Where><Eq><FieldRef Name='FID' /><Value Type='Text'>" + item.Key + "</Value></Eq></Where>";
                                    var items = fileList.GetItems(query);
                                    if (items != null && items.Count > 0)
                                    {
                                        var name = items[0].Name;
                                        names.Add(name);
                                        values.Add(item.Count());
                                        n++;
                                    }
                                }
                                model.Names = names;
                                model.Values = values;
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("MostDownloadsReport.aspx__GetDatas", ex.Message);
            }
            return jsonSerializer.Serialize(model);
        }

    }
}
