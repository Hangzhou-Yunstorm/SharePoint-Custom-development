using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using DocumentsSP.Helper;
using Microsoft.SharePoint.Utilities;
using DocumentsSP.Model;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Linq;
using System.Web.Services;

namespace DocumentsSP.Layouts
{
    public partial class LogList : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 获取日志
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="date">日期</param>
        /// <returns></returns>
        [WebMethod]
        public static string GetLogList(string key, string date)
        {
            string json = "[]";
            try
            {
                var siteId = SPContext.Current.Site.ID;
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (SPSite site = new SPSite(siteId))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            web.AllowUnsafeUpdates = true;
                            var logList = web.Lists.TryGetList(CommonHelper.logListName);
                            DateTime dateTime;
                            if (string.IsNullOrEmpty(date))
                            {
                                dateTime = DateTime.Now;
                            }
                            else
                            {
                                dateTime = DateTime.Parse(date);
                            }
                            string d24String = SPUtility.CreateISO8601DateTimeFromSystemDateTime(dateTime.AddHours(-24)); // 转换时间-24
                            string dString = SPUtility.CreateISO8601DateTimeFromSystemDateTime(dateTime); // 转换时间

                            SPQuery query = new SPQuery();
                            query.Query = "<OrderBy><FieldRef Name='Created' Ascending='False'></FieldRef></OrderBy>" +
                                                   "<Where><And>" +
                                                   "<Gt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + d24String + "</Value></Gt>" +
                                                   "<Lt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + dString + "</Value></Lt>" +
                                                   "</And></Where>";

                            //query.Query = "<OrderBy><FieldRef Name='Created' Ascending='False'></FieldRef></OrderBy>" +
                            //                        "<Where><And>" +
                            //                        "<And><Or><Or><Contains><FieldRef Name='Title' /><Value Type='Text'>" + key + "</Value></Contains>" +
                            //                        "<Contains><FieldRef Name='Operator' /><Value Type='Text'>" + key + "</Value></Contains></Or>" +
                            //                        "<Contains><FieldRef Name='Department' /><Value Type='Text'>" + key + "</Value></Contains></Or>" +
                            //                        "<Gt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + d24String + "</Value></Gt></And>" +
                            //                        "<Lt><FieldRef Name='Created' /><Value Type='DateTime' IncludeTimeValue='TRUE'>" + dString + "</Value></Lt>" +
                            //                        "</And></Where>";

                            var logs = logList.GetItems(query);
                            if (logs != null && logs.Count > 0)
                            {
                                List<LogListModel> models = new List<LogListModel>();
                                foreach (SPListItem item in logs)
                                {
                                    LogListModel model = new LogListModel();
                                    model.Title = item["Title"] == null ? "" : item["Title"].ToString();
                                    model.ObjectName = item["ObjectName"] == null ? "" : item["ObjectName"].ToString();
                                    model.Operate = item["Operate"] == null ? "" : item["Operate"].ToString();
                                    model.ObjectType = item["ObjectType"] == null ? "" : item["ObjectType"].ToString();
                                    model.Operator = item["Operator"] == null ? "" : item["Operator"].ToString();
                                    model.Department = item["Department"] == null ? "" : item["Department"].ToString();
                                    model.ServerIP = item["ServerIP"] == null ? "" : item["ServerIP"].ToString();
                                    model.Created = item["Created"] == null ? "" : Convert.ToDateTime(item["Created"]).ToString("yyyy-MM-dd HH:mm:ss");

                                    models.Add(model);
                                }
                                key = key.Trim();
                                if (!string.IsNullOrEmpty(key))
                                {
                                    models = (from v in models
                                              where v.Title.Contains(key) || v.ObjectName.Contains(key) || v.Operate.Contains(key) ||
                                                    v.ServerIP.Contains(key) || v.Operator.Contains(key) || v.Department.Contains(key)
                                              select v).ToList();
                                }
                                DataContractJsonSerializer dcs = new DataContractJsonSerializer(models.GetType());
                                //序列化
                                using (MemoryStream stream = new MemoryStream())
                                {
                                    dcs.WriteObject(stream, models);
                                    json = Encoding.UTF8.GetString(stream.ToArray());
                                }
                            }
                            web.AllowUnsafeUpdates = false;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("LogList.aspx_GetLogList", ex.Message);
            }
            return json;

        }
    }
}
