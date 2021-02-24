<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Carousel.ascx.cs" Inherits="DocumentsSP.CustomWebPart.Carousel.Carousel" %>

<script type="text/javascript" src="/_layouts/15/Plugins/flexslider/jquery.flexslider-min.js"></script>
<link href="/_layouts/15/Plugins/flexslider/flexslider.css" rel="stylesheet" />

<div style="height: 300px; width: 100%;">
    <div class="flexslider">
        <ul class="slides" id="carousels">
        </ul>
    </div>
</div>
<script type="text/javascript">
    $(function () {
        $.ajax({
            type: "post",
            url: "/_layouts/15/WebPartContent.aspx/GetCarousels",
            data: "",
            contentType: "application/json;charset=utf-8",
            dataType: "json",
            success: function (response, textStatus) {
                $("#carousels").html(response.d);

                $(".flexslider").flexslider({
                    slideshow: true,
                    slideshowSpeed: 5000,
                    before: function (slider) {
                        slider.pause();
                        slider.play();
                    }
                });
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus);
            }
        });
    });
</script>
