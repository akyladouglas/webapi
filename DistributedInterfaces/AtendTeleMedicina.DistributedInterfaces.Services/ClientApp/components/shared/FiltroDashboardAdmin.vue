<template>

  <el-form :model="filtroParams" ref="filtroParams" :rules="filtroValidacoes" label-width="120px" label-position="top" class="forms--filtro-estabelecimento">
    <div class="card">
      <div class="card-header">
        <el-row :gutter="20">

          <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="5">
            <el-form-item label="Estabelecimento" prop="estabelecimentoId">
              <el-select filterable placeholder="Todos" v-model="filtroParams.estabelecimentoId"
                         clearable>
                <el-option v-for="option in api.estabelecimentos" :value="option.id" :label="option.nomeFantasia" :key="option.id" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="5">
            <el-form-item label="Profissional" prop="profissionalId">
              <el-select filterable placeholder="Todos" v-model="filtroParams.profissionalId"
                         clearable>
                <el-option v-for="option in api.profissionais" :value="option.id" :label="option.nome" :key="option.id" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="5">
            <el-form-item label="Tipo Da Consulta" prop="tipoDaConsulta">
              <el-select filterable placeholder="Selecione o Tipo Da Consulta" v-model="filtroParams.tipoDaConsulta"
                         v-loading.body="loading.tipoDaConsulta">
                <el-option v-for="item in enums.tipoDaConsultaTelaHome" :value="item.value" :label="item.label" :key="item.value" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="24" :md="12" :lg="4" :xl="4">
            <el-form-item label="Data Inicial:" prop="dataInicial">
              <input class="form-control" type="date" value="" v-model="filtroParams.dataInicial"  />
            </el-form-item>
          </el-col>

          <el-col :xs="24" :sm="24" :md="12" :lg="4" :xl="4">
            <el-form-item label="Data Final:" prop="dataFinal">
              <input class="form-control" type="date" value="" v-model="filtroParams.dataFinal"  />
            </el-form-item>
          </el-col>

        </el-row>
      </div>
    </div>
    <el-row :gutter="20">
      <el-col :span="24" class="text-right">
        <el-form-item class="forms--margin-xs-from-top">
          <el-button v-if="!loading" type="info" icon="fas fa-filter" @click="onClickFiltrar('filtroParams')"> Filtrar</el-button>
          <el-button v-if="loading" type="info" icon="fa fa-spinner fa-spin" disabled> Aguarde ...</el-button>
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
    name: 'FiltroDashboardAdmin',
    props: ['loading'],
    mixins: [Utils],
    directives: { mask },
    data() {
      
      
      return {
        filtroValidacoes: {
          
        },
        filtroParams: {},
        enums: {
          tipoDaConsultaTelaHome: _enums.tipoDaConsultaTelaHome,
        },
        api: {
          estabelecimentos: [],
          profissionais: [],
        },

        params: {
          skip: 1,
          take: 100000,
          total: 0
        },
      }
    },
    async created () {
      await this.getEstabelecimentos()
      await this.getProfissionais()
    },
    methods: {
      async onClickFiltrar (form) {
        this.$refs[form].validate((valid) => {
          if (valid) {
         //   console.log('FORM', this.filtroParams)
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
  
      async getEstabelecimentos() {

        let { data, status } = await _api.estabelecimentos.get(this.params)
        this.api.estabelecimentos = (status === 200) ? data : []
        //console.log(this.api.estabelecimentos)

      },

      async getProfissionais() {
        let { data, status } = await _api.profissionais.get(this.params)
        this.api.profissionais = (status === 200) ? data : []
      },
    }
  }
</script>
