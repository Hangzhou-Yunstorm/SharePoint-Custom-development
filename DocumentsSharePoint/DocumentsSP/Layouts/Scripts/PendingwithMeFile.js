
$(function () {

    // 加载样式
    loadStyles("/_layouts/15/content/PendingwithMe.css");
    loadStyles("/_layouts/15/content/SetTitle.css");
    loadStyles("/_layouts/15/content/SetMax.css");

    $(".contentwrapper").css("margin-top", "0");

    var hotsName = getQueryString("HostName");
    if (hotsName == null || hotsName == "") {
        hotsName = getHostName();
    }

    var title_td = "<td id='td_title'><div style='margin-bottom: 35px;font-size:18px;'>" +
        "<p>Please be aware that you are current on GKS " + hotsName + " site, click on the Dahua logo to return to your home site.</p>" +
        "<p style='margin-top: 15px; font-size: 16px;'><span style='color:red;'>* </span> If you need to preview files, please download them to your computer.</p>" +
        "</div></td>";
    $("#scriptWPQ1").before(title_td);

    var btn_td = "<td id='td_title'><div style='border-bottom:1px dashed #ccc; height:30px; padding-bottom: 10px;'>" +
        "<div onclick='ModerateRibbonA()' style='cursor:pointer;float:left;'><img src='/_layouts/15/Images/permission/ar.png' alt='Moderate files'></div>" +
        "<div onclick='MultipleDownLoadFile()' style='cursor:pointer;float:right;'><img src='/_layouts/15/Images/down1.png' alt='Download files'></div>" +
        "</div></td>";
    $("#scriptWPQ1").before(btn_td);

});

// 获取国家
function getHostName() {
    var hostName = "";
    $.ajax({
        type: "post",
        url: "/_layouts/15/GetNav.aspx/GetFooter",
        data: "",
        async: false,
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            var footer = response.d;
            var start = footer.indexOf("(") + 1;
            var end = footer.indexOf(")")
            hostName = footer.substring(start, end);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
    return hostName;
}

// 批量下载
function MultipleDownLoadFile() {
    var num = ctx.CurrentSelectedItems;
    var c = ctx.dictSel;

    if (num == 0) {
        alert("Please select the files to download!");
        return;
    } else {
        if (window.confirm('Download maybe slow due to the different location. Do you wish to proceed ?')) {
            if (num == 1) {
                var id = "";
                for (var key in c) {
                    id = c[key].id;
                };
                if (id != "") {
                    var url = ctx.HttpRoot + '/_layouts/15/FileDownload.aspx?ItemID=' + id;
                    window.open(url);
                }
            }
            else if (num > 0) {
                var ids = "";
                for (var key in c) {
                    var id = c[key].id;
                    ids = ids + id + ',';
                }
                if (ids != "") {
                    DownloadAll(ids);
                }
            }
        }
    }

}

// 文件审批
function ModerateRibbonA() {
    var num = ctx.CurrentSelectedItems;

    if (num > 0) {
        if (window.confirm('After successful approve, document will be shared globally soon. Do you wish to approve ?')) {
            SP.Ribbon.PageManager.get_instance().executeRootCommand("Moderate", null, null, null);
        }
    }
    else {
        alert("Please select the files to approval!");
    }
}