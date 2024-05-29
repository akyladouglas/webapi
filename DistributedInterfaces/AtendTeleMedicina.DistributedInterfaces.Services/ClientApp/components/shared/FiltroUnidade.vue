<template>

  <el-form :model="filtroParams" ref="filtroParams" :rules="filtroValidacoes"
            label-width="120px" label-position="top" class="forms--filtro">
    <div class="card">
    <div class="card-header">
    <el-row :gutter="20">
      <el-col :xs="24" :sm="24" :md="12" :lg="8" :xl="6">
        <el-form-item label="Nome" prop="nome">
          <el-input v-model="filtroParams.nome" />
        </el-form-item>
      </el-col>
      <el-col :xs="24" :sm="24" :md="12" :lg="4" :xl="4">
        <el-form-item label="CNES" prop="cnes">
          <el-input v-model="filtroParams.cnes" />
        </el-form-item>
      </el-col>
      <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="5">
        <el-form-item label="Estado" prop="uf">
          <el-select filterable placeholder="Todos" v-model="filtroParams.uf"
                      v-loading.body="carregando.ufs" @change="onSelectUf" @focus="getUfs" clearable>
            <el-option v-for="option in api.ufs" :value="option.ufAbreviado" :label="option.ufExtenso" :key="option.ufAbreviado" />
          </el-select>
        </el-form-item>
      </el-col>
      <el-col :xs="24" :sm="24" :md="12" :lg="7" :xl="7">
        <el-form-item label="Cidade" prop="cidadeId">
          <el-select filterable placeholder="Todas" v-model="filtroParams.cidadeId"
                      v-loading.body="carregando.cidades" :disabled="!filtroParams.uf" clearable>
            <el-option v-for="option in api.cidades" :value="option.ibge" :label="option.nome" :key="option.ibge" />
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
  import { mask } from 'vue-the-mask'
  import Utils from '../../mixins/Utils'
  import _enums from '../../api/Enums'
  import _api from '../../api'

  export default {
    mixins: [Utils],
    directives: { mask },
    name: 'Filtro',
    props: ['loading', 'params'],
    data () {
      var validaCpf = (rule, value, callback) => {
        if (!value) return callback()
        value = value ? value.replace(/[.-\s]/g, '') : null
        if (!value.match(/^[0-9]+$/)) return callback(new Error('Cpf só pode conter números'))
        if (!value) return callback(new Error('Cpf Obrigatório'))
        if (this.mxValidaCPF(value) === false) {
          return callback(new Error('Cpf Inválido'))
        } else {
          callback()
        }
      }
      var validaCns = (rule, value, callback) => {
        if (!value) return callback()
        value = value ? value.replace(/[.-\s]/g, '') : null
        if (!value.match(/^[0-9]+$/)) {
          return callback(new Error('Cartão SUS só pode conter números'))
        } else {
          callback()
        }
      }
      return {
        filtroValidacoes: {
          cpf: [ 
            { validator: validaCpf, trigger: ['change'] }
          ],
          cartaoSUS: [
            { validator: validaCns, trigger: ['blur', 'change'] }
          ]
        },
        filtroParams: {},
        api: {
          ufs: [],
          cidades: []
        },
        carregando: {
          ufs: false,
          cidades: false
        },
        enums: {},
        estadoParams: {
          skip: 1,
          take: 30,
          sort: '+UfAbreviado',
          total: 0
        },
        cidadeParams: {
          skip: 1,
          take: 999,
          sort: '+Nome',
          total: 0,
          ufAbreviado: null
        }
      }
    },
    async mounted () {
      this.filtroParams = {
        ...this.filtroParams
      }
      await this.getUfs()
    },
    methods: {
      async onClickFiltrar (form) {
        if (this.filtroParams.cidadeId) {
          this.filtroParams.cidadeIBGE = this._.find(this.api.cidades, {id: this.filtroParams.cidadeId}).ibge
        }
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
      async getUfs () {
        this.carregando.ufs = true
        let { data, status } = await _api.ufs.get(this.estadoParams)
        this.api.ufs = (status === 200) ? data : []
        if (this.api.ufs.length === 1) {
          this.filtroParams.uf = this.api.ufs[0].ufAbreviado
          this.getCidadesByUf()
        } 
        this.carregando.ufs = false
      },
      async getCidadesByUf () {
        this.carregando.cidades = true
        let { data, paginacao, status } = await _api.cidades.get(this.cidadeParams)
        this.api.cidades = (status === 200) ? data : []
        if (this.api.cidades.length === 1) {
          this.filtroParams.cidadeId = this.api.cidades[0].id
          this.getCidadesByUf()
        }        
        this.cidadeParams.skip = (status === 200) ? paginacao.currentPage : 0
        this.cidadeParams.total = (status === 200) ? paginacao.totalCount : 0
        this.carregando.cidades = false
      },
      async onSelectUf (val) {
        delete this.filtroParams.cidadeId
        this.cidadeParams = {
          ...this.cidadeParams,
          ufAbreviado: val,
          cidadeId: null
        }
        await this.getCidadesByUf()
      }
    }
  }
</script>
