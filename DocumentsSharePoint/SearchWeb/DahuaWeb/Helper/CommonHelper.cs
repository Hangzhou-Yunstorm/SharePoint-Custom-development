using Microsoft.SharePoint.Client;
using System;
using System.IO;
using System.Net;

namespace DahuaWeb
{
    public static class CommonHelper
    {
        /// <summary>
        /// 获取WebClient
        /// </summary>
        /// <returns>WebClient</returns>
        public static WebClient GetWebClient()
        {
            WebClient webclient = new WebClient();
            webclient.Credentials = new NetworkCredential(Constant.loginName, Constant.psw, Constant.domain); ;

            return webclient;
        }

        /// <summary>
        /// 获取ClientContext
        /// </summary>
        /// <returns>ClientContext</returns>
        public static ClientContext GetClientContext()
        {
            ClientContext context = new ClientContext(Constant.webUrl);
            context.Credentials = new NetworkCredential(Constant.loginName, Constant.psw, Constant.domain);

            return context;
        }

        /// <summary>
        /// 获取ClientContext
        /// </summary>
        /// <returns>ClientContext</returns>
        public static ClientContext GetGKSClientContext()
        {
            ClientContext context = new ClientContext("https://gks.dahuasecurity.com/");
            context.Credentials = new NetworkCredential("spadmin", "Dh$2017-GVJ2J", "dahuagks");

            return context;
        }

        /// <summary>
        /// 获取Utc时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeSpanByDate()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var time = Convert.ToInt64(ts.TotalSeconds).ToString();
            return time;
        }

        /// <summary>
        /// 根据时间戳获取时间
        /// </summary>
        /// <param name="date">时间戳</param>
        /// <returns>时间</returns>
        public static DateTime GetDateByTimeSpan(string date)
        {
            DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            DateTime dt = startTime.AddSeconds(long.Parse(date));

            return dt;
        }

        /// <summary>
        /// 删除文件夹下过去一天的zip文件
        /// </summary>
        /// <param name="folder">文件夹路径</param>
        public static void DeleteZipFile(string folder)
        {
            try
            {
                DirectoryInfo directory = new DirectoryInfo(folder);
                if (directory.Exists)
                {
                    FileInfo[] zipFiles = directory.GetFiles();

                    if (zipFiles != null && zipFiles.Length > 0)
                    {
                        for (int m = 0; m < zipFiles.Length; m++)
                        {
                            FileInfo zFile = zipFiles[m];
                            if (zFile.Exists)
                            {
                                string ext = Path.GetExtension(zFile.Name).ToLower();

                                TimeSpan ts = new TimeSpan(0, 6, 0, 0);
                                if (DateTime.Now.ToLocalTime() - zFile.LastWriteTime > ts && ext == ".zip")
                                {
                                    zFile.Delete();
                                    //File.Delete(zFile.FullName);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 判断文件是否加密
        /// </summary>
        /// <param name="item">SPListItem</param>
        /// <returns>是否加密</returns>
        public static bool IsRMS(ListItem item)
        {
            try
            {
                string ext = Path.GetExtension(item.DisplayName).ToLower();
                if (ext == ".docx" || ext == ".pptx" || ext == ".xlsx")
                {
                    var fc = Convert.ToBoolean(item["IsFullControl"]);
                    var or = Convert.ToBoolean(item["IsRead"]);
                    var ass = Convert.ToBoolean(item["IsPrint"]);
                    var ap = Convert.ToBoolean(item["IsSave"]);
                    var ae = Convert.ToBoolean(item["IsEdit"]);
                    //判断是否加密
                    if (fc || or || ass || ap || ae)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}