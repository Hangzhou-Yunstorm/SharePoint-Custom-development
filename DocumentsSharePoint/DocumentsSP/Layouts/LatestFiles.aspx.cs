using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.Services;
using DocumentsSP.Helper;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using DocumentsSP.Model;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Utilities;

namespace DocumentsSP.Layouts
{
    public partial class LatestFiles : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 获取最新文件列表
        /// </summary>
        [WebMethod]
        public static string GetFilesJson()
        {
            string json = "[]";
            try
            {
                var siteId = SPContext.Current.Site.ID;
                using (SPSite site = new SPSite(siteId))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        var fList = web.Lists.TryGetList(CommonHelper.docListName);

                        SPView oView = fList.Views["LatestFiles"];
                        var query = new SPQuery(oView);

                        // TODO
                        string dString = SPUtility.CreateISO8601DateTimeFromSystemDateTime(DateTime.Now.AddDays(-CommonHelper.LatestFileDay)); // 转换当前时间
                        //var query = new SPQuery();
                        query.ViewAttributes = "Scope=\"Recursive\"";
                        query.Query = "<OrderBy><FieldRef Name=\"Modified\" Ascending=\"False\"></FieldRef></OrderBy>" +
                                      "<Where><Geq><FieldRef Name='Modified' /><Value Type='DateTime'>" + dString + "</Value></Geq></Where>";
                        query.RowLimit = 100;

                        var myS = fList.GetItems(query);
                        List<FileModel> models = new List<FileModel>();

                        foreach (SPListItem item in myS)
                        {
                            if (item.FileSystemObjectType == SPFileSystemObjectType.File)
                            {
                                FileModel model = new FileModel();
                                string fileUrl = CommonHelper.EncodeUrl(item.File.ServerRelativeUrl);

                                model.ID = item.ID;
                                model.FID = item["FID"] == null ? "" : item["FID"].ToString();
                                model.Department = CommonHelper.GetSubGroupName(item.File.Author);
                                model.Created = Convert.ToDateTime(item["Modified"]).ToString("yyyy-MM-dd HH:mm:ss");
                                model.Name = "<a href=\"/_layouts/15/FileDetail.aspx?Url=" + fileUrl + "\" target=\"_blank\" >" + item.Name + "</a>";
                                model.Icon = "<img class=\"image\" src=\"/_layouts/15/IMAGES/" + item.File.IconUrl + "\">";
                                model.FileSize = CommonHelper.GetSize(item.File.Length);
                                Get3Count(model);
                                string url = CommonHelper.EncodeUrl(item.File.ParentFolder.Url);
                                model.ParentFolder = "<a href=\"/" + url + "\" target=\"_blank\">Folder</a>";
                                model.CreatedBy = item.File.ModifiedBy.Name;

                                models.Add(model);
                            }
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
                CommonHelper.SetErrorLog("LatestFiles.aspx__GetFilesJson", ex.Message);
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
                        model.AveScore = item.AveScore.ToString();
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
