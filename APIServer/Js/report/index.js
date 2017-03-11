function _print() {
    console.log($(".hide_print"));
    $(".hide_print").hide();
    window.print();
    $(".hide_print").show();
}

$(function () {
    //绑定打印按钮
    $(document).on("click", "#print", function () {
        _print();
    })
    //绑定返回按钮
    $(document).on("click", "#return", function () {
        var meetingID = $("#meetingID").val();
        window.location.href = "/Meeting/Show_organizor?meetingID=" + meetingID;
    })
})



//获取会议状态
function isEnd() {
    console.log($("#isEnd").attr("value"));
    return parseInt($("#isEnd").val()) == 1;
}

//定义表决图表绘制存储结构
function ChartStruct() {
    this.voteID;//表决ID
    this.voteName;//表决名称
    this.voteChart;//容器
    this.optionLabels = new Array();//选项标签数组（x轴数组:eg.  ==> [[1,"***"],[2,"##"],...,[n,"***"] )
    this.optionResults = new Array();//选项结果数组
}

//定义图表数组
var chartStructs = new Array();

//设置图表数组数据
function init_chartStructs() {
    var votes = $(".vote");
    if (!votes)
        return;
    votes.each(function () {
        var vote = $(this);
        console.log(vote);
        var chartStruct = new ChartStruct();
        chartStruct.voteID = parseInt(vote.attr("value"));
        chartStruct.voteName = vote.attr("voteName");
        chartStruct.voteChart = document.getElementById("voteChart" + chartStruct.voteID);

        var options = vote.find(".option");

        if (!options)
            return;
        var index = 1;
        options.each(function () {
            var option = $(this);
            console.log(option);
            var optionNames = option.find(".optionName");
            if (optionNames) {
                chartStruct.optionLabels.push([index,optionNames[0].innerText]);
            }
            var optionResults = option.find(".optionResult");
            if (optionResults) {
                chartStruct.optionResults.push([index,Number(optionResults[0].innerText)]);
            }
            index++;
        });
        console.log(chartStruct);
        chartStructs.push(chartStruct);
    });
}

function drawChart(chartStruct,data) {
    var options = {
        title: chartStruct.voteName,
        fontSize: 10,
        bars: {
            show: true,
            barWidth: 0.6,
            lineWidth: 0,
            fillOpacity: 0.8,
            fillColor: {
                colors: ['#CB4B4B', '#CB4B4B'],
                start: 'top',
                end: 'bottom'
            }
        },
        mouse: {
            track: true,
            relative: true
        },
        yaxis: {
            title: "票数",
            min: 0,
            tickDecimals: 0,
            autoscaleMargin: 1
        },
        xaxis: {
            title: "选项",
            titleAlign: 'center',
            ticks: chartStruct.optionLabels,
            minorTickFreq: 4
        },
        grid: {
            verticalLines: true
        }
    };

    var markers = {
        data : chartStruct.optionResults,
        markers:{
            show:true,
            position: 'ct'
        }
    };

    // Draw Graph
    Flotr.draw(chartStruct.voteChart, [data,markers], options);
}

function draw() {
    for(var i = 0 ; i < chartStructs.length;i++){
        var chartStruct = chartStructs[i];

        var data = [
           { data: chartStruct.optionResults, label: '票数', lines: { show: true }, points: { show: true } },
        ];

        drawChart(chartStruct, data);
    }
}

//绘制图表==========================================
$(function () {
    //console.log(isEnd());
    if (isEnd()) {
        console.log("会议结束");
        init_chartStructs();
        draw();
    }
})
