using System;
using System.Configuration;

namespace SyncGroups
{
    public class Constant
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        //public static string Connection = @"server=10.1.40.201;database=DHDATAMID;user id=DHGKSSA;password=DHGKSSA_321";
        public static string Connection_C = "82DF82DC417EF9303CE14979065207414122FADB33190A7C592A33577294783CA7C5E7D0EA98437BC2382EED165CCE04DE169F6BAEE0E4B0E2C286E652316AB0DA57D6E3DDCEFA0E34DBA1082DFCDAE6";
        public static string Connection = ConfigurationManager.AppSettings["ConnectSql"] == null ? Connection_C : ConfigurationManager.AppSettings["ConnectSql"];

        /// <summary>
        /// 全员组名
        /// </summary>
        public static string AllUserGroup = "GKS-AllUsers";

        /// <summary>
        /// 邮件接收人账号
        /// </summary>
        public static string mailTo = ConfigurationManager.AppSettings["MailTo"];

        /// <summary>
        /// 邮件发送人账号
        /// </summary>
        public static string mailUser = ConfigurationManager.AppSettings["MailUser"];

        /// <summary>
        /// 邮件发送人密码
        /// </summary>
        public static string mailPwd = ConfigurationManager.AppSettings["MailPwd"];

        /// <summary>
        /// 邮箱smtp地址
        /// </summary>
        public static string mailSmtp = ConfigurationManager.AppSettings["MailSmtp"];

        /// <summary>
        /// 邮箱smtp端口
        /// </summary>
        public static int mailPort = Convert.ToInt32(ConfigurationManager.AppSettings["MailPort"]);

        /// <summary>
        /// 海外销售中心ID
        /// </summary>
        public static string rootDepartId = ConfigurationManager.AppSettings["RootId"];

        /// <summary>
        /// 海外销售中心-大欧洲区ID
        /// </summary>
        public static string europeDepartId = ConfigurationManager.AppSettings["EuropeId"];

        /// <summary>
        /// 海外销售中心-产品部下部门Id集合
        /// </summary>
        public static string productSubIds = ConfigurationManager.AppSettings["ProductSubIds"];

        /// <summary>
        /// 网站地址
        /// </summary>
        public static string webUrl = ConfigurationManager.AppSettings["WebUrl"];

        /// <summary>
        /// 登录名
        /// </summary>
        public static string loginName = ConfigurationManager.AppSettings["LoginName"];

        /// <summary>
        /// 密码
        /// </summary>
        public static string psw = ConfigurationManager.AppSettings["Password"];

        /// <summary>
        /// 域名
        /// </summary>
        public static string domain = ConfigurationManager.AppSettings["Domain"];


        /// <summary>
        /// 域名用户
        /// </summary>
        public static string domainuser = ConfigurationManager.AppSettings["DomainUser"];

        /// <summary>
        /// Ensure
        /// </summary>
        public static string enSure = "i:0#.w|" + domainuser + "\\";

        /// <summary>
        /// 运行时间
        /// </summary>
        public static DateTime RunTime = DateTime.MinValue;

        /// <summary>
        /// 24H
        /// </summary>
        public static double TimeSpan = ConfigurationManager.AppSettings["TimeSpan"] == null ? 86400 : double.Parse(ConfigurationManager.AppSettings["TimeSpan"]);

    }
}