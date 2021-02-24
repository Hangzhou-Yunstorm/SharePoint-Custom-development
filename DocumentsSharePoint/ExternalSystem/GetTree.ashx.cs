using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

namespace ExternalSystem
{
    /// <summary>
    /// Summary description for GetTree
    /// </summary>
    public class GetTree : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var fId = context.Request.QueryString["fId"];
            var pId = context.Request.QueryString["pId"];
            var sharer = context.Request.QueryString["sharer"];
            var shareTime = context.Request.QueryString["shareTime"];
            var canWrite = context.Request.QueryString["canWrite"];
            var expiration = context.Request.QueryString["expiration"];

            var treeFolderJson = "[]";
            try
            {
                List<TreeFolder> trees = new List<TreeFolder>();

                using (ClientContext clientContext = CommonHelper.GetClientContext())
                {
                    Web web = clientContext.Web; // The SharePoint web at the URL.
                    // 文档库
                    var documents = web.Lists.GetByTitle(Constant.documents);

                    CamlQuery fquery = new CamlQuery();
                    fquery.ViewXml = "<View Scope=\"RecursiveAll\"><Query><Where><Eq><FieldRef Name='FID' /><Value Type='Text'>" + fId + "</Value></Eq></Where></Query></View>";
                    // 获取文档库Item
                    var items = documents.GetItems(fquery);
                    clientContext.Load(items);
                    clientContext.ExecuteQuery();

                    if (items != null && items.Count > 0)
                    {
                        ListItem item = items[0];

                        var subFolders = item.Folder.Folders;
                        clientContext.Load(subFolders);
                        clientContext.ExecuteQuery();

                        if (subFolders.Count > 0)
                        {
                            foreach (Folder folder in subFolders)
                            {
                                var fItem = folder.ListItemAllFields;
                                clientContext.Load(fItem);
                                clientContext.ExecuteQuery();
                                if (fItem.FieldValues["FID"] != null)
                                {
                                    TreeFolder tf = new TreeFolder();
                                    tf.fId = fItem.FieldValues["FID"].ToString();
                                    tf.name = fItem.FieldValues["FileLeafRef"].ToString();
                                    tf.path = fItem.FieldValues["FileRef"].ToString();
                                    tf.pId = pId;
                                    tf.id = tf.fId + pId;
                                    tf.sharer = sharer;
                                    tf.shareTime = shareTime;
                                    tf.canWrite = Convert.ToBoolean(canWrite);
                                    tf.expiration = expiration;
                                    tf.defaultType = 0;

                                    var sFolders = fItem.Folder.Folders;
                                    clientContext.Load(sFolders);
                                    clientContext.ExecuteQuery();
                                    if (sFolders.Count > 0)
                                    {
                                        tf.isParent = true;
                                    }

                                    trees.Add(tf);
                                }
                            }
                        }
                    }
                }

                if (trees.Count > 0)
                {
                    trees = trees.OrderByDescending(T => T.defaultType).ThenBy(P => P.name).ToList();
                    DataContractJsonSerializer json = new DataContractJsonSerializer(trees.GetType());
                    //序列化
                    using (MemoryStream stream = new MemoryStream())
                    {
                        json.WriteObject(stream, trees);
                        treeFolderJson = Encoding.UTF8.GetString(stream.ToArray());
                    }
                }

            }
            catch { }

            context.Response.ContentType = "text/plain";
            context.Response.Write(treeFolderJson);
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