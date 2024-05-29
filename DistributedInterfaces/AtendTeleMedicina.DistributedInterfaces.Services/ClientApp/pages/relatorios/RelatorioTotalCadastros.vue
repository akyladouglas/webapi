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
          <FiltroRelatorioTotalCadastros :loading="loading.relatorio" :params="params" @emit="onEmitFromFiltro" />
        </el-col>
      </el-row>

      <el-row v-show="listando && api.relatorio.length > 0">
        <el-col :span="24">
          <el-table ref="tabela" :data="api.relatorio" highlight-current-row border v-loading.body="loading.relatorio">
            <el-table-column label="Cidade Selecionada" prop="estado" show-overflow-tooltip />
            <el-table-column label="Outras Cidades" prop="estadoDiferente" show-overflow-tooltip />
            <el-table-column label="Total" prop="total" show-overflow-tooltip />
          </el-table>
        </el-col>

        <el-row :gutter="20" v-show="listando && api.relatorio.length > 0">
          <el-col :xs="24" :sm="24" :md="12" :lg="10" :xl="10" class="forms--margin-xs-from-top">
            <el-button v-if="!loading.relatorio" type="info" icon="fas fa-print" @click="onImprimirRelatorio"> Imprimir Relalório</el-button>
            <el-button v-if="!loading.relatorio" type="info" icon="fas fa-chart-pie" @click="onGerarGrafico"> Exibir Gráfico</el-button>
            <el-button v-if="loading.relatorio" type="info" icon="fa fa-spinner fa-spin" disabled> Aguarde ...</el-button>
          </el-col>
        </el-row>
      </el-row>

      <el-row>
        <el-col :xs="24" :sm="24" :md="24" :lg="12" :xl="12" style="margin-top: 50px" v-if="showGrafico">
          <highcharts :options="options"></highcharts>
        </el-col>
      </el-row>

    </el-card>

  </el-col>
</template>

<script>
  import axios from 'axios'
  import Utils from '../../mixins/Utils'
  import _api from '../../api'
  import FiltroRelatorioTotalCadastros from '../../components/shared/FiltroRelatorioTotalCadastros'
  import { mask } from 'vue-the-mask'
  export default {
    name: 'RelatorioTotalCadastros',
    mixins: [Utils],
    components: { FiltroRelatorioTotalCadastros },
    directives: { mask },
    data() {
      return {
        isValid: true,
        metodo: 'POST',
        listando: true,
        showGrafico: false,
        erros: [],
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
            text: 'Gráfico Total de Cadastros'
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
              name: 'Cadastros',
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
          cidades: [],
          mensagens: [],
          notificacoes: []
        },
        loading: {
          relatorio: false,
          ufs: false,
          cidades: false,
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
          url: `/api/Relatorios/ExcelRelatorioTotalCadastros`,
          params: this.params,
          responseType: 'blob'
        })
          .then((res) => {
            if (res.status === 200) {
              let blob = window.URL.createObjectURL(new Blob([res.data], { type: 'application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet' }))
              let link = document.createElement('a')
              link.href = blob
              link.setAttribute(`download`, `RelatorioTotalCadastros.xlsx`)
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
        if (!val.params.uf) {
          this.$swal({
            title: "Atenção!",
            text: 'O filtro de estado é obrigatório!',
            icon: 'warning',
          })
          return
        }
        if (!val.params.cidade_Id) {
          this.$swal({
            title: "Atenção!",
            text: 'O filtro de cidade é obrigatório!',
            icon: 'warning',
          })
          return
        }
        this.params = {
          ...val.params,
          page: 1
        }
        this.listando = true
        this.getRelatorioTotalCadastros()
      },
      async getRelatorioTotalCadastros () {
        this.loading.relatorio = true
        let { data, paginacao, status } = await _api.relatorios.listaRelatorioTotalCadastros(this.params)
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
      },
      onGerarGrafico () {
        this.showGrafico = true
        this.chartData = []
        this.api.relatorio.map(item => {
          this.chartData.push({
            name: `Selecionada: ${item.estado}`,
            y: item.estado,
            color: '#5cbf5c' }
          )

          this.chartData.push({
            name: `Outras: ${item.estadoDiferente}`,
            y: item.estadoDiferente,
            color: '#c0c0c0' }
          )
        })
        this.options.series[0].data = this.chartData
      }
    }
  }
</script>

<style scoped>
  .card label {
    text-align: left
  }
</style>
