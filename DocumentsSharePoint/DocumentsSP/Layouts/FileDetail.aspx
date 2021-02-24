<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FileDetail.aspx.cs" Inherits="DocumentsSP.Layouts.FileDetail" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0" />
    <link href="Plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="Plugins/starScore/css/star-rating.css" rel="stylesheet" />
    <script type="text/javascript" src="Plugins/starScore/js/star-rating.js"></script>
    <link href="Content/FileDetail.css?v=2017-10-27" rel="stylesheet" />
    <script type="text/javascript" src="Plugins/page/jquery.page.js"></script>
    <link href="Plugins/page/page.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div class="foldDetail" id="pc_div" style="display: none;">
        <div class="topGeneral">
            <h4><%= fileName %></h4>
            <div>
                <div class="fileAuthor">
                    <span id="fileName"><%= fileAuthor %></span>
                    <span>Created on <%= fileCreatTime %></span>
                </div>
                <span id="scoreText">
                    <input id="AVScore" value="0" type="number" style="display: none;" class="rating" min="0" max="5" readonly="true" data-size="sm" /></span>
                <span id="clickCount"></span>
                <span id="downloadCount"></span>
            </div>
        </div>
        <div style="font-size: 12px;">
            <a href="<%= filePath %>" target="_blank"><%= filePath %></a>
        </div>
        <div class="mainContent">
            <div class="leftContent">
                <div class="fileGeneral">
                    <span style="max-width: 435px; display: inline-block; float:left;"><%= fileDescription %></span>
                    <a class="fileDownBtn mr10" target="_blank" href="/_layouts/15/FileDownload.aspx?ItemID=<%=fileID %>">
                        <img src="/_layouts/15/Images/down1.png" alt="download" /></a>
                    <input type="text" style="opacity: 0; z-index: -1; position: absolute; top: 0;" id="copyurl" value="<%=webUrl %>/_layouts/15/FileDetail.aspx?FID=<%=fileFID %>" />
                    <a class="fileDownBtn mr30" style="cursor: pointer;" onclick="Copy()">
                        <img src="/_layouts/15/Images/copy.png" alt="share" /></a>
                </div>
                <div class="leftContentDetail">
                </div>
            </div>
            <div class="rightComments">
                <div class="yourCom">
                    <h4 class="score" id="alreadScore"></h4>
                    <div class="myStar">
                        <input id="ScoreNum" value="0" type="number" style="display: none;" class="rating" min="0" max="5" step="0.5" data-size="sm" />
                    </div>
                    <textarea id="commentText" class="p10" rows="10"></textarea>
                    <input class="submitComment f14" id="publish_sc" type="button" value="Publish" onclick="CreateListItem()" />
                    <input class="submitComment f14" type="button" value="Send to author" onclick="SendEmail()" />
                </div>
                <div class="othersCom">
                    <h4 class="oTitle">User Comments</h4>
                    <div id="Comments">
                    </div>
                    <div class="tcdPageCode" style="display: none;">
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="mobile_div" style="display: none;">
        <table id="mobile_table">
            <tr>
                <td colspan="2">
                    <%--<a href="/Documents/Forms/AllItems.aspx?Mobile=1">Global Knowledge System </a>--%>
                    <div id="mobile_title" onclick="javascript :history.back(-1);">
                        <img src="/_layouts/15/Images/back_d.png" alt="Back" />
                    </div>
                </td>
            </tr>
            <tr id="mobile_attr_title">
                <td id="back_div">
                    <img src="/_layouts/15/Images/<%=fileIcon %>" onerror='this.src="/_layouts/15/Images/48_icgen.png"' alt="File" />
                </td>
                <td id="name_div"><%= fileName %></td>
            </tr>
            <tr class="mobile_attr">
                <td>Creator:</td>
                <td><%= fileAuthor %> </td>
            </tr>
            <tr class="mobile_attr">
                <td>CreatTime:</td>
                <td><%= fileCreatTime %> </td>
            </tr>
            <tr class="mobile_attr">
                <td>FileSize:</td>
                <td><%= fileSize %> </td>
            </tr>
            <tr id="btn_div">
                <td colspan="2">
                    <div>
                        <input type="button" onclick="DownloadFile()" value="Download" />
                    </div>
                    <div>
                        <input type="button" onclick="Preview()" value="Preview" />
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <script type="text/javascript">
        var fileFID = "<%=fileFID %>";
        var fileUID = "<%=fileUID %>";
        var fileAuthorEmail = "<%=fileAuthorEmail %>";
        var fileName = "<%=fileName %>";
        fileName = fileName.replace(/'/g, "?");
        var fileUrl = "<%=fileUrl %>";
    </script>
    <script type="text/javascript" src="Scripts/FileDetail.js?v=2017-10-300"></script>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    File Detail
</asp:Content>
