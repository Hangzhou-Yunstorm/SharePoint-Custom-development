using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using DocumentsSP.Helper;

namespace DocumentsSP.Layouts
{
    public partial class ReturnFolder : LayoutsPageBase
    {
        /// <summary>
        /// 初始化，根据文件的guid跳转到对于的文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // 文件的FID
            string fId = Request.QueryString["FID"];
            // 文件夹的FID
            string folderId = Request.QueryString["FolderFID"];

            try
            {
                if (!string.IsNullOrEmpty(fId) || !string.IsNullOrEmpty(folderId))
                {
                    using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            var fileList = web.Lists.TryGetList(CommonHelper.docListName);
                            if (!string.IsNullOrEmpty(fId))
                            {
                                var query = new SPQuery();
                                query.ViewAttributes = "Scope=\"Recursive\"";
                                query.Query = "<Where><Eq><FieldRef Name='FID' /><Value Type='Text'>" + fId + "</Value></Eq></Where>";

                                var items = fileList.GetItems(query);

                                if (items != null && items.Count > 0)
                                {
                                    var folderUrl = items[0].File.ParentFolder.ServerRelativeUrl;
                                    string url = "/" + CommonHelper.docListName + "/Forms/AllItems.aspx?RootFolder=" + CommonHelper.EncodeUrl(folderUrl);
                                    Response.Redirect(url, false);
                                }
                                else
                                {
                                    Response.Write("<script type=\"text/javascript\">alert(\"The folder was deleted!\");window.close();</script>");
                                }
                            }
                            else
                            {
                                var query = new SPQuery();
                                query.ViewAttributes = "Scope=\"RecursiveAll\"";
                                query.Query = "<Where><Eq><FieldRef Name='FID' /><Value Type='Text'>" + folderId + "</Value></Eq></Where>";

                                var items = fileList.GetItems(query);

                                if (items != null && items.Count > 0)
                                {
                                    var folderUrl = items[0].Folder.ServerRelativeUrl;
                                    string url = "/" + CommonHelper.docListName + "/Forms/AllItems.aspx?RootFolder=" + CommonHelper.EncodeUrl(folderUrl);
                                    Response.Redirect(url, false);
                                }
                                else
                                {
                                    Response.Write("<script type=\"text/javascript\">alert(\"The folder was deleted!\");window.close();</script>");
                                }
                            }
                        }
                    }
                }
                else
                {
                    Response.Write("<script type=\"text/javascript\">alert(\"The folder was deleted!\");window.close();</script>");
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("ReturnFolder.aspx__Page_Load", ex.Message);
                Response.Write("<script type=\"text/javascript\">alert(\"The folder was deleted!\");window.close();</script>");
            }
        }
    }
}
