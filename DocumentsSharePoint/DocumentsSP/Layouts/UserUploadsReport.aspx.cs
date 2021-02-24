using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using DocumentsSP.Helper;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Utilities;
using System.Web.Script.Serialization;
using System.Web.Services;

namespace DocumentsSP.Layouts
{
    public partial class UserUploadsReport : LayoutsPageBase
    {
        public string groupsJson = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            var siteId = SPContext.Current.Site.ID;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite site = new SPSite(siteId))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        var groups = web.SiteGroups;
                        var thdepartIds = CommonHelper.TDGroupIds;
                        foreach (SPGroup group in groups)
                        {
                            if (group.Name != CommonHelper.adminGroup2 && group.Description.StartsWith("Overseas_")) // 同步组的说明都是以‘Overseas_’开头
                            {
                                var departId = group.Description.Replace("Overseas_", "");
                                if (!thdepartIds.Contains(departId))
                                {
                                    var groupName = group.Name;
                                    int endIndex;
                                    bool b = CommonHelper.GetStrIndex(groupName, 30, out endIndex);
                                    if (!b)
                                    {
                                        groupName = groupName.Substring(0, endIndex - 1) + "...";
                                    }
                                    groupsJson += "<option value=\"" + departId + "\">" + groupName + "</option>";
                                }
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 获取指定日期前20天使用情况数据
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns></returns>
        [WebMethod]
        public static string GetDatas(string year, string month, string departId)
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
            if (string.IsNullOrEmpty(departId))
            {
                month = "departId";
            }

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
                            var logList = web.Lists.TryGetList(CommonHelper.logListName);

                            List<string> names = new List<string>();
                            List<int> values = new List<int>();

                            string startStr = SPUtility.CreateISO8601DateTimeFromSystemDateTime(start); // 开始时间
                            string endStr = SPUtility.CreateISO8601DateTimeFromSystemDateTime(end); // 结束时间
                            SPQuery dquery = new SPQuery();

                            if (departId == "All")
                            {
                                dquery.Query = "<Where><And>" +
                                                          "<And><Eq><FieldRef Name='Operate' /><Value Type='Text'>Upload</Value></Eq>" +
                                                          "<Gt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + startStr + "</Value></Gt></And>" +
                                                          "<Lt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + endStr + "</Value></Lt>" +
                                                          "</And></Where>";
                            }
                            else
                            {
                                dquery.Query = "<Where><And><And><And>" +
                                                     "<Eq><FieldRef Name='Operate' /><Value Type='Text'>Upload</Value></Eq>" +
                                                     "<Contains><FieldRef Name='DepartmentId' /><Value Type='Text'>" + departId + "</Value></Contains></And>" +
                                                     "<Gt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + startStr + "</Value></Gt></And>" +
                                                     "<Lt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + endStr + "</Value></Lt></And>" +
                                                     "</Where>";
                            }

                            var logins = logList.GetItems(dquery);

                            List<string> logs = new List<string>();
                            foreach (SPListItem log in logins)
                            {
                                var userId = log["OperatorId"];
                                if (userId != null)
                                {
                                    logs.Add(userId.ToString());
                                }
                            }
                            var g = logs.GroupBy(i => i).OrderByDescending(t => t.Count()).Take(20);
                            foreach (var item in g)
                            {
                                var name = web.EnsureUser(item.Key).Name;
                                names.Add(name);
                                values.Add(item.Count());
                            }

                            model.Names = names;
                            model.Values = values;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("UserUploadsReport.aspx__GetDatas", ex.Message);
            }
            return jsonSerializer.Serialize(model);
        }

    }
}
