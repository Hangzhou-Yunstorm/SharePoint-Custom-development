window.onload = function () {
    var title = document.title;
    var titles = title.split('.');
    if (titles.length > 1) {
        var ntitle = titles[titles.length - 1];
        if (ntitle != null && ntitle != "") {
            ntitle = ntitle.toLowerCase();
            document.getElementById("_fileName").innerHTML = title;
            if (ntitle.indexOf('docx') > -1 || ntitle.indexOf('xlsx') > -1 || ntitle.indexOf('pptx') > -1) {
                document.getElementById("_IsFullControl").style.display = "";
                document.getElementById("_IsRead").style.display = "";
                document.getElementById("_IsPrint").style.display = "";
                document.getElementById("_IsSave").style.display = "";
                document.getElementById("_IsEdit").style.display = "";
                document.getElementById("_Users").style.display = "";
            }
        }
    }
}