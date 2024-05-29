<template>

  <el-form :model="filtroParams" ref="filtroParams" :rules="filtroValidacoes" label-width="120px" label-position="top" class="forms--filtro-estabelecimento">
    <div class="card">
    <div class="card-header">
      <el-row :gutter="20">
        <el-col :xs="24" :sm="24" :md="12" :lg="4" :xl="4">
          <el-form-item label="Data" prop="dataCadastro">
            <el-date-picker prefix-icon="fas fa-calendar-day" v-model="filtroParams.dataCadastro" type="date" format="dd-MM-yyyy" />
          </el-form-item>
        </el-col>
        <el-col :xs="24" :sm="24" :md="12" :lg="8" :xl="8">
          <el-form-item label="Paciente" prop="individuoId">
            <el-select filterable clearable v-model="filtroParams.individuoId" placeholder="Todos ...">
              <el-option v-for="option in api.individuos" :label="option.nomeCompleto" :value="option.id" :key="option.id" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :xs="24" :sm="24" :md="12" :lg="3" :xl="3">
          <el-form-item label="Ativo" prop="ativo">
            <el-select v-model="filtroParams.ativo" placeholder="Todos ...">
              <el-option v-for="item in enums.simNaoTodos" :label="item.label" :value="item.value" :key="item.value" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
    </div>
    </div>
    <el-row :gutter="20">
      <el-col :span="24" class="text-right">
        <el-form-item class="forms--margin-xs-from-top">
          <el-button v-if="!loading" type="info" icon="fas fa-filter"  @click="onClickFiltrar('filtroParams')"> Filtrar</el-button>
          <el-button v-if="loading"  type="info" icon="fa fa-spinner fa-spin" disabled> Aguarde ...</el-button>
        </el-form-item>
      </el-col>
    </el-row>
  </el-form>

</template>

<script>
  import Utils from '../../mixins/Utils'
  import { mask } from 'vue-the-mask'
  import _api from '../../api'
  import _enums from '../../api/Enums'

export default {
    name: 'FiltroHistorico',
    props: ['loading', 'params'],
    mixins: [Utils],
    directives: { mask },
    data() {
      
      return {
        filtroValidacoes: {

        },
        filtroParams: {},

        api: {
          individuos: [],
          profissionais: []
        },
        enums: {
          simNaoTodos: [
            { label: 'Todos', value: null },
            ..._enums.simNao
          ]
        },
        carregando: {
          ufs: false,
          cidades: false
        },
        paramsProfissional: {
          skip: 1,
          take: 999,
          ativo: true,
          total: 0
        },
        paramsFiltro: {
          skip: 1,
          take: 10,
          sort: 'NomeCompleto ASC',
          total: 0
        },
      }
    },
    async created () {
      await this.getIndividuos()
      await this.getProfissionais()
    },
    methods: {
      async getProfissionais() {
        let { data, status } = await _api.profissionais.get(this.paramsProfissional)
        this.api.profissionais = (status === 200) ? data : []
      },

      async getIndividuos() {
        this.paramsFiltro.take = 100000
        let { data, paginacao, status } = await _api.individuos.getAll(this.paramsFiltro)
        /*console.log("data", data)*/
        this.api.individuos = (status === 200) ? data : []
        
      },

      async onClickFiltrar (form) {
        this.$refs[form].validate((valid) => {
          if (valid) {
            /*console.log(this.filtroParams)*/
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
