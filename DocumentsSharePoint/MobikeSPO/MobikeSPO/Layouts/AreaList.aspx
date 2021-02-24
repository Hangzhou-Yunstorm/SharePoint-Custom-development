<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AreaList.aspx.cs" Inherits="MobikeSPO.Layouts.AreaList" DynamicMasterPageFile="~masterurl/default.master" %>

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
         <%=NoContent %>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Area List
</asp:Content>

