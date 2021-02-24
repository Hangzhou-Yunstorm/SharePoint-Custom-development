using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Web.Services;

namespace MobikeSPO.Layouts
{
    public partial class SPOLatestFiles : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 获取最新文件
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static string GetLatestFiles()
        {
            string filesJson = "[]";
            try
            {
                List<FilesModel> mos = new List<FilesModel>();
                using (SPSite site = new SPSite(CustomData.SPUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        //获取List
                        SPList fList = web.Lists.TryGetList(CustomData.DocumentsList);
                        var query = new SPQuery();

                        query.ViewAttributes = "Scope=\"Recursive\"";
                        query.Query = "<OrderBy><FieldRef Name=\"Modified\" Ascending=\"False\"></FieldRef></OrderBy>";
                        query.RowLimit = 100;

                        var files = fList.GetItems(query);

                        foreach (SPListItem item in files)
                        {
                            FilesModel model = new FilesModel();
                            var time = Convert.ToDateTime(item["Modified"]).ToString("yyyy-MM-dd");

                            string itemName = item.Name;
                            if (itemName.Length > 25)
                            {
                                itemName = itemName.Substring(0, 25) + "...";
                            }
                            model.Date_Time = time;
                            model.Name = item.File.ModifiedBy.Name;
                            model.Img = "<img class=\"image\" src=\"/_layouts/15/IMAGES/" + item.File.IconUrl + "\">";
                            var url = "/SPO/_layouts/15/WopiFrame.aspx?sourcedoc={" + item.UniqueId + "}&action=view";
                            model.Title = "<a href=\"" + url + "\" target=\"_blank\" title=\"" + item.Name + "\">" + itemName + "</a>";

                            mos.Add(model);
                        }

                        DataContractJsonSerializer json = new DataContractJsonSerializer(mos.GetType());
                        //序列化
                        using (MemoryStream stream = new MemoryStream())
                        {
                            json.WriteObject(stream, mos);
                            filesJson = Encoding.UTF8.GetString(stream.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return filesJson;
        }
    }
}
