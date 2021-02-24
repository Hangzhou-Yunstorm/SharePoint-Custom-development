var CurrentFolder = {};

//树设置
var treeSetting = {
    async: {
        enable: true,
        url: getUrl
    },
    check: {
        enable: false
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

// 获取数据
function getUrl(treeId, treeNode) {
    try {
        var url = "../GetTree.ashx?fId=" + treeNode.fId + "&pId=" + treeNode.id + "&sharer=" + treeNode.sharer + "&shareTime=" + treeNode.shareTime + "&canWrite=" + treeNode.canWrite + "&expiration=" + treeNode.expiration;
        return url;
    } catch (ex) { }
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
        treeNode.icon = "../Plugins/zTree/css/zTreeStyle/img/loading.gif";
        zTree.updateNode(treeNode);
    }
    zTree.reAsyncChildNodes(treeNode, reloadType, true);
}

// 加载结束
function onAsyncSuccess(event, treeId, treeNode) {
    try {
        var zTree = $.fn.zTree.getZTreeObj("treeObject");
        treeNode.icon = "";
        zTree.updateNode(treeNode);
    } catch (ex) { }
}

//树点击事件
function onClick(e, treeId, treeNode) {
    var index = layer.load(0, {
        shade: [0.1, '#f2f2f2'] //透明度，背景颜色
    });
    var pId = treeNode.pId;
    if (pId == null) {
        pId = "0";
    }
    var canWrite = treeNode.canWrite;
    CurrentFolder = { folderId: treeNode.fId, share: treeNode.sharer, shareTime: treeNode.shareTime, expiration: treeNode.expiration, path: treeNode.path, canWrite: canWrite, pId: pId };

    // 显示上传按钮
    if (canWrite) {
        $("#upload_img").css("display", "inline-block");
    } else {
        $("#upload_img").css("display", "none")
    }

    $.ajax({
        type: "post",
        url: "Index.aspx/OpenFolder",
        data: "{'folderId': '" + treeNode.fId + "','sharer': '" + treeNode.sharer + "','shareTime': '" + treeNode.shareTime + "','expiration': '" + treeNode.expiration + "','canWrite': '" + canWrite + "','pId': '" + treeNode.id + "'}",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            var content = response.d;
            var tableData = new Function('return ' + content)();

            $("#file_table").bootstrapTable('load', tableData);

            $(".detail").popover({ placement: 'bottom' });

            layer.close(index);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
            layer.close(index);
        }
    });
}

//处理要加载到树里的数据
function HandleTreeData(content) {
    var zNodes = [];
    for (var m = 0; m < content.length; m++) {
        var name = content[m].name;
        var id = content[m].id;
        var fId = content[m].fId;
        var pId = content[m].pId;
        var sharer = content[m].sharer;
        var shareTime = content[m].shareTime;
        var expiration = content[m].expiration;
        var path = content[m].path;
        var canWrite = content[m].canWrite;
        var isParent = content[m].isParent;

        zNodes.push({ id: id, fId: fId, pId: pId, name: name, title: name, sharer: sharer, shareTime: shareTime, expiration: expiration, path: path, canWrite: canWrite, isParent: isParent });
    }
    return zNodes;
}

// 刷新页面
function Reload() {
    window.location.reload();
}

$(function () {

    $("#UserName").html("Welcome , " + userName);

    //输入框的enter事件
    $('#inputSearch').bind('keydown', function (event) {
        if (event.keyCode == "13") {
            SearchWeb();
        }
    });

    //var zNodesObject = HandleTreeData(tJson);
    $("#tree_loading").css("display", "none");
    $("#treeObject").css("display", "block");
    if (tJson.length > 0) {
        $.fn.zTree.init($("#treeObject"), treeSetting, tJson);
    }

    LoadTable(fDatas);

    $(".detail").popover({ placement: 'bottom' });
});

// 搜索
function SearchWeb() {
    var searchKey = $("#inputSearch").val();
    if (searchKey == "" || searchKey.trim() == "") {
        return;
    } else {
        searchKey = encodeURIComponent(searchKey);

        var url = "/Search.aspx" + window.location.search + "&searchkey=" + searchKey;
        window.open(url);
    }
}

// 打开文件夹
function OpenFolder(folderId) {
    var index = layer.load(0, {
        shade: [0.1, '#f2f2f2'] //透明度，背景颜色
    });

    var pageDatas = $('#file_table').bootstrapTable('getData');
    var curRow = $.map(pageDatas, function (row) {
        if (row.ID == folderId)
            return row;
    });

    var sharer = curRow[0].Sharer;
    var shareTime = curRow[0].ShareTime;
    var name = curRow[0].HideName;
    var expiration = curRow[0].Expiration;
    var path = curRow[0].Path;
    var canWrite = curRow[0].CanWrite;
    var pId = curRow[0].PId;
    var uId = folderId + pId;

    var ztrees = $.fn.zTree.getZTreeObj("treeObject");
    var node = ztrees.getNodeByParam("id", uId);
    ztrees.expandNode(node, true, false);//指定选中ID节点展开 
    ztrees.cancelSelectedNode();
    ztrees.selectNode(node, true);//指定选中ID的节点  

    // 显示上传按钮
    if (canWrite) {
        $("#upload_img").css("display", "inline-block");
    } else {
        $("#upload_img").css("display", "none")
    }

    CurrentFolder = { folderId: folderId, share: sharer, shareTime: shareTime, expiration: expiration, path: path, canWrite: canWrite, pId: pId };

    $.ajax({
        type: "post",
        url: "Index.aspx/OpenFolder",
        data: "{'folderId': '" + folderId + "','sharer': '" + sharer + "','shareTime': '" + shareTime + "','expiration': '" + expiration + "','canWrite': '" + canWrite + "','pId': '" + uId + "'}",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            var content = response.d;
            var tableData = new Function('return ' + content)();

            $("#file_table").bootstrapTable('load', tableData);

            $(".detail").popover({ placement: 'bottom' });

            layer.close(index);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
            layer.close(index);
        }
    });
}

// 打开文件夹
function OpenFolderByMbx(folderId, sharer, shareTime, expiration, canWrite, pId) {
    var index = layer.load(0, {
        shade: [0.1, '#f2f2f2'] //透明度，背景颜色
    });

    // 显示上传按钮
    if (canWrite) {
        $("#upload_img").css("display", "inline-block");
    } else {
        $("#upload_img").css("display", "none")
    }

    $.ajax({
        type: "post",
        url: "Index.aspx/OpenFolder",
        data: "{'folderId': '" + folderId + "','sharer': '" + sharer + "','shareTime': '" + shareTime + "','expiration': '" + expiration + "','canWrite': '" + canWrite + "','pId': '" + pId + "'}",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            var content = response.d;
            var tableData = new Function('return ' + content)();

            $("#file_table").bootstrapTable('load', tableData);

            $(".detail").popover({ placement: 'bottom' });

            layer.close(index);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
            layer.close(index);
        }
    });
}

// 下载文件
function DownloadFile() {
    var selects = $('#file_table').bootstrapTable('getSelections');
    var ids = $.map(selects, function (row) {
        if (row.Type == 0) {
            return row.ID;
        }
    });
    if (ids == null || ids.length == 0) {
        alert("Please select the files to download!");
        return;
    } else if (ids.length == 1) {
        var url = '/FileDown.aspx?Account=' + userAccount + '&ItemID=' + ids[0];
        window.open(url);
    } else {
        DownloadAll(ids);
    }
}

//批量下载
function DownloadAll(ids) {

    var index = layer.load(0, {
        shade: [0.1, '#f2f2f2'] //透明度，背景颜色
    });

    $.ajax({
        type: "post",
        url: "Index.aspx/DownloadFiles",
        contentType: "application/json;charset=utf-8",
        data: "{'listIds': '" + ids + "'}",
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
                        alert("Please note that: files(under RMS encrypting)  are unavailable for download !");
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
            //alert(textStatus);
            layer.close(index);
        },
    });
}

// Load table
function LoadTable(tableData) {
    $('#file_table').bootstrapTable({
        columns: [{
            field: 'state',
            checkbox: true
        }, {
            field: 'IconUrl',
            title: '',
            align: 'center',
            width: '33'
        }, {
            field: 'Name',
            title: 'Name',
            align: 'left',
            width: '300'
            //}, {
            //    field: 'Created',
            //    title: 'Created',
            //    align: 'left',
            //    sortable: true
            //}, {
            //    field: 'Creator',
            //    title: 'Creator',
            //    align: 'left'
            //}, {
            //    field: 'ShareTime',
            //    title: 'Shared',
            //    align: 'left',
            //    sortable: true,
            //    width: '100'
        }, {
            field: 'Sharer',
            title: 'Shared By',
            align: 'left'
        },
        {
            field: 'Expiration',
            title: 'Expiration',
            align: 'left',
            sortable: true,
            width: '100'
        }, {
            field: 'Size',
            title: 'Size',
            align: 'left',
            sortable: true
        }, {
            field: 'ID',
            title: 'ID',
            visible: false
        },
        {
            field: 'Type',
            title: 'Type',
            visible: false
        }, {
            field: 'HideName',
            title: 'HideName',
            visible: false
        }, {
            field: 'Path',
            title: 'Path',
            visible: false
        }, {
            field: 'CanWrite',
            title: 'CanWrite',
            visible: false
        }, {
            field: 'PId',
            title: 'PId',
            visible: false
        }],
        data: tableData,
        pagination: true,
        search: true,
        pageSize: 15,
        pageList: [15, 30, 50, 100, 200, 500, 1000, 5000],
        onPageChange: function () {
            $(".detail").popover({ placement: 'bottom' });
        }
    });

}

function UploadFile() {
    layer.open.constructor.prototype.callback = {
        LoadTable: function (folderId) {
            OpenFolderByMbx(CurrentFolder.folderId, CurrentFolder.share, CurrentFolder.shareTime, CurrentFolder.expiration, CurrentFolder.canWrite, CurrentFolder.pId);
        }
    };
    //CurrentFolder = { folderId: folderId, share: sharer, shareTime: shareTime, expiration: expiration, path: path, canWrite:canWrite };
    layer.open({
        type: 2,
        title: 'UploadFile',
        closeBtn: 0,
        area: ['500px', '250px'],
        content: 'Upload.aspx?Account=' + userAccount + '&path=' + escape(CurrentFolder.path) + '&folderId=' + CurrentFolder.folderId
    });
}