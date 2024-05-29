<template>

  <el-form :model="filtroParams" ref="filtroParams" :rules="filtroValidacoes" label-width="120px" label-position="top" class="forms--filtro-estabelecimento">
    <div class="card">
    <div class="card-header">
      <el-row :gutter="20">
        <!--<el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12">
    <el-form-item label="Tipo da Consulta" prop="tipoDaConsulta">
      <el-select v-model="filtroParams.tipoDaConsulta" placeholder="Todos...">
        <el-option v-for="option in enums.tipoDaConsulta" :value="option.value" :label="option.label" :key="option.value" />
      </el-select>
    </el-form-item>
  </el-col>-->
        <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12">
          <el-form-item label="Data" prop="data">
            <el-input v-model="filtroParams.dia"
                      v-mask="'##/##/####'"
      masked="true"
             />
          </el-form-item>
        </el-col>

        <!--<el-date-picker v-model="filtroParams.dia"
                        type="date"
                        format="dd/MM/yyyy"
                        value-format="dd/MM/yyyy"
                        placeholder="Escolha uma data">
        </el-date-picker>-->

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
  import moment from 'moment'
  moment.locale('pt-br');


export default {
    name: 'FiltroAgenda',
    props: ['loading', 'params'],
    mixins: [Utils],
    directives: { mask },
    data () {
      return {
        filtroParams: {},
        filtroValidacoes: {},
      }
    },
    async created() {
    },
    methods: {
      async onClickFiltrar (form) {
        this.$refs[form].validate((valid) => {
          if (valid) {
            let dataNoFormat = this.filtroParams.dia;
            let formattedData = moment(dataNoFormat, 'DD/MM/YYYY').format('YYYY-MM-DD');
            //console.log("DATA", formattedData);

            this.filtroParams.dia = formattedData


            this.$emit('emit', { params: this.filtroParams })

            this.filtroParams.dia = formattedData

            //console.log("FILTRO PARAMS AFTER FILTER", this.filtroParams.dia)
            let returnData = moment(this.filtroParams.dia, 'YYYY-MM-DD').format('DD/MM/YYYY');
            //console.log("RETURN DATA", returnData)
            this.filtroParams.dia = returnData
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
