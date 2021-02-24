using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Collections.Generic;
using System.Web.Services;

namespace MobikeSPO.Layouts
{
    public partial class NoticeContent : LayoutsPageBase
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
            //页面ID
            var acId = Convert.ToInt32(acIdStr);
            using (SPSite site = new SPSite(CustomData.SPUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    //当前用户
                    UserName = web.CurrentUser.Name;
                    //获取List
                    SPList list = web.Lists.TryGetList(CustomData.NoticeList);
                    var curItem = list.GetItemById(acId);
                    if (curItem == null)
                    {
                        return;
                    }
                    PageId = curItem.ID;
                    ImgUrl = curItem["TitleImg"] == null ? "/SPO/NewsPictures/defaultbanner.jpg" : curItem["TitleImg"].ToString();
                    AreaTitle = curItem.Title;
                    ContentHtml = curItem["ContentHtml"] == null ? "" : curItem["ContentHtml"].ToString();
                    ComContent = CommonHelper.MaxContent(CustomData.NoticeComment, PageId);
                    List<CommentModel> Nolist = CommonHelper.GetCommentsByPId(0, CustomData.NoticeComment, PageId);
                    if (Nolist != null && Nolist.Count > 0)
                    {
                        for (int i = 0; i < Nolist.Count; i++)
                        {
                            List<CommentModel> Tolist = CommonHelper.GetCommentsByPId(Nolist[i].ID, CustomData.NoticeComment, PageId);
                            NoContent += DataSplicing(Nolist[i], Tolist);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 添加评论
        /// </summary>
        /// <param name="text">内容</param>
        /// <param name="PId">父Id</param>
        /// <param name="pageId">页面Id</param>
        /// <returns>添加ID</returns>
        [WebMethod]
        public static  int Release(string text, int pId, int pageId,int TypeID)
        {
            try
            {
                if (TypeID==0)
                {
                    return CommonHelper.AddComment(text, pId, CustomData.NoticeComment, pageId);
                }
                else if (TypeID==1)
                {
                    return CommonHelper.AddComment(text, pId, CustomData.AreaContentComment, pageId);
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public static string DataSplicing(CommentModel Comm, List<CommentModel> Tolist)
        {
            string DivText = string.Empty;
            DivText += "<div class='spComment_8d833d90' data-automation-id='sp-comment-block-root' id='Posted__" + Comm.ID + "'>";
            DivText += "<div class='spContentRegion_8d833d90 ms-scaleUpIn100'>";
            DivText += "<div class='spAvatar_8d833d90' aria-hidden='true' role='presentation'>";
            DivText += "<canvas class='spAvatarImage_8d833d90' data- src=' ' data- status='success' data- fingerprint='8a2bceaca7e84b8f4da535d772625c6f' width= '32' height= '32'> " + Comm.Name + " 的照片</canvas>";
            DivText += "</div>";
            DivText += "<div class='spContent_8d833d90'>";
            DivText += "<span class='spAuthor_8d833d90'>" + Comm.Name + "</span>";
            DivText += "<div class='spMetadata_8d833d90'>";
            DivText += "<a href= '#comment=17' class='ms-Link root_c02e569e isEnabled_c02e569e'> " + Comm.Date + "</a>";
            DivText += "</div>";
            DivText += "<div class='spText_8d833d90'>";
            DivText += "<span><span>" + Comm.CommentText + "</span></span>";
            DivText += "</div>";
            DivText += "<div class='spActions_8d833d90 ms-fadeIn500'>";
            DivText += "<div onclick='Publish(" + Comm.ID + ")'>";
            DivText += "<button type='button' class='button-button' data-automation-id='comment-reply-button' aria-label='答复" + Comm.Name + "' data-is-focusable='true'>";
            DivText += "<div class='ms-Button-flexContainer flexContainer-80'>";
            DivText += "<div class='ms-Button-textContainer textContainer-81'>";
            DivText += "<img src='Images/reply_back.png'style='float:  left;padding: 5px 5px 0 0'>";
            DivText += "<div class='ms-Button-label label-83' style='float:  left;'>回复</div>";
            DivText += "</div>";
            DivText += "</div>";
            DivText += "</button>";
            DivText += "</div>";
            DivText += "</div>";
            DivText += "</div>";
            DivText += "</div>";
            DivText += "<div class='spComments_8d833d90' id='Se__" + Comm.ID + "'>";
            if (Tolist != null && Tolist.Count > 0)
            {
                for (int i = 0; i < Tolist.Count; i++)
                {
                    DivText += DataS(Tolist[i]);
                }
            }
            DivText += "</div>";
            DivText += "</div>";
            return DivText;
        }

        public static string DataS(CommentModel Comm)
        {
            string DivText = string.Empty;
            DivText += "<div class='spComment_8d833d90' data-automation-id='sp-comment-block-root' id='Posted__" + Comm.ID + "'>";
            DivText += "<div class='spContentRegion_8d833d90 ms-scaleUpIn100'>";
            DivText += "<div class='spAvatar_8d833d90' aria-hidden='true' role='presentation'>";
            DivText += "<canvas class='spAvatarImage_8d833d90' data- src=' ' data- status='success' data- fingerprint='8a2bceaca7e84b8f4da535d772625c6f' width= '32' height= '32'> " + Comm.Name + " 的照片</canvas>";
            DivText += "</div>";
            DivText += "<div class='spContent_8d833d90'>";
            DivText += "<span class='spAuthor_8d833d90'>" + Comm.Name + "</span>";
            DivText += "<div class='spMetadata_8d833d90'>";
            DivText += "<a href= '#comment=17' class='ms-Link root_c02e569e isEnabled_c02e569e'> " + Comm.Date + "</a>";
            DivText += "</div>";
            DivText += "<div class='spText_8d833d90'>";
            DivText += "<span><span>" + Comm.CommentText + "</span></span>";
            DivText += "</div>";
            DivText += "</div>";
            DivText += "</div>";
            DivText += "</div>";
            return DivText;
        }





    }
}
