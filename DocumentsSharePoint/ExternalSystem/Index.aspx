<%@ Page Title="" Language="C#" MasterPageFile="~/External.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="ExternalSystem.Index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <link href="Plugins/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="Plugins/bootstrap-table/bootstrap-table.min.css" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="Plugins/bootstrap/js/bootstrap.min.js"></script>
    <script type="text/javascript" src="Plugins/bootstrap-table/bootstrap-table.min.js"></script>
    <script type="text/javascript" src="Plugins/layer/layer.js"></script>
    <script src="Plugins/bootstrap/twitter-bootstrap-v2/js/bootstrap-tooltip.js"></script>
    <script src="Plugins/bootstrap/twitter-bootstrap-v2/js/bootstrap-popover.js"></script>
    <link href="Content/index.css?v=2017-11-29" rel="stylesheet" />
    <script src="Plugins/zTree/js/jquery.ztree.all-3.5.min.js"></script>
    <link href="Plugins/zTree/css/zTreeStyle/zTreeStyle.css" rel="stylesheet" />
    <script type="text/javascript">
        var fDatas = <%= fDatas%>;
        var tJson = <%= treeFolderJson%>;
        var userName = "<%= userName%>";
        var userAccount = "<%= userAccount%>";
    </script>
    <script type="text/javascript" src="Scripts/index.js?v=2017-11-29"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <form id="index_form" runat="server" onkeydown="if(event.keyCode==13){return false;}">
        <div id="mbx" style="display: none;">
            <a href="/Index.aspx">Index</a>
        </div>
        <div class="fl" id="folder_tree">
            <div id="left_title"><a onclick="Reload();" style="cursor: pointer;" title="Home">Home</a></div>
            <img id="tree_loading" src="/Images/treel.gif" width="200px" />
            <ul style="display: none" id="treeObject" class="ztree data-ztree"></ul>
        </div>
        <div class="fl" id="f_table">
            <div id="toolbar">
                <img id="upload_img" style="display: none; margin-right: 20px;" src="/Images/upload.png" onclick="UploadFile()" title="Uoload file" alt="Upload file" />
                <img src="/Images/down.png" onclick="DownloadFile()" title="Download file" alt="Download file" />
            </div>
            <table id="file_table" data-toolbar="#toolbar"></table>
        </div>
    </form>
</asp:Content>
