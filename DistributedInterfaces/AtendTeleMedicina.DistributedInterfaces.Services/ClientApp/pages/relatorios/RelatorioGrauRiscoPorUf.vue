

<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">

    <el-card class="box-card box-card--main-card">

      <el-row>
        <el-col :span="14">
          <h2 class="box-card--h2">{{$route.meta.title}}</h2>
        </el-col>
        
        <el-col :span="10" class="text-right">
          <el-form :inline="true">
            <el-form-item>
              <el-button v-if="!listando" style="margin-right: -10px" icon="fas fa-undo-alt" type="warning" @click="onListar('formIndividuo')"> Voltar</el-button>
            </el-form-item>
          </el-form>
        </el-col>
      </el-row>

      <el-row>
        <el-col :span="24">
          <FiltroRelatorioGrauDeRiscoUf :loading="loading.individuos" :params="params" @emit="onEmitFromFiltro" />
        </el-col>
      </el-row>

        <el-row v-show="listando && api.dados.length > 0">
          <el-col :xs="24" :sm="24" :md="24" :lg="12" :xl="12">
            <el-table ref="tabela" :data="api.dados"
                      highlight-current-row border
                      v-loading.body="loading.dados"
                      class="table--individuos table--row-click">
              <el-table-column label="Grau de Risco" prop="grauDeRisco" />
              <el-table-column label="Total" prop="total" />
            </el-table>
          </el-col>

          <el-col :xs="24" :sm="24" :md="24" :lg="12" :xl="12" v-if="showGrafico">
            <highcharts :options="options"></highcharts>
          </el-col>

          <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="forms--margin-xs-from-top">
            <el-button v-if="!loading.relatorio" type="info" icon="fas fa-print" @click="onImprimirRelatorio"> Imprimir Relalório</el-button>
            <el-button v-if="!loading.relatorio" type="info" icon="fas fa-chart-pie" @click="onGerarGrafico"> Exibir Gráfico</el-button>
            <el-button v-if="loading.relatorio" type="info" icon="fa fa-spinner fa-spin" disabled> Aguarde ...</el-button>
          </el-col>

        </el-row>
</el-card>

  </el-col>
  
</template>

<script>
  import axios from 'axios'
  import Utils from '../../mixins/Utils'
  import _api from '../../api'
  import FiltroRelatorioGrauDeRiscoUf from '../../components/shared/FiltroRelatorioGrauDeRiscoUf'
  export default {
    name: 'RelatorioGrauRiscoPorUf',
    mixins: [Utils],
    components: { FiltroRelatorioGrauDeRiscoUf },
    data () {
      return {
        showRelatorio: true,
        showGrafico: false,
        toogle: false,
        isDisabled: false,
        isValid: true,
        metodo: 'POST',
        listando: true,
        chartData: [],
        options: {
          chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            type: 'pie',
            height: 300
          },
          // title: false, // oculta titulo
          title: {
            align: 'center',
            margin: 0,
            text: 'Gráfico Grau de Risco'
          },
          style: {
            fontFamily: 'Roboto',
            fontWeight: '400'
          },
          // width: 700,
          height: null,
          legend: {
            align: 'right',
            verticalAlign: 'top',
            layout: 'vertical',
            x: 0,
            y: 100,
            margin: -50
          },
          credits: { enabled: false },
          series: [
            {
              name: 'Total',
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
                enabled: true,
                format: '<b>{point.name}</b>: {point.percentage:.1f} %'
              },
              showInLegend: true
            }
          }
        },
        api: {
          dados: [],
          ufs: [],
          cidades: [],
          mensagens: [],
          notificacoes: []
        },
        loading: {
          dados: false,
          ufs: false,
          cidades: false,
          mensagens: false,
          notificacoes: false,
          atendimento: false,
          relatorio: false
        },
        params: {
          page: 1,
          pageSize: 10,
          sort: 'CorStatus DESC',
          total: 0,
          ufs: {
            skip: 1,
            take: 30,
            sort: '+UfAbreviado',
            total: 0
          },
          cidades: {
            skip: 1,
            take: 999,
            sort: '+Nome',
            total: 0,
            ufAbreviado: null
          }
        }
      }
    },
    async mounted () {
      //this.getRelatorioGrauDeRisco()
    },
    methods: {
      onImprimirRelatorio () {
        this.loading.relatorio = true
        axios({
          method: 'GET',
          url: `/api/Relatorios/ExcelRelatorioGrauDeRiscoUf`,
          params: this.params,
          responseType: 'blob'
        })
          .then((res) => {
            if (res.status === 200) {
              let blob = window.URL.createObjectURL(new Blob([res.data], { type: 'application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet' }))
              let link = document.createElement('a')
              link.href = blob
              link.setAttribute(`download`, `RelatorioGrauDeRiscoUf.xlsx`)
              document.body.appendChild(link)
              link.click()
              this.loading.relatorio = false
            } else {
              //console.log('sem registros para gerar relatório')
            }
          })
          .catch((e) => {
            console.log(e.response);
            this.erroMsg = 'Erro, tenta efetuar a operação novamente ou entre em contato com o suporte'
            this.loading.relatorio = false
          })
      },
      onEmitFromFiltro (val) {
        this.params = {
          ...val.params,
          page: 1
        }
        this.listando = true
        this.getRelatorioGrauDeRisco()
      },
      async getRelatorioGrauDeRisco () {
        this.showGrafico = false
        this.loading.dados = true
        let { data, paginacao, status } = await _api.relatorios.listaRelatorioGrauDeRiscoUf(this.params)
        if (data.length === 0) {
          this.$swal({
            title: "Atenção!",
            text: 'Nenhum registro encontrado!',
            icon: 'warning',
          })
        }
        this.api.dados = (status === 200) ? data : []
        // if (data.length > 0) this.onGerarGrafico()
        this.params.page = (status === 200) ? paginacao.currentPage : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.dados = false
      },
      onGerarGrafico () {
        this.showGrafico = true
        this.chartData = []
        this.api.dados.map(item => {
          this.chartData.push({
            name: item.grauDeRisco,
            y: item.total,
            color: this.corRisco(item.corStatus) })
        })
        this.options.series[0].data = this.chartData
      },
      corRisco (val) {
        let cor = ''
        switch (val) {
          case 0:
            cor = '#c0c0c0'
            break
          case 1:
            cor = '#5cbf5c'
            break
          case 2:
            cor = '#f7e084'
            break
          case 3:
            cor = '#e6a23c'
            break
          case 4:
            cor = '#ff7272'
            break
          default:
            cor = '#c0c0c0'
            break
        }
        return cor
      }
    }
  }
</script>

<style scoped>
  .grau--cinza {
    background: #C0C0C0 !important;
    border: 1px solid #a99696 !important;
    color: #000;
  }

  .grau--verde {
    background: #5cbf5c !important;
    border: 1px solid #5cbf5c !important;
  }

  .grau--amarelo {
    background: #FFFF66 !important;
    border: 1px solid #FFFF33 !important;
    color: #000;
  }

  .grau--laranja {
    background: #FF8000 !important;
    border: 1px solid #FF8000 !important;
    color: #000;
  }

  .grau--vermelho {
    background: #FF0000 !important;
    border: 1px solid #FF0000 !important;
    color: #FFF;
  }
</style>
