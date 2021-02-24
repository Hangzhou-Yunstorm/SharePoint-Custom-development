$(function () {
    // 设置样式
    $("#scriptWPQ1").css("float", "left");
    // 加载样式
    loadStyles("/_layouts/15/content/Whitelist.css");

    var td = "<td valign='top' style='width:100%;height:50px;float:left;'><div class='actionBox'><div class='leftBtn'>" +
		          "<div onclick='AddItem()' style='margin-right:10px;cursor:pointer'><img title='Add Item' src='/_layouts/15/Images/add.png' alt='Add Item'></div>" +
                  "<div onclick='DeleteItem()' style='margin-right:10px;cursor:pointer'><img title='Delete Item' src='/_layouts/15/Images/del.png' alt='Delete Item'></div>" +
                  "<div onclick='UpdateItem()' style='cursor:pointer'><img title='Update Item' src='/_layouts/15/Images/edit.png' alt='Update Item'></div>" +
                  "</div></div></td>";
    $("#scriptWPQ1").before(td);
	
	$('#CSRListViewControlDivWPQ1').attr('style','display:none !important');    
    $('#CSRListViewControlDivWPQ1').css('display','none');

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