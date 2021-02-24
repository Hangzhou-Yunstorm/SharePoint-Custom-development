<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="CarouselWP" Namespace="DocumentsSP.CustomWebPart.Carousel" Assembly="DocumentsSP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=603c7d635dcc890d" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LatestFiles.aspx.cs" Inherits="DocumentsSP.Layouts.LatestFiles" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <link href="Plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="Plugins/Bootstrap-table/bootstrap-table.min.css" rel="stylesheet" />
    <link href="Content/IndexFiles.css?v=2017-09-25" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="Plugins/bootstrap/js/bootstrap.min.js"></script>
    <script type="text/javascript" src="Plugins/Bootstrap-table/bootstrap-table.min.js"></script>
    <script type="text/javascript" src="Scripts/Latest.js?v=2017-09-25"></script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <CarouselWP:Carousel runat="server" ChromeType="None" Description="我的可视 Web 部件" Title="DocumentsSP - Carousel" ID="g_fb21733c_7efc_42ed_8225_40283d08ac2c" __MarkupType="vsattributemarkup" __WebPartId="{FB21733C-7EFC-42ED-8225-40283D08AC2C}" WebPart="true" __designer:IsClosed="false"></CarouselWP:Carousel>

    <div class='splitters' id="title_div">
        <span id="r_title">Latest</span>
        <img onclick='LatestDownLoad()' style='float: right; cursor: pointer;' src='/_layouts/15/Images/down1.png' alt='Download file'>
    </div>
    <div class="rightTable">
        <div id="toolbar"></div>
        <table id="file_table" data-toolbar="#toolbar"></table>
    </div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Latest
</asp:Content>
