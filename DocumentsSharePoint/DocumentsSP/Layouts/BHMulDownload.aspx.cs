using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using DocumentsSP.Helper;
using System.Web.Services;
using System.Web.Script.Serialization;
using DocumentsSP.Model;

namespace DocumentsSP.Layouts
{
    public partial class BHMulDownload : LayoutsPageBase
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 批量下载
        /// </summary>
        /// <param name="listId"></param>
        /// <returns></returns>
        [WebMethod]
        public static string BHDownLoadZIP(string listId)
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            DownLoadModel model = new DownLoadModel();
            try
            {
                if (!string.IsNullOrEmpty(listId))
                {
                    var siteId = SPContext.Current.Site.ID;
                    using (SPSite site = new SPSite(siteId))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            bool isHaveFile = false;
                            bool isHaveFolder = false;
                            bool isHaveRms = false;
                            bool isHaveDelete = false;

                            web.AllowUnsafeUpdates = true;

                            string folder = CommonHelper.ZipFilePath;

                            // delete old file
                            CommonHelper.DeleteZipFile(folder);

                            string[] fileIds = listId.Split(',');
                            var sdl = web.Lists.TryGetList(CommonHelper.docListName);

                            //create the files folder under Downloads\Files
                            string time = DateTime.Now.ToString("yyyyMMddHHmmss");

                            string folderPath = folder + sdl.Title + time;
                            Directory.CreateDirectory(folderPath);

                            //download the files from library
                            for (int i = 0; i < fileIds.Length - 1; i++)
                            {
                                var hID = fileIds[i];
                                var hList = web.Lists.TryGetList(CommonHelper.historyListName);
                                var hItem = hList.GetItemById(Convert.ToInt32(hID));

                                var fId = hItem["FID"].ToString();
                                var query = new SPQuery();
                                query.ViewAttributes = "Scope=\"Recursive\"";
                                query.Query = "<Where><Eq><FieldRef Name='FID' /><Value Type='Text'>" + fId + "</Value></Eq></Where>";
                                var items = sdl.GetItems(query);

                                SPListItem item = null;

                                if (items != null && items.Count > 0)
                                {
                                    item = items[0];
                                    if (item.FileSystemObjectType == SPFileSystemObjectType.Folder)
                                    {
                                        isHaveFolder = true;
                                        continue;
                                    }
                                    SPFile file = item.File;

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
                                            isHaveRms = true;
                                            continue;
                                        }
                                        else
                                        {
                                            isHaveFile = true;
                                            CommonHelper.SetFileDown(item.ID);
                                            file = rmsFile;
                                        }
                                    }
                                    else
                                    {
                                        isHaveFile = true;
                                        CommonHelper.SetFileDown(item.ID);
                                    }

                                    string path = folderPath + @"\" + file.Name;
                                    try
                                    {
                                        FileInfo eFile = new FileInfo(path);
                                        if (eFile.Exists)
                                        {
                                            string fileName = file.Name;
                                            string suffix = Path.GetExtension(fileName);
                                            fileName = fileName.Replace(suffix, "");
                                            path = folderPath + @"\" + fileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + suffix;
                                        }
                                    }
                                    catch { }
                                    FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
                                    byte[] fileByte = file.OpenBinary(SPOpenBinaryOptions.Unprotected);
                                    fs.Write(fileByte, 0, fileByte.Length);
                                    fs.Flush();
                                    fs.Close();
                                }
                                else
                                {
                                    isHaveDelete = true;
                                }
                            }
                            if (isHaveFile)
                            {
                                //zip file
                                string zipName = sdl.Title + time + ".zip";
                                string zipPath = folder + zipName;
                                BHCreateZipFile(folderPath, zipPath);
                                Directory.Delete(folderPath, true);
                                string downloadUrl = site.Url + @"/_layouts/15/DownLoadFiles/" + zipName;
                                web.AllowUnsafeUpdates = false;
                                model.url = downloadUrl;
                                model.name = zipName;

                                if (isHaveRms || isHaveFolder || isHaveDelete)
                                {
                                    // 部分不能下载
                                    model.msg = "1";
                                }
                                else
                                {
                                    model.msg = "0";
                                }
                            }
                            else
                            {
                                // 都不能下载
                                model.msg = "2";
                            }
                        }
                    }
                }
                else
                {
                    model.msg = "3";
                }
                return jsonSerializer.Serialize(model);
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("BHMulDownload.aspx__Page_Load", ex.Message);
                return jsonSerializer.Serialize("[]");
            }
        }

        /// <summary>
        /// 创建压缩文件
        /// </summary>
        /// <param name="filesPath">文件路径</param>
        /// <param name="zipFilePath">压缩文件路径</param>
        private static void BHCreateZipFile(string filesPath, string zipFilePath)
        {
            try
            {
                string[] filenames = Directory.GetFiles(filesPath);
                using (ZipOutputStream s = new ZipOutputStream(File.Create(zipFilePath)))
                {
                    s.SetLevel(5); // 压缩级别 0-9
                    //s.Password = "123"; //Zip压缩文件密码
                    byte[] buffer = new byte[4096]; //缓冲区大小
                    foreach (string file in filenames)
                    {
                        ZipEntry entry = new ZipEntry(Path.GetFileName(file));
                        entry.IsUnicodeText = true;
                        entry.DateTime = DateTime.Now;
                        s.PutNextEntry(entry);
                        using (FileStream fs = File.OpenRead(file))
                        {
                            int sourceBytes;
                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                s.Write(buffer, 0, sourceBytes);
                            } while (sourceBytes > 0);
                        }
                    }
                    s.Finish();
                    s.Close();
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("BHMulDownload.aspx__CreateZipFile", ex.Message);
            }
        }
    }
}
