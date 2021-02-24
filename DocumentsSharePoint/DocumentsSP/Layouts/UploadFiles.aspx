<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UploadFiles.aspx.cs" Inherits="DocumentsSP.Layouts.UploadFiles" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Upload Files</title>
    <script type="text/javascript" src="Scripts/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="Plugins/layer/layer.js"></script>
    <link href="Plugins/zTree/css/zTreeStyle/zTreeStyle.css" rel="stylesheet" />
    <script type="text/javascript" src="Plugins/zTree/js/jquery.ztree.all-3.5.min.js"></script>
    <link type="text/css" href="Content/UploadFiles.css" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/UploadFiles.js"></script>
</head>
<body>
    <form id="upload_form" runat="server">
        <div class="upload_div" id="file_upload">
            <div class="attr">
                <span>Choose Files</span>
                <input type="file" name="file_input" id="file_input" multiple />
            </div>
            <div class="attr">
                <span>Destination Folder</span>
                <input type="text" disabled="disabled" id="folder_path" name="folder_path" />
                <input type="hidden" id="FileFolder" name="FileFolder" />
                <input type="button" value="Choose Folder" class="choose_btn" onclick="SelectFolder()" />
            </div>
            <div class="attr">
                <span>RMS Encryption</span>
                <input type="checkbox" name="file_rms_ckb" id="file_rms_ckb" onchange="Tips()" />
                <span id="Tips">Notice: Only the suffix docx, xlsx, pptx files can be encrypted.</span>
            </div>
            <div class="attr_area">
                <span id="ve_span">Version Comments</span>
                <textarea id="version_area"></textarea>
                <input type="hidden"  id="_CheckinComment" name="_CheckinComment" />
            </div>
            <div class="attr_area">
                <span id="de_span">File Description</span>
                <textarea id="description_area"></textarea>
                <input type="hidden" value="" id="FileDescription" name="FileDescription" />
            </div>
            <div class="attr_submit">
                <input type="button" value="Cancel" onclick="Cancel()" />
                <input type="button" value="Confirm" onclick="Next()" />
            </div>
        </div>
        <div class="upload_div" id="file_rms">
            <div class="attr_rms">
                <span class="fl">RMS Encryption</span>
                <div id="rms_div">
                    <div class="rms_cl">
                        <input type="checkbox" id="FullControl" name="FullControl" />
                        <span>FullControl</span>
                    </div>
                    <div class="rms_cl">
                        <input type="checkbox" id="Read" name="Read" />
                        <span>Read</span>
                    </div>
                    <div class="rms_cl">
                        <input type="checkbox" id="Print" name="Print" />
                        <span>Print</span>
                    </div>
                    <div class="rms_cl">
                        <input type="checkbox" id="Save" name="Save" />
                        <span>Save</span>
                    </div>
                    <div class="rms_cl">
                        <input type="checkbox" id="Edit" name="Edit" />
                        <span>Edit</span>
                    </div>
                </div>
            </div>
            <div id="rms_users" class="attr_area">
                <span class="user_middle_title">Users</span>
                <input type="hidden" id="RMSUsers" name="RMSUsers" />
                <div id="users_select">
                    <div class="ulSelect">
                        <div>
                            <div class="ulA">
                                <div class="ulSearch">
                                    <input type="text" placeholder="Search User" />
                                    <img alt="Search" src="images/search.png" />
                                </div>
                            </div>
                            <div class="ulTree">
                                <ul id="treeUser" class="ztree"></ul>
                            </div>
                        </div>
                    </div>
                    <div class="ulShow">
                        <table>
                            <thead>
                                <tr>
                                    <td>UserName</td>
                                    <td>Operate</td>
                                </tr>
                            </thead>
                            <tbody id="SelContent">
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
            <div class="attr_submit" id="submit_div">
                <input type="button" value="Return" onclick="Return()" />
                <input type="button" value="Cancel" onclick="Cancel()" />
                <input type="button" value="Confirm" onclick="Upload()" />
            </div>
        </div>
    </form>
</body>
</html>
