$(function () {
    if (userType != administratorUser) {
        window.location.href = "/default.aspx";
        return;
    }

    try {
        addFolderTree();
    } catch (e) { }

    $(".second li a").css("color", "#4e4e4e");
    $("#ShareCatalogList a").css("font-weight", "700");
    $("#ShareCatalogList a").css("color", "#0E60AC");

    // 设置文档库样式
    $("#scriptWPQ1").css("float", "left");
    // 加载样式
    loadStyles("/_layouts/15/content/Whitelist.css");

    var td = "<td valign='top' id='threeButtonTD'><div class='actionBox'><div class='leftBtn'>" +
                  "<div onclick='DeleteItem()' style='margin-right:10px;cursor:pointer'><img title='Delete Item' src='/_layouts/15/Images/del.png' alt='Delete Item'></div>" +
                  "</div></div></td>";
    $("#scriptWPQ1").before(td);
});

// 删除
function DeleteItem() {
    var num = ctx.CurrentSelectedItems;
    if (num > 0) {
        SP.Ribbon.PageManager.get_instance().executeRootCommand("Delete", null, null, null);
    } else {
        alert("Please select the item you want to delete!");
    }
}