<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelectExternalUser.aspx.cs" Inherits="DocumentsSP.Layouts.SelectExternalUser" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Access Control</title>
    <script type="text/javascript" src="Scripts/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="Plugins/layer/layer.js"></script>
    <%--<script type="text/javascript" src="Plugins/laydate/laydate.js"></script>--%>
    <script type="text/javascript" src="Plugins/laydate-v5/laydate/laydate.js"></script>
    <link href="Plugins/zTree/css/zTreeStyle/zTreeStyle.css" rel="stylesheet" />
    <script type="text/javascript" src="Plugins/zTree/js/jquery.ztree.all-3.5.min.js"></script>
    <link href="Content/SelectExternalUser.css" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/SelectExternalUser.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <span>Access expiry date: </span>&nbsp;&nbsp;&nbsp;&nbsp;<input type="text" style="height: 20px; padding: 2px;" class="form-control" readonly="readonly" id="expiration" name="expiration" placeholder="Please select expiry date" />&nbsp;&nbsp;&nbsp;&nbsp;<span><span style="color: red">*</span>&nbsp;Leave blank to share document permanently.</span>
        </div>
        <div style="margin: 10px 0;">
            <span>Allow upload: </span>&nbsp;&nbsp;
                <input name="upload_ckb" id="upload_ckb" type="checkbox" checked="checked" />
        </div>
        <hr />
        <div>
            <div class="ulSelect">
                <div>
                    <div>
                        <label>User List</label>
                    </div>
                    <div class="ulA">
                        <div style="float: left; width: 205px;">
                            <a href="#" onclick="SelectAllUsers('0')">Select All</a><a href="#" onclick="SelectAllUsers('1')">Unselect All</a>
                        </div>
                        <div class="ulSearch">
                            <input type="text" placeholder="Search User" />
                            <img src="images/search.png" />
                        </div>
                    </div>
                    <div class="ulTree">
                        <ul id="treeUser" class="ztree"></ul>
                    </div>
                </div>
            </div>
            <div class="ulShow">
                <table>
                    <thead>
                        <tr>
                            <td>UserName</td>
                            <td>Operate</td>
                        </tr>
                    </thead>
                    <tbody id="SelContent">
                    </tbody>
                </table>
                <div class="ulShow_handle">
                    <a id="SubmitData" onclick="SubmitData()">Confirm</a>
                </div>
            </div>
        </div>
    </form>

</body>
</html>
