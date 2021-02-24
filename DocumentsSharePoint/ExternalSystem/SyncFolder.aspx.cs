using Microsoft.SharePoint.Client;
using System;

namespace ExternalSystem
{
    public partial class SyncFolder : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 添加RMS文件夹
        /// </summary>
        protected void SyncFolders(object sender, EventArgs e)
        {
            try
            {
                using (ClientContext context = CommonHelper.GetGKSClientContext())
                {
                    Web web = context.Web; // The SharePoint web at the URL.
                    var documents = web.Lists.GetByTitle(Constant.documents); // 文档库

                    var rootFolder = documents.RootFolder;

                    context.Load(rootFolder);
                    context.ExecuteQuery();

                    var subFolders = rootFolder.Folders;
                    context.Load(subFolders);
                    context.ExecuteQuery();

                    AddFolder(rootFolder, subFolders, context, web);
                }
            }
            catch (Exception ex)
            {

            }

        }

        /// <summary>
        /// 审批文件夹
        /// </summary>
        protected void SyncApproveFolders(object sender, EventArgs e)
        {
            using (ClientContext context = CommonHelper.GetGKSClientContext())
            {
                Web web = context.Web; // The SharePoint web at the URL.
                var documents = web.Lists.GetByTitle(Constant.documents); // 文档库

                CamlQuery query = CamlQuery.CreateAllFoldersQuery();
                var items = documents.GetItems(query);
                context.Load(items);
                context.ExecuteQuery();
                foreach (Microsoft.SharePoint.Client.ListItem item in items)
                {
                    int state = int.Parse(item["_ModerationStatus"].ToString());//0已批准；1未通过；2待定；3草稿
                    if (state == 2)
                    {
                        item["_ModerationStatus"] = 0;
                        item.Update();
                        context.ExecuteQuery();
                    }
                }
            }
        }

        /// <summary>
        /// 遍历创建文件夹
        /// </summary>
        private void AddFolder(Folder rootFolder, FolderCollection subFolders, ClientContext context, Web web)
        {
            var rmsRootUrl = CommonHelper.GetRMSFolderUrl(rootFolder.ServerRelativeUrl);
            var rmsRootFolder = web.GetFolderByServerRelativeUrl(rmsRootUrl);
            context.Load(rmsRootFolder);
            context.ExecuteQuery();

            foreach (var folder in subFolders)
            {
                if (folder.Name == "Forms")
                {
                    continue;
                }
                context.Load(folder);
                context.ExecuteQuery();

                var url = CommonHelper.GetRMSFolderUrl(folder.ServerRelativeUrl);
                try
                {
                    var rmsFolder = web.GetFolderByServerRelativeUrl(url);
                    context.Load(rmsFolder);
                    context.ExecuteQuery();

                    if (!rmsFolder.Exists)
                    {
                        rmsRootFolder.Folders.Add(url);

                        context.Load(rmsRootFolder);
                        context.ExecuteQuery();
                    }
                }
                catch
                {
                    rmsRootFolder.Folders.Add(url);

                    context.Load(rmsRootFolder);
                    context.ExecuteQuery();
                }

                var subSFolders = folder.Folders;
                context.Load(subSFolders);
                context.ExecuteQuery();

                if (subSFolders.Count > 0)
                {
                    AddFolder(folder, subSFolders, context, web);
                }

            }

        }
    }
}