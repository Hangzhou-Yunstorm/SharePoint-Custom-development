using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Collections.Generic;

namespace MobikeSPO.Layouts
{
    public partial class AreaList : LayoutsPageBase
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
            var areaIdStr = Request.QueryString["ID"];
            var areaId = Convert.ToInt32(areaIdStr);
            using (SPSite site = new SPSite(CustomData.SPUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    //当前用户
                    UserName = web.CurrentUser.Name;
                    //获取List
                    SPList list = web.Lists.TryGetList(CustomData.AreaList);
                    var curItem = list.GetItemById(areaId);
                    if (curItem == null)
                    {
                        return;
                    }

                    ImgUrl = curItem["TitleImg"] == null ? "/SPO/NewsPictures/defaultbanner.jpg" : curItem["TitleImg"].ToString();
                    AreaTitle = curItem.Title;

                    var isShowMsg = CommonHelper.ObjToBool(curItem["IsShowMsg"]);
                    var isShowAdmin = CommonHelper.ObjToBool(curItem["IsShowAdmin"]);

                    var secquery = new SPQuery();
                    secquery.Query = "<Where><Eq><FieldRef Name=\"ParentArea\" LookupId=\"True\" /><Value Type=\"Lookup\">" + curItem.ID + "</Value></Eq></Where>";
                    var secItems = list.GetItems(secquery);
                    // 子分区
                    if (secItems != null && secItems.Count > 0)
                    {
                        if (isShowMsg || isShowAdmin)
                        {
                            ContentHtml += "<div class=\"leftContent\">";
                        }
                        foreach (SPListItem secItem in secItems)
                        {
                            string url = "/SPO/_layouts/15/AreaList.aspx?ID=" + secItem.ID;
                            string imgUrl = secItem["Thumbnail"] == null ? "/SPO/NewsPictures/defaultbanner.jpg" : secItem["Thumbnail"].ToString();
                            string title = secItem.Title;

                            ContentHtml += "<div class=\"fabric\">" +
                                                       "<a href=\"" + url + "\"><div class=\"mthumbnailContainer\"><img class=\"\" src=\"" + imgUrl + "\" alt=\"img\" title=\"" + title + "\" /></div></a>" +
                                                       "<div class=\"metadata\">" +
                                                       "<a href=\"" + url + "\"><div title=\"" + title + "\" class=\"lessText\">" + title + "</div></a>" +
                                                       "<div class=\"metadataSubtitle\">" + secItem["SubTitle"] + "</div>" +
                                                       "<div class=\"metadataDescription\" >" + secItem["AreaDescription"] + "</div>" +
                                                       "</div>" +
                                                       "</div>";
                        }
                        if (isShowMsg || isShowAdmin)
                        {
                            ContentHtml += "</div>";
                            if (isShowMsg)
                            {
                                ContentHtml += GetRightContent(curItem, true);
                            }
                            else
                            {
                                ContentHtml += GetRightContent(curItem, false);
                            }
                        }
                    }
                    else
                    {
                        #region Content
                        // 内容
                        SPList aclist = web.Lists.TryGetList(CustomData.AreaContentList);
                        var acquery = new SPQuery();
                        acquery.Query = "<Where><Eq><FieldRef Name=\"BelongArea\" LookupId=\"True\" /><Value Type=\"Lookup\">" + curItem.ID + "</Value></Eq></Where>";
                        var acItems = aclist.GetItems(acquery);

                        if (acItems != null && acItems.Count > 0)
                        {
                            // 单条内容
                            if (acItems.Count == 1)
                            {
                                NoContent += "<div class='commentsWrapper_c2b759ca'>";
                                NoContent += "<div class='spInnerCommentsWrapper_1f49b12d'>";
                                NoContent += "<div class='spRule_1f49b12d'></div>";
                                NoContent += "<div class='spCommentSection_8d833d90' role='complementary' aria-label='评论'>";
                                NoContent += "<div>";
                                NoContent += "<h2 class='spHeader_8d833d90' id='sp-comments'><span id = 'sp-span-count'> 0 </span> 条注释 </h2>";
                                NoContent += "</div>";
                                NoContent += "<div class='spComments_8d833d90'>";
                                NoContent += "<div class='spReply_8d833d90'>";
                                NoContent += "<div class='spAvatar_8d833d90' aria-hidden='true' role='presentation'>";
                                NoContent += "<canvas class='spAvatarImage_8d833d90' data-src='' data-status='success' width='32' height='32' data-fingerprint='8a2bceaca7e84b8f4da535d772625c6f'>test2017 的照片</canvas>";
                                NoContent += "</div>";
                                NoContent += " <div class='spField_8d833d90'>";
                                NoContent += "<div id = 'Te__0' aria-label='添加注释' class='spInput_8d833d90' contenteditable='true' placeholder='添加注释' role='textbox'></div>";
                                NoContent += "</div>";
                                NoContent += "<button id = 'Btn__0' onclick='Release(0)' type='button' class='ms-Button ms-Button--primary spButton_8d833d90 is-disabled root-85 button button-primary button-rounded button-small' data-automation-id='sp-comment-post' aria-label='发布' data-is-focusable='false'>";
                                NoContent += "发布";
                                NoContent += "</button>";
                                NoContent += "</div>";
                                NoContent += "<div id = 'Posted__0'>";

                                ContentHtml = acItems[0]["ContentHtml"].ToString();
                                PageId = acItems[0].ID;

                                ComContent = CommonHelper.MaxContent(CustomData.AreaContentComment, PageId);
                                List<CommentModel> Nolist = CommonHelper.GetCommentsByPId(0, CustomData.AreaContentComment, PageId);
                                if (Nolist != null && Nolist.Count > 0)
                                {
                                    for (int i = 0; i < Nolist.Count; i++)
                                    {
                                        List<CommentModel> Tolist = CommonHelper.GetCommentsByPId(Nolist[i].ID, CustomData.AreaContentComment, PageId);
                                        NoContent += NoticeContent.DataSplicing(Nolist[i], Tolist);
                                    }
                                }
                                NoContent += "</div>";
                                NoContent += "</div>";
                                NoContent += "</div>";
                                NoContent += "</div>";
                                NoContent += "</div>";
                            }
                            else
                            {
                                // 多条内容
                                foreach (SPListItem secItem in acItems)
                                {
                                    string url = "/SPO/_layouts/15/AreaContent.aspx?ID=" + secItem.ID;
                                    string imgUrl = secItem["Thumbnail"] == null ? "/SPO/NewsPictures/defaultbanner.jpg" : secItem["Thumbnail"].ToString();
                                    string title = secItem.Title;

                                    ContentHtml += "<div class=\"fabric\">" +
                                                               "<a href=\"" + url + "\"><div class=\"mthumbnailContainer\"><img class=\"\" src=\"" + imgUrl + "\" alt=\"img\" title=\"" + title + "\" /></div></a>" +
                                                               "<div class=\"metadata\">" +
                                                               "<a href=\"" + url + "\"><div title=\"" + title + "\" class=\"lessText\">" + title + "</div></a>" +
                                                               "<div class=\"metadataSubtitle\">" + secItem["SubTitle"] + "</div>" +
                                                               "<div class=\"metadataDescription\" >" + secItem["ContentDescription"] + "</div>" +
                                                               "</div>" +
                                                               "</div>";
                                }
                            }
                        }
                        #endregion
                    }
                }
            }
        }

        private string GetRightContent(SPListItem item, bool isShowMsg)
        {
            string content = string.Empty;
            if (isShowMsg)
            {
                string rTitle = item["RightTitle"] == null ? item.Title : item["RightTitle"].ToString();

                content += "<div class=\"rightContent\">" +
                                   "<p class=\"r_title\">" + rTitle + "</p>" +
                                   "<p class=\"r_content\">企业微信：" + item["WeChat"] + "</p>" +
                                   "<p class=\"r_content\">邮箱：" + item["Email"] + "</p>" +
                                   "<p class=\"r_content\">服务热线：" + item["ServiceLine"] + "</p>" +
                                   "</div>";
            }
            else
            {
                content += "<div class=\"rightContent\">";

                SPFieldUserValueCollection users = (SPFieldUserValueCollection)item["Administrator"];
                if (users != null)
                {
                    content += "<div class=\"adminText\">管理员</div>";
                    foreach (SPFieldUserValue u in users)
                    {
                        var name = u.User.Name;
                        var email = u.User.Email ?? "mobike_admin@mobike.com";

                        content += "<div class=\"presentation\">" +
                                           "<div class=\"f_div\"><img src=\"/_layouts/15/images/userimg.png\" alt=\"userimage\" /></div>" +
                                           "<div class=\"primaryText\" title=\"" + name + "\">" + name + "</div>" +
                                           "<div class=\"secondaryText\" title=\"高级网络工程师\">高级网络工程师</div>" +
                                           "<div class=\"emailText\" title=\"" + email + "\">" + email + "</div>" +
                                           "</div>";
                    }
                }

                content += "</div>";
            }

            return content;

        }

    }
}
