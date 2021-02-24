<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserGroups.aspx.cs" Inherits="DocumentsSP.Layouts.UserGroups" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <link href="Plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="Plugins/Bootstrap-table/bootstrap-table.min.css" rel="stylesheet" />
    <link href="Content/UserGroups.css" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="Plugins/bootstrap/js/bootstrap.min.js"></script>
    <script type="text/javascript" src="Plugins/Bootstrap-table/bootstrap-table.min.js"></script>
    <script type="text/javascript" src="Plugins/layer/layer.js"></script>
    <%--<script type="text/javascript">
        var gData=<%= Groups%>;
    </script>--%>

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div class="review">
        <div class='leftTree'>
            <h3 class='myTitle'>Admin</h3>
            <ul>
                <li class='primaryDir'>
                    <h4>Library Manage</h4>
                    <ul class='second'>
                        <li id='Authorization'><a href='/Documents/Forms/Authorization.aspx'>Authorization</a></li>
                        <li id='PermissionSettings' style="display: none;"><a href='/_layouts/15/role.aspx'>Permission Settings</a></li>
                        <li id='UserGroups'><a href='/_layouts/15/UserGroups.aspx'>User Groups</a></li>
                        <li id='Whitelist'><a id="whitelist_a" href='/Lists/WhiteList/AllItems.aspx'>WhiteList</a></li>
                    </ul>
                </li>
                <li class='primaryDir' id="li_logs" style="display: none;">
                    <h4>Action Log</h4>
                    <ul class='second'>
                        <li id='FileLogs'><a href='/_layouts/15/LogList.aspx'>System Logs</a></li>
                    </ul>
                </li>
                <li class='primaryDir' id="li_setting" style="display: none;">
                    <h4>Site Settings</h4>
                    <ul class='second'>
                        <li id='HomeGallery'><a href='/Carousel List/Forms/AllItems.aspx'>Home Gallery</a></li>
                        <li id='RecycleBin'><a href='/_layouts/15/AdminRecycleBin.aspx'>The Recycle Bin</a></li>
                        <li><a target='_blank' href='/Documents/Forms/AllItems.aspx'>Primary Directory</a></li>
                        <li id='ShareCatalogList'><a href='/Lists/ShareCatalogList/AllItems.aspx'>Share Catalog Manage</a></li>
                        <li id='FeedbackList'><a href='/Lists/Suggestion/AllItems.aspx'>Suggestion</a></li>
                        <li id='ReportManage'><a href='/Lists/ReportManageList/AllItems.aspx'>Report Manage</a></li>
                    </ul>
                </li>
            </ul>
        </div>
        <div class="rightTable">
            <div id="toolbar">
                <%=Operate %>
            </div>
            <table id="group_table" data-toolbar="#toolbar"></table>
        </div>
    </div>
    <script type="text/javascript" src="Scripts/UserGroup.js?v=2018-06-12"></script>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    User Groups
</asp:Content>
