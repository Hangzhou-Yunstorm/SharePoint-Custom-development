<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Left.ascx.cs" Inherits="DocumentsSP.CustomWebPart.Left.Left" %>

<style>
    .HotTab, .TopTab {
        display: block;
        overflow: hidden;
        margin-bottom: 10px;
        border-bottom: 1px solid #eee;
    }

    .Htab_bt, .Ttab_bt {
        float: left;
        width: 150px;
        height: 36px;
        box-sizing: border-box;
        line-height: 36px;
        margin-right: 10px;
        text-align: center;
        cursor: pointer;
    }

    .noBorderRight {
        border-right: none;
    }

    .Htab-active, .Ttab-active {
        border-bottom: 3px solid #0E60AC;
    }

    .HotContent, .TopContent {
        width: 100%;
        height: 535px;
        overflow: hidden;
    }

        .HotContent .detail, .TopContent .detail {
            height: 500px;
            overflow: hidden;
        }

        .HotContent .more, .TopContent .more {
            text-align: right;
            padding: 0 10px;
        }

    .Hcon_detail, .Tcon_detail {
        display: none;
        width: 100%;
        height: 100%;
    }

    .show {
        display: block;
    }

    .more img {
        width: 46px;
        height: 7px;
    }

    .detail img {
        margin-right: 10px;
    }

    .a_folder {
        margin-right: 50px;
    }
</style>

<div style="width: 650px; height: 580px;">
    <div class="HotTab">
        <div class="Htab_bt Htab-active noBorderRight">Latest</div>
        <div class="Htab_bt noBorderRight">Most Downloads</div>
        <div class="Htab_bt noBorderRight">Common</div>
        <div class="Htab_bt">Region</div>
    </div>
    <div class="HotContent">
        <div class="Hcon_detail show">
            <div id="LatestFileJson" class="detail"></div>
            <div class="more">
                <a href="/_layouts/15/LatestFiles.aspx" title="More">
                    <img src="/_layouts/15/images/more.png" />
                </a>
            </div>
        </div>
        <div class="Hcon_detail">
            <div id="MostDownLoadFileJson" class="detail"></div>
            <div class="more">
                <a href="/_layouts/15/MostDownLoadFiles.aspx" title="More">
                    <img src="/_layouts/15/images/more.png" />
                </a>
            </div>
        </div>
        <div class="Hcon_detail">
            <div id="CommonFileJson" class="detail"></div>
            <div class="more">
                <a href="/Documents/Forms/AllItems.aspx?RootFolder=/Documents/Common" title="More">
                    <img src="/_layouts/15/images/more.png" />
                </a>
            </div>
        </div>
         <div class="Hcon_detail">
            <div id="RegionFileJson" class="detail"></div>
            <div class="more">
                <a href="/_layouts/15/RegionFiles.aspx" title="More">
                    <img src="/_layouts/15/images/more.png" />
                </a>
            </div>
        </div>
    </div>
</div>
<script>
    $(document).ready(function () {
        var tabH = $(".Htab_bt");
        for (var i = 0; i < tabH.length; i++) {
            tabH[i].index = i;
            tabH[i].onclick = function () {
                $(this).addClass('Htab-active').siblings().removeClass('Htab-active');
                $(".Hcon_detail:eq(" + this.index + ")").addClass('show').siblings().removeClass('show');
            };
        }

        $.ajax({
            type: "post",
            url: "/_layouts/15/WebPartContent.aspx/GetLatestFileJson",
            data: "",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (response, textStatus) {
                $("#LatestFileJson").html(response.d);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus);
            }
        });

        $.ajax({
            type: "post",
            url: "/_layouts/15/WebPartContent.aspx/GetMostDownLoadFileJson",
            data: "",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (response, textStatus) {
                $("#MostDownLoadFileJson").html(response.d);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus);
            }
        });

        $.ajax({
            type: "post",
            url: "/_layouts/15/WebPartContent.aspx/GetCommonFileJson",
            data: "",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (response, textStatus) {
                $("#CommonFileJson").html(response.d);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus);
            }
        });

        $.ajax({
            type: "post",
            url: "/_layouts/15/WebPartContent.aspx/GetRegionFilesJson",
            data: "",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (response, textStatus) {
                $("#RegionFileJson").html(response.d);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus);
            }
        });
    });
</script>

