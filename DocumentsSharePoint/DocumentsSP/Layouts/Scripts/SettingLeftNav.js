
// 加载zTree样式
loadStyles("/_layouts/15/content/leftnav.css");

loadStyles("/_layouts/15/content/ECB.css");

function addFolderTree() {

    // 设置文档库样式
    $("#scriptWPQ1").css("width", "960px");
    document.getElementById("scriptWPQ1").vAlign = "top";

    if (userType == administratorUser) {
        var nav = "<div class='leftTree'><h3 class='myTitle'>Admin</h3><ul>" +
                       "<li class='primaryDir'><h4>Library Manage</h4><ul class='second'>" +
                       "<li id='Authorization'><a href='/Documents/Forms/Authorization.aspx'>Authorization</a></li>" +
                       "<li id='PermissionSettings'><a href='/_layouts/15/role.aspx'>Permission Settings</a></li>" +
                       "<li id='UserGroups'><a href='/_layouts/15/UserGroups.aspx'>User Groups</a></li>" +
                       "<li id='Whitelist'><a href='/Lists/WhiteList/AdminAllItems.aspx'>Whitelist</a></li>" +
                       "</ul></li>" +
                       "<li class='primaryDir'><h4>Action Log</h4><ul class='second'>" +
                       "<li id='FileLogs'><a href='/_layouts/15/LogList.aspx'>System Logs</a></li>" +
                       "</ul></li>" +
                       "<li class='primaryDir'><h4>Site Settings</h4><ul class='second'>" +
                       "<li id='HomeGallery'><a href='/Carousel List/Forms/AllItems.aspx'>Home Gallery</a></li>" +
                       "<li id='RecycleBin'><a href='/_layouts/15/AdminRecycleBin.aspx'>The Recycle Bin</a></li>" +
                       "<li><a target='_blank'  href='/Documents/Forms/AllItems.aspx'>Primary Directory</a></li>" +
                       "<li id='ShareCatalogList'><a href='/Lists/ShareCatalogList/AllItems.aspx'>Share Catalog Manage</a></li>" +
                       "<li id='FeedbackList'><a href='/Lists/Suggestion/AllItems.aspx'>Suggestion</a></li>" +
                       "<li id='ReportManage'><a href='/Lists/ReportManageList/AllItems.aspx'>Report Manage</a></li>" +
                       "</ul></li>" +
                       "</ul></div>";

        // 添加树td
        var td = '<td valign="top" style="width: 240px; height: 500px;">' + nav + '</td>'
        $("#scriptWPQ1").before(td);
    } else if (userType == subUser) {
        var nav = "<div class='leftTree'><h3 class='myTitle'>Admin</h3><ul>" +
                        "<li class='primaryDir'><h4>Library Manage</h4><ul class='second'>" +
                        "<li id='Authorization'><a href='/Documents/Forms/Authorization.aspx'>Authorization</a></li>" +
                        "<li id='UserGroups'><a href='/_layouts/15/UserGroups.aspx'>User Groups</a></li>" +
                        "<li id='Whitelist'><a href='/Lists/WhiteList/AllItems.aspx'>Whitelist</a></li>" +
                        "</ul></li></ul></div>";

        // 添加树td
        var td = '<td valign="top" style="width: 240px; height: 500px;">' + nav + '</td>'
        $("#scriptWPQ1").before(td);
    }
}