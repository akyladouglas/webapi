<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">

    <el-card class="box-card box-card--main-card">

      <el-row>
        <el-col :span="14">
          <h2 class="box-card--h2">{{$route.meta.title}}</h2>
        </el-col>
      </el-row>

      <el-row v-show="listando">
        <el-col :span="24">
          <FiltroRelatorioCadastrosUf :loading="loading.relatorio" :params="params" @emit="onEmitFromFiltro" />
        </el-col>
      </el-row>

      <el-row v-show="listando && api.relatorio.length > 0">
        <el-col :xs="24" :sm="24" :md="8" :lg="8" :xl="8">
          <el-table ref="tabela" :data="api.relatorio"
                    highlight-current-row border show-summary
                    v-loading.body="loading.relatorio">
            <el-table-column label="UF" sortable prop="ufAbreviado" width="150" />
            <el-table-column label="Total" prop="total" show-overflow-tooltip />
          </el-table>
        </el-col>
        <el-col :xs="1" :sm="1" :md="1" :lg="1" :xl="1" v-if="showGrafico">&nbsp;</el-col>
        <el-col :xs="24" :sm="24" :md="15" :lg="15" :xl="15" v-if="showGrafico">
          <highcharts :options="options" />
        </el-col>
      </el-row>
    
      <el-row :gutter="20" v-show="listando && api.relatorio.length > 0">
        <el-col :xs="24" :sm="24" :md="12" :lg="10" :xl="10" class="forms--margin-xs-from-top">
          <el-button v-if="!loading.relatorio" type="info" icon="fas fa-print" @click="onImprimirRelatorio"> Imprimir Relalório</el-button>
          <el-button v-if="loading.relatorio" type="info" icon="fa fa-spinner fa-spin" disabled> Aguarde ...</el-button>
          <el-button v-if="!loading.relatorio" type="info" icon="fas fa-chart-pie" @click="onGerarGrafico"> Exibir Gráfico</el-button>
        </el-col>
      </el-row>

    </el-card>

  </el-col>
</template>

<script>
  import axios from 'axios'
  import Utils from '../../mixins/Utils'
  import _api from '../../api'
  import FiltroRelatorioCadastrosUf from '../../components/shared/FiltroRelatorioCadastrosUf'
  export default {
    name: 'RelatorioCadastrosUf',
    mixins: [Utils],
    components: { FiltroRelatorioCadastrosUf },
    data () {
      return {
        isDisabled: false,
        metodo: 'POST',
        listando: true,
        erros: [],
        total: 0,
        showGrafico: false,
        chartData: [],
        options: {
          chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            type: 'pie',
            height: 600
          },
          // title: false, // oculta titulo
          title: {
            align: 'center',
            margin: 0,
            text: 'Gráfico Cadastros por UF'
          },
          style: {
            fontFamily: 'Roboto',
            fontWeight: '400'
          },
          // width: 700,
          height: null,
          legend: {
            align: 'left',
            verticalAlign: 'top',
            layout: 'vertical',
            x: 0,
            y: 100
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
          relatorio: [],
          ufs: [],
          cidades: []
        },
        loading: {
          relatorio: false,
          ufs: false,
          cidades: false,
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
    methods: {
      onImprimirRelatorio () {
        this.loading.relatorio = true
        axios({
          method: 'GET',
          url: `/api/Relatorios/ExcelRelatorioCadastrosUf`,
          params: this.params,
          responseType: 'blob'
        })
          .then((res) => {
            if (res.status === 200) {
              let blob = window.URL.createObjectURL(new Blob([res.data], { type: 'application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet' }))
              let link = document.createElement('a')
              link.href = blob
              link.setAttribute(`download`, `RelatorioCadastrosUf.xlsx`)
              document.body.appendChild(link)
              link.click()
              this.loading.relatorio = false
            } else {
             // console.log('sem registros para gerar relatório')
            }
          })
          .catch((e) => {
            console.log(e.response);
            this.erroMsg = 'Erro, tenta efetuar a operação novamente ou entre em contato com o suporte'
            this.loading.relatorio = false
          })
      },
      onGerarGrafico () {
        this.showGrafico = true
        this.chartData = []
        this.api.relatorio.map(item => {
          this.chartData.push({
            name: item.ufAbreviado,
            y: item.total
          })
        })
        this.options.series[0].data = this.chartData
        this.count = []
        this.api.relatorio.map(item => {
          this.count.push(item.total)
        })
        this.total = this.count.reduce((total, currentElement) => total + currentElement)
      },
      onEmitFromFiltro (val) {
        this.params = {
          ...val.params,
          page: 1
        }
        this.listando = true
        this.getRelatorioCadastrosUf()
      },
      async getRelatorioCadastrosUf () {
        this.loading.relatorio = true
        let { data, paginacao, status } = await _api.relatorios.listaRelatorioCadastrosUf(this.params)
        if (data.length === 0) {
          this.$swal({
            title: "Atenção!",
            text: 'Nenhum registro encontrado!',
            icon: 'warning',
          })
        }
        this.api.relatorio = (status === 200) ? data : []
        this.params.page = (status === 200) ? paginacao.currentPage : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.relatorio = false

        this.count = []
        this.api.relatorio.map(item => {
          this.count.push(item.total)
        })
        this.total = this.count.reduce((total, currentElement) => total + currentElement)
      }
    }
  }
</script>

<style scoped>
  .card label {
    text-align: left
  }
</style>
