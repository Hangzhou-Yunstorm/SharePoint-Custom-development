
$(function () {
    // 加载zTree
    addFolderTree();

    $(".second li a").css("color", "#4e4e4e");
    $("#MyRequest a").css("font-weight", "700");
    $("#MyRequest a").css("color", "#0E60AC");
    try {
        ReadJson();
    } catch (e) { }

});