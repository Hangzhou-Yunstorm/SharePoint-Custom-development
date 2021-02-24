// 自带属性
var navBarHelpOverrideKey = "wssmain";
// 文档库名称
var docListName = "";
// 文档库Id
var docListId = "";

// 用户类型
var administratorUser = "Administrator";          // 超级管理员
var subUser = "GKS Manage";                         // 二级管理员
var approverUser = "GKS Approve";                // 审批者
var reportManageUser = "Report Manage";   // 报表二级管理员
var userType = ""; // 用户类型
var reportType = ""; // 报表用户类型
var isUpload = false;
var isPC = false;

$(function () {

    //hide select option--permission level default permission
    $('#ctl00_PlaceHolderMain_DdlGroup').children('option').each(function () { if ($(this).text() == 'GKS_Default_Permission' || $(this).text() == 'GKS_Default_Permision') $(this).hide() });

    $('#ctl00_PlaceHolderMain_ctl01_ctl01_cblRoles').children('tbody').children('tr').children('td').children('label').each(function () { if ($(this).text().lastIndexOf('GKS_Default_Permission') != -1 || $(this).text().lastIndexOf('GKS_Default_Permision') != -1) $(this).parent().parent().hide() })

    var href = window.location.href.toLocaleLowerCase();

    isPC = IsPC();
    if (!isPC) {
        if (href.indexOf("/_layouts/15/filedetail.aspx") < 0 && href.indexOf("/_layouts/15/osssearchresults.aspx") < 0) {
            window.location.href = "/Documents/Forms/AllItems.aspx?Mobile=1";
        }
        loadStyles("/_layouts/15/content/Mobile.css?v=2017-10-30");
    } else {
        // 获取用户手册路径
        getUserManualPath();
        // 获取用户类型
        GetUserType();
        // 获取用户类型
        GetReportUserType();
        // 获取列表信息
        getListInfo();
        // 是否显示上传按钮
        IsShowUpload();
        // 获取用户名
        getUserName();
        // 设置登录日志
        SetLoginLog();
        // 获取导航栏
        getNav();
        // 获取脚标
        getFooter();

        if (userType == "") {
            $("#myApprove").css("display", "none");
            $("#user_pwm").attr("href", "/Lists/Download History/AllItems.aspx");
        } else {
            if (userType == administratorUser || userType == subUser) {
                $("#a_authorization").css("display", "inline-block");
            }
            getApprove();
        }

        if (href.indexOf("/documents/forms/fileapprove.aspx") > -1) {
            $("#logo_ahref").attr("href", "https://gks.dahuasecurity.com/default.aspx");
            $("#title_ahref").attr("href", "https://gks.dahuasecurity.com/default.aspx");
        } else {
            // 加载样式
            loadStyles("/_layouts/15/content/ShowMaster.css");
            $("#logo_ahref").attr("href", "/default.aspx");
            $("#title_ahref").attr("href", "/default.aspx");
        }

        if (href.indexOf("/_layouts/15/adminrecyclebin.aspx") > -1) {
            setRecycleBin();
            $(".second li a").css("color", "#4e4e4e");
            $("#RecycleBin a").css("color", "#0E60AC");
            $("#RecycleBin a").css("font-weight", "700");
        }

        if (href.indexOf("/_layouts/15/role.aspx") > -1 || href.indexOf("/_layouts/15/addrole.aspx") > -1 || href.indexOf("/_layouts/15/editrole.aspx") > -1) {
            setRecycleBin();
            $(".second li a").css("color", "#4e4e4e");
            $("#PermissionSettings a").css("font-weight", "700");
            $("#PermissionSettings a").css("color", "#0E60AC");
        }
        if (href.indexOf("/_layouts/15/user.aspx") > -1) {
            if (href.indexOf("?") > -1) {
                if (href.indexOf("showlimitedaccessusers=true") == -1) {
                    window.location.href = href + "&showLimitedAccessUsers=true";
                }
            } else {
                if (href.indexOf("showlimitedaccessusers=true") == -1) {
                    window.location.href = href + "?showLimitedAccessUsers=true";
                }
            }
            var isRegion = SetMbx();
            setRecycleBin();
            setFolderPermissions(isRegion);
            $(".second li a").css("color", "#4e4e4e");
            $("#Authorization a").css("font-weight", "700");
            $("#Authorization a").css("color", "#0E60AC");
        }
        if (href.indexOf("/_layouts/15/osssearchresults.aspx") > -1) {
            $("#sideNavBox").removeClass("ms-hide");
            document.getElementById("input_search").style.display = "none";
        }
        if (href.indexOf("/Lists/RMSTask/AllItems.aspx") > -1) {
            $("li.ms-core-menu-item[text='Delete Item']").css("display", "inherit !important");
        }
        if (href.indexOf("/_layouts/15/aclinv.aspx") > -1) {
            $("#ctl00_PlaceHolderMain_ctl03_foldLinkWrapper").hide();
            ShowHideMoreOptions();
        }
    }
});

function IsPC() {
    var userAgentInfo = navigator.userAgent;
    var Agents = ["Android", "iPhone",
                "SymbianOS", "Windows Phone",
                "iPad", "iPod"];
    var flag = true;
    for (var v = 0; v < Agents.length; v++) {
        if (userAgentInfo.indexOf(Agents[v]) > 0) {
            flag = false;
            break;
        }
    }
    return flag;
}

// 设置面包屑
function SetMbx() {
    var isRegion = false;
    try {
        var folderId = decodeURIComponent(getQueryString("obj")).split(',')[1];
        $.ajax({
            type: "post",
            url: "/_layouts/15/GetNav.aspx/GetPathById",
            contentType: "application/json;charset=utf-8",
            data: "{ 'folderId': '" + folderId + "'}",
            dataType: "json",
            async: false,
            success: function (response, textStatus) {
                var content = decodeURIComponent(response.d);
                if (content == null || content == "") {
                    isRegion = true;
                }

                var url = "/" + content;
                var mbx = GetMbx(url);
                $("#DeltaPlaceHolderMain").before(mbx);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus);
            }
        });
    }
    catch (e) {
        console.log(e);
    }
    return isRegion;
}

// 获取面包屑
function GetMbx(fUrl) {
    var mbxDiv = "<div class='top_div'><a href=\"" + GetReturnUrl(docListName) + "\" class='top_a' >" + docListName + "</a>";

    var list = fUrl.split('/');
    if (list.length > 2) {
        var fPUrl = "/" + docListName;
        for (var i = 2; i < list.length ; i++) {
            if (list[i] != null && list[i] != "") {
                fPUrl += "/" + list[i];
                mbxDiv += "&nbsp;/&nbsp; <a href=\"" + GetReturnUrl(fPUrl) + "\" class='top_a' >" + list[i] + "</a>";
            }
        }
    }
    mbxDiv += "</div>";
    return mbxDiv;
}

// 获取返回链接
function GetReturnUrl(url) {
    if (url == docListName) {
        return "/" + docListName + "/Forms/Authorization.aspx";
    }
    return "/" + docListName + "/Forms/Authorization.aspx?RootFolder=" + encodeURIComponent(url);
}

// 设置权限按钮
function setFolderPermissions(isRegion) {
    if (document.getElementById("Ribbon.Permission.Manage.StopInherit-Large") == null) {
        var _5btn = "<div style='height:30px;'>";
        if (!isRegion) {
            _5btn += "<div style='float:left;margin-right:10px;cursor:pointer' onclick='inheritPerms()'><img title='Remove unique permissions' src='/_layouts/15/Images/permission/dup.png' alt='Perm_Inherit'></div>";
        }
        _5btn += "<div style='float:left;margin-right:10px;cursor:pointer' onclick='Perm_AddUser()'><img title='Grant permission' src='/_layouts/15/Images/permission/gp.png' alt='Perm_AddUser'></div>" +
         "<div style='float:left;margin-right:10px;cursor:pointer' onclick='EditRolesForSelectedUsers()'><img title='Edit user permission' src='/_layouts/15/Images/permission/eup.png' alt='Perm_EditUsrPerm'></div>" +
         "<div style='float:left;margin-right:10px;cursor:pointer' onclick='deluser()'><img title='Remove user permission' src='/_layouts/15/Images/permission/rup.png' alt='Perm_RemovePerms'></div>" +
         "<div style='float:left;cursor:pointer' onclick='CheckPerms()'><img title='Check permissions' src='/_layouts/15/Images/permission/cp.png' alt='Perm_CheckUsrPerm'></div>" +
         "</div>";

        $("#DeltaPlaceHolderMain").before(_5btn);
    }
    else {
        var _3btn = "<div style='height:30px;'>" +
		"<div style='float:left;margin-right:10px;cursor:pointer' onclick='Perm_ManageParent()'><img title='Manage the parent' src='/_layouts/15/Images/permission/mp.png' alt='Perm_ManageParent'></div>" +
		"<div style='float:left;margin-right:10px;cursor:pointer' onclick='uniquePerms()'><img title='Stop inheritance permissions' src='/_layouts/15/Images/permission/sip.png' alt='Perm_StopInherit'></div>" +
		"<div style='float:left;cursor:pointer' onclick='CheckPerms()'><img title='Check permissions' src='/_layouts/15/Images/permission/cp.png' alt='Perm_CheckUsrPerm'></div>" +
		"</div>";

        $("#DeltaPlaceHolderMain").before(_3btn);
    }

}

// 授予权限
function Perm_AddUser() {
    SP.Ribbon.PageManager.get_instance().executeRootCommand("Perm_AddUser", null, null, null);
}

// 管理父级
function Perm_ManageParent() {
    SP.Ribbon.PageManager.get_instance().executeRootCommand("Perm_ManageParent", null, null, null);
}

// 获取文档库信息
function getListInfo() {
    $.ajax({
        type: "post",
        url: "/_layouts/15/GetNav.aspx/GetListInfo",
        data: "",
        async: false,
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            var list = response.d.split(',');
            docListName = list[0];
            docListId = list[1];
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
}

//获取待我审批的文件数量
function getApprove() {
    $.ajax({
        type: "post",
        url: "/_layouts/15/GetNav.aspx/GetApprove",
        data: "",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            if (response.d == "0") {
                $("#myApprove").hide();
            }
            else {
                $("#myApprove").show();
                $("#myApprove").html(response.d);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
}

// 获取权限
function GetPermission() {
    $.ajax({
        type: "post",
        url: "/_layouts/15/GetNav.aspx/GetPermission",
        data: "",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            if (response.d == "1") {
                window.location.href = "/_layouts/15/AccessDenied.aspx";
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
            window.location.href = "/_layouts/15/AccessDenied.aspx";
        }
    });
}

// 设置左侧目录
function setRecycleBin() {
    if (userType != administratorUser && userType != subUser) {
        window.location.href = "/default.aspx";
    }
    // 加载zTree样式
    loadStyles("/_layouts/15/Content/RecycleBin.css?v=2017-09-07");

    if (userType == administratorUser) {
        var div = "<div class='leftTree'>" +
                  "<h3 class='myTitle'>Admin</h3>" +
                  "<ul><li class='primaryDir'><h4>Library Manage</h4>" +
                  "<ul class='second'>" +
                  "<li id='Authorization'><a href='/Documents/Forms/Authorization.aspx'>Authorization</a></li>" +
                  "<li id='PermissionSettings'><a href='/_layouts/15/role.aspx'>Permission Settings</a></li>" +
                  "<li id='UserGroups'><a href='/_layouts/15/UserGroups.aspx'>User Groups</a></li>" +
                  "<li id='Whitelist'><a href='/Lists/WhiteList/AdminAllItems.aspx'>Whitelist</a></li></ul></li>" +
                  "<li class='primaryDir'><h4>Action Log</h4><ul class='second'>" +
                  "<li id='FileLogs'><a href='/_layouts/15/LogList.aspx'>System Logs</a></li>" +
                  "</ul></li>" +
                  "<li class='primaryDir'><h4>Site Settings</h4><ul class='second'>" +
                  "<li id='HomeGallery'><a href='/Carousel List/Forms/AllItems.aspx'>Home Gallery</a></li>" +
                  "<li id='RecycleBin'><a href='/_layouts/15/AdminRecycleBin.aspx'>The Recycle Bin</a></li>" +
                  "<li><a target='_blank'  href='/Documents/Forms/AllItems.aspx'>Primary Directory</a></li>" +
                  "<li id='ShareCatalogList'><a href='/Lists/ShareCatalogList/AllItems.aspx'>Share Catalog Manage</a></li>" +
                  "<li id='FeedbackList'><a href='/Lists/Suggestion/AllItems.aspx'>Suggestion</a></li>" +
                  "<li id='ReportManage'><a href='/Lists/ReportManageList/AllItems.aspx'>Report Manage</a></li>" +
                  "</ul></li>" +
                  "</ul></div>";
        $("#DeltaPlaceHolderMain").before(div);
    } else if (userType == subUser) {
        var div = "<div class='leftTree'>" +
                 "<h3 class='myTitle'>Admin</h3>" +
                 "<ul><li class='primaryDir'><h4>Library Manage</h4>" +
                 "<ul class='second'>" +
                 "<li id='Authorization'><a href='/Documents/Forms/Authorization.aspx'>Authorization</a></li>" +
                 "<li id='UserGroups'><a href='/_layouts/15/UserGroups.aspx'>User Groups</a></li>" +
                 "<li id='Whitelist'><a href='/Lists/WhiteList/AllItems.aspx'>Whitelist</a></li></ul></li>" +
                 "</ul></div>";
        $("#DeltaPlaceHolderMain").before(div);
    }
}

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
        url: "/_layouts/15/GetNav.aspx/GetUserName",
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

// 获取页脚内容
function getFooter() {
    $.ajax({
        type: "post",
        url: "/_layouts/15/GetNav.aspx/GetFooter",
        data: "",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            $("#copyright").html(response.d);
            //$("#copyright").css("display", "block");
            $(".footer").css("display", "block");
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
}

//设置用户手册路径
function getUserManualPath() {
    $.ajax({
        type: "post",
        url: "/_layouts/15/GetNav.aspx/GetUserManualPath_CN",
        data: "",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            $("#User_Manual_cn").attr("href", response.d);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
    $.ajax({
        type: "post",
        url: "/_layouts/15/GetNav.aspx/GetUserManualPath_EN",
        data: "",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            $("#User_Manual_en").attr("href", response.d);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
}

//是否显示下载按钮
function IsShowUpload() {
    if (isUpload) {
        //$("#img_upload").css("display", "inline-block");
    }
}

// 获取用户类型
function GetUserType() {
    $.ajax({
        type: "post",
        url: "/_layouts/15/GetNav.aspx/GetUserType",
        data: "",
        async: false,
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            //userType = response.d;
            var value = response.d;
            if (value != null || value != "[]") {
                var model = new Function('return ' + value)();
                if (model != null && model != "") {
                    userType = model.UserType;
                    isUpload = model.IsUpload;
                }
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
}

// 获取报表用户类型
function GetReportUserType() {
    $.ajax({
        type: "post",
        url: "/_layouts/15/GetNav.aspx/GetReportUserType",
        data: "",
        async: false,
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            reportType = response.d;
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
        url: "/_layouts/15/GetNav.aspx/GetNavs",
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

//打开上传页面
function OpenNewFormUrl() {
    var url = "/" + docListName + "/Forms/upload.aspx";
    var options = {};
    SP.SOD.executeFunc('sp.ui.dialog.js', 'SP.UI.ModalDialog.showModalDialog', function () {
        SP.UI.ModalDialog.commonModalDialogOpen(url, options, null, null);
    });
}

//添加用户反馈
function OpenFeedbackNewForm() {
    var url = "/Lists/Suggestion/NewForm.aspx";
    SP.SOD.executeFunc('sp.ui.dialog.js', 'SP.UI.ModalDialog.showModalDialog', function () {
        SP.UI.ModalDialog.commonModalDialogOpen(url, {}, null, null);
    });
}

//搜索
function SearchSP() {
    var skey = $("#inputSearch").val().trim();;
    if (skey == "") {
        return;
    }
    //var ahref = "/_Layouts/15/osssearchresults.aspx?k=" + encodeURIComponent(skey);
    //var defaultKey = encodeURIComponent('{"k":"' + skey + '","o":[{"p":"LastModifiedTime","d":1}],"l":1033}');
    var defaultKey = encodeURIComponent('{"k":"' + skey + '","l":1033}');
    var ahref = "/_Layouts/15/osssearchresults.aspx#Default=" + defaultKey;

    var href = window.location.href;
    if (href != null && href != "") {
        href = href.toLocaleLowerCase();
        if (href.indexOf("/_layouts/15/osssearchresults.aspx") > -1) {
            window.location.href = ahref;
        } else {
            window.open(ahref);
        }
    }
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
function ReadJson() {
    // 设置禁止拖拉文件上传
    setNoUpload();
}

// 设置禁止拖拉文件上传
function setNoUpload() {
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

///
// 文档库专用
///
// 批量下载
function MultipleDownLoad() {
    var num = ctx.CurrentSelectedItems;
    var c = ctx.dictSel;

    if (num == 1) {
        var id = "";
        var isFile = false;
        for (var key in c) {
            var type = c[key].fsObjType;
            // 文件
            if (type == "0") {
                isFile = true;
                id = c[key].id;
            }

        };
        if (isFile) {
            var url = ctx.HttpRoot + '/_layouts/15/FileDownload.aspx?ItemID=' + id;
            window.open(url);
        } else {
            alert("Please select the files to download!");
            return;
        }
    }
    else if (num > 0) {
        var ids = "";
        for (var key in c) {
            var type = c[key].fsObjType;
            // 文件
            if (type == "0") {
                var id = c[key].id;
                ids = ids + id + ',';
            }
        }
        if (ids != "") {
            DownloadAll(ids);
        } else {
            alert("Please select the files to download!");
            return;
        }
    }
    else {
        alert("Please select the files to download!");
        return;
    }
}

//批量下载
function DownloadAll(ids) {
    //var index = layer.load(0, { shade: false });
    var index = layer.load(0, {
        shade: [0.1, '#f2f2f2'] //透明度，背景颜色
    });
    $.ajax({
        type: "post",
        url: "/_layouts/15/MulDownload.aspx/DownLoadZIP",
        contentType: "application/json;charset=utf-8",
        data: "{'listId': '" + ids + "'}",
        dataType: "json",
        success: function (response, textStatus) {
            var value = response.d;
            if (value == null || value == "[]") {
                alert("Download error,please try again!");
            }
            else {
                var model = new Function('return ' + value)();
                if (model != null && model != "") {
                    if (model.msg == "0") {
                        window.location.href = model.url;
                    }
                    else if (model.msg == "1") {
                        alert("Please note that: files(under RMS encrypting) are unavailable for download !");
                        window.location.href = model.url;
                    }
                    else if (model.msg == "2") {
                        alert("All items that you selected，do not include available files(under RMS encrypting), unavailable for download !");
                    }
                    else if (model.msg == "3") {
                        alert("Please select file to download !");
                    }
                    else {
                        alert("Download error,please try again!");
                    }
                }
                else {
                    alert("Download error,please try again!");
                }
            }
            layer.close(index);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(textStatus);
            layer.close(index);
        },
    });
}