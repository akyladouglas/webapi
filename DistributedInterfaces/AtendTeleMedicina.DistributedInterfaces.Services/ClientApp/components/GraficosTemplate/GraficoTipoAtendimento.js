export function getGraficoTipoAtendimento(title, yAxisTitle) {
  return {
    chart: {
      type: 'column',
      height: 383
    },
    title: {
      text: ''
    },
    subtitle: {
      text: ''
    },
    xAxis: {
      type: 'category',
      labels: {
        rotation: 0,
        style: {
          fontSize: '13px',
          fontFamily: 'Verdana, sans-serif'
        }
      }
    },
    yAxis: {
      min: 0,
      title: {
        text: 'Quantidade de atendimento por tipo'
      }
    },
    legend: {
      enabled: false
    },
    tooltip: {
      pointFormat: ''
    },

    series: [{
      name: 'Atendimentos',

      data: [

      ],
      dataLabels: {
        enabled: true,
        rotation: 0,
        color: '#FFFFFF',
        align: 'center',
        format: '{point.y:.0f}', // one decimal
        y: 25, // 10 pixels down from the top
        style: {
          fontSize: '13px',
          fontFamily: 'Verdana, sans-serif'
        }
      }
    }],
  }
}
