var map;
var geocoder;
var geoPlaces = {};
var geoTargetServices = {};
var geoAttackDates = {};

function main() {
    geocoder = new google.maps.Geocoder();
    map = new google.maps.Map(document.getElementById('maaap'), {
        center: {
            lat: 52.3702157,
            lng: 4.895167899999933
        },
        zoom: 3
    });

    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: "http://honeyaccess.azurewebsites.net/api/LogEntries",
        success: function(data) {
            $.each(data.value, function(key, val) {
                if (val.Country) {
                    geoPlaces[val.Country] = geoPlaces[val.Country] ? geoPlaces[val.Country] + 1 : 1;
                }

                geoAttackDates[val.Time.substring(0,10)] = geoAttackDates[val.Time.substring(0,10)] ? geoAttackDates[val.Time.substring(0,10)] + 1 : 1;
                geoTargetServices[val.TargetService] = geoTargetServices[val.TargetService] ? geoTargetServices[val.TargetService] + 1 : 1;
            });

            
            var sortedTargetServices = sortObject(geoTargetServices);

            showAttackDates(geoAttackDates);
            showTopTargetServices(sortedTargetServices);

            var color;
            var dangerLevel;
            var smiley;
            if (geoAttackDates[getCurrentDate()] < 150 || geoAttackDates[getCurrentDate()] === undefined) {
                color = "#00AF00";
                dangerLevel = "Low";
                smiley = "fa-smile-o";
            } else if (geoAttackDates[getCurrentDate()] < 250) {
                color = "orange";
                dangerLevel = "Medium";
                smiley = "fa-meh-o";
            } else {
                color = "#E00000";
                dangerLevel = "High";
                smiley = "fa-frown-o";
            }

            $('.danger-level').css('background-color', color);
            $('.danger-level-text').text('Danger Level is ' + dangerLevel);
            $('.danger-level').find('.col-xs-3').html('<i class="text-right fa ' + smiley + ' fa-2x"></i>');

            console.log("Drawing locations on map :: started");
            loadMap();
        }
    });
}

function getCurrentDate() {
    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1;
    var yyyy = today.getFullYear();
    return yyyy + "-" + mm + "-" + dd;
}
function showAttackDates(data) {
    var arr = [];

    $.each(data, function(key, val) {
        var obj = {label: key, value: val};
        arr.push(obj);
    });

    lineChart(arr);
}

function showTopTargetServices(data) {
    var topCount = 10;
    var arr = [];

    for (i = 0; i < topCount; i++) {
        if (data[i][0] !== "") {
            var obj = {label: data[i][0], value: data[i][1]};
            arr.push(obj);
        } else {
            topCount++;
        }
    }

    donutChart(arr);
}

function donutChart(arr) {
    Morris.Donut({
        element: 'morris-donut-chart',
        data: arr,
        resize: true
    });
}

function lineChart(arr) {
    Morris.Area({
        element: 'morris-area-chart',
        data: arr,
        xkey: 'label',
        ykeys: ['value'],
        labels: ['Attack count'],
        pointSize: 2,
        hideHover: 'auto',
        resize: true
    });
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

var geocodeDelay = 100;
var currentLocation = 0;

function loadMap() {
    if (currentLocation < Object.keys(geoPlaces).length) {
        var locationName = Object.keys(geoPlaces)[currentLocation];
        setTimeout('geocodeCountry("' + locationName + '","' + geoPlaces[locationName] + '")', geocodeDelay);
        currentLocation++;
    } else {
        console.log("Drawing locations on map :: completed");
    }
}

function geocodeCountry(country, attackCount) {
    geocoder.geocode({
        'address': country
    }, function(results, status) {
        if (status == google.maps.GeocoderStatus.OK) {
            addMarker(results[0].geometry.location, country, attackCount, map);
        } else if (status == google.maps.GeocoderStatus.OVER_QUERY_LIMIT) {
            currentLocation--;
            geocodeDelay++;
        }
    });

    loadMap();
}

function addMarker(locationPosition, locationName, attackCount, map) {
    var classChoice;
    if (attackCount < 100) {
        classChoice = 1;
    } else if (attackCount < 200) {
        classChoice = 2;
    } else if (attackCount < 300) {
        classChoice = 3;
    } else if (attackCount < 400) {
        classChoice = 4;
    } else if (attackCount < 500) {
        classChoice = 5;
    } else if (attackCount < 600) {
        classChoice = 6;
    } else if (attackCount < 700) {
        classChoice = 7;
    } else if (attackCount < 800) {
        classChoice = 8;
    } else if (attackCount < 900) {
        classChoice = 9;
    } else if (attackCount < 1000) {
        classChoice = 10;
    } else {
        classChoice = 11;
    }

    var marker = new MarkerWithLabel({
        position: locationPosition,
        labelContent: attackCount,
        map: map,
        labelAnchor: new google.maps.Point(25, 0),
        labelClass: "labels" + classChoice + "",
        labelInBackground: false,
        icon: pinSymbol('red')
    });

    var infowindow = new google.maps.InfoWindow({
        content: locationName
    });

    marker.addListener('click', function() {
        infowindow.open(map, marker);
    });
}

function pinSymbol(color) {
    return {
        // path: 'M 0,0 C -2,-20 -10,-22 -10,-30 A 10,10 0 1,1 10,-30 C 10,-22 2,-20 0,0 z',
        path: '',
        fillColor: color,
        fillOpacity: 1,
        strokeColor: '#000',
        strokeWeight: 2,
        scale: 2
    };
}

google.maps.event.addDomListener(window, 'load', main);
