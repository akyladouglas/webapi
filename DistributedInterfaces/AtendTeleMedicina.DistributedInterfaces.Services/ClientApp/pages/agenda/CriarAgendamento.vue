<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
    <el-card class="box-card box-card--main-card">

      <el-row>
        <el-col :span="14">
          <h2 class="box-card--h2">{{$route.meta.title}}Agendamento de Consultas</h2>
        </el-col>
      </el-row>

      <el-row>
        <el-col :span="24">

          <el-form :model="formAgendamento" status-icon ref="formAgendamento" label-width="120px" label-position="top" class="forms--usuario">
            <el-row :gutter="20">


              <el-col :xs="24" :sm="24" :md="12" :lg="10" :xl="10">
                <el-form-item label="Paciente" prop="paciente">
                  <el-select filterable placeholder="Selecione o paciente" v-model="formAgendamento.individuoId"
                             v-loading.body="loading.individuos" @change="selectedPaciente">
                    <el-option v-for="option in api.individuos" :value="option.id" :label="option.nomeCompleto" :key="option.id" />
                  </el-select>
                </el-form-item>
              </el-col>

              <el-col :xs="24" :sm="24" :md="12" :lg="10" :xl="10">
                <el-form-item label="Estabelecimento" prop="estabelecimento">
                  <el-select filterable placeholder="Selecione o Estabelecimento" v-model="formAgendamento.estabelecimentoId"
                             v-loading.body="loading.estabelecimentos" :disabled="disabledEstabelecimento" @change="selectedEstabelecimento">
                    <el-option v-for="option in api.estabelecimentos" :value="option.id" :label="option.nomeFantasia" :key="option.id" />
                  </el-select>
                </el-form-item>
              </el-col>
              <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="6">
                <el-form-item label="Procedimento" prop="procedimento">
                  <el-select filterable placeholder="Selecione o Procedimento" v-model="formAgendamento.procedimentoId"
                             v-loading.body="loading.procedimentos" :disabled="disabledProcedimento" @change="selectedProcedimento">
                    <el-option v-for="(option, index) in api.procedimentos" :value="option.procedimento.id" :label="option.procedimento.descricao" :key="index" />
                  </el-select>
                </el-form-item>
              </el-col>
              <el-col v-if="!demandaEspontanea" :xs="24" :sm="24" :md="12" :lg="5" :xl="6">
                <el-form-item label="Dia" prop="dia">
                  <el-select filterable placeholder="Selecione o Dia" v-model="formAgendamento.dia"
                             v-loading.body="loading.dias" :disabled="disabledDia" @change="selectDia">
                    <el-option v-for="(option, index) in api.dias" :value="option" :label="option" :key="index" />
                  </el-select>
                </el-form-item>
              </el-col>
              <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="6">
                <el-form-item label="Profissional" prop="profissional">
                  <el-select filterable placeholder="Selecione o Profissional" v-model="formAgendamento.profissionalId"
                             v-loading.body="loading.profissionais" :disabled="disabledProfissional" @change="selectedProfissional">
                    <el-option v-for="option in api.profissionais" :value="option.id" :label="option.nome" :key="option.id" />
                  </el-select>
                </el-form-item>
              </el-col>

              <el-col :xs="24" v-if="!demandaEspontanea" :sm="24" :md="12" :lg="5" :xl="6">
                <el-form-item label="Hora" prop="hora">
                  <el-select filterable placeholder="Selecione o Horário" v-model="formAgendamento.hora"
                             v-loading.body="loading.horas" :disabled="disabledHora" @change="selectedHora">
                    <el-option v-for="(option, index) in api.horas" :value="option" :label="option" :key="index" />
                  </el-select>
                </el-form-item>
              </el-col>

              <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="6">
                <el-form-item label="Retorno" prop="retorno">
                  <el-select filterable placeholder="Selecione Retorno" v-model="formAgendamento.retorno" @change="selectedRetorno">
                    <el-option v-for="item in enums.retorno" :value="item.value" :label="item.label" :key="item.value" />
                  </el-select>
                </el-form-item>
              </el-col>

            </el-row>

            <el-row :gutter="20">
              <el-col :xs="24">
                <el-form-item>
                  <el-button :disabled="disabledButton" flat icon="fas fa-save" type="success" @click="onClickSalvar('formAgendamento')" v-loading.body="loading.agendamento"> Salvar</el-button>
                  <el-button flat icon="fas fa-eraser" type="default" @click="resetForm"> Limpar</el-button>

                  <!--<el-button flat icon="fas fa-undo-alt"  type="warning" @click="$router.go(-1)" :disabled="loading.usuario"> Cancelar</el-button>-->
                  <!--<el-button flat icon="fas fa-undo-alt"  type="warning" @click="$router.go(-1)" :disabled="loading.usuario"> Voltar</el-button>-->
                </el-form-item>
              </el-col>
            </el-row>

          </el-form>




        </el-col>
      </el-row>


    </el-card>
  </el-col>
</template>

<script>
  import VuePerfectScrollbar from 'vue-perfect-scrollbar'
  import Utils from '../../mixins/Utils'
  import jQuery from 'jquery'
  import _api from '../../api'
  import _enums from '../../api/Enums'
  import FiltroAgenda from '../../components/shared/FiltroAgenda'
  import { mask } from 'vue-the-mask'
  import moment from 'moment'
  moment.locale('pt-br')

  export default {
    name: 'AgendaLista',
    mixins: [Utils],
    components: { FiltroAgenda, VuePerfectScrollbar },
    directives: { mask },
    data () {
      return {
        statusPutRecepcao: null,
        dataConsulta: null,
        isDisabled: false,
        listando: true,
        isValid: true,
        disabledProcedimento: true,
        disabledEstabelecimento: true,
        disabledProfissional: true,
        disabledDia: true,
        disabledHora: true,
        disabledButton: true,
        formAgendamento: {
          tipoDaConsulta: 'Totem'
        },
        enums: {
          retorno: [
            ..._enums.retorno
          ]
        },
        formRow: {},
        formIndividuo: {},
        formProfissional: {},
        formUsuario: {},
        formExames: {},
        exames: [],
        tipoExame: 0,
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
        api: {
          agendamentos: [],
          individuos: [],
          exames: [],
          estabelecimentos: [],
          procedimentos: [],
          profissionais: [],
          dias: [],
          horas: []
        },
        loading: {
          agendamentos: false,
          formulario: false,
          usuario: false,
          exames: false,
          individuos: false,
          estabelecimentos: false,
          profissionais: false,
          procedimentos: false,
          dias: false,
          horas: false
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
        paramsAgendamento: {}

      }
    },
    async mounted () {
      await this.getIndividuos()
      // console.log('form', this.formAgendamento)
    },
    methods: {
      async getIndividuos () {
        this.loading.individuos = true
        let { data, paginacao, status } = await _api.individuos.getAll(this.paramsIndividuos)
        if (status === 502) this.loading.individuos = false
        this.api.individuos = (status === 200) ? data : []
        this.paramsIndividuos.skip = (status === 200) ? paginacao.skip : 0
        this.paramsIndividuos.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.individuos = false
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
      async getProcedimentos (val) {
        this.loading.procedimentos = true
        this.paramsProcedimentos = {
          ...this.paramsProcedimentos,
          estabelecimentoId: val
        }
        /* let { data, paginacao, status } = await _api.estabelecimentos.getEstabelecimentoProcedimentos(this.paramsProcedimentos) */
        let { data, paginacao, status } = await _api.estabelecimentosProcedimentosHorarios.get(this.paramsProcedimentos)
        if (status === 502) this.loading.procedimentos = false

        const array = []
        data.forEach(element => {
          // console.log(element.procedimentoId)
          const exist = array.some(obj => obj.procedimentoId === element.procedimentoId)
          if (!exist) { array.push(element) }
        })

        this.api.procedimentos = (status === 200) ? array : []
        // console.log('array', array)

        this.loading.procedimentos = false
      },
      async getProfissionais () {
        this.loading.profissionais = true
        this.paramsProfissional.estabelecimentoId = this.formAgendamento.estabelecimentoId
        this.paramsProfissional.procedimentoId = this.formAgendamento.procedimentoId
        this.paramsProfissional.tipoDaConsulta = 'Totem'

        if (this.formAgendamento.dia != undefined) {
          // console.log("this.formAgendamento.dia", this.formAgendamento.dia)
          let dia = this.formAgendamento.dia.split('/')
          // console.log("dia", dia)

          this.paramsProfissional.DataInicial = new Date(dia[2], dia[1] - 1, dia[0])
          this.paramsProfissional.DataFinal = new Date(dia[2], dia[1] - 1, dia[0])

          // console.log('this.paramsProfissional.DiaInicial', this.paramsProfissional.DataInicial)
          // console.log('this.paramsProfissional.DiaFinal', this.paramsProfissional.DataFinal)
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
      async getHoras () {
        this.loading.horas = true
        // console.log("gethoras this.formagendamento: ", this.formAgendamento)
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
      async selectedPaciente () {
        this.disabledEstabelecimento = false
        await this.getEstabelecimentos()
      },
      async selectedEstabelecimento (val) {
        this.disabledProcedimento = false
        await this.getProcedimentos(val)
      },
      async selectedProcedimento (val) {
        if (this.demandaEspontanea) {
          // console.log(this.demandaEspontanea)
          // console.log('if')
          this.disabledProfissional = false
          await this.getProfissionais()
        } else {
          // console.log('else')
          this.disabledProfissional = true
          await this.getDias()
        }
        this.disabledDia = false
        // console.log(' this.disabledDia', this.disabledDia)
      },
      async selectedProfissional (val) {
        if (this.demandaEspontanea) {

        } else {
          this.disabledHora = false
          this.getHoras()
        }
      },
      async selectedRetorno (val) {
        this.disabledButton = false
      },
      async selectDia (val) {
        this.disabledProfissional = false
        await this.getProfissionais()
      },
      async selectedHora () {
        this.disabledButton = false
      },
      async onClickSalvar (form) {
        this.formAgendamento.numProntuario = this.mxProntuario(7)
        this.formAgendamento.tipoDeAtendimento = 2 // valor do Enum TipoDeAtendimento (Consulta agendada)
        let verificarProfissional = this.api.profissionais.filter(prof => {
          if (prof.id == this.formAgendamento.profissionalId) {
            return prof
          }
        })
        if (verificarProfissional.length != 0) {
          if (this.demandaEspontanea) {
            this.loading.agendamentos = true
            this.formAgendamento.tipoDaConsulta = 'Totem'
            _api.agendamentos.post(this.formAgendamento).then(res => {
              if (res.status === 201) {
                this.loading.agendamento = false
                this.resetForm(form)
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
            this.loading.agendamentos = true
            this.paramsAgendamento = {
              ...this.formAgendamento
            }
            let dia = this.formAgendamento.dia.split('/')

            this.paramsAgendamento.dia = new Date(dia[2], dia[1] - 1, dia[0])
            // console.log(" this.paramsAgendamento", this.paramsAgendamento)

            _api.agendamentos.post(this.paramsAgendamento).then(res => {
              if (res.status === 201) {
                this.loading.agendamento = false
                this.resetForm(form)
              } else {
                this.$swal({
                  title: "Erro!",
                  text: 'Ocorreu um erro ao criar o registro de agendamento!',
                  icon: 'error',
                })
                this.loading.agendamento = false
              }
            })
          }
        } else {
          this.$swal({
            title: "Atenção!",
            text: 'O profissional selecionado não esta cadastrado no procedimento!',
            icon: 'warning',
          })
          this.resetForm(form)
        }
      },

      resetForm () {
        this.disabledProcedimento = true
        this.disabledEstabelecimento = true
        this.disabledProfissional = true
        this.disabledDia = true
        this.disabledHora = true
        this.disabledButton = true

        this.formAgendamento.individuoId = null
        this.formAgendamento.dia = null
        this.formAgendamento.estabelecimentoId = null
        this.formAgendamento.procedimentoId = null
        this.formAgendamento.profissionalId = null
        this.formAgendamento.hora = null

        // console.log('this.formAgendamento: ', this.formAgendamento)
      }

    }
  }
</script>
<style>
  .vm--overlay {
    display: flex;
    width: 14%;
    /*background-color: transparent;*/
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

  p.title {
    font-size: 13px;
    margin-bottom: 0px
  }

  h4.name-profissional {
    font-size: 16px
  }
</style>
