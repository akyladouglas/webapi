<template>

  <el-form :model="filtroParams" ref="filtroParams" :rules="filtroValidacoes" label-width="120px" label-position="top" class="forms--filtro-estabelecimento">
    <div class="card">
    <div class="card-header">
      <el-row :gutter="20">
        <el-col :xs="24" :sm="24" :md="12" :lg="7" :xl="7">
          <el-form-item label="Nome" prop="nomeFantasia">
            <el-input v-model="filtroParams.nomeFantasia" maxlength="50" placeholder="Filtre pelo nome"/>
          </el-form-item>
        </el-col>
        <el-col v-if="!isDemandaEspontanea()" :xs="24" :sm="24" :md="12" :lg="4" :xl="4">
          <el-form-item label="CNPJ" prop="cnpj">
            <el-input v-model="filtroParams.cnpj" maxlength="14" placeholder="Filtre pelo cnpj"/>
          </el-form-item>
        </el-col>
        <el-col v-if="isDemandaEspontanea()" :xs="24" :sm="24" :md="12" :lg="4" :xl="4">
          <el-form-item label="CNES" prop="cnes">
            <el-input v-model="filtroParams.cnes" maxlength="14" />
          </el-form-item>
        </el-col>
        <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="5">
          <el-form-item label="Estado" prop="uf">
            <el-select filterable placeholder="Todos" v-model="filtroParams.uf"
                       v-loading.body="carregando.ufs" @change="onSelectUf"   @clear="onClearUf" clearable>
              <el-option v-for="option in api.ufs" :value="option.ufAbreviado" :label="option.ufExtenso" :key="option.ufAbreviado" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="5">
          <el-form-item label="Cidade" prop="cidadeId">
            <el-select filterable placeholder="Todas" v-model="filtroParams.codIbgeMun"
                       v-loading.body="carregando.cidades" :disabled="!filtroParams.uf"  @clear="onClearCidade" clearable>
              <el-option v-for="option in api.cidades" :value="option.ibge" :label="option.nome" :key="option.ibge" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :xs="24" :sm="24" :md="12" :lg="3" :xl="3">
          <el-form-item label="Ativo" prop="deletado">
            <el-select v-model="filtroParams.ativo" placeholder="Todos ...">
              <el-option v-for="item in enums.simNaoTodos" :label="item.label" :value="item.value" :key="item.value" />
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
  import Utils from '../../mixins/Utils'
  import { mask } from 'vue-the-mask'
  import _api from '../../api'
  import _enums from '../../api/Enums'

export default {
    name: 'FiltroEstabelecimento',
    props: ['loading', 'params'],
    mixins: [Utils],
    directives: { mask },
    data() {
      var validaNome = (rule, value, callback) => {
        if (!value) return callback()
        if (this.mxValidaNome(value) === false) {
          return callback(new Error('O campo pode conter apenas letras'))
        } else {
          callback()
        }
      }

      var validaCNPJ = (rule, value, callback) => {
        if (!value) return callback()
        if (this.mxValidaCnpj(value) === false) {
          return callback(new Error('O campo pode conter apenas números'))
        } else {
          callback()
        }
      }
      return {
        filtroValidacoes: {
          nomeFantasia: [
            { validator: validaNome, trigger: ['blur', 'change'] }
          ],
          cnpj: [
            { validator: validaCNPJ, trigger: ['blur', 'change'] }
          ]
        },
        filtroParams: {},
        enums: {
          simNaoTodos: [
            { label: 'Todos', value: null },
            ..._enums.simNao
          ]
        },
        api: {
          ufs: [],
          cidades: []
        },
        carregando: {
          ufs: false,
          cidades: false
        },
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
          uf: null
        }
      }
    },
    async created () {
      await this.getUfs()
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
  
      async getUfs () {
        this.carregando.ufs = true
        let { data, status } = await _api.ufs.get(this.estadoParams)
        this.api.ufs = (status === 200) ? data : []
        if (this.api.ufs.length === 1) {
          this.getCidadesByUf()
        }
        this.carregando.ufs = false
      },
      async getCidadesByUf () {
        this.carregando.cidades = true
        let { data, paginacao, status } = await _api.cidades.get(this.cidadeParams)
        this.api.cidades = (status === 200) ? data : []
        if (this.api.cidades.length === 1) {
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
      },
      onClearUf() {
        console.log("CLEAR")
        delete this.filtroParams.uf
        delete this.filtroParams.codIbgeMun
      },
      onClearCidade() {
        delete this.filtroParams.codIbgeMun
      }
    }
  }
</script>
