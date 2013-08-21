$(function () {
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