using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using DocumentsSP.Helper;
using Microsoft.SharePoint.Utilities;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Collections.Generic;
using System.Linq;

namespace DocumentsSP.Layouts
{
    public partial class SystemUseReport : LayoutsPageBase
    {
        public int TotalCount = 0;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            var siteId = SPContext.Current.Site.ID;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite site = new SPSite(siteId))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        var group = web.SiteGroups.GetByName(CommonHelper.adminGroup2);
                        TotalCount = group.Users.Count;
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
                            SPUser user = web.CurrentUser;
                            var logList = web.Lists.TryGetList(CommonHelper.logListName);

                            string userdepartId = string.Empty;
                            if (reportType != "Administrator")
                            {
                                var groups = user.Groups;
                                var thdepartIds = CommonHelper.TDGroupIds;
                                foreach (SPGroup group in groups)
                                {
                                    if (group.Name != CommonHelper.adminGroup2 && group.Description.StartsWith("Overseas_")) // 同步组的说明都是以‘Overseas_’开头
                                    {
                                        var departId = group.Description.Replace("Overseas_", "");
                                        if (!thdepartIds.Contains(departId))
                                        {
                                            userdepartId = departId;
                                        }
                                    }
                                }
                            }

                            List<string> names = new List<string>();
                            List<int> values = new List<int>();
                            if (month == "All")
                            {
                                #region 月份
                                DateTime nowDay = new DateTime(Convert.ToInt32(year), 1, 1);
                                for (int m = 0; m < 12; m++)
                                {
                                    DateTime start = nowDay.AddMonths(m);
                                    if (start > DateTime.Now)
                                    {
                                        break;
                                    }
                                    DateTime end = start.AddMonths(1);

                                    string startStr = SPUtility.CreateISO8601DateTimeFromSystemDateTime(start); // 开始时间
                                    string endStr = SPUtility.CreateISO8601DateTimeFromSystemDateTime(end); // 结束时间

                                    SPQuery dquery = new SPQuery();
                                    if (string.IsNullOrEmpty(userdepartId))
                                    {
                                        dquery.Query = "<Where><And>" +
                                                                  "<And><Eq><FieldRef Name='ObjectType' /><Value Type='Text'>Login</Value></Eq>" +
                                                                  "<Gt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + startStr + "</Value></Gt></And>" +
                                                                  "<Lt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + endStr + "</Value></Lt>" +
                                                                  "</And></Where>";
                                    }
                                    else
                                    {
                                        dquery.Query = "<Where><And>" +
                                                                  "<And><And><Eq><FieldRef Name='ObjectType' /><Value Type='Text'>Login</Value></Eq>" +
                                                                  "<Contains><FieldRef Name='DepartmentId' /><Value Type='Text'>" + userdepartId + "</Value></Contains></And>" +
                                                                  "<Gt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + startStr + "</Value></Gt></And>" +
                                                                  "<Lt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + endStr + "</Value></Lt>" +
                                                                  "</And></Where>";
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
                                    int count = 0;
                                    var g = logs.GroupBy(i => i);
                                    foreach (var item in g)
                                    {
                                        count++;
                                    }

                                    if (logins != null)
                                    {
                                        names.Add(start.ToString("yyyy-MM"));
                                        values.Add(count);
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                #region 天数
                                DateTime nowDay = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1);
                                int days = DateTime.DaysInMonth(nowDay.Year, nowDay.Month);

                                for (int m = 0; m < days; m++)
                                {
                                    DateTime start = nowDay.AddDays(m);
                                    if (start > DateTime.Now)
                                    {
                                        break;
                                    }
                                    DateTime end = start.AddDays(1);

                                    string startStr = SPUtility.CreateISO8601DateTimeFromSystemDateTime(start); // 开始时间
                                    string endStr = SPUtility.CreateISO8601DateTimeFromSystemDateTime(end); // 结束时间

                                    SPQuery dquery = new SPQuery();
                                    if (string.IsNullOrEmpty(userdepartId))
                                    {
                                        dquery.Query = "<Where><And>" +
                                                                  "<And><Eq><FieldRef Name='ObjectType' /><Value Type='Text'>Login</Value></Eq>" +
                                                                  "<Gt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + startStr + "</Value></Gt></And>" +
                                                                  "<Lt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + endStr + "</Value></Lt>" +
                                                                  "</And></Where>";
                                    }
                                    else
                                    {
                                        dquery.Query = "<Where><And>" +
                                                                  "<And><And><Eq><FieldRef Name='ObjectType' /><Value Type='Text'>Login</Value></Eq>" +
                                                                  "<Contains><FieldRef Name='DepartmentId' /><Value Type='Text'>" + userdepartId + "</Value></Contains></And>" +
                                                                  "<Gt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + startStr + "</Value></Gt></And>" +
                                                                  "<Lt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + endStr + "</Value></Lt>" +
                                                                  "</And></Where>";
                                    }
                                    var logins = logList.GetItems(dquery);
                                    if (logins != null)
                                    {
                                        List<string> logs = new List<string>();
                                        foreach (SPListItem log in logins)
                                        {
                                            var userId = log["OperatorId"];
                                            if (userId != null)
                                            {
                                                logs.Add(userId.ToString());
                                            }
                                        }
                                        int count = 0;
                                        var g = logs.GroupBy(i => i);
                                        foreach (var item in g)
                                        {
                                            count++;
                                        }

                                        names.Add(start.ToString("yyyy-MM-dd"));
                                        values.Add(count);
                                    }
                                }
                                #endregion
                            }

                            model.Names = names;
                            model.Values = values;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("SystemUseReport.aspx__GetDatas", ex.Message);
            }
            return jsonSerializer.Serialize(model);
        }
    }
}
