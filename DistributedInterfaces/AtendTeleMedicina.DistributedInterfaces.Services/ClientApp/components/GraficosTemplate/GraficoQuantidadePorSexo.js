export function getGraficoPorSexo() {
  return {
    chart: {
      plotBackgroundColor: null,
      plotBorderWidth: null,
      plotShadow: false,
      type: 'pie',
      height: 500
    },
    title: null,
    style: {
      fontFamily: 'Roboto',
      fontWeight: '400'
    },
    height: null,
    legend: {
      labelFormatter: function () {
        return `${this.name}: ${this.y}`
      },
      symbolRadius: 0
    },
    credits: { enabled: true },
    series: [
      {
        innerSize: '50%',
        colorByPoint: true,
        data: []
      }
    ],
    plotOptions: {
      pie: {
        allowPointSelect: true,
        cursor: 'pointer',
        dataLabels: {
          enabled: false
        },
        showInLegend: true,
        colors: [
          '#78A1ED',
          '#ED78DB'
        ]
      }
    },

    tooltip: {
      pointFormat: '{point.percentage:.1f}%'
    }
  }
}
