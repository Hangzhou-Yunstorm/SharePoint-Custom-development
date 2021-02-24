using Microsoft.SharePoint.Client;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;

namespace ExternalSystem
{
    public partial class Upload : System.Web.UI.Page
    {
        public string userAccount = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            FolderId.Text = Request.QueryString["folderId"];
            userAccount = Request.QueryString["Account"];
        }

        #region  上传文件
        /// <summary>
        /// 上传文件
        /// </summary>
        protected void UploadFile(object sender, EventArgs e)
        {
            //HttpFileCollection files = HttpContext.Current.Request.Files;

            if (!FileUpload.HasFile)
            {
                Response.Write("<script>alert('Please select file !');</script>");
                return;
            }
            else
            {
                try
                {
                    var fileName = FileUpload.PostedFile.FileName;
                    if (fileName.Contains("\\"))
                    {
                        var start = fileName.LastIndexOf("\\") + 1;
                        fileName = fileName.Substring(start, fileName.Length - start);
                    }

                    using (ClientContext context = CommonHelper.GetClientContext())
                    {
                        Web web = context.Web;

                        // 文档库
                        var documents = web.Lists.GetByTitle(Constant.documents);
                        CamlQuery fquery = new CamlQuery();
                        fquery.ViewXml = "<View Scope=\"RecursiveAll\"><Query><Where><Eq><FieldRef Name='FID' /><Value Type='Text'>" + FolderId.Text + "</Value></Eq></Where></Query></View>";
                        // 获取文档库Item
                        var fItems = documents.GetItems(fquery);
                        context.Load(fItems);
                        context.ExecuteQuery();

                        //var uId = fItems[0].FieldValues["UniqueId"].ToString();
                        //var files = web.GetFolderById(new Guid(uId)).Files;
                        var files = fItems[0].Folder.Files;
                        context.Load(files);
                        context.ExecuteQuery();

                        Stream stream = FileUpload.PostedFile.InputStream;
                        FileCreationInformation listItem = new FileCreationInformation();
                        listItem.Url = fileName;
                        listItem.ContentStream = stream;
                        listItem.Overwrite = true;
                        Microsoft.SharePoint.Client.File file = files.Add(listItem);
                        context.Load(file);
                        context.RequestTimeout = int.MaxValue;
                        context.ExecuteQuery();
                        Thread.Sleep(3000); //3秒

                        LogModel model = new LogModel();
                        model.Title = "Upload file：" + file.Name;
                        model.Operate = "Upload";
                        model.Operator = userAccount;
                        model.ObjectName = file.ServerRelativeUrl;
                        CommonHelper.SetLog(model);
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('" + ex.Message + "');</script>");
                    return;
                }
                Response.Write("<script>var index = parent.layer.getFrameIndex(window.name);parent.layer.open.callback.LoadTable('" + FolderId.Text + "');parent.layer.close(index);</script>");
                return;
            }
        }

        # endregion
    }
}