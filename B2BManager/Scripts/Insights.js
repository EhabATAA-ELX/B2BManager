var chartlookup = {};
var charts = [];

function RenderChart(chart, uid) {
    chartlookup[chart.ChartID] = chart;
    $.each(charts, function (key, value) {
        value.resize();
    });
    echarts.init(document.getElementById('Chart_' + chart.ChartID)).showLoading();    
    RenderChartData(uid, chart.ChartID);
}

function RenderChartData(uid, chartID) {
    var intervalId = setInterval(function () {
        if (!chartlookup[chartID].loaded || chartlookup[chartID].Trials > 45) {
            GetChartData(uid, chartID);
            chartlookup[chartID].intervalId = intervalId;
        }
        else {
            clearInterval(chartlookup[chartID].intervalId);
        }
    }, 750);
}


function GetChartData(uid, chartID) {
    if (!chartlookup[chartID].loaded) {
        chartlookup[chartID].Trials++;
        var webserviceData = { UID: uid };
        var webserviceMethodName = "";
        switch (chartlookup[chartID].type) {
            case "LineChart":
            case "VerticalBarChart":
                webserviceMethodName = "GetLineChartData"; break;
            case "PieChart": webserviceMethodName = "GetPieChartData"; break;
            case "HorizontalBarChart": webserviceMethodName = "GetHorizontalBarChartData"; break;
            case "CustomDataTable": webserviceMethodName = "GetCustomHTML"; webserviceData.ChartID = chartlookup[chartID].ChartIntID;  break;
        }

        $.ajax({
            type: 'Get',
            url: 'MasterData.svc/' + webserviceMethodName,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: webserviceData,
            async: true,
            success: function (response) {
                if (response.d) {
                    chartlookup[chartID].loaded = true;
                    switch (chartlookup[chartID].type) {
                        case "LineChart": addLineChartData(response.d, chartID,'line'); break;
                        case "VerticalBarChart": addLineChartData(response.d, chartID,'bar'); break;
                        case "PieChart": addPieChartData(response.d, chartID); break;
                        case "HorizontalBarChart": addHorizontalBarChartData(response.d, chartID); break;
                        case "CustomDataTable": document.getElementById('Chart_' + chartID).innerHTML = response.d; break;
                    }
                }
            },

            error: function (e) {
                console.log("Error  : " + e.statusText);
            }
        });
    }
}

function addLineChartData(items, chartID,displayType) {
    var lookup = {};
    var myChart;
    var series = [];
    var legendData = [];
    var xAxisData = [];
    var selected = {};
    for (var item, i = 0; item = items[i++];) {
        var name = item.label;
        

        if (!(name in lookup)) {
            lookup[name] = 1;

            series.push({
                name: name, type: displayType, data: [], connectNulls: false,
            });
            if (!(name == null || name.length == 0)) {
                legendData.push(name);
                selected[name] = legendData.length < 6;
            }
            else {
                selected[name] = true;
            }
            
        }

        for (var j = 0; j < series.length; j++) {
            if (series[j].name == name) {
                series[j].data.push(item.y);
            }
            if ($.inArray(item.x, xAxisData) == -1) {
                xAxisData.push(item.x);
            }
        }
    }

    option = {
        title: {
            text: chartlookup[chartID].Title,
            subtext: chartlookup[chartID].SubTitle,
            x: 'center'
        },
        xAxis: {
            type: 'category',
            data: xAxisData
        },
        tooltip: {
            trigger: 'item'
        },
        legend: {
            data: legendData,
            orient: 'vertical',
            left: '10px',
            type: 'scroll',
            selected: selected
        },
        elements: { point: { radius: 0 } },
        toolbox: {
            show: true,
            padding: '15',
            feature: {
                magicType: {
                    show: true, type: ['line', 'bar'], title: {
                        line: 'Line Display',
                        bar: 'Bar Display'
                    }  },
                dataZoom: {
                    yAxisIndex: 'none',
                    title: {
                        zoom: 'Zoom',
                        back: 'Zoom Reset'
                    }
                },
                saveAsImage: { show: true, title: 'Save as image' }
            }
        },
        dataZoom: [
            {
                show: true,
                realtime: true
            }
        ],
        yAxis: {
            type: 'value'
        },
        series: series,
        grid: {
            left: '25%'
        }
    };
    myChart = echarts.init(document.getElementById('Chart_' + chartID));
    myChart.resize();
    myChart.hideLoading();
    // use configuration item and data specified to show chart
    myChart.setOption(option);
    if (!charts) {
        charts = [];
    }
    charts.push(myChart);
}

function addPieChartData(items, chartID) {
    var lookup = {};
    var myChart;
    var series = [];
    var legendData = [];
    var selected = {};
    series.push({
        type: 'pie', data: [], radius: '55%', center: ['50%', '60%'],
        itemStyle: {
            emphasis: {
                shadowBlur: 10,
                shadowOffsetX: 0,
                shadowColor: 'rgba(0, 0, 0, 0.5)'
            }
        }
    });
    for (var item, i = 0; item = items[i++];) {
        var name = item.name;
        legendData.push(name);
        selected[name] = legendData.length < 6;
        series[0].data.push({ name: name, value: item.value });
    }

    option = {
        title: {
            text: chartlookup[chartID].Title,
            subtext: chartlookup[chartID].SubTitle,
            x: 'center'
        },
        tooltip: {
            trigger: 'item',
            formatter: "{b} : {c} ({d}%)"
        },
        legend: {
            data: legendData,
            orient: 'vertical',
            left: '10px',
            type: 'scroll',
            selected: selected
        },
        toolbox: {
            show: true,
            padding: '15',
            feature: {
                dataView: {
                    title: 'Display Data', lang: [chartlookup[chartID].Title + ' data view', 'Back to chart view', 'Refresh'], show: true, readOnly: true, optionToContent: function (opt) {
                        var series = opt.series[0].data;
                        var table = '<table style="width:100%;text-align:left"><tbody><tr>'
                            + '<td><b>Name</b></td>'
                            + '<td><b>Value</b></td>'
                            + '</tr>';
                        for (var i = 0, l = series.length; i < l; i++) {
                            table += '<tr>'
                                + '<td>' + series[i].name + '</td>'
                                + '<td>' + series[i].value + '</td>'
                                + '</tr>';
                        }
                        table += '</tbody></table>';
                        return table;
                    }
                },
                saveAsImage: { show: true, title: 'Save as image' }
            }
        },
        series: series,
        grid: {
            left: '25%'
        }
    };
    myChart = echarts.init(document.getElementById('Chart_' + chartID));
    myChart.resize();
    myChart.hideLoading();
    // use configuration item and data specified to show chart
    myChart.setOption(option);
    if (!charts) {
        charts = [];
    }
    charts.push(myChart);
}

function addHorizontalBarChartData(items, chartID) {
    var lookup = {};
    var lookupCategory = {};
    var myChart;
    var series = [];
    var legendData = [];
    var categoryData = [];
    var selected = {};
    for (var item, i = 0; item = items[i++];) {
        var name = item.name;

        if (!(name in lookup)) {
            lookup[name] = 1;

            series.push({
                name: name, type: 'bar', stack: item.stack, data: []
            });
            legendData.push(name);
            selected[name] = legendData.length < 20;
        }

        for (var j = 0; j < series.length; j++) {
            if (series[j].name == name) {
                series[j].data.push(item.value);
                if (!(item.category in lookupCategory)) {
                    lookupCategory[item.category] = 1;
                    categoryData.push(item.category);
                }
            }
        }
    }

    option = {
        title: {
            text: chartlookup[chartID].Title,
            subtext: chartlookup[chartID].SubTitle,
            x: 'center'
        },
        tooltip: {
            trigger: 'axis',
            axisPointer: {            
                type: 'shadow'        
            }
        },
        legend: {
            data: legendData,
            orient: 'vertical',
            left: '10px',
            type: 'scroll',
            selected: selected
        },
        xAxis: {
            type: 'value'
        },
        yAxis: {
            type: 'category',
            data: categoryData
        },
        toolbox: {
            show: true,
            padding: '15',
            feature: {
                magicType: {
                    show: true, type: ['stack', 'tiled'], title: {
                        stack: 'Stack Display',
                        tiled: 'Tiled Display'
                    }
                },
                saveAsImage: { show: true, title: 'Save as image' }
            }
        },
        series: series,
        grid: {
            left: '25%'
        }
    };
    myChart = echarts.init(document.getElementById('Chart_' + chartID));
    myChart.resize();
    myChart.hideLoading();
    // use configuration item and data specified to show chart
    myChart.setOption(option);
    if (!charts) {
        charts = [];
    }
    charts.push(myChart);
}

$(function () {
    $(window).on('resize', resize);

    // Resize function
    function resize() {
        $.each(charts, function (key, value) {
            value.resize();
        });
    }
});

function ExportToPDF() {
    var pdf = new jsPDF("p", "pt", "a4");
    pdf.addHTML($('#dashboard'), 0, 0, function () {
        pdf.save($('.dashobard-title').text().replace("*", "").replace(".", "").replace("/", "") + '.pdf');
    });
}

function saveMainPositions() {
    var serializedData = _.map($('.grid-stack > .grid-stack-item'), function (el) {
        var node = $(el).data('_gridstack_node');
        return {
            x: node.x,
            y: node.y,
            w: node.width,
            h: node.height,
            ChartID: node.id
        };
    });

    var dashboardDesignModel = {
        DashboardID: $('#CurrentDashboardID').val(),
        Charts : serializedData
    }

    console.log(dashboardDesignModel);

    $.ajax({
        type: 'Get',
        url: 'MasterData.svc/SaveDashboardDesign',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: {
            dashboardDesign: JSON.stringify(dashboardDesignModel)
        },
        async: true,
        success: function (response) {
            if (response.d == true) {
                $('#windowInformationText').html("Updated with success.");
                $('#windowInformationText').removeClass().addClass("fontgreen");
            }
            else {
                $('#windowInformationText').html("An unexpected error has occurred.<br>Please try again later.");
                $('#windowInformationText').removeClass().addClass("fontred");
            }
            ChangeWindowDisplay('ContentPlaceHolder1_WindowConfirmChanges', true);
        },

        error: function (e) {
            console.log(e);
        }
    });
    return false;
}
