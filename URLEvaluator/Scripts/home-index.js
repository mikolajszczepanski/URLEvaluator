$(function () {
    var proccessing = false;
    var measureSiteApi = $.connection.measureSitePerformanceHub;
    var loadedData = {};
    var history = [];
    var historyAvg = [];
    var chart = null;
    var timer = null;

    setInterval(function () {
        if (timer === null) {
            return;
        }
        timer += 0.5;
        if (chart === null) {
            return;
        }
        var avg = calculateAverageResponseTime(loadedData);
        if (avg === null) {
            return;
        }
        historyAvg.push({ avg: avg, time: timer });
        var x = [];
        var y = [];
        for (var i = 0; i < historyAvg.length; i++) {
            x.push(historyAvg[i].time);
            y.push(historyAvg[i].avg);
        }
        chart.data.datasets[0].data = y;
        chart.data.labels = x;
        chart.update();
    }, 500);

    function calculateAverageResponseTime(data) {
        var sum = 0;
        var n = 0;
        for (var key in data) {
            for (var i = 0; i < data[key].length; i++) {
                sum += data[key][i];
            }
            n += data[key].length;
        }
        if (n === 0) {
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
        $("#score").html('<small>Average response time</small> ' + avg.toFixed(4) + ' <small>sec</small>');
    }

    function renderChart() {
        if (chart !== null) {
            chart.destroy();
        }
        var data = {
            labels: [],
            datasets: [
                {
                    label: "Average response time",
                    fill: false,
                    lineTension: 0.1,
                    backgroundColor: "rgba(75,192,192,0.4)",
                    borderColor: "rgba(75,192,192,1)",
                    borderCapStyle: 'butt',
                    borderDash: [],
                    borderDashOffset: 0.0,
                    borderJoinStyle: 'miter',
                    pointBorderColor: "rgba(75,192,192,1)",
                    pointBackgroundColor: "#fff",
                    pointBorderWidth: 1,
                    pointHoverRadius: 5,
                    pointHoverBackgroundColor: "rgba(75,192,192,1)",
                    pointHoverBorderColor: "rgba(220,220,220,1)",
                    pointHoverBorderWidth: 2,
                    pointRadius: 1,
                    pointHitRadius: 10,
                    data: []
                }
            ]
        };
        var ctx = $("#result-chart");
        chart = new Chart(ctx, {
            type: 'line',
            data: data,
            options: {
                scales: {
                    yAxes: [{
                        scaleLabel: {
                            display: true,
                            labelString: 'Average Response Time[sec]'
                        }
                    }],
                    xAxes: [{
                        scaleLabel: {
                            display: true,
                            labelString: 'Time[sec]'
                        }
                    }]
                }
            }
        });
    }

    function renderResultTable(data) {
        var groupedData = getGroupedDataWithMinAndMax(data);
        var sortedData = groupedData.sort(function (x, y) { return y.max - x.max; });
        var html = "";
        for (var i = 0; i < sortedData.length; i++) {
            html += '<tr><td>' + groupedData[i].url + '</td><td>' + groupedData[i].min + '</td><td>' + groupedData[i].max + '</td></tr>';
        }
        $('#results-table tbody').html(html);
    }

    function renderHistoryTable(data) {
        var sortedData = data.sort(function (x, y) { return y.time - x.time; });
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
        timer = null;
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
        timer = null;
    };
    measureSiteApi.client.getResultHistory = function (url, time, groupGuid, date) {
        if (time !== null) {
            history.push({ url: url, time: time, groupGuid: groupGuid, date: date });
            renderHistoryTable(history);
        }
    };
    $.connection.hub.start().done(function () {
        $('#go').click(function () {
            if (proccessing === false) {
                $('#results-table tbody tr').remove();
                loadedData = {};
                historyAvg = [];
                measureSiteApi.server.evaluateResponseTime($('#url').val());
                proccessing = true;
                timer = 0;
                renderChart();
                $("#result-chart").toggle();
                $("#url").prop('disabled', true);
                $("#go").prop('disabled', true);
            }
        });
        $("#show-history").click(function (event) {
            event.preventDefault();
            var url = $("#url").val();
            $('#history-table tbody tr').remove();
            $("#history-table").toggle();
            measureSiteApi.server.loadHistory(url, null);
            
        });
    });
});