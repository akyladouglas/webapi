<template>

  <el-form :model="filtroParams" ref="filtroParams" :rules="filtroValidacoes"
            label-width="120px" label-position="top" class="forms--filtro">
    <div class="card">
    <div class="card-header">
    <el-row :gutter="20">
      <el-col :xs="24" :sm="24" :md="12" :lg="7" :xl="7">
        <el-form-item label="Nome" prop="nome">
          <el-input v-model="filtroParams.nome" maxlength="50"/>
        </el-form-item>
      </el-col>
      <el-col :xs="24" :sm="24" :md="12" :lg="4" :xl="4">
        <el-form-item label="CPF" prop="cpf" masked="true" maxlength="14" v-mask="'###.###.###-##'">
          <el-input v-model="filtroParams.cpf" />
        </el-form-item>
      </el-col>
      <!--<el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="5">
        <el-form-item label="Estado" prop="ufAbreviado">
          <el-select filterable placeholder="Todos" v-model="filtroParams.ufAbreviado"
                      v-loading.body="carregando.ufs" @change="onSelectUf" @focus="getUfs" clearable>
            <el-option v-for="option in api.ufs" :value="option.ufAbreviado" :label="option.ufExtenso" :key="option.ufAbreviado" />
          </el-select>
        </el-form-item>
      </el-col>-->
      <!--<el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="5">
        <el-form-item label="Cidade" prop="cidadeId">
          <el-select filterable placeholder="Todas" v-model="filtroParams.cidadeId"
                      v-loading.body="carregando.cidades" :disabled="!filtroParams.ufAbreviado" clearable>
            <el-option v-for="option in api.cidades" :value="option.ibge" :label="option.nome" :key="option.ibge" />
          </el-select>
        </el-form-item>
      </el-col>-->
      <el-col :xs="24" :sm="24" :md="12" :lg="3" :xl="3">
        <el-form-item label="Ativo" prop="ativo">
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
  import { mask } from 'vue-the-mask'
  import Utils from '../../mixins/Utils'
  import _enums from '../../api/Enums'
  import _api from '../../api'

  export default {
    mixins: [Utils],
    directives: { mask },
    name: 'Filtro',
    props: ['loading', 'params', 'onMap'],
    data() {

      var validaNome = (rule, value, callback) => {
        if (!value) return callback()
        if (this.mxValidaNome(value) === false) {
          return callback(new Error('O campo pode conter apenas letras'))
        } else {
          callback()
        }
      }

      var validaCpf = (rule, value, callback) => {
        if (!value) return callback()
        value = value ? value.replace(/[.-\s]/g, '') : null
        if (!value.match(/^[0-9]+$/)) return callback(new Error('CPF só pode conter números'))
        if (!value) return callback(new Error('CPF Obrigatório'))
        if (this.mxValidaCPF(value) === false) {
          return callback(new Error('CPF Inválido'))
        } else {
          callback()
        }
      }
      return {
        filtroValidacoes: {
          nome: [
            { validator: validaNome, trigger: ['blur', 'change'] }
          ],
          cpf: [
            { validator: validaCpf, trigger: ['change'] }
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
        enums: {
          simNaoTodos: [
            { label: 'Todos', value: null },
            ..._enums.simNao
          ]
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
          ufAbreviado: null
        }
      }
    },
    async mounted () {
      await this.getUfs()
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
      }
    }
  }
</script>
