
$(function () {
    if (userType == "") {
        window.location.href = "/Lists/Download History/AllItems.aspx";
        return;
    }
    // 加载zTree
    addFolderTree();

    $(".second li a").css("color", "#4e4e4e");
    $("#PendingwithMe a").css("font-weight", "700");
    $("#PendingwithMe a").css("color", "#0E60AC");
    try {
        ReadJson();
    } catch (e) { }

    // 加载样式
    loadStyles("/_layouts/15/content/Whitelist.css?v=2017-12-20");
    loadStyles("/_layouts/15/content/PendingwithMe.css?v=2017-12-20");
    // 设置文档库样式
    $("#scriptWPQ1").css("float", "left");
    $("#scriptWPQ1").css("width", "960px");
    document.getElementById("scriptWPQ1").vAlign = "top";
    var td = "<td valign='top' id='threeButtonTD'><div class='actionBox'>" +
        "<div class='leftBtn' onclick='ModerateRibbon()' style='cursor:pointer;float:left;'><img src='/_layouts/15/Images/permission/ar.png' alt='Moderate files'></div>" +
        "<div id='change_mark'><ul id= 'f_ul'><li><img src='/_layouts/15/images/SwitchSite.png' alt='Switch Site' /><ul><li id='f_li'></li>" +
        GetOthersCountryLi() +
        "</ul></li></ul></div>" +
        "</div></td>";
    $("#scriptWPQ1").before(td);

});

// 获取li
function GetOthersCountryLi() {
    var li;
    $.ajax({
        type: "post",
        url: "/_layouts/15/GetNav.aspx/GetOthersCountryLi",
        data: "",
        async: false,
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            li = response.d;

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
    return li;
}

// 文件审批
function ModerateRibbon() {
    var num = ctx.CurrentSelectedItems;
    if (num > 0) {
        SP.Ribbon.PageManager.get_instance().executeRootCommand("Moderate", null, null, null);
    }
    else {
        alert("Please select the files to approval!");
    }
}