<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Upload.aspx.cs" Inherits="ExternalSystem.Upload" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Upload File</title>
    <script type="text/javascript" src="Scripts/jquery-1.9.1.min.js"></script>
    <script src="Plugins/layer/layer.js"></script>
    <link href="Content/Upload.css?v=2017-11-29" rel="stylesheet" />
    <script src="Scripts/upload.js?v=2017-11-29"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="upload">
            <div class="upload_div file_div">
                <asp:Label ID="SelectLabel" CssClass="w100" runat="server" Text="Select File :"></asp:Label>
                <asp:FileUpload ID="FileUpload" runat="server" />
            </div>
            <div class="upload_div">
                <asp:Label ID="FolderLabel" runat="server" CssClass="w100" Text="Folder Path :"></asp:Label>
                <asp:Label ID="PathLabel" runat="server" Text=""></asp:Label>
            </div>
            <div class="upload_div">
                <p style="font-size: 12px;"><span style="color: red;">*</span> Maximum file cannot exceed 1G !</p>
            </div>
            <div class="upload_div confirm">
                <asp:Button ID="UploadBtn" OnClick="UploadFile" UseSubmitBehavior="false" runat="server" Text="Upload" />
                <input type="button" value="Upload" class="cancel" onclick="uploadfile()" />
                <input type="button" value="Cancel" class="cancel" onclick="CloseWindow()" />
            </div>
            <div style="display: none;">
                <asp:Label ID="FolderId" runat="server"></asp:Label>
                <asp:Label ID="Sharer" runat="server"></asp:Label>
                <asp:Label ID="ShareTime" runat="server"></asp:Label>
                <asp:Label ID="Expiration" runat="server"></asp:Label>
                <asp:Label ID="CanWrite" runat="server"></asp:Label>
                <asp:Label ID="PId" runat="server"></asp:Label>
            </div>
        </div>
    </form>
</body>
</html>
