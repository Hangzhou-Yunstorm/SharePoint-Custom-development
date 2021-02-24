using System;
using System.Net;

namespace MiddleSystem
{
    public class CommonHelper
    {

        public static string webUrl = "https://gks.dahuasecurity.com";

        public static string GetDomainName(string cName)
        {
            string domain = string.Empty;

            var name = cName.ToLower();
            switch (name)
            {
                case "southeastasia":
                    domain = "https://SGapprove.dahuasecurity.com:4431";
                    break;
                case "westerneurope":
                    domain = "https://EUapprove.dahuasecurity.com:4431";
                    break;
                case "usa":
                    domain = "https://USapprove.dahuasecurity.com:4431";
                    break;
                case "brazil":
                    domain = "https://BAapprove.dahuasecurity.com:4431";
                    break;
                default:
                    domain = "http://ikernal-sp01";
                    break;
            }
            return domain;
        }

        /// <summary>
        /// 获取服务器机器名
        /// </summary>
        /// <returns></returns>
        public static string GetServerHostName()
        {
            string name = "";
            try
            {
                name = Dns.GetHostName();
            }
            catch (Exception ex)
            {
            }
            return name;
        }

        /// <summary>
        /// 获取国家名
        /// </summary>
        /// <returns>国家名</returns>
        public static string GetCountryName()
        {
            string country = string.Empty;
            string hostName = GetServerHostName();

            var host = hostName.ToUpper();
            switch (host)
            {
                case "GKS-SP05":
                    country = "SoutheastAsia";
                    break;
                case "GKS-SP06":
                    country = "WesternEurope";
                    break;
                case "GKS-SP03":
                    country = "USA";
                    break;
                case "GKS-SP04":
                    country = "Brazil";
                    break;
                default:
                    country = "Ikernal";
                    break;
            }

            return country;
        }
    }
}