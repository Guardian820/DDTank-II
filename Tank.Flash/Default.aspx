<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Tank.Flash._Default" %>


<html>

<head id="Head1" runat="server">
<title>SuperTank</title>
<meta name="ddtank 337, DDTank Gamit, ddtank pirata, ddtank chines, ddtank wind, ddtank pop, ddtank nova era, ddtank ice, ddtank fire, ddtank br, ddtank brasil, ddtank usa, jogo online, online, easy, hard, ddtank on, ddtank" />
<meta name="description" content="DDTank Wind as rodadas são baseadas em luta de tiros entre os jogadores com uma variedade de armas e equipamentos.Junte-se aos 15 milhões de jogadores em todo o mundo!" />

	<script type="text/javascript" src="script/jquery.js"></script>
    	<script type="text/javascript" src="script/dandantang.js"></script>
    	<script type="text/javascript" src="script/rightClick.js"></script>
    	<script type="text/javascript" src="script/swfobject.js"></script>
    	<script type="text/javascript" src="script/isSafeFlash.js"></script>
		
<style type="text/css"> 
    html, body	{ height:100%; }
      body
        {
            margin: 0px auto;
            padding: 0px;
            background-image: url(images/bgdefault.jpg);
	     background-repeat: no-repeat;
        background-position: center center top;
		
		
       
    </style>
	
	<style>
#fblikepop {background-color: #fff;display: none;    position: fixed;    top: 200px;    _position: absolute;     width: 450px;    border: 10px solid #6F6F6F;    z-index: 200;-moz-border-radius: 9px;    -webkit-border-radius: 9px;    margin: 0pt;    padding: 0pt;    color: #333333;    text-align: left;font-family: arial,sans-serif;    font-size: 13px;} 
#fblikepop body {background: #fff none repeat scroll 0%;    line-height: 1;    margin: 0pt;    height: 100%;} 
.fbflush {cursor: pointer;    font-size: 11px !important;    color: #FFF !important;    text-decoration: none !important;    border: 0 !important;} #fblikebg {display: none;    position: fixed;_position: absolute;  height: 100%;    width: 100%;    top: 0;    left: 0;    background: #000000;    z-index: 100;} 
#fblikepop #closeable {color: #333;float: right;    margin: 7px 0 0 0;} 
#fblikepop h1 {    background: #6D84B4 none repeat scroll 0 0;    border-top: 1px solid #3B5998;    border-left: 1px solid #3B5998;    border-right: 1px solid #3B5998;    color: #FFFFFF !important;    font-size: 14px !important;    font-weight: normal !important;    padding: 5px !important;    margin: 0 !important;font-family: "Lucida Sans Unicode", "Lucida Grande", sans-serif !important;} 
#fblikepop #actionHolder { height: 60px;    overflow: hidden;}
#fblikepop #buttonArea { background: #F2F2F2;    border-top: 1px solid #CCCCCC;    padding: 10px;    min-height: 50px;} 
#fblikepop #buttonArea a { color: #999999 !important;    text-decoration: none !important;    border: 0 !important;    font-size: 10px !important;} #fblikepop #buttonArea a:hover {    color: #333 !important;    text-decoration: none !important; border: 0 !important;} 
#fblikepop #popupMessage {font-size: 12px !important;font-weight: normal !important;    line-height: 22px;    padding: 8px;    background: #fff !important;} 
#fblikepop #counter-display {float: right;    font-size: 11px !important;font-weight: normal !important;    margin: 5px 0 0 0;text-align: right;line-height: 16px;}

	.footerBg-box {
    margin-top: 0px;
    width: 100%;
    height: 188px;
    background:url(../images/footerBg.jpg) no-repeat scroll center top transparent;
}
</style> 
<script src="http://www.google.com/jsapi"></script>
<script>google.load("jquery", "1");</script> 
<script type="text/javascript" src="http://connect.facebook.net/pt_BR/all.js#xfbml=1"></script> 
<script type="text/javascript">
 //<![CDATA[   
  kakinetworkdotcom01username="SupertankOficial",
  kakinetworkdotcom01title="Curta a Pagina para Poder Jogar",
  kakinetworkdotcom01time="30",
  kakinetworkdotcom01wait="0",
  kakinetworkdotcom01lang="br"
 //]]>  
 </script> 
 <script type="text/javascript" src="http://www.flipgames.com.br/site/js/popupFBlikebox.js"></script>
 <script type="text/javascript">
   //<![CDATA[ 
  $(document).ready(function(){$().kakinetworkdotcom({ closeable: false });});  
   //]]>
  </script>
   
</head>
<body scroll="no">
<table width="100%" height="70%" border="0" cellspacing="0" cellpadding="0">
        <tr>
            <td valign="middle">
                <table border="0" align="center" cellpadding="0" cellspacing="0">
                    <tr>
                        <td align="center">
<table width="1240" border="0" align="center" cellpadding="0" cellspacing="0">
  <!--DWLayoutTable-->
  <tr> 
    <td height="0px" colspan="5" valign="top"><table width="100%" border="0" cellpadding="0" cellspacing="0">
        <!--DWLayoutTable-->
        <tr> 
          <td width="1240" height="0px" valign="top"><div align="center">
                          </div></td>
        </tr>
      </table></td>
  </tr>
  <tr> 
    <td width="160" height="600" valign="top"><table width="160" border="0" cellpadding="0" cellspacing="0">
                    <!--DWLayoutTable-->
                    <tr> 
                      <td width="160" height="600" align="center" valign="middle">
</td>
        </tr>
      </table></td>
    <td colspan="3" valign="top"><table width="100" border="0" cellpadding="0" cellspacing="0">
        <!--DWLayoutTable-->
        <tr> 
                        <td align="center">
                            <object classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000" id="7road-ddt-game"
                                codebase="http://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=8,0,0,0"
                                <name="Main" width="1000" height="600" align="middle" id="Main">
                                <param name="allowScriptAccess" value="always" />
                                <param name="movie" value="<%=Flash %>Loading.swf?<%=Content %>&config=<%=Config %>" />
                                <param name="quality" value="high" />
                                <param name="menu" value="false">
                                <param name="bgcolor" value="#000000" />
                                <param name="FlashVars" value="<%=AutoParam %>" />
                                <param name="allowScriptAccess" value="always" />
                                <embed flashvars="<%=AutoParam %>" src="<%=Flash %>Loading.swf?<%=Content %>&config=<%=Config %>"
                                    width="1000" height="600" align="middle" quality="high" name="Main" allowscriptaccess="always"
                                    type="application/x-shockwave-flash" pluginspage="http://www.macromedia.com/go/getflashplayer" />
                            </object>
                        </td>
        </tr>
      </table></td>
</body>
</html>
