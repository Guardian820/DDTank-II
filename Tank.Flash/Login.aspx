<!DOCTYPE html>
<html lang="pt">
<head>
    <meta charset="utf-8">
    
	<title>User Center - Crazy Jogos</title>

    <meta name="keywords" content="ddtank dd tank ddtank 2 337 ddtanque ddtank 337 d d tank 337 ddtank login ddtank dd tank jogos ddtank jogos online jogar dd tank ddtanks dd - tank ddtank pirata online dd tank 2 ddtank jogo ddtank dd tank 337 jogos online ddtank337 jogos ddtank online ddtak dd tank jogo 337 dd tank jogar jogar ddtank jogo dd-tank jogos ddtank ddtank - 337 337 jogos dd tank" />
	<meta name="description" content="As rodadas são baseadas em luta de tiros entre os jogadores com uma variedade de armas e equipamentos. Junte-se aos 15 milhões de jogadores em todo o mundo! " />
    <link rel="shortcut icon" href="http://img13.elex-tech.org/1/2013/0320/c9/1/831296/Logar.jpg">
	<link href="css/337/v3static/css/elexGlobal.css-1305091304.css" tppabs="http://337.eleximg.com/337/v3static/css/elexGlobal.css?1305091304" rel="stylesheet" type="text/css"/>
    <link href="css/337/v3static/css/library_hot5.css" tppabs="http://337.eleximg.com/337/v3static/css/library_hot5.css" rel="stylesheet" type="text/css" />
    <link href="css/337/v3static/css/elexDialog.css-1307081929.css" tppabs="http://337.eleximg.com/337/v3static/css/elexDialog.css?1307081929" rel="stylesheet" type="text/css"/>
    <link href="css/Box_event_4.css" rel="stylesheet" type="text/css" />
<link href="css/thickbox.css" rel="stylesheet" type="text/css" />
<script type="text/javascript" src="script/mainsite.js"></script>
<script type="text/javascript" src="script/gunny-index.js"></script>

<script src="script/thickbox.js" type="text/javascript"></script>

<script language="javascript"  type="text/javascript">
    $(document).ready(function () {
        $("#u").focus();
        $("#p").keypress(function (e) {
            if (e.keyCode == 13) {
                //AjaxGet();
                //document.frmLogin.submit();
                //xmlhttpPost("createLogin.ashx");
                checklogin();
            }
        });        
    });
    function checklogin() {
        var err = 0;
        if (document.getElementById("u").value == '') {
            //alert('Insira um usuário');
            $("#Question").html('   <span style=" color: red; font-family: Tahoma, Lucida Grande, Verdana, Arial, sans-serif; font-size: 12px; line-height: 40px;">Por favor, insira um usuário!&nbsp;</span>');
            err++;
        }
        else if (document.getElementById("p").value == '') {
            //alert('Insira a senha');
            $("#Question").html('   <span style=" color: red; font-family: Tahoma, Lucida Grande, Verdana, Arial, sans-serif; font-size: 12px; line-height: 40px;">Por favor, insira uma senha!&nbsp;</span');
            err++;
        }

        if (err == 0)
        //document.frmLogin.submit();
            xmlhttpPost("createLogin.ashx");
        //return true;
        return false;
    }
	

    function xmlhttpPost(strURL) {
        var xmlHttpReq = false;
        var self = this;
        $("#Question").html('<img alt="" id="loading_img1" src="http://337.eleximg.com/337/v3static/img/loading.gif?" />');

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
        var form = document.forms['frmLogin'];
        var username = form.u.value;
        var password = form.p.value;
        qstr = 'username=' + escape(username) + '&password=' + escape(password);
        return qstr;
    }

    function updatepage(str) {
        if (str == "ok") {
            location.replace("LoginGame.aspx")
            //alert(str);
        }
        else
            $("#Question").html(str);
    }     
    </script>
	<script src="css/337/v3static/js/jquery-1.9.0.min.js" tppabs="http://337.eleximg.com/337/v3static/js/jquery-1.9.0.min.js"></script>
    <style>
	    body {
	        background: url("img27/1/2013/0717/88/a/871373/Logar.jpg")/*tpa=http://img27.elex-tech.org/1/2013/0717/88/a/871373/Logar.jpg*/ no-repeat scroll center 45px #FFFFFF;
	        color: #333333;
	    }
		.flash,.iframe{background: #333; border-bottom: #CCC 1px solid; width:100%; height:100%; font-size:12px; padding-top:5px; position:absolute;z-index:1000;}
		.close{color: #0CF; text-align:center;}
		.close a{cursor:pointer;text-decoration: none;}
	    .run-flash{width:100%;height:100%}
	</style>
	<script>
	//add google analytics
    (function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){
    	  (i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),
    	  m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)
    	  })(window,document,'script','../../../www.google-analytics.com/analytics.js'/*tpa=http://www.google-analytics.com/analytics.js*/,'ga');

    ga('create', 'UA-39391247-1', 'http://web.337.com/pt/ddtank/337.com');
    ga('send', 'pageview');
    </script>
</head>
<body>
	
	<!-- 加载flash -->
		<div id="main-except" >
		<div id="elexHeaderNew" class="elexHeaderNew elexHeaderNew1">
    <div class="elexHeaderNewWrap elexHeaderNewWrap1">
        <div class="headerCenter">
            <div class="inLogo">
                <a title="Início" href="../../../" tppabs="http://web.337.com/pt/">
                    <h1>Jogos<br>Centro</h1>
                </a>
            </div>
            <div class="inLogin">
                <div class="inUserInfo" id="inUserInfo" style="display:none;">
                    <ul>
                        <li id="inSelect">
                            <a href="javascript:void(0)"><i class="icoDownW"></i></a>
                            <div style="display:none;" class="inMenuCont">
                                <div class="inList"><a href="adrielisachttp://web.337.com/pt/user/profile/  user/profile/%27" tppabs="http://web.337.com/pt/user/profile/">Coisas pessoais</a></div>
                                <div class="inList"><a href="adrielisachttp://web.337.com/pt/pay/game/  pay/game/%27" tppabs="http://web.337.com/pt/pay/game/">Pagamentos</a></div>
                                
                                <div class="inList"><a href="adrielisachttp://web.337.com/pt/user/edit/  user/edit/%27" tppabs="http://web.337.com/pt/user/edit/">Configurações de conta</a></div>
                                <div id="elexlogout" class="inList"><a href="javascript:void(0);">Sair</a></div>
                                <div class="inLine"></div>
                            </div>
                        </li>
                        <li id="inImg">
                            <a href="adrielisachttp://web.337.com/pt/user/profile/  user/profile/%27" tppabs="http://web.337.com/pt/user/profile/">
                                                                <img src=" ../../../img.elex-tech.org/10000/2011/1025/f4/c/8312/100x100x75x0.jpg" tppabs="http://img.elex-tech.org/10000/2011/1025/f4/c/8312/100x100x75x0.jpg" id="headUserImg">
                                                                <span id="headUserName"></span>
                                                            </a>
                        </li>
                    </ul>
                </div>
                <div id="inUserLogin">
                    <a onclick="elex.util.login2();return false" href="./user/">Fazer login</a>
                    <a onclick="elex.util.login2();return false" href="./user/Default.aspx">Registrar</a>
					<a onclick="elex.util.login2();return false" href="./">INÍCIO</a>
                </div>
            </div>
            <div class="innav innav1">
                <ul>
                	
                    <li class="vip ">
                        <a href="adrielisachttp://web.337.com/pt/vip/  vip/%27" tppabs="">
                            <span id="vip" class="nav"></span>
                            <span>Entrar Na Conta</span>
                        </a>
                    </li>
                    
                </ul>
            </div>
        </div>
    </div>
</div>									<link rel="stylesheet" href="css/337/v3static/css/_site_signal_x.css-1308071940.css" tppabs="http://337.eleximg.com/337/v3static/css/_site_signal_x.css?1308071940" type="text/css"/>
<link rel="stylesheet" href="css/337/v3static/css/third_login.css-1308221829.css" tppabs="http://337.eleximg.com/337/v3static/css/third_login.css?1308221829" type="text/css"/>

<!-- 加载mCustomScrollbar -->
<link rel="stylesheet" href="css/337/v3static/css/jquery.mCustomScrollbar.css" tppabs="http://337.eleximg.com/337/v3static/css/jquery.mCustomScrollbar.css" type="text/css"/>
<script src="css/337/v3static/js/jquery.mCustomScrollbar.concat.min.js" tppabs="http://337.eleximg.com/337/v3static/js/jquery.mCustomScrollbar.concat.min.js"></script>

<!-- 加载SmallLider -->
<link rel="stylesheet" href="css/337/v3static/css/smallslider.css" tppabs="http://337.eleximg.com/337/v3static/css/smallslider.css" type="text/css"/>
<script src="css/337/v3static/js/jquery.smallslider.js" tppabs="http://337.eleximg.com/337/v3static/js/jquery.smallslider.js"></script>

<!-- 加载 Fancybox -->
<script src="css/337/v3static/js/jquery.fancybox.pack.js" tppabs="http://337.eleximg.com/337/v3static/js/jquery.fancybox.pack.js"></script>
<link rel="stylesheet" href="css/337/v3static/js/fancybox/jquery.fancybox.css" tppabs="http://337.eleximg.com/337/v3static/js/fancybox/jquery.fancybox.css" type="text/css"/>

<script src="css/337/v3static/js/flowplayer/flowplayer.min.js" tppabs="http://337.eleximg.com/337/v3static/js/flowplayer/flowplayer.min.js"></script>
<link rel="stylesheet" type="text/css" href="css/337/v3static/js/flowplayer/skin/minimalist.css" tppabs="http://337.eleximg.com/337/v3static/js/flowplayer/skin/minimalist.css">



<script type="text/javascript">
$(document).ready(function(){
	$('#flashbox').smallslider({
    	onImageStop:true,
        switchEffect:'ease',switchEase: 'easeOutSine',
        switchPath: 'left',switchMode: 'hover',
        showText:true,textSwitch:2
    });
    $(".site_index_sev_all_div").mCustomScrollbar({
          theme:"light"
    });
	
	var annouce_page = 0;
	var announce_num = $('.annouce_content').find('a').length;
	function annouceInterVal()
	{
	    interval_process_1 = setInterval(function(){
	                annouce_page++;
	                if(annouce_page >= announce_num)
	                {
	                    $('.annouce_content').animate({'top':(0-22)*annouce_page+11},250);
	                    setTimeout(function(){
	                        $('.annouce_content').css('top',11);
	                        $('.annouce_content').animate({'top':0},200);
	                    },300);

	                    annouce_page = 0;
	                }
	                else
	                {
	                    $('.annouce_content').animate({'top':(0-22)*annouce_page},500);
	                }
	            }
	            ,3000);
	}
	annouceInterVal();
	
});

function makeScrollable(wrapper, scrollable){

	var wrapper = $(wrapper), scrollable = $(scrollable);
	
	// 隐藏图片，直到他们不会被加载
	scrollable.hide();
	var loading = $('<div class="loading">Loading...</div>').appendTo(wrapper);
	
	// 设置功能将检查所有图像都加载
	var interval = setInterval(function(){
		var images = scrollable.find('img');
		var completed = 0;
		
		// 计数成功加载的图像
		images.each(function(){
			if (this.complete) completed++;	
		});
		
		if (completed == images.length){
			clearInterval(interval);
			setTimeout(function(){
				
				loading.hide();
				// Remove scrollbars	
				wrapper.css({overflow: 'hidden'});						
				
				scrollable.slideDown('slow', function(){
					enable();	
				});					
			}, 1000);	
		}
	}, 100);
	
	function enable(){
		var inactiveMargin = 99;					
		var wrapperWidth = wrapper.width();
		var wrapperHeight = wrapper.height();
		var scrollableHeight = scrollable.outerHeight() + 2*inactiveMargin;

		wrapper.mousemove(function(e){
			var wrapperOffset = wrapper.offset();
			// 滚动菜单
			var top = (e.pageY -  wrapperOffset.top) * (scrollableHeight - wrapperHeight) / wrapperHeight - inactiveMargin;
			if (top < 0){
				top = 0;
			}			
			wrapper.scrollTop(top);
		});
	}
}
	
$(function(){	
	makeScrollable("div.sc_menu_wrapper", "div.sc_menu");
});
</script>
	
<div class="site_index_bg">
	     <div class="site_index_function">
        <div class="site_index_sev_list">
        	<div class="site_index_sev_list_content">
        					<div class="site_index_login">
                    <div class="site_index_login_content">
                        <form id="frmLogin" name="frmLogin" method="post" action="">
                        <div class="site_index_login_input"><label>Conta：</label>
                        	<span class="input"><input style="background-image: url(http://337.eleximg.com/0000000000000000000000000000sec_list_content.png); background-position: 0px -172px; background-repeat: no-repeat no-repeat; display: inline-block; height: 22px; vertical-align: top; width: 171px;" type="text"  name="u" id="u" value="" class="txt_box"></span>
                        </div>
                        <div class="site_index_login_input"><label>Senha：</label>
                        	<span class="input"><input style="background-image: url(http://337.eleximg.com/0000000000000000000000000000sec_list_content.png); background-position: 0px -172px; background-repeat: no-repeat no-repeat; display: inline-block; height: 22px; vertical-align: top; width: 171px;" type="password" id="p" name="p" value="" class="txt_box"></span>
                        </div>
                        <div class="Question" id="Question" style="float:none;padding-left:70px;height:30px"></div>
                        <div class="site_index_login_btn">
                          <input type="submit"  style="background-image: url(http://i.imgur.com/HUIWsje.png); border: 0px; color: white; cursor: pointer; font-size: 14px; font-weight: bold; height: 37px; margin: auto auto auto 60px;  width: 120px;" value="Fazer login"  class="ChoiNgay" onclick="return checklogin();">
                        </div></form>
                        <div class="forget_password2"><a href="adrielisachttp://web.337.com/pt/user/getpass/  user/getpass/%27" tppabs="http://web.337.com/pt/user/getpass/" target="_blank">Esqueceu a senha?</a></div>
                        
                    </div>
                </div>
                <div class="site_index_reg_link">