<template>

  <el-form :model="filtroParams" ref="filtroParams" :rules="filtroValidacoes"
           label-width="120px" label-position="top" class="forms--filtro">
    <div class="card">
      <div class="card-header">
        <el-row :gutter="20">

          <!--<el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="5">
    <el-form-item label="Data" prop="dataCadastro">
      <el-date-picker prefix-icon="fas fa-calendar-day" v-model="params.dataCadastro" type="date" format="dd-MM-yyyy" />
    </el-form-item>
  </el-col>-->





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
            <el-form-item label="Paciente" prop="individuoId">
              <el-select filterable placeholder="Todos" v-model="params.individuoId"
                         v-loading.body="loading.individuo" clearable>
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


    <el-row v-show="listando && api.atendimentos.length > 0">
      <el-col :span="24">
        <el-table ref="tabela" :data="api.atendimentos"
                  v-loading.body="loading.atendimentos"
                  highlight-current-row border>
          <el-table-column label="Data do Atendimento">
            <template slot-scope="scope">
              {{ moment(scope.row.dataCadastro).format("DD/MM/YYYY") }}
            </template>
          </el-table-column>
          <!--<el-table-column label="Atendido por ">
            <template slot-scope="scope">
              {{ scope.row.atendidoMedico === true ? scope.row.profissional.nome : "TRIAGEM"}}
            </template>
          </el-table-column>-->
          <el-table-column label="Paciente" prop="individuo.nomeCompleto" />
          <el-table-column label="Data de Nascimento" prop="individuo.dataNascimento">
            <template slot-scope="scope">
              {{moment(scope.row.individuo.dataNascimento).format("DD/MM/YYYY")}}
            </template>
          </el-table-column>
          <el-table-column label="Idade" prop="individuo.idade" width="80">
          </el-table-column>
          <el-table-column label="Contato do Paciente" prop="individuo.telefoneCelular"/>
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


  </el-form>
</template>

<script>
  import { mask } from 'vue-the-mask'
  import Utils from '../../mixins/Utils'
  import _api from '../../api'

  export default {
    mixins: [Utils],
    name: 'FiltroRelatorioAtendimentoPaciente',
    //props: ['loading', 'params'],
    data () {
      return {
        filtroValidacoes: {},
        filtroParams: {},
        listando: true,
        filtrando: false,
        api: {
          atendimentos: []
        },

        loading: {
          atendimento: false,
          relatorio: false,
          individuo: false,
        },

        params: {
          skip: 1,
          take: 10,
          sort: "DataCadastro DESC",
          atendidoMedico: true,
          atendidoTriagem: false,
          total: 0
        },

        paramsIndividuo: {
          skip: 1,
          take: 999999999,
          sort: 'i.NomeCompleto ASC',
          total: 0
        },

      }
    },
    async mounted () {
      await this.getAtendimentos()
      await this.getIndividuos()
    },
    methods: {

      async getAtendimentos() {
        this.loading.atendimento = true
        console.log(this.params)
        let { data, paginacao, status } = await _api.atendimentos.get(this.params)
        if (status === 502) this.loading.atendimento = false
        this.api.atendimentos = (status === 200) ? data : []
        this.params.skip = (status === 200) ? paginacao.currentPage : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.atendimento = false
      },

      async getIndividuos() {
        this.loading.individuo = true
        let { data, status } = await _api.individuos.getAll(this.paramsIndividuo)
        if (status === 502) this.loading.individuos = false
        this.api.individuos = (status === 200) ? data : []
        this.loading.individuo = false
      },


      //metodo de imprimir relatorio
      async onImprimirRelatorio() {
        this.loading.relatorio = true
        await _api.relatorios.excelRelatorioAtendimentoPaciente(this.params)
        this.loading.relatorio = false
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
