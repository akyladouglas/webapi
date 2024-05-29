<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
    <el-card class="box-card box-card--main-card" v-if="mxHasAccess('Recepcionista')">

      <el-row>
        <el-col :span="14">
          <h2 class="box-card--h2">Agendamento de Consultas</h2>
        </el-col>
      </el-row>

      <el-row v-if="!mxHasAccess('Médico')">
        <el-col :span="24">
          <el-form :model="formAgendamento" status-icon ref="formAgendamento" label-width="120px" label-position="top" class="forms--usuario">
            <el-row :gutter="24">

              <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12">
                <el-form-item label="Paciente" prop="paciente">
                  <el-select filterable placeholder="Selecione o paciente" v-model="formAgendamento.individuoId"
                             v-loading.body="loading.individuos" @change="selectedPaciente">
                    <el-option v-for="option in api.individuos" :value="option.id"
                               :label="`${option.nomeCompleto} / ${option.cpf ? 'CPF:' + option.cpf : 'CPF não informado'} / ${option.cns ? 'CNS:' + option.cns : 'CNS não informado'}`" :key="option.id" />
                  </el-select>
                </el-form-item>
              </el-col>

              <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12">
                <el-form-item label="Estabelecimento" prop="estabelecimento">
                  <el-select filterable placeholder="Selecione o Estabelecimento" v-model="formAgendamento.estabelecimentoId"
                             v-loading.body="loading.estabelecimentos" :disabled="disabledEstabelecimento" @change="selectedEstabelecimento">
                    <el-option v-for="option in api.estabelecimentos" :value="option.id" :label="option.nomeFantasia" :key="option.id" />
                  </el-select>
                </el-form-item>
              </el-col>

              <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
                <el-form-item label="Especialidade" prop="procedimento">
                  <el-select filterable placeholder="Selecione a Especialidade" v-model="formAgendamento.procedimentoId"
                             v-loading.body="loading.procedimentos" :disabled="disabledProcedimento" @change="selectedProcedimento">
                    <el-option v-for="(option, index) in api.procedimentos" :value="option.procedimento.id" :label="option.procedimento.descricao" :key="index" />
                  </el-select>
                </el-form-item>
              </el-col>

              <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
                <el-form-item v-if="!$config.configDB.modulo == 3" label="Tipo Da Consulta" prop="tipoDaConsulta">
                  <el-select filterable placeholder="Selecione o Tipo Da Consulta" v-model="formAgendamento.tipoDaConsulta"
                             v-loading.body="loading.tipoDaConsulta" :disabled="disabledTipoDaConsulta" @change="selectedTipoDaConsulta">
                    <el-option v-for="item in enums.tipoDaConsulta" :value="item.value" :label="item.label" :key="item.value" />
                  </el-select>
                </el-form-item>
              </el-col>

              <el-col v-if="!$config.configDB.modulo == 3" :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
                <el-form-item label="Dia" prop="dia">
                  <el-select filterable placeholder="Selecione o Dia" v-model="formAgendamento.dia"
                             v-loading.body="loading.dias" :disabled="disabledDia" @change="selectDia">
                    <el-option v-for="(option, index) in api.dias" :value="option" :label="option" :key="index" />
                  </el-select>
                </el-form-item>
              </el-col>

              <el-col v-if="!$config.configDB.modulo == 3" :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
                <el-form-item label="Profissional" prop="profissional">
                  <el-select filterable placeholder="Selecione o Profissional" v-model="formAgendamento.profissionalId"
                             v-loading.body="loading.profissionais" :disabled="disabledProfissional" @change="selectedProfissional">
                    <el-option v-for="option in api.profissionais" :value="option.id" :label="option.nome" :key="option.id" />
                  </el-select>
                </el-form-item>
              </el-col>

              <el-col v-if="!$config.configDB.modulo == 3" :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
                <el-form-item label="Hora" prop="hora">
                  <el-select filterable placeholder="Selecione o Horário" v-model="formAgendamento.hora"
                             v-loading.body="loading.horas" :disabled="disabledHora" @change="selectedHora">
                    <el-option v-for="(option, index) in api.horas" :value="option" :label="option" :key="index" />
                  </el-select>
                </el-form-item>
              </el-col>

              <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
                <el-form-item label="Retorno" prop="retorno">
                  <el-select :disabled="disabledRetorno" filterable placeholder="Selecione Retorno" v-model="formAgendamento.retorno" @change="selectedRetorno">
                    <el-option v-for="item in enums.retorno" :value="item.value" :label="item.label" :key="item.value" />
                  </el-select>
                </el-form-item>
              </el-col>

              <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
                <el-form-item label="Agendamentos Anteriores" prop="retornoAgendamentoId">
                  <el-select :disabled="disabledRetornoAgendamento" filterable placeholder="Selecione o Agendamento" v-model="formAgendamento.retornoAgendamentoId" @change="selectedPastAgendamento">
                    <el-option v-for="item in api.agendamentosByIndividuo" :value="item.id" :label="item.dia" :key="item.id" />
                  </el-select>
                </el-form-item>
              </el-col>

            </el-row>

            <el-row :gutter="20">
              <el-col :xs="24">
                <el-form-item>
                  <el-button :disabled="disabledButton" flat icon="fas fa-save" type="success" @click="onClickSalvarAgendamento('formAgendamento')" v-loading.body="loading.agendamentos"> Salvar</el-button>
                  <el-button flat icon="fas fa-eraser" type="default" @click="resetFormAgendamento"> Limpar</el-button>
                </el-form-item>
              </el-col>
            </el-row>
          </el-form>
        </el-col>
      </el-row>
    </el-card>


    <el-card class="box-card box-card--main-card">

      <el-row>
        <el-col v-if="mxHasAccess('Recepcionista') || mxHasAccess('Médico') || mxHasAccess('MédicoAD')" :span="14">
          <h2 class="box-card--h2">Triagem - Fila de Espera</h2>
        </el-col>

        <el-col :span="10" class="text-right">
          <el-form :inline="true">
            <el-form-item>
              <el-button v-if="!listando" style="margin-right: -10px" icon="fas fa-undo-alt" type="warning" @click="onListar('formAgendamento')"> Voltar</el-button>
            </el-form-item>
          </el-form>
        </el-col>
      </el-row>


      <el-empty v-show="listando && api.agendamentos.length === 0" description="Nenhum Agendamento Encontrado"></el-empty>
      <el-row v-if="mxHasAccess('Recepcionista') && isDemandaEspontanea() || mxHasAccess('Médico')" v-show="listando && api.agendamentos.length > 0">
        <el-col :span="24">
          <el-table ref="tabela" :data="api.agendamentos"
                    highlight-current-row border
                    v-loading.body="loading.tableAgendamentos"
                    class="table--agendamentos table--row-click">
            <el-table-column label="Prioridade" prop="individuo.corStatus" align="center" width="150" sortable>
              <template slot-scope="scope">
                <span v-if="scope.row.individuo && scope.row.individuo.corStatus === 1" style="color:forestgreen">{{ prioridadeCor(scope.row.individuo.corStatus) }} </span>
                <span v-else-if="scope.row.individuo && scope.row.individuo.corStatus === 2" style="color:yellow">{{ prioridadeCor(scope.row.individuo.corStatus) }} </span>
                <span v-else-if="scope.row.individuo && scope.row.individuo.corStatus === 3" style="color:orange">{{ prioridadeCor(scope.row.individuo.corStatus) }} </span>
                <span v-else-if="scope.row.individuo && scope.row.individuo.corStatus === 4" style="color:red">{{ prioridadeCor(scope.row.individuo.corStatus) }} </span>
                <span v-else style="color:forestgreen"> {{prioridadeCor(1)}}</span>
              </template>
            </el-table-column>
            <el-table-column label="Especialidade" prop="procedimento.descricao" width="300" align="center" />
            <el-table-column label="Data" prop="individuo.dia" align="center" width="150">
              <template slot-scope="scope">
                <span>{{ moment(scope.row.dia).format('L') }} - </span>
                <span>{{ scope.row.hora.substr(0, 5) }}</span>
              </template>
            </el-table-column>
            <el-table-column label="Paciente" prop="individuo.nomeCompleto" align="center" />
            <el-table-column label="Profissional" prop="profissional.nome" align="center" />


            <el-table-column header-align="center" align="right" width="130" fixed="right" v-if="mxHasAccess('Recepcionista')">
              <template slot-scope="scope">
                <el-dropdown>
                  <el-button type="primary" size="small">
                    Ações <i class="el-icon-arrow-down el-icon--right"></i>
                  </el-button>
                  <el-dropdown-menu slot="dropdown">
                    <ul class="list-unstyled">
                      <li @click="openNegarPresencaTriagem(scope.row)" class="el-dropdown-menu__item text-danger">
                        <i class="fa fa-ban"></i> Não Compareceu
                      </li>

                      <li @click="onAgendamentos(scope.row)" class="el-dropdown-menu__item">
                        <i class="fa fa-user-md"></i> Atender
                      </li>
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

      <el-row v-if="mxHasAccess('Recepcionista') && !isDemandaEspontanea() || mxHasAccess('Triagem') || mxHasAccess('MédicoAD')" v-show="api.agendamentos.length > 0">
        <el-col :span="24">
          <el-table ref="tabela" :data="api.agendamentos"
                    highlight-current-row border
                    v-loading.body="loading.tableAgendamentos"
                    class="table--agendamentos table--row-click">
            <el-table-column label="Tipo" prop="tipoDaConsulta" align="center" width="125" />


            <el-table-column prop="corStatus" width="170" align="center">
              <template slot="header" slot-scope="scope">
                <el-tooltip content="O Grau de risco se da com base nas comorbidades do paciente" placement="top">
                  <span style="font-size: 16px">Grau de Risco</span>
                </el-tooltip>
              </template>
              <template slot-scope="scope">
                <div style="display: flex; justify-content: center; flex-direction: column">
                  <i class="fas fa-circle" :style="{ color: getCorGrauDeRisco(scope.row) }"></i>
                </div>
              </template>
            </el-table-column>

            <el-table-column label="Especialidade" prop="procedimento.descricao" align="center" />
            <el-table-column label="Data" prop="individuo.dia" align="center" width="150">
              <template slot-scope="scope">
                <span>{{ moment(scope.row.dia).format('L') }} - </span>
                <span>{{ scope.row.hora.substr(0, 5) }}</span>
              </template>
            </el-table-column>
            <el-table-column label="Profissional" prop="profissional.nome" align="center" />
            <el-table-column label="Paciente" prop="individuo.nomeCompleto" align="center" />
            <el-table-column header-align="center" align="right" width="160">
              <template slot-scope="scope">
                <el-dropdown>
                  <el-button @click="onAgendamentos(scope.row)" type="primary" size="small">
                    Atender
                  </el-button>
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
    </el-card>
  </el-col>
</template>

<style>
  .vermelho {
    background: #e63946 !important;
    color: #FFF !important
  }

  .laranja {
    background: #e76f51 !important;
    color: #FFF !important
  }

  .amarelo {
    background: #ffca3a !important;
    color: #000 !important
  }

  .verde {
    color: #FFF !important
  }

  .el-table .warning-row {
    background: 'rgb(252, 230, 190)';
  }

  .el-table .success-row {
    background: 'rgb(252, 230, 190)';
  }

  .el-table .other-row {
    background: 'rgb(252, 230, 190)';
  }

</style>

<script>
  import VuePerfectScrollbar from 'vue-perfect-scrollbar'
  import { Notification } from 'element-ui'
  import Utils from '../../mixins/Utils'
  import jQuery from 'jquery'
  import _api from '../../api'
  import _enums from '../../api/Enums'
  import FiltroAgenda from '../../components/shared/FiltroAgenda'
  import { mask } from 'vue-the-mask'
  import { Client as ConversationsClient } from '@twilio/conversations'
  import moment from 'moment'
  moment.locale('pt-br')

  export default {
    name: 'AgendaLista',
    mixins: [Utils],
    components: { FiltroAgenda, VuePerfectScrollbar },
    directives: { mask },
    data () {
      var validaCpf = (rule, value, callback) => {
        if (!this.formIndividuoCadastro.cpf) return callback(new Error('Cpf Obrigatório'))
        if (this.mxValidaCPF(this.formIndividuoCadastro.cpf) === false) {
          return callback(new Error('Cpf Inválido'))
        } else {
          return callback()
        }
      }

      var validaSenhaConfirmacao = (rule, value, callback) => {
        if (value === undefined) {
          return callback(new Error())
        } else if (value !== this.formIndividuoCadastro.senha) {
          Notification({ message: 'A senha não confere.', type: 'error' })
          return callback(new Error())
        } else {
          return callback()
        }
      }

      var validaDataNascimento = (rule, value, callback) => {
        const now = moment()

        if (value === undefined) {
          return callback(new Error())
        } else if (value > moment('01/01/1900') && value < now) {
          return callback()
        } else {
          Notification({ message: 'A Data De Nascimento não pode ser maior que o dia de hoje.', type: 'error' })
          return callback(new Error())
        }
      }
      var validaCelular = (rule, value, callback) => {
        if (value === undefined) {
          return callback(new Error('Campo obrigatório'))
        } else if (value.length < 15) {
          Notification({ message: 'Número inválido.', type: 'error' })
          return callback(new Error())
        } else {
          return callback()
        }
      }
      // var validaCNS = (rule, value, callback) => {
      //  var pattern = /^[0 - 9]/
      //  if (value === undefined) {
      //    return callback()
      //  } else if (value.match(pattern)) {
      //    return callback()
      //  } else {
      //    Notification({ message: 'Número CNS inválido.', type: 'error' })
      //    return callback(new Error())
      //  }
      // }

      var validaCNS = (rule, value, callback) => {
        if (!value) { return callback(new Error('Campo obrigatorio')) }
        if (value.length > 14) {
          if (this.mxValidaCns(value) === false) {
            return callback(new Error('O campo pode conter apenas números'))
          } else {
            callback()
          }
        } else {
          return callback(new Error('Número CNS inválido. O campo deve conter 15(quinze) dígitos'))
        }
      }

      var validaNome = (rule, value, callback) => {
        var pattern = /^[A-ZÀ-Ÿ][A-zÀ-ÿ']+\s([A-zÀ-ÿ']\s?)*[A-ZÀ-Ÿ][A-zÀ-ÿ']+$/
        if (value === undefined) {
          return callback(new Error())
        } else if (value.match(pattern)) {
          return callback()
        } else {
          Notification({ message: 'Primeira letra de cada nome deve ser maiúscula', type: 'error' })
          return callback(new Error())
        }
      }
      var validaNomeSocial = (rule, value, callback) => {
        var pattern = /^[A-ZÀ-Ÿ][A-zÀ-ÿ']+\s([A-zÀ-ÿ']\s?)*[A-ZÀ-Ÿ][A-zÀ-ÿ']+$/
        if (value === undefined) {
          return callback()
        } else if (value.match(pattern)) {
          return callback()
        } else {
          Notification({ message: 'Primeira letra de cada nome deve ser maiúscula', type: 'error' })
          return callback(new Error())
        }
      }
      var validaEmail = (rule, value, callback) => {
        var pattern = /^[a-z*0-9]([\.-]?\w+)*([a-z0-9])@\w+([\.-]?\w+)*(\.\w{2,3})+$/
        if (value === undefined) {
          return callback(new Error())
        } else if (value.match(pattern)) {
          return callback()
        } else {
          Notification({ message: 'Email inválido.', type: 'error' })
          return callback(new Error())
        }
      }
      var validaSenha = (rule, value, callback) => {
        var pattern = '^(?=.*[0-9])(?=.*[a-z]).{8,}'
        if (value === undefined) {
          return callback(new Error())
        } else if (value.match(pattern)) {
          return callback()
        } else {
          Notification({ message: 'A senha deve conter ao menos 1 letra e 1 número e conter no mínimo 8 caracteres.', type: 'error' })
          return callback(new Error())
        }
      }

      return {
        value: null,
        conversationsClient: null,
        options: {},
        // Agendamento de Consultas
        disabledEstabelecimento: true,
        disabledProcedimento: true,
        disabledProfissional: true,
        disabledDia: true,
        disabledTipoDaConsulta: true,
        disabledHora: true,
        disabledRetorno: true,
        disabledRetornoAgendamento: true,
        disabledButton: true,
        formAgendamento: {},
        // Fim Agendamento de Consultas

        statusPutRecepcao: null,
        dataConsulta: null,
        alterarSinaisVitais: false,
        anexarExames: false,
        isDisabled: false,
        listando: true,
        isValid: true,
        formRow: {},
        formIndividuo: {},
        formIndividuoCadastro: {},
        formProfissional: {},
        formUsuario: {},
        formExames: {},
        exames: [],
        tipoExame: 0,

        enums: {
          tipoDaConsulta: _enums.tiposDaConsulta,
          sexos: _enums.sexos,
          racaOuCor: _enums.racaOuCor,
          retorno: [..._enums.retorno]
        },

        item: [
          { label: 'Nenhum', value: 0 },
          { label: 'Baixa', value: 1 },
          { label: 'Média', value: 2 },
          { label: 'Alta', value: 3 }
        ],
        showBtn: false,
        showEnviar: false,
        scrollSettings: {
          maxScrollbarLength: 20
        },
        erros: [],

        validacoes: {
          dia: [{
            required: true, message: 'Campo obrigatório', trigger: 'change'
          }],
          nomeCompleto: [
            { required: true, validator: validaNome, message: 'Campo obrigatório.', trigger: 'blur' },
            { min: 2, max: 100, message: 'Não pode conter menos de 2 e mais que 100 caracteres', trigger: 'submit' }
          ],
          nomeSocial: [
            { required: false, validator: validaNomeSocial, message: 'Campo obrigatório.', trigger: 'blur' }
          ],
          // cpf: [
          //  { required: false, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] },
          //  { validator: validaCpf, trigger: ['blur', 'change', 'submit'] }
          // ],
          cns: [{ required: true, validator: validaCNS, message: 'Deve conter 15 dígitos.', trigger: ['blur', 'submit'] }],
          senha: [{ required: true, validator: validaSenha, message: 'Campo obrigatório', trigger: ['blur', 'submit'] }],
          confirmacao: [{ required: true, message: 'Campo obrigatório', validator: validaSenhaConfirmacao, trigger: ['blur', 'submit'] }],
          telefoneCelular: [{ required: true, validator: validaCelular, message: 'Campo obrigatório', trigger: ['blur', 'submit'] }],
          nomeDaMae: [
            { required: true, validator: validaNome, message: 'Campo obrigatório', trigger: 'blur' },
            { min: 2, max: 100, message: 'Deve conter mais de 2 letras e menos que 100 letras', trigger: ['blur', 'change'] }
          ],
          email: [
            { required: true, validator: validaEmail, message: 'Campo Obrigatório', trigger: ['blur', 'submit'] },
            { type: 'email', message: 'Email inválido', trigger: ['blur', 'change'] }
          ],
          dataNascimento: [{ required: true, validator: validaDataNascimento, message: 'Campo obrigatório', trigger: ['blur'] }],
          racaOuCor: [{ required: true, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],
          sexo: [{ required: true, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }]

        },
        api: {
          agendamentosByIndividuo: [],
          agendamentos: [],
          // api individuo retorna dados do getbyid do paciente quando abrir o modal (sistema modo agendamento)
          individuo: {},
          // api individuos retorna dados do getbyiparam do paciente
          individuos: [],
          estabelecimentos: [],
          procedimentos: [],
          profissionais: [],
          tipoDaConsulta: [],
          dias: [],
          horas: [],
          exames: [],
          usuario: {},
          ufs: []
        },
        loading: {
          individuos: false,
          estabelecimentos: false,
          procedimentos: false,
          profissionais: false,
          dias: false,
          horas: false,
          agendamentos: false,
          tableAgendamentos: false,
          formulario: false,
          usuarios: false,
          exames: false,
          cidades: false,
          ufs: false,
          tipoDaConsulta: false
        },

        paramsIndividuos: {
          skip: 1,
          take: 1000,
          sort: 'i.NomeCompleto ASC',
          total: 0
        },

        paramsEstabelecimentos: {
          ativo: true
        },

        paramsProcedimentos: {
          skip: 1,
          take: 1000,
          total: 0
        },

        paramsProfissional: {
          skip: 1,
          take: 10,
          total: 0
        },

        params: {
          skip: 1,
          take: 10,
          sort: 'DataCadastro ASC',
          total: 0
        },
        paramsAgendamento: {},
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
      await this.getIndividuos()
      await this.getDadosUsuario()
      await this.getAgendamentos()
      console.log('configDB', this.$config.configDB)
      //this.$swal({
      //  icon: "success",
      //  title: "Lorem ipsum",
      //  text: "ipsum lorem!"
      //});
    },
    methods: {
      async onClickSalvarIndividuo (form) {
        this.erros = []
        this.$refs[form].validate(async (valid) => {
          if (valid) {
            this.loading.individuos = true
            this.formIndividuoCadastro.ativo = true
            this.formIndividuoCadastro.imagem = ''
            _api.individuos.post(this.formIndividuoCadastro).then(res => {
              if (res.status === 201) {
                this.loading.individuos = false
                this.onListar(form)
                this.resetForm(form)
                this.getIndividuos()
                this.$swal({
                  title: "Sucesso!",
                  text: 'O paciente foi cadastrado com sucesso!',
                  icon: 'success',
                })
              } else {
                this.$swal({
                  title: "Erro!",
                  text: 'Ocorreu um erro ao criar o registro de cadastro do paciente!',
                  icon: 'error',
                })

                this.loading.individuos = false
              }
            })
          } else {
            this.$swal({
              title: "Atenção!",
              text: 'Preencha todos os campos obrigatórios!',
              icon: 'warning',
            })
            this.loading.individuos = false
            jQuery('.el-form-item__error').get(0).scrollIntoView()

            return false
          }
        })
        this.loading.individuos = false
      },

      async getCep (cep) {
        if (cep.length > 8) {
          let { data, status } = await _api.auxiliar.getCep(cep)

          if (status === 200) {
            this.formIndividuoCadastro.cidadeId = data.ibge
            this.formIndividuoCadastro.logradouro = data.logradouro
            this.formIndividuoCadastro.logradouroBairro = data.bairro
            this.formIndividuoCadastro.ufAbreviado = data.uf
            this.paramsCidades.ibge = data.ibge
            await this.onSelectUf(data.uf)
            await this.getCidadesByUf()
          } else {
            this.paramsCidades.ibge = null
            this.formIndividuoCadastro = {
              ...this.formIndividuoCadastro,
              ufAbreviado: null,
              cidadeId: null,
              logradouro: null,
              bairro: null
            }
          }
        }
      },
      async onSelectUf (val) {
        this.paramsCidades.ufAbreviado = val
        this.formIndividuoCadastro = {
          ...this.formIndividuoCadastro,
          cidadeId: null
        }
        await this.getCidadesByUf()
      },
      async getCidadesByUf () {
        this.loading.cidades = true
        let { data, paginacao, status } = await _api.cidades.get(this.paramsCidades)
        this.api.cidades = (status === 200) ? data : []
        if (this.api.cidades.length === 1) {
          this.formIndividuoCadastro.cidadeId = this.api.cidades[0].ibge
        }
        this.paramsCidades.skip = (status === 200) ? paginacao.currentPage : 0
        this.paramsCidades.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.cidades = false
      },

      // Agendamento de Consultas
      async getIndividuos () {
        this.loading.individuos = true

        if (this.mxHasAccess('Recepcionista')) this.paramsIndividuos.ativo = true

        let { data, paginacao, status } = await _api.individuos.getAll(this.paramsIndividuos)
        if (status === 502) this.loading.individuos = false
        this.api.individuos = (status === 200) ? data : []
        this.paramsIndividuos.skip = (status === 200) ? paginacao.skip : 0
        this.paramsIndividuos.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.individuos = false
      },

      async selectedPaciente () {
        this.disabledEstabelecimento = false
        await this.getEstabelecimentos()
      },

      async getEstabelecimentos () {
        this.loading.estabelecimentos = true
        let { data, paginacao, status } = await _api.estabelecimentos.get(this.paramsEstabelecimentos)
        if (status === 502) this.loading.estabelecimentos = false
        this.api.estabelecimentos = (status === 200) ? data.filter(estab => estab.nomeFantasia ? estab : null) : []
        this.paramsEstabelecimentos.skip = (status === 200) ? paginacao.skip : 0
        this.paramsEstabelecimentos.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.estabelecimentos = false
      },

      async selectedEstabelecimento (val) {
        this.disabledProcedimento = false
        await this.getProcedimentos(val)
      },

      async getProcedimentos (val) {
        this.loading.procedimentos = true
        this.paramsProcedimentos = {
          ...this.paramsProcedimentos,
          estabelecimentoId: val
        }

        //se não for demanda espontanea
        if (this.$config.configDB.modulo !== 3) {
          let { data, status } = await _api.estabelecimentosProcedimentosHorarios.get(this.paramsProcedimentos)

          if (status === 502) this.loading.procedimentos = false

          const array = []
          data.forEach(element => {
            const exist = array.some(obj => obj.procedimentoId === element.procedimentoId)
            if (!exist) { array.push(element) }
          })

          this.api.procedimentos = (status === 200) ? array : []

          this.loading.procedimentos = false
        }
        //se for demanda espontanea
        else {
          let { data, status } = await _api.estabelecimentos.getEstabelecimentoProcedimentos(this.paramsProcedimentos)
          if (status === 502) this.loading.procedimentos = false
          this.api.procedimentos = (status === 200) ? data : []
          this.loading.procedimentos = false
        }
        
      },

      async selectedProcedimento(val) {
        //se não for demanda espontanea
        if (this.$config.configDB.modulo !== 3) {
          this.disabledProfissional = false
          await this.getProfissionais()
        }
        //se for demanda espontanea
        else {
          this.disabledRetorno = false
        }
      },
      async selectedTipoDaConsulta() {
          if (this.formAgendamento.dia) delete this.formAgendamento.dia
          if (this.formAgendamento.profissionalId) delete this.formAgendamento.profissionalId
          this.disabledProfissional = true
          if (this.formAgendamento.hora) delete this.formAgendamento.hora
          this.disabledHora = true
          if (this.formAgendamento.retorno) delete this.formAgendamento.retorno
          this.disabledRetorno = true
          if (this.formAgendamento.retornoAgendamentoId) delete this.formAgendamento.retornoAgendamentoId
          this.disabledRetornoAgendamento = true

          this.disabledDia = false
          await this.getDias()
      },

      async getProfissionais () {
        this.loading.profissionais = true
        this.paramsProfissional.estabelecimentoId = this.formAgendamento.estabelecimentoId
        this.paramsProfissional.procedimentoId = this.formAgendamento.procedimentoId
        this.paramsProfissional.tipoDaConsulta = this.formAgendamento.tipoDaConsulta

        if (this.formAgendamento.dia != undefined) {
          let dia = this.formAgendamento.dia.split('/')
          this.paramsProfissional.DataInicial = new Date(dia[2], dia[1] - 1, dia[0])
          this.paramsProfissional.DataFinal = new Date(dia[2], dia[1] - 1, dia[0])
        }

        let { data, paginacao, status } = await _api.profissionais.getProfissionaisByProcedimento(this.paramsProfissional)
        if (status === 502) this.loading.profissionais = false
        this.api.profissionais = (status === 200) ? data : []
        this.loading.profissionais = false
      },

      async getDias () {
        this.loading.dias = true
        let { data, status, response } = await _api.estabelecimentosProcedimentosHorarios.get(this.formAgendamento)
        if (status === 502) this.loading.dias = false
        const array = []
        if (status !== 200) this.api.dias = []
        data.forEach(element => {
          const exist = array.some(obj => obj === element.dia)
          if (!exist) array.push(element.dia)
        })
        this.api.dias = array.map(element => {
          return moment(element).format('DD/MM/YYYY')
        })
        this.loading.dias = false
      },

      async selectDia (val) {
        this.disabledProfissional = false
        await this.getProfissionais()
      },

      async selectedProfissional (val) {
        if (this.demandaEspontanea) {
          /* this.disabledButton = false */
          this.disabledRetorno = false
        } else {
          this.disabledHora = false
          this.getHoras()
        }
      },

      async selectedRetorno (val) {
        this.disabledButton = false

        if (this.formAgendamento.retorno) {
          this.disabledRetornoAgendamento = false
          this.disabledButton = true
          let paramsRetorno = {
            estabelecimentoId: this.formAgendamento.estabelecimentoId,
            individuoId: this.formAgendamento.individuoId,
            procedimentoId: this.formAgendamento.procedimentoId,

            retorno: true,
            skip: 1,
            take: 999999,
            sort: 'DataCadastro DESC',
            total: 0
          }

          let { data, status, response } = await _api.agendamentos.get(paramsRetorno)
          this.api.agendamentosByIndividuo = (status === 200) ? data : []
          this.api.agendamentosByIndividuo.forEach(item => {
            item.dia = 'Realizado em: ' + moment(item.dia).format('DD/MM/YYYY') + ' às ' + item.hora + ' - ' + item.tipoDaConsulta + ' - ' + 'Médico(a): ' + item.profissional.nome + ' - ' + 'Especialidade: ' + item.procedimento.descricao
          })
        } else {
          delete this.formAgendamento.retornoAgendamentoId
          this.disabledRetornoAgendamento = true
        }
      },

      async selectedPastAgendamento (val) {
        this.disabledButton = false
      },

      async getHoras () {
        this.loading.horas = true

        let dia = this.formAgendamento.dia.split('/')
        let Dia = new Date(dia[2], dia[1] - 1, dia[0])

        let paramsHoras = {
          ...this.formAgendamento,
          Dia
        }
        paramsHoras.dia = null
        let { data, status, response } = await _api.estabelecimentosProcedimentosHorarios.get(paramsHoras)
        // console.log('data', data)
        if (status === 502) this.loading.horas = false
        const array = []
        if (status !== 200) this.api.horas = []
        data.forEach(element => {
          const exist = array.some(obj => obj === element.hora)
          if (!exist) array.push(element.hora)
        })
        this.api.horas = array
        this.loading.horas = false
      },

      async selectedHora () {
        this.disabledRetorno = false
      },
      async getConversationClient () {
        window.conversationsClient = ConversationsClient
        let { data } = await _api.teleConsulta.twGetChatToken(this.options.individuoId)
        let token = data
        this.conversationsClient = new ConversationsClient(token)
        this.conversationsClient.on('connectionStateChanged', (state) => {
          switch (state) {
            case 'connected':
              this.createConversation()
              break
          }
        })
      },
      async createConversation () {
        const uniqueName = `${this.options.agendamentoId}-chat`
        try {
          const activeConnection = await this.conversationsClient.createConversation({ uniqueName: uniqueName })
          const joinedConversation = await activeConnection.join()
          await joinedConversation.add(this.options.profissionalId).catch(err => console.log(err))
          await joinedConversation.add(this.options.individuoId).catch(err => console.log(err))
        } catch (e) {
          console.log('e', e)
          try {
            const joinedConversation = await this.conversationsClient.getConversationByUniqueName(uniqueName)
            await joinedConversation.add(this.options.profissionalId).catch(err => console.log(err))
            await joinedConversation.add(this.options.individuoId).catch(err => console.log(err))
          } catch (e) {
            console.log('e2', e)
          }
        }
      },
      async onClickSalvarAgendamento(form) {
        this.formAgendamento.numProntuario = this.mxProntuario(7)
        this.formAgendamento.tipoDeAtendimento = 2 // valor do Enum TipoDeAtendimento (Consulta agendada)
        this.formAgendamento.condutas = '1' // valor do Enum CondutaEncaminhamento (Retorno para consulta agendada)

        //se não for demanda espontanea
        if (this.$config.configDB.modulo !== 3) {
          let verificarProfissional = this.api.profissionais.filter(prof => {
            if (prof.id === this.formAgendamento.profissionalId) {
              return prof
            }
          })
          if (verificarProfissional.length !== 0) {
            this.loading.agendamentos = true
            this.paramsAgendamento = {
              ...this.formAgendamento
            }
            let dia = this.formAgendamento.dia.split('/')

            this.paramsAgendamento.dia = new Date(dia[2], dia[1] - 1, dia[0])
            if (this.paramsAgendamento.tipoDaConsulta !== 'Teleconsulta') this.paramsAgendamento.presencaConfirmada = false
            else this.paramsAgendamento.presencaConfirmada = true
            _api.agendamentos.post(this.paramsAgendamento).then(async (res) => {
              if (res.status === 201) {
                this.options = {
                  agendamentoId: res.data,
                  individuoId: this.paramsAgendamento.individuoId,
                  profissionalId: this.paramsAgendamento.profissionalId
                }
                await this.getConversationClient()
                await this.createConversation(this.options)
                this.getAgendamentos()
                this.loading.agendamento = false
                this.resetFormAgendamento(form)
              } else {
                this.$swal({
                  title: "Erro!",
                  text: 'Ocorreu um erro ao criar o registro de agendamento!',
                  icon: 'error',
                })
                this.loading.agendamento = false
              }
            })
          } else {
            this.$swal({
              title: "Atenção!",
              text: 'O profissional selecionado não esta cadastrado no procedimento!',
              icon: 'warning',
            })
            this.resetFormAgendamento(form)
          }
        }
        //se for demanda espontanea
        else {
          this.loading.agendamentos = true
          this.formAgendamento.tipoDaConsulta = 'Totem'
          this.formAgendamento.presencaConfirmada = true
          _api.agendamentos.post(this.formAgendamento).then(async (res) => {
            if (res.status === 201) {
              this.options = {
                agendamentoId: res.data,
                individuoId: this.formAgendamento.individuoId,
              }
              this.$swal({
                title: "Criado!",
                text: "Agendamento criado com sucesso.",
                icon: "success"
              });

              await this.getConversationClient()
              await this.getAgendamentos()
              await this.resetFormAgendamento(form)
              this.loading.agendamentos = false
            } else {
              this.$swal({
                title: "Erro!",
                text: "Erro ao criar o agendamento.",
                icon: "error"
              });
              this.loading.agendamento = false
            }
          })
        }
      },

      async resetFormAgendamento () {
        this.disabledProcedimento = true
        this.disabledEstabelecimento = true
        this.disabledProfissional = true
        this.disabledDia = true
        this.disabledHora = true
        this.disabledRetorno = true
        this.disabledTipoDaConsulta = true
        this.disabledRetornoAgendamento = true
        this.disabledButton = true

        this.formAgendamento.individuoId = null
        delete this.formAgendamento.retornoAgendamentoId
        this.formAgendamento.dia = null
        this.formAgendamento.estabelecimentoId = null
        this.formAgendamento.procedimentoId = null
        this.formAgendamento.profissionalId = null
        this.formAgendamento.hora = null
        this.formAgendamento.retorno = null
        this.formAgendamento.tipoDaConsulta = null

        this.loading.individuos = false
        this.loading.estabelecimentos = false
        this.loading.procedimentos = false
        this.loading.profissionais = false
        this.loading.dias = false
        this.loading.horas = false
        this.loading.agendamentos = false
        this.loading.formulario = false
        this.loading.usuarios = false
        this.loading.exames = false
        this.loading.cidades = false
        this.loading.ufs = false
        this.loading.tipoDaConsulta = false
      },

      // Fim Agendamento de Consultas

      async getDadosUsuario () {
        let { data, status } = await _api.usuarios.getById(this.$auth.user().id)
        this.api.usuarios = (status === 200) ? data : []
        // console.log('data getDadosUsuario', data)
        // console.log('status', status)
      },

      prioridadeCor (val) {
        // console.log(val)
        if (val === 1) {
          return 'VerdeBaixoRisco'
        }
        if (val === 2) {
          return 'Amarelo'
        }
        if (val === 3) {
          return 'Laranja'
        }
        if (val === 4) {
          return 'Vermelho'
        }
      },

      async getAgendamentos () {
        if (this.mxHasAccess('Recepcionista')) {
          // quando entrar na rota da agenda retornar os agendamentos somente de hoje
          this.params.DataInicial = new Date(new Date().setHours(0, 0, 0, 0))
          this.params.DataFinal = new Date(new Date().setHours(23, 59, 59, 0))
          this.params.estabelecimentoId = this.$store.state.user.dados.estabelecimentoId
  
          if (this.$route.params.dashboard == true && this.$route.params.tipoDaConsulta == undefined) {
            this.params.dataInicial = new Date()
            this.params.dataFinal = new Date()
          }
          if (this.$route.params.dashboard == true && this.$route.params.tipoDaConsulta != undefined) {
            this.params.dataInicial = new Date()
            this.params.dataFinal = new Date()
            this.params.tipoDaConsulta = this.$route.params.tipoDaConsulta
          }
        }

        if (this.mxHasAccess('Médico') || this.mxHasAccess('MédicoAD')) {
          this.params.profissionalId = this.$auth.user().id

          if (this.$route.params.tipoDaConsulta == undefined) {
            this.params.DataInicial = new Date(new Date().setHours(0, 0, 0, 0))
            this.params.DataFinal = new Date(new Date().setHours(23, 59, 59, 0))
          }
  
          if (this.$route.params.dashboard == true && this.$route.params.tipoDaConsulta != undefined) {
            this.params.DataInicial = new Date(new Date().setHours(0, 0, 0, 0))
            this.params.DataFinal = new Date(new Date().setHours(23, 59, 59, 0))
            this.params.tipoDaConsulta = this.$route.params.tipoDaConsulta
          }
        }

        if (this.mxHasAccess('Triagem')) {
          // quando entrar na rota da agenda retornar os agendamentos somente de hoje
          this.params.DataInicial = new Date(new Date().setHours(0, 0, 0, 0))
          this.params.DataFinal = new Date(new Date().setHours(23, 59, 59, 0))
          this.params.estabelecimentoId = this.$store.state.user.dados.estabelecimentoId
        }

        if (this.params.skip == 0) this.params.skip = 1
  
        this.params.presencaConfirmada = false
        this.params.tipoDaConsulta = null
        this.params.finalizado = false
        this.params.naoCompareceu = false
        this.params.cancelado = false

        this.loading.tableAgendamentos = true
        let { data, paginacao, status } = await _api.agendamentos.get(this.params)
        if (status === 502) this.loading.tableAgendamentos = false
        this.api.agendamentos = (status === 200) ? data : []
        if (this.params.skip == 0) this.params.skip = 1

        // console.log('this.api.agendamentos', this.api.agendamentos)
        this.params.skip = (status === 200) ? paginacao.skip : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.tableAgendamentos = false
      },
      async getPacienteById () {
        // console.log('this.api.agendamentos em row', this.formRow)
        let { data, status } = await _api.individuos.getById(this.formRow.individuoId)
        this.api.individuo = (status === 200) ? data : []
        //  console.log('this.api.individuo', this.api.individuo)
      },
      resetForm (form) {
        this.$refs[form].resetFields()
      },
      onListar (form) {
        let i = this.api.agendamentos.findIndex(x => x.id === this.formAgendamento.id)
        this.$refs.tabela.setCurrentRow(this.api.agendamentos[i])
        this.$refs[form].resetFields()
        this.listando = true
      },
      handleSizeChange (val) {
        this.params.pageSize = val
      },
      handleCurrentChange (val) {
        this.params.skip = val
        this.getAgendamentos()
      },

      onClickSalvar (form) {
        this.loading.usuario = true
        if (form != '') {
          _api.agendamentos.putRecepcao(this.formUsuario).then(res => {
            if (res.status === 204) {
              // console.log(res)
              this.loading.usuario = false
              this.alterarSinaisVitais = false
              // console.log('this.loading.usuario', this.loading.usuario)
            }
          })
        } else {
          this.$swal({
            title: "Atenção!",
            text: 'Preencha todos os campos obrigatórios!',
            icon: 'warning',
          })
          this.loading.usuario = false
          this.alterarSinaisVitais = false
          return false
        }
      },

      onAgendamentos (row) {
        this.$router.push({
          name: 'AtendimentoTriagem',
          params: { agendamento: row }
        })
      },

      async openNegarPresencaTriagem(val) {
        this.$swal({
          title: "Atenção",
          text: `Deseja continuar com o procedimento de negar a presença do paciente?`,
          icon: 'warning',
          showCloseButton: true,
          confirmButtonText: "Sim",
          showCancelButton: true,
          cancelButtonText: "Cancelar",
        }).then(async (result) => {
          if (result.isConfirmed) {
            await this.negarPresencaTriagem(val)

            this.$swal({
              title: "Sucesso!",
              text: 'O agendamento para o paciente foi negado com sucesso!',
              icon: 'success',
            })
          }
        }).catch((result) => {

        })
      },

      negarPresencaTriagem (val) {
        this.loading.agendamentos = true
        if (val != {}) {
          let formCancelado = {}
          formCancelado.id = val.id
          formCancelado.naoCompareceu = true
          formCancelado.finalizado = true
          formCancelado.cancelado = false
          _api.agendamentos.putTriagem(formCancelado).then(res => {
            if (res.status === 204) {
              this.loading.agendamentos = false
            }
            this.getAgendamentos()
          })
        } else {
          this.loading.agendamentos = false
          return this.$swal({
            title: "Atenção!",
            text: 'Preencha todos os campos obrigatórios!',
            icon: 'warning',
          })
        }
      },

      getCorGrauDeRisco (row) {
        if (row.individuo.corStatus != undefined && row.individuo.corStatus == 1) {
          return '#228b22'
        } else if (row.individuo.corStatus != undefined && row.individuo.corStatus == 2) {
          return '#FFFF00'
        } else if (row.individuo.corStatus != undefined && row.individuo.corStatus == 3) {
          return '#ffa500'
        } else if (row.individuo.corStatus != undefined && row.individuo.corStatus == 4) {
          return '#FF0000'
        } else {
          return '#808080'
        }
      }

    }
  }
</script>
<style>
  .vm--overlay {
    display: flex;
    width: 1%;
    background-color: transparent;
    /*align-items: center;
    justify-content: center;*/
  }

  .scroll-area2 {
    position: relative;
    width: 100%;
    height: 600px;
    padding-bottom: 20px;
    overflow-x: hidden;
    overflow-y: auto;
  }

  .scroll-area3 {
    position: relative;
    width: 100%;
    height: 700px;
    padding-bottom: 20px;
    overflow-x: hidden;
    overflow-y: auto;
  }

  p.title {
    font-size: 13px;
    margin-bottom: 0px
  }

  h4.name-profissional {
    font-size: 16px
  }

  .paciente__missing_photo {
    display: flex;
    justify-content: center;
    align-items: center;
    width: 110px;
    height: 110px;
    background-color: #f2f2f2;
    border-radius: 100px;
  }
</style>
