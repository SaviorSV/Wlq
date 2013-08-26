function selected_left_menu(index) {
	var li = $('#leftmenu li:eq(' + index + ')');

	li.addClass('on');
}

function upload_file(type) {
	if (validateFile($('#' + type + '_uploader'), type)) {
		$.ajaxFileUpload({
			url: '/Common/Upload',
			secureuri: false,
			fileElementId: type + '_uploader',
			dataType: 'json',
			data: {
				'type': type
			},
			success: function (r, status) {
				if (r.Error == 0) {
					switch (type) {
						case 'logo':
						case 'post':
							if (r.Url != '' && r.Extention != '') {
								$('#' + type + '_img').attr('src', r.Url);
								$('#' + type + '_hidden').val(r.Extention);
							}
							break;
						case 'file':
							break;
						default:
							return false;
					}
				}
				else
					alert(r.Message)
			}
		});
	}
}

function validateFile(obj, type) {
	var file = obj;

	if (file.val() == '') {
		alert('请选择上传的文件!');
		return false;
	}

	switch (type) {
		case 'logo':
		case 'post':
			if (!(/^.*?\.(gif|png|jpg|jpeg)$/.test(file.val().toLowerCase()))) {
				alert('只能上传jpg、jpeg、png或gif格式的图片！');
				return false;
			}
			break;
		case 'file':
			if (!(/^.*?\.(doc|docx|zip|rar|7z)$/.test(file.val().toLowerCase()))) {
				alert('只能上传doc、docx、zip、rar或7z格式的文件！');
				return false;
			}
			break;
		default:
			return false;
	}

	return true;
}