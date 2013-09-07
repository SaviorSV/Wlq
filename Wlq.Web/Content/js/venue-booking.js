(function ($) {
    var data = null;
    var period_begin = 2400;
    var period_end = 0;
    var thead_data = new Array();

    function initialize(option) {
        data = option
        for (var i in data) {
            thead_data[i] = data[i]['Date'];
            for (var j in data[i]['Periods']) {
                begin_time = parseInt(data[i]['Periods'][j]['BeginTime'])
                end_time = parseInt(data[i]['Periods'][j]['EndTime'])
                if (period_begin > begin_time) {
                    period_begin = begin_time;
                }
                if (period_end < end_time) {
                    period_end = end_time;
                }
            }
        }
    }

    function createHead() {
        var thead = $('<thead></thead>');
        var tr = $('<tr></tr>');
        tr.append($('<th></th>').addClass('none'));
        for (var i in thead_data) {
            str = thead_data[i].substring(5, 10);
            tr.append($('<th>' + str + '</th>'));
        }
        thead.append(tr);
        return thead;
    }

    function createBody() {
        var tbody = $('<tbody></tbody>');

        for (var i = period_begin; i < period_end; i += 100) {
            var tr = $('<tr></tr>');
            t = i / 100;
            t_period = t.toString() + ':00-' + (t + 1).toString() + ':00'
            tr.append($('<th>' + t_period + '</th>'));

            for (var j in data) {
                var rowspan = 0;
                var attrs = null;
                for (var k in data[j]['Periods']) {
                    var begin = parseInt(data[j]['Periods'][k]['BeginTime']);
                    var end = parseInt(data[j]['Periods'][k]['EndTime']);
                    row = getRow(i, begin, end);
                    rowspan = (row == 0) ? rowspan : row;
                    attr = getAttrs(i, data[j]['Periods'][k]);
                    attrs = (attr == null) ? attrs : attr;
                }

                var td = null;

                if (rowspan > 0) {
                    td = $('<td></td>').attr('rowspan', rowspan);
                }
                else if (rowspan == 0) {
                    td = $('<td></td>').addClass('off');
                }

                if (attrs != null) {
                    attrs['Date'] = data[j]['Date']
                    for (var key in attrs) {
                        td.attr(key, attrs[key]);
                    }

                    if (attrs['LimitNumber'] > attrs['BookingNumber'] && !attrs['IsBooked']) {
                        td.addClass('on');
                        td.text('预订');
                    }
                    else if (attrs['IsBooked']) {
                        td.addClass('already');
                        td.text('取消预订');
                    }
                    else {
                        td.addClass('off');
                        td.text('满');
                    }
                }

                tr.append(td);
            }

            tbody.append(tr)
        }
        return tbody
    }

    function getRow(t, begin, end) {
        if (t > begin && t < end)
            return -1;
        else if (t != begin) {
            return 0;
        }
        else {
            return (end - begin) / 100;
        }
    }

    function getAttrs(t, data) {
        if (t == parseInt(data['BeginTime'])) {
            var attrs = new Array();
            attrs['BookingNumber'] = data['BookingNumber'];
            attrs['IsBooked'] = data['IsBooked'];
            attrs['LimitNumber'] = data['LimitNumber'];
            attrs['VenueConfigId'] = data['VenueConfigId'];
            return attrs;
        }
        else
            return null;
    }

    $.fn.extend({
        booking: function (option) {
            initialize(option);

            var table = $('<table></table>').addClass('cd-detail-tbl');

            table.append(createHead());
            table.append(createBody());

            this.empty().append(table);
        }
    });
})(jQuery);
