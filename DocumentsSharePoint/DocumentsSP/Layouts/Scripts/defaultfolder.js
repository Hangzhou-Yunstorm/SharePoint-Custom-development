
$(function () {

    $("select[title^='内容类型']").change(function () {
        var type = $("select[title^='内容类型']").find("option:selected").text();
    });

    // 勾选
    $("input[id^='IsDefault']").attr("checked", true);
    // 隐藏
    $("input[id^='IsDefault']").css("display", "none");

    // 隐藏标签
    var spans = $("span[class='ms-h3 ms-standardheader']");
    for (var m = 0; m < spans.length; m++) {
        var text = spans[m].innerText;
        if (text == "IsDefault") {
            $("span[class='ms-h3 ms-standardheader']")[m].style.display = "none";
            break;
        }
    }
});