<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">

    <el-card class="box-card--main-card">

      <el-row>
        <el-col :span="14">
          <h2 class="box-card--h2">Download das Prescrições Médicas</h2>

        </el-col>
      </el-row>

      <el-row>
        <el-col :span="24">
          <FiltroDownload :loading="loading.documentos" :params="params" @emit="onEmitFromFiltro" />
        </el-col>
      </el-row>

      <el-row v-show="api.documentos.length > 0">
        <el-col :span="24">
          <el-table ref="tabela" :data="api.documentos"
                    highlight-current-row border
                    v-loading.body="loading.documentos"
                    class="table--profissionais table--row-click">
            <el-table-column label="Nome do Paciente" prop="individuoId">
              <template slot-scope="scope">
                {{ getIndividuoNome(scope.row.individuoId) }}
              </template>
            </el-table-column>
            <el-table-column label="Data" prop="dataCadastro">
              <template slot-scope="scope">
                {{ moment(scope.row.dataCadastro).format('DD/MM/YYYY hh:mm') }}
              </template>
            </el-table-column>
            <el-table-column label="Profissional" prop="profissionalId" width="180">
              <template slot-scope="scope">
                {{ getProfissionalNome(scope.row.profissionalId) }}
              </template>
            </el-table-column>
            <el-table-column label="" width="150">
              <template slot-scope="scope">
                <el-button @click="onDownload(scope.row.url)" type="primary" size="small">
                  Download
                </el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-col>

        <el-col :span="24" v-show="listando">
          <el-pagination @size-change="handleSizeChange"
                         @current-change="handleCurrentChange"
                         :current-page.sync="paramsDocs.page"
                         :page-sizes="[10,25,50,100]"
                         :page-size="paramsDocs.pageSize"
                         :total="paramsDocs.total"
                         layout="total, sizes, prev, pager, next, jumper">
          </el-pagination>
        </el-col>
      </el-row>
    </el-card>
  </el-col>
</template>


<script>
  
  import VuePerfectScrollbar from 'vue-perfect-scrollbar'
  import { Notification } from 'element-ui'
  import Utils from '../../mixins/Utils'
  import jQuery, { data } from 'jquery'
  import _api from '../../api'
  import _enums from '../../api/Enums'
  import FiltroDownload from '../../components/shared/FiltroDownload'
  import { mask } from 'vue-the-mask'
  import moment from 'moment'
  moment.locale('pt-br')
  export default {
    name: 'DownloadDocs',
    mixins: [Utils],
    components: { FiltroDownload, VuePerfectScrollbar },
    directives: { mask },
    data() {
      return {

        isDisabled: false,
        metodo: 'POST',
        listando: true,
        erros: [],
        formIndividuo: {},
        enums: {

        },
        api: {
          documentos: [],
          individuos: [],
          profissionais: [],
          agendamentos: [],
          ufs: [],
          cidades: [],
        },
        validacoes: {

        },
        loading: {
          documentos: false,
          individuos: false,
          cidades: false,
          ufs: false,
        },

        paramsDocs: {
          dataCadastro: null,
          skip: 1,
          take: 10,
          sort: 'd.DataCadastro DESC',
          total: 0
        },
        paramsIndividuos: {
          skip: 1,
          take: 999,
          total: 0
        },
        paramsProfissional: {
          skip: 1,
          take: 999,
          total: 0
        },

        params: {
          skip: 1,
          take: 999,
          total: 0
        },
        paramsCidades: {
          skip: 1,
          take: 999,
          sort: '+Nome',
          total: 0,
          ufAbreviado: null
        }
      }
    },

    async mounted() {
      await this.getDocumentos()
      await this.getIndividuos()
      await this.getProfissionais()
    },
    methods: {

      async onDownload(val) {
        window.open(val);
      },
      async getDocumentos() {
        this.loading.documentos = true
        if (this.paramsDocs.dataCadastro == 'Invalid date') {
          this.paramsDocs.dataCadastro = null
        }
        let { data, paginacao, status } = await _api.documentos.get(this.paramsDocs)
        if (status === 502) this.loading.documentos = false
        this.api.documentos = (status === 200) ? data : []
        this.paramsDocs.skip = (status === 200) ? paginacao.skip : 0
        this.paramsDocs.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.documentos = false
      },



      onEmitFromFiltro (val) {
        this.paramsDocs = {
          ...this.paramsDocs,
          ...val.params,
          page: 1
        }
        this.listando = true
        this.getDocumentos()
      },

      getIndividuoNome(val) {
        var individuo = this.api.individuos.filter(individuo => individuo.id == val)
        return individuo[0] ? individuo[0].nomeCompleto : null;
        
      },
      getProfissionalNome(val) {
        var profissional = this.api.profissionais.filter(profissional => profissional.id == val)
        return profissional[0] ? profissional[0].nome : null;
      },

      async getIndividuos() {
        let { data, paginacao, status } = await _api.individuos.getAll(this.paramsIndividuos)
        this.api.individuos = (status === 200) ? data : []
        this.paramsIndividuos.skip = (status === 200) ? paginacao.skip : 0
        this.paramsIndividuos.total = (status === 200) ? paginacao.totalCount : 0
      },

      async getProfissionais() {
        let { data, paginacao, status } = await _api.profissionais.get(this.paramsProfissional)
        this.api.profissionais = (status === 200) ? data : []
        this.paramsProfissional.skip = (status === 200) ? paginacao.skip : 0
        this.paramsProfissional.total = (status === 200) ? paginacao.totalCount : 0
      },

      handleSizeChange (val) {
        this.paramsDocs.take = val
        this.getDocumentos()
      },
      handleCurrentChange (val) {
        this.paramsDocs.skip = val
        this.getDocumentos()
      },
    }
  }
</script>

<style scoped>
</style>
