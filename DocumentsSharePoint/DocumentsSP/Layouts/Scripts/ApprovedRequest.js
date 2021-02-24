
$(function () {
    // 加载zTree
    addFolderTree();

    $(".second li a").css("color", "#4e4e4e");
    $("#ApprovedRequest a").css("color", "#0E60AC");
    $("#ApprovedRequest a").css("font-weight", "700");
    try {
        ReadJson();
    } catch (e) { }

});