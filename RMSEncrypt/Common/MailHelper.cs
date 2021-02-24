using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common
{
    public class MailHelper
    {
        /// <summary>
        /// 发送者
        /// </summary>
        public string mailFrom { get; set; }

        /// <summary>
        /// 收件人
        /// </summary>
        public string[] mailToArray { get; set; }

        /// <summary>
        /// 抄送
        /// </summary>
        public string[] mailCcArray { get; set; }

        /// <summary>
        /// 密送
        /// </summary>
        public string[] mailBccArray { get; set; }
        
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
            int tbWidth = 860;
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
                                                                       "<div style=\"border-radius:10px;width:590px;height:70px;margin:0 auto;\"><div style=\"width:590px;height:70px;border-radius:10px;color:#fff;margin:0 auto;background-color:#0e60ac;\">" +
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
        /// 审批提醒邮件模板
        /// </summary>
        /// <param name="author">文件作者</param>
        /// <param name="time">上传/修改时间</param>
        /// <param name="folderPath">文件目录</param>
        /// <param name="description">文档描述</param>
        /// <param name="url">审批地址</param>
        /// <returns></returns>
        public static string GeneratorApproveMailContent(string docTitle, string author, string time, string folderPath, string description, string url)
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
                                                                                        "<tr><td width=\"15%\" align=\"left\"><span style=\"font-size:14px;width:50px;\">" + authorTitle + "</span></td>" +
                                                                                        "<td align=\"left\" style=\"word-wrap:break-word;\" width=\"35%\"><a style=\"font-size:14px;color:#333333;text-decoration:none\" target=\"_blank\"><strong>" + author + "</strong></a></td>" +
                                                                                        "<td width=\"15%\" align=\"left\"><span style=\"font-size:14px\">" + docUpdatedTitle + "</span></td>" +
                                                                                        "<td align=\"left\" width=\"35%\" colspan=\"2\"><a style=\"font-size:14px;color:#333333;text-decoration:none;\" target=\"_blank\">" + time + "</a></td></tr>" +
                                                                                        "<tr><td align=\"left\" height=\"10\" colspan=\"5\" width=\"100%\"></td></tr><tr><td width=\"15%\" align=\"left\"><span style=\"font-size:14px\">" + fileFolderTitle + "</span></td>" +
                                                                                        "<td align=\"left\" colspan=\"4\" style=\"word-wrap:break-word;\" width=\"85%\"><a style=\"font-size:14px;color:#333333;text-decoration: none;\" target=\"_blank\"><strong>" + folderPath + "</strong></a></td></tr>" +
                                                                                        "<tr><td height=\"30\" colspan=\"5\" width=\"100%\"></td></tr></tbody>" +
                                                                                     "</table>" +
                                                                                 "</td>" +
                                                                             "</tr>" +
                                                                             "<tr>" +
                                                                                 "<td>" +
                                                                                     "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\">" +
                                                                                         "<tbody>" +
                                                                                             "<tr>" +
                                                                                                 "<td style=\"word-break: break-all\"><a href=\"" + url + "\" style=\"font-size: 13px;margin:15px 0; line-height: 22px; text-decoration: none; color: #333; display: block\" target=\"_blank\">" + quickRemark + "<span style=\"font-size: 13px; display: inline-block; color: #259\">&nbsp;点击前往审批</span></a></td>" +
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
        /// 其他简单文本通知模板///需要自定义报错与审批结果反馈
        /// </summary>
        /// <param name="author">文章作者</param>
        /// <param name="reader">评论读者</param>
        /// <param name="docTitle">文章标题</param>
        /// <param name="url">文章链接</param>
        /// <returns></returns>
        public static string GeneratorEvaluateNotification(string author, string reader, string docTitle, string url, string errorDescription, string time)
        {
            string content =
                "<span>" + author + "，您好</span>" +
                "<br/><br/>" +
                "GKS平台，<span style='color:#0094ff;font-weight:bold'>" + reader + "</span>在您的文件：《" + docTitle + "》中发现错误，错误描述：<br/><br/>" + errorDescription + "<a href='" + url + "'>点击查看 </a><br/><br/>此邮件由 知识管理平台 自动发出，请勿回复";
            return content;
        }

        /// <summary>
        /// 简单通知模板
        /// </summary>
        /// <param name="author">作者</param>
        /// <param name="docTitle">文件名</param>
        /// <param name="uploadTime">上传时间</param>
        /// <param name="folderPath">文件路径</param>
        /// <returns></returns>
        public static string GeneratorSimpleNotification(string author, string docTitle, string uploadTime, string errorMsg)
        {
            string content =
                "<span>Hi," + author + "</span>" +
                "<br/><br/>" +
                "The file <" + docTitle + "> you uploaded at " + uploadTime + ",<span style='color:#0094ff;font-weight:bold'>RMS encrypted failed.</span>Therefore the file probably was not encrypted or it was not allowed to download because it was under encryoting.<br/><br/>Error Message:" + errorMsg + "<br/><br/> please try again,or you can encrypt it in your local PC,and then upload again</a><br/><br/>Mail From Global Knowledge System,please do not reply";
            return content;
        }


        /// <summary>
        /// 初始化外发邮箱账户、密码、SMTP地址
        /// </summary>
        /// <param name="mMailFrom">外发邮箱</param>
        /// <param name="mMailPwd">邮箱密码</param>
        /// <param name="mMailSMTP">SMTP地址</param>
        public MailHelper(string mMailFrom, string mMailPwd, string mMailSMTP)
        {
            this.mailFrom = mMailFrom;
            this.mailPwd = mMailPwd;
            this.host = mMailSMTP;
        }
        public bool Send(string type)
        {
            //使用指定的邮件地址初始化MailAddress实例
            MailAddress maddr = new MailAddress(mailFrom, "知识管理平台", Encoding.UTF8);

            //初始化MailMessage实例
            MailMessage myMail = new MailMessage();


            //向收件人地址集合添加邮件地址
            if (mailToArray != null)
            {
                for (int i = 0; i < mailToArray.Length; i++)
                {
                    if (!string.IsNullOrEmpty(mailToArray[i]))
                        myMail.To.Add(mailToArray[i].ToString());
                }
            }


            //向抄送收件人地址集合添加邮件地址
            if (mailCcArray != null)
            {
                for (int i = 0; i < mailCcArray.Length; i++)
                {
                    myMail.CC.Add(mailCcArray[i].ToString());
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
            myMail.BodyEncoding = Encoding.Default;

            myMail.Priority = MailPriority.Normal;

            myMail.IsBodyHtml = isbodyHtml;



            SmtpClient smtp = new SmtpClient(host, 25);
            //smtp.EnableSsl = true;
            //smtp.UseDefaultCredentials = false;
            //指定发件人的邮件地址和密码以验证发件人身份
            smtp.Credentials = new System.Net.NetworkCredential(mailFrom, mailPwd);


            //设置SMTP邮件服务器
            //smtp.Host = host;

            try
            {
                //将邮件发送到SMTP邮件服务器
                smtp.Send(myMail);
                return true;

            }
            catch (System.Net.Mail.SmtpException ex)
            {
                return false;
            }

        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="errorMessage"></param>
        private void SetMailLog(string errorMessage)
        {
            XMLHelper xh = new XMLHelper();
            string path = System.Windows.Forms.Application.StartupPath + "\\App_Data\\LogFile\\";
            ErrorMessageModel emm = new ErrorMessageModel();
            emm.Content = errorMessage;
            emm.Time = DateTime.Now.ToString();
            emm.Title = "邮件发送";
            emm.FilePath = "无";
            xh.CreateLog(emm, path);
        }

        public bool Send()
        {
            //393231120@qq.com urjfruwlrfznbjda    smtp.qq.com
            //使用指定的邮件地址初始化MailAddress实例
            MailAddress maddr = new MailAddress(mailFrom, "GKS Admin", Encoding.UTF8);

            //初始化MailMessage实例
            MailMessage myMail = new MailMessage();


            //向收件人地址集合添加邮件地址
            if (mailToArray != null)
            {
                for (int i = 0; i < mailToArray.Length; i++)
                {
                    if (!string.IsNullOrEmpty(mailToArray[i]))
                        myMail.To.Add(mailToArray[i].ToString());
                }
            }


            //向抄送收件人地址集合添加邮件地址
            if (mailCcArray != null)
            {
                for (int i = 0; i < mailCcArray.Length; i++)
                {
                    myMail.CC.Add(mailCcArray[i].ToString());
                }
            }


            //向密送收件人地址集合添加邮件地址
            if (mailBccArray != null)
            {
                for (int i = 0; i < mailBccArray.Length; i++)
                {
                    myMail.Bcc.Add(mailBccArray[i].ToString());
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
            myMail.BodyEncoding = Encoding.Unicode;

            myMail.Priority = MailPriority.Normal;

            myMail.IsBodyHtml = isbodyHtml;



            SmtpClient smtp = new SmtpClient(host, 25);
            //smtp.EnableSsl = true;
            //smtp.UseDefaultCredentials = false;
            //指定发件人的邮件地址和密码以验证发件人身份
            smtp.Credentials = new System.Net.NetworkCredential(mailFrom, mailPwd);


            //设置SMTP邮件服务器
            //smtp.Host = host;

            try
            {
                ////将邮件发送到SMTP邮件服务器
                //if (myMail.To.Count > 0)
                //    smtp.Send(myMail);
                return true;

            }
            catch (System.Net.Mail.SmtpException ex)
            {
                //MessageBox.Show(ex.Message);
                SetMailLog(ex.Message);
                return false;
            }

        }

        public static MailHelper GetMailHelper(string authorMail, string author, string docTitle, string uploadTime, string errorMsg)
        {
            string smtpAddress = ConfigurationSettings.AppSettings["SMTP"].ToString();
            string outbandEmail = ConfigurationSettings.AppSettings["GKSUserAccount"].ToString();
            string outbandMailPass = ConfigurationSettings.AppSettings["GKSUserAccountPass"].ToString();
            string adminMail = ConfigurationSettings.AppSettings["AdminMail"].ToString();


            string[] bccAdminMail=new string[] { };

            if (adminMail.Contains(";"))
                bccAdminMail = adminMail.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            MailHelper email = new MailHelper(outbandEmail, outbandMailPass, smtpAddress);
            email.mailSubject = "RMS Encrypt File Failed - In GKS";
            string mailContent = MailHelper.GeneratorSimpleNotification(author, docTitle, uploadTime, errorMsg);
            email.mailBody = mailContent;
            email.isbodyHtml = true;    //是否是HTML
            email.mailToArray = new string[] { authorMail, };//接收者邮件集合
            email.mailBccArray = bccAdminMail;

            return email;
        }

    }
}
