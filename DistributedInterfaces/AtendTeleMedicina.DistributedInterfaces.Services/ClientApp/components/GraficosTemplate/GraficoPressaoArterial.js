export function getGraficoPressaoArterial(title, yAxisTitle, xAxisTitle) {
  return {
      chart: { type: 'line' },
      title: { text: title },
      subtitle: { text: '' },
      xAxis: {
        categories: [],
        title: { text: xAxisTitle }
      },
      yAxis: {
        title: { text: yAxisTitle }
      },
      plotOptions: {
        line: {
          dataLabels: { enabled: true },
          enableMouseTracking: true
        }
      },
      series: [{
        name: 'Pressão Sistólica',
        data: []
      },
      {
        name: 'Pressão Diastólica',
        data: []
      }]
  }
}
