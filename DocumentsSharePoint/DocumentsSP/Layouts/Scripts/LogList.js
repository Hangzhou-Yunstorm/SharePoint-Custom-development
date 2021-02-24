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
    $("#FileLogs a").css("font-weight", "700");
    $("#FileLogs a").css("color", "#0E60AC");

    //执行一个laydate实例
    laydate.render({
        elem: '#search_date', //指定元素
        lang: 'en',
        //min: getNowFormatDate(),
        value: getNowFormatDate(),
        trigger: 'click',
        btns: ['now', 'confirm'],
        type: 'datetime'
    });

    $('#log_table').bootstrapTable({
        columns: [{
            field: 'Title',
            title: 'Title',
            align: 'left',
            sortable: true
        }, {
            field: 'ObjectName',
            title: 'ObjectName',
            align: 'left',
            sortable: true
        }, {
            field: 'Operate',
            title: 'Operate',
            align: 'left',
            sortable: true
        }, {
            //    field: 'ObjectType',
            //    title: 'ObjectType',
            //    align: 'left',
            //    sortable: true
            //}, {
            field: 'Operator',
            title: 'Operator',
            align: 'left',
            sortable: true
        }, {
            field: 'Department',
            title: 'Department',
            align: 'left',
            sortable: true
        }, {
            field: 'ServerIP',
            title: 'ServerIP',
            align: 'left',
            sortable: true
        }, {
            field: 'Created',
            title: 'Created',
            align: 'left',
            sortable: true
        }],
        //data: gData,
        pagination: true,
        pageSize: 15,
        pageList: [15, 30, 50, 100, 200, 500, 1000]
    });

    SearchLog();
});

// 维护日志
function Maintain() {
    layer.open.constructor.prototype.callback = {
        LoadTable: function () {
            SearchLog();
        }
    };
    layer.open({
        type: 2,
        title: 'Change Department',
        area: ['600px', '345px'],
        content: 'ChangeDepartment.aspx'
    });
}

// 搜索日志
function SearchLog() {
    var key = $("#search_key").val();
    var date = $("#search_date").val();

    var index = layer.load(0, {
        shade: [0.1, '#f2f2f2'] //透明度，背景颜色
    });

    $.ajax({
        type: "post",
        url: "/_layouts/15/LogList.aspx/GetLogList",
        contentType: "application/json;charset=utf-8",
        data: "{ 'key': '" + key + "', 'date': '" + date + "'}",
        dataType: "json",
        success: function (response, textStatus) {
            var content = new Function('return ' + response.d)();
            $('#log_table').bootstrapTable("load", content);
        },
        complete: function () {
            layer.close(index);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });

}

//yyyy-MM-dd HH:mm:ss
function getNowFormatDate() {
    var date = new Date();
    var seperator1 = "-";
    var seperator2 = ":";
    var month = date.getMonth() + 1;
    var strDate = date.getDate();
    if (month >= 1 && month <= 9) {
        month = "0" + month;
    }
    if (strDate >= 0 && strDate <= 9) {
        strDate = "0" + strDate;
    }
    var currentdate = date.getFullYear() + seperator1 + month + seperator1 + strDate
            + " " + date.getHours() + seperator2 + date.getMinutes()
            + seperator2 + date.getSeconds();
    return currentdate;
}
