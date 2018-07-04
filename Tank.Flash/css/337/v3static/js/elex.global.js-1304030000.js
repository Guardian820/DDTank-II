/*
 * 337.com全局脚本，依赖于jQuery
 * author: laihansheng<cn0819@gmail.com>
 */

//elex全局变量
var elex = window.elex || {};

//存储搜索结果数据
var searchData = searchData || {};
var seachflag = false;

//命名空间
elex.namespace = function(ns) {
    if (!ns || !ns.length) {
        return null;
    }

    var levels = ns.split(".");
    var nsobj = elex;

    // elex is implied, so it is ignored if it is included
    for (var i=(levels[0] == "elex") ? 1 : 0; i<levels.length; ++i) {
        nsobj[levels[i]] = nsobj[levels[i]] || {};
        nsobj = nsobj[levels[i]];
    }

    return nsobj;
};

//扩展
elex.extend = function(target, object) {
	$.extend(target, object);
};

//调试输出
elex.log = function() {
	if (typeof(console) == "object" && typeof(console.log) == "function") {
		console.log.apply(console, arguments);
	}
};

//类创建
elex.createClass = function() {
	var c = function() {
	    if (this.initialize) this.initialize.apply(this, arguments);
	};
	c.prototype.initialize = function() {};
	return c;
};
//类继承
elex.extendClass = function(subclass, superclass) {
    var f = function() {};
    f.prototype = superclass.prototype;
    subclass.prototype = new f();
    subclass.prototype.constructor = subclass;
    subclass.superclass = superclass.prototype;
    subclass.prototype.parent = superclass.prototype;
    if (superclass.prototype.constructor == Object.prototype.constructor) {
        superclass.prototype.constructor = superclass;
    }
};

//配置操作
elex.namespace('elex.config');
//默认配置
elex.config._setting = {
	debug : false,
	uid : 0,
	baseUrl : 'http://www.337.com/',
    newUrl  : 'http://phalcon.337.com/',
	wikiUrl : 'http://www.337.com/page',
	appUrl : 'http://apps.337.com/',
    passportUrl : 'https://passport.337.com',
	cookieDomain : '.337.com',
	cookiePath : '/',
	cookieNameLang : 'elexlang',
	cookieNameUser : 'elx337_user',
	cookieIpKey : 'ipKey'
};
//读取配置
elex.config.get = function(key) {
	return elex.config._setting[key] ? elex.config._setting[key] : '';
};
//写入配置
elex.config.set = function(key, value) {
	if ($.isPlainObject(key)) {
		$.extend(elex.config._setting, key);
	} else {
		elex.config._setting[key] = value;
	}
};

//语言操作
elex.namespace('elex.langs');
elex.langs._content = {};
elex.langs.get = function(key) {
	var key = arguments[0] || '_';
	var msg = elex.langs._content[key] ? elex.langs._content[key] : key;
	for(var i=1; i<arguments.length; i++) {
		msg = msg.replace('{'+(i-1)+'}', arguments[i]);
	}
	return msg;
};
elex.langs.set = function(key, value) {
	if ($.isPlainObject(key)) {
		$.extend(elex.langs._content, key);
	} else {
		elex.langs._content[key] = value;
	}
};
elex.langs.load = function(string) {
	return string.replace(/\[lang\:(\w+)\]/ig, function(find, key){ return elex.langs.get(key); } );
};

//cookie操作
elex.namespace('elex.cookie');
//cookie的读取
elex.cookie.get = function (key, value, options) {
		options = options || {};
	    var result, decode = options.raw ? function (s) { return s; } : decodeURIComponent;
	    return (result = new RegExp('(?:^|; )' + encodeURIComponent(key) + '=([^;]*)').exec(document.cookie)) ? decode(result[1]) : null;
};
//cookie的删除
elex.cookie.remove = function (key) {
	elex.cookie.set(key, null);
};
//cookie的写入
elex.cookie.set = function (key, value, options) {

    options = jQuery.extend({
    	domain : elex.config.get('cookieDomain'),
    	path :  elex.config.get('cookiePath')
    }, options);

    if (value === null || value === undefined) {
        options.expires = -1;
    }

    if (typeof options.expires === 'number') {
        var days = options.expires, t = options.expires = new Date();
        t.setDate(t.getDate() + days);
    }

    value = String(value);

    return (document.cookie = [
        encodeURIComponent(key), '=',
        options.raw ? value : encodeURIComponent(value),
        options.expires ? '; expires=' + options.expires.toUTCString() : '', // use expires attribute, max-age is not supported by IE
        options.path ? '; path=' + options.path : '',
        options.domain ? '; domain=' + options.domain : '',
        options.secure ? '; secure' : ''
    ].join(''));
};

//请求操作
elex.namespace('elex.request');
elex.request._send = function (url, type,  _data, _successCallback, _errorCallback) {
	data = null;
	successCallback = null;
	errorCallback = null;
	if (!$.isPlainObject(_data) && $.isFunction(_data)) {
		successCallback = _data;
		errorCallback = _successCallback;
	} else {
		data = _data;
		successCallback = _successCallback;
		errorCallback = _errorCallback;
	}

	//判断是否为同域
	var host = location.protocol + '//' + location.hostname
	var crossdomain = true;
	if(url[0]=='/' || url.substr(0, host.length) == host ){
		crossdomain = false;
	};

	var options = {
		url: url,
		type: crossdomain ? 'GET' : type, //跨域则强制使用get
		data: arguments.length == 2 ? {} : data,
		timeout: 20000
	};
	if (crossdomain) {
		options['dataType'] = 'jsonp';
		options['jsonp'] = 'callback';
	} else {
		options['dataType'] = 'json';
	}
	if (successCallback) {
		options['success'] = successCallback;
	}
	if (errorCallback) {
		options['error'] = errorCallback;
	}
	jQuery.ajax(options);
};
//使用GET请求
elex.request.get = function(url, data, success, error) {
	elex.request._send(url, 'GET', data, success, error)
}
//使用POST请求
elex.request.post = function(url, data, success, error) {
	elex.request._send(url, 'POST', data, success, error)
}

//字符串操作
String.prototype.strLength = function() {
    var cArr = this.match(/[^\x00-\xff]/ig);
    return this.length + (cArr == null ? 0 : cArr.length) + "&" + cArr;
}

elex.namespace('elex.string');
elex.string.cut = function(str, len){
	len = parseInt(len);
	var temp = str.strLength().split('&')[1];
	var strLen = str.strLength().split('&')[0];
	if(temp == 'null'){
		if(strLen >= len + 2) return str.substr(0, len) + "...";
		else return str;
	}else{
		len = len/2;
		if(strLen >= len + 2) return str.substr(0, len) + "...";
		else return str;
	}
};


elex.string.trim = function(str) {
	return $.trim(str);
};

//用户模块
elex.namespace('http://337.eleximg.com/337/v3static/js/elex.user');
//获取用户信息
elex.user.info = function() {
	//...
};
//337 登录模块
elex.user.login = function(username, password, remember, complete) {
	var data = {
		username : username,
		password : password
	};

	var loginUrl = elex.config.get('baseUrl') + '/user/loginbyajax';
	elex.request.post(loginUrl, data, function(ret){
		if (ret && ret.status == 0) {
			elex.user._afterLogin(ret, remember);
		}
		complete.call(this, ret);
	}, function(){
		complete.call(this, {status : -10});
	});
}

elex.user.login2 = function(username, password, remember, complete) {
    var data = {
        username : username,
        password : password
    };

    var loginUrl = elex.config.get('newUrl') + '/user/ajaxLogin';
    elex.request.post(loginUrl, data, function(ret){
        if (ret && ret.status == 0) {
            elex.user._afterLogin(ret, remember);
        }
        complete.call(this, ret);
    }, function(){
        complete.call(this, {status : -10});
    });
}
//337 第三方登陆
elex.user.authLogin = function(username, password, remember, captcha_code, complete) {
    var data = {
		username : username,
		password : password,
        captcha_code : captcha_code
	};
    var loginUrl = elex.config.get('baseUrl') + '/api/auth_login';
	elex.request.post(loginUrl, data, function(ret){
		if (ret && ret.status == 0) {
			elex.user._afterLogin(ret, remember);
		}
		complete.call(this, ret);
	}, function(){
		complete.call(this, {status : -10});
	});
}
//falce登录模块
elex.user.facebookLogin = function(username, uid, email, complete) {
	var data = {
		username : username,
		uid : uid,
		email : email
	};
	var loginUrl = elex.config.get('baseUrl') + '/user/loginbysns';
	elex.request.post(loginUrl, data, function(ret){
		if (ret && ret.status == 0) {
			elex.user._afterLogin(ret);
		}
		complete.call(this, ret);
	}, function(){
		complete.call(this, {status : -10});
	});
}

//facebook登录模块(新)
elex.user.facebookLogin2 = function(username, uid, email, complete) {
    var data = {
        username : username,
        uid : uid,
        email : email
    };
    var loginUrl = elex.config.get('newUrl') + '/user/fbLogin';
    elex.request.post(loginUrl, data, function(ret){
        if (ret && ret.status == 0) {
            elex.user._afterLogin(ret);
        }
        complete.call(this, ret);
    }, function(){
        complete.call(this, {status : -10});
    });
}

//337退出模块
elex.user.logout = function(complete) {
	var key = elex.config.get('cookieNameUser');
	var data = {
		cookieKey : elex.cookie.get(key)
	};
	var loginUrl = elex.config.get('baseUrl') + '/user/logout';
	elex.request.post(loginUrl, data, function(ret){
		elex.user._afterLogout(ret);
		complete.call(this, {status : 0});
	}, function(){
		elex.user._afterLogout({status : -10}); //未成功也触发退出
		complete.call(this, {status : -10});
	});
}
//337注册模块
elex.user.register = function(username, password, email, inviter, captchacode,complete) {
	var data = {
		username: username,
		password:password,
		email:email,
		inviter:inviter,
		captchacode:captchacode,
		submit:1
	};
	var registerUrl = elex.config.get('baseUrl') + '/user/register';
	elex.request.post(registerUrl, data, function(ret){
		if (ret && ret.status == 0) {
			elex.user._afterLogin(ret);
		}
		complete.call(this, ret);
	}, function(){
		complete.call(this, {status : -10});
	});
}

elex.user.register2 = function(username, password, email, inviter, captchacode,complete) {
    var data = {
        username: username,
        password:password,
        email:email,
        inviter:inviter,
        captchacode:captchacode,
        submit:1
    };
    var registerUrl = elex.config.get('newUrl') + '/user/register';
    elex.request.post(registerUrl, data, function(ret){
        if (ret && ret.status == 0) {
            elex.user._afterLogin(ret);
        }
        complete.call(this, ret);
    }, function(){
        complete.call(this, {status : -10});
    });
}

//登录前后做的事
elex.user._eventCount = 0;
elex.user._afterLoginFuncs = {};
elex.user._afterLogoutFuncs = {};
//监听登录前后的操作
elex.user.addLoginListener = function(func) {
	var key = 'e' + (++elex.user._eventCount);
	elex.user._afterLoginFuncs[key] = func;
	return key;
}
elex.user.removeLoginListener = function(event) {
	if (elex.user._afterLoginFuncs[event]) {
		elex.user._afterLoginFuncs[event] = null;
	}
}
elex.user.addLogoutListener = function(func) {
	var key = 'e' + (++elex.user._eventCount);
	elex.user._afterLogoutFuncs[key] = func;
	return key;
}
elex.user.removeLogoutListener = function(func) {
	if (elex.user._afterLogoutFuncs[event]) {
		elex.user._afterLogoutFuncs[event] = null;
	}
}
elex.user._afterLogin = function(ret, remember) {
	elex.config.set('uid', ret.uid);
	elex.cookie.set(elex.config.get('cookieNameUser'), ret.loginkey, {expires : remember ?  7 : false});
	elex.cookie.set(elex.config.get('cookieIpKey'), ret.ipKey, {expires : remember ?  7 : false});
	//执行注册的事件
	for(var i in elex.user._afterLoginFuncs) {
		if ($.isFunction(elex.user._afterLoginFuncs[i])) {
			elex.user._afterLoginFuncs[i](ret);
		}
	}
};

elex.user._afterLogout = function(ret) {
	elex.cookie.remove(elex.config.get('cookieNameUser'), ret.loginkey);
	//执行注册的事件
	for(var i in elex.user._afterLogoutFuncs) {
		if ($.isFunction(elex.user._afterLogoutFuncs[i])){
			elex.user._afterLogoutFuncs[i](ret);
		}
	}
};

//其它工具
elex.namespace('http://337.eleximg.com/337/v3static/js/elex.util');
//生成随机数
elex.util.random = function(min, max){
    return Math.ceil(Math.random() * (max - min) + min)
}
//关闭窗口
elex.util.closepops = function(name){$.dialog.close(name);}
//tab click标签切换
elex.util.tab = function(titleContainer, contentContainer, overClass){
	$(titleContainer).children().click(function(){
		$(this).addClass(overClass).siblings().removeClass(overClass);
		$(contentContainer).children().hide().eq($(this).index()).show();
	});
};
//tab hover标签切换
elex.util.tabHover = function(titleContainer, contentContainer, overClass){
	$(titleContainer).children().hover(function(){
		$(this).addClass(overClass).siblings().removeClass(overClass);
		$(contentContainer).children().hide().eq($(this).index()).show();
	});
};

//显示更多
elex.util.clickHide = function(Obj , h ) {
	$(Obj).toggle(
		function() {
			$(this).parent().siblings("#hidesection").css("height","auto");
			$(this).children().eq(1).hide();
			$(this).children().eq(0).show();
		},
		function() {
			$(this).parent().siblings("#hidesection").css("height", h);
			$(this).children().eq(0).hide();
			$(this).children().eq(1).show();
		}
	);
};
//添加统计
elex.util.addAnalyze = function (act){
	var reqparam = {action:act};
	var requrl = WIKI_DOMAIN + '/page/?analyze-addanalyze';
	reqAjaxAA(requrl,function(){},reqparam,function(){});
}
//添加统计
elex.util._googleAnalyticsAccount = [];
elex.util.addGoogleAnalytics = function(account){
	elex.util._googleAnalyticsAccount.push(account);
};
elex.util.loadGoogleAnalytics = function (){
	var domain = ".337.com";
	window['_gaq'] = window['_gaq'] || [];
	for(var i=0; i<elex.util._googleAnalyticsAccount.length; i++) {
		_gaq.push(["_setAccount", elex.util._googleAnalyticsAccount[i]]);
		_gaq.push(["_setDomainName", domain]);
		_gaq.push(['_setAllowHash', false]);
		_gaq.push(['_trackPageview']);
	}
	(function() {
		var ga = document.createElement('script');
		ga.type = 'text/javascript';
		ga.async = true;
		ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www/') + '.google-analytics.com/ga.js';
		var s = document.getElementsByTagName('script')[0];
		s.parentNode.insertBefore(ga, s);
	})();
};

//重新载入
elex.util.Reload = function(obj) {
	var url = $(obj).siblings('a').attr('href');
	window.location.href = url;
}

//网站工具条
elex.util.webTools = function(obj,height){
	$(obj).removeAttr('style');
	setInterval(function(){
		if(typeof(height) != 'undefined'){
			var bottom = height + 42;
		}else{
			var bottom = 110;
		}
		var h = parseInt($(document.body).height());
		var y = parseInt($(obj).offset().top);
		var scrollHeight = $(window).height() + $(window).scrollTop() - 20;
		if((h - scrollHeight) <= bottom){
			$(obj).css({'top':h - bottom,"position":"absolute"});
		}else{
			$(obj).removeAttr('style');
		}
	},1);
}

//游戏列表页顶部导航
elex.util.gameNavigation =function(obj,height){
	var me = $(obj);
	$(obj).removeAttr('style');
	$('.elexHeaderNewWrap').css('position','absolute');
	setInterval(function(){
		var headerHeight = $('.elexHeaderNewWrap').height();
		var top = headerHeight -  $(window).scrollTop(); 
	//	elex.log(headerHeight + ':' + top);
		if(top <= height)
		{
			me.css('position','fixed').css('top',height);
		}
		if(top >= height)
		{
			me.removeAttr('style');
		}
	},1);
}

//自适应窗口大小
elex.util.autoWindow = function(obj,cssUrl){
	var default_cssUrl = elex.config.get('cdnUrl') + '/theme/css/space.css?' + elex.config.get('static_vertion');
	windowWidth = $(window).width();
	if(windowWidth <= 1200) $(obj).attr('href',cssUrl); 
	else $(obj).attr('href', default_cssUrl);
}


//失去焦点和获得焦点事件
elex.util.onfocus = function(obj , type , tips){
	if(type == 'focus' ){
		obj.style.border = "1px solid #f6c35b";
		obj.style.color = "#333333";
		if(obj.value == tips){
			obj.value = '';
			return false;
		}
	}else{
		obj.removeAttribute('style');
		if(obj.value == ''){
			obj.value = tips;
			obj.style.color = "#aaaaaa";
			return false;
		}else{
			obj.style.color = "#333333";
		}
	}
}

elex.util.login = function(callback, cancelCallback){
    var url = elex.config.get('wikiUrl') + '/?user-behind_login';
    var loginSuccess = false;
    if (typeof(callback) != 'undefined') {
        var loginCallback = 'lcb' + Math.ceil(Math.random() * (999999 - 100000) + 100000);
        window[loginCallback] = function(ret){
            callback(ret);
            window[loginCallback] = null;
            loginSuccess = true;
        }
        url += '&login_callback=' + loginCallback;
    }
    var options = {};
    if (typeof(cancelCallback) != 'undefined') {
        options['afterClose'] = function(){
            if (!loginSuccess) {
                cancelCallback();
            }
        };
    }
	elex.dialog.show({type : 'url', value : url}, options);
};

elex.util.login2 = function(callback, cancelCallback){
    var url = elex.config.get('newUrl') + '/user/behindLogin';
    var loginSuccess = false;
    if (typeof(callback) != 'undefined') {
        var loginCallback = 'lcb' + Math.ceil(Math.random() * (999999 - 100000) + 100000);
        window[loginCallback] = function(ret){
            callback(ret);
            window[loginCallback] = null;
            loginSuccess = true;
        }
        url += '&login_callback=' + loginCallback;
    }
    var options = {};
    if (typeof(cancelCallback) != 'undefined') {
        options['afterClose'] = function(){
            if (!loginSuccess) {
                cancelCallback();
            }
        };
    }
    elex.dialog.show({type : 'url', value : url}, options);
};


elex.util.register = function(callback){
	var loginCallback = 'lcb' + Math.ceil(Math.random() * (999999 - 100000) + 100000);
	window[loginCallback] = function(ret){
		callback(ret);
		window[loginCallback] = null;
	}
	var url = elex.config.get('wikiUrl') + '/?user-behind_login&type=register&login_callback=' + loginCallback;
	elex.widgets.Dialog.show({type : 'url', value : url});
};

//给列表某一项或者多项添加样式名字
elex.util.addClass = function(Obj , n , addClass){
	if(n == "first"){
		$(Obj + ":" + n).addClass(addClass);
	}else if(n == "last"){
		$(Obj + ":" + n).addClass(addClass);
	}else{
		var ItemLen = $(Obj).length;
		for(i = 1 ; i <= ItemLen ; i++){
			thisID = Obj + ":eq("+ (i - 1) +")";
			if(i%n==0){
				$(thisID).addClass(addClass);
			}
		}
	}
};

//给列表某一项或者多项添加样式
elex.util.addStyel = function(Obj , n , addStyle){
	if(n == "first"){
		$(Obj + ":" + n).attr("style", addStyle);
	}else if(n == "last"){
		$(Obj + ":" + n).attr("style",addStyle);
	}else{
		var ItemLen = $(Obj).length;
		for(i = 1 ; i <= ItemLen ; i++){
			thisID = Obj + ":eq("+ (i - 1) +")";
			if(i%n==0){
				$(thisID).attr("style",addStyle);
			}
		}
	}
};

elex.util.hideMenuBtn = function(obj){
	var button = $(obj).closest('.elxMuneClick').find(':first');
	var menu = $(obj).parent().parent().siblings().css({'top':'','bottom':''});
	var clientHeight = document.documentElement.clientHeight;
	var scrollTop = Math.max(document.documentElement.scrollTop, document.body.scrollTop);
	var direction = (button.offset().top - scrollTop) + menu.outerHeight() <= clientHeight ? 'up' : 'down';
	if(menu.is(':hidden')) {
		if (direction == 'up') {
			menu.css('top', button.outerHeight() - 1).show();
		} else {
			menu.css('bottom', button.outerHeight() + 1).show();
		}
		setTimeout(function(){
			$(document.body).one('click', function(){
				menu.hide();
			});
		}, 10);
	}
}

elex.util.overMenuBtn = function(obj){
	var button = $(obj).closest('.elxMuneHover').find(':first');
	var menu = $(obj).parent().parent().siblings();
	var over = false;
	var hideTimer = function(){
		if (!over) {
			menu.hide();
		}
	};
	$(obj).mouseover(function(){
		if (!over) {
			var clientHeight = document.documentElement.clientHeight;
			var scrollTop = Math.max(document.documentElement.scrollTop, document.body.scrollTop);
			var direction = (button.offset().top - scrollTop) + menu.outerHeight() + 20 <= clientHeight ? 'up' : 'down';
			if (direction == 'up') {
				menu.css('top', button.outerHeight() - 1).show();
				menu.find('.muneIcoImg').addClass('downL');
			} else {
				menu.css('bottom', button.outerHeight() + 1).show();
				menu.find('.muneIcoImg').addClass('topL');
			}
		}
		over = true;
	});
	$(obj).mouseout(function(){
		over = false;
		setTimeout(hideTimer, 50);
	});
	menu.mouseover(function(){
		over = true;
	});
	menu.mouseout(function(){
		over = false;
		setTimeout(hideTimer, 50);
	});
}

//卡片呈现提示
var cardcacheData = {};
elex.util.hovercard = function(obj , type , num){
	var over = false;
	var ajaxflag = false;
	var me = $(obj);
	var popid = 'elexPopup';
	var pgid = 0;
	if(type.indexOf('group') >= 0){
		pgid = type.substring(6);
	}
	var handle;
	me.mouseover(function(){
		handle = setTimeout(hover , 200);
	});
	me.mouseout(function(){
		clearTimeout(handle);
		over = false;
		ajaxflag = false;
		setTimeout(hideTimer,200);
	});
	var hideTimer = function(){
		if (!over) {
			$('.' + popid).remove();
			$('#pop_' + type + '_menu').remove();
		}
	};
	var hover = function(){
		if(typeof(num) == 'undefined') num = 170;
		if (!over) {
			_gaq.push(['_trackEvent', 'ProfileCard', 'Show']);
			var left = Math.round($(me).offset().left);
			var clientHeight = document.documentElement.clientHeight;
			var scrollTop = Math.max(document.documentElement.scrollTop, document.body.scrollTop);
			var top = $(me).offset().top;
			var MeHeight = $(me).height();
			var puid = me.attr('data-uid');
			var loadico = typeof(cardcacheData['data_' + puid]) != 'undefined' ? '' : '<div class="loading" ></div>';
			var html = '';
				html += '<div class="elexPopup" id="'+ popid +'" style="display:none;left:'+ left +'px;z-index:99999;">';
				html += 	'<div class="elexPopupWrap">';
				html += 		'<div class="popopIco"></div>';
				html += 		'<div class="popopCont">';
				html += 			loadico;
				html += 		'</div>';
				html += 		'<div class="popopIco2"></div>';
    			html += 	'</div>';
        		html += '</div>';
    		$('body').append(html);
			if(typeof(cardcacheData['data_' + puid]) != 'undefined'){
				var cacheData = cardcacheData['data_' + puid];
				$('#' + popid ).find('.popopCont').html(cacheData);
			}else{
				if(ajaxflag) return;
				ajaxflag = true;
				elex.request.post(elex.config.get('baseUrl') + "/user/ajax_getinfo", {uid:puid,groupid:pgid}, function(ret){
					$('#' + popid ).find('.popopCont').html(ret.html);
					if(typeof(cardcacheData['data_' + puid]) == 'undefined'){
						cardcacheData['data_' + puid] = ret.html;
					}
					var ajaxpopHeight = $('#' + popid).outerHeight();
					if (direction == 'up') {
						$('#' + popid ).css('top',top + MeHeight);
					}else{
						$('#' + popid ).css('top',top - ajaxpopHeight);
					}
					if($('.inCardBtn_' + puid).length > 0){
						html = $(cardcacheData[puid]).attr('id','pop_' + type + '_menu');
						$('body').append(html);
						elex.menu.comMenu(['.inCardBtn_' + puid , '#pop_' + type + '_menu' , 'over']);
						$('#pop_' + type + '_menu').mouseover(function(){
							over = true;
						});
						$('#pop_' + type + '_menu').mouseout(function(){
							over = false;
							setTimeout(hideTimer,200);
						});
					}
					ajaxflag = false;
					return;
				},
				function(){
					elex.dialog.alert('<div class="elexDialogTipsNew"><p>{lang 337_network_connect_fail}</p></div>');;
					ajaxflag = false;
				});
			}
			var popHeight = $('#' + popid).outerHeight();
			var direction = (top - scrollTop) + MeHeight + num  <= clientHeight ? 'up' : 'down';
			if (direction == 'up') {
				$('#' + popid ).find('.popopIco2').hide();
				$('#' + popid ).find('.popopIco').addClass('topL').show();
				$('#' + popid ).css('top',top + MeHeight);
			} else {
				$('#' + popid ).find('.popopIco').hide();
				$('#' + popid ).find('.popopIco2').addClass('downL').show();
				$('#' + popid ).css('top',top - popHeight );
			}
			$('#' + popid ).show();
			if($('.inCardBtn_' + puid).length > 0){
				html = $(cardcacheData[puid]).attr('id','pop_' + type + '_menu');
				$('body').append(html);
				elex.menu.comMenu(['.inCardBtn_' + puid, '#pop_' + type + '_menu' , 'over']);
				$('#pop_' + type + '_menu').mouseover(function(){
					over = true;
				});
				$('#pop_' + type + '_menu').mouseout(function(){
					over = false;
					setTimeout(hideTimer,200);
				});
				}
			$('#' + popid).mouseover(function(){
				over = true;
			});
			$('#' + popid).mouseout(function(){
				over = false;
				setTimeout(hideTimer,200);
			});
		}
		over = true;
	};
};

//搜索
elex.util.search = function(me , url , data , searchData_id , id){
	if(seachflag){return}
	seachflag = true;
	var height = me.parent().outerHeight();
	var scroll_h = $(window).scrollTop();
	var y = Math.floor(me.parent().offset().top) + height - scroll_h;
	var x = Math.floor(me.parent().offset().left);
	var style = {'top' : y , 'left' : x , 'z-index':99999}
	var searchContainer = $('<div id="'+ id +'" style="display:none"></div>');
	if(typeof searchData[searchData_id] != 'undefined'){
		if($('#' + id).length > 0){
			$('#' + id).html(searchData[searchData_id]);
		}else{
			$('body').append(searchContainer);
			searchContainer.append(searchData[searchData_id]).css(style);
		}
	}else{
		elex.request.get(url,data,function(rs){
			if(rs['result']==0)
			{
				searchData[searchData_id] = rs.html;
				if($('#' + id).length > 0){
					$('#' + id).html(searchData[searchData_id]);
				}else{
					$('body').append(searchContainer);
					searchContainer.append(searchData[searchData_id]).css(style);
				}
				var menu = $('#' + id);
				if(menu.is(':hidden')) {
					menu.show();
					setTimeout(function(){
						$(document.body).one('click', function(){
							menu.remove();
						});
					}, 1000);
				}
				seachflag = false;
				return;
			}
		});
	}
	var menu = $('#' + id);
	if(menu.is(':hidden')) {
		menu.show();
		setTimeout(function(){
			$(document.body).one('click', function(){
				menu.remove();
			});
		}, 10);
	}
	seachflag = false;
}

//loading ico
elex.util.loading = function(fnc , time){
	setTimeout(fnc,time);
};
elex.util.ajaxLoading = function(obj){
	$(obj).siblings('.loading').children().ajaxStart(function(){
		$(this).show();
	}).ajaxStop(function(){
		$(this).hide();
	});
};

elex.util.upload = function(data , callback , title){
	var uploadCallback = 'ucb' + Math.ceil(Math.random() * (999999 - 100000) + 100000);
	window[uploadCallback] = function(ret){
		if ($.isFunction(callback)) {
			callback(ret);
		}
		window[uploadCallback] = null;
	}
	var url = elex.config.get('baseUrl') + '/common/uploadPhoto?upload_callback='+uploadCallback;
	if ($.isPlainObject(data)) {
		url += '&' + $.param(data);
	}
	$.dialog.box('elex_upload_dialog', title , {type : 'url', value : url});
}

//rate
elex.namespace('http://337.eleximg.com/337/v3static/js/elex.rate');
elex.rate.rateit = function(page_id , up_down , base_did , page_type){
	if(up_down == 'up'){
        plus = 1;
    }else{
        up_down = 'down';
        plus    = -1;
    }
	$.ajax({
        url: "/rate/" + up_down,
        data: {pagetype:page_type , basedid:base_did , pageid:page_id},
        cache: false,
        dataType : "xml",
        type : "GET",
        success: function(xml){
            var	message = xml.lastChild.firstChild.nodeValue;
            if(message == '1'){
                var rate_num = parseInt($("#rate_num_"+page_id).html()) + plus;
                $("#rate_num_" + page_id).html(rate_num);
            }else{
                //alert('Sorry,You have voted the page!');
            }
        }
    });
};

//下拉菜单风格
/*
array[0] 触发元素id
array[1] 下拉菜单元素id
array[2] 事件类型
array[3] 给触发元素绑定google打点事件
array[4] 是否开启动画效果
array[5] 下拉菜单流向
/*/
elex.namespace('http://337.eleximg.com/337/v3static/js/elex.menu');
elex.menu.comMenu = function(array){
	var flag = false;
	var tab = $(array[0]);
	var cont = $(array[1]);
	var type = array[2] != false ? array[2] : 'click';
	var silder = array[4] != false ? array[4] : '';
	var dir = array[5] != false ? array[5] : '';
	var scroll_h = $(window).scrollTop();
	if(dir == 'right'){
		var x = Math.floor(tab.offset().left) + tab.width() - cont.width();
	}
	else if(dir != '' && dir != false){
		var x = Math.floor(tab.offset().left) -  dir;
	}
	else{
		var x = Math.floor(tab.offset().left);
	}
	var y = Math.floor(tab.offset().top) + tab.height() - scroll_h;
	$(cont).css({'left' : x + 'px'});
	if(type == 'over'){
		var over = false;
		var hideTimer = function(){
			if (!over) {
				if(silder == true) cont.slideUp("fast");
				else cont.hide();
				tab.removeClass('inActive');
				flag = false;
			}
		};
		tab.mouseover(function(){
			if (!over) {
				tab.addClass('inActive');
				if(silder == true) cont.slideDown("fast");
				else cont.show();
				if(array[3].length != 0 && !flag){
					_gaq.push(array[3]);
					flag = true;
				}
			}
			over = true;
		});
		tab.mouseout(function(){
			if(cont.is(':hidden')){
				flag = true;
			}
			 over = false;
			 setTimeout(hideTimer, 200);
		 });
		cont.mouseover(function(){
			over = true;
		});
		cont.mouseout(function(){
			over = false;
			setTimeout(hideTimer, 200);
		});
	}else{
		tab.click(function(){
			if(cont.is(':hidden')) {
				cont.show();
				setTimeout(function(){
					$(document.body).one('click', function(){
						tab.removeClass('inActive');
						cont.hide();
					});
				}, 200);
			}
		});
	}
}
//获取用户历史记录
elex.namespace('elex.gamelist');
elex.gamelist.history_like = function(parm,type,lang){
	var loading = '<p class="loadingico"></p>';
	var tab =  $('#'+ type.split('_')[0] + '_menu_' + type.split('_')[1]);
	tab.html(loading);
	var url = elex.config.get('baseUrl') + "/game/ajax_get_like_and_history";
	var website = elex.config.get('baseUrl');
	elex.request.get(url , parm , function(ret){
		if(ret.history == null && ret.like == null && ret.recnmmend == null){
			html = '<p class="nohistory">' + elex.langs.get('no_game_history') + '</p>';
			tab.html(html);
		}else{
			var html = ''
			if(type == 'header_mygame'){
				if(ret.history != null){
					html += elex.gamelist.create_history(ret.history,website,lang.mygame,'mygames');
				}
				if(ret.like != null){
					html += elex.gamelist.create_history(ret.like,website,lang.mylike,'mylikes');
				}
				if(ret.recnmmend != null){
					html += elex.gamelist.create_history(ret.recnmmend,website,lang.myguess,'guess');
				}
				tab.html(html);
			}
			tab.find('li a').click(function(){
				index = $(this).closest('.getid').attr('id').split('_')[0];
				_gaq.push(['_trackEvent', 'Header', 'MyGame', 'click_cont_' + index]);
			});
			tab.find('span a').click(function(){
				index = $(this).closest('.getid').attr('id').split('_')[0];
				_gaq.push(['_trackEvent', 'Header', 'MyGame', 'click_title_' + index]);
			});
		}
	});
}
elex.gamelist.create_history = function(obj , url , lang , type){
	if(type == 'guess'){
		link = url + '/gamelist/';
	}else{
		link = url + '/game/' + type + '/';
	}
	html = '';
	html += '<div id="'+ type +'_list_test" class="getid">';
	html +=		'<div class="inTitle"><span><a href="'+ link +'" style="color:#b4c5e9">'+ lang +'</a></span></div>';
	html +=		'<ul id="'+ type +'list_img" class="bartest_1">';
	$.each(obj,function(i,n){
		if(obj[i].name != null){
			html += '<li>';
			html += 	'<p><a href="'+ obj[i].url +'" class="inImg" style="background-image:url('+ obj[i].icon3 +')"></a></p>';
			html += 	'<p><a class="inText" href="'+ obj[i].url +'">'+ obj[i].name +'</a></p>';
			html += '</li>';
		}
	});
	html += 	'</ul>';
	html += '</div>';
	return html;
}

//弹泡泡样式
elex.namespace('http://337.eleximg.com/337/v3static/js/elex.pop');
var mouseFollowSaveData = {};
elex.pop.ousemove = function(parm){
	var default_opt = {
		id : null,
		code : null,
		title : null,
		dialog_id : 'dialog_pop_id',
		index : false,
		pop_h : 167
	}
	var opt = $.extend(default_opt , parm);
	var me = $(opt.id);
	var dialog = opt.dialog_id;
	var over = false;
	var hideTimer = function(){
		if (!over) {
			$('#' + dialog).remove();
		}
	};
	me.mousemove(function(e){
		var h = $(window).height() + $(window).scrollTop();
		var x = e.pageX;
		if(h <= e.pageY + 30 + opt.pop_h){
			var y = e.pageY - 20 - opt.pop_h;
		}else{
			var y = e.pageY + 30;
		}

		var warp = '<div id="'+ dialog +'" class="dialogPopWarp" style_val>'+ opt.code +'</div>';
		var md5Str = typeof(opt.index) == false ? opt.title : opt.title + opt.index;
		if(typeof(mouseFollowSaveData[hex_md5(md5Str)]) == 'undefined'){
			mouseFollowSaveData[hex_md5(md5Str)] = warp;
			str = warp.replace('style_val','style="top:'+ y +'px;left:'+ x +'px;"');
			$('body').append(str);
		}else{
			html = 	mouseFollowSaveData[hex_md5(md5Str)];
			if($('#' + dialog).length == 0){
				str = warp.replace('style_val','style="top:'+ y +'px;left:'+ x +'px;"');
				$('body').append(str);
			}else{
				$('#' + dialog).css({'top':y,'left':x});
			}
		}
		over = true;
	});
	me.mouseout(function(){
		over = false;
		setTimeout(hideTimer, 50);
	});
}

//打scribe日志,记录游戏浏览历史和游戏点击历史
elex.namespace('elex.scribe');
elex.scribe.collectGameView = function(){
	var url  = elex.config.get('baseUrl') + '/game/ajaxScribeLog';
	var data = {logType : 'gameview',gameIds : GlobalGameLog}
	elex.request.post(url, data);
}
elex.scribe.collectGamePlay = function(gid,iid,pos){
	var url  = elex.config.get('baseUrl') + '/game/ajaxScribeLog';
	var data = {logType : 'gameplay',gameid : gid, iconid : iid, position : pos}
	elex.request.post(url, data);
}

//md5 arithmetic
elex.namespace('http://337.eleximg.com/337/v3static/js/elex.md5');
(function($){
	var hexcase = 0;
	var b64pad = '';
	var chrsz = 8;
	elex.md5.val = function(s){
		return elex.md5.binl2hex(elex.md5.core(elex.md5.str2binl(s), s.length * chrsz));
	}
	elex.md5.core = function(x ,len){
		x[len >> 5] |= 0x80 << ((len) % 32);
		x[(((len + 64) >>> 9) << 4) + 14] = len;
		
		var a =  1732584193;
		var b = -271733879;
		var c = -1732584194;
		var d =  271733878;
		
		for(var i = 0; i < x.length; i += 16){
			var olda = a;
			var oldb = b;
			var oldc = c;
			var oldd = d;
		
			a = elex.md5.f(a, b, c, d, x[i+ 0], 7 , -680876936);
			d = elex.md5.f(d, a, b, c, x[i+ 1], 12, -389564586);
			c = elex.md5.f(c, d, a, b, x[i+ 2], 17,  606105819);
			b = elex.md5.f(b, c, d, a, x[i+ 3], 22, -1044525330);
			a = elex.md5.f(a, b, c, d, x[i+ 4], 7 , -176418897);
			d = elex.md5.f(d, a, b, c, x[i+ 5], 12,  1200080426);
			c = elex.md5.f(c, d, a, b, x[i+ 6], 17, -1473231341);
			b = elex.md5.f(b, c, d, a, x[i+ 7], 22, -45705983);
			a = elex.md5.f(a, b, c, d, x[i+ 8], 7 ,  1770035416);
			d = elex.md5.f(d, a, b, c, x[i+ 9], 12, -1958414417);
			c = elex.md5.f(c, d, a, b, x[i+10], 17, -42063);
			b = elex.md5.f(b, c, d, a, x[i+11], 22, -1990404162);
			a = elex.md5.f(a, b, c, d, x[i+12], 7 ,  1804603682);
			d = elex.md5.f(d, a, b, c, x[i+13], 12, -40341101);
			c = elex.md5.f(c, d, a, b, x[i+14], 17, -1502002290);
			b = elex.md5.f(b, c, d, a, x[i+15], 22,  1236535329);
		
			a = elex.md5.g(a, b, c, d, x[i+ 1], 5 , -165796510);
			d = elex.md5.g(d, a, b, c, x[i+ 6], 9 , -1069501632);
			c = elex.md5.g(c, d, a, b, x[i+11], 14,  643717713);
			b = elex.md5.g(b, c, d, a, x[i+ 0], 20, -373897302);
			a = elex.md5.g(a, b, c, d, x[i+ 5], 5 , -701558691);
			d = elex.md5.g(d, a, b, c, x[i+10], 9 ,  38016083);
			c = elex.md5.g(c, d, a, b, x[i+15], 14, -660478335);
			b = elex.md5.g(b, c, d, a, x[i+ 4], 20, -405537848);
			a = elex.md5.g(a, b, c, d, x[i+ 9], 5 ,  568446438);
			d = elex.md5.g(d, a, b, c, x[i+14], 9 , -1019803690);
			c = elex.md5.g(c, d, a, b, x[i+ 3], 14, -187363961);
			b = elex.md5.g(b, c, d, a, x[i+ 8], 20,  1163531501);
			a = elex.md5.g(a, b, c, d, x[i+13], 5 , -1444681467);
			d = elex.md5.g(d, a, b, c, x[i+ 2], 9 , -51403784);
			c = elex.md5.g(c, d, a, b, x[i+ 7], 14,  1735328473);
			b = elex.md5.g(b, c, d, a, x[i+12], 20, -1926607734);
			
			a = elex.md5.h(a, b, c, d, x[i+ 5], 4 , -378558);
			d = elex.md5.h(d, a, b, c, x[i+ 8], 11, -2022574463);
			c = elex.md5.h(c, d, a, b, x[i+11], 16,  1839030562);
			b = elex.md5.h(b, c, d, a, x[i+14], 23, -35309556);
			a = elex.md5.h(a, b, c, d, x[i+ 1], 4 , -1530992060);
			d = elex.md5.h(d, a, b, c, x[i+ 4], 11,  1272893353);
			c = elex.md5.h(c, d, a, b, x[i+ 7], 16, -155497632);
			b = elex.md5.h(b, c, d, a, x[i+10], 23, -1094730640);
			a = elex.md5.h(a, b, c, d, x[i+13], 4 ,  681279174);
			d = elex.md5.h(d, a, b, c, x[i+ 0], 11, -358537222);
			c = elex.md5.h(c, d, a, b, x[i+ 3], 16, -722521979);
			b = elex.md5.h(b, c, d, a, x[i+ 6], 23,  76029189);
			a = elex.md5.h(a, b, c, d, x[i+ 9], 4 , -640364487);
			d = elex.md5.h(d, a, b, c, x[i+12], 11, -421815835);
			c = elex.md5.h(c, d, a, b, x[i+15], 16,  530742520);
			b = elex.md5.h(b, c, d, a, x[i+ 2], 23, -995338651);
			
			a = elex.md5.i(a, b, c, d, x[i+ 0], 6 , -198630844);
			d = elex.md5.i(d, a, b, c, x[i+ 7], 10,  1126891415);
			c = elex.md5.i(c, d, a, b, x[i+14], 15, -1416354905);
			b = elex.md5.i(b, c, d, a, x[i+ 5], 21, -57434055);
			a = elex.md5.i(a, b, c, d, x[i+12], 6 ,  1700485571);
			d = elex.md5.i(d, a, b, c, x[i+ 3], 10, -1894986606);
			c = elex.md5.i(c, d, a, b, x[i+10], 15, -1051523);
			b = elex.md5.i(b, c, d, a, x[i+ 1], 21, -2054922799);
			a = elex.md5.i(a, b, c, d, x[i+ 8], 6 ,  1873313359);
			d = elex.md5.i(d, a, b, c, x[i+15], 10, -30611744);
			c = elex.md5.i(c, d, a, b, x[i+ 6], 15, -1560198380);
			b = elex.md5.i(b, c, d, a, x[i+13], 21,  1309151649);
			a = elex.md5.i(a, b, c, d, x[i+ 4], 6 , -145523070);
			d = elex.md5.i(d, a, b, c, x[i+11], 10, -1120210379);
			c = elex.md5.i(c, d, a, b, x[i+ 2], 15,  718787259);
			b = elex.md5.i(b, c, d, a, x[i+ 9], 21, -343485551);
			
			a = elex.md5.s(a, olda);
			b = elex.md5.s(b, oldb);
			c = elex.md5.s(c, oldc);
			d = elex.md5.s(d, oldd);
		}
		return Array(a, b, c, d);
	}
	elex.md5.s = function(x, y){
	  var lsw = (x & 0xFFFF) + (y & 0xFFFF);
	  var msw = (x >> 16) + (y >> 16) + (lsw >> 16);
	  return (msw << 16) | (lsw & 0xFFFF);
	}
	elex.md5.cmn = function(q, a, b, x, s, t){ return elex.md5.s(elex.md5.bit_rol(elex.md5.s(elex.md5.s(a, q), elex.md5.s(x, t)), s),b);}
	elex.md5.f = function(a, b, c, d, x, s, t){ return elex.md5.cmn((b & c) | ((~b) & d), a, b, x, s, t);}
	elex.md5.g = function(a, b, c, d, x, s, t){ return elex.md5.cmn((b & d) | (c & (~d)), a, b, x, s, t);}
	elex.md5.h = function(a, b, c, d, x, s, t){ return elex.md5.cmn(b ^ c ^ d, a, b, x, s, t);}
	elex.md5.i = function(a, b, c, d, x, s, t){ return elex.md5.cmn(c ^ (b | (~d)), a, b, x, s, t);}
	elex.md5.binl2hex = function(binarray){
		var hex_tab = hexcase ? "0123456789ABCDEF" : "0123456789abcdef";
		var str = "";
		for(var i = 0; i < binarray.length * 4; i++){
			str += hex_tab.charAt((binarray[i>>2] >> ((i%4)*8+4)) & 0xF) + hex_tab.charAt((binarray[i>>2] >> ((i%4)*8  )) & 0xF);
		}
		return str;
	}
	elex.md5.str2binl = function(str){
		var bin = Array();
		var mask = (1 << chrsz) - 1;
		for(var i = 0; i < str.length * chrsz; i += chrsz) bin[i>>5] |= (str.charCodeAt(i / chrsz) & mask) << (i%32);
		return bin;
	}
	elex.md5.bit_rol = function(num, cnt){ return (num << cnt) | (num >>> (32 - cnt));}
})(jQuery);

//轮流展示图片
elex.namespace('http://337.eleximg.com/337/v3static/js/elex.rota');
(function($){
	var interval_prcocess;
	var cur_img_index = 0;
	var global_img_obj = {};
	elex.rota.rota = function(obj,img_obj){
		if(typeof(img_obj) != 'undefined')
		{
			global_img_obj = eval('(' + img_obj + ')');
			for(i=1;i<=5;i++)
			{
				if(typeof(global_img_obj[i]) != 'undefined')
				{
					preload = new Image();
					preload.src=global_img_obj[i].iconUrl;
				}
			}
			interval_prcocess = setInterval("elex.rota.trota('"+obj+"','"+img_obj+"');",500);
		}
	};
	elex.rota.trota = function(obj,img_obj){
		cur_img_index = cur_img_index + 1;
		if(cur_img_index > 5)
		{
			cur_img_index = 1;
		}
		while(typeof(global_img_obj[cur_img_index]) == 'undefined')
		{
			elex.rota.trota(obj,img_obj);
		}
		img_url = global_img_obj[cur_img_index].iconUrl;
		$('#'+obj+' img').attr('src',img_url);
	};
	elex.rota.outrota = function(obj,img_src){
		cur_img_index = 0;
		clearInterval(interval_prcocess);
		$('#'+obj+' img').attr('src',img_src);
	};
})(jQuery);
