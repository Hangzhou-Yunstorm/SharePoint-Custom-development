<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GroupMembers.aspx.cs" Inherits="DocumentsSP.Layouts.GroupMembers" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <link href="Plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="Plugins/Bootstrap-table/bootstrap-table.min.css" rel="stylesheet" />
    <link href="Content/GroupMembers.css" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="Plugins/layer/layer.js"></script>
    <script type="text/javascript" src="Plugins/bootstrap/js/bootstrap.min.js"></script>
    <script type="text/javascript" src="Plugins/Bootstrap-table/bootstrap-table.min.js"></script>
    <script type="text/javascript">
        var groupId="<%= groupId%>";
    </script>
    <script type="text/javascript" src="Scripts/GroupMembers.js?v=2018-06-12"></script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div class="review">
        <h4 style="margin-top: 0;">
            <span>Group Name: </span>&nbsp;&nbsp;&nbsp;&nbsp;<%=groupName %>
        </h4>
        <hr style="margin: 10px 0" />
        <div id="toolbar">
            <%=Operate %>
        </div>
        <table id="group_table" data-toolbar="#toolbar"></table>
    </div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Group Members
</asp:Content>
