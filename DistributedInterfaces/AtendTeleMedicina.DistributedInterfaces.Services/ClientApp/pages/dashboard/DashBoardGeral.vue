<template>
<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">

  <el-card class="box-card box-card--main-card">
    <el-row :gutter="20">
      <el-col :span="24" class="text-right dashboard--periodo">
        <el-date-picker v-model="dataFinal" type="date" placeholder="Data Final" format="dd-MM-yyyy" />
        <el-button type="success" icon="el-icon-check" circle />
      </el-col>
    </el-row>

    <el-row :gutter="20">
      <el-col :xs="24" :sm="24" :md="6" :lg="6" :xl="6" class="text-center">
        <h3 class="dashboard--contador-titulo">Taxa de Ocupação</h3>
        <countTo class="dashboard--contador-valor" :startVal="0" :endVal="3.73" :decimals="2" suffix="%" separator="." :duration="5000" />
      </el-col>
      <el-col :xs="24" :sm="24" :md="6" :lg="6" :xl="6" class="text-center">
        <h3 class="dashboard--contador-titulo">Refeições</h3>
        <countTo class="dashboard--contador-valor" :startVal="0" :endVal="25831" separator="." :duration="5000" />
      </el-col>
      <el-col :xs="24" :sm="24" :md="6" :lg="6" :xl="6" class="text-center">
        <h3 class="dashboard--contador-titulo">Roupa Suja (kg)</h3>
        <countTo class="dashboard--contador-valor" :startVal="0" :endVal="3840" separator="." :duration="5000" />
      </el-col>
      <el-col :xs="24" :sm="24" :md="6" :lg="6" :xl="6" class="text-center">
        <h3 class="dashboard--contador-titulo">Gasto Total (Medicamentos)</h3>
        <countTo class="dashboard--contador-valor" :startVal="0" :endVal="218739" prefix="R$ " separator="." :decimals="2" decimal="," :duration="5000" />
      </el-col>
    </el-row>

    <el-row :gutter="20">
      <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6" class="text-center">
        <h3 class="dashboard--grafico-titulo">Percentual de Refeições</h3>
        <highcharts :options="percentualRefeicoesGrafico"></highcharts>
      </el-col>
      <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12" class="text-center">
        <h3 class="dashboard--grafico-titulo">Pacientes Por Dia</h3>
        <highcharts :options="pacientesPorDiaGrafico"></highcharts>
      </el-col>
      <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6" class="text-center">
        <h3 class="dashboard--contador-box-titulo">Gasto Total de Material</h3>
        <div class="dashboard--contador-box">
          <countTo class="dashboard--contador-valor" :startVal="0" :endVal="7700" prefix="R$ " separator="." :decimals="2" decimal="," :duration="5000" />
        </div>
        <h3 class="dashboard--contador-box-titulo">Nº Consulta no P.A.</h3>
        <div class="dashboard--contador-box">
          <countTo class="dashboard--contador-valor" :startVal="0" :endVal="133" separator="." :duration="5000" />
        </div>
        <h3 class="dashboard--contador-box-titulo">Nº Consultas Especialista</h3>
        <div class="dashboard--contador-box">
          <countTo class="dashboard--contador-valor" :startVal="0" :endVal="49" separator="." :duration="5000" />
        </div>

      </el-col>
    </el-row>

    <el-row :gutter="20">
      <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6" class="text-center">
        <h3 class="dashboard--grafico-titulo">CID PA / CID Internação</h3>
        <highcharts :options="percentualConvenios"></highcharts>
      </el-col>
      <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12" class="text-center">
        <h3 class="dashboard--grafico-titulo">Pacientes em Alta</h3>
        <highcharts :options="pacientesEmAltaGrafico"></highcharts>
      </el-col>
      <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6" class="text-center">
        <h3 class="dashboard--contador-box-titulo">Nº Exames Laboratoriais</h3>
        <div class="dashboard--contador-box">
          <countTo class="dashboard--contador-valor" :startVal="0" :endVal="125" separator="." :duration="5000" />
        </div>
        <h3 class="dashboard--contador-box-titulo">N° Exames Radiograficos</h3>
        <div class="dashboard--contador-box">
          <countTo class="dashboard--contador-valor" :startVal="0" :endVal="35" separator="." :duration="5000" />
        </div>
        <h3 class="dashboard--contador-box-titulo">Faturamento</h3>
        <div class="dashboard--contador-box">
          <countTo class="dashboard--contador-valor" :startVal="0" :endVal="4077" prefix="R$ " separator="." :decimals="2" decimal="," :duration="5000" />
        </div>
      </el-col>
    </el-row>

    <el-row :gutter="20" class="dashboard--box-relatorios">
      <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6" class="text-center">
        <el-button class="dashboard--button dashboard--button--color1">Pacientes</el-button>
      </el-col>
      <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6" class="text-center">
        <el-button class="dashboard--button dashboard--button--color2">Medicamentos</el-button>
      </el-col>
      <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6" class="text-center">
        <el-button class="dashboard--button dashboard--button--color3">Exames</el-button>
      </el-col>
      <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6" class="text-center">
        <el-button class="dashboard--button dashboard--button--color4">Lavanderia</el-button>
      </el-col>
      <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6" class="text-center">
        <el-button class="dashboard--button dashboard--button--color5">Nutrição</el-button>
      </el-col>
      <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6" class="text-center">
        <el-button class="dashboard--button dashboard--button--color6">Faturamento</el-button>
      </el-col>
      <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6" class="text-center">
        <el-button class="dashboard--button dashboard--button--color7">Indicadores</el-button>
      </el-col>
      <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6" class="text-center">
        <el-button class="dashboard--button dashboard--button--color8">Laboratório</el-button>
      </el-col>
    </el-row>

  </el-card>

</el-col>
</template>

<script>
  import countTo from 'vue-count-to'

  export default {
    name: 'DashBoardGeral',
    components: { countTo },
    data () {
      return {
        dataFinal: '',
          percentualRefeicoesGrafico: {
            chart: {
              plotBackgroundColor: null,
              plotBorderWidth: null,
              plotShadow: false,
              type: 'pie',
              height: 280
            },
            title: null,
            style: {
              fontFamily: 'Roboto',
              fontWeight: '400',
            },
            legend: {
              layout: 'horizontal',
              enabled: true,
              align: 'center',
              verticalAlign: 'bottom',
              y: 0,
              padding: 0,
              itemMarginTop: 0,
              itemMarginBottom: 0,
              itemStyle: {
                font: '9pt Roboto, Verdana, sans-serif',
                color: '#000',
                weight: 200
              }
            },
            credits: { enabled: false },
            plotOptions: {
              pie: {
                dataLabels: {
                  format: '{y} %',
                  enabled: true,
                  distance: -25,
                  style: {
                    fontWeight: 'thin',
                    color: 'white'
                  }
                },
                cursor: "pointer",
                showInLegend: true,
              },
            },
            series: [{
              innerSize: '50%',
              colorByPoint: true,
              data: [
                {
                  name: "Paciente",
                  y: 40,
                  color: "#F7D54F"
                },
                {
                  name: "Acompanhante",
                  y: 20,
                  color: "#468C00"
                },
                {
                  name: "Funcionário",
                  y: 40,
                  color: "#D7D7D7"
                }
              ]
            }],
            tooltip: {
            formatter: function () {
              return (
                `${this.point.name}<br /> ${this.point.y.toLocaleString()}%`
              )
            }
          }
        },
        percentualConvenios: {
            chart: {
              plotBackgroundColor: null,
              plotBorderWidth: null,
              plotShadow: false,
              type: 'pie',
              height: 280
            },
            title: null,
            style: {
              fontFamily: 'Roboto',
              fontWeight: '400',
            },
            legend: {
              layout: 'horizontal',
              enabled: true,
              align: 'center',
              verticalAlign: 'bottom',
              y: 0,
              padding: 0,
              itemMarginTop: 0,
              itemMarginBottom: 0,
              itemStyle: {
                font: '9pt Roboto, Verdana, sans-serif',
                color: '#000',
                weight: 200
              }
            },
            credits: { enabled: false },
            plotOptions: {
              pie: {
                dataLabels: false,
                cursor: "pointer",
                showInLegend: true,
              },
            },
            series: [{
              innerSize: '50%',
              colorByPoint: true,
              data: [
                {
                  name: "UNIMED",
                  y: 30
                },
                {
                  name: "AFRAFEP",
                  y: 2
                },
                {
                  name: "FUSEX",
                  y: 9
                },
                {
                  name: "BRADESCO SAUDE",
                  y: 12
                },
                {
                  name: "CASSI",
                  y: 14
                },
                {
                  name: "GEAP",
                  y: 8
                },
                {
                  name: "PARTICULAR",
                  y: 19
                },
                {
                  name: "POSTAL SAUDE",
                  y: 4
                },
                {
                  name: "SAUDE CAIXA",
                  y: 2
                }
              ]
            }],
            tooltip: {
            formatter: function () {
              return (
                `${this.point.name}<br /> ${this.point.y.toLocaleString()}%`
              )
            }
          }
        },
        pacientesPorDiaGrafico: {
          chart: {
              type: 'column',
              height: 280
            },
          style: {
            fontFamily: 'Roboto',
            fontWeight: '400',
          },
          credits: { enabled: false },
          title: null,
          subtitle: null,
          yAxis: {
            title: null,
            plotLines: [{
                value: 0,
                color: 'red',
                dashStyle: 'dash',
                width: 2,
                zIndex: 10,
                label: {}
            }]
          },
          xAxis: {
              categories: [
                  '1',
                  '2',
                  '3',
                  '4',
                  '5',
                  '6',
                  '7',
                  '8',
                  '9',
                  '10',
                  '11',
                  '12',
                  '13',
                  '14',
                  '15',
                  '16',
                  '17',
                  '18',
                  '19',
                  '20',
                  '21',
                  '22',
                  '23',
                  '24',
                  '25',
                  '26',
                  '27',
                  '28',
                  '29',
                  '30',
                  '31'
              ],
              crosshair: true
          },
          plotOptions: {
            column: {
                pointPadding: 0.2,
                borderWidth: 0
            }
          },
          legend: {
            itemStyle: {
              font: '9pt Roboto, Verdana, sans-serif',
              color: '#000',
              weight: 200
            }
          },
          series: [{
            showInLegend: false,
            name: 'Pacientes Por Dia',
            data: [],
            cursor: 'pointer',
            point: {
              events: {
                  click: function () {
                      alert('Dia: ' + this.category + ', valor: ' + this.y);
                  }
              }
            }
          }],
          tooltip: {
            formatter: function () {
              return (
                `${this.point.y}`
              )
            }
          }
        },
        pacientesPorDiaDados: {
          media: 0,
          data: [
            { name: 1, y: 39 },
            { name: 2, y: 29 },
            { name: 3, y: 24 },
            { name: 4, y: 18 },
            { name: 5, y: 17 },
            { name: 6, y: 22 },
            { name: 7, y: 25 },
            { name: 8, y: 13 },
            { name: 9, y: 26 },
            { name: 10, y: 50 },
            { name: 11, y: 14 },
            { name: 12, y: 19 },
            { name: 13, y: 38 },
            { name: 14, y: 20 },
            { name: 15, y: 28 },
            { name: 16, y: 25 },
            { name: 17, y: 36 },
            { name: 18, y: 10 },
            { name: 19, y: 21 },
            { name: 20, y: 21 },
            { name: 21, y: 25 },
            { name: 22, y: 19 },
            { name: 23, y: 33 },
            { name: 24, y: 10 },
            { name: 25, y: 13 },
            { name: 26, y: 30 },
            { name: 27, y: 28 },
            { name: 28, y: 23 },
            { name: 29, y: 14 },
            { name: 30, y: 36 },
            { name: 31, y: 18 }
          ]
        },
        pacientesEmAltaGrafico: {
          chart: {
              type: 'column',
              height: 280
            },
          style: {
            fontFamily: 'Roboto',
            fontWeight: '400',
          },
          credits: { enabled: false },
          title: null,
          subtitle: null,
          yAxis: {
            title: null,
            plotLines: [{
                value: 0,
                color: 'red',
                dashStyle: 'dash',
                width: 2,
                zIndex: 10,
                label: {}
            }]
          },
          xAxis: {
              categories: [
                  '1',
                  '2',
                  '3',
                  '4',
                  '5',
                  '6',
                  '7',
                  '8',
                  '9',
                  '10',
                  '11',
                  '12',
                  '13',
                  '14',
                  '15',
                  '16',
                  '17',
                  '18',
                  '19',
                  '20',
                  '21',
                  '22',
                  '23',
                  '24',
                  '25',
                  '26',
                  '27',
                  '28',
                  '29',
                  '30',
                  '31'
              ],
              crosshair: true
          },
          plotOptions: {
            column: {
                pointPadding: 0.2,
                borderWidth: 0
            }
          },
          legend: {
            itemStyle: {
              font: '9pt Roboto, Verdana, sans-serif',
              color: '#000',
              weight: 200
            }
          },
          series: [{
              showInLegend: false,
              name: 'Pacientes Por Dia',
              data: [],
              cursor: 'pointer',
              point: {
                events: {
                    click: function () {
                        alert('Dia: ' + this.category + ', valor: ' + this.y);
                    }
                }
            }
          }],
          tooltip: {
            formatter: function () {
              return (
                `${this.point.y}`
              )
            }
          }
        },
        pacientesEmAltaDados: {
          media: 0,
          data: [
            { name: 1, y: 10 },
            { name: 2, y: 8 },
            { name: 3, y: 6 },
            { name: 4, y: 11 },
            { name: 5, y: 10 },
            { name: 6, y: 14 },
            { name: 7, y: 9 },
            { name: 8, y: 13 },
            { name: 9, y: 6 },
            { name: 10, y: 10 },
            { name: 11, y: 2 },
            { name: 12, y: 4 },
            { name: 13, y: 9 },
            { name: 14, y: 8 },
            { name: 15, y: 11 },
            { name: 16, y: 2 },
            { name: 17, y: 5 },
            { name: 18, y: 10 },
            { name: 19, y: 12 },
            { name: 20, y: 8 },
            { name: 21, y: 7 },
            { name: 22, y: 10 },
            { name: 23, y: 4 },
            { name: 24, y: 9 },
            { name: 25, y: 4 },
            { name: 26, y: 5 },
            { name: 27, y: 9 },
            { name: 28, y: 10 },
            { name: 29, y: 5 },
            { name: 30, y: 7 },
            { name: 31, y: 6 }
          ]
        },
      }
    },
    created () {
      this.calcPacientesPorDia()
      this.calcPacientesEmAlta()
    },
    methods: {
      calcPacientesPorDia () {
        this.pacientesPorDiaDados.data = this.pacientesPorDiaDados.data.map(item => {
          if (new Date().getDate() === item.name) {
            item = { ...item, color: '#0033FF' }
          } else {
            item = item
          }
          return item;
        });
        let totalDoPerido = this.pacientesPorDiaDados.data.reduce((a, p) => +a + +p.y, 0)
        let totalDeDias = this.pacientesPorDiaDados.data.length
        this.pacientesPorDiaDados.media = (totalDoPerido / totalDeDias).toFixed(0)
        this.pacientesPorDiaGrafico.series[0].data = this.pacientesPorDiaDados.data
        this.pacientesPorDiaGrafico.yAxis.plotLines[0].value = this.pacientesPorDiaDados.media
        this.pacientesPorDiaGrafico.yAxis.plotLines[0].label.text = `Media (${this.pacientesPorDiaDados.media})`
      },
      calcPacientesEmAlta () {
        this.pacientesEmAltaDados.data = this.pacientesEmAltaDados.data.map(item => {
          if (new Date().getDate() === item.name) {
            item = { ...item, color: '#0033FF' }
          } else {
            item = item
          }
          return item;
        });
        let totalDoPerido = this.pacientesEmAltaDados.data.reduce((a, p) => +a + +p.y, 0)
        let totalDeDias = this.pacientesEmAltaDados.data.length
        this.pacientesEmAltaDados.media = (totalDoPerido / totalDeDias).toFixed(0)
        this.pacientesEmAltaGrafico.series[0].data = this.pacientesEmAltaDados.data
        this.pacientesEmAltaGrafico.yAxis.plotLines[0].value = this.pacientesEmAltaDados.media
        this.pacientesEmAltaGrafico.yAxis.plotLines[0].label.text = `Media (${this.pacientesEmAltaDados.media})`
      },
      onSelectCid (val) {
        //console.log('enviar para o objeto para o form')
      }
    }
  }
</script>
