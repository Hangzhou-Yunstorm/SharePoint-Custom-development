using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using DocumentsSP.Helper;
using DocumentsSP.Model;
using System.Web.Script.Serialization;
using System.Web.Services;

namespace DocumentsSP.Layouts
{
    public partial class MulDownload : LayoutsPageBase
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
        public static string DownLoadZIP(string listId)
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
                            //bool isHaveFolder = false;
                            bool isHaveRms = false;
                            web.AllowUnsafeUpdates = true;

                            string folder = CommonHelper.ZipFilePath;

                            // delete old file
                            CommonHelper.DeleteZipFile(folder);

                            string[] fileIds = listId.Split(',');
                            var sdl = web.Lists.TryGetList(CommonHelper.docListName);
                            var rmsfileList = web.Lists.TryGetList(CommonHelper.docRMSListName);

                            //create the files folder under Downloads\Files
                            string time = DateTime.Now.ToString("yyyyMMddHHmmss");

                            string folderPath = folder + sdl.Title + time;
                            Directory.CreateDirectory(folderPath);

                            //download the files from library
                            for (int i = 0; i < fileIds.Length - 1; i++)
                            {
                                var id = Convert.ToInt32(fileIds[i]);
                                var item = sdl.GetItemById(id);
                                if (item != null)
                                {
                                    if (item.FileSystemObjectType == SPFileSystemObjectType.Folder)
                                    {
                                        //isHaveFolder = true;
                                        //continue;
                                        DownloadFolder(folderPath, rmsfileList, item, ref isHaveFile, ref isHaveRms);
                                    }
                                    else
                                    {
                                        SPFile file = item.File;
                                        if (CommonHelper.IsRMS(item))
                                        {
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
                                                isHaveRms = true;
                                                continue;
                                            }
                                            else
                                            {
                                                isHaveFile = true;
                                                CommonHelper.SetFileDown(id);
                                                file = rmsFile;
                                            }
                                        }
                                        else
                                        {
                                            isHaveFile = true;
                                            CommonHelper.SetFileDown(id);
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
                                }
                            }
                            if (isHaveFile)
                            {
                                //zip file
                                string zipName = sdl.Title + time + ".zip";
                                string zipPath = folder + zipName;
                                ZipOutputStream zos = new ZipOutputStream(File.Create(zipPath));
                                zos.SetLevel(5); // 压缩级别 0-9
                                ZipDirectory(folderPath, zos, "", folderPath + @"\");
                                zos.Finish();
                                zos.Close();
                                Directory.Delete(folderPath, true);
                                string downloadUrl = site.Url + @"/_layouts/15/DownLoadFiles/" + zipName;
                                web.AllowUnsafeUpdates = true;
                                model.url = downloadUrl;
                                model.name = zipName;
                                if (isHaveRms)
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
                CommonHelper.SetErrorLog("MulDownload.aspx__Page_Load", ex.Message);
                return jsonSerializer.Serialize("[]");
            }
        }

        /// <summary>
        /// 下载文件夹
        /// </summary>
        /// <param name="path">根路径</param>
        /// <param name="web">SPWeb</param>
        /// <param name="item">SPListItem</param>
        private static void DownloadFolder(string fpath, SPList rmsfileList, SPListItem folderItem, ref bool isHaveFile, ref bool isHaveRms)
        {
            try
            {
                // 创建文件夹
                string folderPath = fpath + "\\" + folderItem.Name;
                Directory.CreateDirectory(folderPath);

                // 子文件夹
                if (folderItem.Folder.SubFolders.Count > 0)
                {
                    foreach (SPFolder folder in folderItem.Folder.SubFolders)
                    {
                        DownloadFolder(folderPath, rmsfileList, folder.Item, ref isHaveFile, ref isHaveRms);
                    }
                }

                // 文件
                if (folderItem.Folder.Files.Count > 0)
                {
                    foreach (SPFile ffile in folderItem.Folder.Files)
                    {
                        var file = ffile;
                        var item = file.Item;
                        var id = item.ID;
                        if (CommonHelper.IsRMS(item))
                        {
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
                                isHaveRms = true;
                                continue;
                            }
                            else
                            {
                                isHaveFile = true;
                                CommonHelper.SetFileDown(id);
                                file = rmsFile;
                            }
                        }
                        else
                        {
                            isHaveFile = true;
                            CommonHelper.SetFileDown(id);
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
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("MulDownload.aspx__DownloadFolder", ex.Message);
            }

        }

        /// <summary>   
        /// 递归压缩文件夹
        /// </summary>   
        /// <param name="folderToZip">要压缩的文件夹路径</param>   
        /// <param name="zipStream">压缩输出流</param>   
        /// <param name="parentFolder">文件所属文件夹</param>   
        /// <param name="rootFolder">文件夹根目录</param>   
        private static void ZipDirectory(string folderToZip, ZipOutputStream s, string parentFolder, string rootFolder)
        {
            ZipEntry entry = null;
            FileStream fs = null;
            try
            {
                if (!string.IsNullOrEmpty(parentFolder))
                {
                    //创建当前文件夹
                    entry = new ZipEntry(Path.Combine(parentFolder.Replace(rootFolder, "") + "/")); //加上 “/” 才会当成是文件夹创建
                    s.PutNextEntry(entry);
                    s.Flush();
                }
                //先压缩文件，再递归压缩文件夹
                var filenames = Directory.GetFiles(folderToZip);
                foreach (string file in filenames)
                {
                    //打开压缩文件
                    fs = File.OpenRead(file);
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    if (string.IsNullOrEmpty(parentFolder))
                    {
                        entry = new ZipEntry(Path.GetFileName(file));
                    }
                    else
                    {
                        entry = new ZipEntry(Path.Combine(parentFolder.Replace(rootFolder, "") + "/" + Path.GetFileName(file)));
                    }
                    entry.IsUnicodeText = true;
                    entry.DateTime = DateTime.Now;
                    entry.Size = fs.Length;
                    fs.Close();
                    s.PutNextEntry(entry);
                    s.Write(buffer, 0, buffer.Length);
                }

                var folders = Directory.GetDirectories(folderToZip);
                foreach (string folder in folders)
                {
                    ZipDirectory(folder, s, folder, rootFolder);
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("MulDownload.aspx__ZipDirectory", ex.Message);
            }
        }


        /// 批量下载（打包成zip文件）
        /// </summary>
        /// <param name="filesPath"></param>
        /// <param name="zipFilePath"></param>
        private static void CreateZipFile(string filesPath, string zipFilePath)
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
                CommonHelper.SetErrorLog("MulDownload.aspx__CreateZipFile", ex.Message);
            }
        }

    }
}
