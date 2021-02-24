
$(function () {
    if (reportType == "") {
        window.location.href = "/default.aspx";
        return;
    }

    if (userType != "") {
        $("#PendingwithMe").css("display", "block");

        if (userType == administratorUser || userType == subUser) {
            $("#ExternalUser").css("display", "block");
            if (userType == administratorUser) {
                $("#ExternalUser_a").attr("href", "/Lists/ExternalUserList/AdminView.aspx");
            }
        }
    }

    // 样式
    $(".second li a").css("color", "#4e4e4e");
    $("#ReviewsStatistics a").css("font-weight", "700");
    $("#ReviewsStatistics a").css("color", "#0E60AC");

    GetHSDatas();

    $('.jumpMenu').dropkick({
        change: function (value, label) {
            window.location.href = value;
        }
    });

});

// 加载数据
function GetHSDatas() {
    var index = layer.load(0, {
        shade: [0.1, '#f2f2f2'] //透明度，背景颜色
    });
    $.ajax({
        type: "post",
        url: "/_layouts/15/DocumentsScoreReport.aspx/GetDatas",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            var value = response.d;
            if (value != null && value != "") {
                var model = new Function('return ' + value)();
                loadHcharts(model.Names, model.Values);
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

// 加载图表
function loadHcharts(names, values) {
    Highcharts.setOptions({
        lang: {
            downloadPDF: 'Export PDF',
            downloadPNG: 'Export PNG',
            downloadXLS: 'Export Excel',
            noData: "No data yet"
        }
    });

    var chart = new Highcharts.Chart({
        chart: {
            renderTo: 'myChart',
            type: 'column'
        },
        credits: {
            enabled: false
        },
        navigation: {
            buttonOptions: {
                enabled: true // 是否显示导出按钮
            }
        },
        exporting: {
            filename: 'Report file' // 导出文件名
        },
        title: {
            text: 'Documents Score Top 20' // 标题
        },
        xAxis: {
            lineColor: '#000',
            lineWidth: 1,
            categories: names,
            labels: {
                rotation: -75,
                style: {
                    color: '#4e4e4e',//颜色
                    fontSize: '13px'  //字体
                },
                formatter: function () {
                    if (this.value.length > 30) {
                        return this.value.substring(0, 30) + "...";
                    } else {
                        return this.value;
                    }
                }
            }
        },
        yAxis: {
            lineColor: '#000',
            gridLineColor: '#fff',
            lineWidth: 1,
            min: 0,
            max: 5,
            startOnTick: false,
            //minTickInterval: 1,
            title: {
                enabled: false
            }
        },
        series: [
            {
                name: 'Score',  // 柱状图属性名
                data: values,
                dataLabels: {
                    enabled: true,
                    align: 'center',
                    style: {
                        fontSize: '12px'
                    }
                }
            }
        ]
    });
}