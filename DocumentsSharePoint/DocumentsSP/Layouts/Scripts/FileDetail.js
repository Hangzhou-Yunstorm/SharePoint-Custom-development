
var fileMScore = "";
// 预览信息
var viewMsg = "";

$(function () {

    if (isPC) {
        $("#mobile_div").css("display", "none");
        $("#pc_div").css("display", "block");

        // 我的评分
        GetMyScore();
        // 下载次数/点击次数/综合评分
        Get3Count();

        $("#alreadScore").html(" Your comments ");
        // 若已评分，设置已评分信息
        if (fileMScore != "-1") {
            $("#ScoreNum").val(fileMScore);
            $("#ScoreNum").attr("readonly", true);
            $("#alreadScore").html("*You have been graded!");
        }

        // 获取评论
        GetComments();
        // 调用打分插件
        SetStar();
        //获取预览
        GetView();
    } else {
        $("#mobile_div").css("display", "block");
        $("#pc_div").css("display", "none");

        $("#mobile_div input").css("display", "none");

        var fileExt = fileName.substring(fileName.lastIndexOf('.')).toLowerCase();
        if (isPicture(fileExt)) {
            viewMsg = "view";
            $("#mobile_div input").css("display", "block");
        }
        else if (fileExt != ".xlsx" && fileExt != ".doc" && fileExt != ".docx" && fileExt != ".ppt" && fileExt != ".pptx") {
            viewMsg = "The file format is not supported.";
            $("#mobile_div input").css("display", "block");
        } else {
            //获取预览
            GetView();
        }
    }

    // 浏览历史
    AddHistory();
});

function isPicture(extension) {
    if (extension == ".png" || extension == ".jpg" || extension == ".gif" || extension == ".bmp" || extension == ".jpeg" || extension == ".tiff" || extension == ".img") {
        return true;
    } else {
        return false;
    }
}

function isVideo(extension) {
    if (extension == ".mp4" || extension == ".ogg" || extension == ".mov" || extension == ".webm") {
        return true
    } else {
        return false;
    }
}

function DownloadFile() {
    var index = layer.load(0, {
        shade: [0.1, '#f2f2f2'] //透明度，背景颜色
    });
    //var url = getQueryString("Url").replace(/'/g, ":");
    $.ajax({
        type: "post",
        url: "/_layouts/15/MblDownload.aspx/GetDownload",
        data: "{'uId': '" + fileUID + "'}",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            if (response.d == "") {
                alert("The file is being encrypted. Please download it later !");
            } else {
                //window.location.href = response.d + "?Mobile=0";
                window.location.href = response.d;
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

function Preview() {
    if (viewMsg == "view") {
        var url = decodeURIComponent(getQueryString("Url"));
        window.location.href = "/_layouts/15/MoFileView.aspx?Url=" + url + "&Mobile=0";
    }
    else if (viewMsg == "") {
        var url = decodeURIComponent(getQueryString("Url"));
        window.location.href = url + "?Mobile=1";
    } else {
        alert(viewMsg);
    }
}

// 复制到剪切板
function Copy() {
    var curl = document.getElementById("copyurl");
    curl.select(); // 选择对象
    document.execCommand("Copy"); // 执行浏览器复制命令
    alert("The url is in your clipboard !");
}

// 评论
function GetComments() {
    $.ajax({
        type: "post",
        url: "FileDetail.aspx/GetComments",
        data: "{ 'fId': '" + fileFID + "'}",
        async: false,
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            var content = response.d;
            var list = new Function('return ' + content)();
            var comments = "";
            if (list.length <= 10) {
                for (var n = 0; n < list.length; n++) {
                    comments += "<div class='user_comment'><div><h6 class='oReviewer'>" + list[n].UserName + "</h6>" +
                        "<div class='oScore'><input value='" + list[n].Score + "' type='number' readonly=\"true\" style='display: none;' class=\"rating\" min=\"0\" max='5' step='0.5' data-size='sm' /></div></div>" +
                        "<div class='oCommentTxt'>" + list[n].CommentText + "</div><div class='oCommentTime'>" + list[n].Time + "</div></div>";
                }
            } else {
                for (var n = 0; n < 10; n++) {
                    comments += "<div class='user_comment'><div><h6 class='oReviewer'>" + list[n].UserName + "</h6>" +
                        "<div class='oScore'><input value='" + list[n].Score + "' type='number' readonly=\"true\" style='display: none;' class=\"rating\" min=\"0\" max='5' step='0.5' data-size='sm' /></div></div>" +
                        "<div class='oCommentTxt'>" + list[n].CommentText + "</div><div class='oCommentTime'>" + list[n].Time + "</div></div>";
                }
                var pageCount = Math.ceil(list.length / 10);
                if (pageCount > 0) {
                    $(".tcdPageCode").css("display", "block");
                    $(".tcdPageCode").createPage({
                        pageCount: pageCount,
                        current: 1,
                        backFn: function (p) {
                            var m = (p - 1) * 10;
                            var commentsPage = "";
                            for (var n = m; n < m + 10; n++) {
                                if (n < list.length) {
                                    commentsPage += "<div class='user_comment'><div><h6 class='oReviewer'>" + list[n].UserName + "</h6>" +
                                        "<div class='oScore'><input value='" + list[n].Score + "' type='number' readonly=\"true\" style='display: none;' class=\"rating\" min=\"0\" max='5' step='0.5' data-size='sm' /></div></div>" +
                                        "<div class='oCommentTxt'>" + list[n].CommentText + "</div><div class='oCommentTime'>" + list[n].Time + "</div></div>";
                                } else {
                                    break;
                                }
                            }
                            $("#Comments").html(commentsPage);
                            SetStar();
                        }
                    });
                }
            }
            $("#Comments").html(comments);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
}

// 我的评分
function GetMyScore() {
    $.ajax({
        type: "post",
        url: "FileDetail.aspx/GetMyScore",
        data: "{ 'fId': '" + fileFID + "'}",
        async: false,
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            fileMScore = response.d;
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
}

// 下载次数/点击次数/平均分
function Get3Count() {
    $.ajax({
        type: "post",
        url: "FileDetail.aspx/Get3Count",
        data: "{ 'fId': '" + fileFID + "'}",
        async: false,
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            var value = response.d;
            if (value != null || value != "[]") {
                var model = new Function('return ' + value)();
                $("#downloadCount").html(model.DownloadCount + " Downloads");
                $("#clickCount").html(model.ClickCount + " Views");
                $("#AVScore").val(model.AveScore);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
}

// 浏览历史
function AddHistory() {
    $.ajax({
        type: "post",
        url: "FileDetail.aspx/AddHistory",
        data: "{ 'uId': '" + fileUID + "'}",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            console.log(textStatus);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
}

// 预览
function GetView() {
    $.ajax({
        type: "post",
        url: "FileDetail.aspx/GetView",
        data: "{'uId': '" + fileUID + "','fileName': '" + fileName + "','fileUrl': '" + fileUrl + "'}",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            var src = response.d;
            if (src == "0") {
                src = "<h3>You don't have permission to preview.</h3>";
                viewMsg = "You don't have permission to preview.";
            }
            else if (src == "1") {
                src = "<h3>The file format is not supported.</h3>";
            }
            else if (src == "xls") {
                var url = "/_layouts/15/WopiFrame.aspx?sourcedoc={" + fileUID + "}&action=default&wdSmallView=1";
                src = "<h3>The file format is not supported.<br />" +
                      "<a href = '" + url + "' target='_blank'>Please click here to preview.</a></h3>";
            }

            if (src == "2big") {
                src = "<h3>Sorry, we can't open your workbook in Excel Online because it exceeds the 50 MB file size limit.</h3>";
                viewMsg = "Sorry, we can't open your workbook in Excel Online because it exceeds the 50 MB file size limit.";
            }

            $(".leftContentDetail").html(src);

            $("#mobile_div input").css("display", "block");
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
}

// 添加评分
function CreateListItem() {
    var myComm = $("#commentText").val().trim();
    if (myComm == null || myComm == "") {
        alert("Please enter a comment !");
        return;
    }
    if (fileMScore != "-1") {
        $(".submitComment").attr("disabled", "disabled");
        addComment(myComm);
    } else {
        var score = $("#ScoreNum").val().trim();
        if (score == "0" || score == 0) {
            alert("Please rate !");
            return;
        }
        $(".submitComment").attr("disabled", "disabled");
        $.ajax({
            type: "post",
            url: "FileDetail.aspx/AddScore",
            contentType: "application/json;charset=utf-8",
            data: "{ 'score': '" + score + "','fId': '" + fileFID + "'}",
            dataType: "json",
            success: function (response, textStatus) {
                addComment(myComm);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus);
                $(".submitComment").removeAttr("disabled");
            }
        });
    }
}

// 添加评论
function addComment(comment) {
    $.ajax({
        type: "post",
        url: "FileDetail.aspx/AddComment",
        contentType: "application/json;charset=utf-8",
        data: "{ 'comment': '" + comment + "','fId': '" + fileFID + "','fileName': '" + fileName + "'}",
        dataType: "json",
        success: function (response, textStatus) {
            $("#commentText").val("");
            parent.location.reload();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
            $(".submitComment").removeAttr("disabled");
        }
    });
}

// 发送邮件
function SendEmail() {
    var content = $("#commentText").val();
    if (content != null && content != "") {
        if (fileAuthorEmail == null || fileAuthorEmail == "") {
            alert("The author doesn't have an email !");
            return;
        }
        else {
            $(".submitComment").attr("disabled", "disabled");
            $.ajax({
                type: "post",
                url: "FileDetail.aspx/SendEmail",
                contentType: "application/json;charset=utf-8",
                data: "{ 'content': '" + content + "','fId': '" + fileFID + "','fileName': '" + fileName + "','fileAuthorEmail': '" + fileAuthorEmail + "'}",
                dataType: "json",
                success: function (response, textStatus) {
                    if (response.d == null || response.d == "") {
                        alert("Email has been sent !");
                        $("#commentText").val("");
                        parent.location.reload();
                    } else {
                        alert(response.d);
                    }
                    $(".submitComment").removeAttr("disabled");
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus);
                    $(".submitComment").removeAttr("disabled");
                }
            });
        }
    }
    else {
        alert("Please enter email content !");
    }
}