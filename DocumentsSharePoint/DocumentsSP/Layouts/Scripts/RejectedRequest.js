
$(function () {
    // 加载zTree
    addFolderTree();

    $(".second li a").css("color", "#4e4e4e");
    $("#RejectedRequest a").css("font-weight", "700");
    $("#RejectedRequest a").css("color", "#0E60AC");
    try {
        ReadJson();
    } catch (e) { }
});