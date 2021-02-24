<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestSearch.aspx.cs" Inherits="DahuaWeb.TestSearch" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Search</title>
    <script src="JS/jquery-1.9.1.min.js"></script>
    <script>

        function btnSumit_click() {
            var key = encodeURIComponent($("#search_text").val());
            var path = encodeURIComponent($("#search_path").val());

            $.ajax({
                type: "post",
                url: "GetSearch.ashx?Name=" + key + "&Folder=" + path + "&Token=" + "<%= token%>",
                success: function (joResult) {
                    alert(joResult.Msg);
                    $("#Json").html(joResult.Data);
                }
            })
        }

    </script>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            Path:<input type="text" id="search_path" />
            Key:<input type="text" id="search_text" />
            <input type="button" value="search" onclick="btnSumit_click()" />
        </div>
        <div id="Json">
        </div>
    </form>
</body>
</html>
