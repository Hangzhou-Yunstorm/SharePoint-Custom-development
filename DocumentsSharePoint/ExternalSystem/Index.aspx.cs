using ICSharpCode.SharpZipLib.Zip;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;

namespace ExternalSystem
{
    public partial class Index : System.Web.UI.Page
    {
        /// <summary>
        /// 表格初始化数据
        /// </summary>
        public string fDatas = "[]";

        /// <summary>
        /// 左侧树
        /// </summary>
        public string treeFolderJson = "[]";

        /// <summary>
        /// 用户名
        /// </summary>
        public string userName = "";

        /// <summary>
        /// 用户
        /// </summary>
        public static string userAccount = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            string region = "";
            string country = "";

            // 单点登录
            var user = Request.QueryString["basic"];
            var password = Request.QueryString["system"];
            var token = Request.QueryString["token"];

            // 登录
            var loginuser = Request.QueryString["name"];
            var loginpsw = HttpContext.Current.Session[loginuser];

            bool isLoginSuccess = false;

            if (string.IsNullOrEmpty(user))
            {
                if (loginpsw == null)
                {
                    Response.Write("<script>window.location.href='/Login.aspx';</script>");
                    return;
                }
                else if (password != loginpsw.ToString())
                {
                    Response.Write("<script>alert('Login fail，account or password is Incorrect !');window.location.href='/Login.aspx';</script>");
                    return;
                }
                else
                {
                    bool isSure = CommonHelper.IsVerification(ref loginuser, password, ref userName, ref region, ref country);
                    if (!isSure || string.IsNullOrEmpty(userName))
                    {
                        string url = "http://www.dahuasecurity.com/";
                        if (HttpContext.Current.Request.UrlReferrer != null)
                        {
                            url = HttpContext.Current.Request.UrlReferrer.ToString();
                        }
                        Response.Write("<script>alert('Login fail，account or password is Incorrect !');window.location.href='/Login.aspx';</script>");
                        return;
                    }
                    else
                    {
                        userAccount = loginuser;
                        isLoginSuccess = true;
                    }
                }
            }
            else
            {
                bool isTimeOut = false;
                bool isSure = CommonHelper.IsVerification(ref user, token, password, ref userName, ref region, ref country, ref isTimeOut);

                if (!isSure || string.IsNullOrEmpty(userName))
                {
                    string url = "http://www.dahuasecurity.com/";
                    if (HttpContext.Current.Request.UrlReferrer != null)
                    {
                        url = HttpContext.Current.Request.UrlReferrer.ToString();
                    }
                    if (isTimeOut)
                    {
                        Response.Write("<script>alert('Request timeout!');window.location.href='" + url + "';</script>");
                    }
                    else
                    {
                        Response.Write("<script>alert('Login fail，account or password is Incorrect !');window.location.href='" + url + "';</script>");
                    }
                    return;
                }
                else
                {
                    userAccount = user;
                    isLoginSuccess = true;
                }
            }
            if (isLoginSuccess)
            {
                LogModel model = new LogModel();
                model.Title = userAccount + " login system";
                model.Operate = "Login";
                model.Operator = userAccount;
                model.ObjectName = userAccount;
                CommonHelper.SetLog(model);

                List<TreeFolder> folders = new List<TreeFolder>();
                // 初始化数据
                fDatas = GetShareContent(folders, userAccount, region, country);

                if (folders.Count > 0)
                {
                    folders = folders.OrderByDescending(T => T.defaultType).ThenBy(P => P.name).ToList();
                    DataContractJsonSerializer json = new DataContractJsonSerializer(folders.GetType());
                    //序列化
                    using (MemoryStream stream = new MemoryStream())
                    {
                        json.WriteObject(stream, folders);
                        treeFolderJson = Encoding.UTF8.GetString(stream.ToArray());
                    }
                }
            }
        }

        /// <summary>
        /// 根据路径获取内容
        /// </summary>
        /// <returns>内容</returns>
        private string GetShareContent(List<TreeFolder> folders, string userAccount, string region, string country)
        {
            string content = "[]";
            try
            {
                using (ClientContext context = CommonHelper.GetClientContext())
                {
                    Web web = context.Web; // The SharePoint web at the URL.

                    // 分享库
                    var externalList = web.Lists.GetByTitle(Constant.shareCatalogList);
                    // 文档库
                    var documents = web.Lists.GetByTitle(Constant.documents);
                    // 条目集合
                    List<FileModel> lists = new List<FileModel>();
                    string pId = "0";

                    #region 区域默认文件夹
                    if (!string.IsNullOrEmpty(region))
                    {
                        try
                        {
                            var rFolder = web.GetFolderByServerRelativeUrl("/" + Constant.documents + "/Region/" + region + "/" + Constant.regionDefault);
                            context.Load(rFolder);
                            context.ExecuteQuery();

                            if (rFolder != null && rFolder.Exists)
                            {
                                var rItem = rFolder.ListItemAllFields;
                                context.Load(rItem);
                                context.ExecuteQuery();

                                var fieldValues = rItem.FieldValues;
                                // 获取文件属性值
                                string name = fieldValues["FileLeafRef"].ToString();
                                string hideName = name;
                                var created = Convert.ToDateTime(fieldValues["Created"]).ToString("yyyy-MM-dd");
                                var creator = ((FieldLookupValue)(fieldValues["Author"])).LookupValue;
                                string fId = fieldValues["FID"].ToString();
                                string path = fieldValues["FileRef"].ToString();

                                // 构造图标
                                string iconUrl = "<img src='/Images/Icons/folder.gif' onerror=\"javascript: this.src = '/Images/Icons/icgen.gif'\" />";
                                name = "<a onclick=\"OpenFolder('" + fId + "')\" title='" + name + "'>" + name + "</a>";
                                GetFoldes(context, rItem, pId, "system", "", "Never expire", folders, true, 1);
                                // 加入集合
                                lists.Add(GetModel(name, hideName, fId, pId, path, iconUrl, created, creator, "system", "", 1, true, "Never expire", "", 1));
                            }
                        }
                        catch { }
                        #region 国家默认文件夹
                        if (!string.IsNullOrEmpty(country))
                        {
                            try
                            {
                                var cFolder = web.GetFolderByServerRelativeUrl("/" + Constant.documents + "/Region/" + region + "/" + country + "/" + Constant.countryDefault);
                                context.Load(cFolder);
                                context.ExecuteQuery();

                                if (cFolder != null && cFolder.Exists)
                                {
                                    var cItem = cFolder.ListItemAllFields;
                                    context.Load(cItem);
                                    context.ExecuteQuery();

                                    var cfieldValues = cItem.FieldValues;
                                    // 获取文件属性值
                                    string name = cfieldValues["FileLeafRef"].ToString();
                                    string hideName = name;
                                    var created = Convert.ToDateTime(cfieldValues["Created"]).ToString("yyyy-MM-dd");
                                    var creator = ((FieldLookupValue)(cfieldValues["Author"])).LookupValue;
                                    string fId = cfieldValues["FID"].ToString();
                                    string path = cfieldValues["FileRef"].ToString();

                                    // 构造图标
                                    string iconUrl = "<img src='/Images/Icons/folder.gif' onerror=\"javascript: this.src = '/Images/Icons/icgen.gif'\" />";
                                    name = "<a onclick=\"OpenFolder('" + fId + "')\" title='" + name + "'>" + name + "</a>";
                                    GetFoldes(context, cItem, pId, "system", "", "Never expire", folders, true, 1);
                                    // 加入集合
                                    lists.Add(GetModel(name, hideName, fId, pId, path, iconUrl, created, creator, "system", "", 1, true, "Never expire", "", 1));
                                }
                            }
                            catch { }
                        }
                        #endregion
                    }
                    #endregion

                    #region 分享目录
                    // 获取本人没有过期的分享目录及文件
                    CamlQuery query = new CamlQuery();
                    string dString = SPUtility.CreateISO8601DateTimeFromSystemDateTime(DateTime.Now.ToLocalTime()); // 转换当前时间
                    query.ViewXml = "<View><Query><Where><And>" +
                                    "<Or><Gt><FieldRef Name='Expiration' /><Value Type='DateTime'>" + dString + "</Value></Gt><IsNull><FieldRef Name='Expiration' /></IsNull></Or>" +
                                    "<Eq><FieldRef Name='UserAccount' /><Value Type='Text'>" + userAccount + "</Value></Eq>" +
                                    "</And></Where></Query></View>";
                    var items = externalList.GetItems(query);
                    context.Load(items);
                    context.ExecuteQuery();

                    // 分享条目是否为空
                    if (items != null && items.Count > 0)
                    {
                        // 遍历集合，构造Model
                        foreach (ListItem item in items)
                        {
                            // 获取属性值
                            var sharer = ((FieldLookupValue)(item["Sharer"])).LookupValue;
                            var shareTime = item["Time"] == null ? "" : Convert.ToDateTime(item["Created"]).ToString("yyyy-MM-dd");
                            var fId = item["FolderId"] == null ? "" : item["FolderId"].ToString();
                            bool canWrite = item["CanWrite"] == null ? false : Convert.ToBoolean(item["CanWrite"]);
                            var expiration = item["Expiration"] == null ? "Never expire" : Convert.ToDateTime(item["Expiration"]).ToLocalTime().ToString("yyyy-MM-dd");

                            // 判断Id是否为空
                            if (!string.IsNullOrEmpty(fId))
                            {
                                CamlQuery fquery = new CamlQuery();
                                fquery.ViewXml = "<View Scope=\"RecursiveAll\"><Query><Where><Eq><FieldRef Name='FID' /><Value Type='Text'>" + fId + "</Value></Eq></Where></Query></View>";
                                // 获取文档库Item
                                var fItems = documents.GetItems(fquery);

                                context.Load(fItems);
                                context.ExecuteQuery();

                                if (fItems != null && fItems.Count > 0)
                                {
                                    ListItem fItem = fItems[0];

                                    // 类型图标
                                    string iconUrl = "";
                                    // 属性集合
                                    var fieldValues = fItem.FieldValues;

                                    // 获取文件属性值
                                    string name = fieldValues["FileLeafRef"].ToString();
                                    string hideName = name;
                                    var created = Convert.ToDateTime(fieldValues["Created"]).ToString("yyyy-MM-dd");
                                    var creator = ((FieldLookupValue)(fieldValues["Author"])).LookupValue;

                                    // 默认为文件
                                    int type = 0;
                                    string size = "";
                                    if (fItem.FileSystemObjectType == FileSystemObjectType.Folder) // 文件夹
                                    {
                                        // 文件夹
                                        type = 1;
                                        // 构造图标
                                        iconUrl = "<img src='/Images/Icons/folder.gif' onerror=\"javascript: this.src = '/Images/Icons/icgen.gif'\" />";
                                        name = "<a onclick=\"OpenFolder('" + fId + "')\" title='" + name + "'>" + name + "</a>";
                                        GetFoldes(context, fItem, pId, sharer, shareTime, expiration, folders, canWrite);
                                    }
                                    else // 文件
                                    {
                                        // 根据名称获取图标地址
                                        string imgUrl = CommonHelper.GetIcon(name);
                                        // 构造图标
                                        iconUrl = "<img src='" + imgUrl + "' onerror=\"javascript: this.src = '/Images/Icons/icgen.gif'\" />";

                                        // 文件说明
                                        string filedes = fieldValues["FileDescription"] == null ? "" : fieldValues["FileDescription"].ToString();
                                        if (string.IsNullOrEmpty(filedes))
                                        {
                                            filedes = "No file description.";
                                        }
                                        else if (filedes.Length > 255)
                                        {
                                            filedes = filedes.Substring(0, 255) + "...";
                                        }
                                        var file = fItem.File;
                                        context.Load(file);
                                        context.ExecuteQuery();

                                        size = Math.Round(Convert.ToDouble(file.Length) / 1024, 2) + "KB";

                                        name = "<a title='" + name + "' class=\"detail\" data-content=\"" + filedes + "\" >" + name + "</a>";
                                    }
                                    string path = fieldValues["FileRef"].ToString();
                                    lists.Add(GetModel(name, hideName, fId, pId, path, iconUrl, created, creator, sharer, shareTime, type, canWrite, expiration, size));
                                }
                            }
                        }
                    }
                    #endregion

                    if (lists.Count > 0)
                    {
                        // 排序，文件夹优先显示
                        //lists = lists.OrderByDescending(T => T.Type).ThenBy(P => P.HideName).ToList();
                        //lists = lists.OrderByDescending(T => T.Type).ToList();
                        lists = lists.OrderByDescending(T => T.DefaultType).ThenByDescending(T => T.Type).ThenBy(P => P.HideName).ToList();

                        //序列化列表
                        DataContractJsonSerializer json = new DataContractJsonSerializer(lists.GetType());
                        using (MemoryStream stream = new MemoryStream())
                        {
                            json.WriteObject(stream, lists);
                            content = Encoding.UTF8.GetString(stream.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                content = "[]";
            }
            return content;

        }

        private static FileModel GetModel(string name, string hideName, string fId, string pId, string path, string iconUrl, string created, string creator, string sharer, string shareTime, int type, bool canWrite, string expiration, string size, int defaultType = 0)
        {
            FileModel model = new FileModel();
            model.Name = name;
            model.HideName = hideName;
            model.ID = fId;
            model.PId = pId;
            model.Path = path;
            model.IconUrl = iconUrl;
            model.Created = created;
            model.Creator = creator;
            model.Sharer = sharer;
            model.ShareTime = shareTime;
            model.Type = type;
            model.CanWrite = canWrite;
            model.Expiration = expiration;
            model.Size = size;
            model.DefaultType = defaultType;

            return model;
        }

        /// <summary>
        /// 遍历获取树节点
        /// </summary>
        /// <param name="context">ClientContext</param>
        /// <param name="fItem">ListItem</param>
        /// <param name="pId">父节点Id</param>
        /// <param name="sharer">分享者</param>
        /// <param name="shareTime">分享时间</param>
        /// <param name="expiration">过期时间</param>
        /// <param name="folders">集合</param>
        private static void GetFoldes(ClientContext context, ListItem fItem, string pId, string sharer, string shareTime, string expiration, List<TreeFolder> folders, bool canWrite, int defaultType = 0)
        {
            try
            {
                TreeFolder tf = new TreeFolder();
                tf.fId = fItem.FieldValues["FID"].ToString();
                tf.name = fItem.FieldValues["FileLeafRef"].ToString();
                tf.path = fItem.FieldValues["FileRef"].ToString();
                tf.pId = pId;
                tf.id = tf.fId + pId;
                tf.sharer = sharer;
                tf.shareTime = shareTime;
                tf.canWrite = canWrite;
                tf.expiration = expiration;
                tf.defaultType = defaultType;

                var subFolders = fItem.Folder.Folders;
                context.Load(subFolders);
                context.ExecuteQuery();
                if (subFolders.Count > 0)
                {
                    tf.isParent = true;
                    //foreach (Folder folder in subFolders)
                    //{
                    //    var item = folder.ListItemAllFields;
                    //    context.Load(item);
                    //    context.ExecuteQuery();
                    //    GetFoldes(context, item, tf.UId, sharer, shareTime, expiration, folders, canWrite);
                    //}
                }

                folders.Add(tf);
            }
            catch { }
        }

        /// <summary>
        /// 根据文件夹获取内容
        /// </summary>
        /// <param name="folderId">文件夹Id</param>
        /// <param name="sharer">分享人</param>
        /// <param name="shareTime">分享时间</param>
        /// <returns>返回结果</returns>
        [WebMethod]
        public static string OpenFolder(string folderId, string sharer, string shareTime, string expiration, bool canWrite, string pId)
        {
            string content = "[]";
            try
            {
                using (ClientContext context = CommonHelper.GetClientContext())
                {
                    Web web = context.Web; // The SharePoint web at the URL.

                    var documents = web.Lists.GetByTitle(Constant.documents); // 文档库

                    CamlQuery fquery = new CamlQuery();
                    fquery.ViewXml = "<View Scope=\"RecursiveAll\"><Query><Where><Eq><FieldRef Name='FID' /><Value Type='Text'>" + folderId + "</Value></Eq></Where></Query></View>";
                    // 获取文档库Item
                    var fItems = documents.GetItems(fquery);

                    context.Load(fItems);
                    context.ExecuteQuery();

                    if (fItems != null && fItems.Count > 0)
                    {
                        var fItem = fItems[0];
                        var folder = fItem.Folder;
                        // 子文件夹
                        var subFolders = folder.Folders;
                        // 文件
                        var files = folder.Files;

                        context.Load(subFolders);
                        context.Load(files);
                        context.ExecuteQuery();

                        List<FileModel> lists = new List<FileModel>();

                        #region 子文件夹
                        foreach (Folder subfolder in subFolders)
                        {
                            var fieldValues = subfolder.ListItemAllFields;
                            context.Load(fieldValues);
                            context.ExecuteQuery();

                            try
                            {
                                var id = fieldValues["FID"].ToString();

                                var state = Convert.ToInt32(fieldValues["_ModerationStatus"]);
                                if (state == 0 || 0 == 0) // 已批准
                                {
                                    string name = fieldValues["FileLeafRef"].ToString();
                                    string hideName = name;
                                    var created = Convert.ToDateTime(fieldValues["Created"]).ToString("yyyy-MM-dd");
                                    var creator = ((FieldLookupValue)(fieldValues["Author"])).LookupValue;

                                    int type = 1;
                                    string iconUrl = "<img src='/Images/Icons/folder.gif' onerror=\"javascript: this.src = '/Images/Icons/icgen.gif'\" />";
                                    string path = fieldValues["FileRef"].ToString();
                                    name = "<a onclick=\"OpenFolder('" + id + "')\" title = '" + name + "' >" + name + "</a>";

                                    lists.Add(GetModel(name, hideName, id, pId, path, iconUrl, created, creator, sharer, shareTime, type, canWrite, expiration, ""));
                                }
                            }
                            catch { }
                        }
                        #endregion

                        #region 文件

                        foreach (Microsoft.SharePoint.Client.File file in files)
                        {
                            try
                            {
                                var fieldValues = file.ListItemAllFields;
                                context.Load(fieldValues);
                                context.ExecuteQuery();

                                var id = fieldValues["FID"].ToString();

                                var state = Convert.ToInt32(fieldValues["_ModerationStatus"]);
                                if (state == 0 || 0 == 0) // 已批准
                                {
                                    string name = fieldValues["FileLeafRef"].ToString();
                                    string hideName = name;
                                    var created = Convert.ToDateTime(fieldValues["Created"]).ToString("yyyy-MM-dd");
                                    var creator = ((FieldLookupValue)(fieldValues["Author"])).LookupValue;

                                    int type = 0;

                                    string imgUrl = CommonHelper.GetIcon(name);
                                    string iconUrl = "<img src='" + imgUrl + "' onerror=\"javascript: this.src = '/Images/Icons/icgen.gif'\" />";

                                    string filedes = fieldValues["FileDescription"] == null ? "" : fieldValues["FileDescription"].ToString();
                                    if (string.IsNullOrEmpty(filedes))
                                    {
                                        filedes = "No file description.";
                                    }
                                    else if (filedes.Length > 255)
                                    {
                                        filedes = filedes.Substring(0, 255) + "...";
                                    }
                                    name = "<a title='" + name + "' class=\"detail\" data-content=\"" + filedes + "\" >" + name + "</a>";
                                    string size = Math.Round(Convert.ToDouble(file.Length) / 1024, 2) + "KB";
                                    string path = fieldValues["FileRef"].ToString();

                                    lists.Add(GetModel(name, hideName, id, pId, path, iconUrl, created, creator, sharer, shareTime, type, canWrite, expiration, size));
                                }
                            }
                            catch { }
                        }
                        #endregion

                        // 排序，文件夹优先显示
                        //lists = lists.OrderByDescending(T => T.Type).ToList();
                        lists = lists.OrderByDescending(T => T.Type).ThenBy(P => P.HideName).ToList();

                        //序列化
                        DataContractJsonSerializer json = new DataContractJsonSerializer(lists.GetType());
                        using (MemoryStream stream = new MemoryStream())
                        {
                            json.WriteObject(stream, lists);
                            content = Encoding.UTF8.GetString(stream.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                content = "[]";
            }
            return content;
        }

        /// <summary>
        /// 批量下载文件
        /// </summary>
        /// <param name="ids">文件Id集合</param>
        /// <returns>下载结果</returns>
        [WebMethod]
        public static string DownloadFiles(string listIds)
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            MulDownLoadModel model = new MulDownLoadModel();
            try
            {
                if (!string.IsNullOrEmpty(listIds))
                {
                    // 是否正在加密
                    bool isHaveRms = false;

                    // 根文件夹
                    string folder = AppDomain.CurrentDomain.BaseDirectory + Constant.documents + "\\";
                    //如果不存在就创建根文件夹
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }

                    // delete old file
                    CommonHelper.DeleteZipFile(folder);

                    string[] fileIds = listIds.Split(',');

                    // 待压缩文件集合
                    List<Microsoft.SharePoint.Client.File> files = new List<Microsoft.SharePoint.Client.File>();

                    //create the files folder under Downloads
                    string time = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string folderPath = folder + Constant.documents + time;
                    Directory.CreateDirectory(folderPath);

                    using (ClientContext context = CommonHelper.GetClientContext())
                    {
                        Web web = context.Web; // The SharePoint web at the URL.

                        //download the files from library
                        for (int i = 0; i < fileIds.Length; i++)
                        {
                            // 获取下载文件Item
                            var documents = web.Lists.GetByTitle(Constant.documents);
                            var rmsDocuments = web.Lists.GetByTitle(Constant.rmsDocuments);

                            CamlQuery fquery = new CamlQuery();
                            fquery.ViewXml = "<View Scope=\"RecursiveAll\"><Query><Where><Eq><FieldRef Name='FID' /><Value Type='Text'>" + fileIds[i] + "</Value></Eq></Where></Query></View>";
                            // 获取文档库Item
                            var fItems = documents.GetItems(fquery);

                            context.Load(fItems);
                            context.ExecuteQuery();

                            if (fItems != null && fItems.Count > 0)
                            {
                                var item = fItems[0];
                                // 文件
                                var file = item.File;
                                context.Load(file);
                                context.ExecuteQuery();

                                // 是否加密文件
                                if (CommonHelper.IsRMS(item))
                                {
                                    //var url = CommonHelper.GetRMSFolderUrl(file.ServerRelativeUrl);
                                    //Microsoft.SharePoint.Client.File rmsFile = null;
                                    //try
                                    //{
                                    //    rmsFile = web.GetFileByServerRelativeUrl(url);
                                    //    context.Load(rmsFile);
                                    //    context.ExecuteQuery();
                                    //}
                                    //catch { }

                                    CamlQuery rfquery = new CamlQuery();
                                    rfquery.ViewXml = "<View Scope=\"RecursiveAll\"><Query><Where><Eq><FieldRef Name='FID' /><Value Type='Text'>" + item["FID"] + "</Value></Eq></Where></Query></View>";
                                    // 获取文档库Item
                                    var rfItems = rmsDocuments.GetItems(rfquery);

                                    context.Load(rfItems);
                                    context.ExecuteQuery();

                                    Microsoft.SharePoint.Client.File rmsFile = null;
                                    if (rfItems != null && rfItems.Count > 0)
                                    {
                                        // 文件
                                        rmsFile = rfItems[0].File;
                                        context.Load(rmsFile);
                                        context.ExecuteQuery();
                                    }

                                    // 加密文件是否存在
                                    if (rmsFile == null || !rmsFile.Exists)
                                    {
                                        isHaveRms = true;
                                        continue;
                                    }
                                    else
                                    {
                                        file = rmsFile;
                                        files.Add(file);
                                    }
                                }
                                else
                                {
                                    files.Add(file);
                                }
                            }
                        }
                    }
                    if (files.Count > 0)
                    {
                        // 文件夹写入文件
                        WriteFile(files, folderPath);

                        //zip file
                        string zipName = Constant.documents + time + ".zip";
                        string zipPath = folder + zipName;
                        // 压缩文件夹
                        CreateZipFile(folderPath, zipPath);
                        // 删除文件夹
                        Directory.Delete(folderPath, true);

                        string downloadUrl = Constant.documents + "/" + zipName;

                        model.url = downloadUrl;
                        if (isHaveRms)
                        {
                            // 部分不能下载
                            model.msg = "1";
                        }
                        else
                        {
                            // 都可以下载
                            model.msg = "0";
                        }
                    }
                    else
                    {
                        // 都不能下载
                        model.msg = "2";
                    }
                }
                else
                {
                    model.msg = "3";
                }
                return jsonSerializer.Serialize(model);
            }
            catch (Exception ex)
            {
                return jsonSerializer.Serialize("[]");
            }
        }

        /// <summary>
        /// 文件夹中写入文件
        /// </summary>
        /// <param name="files">文件列表</param>
        /// <param name="folderPath">文件夹路径</param>
        private static void WriteFile(List<Microsoft.SharePoint.Client.File> files, string folderPath)
        {
            try
            {
                using (WebClient webclient = CommonHelper.GetWebClient())
                {
                    foreach (var file in files)
                    {
                        LogModel model = new LogModel();
                        model.Title = "Download file：" + file.Name;
                        model.Operate = "Download";
                        model.Operator = userAccount;
                        model.ObjectName = file.ServerRelativeUrl;
                        CommonHelper.SetLog(model);

                        string path = folderPath + @"\" + file.Name;
                        try
                        {
                            // 判断是否有同名文件
                            FileInfo eFile = new FileInfo(path);
                            if (eFile.Exists)
                            {
                                string fileName = file.Name;
                                string suffix = Path.GetExtension(fileName);
                                fileName = fileName.Replace(suffix, "");
                                path = folderPath + @"\" + fileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + suffix;
                            }
                        }
                        catch { }

                        // 获取文件流
                        string fileUrl = file.ServerRelativeUrl;
                        FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
                        byte[] buffer = webclient.DownloadData(Constant.webUrl + fileUrl);

                        // 写入文件夹
                        fs.Write(buffer, 0, buffer.Length);
                        fs.Flush();
                        fs.Close();
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 批量下载（打包成zip文件）
        /// </summary>
        /// <param name="filesPath">文件夹路径</param>
        /// <param name="zipFilePath">压缩文件路径</param>
        private static void CreateZipFile(string filesPath, string zipFilePath)
        {
            try
            {
                string[] filenames = Directory.GetFiles(filesPath);
                using (ZipOutputStream s = new ZipOutputStream(System.IO.File.Create(zipFilePath)))
                {
                    s.SetLevel(5); // 压缩级别 0-9
                    //s.Password = "123"; //Zip压缩文件密码
                    byte[] buffer = new byte[4096]; //缓冲区大小
                    foreach (string file in filenames)
                    {
                        ZipEntry entry = new ZipEntry(Path.GetFileName(file));
                        entry.IsUnicodeText = true;
                        entry.DateTime = DateTime.Now;
                        s.PutNextEntry(entry);
                        using (FileStream fs = System.IO.File.OpenRead(file))
                        {
                            int sourceBytes;
                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                s.Write(buffer, 0, sourceBytes);
                            } while (sourceBytes > 0);
                        }
                    }
                    s.Finish();
                    s.Close();
                }
            }
            catch (Exception ex)
            {
            }
        }

    }
}