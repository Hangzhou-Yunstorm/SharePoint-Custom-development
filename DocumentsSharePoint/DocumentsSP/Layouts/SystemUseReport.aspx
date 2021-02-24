<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SystemUseReport.aspx.cs" Inherits="DocumentsSP.Layouts.SystemUseReport" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <link href="Content/Report.css?v=2017-09-18" rel="stylesheet" />
    <script type="text/javascript" src="Plugins/hcharts/highcharts.js"></script>
    <script type="text/javascript" src="Plugins/hcharts/no-data-to-display.src.js"></script>
    <script type="text/javascript" src="Plugins/hcharts/exporting.js"></script>
    <script type="text/javascript" src="Plugins/hcharts/export-xls.js"></script>
    <link href="Plugins/DropKick/css/dropkick.css" rel="stylesheet" />
    <script type="text/javascript" src="Plugins/DropKick/jquery.dropkick-min.js"></script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div class="review">
        <div class='leftTree'>
            <h3 class='myTitle'>Admin</h3>
              <ul>
                <li class='primaryDir'>
                    <h4>My Documents</h4>
                    <ul class='second'>
                        <li id='MyUpload'><a href='/Documents/Forms/MyUpLoad.aspx'>My Uploads</a></li>
                        <li id='MyDownloads'><a href='/Lists/Download History/AllItems.aspx'>My Downloads</a></li>
                    </ul>
                </li>
                <li class='primaryDir'>
                    <h4>Approval List</h4>
                    <ul class='second'>
                        <li id='MyRequest'><a href='/Documents/Forms/My Request.aspx'>My Request</a></li>
                        <li id='PendingwithMe' style="display: none;"><a href='/Documents/Forms/Pending With Me.aspx'>Pending Request</a></li>
                        <li id='ApprovedRequest'><a href='/Documents/Forms/Approved Request.aspx'>Approved Request</a></li>
                        <li id='RejectedRequest'><a href='/Documents/Forms/Rejected Request.aspx'>Rejected Request</a></li>
                    </ul>
                </li>
                <li class='primaryDir'>
                    <h4>Custom List</h4>
                    <ul class='second'>
                        <li id='Subscription'><a href='/Lists/SubscribeDirectoryList/AllItems.aspx'>Subscription List</a></li>
                        <li id='MyShareCatalog'><a href='/Lists/ShareCatalogList/MyShareCatalog.aspx'>My Share Catalog</a></li>
                        <li id='ExternalUser' style="display: none;"><a id="ExternalUser_a" href='/Lists/ExternalUserList/AllItems.aspx'>ExternalUser List</a></li>
                        <li id='ReviewsStatistics'><a href='/_layouts/15/SystemUseReport.aspx'>Reports</a></li>
                    </ul>
                </li>
            </ul>
        </div>

        <div class="rightChart">
            <div class="title">Report</div>
            <div class="titile_select">
                <select class="jumpMenu" tabindex="4">
                    <option selected="selected" value="/_layouts/15/SystemUseReport.aspx">System Use</option>
                    <option value="/_layouts/15/MostDownloadsReport.aspx">Most Downloads</option>
                    <option value="/_layouts/15/DocumentsScoreReport.aspx">Documents Score</option>
                    <option value="/_layouts/15/DepartDownloadsReport.aspx">Department Downloads</option>
                    <option value="/_layouts/15/DepartmentUploadsReport.aspx">Department Uploads</option>
                    <option value="/_layouts/15/UserUploadsReport.aspx">User Uploads</option>
                </select>
            </div>
            <div class="title_date">
                <input type="button" class="get_btn" value="Generation" onclick="GenerationData()" />
            </div>
            <div class="title_date" id="month_select">
                <select class="month_type" tabindex="13">
                    <option selected="selected" value="All">All</option>
                    <option value="1">1</option>
                    <option value="2">2</option>
                    <option value="3">3</option>
                    <option value="4">4</option>
                    <option value="5">5</option>
                    <option value="6">6</option>
                    <option value="7">7</option>
                    <option value="8">8</option>
                    <option value="9">9</option>
                    <option value="10">10</option>
                    <option value="11">11</option>
                    <option value="12">12</option>
                </select>
            </div>
            <div class="title_date" id="year_select">
                <select class="year_type" tabindex="6">
                    <option selected="selected" value="0"><%=DateTime.Now.Year %></option>
                    <option value="1"><%=DateTime.Now.Year-1 %></option>
                    <option value="2"><%=DateTime.Now.Year-2 %></option>
                    <option value="3"><%=DateTime.Now.Year-3 %></option>
                    <option value="4"><%=DateTime.Now.Year-4 %></option>
                    <option value="5"><%=DateTime.Now.Year-5 %></option>
                </select>
            </div>
            <div id="myChart"></div>
        </div>
    </div>
    <script type="text/javascript">
        var tcount = "<%= TotalCount%>";
    </script>
    <script type="text/javascript" src="Scripts/SystemUseReport.js?v=2017-11-2"></script>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    System Use Report
</asp:Content>

