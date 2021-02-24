<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SyncFolder.aspx.cs" Inherits="ExternalSystem.SyncFolder" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>SyncFolder</title>
</head>
<body>
    <form id="sync_form" runat="server" onkeydown="if(event.keyCode==13){return false;}">
        <div id="sync">
            <asp:Button ID="SyncFolderBtn" OnClick="SyncFolders" UseSubmitBehavior="false" runat="server" Text="SyncFolders" />
        </div>
        <br />
        <br />
        <div id="approve">
            <asp:Button ID="SyncApproveFoldersBtn" OnClick="SyncApproveFolders" UseSubmitBehavior="false" runat="server" Text="SyncApproveFolders" />
        </div>
    </form>
</body>
</html>
