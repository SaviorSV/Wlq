var pageIndex = 1;
var totalPage = 1;
var pageUrl = '/';

$(function () {
	$(document).ready(function () {
		if (pageIndex <= 1) {
			pageIndex = 1;
			$("#previous").hide();
		}

		if (pageIndex >= totalPage) {
			pageIndex = totalPage;
			$("#next").hide();
		}
	});

	$("#previous").bind("click", function () {
		var newPage = pageIndex - 1;
		window.location = pageUrl + "&pageIndex=" + newPage;
	});

	$("#next").bind("click", function () {
		var newPage = pageIndex + 1;
		window.location = pageUrl + "&pageIndex=" + newPage;
	});

	$("#pager_redirect").bind("click", function () {
		var newPage = $("#pager_index").val();

		if ($.isNumeric(newPage) && newPage > 0 && newPage <= totalPage)
			window.location = pageUrl + "/" + newPage;
		else
			alert('请输入正确的数字');
	});

	$("#user_login_form").keypress(function (e) {
		kCode = e.keyCode || e.charCode //for cross browser

		if (kCode == 13) {
			$("#btn_login").click();
			return false;
		}
	});

	$('#btn_login').bind("click", function () {
		if ($('#loginName').val() == '' || $('#password').val() == '') {
			alert('请输入用户名密码');
			return false;
		}
		else {
			$('#user_login_form').submit();
		}
	});
});