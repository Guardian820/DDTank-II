$(function(){
    var page = 1;
    var i = 5; //每版放5个图片
    //向后 按钮
    $("span.next").click(function(){    //绑定click事件
	     var $parent = $(this).parents("div.v_show");//� �据当前点击元� 获取到父元� 
		 var $v_show = $parent.find("div.v_content_list"); //寻找到“视频内容展示区域”
		 var $v_content = $parent.find("div.v_content"); //寻找到“视频内容展示区域”外围的DIV元� 
		 var v_width = $v_content.width() ;
		 var len = $v_show.find("li").length;
		 var page_count = Math.ceil(len / i) ;   //只要不是整数，就往大的方向取最小的整数
		 if( !$v_show.is(":animated") ){    //判断“视频内容展示区域”是否正在处于动画
			  if( page == page_count ){  //已经到最后一个版面了,如果再向后，必须跳转到第一个版面。
				$v_show.animate({ left : '0px'}, "slow"); //通过改变left值，跳转到第一个版面
				page = 1;
				}else{
				$v_show.animate({ left : '-='+v_width }, "slow");  //通过改变left值，达到每次换一个版面
				page++;
			 }
		 }
   });
    //往前 按钮
    $("span.prev").click(function(){
	     var $parent = $(this).parents("div.v_show");//� �据当前点击元� 获取到父元� 
		 var $v_show = $parent.find("div.v_content_list"); //寻找到“视频内容展示区域”
		 var $v_content = $parent.find("div.v_content"); //寻找到“视频内容展示区域”外围的DIV元� 
		 var v_width = $v_content.width();
		 var len = $v_show.find("li").length;
		 var page_count = Math.ceil(len / i) ;   //只要不是整数，就往大的方向取最小的整数
		 if( !$v_show.is(":animated") ){    //判断“视频内容展示区域”是否正在处于动画
		 	 if( page == 1 ){  //已经到第一个版面了,如果再向前，必须跳转到最后一个版面。
				$v_show.animate({ left : '-='+v_width*(page_count-1) }, "slow");
				page = page_count;
			}else{
				$v_show.animate({ left : '+='+v_width }, "slow");
				page--;
			}
		}
    });
});




$(function(){
    var page2 = 1;
    var o = 5; //每版放5个图片
    //向后 按钮
    $("span.next2").click(function(){    //绑定click事件
	     var $parent2 = $(this).parents("div.v_show2");//� �据当前点击元� 获取到父元� 
		 var $v_show2 = $parent2.find("div.v_content_list2"); //寻找到“视频内容展示区域”
		 var $v_content2 = $parent2.find("div.v_content2"); //寻找到“视频内容展示区域”外围的DIV元� 
		 var v_width2 = $v_content2.width() ;
		 var len2 = $v_show2.find("li").length;
		 var page_count2 = Math.ceil(len2 / o) ;   //只要不是整数，就往大的方向取最小的整数
		 if( !$v_show2.is(":animated") ){    //判断“视频内容展示区域”是否正在处于动画
			  if( page2 == page_count2 ){  //已经到最后一个版面了,如果再向后，必须跳转到第一个版面。
				$v_show2.animate({ left : '0px'}, "slow"); //通过改变left值，跳转到第一个版面
				page2 = 1;
				}else{
				$v_show2.animate({ left : '-='+v_width2 }, "slow");  //通过改变left值，达到每次换一个版面
				page2++;
			 }
		 }
   });
    //往前 按钮
    $("span.prev2").click(function(){
	     var $parent2 = $(this).parents("div.v_show2");//� �据当前点击元� 获取到父元� 
		 var $v_show2 = $parent2.find("div.v_content_list2"); //寻找到“视频内容展示区域”
		 var $v_content2 = $parent2.find("div.v_content2"); //寻找到“视频内容展示区域”外围的DIV元� 
		 var v_width2 = $v_content2.width();
		 var len2 = $v_show2.find("li").length;
		 var page_count2 = Math.ceil(len2 / o) ;   //只要不是整数，就往大的方向取最小的整数
		 if( !$v_show2.is(":animated") ){    //判断“视频内容展示区域”是否正在处于动画
		 	 if( page2 == 1 ){  //已经到第一个版面了,如果再向前，必须跳转到最后一个版面。
				$v_show2.animate({ left : '-='+v_width2*(page_count2-1) }, "slow");
				page2 = page_count2;
			}else{
				$v_show2.animate({ left : '+='+v_width2 }, "slow");
				page2--;
			}
		}
    });
});
