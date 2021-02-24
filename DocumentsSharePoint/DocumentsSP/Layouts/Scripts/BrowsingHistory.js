$(function () {
    $(".contentwrapper").css("margin-top", "0"); // 轮播图顶部

    var td = "<td id='td_title'><div class='splitters' style='border-bottom:1px dashed #ccc; height:20px; padding-bottom: 10px;'>" +
        "<h2 style='float:left;height:20px;line-height:20px;'>History</h2>" +
        "<img  onclick='BHDownLoad()' style='float:right;cursor:pointer;' src='/_layouts/15/Images/down1.png' alt='Download file'></div></td>";

    $("#scriptWPQ1").before(td);

    loadStyles("/_layouts/15/content/SetTitle.css");

    loadStyles("/_layouts/15/content/SetMax.css");
});


// 下载
function BHDownLoad() {
    var num = ctx.CurrentSelectedItems;
    var c = ctx.dictSel;

    if (num == 1) {
        var id = "";
        for (var key in c) {
            id = c[key].id;
        }
        var url = ctx.HttpRoot + '/_layouts/15/BHDownload.aspx?ItemID=' + id;
        window.open(url);
    }
    else if (num > 0) {
        var ids = '';
        for (var key in c) {
            var id = c[key].id;
            ids = ids + id + ',';
        }
        BHDownloadAll(ids);
    }
    else {
        alert("Please select the files to download!");
    }
}


//批量下载
function BHDownloadAll(ids) {
    //var index = layer.load(0, { shade: false });
    var index = layer.load(0, {
        shade: [0.1, '#f2f2f2'] //透明度，背景颜色
    });
    $.ajax({
        type: "post",
        url: "/_layouts/15/BHMulDownload.aspx/BHDownLoadZIP",
        contentType: "application/json;charset=utf-8",
        data: "{'listId': '" + ids + "'}",
        dataType: "json",
        success: function (response, textStatus) {
            var value = response.d;
            if (value == null || value == "[]") {
                alert("Download error,please try again.");
            }
            else {
                var model = new Function('return ' + value)();
                if (model != null && model != "") {
                    if (model.msg == "0") {
                        window.location.href = model.url;
                    }
                    else if (model.msg == "1") {
                        alert("Please note that: files(under RMS encrypting) ,deleted files, are unavailable for download !");
                        window.location.href = model.url;
                    }
                    else if (model.msg == "2") {
                        alert("All items that you selected， do not include available files (files under RMS encrypting, deleted files), unavailable for download !");
                    }
                    else if (model.msg == "3") {
                        alert("Please select file to download !");
                    }
                    else {
                        alert("Download error,please try again.");
                    }
                }
                else {
                    alert("Download error,please try again.");
                }
            }
            layer.close(index);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert(textStatus);
            layer.close(index);
        },
    });
}