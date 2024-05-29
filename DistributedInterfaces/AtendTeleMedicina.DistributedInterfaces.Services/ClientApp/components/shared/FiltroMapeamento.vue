<template>

  <el-form :model="filtroParams" ref="filtroParams" :rules="filtroValidacoes"
            label-width="120px" label-position="top" class="forms--filtro">
    <div class="card">
    <div class="card-header">
    <el-row :gutter="20">
      <el-col :xs="24" :sm="24" :md="12" :lg="7" :xl="7">
        <el-form-item label="Nome" prop="nomeCompleto">
          <el-input v-model="filtroParams.nomeCompleto" />
        </el-form-item>
      </el-col>
      <el-col :xs="24" :sm="24" :md="12" :lg="4" :xl="4">
        <el-form-item label="Sexo" prop="sexo">
          <el-select v-model="filtroParams.sexo" placeholder="Todos ..." clearable>
            <el-option v-for="item in enums.sexos" :label="item.label" :value="item.value" :key="item.value" />
          </el-select>
        </el-form-item>
      </el-col>      
      <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
        <el-form-item label="Estado" prop="uf">
          <el-select filterable placeholder="Todos" v-model="filtroParams.uf" value-key="ufAbreviado"
                      v-loading.body="carregando.ufs" @change="onSelectUf" @focus="getUfs" clearable>
            <el-option v-for="option in api.ufs" :value="option.ufAbreviado" :label="option.ufExtenso" :key="option.ufAbreviado" />
          </el-select>
        </el-form-item>
      </el-col>
      <el-col :xs="24" :sm="24" :md="12" :lg="7" :xl="7">
        <el-form-item label="Cidade" prop="cidade_Id">
          <el-select filterable placeholder="Todas" v-model="filtroParams.cidade_Id"
                      v-loading.body="carregando.cidades" :disabled="!filtroParams.uf" clearable>
            <el-option v-for="option in api.cidades" :value="option.id" :label="option.nome" :key="option.id" />
          </el-select>
        </el-form-item>
      </el-col>        
      
    </el-row>
    <el-row :gutter="20">

      <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
        <el-form-item label="Monitoramento Encerrado" prop="monitoramento">
          <el-select v-model="filtroParams.monitoramento" placeholder="Todos ..." clearable>
            <el-option v-for="item in enums.simNaoTodos" :label="item.label" :value="item.value" :key="item.value" />
          </el-select>
        </el-form-item>
      </el-col>
      <el-col :xs="24" :sm="24" :md="12" :lg="4" :xl="4">
        <el-form-item label="Resultado Exame" prop="resultadoExame">
          <el-select v-model="filtroParams.resultadoExame" placeholder="Todos ..." clearable>
            <el-option v-for="item in enums.resultadosExame" :label="item.label" :value="item.value" :key="item.value" />
          </el-select>
        </el-form-item>
      </el-col>
      <el-col :xs="24" :sm="24" :md="12" :lg="4" :xl="4">
      <el-form-item label="Em Isolamento" prop="estaEmIsolamento">
        <el-select v-model="filtroParams.estaEmIsolamento" placeholder="Todos ..." clearable>
          <el-option v-for="item in enums.simNaoTodos" :label="item.label" :value="item.value" :key="item.value" />
        </el-select>
      </el-form-item>
      </el-col>
      <el-col :xs="24" :sm="24" :md="12" :lg="4" :xl="4">
        <el-form-item label="Dias Internação" prop="diasDeInternacao" clearable>
          <el-input v-model="filtroParams.diasDeInternacao" />
       </el-form-item>
      </el-col>
      <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
        <el-form-item label="Com Agravamento de Risco" prop="agravamento">
          <el-select v-model="filtroParams.agravamento" placeholder="Todos ..." clearable>
            <el-option v-for="item in enums.simNaoTodos" :label="item.label" :value="item.value" :key="item.value" />
          </el-select>
        </el-form-item>
      </el-col>      
    </el-row>
    <el-row :gutter="20">
      <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
        <el-form-item label="Classificação de Risco" prop="corStatus">
          <el-select v-model="filtroParams.corStatusIn" placeholder="Todos ..."
                     clearable collapse-tags multiple value-key="value">
            <el-option v-for="item in enums.coresStatus" :label="item.label" :value="item.value" :key="item.value" />
          </el-select>
        </el-form-item>
      </el-col>
      <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
          <el-form-item label="Respondeu a Pesquisa" prop="pesquisa">
            <el-select v-model="filtroParams.pesquisa" placeholder="Todos ..." clearable>
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
          <el-checkbox v-if="onMap" style="margin-right: -10px" v-model="filtroParams.showHeatMap" @change="onMapOptions" border class="forms--filtro--check-estabelecimentos">
            <i class="fas fa-fire-alt"></i> Mapa de Calor
          </el-checkbox>
          <el-checkbox v-if="onMap" style="margin-right: -10px" v-model="filtroParams.showEstabelecimentos" label="Exibir Estabelecimentos" @change="onMapOptions" border class="forms--filtro--check-estabelecimentos">
            <i class="fas fa-building"></i> Estabelecimentos
          </el-checkbox>          
          <el-checkbox v-if="onMap" v-model="filtroParams.showMarkers" label="Exibir Marcadors" @change="onMapOptions" border class="forms--filtro--check-estabelecimentos">
            <i class="fas fa-map-marker-alt"></i> Cidadãos
          </el-checkbox>          
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
      var validaNumeroApenas = (rule, value, callback) => {
        if (!value) return callback()
        value = value.replace(/[.-\s]/g, '')
        if (!value.match(/^[0-9]+$/)) {
          return callback(new Error('Campo só pode conter número'))
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
          ],
          diasInternacao: [ 
            { validator: validaNumeroApenas, trigger: ['blur', 'change'] }
          ]
        },
        filtroParams: {
          showMarkers: true,
          showHeatMap: false
        },
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
    watch: {
      'filtroParams.corStatusIn' (val) {
        if (val.includes(null)) {
          this.filtroParams.corStatusIn = [0, 1, 2, 3, 4]
        }
      }
    },
    async mounted () {
      this.filtroParams = {
        ...this.filtroParams,
        corStatusIn: this.params.corStatusIn
      }
      await this.getUfs()
    },
    methods: {
      async onClickFiltrar (form) {
        if (this.filtroParams.cidade_Id) {
          this.filtroParams.cidadeIBGE = this._.find(this.api.cidades, {id: this.filtroParams.cidade_Id}).ibge
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
      onMapOptions () {
        let mapOptions = {
          showHeatMap: this.filtroParams.showHeatMap ? this.filtroParams.showHeatMap : false,
          showMarkers: this.filtroParams.showMarkers ? this.filtroParams.showMarkers : false,
          showEstabelecimentos: this.filtroParams.showEstabelecimentos ? this.filtroParams.showEstabelecimentos : false
        }
        this.$emit('emitHeatMap', mapOptions)
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
