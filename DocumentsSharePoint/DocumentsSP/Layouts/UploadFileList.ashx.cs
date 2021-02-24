using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using DocumentsSP.Helper;

namespace DocumentsSP.Layouts
{
    public partial class UploadFileList : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var form = context.Request.Form;
            var files = context.Request.Files;
            string msg = "";

            var isRms = ConvertToBool(form["file_rms_ckb"]);
            RmsModel rmsModel = new RmsModel();
            if (isRms)
            {
                rmsModel = GetRmsAttr(form["FullControl"], form["Read"], form["Print"], form["Save"], form["Edit"], form["RMSUsers"]);
            }

            var fileFolder = CommonHelper.DecodeUrl(form["FileFolder"]);
            var versionComments = form["_CheckinComment"];
            var fileDescription = form["FileDescription"];

            msg = UploadFiles(files, fileFolder, versionComments, fileDescription, isRms, rmsModel);

            context.Response.ContentType = "text/plain";
            context.Response.Write(msg);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="files">前台文件</param>
        /// <param name="folder">文件夹</param>
        /// <param name="versionComments">版本注释</param>
        /// <param name="fileDescription">文档说明</param>
        /// <param name="isRms">是否加密</param>
        /// <param name="rmsModel">加密属性</param>
        /// <returns>添加结果</returns>
        private string UploadFiles(HttpFileCollection files, string folder, string versionComments, string fileDescription, bool isRms, RmsModel rmsModel)
        {
            string msg = "";
            try
            {
                var siteId = SPContext.Current.Site.ID;
                using (SPSite site = new SPSite(siteId))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        web.AllowUnsafeUpdates = true;

                        var documents = web.Lists.TryGetList(CommonHelper.docListName);
                        var spFolder = web.GetFolder(folder);

                        for (int n = 0; n < files.Count; n++)
                        {
                            var file = files[n];
                            var spFile = spFolder.Files.Add(file.FileName, file.InputStream, true, versionComments, false);

                            var item = documents.GetItemByUniqueId(spFile.UniqueId);
                            item["FileDescription"] = fileDescription;

                            if (isRms && CommonHelper.IsRMSFile(item.Name))
                            {
                                item["IsFullControl"] = rmsModel.IsFullControl;
                                item["IsRead"] = rmsModel.IsRead;
                                item["IsSave"] = rmsModel.IsSave;
                                item["IsPrint"] = rmsModel.IsPrint;
                                item["IsEdit"] = rmsModel.IsEdit;

                                var userCount = rmsModel.RMSUserAccounts.Count;
                                //var userCount = 3;
                                if (userCount > 0)
                                {
                                    SPFieldUserValueCollection uvc = new SPFieldUserValueCollection();
                                    for (int m = 0; m < userCount; m++)
                                    {
                                        var ac = "i:0#.w|" + CommonHelper.Domain + "\\" + rmsModel.RMSUserAccounts[m];
                                        //var ac = "i:0#.w|" + "ikernal" + "\\" + "test0" + (m + 1);

                                        SPUser pu = web.EnsureUser(ac);
                                        uvc.Add(new SPFieldUserValue(web, pu.ID, pu.Name));
                                    }
                                    item["Users"] = uvc;
                                }
                            }
                            try
                            {
                                item.SystemUpdate(false);
                            }
                            catch
                            {
                                item.SystemUpdate(false);
                            }
                        }

                        web.AllowUnsafeUpdates = false;
                    }
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("UploadFileList.ashx__UploadFiles", ex.Message);
                msg = ex.Message;
            }

            return msg;
        }

        /// <summary>
        ///  CheckBox转Bool
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool ConvertToBool(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            else if ("on".Equals(str))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///  获取加密属性
        /// </summary>
        /// <returns></returns>
        public RmsModel GetRmsAttr(string fullcontrol, string read, string print, string save, string edit, string accounts)
        {
            RmsModel mo = new RmsModel();

            mo.IsFullControl = ConvertToBool(fullcontrol);
            mo.IsRead = ConvertToBool(read);
            mo.IsPrint = ConvertToBool(print);
            mo.IsSave = ConvertToBool(save);
            mo.IsEdit = ConvertToBool(edit);
            mo.RMSUserAccounts = accounts.Split(',').ToList();

            return mo;
        }

        public class RmsModel
        {
            public bool IsFullControl { get; set; }
            public bool IsRead { get; set; }
            public bool IsPrint { get; set; }
            public bool IsSave { get; set; }
            public bool IsEdit { get; set; }
            public List<string> RMSUserAccounts { get; set; }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
