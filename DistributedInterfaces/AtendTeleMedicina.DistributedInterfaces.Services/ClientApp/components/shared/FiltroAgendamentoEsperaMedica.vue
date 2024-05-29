<template>

  <el-form :model="filtroParams" ref="filtroParams" :rules="filtroValidacoes" label-width="120px" label-position="top" class="forms--filtro-estabelecimento">
    <div class="card">
    <div class="card-header">
    <el-row :gutter="20">
      <el-col :xs="24" :sm="24" :md="8" :lg="8" :xl="8">

        <!--<el-form-item label="Tipo da Consulta" prop="tipoDaConsulta">
          <el-select v-model="filtroParams.tipoDaConsulta" placeholder="Todos...">
            <el-option v-for="option in enums.tipoDaConsulta" :value="option.value" :label="option.label" :key="option.value" />
          </el-select>
        </el-form-item>-->

        <el-form-item label="Tipo da Consulta" prop="tipoDaConsulta">
            <el-select v-model="filtroParams.tipoDaConsulta" placeholder="Todos ...">
              <el-option v-for="option in enums.tipoDaConsulta" :label="option.label" :value="option.value" :key="option.value" />
            </el-select>
          </el-form-item>

      </el-col>
      <el-col :xs="24" :sm="24" :md="8" :lg="8" :xl="8">

        <el-form-item label="Data" prop="dia">
          <el-date-picker prefix-icon="fas fa-calendar-day" v-model="filtroParams.dia" type="date" format="dd-MM-yyyy" />
        </el-form-item>


      </el-col>
      <!--<el-col :xs="24" :sm="24" :md="8" :lg="8" :xl="8">
          <el-form-item label="Profissional" prop="profissionalId" v-if="!mxHasAccess('Médico') || !mxHasAccess('MédicoAD')">
            <el-select filterable placeholder="Todos" v-model="params.profissionalId"
                       v-loading.body="loading.profissionais" clearable>
              <el-option v-for="option in api.profissionais" :value="option.id" :label="option.nome" :key="option.id" />
            </el-select>
          </el-form-item>
      </el-col>-->  
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
  import _enums from '../../api/Enums'
  import _api from '../../api'

export default {
    name: 'FiltroAgendamentoEsperaMedica',
    props: ['loading', 'params'],
    mixins: [Utils],
    directives: { mask },
    data () {
      return {
        filtroParams: {
        },
        filtroValidacoes: {},
        enums: {
          tipoDaConsulta: [
            { label: 'Todos', value: null },
            ..._enums.tipoDaConsulta
          ],
          simNao: [
            { label: 'Todos', value: null },
            ..._enums.simNao
          ]
        },
        api: {
          profissionais: [],
        },

        //loading: {
        //  profissionais: false,
        //},

        paramsProfissionais: {
          skip: 1,
          take: 999999,
          sort: 'Nome ASC',
          total: 0
        },

      }
    },

    methods: {

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
      async getProfissionais() {
        //this.loading.profissionais = true
        let { data, paginacao, status } = await _api.profissionais.get(this.paramsProfissionais)
        if (status === 502) this.loading.profissionais = false
        this.api.profissionais = (status === 200) ? data : []
        this.paramsProfissionais.skip = (status === 200) ? paginacao.skip : 0
        this.paramsProfissionais.total = (status === 200) ? paginacao.totalCount : 0
      //  this.loading.profissionais = false
      },

    }
  }
</script>
