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
          <FiltroRelatorioAtendimentoPaciente :loading="loading.relatorio" :params="params" @emit="onEmitFromFiltro" />
        </el-col>
      </el-row>

      <el-row v-show="listando && api.relatorio.length > 0">
        <el-col :span="12">
          <el-table ref="tabela" :data="api.relatorio" highlight-current-row border v-loading.body="loading.relatorio">
            <el-table-column label="Nome" prop="nome" show-overflow-tooltip />
            <el-table-column label="Total" prop="total" show-overflow-tooltip width="100" />
          </el-table>
        </el-col>
        </el-row>
        <el-row :gutter="20" v-show="listando && api.relatorio.length > 0">
          <el-col :xs="24" :sm="24" :md="12" :lg="10" :xl="10" class="forms--margin-xs-from-top">
            <el-button v-if="!loading.relatorio" type="info" icon="fas fa-print" @click="onImprimirRelatorio"> Imprimir Relalório</el-button>
            <el-button v-if="loading.relatorio" type="info" icon="fa fa-spinner fa-spin" disabled> Aguarde ...</el-button>
          </el-col>
        </el-row>

    </el-card>

  </el-col>
</template>

<style>
  .card label {
    text-align: left
  }
</style>

<script>
  import axios from 'axios'
  import Utils from '../../mixins/Utils'
  import _api from '../../api'
  import FiltroRelatorioAtendimentoPaciente from '../../components/shared/FiltroRelatorioAtendimentoPaciente'
  import { mask } from 'vue-the-mask'
  export default {
    name: 'RelatorioAtendimentoPaciente',
    mixins: [Utils],
    components: { FiltroRelatorioAtendimentoPaciente },
    data () {
      return {
        metodo: 'POST',
        listando: true,
        erros: [],
        listarelatorio: [],
        api: {
          relatorio: [],
          ufs: [],
          cidades: []
        },
        loading: {
          relatorio: false,
          ufs: false,
          cidades: false
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
          url: `/api/Relatorios/ExcelRelatorioAtendimentoPorMedico`,
          params: this.params,
          responseType: 'blob'
        })
          .then((res) => {
            if (res.status === 200) {
              let blob = window.URL.createObjectURL(new Blob([res.data], { type: 'application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet' }))
              let link = document.createElement('a')
              link.href = blob
              link.setAttribute(`download`, `RelatorioAtendimentoPorMedico.xlsx`)
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
        let { data, paginacao, status } = await _api.relatorios.listaRelatorioAtendimentoPorMedico(this.params)
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
      handleSizeChange (val) {
        this.params.pageSize = val
      },
      handleCurrentChange (val) {
        this.params.currentPage = val
        this.getRelatorioCadastrosUf()
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
