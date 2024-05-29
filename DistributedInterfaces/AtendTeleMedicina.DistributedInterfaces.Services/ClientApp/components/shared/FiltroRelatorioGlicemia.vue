<template>

  <el-form :model="filtroParams" ref="formFiltro" :rules="validacoes"
           label-width="120px" label-position="top" class="forms--filtro">
    <div class="card">
      <div class="card-header">
        <el-row :gutter="24">

          <el-col :xs="24" :sm="24" :md="12" :lg="4" :xl="4">
            <el-form-item label="Data Inicial" prop="dataInicial">
              <el-date-picker v-model="filtroParams.dataInicial"
                              type="date"
                              placeholder="Data Inicial"
                              format="dd/MM/yyyy">
              </el-date-picker>
            </el-form-item>
          </el-col>

          <el-col :xs="24" :sm="24" :md="12" :lg="4" :xl="4">
            <el-form-item label="Data Final" prop="dataFinal">
              <el-date-picker v-model="filtroParams.dataFinal"
                              type="date"
                              format="dd/MM/yyyy"
                              placeholder="Data Final">
              </el-date-picker>
            </el-form-item>
          </el-col>

          <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="5">
            <el-form-item label="Paciente" prop="individuoId">
              <el-select filterable placeholder="Todos" v-model="filtroParams.individuoId"
                         v-loading.body="loading.individuos" clearable>
                <el-option v-for="option in api.individuos" :value="option.id" :label="option.nomeCompleto" :key="option.id" />
              </el-select>
            </el-form-item>
          </el-col>

        </el-row>

      </div>
    </div>

    <el-row :gutter="24">
      <el-col :span="24" class="text-right">
        <el-form-item class="forms--margin-xs-from-top">
          <el-button v-if="!loading.filtrando" type="info" icon="fas fa-filter" @click="onClickFiltrar('formFiltro')"> Filtrar</el-button>
          <el-button v-if="loading.filtrando" type="info" icon="fa fa-spinner fa-spin" disabled> Aguarde ...</el-button>
          <el-button v-if="!loading.relatorio" type="success" icon="fas fa-file-excel" @click="onImprimirRelatorio"> Gerar Relalório Excel</el-button>
          <el-button v-if="loading.relatorio" type="info" icon="fa fa-spinner fa-spin" disabled> Aguarde ...</el-button>
        </el-form-item>
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
    name: 'FiltroRelatorioGlicemias',
    props: {
      gerarRelatorioParams: {},
    },
    data () {
      return {
        filtroParams: {},
        validacoes: {},
        
        api: {
          individuos: [],
        },

        loading: {
          filtrando: false,
          relatorio: false,
          individuos: false,
        },

        paramsIndividuos: {
          skip: 1,
          take: 100000,
          sort: 'i.NomeCompleto ASC',
          total: 0
        },
      }
    },
    async mounted() {
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

      //metodo de imprimir relatorio
      async onImprimirRelatorio() {
        this.loading.relatorio = true
        await _api.relatorios.excelRelatorioGlicemia(this.gerarRelatorioParams)
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
    }
  }
</script>
