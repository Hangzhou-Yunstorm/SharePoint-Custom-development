using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace SyncGroups
{
    class EmailHelper
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
        /// 获取用户组列表的tr
        /// </summary>
        /// <param name="groups"></param>
        /// <returns></returns>
        private static string GetContent(List<string> groups)
        {
            string content = "";
            foreach (string group in groups)
            {
                content += "<tr><td width=\"20%\" align=\"left\"><span style=\"font-size:14px;\">Group name:</span></td>" +
                               "<td align=\"left\" colspan=\"4\" width=\"80%\"><span><strong>" + group + "</strong></span></td></tr>";
            }
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
        public static string GeneratorGroupMailContent(string subTitle, List<string> groups, string url)
        {
            string content = "<tr>" +
                                  "<td height=\"15\" style=\"font-size: 0px; line-height: 0px\">&nbsp;</td>" +
                             "</tr>" +
                             "<tr>" +
                                 "<td>" +
                                     "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\">" +
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
                                                                        "<tr><td width=\"20%\" align=\"left\"><span style=\"font-size:14px;\">Group's count:</span></td>" +
                                                                        "<td align=\"left\" colspan=\"4\" width=\"80%\"><span><strong>" + groups.Count + "</strong></span></td></tr>" +
                                                                        "</tbody>" +
                                                                     "</table>" +
                                                                 "</td>" +
                                                             "</tr>" +
                                                             "<tr>" +
                                                                 "<td>" +
                                                                     "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" class=\"zhwd-mobile-no-fullwidth\">" +
                                                                        "<tbody>" +
                                                                        "<tr><td align=\"left\" height=\"10\" colspan=\"5\" width=\"100%\"></td></tr>" +
                                                                         GetContent(groups) +
                                                                        "<tr><td height=\"30\" colspan=\"5\" width=\"100%\"></td></tr></tbody>" +
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
        /// 用户组邮件模板
        /// </summary>
        /// <param name="subTitle">二级标题</param>
        /// <param name="textTitle">内容标题</param>
        /// <param name="text">内容</param>
        /// <param name="url">访问路径</param>
        /// <returns>邮件内容</returns>
        public static string GeneratorGroupErrorContent(string subTitle, string msg, string url)
        {
            string content = "<tr>" +
                                  "<td height=\"15\" style=\"font-size: 0px; line-height: 0px\">&nbsp;</td>" +
                             "</tr>" +
                             "<tr>" +
                                 "<td>" +
                                     "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\">" +
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
                                                                        "<tr><td width=\"100%\" align=\"left\"><span style=\"font-size:14px;\">" + msg + "</span></td></tr>" +
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
                SmtpClient smtp = new SmtpClient(host, Constant.mailPort);
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
