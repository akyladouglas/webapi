<template>

  <el-form :model="filtroParams" ref="filtroParams" :rules="filtroValidacoes"
           label-width="120px" label-position="top" class="forms--filtro">
    <div class="card">
      <div class="card-header">
        <el-row :gutter="20">

          <el-col :xs="24" :sm="24" :md="12" :lg="4" :xl="4">
            <el-form-item label="Data Inicial" prop="dataInicial">
              <el-date-picker v-model="params.dataInicial"
                              type="date"
                              placeholder="Data Inicial"
                              format="dd/MM/yyyy">
              </el-date-picker>
            </el-form-item>
          </el-col>

          <el-col :xs="24" :sm="24" :md="12" :lg="4" :xl="4">
            <el-form-item label="Data Final" prop="dataFinal">
              <el-date-picker v-model="params.dataFinal"
                              type="date"
                              format="dd/MM/yyyy"
                              placeholder="Data Final">
              </el-date-picker>
            </el-form-item>
          </el-col>

          <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="5">
            <el-form-item label="Profissional" prop="profissionalId" v-if="!mxHasAccess('Médico')">
              <el-select filterable placeholder="Todos" v-model="params.profissionalId"
                         v-loading.body="loading.profissionais" clearable>
                <el-option v-for="option in api.profissionais" :value="option.id" :label="option.nome" :key="option.id" />
              </el-select>
            </el-form-item>
          </el-col>

          <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="5">
            <el-form-item label="Paciente" prop="individuoId" v-if="!mxHasAccess('Médico')">
              <el-select filterable placeholder="Todos" v-model="params.individuoId"
                         v-loading.body="loading.individuos" clearable>
                <el-option v-for="option in api.individuos" :value="option.id" :label="option.nomeCompleto" :key="option.id" />
              </el-select>
            </el-form-item>
          </el-col>

        </el-row>

      </div>
    </div>
    <el-row :gutter="20">
      <el-col :span="24" class="text-right">
        <el-form-item class="forms--margin-xs-from-top">
          <el-button v-if="!filtrando" type="info" icon="fas fa-filter" @click="getAtendimentos('params')"> Filtrar</el-button>
          <el-button v-if="filtrando" type="info" icon="fa fa-spinner fa-spin" disabled> Aguarde ...</el-button>
          <el-button v-if="!loading.relatorio" type="success" icon="fas fa-file-excel" @click="onImprimirRelatorio"> Gerar Relalório Excel</el-button>
          <el-button v-if="loading.relatorio" type="info" icon="fa fa-spinner fa-spin" disabled> Aguarde ...</el-button>
        </el-form-item>
      </el-col>
    </el-row>

    <el-row v-if="api.atendimentos.length === 0">
      <div style="height:100%; width:100%">
        <el-empty description="Nenhum dado encontrado"></el-empty>
      </div>
    </el-row>
    
    <el-row v-if="listando && api.atendimentos.length > 0">
      <el-col :span="24">
        <el-table ref="tabela" :data="api.atendimentos"
                  v-loading.body="loading.atendimentos"
                  highlight-current-row border>
          <el-table-column label="Data do Atendimento">
            <template slot-scope="scope">
              {{ moment(scope.row.dataCadastro).format("DD/MM/YYYY") }}
            </template>
          </el-table-column>
          <el-table-column label="Atendido por ">
            <template slot-scope="scope">
              {{ scope.row.atendidoMedico === true ? scope.row.profissional.nome : "TRIAGEM"}}
            </template>
          </el-table-column>
          <el-table-column label="Paciente" prop="individuo.nomeCompleto" />
          <el-table-column label="Contato do Paciente" prop="individuo.telefoneCelular" />
          <el-table-column label="Email do Paciente" prop="individuo.email" />

        </el-table>
      </el-col>
      <el-col :span="24" v-show="listando">
        <el-pagination @size-change="handleSizeChange"
                       @current-change="handleCurrentChange"
                       :current-page.sync="params.page"
                       :page-sizes="[10]"
                       :page-size="params.pageSize"
                       :total="params.total"
                       layout="total, sizes, prev, pager, next, jumper">
        </el-pagination>
      </el-col>
    </el-row>

    <el-row :gutter="20" v-show="listando && api.atendimentos.length > 0">
      <el-col :xs="24" :sm="24" :md="12" :lg="10" :xl="10" class="forms--margin-xs-from-top">
        <!--<el-button v-if="!loading.relatorio" type="info" icon="fas fa-print" @click="onImprimirRelatorio"> Imprimir Relalório</el-button>
        <el-button v-if="loading.relatorio" type="info" icon="fa fa-spinner fa-spin" disabled> Aguarde ...</el-button>-->
      </el-col>
    </el-row>

  </el-form>
</template>

<script>
  import Utils from '../../mixins/Utils'
  import _enums from '../../api/Enums'
  import _api from '../../api'

  export default {
    mixins: [Utils],
    name: 'FiltroRelatorioAtendimento',
   // props: ['loading', 'params'],
    data () {
      return {
        filtroValidacoes: {},
        filtroParams: {},
        listando: true,
        filtrando: false,

        api: {
          atendimentos: [],
          profissionais: [],
          individuos: [],
        },

        loading: {
          atendimento: false,
          relatorio: false,
          individuos: false,
          profissionais: false,
        },

        params: {
          skip: 1,
          take: 10,
          sort: "DataCadastro DESC",
          atendidoMedico: true,
          total: 0
        },

        paramsProfissionais: {
          skip: 1,
          take: 999999,
          sort: 'Nome ASC',
          total: 0
        },
        paramsIndividuos: {
          skip: 1,
          take: 10,
          sort: 'i.NomeCompleto ASC',
          total: 0
        },
      }
    },
    async mounted() {
      if (this.mxHasAccess('Médico')) {
        this.params.profissionalId = this.$auth.user().id
        await this.getAtendimentos()
      }
      await this.getAtendimentos()
      await this.getProfissionais()
      await this.getIndividuos()
    },
    methods: {
      async getIndividuos() {
        this.loading.individuos = true
        let { data, status } = await _api.individuos.getAll(this.paramsIndividuos)
        if (status === 502) this.loading.individuos = false
        this.api.individuos = (status === 200) ? data : []
        this.loading.individuos = false
      },

      async getAtendimentos() {
        this.loading.atendimento = true
        this.filtrando = true
        let { data, paginacao, status } = await _api.atendimentos.get(this.params)
        if (status === 502) this.loading.atendimento = false
        this.api.atendimentos = (status === 200) ? data : []
        this.params.skip = (status === 200) ? paginacao.currentPage : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.atendimento = false
        this.filtrando = false
      },

      async getProfissionais() {
        this.loading.profissionais = true
        let { data, paginacao, status } = await _api.profissionais.get(this.paramsProfissionais)
        if (status === 502) this.loading.profissionais = false
        this.api.profissionais = (status === 200) ? data : []
        this.paramsProfissionais.skip = (status === 200) ? paginacao.skip : 0
        this.paramsProfissionais.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.profissionais = false
      },

      //metodo de imprimir relatorio
      async onImprimirRelatorio() {
        this.loading.relatorio = true
        await _api.relatorios.excelRelatorioAtendimento(this.params)
        this.loading.relatorio = false
      },

      async onClickFiltrar(form) {
        this.$refs[form].validate((valid) => {
          if (valid) {
            this.$emit('emit', { params: this.filtroParams })
          } else {
            this.$swal({
              title: "Atenção!",
              text: "Verifique o preenchimento dos filtros!",
              icon: 'warning',
            })
          }
        })
      },


      handleSizeChange(val) {
        this.params.pageSize = val
      },

      handleCurrentChange(val) {
        this.params.skip = val
        this.getAtendimentos()
      },


    }
  }
</script>
