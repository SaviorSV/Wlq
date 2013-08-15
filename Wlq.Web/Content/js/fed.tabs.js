/**
*Tab组件
*@
*/

(function($) {
    $.fed = $.fed || {};	
	$.extend($.fed, {	  
	
	  /**
	  * 参数：id Tab对象的ID值
	  * 参数:opt   event
	  *		event: 绑定事件类型  click/mouseover
	  *		currindex: 初始显示的标签 默认是0 第一个
	  *		interval: 自动切换的时间(单位毫秒)，必须>0,不设表示不切换
	  * 示例：$.fed.tabs("#tabs",{event:"mouseover",currindex:1,interval:3000});
	  */
	  tabs:function(id,opt){
	    var defopt={event:"click",currindex:0,onSuccess:function(tabid){}};
		defopt=$.extend(defopt,opt);
		var $tabs= $(id).children("div[name='tabhead']").find("ul").find("li");    //  $(id+" > div[name='tabhead'] > ul > li");	
		if ($tabs.length==0){
		  $tabs=$(id+" > ul > li");
		}
		
		var $bodys=$(id).children("div[name='tabpanel']");
		if ($bodys.length==0){
			var $bodys=$(id+" > div[name!='tabhead']");
		}
		$tabs.eq(defopt.currindex).addClass("current");
		$bodys.hide().eq(defopt.currindex).show();
		
		
		
		//当前显示的Current的Index值保存起来
		$tabs.data("curr",defopt.currindex);
		
		//Tab标签的事件绑定
		$tabs.bind(defopt.event,function(){
			$tabs.removeClass("current");
			$(this).addClass("current");
		    var tabid=$(this).find("a").attr("href");
			
			if ($.browser.msie){
			   tabid= "#"+tabid.split("#")[1];
			}
			
			
			$bodys.hide();
			$(tabid).show();
			
	
			var ajaxUrl=$(this).find("a").attr("url");			
			
			if (ajaxUrl && $.trim($(tabid).html())==""){
				  $.get(ajaxUrl,{},function(data){				  
					$(tabid).html(data);
				  });
			}
			
			
			defopt.onSuccess(tabid);
			
			return false;
		});
		
		//消除Click引起的HREF书签bug
		$tabs.bind("click",function(){			
		    var tabid=$(this).find("a").attr("href");			
			if (tabid.substr(0,1)=="#"){
			   return false;
			}
		});
			
		/**
		* 如果设置了间隔时间，就自动轮播
		*/
		if (defopt.interval && defopt.interval>0){
		
		    $(id).data("auto","1");
		    
		   $(id).hover(
			   function(){
				 $(id).data("auto","0");
			   },
			   function(){
				 $(id).data("auto","1");
			   }
		   );
		   
		   
			var len=$tabs.length;	
			var show=function(i){
				$tabs.removeClass("current");
				$tabs.eq(i).addClass("current");
				$bodys.hide().eq(i).fadeIn();
				$tabs.data("curr",i);
				
				
			}
		 
		    //设定定时器
			
			var timer=setInterval(function(){	
			   
		       if (parseInt($(id).data("auto"))==1  &&   $tabs.length>1  ){
				   var index= $tabs.data("curr");
				   var next= parseInt(index)+1;
				   if (next>=len){next=0;} 
				   show(next);
			   }	
			 },defopt.interval);
		}
		
	  }
	});
  	
})(jQuery);
