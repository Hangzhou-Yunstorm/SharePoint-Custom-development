$(function () {
    //视频
    //Video();

    //快速链接
    QuickLinks();

    //最近使用的文档
    Documents();

    //公告
    Announcement();

    // 通讯录
    AddressBook();
})

//视频
function Video() {
    var VideoUrl = "/SPO/Videos/摩拜单车422一周年历程/摩拜单车422一周年历程.mp4?chId=8cdb14a2%2Dfb4f%2D4d44%2D8498%2De78cb63ef2c9&amp;vId=a3f5e44e%2Db6cd%2D426c%2D85ba%2Df21f8ed952d6&amp;width=640&amp;height=360&amp;autoPlay=true&amp;showInfo=true";
    $(".embedCode_11607c64 iframe").attr("src", VideoUrl);
}

//快速链接
function QuickLinks() {
    $.ajax({
        type: "post",
        url: "/_layouts/15/HelperPage.aspx/GetIconJson",
        data: "",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            if (response.d != "") {
                var DivList = new Function('return ' + response.d)();

                var DivFocusZone = $(".Quick-links");
                DivFocusZone.text("")
                var DivText = '<div role="list" aria- label="快速链接列表。列表中有 ' + DivList.length + ' 个项目。 使用向左键和向右键在链接之间移动。" data- drag - tag="compact-card-drag-zone" data- automation - id="list-wrapper" >'
                for (var i = 0; i < DivList.length; i++) {
                    DivText += '<div class="compactCardCell_6e9fe6ab" style="width: 230px;">';
                    DivText += '<a href="' + DivList[i].Url + '" target="_self" class="compactCardWrapper_6e9fe6ab ms-CompactCard-wrapper clickableCard_6e9fe6ab" role="listitem" data- interception="propagate" data- is - focusable="true" data- is - focused -in="false" data- drag - tag="compact-card-drag-leaf" aria- label="' + DivList[i].Name + '，列表中的第 ' + i + 1 + ' 个链接(共 ' + DivList.length + ' 个)。"  tabindex= "' + i + '" >';
                    DivText += '<div class="compactCard_6e9fe6ab" data-drag-handle="compact-card-drag-handler">';
                    DivText += '<div class="ms-CompactCard-icon-wrapper cardIconWrapper_6e9fe6ab" style="padding: 0px;">';
                    DivText += '<div class="ms-Icon ms-Icon-imageContainer root-76 imageContainer-78 root-76">';
                    DivText += '<div class="ms-Image root_031d536a" style="width: 46px; height: 46px;">';
                    DivText += '<img src="' + DivList[i].Img + '" role="presentation" class="ms-Image-image image_031d536a ms-Image-image--portrait imageIsPortrait_031d536a ms-Image-image--cover imageIsCover_031d536a is-loaded imageIsLoaded_031d536a is-fadeIn css-67">';
                    DivText += '</div>';
                    DivText += '</div>';
                    DivText += '</div>';
                    DivText += '<div data-automation-id="less-text" title="' + DivList[i].Name + '" class="lessText_e036c8c9 cardText_6e9fe6ab">';
                    DivText += '' + DivList[i].Name + '';
                    DivText += '</div>';
                    DivText += '</div>';
                    DivText += '</a>';
                    DivText += '</div >';
                }
                DivText += '</div>'
                DivFocusZone.append(DivText)
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
}

//最近使用的文档
function Documents() {
    $.ajax({
        type: "post",
        url: "/_layouts/15/HelperPage.aspx/GetLatestFiles",
        data: "",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            if (response.d != "") {
                var List_Documents = new Function('return ' + response.d)();

                var Div_Documents = $("._Documents");
                Div_Documents.text("")
                var _Documents = '';
                for (var i = 0; i < List_Documents.length; i++) {
                    _Documents += '<div role="presentation" class="ms-List-cell" data-list-index="0" data-automationid="ListCell">';
                    _Documents += '<div role="row" class="ms-FocusZone ms-DetailsRow css-67 root_1a072bea owner_1fadc7de" data-is-focusable="true" data-selection-index="0" data-item-index="0" aria-rowindex="0" data-is-draggable="false" draggable="false" data-automationid="DetailsRow" aria-selected="false" data-focuszone-id="FocusZone16" style="min-width: 738.656px;">';
                    _Documents += '<div class="ms-DetailsRow-fields fields_1a072bea" data-automationid="DetailsRowFields" role="presentation">';
                    _Documents += '<div role="gridcell" aria-colindex="0" class="ms-DetailsRow-cell cell_1a072bea" data-automationid="DetailsRowCell" data-automation-key="column1" style="width: 67px;">';
                    _Documents += '<div class="fileIcon_b82bf29c">';
                    _Documents += '<img alt="file_ico" src="' + List_Documents[i].Img + '">';
                    _Documents += '</div>';
                    _Documents += '</div>';
                    _Documents += '<div role="rowheader" aria-colindex="1" class="ms-DetailsRow-cell cell_1a072bea isRowHeader_1a072bea" data-automationid="DetailsRowCell" data-automation-key="column2" style="width: 389.656px;">';
                    _Documents += '<a href="' + List_Documents[i].Url + '" target="_blank" class="ms-Link root_c02e569e isEnabled_c02e569e" tabindex="-1">' + List_Documents[i].Title + '</a>';
                    _Documents += '</div>';
                    _Documents += '<div role="gridcell" aria-colindex="2" class="ms-DetailsRow-cell cell_1a072bea" data-automationid="DetailsRowCell" data-automation-key="column3" style="width: 116px;">';
                    _Documents += '<span>' + List_Documents[i].Date_Time + '</span>';
                    _Documents += '</div>';
                    _Documents += '<div role="gridcell" aria-colindex="3" class="ms-DetailsRow-cell cell_1a072bea" data-automationid="DetailsRowCell" data-automation-key="column4" style="width: 166px;">';
                    _Documents += '<span>' + List_Documents[i].Name + '</span>';
                    _Documents += '</div>';
                    _Documents += '</div>';
                    _Documents += '<span role="checkbox" class="checkCover_1a072bea" aria-checked="false" data-selection-toggle="true"></span>';
                    _Documents += '</div>';
                    _Documents += '</div>';
                }
                _Documents += '';
                Div_Documents.append(_Documents);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
}

//公告
function Announcement() {
    $.ajax({
        type: "post",
        url: "/_layouts/15/HelperPage.aspx/GetIndexNotice",
        data: "",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (response, textStatus) {
            if (response.d != "") {
                var List_TheSign = new Function('return ' + response.d)();
                var Div_TheSign = $(".TheSign");
                Div_TheSign.text("")
                var The_sign = '';
                for (var i = 0; i < List_TheSign.length; i++) {
                    if (List_TheSign[i].Content.length > 200) {
                        List_TheSign[i].Content = List_TheSign[i].Content.substring(0, 100) + "...";
                    }
                    The_sign += '<div role="listitem" class="ms-List-cell" data-list-index="0" data-automationid="ListCell">'
                    The_sign += '<div class="itemArea_1199886e">'
                    The_sign += '<div class="item_1199886e">'
                    The_sign += '<div class="newsItem_2e6d8972 newsItem__medium_2e6d8972 newsItem__hasImage_2e6d8972" data-automation-id="newsItem">'
                    The_sign += '<div class="imageArea_2e6d8972">'
                    The_sign += '<a class="imageArea_link_2e6d8972" data-interception="propagate" href="' + List_TheSign[i].Url + '" aria-hidden="true" role="presentation" data-is-focusable="false">'
                    The_sign += '<div class="ms-Image root_031d536a imageArea_image_2e6d8972">'
                    The_sign += '<img class="ms-Image-image image_031d536a ms-Image-image--landscape imageIsLandscape_031d536a ms-Image-image--cover imageIsCover_031d536a is-loaded imageIsLoaded_031d536a is-fadeIn css-67" aria-hidden="true" role="presentation" title="mobike&amp;line" src="' + List_TheSign[i].Img + '">'
                    The_sign += '</div>'
                    The_sign += '</a>'
                    The_sign += '</div>'
                    The_sign += '<div class="text_2e6d8972">'
                    The_sign += '<div>'
                    The_sign += '<a href="' + List_TheSign[i].Url + '" data-interception="propagate" aria-label="' + List_TheSign[i].Title + '' + List_TheSign[i].Name + ' 于 ' + List_TheSign[i].Date_Time + ' 发布。 说明:' + List_TheSign[i].Content + '" class="text_title_2e6d8972" >' + List_TheSign[i].Title + '</a >'
                    The_sign += '<div class="text_description_2e6d8972" title="' + List_TheSign[i].Content + '">'
                    The_sign += '' + List_TheSign[i].Content.length < 20 ? List_TheSign[i].Content.substring(0, 20) : List_TheSign[i].Content.substring(0, 20) + "..." + ''
                    The_sign += '</div>'
                    The_sign += '</div>'
                    The_sign += '<div class="metadata_9c132cc8" title="' + List_TheSign[i].Name + ' ' + List_TheSign[i].Date_Time + '">'
                    The_sign += '<div class="authorDate_9c132cc8">'
                    The_sign += '<span class="author_9c132cc8" aria-hidden="true" role="presentation">' + List_TheSign[i].Name + '</span> <span class="date_9c132cc8" aria-hidden="true" role="presentation">' + List_TheSign[i].Date_Time + '</span>'
                    The_sign += '</div>'
                    The_sign += '<div class="views_9c132cc8">'
                    The_sign += '<span class="viewCounts_0866e272" title="" aria-hidden="true" role="presentation"></span>'
                    The_sign += '</div>'
                    The_sign += '</div>'
                    The_sign += '</div>'
                    The_sign += '</div>'
                    The_sign += '</div>'
                    The_sign += '</div>'
                    The_sign += '</div>'
                }
                The_sign += '';
                Div_TheSign.append(The_sign);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus);
        }
    });
}

//问卷调查
function QSurvey() {

}

//通讯录
function AddressBook() {
    var Address_Book = [
        {
            Img: "/_layouts/15/Images/connect/1.jpg"
        },
        {
            Img: "/_layouts/15/Images/connect/2.jpg"
        },
        {
            Img: "/_layouts/15/Images/connect/3.jpg"
        }
    ];
    var Div_AddressBook = $(".Address-Book");
    Div_AddressBook.text("")
    var The_AddressBook = '<div class=" ms-Image root_031d536a" style="width: 353.328px; height: 198.747px;">';
    for (var i = 0; i < Address_Book.length; i++) {
        if (i == 0) {
            The_AddressBook += '<img style=" display: block;" src="' + Address_Book[i].Img + '" tabindex="' + i + '" alt="' + Address_Book[i].Img + '" class="ms-Image-image image_031d536a ms-Image-image--portrait imageIsPortrait_031d536a ms-Image-image--cover imageIsCover_031d536a is-loaded imageIsLoaded_031d536a is-fadeIn css-67">'
        }
        else {
            The_AddressBook += '<img style=" display: none;" src="' + Address_Book[i].Img + '" tabindex="' + i + '" alt="' + Address_Book[i].Img + '" class="ms-Image-image image_031d536a ms-Image-image--portrait imageIsPortrait_031d536a ms-Image-image--cover imageIsCover_031d536a is-loaded imageIsLoaded_031d536a is-fadeIn css-67">'
        }
    }
    The_AddressBook += '</div>'
    The_AddressBook += ' <div class="indexButtonContainer_8d115fcd sliderButtons_0bebf960" style="left: 10px;">'
    The_AddressBook += '<div onclick="changeImgLess()" class="indexButton_8d115fcd leftPositioned_8d115fcd ms-font-l" title="上一张" data-automation-id="prev-button"><i role="presentation" aria-hidden="true" data-icon-name="ChevronLeft" class="ms-Icon css-79 root-76"></i></div>'
    The_AddressBook += '</div>'
    The_AddressBook += '<div class="indexButtonContainer_8d115fcd sliderButtons_0bebf960" style="right: 10px;">'
    The_AddressBook += '<div onclick="changeImgAdd()" class="indexButton_8d115fcd rightPositioned_8d115fcd ms-font-l" title="下一张" data-automation-id="next-button"><i role="presentation" aria-hidden="true" data-icon-name="ChevronRight" class="ms-Icon css-79 root-76"></i></div>'
    The_AddressBook += '</div>'
    The_AddressBook += '<div class="currentActiveItem_0bebf960 ms-font-l" data-automation-id="item-count">'
    The_AddressBook += '第 1 项(共 ' + Address_Book.length + ' 项)'
    The_AddressBook += '</div>'
    Div_AddressBook.append(The_AddressBook);
}

var intList;
function changeImgAdd() {
    intList = 0;
    for (var i = 0; i < $(".Address-Book img").length; i++) {
        if ($(".Address-Book img").eq(i).css("display") == "block") {
            intList = $(".Address-Book img").eq(i).attr("tabindex");
        }
    }
    $(".Address-Book img").eq(intList).css("display", "none")
    if (intList == $(".Address-Book img").length - 1) {
        $(".Address-Book img").eq(0).css("display", "block")
    } else {
        $(".Address-Book img").eq(++intList).css("display", "block")
    }
}

function changeImgLess() {
    intList = 0;
    for (var i = 0; i < $(".Address-Book img").length; i++) {
        if ($(".Address-Book img").eq(i).css("display") == "block") {
            intList = $(".Address-Book img").eq(i).attr("tabindex");
        }
    }

    $(".Address-Book img").eq(intList).css("display", "none")
    if (intList == 0) {
        $(".Address-Book img").eq($(".Address-Book img").length - 1).css("display", "block")
    } else {
        $(".Address-Book img").eq(--intList).css("display", "block")
    }
}