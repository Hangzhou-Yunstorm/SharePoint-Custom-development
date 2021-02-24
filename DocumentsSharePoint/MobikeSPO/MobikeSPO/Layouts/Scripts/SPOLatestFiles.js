
$(function () {
    LoadData();
});

function loadTable(data) {
    $('#file_table').bootstrapTable({
        columns: [{
            field: 'Img',
            title: '',
            width: 35,
            align: 'center',
        }, {
            field: 'Title',
            title: '标题',
            align: 'left',
        }, {
            field: 'Date_Time',
            title: '已修改',
            align: 'left',
        }, {
            field: 'Name',
            title: '修改者',
            align: 'left',
        }],
        data: data,
        pagination: false,
        pageSize: 30,
        pageList: [30, 50, 100, 200, 500, 1000]
    });
}

// 加载数据
function LoadData() {
    $.ajax({
        type: "post",
        url: "/_layouts/15/SPOLatestFiles.aspx/GetLatestFiles",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            var content = new Function('return ' + response.d)();
            loadTable(content);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
}


