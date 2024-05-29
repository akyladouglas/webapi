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
      <el-col :xs="24" :sm="24" :md="8" :lg="8" :xl="8">

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
  import _enums from '../../api/Enums'

export default {
    name: 'FiltroEstabelecimento',
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
        }
      }
    },
    async created () {
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
      }
    }
  }
</script>
