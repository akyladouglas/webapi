<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">

    <el-card class="box-card box-card--main-card">
      <el-row v-show="listando">
        <el-col :span="24">
          <FiltroRelatorioUm :loading="loading.individuos" :params="params" @emit="onEmitFromFiltro" />
         </el-col>
      </el-row>

      <el-row v-show="listando && api.individuos.length > 0">
        <el-col :span="24">
          <el-table ref="tabela" :data="api.individuos" highlight-current-row border v-loading.body="loading.individuos">
            <el-table-column label="Nome" sortable prop="nomeCompleto" fixed width="300" show-overflow-tooltip />
            <el-table-column align="center" label="Grau de Risco" prop="corStatus" width="130">
              <template slot-scope="scope">
                <el-tag effect="dark" :class="corRisco(scope.row.corStatus)">{{ scope.row.corStatus }}</el-tag>
              </template>
            </el-table-column>
            <el-table-column label="Cpf" prop="cpf" width="120" />
            <el-table-column label="Celular" prop="telefoneCelular" width="120" />
            <el-table-column label="UF" prop="ufAbreviado" width="70" />
            <el-table-column label="Cidade" prop="cidade.nome" show-overflow-tooltip />
          </el-table>
        </el-col>

        <el-col :span="24" v-show="listando">
          <el-pagination @size-change="handleSizeChange"
                         @current-change="handleCurrentChange"
                         :current-page.sync="params.page"
                         :page-sizes="[10,25,50,100]"
                         :page-size="params.pageSize"
                         :total="params.total"
                         layout="total, sizes, prev, pager, next, jumper">
          </el-pagination>
        </el-col>
        </el-row>
        <el-row :gutter="20" v-show="listando && api.individuos.length > 0">
          <el-col :xs="24" :sm="24" :md="12" :lg="10" :xl="10" class="forms--margin-xs-from-top">
            <el-button v-if="!loading.relatorio" type="info" icon="fas fa-print" @click="onImprimirRelatorio"> Imprimir Relalório</el-button>
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
  import _enums from '../../api/Enums'
  import FiltroRelatorioUm from '../../components/shared/FiltroRelatorioUm'
  export default {
    name: 'RelatorioIndividuo',
    mixins: [Utils],
    components: { FiltroRelatorioUm },
    data () {
      return {
        isDisabled: false,
        isValid: true,
        metodo: 'POST',
        listando: true,
        erros: [],
        formIndividuo: {},
        enums: {
          coresStatus: _enums.coresStatus
        },
        listaIndividuos: [],
        api: {
          individuos: [],
          ufs: [],
          cidades: []

        },
        loading: {
          individuos: false,
          ufs: false,
          cidades: false,
          relatorio: false
        },
        params: {
          page: 1,
          pageSize: 10,
          sort: 'i.NomeCompleto ASC',
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
          url: `/api/Relatorios/ExcelRelatorioIndividuo`,
          params: this.params,
          responseType: 'blob'
        })
          .then((res) => {
            if (res.status === 200) {
              let blob = window.URL.createObjectURL(new Blob([res.data], { type: 'application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet' }))
              let link = document.createElement('a')
              link.href = blob
              link.setAttribute(`download`, `RelatorioIndividuos.xlsx`)
              document.body.appendChild(link)
              link.click()
              this.loading.relatorio = false
            } else {
              //console.log('sem registros para gerar relatório')
            }
          })
          .catch((e) => {
            console.log(e.response)
            this.erroMsg = 'Erro, tenta efetuar a operação novamente ou entre em contato com o suporte'
            this.loading.relatorio = false
          })
      },
      onEmitFromFiltro (val) {
        this.params = {
          ...this.params,
          ...val.params,
          page: 1
        }
        this.listando = true
        this.getRelatorioIndividuos()
      },
      async getRelatorioIndividuos () {
        this.loading.individuos = true
        let { data, paginacao, status } = await _api.relatorios.listaRelatorioIndividuo(this.params)
        this.api.individuos = (status === 200) ? data : []
        this.params.page = (status === 200) ? paginacao.currentPage : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.individuos = false
      },
      corRisco (val) {
        let cor = ''
        switch (val) {
          case 0:
            cor = 'grau--cinza'
            break
          case 1:
            cor = 'grau--verde'
            break
          case 2:
            cor = 'grau--amarelo'
            break
          case 3:
            cor = 'grau--laranja'
            break
          case 4:
            cor = 'grau--vermelho'
            break
          default:
            cor = 'grau--cinza'
            break
        }
        return cor
      },
      resetForm (form) {
        this.$refs[form].resetFields()
      },
      handleSizeChange (val) {
        this.params.pageSize = val
      },
      handleCurrentChange (val) {
        this.params.currentPage = val
        this.getRelatorioIndividuos()
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
  .card label{
    text-align:left
  }
</style>
