using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace MobikeSPO.Layouts
{
    public partial class NoticeList : LayoutsPageBase
    {
        // 内容
        public string ContentHtml = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            using (SPSite site = new SPSite(CustomData.SPUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    //获取List
                    SPList list = web.Lists.TryGetList(CustomData.NoticeList);
                    var nItems = list.GetItems();

                    // 内容
                    if (nItems != null && nItems.Count > 0)
                    {
                        foreach (SPListItem nItem in nItems)
                        {
                            string url = "/SPO/_layouts/15/NoticeContent.aspx?ID=" + nItem.ID;
                            string imgUrl = nItem["Thumbnail"] == null ? "/SPO/NewsPictures/defaultbanner.jpg" : nItem["Thumbnail"].ToString();
                            string title = nItem.Title;
                            string userName = nItem[SPBuiltInFieldId.Editor].ToString().Split('#')[1];
                            string time = Convert.ToDateTime(nItem["Modified"]).ToString("MM-dd HH:mm");

                            ContentHtml += "<div class=\"fabric\">" +
                                           "<a href=\"" + url + "\"><div class=\"mthumbnailContainer\"><img class=\"\" src=\"" + imgUrl + "\" alt=\"img\" title=\"" + title + "\" /></div></a>" +
                                           "<div class=\"metadata\">" +
                                           "<a href=\"" + url + "\"><div title=\"" + title + "\" class=\"lessText\">" + title + "</div></a>" +
                                           "<div class=\"metadataDescription\" >" + nItem["NoticeDescription"] + "</div>" +
                                           "<div class=\"metadataSubtitle\">" + userName + "</div>" +
                                           "<div class=\"metadataDate\">" + time + "</div>" +
                                           "</div>" +
                                           "</div>";
                        }
                    }

                }

            }
        }

    }
}
