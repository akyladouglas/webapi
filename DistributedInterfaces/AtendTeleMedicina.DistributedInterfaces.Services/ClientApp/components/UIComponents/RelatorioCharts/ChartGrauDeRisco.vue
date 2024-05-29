<template>
  <el-row :gutter="20">
    <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6" class="text-center">
      <!--<h3 class="dashboard--grafico-titulo">Gráfico </h3>-->
      <highcharts :options="percentualRefeicoesGrafico"></highcharts>
    </el-col>
  </el-row>
</template>




<script>
  import countTo from 'vue-count-to'

  export default {
    name: 'ChartGrauDeRisco',
    components: { countTo },
    data() {
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
      }
    }
  }
</script>
