$(function () {
    getUsers();
});

function loadTable(gData) {
    $('#group_table').bootstrapTable({
        columns: [{
            field: 'state',
            checkbox: true
        }, {
            field: 'id',
            title: 'Account',
            align: 'left'
            //visible: false
        }, {
            field: 'name',
            title: 'Name',
            align: 'left'
        }, {
            field: 'email',
            title: 'Email',
            align: 'left'
        }],
        data: gData,
        pagination: true,
        search: true,
        pageSize: 15,
        pageList: [15, 30, 50, 100, 200, 500, 1000]
    });
}

function getUsers() {
    var index = layer.load(0, {
        shade: [0.1, '#f2f2f2'] //透明度，背景颜色
    });
    $.ajax({
        type: "post",
        url: "GroupMembers.aspx/GetUsers",
        data: "{'groupId': '" + groupId + "'}",
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


function addUsers() {
    layer.open.constructor.prototype.callback = {
        LoadTable: function () {
            window.location.reload();
        }
    };
    layer.open({
        type: 2,
        title: 'Edit Users',
        area: ['850px', '560px'],
        content: 'EditUsers.aspx?ID=' + groupId
    });
}

function deleteUsers() {
    var selects = $('#group_table').bootstrapTable('getSelections');
    var names = $.map(selects, function (row) {
        return row.id;
    });

    if (names == null || names == "") {
        alert('Please select the users you want to delete!');
        return;
    } else {
        var r = confirm("Make sure you want to delete the selected users from group ?")
        if (r) {
            var index = layer.load(0, {
                shade: [0.1, '#f2f2f2'] //透明度，背景颜色
            });
            $.ajax({
                type: "post",
                url: "GroupMembers.aspx/DeleteUsers",
                data: "{'names': '" + names + "'}",
                contentType: "application/json;charset=utf-8",
                dataType: "json",
                success: function (response, textStatus) {
                    window.location.reload();
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


