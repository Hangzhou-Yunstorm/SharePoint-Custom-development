using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Collections.Generic;

namespace MobikeSPO.Layouts
{
    public partial class AreaContent : LayoutsPageBase
    {
        // 图片链接
        public string ImgUrl = "";
        // 名称
        public string AreaTitle = "";
        // 内容
        public string ContentHtml = "";
        //评论内容
        public string NoContent = "";
        //当前用户
        public string UserName = "";
        //当前页面ID
        public int PageId = 0;
        //评论总行数
        public int ComContent = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            var acIdStr = Request.QueryString["ID"];
            var acId = Convert.ToInt32(acIdStr);
            using (SPSite site = new SPSite(CustomData.SPUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    //当前用户
                    UserName = web.CurrentUser.Name;
                    //获取List
                    SPList list = web.Lists.TryGetList(CustomData.AreaContentList);
                    var curItem = list.GetItemById(acId);

                    if (curItem == null)
                    {
                        return;
                    }
                    PageId = curItem.ID;
                    ImgUrl = curItem["TitleImg"] == null ? "/SPO/NewsPictures/defaultbanner.jpg" : curItem["TitleImg"].ToString();
                    AreaTitle = curItem.Title;
                    ContentHtml = curItem["ContentHtml"] == null ? "" : curItem["ContentHtml"].ToString(); ;
                    ComContent = CommonHelper.MaxContent(CustomData.AreaContentComment, PageId);
                    List<CommentModel> Nolist = CommonHelper.GetCommentsByPId(0, CustomData.AreaContentComment, acId);
                    if (Nolist != null && Nolist.Count > 0)
                    {
                        for (int i = 0; i < Nolist.Count; i++)
                        {
                            List<CommentModel> Tolist = CommonHelper.GetCommentsByPId(Nolist[i].ID, CustomData.AreaContentComment, acId);
                            NoContent += NoticeContent.DataSplicing(Nolist[i], Tolist);
                        }
                    }
                }
            }

        }
    }
}