$(function () {
    if (userType != administratorUser && userType != subUser) {
        window.location.href = "/default.aspx";
        return;
    }
    try {
        addFolderTree();
    } catch (e) { }

    $(".second li a").css("color", "#4e4e4e");
    $("#FileLogs a").css("color", "#0E60AC");
    $("#FileLogs a").css("font-weight", "700");

});