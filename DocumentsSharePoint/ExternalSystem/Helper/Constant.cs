using System.Configuration;

namespace ExternalSystem
{
    public class Constant
    {
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
        /// 外部人员列表名
        /// </summary>
        public static string externalList = "ExternalUserList";

        /// <summary>
        /// 外部人员Log
        /// </summary>
        public static string externalLogList = "ExternalLogs";

        /// <summary>
        /// 分享列表名
        /// </summary>
        public static string shareCatalogList = "ShareCatalogList";

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

        /// <summary>
        ///  区域默认文件夹名
        /// </summary>
        public static string regionDefault = ConfigurationManager.AppSettings["RegionDefault"];

        /// <summary>
        ///  国家默认文件夹名
        /// </summary>
        public static string countryDefault = ConfigurationManager.AppSettings["CountryDefault"];
    }
}