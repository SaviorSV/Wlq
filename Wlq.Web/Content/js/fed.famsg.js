if( $.browser.msie && $.browser.version === '6.0' ){
		document.execCommand( 'BackgroundImageCache', false, true ); 
}


(function($) {   
	$.famsg = $.famsg || {};
	
	var loadQueue = [],
		$numbtn,
		$numtab;
	
	$.extend($.famsg, {
		
		// 广告系统的图片队列
		loadQueue : function( data ){
			loadQueue.push( data );
			if( loadQueue[0] !== 'runing' ){
				$.famsg.loadDequeue();
			}
		},
			
		loadDequeue : function(){
			var fn = loadQueue.shift();
			if( fn === 'runing' ){
				fn = loadQueue.shift();
			}
			
			if( fn ){
				loadQueue.unshift( 'runing' );
				fn();
			}
		},	
	
	   //全屏广告
	   fullscreen:function(opt){
	       var defopt={"load":2000,"wait":3000,title:"",onStart:function(){},onEnd:function(){} };
		   if (opt!=null){ defopt=$.extend(defopt, opt);}	
		   var id="fullscreen_"+(new Date()).getTime();
		   var $full=$("<div id='"+id+"' class='fullscreenmsg'></div>").hide();
			$("body").prepend( $full);
		   var $closebtn=$("<a href='javascript:void(0);' target='_self' class='close'>X 关闭</a>").click(function(){ $full.slideUp('slow'); });
		 
		   
		   $full.append( $closebtn).append( $("<a class='adimg' href='"+defopt.link+"' target='_blank'><img src='"+defopt.img+"' alt='"+defopt.title+"' title='"+defopt.title+"' ></a>"));		  
		   
		    setTimeout(function(){ defopt.onStart();},defopt.load);
		   	setTimeout(function(){
  			    $full.slideDown('slow',function(){});
			},defopt.load);
			
		   	setTimeout(function(){
  			    $full.slideUp('slow',function(){
				    defopt.onEnd();
				});
			},(defopt.load+defopt.wait));   
		   
	   },
	   
	   
	   //右下角弹出消息框
	    popupmsg:function(obj,opt){
		    var defopt={'width':'300px','height':'180px','load':5000,'stop':4000,'speed':1500};	
			if (opt!=null){defopt=$.extend(defopt, opt);}	
			var winHeight=$.famsg.getWinHeight();
			
			$(obj).css("top",winHeight).css("width",defopt.width).hide();			
			$("<a class='close' href='javascript:void 0'>×</a>").appendTo($(obj)).click(function(){
					$(obj).hide();
			});
			
		
		  
	      //修复IE6的滚动时抖动的bug
	      if($.browser.msie && $.browser.version=="6.0"){
		     $("body").css({"background-image":"url(about:blank)","background-attachment":"fixed"});
		  }
		  
			
			//消息框上升
			
			setTimeout(function(){				 
				 $(obj).data("lastscoll",$.famsg.getWinHeight()).show();				
				 if ($.browser.msie){	
				     var step=5;
					 var counter=0;
					 
					 var timer=setInterval(function(){
					     var nowtop=parseInt($.famsg.getWinHeight())-step*counter;	
                         nowtop=Math.max(nowtop,parseInt($.famsg.getWinHeight())-parseInt(defopt.height));
						 counter++
                         if (nowtop<=parseInt($.famsg.getWinHeight())-parseInt(defopt.height))	{
						    clearInterval(timer);
							timer=null;
							
						     var realTop=parseInt(document.documentElement.clientHeight)-parseInt(defopt.height)
							   $(obj).attr("style"," top:expression(eval(document.documentElement.scrollTop + "+realTop+")); ");
							 
						 }else{
						   
						    var newscroll=parseInt($.famsg.getWinHeight());
							var lastscroll=parseInt($(obj).data("lastscoll"));
							var dis=newscroll-lastscroll;
							if (dis!=0){
							   
							   nowtop=nowtop+dis;							  
							   $(obj).data("lastscoll",newscroll);
							}
							
							$(obj).css("top",nowtop).show();	
						 }
					 },20);
					
					/*
					 
					 $(obj).css("top",winHeight);
					 
					   console.log("当前："+winHeight+" 目标："+ (parseInt($.famsg.getWinHeight())-parseInt(defopt.height))  );
					 $(obj).animate( {top:parseInt($.famsg.getWinHeight())-parseInt(defopt.height) },
					                 {duration:defopt.speed,	
									  	step:function(now,fx){	
										var data = fx.elem.id + ' ' + fx.prop + ': ' + now;
										var newscroll=parseInt($.famsg.getWinHeight());
										var lastscroll=parseInt($(obj).data("lastscoll"));
										var dis=newscroll-lastscroll;
										
										$(obj).data("lastscoll",newscroll);
										if (dis!=0){
											var oldtop=$(obj).css("top");
											var newtop=parseInt($(obj).css("top"))+dis;
											console.log(dis+":"+oldtop+"  -> "+newtop);
											$(obj).css("top",newtop).show();
										}
								      },				 
									  complete:function(){
											 //$(obj).css("top",$.famsg.getWinHeight()-defopt.height);
											 
											 var realTop=parseInt(document.documentElement.clientHeight)-parseInt(defopt.height);
											  console.log(realTop);
											 $(obj).attr("style"," top:expression(eval(document.documentElement.scrollTop + "+realTop+")); ");
											 
									  }
									 });	
									 
						*/			 
									 
									 
				 }else{
					 $(obj).animate( {top:parseInt($.famsg.getWinHeight())-parseInt(defopt.height) },
					                 {duration:defopt.speed,	
									  	step:function(now,fx){	
										var data = fx.elem.id + ' ' + fx.prop + ': ' + now;
										var newscroll=parseInt($.famsg.getWinHeight());
										var lastscroll=parseInt($(obj).data("lastscoll"));
										$(obj).data("lastscoll",newscroll);
								      },				 
									  complete:function(){
											 $(obj).css("top",$.famsg.getWinHeight()-defopt.height);
									  }
									 });	
				 
				 }			

                 				 
			},defopt.load);
			
			//消息框下降
			
		
			setTimeout(function(){
			  var winHeight=$.famsg.getWinHeight();	
			   $(obj).data("lastscoll",winHeight).show();
			   if($(obj).css("display")!="none"){
			   
				 if ($.browser.msie){		
					 var step=5;
					 var counter=0;
					 var timer=setInterval(function(){
					     var nowtop=parseInt($.famsg.getWinHeight()-parseInt(defopt.height))+step*counter;	
                         nowtop=Math.min(nowtop,parseInt($.famsg.getWinHeight()));
						 counter++
                         if (nowtop>=parseInt($.famsg.getWinHeight()))	{
						    clearInterval(timer);
							timer=null;							
						    
							 $(obj).hide();
							
						 }else{
						   
						    var newscroll=parseInt($.famsg.getWinHeight());
							var lastscroll=parseInt($(obj).data("lastscoll"));
							var dis=newscroll-lastscroll;
							if (dis!=0){
							   nowtop=nowtop+dis;							  
							   $(obj).data("lastscoll",newscroll);
							}
							
							$(obj).css("top",nowtop).show();	
						 }
					 },20);
					
					
				 }else{		
				  
				   $(obj).animate( {top: parseInt($(obj).css("top"))+parseInt(defopt.height) },
					                 {duration:defopt.speed,	
									  	step:function(now,fx){	
										    var data = fx.elem.id + ' ' + fx.prop + ': ' + now;
								           
											var newscroll=parseInt($.famsg.getWinHeight());
											var lastscroll=parseInt($(obj).data("lastscoll"));
											$(obj).data("lastscoll",newscroll);
											
								        },				 
									  complete:function(){
											 $(obj).css("top",$.famsg.getWinHeight()-defopt.height);
									  }
									 });	
				 }
			   }
			},defopt.load+defopt.stop);
			
			
	   },	     
	   //计算窗口高度+鼠标滚轮的高度
	   getWinHeight:function(){
	      var scrollTop=Math.max(document.documentElement.scrollTop,document.body.scrollTop);
		  var screenHeight=document.documentElement.clientHeight;
		  return scrollTop+screenHeight;
	   },	  
	   
	   /**
	   * 横向滚动字幕
	   */
	   scrollertxt:function(obj,opt){
	     var _opt={
			 "boxwidth":"auto",
			 "wait":8000,
			 "speed":2000,
			 "data":''};
		 if (opt!=null){ defopt=$.extend(_opt, opt);}
		 var id="scollertxt"+(new Date()).getTime();
		 
		 var _datahtml="";
		 //alert(_opt.data.length);
			$(_opt.data).each(function(i){
				  _datahtml  += "<li style='width:"+_opt.boxwidth+"px'><a href='"+this.url+"' target='_blank'>"+this.title+"</a></li>";
				});
		 var _obj = $(obj);	
		 
		 //alert($(_opt.data).length);
		 	 
		 var $ul=$("<ul style='width:"+_opt.boxwidth*3+"px' id='"+id+"' >"+_datahtml+"</ul>");
		 var $infobox = $("<div style='overflow:hidden;width:"+_opt.boxwidth+"px'></div>").append($ul);
		  
		  _obj.append($infobox);		 
		 var $leftbtn=$("<a href='#' class='scollleft' data='"+id+"' id='btn_l_"+id+"' ></a>");
		 var $rightbtn=$("<a href='#' class='scollright' data='"+id+"'></a>");
		 
		 _obj.append($leftbtn).append($rightbtn);	
		 $ul.data("state","0");
		 if ($(_opt.data).length > 1){
		 $ul.attr("timer", setInterval("$.famsg.scrollerLeft('"+id+"',"+_opt.speed+",-"+_opt.boxwidth+")",_opt.wait)  );};
		 
		// _obj.mouseover(function(){}).mouseout(function(){alert('123');});
		 
		 _obj.find('li').mouseover(function(){
						clearInterval($(this).parent().attr("timer"));
			 }).mouseout(function(){				 
					 $(this).parent().find('li').css('margin-left','0px');
					// alert($(this).parent().attr("id"));
					 $(this).parent().attr("timer", setInterval("$.famsg.scrollerLeft('"+$(this).parent().attr('id')+"',"+_opt.speed+",-"+_opt.boxwidth+")",_opt.wait)  );						 
				});;
		 
		 $leftbtn.click(function(){						
						var ulid = null;
						var ulid=$(this).attr("data");
						if ($("#"+ulid).data("state")=="0"){	
							$("#"+ulid).find('li').css('margin-left','0px');
							clearInterval($("#"+ulid).attr("timer"));
							$("#"+ulid).attr("timer",null);
							$.famsg.scrollerLeft(ulid,500,'-'+_opt.boxwidth);
						}						
				}).mouseover(function(){				
				       var ulid=$(this).attr("data");
						clearInterval($("#"+ulid).attr("timer"));
				}).mouseout(function(){
					 var ulid=$(this).attr("data");					 
					 $("#"+ulid).find('li').css('margin-left','0px');
					 $("#"+ulid).attr("timer", setInterval("$.famsg.scrollerLeft('"+ulid+"',"+_opt.speed+",-"+_opt.boxwidth+")",_opt.wait)  );						 
				});
		 
		 $rightbtn.click(function(){
	               var ulid = null;
						var ulid=$(this).attr("data");
						if ($("#"+ulid).data("state")=="0"){	
							$("#"+ulid).find('li').css('margin-left','0px');
							clearInterval($("#"+ulid).attr("timer"));
							$("#"+ulid).attr("timer",null);
							$.famsg.scrollerRight(ulid,500,'-'+_opt.boxwidth);
						}
				}).mouseover(function(){				
				       var ulid=$(this).attr("data");
						clearInterval($("#"+ulid).attr("timer"));
				}).mouseout(function(){
					 var ulid=$(this).attr("data");					 
					 $("#"+ulid).find('li').css('margin-left','0px');
					 $("#"+ulid).attr("timer", setInterval("$.famsg.scrollerLeft('"+ulid+"',"+_opt.speed+",-"+_opt.boxwidth+")",_opt.wait)  );						 
				});
		 
		 
	   },
	   scrollerLeft:function(id,speed,scrollnum){
		   //alert(scrollnum);
		    var ulobj=$("#"+id);
			//ulobj.find('li').css('margin-left','0px');
			var _li = null;
			var _li =  ulobj.find('li:first');
			if (ulobj.data("state")=="0"){			
			   ulobj.data("state","1");
				//ulobj.find('li').css('margin-left','0px');
				_li.animate({marginLeft:scrollnum},speed,function(){
					//_li.css('color','#ff0000');
					_li.css('margin-left','0px');
					ulobj.append(_li);
					//_li.css('margin-left','0px');
					ulobj.find('li').css('margin-left','0px');				
					ulobj.data("state","0");
				});
			
			}
			
	   },
	   scrollerRight:function(id,speed,scrollnum){
			var ulobj=$("#"+id);
			//ulobj.find('li').css('margin-left','0px');
			
			 var _lastli =  ulobj.find('li:last');
			 _lastli.css('margin-left',scrollnum);
			 ulobj.prepend(_lastli);
			if (ulobj.data("state")=="0"){			
			   ulobj.data("state","1");
				
				_lastli.animate({marginLeft:'0px'},speed,function(){					
					ulobj.find('li').css('margin-left','0px');				
					ulobj.data("state","0");
				});
			
			}			
	   },
	   //首页轮播广告
	   lunbo:function(id,opt){
		 var defopt={width:520,height:260,interval:3000,type:1};	
		 if (opt!=null){defopt=$.extend(defopt, opt);}	
		 var data= defopt.data;
		 var $piclist=$("<ul class='pic-list'></ul>");
		 var $tablist=$("<ul class='num'></ul>");
		 var len=data.length;
		 var tabwidth= (defopt.width-len)/len;		
		 
		 //组合结构
		 for (var i=0;i<len;i++){
		     var item=data[i];
		     //var $pic=$("<li index='"+i+"'><a href='"+item.link+"' target='_blank'><img src='"+item.img+"' name='lunboimg' style='width:"+defopt.width+"px; height:"+defopt.height+"px;' /></a></li>");
			 //var $tab= $("<li index='"+i+"' style='width:"+tabwidth+"px;'><a href='"+item.link+"' target='_blank'>"+item.title+"</a><span></span></li>");
			 
			 var $pic=$("<li index='"+i+"'><a href='"+item.link+"' target='_blank'><img src='"+item.img+"' name='lunboimg' usemap='#"+item.mapid+"' style='width:"+defopt.width+"px; height:"+defopt.height+"px;' /></a></li>");
			 var $tab= $("<li index='"+i+"' style='width:"+tabwidth+"px;'><a href='"+item.link+"' target='_blank'>"+item.title+"</a><span></span></li>");
			 
			 $piclist.append($pic);
			 $tablist.append($tab);
		 } 
		 
		 //添加到目标DIV
		 var $box
		 if (id.indexOf("#")==0){
		   $box=$(id);
		 }else{
		   $box=$("#"+id);
		 }
		 
		 
		 
		 $box.append($("<div class='adlunbo_style_"+defopt.type+"' style='width:"+defopt.width+"px; height:"+defopt.height+"px;'></div>").append($tablist).append($piclist));		 
		 
		 $tablist.find("li").eq(0).addClass("current");
		 $box.data("curr",0);	
		 $box.data("auto",1);	
		 
		 //内部函数，根据索引显示相应的li
		 var show=function(i){
		    var tab=$tablist.find("li").eq(i);
		    if (!$(tab).hasClass("current")){
				$tablist.find("li").removeClass("current");
				$(tab).addClass("current");
				$box.data("curr",i);			
				$piclist.find("li").hide();				
				$piclist.find("li").eq(i).fadeIn();				
			}
		 }
		 
		 //Tab鼠标悬浮事件绑定
		 $tablist.find("li").mouseover(function(){
		    $box.data("auto","0");
			var i=$(this).attr("index");
			show(i);			
		 }).mouseout(function(){
			$box.data("auto","1");		
		 });
		 
		
		 
		 
		 //图片鼠标悬浮事件绑定
		 $piclist.find("li").mouseover(function(){
		    $box.data("auto","0");
		 }).mouseout(function(){
			$box.data("auto","1");
		 });
		 
		 //定时执行轮播
		 var timer=setInterval(function(){
			if (parseInt($box.data("auto"))==1 ){
			  var index= $box.data("curr");
			  var next= parseInt(index)+1;
			  if (next>=len){next=0;}
			  show(next);
			} 
		 },defopt.interval);
		 
	   },
	   
	    /**
		 * 科捷广告系统的广告无阻塞加载器
		 * @param { jQuery Object } 包含了textarea的jQuery对象
		 */
	   	loadKjImg : function( elem ){
			var url = elem.val().match( /src="([\s\S]*?)"/i )[1],
				parent = elem[0].parentNode,
				docWrite = document.write,
				script = document.createElement( 'script' ),
				head = document.head || 
					document.getElementsByTagName( 'head' )[0] || 
					document.documentElement;
					
			document.write = function( text ){
				parent.innerHTML = text;
			};

			script.type = 'text/javascript';
			script.src = url;
			
			script.onerror = script.onload = script.onreadystatechange = function( e ){
				e = e || window.event;
				if( !script.readyState || 
				/loaded|complete/.test(script.readyState) ||
				e === 'error'
				){
					script.onerror = script.onload = script.onreadystatechange = null;
					document.write = docWrite;
					head.removeChild( script );
					head = script = elem = parent = null;
					$.famsg.loadDequeue();
				}
			}
			
			head.insertBefore( script, head.firstChild );
		},
	 
	    lunbolazy:function(id,opt){	
		    var defopt={interval:4000};	
		     if (opt!=null){defopt=$.extend(defopt, opt);}	
            var $box=	$(id);	
			var len=$box.find("li").length;
			$numtab=$("<ul class='num'></ul>");
			for (var i=0;i<len;i++){  
				var clsname="";
				if (i==0){clsname=" class='current' ";}
				$numbtn=$("<li style='width: 129px;' index='"+i+"' "+clsname+"><a  href='javascript:;' target='_self'>"+(i+1)+"</a><span></span></li>");
				$numbtn.mouseover(function(){
				
				 $numtab.find("li").removeClass("current");
				 $(this).addClass("current");
				 $box.find("li").hide();
				 
				var currli=$box.find("li").eq($(this).attr("index")),
					cache_textarea=$(currli).find("textarea");
					
				 if( cache_textarea.length ){
				    (function( elem ){
						$.famsg.loadQueue(function(){
							$.famsg.loadKjImg( elem );
						});
					})( cache_textarea );					 
				 }
				 
				 $(currli).fadeIn();
			     $box.data("auto","0");
				 $box.data("curr",$(this).attr("index"));
			  });
			  $numtab.append($numbtn);			  
			}			
			$box.before($numtab);			
			$box.data("auto",1);
			$box.data("curr",0);
			
			 
			 //图片鼠标悬浮事件绑定
			 $box.find("li").mouseover(function(){
				$box.data("auto","0");
			 }).mouseout(function(){
				$box.data("auto","1");
			 });
			 
			 //定时执行轮播
			 
			 
			 var timer=setInterval(function(){
				if (parseInt($box.data("auto"))==1 ){
				  var index= $box.data("curr");
				  var next= parseInt(index)+1;
				  if (next>=len){next=0;}
				  show(next);
				} 
			 },defopt.interval);
			 
			 
			  //内部函数，根据索引显示相应的li
		    var show=function(i){			    
				var tab=$numtab.find("li").eq(i);				
				if (!$(tab).hasClass("current")){
					$numtab.find("li").removeClass("current");
					$(tab).addClass("current");
					$box.data("curr",i);			
					$box.find("li").hide();				
					
					var currli=$box.find("li").eq(i),
						cache_textarea=$(currli).find("textarea");
						
					if( cache_textarea.length ){
						$.famsg.loadQueue(function(){
							$.famsg.loadKjImg( cache_textarea );
						});			 
					}
					
					$(currli).fadeIn();
					
					//$box.find("li").eq(i).fadeIn();						 
				}
		    }
		 
		 
			
		}, 
	   
	   //固定坐标位不动的悬浮层
	   floatdiv:function(id,opt){
	      var o={offset:480,top:100};	
		  if (opt!=null){o=$.extend(o, opt);}	
		  
		  var offset=  o.offset;
		  if (offset<0){		   
		    offset=offset-$(id).width();
		  }
		  if($.browser.msie && $.browser.version=="6.0"){
		     $("body").css({"background-image":"url(about:blank)","background-attachment":"fixed"});			
			 $(id).attr("style","position:absolute;  left:50%; top:expression(eval(document.documentElement.scrollTop + "+o.top+")); margin-left:"+offset+"px;");
		  }else{
		     $(id).css({'position':'fixed','left':'50%','top':o.top+'px','margin-left':offset+'px'});
		  }
	   },
	   
	   /**
		* 首页右侧悬浮浏览历史数据  $.famsg.showFlotHistory()
		* 参数说明：
		* 	signup：0 未登录，1：已经登录  必填
		* 	msgcount: 未读站内信数量  (当signup 为1时必须传此值) 
		*	signupurl: 默认：http://user.5173.com/UserRegister.aspx  可修改，非必填
		*	loginurl: 默认 https://passport.5173.com/?returnUrl=http://www.5173.com  可修改，非必填
		*	msgurl: 默认 http://message.5173.com/MyInfo/SiteMessageList.aspx?TagType=0, 可修改，非必填
		*/
	   showFlotHistory:function(opt){
	      var o={'signupurl':'https://passport.5173.com/User/Register',
				 'loginurl':'https://passport.5173.com/?returnUrl=http://www.5173.com',
				 'msgurl':'http://message.5173.com/MyInfo/SiteMessageList.aspx?TagType=0',
				 'sound':0,
				 'msgcount':0,
				 'signup':0};	
		  if (opt!=null){o=$.extend(o, opt);}	
		  
	      var $his=$("<div id='browsehistory' class='browse_history'></div>");
		  var html=new Array();
		  //登录部分
		  html.push("<div class='login_state'>");
		  if (o.signup==0){
		    html.push("<ul><li><a href='"+o.signupurl+"'>注册</a></li> <li><a href='"+o.loginurl+"'>登录</a></li> </ul>");
		  }else{
		    html.push(" <a href='"+o.msgurl+"' class='messages_info'>未读站内信<span class='no'>"+o.msgcount+"</span></a>");
		  }
		  html.push(" </div>");
        
          //最近浏览
		  var items=o.visited;
		  
		  if (items && items.length>0 ){
			  html.push("<div class='late_browse'><h4>最近浏览</h4><ul>");		 
			  for(var i=0;i<items.length;i++){
				var item=items[i];
				 html.push("<li><span class='name'><a href='"+item.title_a+"' title='"+item.title+"'>"+item.title+"</a></span>");
				 html.push("  <span class='info'><a href='"+item.type_a+"' title='"+item.type+"'>"+item.type+"</a></span>");
				 html.push("  <span class='price'><a href='"+item.price_a+"' title='"+unescape(item.price_til)+"'>"+item.price+"</a></span></li>");
			  }
			  html.push("</ul><b class='bottom'></b></div>");
		  }
          
          //快速购买
		  html.push("<div class='quick_buy'><h4>快速购买</h4><ul>");
		  html.push("<li class='card'><a onclick=\"__utmTrackEvent('%e9%a6%96%e9%a1%b5%e7%82%b9%e5%87%bb','%e5%bf%ab%e9%80%9f%e8%b4%ad%e4%b9%b0','%e7%82%b9%e5%8d%a1');\"    href='http://dkjy.5173.com/BuyIndex.aspx' target='_blank'>点卡</a></li>");
		  html.push("<li class='phone'><a  onclick=\"__utmTrackEvent('%e9%a6%96%e9%a1%b5%e7%82%b9%e5%87%bb','%e5%bf%ab%e9%80%9f%e8%b4%ad%e4%b9%b0','%e6%89%8b%e6%9c%ba');\"  href='http://chong.5173.com/' target='_blank'>手机</a></li>");
		  html.push("<li class='qq'><a onclick=\"__utmTrackEvent('%e9%a6%96%e9%a1%b5%e7%82%b9%e5%87%bb','%e5%bf%ab%e9%80%9f%e8%b4%ad%e4%b9%b0','QQ');\"  href='http://trading.5173.com/search/0725c34d0d424b8898465c86a28f6bac.shtml?ga=&gs=' target='_blank'>QQ增值</a></li>");
		  
		  //html.push("<li class='lottery'><a href='http://gg.5173.com/adpolestar/wayl/;ad=DF42564A_E521_FAA1_231F_8550A77413C9;ap=0;pu=5173;/?http://www.658.com/ProIndex.aspx?proid=106622' target='_blank'>彩票</a></li>");
		  
		  html.push("<li class='gamegold'><a href='http://dkhg.5173.com/' target='_blank' onclick=\"__utmTrackEvent('%E9%A6%96%E9%A1%B5%E7%82%B9%E5%87%BB','%E5%BF%AB%E9%80%9F%E8%B4%AD%E4%B9%B0','%E5%8D%A1%E6%8D%A2%E5%B8%81');\">卡换币</a></li>");
		  
		  html.push("<li class='xy'><a onclick=\"__utmTrackEvent('%e9%a6%96%e9%a1%b5%e7%82%b9%e5%87%bb','%e5%bf%ab%e9%80%9f%e8%b4%ad%e4%b9%b0','%e8%bf%85%e6%b8%b8');\"  href='http://tool.5173.com/xunyou/index.aspx' target='_blank'>加速器</a></li>");
		  html.push("<li class='jdyou'><a href='http://tool.5173.com/jdyou/index.aspx' target='_blank'>网游工具</a></li>");
		   html.push("<li class='qp_trad'><a href='http://zzjy.5173.com/' target='_blank'>棋牌游戏</a></li>");
		  html.push("</ul><b class='bottom'></b></div>");
		  
		  var $gotoTop=$(" <a href='javascript:void(0)' class='browse_gotop' style='display:none;'></a>");
	
		  
          $his.html(html.join("")).append($gotoTop).appendTo($("body"));
		  
		  $.famsg.floatdiv($his,{top:100,offset:480});
		  
		  $gotoTop.click(function(){
			$(window).scrollTop(0);
		   });
		   $(window).scroll(function(){
			  if ($(window).scrollTop()>0){
				$gotoTop.show();
			  }else{
			    $gotoTop.hide();
			  }
		   });
		   
	   
	   },
	   
	   /**
	   * 右下角弹窗，延时变小窗
	   * 接口名称： $.famsg.popright();
	   * 参数说明：
	   *        big：大图片设定  {img:图片URL，link：链接地址}  ，必填
	   *        small: 小图片设定 {img:图片URL，link：链接地址}  ，必填
	   *        delay: 延时变小窗的时间，默认是 5000 ms ，非必填
	   *        zindex: 弹窗的z-index的属性，默认是 1000 ，非必填 （遇到弹窗冲突的时候，可以修改此值）
	   */
	   popright:function(opt){
	      var o={delay:5000,zindex:1000};	
		  if (opt!=null){o=$.extend(o, opt);}	
		  
	      //修复IE6的滚动时抖动的bug
	      if($.browser.msie && $.browser.version=="6.0"){
		     $("body").css({"background-image":"url(about:blank)","background-attachment":"fixed"});
		  }
		 
		  var $big=$("<div  class='popr-large' style='z-index:"+o.zindex+"'></div>");
		  var $small=$("<div  class='popr-small' style='z-index:"+(o.zindex+1)+"'></div>");
		   
		  //生成大窗 
		  var $bigimg=$("<a href='"+o.big.link+"' class='imgborder' target='_blank'><img src='"+o.big.img+"' /></a>");
		  var $bigclose=$("<a href='javascript:void(0);' target='_self' class='close'>×</a>").click(function(){
		      $big.hide();
			  $small.show();
		  });
		  
		  $big.append($bigimg).append($bigclose).appendTo($("body"));
		  
		  //生成小窗
		  var $smallimg=$("<a href='"+o.small.link+"' class='imgborder'  target='_blank'><img src='"+o.small.img+"' /></a>");
		  var $smallclose=$("<a href='javascript:void(0);' target='_self' class='close'>关闭</a>").click(function(){
		      $small.hide();
		  });
		  var $smallreturn=$("<a href='javascript:void(0);' target='_self' class='replay'>重播</a>").click(function(){
		       $small.hide();
			   $big.show();
		  });
		  $small.append($smallimg).append($smallclose).append($smallreturn).appendTo($("body"));
		  
		  //修复IE6的滚动的bug
		  if($.browser.msie && $.browser.version=="6.0"){
		     var bigtop=$big.offset().top;			 
			 $big.attr("style"," top:expression(eval(document.documentElement.scrollTop + "+bigtop+")); ");
			 var smailtop=$small.offset().top;	
			 $small.attr("style"," top:expression(eval(document.documentElement.scrollTop + "+smailtop+")); ");
		  }
		  
		  $small.hide();
		  
		  //延时
		  setTimeout(function(){ $bigclose.click();},o.delay);		  
		  
	   }
	   
	   
	});
})(jQuery);
