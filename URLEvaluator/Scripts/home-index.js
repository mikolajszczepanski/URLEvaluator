$(function () {
    var proccessing = false;
    var measureSiteApi = $.connection.measureSitePerformanceHub;
    var loadedData = {};
    var history = [];

    function calculateAverageResponseTime(data) {
        var sum = 0;
        var n = 0;
        for (var key in data) {
            for (var i = 0; i < data[key].length; i++) {
                sum += data[key][i];
            }
            n += data[key].length;
        }
        if (n == 0) {
            return null;
        }
        return sum / n;
    }

    function getGroupedDataWithMinAndMax(data) {
        var groupedData = [];

        for (var key in data) {
            var min = Math.min.apply(Math, data[key]);
            var max = Math.max.apply(Math, data[key]);
            groupedData.push({ url: key, min: min, max: max });
        }
        return groupedData;
    }

    function showResponseTimeResult(data) {
        var avg = calculateAverageResponseTime(data);
        if (avg === null) {
            return;
        }
        $("#score").html('<small>Average load time</small> ' + avg.toFixed(4) + ' <small>sec</small>');
    }

    function renderResultTable(data) {
        var groupedData = getGroupedDataWithMinAndMax(data);
        var sortedData = groupedData.sort(function (x, y) { return y.max - x.max });
        var html = "";
        for (var i = 0; i < sortedData.length; i++) {
            html += '<tr><td>' + groupedData[i].url + '</td><td>' + groupedData[i].min + '</td><td>' + groupedData[i].max + '</td></tr>';
        }
        $('#results-table tbody').html(html);
    }

    function renderHistoryTable(data) {
        var sortedData = data.sort(function (x, y) { return y.time - x.time });
        var html = "";
        for (var i = 0; i < sortedData.length; i++) {
            html += '<tr><td>' + data[i].url + '</td><td>' + data[i].time + '</td><td>' + data[i].date + '</td></tr>';
        }
        $('#history-table tbody').html(html);
    }

    measureSiteApi.client.end = function () {
        proccessing = false;
        $("#url").prop('disabled', false);
        $("#go").prop('disabled', false);
    };
    measureSiteApi.client.urlFailResponse = function (url) {
    };
    measureSiteApi.client.urlSuccessResponse = function (url, time) {
        if (loadedData[url] === undefined) {
            loadedData[url] = [];
        }
        loadedData[url].push(parseFloat(time));
        showResponseTimeResult(loadedData);
        renderResultTable(loadedData);
    };
    measureSiteApi.client.error = function (message) {
        console.log(message);
        $('#errors').append("<div class=\"alert alert-danger\"><a href=\"#\" class=\"close\" data-dismiss=\"alert\" aria-label=\"close\" title=\"close\">×</a><strong>Error occured!</strong> " + message + "</div>");
        $("#url").prop('disabled', false);
        $("#go").prop('disabled', false);
        proccessing = false;
    };
    measureSiteApi.client.getResultHistory = function (url, time, groupGuid, date) {
        if (time != null) {
            history.push({ url: url, time: time, groupGuid: groupGuid, date: date });
            renderHistoryTable(history);
        }
    };
    $.connection.hub.start().done(function () {
        $('#go').click(function () {
            if (proccessing == false) {
                $('#results-table tbody tr').remove();
                loadedData = {};
                measureSiteApi.server.evaluateResponseTime($('#url').val());
                proccessing = true;
                $("#url").prop('disabled', true);
                $("#go").prop('disabled', true);
            }
        });
        $("#show-history").click(function () {
            var url = $("#url").val();
            $('#history-table tbody tr').remove();
            $("#history-table").toggle();
            measureSiteApi.server.loadHistory(url, null);
            
        });
    });
});