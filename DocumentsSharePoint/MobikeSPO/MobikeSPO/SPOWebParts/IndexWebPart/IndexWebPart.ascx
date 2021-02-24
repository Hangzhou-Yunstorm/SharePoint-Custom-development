<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="IndexWebPart.ascx.cs" Inherits="MobikeSPO.SPOWebParts.IndexWebPart.IndexWebPart" %>

<link href="/_layouts/15/Content/MobikeHome.css" rel="stylesheet" />
<script src="/_layouts/15/Scripts/MobikeHome.js"></script>

<div class="CanvasZone ms-Grid-row CanvasZone--read CanvasZone--centerAlign">
    <div class="CanvasSection ms-Grid-col ms-sm12 ms-xl8 CanvasSection--read">
        <div class="ControlZone NoMTop">
            <div>
                <div class="ControlZone-control">
                    <div style="width: 100%;">
                        <div class="promptingTips">
                        </div>
                        <div class="realWebPart">
                            <div class="HTMLEmbed_11607c64" style="max-width: 739px;">
                                <div role="row" class="embedCode_11607c64" tabindex="0" aria-label="按 Enter 键或向下箭头键以输入嵌入的内容。" style="padding-bottom: 56.2923%;">
                                    <%--<iframe width="739" height="416" allowfullscreen="" tabindex="0"></iframe>--%>
                                    <video id="ctl00_ctl42_idPageMediaPlayer" width="739" height="416" poster="" autoplay="" onloadstart="" data-init="1" controls="">
                                        <source src="/SPO/Videos/摩拜单车422一周年历程/摩拜单车422一周年历程.mp4" data-label="" type="video/mp4">
                                    </video>
                                </div>
                                <div class="captionElement__caption_a52d03de captionElement__centerAlign_a52d03de">
                                    <span></span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div>
                </div>
            </div>
        </div>
        <!-- react-empty: 快速链接 -->
        <div class="ControlZone">
            <div>
                <div class="ControlZone-control">
                    <div style="width: 100%;">
                        <div class="webPartChrome_d27fb994">
                            <div class="webPartHeader_d27fb994 headerLgMargin_d27fb994">
                                <div class="title_d27fb994 captionElement__title_a52d03de captionElement__leftAlign_a52d03de">
                                    <span>快速链接</span>
                                </div>
                            </div>
                            <div class="content_d27fb994">
                                <div>
                                    <div>
                                        <div class="ms-Fabric css-64">
                                            <div>
                                                <div role="presentation" class="ms-FocusZone compactCardLayout_6e9fe6ab Quick-links">
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- react-empty: 快速链接下分割线 -->
        <div class="ControlZone">
            <div>
                <div class="ControlZone-control">
                    <div style="width: 100%;">
                        <div aria-hidden="true" class="dividerPlaceholder_cb20282c" role="presentation" tabindex="-1">
                            <hr aria-hidden="true" class="divider_cb20282c" role="presentation">
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- react-empty: 最近使用的文档 -->
        <div class="ControlZone">
            <div>
                <div class="ControlZone-control">
                    <div style="width: 100%;">
                        <div class="webPartChrome_d27fb994">
                            <div class="webPartHeader_d27fb994 headerLgMargin_d27fb994">
                                <div class="title_d27fb994 captionElement__title_a52d03de captionElement__leftAlign_a52d03de">
                                    <span>最近使用的文档</span>
                                </div>
                                <a aria-label="浏览所有项目" class="ms-Link root_c02e569e seeAll_d27fb994 isEnabled_c02e569e" href="/SPO/_layouts/15/SPOLatestFiles.aspx">全部查看</a>
                            </div>
                            <div class="content_d27fb994">
                                <div>
                                    <div>
                                        <div class="ms-Fabric ListLayout_4665c459 css-64" role="application" aria-label="使用向上键和向下键浏览内容的网格行。按 Enter 键导航到内容 URL。">
                                            <div class="ms-Viewport" style="min-width: 1px; min-height: 1px;">
                                                <div class="ms-DetailsList root_284156b6 is-horizontalConstrained rootIsHorizontalConstrained_284156b6">
                                                    <div role="grid" aria-rowcount="7" aria-colcount="4" aria-readonly="true">
                                                        <div role="presentation">
                                                            <div role="row" class="ms-FocusZone ms-DetailsHeader root_b78ba6e1">
                                                                <div role="columnheader" aria-sort="none" aria-disabled="false" aria-colindex="0" class="ms-DetailsHeader-cell cell_b78ba6e1 is-actionable cellIsActionable_b78ba6e1 is-empty cellIsEmpty_b78ba6e1" data-automationid="ColumnsHeaderColumn" data-item-key="column1" style="width: 67px;">
                                                                    <span class="cellTooltip_b78ba6e1">
                                                                        <span id="header13-column1" aria-labelledby="header13-column1-name " class="ms-DetailsHeader-cellTitle cellTitle_b78ba6e1" role="button" aria-describedby="header13-column1-tooltip" aria-haspopup="false" tabindex="0">
                                                                            <span id="header13-column1-name" class="ms-DetailsHeader-cellName"></span>
                                                                        </span>
                                                                    </span>
                                                                </div>
                                                                <div aria-hidden="true" role="button" class="ms-DetailsHeader-cellSizer cellSizer_b78ba6e1" tabindex="-1">
                                                                </div>
                                                                <div role="columnheader" aria-sort="none" aria-disabled="false" aria-colindex="1" class="ms-DetailsHeader-cell cell_b78ba6e1 is-actionable cellIsActionable_b78ba6e1" style="width: 378px;">
                                                                    <span class="cellTooltip_b78ba6e1">
                                                                        <span id="header13-column2" aria-labelledby="header13-column2-name " class="ms-DetailsHeader-cellTitle cellTitle_b78ba6e1" data-is-focusable="true" role="button" aria-describedby="header13-column2-tooltip" aria-haspopup="false" tabindex="-1">
                                                                            <span id="header13-column2-name" class="ms-DetailsHeader-cellName">
                                                                                <!-- react-text: 17 -->
                                                                                标题<!-- /react-text --></span>
                                                                        </span>
                                                                    </span>
                                                                </div>
                                                                <div aria-hidden="true" role="button" data-is-focusable="false" data-sizer-index="1" class="ms-DetailsHeader-cellSizer cellSizer_b78ba6e1" tabindex="-1">
                                                                </div>
                                                                <div role="columnheader" aria-sort="none" aria-disabled="false" aria-colindex="2" class="ms-DetailsHeader-cell cell_b78ba6e1 is-actionable cellIsActionable_b78ba6e1" data-automationid="ColumnsHeaderColumn" data-item-key="column3" style="width: 116px;">
                                                                    <span class="cellTooltip_b78ba6e1">
                                                                        <span id="header13-column3" aria-labelledby="header13-column3-name " class="ms-DetailsHeader-cellTitle cellTitle_b78ba6e1" data-is-focusable="true" role="button" aria-describedby="header13-column3-tooltip" aria-haspopup="false" tabindex="-1">
                                                                            <span id="header13-column3-name" class="ms-DetailsHeader-cellName">
                                                                                <!-- react-text: 23 -->
                                                                                已修改<!-- /react-text --></span>
                                                                        </span>
                                                                    </span>
                                                                </div>
                                                                <div aria-hidden="true" role="button" data-is-focusable="false" data-sizer-index="2" class="ms-DetailsHeader-cellSizer cellSizer_b78ba6e1" tabindex="-1">
                                                                </div>
                                                                <div role="columnheader" aria-sort="none" aria-disabled="false" aria-colindex="3" class="ms-DetailsHeader-cell cell_b78ba6e1 is-actionable cellIsActionable_b78ba6e1" data-automationid="ColumnsHeaderColumn" data-item-key="column4" style="width: 166px;">
                                                                    <span class="cellTooltip_b78ba6e1">
                                                                        <span id="header13-column4" aria-labelledby="header13-column4-name " class="ms-DetailsHeader-cellTitle cellTitle_b78ba6e1" data-is-focusable="true" role="button" aria-describedby="header13-column4-tooltip" aria-haspopup="false" tabindex="-1">
                                                                            <span id="header13-column4-name" class="ms-DetailsHeader-cellName">
                                                                                <!-- react-text: 29 -->
                                                                                修改者<!-- /react-text --></span>
                                                                        </span>
                                                                    </span>
                                                                </div>
                                                                <div aria-hidden="true" role="button" data-is-focusable="false" data-sizer-index="3" class="ms-DetailsHeader-cellSizer cellSizer_b78ba6e1" tabindex="-1">
                                                                </div>
                                                            </div>
                                                        </div>
                                                        <div role="presentation">
                                                            <div role="presentation" class="ms-FocusZone focusZone_284156b6" data-focuszone-id="FocusZone15">
                                                                <div class="ms-SelectionZone" role="presentation">
                                                                    <div role="presentation" class="ms-List">
                                                                        <div class="ms-List-surface" role="presentation">
                                                                            <div class="ms-List-page _Documents" role="presentation">
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="CanvasSection ms-Grid-col ms-sm12 ms-xl4 CanvasSection--read">
        <!-- react-empty:公告 -->
        <div aria-labelledby="cswpAccessibleLabelContextual_82f13ef7-ca1b-4d21-bb83-bf2a253e0f25" class="ControlZone NoMTop" data-sp-a11y-id="ControlZone_82f13ef7-ca1b-4d21-bb83-bf2a253e0f25">
            <div>
                <div class="ControlZone-control">
                    <div style="width: 100%;">
                        <div class="webPartChrome_d27fb994">
                            <div class="webPartHeader_d27fb994 headerLgMargin_d27fb994">
                                <div class="title_d27fb994 captionElement__title_a52d03de captionElement__leftAlign_a52d03de">
                                    <span>公告</span>
                                </div>
                                <a aria-label="查看所有公告" class="ms-Link root_c02e569e seeAll_d27fb994 isEnabled_c02e569e" data-automation-id="newsSeeAllLink" href="/SPO/_layouts/15/NoticeList.aspx">查看全部</a>
                            </div>
                            <div class="content_d27fb994">
                                <div>
                                    <div>
                                        <div class="listNewsLayout_1199886e listNewsLayout__small_1199886e" data-automation-id="listNewsLayout">
                                            <div role="list" aria-label="公告">
                                                <div role="list" class="ms-List">
                                                    <div class="ms-List-surface" role="presentation">
                                                        <div class="ms-List-page TheSign" role="presentation">
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div>
                    <!-- react-empty: 46 -->
                </div>
            </div>
        </div>
        <!-- react-empty: 问卷调查 -->
        <div aria-labelledby="cswpAccessibleLabelContextual_7c6602b2-34a5-4132-87cf-bb243cd82b7e" class="ControlZone" data-sp-a11y-id="ControlZone_7c6602b2-34a5-4132-87cf-bb243cd82b7e">
            <div>
                <div class="ControlZone-control">
                    <div style="width: 100%;">
                        <div class="webPartChrome_d27fb994">
                            <div class="webPartHeader_d27fb994 headerLgMargin_d27fb994">
                                <div class="title_d27fb994 captionElement__title_a52d03de captionElement__leftAlign_a52d03de">
                                    <span>问卷调查</span>
                                </div>
                            </div>
                            <div class="content_d27fb994">
                                <div>
                                    <div>
                                        <div role="application" style="max-width: 353.328px;">
                                            <div class="carouselContainer_0bebf960 filmStrip_0bebf960">
                                                <div class="slick-initialized slick-slider">
                                                    <div class="slick-list">
                                                        <div class="slick-track" style="opacity: 1; transform: translate3d(0px, 0px, 0px); width: 1119.94px;">
                                                            <div data-index="0" class="slick-slide slick-active" tabindex="0" style="outline: none; width: 373.313px;">
                                                                <a role="listitem" href="https://mobike-my.sharepoint.cn/personal/liuxingshun_mobike_com/_layouts/15/guestaccess.aspx?guestaccesstoken=hiDSGrxm0FTYxaVvO8bO5i0HuM3yhyuk7Lk2FEG1qHs%3d&amp;docid=1_151d5e02a3677411d94ee991b534faa0b&amp;wdFormId=%7b81CD6C49-B407-4F62-8901-5548DDBA4750%7d&amp;web=1" target="_self" class="quickItemTile_f24d7c74" aria-label="向我们反馈站点使用中的问题，列表中的第 1 个链接(共 1 个)。 图像: 邀请小伙伴反馈使用中的问题。" tabindex="0">
                                                                    <div role="presentation" class="ms-FocusZone" data-focuszone-id="FocusZone23">
                                                                        <div class="ms-DocumentCard root_118a77a5">
                                                                            <div class="ms-DocumentCardPreview preview_118a77a5">
                                                                                <div>
                                                                                    <div class="ms-Image root_031d536a" style="width: 353.328px; height: 198.747px;">
                                                                                        <img src="/_layouts/15/Images/wenjuan.jpg" role="presentation" alt="" class="ms-Image-image image_031d536a ms-Image-image--portrait imageIsPortrait_031d536a ms-Image-image--cover imageIsCover_031d536a is-loaded imageIsLoaded_031d536a is-fadeIn css-67">
                                                                                    </div>
                                                                                </div>
                                                                            </div>
                                                                            <div class="tilePlateArea_f24d7c74">
                                                                                <div data-automation-id="less-text" class="lessText_e036c8c9 tileText_f24d7c74">
                                                                                    向我们反馈站点使用中的问题
                                                                                </div>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </a>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="screenReaderOnly_0bebf960" id="469bffaf-04c0-4a88-9650-1cef9640f230-focusZone">
                                                使用向右键和向左键可在传送中的图像之间导航。
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div>
                    <!-- react-empty: 53 -->
                </div>
            </div>
        </div>
        <!-- react-empty: 通讯录 -->
        <div aria-labelledby="cswpAccessibleLabelContextual_3d699356-e495-4eb5-b05c-0c5c813ee291" class="ControlZone" data-sp-a11y-id="ControlZone_3d699356-e495-4eb5-b05c-0c5c813ee291">
            <div>
                <div class="ControlZone-control">
                    <div style="width: 100%;">
                        <div class="imageGallery_9d1054de">
                            <div class="titlePadding_9d1054de">
                                <div class="captionElement__title_a52d03de captionElement__leftAlign_a52d03de">
                                    <span>通讯录</span>
                                </div>
                            </div>
                            <div role="application" style="max-width: 353.328px;">
                                <div class="carouselContainer_0bebf960 Address-Book">
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div>
                    <!-- react-empty: 60 -->
                </div>
            </div>
        </div>
        <!-- react-empty: 61 -->
    </div>
    <!-- react-empty: 62 -->
</div>
<div class="CanvasZoneContainer CanvasZoneContainer--read" data-negative-space="true">
    <div class="CanvasZone ms-Grid-row CanvasZone--read CanvasZone--centerAlign" data-automation-id="CanvasZone" data-drag-tag="CanvasZone" data-negative-space="true" data-sp-a11y-id="CanvasZone_3">
        <div class="CanvasSection ms-Grid-col ms-sm12 ms-xl12 CanvasSection--read">
            <!-- react-empty: 67 -->
            <div aria-labelledby="cswpAccessibleLabelContextual_7c553c79-3d95-4296-a41e-e4c22c69f3c2" class="ControlZone" data-sp-a11y-id="ControlZone_7c553c79-3d95-4296-a41e-e4c22c69f3c2">
                <div>
                    <div class="ControlZone-control">
                        <div style="width: 100%;">
                            <div class="ms-Fabric css-64">
                                <div class="embedContainer_2126e996">
                                    <div>
                                        <div class="ms-Fabric bingPreview_ce375070 css-64">
                                            <a class="thumbnailLink_ce375070" href="https://mobike.com/cn/" target="_blank">
                                                <div class="ms-Image root_031d536a thumbnailContainer_ce375070" style="width: 240px; height: 135px;">
                                                    <img class="ms-Image-image image_031d536a ms-Image-image--landscape imageIsLandscape_031d536a ms-Image-image--cover imageIsCover_031d536a is-loaded imageIsLoaded_031d536a is-fadeIn css-70" src="/_layouts/15/Images/mologo.png" title="Mobike">
                                                </div>
                                            </a>
                                            <div class="metadata_ce375070">
                                                <a class="ms-Link root_c02e569e titleContainer_ce375070 isEnabled_c02e569e" href="https://mobike.com/cn/" target="_blank">
                                                    <div data-automation-id="less-text" title="Mobike" class="lessText_e036c8c9 metadataTitle_ce375070">
                                                        Mobike
                                                    </div>
                                                </a>
                                                <div class="metadataSubtitle_ce375070">
                                                    mobike.com
                                                </div>
                                                <div class="metadataDescription_ce375070" title="京 icp 备15053449 号 京公网安备 11010502032956 号 增值电信业务经营许可证编号：京b2-20170053">
                                                    京 icp 备15053449 号 京公网安备 11010502032956 号 增值电信业务经营许可证编号：京b2-20170053
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="screenReaderAlert_a7118188">
                                </div>
                                <div class="screenReaderAlert_a7118188">
                                </div>
                            </div>
                        </div>
                    </div>
                    <div>
                        <!-- react-empty: 73 -->
                    </div>
                </div>
            </div>
            <!-- react-empty: 74 -->
        </div>
        <!-- react-empty: 75 -->
    </div>
</div>
