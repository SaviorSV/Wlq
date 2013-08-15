//返回val的字节长度
function getByteLen(val) {
	var len = 0;
	for (var i=0;i<val.length;i++) { 
		var tempstr=val.substr(i,1);
		if (tempstr.match(/[^\x00-\xff]/ig)!=null){ //全角
			len=len+1;		
		}else{
			len=len+1;
		}
	}	
	return len;
}


//返回val在规定字节长度max内的值
function getByteVal(val, max) {
	var returnValue = '';
	var byteValLen = 0;
	for (var i = 0; i < val.length; i++) {
		var tempstr=val.substr(i,1);
		if (tempstr.match(/[^\x00-\xff]/ig) != null){
			byteValLen += 1;
		}else{
			byteValLen += 1;
		}	
		if (byteValLen > max){
			break;
		}	
			returnValue += tempstr;
	}
	
	return returnValue;
}


		   
function setByte(area,info){
	var _area = area;
	var _info = info;
	var _max = parseInt(_area.attr('maxlength'));
	
	var _val;
	_area.bind('keyup change', function() { //绑定keyup和change事件
	    
		if ($(_info).size() < 1) {//避免每次弹起都会插入一条提示信息
			_info.html(_max);
		}
		_val = $(this).val();
		
		_cur = getByteLen(_val);
		
		
		if (_cur == 0) {//当默认值长度为0时,可输入数为默认maxlength值
			_info.html(_max);
		} else if (_cur < _max) {//当默认值小于限制数时,可输入数为max-cur
			_info.html(_max - _cur);
		} else {//当默认值大于等于限制数时
			_info.html(0);
			$(this).val(getByteVal(_val,_max)); //截取指定字节长度内的值
		}
	});
}


