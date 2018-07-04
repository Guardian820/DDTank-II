﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="forgotpass.aspx.cs" Inherits="Tank.Flash.auth.forgotpass" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="../script/jquery.js" type="text/javascript"></script>

    <script language="javascript" type="text/javascript">
        function href() {
            var randomnum = Math.random();
            var getimagecode = document.getElementById("ImageCode");
            getimagecode.src = "ValidateCode.aspx? " + randomnum;
        }
        function xmlhttpPost(strURL) {
            var xmlHttpReq = false;
            var self = this;
            // Mozilla/Safari
            if (window.XMLHttpRequest) {
                self.xmlHttpReq = new XMLHttpRequest();
            }
            // IE
            else if (window.ActiveXObject) {
                self.xmlHttpReq = new ActiveXObject("Microsoft.XMLHTTP");
            }
            self.xmlHttpReq.open('POST', strURL, true);
            self.xmlHttpReq.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
            self.xmlHttpReq.onreadystatechange = function () {
                if (self.xmlHttpReq.readyState == 4) {
                    updatepage(self.xmlHttpReq.responseText);
                }
            }
            self.xmlHttpReq.send(getquerystring());
        }

        function getquerystring() {
            var form = document.forms['f1'];
            var username = form.username.value;
            var password = form.password.value;
            var repassword = form.repassword.value;
            var email = form.email.value;
            //var sex = form.sex.value;
            var code = form.code.value;
            qstr = 'username=' + escape(username)
            + '&password=' + escape(password)
            + '&repassword=' + escape(repassword)
            + '&email=' + escape(email)
            //+ '&sex=' + escape(sex)
            + '&code=' + escape(code);  // NOTE: no '?' before querystring
            return qstr;
        }

        function updatepage(str) {
            if (str == "ok") {
                //alert("Reg Success。");
                //location.replace("login.aspx")
                $("#lbError").html('<span id="lbError" style="color:Green;" >Registration Succeed!</span>');
                var form = document.forms['f1'];
                form.username.value = '';
                form.password.value = '';
                form.repassword.value = '';
                form.email.value = '';
                form.code.value = '';
                href();
            }
            else
                $("#lbError").html(str);
        }     
    </script>

    <style type="text/css">
        .style3
        {
            text-align: right;
            height: 50px;
            width: 180px;
        }
        .style4
        {
            text-align: left;
            height: 50px;
        }
        .style5
        {
            text-align: center;
            height: 38px;
        }
        .w0
        {
            width: 60px;
        }
        .user_input
        {
            width: 150px;
        }
        #ReLoad{height:23px;width:25px;background:url(../images/iconRe.jpg) no-repeat;text-indent:-9999px;display:block;overflow:hidden;}

        .style10
        {
            width: 85px;
        }

        .style13
        {
            width: 100%;
            height: 355px;
        }
        .style18
        {
            text-align: center;
            height: 30px;
        }
        .style19
        {
            text-align: right;
            height: 40px;
            width: 180px;
        }
        .style20
        {
            text-align: left;
            height: 40px;
        }
        
    </style>

</head>
<body style="background: url(../images/forgot.png) no-repeat scroll 0pt 0pt transparent;">
    <form name="f1">
   
   <table class="style13">
                                    <tr>
                                        <td class="style3">
                                        </td>
                                        <td class="style4">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style18" colspan="2">
                                           <div class="top">
                             <span id="lbError" style="color:Red;" ></span><br />
                            </div>
                            </td>
                                    </tr>
                                    <tr>
                                        <td class="style19">
                                           <span  style="font-size: 18px; color: #F60; font-weight: bold;">Tài khoản：</span>
                                        </td>
                                        <td class="style20">
                                          <input type="text" class="user_input" value="" name="username" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style19">
                                           <span  style="font-size: 18px; color: #F60; font-weight: bold;">Email：</span>
                                        </td>
                                        <td class="style20">
                                           <input type="text" class="user_input" value="" name="email" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="style19">
                                            <span  style="font-size: 18px; color: #F60; font-weight: bold;">Mã bảo vệ:&nbsp;&nbsp;&nbsp; </span>
                                            </td>
                                        <td class="style20">
                                             <table style="width: 27%;">
                                                 <tr>
                                                     <td class="style10">
                                                         <img id="ImageCode" src="ValidateCode.aspx" height="24px" alt="" />
                                  </td>
                                                     <td>
                                                        <a id="ReLoad" title="" href="javascript:href()" style="font-size: 12px; color: blue">Tải lại</a> </td>
                                                 </tr>
                                             </table>
                                             </td>
                                    </tr>
                                    <tr>
                                        <td class="style19">
                                           <span  style="font-size: 18px; color: #F60; font-weight: bold;">Nhập mã bảo vệ：</span>
                                           </td>
                                        <td class="style20">
                                         <input type="text" class="user_input" name="code" size="4" />
                                          
                                  </td>
                                    </tr>
                                    <tr>
                                        <td class="style5" colspan="2">
                                            
                                            <input type="button" value="Lấy lại password"
                                                onclick='JavaScript:xmlhttpPost("forgotpass.ashx")' 
                                                style="width: 200px; height: 30px; color: #FF6600; font-weight: bold; font-size: large; cursor: pointer" />
                              
                              </td>
                                    </tr>
                                </table>
    </form>
</body>
</html>
