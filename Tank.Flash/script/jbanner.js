//
//initCallback: baner_onload_callback,
//onButtonAfterAnimationCallBack: animation_callback
function banner_onload_callback (carousel, item) {
    //external control
	
    jQuery("ul.ImgControl > li > a").each(function (index) {
        var item = jQuery(this);	
    });

    //binding event to all items
    carousel.list.find("> li.jcarousel-item").each(function (index) {
        var item = jQuery(this);

        /*item.bind("click", function () {
            set_active(carousel, item);
            move_to_visible(carousel, item);       

            //active button
            active_button(index);
        });*/

        item.bind("mouseover", function () { //hover
            carousel.list.find("> li.Hover").removeClass("Hover");         
            item.addClass("Hover");         
            return false;
        });
    });

    jQuery(document).bind("mouseover", function () { //hover remove
        carousel.list.find("> li.Hover").removeClass("Hover");         
    });

    //active item
    var active_item = carousel.list.find("> li.ActiveBanner");
    var list_array = carousel.list.find("> li");
    var active_index = jQuery.inArray(active_item.get(0), list_array);
	
    //set active clicked
    jQuery("ul.ImgControl > li.ActiveBanner").removeClass("ActiveBanner"); //clear
    jQuery("ul.ImgControl > li").eq(active_index).addClass("ActiveBanner"); //set active	
    //set active on carousel
    set_active(carousel, active_item);

    //scroll carousel
    setTimeout(function () {
        carousel.scroll(active_index+1, /*animating*/false, /*callback*/function () {
            move_to_visible(carousel, active_item);
        }); //scroll to item
    }, 1);

    if (active_index == (list_array.length - 1)) {
      setTimeout(function() {
          carousel.next();
        }, 1);
    }

    
}

function active_button (active_index) {
    
	jQuery(" ul.ImgControl > li.ActiveBanner").removeClass("ActiveBanner"); //clear	
    jQuery(" ul.ImgControl > li").eq(active_index).addClass("ActiveBanner");		
	
}

function set_active (carousel, active_item) {
    carousel.list.find("> li.ActiveBanner").removeClass("ActiveBanner"); //clear
    active_item.addClass("ActiveBanner"); //set active
	
}

function move_to_visible (carousel, active_item) {
    if ( active_item.position().left + active_item.width() + carousel.list.position().left > carousel.clip.width() ) { //force visible for active item (right-item)
        carousel.next();
    }
    else if ( active_item.position().left + carousel.list.position().left < 0 ) { //force visible for active item (left-item)
        carousel.prev();
    }
}

function animation_callback (carousel) {
    var active_item = carousel.list.find("> li.ActiveBanner");
	
    var active_index = jQuery.inArray(active_item.get(0), carousel.list.find("> li"));
	
	//alert(active_index);
	
    if ( active_item.length > 0 ) {
        if ( active_item.position().left + active_item.width() + carousel.list.position().left > carousel.clip.width() ) { //right-item-invisible
            //set active for prev item
            active_item.removeClass("ActiveBanner");
			
            active_item.prev().addClass("ActiveBanner");
            active_index =active_index-1;
        }
        else if ( active_item.position().left + carousel.list.position().left < 0 ) { //left-item-invisible
            //set active for next item
            active_item.removeClass("ActiveBanner");
			
            active_item.next().addClass("ActiveBanner");
            active_index=active_index+1;
			
        }		
        
		
        active_button(active_index);
    }
}
