$(function () {
    if (userType != administratorUser && userType != subUser) {
        window.location.href = "/default.aspx";
        return;
    }

    // 校验管理员，防止篡改链接访问
    var href = window.location.href;
    if (href != null && href != "") {
        href = href.toLocaleLowerCase();
        if (href.indexOf("/lists/whitelist/adminallitems.aspx") > -1 && userType != administratorUser) {
            window.location.href = "/Lists/WhiteList/AllItems.aspx";
            return;
        }
    }

    try {
        addFolderTree();
    } catch (e) { }

    $(".second li a").css("color", "#4e4e4e");
    $("#ReportManage a").css("font-weight", "700");
    $("#ReportManage a").css("color", "#0E60AC");

    // 设置文档库样式
    $("#scriptWPQ1").css("float", "left");
    // 加载样式
    loadStyles("/_layouts/15/content/Whitelist.css");

    var td = "<td valign='top' id='threeButtonTD'><div class='actionBox'><div class='leftBtn'>" +
		          "<div onclick='AddItem()' style='margin-right:10px;cursor:pointer'><img title='Add Item' src='/_layouts/15/Images/add.png' alt='Add Item'></div>" +
                  "<div onclick='DeleteItem()' style='margin-right:10px;cursor:pointer'><img title='Delete Item' src='/_layouts/15/Images/del.png' alt='Delete Item'></div>" +
                  "<div onclick='UpdateItem()' style='cursor:pointer'><img title='Update Item' src='/_layouts/15/Images/edit.png' alt='Update Item'></div>" +
                  "</div></div></td>";
    $("#scriptWPQ1").before(td);
});

// 添加
function AddItem() {
    SP.Ribbon.PageManager.get_instance().executeRootCommand("NewDefaultListItem", null, null, null);
}

// 编辑
function UpdateItem() {
    var num = ctx.CurrentSelectedItems;
    if (num == 1) {
        SP.Ribbon.PageManager.get_instance().executeRootCommand("EditProperties", null, null, null);
    } else {
        alert("Please select an item for editing!");
    }

}

// 删除
function DeleteItem() {
    var num = ctx.CurrentSelectedItems;
    if (num > 0) {
        SP.Ribbon.PageManager.get_instance().executeRootCommand("Delete", null, null, null);
    } else {
        alert("Please select the item you want to delete!");
    }
}