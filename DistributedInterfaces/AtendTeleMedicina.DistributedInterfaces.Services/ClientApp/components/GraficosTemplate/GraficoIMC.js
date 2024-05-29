export function getGraficoIMC(title, yAxisTitle) {
  return {
    chart: { type: 'line' },
    title: { text: title },
    subtitle: { text: '' },
    xAxis: {
      categories: [],
      title: { text: 'Idade(Ano e Mês quando houve o atendimento)' }
    },
    yAxis: {
      title: { text: yAxisTitle },
      plotBands: [{ // Laranja Baixo Peso
        from: 0.0,
        to: 18.4,
        color: 'rgba(255, 236, 220, 0.8)',
        label: {
          text: 'Baixo Peso',
          style: { color: '#606060' }
        }
      },
      { // Verde Adequado ou Eutrófico
        from: 18.5,
        to: 24.9,
        color: 'rgba(221, 255, 221, 0.8)',
        label: {
          text: 'Adequado ou Eutrófico',
          style: { color: '#606060' }
        }
      },
      { // Amarelo Sobrepeso
        from: 25.0,
        to: 29.9,
        color: 'rgba(255, 255, 221, 0.8)',
        label: {
          text: 'Sobrepeso',
          style: { color: '#606060' }
        }
      },
      { // Vermelho Obesidade
        from: 30,
        to: 100,
        color: 'rgba(255, 221, 221, 0.8)',
        label: {
          text: 'Obesidade',
          style: { color: '#606060' }
        }
      }]
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
