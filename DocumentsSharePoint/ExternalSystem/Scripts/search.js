
$(function () {

    $("#UserName").html("Welcome , " + userName);

    //$("#mySearchBox").css("display", "none");

    LoadTable();

    var searchKey = getQueryString("SearchKey");
    if (searchKey != "") {
        $("#inputSearch").val(searchKey);

        SearchResults(searchKey);
    }

    //输入框的enter事件
    $('#inputSearch').bind('keydown', function (event) {
        if (event.keyCode == "13") {
            SearchWeb();
        }
    });

});

// 点击搜索
function SearchWeb() {
    var searchKey = $("#inputSearch").val();
    if (searchKey == "" || searchKey.trim() == "") {
        return;
    } else {
        searchKey = encodeURIComponent(searchKey);
        SearchResults(searchKey);
    }
}

//获取参数
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) {
        return decodeURI(r[2]);
    }
    return null;
}

// 搜索结果
function SearchResults(searchKey) {

    var index = layer.load(0, {
        shade: [0.1, '#f2f2f2'] //透明度，背景颜色
    });

    $.ajax({
        type: "post",
        url: "Search.aspx/GetSearchResult",
        data: "{'account': '" + userAccount + "','region': '" + region + "','country': '" + country + "','searchKey': '" + searchKey + "'}",
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
function LoadTable() {
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
        }],
        pagination: true,
        //search: true,
        pageSize: 15,
        pageList: [15, 30, 50, 100, 200, 500, 1000, 5000],
        onPageChange: function () {
            $(".detail").popover({ placement: 'bottom' });
        }
    });
}