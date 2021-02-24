//主函数
$(function () {
    $("#sp-span-count").html(comContent);
})

//发布
function Release(Obj) {
    var DivFocusZone;
    var CommentText = $("#Te__" + Obj).html();
    if (CommentText.split("<div>").join("").split("<br>").join("").split("</div>").join("").trim() == "") {
        layer.msg("内容不可以为空！", { tiem: 500, icon: 0 });
        return false;
    }
    var DivText = '';
    $.ajax({
        type: "post",
        url: "/_layouts/15/NoticeContent.aspx/Release",
        data: "{'text':'" + CommentText + "','pId': '" + Obj + "','pageId': '" + pageId + "','TypeID': '" + TypeID + "'}",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            if (response.id != 0) {
                if (Obj == 0) {
                    DivText += '<div class="spComment_8d833d90" data-automation-id="sp-comment-block-root" id="Posted__' + response.d + '">'
                    DivText += '<div class="spContentRegion_8d833d90 ms-scaleUpIn100">'
                    DivText += '<div class="spAvatar_8d833d90" aria-hidden="true" role="presentation">'
                    DivText += '<canvas class="spAvatarImage_8d833d90" data- src="/_layouts/15/userphoto.aspx?size=S&amp;accountname=test2017@mobike.com" data- status="success" data- fingerprint="8a2bceaca7e84b8f4da535d772625c6f" width="32" height="32" > test2017 的照片</canvas >'
                    DivText += '</div >'
                    DivText += '<div class="spContent_8d833d90">'
                    DivText += '<span class="spAuthor_8d833d90" >' + userName + '</span >'
                    DivText += '<div class="spMetadata_8d833d90">'
                    DivText += '<a href="#comment=17" class="ms-Link root_c02e569e isEnabled_c02e569e" > ' + getCurrentTime() + '</a >'
                    DivText += '</div > '
                    DivText += '<div class="spText_8d833d90">'
                    DivText += '<span><span>' + CommentText + '</span></span>'
                    DivText += '</div>'
                    DivText += '<div class="spActions_8d833d90 ms-fadeIn500">'
                    DivText += '<div onclick="Publish(' + response.d + ')">'
                    DivText += '<button type="button" class="button-button" data-automation-id="comment-reply-button" aria-label="答复' + userName + '" data-is-focusable="true">'
                    DivText += '<div class="ms-Button-flexContainer flexContainer-80">'
                    DivText += '<div class="ms-Button-textContainer textContainer-81">'
                    DivText += ' <img src="Images/reply_back.png" style="float:  left;padding: 5px 5px 0 0">'
                    DivText += ' <div class="ms-Button-label label-83" style="float:  left;">回复</div>'
                    DivText += ' </div>'
                    DivText += '</div>'
                    DivText += '</button>'
                    DivText += '</div>'
                    DivText += '</div>'
                    DivText += '</div>'
                    DivText += '</div>'
                    DivText += '<div class="spComments_8d833d90 Se_Id" id="Se__' + response.d + '"></div>'
                    DivText += '</div>'
                    DivFocusZone = $("#Posted__" + Obj);
                    
                }
                else {
                    DivText += '<div class="spComment_8d833d90" data-automation-id="sp-comment-block-root" id="Posted__' + response.d + '">'
                    DivText += '<div class="spContentRegion_8d833d90 ms-scaleUpIn100">'
                    DivText += '<div class="spAvatar_8d833d90" aria-hidden="true" role="presentation">'
                    DivText += '<canvas class="spAvatarImage_8d833d90" data- src="/_layouts/15/userphoto.aspx?size=S&amp;accountname=test2017@mobike.com" data- status="success" data- fingerprint="8a2bceaca7e84b8f4da535d772625c6f" width="32" height="32" > test2017 的照片</canvas >'
                    DivText += '</div >'
                    DivText += '<div class="spContent_8d833d90">'
                    DivText += '<span class="spAuthor_8d833d90" >' + userName + '</span >'
                    DivText += '<div class="spMetadata_8d833d90">'
                    DivText += '<a href="#comment=17" class="ms-Link root_c02e569e isEnabled_c02e569e" > ' + getCurrentTime() + '</a >'
                    DivText += '</div > '
                    DivText += '<div class="spText_8d833d90">'
                    DivText += '<span><span>' + CommentText + '</span></span>'
                    DivText += '</div>'
                    DivText += '</div>'
                    DivText += '</div>'
                    DivText += '<div class="spComments_8d833d90 Se_Id" id="Se__' + response.d + '"></div>'
                    DivText += '</div>'
                    DivFocusZone = $("#Se__" + Obj);
                }
                DivFocusZone.prepend(DivText)
                $("#Te__" + Obj).html("");
                var intcount = $("#sp-span-count").html();
                $("#sp-span-count").html(++intcount)
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
}

//回复
function Publish(Obj) {
    var Test = "#Se__" + Obj;
    var DivFocusZone = $(Test);
    var DivText = '';
    $(".CommForm").remove();
    DivText += '<div class="spReply_8d833d90 CommForm">'
    DivText += '<div class="spActions_8d833d90 ms-fadeIn500"></div>'
    DivText += '<div class="spAvatar_8d833d90" aria-hidden="true" role="presentation">'
    DivText += '<canvas class="spAvatarImage_8d833d90" data-src="/_layouts/15/userphoto.aspx?size=S&amp;accountname=test2017@mobike.com" data-status="success" width= "32" height= "32" data- fingerprint="8a2bceaca7e84b8f4da535d772625c6f" > test2017 的照片</canvas >'
    DivText += '</div>'
    DivText += '<div class="spField_8d833d90">'
    DivText += '<div aria-label="添加注释" class="spInput_8d833d90" contenteditable="true" placeholder="添加注释" id="Te__' + Obj + '" role="textbox"></div>'
    DivText += '</div>'
    DivText += '<div class="spReplyButtonBlock_8d833d90">'
    DivText += '<button id= "Btn__' + Obj + '"  onclick= "Release(' + Obj + ')" type= "button" class="ms-Button ms-Button--primary spButton_8d833d90 is-disabled root-85 button button-primary button-rounded button-small" data- automation - id="sp-comment-post" aria- label="发布" data- is - focusable="false" >'
    DivText += '发布'
    DivText += '</button >'
    DivText += '</div>'
    DivText += '</div>'
    DivFocusZone.append(DivText)
}

//获取当前时间
function getCurrentTime() {
    var date = new Date();
    var strYear = date.getFullYear();
    var strDay = date.getDate();
    var strMonth = date.getMonth() + 1;
    var strHour = date.getHours(); //时
    var strMinute = date.getMinutes(); //分
    var strSecond = date.getSeconds(); //秒  
    if (strMonth < 10) {
        strMonth = "0" + strMonth;
    }
    if (strDay < 10) {
        strDay = "0" + strDay;
    }
    if (strHour < 10) {
        strHour = "0" + strHour;
    }
    if (strMinute < 10) {
        strMinute = "0" + strMinute;
    }
    if (strSecond < 10) {
        strSecond = "0" + strSecond;
    }
    datastr = strYear + "/" + strMonth + "/" + strDay + " " + strHour + ":" + strMinute + ":" + strSecond;
    return datastr;
}
