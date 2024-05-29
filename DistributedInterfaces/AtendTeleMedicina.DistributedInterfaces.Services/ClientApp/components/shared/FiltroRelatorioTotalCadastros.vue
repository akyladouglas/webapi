<template>

  <el-form :model="filtroParams" ref="filtroParams" :rules="filtroValidacoes"
           label-width="120px" label-position="top" class="forms--filtro">
    <div class="card">
      <div class="card-header">
        <el-row :gutter="20">

          <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="5">
            <el-form-item label="Estado" prop="uf">
              <el-select filterable placeholder="Todos" v-model="filtroParams.uf"
                         v-loading.body="carregando.ufs" @change="onSelectUf" @focus="getUfs" clearable>
                <el-option v-for="option in api.ufs" :value="option.ufAbreviado" :label="option.ufExtenso" :key="option.ufAbreviado" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="5">
            <el-form-item label="Cidade" prop="cidade_Id">
              <el-select filterable placeholder="Todas" v-model="filtroParams.cidade_Id"
                         v-loading.body="carregando.cidades" :disabled="!filtroParams.uf" clearable>
                <el-option v-for="option in api.cidades" :value="option.id" :label="option.nome" :key="option.id" />
              </el-select>
            </el-form-item>
          </el-col>

          <!--<el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
            <el-form-item label="Classificação de Risco" prop="corStatus">
              <el-select v-model="filtroParams.corStatus" placeholder="Todos ...">
                <el-option v-for="item in enums.coresStatus" :label="item.label" :value="item.value" :key="item.value" />
              </el-select>
            </el-form-item>
          </el-col>-->

          <el-col :xs="24" :sm="24" :md="12" :lg="4" :xl="4">
            <el-form-item label="Data Inicial" prop="dataInicial">
              <el-date-picker v-model="filtroParams.dataInicial"
                              type="date"
                              placeholder="Data Inicial"
                              format="dd/MM/yyyy">
              </el-date-picker>
            </el-form-item>
          </el-col>

          <el-col :xs="24" :sm="24" :md="12" :lg="4" :xl="4">
            <el-form-item label="Data Final" prop="dataFinal">
              <el-date-picker v-model="filtroParams.dataFinal"
                              type="date"
                              format="dd/MM/yyyy"
                              placeholder="Data Final">
              </el-date-picker>
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
  import { mask } from 'vue-the-mask'
  import Utils from '../../mixins/Utils'
  import _enums from '../../api/Enums'
  import _api from '../../api'

  export default {
    mixins: [Utils],
    directives: { mask },
    name: 'FiltroRelatorioTotalCadastros',
    props: ['loading', 'params'],
    data () {
      return {
        filtroValidacoes: {},
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
          simNaoTodos: _enums.simNaoTodos,
          sexos: [
            { label: 'Todos', value: null },
            ..._enums.sexos
          ],
          coresStatus: [
            { label: 'Todos', value: null },
            ..._enums.coresStatus
          ],
          resultadosExame: [
            { label: 'Todos', value: null },
            ..._enums.resultadosExame
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
      async onFiltrar (form) {
        var i = _api.relatorios.Lista(form);
      },
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
          this.filtroParams.cidade_Id = this.api.cidades[0].id
          this.getCidadesByUf()
        }
        this.cidadeParams.skip = (status === 200) ? paginacao.currentPage : 0
        this.cidadeParams.total = (status === 200) ? paginacao.totalCount : 0
        this.carregando.cidades = false
      },
      async onSelectUf (val) {
        this.api.cidades = []
        delete this.filtroParams.cidade_Id
        this.cidadeParams = {
          ...this.cidadeParams,
          ufAbreviado: val,
          cidade_Id: null
        }
        await this.getCidadesByUf()
      }
    }
  }
</script>
