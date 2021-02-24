using DocumentsSP.Helper;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DocumentsSP
{
    public class EmailHelper
    {
        /// <summary>
        /// 发送者
        /// </summary>
        public string mailFrom { get; set; }

        /// <summary>
        /// 收件人
        /// </summary>
        public List<string> mailToArray { get; set; }

        /// <summary>
        /// 抄送
        /// </summary>
        public List<string> mailCcArray { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string mailSubject { get; set; }

        /// <summary>
        /// 正文
        /// </summary>
        public string mailBody { get; set; }

        /// <summary>
        /// 发件人密码
        /// </summary>
        public string mailPwd { get; set; }

        /// <summary>
        /// SMTP邮件服务器
        /// </summary>
        public string host { get; set; }

        /// <summary>
        /// 正文是否是html格式
        /// </summary>
        public bool isbodyHtml { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        public string[] attachmentsPath { get; set; }

        /// <summary>
        /// 邮件顶部标题
        /// </summary>
        /// <param name="headerTitle">邮件正文大标题</param>
        /// <returns></returns>
        public static string GeneratorMailHeader(string headerTitle)
        {
            int tbWidth = 1000;
            string BasicHeaderContent =
               "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"zhwd-mobile-fullwidth\" style=\"background-color: #f7f9fa;margin:0 auto;\" width=\"" + tbWidth + "\">" +
               "<tbody>" +
                   "<tr>" +
                       "<td align=\"center\">" +
                           "<table cellpadding=\"0\" cellspacing=\"0\" class=\"zhwd-mobile-no-radius\" style=\"border-radius: 4px; border: 1px solid #dedede; margin: 30px auto; background-color: #ffffff\">" +
                              "<tbody>" +
                                   "<tr>" +
                                       "<td style=\"padding: 25px 35px 45px 35px\" class=\"zhwd-mobile-collapse-padding\" align=\"left\">" +
                                           "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"font-family:'Microsoft YaHei'\">" +
                                              "<tbody>" +
                                                   "<tr>" +
                                                       "<td>" +
                                                           "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">" +
                                                               "<tbody>" +
                                                                   "<tr height=\"70\">" +
                                                                       "<td align=\"center\" height=\"70\">" +
                                                                       "<div style=\"border-radius:10px;width:590px;height:70px;margin:0 auto;\"><div style=\"width:800px;height:70px;border-radius:10px;color:#fff;margin:0 auto;background-color:#0e60ac;\">" +
                                                                       "<span style=\"font-size:22px;height:70px;font-family:Microsoft YaHei;margin:0px auto;line-height:70px\">" + headerTitle + "</span></div></div>" +
                                                                       "</td>" +
                                                                   "</tr>" +
                                                               "</tbody>" +
                                                           "</table>" +
                                                       "</td>" +
                                                   "</tr>";
            return BasicHeaderContent;
        }

        /// <summary>
        /// 邮件底部标题
        /// </summary>
        /// <param name="mailFooter">邮件底部标题</param>
        /// <returns></returns>
        public static string GeneratorMailFooter(string mailFooter)
        {
            string BasicFooter =
                                                    "</tbody>" +
                                                "</table>" +
                                            "</td>" +
                                        "</tr>" +
                                    "</tbody>" +
                                "</table>" +
                            "</td>" +
                        "</tr>" +
                        "<tr>" +
                            "<td>" +
                                "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">" +
                                    "<tbody>" +
                                        "<tr>" +
                                            "<td style=\"padding: 20px 25px 45px 25px; font-size: 16px; color: #575757; line-height: 25px\" class=\"zhwd-mobile-small-footer\" align=\"center\">" +
                                                mailFooter +
                                            "</td>" +
                                        "</tr>" +
                                    "</tbody>" +
                                "</table>" +
                            "</td>" +
                        "</tr>" +
                    "</tbody>" +
                "</table>";
            return BasicFooter;
        }

        /// <summary>
        /// 审批拒绝邮件模板
        /// </summary>
        /// <param name="Approver">审批人</param>
        /// <param name="time">审批时间</param>
        /// <param name="filedirectory">文件所在目录</param>
        /// <param name="description">审批备注</param>
        /// <param name="url">我的请求</param>
        /// <returns></returns>
        public static string GeneratorRejectMailContent(string docTitle, string Approver, string time, string filedirectory, string description, string url)
        {
            string authorTitle = "Approver";
            string docUpdatedTitle = "Approval time";
            string fileFolderTitle = "File directory";
            string content = "";
            string quickRemark = description;
            if (quickRemark.Length > 500)
                quickRemark = quickRemark.Substring(0, 500) + "......";
            else
                quickRemark += "......";
            content += "<tr>" +
                                                 "<td height=\"25\" style=\"font-size: 0px; line-height: 0px\">&nbsp;</td>" +
                                             "</tr>" +
                                             "<tr>" +
                                                 "<td>" +
                                                     "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\">" +
                                                         "<tbody>" +
                                                             "<tr>" +
                                                                 "<td height=\"35\" style=\"font-size: 0px; line-height: 0px; border-top: 1px #f1f4f6 solid\">&nbsp;</td>" +
                                                             "</tr>" +
                                                             "<tr>" +
                                                                 "<td>" +
                                                                     "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\">" +
                                                                         "<tbody>" +
                                                                             "<tr>" +
                                                                                 "<td><a href=\"" + url + "\" style=\"font-size: 17px; font-weight: bold; text-decoration: none; color: #259; border: none; outline: none\" target=\"_blank\">" + docTitle + "</a></td>" +
                                                                             "</tr>" +
                                                                             "<tr>" +
                                                                                 "<td>" +
                                                                                     "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" class=\"zhwd-mobile-no-fullwidth\">" +
                                                                                        "<tbody>" +
                                                                                        "<tr><td align=\"left\" height=\"10\" colspan=\"5\" width=\"100%\"></td></tr>" +
                                                                                        "<tr><td width=\"20%\" align=\"left\"><span style=\"font-size:14px;width:50px;\">" + authorTitle + "</span></td>" +
                                                                                        "<td align=\"left\" style=\"word-wrap:break-word;\" width=\"20%\"><a style=\"font-size:14px;color:#333333;text-decoration:none\" target=\"_blank\"><strong>" + Approver + "</strong></a></td>" +
                                                                                        "<td width=\"25%\" align=\"left\"><span style=\"font-size:14px\">" + docUpdatedTitle + "</span></td>" +
                                                                                        "<td align=\"left\" width=\"35%\" colspan=\"2\"><a style=\"font-size:14px;color:#333333;text-decoration:none;\" target=\"_blank\">" + time + "</a></td></tr>" +
                                                                                        "<tr><td align=\"left\" height=\"10\" colspan=\"5\" width=\"100%\"></td></tr><tr><td width=\"20%\" align=\"left\"><span style=\"font-size:14px\">" + fileFolderTitle + "</span></td>" +
                                                                                        "<td align=\"left\" colspan=\"4\" style=\"word-wrap:break-word;\" width=\"80%\"><a  href=\"" + filedirectory + "\" style=\"font-size:14px;color:#333333;text-decoration: none;white-space: nowrap;\" target=\"_blank\"><strong>" + filedirectory + "</strong></a></td></tr>" +
                                                                                        "<tr><td height=\"30\" colspan=\"5\" width=\"100%\"></td></tr></tbody>" +
                                                                                     "</table>" +
                                                                                 "</td>" +
                                                                             "</tr>" +
                                                                             "<tr>" +
                                                                                 "<td>" +
                                                                                     "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\">" +
                                                                                         "<tbody>" +
                                                                                             "<tr>" +
                                                                                                 "<td style=\"word-break: break-all\"><a href=\"" + url + "\" style=\"font-size: 13px;margin:15px 0; line-height: 22px; text-decoration: none; color: #333; display: block\" target=\"_blank\">" + quickRemark + "<span style=\"font-size: 13px; display: inline-block; color: #259\">&nbsp;Click to view</span></a></td>" +
                                                                                                 "<td width=\"35\" class=\"zhwd-mobile-hide\">&nbsp;</td>" +
                                                                                             "</tr>" +
                                                                                         "</tbody>" +
                                                                                   "</table>" +
                                                                                 "</td>" +
                                                                             "</tr>" +
                                                                             "<tr>" +
                                                                                 "<td height=\"28\" style=\"font-size: 0px; line-height: 0px\">&nbsp;</td>" +
                                                                             "</tr>" +
                                                                         "</tbody>" +
                                                                     "</table>" +
                                                                 "</td>" +
                                                             "</tr>" +
                                                         "</tbody>" +
                                                     "</table>" +
                                                 "</td>" +
                                             "</tr>";

            return content;
        }

        /// <summary>
        /// 审批提醒邮件模板
        /// </summary>
        /// <param name="author">文件作者</param>
        /// <param name="time">上传/修改时间</param>
        /// <param name="folderPath">文件目录</param>
        /// <param name="description">文档描述</param>
        /// <param name="url">审批地址</param>
        /// <returns></returns>
        public static string GeneratorApproveMailContent(string docTitle, string author, string time, string folderPath, string description, string url, string country)
        {
            string authorTitle = "Author";
            string docUpdatedTitle = "Updated";
            string fileFolderTitle = "Folder Path";
            string content = "";
            string quickRemark = description;

            if (quickRemark.Length > 500)
                quickRemark = quickRemark.Substring(0, 500) + "......";
            else
                quickRemark += "......";
            content += "<tr>" +
                                   "<td height=\"25\" style=\"font-size: 0px; line-height: 0px\">&nbsp;</td>" +
                               "</tr>" +
                               "<tr>" +
                                   "<td>" +
                                       "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\">" +
                                           "<tbody>" +
                                               "<tr>" +
                                                   "<td height=\"35\" style=\"font-size: 16px; line-height: 0px; border-top: 1px #f1f4f6 solid\">There is a pending document in " + country + " site.</td>" +
                                               "</tr>" +
                                               "<tr>" +
                                                   "<td height=\"35\" style=\"font-size: 0px; line-height: 0px; border-top: 1px #f1f4f6 solid\">&nbsp;</td>" +
                                               "</tr>" +
                                               "<tr>" +
                                                   "<td>" +
                                                       "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\">" +
                                                           "<tbody>" +
                                                               "<tr>" +
                                                                   "<td><a href=\"" + url + "\" style=\"font-size: 17px; font-weight: bold; text-decoration: none; color: #259; border: none; outline: none\" target=\"_blank\">" + docTitle + "</a></td>" +
                                                               "</tr>" +
                                                               "<tr>" +
                                                                   "<td>" +
                                                                       "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" class=\"zhwd-mobile-no-fullwidth\">" +
                                                                          "<tbody>" +
                                                                          "<tr><td align=\"left\" height=\"10\" colspan=\"5\" width=\"100%\"></td></tr>" +
                                                                          "<tr><td width=\"20%\" align=\"left\"><span style=\"font-size:14px;width:50px;\">" + authorTitle + "</span></td>" +
                                                                          "<td align=\"left\" style=\"word-wrap:break-word;\" width=\"20%\"><a style=\"font-size:14px;color:#333333;text-decoration:none\" target=\"_blank\"><strong>" + author + "</strong></a></td>" +
                                                                          "<td width=\"25%\" align=\"left\"><span style=\"font-size:14px\">" + docUpdatedTitle + "</span></td>" +
                                                                          "<td align=\"left\" width=\"35%\" colspan=\"2\"><a style=\"font-size:14px;color:#333333;text-decoration:none;\" target=\"_blank\">" + time + "</a></td></tr>" +
                                                                          "<tr><td align=\"left\" height=\"10\" colspan=\"5\" width=\"100%\"></td></tr><tr><td width=\"20%\" align=\"left\"><span style=\"font-size:14px\">" + fileFolderTitle + "</span></td>" +
                                                                          "<td align=\"left\" colspan=\"4\" style=\"word-wrap:break-word;\" width=\"80%\"><span style=\"font-size:14px;color:#333333;white-space: nowrap;\"><strong>" + folderPath + "</strong></span></td></tr>" +
                                                                          "<tr><td height=\"30\" colspan=\"5\" width=\"100%\"></td></tr></tbody>" +
                                                                       "</table>" +
                                                                   "</td>" +
                                                               "</tr>" +
                                                               "<tr>" +
                                                                   "<td>" +
                                                                       "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\">" +
                                                                           "<tbody>" +
                                                                               "<tr>" +
                                                                                   "<td style=\"word-break: break-all\"><a href=\"" + url + "\" style=\"font-size: 13px;margin:15px 0; line-height: 22px; text-decoration: none; color: #333; display: block\" target=\"_blank\">" + quickRemark + "<span style=\"font-size: 13px; display: inline-block; color: #259\">&nbsp;Click for approval</span></a></td>" +
                                                                                   "<td width=\"35\" class=\"zhwd-mobile-hide\">&nbsp;</td>" +
                                                                               "</tr>" +
                                                                           "</tbody>" +
                                                                     "</table>" +
                                                                   "</td>" +
                                                               "</tr>" +
                                                               "<tr>" +
                                                                   "<td height=\"28\" style=\"font-size: 0px; line-height: 0px\">&nbsp;</td>" +
                                                               "</tr>" +
                                                           "</tbody>" +
                                                       "</table>" +
                                                   "</td>" +
                                               "</tr>" +
                                           "</tbody>" +
                                       "</table>" +
                                   "</td>" +
                               "</tr>";
                   
            return content;
        }

        /// <summary>
        /// 反馈提醒邮件模板
        /// </summary>
        /// <param name="author">反馈者</param>
        /// <param name="time">反馈时间</param>
        /// <param name="folderPath">文件目录</param>
        /// <param name="description">反馈内容</param>
        /// <param name="url">文件地址</param>
        /// <returns></returns>
        public static string GeneratorFeedbackMailContent(string docTitle, string author, string time, string folderPath, string description, string url)
        {
            string authorTitle = "author";
            string docUpdatedTitle = "Feedback date";
            string fileFolderTitle = "File Path";
            string content = "";
            string quickRemark = description;
            if (quickRemark.Length > 500)
                quickRemark = quickRemark.Substring(0, 500) + "......";
            else
                quickRemark += "......";
            content += "<tr>" +
                                                 "<td height=\"25\" style=\"font-size: 0px; line-height: 0px\">&nbsp;</td>" +
                                             "</tr>" +
                                             "<tr>" +
                                                 "<td>" +
                                                     "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\">" +
                                                         "<tbody>" +
                                                             "<tr>" +
                                                                 "<td height=\"35\" style=\"font-size: 0px; line-height: 0px; border-top: 1px #f1f4f6 solid\">&nbsp;</td>" +
                                                             "</tr>" +
                                                             "<tr>" +
                                                                 "<td>" +
                                                                     "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\">" +
                                                                         "<tbody>" +
                                                                             "<tr>" +
                                                                                 "<td><a href=\"" + url + "\" style=\"font-size: 17px; font-weight: bold; text-decoration: none; color: #259; border: none; outline: none\" target=\"_blank\">" + docTitle + "</a></td>" +
                                                                             "</tr>" +
                                                                             "<tr>" +
                                                                                 "<td>" +
                                                                                     "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" class=\"zhwd-mobile-no-fullwidth\">" +
                                                                                        "<tbody>" +
                                                                                        "<tr><td align=\"left\" height=\"10\" colspan=\"5\" width=\"100%\"></td></tr>" +
                                                                                        "<tr><td width=\"20%\" align=\"left\"><span style=\"font-size:14px;width:50px;\">" + authorTitle + "</span></td>" +
                                                                                        "<td align=\"left\" style=\"word-wrap:break-word;\" width=\"20%\"><a style=\"font-size:14px;color:#333333;text-decoration:none\" target=\"_blank\"><strong>" + author + "</strong></a></td>" +
                                                                                        "<td width=\"25%\" align=\"left\"><span style=\"font-size:14px\">" + docUpdatedTitle + "</span></td>" +
                                                                                        "<td align=\"left\" width=\"35%\" colspan=\"2\"><a style=\"font-size:14px;color:#333333;text-decoration:none;\" target=\"_blank\">" + time + "</a></td></tr>" +
                                                                                        "<tr><td align=\"left\" height=\"10\" colspan=\"5\" width=\"100%\"></td></tr><tr><td width=\"20%\" align=\"left\"><span style=\"font-size:14px\">" + fileFolderTitle + "</span></td>" +
                                                                                        "<td align=\"left\" colspan=\"4\" style=\"word-wrap:break-word;\" width=\"80%\"><a href=\"" + folderPath + "\" style=\"font-size:14px;color:#333333;text-decoration: none;white-space: nowrap;\" target=\"_blank\"><strong>" + folderPath + "</strong></a></td></tr>" +
                                                                                        "<tr><td height=\"30\" colspan=\"5\" width=\"100%\"></td></tr></tbody>" +
                                                                                     "</table>" +
                                                                                 "</td>" +
                                                                             "</tr>" +
                                                                             "<tr>" +
                                                                                 "<td>" +
                                                                                     "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\">" +
                                                                                         "<tbody>" +
                                                                                             "<tr>" +
                                                                                                 "<td style=\"word-break: break-all\"><a href=\"" + url + "\" style=\"font-size: 13px;margin:15px 0; line-height: 22px; text-decoration: none; color: #333; display: block\" target=\"_blank\">" + quickRemark + "<span style=\"font-size: 13px; display: inline-block; color: #259\">&nbsp;Click to view</span></a></td>" +
                                                                                                 "<td width=\"35\" class=\"zhwd-mobile-hide\">&nbsp;</td>" +
                                                                                             "</tr>" +
                                                                                         "</tbody>" +
                                                                                   "</table>" +
                                                                                 "</td>" +
                                                                             "</tr>" +
                                                                             "<tr>" +
                                                                                 "<td height=\"28\" style=\"font-size: 0px; line-height: 0px\">&nbsp;</td>" +
                                                                             "</tr>" +
                                                                         "</tbody>" +
                                                                     "</table>" +
                                                                 "</td>" +
                                                             "</tr>" +
                                                         "</tbody>" +
                                                     "</table>" +
                                                 "</td>" +
                                             "</tr>";

            return content;
        }

        /// <summary>
        /// 用户组邮件模板
        /// </summary>
        /// <param name="subTitle">二级标题</param>
        /// <param name="textTitle">内容标题</param>
        /// <param name="text">内容</param>
        /// <param name="url">访问路径</param>
        /// <returns>邮件内容</returns>
        public static string GeneratorSubscribeMailContent(string subTitle, List<string> folders, string url)
        {
            string content = "<tr>" +
                                  "<td height=\"15\" style=\"font-size: 0px; line-height: 0px\">&nbsp;</td>" +
                             "</tr>" +
                             "<tr>" +
                                 "<td>" +
                                     "<table border=\"0\" style=\"min-width:800px;\" cellpadding=\"0\" cellspacing=\"0\">" +
                                         "<tbody>" +
                                             "<tr>" +
                                                 "<td>" +
                                                     "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\">" +
                                                         "<tbody>" +
                                                             "<tr>" +
                                                                 "<td><a href=\"" + url + "\" style=\"font-size: 18px; font-weight: bold; text-decoration: none; color: #259; border: none; outline: none\" target=\"_blank\">" + subTitle + "</a></td>" +
                                                             "</tr>" +
                                                             "<tr>" +
                                                                 "<td>" +
                                                                     "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" class=\"zhwd-mobile-no-fullwidth\">" +
                                                                        "<tbody>" +
                                                                        "<tr><td align=\"left\" height=\"10\" colspan=\"5\" width=\"100%\"></td></tr>" +
                                                                        "<tr><td width=\"80%\" colspan=\"4\" align=\"left\"><span style=\"font-size:14px;\">Total Count</span></td>" +
                                                                        "<td align=\"right\" width=\"20%\"><span><strong>" + folders.Count + "</strong></span></td></tr>" +
                                                                        "<tr><td align=\"left\" height=\"10\" colspan=\"5\" width=\"100%\"></td></tr>" +
                                                                        GetContent(folders) +
                                                                        "<tr><td height=\"20\" colspan=\"5\" width=\"100%\"></td></tr>" +
                                                                        "</tbody>" +
                                                                     "</table>" +
                                                                 "</td>" +
                                                             "</tr>" +
                                                             "<tr>" +
                                                                 "<td>" +
                                                                     "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\">" +
                                                                         "<tbody>" +
                                                                             "<tr>" +
                                                                                 "<td><a href=\"" + url + "\" style=\"font-size: 14px;line-height: 22px; text-decoration: none; color: #333; display: block\" target=\"_blank\"><span style=\"font-size: 14px; display: inline-block; color: #259\">...Click to view</span></a></td>" +
                                                                                 "<td width=\"15\" class=\"zhwd-mobile-hide\">&nbsp;</td>" +
                                                                             "</tr>" +
                                                                         "</tbody>" +
                                                                   "</table>" +
                                                                 "</td>" +
                                                             "</tr>" +
                                                             "<tr>" +
                                                                 "<td height=\"15\" style=\"font-size: 0px; line-height: 0px\">&nbsp;</td>" +
                                                             "</tr>" +
                                                         "</tbody>" +
                                                     "</table>" +
                                                 "</td>" +
                                             "</tr>" +
                                         "</tbody>" +
                                     "</table>" +
                                 "</td>" +
                             "</tr>";

            return content;
        }

        /// <summary>
        /// 获取用户组列表的tr
        /// </summary>
        /// <param name="groups"></param>
        /// <returns></returns>
        private static string GetContent(List<string> folders)
        {
            string content = "";
            var g = folders.GroupBy(i => i);
            foreach (var item in g)
            {
                content += "<tr><td width=\"80%\" colspan=\"4\" align=\"left\"><a href=\"" + item.Key + "\" style=\"font-size:14px;\">" + item.Key + "</a></td>" +
                               "<td align=\"right\" width=\"20%\"><span><strong>" + item.Count() + "</strong></span></td></tr>";
            }
            return content;
        }

        /// <summary>
        /// 初始化外发邮箱账户、密码、SMTP地址
        /// </summary>
        /// <param name="mMailFrom">外发邮箱</param>
        /// <param name="mMailPwd">邮箱密码</param>
        /// <param name="mMailSMTP">SMTP地址</param>
        public EmailHelper(string mMailFrom, string mMailPwd, string mMailSMTP)
        {
            this.mailFrom = mMailFrom;
            this.mailPwd = mMailPwd;
            this.host = mMailSMTP;
        }

        /// <summary>
        /// 邮件发送
        /// </summary>
        /// <returns>发送结果</returns>
        public bool Send(Guid siteId, SPUser cUser)
        {
            try
            {
                if (mailToArray.Count > CommonHelper.mailToMaxCount)
                {
                    return false;
                }
                //使用指定的邮件地址初始化MailAddress实例
                MailAddress maddr = new MailAddress(mailFrom, "GKS Admin", Encoding.UTF8);
                //初始化MailMessage实例
                MailMessage myMail = new MailMessage();

                //向收件人地址集合添加邮件地址
                if (mailToArray != null && mailToArray.Count > 0)
                {
                    for (int i = 0; i < mailToArray.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(mailToArray[i]))
                            myMail.To.Add(mailToArray[i]);
                    }
                }

                //发件人地址
                myMail.From = maddr;
                //电子邮件的标题
                myMail.Subject = mailSubject;
                //电子邮件的主题内容使用的编码
                myMail.SubjectEncoding = Encoding.UTF8;
                //电子邮件正文
                myMail.Body = mailBody;
                //电子邮件正文的编码
                myMail.BodyEncoding = Encoding.UTF8;
                myMail.Priority = MailPriority.Normal;
                myMail.IsBodyHtml = isbodyHtml;
                //SmtpClient smtp = new SmtpClient(host);
                SmtpClient smtp = new SmtpClient(host, CommonHelper.mailPort);
                //smtp.EnableSsl = true;
                //smtp.UseDefaultCredentials = false;
                //指定发件人的邮件地址和密码以验证发件人身份
                smtp.Credentials = new System.Net.NetworkCredential(mailFrom, mailPwd);
                //设置SMTP邮件服务器
                //smtp.Host = host;

                //将邮件发送到SMTP邮件服务器
                smtp.Send(myMail);

                return true;
            }
            catch (SmtpException ex)
            {
                CommonHelper.SetErrorLog("EmailHelper.cs__Send", "Message:" + ex.Message + ",StatusCode:" + ex.StatusCode, siteId, cUser);
                return false;
            }
        }

        /// <summary>
        /// 邮件发送
        /// </summary>
        /// <returns>发送结果</returns>
        public bool Send()
        {
            try
            {
                //使用指定的邮件地址初始化MailAddress实例
                MailAddress maddr = new MailAddress(mailFrom, "GKS Admin", Encoding.UTF8);
                //初始化MailMessage实例
                MailMessage myMail = new MailMessage();

                //向收件人地址集合添加邮件地址
                if (mailToArray != null && mailToArray.Count > 0)
                {
                    for (int i = 0; i < mailToArray.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(mailToArray[i]))
                            myMail.To.Add(mailToArray[i]);
                    }
                }

                //发件人地址
                myMail.From = maddr;
                //电子邮件的标题
                myMail.Subject = mailSubject;
                //电子邮件的主题内容使用的编码
                myMail.SubjectEncoding = Encoding.UTF8;
                //电子邮件正文
                myMail.Body = mailBody;
                //电子邮件正文的编码
                myMail.BodyEncoding = Encoding.UTF8;
                myMail.Priority = MailPriority.Normal;
                myMail.IsBodyHtml = isbodyHtml;
                //SmtpClient smtp = new SmtpClient(host, 587);
                SmtpClient smtp = new SmtpClient(host, CommonHelper.mailPort);
                //smtp.EnableSsl = true;
                //smtp.UseDefaultCredentials = false;
                //指定发件人的邮件地址和密码以验证发件人身份
                smtp.Credentials = new System.Net.NetworkCredential(mailFrom, mailPwd);
                //设置SMTP邮件服务器
                //smtp.Host = host;

                //将邮件发送到SMTP邮件服务器
                smtp.Send(myMail);

                return true;
            }
            catch (SmtpException ex)
            {
                return false;
            }
        }
    }
}
