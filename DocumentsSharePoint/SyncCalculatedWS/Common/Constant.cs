using System;
using System.Configuration;

namespace SyncCalculatedWS
{
    public class Constant
    {
        /// <summary>
        /// 文件统计详情列表
        /// </summary>
        public static string documentStatisticalDetails = ConfigurationManager.AppSettings["DocumentStatisticalDetails"];

        /// <summary>
        /// 文件计算列表
        /// </summary>
        public static string documentCalculated = ConfigurationManager.AppSettings["DocumentCalculated"];

        /// <summary>
        /// 文件评分列表
        /// </summary>
        public static string scoreList = ConfigurationManager.AppSettings["ScoreList"];

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
        /// 12H
        /// </summary>
        public static double TimeSpan = ConfigurationManager.AppSettings["TimeSpan"] == null ? 43200 : double.Parse(ConfigurationManager.AppSettings["TimeSpan"]);

    }
}