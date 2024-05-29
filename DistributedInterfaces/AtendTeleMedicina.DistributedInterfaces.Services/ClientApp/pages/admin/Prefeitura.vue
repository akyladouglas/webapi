<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">

    <el-card class="box-card box-card--main-card">

      <el-row>
        <el-col :span="14">
          <!-- <h2 class="box-card--h2">{{$route.meta.title}}</h2> -->
          <h2 class="box-card--h2">Prefeitura</h2>
        </el-col>
        <el-col :span="10" class="text-right">
          <el-form :inline="true">
            <el-form-item>
              <el-button v-if="listando" style="margin-right: -10px" icon="fas fa-plus" type="success" @click="onClickNovo"> Novo</el-button>
              <el-button v-if="!listando" style="margin-right: -10px" icon="fas fa-undo-alt" type="warning" @click="onListar('formPrefeitura')"> Voltar</el-button>
            </el-form-item>
          </el-form>
        </el-col>
      </el-row>

      <el-row v-show="listando">
        <el-col :span="24">
          <FiltroEstabelecimento :loading="loading.prefeituras" :params="params" @emit="onEmitFromFiltro" />
        </el-col>
      </el-row>

      <el-row v-show="listando && api.prefeituras.length > 0">
        <el-col :span="24">
          <el-table ref="tabela" :data="api.prefeituras"
                                  highlight-current-row border
                                  v-loading.body="loading.prefeituras"
                                  class="table--prefeituras table--row-click">
            <el-table-column label="Nome" prop="nome" fixed />
            <el-table-column label="CNPJ" prop="cnpj" width="180" />
            <el-table-column header-align="center" align="right" width="140" fixed="right">
              <template slot-scope="scope">
                <el-dropdown>
                  <el-button type="primary" size="small">
                    Ações <i class="el-icon-arrow-down el-icon--right"></i>
                  </el-button>
                  <el-dropdown-menu slot="dropdown">
                    <ul class="list-unstyled">
                      <li @click="onEditar(scope.$index, scope.row)"  class="el-dropdown-menu__item"><i class="fas fa-user-edit"></i> Editar</li>
                      <el-popconfirm v-if="mxHasAccess('Administrador','GestorMunicipio')"
                        popper-class="forms--dropdown--popover"
                        confirmButtonText='Sim, pode desativar'
                        confirmButtonType="danger"
                        cancelButtonType="primary"
                        cancelButtonText='Não'
                        icon="el-icon-info"
                        iconColor="red"
                        @onConfirm="onExcluir(scope.$index, scope.row)"
                        title="Você tem certeza que deseja desativar o cidadão?">
                      <li slot="reference" class="el-dropdown-menu__item text-danger"><i class="fas fa-trash"></i> Desativar</li>
                      </el-popconfirm>
                    </ul>
                  </el-dropdown-menu>
                </el-dropdown>
              </template>
            </el-table-column>            
          </el-table>
        </el-col>

        <el-col :span="24" v-show="listando">
          <el-pagination
            @size-change="handleSizeChange"
            @current-change="handleCurrentChange"
            :current-page.sync="params.page"
            :page-sizes="[10,25,50,100]"
            :page-size="params.pageSize"
            :total="params.total"
            layout="total, sizes, prev, pager, next, jumper">
          </el-pagination>
        </el-col>
      </el-row>

      <el-row v-show="!listando">

        <el-row :gutter="20" v-if="erros.length > 0">
          <el-col :xs="24">
            <div id="erros" class="alert alert-danger" role="alert">
              <ul>
                <li v-for="erro in erros" :key="erro">{{erro}}</li>
              </ul>
            </div>
          </el-col>
        </el-row>

        <el-form :model="formPrefeitura" status-icon :rules="validacoes" :disabled="isDisabled"
                 ref="formPrefeitura" label-width="120px" label-position="top" class="form--estabelecimento">
          <el-row :gutter="20">
            <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12">
              <el-form-item label="Razão Social" prop="razaoSocial">
                <el-input v-model="formPrefeitura.razaoSocial" />
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
              <el-form-item label="CNPJ" prop="cnpj">
                <el-input v-model="formPrefeitura.cnpj"  v-mask="'##.###.###/####-##'" />
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
              <el-form-item label="Cpf do Responsável" prop="responsavelCpf">
                <el-input v-model="formPrefeitura.responsavelCpf" masked="true" maxlength="14" v-mask="'###.###.###-##'" />
              </el-form-item>
            </el-col>
          </el-row>

          <el-row :gutter="20">

            <el-col :xs="24" :sm="24" :md="12" :lg="4" :xl="8">
              <el-form-item label="Tipo de Empresa" prop="tipoEmpresa">
                <el-input v-model="formPrefeitura.tipoEmpresa" />
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="4" :xl="8">
              <el-form-item label="Ramo de Atividade" prop="ramoAtividade">
                <el-input v-model="formPrefeitura.ramoAtividade" />
              </el-form-item>
            </el-col>
          </el-row>

          <el-row :gutter="20">
           <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
              <el-form-item label="CEP" prop="logradouroCep">
                <el-input v-model="formPrefeitura.logradouroCep" @input="getCep" masked="true" maxlength="9" v-mask="'#####-###'" />
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
              <el-form-item label="Estado" prop="ufAbreviado">
                <el-select filterable placeholder="Selecione o Estado" v-model="formPrefeitura.ufAbreviado"
                           v-loading.body="loading.ufs" @change="onSelectUf" >
                  <el-option v-for="option in api.ufs" :value="option.ufAbreviado" :label="option.ufExtenso" :key="option.ufAbreviado" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
              <el-form-item label="Cidade" prop="cidadeId">
                <el-select filterable placeholder="Selecione a Cidade" v-model="formPrefeitura.cidadeId"
                           v-loading.body="loading.cidades" >
                  <el-option v-for="option in api.cidades" :value="option.ibge" :label="option.nome" :key="option.ibge" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
              <el-form-item label="Bairro" prop="bairro">
                <el-input v-model="formPrefeitura.logradouroBairro"  />
              </el-form-item>
            </el-col>
          </el-row>

          <el-row :gutter="20">
            <el-col :xs="24" :sm="24" :md="12" :lg="8" :xl="8">
              <el-form-item label="Endereço" prop="logradouro">
                <el-input v-model="formPrefeitura.logradouro" />
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="2" :xl="2">
              <el-form-item label="Número" prop="numero">
                <el-input v-model="formPrefeitura.logradouroNumero" />
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="3" :xl="3">
              <el-form-item label="Complemento" prop="complemento">
                <el-input v-model="formPrefeitura.logradouroComplemento" />
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="5">
              <el-form-item prop="deletado" class="forms--label-no-title">
                <el-checkbox v-model="formPrefeitura.deletado" label="Deletado" border />
              </el-form-item>
            </el-col>             
          </el-row>

          <el-row :gutter="20">
            <el-col :xs="24">
              <el-form-item>
                  <el-button flat icon="fas fa-save" type="success" :disabled="!isValid" v-if="mxHasAccess('Administrador', 'Atendente', 'Recepcionista')" @click="onClickSalvar('formPrefeitura')" v-loading.body="loading.prefeituras"> Salvar</el-button>
                  <el-button flat icon="fas fa-undo-alt" type="warning" @click="onListar('formPrefeitura')" :disabled="loading.prefeituras"> Voltar</el-button>
                  <el-button flat icon="fas fa-eraser" v-if="metodo === 'POST'" type="default" @click="resetForm('formPrefeitura')" :disabled="loading.prefeituras"> Limpar</el-button>
              </el-form-item>
            </el-col>
          </el-row>

        </el-form>

      </el-row>
   </el-card>

  </el-col>
</template>

<script>
  import Utils from '../../mixins/Utils'
  import jQuery from 'jquery'
  import _api from '../../api'
  import _enums from '../../api/Enums'
  import FiltroEstabelecimento from '../../components/shared/FiltroEstabelecimento'
  import { mask } from 'vue-the-mask'
  export default {
    name: 'Prefeitura',
    mixins: [Utils],
    components: { FiltroEstabelecimento },
    directives: { mask },
    data () {
      return {
        isDisabled: false,
        isValid: true,
        metodo: 'POST',
        metodoModelo: 'PUT',
        listando: true,
        erros: [],
        formPrefeitura: {},
        validacoes: {
          nome: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' },
            { min: 3, max: 255, message: 'Nome não pode conter menos de 4 e mais que 255 caracteres', trigger: 'submit' }
          ],
          ufAbreviado: [{ required: true, message: 'Campo obrigatório', trigger: ['blur', 'change'] }],
          cidadeId: [{ required: true, message: 'Campo obrigatório', trigger: ['blur', 'change'] }],
          logradouroCep: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' }
          ],
          logradouro: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' },
            { min: 1, max: 150, message: 'Endereço não pode conter menos de 1 e mais que 150 caracteres', trigger: 'change' }
          ],
          logradouroNumero: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' }
          ],             
          logradouroBairro: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' },
            { min: 1, max: 100, message: 'Bairro não pode conter menos de 1 e mais que 100 caracteres', trigger: 'change' }
          ]      
        },
        enums: {},
        api: {
          prefeituras: [],
          ufs: [],
          cidades: []
        },
        loading: {
          prefeituras: false,
          ufs: false,
          cidades: false
        },
        params: {
          skip: 1,
          take: 10,
          sort: 'p.Nome ASC',
          total: 0
        },
        paramsUfs: {
          skip: 1,
          take: 30,
          sort: '+UfAbreviado',
          total: 0
        },
        paramsCidades: {
          skip: 1,
          take: 999,
          sort: '+Nome',
          total: 0,
          ufAbreviado: null
        }        
      }
    },
    async mounted () {
      await this.getEstabelecimentos()
      await this.getUfs()
    },
    methods: {
      onEmitFromFiltro (val) {
        this.params = {
          ...this.params,
          ...val.params,
          skip: 1
        }
        this.listando = true
        this.getEstabelecimentos()
      },
      async getEstabelecimentos () {
        this.loading.prefeituras = true
        let { data, paginacao, status } = await _api.prefeituras.get(this.params)
        if (status === 502) this.loading.prefeituras = false
        this.api.prefeituras = (status === 200) ? data : []
        this.params.skip = (status === 200) ? paginacao.currentPage : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.prefeituras = false
      },      
      async getUfs () {
        this.loading.ufs = true
        let { data, paginacao, status } = await _api.ufs.get(this.paramsUfs)
        this.api.ufs = (status === 200) ? data : []
        if (this.api.ufs.length === 1) {
          this.formPrefeitura.ufAbreviado = this.api.ufs[0].ufAbreviado
          this.getCidadesByUf()
        }
        this.paramsUfs.skip = (status === 200) ? paginacao.currentPage : 0
        this.paramsUfs.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.ufs = false
      },
      async getCidadesByUf () {
        this.loading.cidades = true
        let { data, paginacao, status } = await _api.cidades.get(this.paramsCidades)
        this.api.cidades = (status === 200) ? data : []
        if (this.api.cidades.length === 1) {
          this.formPrefeitura.cidadeId = this.api.cidades[0].ibge
        }
        this.paramsCidades.skip = (status === 200) ? paginacao.currentPage : 0
        this.paramsCidades.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.cidades = false
      },
      async onSelectUf (val) {
        this.paramsCidades.ufAbreviado = val
        this.formPrefeitura = {
          ...this.formPrefeitura,
          cidadeId: null
        }
        await this.getCidadesByUf()
      },
      async onClickNovo () {
        this.isDisabled = false
        this.listando = false
        this.metodo = 'POST'
        this.formPrefeitura = {
          ...this.formPrefeitura,
          ativo: true
        }
      },
      async onEditar (index, row) {
        this.metodo = 'PUT'
        this.isDisabled = false
        this.listando = false
        await this.onSelectUf(row.ufAbreviado)
        await this.getCidadesByUf()
        this.formPrefeitura = {
          ...row
        }
      },
      async onExcluir (index, row) {
        await _api.prefeituras.delete(row.id)
        this.getAgentesFiscalizacao()
      },
      onListar (form) {
        let i = this.api.prefeituras.findIndex(x => x.id === this.formPrefeitura.id)
        this.$refs.tabela.setCurrentRow(this.api.prefeituras[i])
        this.$refs[form].resetFields()
        this.listando = true
      },
      async onClickSalvar (form) {
        this.erros = []
        this.loading.prefeituras = true
        this.$refs[form].validate((valid) => {
          if (valid) {
            if (this.metodo === 'POST') {
              _api.prefeituras.post(this.formPrefeitura).then(res => {
                if (res.status === 201) {
                  this.onListar(form)
                  this.getAgentesFiscalizacao()
                  this.loading.prefeituras = false
                }
                this.loading.prefeituras = false
              })
            } else {
              var estabelecimento = this._.clone(this.formPrefeitura)
              _api.prefeituras.put(this.formPrefeitura).then(res => {
                if (res.status === 204) {
                  let i = this.api.prefeituras.findIndex(x => x.id === estabelecimento.id)
                  this.api.prefeituras[i] = estabelecimento
                  this.$refs.tabela.setCurrentRow(estabelecimento)
                  this.onListar(form)
                } else {
                  res.data.forEach(i => {
                    this.erros.push(i)
                  })
                   jQuery('.form--estabelecimento').get(0).scrollIntoView()
                }
                this.loading.prefeituras = false
              })
            }
          } else {
            this.$swal({
              title: "Atenção!",
              text: 'Verifique o preenchimento do formulário!',
              icon: 'warning',
            })
            this.loading.prefeituras = false
            jQuery('.el-form-item__error').get(0).scrollIntoView()
            return false
          }
        })
      },
      async getCep (cep) {
        if (this.listando) return
        if (cep.length > 8) {
          let { data, status } = await _api.auxiliar.getCep(cep)
          if (status === 200) {
            this.formPrefeitura.logradouro = data.logradouro
            this.formPrefeitura.logradouroBairro = data.bairro
            this.formPrefeitura.ufAbreviado = data.uf
            this.paramsCidades.ibge = data.ibge
            await this.onSelectUf(data.uf)
            await this.getCidadesByUf()
          } else {
            this.paramsCidades.ibge = null
            this.formPrefeitura = {
                ...this.formPrefeitura,
                ufAbreviado: null,
                cidadeId: null,
                logradouro: null,
                bairro: null,
              }
          }
        }
      },
      limparSelecao (rows) {
        if (rows) {
          rows.forEach(row => {
            this.$refs.tabela.toggleRowSelection(row)
          })
        } else {
          this.$refs.tabela.clearSelection()
        }
      },
      resetForm (form) {
        this.$refs[form].resetFields()
      },
      handleSizeChange (val) {
        this.params.pageSize = val
      },
      handleCurrentChange (val) {
        this.params.currentPage = val
        this.getAgentesFiscalizacao()
      }
    }
  }
</script>
