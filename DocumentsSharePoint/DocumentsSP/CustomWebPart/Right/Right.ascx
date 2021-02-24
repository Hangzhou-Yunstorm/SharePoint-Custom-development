<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Right.ascx.cs" Inherits="DocumentsSP.CustomWebPart.Right.Right" %>

<div style="width: 460px; height: 580px;">
    <div class="TopTab">
        <div class="Ttab_bt Ttab-active noBorderRight">History</div>
        <div id="Subscription_Title" class="Ttab_bt noBorderRight">Subscription</div>
    </div>
    <div class="TopContent">
        <div class="Tcon_detail show">
            <div id="HistoryFileJson" class="detail"></div>
            <div class="more">
                <a href="/Lists/Browsing History/AllItems.aspx" title="More">
                    <img src="/_layouts/15/images/more.png" />
                </a>
            </div>
        </div>
        <div class="Tcon_detail">
            <div id="SubscriptionFileJson" class="detail"></div>
            <div class="more">
                <a href="/Lists/SubscribeList/AllItems.aspx" title="More">
                    <img src="/_layouts/15/images/more.png" />
                </a>
            </div>
        </div>
    </div>
</div>
<script>
    $(document).ready(function () {
        var tabT = $(".Ttab_bt");
        for (var i = 0; i < tabT.length; i++) {
            tabT[i].index = i;
            tabT[i].onclick = function () {
                $(this).addClass('Ttab-active').siblings().removeClass('Ttab-active');
                $(".Tcon_detail:eq(" + this.index + ")").addClass('show').siblings().removeClass('show');
            };
        }

        $.ajax({
            type: "post",
            url: "/_layouts/15/WebPartContent.aspx/GetHistoryList",
            data: "",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (response, textStatus) {
                $("#HistoryFileJson").html(response.d);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus);
            }
        });

        $.ajax({
            type: "post",
            url: "/_layouts/15/WebPartContent.aspx/GetSubscribeList",
            data: "",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (response, textStatus) {
                $("#SubscriptionFileJson").html(response.d);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus);
            }
        });

        $.ajax({
            type: "post",
            url: "/_layouts/15/WebPartContent.aspx/GetSubscribeCount",
            data: "",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (response, textStatus) {
                if (response.d == "0") {
                    $("#Subscription_Title").html("Subscription");
                } else {
                    $("#Subscription_Title").html("Subscription( " + response.d + " )");
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus);
            }
        });
    });
</script>
