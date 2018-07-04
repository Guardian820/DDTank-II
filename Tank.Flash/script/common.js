jQuery(document).ready(function(){	
								
	if(jQuery("#img > li").length > 1){
		new FadeGallery(jQuery("#img"),{
			control_event: "mouseover",
			auto_play: true,
			control: jQuery("ul#imgControl"),
			delay: 2
		});
	}	
		
		// Su dung Tab
		if( jQuery("ul.Tab li a").length > 0) {				
			jQuery(".ContentTab").hide();
			jQuery("#serverTabHot").show(); // show tab noi dung tab first
			jQuery('ul.Tab li a').bind('mouseover', function() {
				
				var idTab= jQuery(this).attr("rel");
				jQuery('ul.Tab li').removeClass("ui-state-active");
				jQuery(this).parent().addClass("ui-state-active");
				
				jQuery(".ContentTab").hide();
				jQuery("#"+idTab).show();
				return false;
			});
		}
	
							
	if (jQuery(".SelectUI").length > 0) {      
        jQuery(".SelectUI").addSelectUI({
            scrollbarWidth: 10 //default is 10
        });
    } 	
		
	/*even header*/
	if(jQuery("#listTop1").length > 0){
		jQuery("#listTop1").jcarousel({
			scroll: 1,
			//auto: 2,
			//wrap: 'last',
			initCallback: banner_onload_callback,
			onButtonAfterAnimationCallBack: animation_callback
		});
	}
	if(jQuery("#listTop2").length > 0){
		jQuery("#listTop2").jcarousel({
			scroll: 1,
			//auto: 2,
			//wrap: 'last',
			initCallback: banner_onload_callback,
			onButtonAfterAnimationCallBack: animation_callback
		});
	}
	if(jQuery("#listTop3").length > 0){
		jQuery("#listTop3").jcarousel({
			scroll: 1,
			//auto: 2,
			//wrap: 'last',
			initCallback: banner_onload_callback,
			onButtonAfterAnimationCallBack: animation_callback
		});
	}
	if(jQuery("#listTop4").length > 0){
		jQuery("#listTop4").jcarousel({
			scroll: 1,
			//auto: 2,
			//wrap: 'last',
			initCallback: banner_onload_callback,
			onButtonAfterAnimationCallBack: animation_callback
		});
	}  			
	/**********/
	if(jQuery("#container #boxTab").length >0 ){
		var actTab =0;		
		jQuery("#container #boxTab").tabs({
			event: 'mouseover',
			selected: actTab
			
		});
	}
	
	/******popup*****/
	if(jQuery("a.OpenPopup").length >0 ){	
		jQuery("a.OpenPopup").click(function(){											 
			var url = $(this).attr('href');
			var strclass = $(this).attr('rel');			
			if(jQuery('#subspamlink iframe').length>0){
				jQuery('#subspamlink iframe').attr('src',url);
				jQuery('#subspamlink').removeClass('PopupQuick');								
				jQuery('#subspamlink').addClass(strclass);
			}
			createOverlays("subspamlink");			
			jQuery('.SurveyClose').bind('click', function() {
				closeVideo('subspamlink');				
				return false;
			});
			jQuery('#thewindowbackground').bind('click', function() {
				closeVideo('subspamlink');				
				return false;
			});	
			
			return false;
		});			
	}
	
	/**************/
	var windowWidth = jQuery(window).width();
	var windowHeight = jQuery(window).height();
	var popupWidth = jQuery('.Popup').width();
	var popupHeight = jQuery('.Popup').height();
	$('.Popup').css('top',(windowHeight-popupHeight)/2);
	$('.Popup').css('left',(windowWidth-popupWidth)/2);
	
	$(window).resize(function() {
		windowWidth = jQuery(window).width();
		windowHeight = jQuery(window).height();
  		$('.Popup').css('top',(windowHeight-popupHeight)/2);
		$('.Popup').css('left',(windowWidth-popupWidth)/2);
		$('.Popup').css('opacity',1);
	});
	
	$('#viewDetail').click(
		function(){
			$('.opaEff').addClass('visible');
			$('.Popup').show();
			$('#scrollbar1').tinyscrollbar({ size: 440 });
			$('.Popup').css('opacity',1);
		}
	);
	$('.PopupClose').click(
		function(){
			$('.opaEff').removeClass('visible');
			$('.Popup').hide('visible');	
		}
	)
	$('.opaEff').click(
		function(){
			$('.opaEff').removeClass('visible');
			$('.Popup').hide('visible');	
		}
	)	
	
	
});

