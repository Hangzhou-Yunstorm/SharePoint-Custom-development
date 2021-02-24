<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GKSData.aspx.cs" Inherits="DocumentsSP.Layouts.GKSData" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>GKS Data</title>
    <script type="text/javascript" src="Scripts/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="Plugins/laydate-v5/laydate/laydate.js"></script>
    <script type="text/javascript">
        $(function () {
            //日期时间范围
            laydate.render({
                elem: '#date_range'
              , trigger: 'click'
              , range: "至"
               , btns: ['confirm']
            });
        });
    </script>
    <style type="text/css">
        #date_range {
            width: 180px;
            height: 26px;
            font-size: 14px;
            padding: 1px 1px 1px 5px;
        }

        #GetData {
            width: 80px;
            height: 32px;
            font-size: 18px;
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">
        <div>
            <div style="width: 80%; margin: 20px auto;">
                <asp:TextBox ID="date_range" runat="server"></asp:TextBox>
                <asp:Button ID="GetData" runat="server" OnClick="GetData_Click" Text="GET" />
            </div>
            <table border="1" style="width: 80%; margin: 0 auto; border-spacing: 0; color: #4f4f4f;">
                <tr>
                    <th colspan="11" style="background-color: #3a88ca; color: #fff;">GKS系统历史数据</th>
                </tr>
                <tr>
                    <th></th>
                    <th>登录</th>
                    <th>平台总文件、文件夹数量</th>
                    <th>文件上传</th>
                    <th>目录创建</th>
                    <th>文件下载</th>
                    <th>RMS加密</th>
                    <th>外部站点登录</th>
                    <th>外部站点上传</th>
                    <th>外部站点下载</th>
                    <th>外部站点用户</th>
                </tr>
                <tr>
                    <th>历史数据</th>
                    <th><asp:Label ID="Label11" runat="server" Text="0"></asp:Label></th>
                    <th><asp:Label ID="Label12" runat="server" Text="0"></asp:Label></th>
                    <th><asp:Label ID="Label13" runat="server" Text="0"></asp:Label></th>
                    <th><asp:Label ID="Label14" runat="server" Text="0"></asp:Label></th>
                    <th><asp:Label ID="Label15" runat="server" Text="0"></asp:Label></th>
                    <th><asp:Label ID="Label16" runat="server" Text="0"></asp:Label></th>
                    <th><asp:Label ID="Label17" runat="server" Text="0"></asp:Label></th>
                    <th><asp:Label ID="Label18" runat="server" Text="0"></asp:Label></th>
                    <th><asp:Label ID="Label19" runat="server" Text="0"></asp:Label></th>
                    <th><asp:Label ID="Label20" runat="server" Text="0"></asp:Label></th>
                </tr>
                <tr>
                    <th><asp:Label ID="Label1" runat="server" Text="-"></asp:Label></th>
                    <th><asp:Label ID="Label2" runat="server" Text="0"></asp:Label></th>
                    <th>-</th>
                    <th><asp:Label ID="Label4" runat="server" Text="0"></asp:Label></th>
                    <th><asp:Label ID="Label5" runat="server" Text="0"></asp:Label></th>
                    <th><asp:Label ID="Label6" runat="server" Text="0"></asp:Label></th>
                    <th><asp:Label ID="Label7" runat="server" Text="0"></asp:Label></th>
                    <th><asp:Label ID="Label8" runat="server" Text="0"></asp:Label></th>
                    <th><asp:Label ID="Label9" runat="server" Text="0"></asp:Label></th>
                    <th><asp:Label ID="Label10" runat="server" Text="0"></asp:Label></th>
                    <th>-</th>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
