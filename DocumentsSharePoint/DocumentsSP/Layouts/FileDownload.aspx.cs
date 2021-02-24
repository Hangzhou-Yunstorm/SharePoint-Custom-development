using DocumentsSP.Helper;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System;

namespace DocumentsSP.Layouts
{
    public partial class FileDownload : LayoutsPageBase
    {
        /// <summary>
        /// 是否是文件夹
        /// </summary>
        public static bool isFolder = false;
        /// <summary>
        /// 是否正在加密
        /// </summary>
        public static bool isFileRMS = false;
        /// <summary>
        /// 是否加密文件
        /// </summary>
        public static bool isRMSFile = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            string fileId = Request.QueryString["ItemID"];
            if (string.IsNullOrEmpty(fileId))
            {
                return;
            }
            isFolder = false;
            isFileRMS = false;
            isRMSFile = false;
            var file = GetFile(Convert.ToInt32(fileId));
            if (isFolder)
            {
                Response.Write("<script type=\"text/javascript\">alert(\"Please select file to download !\");window.close();</script>");
            }
            else if (file == null)
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
                    //var userAgent = Request.UserAgent.ToLower();
                    //var brower = Request.Browser.Browser.ToLower();
                    //if (brower.StartsWith("internetexplorer") || brower.StartsWith("ie") || userAgent.Contains("edge"))
                    //{
                        var enUrl = "/_layouts/15/download.aspx?SourceUrl=" + CommonHelper.EncodeUrl(file.ServerRelativeUrl);
                        Response.Redirect(enUrl, false);
                    //}
                    //else
                    //{
                    //    Response.Redirect(file.ServerRelativeUrl, false);
                    //}

                    //// 非加密文件，直接连接下载
                    //if (!isRMSFile)
                    //{
                    //    Response.Redirect(dFile.ServerRelativeUrl, false);
                    //}
                    //else
                    //{
                    //    var doc = dFile.OpenBinary(SPOpenBinaryOptions.Unprotected);

                    //    Response.ClearHeaders();
                    //    Response.Clear();
                    //    Response.Expires = 0;
                    //    Response.Buffer = true;
                    //    if (fileName.EndsWith(".docx"))
                    //        Response.ContentType = "Application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    //    else
                    //        Response.ContentType = "application/octet-stream";

                    //    //fileName = HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8);
                    //    Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);
                    //    Response.BinaryWrite(doc);
                    //    Response.Flush();
                    //    Response.Close();
                    //    HttpContext.Current.ApplicationInstance.CompleteRequest();
                    //    //Response.End();
                    //}
                }
                catch (Exception ex)
                {
                    CommonHelper.SetErrorLog("FileDownload.aspx__Page_Load", ex.Message);
                }
            }
        }

        /// <summary>
        /// 修改下载次数
        /// </summary>
        /// <returns>返回数据</returns>
        private static SPFile GetFile(int fileId)
        {
            var siteId = SPContext.Current.Site.ID;
            SPFile file = null;
            using (SPSite site = new SPSite(siteId))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    web.AllowUnsafeUpdates = true;
                    try
                    {
                        var fileList = web.Lists.TryGetList(CommonHelper.docListName);

                        var item = fileList.GetItemById(fileId);
                        if (item != null)
                        {
                            if (item.FileSystemObjectType == SPFileSystemObjectType.File)
                            {
                                file = item.File;
                                bool isAllowDown = true;

                                if (CommonHelper.IsRMS(item))
                                {
                                    var rmsfileList = web.Lists.TryGetList(CommonHelper.docRMSListName);
                                    SPFile rmsFile = null;
                                    if (item["FID"] != null)
                                    {
                                        var query = new SPQuery();
                                        query.ViewAttributes = "Scope=\"Recursive\"";
                                        query.Query = "<OrderBy><FieldRef Name=\"Modified\" Ascending=\"False\"></FieldRef></OrderBy>" +
                                                               "<Where><Eq><FieldRef Name='FID' /><Value Type='Text'>" + item["FID"] + "</Value></Eq></Where>";

                                        var items = rmsfileList.GetItems(query);
                                        if (items != null && items.Count > 0)
                                        {
                                            rmsFile = items[0].File;
                                        }
                                    }
                                    if (rmsFile == null || !rmsFile.Exists)
                                    {
                                        isAllowDown = false;
                                        isFileRMS = true;
                                    }
                                    else
                                    {
                                        file = rmsFile;
                                        isRMSFile = true;
                                    }
                                }
                                if (isAllowDown)
                                {
                                    CommonHelper.SetFileDown(fileId);
                                }
                            }
                            else
                            {
                                isFolder = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonHelper.SetErrorLog("FileDownload.aspx__AddDownCount", ex.Message);
                    }
                    web.AllowUnsafeUpdates = false;
                }
            }
            return file;
        }
    }
}
