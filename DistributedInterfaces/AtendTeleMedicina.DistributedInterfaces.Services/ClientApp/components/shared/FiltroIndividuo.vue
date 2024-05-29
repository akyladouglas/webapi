<template>

  <el-form :model="filtroParams" ref="filtroParams" label-width="120px" label-position="top" class="forms--filtro-individuo">
    <div class="card">
    <div class="card-header">
      <el-row :gutter="20">
        <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
          <el-form-item label="Nome" prop="nomeCompleto">
            <el-input v-model="filtroParams.nomeCompleto" maxlength="50"/>
          </el-form-item>
        </el-col>
        <el-col :xs="24" :sm="24" :md="24" :lg="8" :xl="8">
          <el-form-item label="CPF" prop="cpf">
            <el-input show-word-limit v-model="filtroParams.cpf" masked="true" maxlength="14" v-mask="'###.###.###-##'" />
          </el-form-item>
        </el-col>
        <el-col :xs="24" :sm="24" :md="24" :lg="8" :xl="8">
          <el-form-item label="CNS" prop="cns">
            <el-input v-model="filtroParams.cns" maxlength="15" show-word-limit />
          </el-form-item>
        </el-col>
        <el-col :xs="24" :sm="24" :md="24" :lg="8" :xl="8">
          <el-form-item label="Email" prop="email">
            <el-input v-model="filtroParams.email" maxlength="30"/>
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
    name: 'FiltroIndividuo',
    props: ['loading', 'params'],
    mixins: [Utils],
    directives: { mask },
    data () {
      return {
        filtroParams: {},
        filtroValidacoes: {}
      }
    },
    async created () {
    },
    methods: {
      async onClickFiltrar (form) {
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
      }
    }
  }
</script>
