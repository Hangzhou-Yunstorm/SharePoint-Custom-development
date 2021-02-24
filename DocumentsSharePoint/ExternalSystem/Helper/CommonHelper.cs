using Microsoft.SharePoint.Client;
using System;
using System.IO;
using System.Net;

namespace ExternalSystem
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
        /// 验证链接并返回用户名
        /// </summary>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <param name="password"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool IsVerification(ref string user, string token, string password, ref string userName, ref string region, ref string country, ref bool isTimeOut)
        {
            bool isValid = false;
            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(password))
            {
                SymmCrypt symmCrypt = new SymmCrypt();

                try
                {
                    token = symmCrypt.DESDeCode(token, Constant.key, Constant.key);
                    if (token.Contains(Constant.key))
                    {
                        var timeSpan = token.Replace(Constant.key, "");
                        DateTime time = GetDateByTimeSpan(timeSpan);
                        if ((DateTime.UtcNow - time).TotalSeconds > 3600)
                        {
                            isTimeOut = true;
                        }
                        else
                        {
                            user = symmCrypt.DESDeCode(user, Constant.key, Constant.key);
                            password = symmCrypt.DESDeCode(password, Constant.key, Constant.key);

                            using (ClientContext context = CommonHelper.GetClientContext())
                            {
                                // The SharePoint web at the URL.
                                Web web = context.Web;
                                // 外部人员列表
                                var externalList = web.Lists.GetByTitle(Constant.externalList);
                                CamlQuery query = new CamlQuery();
                                query.ViewXml = "<View><Query><Where><Eq><FieldRef Name='Account' /><Value Type='Text'>" + user + "</Value></Eq></Where></View></Query>";

                                var items = externalList.GetItems(query);
                                context.Load(items);
                                context.ExecuteQuery();

                                if (items != null && items.Count > 0)
                                {
                                    ListItem updateItem = items[0];

                                    if (updateItem["PassWord"].ToString() == password)
                                    {
                                        if (updateItem["ObjectName"] != null)
                                        {
                                            userName = updateItem["ObjectName"].ToString();
                                        }
                                        else
                                        {
                                            userName = updateItem["Account"].ToString();
                                        }
                                        if (updateItem["Region"] != null)
                                        {
                                            region = ((FieldLookupValue)(updateItem["Region"])).LookupValue;
                                        }
                                        if (updateItem["Country"] != null)
                                        {
                                            country = ((FieldLookupValue)(updateItem["Country"])).LookupValue;
                                        }
                                        isValid = true;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        /// <summary>
        ///   验证登录并返回用户名
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="userName"></param>
        /// <param name="region"></param>
        /// <param name="country"></param>
        /// <returns></returns>
        public static bool IsVerification(ref string user, string password, ref string userName, ref string region, ref string country)
        {
            bool isValid = false;
            SymmCrypt symmCrypt = new SymmCrypt();
            try
            {
                user = symmCrypt.DESDeCode(user, Constant.key, Constant.key);
                password = symmCrypt.DESDeCode(password, Constant.key, Constant.key);

                using (ClientContext context = CommonHelper.GetClientContext())
                {
                    // The SharePoint web at the URL.
                    Web web = context.Web;
                    // 外部人员列表
                    var externalList = web.Lists.GetByTitle(Constant.externalList);
                    CamlQuery query = new CamlQuery();
                    query.ViewXml = "<View><Query><Where><Eq><FieldRef Name='Account' /><Value Type='Text'>" + user + "</Value></Eq></Where></View></Query>";

                    var items = externalList.GetItems(query);
                    context.Load(items);
                    context.ExecuteQuery();

                    if (items != null && items.Count > 0)
                    {
                        ListItem updateItem = items[0];

                        if (updateItem["PassWord"].ToString() == password)
                        {
                            if (updateItem["ObjectName"] != null)
                            {
                                userName = updateItem["ObjectName"].ToString();
                            }
                            else
                            {
                                userName = updateItem["Account"].ToString();
                            }
                            if (updateItem["Region"] != null)
                            {
                                region = ((FieldLookupValue)(updateItem["Region"])).LookupValue;
                            }
                            if (updateItem["Country"] != null)
                            {
                                country = ((FieldLookupValue)(updateItem["Country"])).LookupValue;
                            }
                            isValid = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isValid = false;
            }
            return isValid;
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
        /// 根据文件名获取文件类型图标路径
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>文件类型图标路径</returns>
        public static string GetIcon(string fileName)
        {
            string suffix = Path.GetExtension(fileName).ToLower();

            if (IsPicture(suffix))
            {
                suffix = ".picture";
            }
            else if (IsVideo(suffix))
            {
                suffix = ".video";
            }
            else if (IsHtml(suffix))
            {
                suffix = ".html";
            }

            switch (suffix)
            {
                case ".doc":
                case ".docx":
                    return "/Images/Icons/icdocx.png";

                case ".xls":
                case ".xlsx":
                case ".xlt":
                case ".xltm":
                case ".xla":
                case ".xlam":
                case ".xltx":
                case ".xlsb":
                case ".xlsm":
                    return "/Images/Icons/icxlsx.png";

                case ".ppt":
                case ".pptx":
                case ".potm":
                case ".potx":
                case ".ppam":
                case ".pptm":
                    return "/Images/Icons/icpptx.png";

                case ".pdf":
                    return "/Images/Icons/icpdf.png";

                case ".xsn":
                    return "/Images/Icons/icxsn.png";

                case ".ppsdc":
                    return "/Images/Icons/icppsdc.png";

                case ".xps":
                    return "/Images/Icons/icxps.png";

                case ".one":
                case ".onp":
                case ".ont":
                    return "/Images/Icons/ICONE.gif";

                case ".picture":
                    return "/Images/Icons/icpng.gif";

                case ".xsd":
                    return "/Images/Icons/icxsd.gif";

                case ".xsl":
                    return "/Images/Icons/icxsl.gif";

                case ".vdw":
                case ".vdx":
                case ".vtx":
                case ".vst":
                case ".vstx":
                case ".vstm":
                case ".vsd":
                case ".vsdx":
                case ".vsdm":
                case ".visiogeneric":
                    return "/Images/Icons/icvdw.gif";

                case ".tif":
                case ".tiff":
                    return "/Images/Icons/ictif.gif";

                case ".video":
                    return "/Images/Icons/icwmv.gif";

                case ".txt":
                    return "/Images/Icons/ictxt.gif";

                case ".xml":
                    return "/Images/Icons/icxml.gif";

                case ".rtf":
                    return "/Images/Icons/icrtf.gif";

                case ".js":
                    return "/Images/Icons/icjs.gif";

                case ".master":
                    return "/Images/Icons/icmaster.gif";

                case ".css":
                    return "/Images/Icons/iccss.gif";

                case ".config":
                    return "/Images/Icons/icconfig.gif";

                case ".html":
                    return "/Images/Icons/ichtm.gif";

                case ".msi":
                    return "/Images/Icons/icmsi.gif";

                case ".vbs":
                case ".vbe":
                case ".wsf":
                    return "/Images/Icons/icvbs.gif";

                case ".vss":
                case ".vsl":
                case ".vssm":
                case ".vssx":
                case ".vsx":
                    return "/Images/Icons/icvss.gif";

                case ".zip":
                case ".rar":
                    return "/Images/Icons/iczip.gif";

                default:
                    return "/Images/Icons/icgen.gif";

            }
        }

        // 判断文件是否是图片
        public static bool IsPicture(string suffix)
        {
            string strFilter = ".jpeg|.gif|.jpg|.png|.bmp|.pic|.tiff|.ico|.iff|.lbm|.mag|.mac|.mpt|.opt|";
            char[] separtor = { '|' };
            string[] tempFileds = strFilter.Trim().Split(separtor);
            foreach (string str in tempFileds)
            {
                if (str.ToLower() == suffix)
                {
                    return true;
                }
            }
            return false;
        }


        // 判断文件是否是视频
        public static bool IsVideo(string suffix)
        {
            string strFilter = ".avi|.mp4|.wmv|.rmvb|.rm|.flash|.mid|.3gp|.asf|.vob|.dat|.flv|.mkv|.mpeg|.swf|";
            char[] separtor = { '|' };
            string[] tempFileds = strFilter.Trim().Split(separtor);
            foreach (string str in tempFileds)
            {
                if (str.ToLower() == suffix)
                {
                    return true;
                }
            }
            return false;
        }

        // 判断文件是否是视频
        public static bool IsHtml(string suffix)
        {
            string strFilter = ".htm|.html|.shtml|.cshtml|.jsp|.aspx|.asp|.php|.xhtml|.dhtml|.jspx|.mspx|.mpeg|.swf|";
            char[] separtor = { '|' };
            string[] tempFileds = strFilter.Trim().Split(separtor);
            foreach (string str in tempFileds)
            {
                if (str.ToLower() == suffix)
                {
                    return true;
                }
            }
            return false;
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

        /// <summary>
        /// 根据文件夹路径获取加密后的文件夹路径
        /// </summary>
        /// <param name="url">文件夹路径</param>
        /// <returns>加密后的文件夹路径</returns>
        public static string GetRMSFolderUrl(string url)
        {
            int index = url.IndexOf(Constant.documents);
            if (index > -1)
            {
                url = url.Remove(index, Constant.documents.Length).Insert(index, Constant.rmsDocuments);
            }
            return url;
        }

        /// <summary>
        /// 添加操作日志
        /// </summary>
        /// <param name="model">日志Model</param>
        public static void SetLog(LogModel model)
        {
            try
            {
                using (ClientContext context = GetClientContext())
                {
                    // The SharePoint web at the URL.
                    Web web = context.Web;
                    // 外部Log列表
                    var externalLogList = web.Lists.GetByTitle(Constant.externalLogList);

                    ListItemCreationInformation item = new ListItemCreationInformation();
                    var log = externalLogList.AddItem(item);
                    log["Title"] = model.Title;
                    log["ObjectName"] = model.ObjectName;
                    log["Operate"] = model.Operate;
                    log["Operator"] = model.Operator;
                    log["ServerIP"] = GetServerHostName();

                    log.Update();
                    context.ExecuteQuery();
                }
            }
            catch { }
        }

        /// <summary>
        /// 获取服务器机器名
        /// </summary>
        /// <returns></returns>
        public static string GetServerHostName()
        {
            string name = Dns.GetHostName();
            return name;
        }

    }
}