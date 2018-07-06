// chart colors

var $border_color = "#efefef";

var $green = "#ed6d49";
var $yellow = "#74b749";
var $orange = "#0daed3";
var $blue = "#ffb400";
var $red = "#f63131";
var $teal = "#34A097";

//Google Visualization 
google.load("visualization", "1", {
  packages: ["corechart"]
});

$(document).ready(function () {
  drawChart1();
  drawChart2();
  drawChart3();
  drawChart4();
  drawRegionsMap();
  drawTable();
  candlestick();
  bubbleChart();
})

function drawChart1() {
  var data = google.visualization.arrayToDataTable([
    ['Year', 'Google+', 'Facebook'],
    ['2005', 2110, 3430],
    ['2006', 3880, 1290],
    ['2007', 1250, 430],
    ['2008', 1190, 1950],
    ['2009', 2120, 2970],
    ['2010', 3270, 4960],
    ['2011', 2950, 2090],
    ['2012', 1800, 5440]
    ]);

  var options = {
    width: 'auto',
    pointSize: 7,
    lineWidth: 1,
    height: '200',
    backgroundColor: 'transparent',
    colors: [$blue, $teal, $red, $green, $orange, $yellow],
    tooltip: {
      textStyle: {
        color: '#666666',
        fontSize: 11
      },
      showColorCode: true
    },
    legend: {
      textStyle: {
        color: 'black',
        fontSize: 12
      }
    },
    chartArea: {
      left: 40,
      top: 10,
      height: "80%"
    }
  };

  var chart = new google.visualization.AreaChart(document.getElementById('area_chart'));
  chart.draw(data, options);
}


function drawChart2() {
  var data = google.visualization.arrayToDataTable([
    ['Year', 'Visitors', 'Sales'],
    ['1989', 4903, 1800],
    ['1990', 1250, 1850],
    ['1991', 1300, 1900],
    ['1992', 1350, 1950],
    ['1993', 1400, 2000],
    ['1994', 1450, 2050],
    ['1995', 1500, 2100],
    ['1996', 1550, 2150],
    ['1997', 1600, 3197],
    ['1998', 1650, 2250],
    ['1999', 3389, 2300],
    ['2000', 1750, 2350],
    ['2001', 6129, 7753],
    ['2002', 1850, 2450],
    ['2003', 1900, 2598],
    ['2004', 1950, 2550],
    ['2005', 2000, 2600],
    ['2006', 2050, 2650],
    ['2007', 2100, 2700],
    ['2008', 2150, 2750],
    ['2009', 1290, 1967],
    ['2010', 1290, 1967],
    ['2011', 1290, 1967],
    ['2012', 1290, 1967],
    ['2013', 1290, 1967],

    ]);

  var options = {
    width: 'auto',
    height: '160',
    backgroundColor: 'transparent',
    colors: [$blue, $red, $teal, $green, $orange, $yellow],
    tooltip: {
      textStyle: {
        color: '#666666',
        fontSize: 11
      },
      showColorCode: true
    },
    legend: {
      textStyle: {
        color: 'black',
        fontSize: 12
      }
    },
    chartArea: {
      left: 100,
      top: 10
    },
    focusTarget: 'category',
    hAxis: {
      textStyle: {
        color: 'black',
        fontSize: 12
      }
    },
    vAxis: {
      textStyle: {
        color: 'black',
        fontSize: 12
      }
    },
    pointSize: 8,
    chartArea: {
      left: 60,
      top: 10,
      height: '80%'
    },
    lineWidth: 2,
  };

  var chart = new google.visualization.LineChart(document.getElementById('line_chart'));
  chart.draw(data, options);
}


function drawChart3() {
  var data = google.visualization.arrayToDataTable([
    ['Year', 'Visits', 'Orders', 'Income', 'Expenses'],
    ['2007', 300, 800, 900, 300],
    ['2008', 1170, 860, 1220, 564],
    ['2009', 260, 1120, 2870, 2340],
    ['2010', 1030, 540, 3430, 1200],
    ['2011', 200, 700, 1700, 770],
    ['2012', 1170, 2160, 3920, 800],
    ['2013', 2170, 1160, 2820, 500] ]);

  var options = {
    width: 'auto',
    height: '160',
    backgroundColor: 'transparent',
    colors: [$blue, $red, $teal, $green, $orange, $yellow],
    tooltip: {
      textStyle: {
        color: '#666666',
        fontSize: 11
      },
      showColorCode: true
    },
    legend: {
      textStyle: {
        color: 'black',
        fontSize: 12
      }
    },
    chartArea: {
      left: 60,
      top: 10,
      height: '80%'
    },
  };

  var chart = new google.visualization.ColumnChart(document.getElementById('column_chart'));
  chart.draw(data, options);
}

function drawChart4() {
  var data = google.visualization.arrayToDataTable([
    ['Task', 'Hours per Day'],
    ['Eat', 2],
    ['Work', 9],
    ['Commute', 2],
    ['Read', 2],
    ['Sleep', 7],
    ['Play', 2],
    ]);

  var options = {
    width: 'auto',
    height: '260',
    backgroundColor: 'transparent',
    colors: [$blue, $red, $teal, $green, $orange, $yellow],
    tooltip: {
      textStyle: {
        color: '#666666',
        fontSize: 11
      },
      showColorCode: true
    },
    legend: {
      position: 'left',
      textStyle: {
        color: 'black',
        fontSize: 12
      }
    },
    chartArea: {
      left: 0,
      top: 10,
      width: "100%",
      height: "100%"
    }
  };

  var chart = new google.visualization.PieChart(document.getElementById('pie_chart'));
  chart.draw(data, options);
}

//Geo Charts
google.load('visualization', '1', {'packages': ['geochart']});
google.setOnLoadCallback(drawRegionsMap);

function drawRegionsMap() {
  var data = google.visualization.arrayToDataTable([
    ['Country', 'Popularity'],
    ['Germany', 200],
    ['IN', 900],
    ['United States', 300],
    ['Brazil', 400],
    ['Canada', 500],
    ['France', 600],
    ['RU', 700]
    ]);

  var options = {
    width: 'auto',
    height: '260',
    backgroundColor: 'transparent',
    colors: [$blue, $red, $teal, $green, $orange, $yellow],
  };

  var chart = new google.visualization.GeoChart(document.getElementById('geo_chart'));
  chart.draw(data, options);
};

//Table Charts
google.load('visualization', '1', {packages:['table']});
google.setOnLoadCallback(drawTable);
function drawTable() {
  var data = new google.visualization.DataTable();
  data.addColumn('string', 'Name');
  data.addColumn('number', 'Salary');
  data.addColumn('boolean', 'Full Time Employee');
  data.addRows([
    ['Williams',  {v: 10000, f: '$12,000'}, true],
    ['Rosy',   {v: 18000, f: '$19,000'},  false],
    ['John', {v: 12500, f: '$32,500'}, false],
    ['Prinu',   {v: 28000, f: '$21,000'}, true],
    ['Maxwell',  {v: 10000, f: '$14,000'}, true]
    ]);

  var table = new google.visualization.Table(document.getElementById('table_chart'));
  table.draw(data, {showRowNumber: true});
}

//Candlestick Chart
function candlestick() {
  var data = google.visualization.arrayToDataTable([
    ['Mon', 20, 28, 38, 45],
    ['Tue', 31, 38, 55, 66],
    ['Wed', 50, 55, 77, 80],
    ['Thu', 77, 47, 56, 50],
    ['Fri', 68, 66, 22, 15],
    ['Sat', 23, 31, 12, 35]
    // Treat first row as data as well.
    ], true);

  var options = {
    legend: 'none',
    width: 'auto',
    height: '280',
    backgroundColor: 'transparent',
    colors: [$blue, $red, $teal, $green, $orange, $yellow],
  };

  var chart = new google.visualization.CandlestickChart(document.getElementById('candlestick_chart'));
  chart.draw(data, options);
}

// google.setOnLoadCallback(drawVisualization);

//Bubble Chart

google.load("visualization", "1", {packages:["corechart"]});
google.setOnLoadCallback(bubbleChart);
function bubbleChart() {
  var data = google.visualization.arrayToDataTable([
    ['ID', 'Life Expectancy', 'Fertility Rate', 'Region',     'Population'],
    ['CAN',    80.66,              1.67,      'North America',  33739900],
    ['DEU',    79.84,              1.36,      'Europe',         81902307],
    ['DNK',    78.6,               1.84,      'Europe',         5523095],
    ['SL',     72.73,              2.78,      'South Asia',    109716203],
    ['GBR',    80.05,              2,         'Europe',         61801570],
    ['IRN',    72.49,              1.7,       'Middle East',    73137148],
    ['IRQ',    68.09,              4.77,      'Middle East',    31090763],
    ['ISR',    81.55,              2.96,      'Middle East',    7485600],
    ['RUS',    68.6,               1.54,      'Europe',         141850000],
    ['USA',    78.09,              2.05,      'North America',  307007000]
    ]);

  var options = {
    title: 'Correlation between life expectancy, fertility rate and population of some world countries (2012)',
    hAxis: {title: 'Life Expectancy'},
    vAxis: {title: 'Fertility Rate'},
    colors: [$blue, $red, $teal, $green, $orange, $yellow],
    fontSize: 11,
    bubble: {textStyle: {fontSize: 11}}
  };

  var chart = new google.visualization.BubbleChart(document.getElementById('bubble_chart'));
  chart.draw(data, options);
}

//Resize charts and graphs on window resize
$(document).ready(function () {
  $(window).resize(function(){
    drawChart1();
    drawChart2();
    drawChart3();
    drawChart4();
    drawTable();
    bubbleChart();
    drawRegionsMap();
    candlestick()
  });
});
