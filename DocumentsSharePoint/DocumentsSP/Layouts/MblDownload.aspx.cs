using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using DocumentsSP.Helper;
using System.Web.Services;
using System.IO;

namespace DocumentsSP.Layouts
{
    public partial class MblDownload : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 获取下载文件路径
        /// </summary>
        /// <param name="url">文件路径</param>
        /// <returns></returns>
        [WebMethod]
        public static string GetDownload(string uId)
        {
            string msg = "";
            SPFile file = GetFile(uId);

            if (file != null)
            {
                string folderPath = CommonHelper.MblDownloadPath;
                string fileName = file.Name;
                string subPath = DateTime.Now.ToString("yyyyMMddHHmmss");
                try
                {
                    try
                    {
                        CommonHelper.DeleteSubFolders(folderPath);
                        Directory.CreateDirectory(folderPath + subPath);
                    }
                    catch { }

                    FileStream fs = new FileStream(folderPath + subPath + "\\" + fileName, FileMode.OpenOrCreate);
                    byte[] fileByte = file.OpenBinary(SPOpenBinaryOptions.Unprotected);
                    fs.Write(fileByte, 0, fileByte.Length);
                    fs.Flush();
                    fs.Close();
                }
                catch (Exception ex)
                {
                    CommonHelper.SetErrorLog("MblDownload.aspx__AddFile", ex.Message);
                }

                msg = CommonHelper.MblDownloadUrl + subPath + "/" + CommonHelper.EncodeUrl(fileName);
            }
            return msg;
        }

        /// <summary>
        /// 修改下载次数
        /// </summary>
        /// <returns>返回数据</returns>
        public static SPFile GetFile(string uId)
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

                        var fileList = web.Lists.TryGetList(CommonHelper.docListName);
                        var item = fileList.GetItemByUniqueId(new Guid(uId));

                        if (item != null)
                        {
                            file = item.File;
                            bool isAllowDown = true;

                            if (CommonHelper.IsRMS(item))
                            {
                                SPFile rmsFile = null;
                                try
                                {
                                    if (item["FID"] != null)
                                    {
                                        var rmsfileList = web.Lists.TryGetList(CommonHelper.docRMSListName);

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
                                }
                                catch (Exception e)
                                {
                                    CommonHelper.SetErrorLog("MblDownload.aspx__AddDCount_GetFile", e.Message);
                                }
                                if (rmsFile == null || !rmsFile.Exists)
                                {
                                    file = null;
                                }
                                else
                                {
                                    file = rmsFile;
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
                        CommonHelper.SetErrorLog("MblDownload.aspx__AddDCount", ex.Message);
                    }
                    web.AllowUnsafeUpdates = false;
                }
            }
            return file;
        }
    }
}
