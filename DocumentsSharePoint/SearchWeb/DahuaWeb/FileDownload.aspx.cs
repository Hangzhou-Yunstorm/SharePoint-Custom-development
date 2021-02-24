using Microsoft.SharePoint.Client;
using System;
using System.Net;
using System.Web;

namespace DahuaWeb
{
    public partial class FileDownload : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string fileId = Request.QueryString["ItemId"];
            if (string.IsNullOrEmpty(fileId))
            {
                Response.Write("<script type=\"text/javascript\">alert(\"Invalid link !\");window.close();</script>");
                return;
            }
            bool isFileRMS = false;
            var file = GetFile(fileId, ref isFileRMS);

            if (file == null)
            {
                Response.Write("<script type=\"text/javascript\">alert(\"The file was deleted!\");window.close();</script>");
            }
            else if (isFileRMS)
            {
                Response.Write("<script type=\"text/javascript\">alert(\"The file is being encrypted. Please download it later!\");window.close();</script>");
            }
            else
            {
                try
                {
                    string fileName = file.Name;
                    string fileUrl = file.ServerRelativeUrl;

                    byte[] buffer = null;
                    using (WebClient webclient = CommonHelper.GetWebClient())
                    {
                        // 下载数据  
                        buffer = webclient.DownloadData(Constant.webUrl + fileUrl);
                    }

                    Response.ClearHeaders();
                    Response.Clear();
                    Response.Expires = 0;
                    Response.Buffer = true;
                    if (fileName.EndsWith(".docx"))
                        Response.ContentType = "Application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    else
                        Response.ContentType = "application/octet-stream";

                    Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));
                    Response.BinaryWrite(buffer);
                    Response.Flush();
                    Response.Close();
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
                catch (Exception ex)
                {
                    Response.Write("<script type=\"text/javascript\">alert(\"The file download error!\");window.close();</script>");
                }
            }
        }


        /// <summary>
        /// 下载文件
        /// </summary>
        /// <returns>返回数据</returns>
        public static File GetFile(string fileId, ref bool isFileRMS)
        {
            File file = null;
            try
            {
                using (ClientContext context = CommonHelper.GetClientContext())
                {
                    Web web = context.Web; // The SharePoint web at the URL.

                    var documents = web.Lists.GetByTitle(Constant.documents);
                    var rmsDocuments = web.Lists.GetByTitle(Constant.rmsDocuments);

                    CamlQuery fquery = new CamlQuery();
                    fquery.ViewXml = "<View Scope=\"RecursiveAll\"><Query><Where><Eq><FieldRef Name='FID' /><Value Type='Text'>" + fileId + "</Value></Eq></Where></Query></View>";
                    // 获取文档库Item
                    var fItems = documents.GetItems(fquery);
                    context.Load(fItems);
                    context.ExecuteQuery();

                    if (fItems != null && fItems.Count > 0)
                    {
                        var item = fItems[0];
                        if (item.FileSystemObjectType == FileSystemObjectType.File)
                        {
                            file = item.File;
                            context.Load(file);
                            context.ExecuteQuery();

                            if (CommonHelper.IsRMS(item))
                            {
                                CamlQuery rfquery = new CamlQuery();
                                rfquery.ViewXml = "<View Scope=\"Recursive\"><Query><Where><Eq><FieldRef Name='FID' /><Value Type='Text'>" + item["FID"] + "</Value></Eq></Where></Query></View>";
                                // 获取文档库Item
                                var rfItems = rmsDocuments.GetItems(rfquery);

                                context.Load(rfItems);
                                context.ExecuteQuery();

                                File rmsFile = null;
                                if (rfItems != null && rfItems.Count > 0)
                                {
                                    // 文件
                                    rmsFile = rfItems[0].File;
                                    context.Load(rmsFile);
                                    context.ExecuteQuery();
                                }
                                if (rmsFile == null || !rmsFile.Exists)
                                {
                                    isFileRMS = true;
                                }
                                else
                                {
                                    file = rmsFile;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { }
            return file;
        }

    }
}