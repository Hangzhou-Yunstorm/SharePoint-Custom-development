using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Services;

namespace MobikeSPO.Layouts
{
    public partial class HelperPage : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///获取当前用户名
        /// </summary>
        /// <returns>当前用户名</returns>
        [WebMethod]
        public static string GetUserName()
        {
            var userName = SPContext.Current.Web.CurrentUser.Name;
            return userName;
        }

        /// <summary>
        /// 获取导航内容
        /// </summary>
        /// <returns>导航内容</returns>
        [WebMethod]
        public static string GetNavs()
        {
            string navLi = "<li class='f_li'><a href=\"/SPO/default.aspx\">主页</a></li>";
            //"<li class='f_li'><a href=\"/SPO/default.aspx\">员工必看</a></li>";
            //"<li class='f_li'><a href=\"https://mobike.sharepoint.cn/sites/Office3652\">Google Drive迁移引导</a></li>";
            try
            {

                using (SPSite site = new SPSite(CustomData.SPUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        //获取List
                        SPList list = web.Lists.TryGetList(CustomData.AreaList);

                        var query = new SPQuery();
                        query.Query = "<Where><And>" +
                                              "<IsNull><FieldRef Name=\"ParentArea\"/></IsNull>" +
                                              "<IsNotNull><FieldRef Name=\"NavOrder\"/></IsNotNull></And></Where>" +
                                              "<OrderBy><FieldRef Name=\"NavOrder\" ></FieldRef></OrderBy>";
                        var firstItems = list.GetItems(query);

                        if (firstItems != null && firstItems.Count > 0)
                        {
                            foreach (SPListItem item in firstItems)
                            {
                                string title = item.Title;
                                if (title.Length > 15)
                                {
                                    title = title.Substring(0, 15) + "...";
                                }
                                string url = "/SPO/_layouts/15/AreaList.aspx?ID=" + item.ID;
                                navLi += "<li class='f_li'><a href=\"" + url + "\" title=\"" + item.Title + "\">" + title;

                                var secquery = new SPQuery();
                                secquery.Query = "<Where><Eq><FieldRef Name=\"ParentArea\" LookupId=\"True\" /><Value Type=\"Lookup\">" + item.ID + "</Value></Eq></Where>";
                                var secItems = list.GetItems(secquery);
                                if (secItems != null && secItems.Count > 0)
                                {
                                    navLi += "<img src=\"/SPO/_layouts/15/Images/lila.png\" /></a>";
                                    navLi += "<ul class=\"secondNav\">";
                                    foreach (SPListItem secItem in secItems)
                                    {
                                        string secTitle = secItem.Title;
                                        if (secTitle.Length > 15)
                                        {
                                            secTitle = secTitle.Substring(0, 15) + "...";
                                        }
                                        string secUrl = "/SPO/_layouts/15/AreaList.aspx?ID=" + secItem.ID;
                                        navLi += "<li><a href=\"" + secUrl + "\" title=\"" + secItem.Title + "\">" + secTitle + "</a></li>";
                                    }
                                    navLi += "</ul></li>";
                                }
                                else
                                {
                                    navLi += "</a></li>";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return navLi;
        }


        /// <summary>
        /// 获取导航内容
        /// </summary>
        /// <returns>导航内容</returns>
        [WebMethod]
        public static string GetIconJson()
        {
            string iconJson = string.Empty;
            try
            {
                using (SPSite site = new SPSite(CustomData.SPUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        //获取List
                        SPList list = web.Lists.TryGetList(CustomData.AreaList);

                        var query = new SPQuery();
                        query.Query = "<Where><And>" +
                                              "<IsNull><FieldRef Name=\"ParentArea\"/></IsNull>" +
                                              "<IsNotNull><FieldRef Name=\"NavOrder\"/></IsNotNull></And></Where>" +
                                              "<OrderBy><FieldRef Name=\"NavOrder\" ></FieldRef></OrderBy>";

                        var firstItems = list.GetItems(query);

                        var defaultIcon = "/SPO/NewsPictures/小图标/moren.jpg";

                        List<IconModel> icons = new List<IconModel>();
                        //icons.Add(new IconModel() { Name = "员工必看", Img = defaultIcon });

                        if (firstItems != null && firstItems.Count > 0)
                        {
                            foreach (SPListItem item in firstItems)
                            {
                                var img = item["IconImg"] == null ? defaultIcon : item["IconImg"].ToString();
                                string url = "/SPO/_layouts/15/AreaList.aspx?ID=" + item.ID;
                                icons.Add(new IconModel() { Name = item.Title, Img = img, Url = url });

                                if (icons.Count > 12)
                                {
                                    break;
                                }
                            }
                        }

                        DataContractJsonSerializer json = new DataContractJsonSerializer(icons.GetType());
                        //序列化
                        using (MemoryStream stream = new MemoryStream())
                        {
                            json.WriteObject(stream, icons);
                            iconJson = Encoding.UTF8.GetString(stream.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return iconJson;
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
                        query.RowLimit = 6;

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
                            model.Title = itemName;
                            model.Img = "/_layouts/15/IMAGES/" + item.File.IconUrl;
                            model.Url = "/SPO/_layouts/15/WopiFrame.aspx?sourcedoc={" + item.UniqueId + "}&action=view";

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

        /// <summary>
        /// 获取公告
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static string GetIndexNotice()
        {
            string filesJson = "[]";
            try
            {
                List<NoticeModel> mos = new List<NoticeModel>();
                using (SPSite site = new SPSite(CustomData.SPUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        //获取List
                        SPList fList = web.Lists.TryGetList(CustomData.NoticeList);
                        var query = new SPQuery();
                        query.Query = "<OrderBy><FieldRef Name=\"Modified\" Ascending=\"False\"></FieldRef></OrderBy>";
                        query.RowLimit = 4;

                        var notices = fList.GetItems(query);

                        foreach (SPListItem item in notices)
                        {
                            NoticeModel model = new NoticeModel();
                            var time = Convert.ToDateTime(item["Modified"]).ToString("MM-dd HH:mm");

                            string title = item.Title;
                            if (title.Length > 15)
                            {
                                title = title.Substring(0, 15) + "...";
                            }

                            model.Date_Time = time;
                            model.Name = item[SPBuiltInFieldId.Editor].ToString().Split('#')[1];
                            model.Title = title;
                            model.Img = item["TitleImg"] == null ? "/SPO/NewsPictures/defaultbanner.jpg" : item["TitleImg"].ToString();
                            model.Url = "/SPO/_layouts/15/NoticeContent.aspx?ID=" + item.ID;
                            var content = item["NoticeDescription"] == null ? "" : item["NoticeDescription"].ToString();
                            if (content.Length > 25)
                            {
                                content = content.Substring(0, 25) + "...";
                            }
                            model.Content = content;

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
