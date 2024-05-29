<template>

  <el-form :model="filtroParams" ref="filtroParams" :rules="filtroValidacoes" label-width="120px" label-position="top" class="forms--filtro-estabelecimento">
    <div class="card">
    <div class="card-header">
      <el-row :gutter="20">
        <el-col :xs="24" :sm="24" :md="24" :lg="8" :xl="8">
          <el-form-item label="Paciente" prop="individuoId">
            <el-select filterable label="Selecione" v-model="filtroParams.individuoId"
                           v-loading.body="loading.individuos"  clearable>
                    <el-option v-for="item in api.individuos" :value="item.id" :label="item.nomeCompleto" :key="item.id" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :xs="24" :sm="24" :md="24" :lg="8" :xl="8">
          <el-form-item label="Profissional" prop="profissionalId">
            <el-select filterable label="Selecione" v-model="filtroParams.profissionalId"
                       :loading="loading.individuos" clearable>
              <el-option v-for="item in api.profissionais" :value="item.id" :label="item.nome" :key="item.id" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :xs="24" :sm="24" :md="24" :lg="8" :xl="8">
          <el-form-item label="Data:" prop="dataCadastro">
            <input class="form-control" type="date" value="" v-model="filtroParams.dataCadastro" style="width: 300px; height:40px"/>
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
  import moment from 'moment'

export default {
    name: 'FiltroDownload',
    props: ['loading', 'params'],
    mixins: [Utils],
    directives: { mask },
    data () {
      return {
        filtroParams: {},
        filtroValidacoes: {},
        api: {
          individuos: [],
          profissionais: [],
        },
        pickerOptions: {
          disabledDate: this.getDatasDisponiveis
        },
        paramsProfissional: {
          skip: 1,
          take: 999,
          ativo: true,
          total: 0
        },
        paramsIndividuos: {
          skip: 1,
          take: 999,
          total: 0
        },
      }
    },
    async mounted() {
      await this.getIndividuos()
      await this.getProfissionais()
    },
    methods: {
      getDatasDisponiveis(date) {
        const indisponivel = date.getDay()
        return indisponivel === 6 || indisponivel === 0 
      },

      async getProfissionais() {
        let { data, paginacao, status } = await _api.profissionais.get(this.paramsProfissional)
        this.api.profissionais = (status === 200) ? data : []
        this.paramsProfissional.skip = (status === 200) ? paginacao.skip : 0
        this.paramsProfissional.total = (status === 200) ? paginacao.totalCount : 0
      },

      async getIndividuos() {
        let { data, paginacao, status } = await _api.individuos.getAll(this.paramsIndividuos)
        this.api.individuos = (status === 200) ? data : []
        this.paramsIndividuos.skip = (status === 200) ? paginacao.skip : 0
        this.paramsIndividuos.total = (status === 200) ? paginacao.totalCount : 0
      },

      async onClickFiltrar(form) {
        
        this.filtroParams.dataCadastro = moment(this.filtroParams.dataCadastro).format('YYYY-MM-DD')
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
