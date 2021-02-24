$(function () {
    LoadData();
});

function loadTable(data) {
    $('#file_table').bootstrapTable({
        columns: [{
            field: 'state',
            checkbox: true
        }, {
            field: 'ParentFolder',
            title: 'ParentFolder',
            align: 'left'
        }, {
            field: 'Icon',
            title: '',
            width: 35,
            align: 'center',
        }, {
            field: 'Name',
            title: 'Name',
            align: 'left',
            //sortable: true
        }, {
            field: 'AveScore',
            title: 'AveScore',
            align: 'left',
            sortable: true
        }, {
            field: 'ClickCount',
            title: 'ClickCount',
            align: 'left',
            sortable: true
        }, {
            field: 'DownCount',
            title: 'DownCount',
            align: 'left',
            sortable: true
        }, {
            field: 'FileSize',
            title: 'File Size',
            align: 'left',
            //sortable: true
        }, {
            field: 'Created',
            title: 'CreateTime',
            align: 'left',
            //sortable: true
        }, {
            field: 'CreatedBy',
            title: 'Creator',
            align: 'left',
            //sortable: true
        }, {
            field: 'Department',
            title: 'Department',
            align: 'left',
            //sortable: true
        }, {
            field: 'ID',
            title: 'ID',
            visible: false
        }],
        data: data,
        pagination: false,
        pageSize: 15,
        pageList: [15, 30, 50, 100, 200, 500, 1000]
    });
}

// 加载数据
function LoadData() {
    var index = layer.load(0, {
        shade: [0.1, '#f2f2f2'] //透明度，背景颜色
    });
    $.ajax({
        type: "post",
        url: "/_layouts/15/RegionFiles.aspx/GetRegionFilesJson",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            var content = new Function('return ' + response.d)();
            loadTable(content);
            //$('#file_table').bootstrapTable("load", content);
        },
        complete: function () {
            layer.close(index);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
}

function RegionDownLoad() {
    var selects = $('#file_table').bootstrapTable('getSelections');
    var ids = $.map(selects, function (row) {
        return row.ID;
    });

    if (ids == null || ids.length == 0) {
        alert("Please select the files to download!");
        return;
    } else if (ids.length == 1) {
        var url = '/_layouts/15/FileDownload.aspx?ItemID=' + ids[0];
        window.open(url);
    } else {
        var idlist = ids.join(",") + ",";
        DownloadAll(idlist);
    }
}
