export function getGraficoGeral() {
  return {
    chart: {
      plotBackgroundColor: null,
      plotBorderWidth: null,
      plotShadow: false,
      type: 'pie',
      height: 400
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
          '#778899',
          '#005A9C',
          '#228B22',
          '#FF0000'
        ]
      }
    },
  }
}
