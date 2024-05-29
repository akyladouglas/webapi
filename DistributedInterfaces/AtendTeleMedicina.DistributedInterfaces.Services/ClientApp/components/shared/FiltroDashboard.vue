<template>

  <el-form :model="params" ref="params" :rules="filtroValidacoes" label-width="120px" label-position="top" class="forms--filtro-estabelecimento">
    <div class="card">
    <div class="card-header">
      <el-row :gutter="20">

        <el-col :xs="24" :sm="24" :md="24" :lg="7" :xl="4">
          <el-form-item label="Data Inicial:" prop="dataInicial">
            <input class="form-control" type="date" value="" v-model="params.dataInicial" style="width: 300px; height:40px" />
          </el-form-item>
        </el-col>

        <el-col :xs="24" :sm="24" :md="24" :lg="6" :xl="10">
          <el-form-item label="Data Final:" prop="dataFinal">
            <input class="form-control" type="date" value="" v-model="params.dataFinal" style="width: 300px; height:40px" />
          </el-form-item>
        </el-col>

      </el-row>
    </div>
    </div>
    <el-row :gutter="20">
      <el-col :span="24" class="text-right">
        <el-form-item class="forms--margin-xs-from-top">
          <el-button v-if="!loading" type="info" icon="fas fa-filter"  @click="onClickFiltrar('params')"> Filtrar</el-button>
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
    name: 'FiltroDashboard',
    props: ['loading', 'params'],
    mixins: [Utils],
    directives: { mask },
    data() {
      
      return {
        filtroValidacoes: {
         
        },
        params: {},
        enums: {

        },
        api: {
         
        },
        carregando: {

        },

      }
    },
    async created () {

    },
    methods: {
      async onClickFiltrar (form) {
        this.$refs[form].validate((valid) => {
          if (valid) {
            this.$emit('emit', { params: this.params })
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
