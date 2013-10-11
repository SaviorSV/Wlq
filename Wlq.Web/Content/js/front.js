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
		var kCode = e.keyCode || e.charCode;

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

function booking_confirm(postId) {
	var offsetTop = $("#btn_booking_" + postId).offset().top - 560;

	$("#popup").css("top", offsetTop + "px");
	$("#popup").show();
	$("#chk_remark_confirm").attr("checked", false);
	$("#btn_remark_confirm").attr("onclick", "normal_booking(" + postId + ")");
}

function normal_booking(postId) {
	if (!$("#chk_remark_confirm").attr("checked")) {
		alert('请勾选同意安全管理须知');
		return;
	}

	var involvedType = $('input:radio[name=involvedType][checked]').val();

	if (!involvedType) {
		alert('请选择参与形式');
		return;
	}

	if (!confirm('确认预订？')) {
		return;
	}

	$("#popup").hide();

	$.ajax({
		type: "POST",
		url: '/Common/Booking',
		dataType: 'json',
		data: {
			'postId': postId,
			'bookingDate': GetToday(),
			'involvedType': involvedType
		},
		success: function (r, status) {
			if (r.Success) {
				var button = $("#btn_booking_" + postId);

				button.removeClass('btn');
				button.addClass('btn-disable');
				button.html('取消预约');
				button.attr("onclick", "normal_cancel_booking(" + postId + ")");

				var bookingNumber = parseInt($("#booking_number_" + postId).html());
				var bookingNumberLeft = parseInt($("#booking_number_left_" + postId).html());
				
				$("#booking_number_" + postId).html(++bookingNumber);
				if (bookingNumberLeft > 0) {
					$("#booking_number_left_" + postId).html(--bookingNumberLeft);
				}
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
				button.attr("onclick", "booking_confirm(" + postId + ")");

				var bookingNumber = parseInt($("#booking_number_" + postId).html());
				var bookingNumberLeft = parseInt($("#booking_number_left_" + postId).html());

				$("#booking_number_left_" + postId).html(++bookingNumberLeft);
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

/******************* 轮播 *******************/

var banner_t = banner_n = 0, banner_count;

$(document).ready(function () {
	banner_count = $("#banner_list a").length;
	$("#banner_list a:not(:first-child)").hide();
	$("#banner_info").html($("#banner_list a:first-child").find("img").attr('alt'));
	$("#banner_info").click(function () { window.open($("#banner_list a:first-child").attr('href'), "_blank") });
	$("#banner li").click(function () {
		var i = $(this).text() - 1; //获取Li元素内的值，即1，2，3，4 
		banner_n = i;
		if (i >= banner_count) return;
		$("#banner_info").html($("#banner_list a").eq(i).find("img").attr('alt'));
		$("#banner_info").unbind().click(function () { window.open($("#banner_list a").eq(i).attr('href'), "_blank") })
		$("#banner_list a").filter(":visible").fadeOut(500).parent().children().eq(i).fadeIn(1000);
		$(this).css({ "background": "#1f8fcf", 'color': '#fff' }).siblings().css({ "background": "#999", 'color': '#fff' });
	});
	banner_t = setInterval("showAuto()", 4000);
	$("#banner").hover(function () { clearInterval(banner_t) }, function () { banner_t = setInterval("showAuto()", 4000); });
});

function showAuto() {
	banner_n = banner_n >= (banner_count - 1) ? 0 : ++banner_n;
	$("#banner li").eq(banner_n).trigger('click');
}

/******************* 轮播 *******************/
