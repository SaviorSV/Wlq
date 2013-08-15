/**
 * jquer插件版居中弹出层(在fuyun的基础上改进)
 * @author fuyun, Mark
 * @version 1.0
 **/

//判断jquery是否加载，如果没有加载自动加载 jquery.js

(function($){
    $.extend({
        LAYER:{
            openID:'',    
            position:'',
			mt:'',
            parentIsLoad:true,
            /**
             * 在body下面生成弹出层的父级框架与庶罩层与iframe层
             * @param 父级
             * @returns null
             **/   
            init:function(pid){
                if(pid==undefined){pid='UED_box'};
                if(this.parentIsLoad){
                    $('<div id="'+ pid +'"></div>').appendTo("body");
                    $('<div class="UED_SHUCOVER_V1 UED_hide" id="UED_SHUCOVER_V1"><iframe class="UED_SHUCOVER_IFRAME_V1" id="UED_SHUCOVER_IFRAME_V1" src="about:blank"></iframe></div>').appendTo("body");                        
                    this.parentIsLoad = false;
                }                    
            },
            /**
             * 显示弹出层
             * @param json参数 
             * @returns null
             **/                
            show:function(json){                
                var def = {
                    overlay:{color:'#000',opacity:0.7}, //庶罩层颜色与透明度
					position:'fixed',
					mt:'200px',
                    layerContainer:'UED_LAYER_PARENT_FRAME_V1' //默认父级id
                }
                def = $.extend(def,json);               
                
                //排查错误
                if(!document.getElementById(def.id)){
                    alert('弹出层出错: 页面上没有发现id='+def.id);
                    return false;
                }
				
                this.init(def.layerContainer);  
				this.position = def.position; //设置定位模式
				this.mt = def.mt; //设置定位模式
                this.openID = json.id;                    
                this.setpos($('#'+ this.openID));                    
                //配置庶罩层                        
                this.is6FIX('100%');
                //把页面的的框架移到到body下面的父节点。防止在控件中使用弹出层是受外部样式影响
                //显示弹出层与庶罩层与iframe层
                $('#'+this.openID).prependTo($('#'+def.layerContainer));
				$('#'+this.openID).show();
                $('#UED_SHUCOVER_V1').css({'background-color':def.overlay.color,'opacity':def.overlay.opacity}).show();
            },
            /**
             * 设置弹出层在中间显示
             * @param jQuery Object
             * @returns null
             **/
            setpos:function(obj){
                obj.addClass('UED_LAYER_PARENT_V1');
                var h = obj.height();
                var w = obj.width();
                var mr = (h/2*-1) + 'px';
                var ml = (w/2*-1) + 'px';   
                obj.css({'margin-left':ml,'margin-top':mr});
                
                //处理判断当弹出层的高度大于可视区域时，弹出层不到固定在页面
                var vH= document.body.clientHeight==0 ? document.body.clientHeight : document.documentElement.clientHeight;                
				
				if(h > vH || this.position === 'absolute'){
					//alert(this.mt);
                    obj.css({top:this.mt,position:'absolute',marginTop:'0'});
                } 
            },
            /**
             * 关闭当前打开的弹出层
             * @returns null
             **/                
            close:function(){
                $('#'+this.openID).hide();
                $('#'+this.openID).removeClass('UED_LAYER_PARENT_V1');
                $('#UED_SHUCOVER_V1').hide();
                this.is6FIX('auto');
            },
            
            /**
             * IE6下面需要把html与 body高度设置为100%时弹出层才会有效.
             **/
            is6FIX:function(value){
                if($.browser.msie&&($.browser.version == "6.0")){
                    $('html').css({height:value});
                    $('body').css({height:value});
                }                
            }
        }
    });
})(jQuery);

