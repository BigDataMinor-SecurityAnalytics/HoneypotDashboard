var recentAttacksCount = 500;
var geoAttackers = {};
var geoIpCountry = {};
var geoIpDomain = {};

$(function() {
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: "http://honeyaccess.azurewebsites.net/api/LogEntries",
        success: function(data) {
            $.each(data.value, function(key, val) {
                geoAttackers[val.RemoteIp] = geoAttackers[val.RemoteIp] ? geoAttackers[val.RemoteIp] + 1 : 1;
                geoIpCountry[val.RemoteIp] = val.Country;
                geoIpDomain[val.RemoteIp] = val.RemoteDomain;
            });

            for (var i = data.value.length - 1; i > data.value.length - recentAttacksCount; i--) {
                $('.table-recent-attacks').find('tbody')
                    .append("<tr>"+
                        "<td>"+data.value[i].Time+"</td>"+
                        "<td>"+data.value[i].TargetService+"</td>"+
                        "<td>"+data.value[i].RemoteIp+"</td>"+
                        "<td>"+data.value[i].RemoteDomain+"</td>"+
                        "<td>"+data.value[i].Country+"</td>"+
                    "</tr>");
            }

            var sortedAttackers = sortObject(geoAttackers);
            showTopAttackers(sortedAttackers);
        }
    });
});

function barChart(arr) {
    Morris.Bar({
        element: 'morris-bar-chart',
        data: arr,
        xkey: 'attackIP',
        ykeys: ['attackCount'],
        labels: ['Attack count'],
        hideHover: 'auto',
        resize: true,
        xLabelMargin: 0,
        gridTextSize: 11
    });
}

function showTopAttackers(data) {
    var topCount = 5;
    var arr = [];

    for (i = 0; i < topCount; i++) {
        if (data[i][0] !== "") {
            $('.table-top-attackers').find('tbody')
                .append("<tr><td>"+data[i][1]+"</td><td>"+data[i][0]+"</td><td>"+geoIpCountry[data[i][0]]+"</td><td>"+geoIpDomain[data[i][0]]+"</td></tr>");

            var obj = {attackCount: data[i][1], attackIP: data[i][0]};
            arr.push(obj);
        } else {
            topCount++;
        }
    }

    barChart(arr);
}

function sortObject(object) {
    var sortedArray = [];
    for (var value in object) {
        sortedArray.push([value, object[value]]);
    }

    sortedArray.sort(function(a, b) {
        return b[1] - a[1];
    });
    return sortedArray;
}