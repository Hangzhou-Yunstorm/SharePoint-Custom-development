
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
//分享的文件Id集合
var folderIds = "";
//分享区域
var region = "";

$(function () {
    //搜索图标点击事件
    $(".ulSearch img").click(function () {
        Search();
    });

    folderIds = getQueryString("Ids");
    region = getQueryString("Region");

    //var expirat = {
    //    elem: '#expiration',
    //    format: 'YYYY/MM/DD hh:mm:ss',
    //    min: laydate.now(), //设定最小日期为当前日期
    //    istime: true,
    //    istoday: false
    //};
    //laydate(expirat);

    //执行一个laydate实例
    laydate.render({
        elem: '#expiration', //指定元素
        lang: 'en',
        //min: getNowFormatDate(),
        //value: getNowFormatDate(),
        trigger: 'click',
        //btns: ['now', 'confirm'],
        type: 'datetime'
    });

    //input回车事件
    $('.ulSearch input').bind('keypress', function (event) {
        if (event.keyCode == "13") {
            Search();
            return false;
        }
    });

    var index = layer.load(0, {
        shade: [0.1, '#f2f2f2'] //透明度，背景颜色
    });

    $.ajax({
        type: "post",
        url: "SelectExternalUser.aspx/GetEmployee",
        data: "{ 'region':'" + region + "'}",
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

})

// 获取参数
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) {
        return decodeURI(r[2]);
    }
    return null;
}

//处理要加载到树里的数据
function HandleTreeData(content) {
    var open = false;
    var zNodes = [];
    for (var m = 0; m < content.length; m++) {
        var id = m;
        var name = content[m].Name;
        var account = content[m].Account;
        zNodes.push({ id: id, name: name, title: account, account: account });
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

    if (key != null && key != "") {
        //var index = layer.load(0, { shade: false });
        var index = layer.load(0, {
            shade: [0.1, '#f2f2f2'] //透明度，背景颜色
        });
        $.ajax({
            type: "post",
            url: "SelectExternalUser.aspx/GetEmployeeByInput",
            data: "{ 'search':'" + key + "','region':'" + region + "'}",
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

//全选/全不选
function SelectAllUsers(value) {
    var index = layer.load(0, {
        shade: [0.1, '#f2f2f2'] //透明度，背景颜色
    });
    try {
        var zTree = $.fn.zTree.getZTreeObj("treeUser");
        if (value == "0") {
            zTree.checkAllNodes(true);
            var nodes = zTree.getCheckedNodes(true);
            for (var m = 0; m < nodes.length; m++) {
                var name = nodes[m].name;
                var account = nodes[m].account;
                if (!CheckData(account, nodes[m])) {
                    var data = { name: name, account: account };
                    CurrentUsers.push(data);
                    var del = "<img src='images/del.png' onclick='deleteObj(\"" + account + "\")' title='Delete" + name + "'  />";
                    var htm = "<tr id='td_" + account + "' title='" + name + "'><td id='tdd_" + account + "'>" + name + "</td><td>" + del + "</td></tr>";
                    $("#SelContent").append(htm);
                }
            }
        }
        else {
            zTree.checkAllNodes(false);
            var nodes = zTree.getCheckedNodes(false);
            for (var m = 0; m < nodes.length; m++) {
                var account = nodes[m].account;
                for (var n = CurrentUsers.length - 1; n >= 0; n--) {
                    var tmpId = CurrentUsers[n].account;
                    if (account == tmpId) {
                        CurrentUsers.splice(n, 1);
                        $("#td_" + account).remove();
                        break;
                    }
                }
            }
        }
    } catch (e) { }
    layer.close(index);
}

//确定
function SubmitData() {
    var userAccounts = "";
    if (CurrentUsers.length > 0) {
        var index = layer.load(0, {
            shade: [0.1, '#f2f2f2'] //透明度，背景颜色
        });

        for (var n = 0; n < CurrentUsers.length; n++) {
            userAccounts += CurrentUsers[n].account + ",";
        }
        if (userAccounts != "") {
            userAccounts = userAccounts.substring(0, userAccounts.length - 1);
        }

        var readOrWrite = "0";

        if ($('#upload_ckb').is(':checked')) {
            readOrWrite = "1";
        }

        var expiration = $("#expiration").val();

        $.ajax({
            type: "post",
            url: "SelectExternalUser.aspx/AddShareCatalog",
            data: "{ 'expiration':'" + expiration + "','userAccounts':'" + userAccounts + "','ids':'" + folderIds + "', 'readOrWrite':'" + readOrWrite + "'}",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (response, textStatus) {
                var msg = response.d;
                if (msg == null || msg == "") {
                    var index2 = parent.layer.getFrameIndex(window.name);
                    parent.layer.open.callback.LoadTable();
                    parent.layer.close(index2);
                } else {
                    alert(msg);
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
    else {
        alert("Please select the person you want to share !");
    }
}
