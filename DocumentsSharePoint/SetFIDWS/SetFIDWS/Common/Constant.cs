using System;
using System.Configuration;

namespace SetFIDWS
{
    public class Constant
    {
        /// <summary>
        /// 网站地址
        /// </summary>
        public static string webUrl = ConfigurationManager.AppSettings["WebUrl"];
    }
}