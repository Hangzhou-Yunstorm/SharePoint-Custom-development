using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.Services;
using DocumentsSP.Helper;
using DocumentsSP.Model;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Linq;

namespace DocumentsSP.Layouts
{
    public partial class MostDownLoadFiles : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 获取文件列表
        /// </summary>
        [WebMethod]
        public static string GetFilesJson()
        {
            string json = "[]";
            try
            {
                List<FileModel> models = new List<FileModel>();

                GetDocumentCalculatedTimer timer = new GetDocumentCalculatedTimer();
                var dcList = timer.GetMostDownloadList(false);

                foreach (var mo in dcList)
                {
                    FileModel model = new FileModel();
                    model.FID = mo.FID;
                    model.ClickCount = mo.ClickCount;
                    model.DownCount = mo.DownCount;
                    model.AveScore = mo.AveScore;
                    model.FileUrl = mo.FileUrl;
                    model.ID = mo.ID;
                    model.Created = mo.Created;
                    model.Name = "<a href=\"/_layouts/15/FileDetail.aspx?Url=" + mo.FileUrl + "\" target=\"_blank\" title=\"" + mo.Name + "\">" + mo.Name + "</a>";
                    model.Icon = "<img class=\"image\" src=\"/_layouts/15/IMAGES/" + mo.Icon + "\">";
                    model.FileSize = mo.FileSize;
                    model.ParentFolder = "<a href=\"/" + mo.ParentFolder + "\" target=\"_blank\">Folder</a>";
                    model.CreatedBy = mo.CreatedBy;

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
            catch (Exception ex)
            {
                CommonHelper.SetErrorLog("MostDownLoadFiles.aspx__GetFilesJson", ex.Message);
            }
            return json;
        }

    }
}
