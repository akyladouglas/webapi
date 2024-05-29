export function getGraficoPesoAltura(title, yAxisTitle) {
  return {
    chart: { type: 'line' },
    title: { text: title },
    subtitle: { text: '' },
    xAxis: {
      categories: [],
      title: { text: 'Idade(Ano e Mês quando houve o atendimento)' }
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
      name: '',
      data: []
    }]
  }
}
