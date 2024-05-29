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
              <!--<el-button v-if="!listando" style="margin-right: -10px" icon="fas fa-undo-alt" type="warning" @click="onListar('formProfissional')"> Voltar</el-button>-->
            </el-form-item>
          </el-form>
        </el-col>
      </el-row>

      <el-row v-show="listando">
        <el-col :span="24">
          <FiltroProfissional :loading="loading.profissionais" :params="params" @emit="onEmitFromFiltro" />
        </el-col>
      </el-row>
      <el-row v-show="listando && api.profissionais.length > 0">
        <el-col :span="24">
          <el-table ref="tabela" :data="api.profissionais"
                    highlight-current-row border
                    v-loading.body="loading.profissionais"
                    class="table--profissionais table--row-click">
            <el-table-column label="Nome" prop="nome" fixed />
            <el-table-column label="Ativo" prop="ativo" width="150">
              <template slot-scope="scope">
                <span>{{ scope.row.ativo == true ? 'SIM' : 'NÃO' }}</span>
              </template>
            </el-table-column>
            <el-table-column label="CNS" prop="cns" width="180">
              <template slot-scope="scope">
                <span :style="{ color: scope.row.cns ? '#606266' : '#FF0000' }">{{ scope.row.cns ? scope.row.cns : 'Não cadastrado' }}</span>
              </template>
            </el-table-column>
            <el-table-column label="Nº do Conselho" prop="crm" />
            <el-table-column header-align="center" align="right" width="140" fixed="right">

              <template slot-scope="scope">
                <el-dropdown>
                  <el-button type="primary" size="small">
                    Ações <i class="el-icon-arrow-down el-icon--right"></i>
                  </el-button>
                  <el-dropdown-menu slot="dropdown">
                    <ul class="list-unstyled">
                      <li @click="onEditar(scope.$index, scope.row)" class="el-dropdown-menu__item"><i class="fas fa-user-edit"></i> Editar</li>
                      <li @click="onDesativar(scope.row)" v-show="scope.row.id !== $auth.user().id && scope.row.ativo === true" class="el-dropdown-menu__item text-danger"><i class="fas fa-trash"></i> Desativar</li>
                      <li @click="onAtivar(scope.row)" v-show="!scope.row.ativo" slot="reference" class="el-dropdown-menu__item text-success"><i class="fas fa-check-circle"></i>Ativar</li>

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
        <el-form :model="formProfissional" status-icon :rules="validacoes"
                 ref="formProfissional" label-width="120px" label-position="top" class="form--estabelecimento">

          <el-row :gutter="24">

            <el-col :xs="24" :sm="24" :md="12" :lg="4" :xl="4">
              <el-form-item label="CPF" prop="cpf">
                <el-input v-model="formProfissional.cpf" v-mask="'###.###.###-##'" maxlength="14" placeholder="Digite o cpf"/>
              </el-form-item>
            </el-col>

            <el-col :xs="24" :sm="24" :md="12" :lg="4" :xl="4">
              <el-form-item label="CNS" prop="cns">
                <el-input v-model="formProfissional.cns" v-mask="'###############'" maxlength="15" placeholder="Digite o cns"/>
              </el-form-item>
            </el-col>

            <el-col :xs="24" :sm="24" :md="12" :lg="4" :xl="4">
              <el-form-item label="Nº do Conselho" prop="crm">
                <el-input v-model="formProfissional.crm" minlength="5" maxlength="6" placeholder="Digite o número do conselho"/>
              </el-form-item>
            </el-col>

            <el-col :xs="24" :sm="24" :md="12" :lg="4" :xl="4">
              <el-form-item label="Estado Emissor" prop="crmUF">
                <el-select filterable placeholder="Selecione o Estado" v-model="formProfissional.crmUF"
                           v-loading.body="loading.ufs" :disabled="loading.cidades" @change="onSelectUf">
                  <el-option v-for="option in api.ufs" :value="option.ufAbreviado" :label="option.ufExtenso" :key="option.ufAbreviado" />
                </el-select>
              </el-form-item>
            </el-col>

            <el-col :xs="24" :sm="24" :md="12" :lg="4" :xl="4">
              <el-form-item label="Tipo Conselho" prop="conselho">
                <el-select value-key="claimValue" placeholder="Selecione o perfil" v-model="formProfissional.conselho">
                  <el-option v-for="(option, i) in enums.tipoConselho" :value="option.value" :label="option.label" :key="`${i+1}`" />
                </el-select>
              </el-form-item>
            </el-col>

            <el-col :xs="24" :sm="24" :md="12" :lg="4" :xl="4">
              <el-form-item label="Perfis" prop="userClaims">
                <el-select v-if="mxHasAccess('Administrador')" clearable collapse-tags multiple value-key="claimValue" placeholder="Selecione o perfil" v-model="formProfissional.userClaims">
                  <el-option v-if="$config.configDB.modulo === 5" v-for="(option, i) in enums.claimsMedicoConde" :value="option" :label="option.claimValue" :key="`${i+1}`" />
                  <el-option v-if="$config.configDB.modulo === 3" v-for="(option, i) in enums.claimsMedico" :value="option" :label="option.claimValue" :key="`${i+1}`" />
                </el-select>
              </el-form-item>
            </el-col>

          </el-row>

          <el-row :gutter="24">
            <el-col :xs="24" :sm="24" :md="12" :lg="metodo === 'POST' ? 5 : 7" :xl="metodo === 'POST' ? 5 : 7" >
              <el-form-item label="Nome" prop="nome">
                <el-input v-model="formProfissional.nome" placeholder="Digite o nome"/>
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="3" :xl="3">
              <el-form-item label="Sobrenome" prop="sobrenome" v-if=alterarNome>
                <el-input v-model="formProfissional.sobrenome" :disabled="!alterarNome" placeholder="Digite o sobrenome"/>
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="4" :xl="4">
              <el-form-item label="Telefone" prop="telefone">
                <el-input v-model="formProfissional.telefone" placeholder="Digite o telefone celular"/>
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="metodo === 'POST' ? 6 : 8" :xl="metodo === 'POST' ? 6 : 8">
              <el-form-item label="E-mail" prop="email">
                <el-input v-model="formProfissional.email" placeholder="Digite o email"/>
              </el-form-item>
            </el-col>

            <el-col :xs="24" :sm="24" :md="12" :lg="metodo === 'POST' ? 6 : 8" :xl="metodo === 'POST' ? 6 : 8">
              <el-form-item label="Ocupação" prop="ocupacaoId">
                <el-select v-if="mxHasAccess('Administrador')" placeholder="Selecione a ocupação" v-model="formProfissional.ocupacaoId" filterable>
                  <el-option v-for="option in api.cboEsus" :value="option.id" :label="`${option.descricao} (CBO: ${option.codigo})`" :key="option.id" />
                </el-select>
              </el-form-item>
            </el-col>

          </el-row>

          <el-row :gutter="24">

            <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
              <el-form-item label="Data de Nascimento" prop="dataNascimento">
                <el-date-picker prefix-icon="fas fa-calendar-day" :picker-options="pickerOptions" v-model="formProfissional.dataNascimento" type="date" format="dd-MM-yyyy" v-mask="'##-##-####'" placeholder="Digite a data do nascimento"/>
              </el-form-item>
            </el-col>

            <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
              <el-form-item label="Usuário" prop="username">
                <el-input v-model="formProfissional.cpf" :disabled="metodo === 'PUT' || metodo === 'POST'"/>
              </el-form-item>
            </el-col>

            <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
              <el-form-item label="Senha" v-if="!mxHasAccess('Medico')" class="forms--checkbox-right" prop="senha">
                <el-checkbox v-model="alterarSenha" @change="onAlterarSenha" :disabled="mxHasAccess('Médico')" v-if="metodo === 'PUT'">Alterar Senha</el-checkbox>
                <el-input v-model="formProfissional.senha" type="password" :disabled="!alterarSenha" placeholder="Digite a senha"/>
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
              <el-form-item label="Confirmação" v-if="!mxHasAccess('Medico')" prop="senhaConfirmacao">
                <el-input v-model="formProfissional.senhaConfirmacao" type="password" :disabled="!alterarSenha" placeholder="Confirme a senha"/>
              </el-form-item>
            </el-col>

          </el-row>

          <el-row :gutter="24">
            <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
              <el-form-item label="Sexo" prop="sexo">
                <el-select v-model="formProfissional.sexo" clearable placeholder="Selecione o sexo">
                  <el-option v-for="item in enums.sexos" :label="item.label" :value="item.value" :key="item.value" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
              <el-form-item label="CEP" prop="logradouroCep">
                <el-input v-model="formProfissional.logradouroCep" @input="getCep" masked="true" maxlength="9" v-mask="'#####-###'" placeholder="Digite o CEP"/>
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
              <el-form-item label="Estado" prop="uf">
                <el-select filterable placeholder="Selecione o Estado" v-model="formProfissional.ufAbreviado"
                           v-loading.body="loading.ufs" :disabled="loading.cidades" @change="onSelectUf">
                  <el-option v-for="option in api.ufs" :value="option.ufAbreviado" :label="option.ufExtenso" :key="option.ufAbreviado" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
              <el-form-item label="Cidade" prop="cidadeId">
                <el-select filterable placeholder="Selecione a Cidade" v-model="formProfissional.cidadeId"
                           v-loading.body="loading.cidades">
                  <el-option v-for="option in api.cidades" :value="option.ibge" :label="option.nome" :key="option.ibge" />
                </el-select>
              </el-form-item>
            </el-col>

          </el-row>

          <el-row :gutter="24">
            <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
              <el-form-item label="Bairro" prop="logradouroBairro">
                <el-input v-model="formProfissional.logradouroBairro" placeholder="Digite o bairro"/>
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="8" :xl="8">
              <el-form-item label="Endereço" prop="logradouro">
                <el-input v-model="formProfissional.logradouro" placeholder="Digite o logradouro"/>
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="3" :xl="3">
              <el-form-item label="Número" prop="logradouroNumero">
                <el-input v-model="formProfissional.logradouroNumero" placeholder="Número do imovel"/>
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="5">
              <el-form-item label="Complemento" prop="complemento">
                <el-input v-model="formProfissional.logradouroComplemento" placeholder="Digite o complemento"/>
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="1" :xl="1">
              <el-form-item prop="ativo" class="forms--label-no-title">
                <el-checkbox v-model="formProfissional.ativo" label="Ativo" border />
              </el-form-item>
            </el-col>
          </el-row>

          <el-row :gutter="20">
            <el-col :xs="24">
              <el-form-item>
                <!-- <el-button flat icon="fas fa-save" type="success" :disabled="!isValid" v-if="mxHasAccess('Administrador', 'Atendente', 'Recepcionista')" @click="onClickSalvar('formProfissional')" v-loading.body="loading.profissionais"> Salvar</el-button> -->
                <el-button flat icon="fas fa-save" type="success" v-if="mxHasAccess('Administrador', 'Atendente', 'Recepcionista')" @click="onClickSalvar('formProfissional')"> Salvar</el-button>
                <el-button flat icon="fas fa-undo-alt" type="warning" @click="onListar('formProfissional')" :disabled="loading.profissionais"> Voltar</el-button>
                <el-button flat icon="fas fa-eraser" v-if="metodo === 'POST'" type="default" @click="resetForm('formProfissional')" :disabled="loading.profissionais"> Limpar</el-button>
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
  import jQuery from 'jquery'
  import _api from '../../../api'
  import _enums from '../../../api/Enums'
  import FiltroProfissional from '../../../components/shared/FiltroProfissional'
  import { mask } from 'vue-the-mask'
  import { Notification } from 'element-ui'

  export default {
    name: 'AdminProfissional',
    mixins: [Utils],
    components: { FiltroProfissional },
    directives: { mask },
    data () {
      var validaCpf = (rule, value, callback) => {
        if (!this.formProfissional.cpf) return callback(new Error('CPF Obrigatório'))
        if (this.mxValidaCPF(this.formProfissional.cpf) === false) {
          return callback(new Error('CPF Inválido'))
        } else {
          callback()
        }
      }

      var validaSenha = (rule, value, callback) => {
        var pattern = '^(?=.*[0-9])(?=.*[a-z]).{8,}'
        if (value === undefined || value.trim() === '') return callback(new Error('O campo senha é obrigatório'))
        if (value.length < 8) return callback(new Error('O campo senha é obrigatório e no minimo 8 caracteres'))
        if (value.match(pattern)) return callback()
        else return callback(new Error('A senha deve conter ao menos 1 letra e 1 número e conter no mínimo 8 caracteres.'))
      }

      var validaSenhaConfirmacao = (rule, value, callback) => {
        if (value === undefined) {
          return callback(new Error('Campo obrigatorio'))
        } else if (value !== this.formProfissional.senha) {
          return callback(new Error('A senha não confere.'))
        } else {
          return callback()
        }
      }

      var validaCns = (rule, value, callback) => {
        if (!value) { return callback() }
        if (this.mxValidaCnpj(value) === false) {
          return callback(new Error('O campo pode conter apenas números'))
        } else {
          callback()
        }
      }

      var validaCRM = (rule, value, callback) => {
        if (!value) { return callback(new Error('Campo obrigatorio')) }
        if (value.length < 4) { return callback(new Error('O campo deve conter no mínimo 4 caracteres do CRM')) }
        if (this.mxValidaCRM(value) === false) {
          return callback(new Error('O campo pode conter apenas caracteres do CRM'))
        } else {
          callback()
        }
      }

      var validaNome = (rule, value, callback) => {
        if (!value) { return callback(new Error('Campo obrigatorio')) }
        if (this.mxValidaNome(value) == false) {
          return callback(new Error('O campo pode conter apenas letras'))
        } else {
          callback()
        }
      }

      return {
        isValid: true,
        metodo: 'POST',
        metodoModelo: 'PUT',
        listando: true,
        auxNome: '',
        auxSobrenome: '',
        formProfissional: {},
        pickerOptions: {
          disabledDate: this.getDisabledDates
        },
        validacoes: {
          cpf: [
            { required: true, validator: validaCpf, trigger: ['blur', 'change', 'submit'] },
            { required: true, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }
          ],

          cns: [
            { validator: validaCns, trigger: ['blur', 'change', 'submit'] }
          ],

          dataNascimento: [
            { required: false, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }
          ],

          sexo: [
            { required: false, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }
          ],

          telefone: [
            { required: true, validator: this.mxValidaCelularIndividuo, trigger: ['blur', 'change', 'submit'] }
          ],

          crm: [
            { required: true, validator: validaCRM, trigger: ['blur', 'change', 'submit'] }
          ],

          crmUF: [{ required: true, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],

          conselho: [{ required: true, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],

          nome: [
            { required: true, validator: validaNome, trigger: ['blur', 'change', 'submit'] }
          ],

          sobrenome: [
            { required: true, validator: validaNome, trigger: ['blur', 'change', 'submit'] }
          ],

          email: [
            { required: true, message: 'Campo Obrigatório', trigger: ['blur', 'change', 'submit'] },
            { type: 'email', message: 'Email inválido', trigger: ['blur', 'change', 'submit'] }
          ],

          senha: [
            { required: true, validator: validaSenha, trigger: ['submit'] }
          ],

          senhaConfirmacao: [{ required: true, validator: validaSenhaConfirmacao, trigger: ['submit'] }],

          ufAbreviado: [{ required: false, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],

          cidadeId: [{ required: false, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],

          logradouroCep: [
            { required: false, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }
          ],

          ocupacaoId: [{ required: true, message: 'Campo obrigatório', trigger: ['submit'] }],

          logradouro: [
            { required: false, message: 'Campo obrigatório', trigger: ['submit'] },
            { min: 1, max: 150, message: 'Endereço não pode conter menos de 1 e mais que 150 caracteres', trigger: 'change' }
          ],

          logradouroNumero: [
            { required: false, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }
          ],

          logradouroBairro: [
            { required: false, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] },
            { min: 1, max: 100, message: 'Bairro não pode conter menos de 1 e mais que 100 caracteres', trigger: 'change' }
          ],

          userClaims: [{ required: true, message: 'Campo obrigatório', trigger: ['submit'] }]
        },
        enums: {
          sexos: _enums.sexos,
          genero: _enums.genero,
          perfis: _enums.perfis,
          claimsMedico: _enums.claimsMedico,
          claimsMedicoConde: _enums.claimsMedicoConde,
          tipoConselho: _enums.tipoConselho
        },
        api: {
          profissionais: [],
          ufs: [],
          cidades: [],
          cboEsus: []
        },
        loading: {
          profissionais: false,
          ufs: false,
          cidades: false
        },
        params: {
          skip: 1,
          take: 10,
          sort: 'Nome ASC',
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
        },
        storeSenha: '',
        alterarSenha: true,
        alterarNome: false
      }
    },
    async mounted () {
      await this.getProfissionais()
      await this.getUfs()
      await this.getCbos()
    },
    methods: {
      async onClickNovo () {
        this.alterarSenha = true
        this.listando = false
        this.alterarNome = true
        this.metodo = 'POST'
        this.formProfissional = {
          ativo: true
        }
        this.$refs.formProfissional.resetFields()
      },

      async onEditar (index, row) {
        this.metodo = 'PUT'
        this.listando = false
        this.alterarNome = false
        this.alterarSenha = false
        this.getCep(row.logradouroCep)
        this.formProfissional = {
          ...row,
          senhaConfirmacao: row.senha
        }
        this.storeSenha = this.formProfissional.senha.slice()
      },

      async onDesativar (row) {
        this.$swal({
          title: 'Desativar Profissional',
          text: `Você tem certeza que deseja desativar o Profissional: ${row.nome}?`,
          icon: 'warning',
          showCloseButton: true,
          confirmButtonText: 'Sim',
          showCancelButton: true,
          cancelButtonText: 'Cancelar'
        }).then(async (result) => {
          if (result.isConfirmed) {
            await _api.profissionais.delete(row.id)
            this.getProfissionais()

            this.$swal({
              title: 'Sucesso!',
              text: 'O profissional foi desativado com sucesso!',
              icon: 'success'
            })
          }
        }).catch((result) => {

        })
      },

      // Ativar Profissional:
      async onAtivar (row) {
        if (row != null) {
          let form = {}

          row.ativo = true
          row.cadastroEditado = true
          row.descricaoEditado = 'Ativar Usuario'
          row.usuarioEditou = row.usuarioEditou = this.$auth.user().id

          form = { ...row }

          await _api.profissionais.putAtivar(form)
          this.getProfissionais()
        }
      },

      getDisabledDates (date) {
        return date >= Date.now()
      },

      onAlterarSenha () {
        if (this.alterarSenha) {
          this.formProfissional.senhaAlterada = true
          this.formProfissional.senha = ''
          this.formProfissional.senhaConfirmacao = ''
        } else {
          this.formProfissional.senha = this.storeSenha
          this.formProfissional.senhaConfirmacao = this.storeSenha
        }
      },

      async addProfissionalMemed (params) {
        _api.memed.post(params).then(res => {
          if (res.status === 200) {
            try {
              let result = JSON.parse(res.data)
              const { external_id, token } = result.data.attributes
              if (result != null) {
                let obj = {
                  id: external_id,
                  token: token
                }
                _api.profissionais.addToken(obj)
              }
            } catch (e) {
              //   console.log('ERRO TOKEN', e)
            }
          }
        }).catch(e => {
          //  console.log('erro memed', e)
        })
      },

      onEmitFromFiltro (val) {
        this.params = {
          ...this.params,
          ...val.params,
          skip: 1
        }
        this.listando = true
        this.getProfissionais()
      },

      async getProfissionais () {
        this.loading.profissionais = true
        let { data, paginacao, status } = await _api.profissionais.get(this.params)
        if (status === 502) this.loading.profissionais = false
        this.api.profissionais = (status === 200) ? data : []
        this.params.skip = (status === 200) ? paginacao.skip : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.profissionais = false
      },

      async getUfs () {
        this.loading.ufs = true
        let { data, paginacao, status } = await _api.ufs.get(this.paramsUfs)
        this.api.ufs = (status === 200) ? data : []
        if (this.api.ufs.length === 1) {
          this.formProfissional.ufAbreviado = this.api.ufs[0].ufAbreviado
          this.getCidadesByUf()
        }
        this.paramsUfs.skip = (status === 200) ? paginacao.skip : 0
        this.paramsUfs.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.ufs = false
      },
      async getCidadesByUf () {
        this.loading.cidades = true
        let { data, paginacao, status } = await _api.cidades.get(this.paramsCidades)
        this.api.cidades = (status === 200) ? data : []
        if (this.api.cidades.length === 1) {
          this.formProfissional.cidadeId = this.api.cidades[0].ibge
        }
        this.paramsCidades.skip = (status === 200) ? paginacao.currentPage : 0
        this.paramsCidades.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.cidades = false
      },

      async onSelectUf (val) {
        this.paramsCidades.ufAbreviado = val
        this.formProfissional = {
          ...this.formProfissional,
          cidadeId: null
        }
        await this.getCidadesByUf()
      },

      onListar (form) {
        let i = this.api.profissionais.findIndex(x => x.id === this.formProfissional.id)
        this.$refs.tabela.setCurrentRow(this.api.profissionais[i])
        this.$refs[form].resetFields()
        this.listando = true
      },

      async onClickSalvar (form) {
        if (this.metodo == 'PUT') {
          this.$prompt('', 'Alteração de dados cadastrados', {
            confirmButtonText: 'Confirmar',
            cancelButtonText: 'Cancelar',
            inputPattern: /^[A-Za-záàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ ]+$/,
            inputErrorMessage: 'O campo pode conter apenas letras'
          }).then(({ value }) => {
            this.formProfissional = {
              ...this.formProfissional,
              imagem: null,
              descricaoEditado: value,
              cadastroEditado: true,
              usuarioEditou: this.$auth.user().id
            }
            this.formProfissional.descricaoEditado = value
            this.formProfissional.cadastroEditado = true
            this.formProfissional.usuarioEditou = this.$auth.user().id
            // console.log('this.formProfissional', this.formProfissional)
            this.Salvar(form)
          }).catch(() => {

          })
        } else {
          this.Salvar(form)
        }
      },

      Salvar (form) {
        this.loading.profissionais = true

        var userClaims = []
        if (this.formProfissional.userClaims) {
          this.formProfissional.userClaims.forEach(claimObj => {
            userClaims.push(claimObj.claimValue)
          })
        }
  
        this.$refs[form].validate((valid) => {
          if (valid) {
            if (this.metodo === 'POST') {
              let nomeMemed = this.formProfissional.nome
              let newCpf = this.formProfissional.cpf.match(/\d/g).join('')
              let sobrenomeMemed = this.formProfissional.sobrenome
              let nomeCompleto = this.formProfissional.nome + ' ' + this.formProfissional.sobrenome
              // this.formProfissional.nome + ' ' + this.formProfissional.sobrenome
              this.formProfissional.cpf = newCpf
              this.formProfissional.username = newCpf
              // console.log('nomeCompleto: ', nomeCompleto)
              // console.log('this.formProfissional: ', this.formProfissional)
              this.formProfissional.nome = this.formProfissional.nome
              this.formProfissional.sobrenome = this.formProfissional.sobrenome

              let claims = []
              this.formProfissional.userClaims.forEach(item => {
                claims.push({
                  id: this.mxGerarGuid(),
                  claimType: item.claimType,
                  claimValue: item.claimValue
                })
              })
              this.formProfissional.userClaims = claims
              this.auxNome = this.formProfissional.nome
              this.auxSobrenome = this.formProfissional.sobrenome

              let test = {
                ...this.formProfissional,
                nome: this.auxNome + ' ' + this.auxSobrenome
              }

              _api.profissionais.post(test).then(res => {
                this.paramsAddProfissional = {
                  id: res.data.id,
                  nome: nomeMemed,
                  sobrenome: sobrenomeMemed,
                  crm: res.data.crm.replace(/[^0-9]/g, ''),
                  crmUF: res.data.crmUF,
                  conselho: res.data.conselho,
                  uf: res.data.ufAbreviado,
                  dataNascimento: res.data.dataNascimento,
                  cpf: newCpf
                }

                this.addProfissionalMemed(this.paramsAddProfissional)
                this.onListar(form)
                this.getProfissionais()
                this.loading.profissionais = false
              }).catch(e => {
                nomeMemed = this.formProfissional.nome
                sobrenomeMemed = this.formProfissional.sobrenome

                this.auxNome = ''
                this.auxSobrenome = ''
                // console.log('nomeMemed', nomeMemed)
                // console.log('sobrenomeMemed', sobrenomeMemed)
              })
            } else {
              let claims = []
              this.formProfissional.userClaims.forEach(item => {
                claims.push({
                  id: this.mxGerarGuid(),
                  claimType: item.claimType,
                  claimValue: item.claimValue
                })
              })
              this.formProfissional.userClaims = claims
              _api.profissionais.put(this.formProfissional).then(res => {
                try {
                  if (res.status === 200 || res.status === 204) {
                    let i = this.api.profissionais.findIndex(x => x.id === this.formProfissional.id)
                    this.perfils = []
                    this.onListar(form)
                    this.getProfissionais()
                    this.loading.profissionais = false
                    this.api.usuarios[i] = res.data
                    // console.log('res.data', res.data)
                    this.$refs.tabela.setCurrentRow(res.data)
                    this.formProfissional = {}
                  }
                } catch (e) {
                  return false
                }
              }).catch(e => {
                this.formProfissional.nome = this.formProfissional.nome
                this.formProfissional.sobrenome = this.formProfissional.sobrenome
                // console.log('this.paramsAddProfissional', this.paramsAddProfissional)
              })
            }
          } else {
            this.$swal({
              title: 'Atenção!',
              text: 'Verifique o preenchimento do formulário!',
              icon: 'warning'
            })
            this.loading.profissionais = false
            jQuery('.el-form-item__error').get(0).scrollIntoView()

            return false
          }
        })

        this.loading.profissionais = false
      },

      async getCep (cep) {
        if (this.listando) return
        cep.replace(/[.-\s]/g, '')
        if (cep.length >= 8) {
          let { data, status } = await _api.auxiliar.getCep(cep)

          if (status === 200) {
            this.formProfissional.logradouro = data.logradouro
            this.formProfissional.logradouroBairro = data.bairro
            this.formProfissional.ufAbreviado = data.uf
            this.paramsCidades.ibge = data.ibge
            await this.onSelectUf(data.uf)
            await this.getCidadesByUf()
          } else {
            this.paramsCidades.ibge = null
            this.formProfissional = {
              ...this.formProfissional,
              ufAbreviado: null,
              cidadeId: null,
              logradouro: null,
              bairro: null
            }
          }
        }
      },

      async getCbos () {
        let { data, status } = await _api.ocupacao.get()
        if (status === 200) {
          this.api.cboEsus = data
        } else {
          Notification({ title: 'Erro', message: 'Erro ao buscar lista de ocupações.', type: 'error' })
        }
      },

      resetForm (form) {
        this.$refs[form].resetFields()
      },

      handleSizeChange (val) {
        this.params.take = val
        this.getProfissionais()
      },

      handleCurrentChange (val) {
        this.params.skip = val
        this.getProfissionais()
      }

    }
  }
</script>
