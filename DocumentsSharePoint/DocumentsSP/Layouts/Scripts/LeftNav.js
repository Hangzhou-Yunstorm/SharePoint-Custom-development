
// 加载zTree样式
loadStyles("/_layouts/15/content/leftnav.css");

loadStyles("/_layouts/15/content/ECB.css");

function addFolderTree() {

    // 设置文档库样式
    $("#scriptWPQ1").css("width", "960px");
    document.getElementById("scriptWPQ1").vAlign = "top";

    var nav = "<div class='leftTree'><h3 class='myTitle'>Admin</h3><ul>" +
                   "<li class='primaryDir'><h4>My Documents</h4><ul class='second'>" +
                   "<li id='MyUpload'><a href='/Documents/Forms/MyUpLoad.aspx'>My Uploads</a></li>" +
                   "<li id='MyDownloads'><a href='/Lists/Download History/AllItems.aspx'>My Downloads</a></li>" +
                   "</ul></li>" +
                   "<li class='primaryDir'><h4>Approval List</h4><ul class='second'>" +
                   "<li id='MyRequest'><a href='/Documents/Forms/My Request.aspx'>My Request</a></li>";

    if (userType != "") {
        nav += "<li id='PendingwithMe'><a href='/Documents/Forms/Pending With Me.aspx'>Pending Request</a></li>";
    }

    nav += "<li id='ApprovedRequest'><a href='/Documents/Forms/Approved Request.aspx'>Approved Request</a></li>" +
                "<li id='RejectedRequest'><a href='/Documents/Forms/Rejected Request.aspx'>Rejected Request</a></li>" +
                "</ul></li>" +
                "<li class='primaryDir'><h4>Custom List</h4><ul class='second'>" +
                "<li id='Subscription'><a href='/Lists/SubscribeDirectoryList/AllItems.aspx'>Subscription List</a></li>" +
                "<li id='MyShareCatalog'><a href='/Lists/ShareCatalogList/MyShareCatalog.aspx'>My Share Catalog</a></li>";

    if (userType == administratorUser || userType == subUser) {
        var exurl = "/Lists/ExternalUserList/AllItems.aspx";
        if (userType == administratorUser) {
            exurl = "/Lists/ExternalUserList/AdminView.aspx";
        }
        nav += "<li id='ExternalUser'><a href='" + exurl + "'>ExternalUser List</a></li>";
    }
    if (reportType != "") {
        nav += "<li id='ReviewsStatistics'><a href='/_layouts/15/SystemUseReport.aspx'>Reports</a></li>";
    }
    nav += "</ul></li>";
    nav += "</ul></div>";

    // 添加树td
    var td = '<td valign="top" style="width: 240px; height: 500px;">' + nav + '</td>'
    $("#scriptWPQ1").before(td);
}