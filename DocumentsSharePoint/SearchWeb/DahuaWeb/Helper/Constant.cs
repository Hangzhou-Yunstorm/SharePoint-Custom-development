using System.Configuration;

namespace DahuaWeb
{
    public class Constant
    {
        /// <summary>
        /// 当前网站地址
        /// </summary>
        public static string webSiteUrl = ConfigurationManager.AppSettings["WebSiteUrl"];

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
        /// 文档列表名
        /// </summary>
        public static string documents = "Documents";

        /// <summary>
        /// 加密文档列表名
        /// </summary>
        public static string rmsDocuments = "RMSDocuments";

        /// <summary>
        /// 加密解密key
        /// </summary>
        public static string key = "dahuakey";
    }
}