// ================================ Recent Orders Chart Start ================================
// ================================ Recent Orders Chart Start ================================ 
function createChartTwo(chartId, chartColor) {

    var options = {
        series: [
            {
                name: 'This Day',
                data: [18, 25, 20, 35, 25, 55, 45, 50, 40],
            },
        ],
        chart: {
            type: 'area',
            width: '100%',
            height: 360,
            sparkline: {
                enabled: false // Remove whitespace
            },
            toolbar: {
                show: false
            },
            padding: {
                left: 0,
                right: 0,
                top: 0,
                bottom: 0
            }
        },
        dataLabels: {
            enabled: false
        },
        stroke: {
            curve: 'smooth',
            width: 4,
            colors: [chartColor],
            lineCap: 'round'
        },
        grid: {
            show: true,
            borderColor: '#D1D5DB',
            strokeDashArray: 1,
            position: 'back',
            xaxis: {
                lines: {
                    show: false
                }
            },
            yaxis: {
                lines: {
                    show: true
                }
            },
            row: {
                colors: undefined,
                opacity: 0.5
            },
            column: {
                colors: undefined,
                opacity: 0.5
            },
            padding: {
                top: -30,
                right: 0,
                bottom: -10,
                left: 0
            },
        },
        fill: {
            type: 'gradient',
            colors: [chartColor], // Set the starting color (top color) here
            gradient: {
                shade: 'light', // Gradient shading type
                type: 'vertical',  // Gradient direction (vertical)
                shadeIntensity: 0.5, // Intensity of the gradient shading
                gradientToColors: [`${chartColor}00`], // Bottom gradient color (with transparency)
                inverseColors: false, // Do not invert colors
                opacityFrom: .6, // Starting opacity
                opacityTo: 0.3,  // Ending opacity
                stops: [0, 100],
            },
        },
        // Customize the circle marker color on hover
        markers: {
            colors: [chartColor],
            strokeWidth: 3,
            size: 0,
            hover: {
                size: 10
            }
        },
        xaxis: {
            labels: {
                show: false
            },
            categories: [`Jan`, `Feb`, `Mar`, `Apr`, `May`, `Jun`, `Jul`, `Aug`, `Sep`, `Oct`, `Nov`, `Dec`],
            tooltip: {
                enabled: false,
            },
            tooltip: {
                enabled: false
            },
            labels: {
                formatter: function (value) {
                    return value;
                },
                style: {
                    fontSize: "14px"
                }
            },
        },
        yaxis: {
            labels: {
                show: false
            },
        },
        tooltip: {
            x: {
                format: 'dd/MM/yy HH:mm'
            },
        },
    };

    var chart = new ApexCharts(document.querySelector(`#${chartId}`), options);
    chart.render();
}
createChartTwo('recent-orders', '#487fff');
// ================================ Recent Orders Chart End ================================ 


// ================================ Recent Orders Chart End ================================ 

// ================================ Custom Statistics Donut chart Start ================================ 
var options = {
    series: [nakit, krediKarti, iskonto, ikram], // Nakit, Kredi Kartı, İskonto, İkram
    colors: ['#FF9F29', '#487FFF', '#4CAF50', '#FF6347'], // Renkler: Nakit, Kredi Kartı, İskonto, İkram
    labels: ['Nakit', 'Kredi Karti', 'Iskonto', 'Ikram'],
    legend: {
        show: false
    },
    chart: {
        type: 'donut',
        height: 320,
        sparkline: {
            enabled: true // Remove whitespace
        },
        margin: {
            top: 0,
            right: 0,
            bottom: 0,
            left: 0
        },
        padding: {
            top: 0,
            right: 0,
            bottom: 0,
            left: 0
        }
    },
    stroke: {
        width: 0,
    },
    dataLabels: {
        enabled: false
    },
    responsive: [{
        breakpoint: 480,
        options: {
            chart: {
                width: 200
            },
            legend: {
                position: 'bottom'
            }
        }
    }],
};

var chart = new ApexCharts(document.querySelector("#statisticsDonutChart"), options);
chart.render();

// ================================ Custom Statistics Donut chart End ================================ 

// ================================ Revenue Report Chart Start ================================ 
// Günlük ve aylık veriler ve kategoriler
// Haftalık ve Aylık kategoriler
var weeklyCategories = ['Paz', 'Pzt', 'Sal', 'Car', 'Per', 'Cum', 'Cmt'];
var monthlyCategories = ['Oca', 'Sub', 'Mar', 'Nis', 'May', 'Haz', 'Tem', 'Agu', 'Eyl', 'Eki', 'Kas', 'Ara'];

// Haftalık ve aylık veri setleri
//var weeklySeries = [{
//    name: 'Kredi Karti',
//    data: [3000, 4000, 3500, 5000, 6000, 4500, 5500]
//}, {
//    name: 'Nakit',
//    data: [2500, 3000, 2700, 4000, 5000, 3500, 4500]
//}];

//var monthlySeries = [{
//    name: 'Kredi Karti',
//    data: [20000, 16000, 14000, 25000, 45000, 18000, 28000, 11000, 26000, 48000, 18000, 22000]
//}, {
//    name: 'Nakit',
//    data: [15000, 18000, 19000, 20000, 35000, 20000, 18000, 13000, 18000, 38000, 14000, 16000]
//}];

// Grafik oluşturma ayarları
// Grafik oluşturma ayarları
var options = {
    series: weeklySeries,
    chart: {
        type: 'bar',
        height: 250,
        toolbar: {
            show: false
        }
    },
    colors: ['#487FFF', '#FF9F29'],
    xaxis: {
        categories: weeklyCategories // Haftalık kategoriler burada
    },
    plotOptions: {
        bar: {
            borderRadius: 4,
            columnWidth: '50%'
        }
    },
    fill: {
        opacity: 1
    },
    dataLabels: {
        enabled: false
    },
    stroke: {
        show: true,
        width: 2,
        colors: ['transparent']
    }
};

// Grafik başlatma
var chart = new ApexCharts(document.querySelector("#paymentStatusChart"), options);
chart.render();

// Select kutusu değişikliklerini izleme ve tabloyu güncelleme
document.getElementById("timePeriodSelect").addEventListener('change', function () {
    var selectedValue = this.value;

    if (selectedValue === 'weekly') {
        chart.updateOptions({
            series: weeklySeries,
            xaxis: {
                categories: weeklyCategories
            }
        });
    } else if (selectedValue === 'monthly') {
        chart.updateOptions({
            series: monthlySeries,
            xaxis: {
                categories: monthlyCategories
            }
        });
    }
});

// ================================ Revenue Report Chart End ================================ 

// ================================ J Vector Map Start ================================ 
$('#world-map').vectorMap(
    {
        map: 'world_mill_en',
        backgroundColor: 'transparent',
        borderColor: '#fff',
        borderOpacity: 0.25,
        borderWidth: 0,
        color: '#000000',
        regionStyle: {
            initial: {
                fill: '#D1D5DB'
            }
        },
        markerStyle: {
            initial: {
                r: 5,
                'fill': '#fff',
                'fill-opacity': 1,
                'stroke': '#000',
                'stroke-width': 1,
                'stroke-opacity': 0.4
            },
        },
        markers: [{
            latLng: [35.8617, 104.1954],
            name: 'China : 250'
        },

        {
            latLng: [25.2744, 133.7751],
            name: 'AustrCalia : 250'
        },

        {
            latLng: [36.77, -119.41],
            name: 'USA : 82%'
        },

        {
            latLng: [55.37, -3.41],
            name: 'UK   : 250'
        },

        {
            latLng: [25.20, 55.27],
            name: 'UAE : 250'
        }],

        series: {
            regions: [{
                values: {
                    "US": '#487FFF ',
                    "SA": '#FF9F29',
                    "AU": '#45B369',
                    "CN": '#F86624',
                    "GB": '#487FFF',
                },
                attribute: 'fill'
            }]
        },
        hoverOpacity: null,
        normalizeFunction: 'linear',
        zoomOnScroll: false,
        scaleColors: ['#000000', '#000000'],
        selectedColor: '#000000',
        selectedRegions: [],
        enableZoom: false,
        hoverColor: '#fff',
    });
// ================================ J Vector Map End ================================