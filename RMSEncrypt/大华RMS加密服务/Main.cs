using Common;
using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace 大华RMS加密服务
{
    public partial class Main : System.Windows.Forms.Form
    {
        /// <summary>
        /// 加密进程
        /// </summary>
        BackgroundWorker bwTask = new BackgroundWorker();

        /// <summary>
        /// 计时进程
        /// </summary>
        BackgroundWorker bwTiming = new BackgroundWorker();

        /// <summary>
        /// 本次运行已加密总数量
        /// </summary>
        int totalCount = 0;

        /// <summary>
        /// 加密正确数量
        /// </summary>
        int rightCount = 0;

        /// <summary>
        /// 加密出错数量
        /// </summary>
        int errorCount = 0;

        /// <summary>
        /// 执行次数
        /// </summary>
        int RunCount = 0;

        public Main()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            this.ControlBox = false;
            this.labCurrentStatus.Text = "程序正在启动中。";
            EncryptFile.KillOfficeProcess();
            StartTiming();

            StartTask();
        }

        #region 后台线程（计时任务）
        /// <summary>
        /// 计时任务初始化
        /// </summary>
        private void StartTiming()
        {
            using (bwTiming)
            {
                bwTiming.WorkerReportsProgress = true;
                bwTiming.RunWorkerCompleted += bwTiming_RunWorkerCompleted;
                bwTiming.DoWork += bwTiming_DoWork;
                bwTiming.RunWorkerAsync();
            }
        }

        /// <summary>
        /// 计时任务开始执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void bwTiming_DoWork(object sender, DoWorkEventArgs e)
        {
            DateTime startTime = DateTime.Now;
            while (true)
            {
                DateTime nowTime = DateTime.Now;
                TimeSpan ts = nowTime - startTime;
                string time = "服务已运行" + ts.Days + "天" + ts.Hours + "小时" + ts.Minutes + "分钟" + ts.Seconds + "秒";
                this.tslRunTime.Text = time;
                Thread.CurrentThread.Join(1000);
            }
        }

        /// <summary>
        /// 计时任务结束
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void bwTiming_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bwTiming.Dispose();
            throw new NotImplementedException();
        }

        #endregion

        #region 后台线程（加密任务）

        /// <summary>
        /// 加密任务初始化
        /// </summary>
        private void StartTask()
        {
            using (bwTask)
            {
                bwTask.WorkerReportsProgress = true;
                bwTask.RunWorkerCompleted += bwTask_RunWorkerCompleted;
                bwTask.DoWork += bwTask_DoWork;
                bwTask.RunWorkerAsync();
            }
        }

        /// <summary>
        /// 加密任务开始执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void bwTask_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                while (true)
                {
                    RunCount++;
                    this.labRunCount.Text = RunCount + "次";
                    this.labCurrentStatus.Text = "正在连接文档库。";
                    using (ClientContext context = GetClientContext())
                    {
                        Web web = context.Web;
                        CamlQuery query = new CamlQuery();
                        //query.ViewXml = "<Query><Where><Eq><FieldRef Name='State' /><Value Type='Boolean'>0</Value></Eq></Where></Query>";
                        //query.ViewXml = "<View><Eq><FieldRef Name=\"State\" /><Value Type=\"Boolean\">false</Value></Eq></View>";
                        string title = ConfigurationSettings.AppSettings["RMSTask"];
                        var items = web.Lists.GetByTitle(title).GetItems(query);
                        context.Load(items);
                        context.ExecuteQuery();
                        int s = items.Count;
                        for (int i = 0; i < items.Count; i++)
                        {
                            if (items[i] != null)
                            {
                                context.Load(items[i]);
                                string tmpTitle = Common.Helper.ObjToStr(items[i]["Title"]);
                                this.labCurrentStatus.Text = "正在加密《" + tmpTitle + "》。";
                                string tmpState = Common.Helper.ObjToStr(items[i]["State"]);
                                if (!string.IsNullOrEmpty(tmpState) && tmpState.ToLower() == "false")
                                {
                                    totalCount++;
                                    HandleItem(items[i]);

                                    this.labTotalCount.Text = totalCount.ToString() + "个";
                                }
                            }
                        }
                        //foreach (ListItem item in items)
                        //{
                        //    if (item != null)
                        //    {
                        //        string tmpTitle = Common.Helper.ObjToStr(item["Title"]);
                        //        this.labCurrentStatus.Text = "正在加密《" + tmpTitle + "》。";
                        //        string tmpState = Common.Helper.ObjToStr(item["State"]);
                        //        if (!string.IsNullOrEmpty(tmpState) && tmpState.ToLower() == "false")
                        //        {
                        //            totalCount++;
                        //            HandleItem(item);
                        //            this.labTotalCount.Text = totalCount.ToString() + "个";
                        //        }
                        //    }
                        //}
                        this.labCurrentStatus.Text = "本轮执行完成，等待中。";
                        int CustomTS = Convert.ToInt32(this.tbxTimeSpan.Text) * 60000;//循环执行间隔时间
                        Thread.Sleep(CustomTS);
                    }
                }
            }
            catch (Exception ex)
            {
                SetLog("Exception Message:" + ex.Message);//记录日志
                EncryptFile.KillOfficeProcess();//清理Office进程
                bwTask.Dispose();
                bwTiming.Dispose();
                Thread.Sleep(60000);//等待一分钟
                System.Diagnostics.Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);  //重新开启当前程序
                Close();//关闭当前程序
                //System.Windows.Forms.Application.Restart();//遇到异常自动重启
            }
        }

        /// <summary>
        /// 加密任务结束
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void bwTask_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bwTask.Dispose();
            //throw new NotImplementedException();
        }

        #endregion

        #region 加密任务的执行操作
        /// <summary>
        /// 处理一条加密任务
        /// </summary>
        /// <param name="item">ListItem</param>
        private bool HandleItem(ListItem item)
        {

            bool currentItemStatus = false;
            RMSFileModel fileModel = new RMSFileModel();
            //string _fid = Common.Helper.ObjToStr(item["FID"]);
            //if (_fid == "123546876544324523")
            //{
            //    fileModel.Users = Common.Helper.ObjToFieldUserValues(item["Users"]);
            //}
            //else
            //{
            //    return false;
            //}

            fileModel.Author = Common.Helper.ObjToStr(Common.Helper.ObjToFieldUserValue(item["Author"]).LookupValue);
            fileModel.ID = Common.Helper.ObjToInt(item["ID"]);
            fileModel.FID = Common.Helper.ObjToStr(item["FID"]);
            fileModel.FTitle = Common.Helper.ObjToStr(item["Title"]);
            fileModel.FUrlSourcePath = Common.Helper.ObjToStr(item["SourcePath"]);
            fileModel.FUrlRMSPath = Common.Helper.ObjToStr(item["RMSPath"]);
            fileModel.FIsFullControl = Common.Helper.ObjToBool(item["IsFullControl"]);
            fileModel.FIsRead = Common.Helper.ObjToBool(item["IsRead"]);
            fileModel.FIsPrint = Common.Helper.ObjToBool(item["IsPrint"]);
            fileModel.FIsSave = Common.Helper.ObjToBool(item["IsSave"]);
            fileModel.FIsEdit = Common.Helper.ObjToBool(item["IsEdit"]);
            fileModel.FState = Common.Helper.ObjToStr(item["State"]);
            fileModel.FContent = Common.Helper.ObjToStr(item["Content"]);
            fileModel.Users = Common.Helper.ObjToFieldUserValues(item["Users"]);
            fileModel.AuthorEmail = Common.Helper.GetFieldUserValueEmail(Common.Helper.ObjToFieldUserValue(item["Author"]));
            fileModel.UploadTime = Common.Helper.ObjToStr(DateTime.Parse(item["Created"].ToString()).ToLocalTime());
            EncryptFile.KillOfficeProcess();//清理Office进程
            bool isUrl = GetFileFromSP(fileModel);//从SharePoint下载文件
            if (!hasRMSPermission(fileModel))
            {
                bool isDeleted = UpdateDistinctSP(fileModel);//删除当前加密任务
                return currentItemStatus;
            }
            if (!isUrl)
            {
                SetLog(fileModel);
            }
            else
            {
                string path = "";
                bool isPath = isCreateDirectory(fileModel, ref path);//创建本地加密文件夹
                if (!isPath)
                {
                    SetLog(fileModel);
                }
                else
                {
                    string type = "null";
                    bool isExt = isExtension(fileModel, ref type);//判断格式
                    if (!isExt)
                    {
                        SetLog(fileModel);
                    }
                    else
                    {
                        EncryptFile.KillOfficeProcess();//清理Office进程
                        bool isRms = false;
                        if (type == "docx")
                        {
                            isRms = Common.EncryptFile.EncryptWord(fileModel, path);//加密word
                        }
                        else if (type == "xlsx")
                        {
                            isRms = Common.EncryptFile.EncryptExcel(fileModel, path);//加密excel
                        }
                        else if (type == "pptx")
                        {
                            isRms = Common.EncryptFile.EncryptPPT(fileModel, path);//加密ppt
                        }
                        else
                        {
                            isRms = false;
                        }
                        if (!isRms)
                        {
                            if (fileModel.IsFalse == true)
                            {
                                try
                                {
                                    EncryptFile.KillOfficeProcess();//清理Office进程
                                                                    //上传源文件至RMS库
                                    string relativeFolderPath = Helper.UrlToPath(fileModel.FUrlSourcePath);
                                    string creationResult = CreateUploadFolderPath(relativeFolderPath);
                                    bool isUpload = UploadRMSErrorDoc(fileModel, creationResult);//将本身已加密的文件上传到加密文档库
                                    DeleteFile(fileModel);//清空临时文件

                                }
                                catch (Exception ex)
                                {

                                }
                            }
                            SetLog(fileModel);
                        }
                        else
                        {
                            EncryptFile.KillOfficeProcess();//清理Office进程
                            string relativeFolderPath = Helper.UrlToPath(fileModel.FUrlSourcePath);
                            string creationResult = CreateUploadFolderPath(relativeFolderPath);
                            bool isUpload = UploadDoc(fileModel, creationResult);//将加密后的文件上传到加密文档库
                            if (!isUpload)
                            {
                                SetLog(fileModel);
                            }
                            else
                            {
                                bool isInsert = InsertSP(fileModel);//新增一条历史记录
                                if (!isInsert)
                                {
                                    SetLog(fileModel);
                                }
                                else
                                {
                                    bool isUpdata = UpdateSP(fileModel);//删除当前加密任务
                                    if (!isUpdata)
                                    {
                                        SetLog(fileModel);
                                    }
                                    else
                                    {
                                        rightCount++;
                                        this.labRightCount.Text = rightCount.ToString() + "个";
                                        DeleteFile(fileModel);//清空临时文件
                                        currentItemStatus = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return currentItemStatus;
        }

        private bool hasRMSPermission(RMSFileModel filemodel)
        {
            try
            {
                if (!filemodel.FIsEdit && !filemodel.FIsFullControl && !filemodel.FIsPrint && !filemodel.FIsRead && !filemodel.FIsSave)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 创建本地加密文件夹
        /// </summary>
        /// <param name="fileModel">RMSFileModel</param>
        /// <returns>创建结果</returns>
        private bool isCreateDirectory(RMSFileModel fileModel, ref string path)
        {
            try
            {
                //确认路径及创建文件夹
                path = System.Windows.Forms.Application.StartupPath + "\\App_Data\\RMSFile\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return true;
            }
            catch (Exception ex)
            {
                fileModel.FErrorMessage = "创建本地加密文件夹";
                fileModel.FContent = ex.Message;
                fileModel.IsFalse = false;
                return false;
            }
        }

        /// <summary>
        /// 判断格式
        /// </summary>
        /// <param name="fileModel"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool isExtension(RMSFileModel fileModel, ref string type)
        {
            try
            {
                string extension = Path.GetExtension(fileModel.FSourcePath).ToLower();//获取扩展名
                if (extension.IndexOf("docx") > -1)
                {
                    type = "docx";
                }
                else if (extension.IndexOf("xlsx") > -1)
                {
                    type = "xlsx";
                }
                else if (extension.IndexOf("ppt") > -1)
                {
                    type = "pptx";
                }
                else
                {
                    type = "null";
                    fileModel.FErrorMessage = "判断格式";
                    fileModel.FContent = "文件后缀名为：" + extension + "，此格式无法加密，只能加密（docx,xlsx,pptx）格式的文件";
                    fileModel.IsFalse = true;
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                fileModel.FErrorMessage = "判断格式";
                fileModel.FContent = ex.Message;
                fileModel.IsFalse = true;
                return false;
            }
        }

        /// <summary>
        /// 执行加密操作
        /// </summary>
        /// <param name="fileModel">文件属性</param>
        /// <returns>加密结果</returns>
        private bool HandleRMS(RMSFileModel fileModel)
        {
            try
            {
                //确认路径及创建文件夹
                string path = System.Windows.Forms.Application.StartupPath + "\\App_Data\\RMSFile\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                //清理Office进程
                EncryptFile.KillOfficeProcess();
                string extension = Path.GetExtension(fileModel.FSourcePath).ToLower();//获取扩展名
                if (extension.IndexOf("doc") > -1)
                {
                    return Common.EncryptFile.EncryptWord(fileModel, path);
                }
                if (extension.IndexOf("xls") > -1)
                {
                    return Common.EncryptFile.EncryptExcel(fileModel, path);
                }
                if (extension.IndexOf("ppt") > -1)
                {
                    return Common.EncryptFile.EncryptPPT(fileModel, path);
                }
                fileModel.FErrorMessage = "格式错误";
                fileModel.FContent = "无法加密该格式的文件";
                return false;
            }
            catch (Exception ex)
            {
                fileModel.FErrorMessage = "获取文件扩展名错误";
                fileModel.FContent = ex.Message;
                return false;
            }
        }

        ///// <summary>
        ///// 下载文件
        ///// </summary>
        ///// <param name="path">SharePoint文档库的文件url</param>
        ///// <returns>本地源文件路径</returns>
        //private bool GetFileFromSP(RMSFileModel fileModel)
        //{
        //    try
        //    {
        //        using (ClientContext context = GetClientContext())
        //        {
        //            Web web = context.Web;
        //            CamlQuery fquery = new CamlQuery();
        //            var documents = web.Lists.GetByTitle(ConfigurationSettings.AppSettings["SourceDoc"].ToString());

        //            fquery.ViewXml = "<View Scope=\"RecursiveAll\"><Query><Where><Eq><FieldRef Name='FID' /><Value Type='Text'>" + fileModel.FID + "</Value></Eq></Where></Query></View>";
        //            // 获取文档库Item
        //            var fItems = documents.GetItems(fquery);
        //            context.Load(fItems);
        //            //context.Load(fItems[0]);
        //            context.ExecuteQuery();

        //            using (WebClient wc = new WebClient())
        //            {
        //                //EncryptFile.KillOfficeProcess();
        //                wc.Credentials = GetCredential();
        //                byte[] fileContents = wc.DownloadData(ConfigurationSettings.AppSettings["URL"].ToString() + fItems[0].FieldValues["FileRef"].ToString());
        //                string strFileName = Path.GetFileName(fItems[0].FieldValues["FileRef"].ToString());
        //                string path = System.Windows.Forms.Application.StartupPath + "\\App_Data\\SourceFile\\";
        //                string filePath = path + strFileName;
        //                if (!Directory.Exists(path))
        //                {
        //                    Directory.CreateDirectory(path);
        //                }
        //                DeleteFile(fileModel);
        //                FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        //                fs.Write(fileContents, 0, fileContents.Length);
        //                fs.Flush();
        //                fs.Close();
        //                fileModel.FSourcePath = filePath;
        //                return true;
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.Message.Contains("cannot") && ex.Message.Contains("access"))
        //        {
        //            fileModel.FErrorMessage = "下载文件";
        //            fileModel.FContent = ex.Message;
        //            fileModel.IsFalse = false;
        //            EncryptFile.KillOfficeProcess();
        //        }
        //        else
        //        {
        //            fileModel.FErrorMessage = "下载文件";
        //            fileModel.FContent = ex.Message;
        //            fileModel.IsFalse = true;
        //        }
        //        return false;
        //    }
        //}

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="path">SharePoint文档库的文件url</param>
        /// <returns>本地源文件路径</returns>
        private bool GetFileFromSP(RMSFileModel fileModel)
        {
            try
            {
                using (ClientContext context = GetClientContext())
                {
                    Web web = context.Web;
                    CamlQuery fquery = new CamlQuery();
                    var documents = web.Lists.GetByTitle(ConfigurationSettings.AppSettings["SourceDoc"].ToString());

                    fquery.ViewXml = "<View Scope=\"Recursive\"><Query><Where><Eq><FieldRef Name='FID' /><Value Type='Text'>" + fileModel.FID + "</Value></Eq></Where></Query></View>";
                    // 获取文档库Item
                    var fItems = documents.GetItems(fquery);
                    context.Load(fItems);
                    context.ExecuteQuery();

                    if (fItems != null && fItems.Count > 0)
                    {
                        using (WebClient wc = new WebClient())
                        {
                            wc.Credentials = GetCredential();

                            byte[] fileContents = wc.DownloadData(ConfigurationSettings.AppSettings["URL"].ToString() + fItems[0].FieldValues["FileRef"].ToString());
                            string strFileName = Path.GetFileName(fItems[0].FieldValues["FileRef"].ToString());
                            string path = Application.StartupPath + "\\App_Data\\SourceFile\\";
                            string filePath = path + strFileName;
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            DeleteFile(fileModel);
                            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                            fs.Write(fileContents, 0, fileContents.Length);
                            fs.Flush();
                            fs.Close();
                            fileModel.FSourcePath = filePath;
                            return true;
                        }
                    }
                    else
                    {
                        fileModel.FErrorMessage = "下载文件";
                        fileModel.FContent = "文件不存在, FID=" + fileModel.FID;
                        fileModel.IsFalse = true;
                        EncryptFile.KillOfficeProcess();
                        return false;
                    }
                }

            }
            catch (Exception ex)
            {
                fileModel.FErrorMessage = "下载文件";
                fileModel.FContent = ex.Message;
                fileModel.IsFalse = false;
                EncryptFile.KillOfficeProcess();
                return false;
            }
        }


        #endregion


        #region 完成加密后的操作

        private string CreateUploadFolderPath(string folderPath)
        {

            //try
            //{
            string[] pathArray = folderPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string currentPath = GeneratorFolderPath(pathArray);
            using (ClientContext context = GetClientContext())
            {
                Web web = context.Web;
                List list = context.Web.Lists.GetByTitle("RMSDocuments");
                Folder rmsRootFolder = list.RootFolder;
                //var rmsRootFolder = web.GetFolderByServerRelativeUrl("/rmsdocuments");
                context.Load(rmsRootFolder);
                context.ExecuteQuery();
                //for (int i = 0; i < pathArray.Length; i++)
                //{
                //    string currentPath = GeneratorFolderPath(pathArray, i);

                try
                {
                    var rmsCurrentFolder = web.GetFolderByServerRelativeUrl(currentPath);//"/documents/product/test"
                    context.Load(rmsCurrentFolder);
                    context.ExecuteQuery();

                }
                catch (Exception ex)
                {
                    rmsRootFolder.Folders.Add(currentPath);
                    rmsRootFolder.Update();
                    //context.Load(rmsRootFolder);
                    context.ExecuteQuery();
                }

                //}
            }
            return currentPath;
            //}
            //catch (Exception ex)
            //{
            //    return currentPath;
            //}
        }

        private string GeneratorFolderPath(string[] pathArray)
        {
            string currentPath = "";
            for (int i = 0; i < pathArray.Length; i++)
            {
                if (i < pathArray.Length - 1)
                {
                    if (pathArray[i].Length > 5)
                        currentPath += pathArray[i].Substring(0, 5).Trim() + "-";
                    else
                        currentPath += pathArray[i].Trim() + "-";
                }
                else
                {
                    if (pathArray[i].Length > 5)
                        currentPath += pathArray[i].Substring(0, 5).Trim();
                    else
                        currentPath += pathArray[i].Trim();
                }
            }

            return currentPath.Trim();
        }

        /// <summary>
        /// 将加密后的文档上传到SharePoint加密文档库
        /// </summary>
        /// <param name="filemodel">文件属性</param>
        /// <returns>上传是否成功</returns>
        private bool UploadDoc(RMSFileModel filemodel, string folderPath)
        {
            try
            {

                string FUrlRMSPath = ConfigurationSettings.AppSettings["URL"] + "/" + ConfigurationSettings.AppSettings["RMSDocUrl"].ToString() + "/" + folderPath + "/" + filemodel.FTitle;
                string FUrlRMSFolderPath = ConfigurationSettings.AppSettings["URL"] + "/" + ConfigurationSettings.AppSettings["RMSDocUrl"].ToString() + "/" + folderPath;



                //var result = CreateUploadFolderPath(p);
                string direct = @filemodel.FRMSPath;


                //**************
                //WebClient client = new WebClient();
                //client.Credentials = GetCredential();
                //client.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
                //client.Headers.Add(HttpRequestHeader.ContentType, "application/json;odata=verbose");
                //client.Headers.Add(HttpRequestHeader.Accept, "application/json;odata=verbose");
                //using (FileStream fs = new FileStream(direct, FileMode.Open, FileAccess.Read))
                //{
                //    int fslen = (int)fs.Length;
                //    byte[] bytes = new byte[fslen];
                //    int length = fs.Read(bytes, 0, bytes.Length);
                //    client.UploadData(FUrlRMSPath, "PUT", bytes);
                //    FUrlRMSPath = FUrlRMSPath.Replace(" ", "%20");
                //    filemodel.FUrlRMSPath = FUrlRMSPath;
                //    filemodel.FContent = "文件已加密，并成功上传到指定文档库";
                //}
                //*******************


                using (ClientContext context = GetClientContext())
                {
                    Web web = context.Web;

                    FileCreationInformation fileCI = new FileCreationInformation();
                    //string direct = @filemodel.FRMSPath;
                    using (FileStream fs = new FileStream(direct, FileMode.Open, FileAccess.Read))
                    {
                        BinaryReader br = new BinaryReader(fs);


                        fileCI.ContentStream = fs;
                        fileCI.Overwrite = true;
                        //string FUrlRMSPath = ConfigurationSettings.AppSettings["URL"] + "/Lists/" + ConfigurationSettings.AppSettings["RMSDocUrl"] + Helper.UrlToPath(filemodel.FUrlSourcePath) + filemodel.FTitle;
                        //string FUrlRMSPath = ConfigurationSettings.AppSettings["URL"] + "/" + ConfigurationSettings.AppSettings["RMSDocUrl"] + Helper.UrlToPath(filemodel.FUrlSourcePath) + filemodel.FTitle;
                        fileCI.Url = FUrlRMSPath;
                        //var file = list.RootFolder.Files.Add(fileCI);
                        var files = web.GetFolderByServerRelativeUrl(FUrlRMSFolderPath).Files;
                        context.Load(files);
                        context.ExecuteQuery();

                        var file = files.Add(fileCI);

                        context.Load(file);
                        context.RequestTimeout = int.MaxValue;
                        context.ExecuteQuery();

                        var item = file.ListItemAllFields;
                        context.Load(item);
                        context.ExecuteQuery();

                        var fid = item.FieldValues["FID"];
                        if (fid == null || fid.ToString() == "")
                        {
                            item["FID"] = filemodel.FID;
                            item.Update();
                            context.ExecuteQuery();
                        }


                        //context.ExecuteQuery();
                        FUrlRMSPath = FUrlRMSPath.Replace(" ", "%20");
                        filemodel.FUrlRMSPath = FUrlRMSPath;
                        filemodel.FContent = "文件已加密，并成功上传到指定文档库";
                        fs.Close();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                filemodel.FErrorMessage = "上传加密文件";
                filemodel.FContent = "文件已加密，但上传时发生错误，错误详情：" + ex.Message;
                filemodel.IsFalse = false;
                return false;
            }
        }


        /// <summary>
        /// 将加密后的文档上传到SharePoint加密文档库
        /// </summary>
        /// <param name="filemodel">文件属性</param>
        /// <returns>上传是否成功</returns>
        private bool UploadRMSErrorDoc(RMSFileModel filemodel, string folderPath)
        {
            try
            {
                string FUrlRMSPath = ConfigurationSettings.AppSettings["URL"] + "/" + ConfigurationSettings.AppSettings["RMSDocUrl"].ToString() + "/" + folderPath + "/" + filemodel.FTitle;
                string FUrlRMSFolderPath = ConfigurationSettings.AppSettings["URL"] + "/" + ConfigurationSettings.AppSettings["RMSDocUrl"].ToString() + "/" + folderPath;



                //var result = CreateUploadFolderPath(p);
                string direct = @filemodel.FSourcePath;


                //**************
                //WebClient client = new WebClient();
                //client.Credentials = GetCredential();
                //client.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
                //client.Headers.Add(HttpRequestHeader.ContentType, "application/json;odata=verbose");
                //client.Headers.Add(HttpRequestHeader.Accept, "application/json;odata=verbose");
                //using (FileStream fs = new FileStream(direct, FileMode.Open, FileAccess.Read))
                //{
                //    int fslen = (int)fs.Length;
                //    byte[] bytes = new byte[fslen];
                //    int length = fs.Read(bytes, 0, bytes.Length);
                //    client.UploadData(FUrlRMSPath, "PUT", bytes);
                //    FUrlRMSPath = FUrlRMSPath.Replace(" ", "%20");
                //    filemodel.FUrlRMSPath = FUrlRMSPath;
                //    filemodel.FContent = "文件已加密，并成功上传到指定文档库";
                //}
                //*******************

                using (ClientContext context = GetClientContext())
                {
                    Web web = context.Web;

                    FileCreationInformation fileCI = new FileCreationInformation();
                    //string direct = @filemodel.FRMSPath;
                    using (FileStream fs = new FileStream(direct, FileMode.Open, FileAccess.Read))
                    {
                        BinaryReader br = new BinaryReader(fs);


                        fileCI.ContentStream = fs;
                        fileCI.Overwrite = true;
                        //string FUrlRMSPath = ConfigurationSettings.AppSettings["URL"] + "/Lists/" + ConfigurationSettings.AppSettings["RMSDocUrl"] + Helper.UrlToPath(filemodel.FUrlSourcePath) + filemodel.FTitle;
                        //string FUrlRMSPath = ConfigurationSettings.AppSettings["URL"] + "/" + ConfigurationSettings.AppSettings["RMSDocUrl"] + Helper.UrlToPath(filemodel.FUrlSourcePath) + filemodel.FTitle;
                        fileCI.Url = FUrlRMSPath;
                        //var file = list.RootFolder.Files.Add(fileCI);
                        var files = web.GetFolderByServerRelativeUrl(FUrlRMSFolderPath).Files;
                        context.Load(files);
                        context.ExecuteQuery();


                        var file = files.Add(fileCI);

                        context.Load(file);
                        context.RequestTimeout = int.MaxValue;
                        context.ExecuteQuery();

                        var item = file.ListItemAllFields;
                        context.Load(item);
                        context.ExecuteQuery();

                        var fid = item.FieldValues["FID"];
                        if (fid == null || fid.ToString() == "")
                        {
                            item["FID"] = filemodel.FID;
                            item.Update();
                            context.ExecuteQuery();
                        }



                        //context.ExecuteQuery();
                        FUrlRMSPath = FUrlRMSPath.Replace(" ", "%20");
                        filemodel.FUrlRMSPath = FUrlRMSPath;
                        filemodel.FContent = "文件已加密，并成功上传到指定文档库";
                        fs.Close();
                    }
                    //return true;
                }
                return true;
            }
            catch (Exception ex)
            {
                filemodel.FErrorMessage = "上传加密文件";
                filemodel.FContent = "文件已加密，但上传时发生错误，错误详情：" + ex.Message;
                filemodel.IsFalse = false;
                return false;
            }
        }

        /// <summary>
        /// 新增加密历史记录
        /// </summary>
        /// <param name="filemodel"></param>
        /// <returns></returns>
        private bool InsertSP(RMSFileModel filemodel)
        {
            try
            {
                using (ClientContext context = GetClientContext())
                {
                    filemodel.FContent = "文件已成功加密";
                    string title = ConfigurationSettings.AppSettings["RMSHistory"];
                    List list = context.Web.Lists.GetByTitle(title);
                    ListItemCreationInformation itemCI = new ListItemCreationInformation();
                    ListItem item = list.AddItem(itemCI);
                    item["Title"] = filemodel.FTitle;
                    item["SourcePath"] = filemodel.FUrlSourcePath;
                    item["RMSPath"] = filemodel.FUrlRMSPath;
                    item["IsFullControl"] = filemodel.FIsFullControl;
                    item["IsRead"] = filemodel.FIsRead;
                    item["IsPrint"] = filemodel.FIsPrint;
                    item["IsSave"] = filemodel.FIsSave;
                    item["IsEdit"] = filemodel.FIsEdit;
                    item["State"] = 1;
                    item["Content"] = filemodel.FContent;
                    item["Users"] = filemodel.Users;
                    item.Update();
                    context.ExecuteQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                filemodel.FErrorMessage = "更新历史记录";
                filemodel.FContent = "文件已加密，并成功上传至加密文档库，但是向历史记录里写入数据时发生错误，错误详情：" + ex.Message;
                filemodel.IsFalse = false;
                throw;
            }
        }

        /// <summary>
        /// 删除多余加密任务
        /// </summary>
        /// <param name="filemodel"></param>
        /// <returns></returns>
        private bool UpdateDistinctSP(RMSFileModel filemodel)
        {
            try
            {
                using (ClientContext context = GetClientContext())
                {
                    string title = ConfigurationSettings.AppSettings["RMSTask"];
                    List list = context.Web.Lists.GetByTitle(title);
                    ListItem item = list.GetItemById(filemodel.ID);
                    item.DeleteObject();
                    context.ExecuteQuery();
                    filemodel.FContent = "删除多余加密任务";
                    return true;
                }
            }
            catch (Exception ex)
            {
                filemodel.FErrorMessage = "删除加密任务";
                filemodel.FContent = "删除多余任务时发生错误，错误详情：" + ex.Message;
                filemodel.IsFalse = false;
                return false;
            }
        }


        /// <summary>
        /// 删除加密任务
        /// </summary>
        /// <param name="filemodel"></param>
        /// <returns></returns>
        private bool UpdateSP(RMSFileModel filemodel)
        {
            try
            {
                using (ClientContext context = GetClientContext())
                {
                    string title = ConfigurationSettings.AppSettings["RMSTask"];
                    List list = context.Web.Lists.GetByTitle(title);
                    ListItem item = list.GetItemById(filemodel.ID);
                    item.DeleteObject();
                    context.ExecuteQuery();
                    filemodel.FContent = "文件已加密，并成功上传到指定文档库和删除加密任务";
                    return true;
                }
            }
            catch (Exception ex)
            {
                filemodel.FErrorMessage = "删除加密任务";
                filemodel.FContent = "文件已加密，并成功上传到加密文档库，但是删除加密任务时发生错误，错误详情：" + ex.Message;
                filemodel.IsFalse = false;
                return false;
            }
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="filemodel"></param>
        private void SetLog(RMSFileModel filemodel)
        {
            errorCount++;
            this.labErrorCount.Text = errorCount.ToString() + "个";
            UpdateErrorFile(filemodel);
            DeleteFile(filemodel);
            XMLHelper xh = new XMLHelper();
            string path = System.Windows.Forms.Application.StartupPath + "\\App_Data\\LogFile\\";
            ErrorMessageModel emm = new ErrorMessageModel();
            emm.Content = filemodel.FContent;
            emm.Time = DateTime.Now.ToString();
            emm.Title = filemodel.FErrorMessage;
            emm.FilePath = filemodel.FUrlSourcePath;
            xh.CreateLog(emm, path);
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="errorMessage"></param>
        private void SetLog(string errorMessage)
        {
            XMLHelper xh = new XMLHelper();
            string path = System.Windows.Forms.Application.StartupPath + "\\App_Data\\LogFile\\";
            ErrorMessageModel emm = new ErrorMessageModel();
            emm.Content = errorMessage;
            emm.Time = DateTime.Now.ToString();
            emm.Title = "服务运行";
            emm.FilePath = "无";
            xh.CreateLog(emm, path);
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="errorMessage"></param>
        private void SetMailLog(string errorMessage)
        {
            XMLHelper xh = new XMLHelper();
            string path = System.Windows.Forms.Application.StartupPath + "\\App_Data\\LogFile\\";
            ErrorMessageModel emm = new ErrorMessageModel();
            emm.Content = errorMessage;
            emm.Time = DateTime.Now.ToString();
            emm.Title = "邮件发送";
            emm.FilePath = "无";
            xh.CreateLog(emm, path);
        }

        /// <summary>
        /// 删除临时文件
        /// </summary>
        /// <param name="filemodel"></param>
        private void DeleteFile(RMSFileModel filemodel)
        {
            EncryptFile.KillOfficeProcess();
            if (!String.IsNullOrEmpty(filemodel.FSourcePath))
            {
                System.IO.File.Delete(filemodel.FSourcePath);
            }
            if (!String.IsNullOrEmpty(filemodel.FRMSPath))
            {
                System.IO.File.Delete(filemodel.FRMSPath);
            }
        }

        /// <summary>
        /// 更新错误文件的任务状态
        /// </summary>
        /// <param name="filemodel"></param>
        private void UpdateErrorFile(RMSFileModel filemodel)
        {
            try
            {
                using (ClientContext context = GetClientContext())
                {
                    string title = ConfigurationSettings.AppSettings["RMSTask"];
                    List list = context.Web.Lists.GetByTitle(title);
                    ListItem item = list.GetItemById(filemodel.ID);
                    item["State"] = filemodel.IsFalse;
                    item["Content"] = filemodel.FContent;
                    item.Update();
                    context.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                XMLHelper xh = new XMLHelper();
                string path = System.Windows.Forms.Application.StartupPath + "\\App_Data\\LogFile\\";
                ErrorMessageModel emm = new ErrorMessageModel();
                emm.Content = ex.Message;
                emm.Time = DateTime.Now.ToString();
                emm.Title = "更新错误文件的任务状态";
                emm.FilePath = "无";
                xh.CreateLog(emm, path);
            }
        }

        #endregion

        #region SharePoint相关操作

        /// <summary>
        /// 获取SharePoint.Client.Web对象
        /// </summary>
        /// <returns>SharePoint.Client.Web对象</returns>
        private static Web GetWeb()
        {
            ClientContext context = GetClientContext();
            Web web = context.Web;
            return web;
        }

        /// <summary>
        /// 获取SharePoint客户端对象
        /// </summary>
        /// <returns>SharePoint客户端对象</returns>
        private static ClientContext GetClientContext()
        {
            string url = ConfigurationSettings.AppSettings["URL"];
            ClientContext context = new ClientContext(url);
            context.Credentials = GetCredential();
            return context;
        }

        /// <summary>
        /// 获取登陆凭据
        /// </summary>
        /// <returns>登陆凭据</returns>
        private static NetworkCredential GetCredential()
        {
            string UserName = ConfigurationSettings.AppSettings["UserName"];
            string UserPwd = ConfigurationSettings.AppSettings["UserPwd"];
            string Domain = ConfigurationSettings.AppSettings["Domain"];
            NetworkCredential credentials = new NetworkCredential(UserName, UserPwd, Domain);
            return credentials;
        }

        #endregion

        #region 窗口事件
        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            //e.Cancel = true;
            //this.ShowInTaskbar = false;
            //this.WindowState = FormWindowState.Minimized;
            //ExitService();
        }

        /// <summary>
        /// 托盘图标双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = true;  //显示在系统任务栏
                this.WindowState = FormWindowState.Normal;  //还原窗体
            }
        }

        /// <summary>
        /// 托盘右键事件（显示加密窗口）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 显示加密窗口ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = true;  //显示在系统任务栏
                this.WindowState = FormWindowState.Normal;  //还原窗体
            }
        }

        /// <summary>
        /// 托盘右键事件（隐藏加密窗口）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 隐藏加密窗口ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
        }
        /// <summary>
        /// 托盘右键事件（查看加密记录）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 查看加密记录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string url = ConfigurationSettings.AppSettings["RMSHistoryFullURL"];
            System.Diagnostics.Process.Start(url);
        }

        /// <summary>
        /// 托盘右键事件（查看加密任务）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 查看加密任务ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string url = ConfigurationSettings.AppSettings["RMSTaskFullURL"];
            System.Diagnostics.Process.Start(url);
        }

        /// <summary>
        /// 托盘右键事件（查看错误日志）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 查看错误日志ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = System.Windows.Forms.Application.StartupPath + "\\App_Data\\LogFile\\";
            System.Diagnostics.Process.Start(path);
        }

        /// <summary>
        /// 托盘右键事件（退出加密服务）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 退出加密服务ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ExitService();
        }

        private void ExitService()
        {
            if (MessageBox.Show("确定要关闭RMS加密服务吗？（关闭之后无法对SharePoint的文件进行RMS加密。）", "关闭加密服务", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                bwTask.Dispose();
                bwTiming.Dispose();
                Application.Exit();
            }
        }

        /// <summary>
        /// 只能输入数字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbxTimeSpan_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// 查看加密历史
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lkbViewHistory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = ConfigurationSettings.AppSettings["RMSHistoryFullURL"];
            System.Diagnostics.Process.Start(url);
        }

        /// <summary>
        /// 查看加密任务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lkbViewTask_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = ConfigurationSettings.AppSettings["RMSTaskFullURL"];
            System.Diagnostics.Process.Start(url);
        }

        /// <summary>
        /// 查看错误日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lkbViewError_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string path = System.Windows.Forms.Application.StartupPath + "\\App_Data\\LogFile\\";
            System.Diagnostics.Process.Start(path);
        }

        #endregion
    }
}
