/*
 * 337.com常用操作
 * author: laihansheng<cn0819@gmail.com>
 */

elex.namespace('elex.web.common');

//337 游戏历史模块
elex.web.common.getUserGameHistory = function(cookieGameHistory) {
	var GET_GAME_HISTORY_URL = elex.config.get('wikiUrl') + "/api/?get_history";
	var inIndex = false;
	if ($('#hasChildrenOne').length > 0) {
		inIndex = true;
	}
	
	var data = null;
	if (elex.config.get('uid') != '0') {
		data = { uid : elex.config.get('uid'), nums : 7 };
	} else {
		var cookieGameHistory = elex.cookie.get('337_game_history');
		if (!cookieGameHistory) {
			$('#userGameHistory .noNotes').html(elex.langs.get('no_game_history'));
			if (inIndex == true) {
				$('#noCookie').show();
			}
			return;
		}  else {
			data = { gkeys : cookieGameHistory, nums : 7  };
		}
	}
	elex.request.get(GET_GAME_HISTORY_URL, data, function(response) {
		//elex.log(response instanceof Array);
		if (response.length > 0) {
			var indexHtml = '';
			var html = '';
			var i = 0;
			$.each(response, function(index, rs) {
				if (rs.icon3 == '') rs.icon3 = '../../../../p.img.elex-tech.us/337/theme/img/index/logo.png'/*tpa=http://p.img.elex-tech.us/337/theme/img/index/logo.png*/;
				html += '<div class="inList">';
	            html += '    <div class="inImg"><a href="' + rs.url + '"><img src="' + rs.icon3 + '" alt="' + rs.name + '" /></a></div>';
	            html += '    <div class="inText"><h3><a href="' + rs.url + '">' + rs.name_fix + '</a></h3></div>';
	            html += '</div>';
	            
				if (inIndex == true && i < 2) {
		            indexHtml += '<div class="inList">';
		            indexHtml += '    <div class="inImg">';
		            indexHtml += '        <p><a href="' + rs.url + '"><img src="' + rs.icon3 + '" /></a></p>';
		            indexHtml += '    </div>';
		            indexHtml += '    <div class="inText">';
		            indexHtml += '        <h2><a href="' + rs.url + '">' + rs.name_fix + '</a></h2>';
		            indexHtml += '        <p>' + rs.context_fix + '</p>';
		            indexHtml += '        <p><a href="' + rs.url + '">' + elex.langs.get('play_now') + '</a></p>';
		            indexHtml += '    </div>';
		            indexHtml += '    <div class="clear"></div>';
		            indexHtml += '</div>';
	            }
	            i++;
	
			});
			if (response.length > 0) {
				if (response.length >= 7) {
					html += '<div class="inMore"><a href="' + elex.config.get('baseUrl') + '/game/recentplays/">' + elex.langs.get('view_more') + ' »</a></div>';
				}
				$('#userGameHistory .elexRightContWrap').html(html);
				if (inIndex == true) {
					$('#hasChildrenOne').html(indexHtml);
					$('#hasCookie').show();
				}
			} else {
				$('#userGameHistory .noNotes').html(elex.langs.get('no_game_history'));
				if (inIndex == true) {
					$('#noCookie').show();
				}
			}
		} else {
			$('#userGameHistory .noNotes').html(elex.langs.get('no_game_history'));
			if (inIndex == true) {
				$('#noCookie').show();
			}
		}	
	});
}

//facebook login
elex.web.common.facebookLogin = function() {
	if(typeof(FB) == 'undefined') {
		setTimeout(elex.web.common.facebookLogin, 1000);
		return;
	}
	FB.login(function(response) {
		if (response.authResponse) {
			FB.api('/me', function(response){
				elex.user.facebookLogin(response.name, response.id, response.email, function(ret){
					if(ret.status < 0) {
						if (ret.status == -6) {
							alert(elex.langs.get('email_be_used'));
						} else {
							alert(elex.langs.get('username_or_password_error'));
						}
					}
				});
			});
		} else {
			//cancel
		}
    }, {scope: 'email'});
	return false;
}

/*
 * 初始化用户登录框
 */
elex.web.common.initUserLogin = function(){
	//注册登录后的事件
	elex.user.addLoginListener(function(ret){
		$('#inUserLogin').hide();
		$('#headUserName').html(ret.nickname ? ret.nickname : ret.username);
		if(ret.icon && $('#headUserImg').length > 0 ){
			$('#headUserImg').attr('src' , ret.icon); 
		}
		$('#inUserInfo').show();
	});
	//注册退出后的事件
	elex.user.addLogoutListener(function(){
		window.location.href = window.location.href;
	});
	
	
	//登录
	$('#sign_post').click(function(){
		var thisBtn = $(this).hide();
		var obj = $(this).siblings('.loading').children().show();
		$(obj).show();
		var username = $("#head_login_username").val();
		var password = $("#head_login_password").val();
		var remember = document.getElementById("cktime").checked;
		elex.util.loading(
			function(){
				elex.user.login(username, password, remember, function(ret) {
					if(ret.status < 0) {
						$(obj).hide();
						if (ret.status == -6) {
							alert(elex.langs.get('email_be_used'));
						} else {
							alert(elex.langs.get('username_or_password_error'));
						}
						$(thisBtn).show();
					}
				});
			},
			2000
		);
		
	});
	
	//退出
	$('#elexlogout').click(function(){
		elex.user.logout();
	});
	
	//facebook connect
	window.fbAsyncInit = function() {
		FB.init({appId: '220782057940018', status: true, cookie: true, xfbml: true, oauth: true});
	};
	(function() {
	    var e = document.createElement('script');
		e.async = true;
		e.src = document.location.protocol +'//connect.facebook.net/en_US/all.js';
		document.getElementById('fb-root').appendChild(e);
	}());
};

/*
 * 初始化用户语言
 */
elex.web.common.initLanguage = function() {
	$('.set_lang_btn').click(function(){
		var url = elex.config.get('newUrl') + '/common/setlanguage';
		elex.dialog.show(url , { title : elex.langs.get('select_your_language') , id : 'setlang_id'});
	});
}

/*
 * 初始化用户游戏历史
 */
elex.web.common.initGameHistory =  function() {
	$("#inHistory").hover(
		function(){ 
			$('#userGameHistory').show();
		}, 
		function(){
			$('#userGameHistory').hide();
		} 
	);
	//显示历史
	elex.web.common.getUserGameHistory();
}

elex.namespace('elex.common');

/***获取页面快捷方式**/
elex.common.getPageShortCut = function(title){
    if(typeof(title) == 'undefined' || title.length == 0)
    {
        title = document.title;
    }
    var sc_url = elex.config.get('baseUrl') + '/ajax/get_short_cut?title='+title;
    //elex.log(sc_url);
    window.location.href = sc_url;
}
elex.common.addBookMark = function(b_title,cut_src){
    if(typeof(b_title) == 'undefined' || b_title.length == 0)
    {
        b_title = document.title;
    }
	var b_url = window.location.href;
	//去掉src
	if(cut_src == 1)
	{
		b_url = b_url.replace(/(\?src=[^&]*&)/g,'?');
		b_url = b_url.replace(/(\?src=[^&]*)/g,'');
		b_url = b_url.replace(/(&src=[^&]*)/g,'');
	}
	var ctrl = (navigator.userAgent.toLowerCase()).indexOf('mac') != -1 ? 'Command/Cmd': 'CTRL';
	var msg = elex.config.get('337_favorite_charme_tips');
	var temp = msg.replace('(0)',' <b>' + ctrl + ' + D</b> ');
    //elex.log(msg);
	if(document.all){
		window.external.addFavorite(b_url , b_title);
	}else if(window.sidebar){
		window.sidebar.addPanel(b_title , b_url,"");
	}else{
		elex.dialog.alert('<div class="elexDialogTips">'+ temp +'</div>',{title:'http://337.eleximg.com/337/v3static/js/337.COM',time : 5000 , showClose:true});
	}
}
/*
 * 页面载入执行
 */
$(document).ready(function(){
	//初始化dialog
	elex.widgets.Dialog.setConfig({
		initWidth : 300,
		initHeight : 200,
		closeText : elex.langs.get('close') + ' ×'
	});
	
	//初始化相关函数
	elex.web.common.initUserLogin();
	elex.web.common.initLanguage();
});
