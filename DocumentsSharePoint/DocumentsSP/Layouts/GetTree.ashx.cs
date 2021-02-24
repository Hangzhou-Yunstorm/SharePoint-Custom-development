using DocumentsSP.Helper;
using DocumentsSP.Model;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Linq;

namespace DocumentsSP.Layouts
{
    public partial class GetTree : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var pId = Convert.ToInt32(context.Request.QueryString["Id"]);
            // 节点集合
            List<TFolderModel> folders = new List<TFolderModel>();

            using (SPSite site = new SPSite(SPContext.Current.Site.ID))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    //获取List
                    SPList list = web.Lists.TryGetList(CommonHelper.docListName);
                    var fItem = list.GetItemById(Convert.ToInt32(pId));

                    // 子节点
                    var subFolders = fItem.Folder.SubFolders;
                    // 遍历子节点
                    if (subFolders != null && subFolders.Count > 0)
                    {
                        foreach (SPFolder folder in subFolders)
                        {
                            var subFolderC = folder.SubFolders;

                            TFolderModel fd = new TFolderModel();
                            fd.id = folder.Item.ID;
                            fd.order = folder.Item["Order Index"] == null ? 0 : Convert.ToInt32(folder.Item["Order Index"]);
                            fd.pId = pId;
                            fd.name = folder.Name;
                            string url = CommonHelper.EncodeUrl(folder.ServerRelativeUrl);
                            fd.furl = folder.ServerRelativeUrl;
                            // 是否含有子节点，即父节点
                            if (subFolderC != null && subFolderC.Count > 0)
                            {
                                fd.isParent = true;
                            }
                            folders.Add(fd);
                        }
                        folders = folders.OrderBy(T => T.order).ThenBy(T => T.name).ToList();
                    }
                }
            }

            var treeFolderJson = "[]";
            if (folders.Count > 0)
            {
                DataContractJsonSerializer json = new DataContractJsonSerializer(folders.GetType());
                //序列化
                using (MemoryStream stream = new MemoryStream())
                {
                    json.WriteObject(stream, folders);
                    treeFolderJson = Encoding.UTF8.GetString(stream.ToArray());
                }
            }

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
