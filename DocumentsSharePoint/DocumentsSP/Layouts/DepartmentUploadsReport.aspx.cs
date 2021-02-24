using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using DocumentsSP.Helper;
using Microsoft.SharePoint.Utilities;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Web.Services;

namespace DocumentsSP.Layouts
{
    public partial class DepartmentUploadsReport : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 获取指定日期(月)部门上传数量
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns></returns>
        [WebMethod]
        public static string GetDatas(string year, string month, string reportType)
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
                            var dList = web.Lists.TryGetList(CommonHelper.logListName);

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

                            List<string> names = new List<string>();
                            List<int> values = new List<int>();

                            SPGroupCollection groups;
                            if (reportType == "Administrator")
                            {
                                groups = web.SiteGroups;
                            }
                            else
                            {
                                groups = web.CurrentUser.Groups;
                            }

                            var thdepartIds = CommonHelper.TDGroupIds;
                            foreach (SPGroup group in groups)
                            {
                                if (group.Name != CommonHelper.adminGroup2 && group.Description.StartsWith("Overseas_")) // 同步组的说明都是以‘Overseas_’开头
                                {
                                    var departId = group.Description.Replace("Overseas_", "");
                                    if (!thdepartIds.Contains(departId))
                                    {
                                        SPQuery dquery = new SPQuery();
                                        dquery.Query = "<Where><And>" +
                                                                   "<And><And><Contains><FieldRef Name='DepartmentId' /><Value Type='Text'>" + departId + "</Value></Contains>" +
                                                                   "<Eq><FieldRef Name='Operate' /><Value Type='Text'>Upload</Value></Eq></And>" +
                                                                   "<Gt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + startStr + "</Value></Gt></And>" +
                                                                   "<Lt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + endStr + "</Value></Lt>" +
                                                                   "</And></Where>";

                                        var uploads = dList.GetItems(dquery);
                                        int count = 0;
                                        if (uploads != null && uploads.Count > 0)
                                        {
                                            count = uploads.Count;
                                        }
                                        names.Add(group.Name);
                                        values.Add(count);
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
                CommonHelper.SetErrorLog("DepartmentUploadsReport.aspx__GetDatas", ex.Message);
            }
            return jsonSerializer.Serialize(model);
        }

    }
}
