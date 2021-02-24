
$(function () {
    try {
        $(".contentwrapper").css("margin-top", "0"); // 轮播图顶部
        ReadJson();
    } catch (e) { }

    var td = "<td id='td_title'><div class='splitters' style='border-bottom:1px dashed #ccc; height:20px; padding-bottom: 10px;' >" +
        "<h2 style='float:left;height:20px;line-height:20px;'>Most Downloads</h2>" +
        "<img  onclick='MultipleDownLoad()' style='float:right;cursor:pointer;' src='/_layouts/15/Images/down1.png' alt='Download file'></div></td>";
    $("#scriptWPQ1").before(td);

    loadStyles("/_layouts/15/content/SetTitle.css");

    loadStyles("/_layouts/15/content/common.css");

    loadStyles("/_layouts/15/content/SetMax.css");

    loadStyles("/_layouts/15/content/ECB.css");

});