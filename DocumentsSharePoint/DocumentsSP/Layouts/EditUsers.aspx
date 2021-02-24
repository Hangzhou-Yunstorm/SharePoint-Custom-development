<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditUsers.aspx.cs" Inherits="DocumentsSP.Layouts.EditUsers" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Edit Users</title>
    <script type="text/javascript" src="Scripts/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="Plugins/layer/layer.js"></script>
    <link href="Plugins/zTree/css/zTreeStyle/zTreeStyle.css" rel="stylesheet" />
    <script type="text/javascript" src="Plugins/zTree/js/jquery.ztree.all-3.5.min.js"></script>
    <link href="Content/AddUsers.css?v=2017-09-190" rel="stylesheet" />
    <script type="text/javascript">
        var groupId = "<%= groupId %>";
    </script>
    <script type="text/javascript" src="Scripts/EditUsers.js?v=2018-06-111"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <span>Group Name: </span>&nbsp;&nbsp;&nbsp;&nbsp;<%=groupName %>
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
                        <%=selContent %>
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
