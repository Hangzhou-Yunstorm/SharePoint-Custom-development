<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangeDepartment.aspx.cs" Inherits="DocumentsSP.Layouts.ChangeDepartment" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Change Department</title>
    <script type="text/javascript" src="Scripts/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="Plugins/layer/layer.js"></script>
    <link href="Plugins/DropKick/css/dropkick.css" rel="stylesheet" />
    <script type="text/javascript" src="Plugins/DropKick/jquery.dropkick-min.js"></script>
    <script type="text/javascript" src="Scripts/ChangeDepartment.js?v=2017-11-9"></script>
</head>

<body>
    <form id="change_form" runat="server">
        <div style="width: 500px; margin: 30px 0 0 80px;">
            <div style="margin-bottom: 20px;">
                <span style="display: inline-block; margin-right: 10px;">Old Department</span>
                <input type="text" id="old_department" list="g_list" style="height: 20px; width: 245px; padding: 3px;" placeholder="Input old department name" />
                <datalist id="g_list">
                    <%=groupListJson %>
                </datalist>
            </div>
            <div id="group_select" style="height: 50px;">
                <span style="float: left; display: inline-block; margin-right: 8px;">New Department</span>
                <select class="group_type" tabindex="13" style="display: none;">
                    <%=groupsJson %>
                </select>
            </div>
            <br />
            <div>
                <input type="button" value="Confirm" onclick="ChangeDepart()" style="height: 30px; width: 100px; margin-left: 150px; background-color: #0E60AC; color: #fff; font-size: 13px; border: 0;" />
                <input type="button" value="Cancel" onclick="Cancel()" style="height: 30px; width: 100px; margin-left: 35px; background-color: #40a756; color: #fff; font-size: 13px; border: 0;" />
            </div>
        </div>
    </form>
</body>
</html>
