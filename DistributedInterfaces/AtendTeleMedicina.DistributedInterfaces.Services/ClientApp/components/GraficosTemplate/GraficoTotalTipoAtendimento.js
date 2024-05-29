export function getGraficoTotalTipoAtendimento(title, yAxisTitle) {
  return {
    chart: {
      type: 'column',
      height: 300
    },
    title: { text: title},
    subtitle: {text: ''},
    xAxis: {
      categories: [],
      crosshair: false
    },
    yAxis: {
      min: 0,
      title: {
        text: yAxisTitle
      }
    },

    tooltip: {
      headerFormat: '<span style="font-size:10px"></span><table>',
      pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
        '<td style="padding:0"><b>{point.y:.0f} Atendimentos</b></td></tr>',
      footerFormat: '</table>',
      shared: false,
      useHTML: false
    },
    plotOptions: {
      column: {
        pointPadding: 0.1,
        borderWidth: 0
      }
    },
    series: [{
      name: 'Totem',
      data: []

    }, {
      name: 'Presencial',
      data: []

    }, {
      name: 'TeleConsulta',
      data: []

    }]
  }
}
