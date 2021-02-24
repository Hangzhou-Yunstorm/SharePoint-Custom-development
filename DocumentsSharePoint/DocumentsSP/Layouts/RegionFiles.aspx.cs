using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using DocumentsSP.Helper;
using System.Web.Services;
using System.Collections.Generic;
using DocumentsSP.Model;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Linq;

namespace DocumentsSP.Layouts
{
    public partial class RegionFiles : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 获取区域文件列表
        /// </summary>
        [WebMethod]
        public static string GetRegionFilesJson()
        {
            string json = "[]";
            try
            {
                var siteId = SPContext.Current.Site.ID;
                using (SPSite site = new SPSite(siteId))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        //var fList = web.Lists.TryGetList(CommonHelper.docListName);
                        SPFolder folder = web.GetFolder("/" + CommonHelper.RegionPath);

                        if (folder == null || !folder.Exists)
                        {
                            return json;
                        }

                        //var query = new SPQuery();
                        //query.ViewAttributes = "Scope=\"Recursive\"";
                        //query.Query = "<OrderBy><FieldRef Name=\"Modified\" Ascending=\"False\"></FieldRef></OrderBy>";
                        //query.RowLimit = 100;
                        //query.Folder = folder;
                        //var myS = fList.GetItems(query);

                        List<FileModel> models = new List<FileModel>();

                        var files = CommonHelper.GetRegionFilesByFolder(folder, 100);

                        foreach (SPFile file in files)
                        {
                            FileModel model = new FileModel();
                            string fileUrl = CommonHelper.EncodeUrl(file.ServerRelativeUrl);

                            model.ID = file.Item.ID;
                            model.FID = file.Item["FID"] == null ? "" : file.Item["FID"].ToString();
                            model.Department = CommonHelper.GetSubGroupName(file.Author);
                            model.Created = Convert.ToDateTime(file.Item["Modified"]).ToString("yyyy-MM-dd HH:mm:ss");
                            model.Name = "<a href=\"/_layouts/15/FileDetail.aspx?Url=" + fileUrl + "\" target=\"_blank\" >" + file.Name + "</a>";
                            model.Icon = "<img class=\"image\" src=\"/_layouts/15/IMAGES/" + file.IconUrl + "\">";
                            model.FileSize = CommonHelper.GetSize(file.Length);
                            Get3Count(model);
                            string url = CommonHelper.EncodeUrl(file.ParentFolder.Url);
                            model.ParentFolder = "<a href=\"/" + url + "\" target=\"_blank\">Folder</a>";
                            model.CreatedBy = file.ModifiedBy.Name;

                            models.Add(model);

                        }
                        DataContractJsonSerializer dcs = new DataContractJsonSerializer(models.GetType());
                        //序列化
                        using (MemoryStream stream = new MemoryStream())
                        {
                            dcs.WriteObject(stream, models);
                            json = Encoding.UTF8.GetString(stream.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("WebPartContent.aspx__GetRegionFilesJson", ex.Message);
            }
            return json;
        }

        private static void Get3Count(FileModel model)
        {
            try
            {
                model.DownCount = "0";
                model.ClickCount = "0";
                model.AveScore = "0.0";

                if (!string.IsNullOrEmpty(model.FID))
                {
                    GetDocumentCalculatedTimer timer = new GetDocumentCalculatedTimer();
                    var dcList = timer.GetDocumentCalculatedList(false);
                    var item = dcList.FirstOrDefault(T => T.FID == model.FID);

                    if (item != null)
                    {
                        model.DownCount = item.DownloadCount.ToString();
                        model.ClickCount = item.ClickCount.ToString();
                        model.AveScore = item.AveScore;
                    }
                }
            }
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("LatestFiles.aspx__Get3Count", ex.Message);
            }
        }

    }
}
