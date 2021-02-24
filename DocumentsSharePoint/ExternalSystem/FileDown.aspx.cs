using Microsoft.SharePoint.Client;
using System;
using System.Net;
using System.Web;

namespace ExternalSystem
{
    public partial class FileDown : System.Web.UI.Page
    {
        /// <summary>
        /// 是否正在加密
        /// </summary>
        public static bool isFileRMS = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            string userAccount = Request.QueryString["Account"];
            string fileId = Request.QueryString["ItemID"];
            if (string.IsNullOrEmpty(fileId))
            {
                Response.Write("<script type=\"text/javascript\">alert(\"Invalid link !\");window.close();</script>");
                return;
            }
            isFileRMS = false;
            var file = GetFile(fileId);

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
                LogModel model = new LogModel();
                model.Title = "Download file：" + file.Name;
                model.Operate = "Download";
                model.Operator = userAccount;
                model.ObjectName = file.ServerRelativeUrl;
                CommonHelper.SetLog(model);

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
        /// 修改下载次数
        /// </summary>
        /// <returns>返回数据</returns>
        //[WebMethod]
        public static File GetFile(string fileId)
        {
            File file = null;
            try
            {
                using (ClientContext context = CommonHelper.GetClientContext())
                {
                    Web web = context.Web; // The SharePoint web at the URL.

                    var documents = web.Lists.GetByTitle(Constant.documents);
                    var rmsDocuments = web.Lists.GetByTitle(Constant.rmsDocuments);

                    //var item = documents.GetItemById(fileId);
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
                                //var url = CommonHelper.GetRMSFolderUrl(file.ServerRelativeUrl);
                                //File rmsFile = null;
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