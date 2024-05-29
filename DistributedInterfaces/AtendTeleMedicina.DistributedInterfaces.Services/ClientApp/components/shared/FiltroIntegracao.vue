<template>

  <el-form :model="filtroParams" ref="filtroParams" label-width="120px" label-position="top" class="forms--filtro">
    <div class="card">
      <div class="card-header">
        <el-row :gutter="20">
          <el-col :xs="24" :sm="24" :md="12" :lg="8" :xl="8">
            <el-form-item label="Estabelecimento" prop="estabelecimento">
              <el-select filterable placeholder="Selecione o Estabelecimento" v-model="filtroParams.estabelecimentoId"
                          v-loading.body="loading.estabelecimentos" @change="onChangeEstabelecimento">
                <el-option v-for="option in api.estabelecimentos" :value="option.id" :label="option.nomeFantasia" :key="option.id" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="24" :md="12" :lg="2" :xl="2">
            <el-form-item label="Mês" prop="mes">
              <el-input type="number" v-model="filtroParams.mes" />
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="24" :md="12" :lg="2" :xl="2">
            <el-form-item label="Ano" prop="ano">
              <el-input type="number" v-model="filtroParams.ano" />
            </el-form-item>
          </el-col>
        </el-row>
      </div>
    </div>
    <el-row :gutter="20">
      <el-col :span="24" class="text-right">
        <el-form-item class="forms--margin-xs-from-top">
          <el-button v-if="!loadingLotes" type="info" icon="fas fa-filter" @click="onClickFiltrar('filtroParams')"> Filtrar</el-button>
          <el-button v-if="loadingLotes" type="info" icon="fa fa-spinner fa-spin" disabled> Aguarde ...</el-button>
          <el-button v-if="!loadingLotes" type="info" icon="fas fa-box" @click="onClickGerar('filtroParams')"> Gerar Lote</el-button>
          <el-button v-if="loadingLotes" type="info" icon="fa fa-spinner fa-spin" disabled> Aguarde ...</el-button>
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

  export default {
    mixins: [Utils],
    name: 'FiltroIntegracao',
    props: ['params', 'loadingLotes'],
    data () {
      return {
        filtroParams: {
          mes: moment().format('MM') - 1,
          ano: moment().format('YYYY')
        },
        api: {
          estabelecimentos: []
        },
        loading: {
          estabelecimentos: false
        },
        enums: {
          simNaoTodos: _enums.simNaoTodos
        },
        estabParams: {
          skip: 1,
          take: 30,
          ativo: true
        }
      }
    },
    async mounted () {
      await this.getEstabelecimentos()
    },
    methods: {
      async onClickFiltrar (form) {
        this.$refs[form].validate((valid) => {
          if (valid) {
            this.$emit('filtrar', this.filtroParams)
          } else {
            this.$swal({
              title: "Atenção!",
              text: "Verifique o preenchimento dos filtros!",
              icon: 'warning',
            })
          }
        })
      },
      async onClickGerar (form) {
        this.$refs[form].validate((valid) => {
          if (valid) {
            this.$emit('gerar', this.filtroParams)
          } else {
            tthis.$swal({
              title: "Atenção!",
              text: "Verifique o preenchimento dos filtros!",
              icon: 'warning',
            })
          }
        })
      },
      async getEstabelecimentos () {
        this.loading.estabelecimentos = true
        let { data, paginacao, status } = await _api.estabelecimentos.get(this.estabParams)
        if (status === 502) this.loading.estabelecimentos = false
        this.api.estabelecimentos = (status === 200) ? data.filter(estab => estab.nomeFantasia ? estab : null) : []
        if (this.api.estabelecimentos.length === 1) this.filtroParams.estabelecimentoId = this.api.estabelecimentos[0].id
        this.estabParams.skip = (status === 200) ? paginacao.skip : 0
        this.estabParams.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.estabelecimentos = false
      },
      onChangeEstabelecimento () {
        console.log('')
      }
    }
  }
</script>
