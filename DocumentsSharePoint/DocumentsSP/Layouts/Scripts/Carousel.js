$(function () {

    if (userType != administratorUser && userType != subUser) {
        window.location.href = "/default.aspx";
        return;
    }

    try {
        addFolderTree();
    } catch (e) { }

    // 加载zTree样式
    loadStyles("/_layouts/15/content/Carousel.css");

    $(".second li a").css("color", "#4e4e4e");
    $("#HomeGallery a").css("color", "#0E60AC");
    $("#HomeGallery a").css("font-weight", "700");

    // 加载样式
    loadStyles("/_layouts/15/content/Whitelist.css");

    var td = "<td valign='top' id='threeButtonTD'><div class='actionBox'>" +
		"<div class='leftBtn'>" +
		"<div onclick='AddPicture()' style='margin-right:10px;cursor:pointer'><img title='Add Item' src='/_layouts/15/Images/add.png' alt='Add Item'></div>" +
		"<div onclick='DeletePicture()' style='margin-right:10px;cursor:pointer'><img title='Delete Item' src='/_layouts/15/Images/del.png' alt='Delete Item'></div>" +
		"<div onclick='UpdatePicture()' style='cursor:pointer'><img title='Update Item' src='/_layouts/15/Images/edit.png' alt='Update Item'></div>" +
		"</div></div></td>";
    $("#scriptWPQ1").before(td);

    $("#scriptWPQ1").css("float", "left");
    //var text = "Note : banner image width should be 1200px, height should be 300px, image file size shoule be less than 4MB";
    var text = "Note : banner image size  should be 1200 x 300, image file size shoule be less than 4MB";
    $("#scriptWPQ1").after('<td valign="top" style="width: 1000px; height: 80px; float:left;"><div style="margin:50px 30px 0"><span style="color:red">*</span>' + text + '</div></td>');
});

// 上传图片
function AddPicture() {
    SP.Ribbon.PageManager.get_instance().executeRootCommand("UploadDocument", null, null, null);
}

// 编辑图片
function UpdatePicture() {
    var num = ctx.CurrentSelectedItems;
    if (num == 1) {
        SP.Ribbon.PageManager.get_instance().executeRootCommand("EditProperties", null, null, null);
    } else {
        alert("Please select an image for editing!");
    }

}

// 删除图片
function DeletePicture() {
    var num = ctx.CurrentSelectedItems;
    if (num > 0) {
        SP.Ribbon.PageManager.get_instance().executeRootCommand("Delete", null, null, null);
    } else {
        alert("Please select the images you want to delete!");
    }
}