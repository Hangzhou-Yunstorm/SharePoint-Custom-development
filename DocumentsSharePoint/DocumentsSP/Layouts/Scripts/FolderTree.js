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

$(function () {
    GetFolderTree();
});

// 选择的文件夹
var sfolder = "";

//树点击事件
function onClick(e, treeId, treeNode) {
    var index = layer.load(0, {
        shade: [0.1, '#f2f2f2'] //透明度，背景颜色
    });

    if (isHadUploadPermission(treeNode.furl)) {
        sfolder = encodeURIComponent(treeNode.furl);
    } else {
        alert("You have no upload permissions on the folder !");
    }

    layer.close(index);
}

// 是否有上传权限
function isHadUploadPermission(url) {
    url = encodeURIComponent(url);
    var isAdd = false;
    $.ajax({
        type: "post",
        url: "FolderTree.aspx/IsFolderHadPermisson",
        data: '{"rootFolder": "' + url + '"}',
        async: false,
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            var content = response.d.split(',');
            if (content[0] == "OK") {
                isAdd = true;
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
    return isAdd;
}

// 获取数据
function getUrl(treeId, treeNode) {
    var url = "GetTree.ashx?Id=" + treeNode.id;
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
        treeNode.icon = "Plugins/zTree/css/zTreeStyle/img/loading.gif";
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

//获取文件夹树
function GetFolderTree() {
    $.ajax({
        type: "post",
        url: "FolderTree.aspx/GetFolderTree",
        data: "{'rootFolder': ''}",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            var content = new Function('return ' + response.d)();
            var zNodesObject = content;
            $.fn.zTree.init($("#treeObject"), treeSetting, zNodesObject);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
}

function Confirm() {
    if (sfolder == "") {
        alert("Please select an folder to confirm !");
        return;
    }
    var index = parent.layer.getFrameIndex(window.name);
    parent.layer.open.callback.LoadTable(sfolder);
    parent.layer.close(index);
}

function Cancel() {
    var index = parent.layer.getFrameIndex(window.name);
    parent.layer.close(index);
}
