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
              <el-button v-if="listando" style="margin-right: -10px" icon="fas fa-plus" type="success" @click="onClickNovo"> Novo</el-button>
              <!--<el-button v-if="!listando" style="margin-right: -10px" icon="fas fa-undo-alt" type="warning" @click="onListar('formEstabelecimento')"> Voltar</el-button>-->
            </el-form-item>
          </el-form>
        </el-col>
      </el-row>

      <el-row v-show="listando">
        <el-col :span="24">
          <FiltroEstabelecimento :loading="loading.estabelecimentos" :params="params" @emit="onEmitFromFiltro" />
        </el-col>
      </el-row>

      <el-row v-show="listando && api.estabelecimentos.length > 0">
        <el-col :span="24">
          <el-table ref="tabela" :data="api.estabelecimentos"
                    highlight-current-row border
                    v-loading.body="loading.estabelecimentos"
                    :row-class-name="tableRow"
                    class="table--estabelecimentos table--row-click">
            <el-table-column label="Nome Fantasia" prop="nomeFantasia" fixed />
            <el-table-column label="Ativo" prop="ativo">
              <template slot-scope="scope">
                <span>{{ scope.row.ativo == true ? 'SIM' : 'NÃO' }}</span>
              </template>
            </el-table-column>

            <el-table-column v-if="isDemandaEspontanea()" label="CNES" prop="cnes" width="180">
              <template slot-scope="scope">
                <span :style="{ color: scope.row.cnes ? '#606266' : '#FF0000' }">{{ scope.row.cnes ? scope.row.cnes : 'Não cadastrado' }}</span>
              </template>
            </el-table-column>

            <el-table-column v-if="!isDemandaEspontanea()" label="CNPJ" prop="cnpj" width="180">
              <template slot-scope="scope">
                <span :style="{ color: scope.row.cnpj ? '#606266' : '#FF0000' }">{{ scope.row.cnpj ? scope.row.cnpj : 'Não cadastrado' }}</span>
              </template>
            </el-table-column>

            <el-table-column label="Logradouro" prop="logradouro" />
            <el-table-column header-align="center" align="right" width="140" fixed="right">
              <template slot-scope="scope">
                <el-dropdown>
                  <el-button type="primary" size="small">
                    Ações <i class="el-icon-arrow-down el-icon--right"></i>
                  </el-button>
                  <el-dropdown-menu slot="dropdown">
                    <ul class="list-unstyled">
                      <li v-show="scope.row.ativo == true" @click="onVincularProfissional(scope.row)" class="el-dropdown-menu__item">
                        <i class="fas fa-calendar-plus"></i> Vincular Profissional
                      </li>
                      <li v-show="scope.row.ativo == true" @click="onProcedimentos(scope.row)" class="el-dropdown-menu__item">
                        <i class="fas fa-calendar-plus"></i> Procedimentos
                      </li>
                      <li v-show="scope.row.ativo == true" @click="onCotas(scope.row)" class="el-dropdown-menu__item">
                        <i class="fas fa-exchange-alt"></i> Distribuir Cotas
                      </li>
                      <li v-show="scope.row.ativo == true" @click="onHorarios(scope.row)" class="el-dropdown-menu__item">
                        <i class="fas fa-clock"></i> Horários
                      </li>
                      <li @click="onEditar(scope.$index, scope.row)" class="el-dropdown-menu__item"><i class="fas fa-user-edit"></i> Editar</li>
                      <el-popconfirm v-if="mxHasAccess('Administrador','GestorMunicipio')"
                                     popper-class="forms--dropdown--popover"
                                     confirmButtonText='Sim, pode desativar'
                                     confirmButtonType="danger"
                                     cancelButtonType="primary"
                                     cancelButtonText='Não'
                                     icon="el-icon-info"
                                     iconColor="red"
                                     @confirm="onExcluir(scope.row)"
                                     title="Você tem certeza que deseja desativar o estabelecimento?">
                        <li v-show="scope.row.ativo == true" slot="reference" class="el-dropdown-menu__item text-danger"><i class="fas fa-trash"></i> Desativar</li>
                        <li @click="onAtivar(scope.row)" v-show="!scope.row.ativo" slot="reference" class="el-dropdown-menu__item text-success"><i class="fas fa-check-circle"></i>Ativar</li>

                      </el-popconfirm>
                    </ul>
                  </el-dropdown-menu>
                </el-dropdown>
              </template>
            </el-table-column>
          </el-table>
        </el-col>

        <el-col :span="24" v-show="listando">
          <el-pagination @size-change="handleSizeChange"
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

        <el-form :model="formEstabelecimento" status-icon :rules="validacoes" :disabled="isDisabled"
                 ref="formEstabelecimento" label-width="120px" label-position="top" class="form--estabelecimento">

          <el-row :gutter="24">
            <el-col :xs="24" :sm="24" :md="12" :lg="9" :xl="9">
              <el-form-item label="Razão Social" prop="nomeFantasia">
                <el-input v-model="formEstabelecimento.nomeFantasia" minlength="10" maxlength="50" placeholder="Digite a razão social"/>
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="5">
              <el-form-item label="CNES" prop="cnes">
                <el-input v-model="formEstabelecimento.cnes" minlength="7" maxlength="15" placeholder="Digite o CNES" />
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="5">
              <el-form-item label="INE" prop="codIne">
                <el-input v-model="formEstabelecimento.codIne" placeholder="Digite o código INE"/>
              </el-form-item>
            </el-col>          
            <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="5">
              <el-form-item label="CNPJ" prop="cnpj">
                <el-input v-model="formEstabelecimento.cnpj" v-mask="'##.###.###/####-##'" maxlength="18" placeholder="Digite o CNPJ"/>
              </el-form-item>
            </el-col>              
          </el-row>

          <el-row :gutter="24">
            <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
              <el-form-item label="CEP" prop="logradouroCep">
                <el-input v-model="formEstabelecimento.logradouroCep" @input="getCep" masked="true" maxlength="9" v-mask="'#####-###'" placeholder="Digite o CEP"/>
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
              <el-form-item label="Estado" prop="ufAbreviado">
                <el-select filterable placeholder="Selecione o Estado" v-model="formEstabelecimento.ufAbreviado"
                           v-loading.body="loading.ufs" @change="onSelectUf">
                  <el-option v-for="option in api.ufs" :value="option.ufAbreviado" :label="option.ufExtenso" :key="option.ufAbreviado" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
              <el-form-item label="Cidade" prop="cidadeId">
                <el-select filterable placeholder="Selecione a Cidade" v-model="formEstabelecimento.cidadeId"
                           v-loading.body="loading.cidades">
                  <el-option v-for="option in api.cidades" :value="option.ibge" :label="option.nome" :key="option.ibge" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
              <el-form-item label="Bairro" prop="bairro">
                <el-input v-model="formEstabelecimento.logradouroBairro" minlength="5" maxlength="30" placeholder="Digite o bairro"/>
              </el-form-item>
            </el-col>
          </el-row>

          <el-row :gutter="24">
            <el-col :xs="24" :sm="24" :md="12" :lg="11" :xl="11">
              <el-form-item label="Endereço" prop="logradouro">
                <el-input v-model="formEstabelecimento.logradouro" minlength="10" maxlength="60" placeholder="Digite o logradouro"/>
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="4" :xl="4">
              <el-form-item label="Número" prop="numero">
                <el-input v-model="formEstabelecimento.logradouroNumero" minlength="1" maxlength="10" placeholder="Digite o número"/>
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="7" :xl="7">
              <el-form-item label="Complemento" prop="complemento">
                <el-input v-model="formEstabelecimento.logradouroComplemento" placeholder="Digite o complemento"/>
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="1" :xl="1">
              <el-form-item prop="ativo" class="forms--label-no-title">
                <el-checkbox v-model="formEstabelecimento.ativo" label="Ativo" border />
              </el-form-item>
            </el-col>
          </el-row>

          <el-row :gutter="20">
            <el-col :xs="24">
              <el-form-item>
                <el-button flat icon="fas fa-save" type="success" :disabled="!isValid" v-if="mxHasAccess('Administrador', 'Atendente', 'Recepcionista')" @click="onClickSalvar('formEstabelecimento')" v-loading.body="loading.estabelecimentos"> Salvar</el-button>
                <el-button flat icon="fas fa-undo-alt" type="warning" @click="onListar('formEstabelecimento')" :disabled="loading.estabelecimentos"> Voltar</el-button>
                <el-button flat icon="fas fa-eraser" v-if="metodo === 'POST'" type="default" @click="resetForm('formEstabelecimento')" :disabled="loading.estabelecimentos"> Limpar</el-button>
              </el-form-item>
            </el-col>
          </el-row>

        </el-form>

      </el-row>
    </el-card>

  </el-col>
</template>

<style>
  .el-table .warning-row {
    background: #FFDCDC;
  }

  .el-table .success-row {
    background: #D5FED3;
  }
</style>

<script>
  import Utils from '../../../mixins/Utils'
  import _api from '../../../api'
  import _enums from '../../../api/Enums'
  import FiltroEstabelecimento from '../../../components/shared/FiltroEstabelecimento'
  import { mask } from 'vue-the-mask'
  export default {
    name: 'Estabelecimento',
    mixins: [Utils],
    components: { FiltroEstabelecimento },
    directives: { mask },
    data () {
      //var validaCpf = (rule, value, callback) => {
      //  if (!value) { return callback(new Error('Campo obrigatorio')) }
      //  value = value ? value.replace(/[.-\s]/g, '') : null
      //  if (!value.match(/^[0-9]+$/)) return callback(new Error('Cpf só pode conter números'))
      //  if (!value) return callback(new Error('Cpf Obrigatório'))
      //  if (this.mxValidaCPF(value) === false) {
      //    return callback(new Error('Cpf Inválido'))
      //  } else {
      //    callback()
      //  }
      //}

      //var validaEndereco = (rule, value, callback) => {
      //  if (!value) return callback()
      //  if (this.mxValidaComplemento(value) === false) {
      //    return callback(new Error('O campo pode conter apenas letras e números'))
      //  } else {
      //    callback()
      //  }
      //}

      var validaNome = (rule, value, callback) => {
        if (!value) { return callback(new Error('Campo obrigatorio')) }
        if (this.mxValidaNome(value) === false) {
          return callback(new Error('O campo pode conter apenas letras'))
        } else {
          callback()
        }
      }

      //var validaNumero = (rule, value, callback) => {
      //  if (!value) { return callback(new Error('Campo obrigatorio')) }
      //  if (this.mxValidaNumero(value) === false) {
      //    return callback(new Error('O campo pode conter apenas Numeros'))
      //  } else {
      //    callback()
      //  }
      //}

      //var validaCNPJ = (rule, value, callback) => {
      //  if (!value) { return callback(new Error('Campo obrigatorio')) }
      //  if (value < 6) return callback(new Error('O campo pode conter mais que 7(sete) números'))
      //  if (this.mxValidaCnpj(value) === false) {
      //    return callback(new Error('Não é permitido a utilização de letras e caracteres especiais, exceto . - /'))
      //  } else {
      //    callback()
      //  }
      //}

      var validaCnes = (rule, value, callback) => {
        if (!value) { return callback(new Error('Campo obrigatorio')) }
        if (value < 6) return callback(new Error('O campo pode conter mais que 7(sete) números'))
        if (this.mxValidaCnes(value) === false) {
          return callback(new Error('Não é permitido a utilização de letras e caracteres especiais, exceto . - /'))
        } else {
          callback()
        }
      }

      return {
        isDisabled: false,
        isValid: true,
        metodo: 'POST',
        listando: true,
        formEstabelecimento: {},
        validacoes: {

          nomeFantasia: [
            { required: true, validator: validaNome, trigger: ['blur', 'change', 'submit'] }
          ],

          cnes: [
            { required: true, validator: validaCnes, trigger: ['blur', 'change', 'submit'] }
          ],

          codIne: [{ required: true, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],

          // responsavelCpf: [
          //   { required: true, validator: validaCpf, trigger: ['blur', 'change', 'submit'] }
          // ],

          // tipoEmpresa: [
          //   { required: true, validator: validaNome, trigger: ['blur', 'change', 'submit'] }
          // ],

          // ramoAtividade: [
          //   { required: true, validator: validaNome, trigger: ['blur', 'change', 'submit'] }
          // ],

          ufAbreviado: [{ required: true, message: 'Campo obrigatório', trigger: ['blur', 'submit'] }],

          logradouroCep: [
            { required: true, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }
          ],

          cidadeId: [
            { required: true, message: 'Campo obrigatório', trigger: ['submit'] }
          ],

          logradouro: [
            { required: true, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }
          ]
          // numero: [
          //  { required: true, message: 'Campo obrigatório', trigger: ['submit'] },
          // ],
          // bairro: [
          //  { required: true, message: 'campo obrigatório', trigger: ['submit'] },
          // ]
        },
        enums: {},
        api: {
          estabelecimentos: [],
          ufs: [],
          cidades: []
        },
        loading: {
          estabelecimentos: false,
          ufs: false,
          cidades: false
        },
        params: {
          skip: 1,
          take: 10,
          sort: 'e.NomeFantasia ASC',
          total: 0
        },
        paramsUfs: {
          skip: 1,
          take: 30,
          total: 0
        },
        paramsCidades: {
          skip: 1,
          take: 999,
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
      // Ativar Estabelecimento:
      async onAtivar (row) {
        if (row != null) {
          let form = { ativo: true, id: row.id}

          await _api.estabelecimentos.delete(form)
          this.getEstabelecimentos()
        }
      },
      tableRow ({ row }) {
        if (row.ativo === true) {
          return 'success-row'
        } else if (row.ativo === false) {
          return 'warning-row'
        }
        return ''
      },
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
        this.loading.estabelecimentos = true
        let { data, paginacao, status } = await _api.estabelecimentos.get(this.params)
        if (status === 502) this.loading.estabelecimentos = false
        this.api.estabelecimentos = (status === 200) ? data : []
        this.params.skip = (status === 200) ? paginacao.skip : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.estabelecimentos = false
      },

      async onClickNovo () {
        this.isDisabled = false
        this.listando = false
        this.metodo = 'POST'
        this.formEstabelecimento = {
          ativo: true
        }
        this.$refs.formEstabelecimento.resetFields()
      },
      async onEditar (index, row) {
        this.metodo = 'PUT'
        this.isDisabled = false
        this.listando = false
        this.formEstabelecimento = {}

        this.formEstabelecimento = {
          ...row,
          logradouroNumero: row.numero,
          logradouroComplemento: row.complemento,
          logradouroCep: row.cep,
          ufAbreviado: row.uf,
          cidadeId: row.codIbgeMun
        }

        await this.onSelectUf(this.formEstabelecimento.ufAbreviado)
        await this.getCidadesByUf()
        let cidade = this.api.cidades.filter(cidades => {
          if (cidades.ibge == this.formEstabelecimento.codIbgeMun) {
            return cidades
          }
        })
        this.formEstabelecimento.cidadeId = cidade[0].nome
        this.formEstabelecimento.logradouro = row.logradouro
        this.formEstabelecimento.logradouroBairro = row.bairro
      },
      async onExcluir (row) {
        if (row != null) {
          let form = { ativo : false, id: row.id}
          await _api.estabelecimentos.delete(form)
          this.getEstabelecimentos()
        } 
      },
      onListar (form) {
        let i = this.api.estabelecimentos.findIndex(x => x.id === this.formEstabelecimento.id)
        this.$refs.tabela.setCurrentRow(this.api.estabelecimentos[i])
        this.$refs[form].resetFields()
        this.listando = true
      },

      onVincularProfissional (row) {
        this.$router.push({
          name: 'EstabelecimentoProcedimentoProfissional',
          params: { estabelecimento: row }
        })
      },

      async onClickSalvar (form) {
        if (this.formEstabelecimento.logradouro != '') {
          // passando objeto com os nomes certos para o model
          let formNovo = {
            id: this.formEstabelecimento.id,
            nomeFantasia: this.formEstabelecimento.nomeFantasia,
            ramoAtividade: this.formEstabelecimento.ramoAtividade,
            tipoEmpresa: this.formEstabelecimento.tipoEmpresa,
            ativo: this.formEstabelecimento.ativo,
            cnes: this.formEstabelecimento.cnes,
            codIne: this.formEstabelecimento.codIne,
            responsavelCpf: this.formEstabelecimento.responsavelCpf,
            tipo: 'E',
            logradouro: this.formEstabelecimento.logradouro,
            bairro: this.formEstabelecimento.logradouroBairro,
            complemento: this.formEstabelecimento.logradouroComplemento,
            numero: this.formEstabelecimento.logradouroNumero,
            cep: this.formEstabelecimento.logradouroCep,
            uf: this.formEstabelecimento.ufAbreviado,
            cnpj: this.formEstabelecimento.cnpj,
            deletado: false
          }

          this.loading.estabelecimentos = true
          this.$refs[form].validate(async (valid) => {
            if (valid) {
              formNovo.codIbgeMun = await this.getCodIbge(this.formEstabelecimento.logradouroCep)
              if (this.metodo === 'POST') {
                _api.estabelecimentos.post(formNovo).then(res => {
                  if (res.status === 201) {
                    this.onListar(form)
                    this.getEstabelecimentos()
                    location.reload()
                    this.loading.estabelecimentos = false
                  }
                  this.loading.estabelecimentos = false
                })
              } else {
                var estabelecimento = this._.clone(this.formEstabelecimento)

                _api.estabelecimentos.put(formNovo).then(res => {
                  if (res.status === 204) {
                    this.formEstabelecimento = {}
                    this.getEstabelecimentos()
                    let i = this.api.estabelecimentos.findIndex(x => x.id === estabelecimento.id)
                    this.api.estabelecimentos[i] = estabelecimento
                    this.$refs.tabela.setCurrentRow(estabelecimento)
                    this.onListar(form)
                  }
                  this.loading.estabelecimentos = false
                })
              }
            } else {
              this.$swal({
                title: "Atenção!",
                text: 'Verifique o preenchimento do formulário!',
                icon: 'warning',
              })             
              this.loading.estabelecimentos = false
            }
          })
        }

        this.loading.estabelecimentos = false
      },
      onProcedimentos (row) {
        this.$router.push({
          name: 'EstabelecimentoProcedimentos',
          params: { estabelecimento: row }
        })
      },
      onCotas (row) {
        this.$router.push({
          name: 'EstabelecimentoCotas',
          params: { estabelecimento: row }
        })
      },
      onHorarios (row) {
        this.$router.push({
          name: 'EstabelecimentoHorarios',
          params: { estabelecimento: row }
        })
      },
      async getCep (cep) {
        if (this.listando) return
        cep.replace(/[.-\s]/g, '')
        if (cep.length >= 8) {
          let { data, status } = await _api.auxiliar.getCep(cep)
          if (status === 200) {
            if (data.logradouro != '') {
              this.formEstabelecimento.logradouro = data.logradouro
            }
            if (data.bairro != '') {
              this.formEstabelecimento.logradouroBairro = data.bairro
            }

            // this.formEstabelecimento.logradouroBairro = data.bairro
            this.formEstabelecimento.ufAbreviado = data.uf
            this.paramsCidades.ibge = data.ibge
            await this.onSelectUf(data.uf)
            await this.getCidadesByUf()
          } else {
            this.paramsCidades.ibge = null
            this.formEstabelecimento = {
              ...this.formEstabelecimento,
              ufAbreviado: null,
              cidadeId: null,
              logradouro: null,
              bairro: null
            }
          }
        }
      },
      async getCodIbge(cep) {
        cep.replace(/[.-\s]/g, '')
        if (cep.length >= 8) {
          let { data, status } = await _api.auxiliar.getCep(cep)
          if (status === 200) {
            return data.ibge
          } else {
            console.log('ERRO AO BUSCAR COD IBGE')
          }
        }
      },

      async onSelectUf (val) {
        this.paramsCidades.ufAbreviado = val
        this.formEstabelecimento = {
          ...this.formEstabelecimento,
          cidadeId: null
        }
        await this.getCidadesByUf()
      },

      async getCidadesByUf () {
        this.loading.cidades = true
        let { data, paginacao, status } = await _api.cidades.get(this.paramsCidades)
        this.api.cidades = (status === 200) ? data : []
        if (this.api.cidades.length === 1) {
          this.formEstabelecimento.cidadeId = this.api.cidades[0].ibge
        }
        this.paramsCidades.skip = (status === 200) ? paginacao.skip : 0
        this.paramsCidades.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.cidades = false
      },

      async getUfs () {
        this.loading.ufs = true
        let { data, paginacao, status } = await _api.ufs.get(this.paramsUfs)
        this.api.ufs = (status === 200) ? data : []
        if (this.api.ufs.length === 1) {
          this.formEstabelecimento.ufAbreviado = this.api.ufs[0].ufAbreviado
          this.getCidadesByUf()
        }
        this.paramsUfs.skip = (status === 200) ? paginacao.skip : 0
        this.paramsUfs.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.ufs = false
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
        this.params.take = val
        this.getEstabelecimentos()
      },
      handleCurrentChange (val) {
        this.params.skip = val
        this.getEstabelecimentos()
      }
    }
  }
</script>
