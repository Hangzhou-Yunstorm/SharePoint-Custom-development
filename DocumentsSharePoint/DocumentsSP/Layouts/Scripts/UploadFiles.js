var treeSettingUser = {
    check: {
        enable: true
    },
    data: {
        simpleData: {
            enable: true
        },
        key: {
            title: "title"
        },
        view: {
            showTitle: true
        }
    },
    callback: {
        onCheck: onCheck
    }
};

//当前选中的用户
var CurrentUsers = [];

$(function () {
    var folder = getQueryString("Folder");
    $("#folder_path").val(decodeURIComponent(folder));
    $("#FileFolder").val(folder);

    $(".ulSearch img").click(function () {
        Search();
    });

    //input回车事件
    $('.ulSearch input').bind('keypress', function (event) {
        if (event.keyCode == "13") {
            Search();
            return false;
        }
    });

});

//获取参数
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) {
        return decodeURI(r[2]);
    }
    return null;
}

function Next() {

    if (document.getElementById("file_input").files.length == 0) {
        alert("Plsease choose file to upload !");
    } else if ($("#FileFolder").val() == "") {
        alert("Upload in the root directory is not allowed !");
    } else {

        var isRms = $("#file_rms_ckb").is(':checked');
        if (isRms) {
            $("#file_rms").css("display", "block");
            $("#file_upload").css("display", "none");
        } else {
            Upload();
        }
    }
}

function Return() {
    $("#file_rms").css("display", "none");
    $("#file_upload").css("display", "block");
}

function Tips() {
    var isRms = $("#file_rms_ckb").is(':checked');
    if (isRms) {
        $("#Tips").css("display", "inline-block");
    }
}

function Upload() {
    if (document.getElementById("file_input").files.length == 0) {
        alert("Plsease choose file to upload !");
    } else if ($("#FileFolder").val() == "") {
        alert("Upload in the root directory is not allowed !");
    } else {
        var index = layer.load(0, {
            shade: [0.1, '#f2f2f2'] //透明度，背景颜色
        });

        var rmsUsers = "";
        if (CurrentUsers.length > 0) {
            for (var n = 0; n < CurrentUsers.length; n++) {
                rmsUsers += CurrentUsers[n].account + ",";
            }
            if (rmsUsers != "") {
                rmsUsers = rmsUsers.substring(0, rmsUsers.length - 1);
            }
        }

        $("#RMSUsers").val(rmsUsers);
        $("#_CheckinComment").val($("#version_area").val());
        $("#FileDescription").val($("#description_area").val());

        var data = new FormData($("#upload_form")[0]);
        $.ajax({
            type: "post",
            url: 'UploadFileList.ashx',
            data: data,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response != "") {
                    alert(response);
                } else {
                    alert("Upload Success !");

                    var index2 = parent.layer.getFrameIndex(window.name);
                    parent.layer.open.callback.LoadTable();
                    parent.layer.close(index2);
                }
            },
            complete: function () {
                layer.close(index);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus);
            }
        });
    }
}

function SelectFolder() {
    layer.open.constructor.prototype.callback = {
        LoadTable: function (folder) {
            $("#folder_path").val(decodeURIComponent(folder));
            $("#FileFolder").val(folder);
        }
    };
    layer.open({
        type: 2,
        title: 'Select Folder',
        area: ['550px', '550px'],
        content: 'FolderTree.aspx'
    });
}

function Cancel() {
    if (confirm("Are you sure to cancel the upload ?")) {
        var index = parent.layer.getFrameIndex(window.name);
        parent.layer.close(index);
    }
}


//处理要加载到树里的数据
function HandleTreeData(content) {
    var zNodes = [];
    for (var m = 0; m < content.length; m++) {
        var id = m;
        var name = content[m].Name;
        var account = content[m].Account;

        zNodes.push({ id: id, name: name + "(" + account + ")", title: account, account: account });
    }
    return zNodes;
}

//树勾选/取消勾选事件（用户）
function onCheck(e, treeId, treeNode) {
    var name = treeNode.name;
    var account = treeNode.account;
    if (treeNode.checked) {
        if (!CheckData(name, treeNode)) {
            var data = { name: name, account: account };
            CurrentUsers.push(data);
            var del = "<img src='images/del.png' onclick='deleteObj(\"" + account + "\")' title='Delete" + name + "'  />";
            var htm = "<tr id='td_" + account + "' title='" + name + "'><td id='tdd_" + account + "'>" + name + "</td><td>" + del + "</td></tr>";
            $("#SelContent").append(htm);
        }
    }
    else {
        for (var m = 0; m < CurrentUsers.length; m++) {
            var d_name = CurrentUsers[m].account;
            if (d_name == account) {
                $("#td_" + account).remove();
                CurrentUsers.splice(m, 1);
                break;
            }
        }
    }
}

//搜索
function Search() {
    var key = $(".ulSearch input").val();

    var index = layer.load(0, {
        shade: [0.1, '#f2f2f2'] //透明度，背景颜色
    });
    $.ajax({
        type: "post",
        url: "AddUsers.aspx/GetUsersBySearch",
        data: "{ 'search':'" + key + "'}",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            var content = response.d;
            content = new Function('return ' + content)();
            var zNodesUser = HandleTreeData(content);
            $.fn.zTree.init($("#treeUser"), treeSettingUser, zNodesUser);
            examineCheck();
        },
        complete: function () {
            layer.close(index);
        }
    });

}

//检测以选中的对象是否重复（返回true：重复、false：不重复）
function CheckData(account, treeNode) {
    for (var m = 0; m < CurrentUsers.length; m++) {
        var tmpId = CurrentUsers[m].account;
        if (tmpId == account) {
            return true;
        }
    }
    return false;
}

//检查用户是否需要勾选上
function examineCheck() {
    var zTree = $.fn.zTree.getZTreeObj("treeUser");
    for (var m = 0; m < CurrentUsers.length; m++) {
        var account = CurrentUsers[m].account;
        var nodes = zTree.getNodesByParam("account", account, null);
        for (var i = 0, l = nodes.length; i < l; i++) {
            zTree.checkNode(nodes[i], true, true);
        }
    }
}

//删除
function deleteObj(account) {
    for (var m = 0; m < CurrentUsers.length; m++) {
        var tmpId = CurrentUsers[m].account;
        if (account == tmpId) {
            CurrentUsers.splice(m, 1);
            break;
        }
    }
    $("#td_" + account).remove();
    var zTree = $.fn.zTree.getZTreeObj("treeUser");
    var nodes = zTree.getNodesByParam("account", account, null);
    for (var i = 0, l = nodes.length; i < l; i++) {
        zTree.checkNode(nodes[i], false, false);
    }
}
