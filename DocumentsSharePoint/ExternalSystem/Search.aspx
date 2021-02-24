<%@ Page Title="" Language="C#" MasterPageFile="~/External.Master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="ExternalSystem.Search" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <link href="Plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="Plugins/bootstrap-table/bootstrap-table.min.css" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="Plugins/bootstrap/js/bootstrap.min.js"></script>
    <script type="text/javascript" src="Plugins/bootstrap-table/bootstrap-table.min.js"></script>
    <script type="text/javascript" src="Plugins/layer/layer.js"></script>
    <script src="Plugins/bootstrap/twitter-bootstrap-v2/js/bootstrap-tooltip.js"></script>
    <script src="Plugins/bootstrap/twitter-bootstrap-v2/js/bootstrap-popover.js"></script>
    <link href="Content/search.css?v=2017-11-11" rel="stylesheet" />
    <script type="text/javascript">
        var userName = "<%= userName%>";
        var userAccount = "<%= userAccount%>";
        var region = "<%= region%>";
        var country = "<%= country%>";
    </script>
    <script type="text/javascript" src="Scripts/search.js?v=2017-10-30"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <form id="search_form" runat="server" onkeydown="if(event.keyCode==13){return false;}">
        <div class="fl" id="f_table">
            <div id="toolbar">
                <%--<input type="text" id="search_key" placeholder="Search" />--%>
                <img src="/Images/down.png" onclick="DownloadFile()" title="Download file" alt="Download file" />
            </div>
            <table id="file_table" data-toolbar="#toolbar"></table>
        </div>
    </form>
</asp:Content>
