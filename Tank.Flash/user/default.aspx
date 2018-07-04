<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="register.aspx.cs" Inherits="Tank.Flash.auth._register" %>
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <title>User Center - Crazy Jogos</title>
    <meta name="description" content="">
    <meta name="author" content="">
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
                $("#lbError").html('<span id="lbError" style="color:Green;" >Conta criada com sucesso</span>');
               var form = document.forms['f1'];
               form.username.value='';
               form.password.value = '';
               form.repassword.value = '';
               form.email.value = '';
               form.code.value = '';
               //form.sex.value = 0;
               href();
            }
            else
                $("#lbError").html(str);
        }     
    </script>

    <style type="text/css">
        .style1
        {
            text-align: center;
        }
        .style3
        {
            text-align: right;
            height: 71px;
            width: 161px;
        }
        .style4
        {
            text-align: left;
            height: 71px;
        }
        .style5
        {
            text-align: center;
            height: 40px;
        }
        .w0
        {
            width: 60px;
        }
        .user_input
        {
            width: 150px;
        }
        .style8
        {
            text-align: right;
            height: 30px;
            width: 161px;
        }
        .style9
        {
            text-align: left;
            height: 30px;
        }
        #ReLoad{height:23px;width:25px;background:url(../images/iconRe.GIF) no-repeat;text-indent:-9999px;display:block;overflow:hidden;}

        .style10
        {
            width: 127px;
        }

        .style11
        {
            text-align: right;
            height: 40px;
            width: 161px;
        }
        .style12
        {
            text-align: left;
            height: 40px;
        }
        
        #Select1
        {
            width: 54px;
        }
        #gende
        {
            width: 54px;
        }
        
        .top
        {
            width: 372px;
        }
        
    </style>

    <link href="../css/337/v3static/css/elexGlobal.css-1305091305.css" tppabs="http://337.eleximg.com/337/v3static/css/elexGlobal.css?1305091305" rel="stylesheet" type="text/css"/>
    <link id="hasElexCn" href="../css/337/v3static/css/elexCn.css-1304030000.css" tppabs="http://337.eleximg.com/337/v3static/css/elexCn.css?1304030000" rel="stylesheet" type="text/css"/>
	<link href="../css/337/v3static/css/header_v3.css-1304270007.css" tppabs="http://337.eleximg.com/337/v3static/css/header_v3.css?1304270007" rel="stylesheet" type="text/css"/>
		<link href="../css/337/v3static/css/elexDialog.css-1307081929.css" tppabs="http://337.eleximg.com/337/v3static/css/elexDialog.css?1307081929" rel="stylesheet" type="text/css"/>
	
    <link rel="shortcut icon" href="http://337.eleximg.com/337/v3static/ico/favicon.ico">
    



<!--[if lt IE 9]>
<script src="../../../../html5shim.googlecode.com/svn/trunk/html5.js" tppabs="http://html5shim.googlecode.com/svn/trunk/html5.js"></script>
<![endif]-->
<link href="../css/337/v3static/css/bootstrap.min.css" tppabs="http://337.eleximg.com/337/v3static/css/bootstrap.min.css" rel="stylesheet" type="text/css"/>
<link href="../css/337/v3static/css/appsGlobals.css" tppabs="http://337.eleximg.com/337/v3static/css/appsGlobals.css" rel="stylesheet" type="text/css"/>
<link href="../css/337/v3static/css/apps_register_new.css-1306281508.css" tppabs="http://337.eleximg.com/337/v3static/css/apps_register_new.css?1306281508" rel="stylesheet" type="text/css"/>
<link rel="stylesheet" href="../css/337/v3static/css/third_login.css-1308221829.css" tppabs="http://337.eleximg.com/337/v3static/css/third_login.css?1308221829" type="text/css"/>

    <script>
	//add google analytics
    (function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){
    	  (i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),
    	  m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)
    	  })(window,document,'script','../../../../www.google-analytics.com/analytics.js'/*tpa=http://www.google-analytics.com/analytics.js*/,'ga');
    ga('create', 'UA-39391247-1', 'http://web.337.com/pt/user/337.com');
    ga('send', 'pageview');
	</script>
</head>
<body>





<div class="wrapper">
    <!--loginBox start-->
    <div class="login_box">
        <div class="modal-header">
            <h3 id="myModalLabel">DDTank Crazy II&nbsp;</h3>
        </div>
        <form id="frmLogin" name="frmLogin" method="post" action="">
           <a href="../" id="entrar" style="-webkit-box-shadow: rgba(255, 255, 255, 0.2) 0px 1px 0px inset, rgba(0, 0, 0, 0.0470588) 0px 1px 2px; background-color: whitesmoke; background-image: linear-gradient(rgb(255, 255, 255), rgb(230, 230, 230)); background-repeat: repeat no-repeat; border-bottom-left-radius: 4px; border-bottom-right-radius: 4px; border-color: rgba(0, 0, 0, 0.0980392) rgba(0, 0, 0, 0.0980392) rgb(179, 179, 179); border-style: solid; border-top-left-radius: 4px; border-top-right-radius: 4px; border-width: 1px; box-shadow: rgba(255, 255, 255, 0.2) 0px 1px 0px inset, rgba(0, 0, 0, 0.0470588) 0px 1px 2px; color: #333333; cursor: pointer; font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif; font-size: inherit; line-height: 20px; margin: 0px; outline: 0px; padding: 4px 12px; text-shadow: rgba(255, 255, 255, 0.74902) 0px 1px 1px; vertical-align: middle; width: 200px;"  target="_blank">Fazer login no jogo</a>
        </form>
        <div class="other_login">
            <div class="oauth-button third-login-48 login-facebook-48" data-type='facebook' title="facebook"></div>
            <div class="oauth-button third-login-48 login-google-48" data-type='google' title="google"></div>
                        <div class="oauth-button third-login-48 login-orkut-48" data-type='orkut' title="orkut"></div>
                                                <div class="oauth-button third-login-48 login-twitter-48" data-type='twitter' title="twitter"></div>
                        <div class="oauth-button third-login-48 login-yahoo-48" data-type='yahoo' title="Yahoo"></div>
            <div class="oauth-button third-login-48 login-msn-48" data-type='wl' title="Windows Live Messenger"></div>
                    </div>
    </div>
    <!--loginBox end-->
    <!--registerBox start-->
    <div class="register_box">
        <div class="modal-header">
            <h3 id="myModalLabel">Cadastre-se agora</h3>
        </div>
        <form name="f1">
            <div class="rg_username">
            	<div class="reg_label">Nome de Usuário：</div>
                <input type="text" class="user_input"   value="" name="username" />
                <div class="input_status" id="pwuser_status" style=""></div>
            </div>
            <div class="rg_pw">
            	<div class="reg_label">Nova senha：</div>
                <input type="password" class="user_input"   value="" name="password" />
                <div class="input_status" id="passwd_status" style=""></div>
            </div>

            <div class="rg_repw">
            	<div class="reg_label">Digite a senha novamente：</div>
                <input type="password" class="user_input"   value="" name="repassword" />
                <div class="input_status" id="repasswd_status" style=""></div>
            </div>



            <div class="rg_email">
            	<div class="reg_label">E-mail：</div>
               <input type="text" class="user_input"  value="" name="email" />
                <div class="input_status" id="pwemail_status" style=""></div>
            </div>
             <div class="rg_captcha">
            	<div class="reg_label"><span style="background-color: white; color: #333333; font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif; font-size: 12px; line-height: 30px; text-align: right;">Código：</span><br /></div>
               <img id="ImageCode" src="ValidateCode.aspx" height="24px" alt="" />
                <div class="input_status" id="pwemail_status" style=""></div>
            </div>
            <div class="rg_codig">
            	<div class="reg_label"><span style="background-color: white; color: #333333; font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif; font-size: 12px; line-height: 30px; text-align: right;">Digite o código：</span><br /></div>
               <input type="text"   style="-webkit-box-shadow: rgba(0, 0, 0, 0.0745098) 0px 1px 1px inset; -webkit-transition: border 0.2s linear, box-shadow 0.2s linear; background-color: #f0f0f0; border-bottom-left-radius: 4px; border-bottom-right-radius: 4px; border-top-left-radius: 4px; border-top-right-radius: 4px; border: 1px solid rgb(221, 223, 223); box-shadow: rgba(0, 0, 0, 0.0745098) 0px 1px 1px inset; color: #555555; float: left; height: 30px; line-height: 30px; margin: 0px 0px 4px; padding: 4px 6px; transition: border 0.2s linear, box-shadow 0.2s linear; vertical-align: middle; width: 209px;" class="user_input" name="code" size="4" style="width: 150px" />
                <div class="input_status" id="pwemail_status" style=""></div>
            </div>
           
            <div class="rg_btn" id="register_btn">
               <span id="lbError" style="color:Red;" ></span>
              <input class="btn btn-large btn-block btn-primary" type="button"   value="Registrar"
                                                onclick='JavaScript:xmlhttpPost("register.ashx")' 
                                                 />               <span class="ajaxLoading" id="register_loading" style="display:none;">
                        <img src="../css/337/v3static/img/loading.gif-.gif" tppabs="http://337.eleximg.com/337/v3static/img/loading.gif?" alt="ajax loading" />
                    </span>
                </button>
            </div>
<!--            <div id="elexTipsAjax" class="elexTips alert alert-error" style="display:none;"></div>-->
            <!--<div class="login" id="">
                <a>Fazer login</a>
            </div>-->
        </form>
    </div>
    <!--registerBox end-->
</div>
<div id="fb-root"></div>



<script type="text/javascript" src="../css/337/v3static/js/jquery.min.js" tppabs="http://337.eleximg.com/337/v3static/js/jquery.min.js"></script>
<script src="../css/337/v3static/js/elex.library.js-1304030000.js" tppabs="http://337.eleximg.com/337/v3static/js/elex.library.js?1304030000"></script>
<script src="../css/337/v3static/js/elex.global.js-1304030000.js" tppabs="http://337.eleximg.com/337/v3static/js/elex.global.js?1304030000"></script>
<script src="../css/337/v3static/js/elex.dialog.js-1304030000.js" tppabs="http://337.eleximg.com/337/v3static/js/elex.dialog.js?1304030000"></script>
<script src="../css/337/v3static/js/elex.common.js-1304030000.js" tppabs="http://337.eleximg.com/337/v3static/js/elex.common.js?1304030000"></script>
<!--<script type="text/javascript" src="http://337.eleximg.com/337/v3static/js/googleconnect.js?v=2"></script>-->
<!--<script type="text/javascript" src="http://337.eleximg.com/337/v3static/js/bahaconnect.js"></script>-->

<script src="../css/337/v3static/js/xaauto.js-v=11.js#,pt," tppabs="http://337.eleximg.com/337/v3static/js/xaauto.js?v=11#,pt,"></script>

<script type="text/javascript">

    //set config
    elex.config.set({
        baseUrl : 'http://web.337.com/pt',
        wikiUrl : 'http://web.337.com/pt/page',
        newUrl : 'http://web.337.com/pt',
        cdnUrl : 'http://337.eleximg.com/337/v3static',
        static_vertion : '1304030000',
        uid : '0'
    });

    //set language
    elex.langs.set({
        //login
        close : 'fechar',
        network_connect_fail : 'Falha ao conectar na internet, favor tentar novamente',
        email_be_used : 'O email já foi usado, favor trocar outro email',
        username_or_password_error : 'Nome de usuário ou senha incorretos',
        play_now : 'Jogar',
        view_more : 'Mostrar mais',
        no_game_history : 'Sem histórico de jogo.',
        no_permission : 'Você não tem permissão para fazer esta operação!',
        upload_picture : 'Carregar a foto',
        captchacode_check_error :'Erro no código Captcha, favor digitar de novo!',
        select_your_language :'Escolha a sua língua',
        goto_top : 'De volta para o topo'
    });

    $('#gamebase_connect').click(function()
    {
        window.location.href = 'http://www.gamebase.com.tw/etc/api/xlhc_login_api.php';
    });


    var changeWidth = function(){
        /*
        var regWidth = $('.register_box').width();
        //elex.log(regWidth);

        if(regWidth > 280){
            $('.wrapper').width(812);         
        }else{
            $('.wrapper').width(600);
        }
        */
    }
    //注册
    var reg_status = {
        'username' : false,
        'password' : false,
        'repassword' : false,
        'email' : false
    }
    username_error_msg = {
        '-1' : 'Username is incorrect. Please try again.',
        '-2' : 'Username cannot be used. Please try again.',
        '-3' : 'nome de usuário já existe'
    }
    //$('#reg_pwuser').focus();
    $("#reg_pwuser").blur(function(event){
        $('#pwuser_status').html('').removeAttr('class');
        var error = "";
        var username_reg = /^[0-9a-zA-Z_]*$/;
        var username = this.value.replace(/^\s+/, '').replace(/\s+$/, '');
        if(username == "" || username == "Nome de Usuário"){
            error = "O nome do usuário não pode ser deixado em branco";
        }else if(username.length <= 3 || username.length > 15){
            error = "4-15 Caracteres";
        }else if(!username_reg.test(username)){
            error = "O nome de usuário deve ser formado com caracteres e números";
        }
        if (error != '') {
            $("#pwuser_status").html(error).addClass('input_status');
            $('#reg_pwuser').removeClass('success');
            reg_status['username'] = false;
            changeWidth();
        }else {
            elex.request.post('checkUsername.htm'/*tpa=http://web.337.com/pt/user/checkUsername*/, {"username":username}, function(ret){
                if(ret && ret.status == 0){
                    $("#pwuser_status").removeAttr('class');
                    reg_status['username'] = true;
                    changeWidth();
                    $("#reg_pwuser").addClass('success');
                }else{
                    var error_msg = ret ? ret.msg: "Falha na verificação do nome do usuário";
                    $("#pwuser_status").html(error_msg).removeAttr('class').addClass('input_status');
                    $('#reg_pwuser').removeClass('success');
                    reg_status['username'] = false;
                    changeWidth();
                }
            },function(){
                $("#pwuser_status").html("Falha ao conectar na internet, favor tentar novamente").removeAttr('class').addClass('input_status');
                $('#reg_pwuser').removeClass('success');
                reg_status['username'] = true;
                changeWidth();
            });
        }
    });
    $("#reg_pwpwd").blur(function(){
        $('#passwd_status').html('').removeAttr('class');
        var error = '';
        var pwd = this.value.replace(/^\s+/, '').replace(/\s+$/, '');
        if(pwd == "" || pwd == "Nova senha"){
            error = "Senha não pode estar vazia";
        }else if(pwd.length < 6 || pwd.length > 20){
            error = "A senha precisa ter 6 à 20 caracteres";
        }
        if (error != '') {
            $("#passwd_status").html(error).addClass('input_status');
            $('#reg_pwpwd').removeClass('success');
            reg_status['password'] = false;
            changeWidth();
        }else {
            if(this.value == $("#reg_pwpwd1").val()){
                $("#repasswd_status").html('').removeAttr('class');
                reg_status['repassword'] = true;
                $("#reg_pwpwd1").addClass('success');
            }else{
                $("#repasswd_status").html("Confirmar a senha e a senha original está diferente").addClass('input_status');
                $('#reg_pwpwd1').removeClass('success');
                reg_status['repassword'] = false;
            }
            $("#passwd_status").removeAttr('class');
            reg_status['password'] = true;
            changeWidth();
            $("#reg_pwpwd").addClass('success');
        }
    });
    $("#reg_pwpwd1").blur(function(){
        $('#repasswd_status').html('').removeAttr('class');
        var error = "";
        if(this.value != $("#reg_pwpwd").val()){
            error = "Confirmar a senha e a senha original está diferente";
        }else if($(this).val() == '' || $(this).val() == '{$langs["337_password_again"]}'){
            error = "Senha não pode estar vazia";
        }
        if (error != '') {
            $("#repasswd_status").html(error).addClass('input_status');
            $('#reg_pwpwd1').removeClass('success');
            reg_status['repassword'] = false;
            changeWidth();
        }else {
            $("#repasswd_status").removeAttr('class');
            reg_status['repassword'] = true;
            changeWidth();
            $("#reg_pwpwd1").addClass('success');
        }
    });
    $('#reg_pwemail').blur(function(){
        $('#pwemail_status').html('').removeAttr('class');
        var error = '';
        var email_reg = /^[\w\-\.]+@[\w\-\.]+(\.\w+)+$/;
        var email = this.value.replace(/^\s+/, '').replace(/\s+$/, '');
        //elex.log(email_reg.test(email));
        if(email == "" || email == "E-mail"){
            error = "O endereço de e-mail não pode estar vazio";
        }else if(!email_reg.test(email) || email.length <= 6){
            error = "O formato do email não está de acordo com a regra";
        }
        if (error != '') {
            $("#pwemail_status").html(error).addClass('input_status');
            $('#reg_pwemail').removeClass('success');
            reg_status['email'] = false;
            changeWidth();
        }else {
            $("#pwemail_status").removeAttr('class');
            elex.request.post('checkEmail.htm'/*tpa=http://web.337.com/pt/user/checkEmail*/, {"email":email}, function(ret){
                if(ret && ret.status >= 0){
                    $("#pwemail_status").removeAttr('class');
                    reg_status['email'] = true;
                    changeWidth();
                    $("#reg_pwemail").addClass('success');
                }else{
                    var error_msg = "O email já foi usado, favor trocar outro email";
                    $("#pwemail_status").html(error_msg).removeAttr('class').addClass('input_status');
                    $('#reg_pwemail').removeClass('success');
                    reg_status['email'] = false;
                    changeWidth();
                }
            },function(){
                $("#pwemail_status").removeAttr('class');
                reg_status['email'] = true;
                changeWidth();
                $("#reg_pwemail").addClass('success');
            });
        }
    });
    var regFlag = false;
    $('#register_btn').click(function(){
        $('#register_loading').css('display','inline');
        if (reg_status['username'] && reg_status['password'] && reg_status['repassword'] && reg_status['email']) {
            if(regFlag){
                return;
            }
            regFlag = true;
//            $("#elexTipsAjax").removeAttr('class').html('').hide();

            var username = $("#reg_pwuser").val();
            var	password = $("#reg_pwpwd").val();
            var email = $("#reg_pwemail").val();
            elex.request.post('register.htm'/*tpa=http://web.337.com/pt/user/register*/, {"username":username,"password":password,"email":email}, function(ret){
                if (ret.uid > 0) {
                    regFlag = false;
                    elex.user._afterLogin(ret);
                    window.location.href = getRedirectUrl();

                } else {
                    var err_msg = ret ? ret.msg : "Falha ao registrar, favor checar o que você inseriu ";
//                    $("#elexTipsAjax").removeAttr('class').html(err_msg).addClass('elexTips alert alert-error').show();
                    $('#register_loading').hide();
                    regFlag = false;
                }
            });
        } else{
            $("#reg_pwuser").blur();
            $('#register_loading').hide();
        }

    });
    //登陆
    //$('#login_username').focus();
    var flag = false;
    $('#login_btn').click(function(){
        if(flag) return;
        flag = true;
        $("#elexSiteTips").removeAttr('class').html('');
        var username = $("#login_username").val();
        var password = $("#login_password").val();
        if(username == "" || username == "Digite sua conta"){
            setTimeout(function(){
                //$('#setSiteLoading').hide();
                $("#elexSiteTips").html("O nome do usuário não pode ser deixado em branco").addClass('alert alert-error').show();
            },1000);
            flag = false;
            return;
        }
        if(password == "" || password == "Digite sua senha"){
            setTimeout(function(){
                $('#setSiteLoading').hide();
                $("#elexSiteTips").html("Senha não pode estar vazia").show().addClass('alert alert-error').show();
            },1000);
            flag = false;
            return;
        }
        $('#login_loading').css('display','inline');
        var remember = 1;
        var rt = 0;
        //elex.user.login(username, password, remember, function(ret) {
        elex.request.post('ajaxLogin.htm'/*tpa=http://web.337.com/pt/user/ajaxLogin*/, {"username":username,"password":password}, function(ret){

            if(ret.uid > 0) {
                elex.user._afterLogin(ret, remember);
                window.location.href = getRedirectUrl();
            } else if(ret.status < 0) {
            	 setTimeout(function(){
                     $('#setSiteLoading').hide();
                     $("#elexSiteTips").html("Nome de usuário ou senha incorretos").show().addClass('alert alert-error').show();
                 },500);
                 flag = false;
            }else {
            	setTimeout(function(){
                    $('#setSiteLoading').hide();
                    $("#elexSiteTips").html("Usuário Erro de servidor Conectar").show().addClass('alert alert-error').show();
                },500);
                flag = false;
            }
            $('#login_loading').hide();
        });

    });

    function getRedirectUrl() {
        var referrer = location.hash ? decodeURIComponent(location.hash.substr(1)) : '';
        if(referrer == '' || !/^https?\:\/\/\w*\.337\.com/g.test(referrer)) {
            referrer = location.href.split("#")[0];
        }
        return referrer;
    }

    $('#loginform input').keypress(function(event) {
        if (event.which == '13') {
            $('#login_btn').click();
        }
    });

    $('.oauth-button').click(function($em){
		var type = $(this).data('type');
        var url = "/oauth/"+type;
        var w = 600;
        var h = 400;
        var t = (screen.height - h) / 2;
        var l = (screen.width - w) / 2; 
    	window.open (url, type + 'Connect', 'height='+h+', width='+w+', top='+t+', left='+l+', toolbar=no, menubar=no, scrollbars=no, resizable=no,location=n o, status=no');
    	window.oauthCallback = function(data){
    		if(data.status == 0) {
				elex.user._afterLogin(data);
              	//获取返回页地址
                var referrer = location.hash ? decodeURIComponent(location.hash.substr(1)) : '';
                if(referrer == '' || !/^https?\:\/\/\w*\.337\.com/g.test(referrer)) {
                	window.location.reload(true);
                }else{
                	window.location.href = referrer;
                }
			}else if(data.status < 0) {
				if (data.status == -6) {
					$("#elexSiteTips").removeAttr('class').html('email_be_used').addClass('elexTips alert alert-error').show();
                } else {
                    $("#elexSiteTips").removeAttr('class').html('username_or_password_error').addClass('elexTips alert alert-error').show();
                }
    		}else{
            	$("#elexSiteTips").removeAttr('class').html('Usuário Erro de servidor Conectar').addClass('elexTips alert alert-error').show();
           	}
    	};
    });
</script>

<!-- Google Code for opiece-tr 337 homepage -->
<script type="text/javascript">
/* <![CDATA[ */
var google_conversion_id = 1013369298;
var google_conversion_label = "zP-DCJah8AQQ0pOb4wM";
var google_custom_params = window.google_tag_params;
var google_remarketing_only = true;
/* ]]> */
</script>
<script type="text/javascript" src="../../../../www.googleadservices.com/pagead/conversion.js" tppabs="http://www.googleadservices.com/pagead/conversion.js">
</script>
<noscript>
<div style="display:inline;">
<img height="1" width="1" style="border-style:none;" alt="" src="../../../../www.google.com.br/ads/user-lists/1013369298/-label=zP-DCJah8AQQ0pOb4wM&script=0&random=2357285774&ipr=y.gif" tppabs="http://googleads.g.doubleclick.net/pagead/viewthroughconversion/1013369298/?value=0&label=zP-DCJah8AQQ0pOb4wM&guid=ON&script=0"/>
</div>
</noscript>

<!-- Google Code for xlhc-br remarketing 337 -->
<script type="text/javascript">
/* <![CDATA[ */
var google_conversion_id = 1014774723;
var google_conversion_label = "4uodCIWC1AUQw_fw4wM";
var google_custom_params = window.google_tag_params;
var google_remarketing_only = true;
/* ]]> */
</script>
<script type="text/javascript" src="../../../../www.googleadservices.com/pagead/conversion.js" tppabs="http://www.googleadservices.com/pagead/conversion.js">
</script>
<noscript>
<div style="display:inline;">
<img height="1" width="1" style="border-style:none;" alt="" src="../../../../www.google.com.br/ads/user-lists/1014774723/-label=4uodCIWC1AUQw_fw4wM&script=0&random=132479384&ipr=y.gif" tppabs="http://googleads.g.doubleclick.net/pagead/viewthroughconversion/1014774723/?value=0&label=4uodCIWC1AUQw_fw4wM&guid=ON&script=0"/>
</div>
</noscript>

<!-- Google Code for tr-xlhc homepage -->
<script type="text/javascript">
/* <![CDATA[ */
var google_conversion_id = 996888963;
var google_conversion_label = "oSv9CO248wUQg6Ot2wM";
var google_custom_params = window.google_tag_params;
var google_remarketing_only = true;
/* ]]> */
</script>
<script type="text/javascript" src="../../../../www.googleadservices.com/pagead/conversion.js" tppabs="http://www.googleadservices.com/pagead/conversion.js"></script>
<noscript>
<div style="display:inline;">
<img height="1" width="1" style="border-style:none;" alt="" src="../../../../www.google.com.br/ads/user-lists/996888963/-label=oSv9CO248wUQg6Ot2wM&script=0&random=4105543456&ipr=y.gif" tppabs="http://googleads.g.doubleclick.net/pagead/viewthroughconversion/996888963/?value=0&label=oSv9CO248wUQg6Ot2wM&guid=ON&script=0"/>
</div>
</noscript>

<!-- Google Code for skydragon homepage -->
<script type="text/javascript">
/* <![CDATA[ */
var google_conversion_id = 964403695;
var google_conversion_label = "F_EBCMnDuwUQ78PuywM";
var google_custom_params = window.google_tag_params;
var google_remarketing_only = true;
/* ]]> */
</script>
<script type="text/javascript" src="../../../../www.googleadservices.com/pagead/conversion.js" tppabs="http://www.googleadservices.com/pagead/conversion.js">
</script>
<noscript>
<div style="display:inline;">
<img height="1" width="1" style="border-style:none;" alt="" src="../../../../www.google.com.br/ads/user-lists/964403695/-label=F_EBCMnDuwUQ78PuywM&script=0&random=3849619994&ipr=y.gif" tppabs="http://googleads.g.doubleclick.net/pagead/viewthroughconversion/964403695/?value=0&label=F_EBCMnDuwUQ78PuywM&guid=ON&script=0"/>
</div>
</noscript>

<!-- Google Code for BA 337 homepage -->
<script type="text/javascript">
/* <![CDATA[ */
var google_conversion_id = 1004391334;
var google_conversion_label = "dm7xCKKL0AcQppf33gM";
var google_custom_params = window.google_tag_params;
var google_remarketing_only = true;
/* ]]> */
</script>
<script type="text/javascript" src="../../../../www.googleadservices.com/pagead/conversion.js" tppabs="http://www.googleadservices.com/pagead/conversion.js">
</script>
<noscript>
<div style="display:inline;">
<img height="1" width="1" style="border-style:none;" alt="" src="../../../../www.google.com.br/ads/user-lists/1004391334/-label=dm7xCKKL0AcQppf33gM&script=0&random=2231061853&ipr=y.gif" tppabs="http://googleads.g.doubleclick.net/pagead/viewthroughconversion/1004391334/?value=0&label=dm7xCKKL0AcQppf33gM&guid=ON&script=0"/>
</div>
</noscript>

<!-- Google Code for xlhc-tw 337homepage -->
<script type="text/javascript">
/* <![CDATA[ */
var google_conversion_id = 977241166;
var google_conversion_label = "7jjlCKKlqQUQzoj-0QM";
var google_custom_params = window.google_tag_params;
var google_remarketing_only = true;
/* ]]> */
</script>
<script type="text/javascript" src="../../../../www.googleadservices.com/pagead/conversion.js" tppabs="http://www.googleadservices.com/pagead/conversion.js">
</script>
<noscript>
<div style="display:inline;">
<img height="1" width="1" style="border-style:none;" alt="" src="../../../../www.google.com.br/ads/user-lists/977241166/-label=7jjlCKKlqQUQzoj-0QM&script=0&random=704572044&ipr=y.gif" tppabs="http://googleads.g.doubleclick.net/pagead/viewthroughconversion/977241166/?value=0&label=7jjlCKKlqQUQzoj-0QM&guid=ON&script=0"/>
</div>
</noscript>

<!-- Google Code for butian 337homepage remarketing -->
<script type="text/javascript">
/* <![CDATA[ */
var google_conversion_id = 960556027;
var google_conversion_label = "cPPMCIXzrAYQ-9eDygM";
var google_custom_params = window.google_tag_params;
var google_remarketing_only = true;
/* ]]> */
</script>
<script type="text/javascript" src="../../../../www.googleadservices.com/pagead/conversion.js" tppabs="http://www.googleadservices.com/pagead/conversion.js">
</script>
<noscript>
<div style="display:inline;">
<img height="1" width="1" style="border-style:none;" alt="" src="../../../../www.google.com.br/ads/user-lists/960556027/-label=cPPMCIXzrAYQ-9eDygM&script=0&random=2799526554&ipr=y.gif" tppabs="http://googleads.g.doubleclick.net/pagead/viewthroughconversion/960556027/?value=0&label=cPPMCIXzrAYQ-9eDygM&guid=ON&script=0"/>
</div>
</noscript>

<!-- Google Code for rmkt_warfare_sa_337homepage -->
<script type="text/javascript">
/* <![CDATA[ */
var google_conversion_id = 1013369298;
var google_conversion_label = "QRc-CPa2xAUQ0pOb4wM";
var google_custom_params = window.google_tag_params;
var google_remarketing_only = true;
/* ]]> */
</script>
<script type="text/javascript" src="../../../../www.googleadservices.com/pagead/conversion.js" tppabs="http://www.googleadservices.com/pagead/conversion.js">
</script>
<noscript>
<div style="display:inline;">
<img height="1" width="1" style="border-style:none;" alt="" src="../../../../www.google.com.br/ads/user-lists/1013369298/-label=QRc-CPa2xAUQ0pOb4wM&script=0&random=2897254772&ipr=y.gif" tppabs="http://googleads.g.doubleclick.net/pagead/viewthroughconversion/1013369298/?value=0&label=QRc-CPa2xAUQ0pOb4wM&guid=ON&script=0"/>
</div>
</noscript>


</body>
</html>