<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
    <el-card class="box-card box-card--main-card">

      <el-row>
        <el-col :span="14">
          <h2 class="box-card--h2">{{$route.meta.title}}</h2>
        </el-col>
        <el-col :span="10" class="text-right">
          <el-form :inline="true">
            <el-form-item>
              <!--<el-button v-if="listando" style="margin-right: -10px" icon="fas fa-plus" type="success" @click="onClickNovo"> Novo</el-button>-->
              <el-button v-if="!listando" style="margin-right: -10px" icon="fas fa-undo-alt" type="warning" @click="onListar('formProcedimento')"> Voltar</el-button>
            </el-form-item>
          </el-form>
        </el-col>
      </el-row>

      <el-row v-show="listando">
        <el-col :span="24">
          <FiltroProcedimento :loading="loading.procedimentos" :params="params" @emit="onEmitFromFiltro" />
        </el-col>
      </el-row>

      <el-row v-show="listando && api.procedimentos.length > 0">
        <el-col :span="24">
          <el-table ref="tabela" :data="api.procedimentos"
                                  highlight-current-row border
                                  v-loading.body="loading.procedimentos"
                                  class="table--procedimentos table--row-click">
            <el-table-column label="Tipo" prop="tipo" align="center" width="125" fixed />
            <el-table-column label="Código" prop="codigo" align="center" width="125" />
            <el-table-column label="Descrição" prop="descricao" />
            <el-table-column label="Sexo" prop="sexo" align="center" width="75">
              <template slot-scope="scope">
                <i v-if="scope.row.sexo != 'F'" class="fas fa-male"></i>
                <i v-if="scope.row.sexo != 'M'" class="fas fa-female"></i>
              </template>
            </el-table-column>
            <el-table-column header-align="center" align="right" width="140" fixed="right">
              <template slot-scope="scope">
                <el-dropdown>
                  <el-button type="primary" size="small">
                    Ações <i class="el-icon-arrow-down el-icon--right"></i>
                  </el-button>
                  <el-dropdown-menu slot="dropdown">
                    <ul class="list-unstyled">
                      <!--<li @click="onEditar(scope.$index, scope.row)"  class="el-dropdown-menu__item">
                        <i class="fas fa-edit"></i>
                        Editar
                      </li>-->
                      <li @click="onEditarCota(scope.$index, scope.row)"  class="el-dropdown-menu__item">
                        <i class="fas fa-retweet"></i>
                        Editar Cota
                      </li>
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

        <el-form :model="formProcedimento" status-icon :rules="validacoes" :disabled="isDisabled"
                 ref="formProcedimento" label-width="120px" label-position="top" class="form--procedimento">
          <el-row :gutter="20">
            <el-col :xs="24" :sm="24" :md="8" :lg="6" :xl="6">
              <el-form-item label="Tipo" prop="tipo">
                <el-select :disabled="modo == 'editarCota'" v-model="formProcedimento.tipo" placeholder="Selecione...">
                  <el-option v-for="option in enums.tipoProcedimento" :value="option.value" :label="option.label" :key="option.value" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="8" :lg="6" :xl="6">
              <el-form-item label="Código" prop="codigo">
                <el-input :disabled="modo == 'editarCota'" v-model="formProcedimento.codigo" maxlength="9" />
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="8" :lg="12" :xl="12">
              <el-form-item label="Descrição" prop="descricao">
                <el-input :disabled="modo == 'editarCota'" v-model="formProcedimento.descricao" minlength="6" maxlength="100" />
              </el-form-item>
            </el-col>
          </el-row>

          <el-row :gutter="20">
            <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
              <el-form-item label="Sexo" prop="sexo">
                <el-radio-group :disabled="modo == 'editarCota'" v-model="formProcedimento.sexo">
                  <el-radio label="F">Feminino</el-radio>
                  <el-radio label="M">Masculino</el-radio>
                  <el-radio label="*">Ambos</el-radio>
                </el-radio-group>
              </el-form-item>
            </el-col>
          </el-row>

          <template v-if="modo != 'adicionar'">
            <el-row :gutter="20">
              <el-col :xs="24" :sm="24" :md="8" :lg="8" :xl="8">
                <el-form-item label="Cota disponível" prop="ramoAtividade">
                  <el-input :disabled="true" v-model="formProcedimento.cotaTotal" :maxlength="3" />
                </el-form-item>
              </el-col>
            <el-col :xs="24" :sm="24" :md="8" :lg="8" :xl="8">
                <el-form-item label="Cota Distribuída" prop="cotaEstabelecimento">
                  <el-input :disabled="true" v-model="formProcedimento.cotaEstabelecimento" :maxlength="3" />
                </el-form-item>
              </el-col>
            </el-row>
          </template>

          <template v-if="modo == 'editarCota'">
            <el-row :gutter="20">
              <el-col :xs="24" :sm="24" :md="8" :lg="8" :xl="8">
                <el-form-item label="Quantidade" prop="quantidade">
                  <el-input v-model="formProcedimento.quantidade" minlength="1" maxlength="3" />
                  <!--<el-input-number v-model="formProcedimento.quantidade" :min="1" :max="999" :maxlength="3" />-->
                </el-form-item>
              </el-col>
            </el-row>
          </template>

          <el-row :gutter="20">
            <el-col :xs="24">
              <el-form-item>
                  <el-button flat icon="fas fa-save" type="success" :disabled="!isValid" v-if="mxHasAccess('Administrador', 'Atendente', 'Recepcionista')" @click="onClickSalvar('formProcedimento')" v-loading.body="loading.procedimentos"> Salvar</el-button>
                  <el-button flat icon="fas fa-undo-alt" type="warning" @click="onListar('formProcedimento')" :disabled="loading.procedimentos"> Voltar</el-button>
                  <el-button flat icon="fas fa-eraser" v-if="metodo === 'POST'" type="default" @click="resetForm('formProcedimento')" :disabled="loading.procedimentos"> Limpar</el-button>
              </el-form-item>
            </el-col>
          </el-row>

        </el-form>

      </el-row>

   </el-card>
  </el-col>
</template>

<script>
  import Utils from '../../../mixins/Utils'
  import jQuery from 'jquery'
  import _api from '../../../api'
  import _enums from '../../../api/Enums'
  import FiltroProcedimento from '../../../components/shared/FiltroProcedimento'
  import { mask } from 'vue-the-mask'
  export default {
      name: 'Procedimentos',
      mixins: [Utils],
      components: { FiltroProcedimento },
      directives: { mask },
      data () {

        var validaCodigo = (rule, value, callback) => {
          if (!value) return callback(new Error('Campo obrigatório'))
          if (value.length > 7) {
            if (this.mxValidaCodigo(value) === false) {
              return callback(new Error('O campo pode conter apenas letras e números'))
            } else {
              callback()
            }
          } else {
            return callback(new Error('Digite no minimo 8 caracteres'))
          }
        }

        var validaDescricao = (rule, value, callback) => {
          if (!value) return callback(new Error('Campo obrigatório'))
          if (value.length > 3) {
            if (this.mxValidaDescricao(value) === false) {
              return callback(new Error('Caracteres inseridos no campo estão invalidos'))
            } else {
              callback()
            }
          } else {
            return callback(new Error('Digite no minimo 6 caracteres'))
          }
          
        }

        var validaQuantidade = (rule, value, callback) => {
          if (!value) return callback()
          if (this.mxValidaNumero(value) === false) {
            return callback(new Error('O campo pode conter apenas números'))
          } else {
            callback()
          }
        }

        return {
          isDisabled: false,
          listando: true,
          isValid: true,
          modo: "adicionar",
          metodo: 'POST',
          formProcedimento: {},
          erros: [],
          enums: {
            tipoProcedimento: [
              ..._enums.tipoProcedimento
            ],
            sexo: [
              { label: "Masculino", value: "M" },
              { label: "Feminino", value: "F" },
              { label: "Ambos", value: "*" }
            ]
          },
          validacoes: {
            quantidade: [
              { validator: validaQuantidade, trigger: ['blur', 'change', 'submit'] },
            ],
            codigo: [
              { required: true, validator: validaCodigo, trigger: ['blur', 'change', 'submit'] },
            ],
            descricao: [
              { required: true, validator: validaDescricao, trigger: ['blur', 'change', 'submit'] },
            ],
            tipo: [
              { required: true, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] },
            ],
            sexo: [
              { required: true, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }
            ],
            
          },
          api: {
            procedimentos: [],
          },
          loading: {
            procedimentos: false,
          },
          params: {
            skip: 1,
            take: 10,
            sort: 'Descricao ASC',
            total: 0
          },
        }
      },
      async mounted () {
        await this.getProcedimentos()
      },
      methods: {
        onEmitFromFiltro(val) {
          this.params = {
            ...this.params,
            ...val.params,
            skip: 1
          }
          this.listando = true
          this.getProcedimentos()
        },
        async getProcedimentos() {
          this.loading.procedimentos = true
          let { data, paginacao, status } = await _api.procedimentos.get(this.params)
          if(status === 502) this.loading.procedimentos = false;
          this.api.procedimentos = (status === 200) ? data : []
          this.params.skip = (status === 200) ? paginacao.skip : 0
          this.params.total = (status === 200) ? paginacao.totalCount : 0
          this.loading.procedimentos = false
        },
        onClickNovo() {
          this.isDisabled = false
          this.listando = false
          this.modo = 'adicionar'
          this.metodo = 'POST'
          this.formProcedimento = {
            ...this.formProcedimento,
            ativo: true
          }
        },
        onEditar(index, row) {
          this.modo = 'editar'
          this.metodo = 'PUT'
          this.isDisabled = false
          this.listando = false
          this.formProcedimento = {
            ...row
          }
        },
        onEditarCota(index, row) {
          this.modo = 'editarCota'
          this.metodo = 'PUT'
          this.isDisabled = false
          this.listando = false
          this.formProcedimento = {
            ...row
          }
        },
        async onClickSalvar (form) {
          this.erros = []
          this.loading.procedimentos = true
          this.$refs[form].validate((valid) => {
            if (valid) {
              if (this.metodo === 'POST') {
                _api.procedimentos.post(this.formProcedimento).then(res => {
                  if (res.status === 201) {
                    this.onListar(form)
                    this.getProcedimentos()
                    this.loading.procedimentos = false
                  }
                  this.loading.procedimentos = false
                })
              } else {
                if(this.modo === 'editar') {
                  var procedimento = this._.clone(this.formProcedimento)
                  _api.procedimentos.put(this.formProcedimento).then(res => {
                    if (res.status === 204) {
                      this.$swal({
                        title: "Sucesso!",
                        text: 'O procedimento foi editado com sucesso!',
                        icon: 'success',
                      })
                      let i = this.api.procedimentos.findIndex(x => x.id === procedimento.id)
                      this.api.procedimentos[i] = procedimento
                      this.$refs.tabela.setCurrentRow(procedimento)
                      this.onListar(form)
                    } else {
                      res.data.forEach(i => {
                        this.erros.push(i)
                      })
                      jQuery('.form--procedimento').get(0).scrollIntoView()
                    }
                    this.loading.procedimentos = false
                  })
                } else {
                  // Adicionar Cota
                  var procedimento = this._.clone(this.formProcedimento)
                  _api.procedimentosCota.put(procedimento.id, procedimento.quantidade).then(res => {
                    if (res.status === 204) {
                      let i = this.api.procedimentos.findIndex(x => x.id === procedimento.id)
                      this.api.procedimentos[i] = procedimento
                      this.$refs.tabela.setCurrentRow(procedimento)
                      this.onListar(form)
                      this.getProcedimentos()
                      this.$swal({
                        title: "Sucesso!",
                        text: 'A cota foi adicionado ao procedimento com sucesso!',
                        icon: 'success',
                      })
                    } else {
                      res.data.forEach(i => {
                        this.erros.push(i)
                      })
                      jQuery('.form--procedimento').get(0).scrollIntoView()
                    }
                    this.loading.procedimentos = false
                  })
                }
              }
            } else {
              this.$swal({
                title: "Atenção!",
                text: 'Verifique o preenchimento do formulário!',
                icon: 'warning',
              })
              this.loading.procedimentos = false
              jQuery('.el-form-item__error').get(0).scrollIntoView()
              return false
            }
          })
          this.loading.procedimentos = false
        },
        resetForm(form) {
          this.$refs[form].resetFields()
        },
        onListar(form) {
          let i = this.api.procedimentos.findIndex(x => x.id === this.formProcedimento.id)
          this.$refs.tabela.setCurrentRow(this.api.procedimentos[i])
          this.$refs[form].resetFields()
          this.listando = true
        },
        handleSizeChange (val) {
          this.params.take = val
          this.getProcedimentos()
        },
        handleCurrentChange (val) {
          this.params.skip = val
          this.getProcedimentos()
        }
      }
  }
</script>
