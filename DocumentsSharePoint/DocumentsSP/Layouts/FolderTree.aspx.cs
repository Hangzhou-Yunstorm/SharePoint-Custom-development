using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Collections.Generic;
using System.Web.Services;
using DocumentsSP.Model;
using DocumentsSP.Helper;
using System.Linq;
using System.Web;

namespace DocumentsSP.Layouts
{
    public partial class FolderTree : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 是否有文件夹的添加权限
        /// </summary>
        /// <param name="rootFolder">文件夹路径</param>
        /// <returns>返回结果</returns>
        [WebMethod]
        public static string IsFolderHadPermisson(string rootFolder)
        {
            string msg = string.Empty;

            bool isPermissionAdd = false;
            bool isPermissionExport = false;

            rootFolder = CommonHelper.DecodeUrl(rootFolder);
            try
            {
                CommonHelper.IsFolderHadPermissons(rootFolder, SPContext.Current.Site.ID, SPContext.Current.Web.CurrentUser, ref isPermissionAdd, ref isPermissionExport);
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("FolderTree.aspx__IsFolderHadPermisson", ex.Message + " Error:" + rootFolder);
            }
            string addMsg = isPermissionAdd ? "OK" : "NO";
            string exportMsg = isPermissionExport ? "OK" : "NO";
            msg = addMsg + "," + exportMsg;

            return msg;
        }

        /// <summary>
        /// 获取文件夹树
        /// </summary>
        /// <returns>返回数据</returns>
        [WebMethod]
        public static string GetFolderTree(string rootFolder)
        {
            try
            {
                rootFolder = CommonHelper.DecodeUrl(rootFolder);
                List<TFolderModel> folders = new List<TFolderModel>();

                using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        //获取List
                        SPList list = web.Lists.TryGetList(CommonHelper.docListName);
                        var folderList = list.RootFolder.SubFolders;

                        if (folderList != null && folderList.Count > 0)
                        {
                            SPFolder gFolder = null;
                            // 文档库根节点
                            if (string.IsNullOrEmpty(rootFolder) || rootFolder == "/" + CommonHelper.docListName || rootFolder == "/" + CommonHelper.docListName + "/")
                            {
                                foreach (SPFolder subGF in folderList)
                                {
                                    if (subGF.Name != "Forms")
                                    {
                                        ReadFolders(subGF, 0, folders, rootFolder);
                                    }
                                }
                            }
                            else
                            {
                                // 遍历找到当前节点的一级节点
                                foreach (SPFolder folder in folderList)
                                {
                                    if (rootFolder.StartsWith(folder.ServerRelativeUrl))
                                    {
                                        // 一级节点
                                        gFolder = folder;
                                        break;
                                    }
                                }
                                if (gFolder != null)
                                {
                                    // 遍历一级节点
                                    SPFolderCollection subFolderC = gFolder.SubFolders;
                                    if (subFolderC != null && subFolderC.Count > 0)
                                    {
                                        foreach (SPFolder subF in subFolderC)
                                        {
                                            //SPRoleDefinition limitedAccessRole = SPContext.Current.Web.RoleDefinitions["受限访问"];
                                            //foreach (SPRoleAssignment roleAssignment in folder.Item.RoleAssignments)
                                            //{
                                            //    if (roleAssignment.RoleDefinitionBindings.Contains(limitedAccessRole))
                                            //    {
                                            //        // Your limited access
                                            //    }
                                            //}
                                            ReadFolders(subF, gFolder.Item.ID, folders, rootFolder);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                string szJson = "";
                if (folders.Count > 0)
                {
                    // 排序
                    folders = folders.OrderBy(T => T.order).ThenBy(T => T.name).ToList();

                    DataContractJsonSerializer json = new DataContractJsonSerializer(folders.GetType());
                    //序列化
                    using (MemoryStream stream = new MemoryStream())
                    {
                        json.WriteObject(stream, folders);
                        szJson = Encoding.UTF8.GetString(stream.ToArray());
                    }
                }
                return szJson;
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("FolderTree.aspx__Page_Load", ex.Message + " Error:" + rootFolder);
                return "[]";
            }
        }

        /// <summary>
        /// 读取文件夹目录
        /// </summary>
        /// <param name="folder">文件夹对象</param>
        /// <param name="pId">上级id</param>
        /// <param name="listName">文档库url名</param>
        private static void ReadFolders(SPFolder folder, int pId, List<TFolderModel> folders, string rootFolder)
        {
            try
            {
                SPFolderCollection subFolderC = folder.SubFolders;

                TFolderModel fd = new TFolderModel();
                fd.id = folder.Item.ID;
                fd.pId = pId;
                fd.name = folder.Name;
                fd.order = folder.Item["Order Index"] == null ? 0 : Convert.ToInt32(folder.Item["Order Index"]);
                fd.title = folder.Name;
                fd.furl = folder.ServerRelativeUrl;
                if (subFolderC != null && subFolderC.Count > 0)
                {
                    fd.isParent = true;
                }
                if (rootFolder.StartsWith(fd.furl))
                {
                    fd.open = true;
                }
                folders.Add(fd);

                if (subFolderC != null && subFolderC.Count > 0 && rootFolder.StartsWith(fd.furl))
                {
                    foreach (SPFolder subF in subFolderC)
                    {
                        ReadFolders(subF, folder.Item.ID, folders, rootFolder);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("FolderTree.aspx__ReadFolders", ex.Message + " Error:" + rootFolder);
            }
        }
    }
}
