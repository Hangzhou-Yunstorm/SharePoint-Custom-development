using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using DocumentsSP.Helper;
using Microsoft.SharePoint.Utilities;

namespace DocumentsSP.Layouts
{
    public partial class GKSData : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 历史数据
            var siteId = SPContext.Current.Site.ID;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite site = new SPSite(siteId))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        web.AllowUnsafeUpdates = true;
                        var logList = web.Lists.TryGetList(CommonHelper.logListName);
                        var docList = web.Lists.TryGetList(CommonHelper.docListName);
                        //var downloadList = web.Lists.TryGetList(CommonHelper.downloadListName);
                        var rmsHistory = web.Lists.TryGetList(CommonHelper.rmsHistory);
                        var externalLogs = web.Lists.TryGetList(CommonHelper.externalLogs);
                        var externalUsers = web.Lists.TryGetList(CommonHelper.externalUserList);

                        // 登录
                        var query1 = new SPQuery();
                        query1.Query = "<Where><Eq><FieldRef Name='ObjectType' /><Value Type='Text'>Login</Value></Eq></Where>";
                        var items1 = logList.GetItems(query1);
                        if (items1 != null)
                        {
                            Label11.Text = items1.Count + "次";
                        }

                        // 文件/文件夹总数
                        Label12.Text = docList.ItemCount + "个";

                        // 文件数
                        var query2 = new SPQuery();
                        query2.ViewAttributes = "Scope=\"Recursive\"";
                        var items2 = docList.GetItems(query2);
                        if (items2 != null)
                        {
                            Label13.Text = items2.Count + "个";
                        }

                        // 文件夹数
                        Label14.Text = (Convert.ToInt32(Label12.Text.Replace("个", "")) - Convert.ToInt32(Label13.Text.Replace("个", ""))) + "个";

                        // 下载次数
                        //Label15.Text = downloadList.ItemCount + "次";
                        var query5 = new SPQuery();
                        query5.Query = "<Where><Eq><FieldRef Name='Operate' /><Value Type='Text'>Download</Value></Eq></Where>";
                        var items5 = logList.GetItems(query5);
                        if (items5 != null)
                        {
                            Label15.Text = items5.Count + "次";
                        }

                        // RMS 加密
                        Label16.Text = rmsHistory.ItemCount + "个";

                        // 外部站点登录
                        var query7 = new SPQuery();
                        query7.Query = "<Where><Eq><FieldRef Name='Operate' /><Value Type='Text'>Login</Value></Eq></Where>";
                        var items7 = externalLogs.GetItems(query7);
                        if (items7 != null)
                        {
                            Label17.Text = items7.Count + "次";
                        }

                        // 外部站点上传
                        var query8 = new SPQuery();
                        query8.Query = "<Where><Eq><FieldRef Name='Operate' /><Value Type='Text'>Upload</Value></Eq></Where>";
                        var items8 = externalLogs.GetItems(query8);
                        if (items8 != null)
                        {
                            Label18.Text = items8.Count + "个";
                        }

                        // 外部站点下载
                        var query9 = new SPQuery();
                        query9.Query = "<Where><Eq><FieldRef Name='Operate' /><Value Type='Text'>Download</Value></Eq></Where>";
                        var items9 = externalLogs.GetItems(query9);
                        if (items9 != null)
                        {
                            Label19.Text = items9.Count + "个";
                        }

                        // 外部站点用户数
                        Label20.Text = externalUsers.ItemCount + "个";

                        web.AllowUnsafeUpdates = false;
                    }
                }
            });
        }

        protected void GetData_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(date_range.Text))
            {
                Response.Write("<script>alert('Please select date !');</script>");
                return;
            }
            Label1.Text = date_range.Text;
            var date = date_range.Text.Replace(" ", "").Split('至');

            string startStr = SPUtility.CreateISO8601DateTimeFromSystemDateTime(Convert.ToDateTime(date[0])); // 开始时间
            string endStr = SPUtility.CreateISO8601DateTimeFromSystemDateTime(Convert.ToDateTime(date[1]).AddDays(1)); // 结束时间

            var siteId = SPContext.Current.Site.ID;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SPSite site = new SPSite(siteId))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        web.AllowUnsafeUpdates = true;
                        var logList = web.Lists.TryGetList(CommonHelper.logListName);
                        //var downloadList = web.Lists.TryGetList(CommonHelper.downloadListName);
                        var rmsHistory = web.Lists.TryGetList(CommonHelper.rmsHistory);
                        var externalLogs = web.Lists.TryGetList(CommonHelper.externalLogs);

                        // 登录
                        var query1 = new SPQuery();
                        query1.Query = "<Where><And>" +
                                                 "<And><Eq><FieldRef Name='ObjectType' /><Value Type='Text'>Login</Value></Eq>" +
                                                 "<Gt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + startStr + "</Value></Gt></And>" +
                                                 "<Lt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + endStr + "</Value></Lt>" +
                                                 "</And></Where>";
                        var items1 = logList.GetItems(query1);
                        if (items1 != null)
                        {
                            Label2.Text = items1.Count + "次";
                        }

                        // 文件上传
                        var query12 = new SPQuery();
                        query12.Query = "<Where><And>" +
                                                 "<And><Eq><FieldRef Name='Operate' /><Value Type='Text'>Upload</Value></Eq>" +
                                                 "<Gt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + startStr + "</Value></Gt></And>" +
                                                 "<Lt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + endStr + "</Value></Lt>" +
                                                 "</And></Where>";
                        var items12 = logList.GetItems(query12);
                        if (items12 != null)
                        {
                            Label4.Text = items12.Count + "个";
                        }

                        // 文件夹创建
                        var query2 = new SPQuery();
                        query2.Query = "<Where><And>" +
                                                 "<And><Contains><FieldRef Name='Title' /><Value Type='Text'>Add folder</Value></Contains>" +
                                                 "<Gt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + startStr + "</Value></Gt></And>" +
                                                 "<Lt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + endStr + "</Value></Lt>" +
                                                 "</And></Where>";
                        var items2 = logList.GetItems(query2);
                        if (items2 != null)
                        {
                            Label5.Text = items2.Count + "个";
                        }

                        // 下载次数
                        var query4 = new SPQuery();
                        query4.Query = "<Where><And><And>" +
                                                "<Gt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + startStr + "</Value></Gt>" +
                                                "<Lt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + endStr + "</Value></Lt></And>" +
                                                "<Eq><FieldRef Name='Operate' /><Value Type='Text'>Download</Value></Eq>" +
                                                "</And></Where>";
                        var items4 = logList.GetItems(query4);

                        if (items4 != null)
                        {
                            Label6.Text = items4.Count + "次";
                        }

                        // RMS加密
                        var query6 = new SPQuery();
                        query6.Query = "<Where><And>" +
                                              "<Gt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + startStr + "</Value></Gt>" +
                                              "<Lt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + endStr + "</Value></Lt>" +
                                              "</And></Where>";
                        var items6 = rmsHistory.GetItems(query6);
                        if (items6 != null)
                        {
                            Label7.Text = items6.Count + "个";
                        }

                        // 外部站点登录
                        var query7 = new SPQuery();
                        query7.Query = "<Where><And>" +
                                                "<And><Eq><FieldRef Name='Operate' /><Value Type='Text'>Login</Value></Eq>" +
                                                "<Gt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + startStr + "</Value></Gt></And>" +
                                                "<Lt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + endStr + "</Value></Lt>" +
                                                "</And></Where>";
                        var items7 = externalLogs.GetItems(query7);
                        if (items7 != null)
                        {
                            Label8.Text = items7.Count + "次";
                        }

                        // 外部站点上传
                        var query8 = new SPQuery();
                        query8.Query = "<Where><And>" +
                                                "<And><Eq><FieldRef Name='Operate' /><Value Type='Text'>Upload</Value></Eq>" +
                                                "<Gt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + startStr + "</Value></Gt></And>" +
                                                "<Lt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + endStr + "</Value></Lt>" +
                                                "</And></Where>";
                        var items8 = externalLogs.GetItems(query8);
                        if (items8 != null)
                        {
                            Label9.Text = items8.Count + "个";
                        }

                        // 外部站点下载
                        var query9 = new SPQuery();
                        query9.Query = "<Where><And>" +
                                            "<And><Eq><FieldRef Name='Operate' /><Value Type='Text'>Download</Value></Eq>" +
                                            "<Gt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + startStr + "</Value></Gt></And>" +
                                            "<Lt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + endStr + "</Value></Lt>" +
                                            "</And></Where>";
                        var items9 = externalLogs.GetItems(query9);
                        if (items9 != null)
                        {
                            Label10.Text = items9.Count + "个";
                        }

                        web.AllowUnsafeUpdates = false;
                    }
                }
            });
        }
    }
}
