using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Net;

namespace SyncGroups
{
    public static class CommonHelper
    {
        /// <summary>
        /// 获取ClientContext
        /// </summary>
        /// <returns>ClientContext</returns>
        public static ClientContext GetClientContext()
        {
            ClientContext context = new ClientContext(Constant.webUrl);
            try
            {
                context.Credentials = new NetworkCredential(Constant.loginName, Constant.psw, Constant.domain);
            }
            catch (Exception ex)
            {
                XMLHelper.SetLog(ex.Message, "GetClientContext");
            }

            return context;
        }

        /// <summary>
        /// 获取接收人列表
        /// </summary>
        /// <returns>接收人列表</returns>
        private static List<string> GetMailUser()
        {
            List<string> mailUsers = new List<string>();
            try
            {
                string mailTo = Constant.mailTo;
                if (!string.IsNullOrEmpty(mailTo))
                {
                    var mailToArr = mailTo.Split(',');
                    for (int m = 0; m < mailToArr.Length; m++)
                    {
                        if (!string.IsNullOrEmpty(mailToArr[m]))
                        {
                            mailUsers.Add(mailToArr[m]);
                        }
                    }
                }
            }
            catch { }

            return mailUsers;
        }

        /// <summary>
        /// 用户组禁用发送邮件
        /// </summary>
        /// <param name="deleteGroups">禁用用户组</param>
        public static void SendDeleteGroupMail(List<string> deleteGroups)
        {
            string title = "UserGroups Disable";
            EmailHelper email = new EmailHelper(Constant.mailUser, Constant.mailPwd, Constant.mailSmtp);
            email.mailSubject = title;
            string url = Constant.webUrl + "/_layouts/15/UserGroups.aspx";
            string subTitle = "These groups were disabled in database.";

            string mailContent = EmailHelper.GeneratorGroupMailContent(subTitle, deleteGroups, url);

            email.mailBody = EmailHelper.GeneratorMailHeader(title) + mailContent + EmailHelper.GeneratorMailFooter(title);
            email.isbodyHtml = true;    //是否是HTML
            email.mailToArray = GetMailUser();
            email.Send();

        }

        /// <summary>
        /// 用户组禁用发送邮件
        /// </summary>
        /// <param name="deleteGroups">禁用用户组</param>
        public static void SendErrorGroupMail()
        {
            string title = "UserGroups Synchronize  Exception";
            EmailHelper email = new EmailHelper(Constant.mailUser, Constant.mailPwd, Constant.mailSmtp);
            email.mailSubject = title;
            string url = Constant.webUrl + "/_layouts/15/UserGroups.aspx";
            string subTitle = "Middle database connection failure.";
            string msg = "There is an exception in the Middle Database,please fix it in time.";
            string mailContent = EmailHelper.GeneratorGroupErrorContent(subTitle, msg, url);

            email.mailBody = EmailHelper.GeneratorMailHeader(title) + mailContent + EmailHelper.GeneratorMailFooter(title);
            email.isbodyHtml = true;    //是否是HTML
            email.mailToArray = GetMailUser();
            email.Send();

        }

        /// <summary>
        /// 用户组改名发送邮件
        /// </summary>
        /// <param name="updateGroup">改名用户组</param>
        public static void SendUpdateGroupMail(List<string> updateGroup)
        {
            string title = "UserGroups Update";
            EmailHelper email = new EmailHelper(Constant.mailUser, Constant.mailPwd, Constant.mailSmtp);
            email.mailSubject = title;
            string url = Constant.webUrl + "/_layouts/15/UserGroups.aspx";
            string subTitle = "These groups name were changed in database.";

            string mailContent = EmailHelper.GeneratorGroupMailContent(subTitle, updateGroup, url);

            email.mailBody = EmailHelper.GeneratorMailHeader(title) + mailContent + EmailHelper.GeneratorMailFooter(title);
            email.isbodyHtml = true;    //是否是HTML
            email.mailToArray = GetMailUser();
            email.Send();

        }

    }
}