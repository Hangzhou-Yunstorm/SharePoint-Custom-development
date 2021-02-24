<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MoFileView.aspx.cs" Inherits="DocumentsSP.Layouts.MoFileView" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>File View</title>
    <style type="text/css">
        .watermark_div {
            width: 100%;
            background: url("/_layouts/15/GetMoWaterMark.ashx") repeat;
        }

            .watermark_div img, .watermark_div video {
                width: 100%;
                max-width: 100%;
                filter: alpha(opacity=90);
                moz-opacity: .90;
                opacity: .90;
            }
    </style>
</head>

<body>
    <form id="form1" runat="server">
        <div class="watermark_div">
            <%=ViewStr %>
        </div>
    </form>
</body>
</html>
