function selected_left_menu(index) {
	var li = $("#leftmenu li:eq(" + index + ")");

	li.addClass("on");
}

function validateFile(obj, type) {
	var file = obj;

	if (file.val() == "") {
		alert("请选择上传的文件!");
		return false;
	}

	switch (type) {
		case "logo":
			if (!(/^.*?\.(gif|png|jpg|jpeg)$/.test(file.val().toLowerCase()))) {
				alert("只能上传jpg、jpeg、png或gif格式的图片！");
				return false;
			}
			break;
		case "file":
			if (!(/^.*?\.(doc|docx|zip|rar|7z)$/.test(file.val().toLowerCase()))) {
				alert("只能上传doc、docx、zip、rar或7z格式的文件！");
				return false;
			}
			break;
		default:
			return false;
	}

	return true;
}