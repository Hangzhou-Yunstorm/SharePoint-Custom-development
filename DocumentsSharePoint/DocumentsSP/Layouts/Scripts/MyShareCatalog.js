
$(function () {

    // 加载zTree
    addFolderTree();

    $(".second li a").css("color", "#4e4e4e");
    $("#MyShareCatalog a").css("font-weight", "700");
    $("#MyShareCatalog a").css("color", "#0E60AC");
    try {
        ReadJson();
    } catch (e) { }

    // 设置文档库样式
    $("#scriptWPQ1").css("float", "left");
    // 加载样式
    loadStyles("/_layouts/15/content/Whitelist.css");

    var td = "<td valign='top' id='threeButtonTD'><div class='actionBox'><div class='leftBtn'>" +
             "<div onclick='DeleteItem()' style='cursor:pointer;'><img src='/_layouts/15/Images/del.png' alt='Cancel Share'></div>" +
             "</div></div></td>";
    $("#scriptWPQ1").before(td);

});

// 取消分享
function DeleteItem() {
    var num = ctx.CurrentSelectedItems;
    if (num > 0) {
        SP.Ribbon.PageManager.get_instance().executeRootCommand("Delete", null, null, null);
    } else {
        alert("Please select the item you want to delete!");
    }
}
