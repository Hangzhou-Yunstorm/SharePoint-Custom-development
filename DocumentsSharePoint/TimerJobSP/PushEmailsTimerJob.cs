using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using System.Collections.Generic;
using DocumentsSP.Helper;
using DocumentsSP.Model;
using DocumentsSP;

namespace TimerJobSP
{
    public class PushEmailsTimerJob : SPJobDefinition
    {
        public PushEmailsTimerJob() : base() { }

        public PushEmailsTimerJob(string TimerName, SPWebApplication webapp)
            : base(TimerName, webapp, null, SPJobLockType.ContentDatabase)
        {
            this.Title = "PushEmailsTimerJobS";
        }

        public override void Execute(Guid targetInstanceId)
        {
            SPWebApplication webapp = Parent as SPWebApplication;
            SPContentDatabase contentDB = webapp.ContentDatabases[targetInstanceId];
            using (SPWeb web = contentDB.Sites[0].OpenWeb())
            {
                try
                {
                    // 邮件发送人员列表
                    var userEmaiPushList = web.Lists.TryGetList(CommonHelper.userEmaiPushList);
                    // 订阅文件列表
                    var subscribeList = web.Lists.TryGetList(CommonHelper.subscribeList);

                    try
                    {
                        // 删除24小时之前的订阅
                        SPQuery dquery = new SPQuery();
                        string dString = SPUtility.CreateISO8601DateTimeFromSystemDateTime(DateTime.Now.ToLocalTime().AddHours(-24)); // 转换当前时间
                        dquery.Query = "<Where><Lt><FieldRef Name='Time' /><Value Type='DateTime'>" + dString + "</Value></Lt></Where>";
                        var ditems = subscribeList.GetItems(dquery);

                        while (ditems.Count > 0)
                        {
                            ditems[0].Delete();
                        }
                    }
                    catch { }

                    SPQuery query = new SPQuery();
                    query.Query = "<Where><Eq><FieldRef Name=\"IsPush\" /><Value Type=\"Boolean\">1</Value></Eq></Where>";
                    var items = userEmaiPushList.GetItems(query);

                    // 发送邮件的人员
                    if (items != null && items.Count > 0)
                    {
                        foreach (SPListItem item in items)
                        {
                            string userName = item["EmailUser"].ToString();
                            SPFieldUser _user = (SPFieldUser)item.Fields["EmailUser"];
                            SPFieldUserValue userValue = (SPFieldUserValue)_user.GetFieldValue(userName);
                            var user = userValue.User;
                            string emailTo = user.Email;

                            if (!string.IsNullOrEmpty(emailTo))
                            {
                                SPQuery squery = new SPQuery();
                                squery.Query = "<Where><Eq><FieldRef Name='UName' LookupId='True' /><Value Type='User'>" + user.ID + "</Value></Eq></Where>";
                                var sitems = subscribeList.GetItems(squery);

                                // 订阅的消息
                                if (sitems != null && sitems.Count > 0)
                                {
                                    List<string> folders = new List<string>();
                                    foreach (SPListItem sitem in sitems)
                                    {
                                        if (sitem["FolderPath"] != null)
                                        {
                                            folders.Add(web.Url + sitem["FolderPath"].ToString());
                                        }
                                    }

                                    try
                                    {
                                        LogModel model = new LogModel();
                                        model.Title = "Subscribe file Send mail";
                                        model.Operate = "Mail";
                                        model.Operater = user;
                                        model.Operator = user.Name;
                                        model.OperatorId = user.LoginName;
                                        model.ServerIP = CommonHelper.GetServerHostName();
                                        model.Department = CommonHelper.GetSubGroupName(user);
                                        model.DepartmentId = CommonHelper.GetSubGroupId(user);
                                        model.ObjectName = user.Email;
                                        model.ObjectType = "Subscribe";
                                        CommonHelper.SetLog(model, web.Site.ID);
                                    }
                                    catch { }

                                    // Send Mail
                                    SendMail(emailTo, folders, web.Url);
                                }
                            }
                        }
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="count">更新数量</param>
        /// <param name="webUrl">网站地址</param>
        private static void SendMail(string mailTo, List<string> folders, string webUrl)
        {
            try
            {
                List<string> mailToList = new List<string>();
                mailToList.Add(mailTo);

                string title = "Subscription file update";
                EmailHelper email = new EmailHelper(CommonHelper.mailUser, CommonHelper.mailPwd, CommonHelper.mailSmtp);
                email.mailSubject = title;
                string url = webUrl + "/Lists/SubscribeList/AllItems.aspx";
                string subTitle = "Here are some new subscription files update.";

                string mailContent = EmailHelper.GeneratorSubscribeMailContent(subTitle, folders, url);

                email.mailBody = EmailHelper.GeneratorMailHeader(title) + mailContent + EmailHelper.GeneratorMailFooter(title);
                email.isbodyHtml = true;    //是否是HTML
                email.mailToArray = mailToList;
                email.Send();
            }
            catch { }
        }

    }
}
