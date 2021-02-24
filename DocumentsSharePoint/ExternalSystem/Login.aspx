<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ExternalSystem.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Login</title>
    <script src="Scripts/jquery-1.9.1.min.js"></script>
    <style>
        body {
            margin: 0;
            padding: 0;
            font-family: 'microsofr yahei';
        }

        .l_div {
            margin: 20px 0;
            text-align: center;
        }

            .l_div input {
                height: 25px;
                padding: 2px 2px 2px 6px;
                width: 222px;
                border: solid 1px #d8d8d8;
                font-size: 13px;
                background-color: #fff;
            }

            .l_div span {
                display: inline-block;
                width: 80px;
            }

        .s_div {
            text-align: center;
        }

            .s_div div {
                font-size: 16px;
                font-weight: bold;
            }

                .s_div div span {
                    display: inline-block;
                    width: 92px;
                }

                .s_div div hr {
                    border: none;
                    border-top: 1.5px solid #d8d8d8;
                    display: inline-block;
                    width: 65px;
                }

        .login_btn {
            height: 30px !important;
            width: 232px;
            border: 1px;
            cursor: pointer;
            background-color: #0089CD;
            color: #fff;
            font-size: 16px;
            /*box-shadow: 0px 2px 5px 0px #0089CD;*/
        }

        .login_div {
            border: 1px solid #efefef;
            background-color: #efefef;
            margin: 0 auto;
            width: 1000px;
            height: 400px;
            margin-top: 100px;
            box-shadow: 0px 0px 10px 5px rgba(91,91,91,0.16);
        }

        .logo_div {
            padding: 15px 0px 0px 20px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login_div">
            <div class="logo_div">
                <img src="Images/login.png" height="50" />
            </div>
            <div class="s_div">
                <div>
                    <hr />
                    <span>User Login</span>
                    <hr />
                </div>
            </div>
            <div class="l_div">
                <input type="text" id="Account" value="<%=Account %>" placeholder="Account" />
            </div>
            <div class="l_div">
                <input type="password" id="Password" value="" placeholder="Password" />
            </div>
            <div class="s_div">
                <input type="button" class="login_btn" onclick="Login()" value="Login" />
            </div>
        </div>
        <script type="text/javascript">
            //输入框的enter事件
            $('#Password').bind('keydown', function (event) {
                if (event.keyCode == "13") {
                    Login();
                }
            });

            function Login() {
                var account = document.getElementById("Account").value;
                var psw = document.getElementById("Password").value;
                if (account == "" || psw == "") {
                    alert("Please enter account and password to login !")
                } else {
                    $.ajax({
                        type: "post",
                        url: "Login.aspx/LoginSystem",
                        data: "{'account': '" + account + "','psw': '" + psw + "'}",
                        contentType: "application/json;charset=utf-8",
                        dataType: "json",
                        success: function (response, textStatus) {
                            var url = response.d;
                            window.location.href = url;
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            console.log(textStatus);
                        }
                    });
                }
            }
        </script>
    </form>
</body>
</html>
