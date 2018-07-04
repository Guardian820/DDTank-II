// SEO url
var urlArray = ['google.com','bing.com','search.com','ask.com','yahoo.com','aol.com','yandex.com','aol.com','naver.com','baidu.com','mamma.com','live.com','alltheweb.com','altavista.de','lycos.com'];

//not Track link
var notTrackHref = ['https://khuyenmai.zing.vn','http://diendan.zing.vn','https://hotro1.zing.vn']; 

// track info
var pid = 49;
var SEOTrackInfo = {
	pid: 49,
	bannerid: 180,
	eventid: 126,
	type: 442
};
var directTrackInfo = {
	pid: 49,
	bannerid: 732,
	eventid: 63,
	type: 411
};
var specialLink ={
	quickRegLink: {
		link: 'https://id.zing.vn/quickregister/game/index.49.html',
		type: 154
	}
}
jQuery(document).ready(function(){
	trackLink('', true); //apply Tracking link
});

function getParams(pos) { 
	var pathArray = window.location.pathname.split( '/' );
	var params = pathArray[pathArray.length -1];
	if (params) {
		var listParams = params.split('.');
		if (listParams) {			
			if (listParams[pos] && listParams[pos] != undefined) {
				return listParams[pos];
			}			
		}
	}
	return false;	
}

function checkURLReferer(urlArray){
	var _referer = document.referrer.split('/');			
	var _url =  _referer[2];
	for (i in urlArray){
		var pattUrl = new RegExp(urlArray[i]);
		if (pattUrl.test(_url)){
			return true;
		} 
	}
	return false;
} 
var eventid = getParams(1); 
var bannerid = getParams(2);
var typeid  = getParams(3);
var notTrackId = new Array("typeRegTopbar","typeDownload","guideReg","textReg","guideDownload","textDownload","typeReg","typePlayNow","typePromotion01","typePromotion02");
var notTrackLink = new Array("http://pay.zing.vn","https://pay.zing.vn","https://hotro1.zing.vn","http://hotro1.zing.vn");

var link = "";

function trackLink(parentSelector, allTrack) {
	setTimeout(function(){		
		if (pid != false && eventid != false && bannerid != false){

			link = 'pid='+ pid + '&eventid='+ eventid + '&bannerid=' + bannerid;
			if(typeid != false){
				link += '&type=' + typeid;
			}
		}else {
			if(allTrack != undefined && allTrack != false){
				try {				
					var urlReferrer = document.referrer;
					if(urlReferrer != '' && checkURLReferer(urlArray) && SEOTrackInfo != undefined){// SEO track
						link = 'pid='+ SEOTrackInfo.pid + '&eventid='+ SEOTrackInfo.eventid + '&bannerid=' + SEOTrackInfo.bannerid + '&type=' + SEOTrackInfo.type;
						jQuery("body").append('<div id="iframeTrack">'
											 +'	   <iframe src="https://appclick.zing.vn/UserClick/Analytics.php?'+link+'" width="1" height="1"></iframe>'
											 +'</div>');
						pid = SEOTrackInfo.pid;
						eventid = SEOTrackInfo.eventid;
						bannerid = SEOTrackInfo.bannerid;
					} else if(directTrackInfo != undefined){// Direct Track
						link = 'pid='+ directTrackInfo.pid + '&eventid='+ directTrackInfo.eventid + '&bannerid=' + directTrackInfo.bannerid + '&type=' + directTrackInfo.type;
						pid = directTrackInfo.pid;
						eventid = directTrackInfo.eventid;
						bannerid = directTrackInfo.bannerid;
					}
					jQuery("#iframeTrack").hide();
				} catch(exp){}
			}
		}
		
		parentSelector = parentSelector != undefined && parentSelector != "" && jQuery(parentSelector).length > 0 ? parentSelector:"body";
		jQuery(parentSelector).find("a").not(".notTrack, .NotTrack").each(function (index) { // use class "notTrack" for untracking a
			if(jQuery.inArray(jQuery(this).attr("id"), notTrackId) == -1){
				var lnk = jQuery(this);
				var href = lnk.attr("href");
				
					for ( var k in specialLink ){
						var pattSLink = new RegExp("^" + specialLink[k].link,"i");
						
						if( pattSLink.test(href) && pid != false && eventid != false && bannerid != false){
							//lnk.attr("href",'https://appclick.zing.vn/UserClick/Analytics.php?pid='+ pid + '&eventid='+ eventid + '&bannerid=' + bannerid + '&type=' + specialLink[k].type);
							if( href.search("#") != -1){
								var arr_href =  href.split("#");
								arr_href[0] = 'https://appclick.zing.vn/UserClick/Analytics.php' + (arr_href[0].indexOf("?") != -1 ? '?' + arr_href[0].split("?")[1] + '&' : '?') + 'pid='+ pid + '&eventid='+ eventid + '&bannerid=' + bannerid + '&type=' + specialLink[k].type;
								var _href = arr_href[0] + "#" + arr_href[1];
								lnk.attr("href", _href);
							} else if ( href.indexOf("?") != -1) {
								var arr_href =  href.split("?");
								var _href = 'https://appclick.zing.vn/UserClick/Analytics.php?pid='+ pid + '&eventid='+ eventid + '&bannerid=' + bannerid + '&type=' + specialLink[k].type + '&' + arr_href[1];
								lnk.attr("href", _href);
							} else {
								lnk.attr("href",'https://appclick.zing.vn/UserClick/Analytics.php?pid='+ pid + '&eventid='+ eventid + '&bannerid=' + bannerid + '&type=' + specialLink[k].type);
							}
							return true;
						}
					}
					for (var k in notTrackHref){					
						var pattstr = "^"+notTrackHref[k];
						var pattLink = new RegExp(pattstr,"i");
						if(pattLink.test(href)){
							return true;
						}
					}
				
				if (href &&  href.charAt(0)!= "#" && link != ''){
					if( href.search("#") != -1){
						arr_href =  href.split("#");
						arr_href[0] = arr_href[0] + (arr_href[0].indexOf("?") != -1 ? '&' : '?') + link;
						_href = arr_href[0] + "#" + arr_href[1];
						lnk.attr("href", _href);
					} else {
						lnk.attr("href", href + (href.indexOf("?") != -1 ? '&' : '?') + link);
					}
				}
			}
		});
	},10);
}