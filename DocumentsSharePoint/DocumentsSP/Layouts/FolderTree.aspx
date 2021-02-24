<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FolderTree.aspx.cs" Inherits="DocumentsSP.Layouts.FolderTree" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Folder Tree</title>
    <script type="text/javascript" src="Scripts/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="Plugins/layer/layer.js"></script>
    <script type="text/javascript" src="Plugins/zTree/js/jquery.ztree.all-3.5.min.js"></script>
    <link href="Plugins/zTree/css/zTreeStyle/zTreeStyle.css" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/FolderTree.js"></script>
    <style>
        .f_submit {
            margin: 10px;
            float: right;
        }

            .f_submit input {
                padding: 7px 10px;
                border: 1px solid #ababab;
                background-color: #fdfdfd;
                margin-left: 10px;
                font-family: "Segoe UI","Segoe",Tahoma,Helvetica,Arial,sans-serif;
                font-size: 13px;
                color: #444;
                width: 100px;
            }

        .ztree li span.button.ico_docu, .ztree li span.button.ico_close, .ztree li span.button.ico_open {
            background-position: -110px 0;
            display: inline-block;
        }
    </style>
</head>

<body>
    <form id="tree_form" runat="server">
        <div style="height: 440px; width: 500px; overflow: auto; padding-left: 10px;">
            <ul id="treeObject" class="ztree data-ztree"></ul>
        </div>
        <div class="f_submit">
            <input type="button" value="Confirm" onclick="Confirm()" />
            <input type="button" value="Cancel" onclick="Cancel()" />
        </div>
    </form>
</body>
</html>
