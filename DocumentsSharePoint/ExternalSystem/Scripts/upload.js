
$(function () {
    var path = getQueryString("path");
    path = unescape(path);
    $("#PathLabel").html(path);
});

//获取参数
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) {
        return r[2];
    }
    return null;
}

function CloseWindow() {
    var index = parent.layer.getFrameIndex(window.name);
    //parent.layer.open.callback.LoadTable();
    parent.layer.close(index);
}

function Confirm() {
    var index = parent.layer.getFrameIndex(window.name);
    parent.layer.open.callback.LoadTable();
    parent.layer.close(index);
}

function uploadfile() {
    var dom = document.getElementById("FileUpload");
    if (dom.files == null || dom.files.length == 0) {
        alert('Please select file !');
        return
    } else {
        var fileSize = dom.files[0].size;//文件的大小，单位为字节B
        if (fileSize > 1024 * 1024 * 1024) {
            alert('Maximum file cannot exceed 1G !');
            return;
        } else {
            var sub = document.getElementById("UploadBtn");
            sub.click();

            var index = layer.load(0, {
                shade: [0.1, '#f2f2f2'] //透明度，背景颜色
            });
            $(".confirm input").attr("disabled", true);
            $(".confirm input").css("background-color", "#eee");
            $(".confirm input").css("color", "#969696");
            $(".confirm input").css("cursor", "not-allowed");
        }
    }
}