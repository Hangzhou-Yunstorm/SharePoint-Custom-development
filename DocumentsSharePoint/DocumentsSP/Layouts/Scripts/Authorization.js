$(function () {
    if (userType != administratorUser && userType != subUser) {
        window.location.href = "/default.aspx";
        return;
    }
    try {
        addFolderTree();
        setNoUpload();
    } catch (e) { }

    $(".second li a").css("color", "#4e4e4e");
    $("#Authorization a").css("color", "#0E60AC");
    $("#Authorization a").css("font-weight", "700");

    // 加载样式
    loadStyles("/_layouts/15/content/Whitelist.css");

    // 设置文档库样式
    $("#scriptWPQ1").css("float", "left");
    $("#scriptWPQ1").css("width", "960px");
    document.getElementById("scriptWPQ1").vAlign = "top";
    var td = "<td valign='top' id='threeButtonTD'><div class='actionBox'>" +
        "<div class='leftBtn' onclick='GShareFolder()'><img title='Advanced sharing' src='/_layouts/15/Images/permission/aauth.png' alt='Share folder'></div>" +
        "</div></td>";
    $("#scriptWPQ1").before(td);

    // 添加面包屑
    var fUrl = decodeURIComponent(ctx.rootFolder);
    var mbx = GetMbx(fUrl);
    $("#DeltaPlaceHolderMain").before(mbx);
});

function ShareFolder() {
    var num = ctx.CurrentSelectedItems;

    if (num == 1) {
        SP.Ribbon.PageManager.get_instance().executeRootCommand("ShareItem", null, null, null);
    }
    else {
        alert("Please select one and only one folder to share !");
    }
}

function GShareFolder() {
    var num = ctx.CurrentSelectedItems;
    var c = ctx.dictSel;

    if (num == 1) {
        var id = "";
        for (var key in c) {
            id = c[key].id;
        };
        var url = "/_layouts/15/User.aspx?List=" + docListId + "&obj=" + docListId + "," + id + ",LISTITEM&showLimitedAccessUsers=true";
        window.open(url);
    }
    else {
        alert("Please select one and only one folder to share !");
    }
}
