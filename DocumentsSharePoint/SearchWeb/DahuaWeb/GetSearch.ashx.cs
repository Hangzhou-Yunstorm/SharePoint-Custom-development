using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace DahuaWeb
{
    /// <summary>
    /// GetSearch 的摘要说明
    /// </summary>
    public class GetSearch : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            Hashtable hash = new Hashtable();
            JavaScriptSerializer jss = new JavaScriptSerializer();

            var key = context.Request.QueryString["Name"];
            var path = context.Request.QueryString["Folder"];
            var token = context.Request.QueryString["Token"];

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(path))
            {
                hash["Msg"] = "Invalid parameter";
                hash["Data"] = "[]";
                hash["Status"] = -1;
            }
            else
            {
                SymmCrypt symmCrypt = new SymmCrypt();
                token = symmCrypt.DESDeCode(token, Constant.key, Constant.key);

                var isCorrect = true;
                if (token.Contains(Constant.key))
                {
                    var timeSpan = token.Replace(Constant.key, "");
                    DateTime time = CommonHelper.GetDateByTimeSpan(timeSpan);
                    if ((DateTime.UtcNow - time).TotalSeconds > 60 * 60)
                    {
                        isCorrect = false;

                        hash["Msg"] = "Time Out";
                        hash["Data"] = "[]";
                        hash["Status"] = 0;
                    }
                }
                else
                {
                    isCorrect = false;

                    hash["Msg"] = "Invalid token";
                    hash["Data"] = "[]";
                    hash["Status"] = -1;
                }
                if (isCorrect)
                {
                    key = Microsoft.JScript.GlobalObject.decodeURIComponent(key);
                    path = Microsoft.JScript.GlobalObject.decodeURIComponent(path);
                    string msg = string.Empty;

                    var list = GetJson(key, path, ref msg);
                    if (string.IsNullOrEmpty(msg))
                    {
                        string listJson = JsonConvert.SerializeObject(list);

                        hash["Msg"] = "Success";
                        hash["Status"] = 1;
                        hash["Data"] = listJson;
                    }
                    else
                    {
                        hash["Msg"] = msg;
                        hash["Data"] = "[]";
                        hash["Status"] = -1;
                    }
                }
            }

            var obj = jss.Serialize(hash);
            context.Response.ContentType = "application/json";
            context.Response.Write(obj);
            context.Response.End();
        }

        /// <summary>
        /// 根据路径/关键字获取文件
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="path">路径</param>
        /// <param name="msg">信息</param>
        /// <returns></returns>
        private List<FileJsonModel> GetJson(string key, string path, ref string msg)
        {
            List<FileJsonModel> list = new List<FileJsonModel>();
            try
            {
                using (ClientContext context = CommonHelper.GetClientContext())
                {
                    Web web = context.Web; // The SharePoint web at the URL.
                    // 文件路径
                    string folderUrl = Constant.documents + "/" + path;

                    try
                    {
                        var folder = web.GetFolderByServerRelativeUrl(Constant.webUrl + folderUrl);
                        context.Load(folder);
                        context.ExecuteQuery();
                        folderUrl = folder.ServerRelativeUrl;
                    }
                    catch
                    {
                        msg = "Invalid folder";
                    }
                    if (string.IsNullOrEmpty(msg))
                    {
                        var documents = web.Lists.GetByTitle(Constant.documents);
                        CamlQuery fquery = new CamlQuery();
                        fquery.ViewXml = "<View Scope=\"Recursive\"><Query><Where><Contains><FieldRef Name='FileLeafRef' /><Value Type='Text'>" + key + "</Value></Contains></Where></Query></View>";
                        fquery.FolderServerRelativeUrl = folderUrl;
                        // 获取文档库Item
                        var fItems = documents.GetItems(fquery);
                        context.Load(fItems);
                        context.ExecuteQuery();

                        if (fItems != null && fItems.Count > 0)
                        {
                            foreach (ListItem item in fItems)
                            {
                                if (item.FieldValues["FID"] != null)
                                {
                                    FileJsonModel mo = new FileJsonModel();
                                    mo.Name = item.FieldValues["FileLeafRef"].ToString();
                                    mo.FID = item.FieldValues["FID"].ToString();
                                    mo.Description = item.FieldValues["FileDescription"] == null ? "" : item.FieldValues["FileDescription"].ToString();
                                    mo.Url = Constant.webSiteUrl + "FileDownload.aspx?ItemId=" + item.FieldValues["FID"];
                                    list.Add(mo);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return list;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}