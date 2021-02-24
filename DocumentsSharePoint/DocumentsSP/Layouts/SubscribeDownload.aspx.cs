using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web;
using DocumentsSP.Helper;
using Microsoft.JScript;

namespace DocumentsSP.Layouts
{
    public partial class SubscribeDownload : LayoutsPageBase
    {
        /// <summary>
        /// 是否正在加密
        /// </summary>
        public static bool isFileRMS = false;
        /// <summary>
        /// 是否加密文件
        /// </summary>
        public static bool isRMSFile = false;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            string itemId = Request.QueryString["ItemID"];
            if (string.IsNullOrEmpty(itemId))
            {
                return;
            }
            isFileRMS = false;
            isRMSFile = false;
            SPFile file = GetFile(itemId);
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

                    //SPFile dFile = file;
                    //// 非加密文件，直接连接下载
                    //if (!isRMSFile)
                    //{
                    //    Response.Redirect(dFile.ServerRelativeUrl, false);
                    //}
                    //else
                    //{
                    //    string fileName = file.Name;
                    //    var doc = file.OpenBinary(SPOpenBinaryOptions.Unprotected);

                    //    Response.ClearHeaders();
                    //    Response.Clear();
                    //    Response.Expires = 0;
                    //    Response.Buffer = true;
                    //    if (fileName.EndsWith(".docx"))
                    //        Response.ContentType = "Application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    //    else
                    //        Response.ContentType = "application/octet-stream";

                    //    fileName = HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8);
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
                    CommonHelper.SetErrorLog("SubscribeDownload.aspx__Page_Load", ex.Message);
                }
            }
        }

        /// <summary>
        /// 修改下载次数
        /// </summary>
        /// <returns>返回数据</returns>
        private static SPFile GetFile(string hId)
        {
            SPFile file = null;
            var siteId = SPContext.Current.Site.ID;

            using (SPSite site = new SPSite(siteId))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    web.AllowUnsafeUpdates = true;
                    try
                    {
                        var hList = web.Lists.TryGetList(CommonHelper.subscribeList);
                        var hItem = hList.GetItemById(System.Convert.ToInt32(hId));

                        var fileList = web.Lists.TryGetList(CommonHelper.docListName);
                        //Guid g = new Guid(hItem["FID"].ToString());
                        //var item = fileList.GetItemByUniqueId(g);

                        var fId = hItem["FID"].ToString();
                        var query = new SPQuery();
                        query.ViewAttributes = "Scope=\"Recursive\"";
                        query.Query = "<Where><Eq><FieldRef Name='FID' /><Value Type='Text'>" + fId + "</Value></Eq></Where>";

                        var items = fileList.GetItems(query);
                        SPListItem item = null;

                        if (items != null && items.Count > 0)
                        {
                            item = items[0];
                            file = item.File;
                            bool isAllowDown = true;

                            if (CommonHelper.IsRMS(item))
                            {
                                var rmsfileList = web.Lists.TryGetList(CommonHelper.docRMSListName);
                                SPFile rmsFile = null;
                                if (item["FID"] != null)
                                {
                                    var fquery = new SPQuery();
                                    fquery.ViewAttributes = "Scope=\"Recursive\"";
                                    fquery.Query = "<OrderBy><FieldRef Name=\"Modified\" Ascending=\"False\"></FieldRef></OrderBy>" +
                                                            "<Where><Eq><FieldRef Name='FID' /><Value Type='Text'>" + item["FID"] + "</Value></Eq></Where>";

                                    var fitems = rmsfileList.GetItems(fquery);
                                    if (fitems != null && fitems.Count > 0)
                                    {
                                        rmsFile = fitems[0].File;
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
                                CommonHelper.SetFileDown(item.ID);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonHelper.SetErrorLog("SubscribeDownload.aspx__AddDCount", ex.Message);
                    }
                    web.AllowUnsafeUpdates = false;
                }
            }
            return file;
        }
    }
}
