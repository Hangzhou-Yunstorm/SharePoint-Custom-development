using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 公共方法
    /// </summary>
    public class Helper
    {
        /// <summary>
        /// Object转sp user对象
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static FieldUserValue ObjToFieldUserValue(object o)
        {
            try
            {
                if (o == null)
                {
                    return null;
                }
                else
                {
                    return (FieldUserValue)o;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Object转sp user[]对象
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static FieldUserValue[] ObjToFieldUserValues(object o)
        {
            try
            {
                if (o == null)
                {
                    return null;
                }
                else
                {
                    return (FieldUserValue[])o;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Object转sp user对象
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string GetFieldUserValueEmail(FieldUserValue o)
        {
            if (o == null)
            {
                return "";
            }
            else
            {
                return o.Email;
            }
        }

        /// <summary>
        /// Object转String
        /// </summary>
        /// <param name="o">Object</param>
        /// <returns>String</returns>
        public static string ObjToStr(object o)
        {
            try
            {
                if (o == null)
                {
                    return "";
                }
                else
                {
                    return o.ToString();
                }
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Object转Bool
        /// </summary>
        /// <param name="o">Object</param>
        /// <returns>Bool</returns>
        public static bool ObjToBool(object o)
        {
            try
            {
                if (o == null)
                {
                    return false;
                }
                else
                {
                    if (o.ToString().ToLower() == "false")
                    {
                        return false;
                    }
                    else if (o.ToString().ToLower() == "true")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Object转Int
        /// </summary>
        /// <param name="o">Object</param>
        /// <returns>Int</returns>
        public static int ObjToInt(object o)
        {
            if (o == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(o);
            }
        }

        /// <summary>
        /// Object判断并转为Email（String格式）
        /// </summary>
        /// <param name="o">Object</param>
        /// <returns>Email</returns>
        public static string ObjIsEmail(object o)
        {
            string email = ObjToStr(o);
            if (!String.IsNullOrEmpty(email))
            {
                if (email.IndexOf("@") > -1 || email.ToLower() == "everyone")
                {
                    return email;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 月份转换位完整月份
        /// </summary>
        /// <param name="month">月份</param>
        /// <returns>完整月份</returns>
        public static string MonthToFullMonth(int month)
        {
            if (month < 10)
            {
                return "0" + month;
            }
            else
            {
                return month.ToString();
            }
        }

        /// <summary>
        /// 从源文件的URL提取出文件目录
        /// </summary>
        /// <param name="FUrlSourcePath">源文件的URL</param>
        /// <returns>文件目录</returns>
        public static string UrlToPath(string FUrlSourcePath)
        {
            try
            {
                //http://sharepoint-yc01/Documents/A目录/大华海外营销资料管理平台需求规格说明书(1).docx
                string url = ConfigurationSettings.AppSettings["URL"];//http://sharepoint-yc01
                int index = url.IndexOf(url) + url.Length;
                string tempUrl = FUrlSourcePath.Substring(index);
                string sourcedocurl = ConfigurationSettings.AppSettings["SourceDocUrl"];//Documents
                index = tempUrl.IndexOf(sourcedocurl) + sourcedocurl.Length;
                tempUrl = tempUrl.Substring(index);
                index = tempUrl.LastIndexOf('/') + 1;
                tempUrl = tempUrl.Substring(0, index);
                return tempUrl;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
