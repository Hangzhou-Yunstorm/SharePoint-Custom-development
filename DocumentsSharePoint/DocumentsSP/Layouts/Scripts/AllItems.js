// 是否可以添加
var isAdd = false;
// 是否可以导出
var isExport = false;

$(function () {
    // 加载样式
    loadStyles("/_layouts/15/content/AllItems.css?v=2017-10-12");
    // 添加TD
    addTD();

    // 添加面包屑
    var fUrl = decodeURIComponent(ctx.rootFolder);
    var mbx = GetDocumentMbx(fUrl);
    $("#DeltaPlaceHolderMain").before(mbx);
});

// 上传多个文件
function UploadFiles() {
    layer.open.constructor.prototype.callback = {
        LoadTable: function () {
            window.location.reload();
        }
    };
    layer.open({
        type: 2,
        title: 'Upload Files',
        area: ['850px', '600px'],
        content: "/_layouts/15/UploadFiles.aspx?Folder=" + ctx.rootFolder
    });
}

// 获取面包屑
function GetDocumentMbx(fUrl) {
    var mbxDiv = "<div class='top_div'>";
    var list = fUrl.split('/');
    if (list.length > 2) {
        var fPUrl = "/" + docListName;
        for (var i = 2; i < list.length ; i++) {
            if (list[i] != null && list[i] != "") {
                fPUrl += "/" + list[i];
                mbxDiv += "<a href=\"" + GetDocReturnUrl(fPUrl) + "\" class='top_a' >" + list[i] + "</a>";
                if (i != list.length - 1) {
                    mbxDiv += "&nbsp;/&nbsp; ";
                }
            }
        }
    }
    mbxDiv += "</div>";
    return mbxDiv;
}

// 获取返回链接
function GetDocReturnUrl(url) {
    return "/" + docListName + "/Forms/AllItems.aspx?RootFolder=" + encodeURIComponent(url);
}

// 添加TD
function addTD() {
    // 设置文档库样式
    $("#scriptWPQ1").css("float", "left");
    $("#scriptWPQ1").css("width", "960px");
    document.getElementById("scriptWPQ1").vAlign = "top";

    var folderUrl = decodeURIComponent(ctx.rootFolder);
    var listFolderUrl = "/Documents/Forms/AllItems.aspx";
    if (folderUrl == null || folderUrl == "" || folderUrl == "/" + docListName + "/" || folderUrl == "/" + docListName) {
        folderUrl = docListName;
    } else {
        folderUrl = folderUrl.replace("/" + docListName + "/", "").split('/')[0];
        listFolderUrl += "?RootFolder=/" + encodeURIComponent(docListName + "/" + folderUrl);
    }

    // 添加树td
    var td = '<td id="treeTD" valign="top"><div id="folderTree"><div id="left_title"><a href="' + listFolderUrl + '" title="' + folderUrl + '">' + folderUrl + '</a></div>' +
             '<img id="tree_loading" style="display:none" src="/_layouts/15/images/treel.gif" width="220px"><ul id="treeObject" class="ztree data-ztree"></ul></div></td>';
    $("#scriptWPQ1").before(td);

    // 添加树
    addFolderTree();

    var fUrl = decodeURIComponent(ctx.rootFolder);
    var td3Btn = "<td valign='top' id='threeButtonTD'><div class='actionBox'><div class='leftBtn'>";

    // 订阅
    td3Btn += "<div onclick='Subscribe()' style='cursor:pointer'><img src='/_layouts/15/Images/subscribe.png' title='Subscribe folder' alt='Subscribe folder'></div>";

    // 获取文件夹权限
    isHadPermission();

    if (isExport || isAdd) {
        // 分享
        var region = GetRegion();
        if (region != "") {
            td3Btn += "<div onclick=\"ShareFolder('" + region + "')\" style='cursor:pointer'><img src='/_layouts/15/Images/share2.png' title='Share folder' alt='Share folder'></div>";
        }
    }

    if (isExport) {
        td3Btn += "<div onclick='ExportToExcel()' style='margin-right:10px;cursor:pointer'><img src='/_layouts/15/Images/export.png' title='Exporting the report' alt='Export'></div>";
    }
    if (isAdd) {
        if (fUrl.startsWith("/" + docListName + "/Common") || fUrl.startsWith("/" + docListName + "/common")) {
            td3Btn += "<div onclick='AddDocument()' style='margin-right:10px;cursor:pointer'><img src='/_layouts/15/Images/upload1.png' title='Upload file' alt='Upload file'></div>";
        }
        else {
            td3Btn += "<div onclick='CreatFolder()' style='margin-right:10px;cursor:pointer'><img src='/_layouts/15/Images/add.png' title='Add a folder' alt='Add a folder'></div>" +
                             "<div onclick='AddDocument()' style='margin-right:10px;cursor:pointer'><img src='/_layouts/15/Images/upload1.png' title='Upload file' alt='Upload file'></div>";
        }
    } else {
        // 加载样式
        loadStyles("/_layouts/15/content/NoAddPermission.css");
    }
    td3Btn += "<div onclick='MultipleDownLoad()' style='cursor:pointer'><img src='/_layouts/15/Images/down1.png' title='Download file' alt='Download file'></div>";
    td3Btn += "</div></div></td>";
    $("#scriptWPQ1").before(td3Btn);
}

// 是否有权限
function isHadPermission() {
    $.ajax({
        type: "post",
        url: "/_layouts/15/FolderTree.aspx/IsFolderHadPermisson",
        data: "{'rootFolder': '" + ctx.rootFolder + "'}",
        async: false,
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            var content = response.d.split(',');
            if (content[0] == "OK") {
                isAdd = true;
            }
            if (content[1] == "OK") {
                isExport = true;
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
}

// 获取区域
function GetRegion() {
    var region = "";
    var folderUrl = decodeURIComponent(ctx.rootFolder);
    if (folderUrl != null && folderUrl != "") {

        folderUrl = folderUrl.toLocaleUpperCase();

        var regionP = ("/" + docListName + "/Region/").toLocaleUpperCase();

        if (folderUrl.startsWith(regionP)) {
            var folderR = folderUrl.replace(regionP, "");
            if (folderR != "") {
                region = folderR.split('/')[0];
            }
        }
    }
    return escape(region);
}

// 分享文件夹
function ShareFolder(region) {
    var num = ctx.CurrentSelectedItems;
    var c = ctx.dictSel;

    if (num > 0) {
        var ids = "";
        for (var key in c) {
            //var type = c[key].fsObjType;
            //if (type == "1") {
            var id = c[key].id;
            ids = ids + id + ',';
            //}
        }
        if (ids != "") {
            ids = ids.substring(0, ids.length - 1);
            layer.open.constructor.prototype.callback = {
                LoadTable: function () {
                    //window.location.reload();
                    alert("Share successful !");
                }
            };
            layer.open({
                type: 2,
                title: 'Access Control',
                area: ['850px', '560px'],
                content: '/_layouts/15/SelectExternalUser.aspx?Ids=' + ids + '&Region=' + region
            });
        } else {
            alert("Please select the folder to share !");
        }
    } else {
        alert("Please select the folder to share!");
    }
}

// 创建文件夹
function CreatFolder() {
    if (!SP.Ribbon.PageManager.get_instance().executeRootCommand("NewFolder", null, null, null)) {
        alert("You don't have permission to add , Please set permission and try again!");
    }
}

// 创建文件
function AddDocument() {
    if (!SP.Ribbon.PageManager.get_instance().executeRootCommand("UploadDocument", null, null, null)) {
        alert("You don't have permission to upload , Please set permission and try again!");
    }
}

// 导出Excel
function ExportToExcel() {
    SP.Ribbon.PageManager.get_instance().executeRootCommand("ExportToSpreadsheet", null, null, null);
}

// 加载树
function addFolderTree() {
    // 加载zTree样式
    loadStyles("/_layouts/15/Plugins/zTree/css/zTreeStyle/zTreeStyle.css");
    // 获取文件夹树
    GetFolderTree();
}

//树设置
var treeSetting = {
    async: {
        enable: true,
        url: getUrl
    },
    check: {
        enable: false
    },
    view: {
        fontCss: setFontCss
    },
    data: {
        simpleData: {
            enable: true
        },
        key: {
            title: "title",
            number: "number"
        },
        view: {
            showTitle: true
        }
    },
    callback: {
        onClick: onClick,
        beforeExpand: beforeExpand,
        onAsyncSuccess: onAsyncSuccess,
        onAsyncError: onAsyncSuccess
    }
};

//获取文件夹树
function GetFolderTree() {
    //var rUrl = decodeURIComponent(ctx.rootFolder);
    $.ajax({
        type: "post",
        url: "/_layouts/15/FolderTree.aspx/GetFolderTree",
        data: "{'rootFolder': '" + ctx.rootFolder + "'}",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            var content = new Function('return ' + response.d)();
            //var zNodesObject = HandleTreeData(content);
            var zNodesObject = content;

            //$("#tree_loading").css("display", "none");
            //$("#treeObject").css("display", "block");

            $.fn.zTree.init($("#treeObject"), treeSetting, zNodesObject);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
}

//树点击事件
function onClick(e, treeId, treeNode) {
    var url = "/" + docListName + "/Forms/AllItems.aspx?RootFolder=" + encodeURIComponent(treeNode.furl);
    window.location.href = url;
}

//处理要加载到树里的数据
function HandleTreeData(content) {
    var zNodes = [];
    for (var m = 0; m < content.length; m++) {
        var name = content[m].name;
        var id = content[m].id;
        var pId = content[m].pId;
        var furl = content[m].furl;

        var rUrl = decodeURIComponent(ctx.rootFolder);
        var open = false;
        if (rUrl.startsWith(furl)) {
            open = true;
        }
        zNodes.push({ id: id, pId: pId, name: name, title: name, open: open, furl: furl });
    }
    return zNodes;
}

// 设置当前点击节点树格式
function setFontCss(treeId, treeNode) {
    var rUrl = decodeURIComponent(ctx.rootFolder);
    return treeNode.furl == rUrl ? { 'font-weight': "700", 'color': "#0E60AC" } : {};
};

// 获取数据
function getUrl(treeId, treeNode) {
    var url = "/_layouts/15/GetTree.ashx?Id=" + treeNode.id;
    return url;
}

// 加载前
function beforeExpand(treeId, treeNode) {
    if (treeNode.zAsync) {
        return true;
    } else if (!treeNode.isAjaxing) {
        ajaxGetNodes(treeNode, "refresh");
        return true;
    } else {
        return false;
    }
}

// 加载按钮
function ajaxGetNodes(treeNode, reloadType) {
    var zTree = $.fn.zTree.getZTreeObj("treeObject");
    if (reloadType == "refresh") {
        treeNode.icon = "/_layouts/15/Plugins/zTree/css/zTreeStyle/img/loading.gif";
        zTree.updateNode(treeNode);
    }
    zTree.reAsyncChildNodes(treeNode, reloadType, true);
}

// 加载结束
function onAsyncSuccess(event, treeId, treeNode) {
    var zTree = $.fn.zTree.getZTreeObj("treeObject");
    treeNode.icon = "";
    zTree.updateNode(treeNode);
}

// 订阅文件夹
function Subscribe() {
    var num = ctx.CurrentSelectedItems;
    var c = ctx.dictSel;

    if (num > 0) {
        var ids = "";
        for (var key in c) {
            var type = c[key].fsObjType; // 0：文件，1：文件夹
            if (type == "1") {
                var id = c[key].id;
                ids = ids + id + ',';
            }
        }
        if (ids != "") {
            $.ajax({
                type: "post",
                url: "/_layouts/15/GetNav.aspx/SetSubscribeFolder",
                data: "{'folderIds': '" + ids + "'}",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                success: function (response, textStatus) {
                    alert("Subscribe successfully !");
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus);
                }
            });
        } else {
            alert("Please select the folder to subscribe !");
        }

    } else {
        alert("Please select the folder to subscribe !");
    }
}