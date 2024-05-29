<template>
<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">

  <el-card class="box-card box-card--main-card">
    <el-row>
      <el-col :span="14">
        <h2 class="box-card--h2">Dashboard Atendimento Médico</h2>
      </el-col>
      <el-col :span="24">
        <FiltroDashboard :loading="loading.individuos" :params="params" @emit="onFiltrar" />
      </el-col>
    </el-row>

    <el-row :gutter="20">
      <el-col :offset="4" :xs="24" :sm="24" :md="12" :lg="8" :xl="8" class="text-center">
        <h3 class="dashboard--grafico-titulo">Atendimentos</h3>
        <highcharts :options="graficoAtendimentos"></highcharts>
      </el-col>
    </el-row>

  </el-card>

</el-col>
</template>

<script>
  import _api from '../../api'
  import countTo from 'vue-count-to'
  import FiltroDashboard from '../../components/shared/FiltroDashboard'
  
  export default {
    name: 'DashboardGeral',
    components: { countTo, FiltroDashboard },
    data () {
      return {
        graficoAtendimentos: {
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
              return `${this.profissionalNome}: ${this.y}`
            },
            symbolRadius: 0
          },
          credits: { enabled: true },
          series: [
            {
              innerSize: '50%',
              colorByPoint: true,
              data: this.data
            }
          ],
          plotOptions: {
            pie: {
              allowPointSelect: true,
              cursor: 'pointer',
              dataLabels: {
                enabled: false
              },
              showInLegend: true
            }
          },
          tooltip: {
            pointFormat: '{point.percentage:.1f}%'
          }
        },

        api: {
          graficoAtendimento: {},
        },
        loading: {
          graficoAtendimento: false,
        },

        params: {
          //dataInicial: this.moment().subtract(1, 'y').format('YYYY-MM-DD'),
          //dataFinal: this.moment().format('YYYY-MM-DD')
        }

      }
    },
    created () {
      this.getGraficoAtendimento()
    },
    methods: {
      async onFiltrar(val) {
        if (val) {
          this.params = {
            ...val.params
          }
        }
        await this.getGraficoAtendimento()
      },

      async getGraficoAtendimento() {
        let params = {
          ...this.params
        }
        this.loading.graficoAtendimento = true
        let { data, status } = await _api.dashboard.getContadorAtendimentos(params)

        //prop name para aparecer o nome quando passar o mouse e prop y para pegar no contador / propriedades do highchart
        data.forEach(x => {
          x.name = `${x.profissionalNome}`
          x.y = parseInt(x.atendimentos)
        })

        this.graficoAtendimentos.series[0].data = data
        console.log("this.graficoAtendimentos.series[0].data", this.graficoAtendimentos.series[0].data)

        if (data.length === 0) {
          this.$swal({
            title: "Atenção!",
            text: 'Nenhum registro encontrado!',
            icon: 'warning',
          })
        }
        this.loading.graficoAtendimento = false
      },
    }
  }
</script>
