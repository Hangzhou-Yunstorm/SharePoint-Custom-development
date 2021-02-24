
$(function () {
    // 加载zTree
    addFolderTree();

    $(".second li a").css("color", "#4e4e4e");
    $("#MyUpload a").css("font-weight", "700");
    $("#MyUpload a").css("color", "#0E60AC");
    try {
        ReadJson();
    } catch (e) { }

});