<template>

  <el-form :model="params" ref="params" :rules="filtroValidacoes" label-width="120px" label-position="top" class="forms--filtro-estabelecimento">
    <div class="card">
    <div class="card-header">
      <el-row :gutter="24">

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

      </el-row>
    </div>
    </div>
    <el-row :gutter="20">
      <el-col :span="24" class="text-right">
        <el-form-item class="forms--margin-xs-from-top">
          <el-button v-if="!loading.filtro" type="info" icon="fas fa-filter"  @click="onClickFiltrar('params')"> Filtrar</el-button>
          <el-button v-if="loading.filtro"  type="info" icon="fa fa-spinner fa-spin" disabled> Aguarde ...</el-button>
        </el-form-item>
      </el-col>
    </el-row>
  </el-form>

</template>

<script>
  import Utils from '../../mixins/Utils'
  import _enums from '../../api/Enums'
  import _api from '../../api'
  import moment from 'moment'
  import { mask } from 'vue-the-mask'

  moment.locale('pt-br')

  import Hub from '../../Hub'

  /*var _hub = new Hub()*/
  export default {
    name: 'FiltroMonitoramento',
    /*props: ['loading', 'params'],*/
    directives: { mask },

    mixins: [Utils],
    data () {
      return {

        params: {
          dataInicial: null,
          dataFinal: null
        },
        loading: {
          filtro: false,
        },

        filtroValidacoes: {},

      }
    },

    methods: {
      async onClickFiltrar(form) {
        this.loading.filtro = true
        this.$refs[form].validate((valid) => {
          if (valid) {
            if (this.params.dataInicial) {
              this.params.dataInicial = moment(this.params.dataInicial).format('YYYY-MM-DD HH:mm:ss')
            }
            if (this.params.dataFinal) {
              this.params.dataFinal = moment(this.params.dataFinal).format('YYYY-MM-DD HH:mm:ss')
            }
            this.$emit('emit', { params: this.params })
          } else {
            this.$swal({
              title: "Atenção!",
              text: "Verifique o preenchimento dos filtros!",
              icon: 'warning',
            })
          }
        })
        this.loading.filtro = false
      },

    }
  }
</script>
