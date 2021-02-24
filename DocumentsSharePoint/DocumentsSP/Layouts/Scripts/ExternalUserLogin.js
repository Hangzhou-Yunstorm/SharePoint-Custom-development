
$(function () {
    if (userType != administratorUser && userType != subUser) {
        window.location.href = "/Documents/Forms/My%20Request.aspx";
        return;
    }

    // 校验管理员，防止篡改链接访问
    var href = window.location.href;
    if (href != null && href != "") {
        href = href.toLocaleLowerCase();
        if (href.indexOf("/lists/externaluserlist/adminview.aspx") > -1 && userType != administratorUser) {
            window.location.href = "/Lists/ExternalUserList/AllItems.aspx";
            return;
        }
    }

    // 加载zTree
    addFolderTree();

    $(".second li a").css("color", "#4e4e4e");
    $("#ExternalUser a").css("font-weight", "700");
    $("#ExternalUser a").css("color", "#0E60AC");
    try {
        ReadJson();
    } catch (e) { }

    // 加载样式
    loadStyles("/_layouts/15/content/Whitelist.css");
    loadStyles("/_layouts/15/content/PendingwithMe.css");
    // 设置文档库样式
    $("#scriptWPQ1").css("float", "left");
    $("#scriptWPQ1").css("width", "960px");
    document.getElementById("scriptWPQ1").vAlign = "top";
    var td = "<td valign='top' id='threeButtonTD'><div class='actionBox'>" +
                  "<div onclick='AddItem()' style='margin-right:10px;cursor:pointer;float:left;'><img title='Add Item' src='/_layouts/15/Images/add.png' alt='Add Item'></div>" +
                  "<div onclick='UpdateItem()' style='margin-right:10px;cursor:pointer;float:left;'><img title='Update Item' src='/_layouts/15/Images/edit.png' alt='Update Item'></div>" +
                  "<div onclick='DeleteItem()' style='margin-right:10px;cursor:pointer;float:left;'><img title='Delete Item' src='/_layouts/15/Images/del.png' alt='Delete Item'></div>" +
                  //"<div onclick='ReturnRegion()' style='margin-right:10px;cursor:pointer;float:left;'><img title='Region Item' src='/_layouts/15/Images/RegionM.png' alt='Region Item'></div>" +
                  //"<div onclick='ReturnCountry()' style='cursor:pointer;float:left;'><img title='Country Item' src='/_layouts/15/Images/CountryM.png' alt='Country Item'></div>" +
                  "</div></td>";
    $("#scriptWPQ1").before(td);

});

function ReturnRegion() {
    window.location.href = "/Lists/RegionList/AllItems.aspx";
}

function ReturnCountry() {
    window.location.href = "/Lists/RegionCountryList/AllItems.aspx";
}

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
