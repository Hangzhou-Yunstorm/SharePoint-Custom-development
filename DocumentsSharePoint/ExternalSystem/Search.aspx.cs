using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Services;

namespace ExternalSystem
{

    public partial class Search : System.Web.UI.Page
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string userName = "";

        /// <summary>
        /// 用户
        /// </summary>
        public string userAccount = "";

        /// <summary>
        /// 区域
        /// </summary>
        public string region = "";

        /// <summary>
        /// 国家
        /// </summary>
        public string country = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            // 单点登录
            var user = Request.QueryString["basic"];
            var password = Request.QueryString["system"];
            var token = Request.QueryString["token"];

            // 登录
            var loginuser = Request.QueryString["name"];
            var loginpsw = HttpContext.Current.Session[loginuser];

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
                }
            }
        }

        [WebMethod]
        public static string GetSearchResult(string account, string region, string country, string searchKey)
        {
            string content = "[]";
            try
            {
                searchKey = Microsoft.JScript.GlobalObject.decodeURIComponent(searchKey);
                using (ClientContext context = CommonHelper.GetClientContext())
                {
                    Web web = context.Web; // The SharePoint web at the URL.

                    // 分享库
                    var externalList = web.Lists.GetByTitle(Constant.shareCatalogList);
                    // 文档库
                    var documents = web.Lists.GetByTitle(Constant.documents);
                    // 条目集合
                    List<FileModel> lists = new List<FileModel>();

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
                                GetSearchByFolder(searchKey, rFolder.ServerRelativeUrl, "system", "Never expire", lists, context, documents);
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
                                    GetSearchByFolder(searchKey, cFolder.ServerRelativeUrl, "system", "Never expire", lists, context, documents);
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
                                    "<Eq><FieldRef Name='UserAccount' /><Value Type='Text'>" + account + "</Value></Eq>" +
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
                                    // 属性集合
                                    var fieldValues = fItem.FieldValues;

                                    if (fItem.FileSystemObjectType == FileSystemObjectType.Folder) // 文件夹
                                    {
                                        GetSearchByFolder(searchKey, fieldValues["FileRef"].ToString(), sharer, expiration, lists, context, documents);
                                    }
                                    else // 文件
                                    {
                                        // 获取文件属性值
                                        string name = fieldValues["FileLeafRef"].ToString();

                                        if (name.Contains(searchKey))
                                        {
                                            // 根据名称获取图标地址
                                            string imgUrl = CommonHelper.GetIcon(name);
                                            // 构造图标
                                            string iconUrl = "<img src='" + imgUrl + "' onerror=\"javascript: this.src = '/Images/Icons/icgen.gif'\" />";

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

                                            string size = Math.Round(Convert.ToDouble(file.Length) / 1024, 2) + "KB";
                                            name = "<a title='" + name + "' class=\"detail\" data-content=\"" + filedes + "\" >" + name + "</a>";

                                            FileModel mo = new FileModel() { ID = fId, Name = name, Sharer = sharer, Expiration = expiration, Size = size, IconUrl = iconUrl };
                                            lists.Add(mo);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    if (lists.Count > 0)
                    {
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

        private static void GetSearchByFolder(string key, string folderUrl, string shareby, string expiration, List<FileModel> lists, ClientContext context, List documents)
        {
            try
            {
                CamlQuery fquery = new CamlQuery();
                fquery.ViewXml = "<View Scope=\"Recursive\"><Query><Where><Contains><FieldRef Name='FileLeafRef' /><Value Type='Text'>" + key + "</Value></Contains></Where></Query></View>";
                fquery.FolderServerRelativeUrl = folderUrl;

                // 获取文档库Item
                var fItems = documents.GetItems(fquery);
                context.Load(fItems);
                context.ExecuteQuery();

                if (fItems != null && fItems.Count > 0)
                {
                    foreach (ListItem fItem in fItems)
                    {
                        // 属性集合
                        var fieldValues = fItem.FieldValues;

                        if (fieldValues["FID"] != null)
                        {
                            string fId = fieldValues["FID"].ToString();
                            // 获取文件名
                            string name = fieldValues["FileLeafRef"].ToString();
                            // 根据名称获取图标地址
                            string imgUrl = CommonHelper.GetIcon(name);
                            // 构造图标
                            string iconUrl = "<img src='" + imgUrl + "' onerror=\"javascript: this.src = '/Images/Icons/icgen.gif'\" />";

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

                            string size = Math.Round(Convert.ToDouble(file.Length) / 1024, 2) + "KB";
                            name = "<a title='" + name + "' class=\"detail\" data-content=\"" + filedes + "\" >" + name + "</a>";

                            FileModel mo = new FileModel() { ID = fId, Name = name, Sharer = shareby, Expiration = expiration, Size = size, IconUrl = iconUrl };
                            lists.Add(mo);
                        }
                    }

                }
            }
            catch { }
        }



    }
}