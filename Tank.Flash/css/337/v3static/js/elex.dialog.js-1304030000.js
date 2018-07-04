/**
 * Dialog - modified by laihansheg
 *
 * @author    caixw <http://www.caixw.com>
 * @copyright Copyright (C) 2010, http://www.caixw.com
 * @license   FreeBSD license
 */


/**
 * jQuery的Dialog插件。
 *
 * @param object content
 * @param object this.options 选项。
 * @return 
 */
(function($, undefined) {
	elex.Dialog = function(content, options) {
		this.options = $.extend({}, __defaults);
        if ($.isPlainObject(content)) {
	        $.extend(this.options, content);
            //兼容最初的格式
            if (this.options.type && this.options.value) {
                this.options[this.options.type] = this.options.value;
            }
        } else {
            //兼容之后的格式
            if (content.substring(0, 1) == '#') {
                this.options.dom = content.substring(1);
            } else if (content.substring(0, 4) == 'http') {
                this.options.url = content;
            } else {
                this.options.html = content;
            }
            
        }
	    $.extend(this.options, options);
		this.options.id = this.options.id ? this.options.id : 'dialog-' + __count; // 唯一ID
	    this.overlayId = this.options.id + '-overlay'; // 遮罩层ID
	    this.timeId = null;  // 自动关闭计时器 
	    this.isShow = false;
	    //this.isIe = $.browser.msie;
	    //this.isIe6 = $.browser.msie && ('6.0' == $.browser.version);

	    /* 对话框的布局及标题内容。*/
		var closeHtml = !this.options.showClose ? '' : '<a class="dialog_close">' + this.options.closeText + '</a>';
	    var titleHtml =  !this.options.showTitle ? '' : '<div class="dialog_title"><h3>' + this.options.title + '</h3>' + closeHtml + '<div class="dialog_clear"></div></div>';
		var contentHtml = '<div class="dialog_content"></div>';
		var buttonHtml = '<div class="dialog_button" style="display:none;"></div>';
		var dialogHtml = '<div id="' + this.options.id + '" class="dialog">'+ titleHtml + contentHtml + buttonHtml + '</div>';
		this.dialog = $(dialogHtml).hide();
		var me = this;
		this.btnTypes = ['close','ok','cancel','yes','no'];
        this.closeStatus = 'close';
		if (this.options.buttons) {
			var _buttons = this.options.buttons.split(',');
			var buttons = [];
			for (var i=0; i<_buttons.length; i++) {
				var _thisbtn = _buttons[i];
				var thisbtn = [];
				buttons.push({
                    type:_buttons[i],
					text:elex.langs.get(_buttons[i]),
					style:(_buttons[i] == 'ok' || _buttons[i] == 'yes' ? 'blue' : 'gray'),
					click: function() {
						me.close();
					}
				});
			}
			this.addButton(buttons);
		}
	    $('body').append(this.dialog);
		if(this.options.width) this.dialog.find('.dialog_content').width(this.options.width);
		if(this.options.height) this.dialog.find('.dialog_content').height(this.options.height);
	    this.init();
	    this.setContent(this.options);
	    __count++;
	    __zindex++;
		__dialogs['dialog' + this.options.id] = __lastDialog = this;
	}
	
	elex.Dialog.prototype = {
	    /**
	     * 重置对话框的位置。
	     *
	     * 主要是在需要居中的时候，每次加载完内容，都要重新定位
	     *
	     * @return void
	     */
	    resetPos : function(){
	        /* 是否需要居中定位，必需在已经知道了dialog元素大小的情况下，才能正确居中，也就是要先设置dialog的内容。 */
	        if(this.options.center){
	            var left = ($(window).width() - this.dialog.width()) / 2;
	            var top = ($(window).height() - this.dialog.height()) / 2;
			//	if($.browser.version== '7.0'||$.browser.version== '6.0'){this.dialog.find('.dialog_title').css('width',this.dialog.find('.dialog_content').width()-15);}
	            if(!this.isIe6 && this.options.fixed){
					this.dialog.css({top:top,left:left});
				}else{
					this.dialog.css({top:top+$(document).scrollTop(),left:left+$(document).scrollLeft()});
				}
	        }
	    },
	
	    /**
	     * 初始化位置及一些事件函数。
	     *
	     * 其中的this表示Dialog对象而不是init函数。
	     */
	    init : function(){
	        /* 是否需要初始化背景遮罩层 */
	        var me = this; 
	        if(this.options.modal){
	            $('body').append('<div id="' + this.overlayId + '" class="dialog-overlay"></div>');
	            $('#' + this.overlayId).css({
	            	'left':0,
					'top':0,
	                /*'width':$(document).width(),*/
	                'width':'100%',
	                /*'height':'100%',*/
	                'height':$(document).height(),
	                'z-index':++__zindex,
	                'position':'absolute'
	            }).hide();
	        }
	
	        this.dialog.css({'z-index':++__zindex, 'position':this.options.fixed ? 'fixed' : 'absolute'});
	
			/*  IE6 兼容fixed代码 */
	        if(this.isIe6 && this.options.fixed){
	            this.dialog.css('position','absolute');
	            this.resetPos();
	            var top = parseInt(this.dialog.css('top')) - $(document).scrollTop();
	            var left = parseInt(this.dialog.css('left')) - $(document).scrollLeft();
	            $(window).scroll(function(){
	                me.dialog.css({'top':$(document).scrollTop() + top,'left':$(document).scrollLeft() + left});
	            });
	        }
	
	        /* 以下代码处理框体是否可以移动 */
	        var mouse={x:0,y:0};
	        function moveDialog(event){
	            var e = window.event || event;
	            var top = parseInt(me.dialog.css('top')) + (e.clientY - mouse.y);
	            var left = parseInt(me.dialog.css('left')) + (e.clientX - mouse.x);
	            me.dialog.css({top:top,left:left});
	            mouse.x = e.clientX;
	            mouse.y = e.clientY;
	        };
	        this.dialog.find('.dialog_title').mousedown(function(event){
	            if(!me.options.draggable){  return; }
	
	            var e = window.event || event;
	            mouse.x = e.clientX;
	            mouse.y = e.clientY;
	            $(document).bind('mousemove',moveDialog);
	        });
	        $(document).mouseup(function(event){
	            $(document).unbind('mousemove', moveDialog);
	        });
	
	        /* 绑定一些相关事件。 */
	        this.dialog.find('.dialog_close').bind('click', function(){me.close();});
	        this.dialog.bind('mousedown', function(){
				me.dialog.css('z-index', ++__zindex);
			});
	
	        // 自动关闭 
	        if(0 != this.options.time){
				this.timeId = setTimeout(function(){me.close();}, this.options.time);
			}
	    },
	
	
	    /**
	     * 设置对话框的内容。 
	     *
	     * @param string c 可以是HTML文本。
	     * @return void
	     */
	    setContent : function(c){
	    	var me = this;
	        var div = this.dialog.find('.dialog_content');
			var type = 'html';
            if (c.dom) type = 'dom';
            else if (c.text) type = 'text';
            else if (c.img) type = 'img';
            else if (c.url) type = 'url';
            else if (c.iframe) type = 'iframe';
            switch(type){
	            case 'id': // 将ID的内容复制过来，原来的还在。
	                div.html($('#' + c.dom).html());
	                break;
	            case 'img':
	                div.html('<div class="loading"></div>');
	                $('<img alt="" />').load(function(){
	                	div.empty().append($(this));
	                	me.resetPos();
	                }).attr('src', c.img);
	                break;
	            case 'url':
	                div.html('<div class="loading"></div>');
					elex.request.get(c.url, function(json) {
						if (typeof(json) == 'object' && json.html) {
							div.html(json.html);
						} else {
							div.html(json);
						}
                		me.resetPos();
                	}, function(xml,textStatus,error){
                     	div.html('<div class="elexDialogTipsNew"><p>Error!</p></div>')
                    });
	                break;
	            case 'iframe':
					var iframewidth = typeof(this.options.width) != 'undefined' ? this.options.width : '470';
					var iframeheight = typeof(this.options.height) != 'undefined' ? this.options.height : '300'; 
	                div.append($('<iframe src="' + c.iframe + '" width="'+ iframewidth +'"  height="'+ iframeheight +'" frameborder="0" />')).css({'width' : iframewidth + 'px' , 'height' : iframeheight + 'px'});
	                break;
	            case 'text':
	                div.html('<div style="padding:15px 20px;">' + c.text + '</div>');
	                break;
	            case 'html':
	                div.html(c.html);
	                break;
	            default:
	                div.html(c);
	                break;
            }
	
	    },
		
		/**
	     * 显示对话框
	     */
		addButton : function(btn) {
            var me = this;
            if ($.isArray(btn)) {
                for(b in btn) {
                    me.addButton(btn[b]);
                }
            } else {
				var $btn = $('<a href="###" class="' + (btn.style ? btn.style : 'gray') + '"><b>' + btn.text + '</b></a>');
				if (btn.click != undefined) {
					$btn.click(function(){
                        me.closeStatus = btn.type;
						btn.click();
						return false;
					});
				}
				this.dialog.find('.dialog_button').append($btn).show();
            }
		},
	
	    /**
	     * 显示对话框
	     */
	    show : function(){
			var me = this;
	        if(undefined != this.options.beforeShow && !this.options.beforeShow()) return;
	
	        /**
	         * 获得某一元素的透明度。IE从滤境中获得。
	         *
	         * @return float
	         */
	        var getOpacity = function(id){
	            if(!me.isIe) {   return $('#' + id).css('opacity');    }
	
	            var el = document.getElementById(id);
	            return (undefined != el
	                    && undefined != el.filters
	                    && undefined != el.filters.alpha
	                    && undefined != el.filters.alpha.opacity)
	                ? el.filters.alpha.opacity / 100 : 1;
	        }
	        /* 是否显示背景遮罩层 */
	        if(this.options.modal){
				$('#' + this.overlayId).fadeTo('slow', getOpacity(this.overlayId));
			}
			
			this.dialog.fadeTo('slow', getOpacity(this.options.id), function(){
				if(undefined != me.options.afterShow){
	            	me.options.afterShow();
				}
				me.isShow = true;
			});
			
			//检测最小高宽
			var contentDiv = this.dialog.find('.dialog_content');
			if (contentDiv.find('.loading').length == 0) {
				if (contentDiv.width() < 240) 
					contentDiv.width('240px');
				if (contentDiv.height() < 100) 
					contentDiv.height('100px');
			}
			
			//ie7 ie6 设置宽度
		//	if($.browser.version== '7.0'||$.browser.version== '6.0'){this.dialog.find('.dialog_title').css('width',this.dialog.find('.dialog_content').width()-15);}
	        
			// 自动关闭 
	        if(0 != this.options.time){  this.timeId = setTimeout(this.close, this.options.time);}
			
			
	        this.resetPos();
	    },
	
	
	    /**
	     * 隐藏对话框。但并不取消窗口内容。
	     */
	    hide : function(){
	    	var me = this;
	        if(!this.isShow){ return;}
	
	        if(undefined != this.options.beforeHide && !this.options.beforeHide()) return;  
	
	        this.dialog.fadeOut('slow', function(){
	            if(undefined != me.options.afterHide){   me.options.afterHide(); }
	        });
	        if(this.options.modal){   $('#' + this.overlayId).fadeOut('slow');   }
	
	        this.isShow = false;
	    },
	
	    /**
	     * 关闭对话框 
	     *
	     * @return void
	     */
	    close : function() {
	    	var me = this;
	        if (this.options.beforeClose != undefined && this.options.beforeClose != null && !this.options.beforeClose(this.closeStatus)) {
				return;
			}
	
	        this.dialog.fadeOut('slow', function(){
	            $(me.dialog).remove();
	            me.isShow = false;
	            if(me.options.afterClose != undefined && me.options.afterClose != null){
					me.options.afterClose(me.closeStatus);
				}
	        });
	        if(this.options.modal){
				$('#'+this.overlayId).fadeOut('slow', function(){$(this).remove();});
			}
	        clearTimeout(this.timeId);
			__dialogs['dialog' + this.options.id] = null;
	    },
		
		/**
	     * 移除对话框 
	     *
	     * @return void
	     */
		remove : function(){
			if(this.options.modal){
				$('#'+this.overlayId).remove();
			}
			this.dialog.remove();
		}
	}
	
	var __zindex = 50000;
	var __count = 1;
	var __version = '1.0 beta';
	var __dialogs = {};
	var __lastDialog = null;
	var __defaults = { // 默认值。 
		id:false,              // 对话框的id，若为false，则由系统自动产生一个唯一id。 
	    title:'http://337.eleximg.com/337/v3static/js/337.COM',       // 标题文本
	    showTitle:true,        // 是否显示标题栏。
	    closeText:'CLOSE X',         // 关闭按钮文字，若不想显示关闭按钮请通过CSS设置其display为none 
	    showClose:true,       // 是否显示关闭按钮
	    draggable:true,        // 是否可移动
	    modal:true,            // 是否是模态对话框 
	    center:true,           // 是否居中。 
	    fixed:true,            // 是否跟随页面滚动。
	    time:0,                // 自动关闭时间，为0表示不会自动关闭。 
	    buttons:'',        // 展示的按钮，可选：close,ok,cancel,yes,no
	    beforeClose:null,      // 关闭前回调函数，return false,则不关闭
	    afterClose:null        // 关闭后回调函数，当窗口被关闭时回调函数, 返回码  0:close, 1:ok, 2:cancel, 3:yes, 4:no, 5:other
	};
	
	elex.Dialog.setConfig = function(options) {
		elex.extend(__defaults, options);
	}
	
	elex.Dialog.show = function(content, options) {
		if(__lastDialog) {
            __lastDialog.remove();
            __lastDialog = null;
        }
		options = options || [];
		if(options.buttons != undefined && options.buttons != '') {
			if(options.showClose == undefined) options.showClose = false;
		}
		__lastDialog = new elex.Dialog(content, options);
		__lastDialog.show();
		return __lastDialog;
	}
	
	elex.Dialog.close = function(dialog_id){
		var dialog = dialog_id == undefined ? __lastDialog : __dialogs['dialog' + dialog_id];
		if (dialog && dialog.close) {
			dialog.close();
		}
	}
	
	elex.Dialog.resetPos = function(){
		__lastDialog.resetPos();
	}
	
	elex.namespace('elex.dialog');
	elex.dialog.show =  elex.Dialog.show;
	elex.dialog.close =  elex.Dialog.close;
	elex.dialog.resetPos =  elex.Dialog.resetPos;
	
	/* 警告窗口 */
	elex.dialog.alert = function(content, options) {
		options = options || {};
		elex.extend(options, {buttons:'ok'});
		elex.dialog.show(content, options);
	}
	
	/* 确认窗口 */
	elex.dialog.confirm = function(content, options) {
		options = options || {};
		elex.extend(options, {buttons:'ok,cancel'});
		elex.dialog.show(content, options);
	}
	
	/*兼容旧的问题*/
	//旧的命名空间
	elex.namespace('elex.widgets');
	elex.widgets.Dialog = elex.Dialog;
	elex.widgets.Dialog.setCompatible = function(){};
	//旧的jquery方法
	if (typeof(jQuery.dialog) == 'undefined') {
		var dialogs = {};
		jQuery.dialog = {
			box : function(id, title, content, position, callback) {
				var options = {
					id : id,
					title : title
				};
				elex.dialog.show(content, options);
			},
			close : function(id) {
				elex.dialog.close(id);
			}
		};
	}
})(jQuery);
