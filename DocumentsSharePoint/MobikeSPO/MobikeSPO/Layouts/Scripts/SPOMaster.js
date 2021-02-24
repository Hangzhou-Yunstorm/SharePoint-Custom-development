
$(function () {
    // 设置登录日志
    //SetLoginLog();

    // 获取用户名
    getUserName()

    // 获取导航栏
    getNav();

    var s = Number(document.body.scrollWidth);
    if (s > 1366) {
        $(".myInputBox").css("width", "280px")
        $(".mySearchButton img").css("right", "283px")
    }
});


// 设置登录日志
function SetLoginLog() {
    $.ajax({
        type: "post",
        url: "/_layouts/15/GetNav.aspx/SetLoginLog",
        data: "",
        async: false,
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            // nothing
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
}

// 获取用户名
function getUserName() {
    $.ajax({
        type: "post",
        url: "/_layouts/15/HelperPage.aspx/GetUserName",
        data: "",
        async: false,
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            $("#user_name").html(response.d);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
}

// 获取导航
function getNav() {
    $.ajax({
        type: "post",
        url: "/_layouts/15/HelperPage.aspx/GetNavs",
        data: "",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            $("#masterul").html(response.d);
            setNav();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
}

// 设置导航样式
function setNav() {
    $("#headNav .secondNav").hover(function () {
        $(this).parent().children("a").css({ "color": "#fff", "backgroundColor": "#074682" });
    }, function () {
        $(this).parent().children("a").css({ "color": "#fff", "backgroundColor": "#0E60AC" })
    });
}

//获取参数
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) {
        return decodeURI(r[2]);
    }
    return null;
}


//搜索
function SearchSP() {
    //var skey = $("#inputSearch").val().trim();;
    //if (skey == "") {
    //    return;
    //}

    //var defaultKey = encodeURIComponent('{"k":"' + skey + '","l":1033}');
    //var ahref = "/_Layouts/15/osssearchresults.aspx#Default=" + defaultKey;

    //var href = window.location.href;
    //if (href != null && href != "") {
    //    href = href.toLocaleLowerCase();
    //    if (href.indexOf("/_layouts/15/osssearchresults.aspx") > -1) {
    //        window.location.href = ahref;
    //    } else {
    //        window.open(ahref);
    //    }
    //}
}

// 回车键搜索
function entersearch() {
    var event = window.event || arguments.callee.caller.arguments[0];
    if (event.keyCode == 13) {
        SearchSP();
    }
}

///
// 文档库专用（仅文件）
///
// 设置禁止拖拉文件上传
function SetNoUpload() {
    // 加载样式
    loadStyles("/_layouts/15/Content/NoUpload.css");
    ExecuteOrDelayUntilScriptLoaded(function () {
        g_uploadType = DragDropMode.NOTSUPPORTED;
        SPDragDropManager.DragDropMode = DragDropMode.NOTSUPPORTED;
    }, "DragDrop.js");
}

// 动态加载css文件
function loadStyles(url) {
    var link = document.createElement("link");
    link.type = "text/css";
    link.rel = "stylesheet";
    link.href = url;
    document.getElementsByTagName("head")[0].appendChild(link);
}