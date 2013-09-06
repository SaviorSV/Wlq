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
		window.location = pageUrl + "pageIndex=" + newPage;
	});

	$("#next").bind("click", function () {
		var newPage = pageIndex + 1;
		window.location = pageUrl + "pageIndex=" + newPage;
	});

	$("#pager_redirect").bind("click", function () {
		var newPage = $("#pager_index").val();

		if ($.isNumeric(newPage) && newPage > 0 && newPage <= totalPage)
			window.location = pageUrl + "pageIndex=" + newPage;
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

function selected_myhome_menu(index) {
	$('#my_home_tags li:eq(' + index + ') a').addClass('on');
}

function GetToday() {
	var today = new Date();
	return today.getFullYear() + "/" + (today.getMonth() + 1) + "/" + today.getDate();
}

function normal_booking(postId) {
	if (!confirm('确认预订？')) {
		return;
	}

	$.ajax({
		type: "POST",
		url: '/Common/Booking',
		dataType: 'json',
		data: {
			'postId': postId,
			'bookingDate': GetToday()
		},
		success: function (r, status) {
			if (r.Success) {
				var button = $("#btn_booking_" + postId);

				button.removeClass('btn');
				button.addClass('btn-disable');
				button.html('取消预约');
				button.attr("onclick", "normal_cancel_booking(" + postId + ")");

				var bookingNumber = parseInt($("#booking_number_" + postId).html());
				$("#booking_number_" + postId).html(++bookingNumber);
			}
			else if (r.Message != '') {
				alert(r.Message)
			}
		}
	});
}

function normal_cancel_booking(postId) {
	if (!confirm('是否取消预约？')) {
		return;
	}

	$.ajax({
		type: "POST",
		url: '/Common/CancelBooking',
		dataType: 'json',
		data: {
			'postId': postId,
			'bookingDate': GetToday()
		},
		success: function (r, status) {
			if (r.Success) {
				var button = $("#btn_booking_" + postId);

				button.removeClass('btn-disable');
				button.addClass('btn');
				button.html('预 约');
				button.attr("onclick", "normal_booking(" + postId + ")");

				var bookingNumber = parseInt($("#booking_number_" + postId).html());

				if (bookingNumber > 0) {
					$("#booking_number_" + postId).html(--bookingNumber);
				}
			}
		}
	});
}

function join_group(groupId) {
	$.ajax({
		type: "POST",
		url: '/Common/JoinGroup',
		dataType: 'json',
		data: {
			'groupId': groupId
		},
		success: function (r, status) {
			if (r.Success) {
				var button = $("#btn_group_" + groupId);

				button.removeClass('btn-gz');
				button.addClass('qxgz');
				button.attr("onclick", "quit_group(" + groupId + ")");
			}
		}
	});
}

function quit_group(groupId) {
	if (!confirm('是否取消关注？')) {
		return;
	}

	$.ajax({
		type: "POST",
		url: '/Common/QuitGroup',
		dataType: 'json',
		data: {
			'groupId': groupId
		},
		success: function (r, status) {
			if (r.Success) {
				var button = $("#btn_group_" + groupId);

				button.removeClass('qxgz');
				button.addClass('btn-gz');
				button.attr("onclick", "join_group(" + groupId + ")");
			}
		}
	});
}

function concern_post(postId) {
	$.ajax({
		type: "POST",
		url: '/Common/ConcernPost',
		dataType: 'json',
		data: {
			'postId': postId
		},
		success: function (r, status) {
			if (r.Success) {
				var button = $("#btn_post_" + postId);

				button.removeClass('btn');
				button.addClass('btn-disable');
				button.html('取消关注');
				button.attr("onclick", "unconcern_post(" + postId + ")");
			}
		}
	});
}

function unconcern_post(postId) {
	if (!confirm('是否取消关注？')) {
		return;
	}

	$.ajax({
		type: "POST",
		url: '/Common/UnconcernPost',
		dataType: 'json',
		data: {
			'postId': postId
		},
		success: function (r, status) {
			if (r.Success) {
				var button = $("#btn_post_" + postId);

				button.removeClass('btn-disable');
				button.addClass('btn');
				button.html('关 注');
				button.attr("onclick", "concern_post(" + postId + ")");
			}
		}
	});
}
