
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
    $("#ReviewsStatistics a").css("font-weight", "700");
    $("#ReviewsStatistics a").css("color", "#0E60AC");
    // 柱状图
    loadHcharts();


});
function loadHcharts() {
    Highcharts.setOptions({
        lang: {
            downloadPDF: 'Export PDF',
            downloadPNG: 'Export PNG',
            downloadXLS: 'Export Excel',
            noData: "No ratings yet"
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
            filename: 'Score file'
        },
        title: {
            text: 'Overall rating'
        },
        xAxis: {
            lineColor: '#000',
            lineWidth: 1,
            categories: names,
            labels: {
                rotation: -30,
                style: {
                    color: '#4e4e4e',//颜色
                    fontSize: '14px'  //字体
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
            minTickInterval: 1,
            title: {
                enabled: false
            }
        },
        series: [
            {
                name: 'Overall rating',
                data: scores
            }
        ]
    });
}