<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AreaContent.aspx.cs" Inherits="MobikeSPO.Layouts.AreaContent" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <link href="Content/Area.css" rel="stylesheet" />
     <link href="Content/MobikePublish.css" rel="stylesheet" />
      <script type="text/javascript">
        var userName = "<%=UserName%>";
        var pageId = "<%=PageId%>";
          var comContent=  <%=ComContent %>;
          var TypeID=1;
    </script>
    <script src="Scripts/MobikePublish.js?v=2017-12-22" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div id="top_email"><span class="r_span">已于 2017/12/25 发布</span></div>
    <div class="title_img" style="background-image: url('<%=ImgUrl %>')">
        <div class="a_title">
            <span><%=AreaTitle %></span>
        </div>
    </div>
    <div class="a_content">
        <%=ContentHtml %>
    </div>
        <div class="commentsWrapper_c2b759ca">
        <div class="spInnerCommentsWrapper_1f49b12d">
            <div class="spRule_1f49b12d"></div>
            <div class="spCommentSection_8d833d90" role="complementary" aria-label="评论">
                <div>
                    <h2 class="spHeader_8d833d90" id="sp-comments"><span id="sp-span-count">0</span>条注释</h2>
                </div>
                <div class="spComments_8d833d90">
                    <div class="spReply_8d833d90">
                        <div class="spAvatar_8d833d90" aria-hidden="true" role="presentation">
                            <canvas class="spAvatarImage_8d833d90" data-src="" data-status="success" width="32" height="32" data-fingerprint="8a2bceaca7e84b8f4da535d772625c6f">test2017 的照片</canvas>
                        </div>
                        <div class="spField_8d833d90">
                            <div id="Te__0" aria-label="添加注释" class="spInput_8d833d90" contenteditable="true" placeholder="添加注释" role="textbox"></div>
                        </div>
                        <button id="Btn__0" onclick="Release(0)" type="button" class="ms-Button ms-Button--primary spButton_8d833d90 is-disabled root-85 button button-primary button-rounded button-small" data-automation-id="sp-comment-post" aria-label="发布" data-is-focusable="false">
                            发布
                        </button>
                    </div>
                    <div id="Posted__0">
                        <%=NoContent %>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Area Content
</asp:Content>
