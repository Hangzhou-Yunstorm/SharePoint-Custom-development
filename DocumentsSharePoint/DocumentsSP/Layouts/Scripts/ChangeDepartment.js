
$(function () {
    $('.group_type').dropkick();
});

// 修改数据
function ChangeDepart() {
    if (window.confirm('Are you sure to change it ?')) {

        var oldName = $("#old_department").val();
        var departName = $(".group_type").find("option:selected").text();
        var departId = $(".group_type").find("option:selected").val();

        var index = layer.load(0, {
            shade: [0.1, '#f2f2f2'] //透明度，背景颜色
        });

        $.ajax({
            type: "post",
            url: "/_layouts/15/ChangeDepartment.aspx/ChangeDepart",
            contentType: "application/json;charset=utf-8",
            data: "{'oldName': '" + oldName + "','departName': '" + departName + "','departId': '" + departId + "'}",
            dataType: "json",
            success: function (response, textStatus) {
                var value = response.d;
                if (value != null && value != "") {
                    alert(value);
                } else {
                    var index2 = parent.layer.getFrameIndex(window.name);
                    parent.layer.open.callback.LoadTable();
                    parent.layer.close(index2);
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
}

function Cancel() {
    var index2 = parent.layer.getFrameIndex(window.name);
    parent.layer.open.callback.LoadTable();
    parent.layer.close(index2);
}
