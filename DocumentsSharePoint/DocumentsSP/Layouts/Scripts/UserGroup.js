$(function () {
    if (userType != administratorUser && userType != subUser) {
        window.location.href = "/default.aspx";
        return;
    }
    if (userType == administratorUser) {
        $("#PermissionSettings").css("display", "block");
        $("#li_logs").css("display", "block");
        $("#li_setting").css("display", "block");

        $("#whitelist_a").attr("href", "/Lists/WhiteList/AdminAllItems.aspx");
    }

    $(".second li a").css("color", "#4e4e4e");
    $("#UserGroups a").css("font-weight", "700");
    $("#UserGroups a").css("color", "#0E60AC");

    GetGroups();
});

function loadTable(gData) {
    $('#group_table').bootstrapTable({
        columns: [{
            field: 'state',
            checkbox: true
        }, {
            field: 'icon',
            title: '',
            align: 'left',
            width: '53'
        }, {
            field: 'name',
            title: 'Name',
            align: 'left'
        }, {
            field: 'members',
            title: 'Members',
            align: 'left'
        }, {
            field: 'operation',
            title: 'Operation',
            align: 'left'
        }, {
            field: 'id',
            title: 'ID',
            visible: false
        }, {
            field: 'hideName',
            title: 'Name',
            visible: false
        }],
        data: gData,
        pagination: true,
        pageSize: 15,
        pageList: [15, 30, 50, 100, 200, 500, 1000]
    });
}

function GetGroups() {
    var index = layer.load(0, {
        shade: [0.1, '#f2f2f2'] //透明度，背景颜色
    });
    $.ajax({
        type: "post",
        url: "UserGroups.aspx/GetGroups",
        data: "",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            var content = new Function('return ' + response.d)();
            loadTable(content);
            layer.close(index);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
            layer.close(index);
        }
    });
}


function deleteGroup() {
    var selects = $('#group_table').bootstrapTable('getSelections');
    var ids = $.map(selects, function (row) {
        return row.id;
    });

    if (ids == null || ids == "") {
        alert('Please select the user groups you want to delete!');
        return;
    } else {
        var r = confirm("Make sure you want to delete the selected user groups?")
        if (r) {
            var index = layer.load(0, {
                shade: [0.1, '#f2f2f2'] //透明度，背景颜色
            });
            $.ajax({
                type: "post",
                url: "UserGroups.aspx/DeleteGroup",
                data: "{'ids': '" + ids + "'}",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                success: function (response, textStatus) {
                    if (response.d == "No") {
                        alert("Only site administrator has permission to operate !");
                    } else {
                        window.location.reload();
                    }
                    layer.close(index);
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus);
                    layer.close(index);
                }
            });
        }
    }
}


function addGroup() {
    layer.open.constructor.prototype.callback = {
        LoadTable: function () {
            window.location.reload();
        }
    };
    layer.open({
        type: 2,
        title: 'Add User Group',
        area: ['850px', '560px'],
        content: 'AddUsers.aspx'
    });
}

function changeName(name) {
    var reg = new RegExp(",", "g");
    var oldName = name.replace(reg, " ");

    var newName = prompt("Please enter a new user group name:", oldName);
    if (newName != null && newName != oldName) {
        $.ajax({
            type: "post",
            url: "UserGroups.aspx/RenameGroup",
            data: "{'oldName': '" + oldName + "','newName': '" + newName + "'}",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (response, textStatus) {
                if (response.d == "No") {
                    alert("Only site administrator has permission to operate !");
                } else {
                    window.location.reload();
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus);
            }
        });
    }
}
