//minified
(function(){window.XA||(XA={_url:"//xa.xingcloud.com/v4/",_actions:[],_updates:[],_sending:false,init:function(a){if(!a.app)throw Error("App is required.");XA._app=a.app;XA._uid=a.uid||"random"},setUid:function(a){XA._uid=a},action:function(){for(var a=0,b=arguments.length;a<b;a++)XA._actions.push(arguments[a]);XA._asyncSend()},update:function(){for(var a=0,b=arguments.length;a<b;a++)XA._updates.push(arguments[a]);XA._asyncSend()},_asyncSend:function(){setTimeout(function(){var a=XA._url+XA._app+
"/"+XA._uid+"?",b=null,c="",d=0;if(!(XA._updates.length+XA._actions.length==0||XA._sending)){for(XA._sending=true;b=XA._updates.shift();)if(c="update"+d++ +"="+encodeURIComponent(b)+"&",a.length+c.length>=1980){XA._updates.unshift(b);break}else a+=c;for(d=0;b=XA._actions.shift();)if(c="action"+d++ +"="+encodeURIComponent(b)+"&",a.length+c.length>=1980){XA._actions.unshift(b);break}else a+=c;(new Image).src=a+"_ts="+(new Date).getTime();XA._updates.length+XA._actions.length>0&&XA._asyncSend();XA._sending=
false}},0)}})})();
setTimeout(function () {
	var uid = "123456";
	var lng = "";
	var vipexpire = "";
	var site = /(xaauto\.js)(.*)#/,
		scripts = document.getElementsByTagName('script'); 
	for(var i=0,l=scripts.length;i<l;i++){
		var surl = scripts[i].src;
		if(site.test(surl)){
			var uidstr = surl.substring(surl.indexOf("#")+1);
			var uidarr = uidstr.split(',');
			uid = uidarr[0];
			if(uidarr.length>1){
				lng = uidarr[1];
			}
			if(uidarr.length>2){
				vipexpire = uidarr[2];
			}
			if(uid == null || uid == ""){
				uid = "123456";
			}
    	}
	}
	var url = document.location.href;
	var arrStr = url.substring(url.indexOf("?")+1).split("&");
	var src = "";
	var edm = "";
	var xattr = "";
	for(var i =0;i<arrStr.length;i++){
		var loc = arrStr[i].indexOf("src=");
		if(loc!=-1){
			src = arrStr[i].replace("src=","").replace("?","");
		}
		loc = arrStr[i].indexOf("edm=");
		if(loc!=-1){
			edm = arrStr[i].replace("edm=","").replace("?","");
		}
		loc = arrStr[i].indexOf("xattr=");
		if(loc!=-1){
			xattr = arrStr[i].replace("xattr=","").replace("?","");
			xattr = decodeURIComponent(xattr);
		}
	}
	XA.init({app:"web337",uid:uid});
	XA.action('visit');
	if(src != ""){
		XA.update('ref,'+src);
	}
	if(edm != ""){
		XA.update('edm,'+edm);
	}
	if(lng != ""){
		XA.update('language,'+lng);
	}
	if(uid != "123456" && vipexpire!=""){
		XA.update('vipexpire,'+vipexpire);
	}
	if(xattr != ""){
		XA.update(xattr);
	}
},50);
