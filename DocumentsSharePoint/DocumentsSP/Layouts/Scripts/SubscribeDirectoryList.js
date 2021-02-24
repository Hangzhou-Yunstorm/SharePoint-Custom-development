
$(function () {

    // 加载zTree
    addFolderTree();

    $(".second li a").css("color", "#4e4e4e");
    $("#Subscription a").css("font-weight", "700");
    $("#Subscription a").css("color", "#0E60AC");
    try {
        ReadJson();
    } catch (e) { }

    // 设置文档库样式
    $("#scriptWPQ1").css("float", "left");
    // 加载样式
    loadStyles("/_layouts/15/content/Whitelist.css");

    var td = "<td valign='top' id='threeButtonTD'><div class='actionBox'><div class='leftBtn'>" +
             "<div onclick='DeleteItem()' style='cursor:pointer;'><img src='/_layouts/15/Images/unsub.png' alt='Cancel Subscribe'></div>";
    if (IsMailPush()) {
        td += "<div onclick='CancelMailPush()' style='cursor:pointer;'><img src='/_layouts/15/Images/cmp.png' alt='Cancel Mail Push'></div>";
    } else {
        td += "<div onclick='MailPush()' style='cursor:pointer;'><img src='/_layouts/15/Images/mailpush.png' alt='Mail Push'></div>";
    }

    td += "</div></div></td>";
    $("#scriptWPQ1").before(td);

});

// 取消订阅
function DeleteItem() {
    var num = ctx.CurrentSelectedItems;
    if (num > 0) {
        SP.Ribbon.PageManager.get_instance().executeRootCommand("Delete", null, null, null);
    } else {
        alert("Please select the item you want to delete!");
    }
}

// 邮件推送
function MailPush() {
    $.ajax({
        type: "post",
        url: "/_layouts/15/GetNav.aspx/MailPush",
        data: "",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            var content = response.d;
            if (content == null || content == "") {
                window.location.reload();
            } else {
                alert(content);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
}

// 取消邮件推送
function CancelMailPush() {
    $.ajax({
        type: "post",
        url: "/_layouts/15/GetNav.aspx/CancelMailPush",
        data: "",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            var content = response.d;
            if (content == null || content == "") {
                window.location.reload();
            } else {
                alert(content);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
}

// 是否推送邮件
function IsMailPush() {
    var isPush = false;
    $.ajax({
        type: "post",
        url: "/_layouts/15/GetNav.aspx/IsMailPush",
        data: "",
        async: false,
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            var content = response.d;
            if (content == "0") {
                isPush = true;
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
    return isPush;
}

