using System;
using System.Configuration;

namespace SyncExternalUserWS
{
    public class Constant
    {
        /// <summary>
        /// 外部人员列表名
        /// </summary>
        public static string externalList = ConfigurationManager.AppSettings["ExternalList"];

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
        /// 运行时间
        /// </summary>
        public static DateTime RunTime = DateTime.MinValue;

        /// <summary>
        /// 24H
        /// </summary>
        public static double TimeSpan = ConfigurationManager.AppSettings["TimeSpan"] == null ? 86400 : double.Parse(ConfigurationManager.AppSettings["TimeSpan"]);

        /// <summary>
        /// 上次更新时间文件路径
        /// </summary>
        public static string LastTimePath = ConfigurationManager.AppSettings["LastTimePath"];

        /// <summary>
        /// 上次更新时间文件路径
        /// </summary>
        public static string GlobleWebSync = ConfigurationManager.AppSettings["GlobleWebSync"];

        /// <summary>
        /// 加密key
        /// </summary>
        public static string SCKey = ConfigurationManager.AppSettings["SCKey"];
    }
}