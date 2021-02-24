
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

var CurrentUsers = [];

$(function () {
    //搜索图标点击事件
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
    GetCurrentUsers();
})

function GetUsers() {
    var index = layer.load(0, {
        shade: [0.1, '#f2f2f2'] //透明度，背景颜色
    });
    $.ajax({
        type: "post",
        url: "AddUsers.aspx/GetUsers",
        data: "",
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

function GetCurrentUsers() {
    var index = layer.load(0, {
        shade: [0.1, '#f2f2f2'] //透明度，背景颜色
    });
    $.ajax({
        type: "post",
        url: "EditUsers.aspx/GetCurrentUsers",
        data: "{ 'groupId':'" + groupId + "'}",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            var content = response.d;
            CurrentUsers = new Function('return ' + content)();
            GetUsers();
        },
        complete: function () {
            layer.close(index);
        }
    });
}

//处理要加载到树里的数据
function HandleTreeData(content) {
    var zNodes = [];
    for (var m = 0; m < content.length; m++) {
        var name = content[m].Name;
        var id = m;
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
            var del = "<img src=\"images/del.png\" onclick=\"deleteObj('" + escape(account) + "')\" title=\"Delete " + name + "\"  />";
            var htm = "<tr id=\"td_" + account + "\" title=\"" + name + "\"><td id=\"tdd_" + account + "\">" + name + "</td><td>" + del + "</td></tr>";
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
    account = unescape(account);
    for (var m = 0; m < CurrentUsers.length; m++) {
        var tmpId = CurrentUsers[m].account;
        if (account == tmpId) {
            CurrentUsers.splice(m, 1);
            break;
        }
    }
    $("#td_" + account).remove();
    var zTree = $.fn.zTree.getZTreeObj("treeUser");
    if (zTree != null && zTree != "") {
        var nodes = zTree.getNodesByParam("account", account, null);
        for (var i = 0, l = nodes.length; i < l; i++) {
            zTree.checkNode(nodes[i], false, false);
        }
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
                if (!CheckData(name, nodes[m])) {
                    var data = { name: name, account: account };
                    CurrentUsers.push(data);
                    var del = "<img src=\"images/del.png\" onclick=\"deleteObj('" + escape(account) + "')\" title=\"Delete" + name + "\"  />";
                    var htm = "<tr id=\"td_" + account + "\"title=\"" + name + "\"><td id=\"tdd_" + account + "\">" + name + "</td><td>" + del + "</td></tr>";
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

    var userNames = "";
    if (CurrentUsers.length > 0) {
        for (var n = 0; n < CurrentUsers.length; n++) {
            userNames += CurrentUsers[n].account + ",";
        }
        if (userNames != "") {
            userNames = userNames.substring(0, userNames.length - 1);
        }
    }
    var index = layer.load(0, {
        shade: [0.1, '#f2f2f2'] //透明度，背景颜色
    });
    userNames = escape(userNames);
    $.ajax({
        type: "post",
        url: "EditUsers.aspx/AddUsers",
        data: "{'userNames':'" + userNames + "'}",
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
