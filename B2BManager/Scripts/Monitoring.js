var myChart;
function GetData(loop) {
    $.ajax({
        type: 'Get',
        url: 'MonitoringDataService.svc/GetData',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: { ChartLineTime: parseInt($('.ddlChartLineTime')[0].value) },
        success: function (response) {
            addData(loop, response);
            if (loop) {
                setInterval(function () {
                    GetData();
                }, 15000);
            }
        },

        error: function (e) {
            console.log("Error  : " + e.statusText);
        }
    });
}

function addData(initShownMessages, response) {
    var lookup = {};
    var items = response.d;
    var series = [];
    var legendData = [];

    for (var item, i = 0; item = items[i++];) {
        var name = item.label;

        if (!(name in lookup)) {
            lookup[name] = 1;

            if (initShownMessages) {
                shownMessages.push(item.messageID);
            }

            if ($.inArray(item.messageID, shownMessages) > -1) {
                series.push({
                    name: name, type: 'line', smooth: true, data: [], markLine: { data: [] }, messageID: item.messageID, itemStyle: { normal: { areaStyle: { type: 'default' } } }
                });
                legendData.push(name);
            }
        }

        for (var j = 0; j < series.length; j++) {
            if (series[j].name == name) {
                //ticks are in nanotime; convert to microtime
                var ticksToMicrotime = item.time / 10000;

                //ticks are recorded from 1/1/1; get microtime difference from 1/1/1/ to 1/1/1970
                var epochMicrotimeDiff = Math.abs(new Date(0, 0, 1).setFullYear(1));
                var epochDate = new Date(ticksToMicrotime);
                //new date is ticks, converted to microtime, minus difference from epoch microtime
                var tickDate = new Date(ticksToMicrotime - epochMicrotimeDiff + epochDate.getTimezoneOffset() * 60000);

                if (series[j].minDate == null) {
                    series[j].minDate = tickDate;
                    series[j].maxDate = tickDate;
                }
                else {
                    if (series[j].minDate > tickDate) {
                        series[j].minDate = tickDate;
                    }
                    else {
                        series[j].maxDate = tickDate;
                    }
                }

                series[j].ExpectedResponseTimeInMilliseconds = item.ExpectedResponseTimeInMilliseconds;
                series[j].WorstAcceptableResponseTimeInMilliseconds = item.WorstAcceptableResponseTimeInMilliseconds;

                series[j].data.push([tickDate, item.y]);
            }
        }
    }

    var seriesLength = series.length;
    for (var j = 0; j < seriesLength; j++) {

        series[j].markLine.data.push([
            {
                name: "Expected response time for " + series[j].name,
                xAxis: series[j].minDate,
                yAxis: series[j].ExpectedResponseTimeInMilliseconds
            }, {
                name: "Expected response time for " + series[j].name,
                xAxis: series[j].maxDate,
                yAxis: series[j].ExpectedResponseTimeInMilliseconds
            }
        ]);

        var NewSeries = {
            name: "Worst acceptable time for " + series[j].name, type: 'line', smooth: true, data: [], messageID: series[j].messageID
        };
        legendData.push("Worst acceptable time for " + series[j].name);

        NewSeries.data.push([series[j].minDate, series[j].WorstAcceptableResponseTimeInMilliseconds]);
        NewSeries.data.push([series[j].maxDate, series[j].WorstAcceptableResponseTimeInMilliseconds]);
        series.push(NewSeries);
    }


    // based on prepared DOM, initialize echarts instance
    myChart = echarts.init(document.getElementById('mainChart'));

    option = {
        xAxis: {
            type: 'time'
        },
        tooltip: {
            trigger: 'item',
            formatter: function (params) {              
                if (params.value != "") {
                    if (params.seriesName.startsWith("Worst")) {
                        return params.seriesName;
                    }
                    else {
                        var date = new Date(params.value[0]);
                        data = date.getFullYear() + '-'
                            + (date.getMonth() + 1) + '-'
                            + date.getDate() + ' '
                            + (date.getHours().toString().length == 1 ? "0" : "") + date.getHours() + ':'
                            + (date.getMinutes().toString().length == 1 ? "0" : "") + date.getMinutes() + ':'
                            + (date.getSeconds().toString().length == 1 ? "0" : "") + date.getSeconds();
                        return params.seriesName + '<br/>' + "Sent on: " + data + '<br/>' +
                            "Within: " + params.value[1] + " ms";
                    }
                }
                else {
                    return params.name.split(' > ')[0];
                }
            }
        },
        legend: {
            data: legendData
        },
        toolbox: {
            show: true,
            feature: {
                magicType: { show: true, type: ['line', 'bar'], title: 'Change chart type' },
                restore: { show: true, title: 'Refresh' },
                saveAsImage: { show: true, title: 'Save as image' }
            }
        },
        calculable: true,
        yAxis: {
            type: 'value'
        },
        series: series
    };

    // use configuration item and data specified to show chart
    myChart.setOption(option);
}

var shownMessages = [];
function ToogleLayer(messageId) {
    var img = $("#chartImg_" + messageId.toString())[0];
    if ($.inArray(messageId, shownMessages) > -1) {
        shownMessages = shownMessages.filter(function (item) {
            return item !== messageId
        });
    }
    else {
        shownMessages.push(messageId);
    }

    GetData();

    if ((img.src).includes("add")) {
        img.src = img.src.toString().replace("add", "delete");
    }
    else {
        img.src = img.src.toString().replace("delete", "add");
    }
    return false;
}

function ChangeActivationStatus(MessageID) {
    var chartImg = $("#Manage_" + MessageID.toString())[0];
    chartImg.src = chartImg.src.replace("play.png", "ajax-loader.gif").replace("pause.png", "ajax-loader.gif");
    $.ajax({
        type: 'Get',
        url: 'MonitoringDataService.svc/ChangeActivationStatus',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: { MessageID: MessageID },
        success: function (response) {
            switch (response.d) {
                case "active": 
                case "stopped": {
                    setTimeout(function () { __doPostBack('ContentPlaceHolder1_gridSearch', ''); }, 1);
                    GetData();
                    break;
                }
                default: {
                    alert("Unexpected error has occurred");
                }
            }
        },

        error: function (e) {
            alert("Unexpected error has occurred");
        }
    });
    return false;

}


$(document).ready(function () {
    GetData(true);
});