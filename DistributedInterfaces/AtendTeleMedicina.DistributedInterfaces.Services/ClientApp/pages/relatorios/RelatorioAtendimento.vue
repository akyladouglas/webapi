<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">

    <el-card class="box-card box-card--main-card">

      <el-row>
        <el-col :span="14">
          <h2 class="box-card--h2">Relatório De Atendimento Médico</h2>
        </el-col>
        <el-col :span="10" class="text-right">
          <el-form :inline="true">
            <el-form-item>
              <el-button v-if="!listando" style="margin-right: -10px" icon="fas fa-undo-alt" type="warning" @click="onListar('formIndividuo')"> Voltar</el-button>
            </el-form-item>
          </el-form>
        </el-col>
      </el-row>

      <el-row v-show="listando">
        <el-col :span="24">
          <FiltroRelatorioAtendimento :loading="loading.atendimentos" :params="params" @emit="onEmitFromFiltro" />
        </el-col>
      </el-row>

      

    </el-card>

  </el-col>
</template>

<script>
  import axios from 'axios'
  import Utils from '../../mixins/Utils'
  import _api from '../../api'
  import FiltroRelatorioAtendimento from '../../components/shared/FiltroRelatorioAtendimento'
  export default {
    name: 'RelatorioAtendimento',
    mixins: [Utils],
    components: { FiltroRelatorioAtendimento },
    data () {
      return {
        isDisabled: false,
        isValid: true,
        metodo: 'POST',
        listando: true,
        erros: [],
        formIndividuo: {},
        api: {
          atendimentos: [],
          ufs: [],
          cidades: [],
          usuarios: []
        },
        loading: {
          atendimento: false,
          ufs: false,
          cidades: false,
          mensagens: false,
          usuarios: false,
          relatorio: false
        },
        //params: {
        //  page: 1,
        //  pageSize: 10,
        //  sort: 'CorStatus DESC',
        //  total: 0,
        //  ufs: {
        //    skip: 1,
        //    take: 30,
        //    sort: '+UfAbreviado',
        //    total: 0
        //  },
        //  cidades: {
        //    skip: 1,
        //    take: 999,
        //    sort: '+Nome',
        //    total: 0,
        //    ufAbreviado: null
        //  }
        //}
        params: {
          page: 1,
          pageSize: 10,
          sort: 'DataCadastro DESC',
          total: 0,
        },
      }
    },
    async mounted() {
    },
    methods: {
      onEmitFromFiltro (val) {
        this.params = {
          ...val.params,
          page: 1
        }
        this.listando = true
/*        this.getAtendimentos()*/
      },

      
      //async getRelatorioAtendimentoDia () {
      //  this.loading.atendimentos = true
      //  let { data, paginacao, status } = await _api.relatorios.listaRelatorioAtendimentoDia(this.params)
      //  console.log("data", data)
      //  console.log("status", status)
      //  //if (data.length === 0) {
      //this.$swal({
      //  title: "Atenção!",
      //  text: 'Nenhum registro encontrado!',
      //  icon: 'warning',
      //})
      //}
      //  //this.api.atendimentos = (status === 200) ? data : []
      //  //this.params.page = (status === 200) ? paginacao.currentPage : 0
      //  //this.params.total = (status === 200) ? paginacao.totalCount : 0
      //  //this.loading.atendimentos = false
      //},
      //onListar (form) {
      //  let i = this.api.individuos.findIndex(x => x.id === this.formIndividuo.id)
      //  this.$refs.tabela.setCurrentRow(this.api.individuos[i])
      //  this.$refs[form].resetFields()
      //  this.listando = true
      //},
      //onImprimirAcompanhamento (row) {
      //  axios({
      //    method: 'GET',
      //    url: `/api/AtendTeleMedicina/ExcelRelatorioAcompanhamento/${row.id}`,
      //    responseType: 'blob'
      //  })
      //    .then((res) => {
      //      if (res.status === 200) {
      //        let blob = window.URL.createObjectURL(new Blob([res.data], { type: 'application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet' }))
      //        let link = document.createElement('a')
      //        link.href = blob
      //        link.setAttribute(`download`, `RelatorioAtendTeleMedicinaAcompanhamento ${row.nomeCompleto}.xlsx`)
      //        document.body.appendChild(link)
      //        link.click()
      //      } else {
      //       // console.log('status diferente de 200')
      //      }
      //    })
      //    .catch((e) => {
      //      console.log(e.response)
      //      this.erroMsg = 'Erro, tenta efetuar a operação novamente ou entre em contato com o suporte'
      //      this.carregando = false
      //    })
      //},
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

  .card label {
    text-align: left
  }

</style>
