<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
    <el-card class="box-card box-card--main-card">

      <el-row>
        <el-col :span="14">
          <h2 class="box-card--h2">{{$route.meta.title}} Agenda</h2>
        </el-col>
      </el-row>

      <el-row>
        <el-col :span="6">
          <!-- CALENDARIO -->
          <vue-cal :selected-date="dataSelecionada"
                   locale="pt-br"
                   class="vuecal--date-picker mini-calendario"
                   xsmall
                   hide-view-selector
                   :time="false"
                   :transitions="false"
                   :startWeekOnSunday="true"
                   active-view="month"
                   :events="eventos"
                   :disable-days="disabledDays"
                   @cell-click="dataSelecionada = $event"
                   @view-change="handleViewChange($event)"
                   @cell-focus="handleCellFocus"
                   @ready="handleCalReady($event)"
                   :disable-views="['week']"
                   style="width: 300px;height: 300px">
            <template #arrow-prev>
              <i class="fas fa-arrow-left"></i>
            </template>
            <template #arrow-next>
              <i class="fas fa-arrow-right"></i>
            </template>
          </vue-cal>

          <ul class="tipos-atendimento mt-2 list-unstyled">
            <li><div class="box-cor box-cor-horariosLivres"></div> Horários Livres</li>
          </ul>

          <ul class="tipos-atendimento mt-2 list-unstyled">
            <li><div class="box-cor box-cor-aguardandoTriagem"></div> Aguardando Triagem</li>
            <li><div class="box-cor box-cor-aguardandoAtendimentoMedico"></div> Aguardando Atendimento</li>
            <li><div class="box-cor box-cor-finalizado"></div> Finalizados</li>
            <li><div class="box-cor box-cor-canceladoAusente"></div> Cancelados/Ausentes</li>
          </ul>
        </el-col>
        <el-col :span="18">

          <!-- LISTA -->
          <vue-cal locale="pt-br"
                   :selected-date="dataSelecionada"
                   :time-from="$store.state.app.agenda.horaInicial"
                   :time-to="$store.state.app.agenda.horaFinal"
                   :time-step="$store.state.app.intervaloConsulta"
                   :disable-days="disabledDays"
                   :special-hours="specialHours"
                   :time-cell-height="130"
                   active-view="day"
                   :disable-views="['years', 'year', 'month']"
                   :hide-weekends="false"
                   @view-change="handleViewChange($event)"
                   @cell-click="handleSelectDateTime"
                   @cell-focus="dataSelecionada = $event.date || $event"
                   :events="eventos"
                   :on-event-click="onEventClick">

            <template #arrow-prev>
              <i class="fas fa-arrow-left"></i>
            </template>
            <template #arrow-next>
              <i class="fas fa-arrow-right"></i>
            </template>
          </vue-cal>
        </el-col>
      </el-row>
    </el-card>

    <el-dialog :visible.sync="showDialog" width="40%">
      <div style="display: flex; margin-top: -10px; margin-bottom: 5px; flex-direction: column; align-items: start">
        <h1 style="line-height: 24px; font-size: 18px; color: #303133; font-family: 'Roboto', arial, sans-serif; font-weight: normal">Agendamento Atual</h1>
      </div>
      <el-form :model="formDialogAgendamentoExistente" :rules="agendamentoExistenteValidacoes" ref="formDialogAgendamentoExistente" label-width="200px" label-position="left" class="forms--agendamento">
        <el-row :gutter="20">
          <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
            <el-form-item label="Remarcar Agendamento" prop="remarcarAgendamento">
              <el-radio-group fill="#4bc2ae" v-model="remarcarAgendamento" @change="remarcarAgendamentos">
                <el-radio-button v-for="item in enums.simNao" :label="item.value" :key="item.value">{{item.label}}</el-radio-button>
              </el-radio-group>
            </el-form-item>
            <el-form-item label="Dia" prop="dia">

              <el-select filterable placeholder="Selecione o Dia" :required="remarcarAgendamento == true" :disabled="remarcarAgendamento == false" v-model="formDiaHoraDialog.dia" @change="selectDiaDialog">
                <el-option v-for="(option, index) in api.diasDialog" :value="option" :label="option" :key="index" />
              </el-select>
            </el-form-item>
            <el-form-item label="Hora" prop="hora">
              <el-select filterable placeholder="Selecione o Horário 1" :required="remarcarAgendamento == true" v-loading.body="loading.horas" :disabled="disabledHoraDialog == true " v-model="formDiaHoraDialog.hora">
                <el-option v-for="(option, index) in api.horasDialog" :value="option" :label="option" :key="index" />
              </el-select>
            </el-form-item>
            <el-form-item label="Paciente" prop="individuoId">
              <el-select filterable placeholder="Selecione o paciente" v-model="formDialogAgendamentoExistente.individuoId"
                         v-loading.body="loading.individuos" disabled>
                <el-option v-for="option in api.individuos" :value="option.id" :label="option.nomeCompleto" :key="option.id" />
              </el-select>
            </el-form-item>
            <el-form-item label="Estabelecimento" prop="estabelecimentoId">
              <el-select filterable placeholder="Selecione o Estabelecimento" v-model="formDialogAgendamentoExistente.estabelecimentoId"
                         v-loading.body="loading.estabelecimentos" disabled>
                <el-option v-for="option in api.estabelecimentos" :value="option.id" :label="option.nomeFantasia" :key="option.id" />
              </el-select>
            </el-form-item>
            <el-form-item label="Tipo da Consulta" prop="tipoDaConsulta">
              <el-select clearable v-model="formDialogAgendamentoExistente.tipoDaConsulta" placeholder="Selecione o Tipo de Consulta" disabled>
                <el-option v-for="option in enums.tiposDeConsulta" :value="option.value" :label="option.label" :key="option.value" />
              </el-select>
            </el-form-item>
            <el-form-item label="Procedimento" prop="procedimentoId">
              <el-select clearable filterable placeholder="Selecione o Procedimento" v-model="formDialogAgendamentoExistente.procedimentoId"
                         v-loading.body="loading.procedimentos" disabled>
                <el-option v-for="option in api.procedimentosByParam" :value="option.id" :label="option.descricao" :key="option.id" />
              </el-select>
            </el-form-item>
            <el-form-item label="Médico" prop="profissionalId">
              <el-select clearable filterable placeholder="Selecione o Médico" v-model="formDialogAgendamentoExistente.profissionalId" disabled
                         v-loading.body="loading.profissionais">
                <el-option v-for="option in api.profissionaisByParam" :value="option.id" :label="option.nome" :key="option.id" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      <span slot="footer" class="dialog-footer">
        <div>
          <el-button type="success" v-if="!loading.reagendar && remarcarAgendamento" @click="onClickSalvarReagendamento"> Salvar</el-button>
          <el-button type="info" v-if="loading.reagendar && remarcarAgendamento" icon="fa fa-spinner fa-spin" disabled> Aguarde ...</el-button>
          <el-button type="danger" @click="handleCancelarReagendamento"> Cancelar</el-button>
        </div>
      </span>
    </el-dialog>

    <el-dialog @close="closeDialogAgendamento()" destroy-on-close title="Agendamento" :visible.sync="showModalAgendamento" width="40%">
      <el-form :model="formAgendamento" :rules="agendamentoValidacoes" ref="formAgendamento" label-width="200px" label-position="left" class="forms--agendamento">
        <el-row :gutter="20">
          <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
            <el-form-item label="Dia" prop="dia">
              <el-date-picker prefix-icon="fas fa-calendar-day" :picker-options="pickerOptions" v-model="formAgendamento.dia" type="date" format="dd-MM-yyyy" v-mask="'##-##-####'" />
            </el-form-item>
            <el-form-item label="Horário" prop="hora">
              <el-time-select :picker-options="pickerOptions" v-model="formAgendamento.hora" v-mask="'##:##'" />
            </el-form-item>
            <el-form-item label="Paciente" prop="individuoId">
              <el-select filterable placeholder="Selecione o paciente" v-model="formAgendamento.individuoId"
                         v-loading.body="loading.individuos" @change="onSelectPaciente">
                <el-option v-for="option in api.individuos" :value="option.id" :label="option.nomeCompleto" :key="option.id" />
              </el-select>
            </el-form-item>
            <el-form-item label="Estabelecimento" prop="estabelecimentoId">
              <el-select filterable placeholder="Selecione o Estabelecimento" v-model="formAgendamento.estabelecimentoId"
                         v-loading.body="loading.estabelecimentos" :disabled="!formAgendamento.individuoId" @change="onSelectEstabelecimento">
                <el-option v-for="option in api.estabelecimentos" :value="option.id" :label="option.nomeFantasia" :key="option.id" />
              </el-select>
            </el-form-item>
            <el-form-item label="Tipo da Consulta" prop="tipoDaConsulta">
              <el-select clearable v-model="formAgendamento.tipoDaConsulta" :disabled="!formAgendamento.estabelecimentoId || enums.tiposDeConsulta.length === 0" @change="onSelectTipoDaConsulta" placeholder="Selecione o Tipo de Consulta">
                <el-option v-for="option in enums.tiposDeConsulta" :value="option.value" :label="option.label" :key="option.value" />
              </el-select>
            </el-form-item>
            <el-form-item label="Especialidade" prop="procedimentoId">
              <el-select clearable filterable placeholder="Selecione a Especialidade" v-model="formAgendamento.procedimentoId"
                         v-loading.body="loading.procedimentos" :disabled="!formAgendamento.tipoDaConsulta" @change="onSelectProcedimento">
                <el-option v-for="(option, index) in api.procedimentos" :value="option.procedimento.id" :label="option.procedimento.descricao" :key="index" />
              </el-select>
            </el-form-item>
            <el-form-item label="Médico" prop="profissionalId">
              <el-select clearable filterable placeholder="Selecione o Médico" :disabled="!formAgendamento.procedimentoId" v-model="formAgendamento.profissionalId" @change="onSelectProfissional"
                         v-loading.body="loading.profissionais">
                <el-option v-for="option in api.profissionais" :value="option.id" :label="option.nome" :key="option.id" />
              </el-select>
            </el-form-item>
            <el-form-item v-show="!disabledRetorno" label="Retorno" prop="retorno">
              <el-radio-group fill="#4bc2ae" v-model="isRetorno" @change="onSelectRetorno">
                <el-radio-button v-for="item in enums.simNao" :label="item.value" :key="item.value">{{item.label}}</el-radio-button>
              </el-radio-group>
            </el-form-item>
            <el-form-item v-show="isRetorno" :required="formAgendamento.retorno" label="Agendamentos Anteriores" prop="retornoAgendamentoId">
              <el-select :disabled="!isRetorno" filterable placeholder="Selecione o Agendamento" v-model="formAgendamento.retornoAgendamentoId">
                <el-option v-for="item in api.agendamentosByIndividuo" :value="item.id" :label="item.dia" :key="item.id" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button @click="handleCancelaAgendar">Cancelar</el-button>
        <el-button type="success" :loading="loading.agendamento" @click="handleAgendar('formAgendamento')"> Agendar</el-button>
      </span>
    </el-dialog>

  </el-col>
</template>

<script>
  import VueCal from 'vue-cal'
  import 'vue-cal/dist/vuecal.css'
  import 'vue-cal/dist/i18n/pt-br.js'
  import Utils from '../../mixins/Utils'
  import _api from '../../api'
  import _enums from '../../api/Enums'
  import { mask } from 'vue-the-mask'
  import { Client as ConversationsClient } from '@twilio/conversations'
  import moment from 'moment'
  moment.locale('pt-br')

  export default {
    name: 'Agenda',
    components: { VueCal },
    mixins: [Utils],
    directives: { mask },
    data () {
      return {
        conversationsClient: null,
        options: {},
        isRetorno: false,
        disabledBtn: true,
        dataSelecionada: new Date(),
        disabledDays: [],
        horariosPeriodoSelecionado: [],
        specialHours: {
          1: [],
          2: [],
          3: [],
          4: [],
          5: [],
          6: [],
          7: []
        },
        // specialHours: {
        //   1: { from: 10 * 60, to: 12 * 60, class: 'horario-disponivel' },
        //   2: { },
        //   3: [
        //     { from: 7 * 60, to: 12 * 60, class: 'horario-disponivel' },
        //     { from: 14 * 60, to: 18 * 60, class: 'horario-disponivel' }
        //   ],
        //   4: {},
        //   5: {from: 10 * 60, to: 12 * 60, class: 'horario-disponivel'},
        //   6: { from: 8 * 60, to: 8 * 60 + 45, class: 'horario-disponivel' }, // hora inicial multiplica a hora por 60, final multiplica a hora inicial e soma a quantidade de minutos
        //   7: { }
        // },

        // dialog dos agendamentos ja existentes
        showDialog: false,
        formDialogAgendamentoExistente: {},
        formDiaHoraDialog: {
          dia: '',
          hora: ''
        },

        showModalAgendamento: false,

        disabledRetorno: true,
        disabledHoraDialog: true,
        remarcarAgendamento: false,
        consultaProfissional: {},
        formAgendamento: {
          retorno: false,
          dia: Date.now(),
          hora: Date.now()
        },
        autorizado: {
          id: this.$auth.user().id,
          nome: this.$auth.user().roles.includes('Recepcionista') ? 'Recepcionista' : null
        },
        enums: {
          sexos: _enums.sexos,
          racaOuCor: _enums.racaOuCor,
          tiposDeConsulta: [],
          retorno: [..._enums.retorno],
          simNao: _enums.simNao
        },
        pickerOptions: {
          disabledDate: this.disableBefore
        },
        api: {
          dias: [],
          agendamentos: [],
          agendamentosByIndividuo: [],
          profissionais: [],
          individuos: [],
          estabelecimentos: [],
          procedimentos: [],
          procedimentosByParam: [],
          profissionaisByParam: [],
          diasDialog: [],
          horasDialog: []
        },
        loading: {
          horas: false,
          dias: false,
          agendamentos: false,
          profissionais: false,
          individuos: false,
          estabelecimentos: false,
          procedimentos: false,
          agendamento: false,
          reagendar: false
        },
        total: {
          presencial: 0,
          totem: 0,
          teleconsulta: 0
        },
        eventos: [],
        params: {
          skip: 1,
          take: 999999,
          sort: 'DataCadastro ASC',
          total: 0
        },
        paramsIndividuos: {
          skip: 1,
          take: 1000,
          sort: 'i.NomeCompleto ASC',
          total: 0
        },
        paramsProf: {
          skip: 1,
          take: 1000,
          sort: 'Nome ASC',
          total: 0
        },
        paramsEstabelecimentos: {
          ativo: true,
          skip: 1,
          take: 1000,
          total: 0
        },
        paramsProcedimentos: {
          skip: 1,
          take: 1000,
          total: 0
        },
        agendamentoExistenteValidacoes: {
          dia: [{
            required: true, message: 'Campo obrigatório', trigger: 'submit'
          }],
          hora: [{
            required: true, message: 'Campo obrigatório', trigger: 'submit'
          }]
        },
        agendamentoValidacoes: {
          dia: [{
            required: true, message: 'Campo obrigatório', trigger: 'submit'
          }],
          hora: [{
            required: true, message: 'Campo obrigatório', trigger: 'submit'
          }],
          individuoId: [{ required: true, message: 'Campo obrigatório', trigger: 'submit' }],
          estabelecimentoId: [{ required: true, message: 'Campo obrigatório', trigger: 'submit' }],
          tipoDaConsulta: [{ required: true, message: 'Campo obrigatório', trigger: 'submit' }],
          procedimentoId: [{ required: true, message: 'Campo obrigatório', trigger: 'submit' }],
          profissionalId: [{ required: true, message: 'Campo obrigatório', trigger: 'submit' }]
          // retorno: [{ required: true, message: 'Campo obrigatório', trigger: 'submit' }]
        }
      }
    },
    async mounted () {
      await this.getAgendamentos()
    },
    methods: {
      closeDialogAgendamento () {
        this.formAgendamento = {}
        this.isRetorno = false
      },
      async getAgendamentos () {
        this.loading.agendamentos = true
        let { data, paginacao, status } = await _api.agendamentos.get(this.params)
        if (status === 502) this.loading.agendamentos = false
        this.api.agendamentos = (status === 200) ? data : []
        this.params.skip = (status === 200) ? paginacao.skip : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.agendamentos = false
        this.api.agendamentos.forEach(x => {
          let numEventos = this.api.agendamentos.filter(e => e.dia === x.dia && e.hora === x.hora).length;
          let width = 100 / numEventos;

          let horaInicial = this.moment(x.dia + ' ' + x.hora, 'YYYY-MM-DD HH:mm').format('YYYY-MM-DD HH:mm')
          let horaFinal = this.moment(horaInicial).add(this.$store.state.app.intervaloConsulta, 'minutes').format('YYYY-MM-DD HH:mm')
          let evento = {}
          if (this.mxHasAny('Recepcionista')) {
            evento = {
              start: horaInicial,
              end: horaFinal,
              title: `<strong>PACIENTE:</strong> ${x.individuo.nomeCompleto.toUpperCase()}<br />
                      <strong>TELEFONE:</strong> ${x.individuo.telefoneCelular}<br />
                      <strong>MÉDICO:</strong> ${x.profissional ? x.profissional.nome.toUpperCase() : ''}`,
              content: `<strong>Tipo da consulta: ${x.tipoDaConsulta}</strong><br />              
              (${this.moment(horaInicial).format('HH:mm')} - ${this.moment(horaFinal).format('HH:mm')})<br />`,
              class: `${((x.finalizado === true) && (x.presencaConfirmada === true) && (x.cancelado === false) ? 'Finalizado' : '') || ((x.cancelado === true || x.naoCompareceu === true) && (x.finalizado === true) ? 'canceladoAusente' : '') || ((x.presencaConfirmada === true) && (x.finalizado === false) ? 'aguardandoAtendimentoMedico' : '') || ((x.presencaConfirmada === false) && (x.finalizado === false) ? 'aguardandoTriagem' : '')}`
            }
          } else {
            evento = {
              start: horaInicial,
              end: horaFinal,
              title: `<strong>PACIENTE:</strong> ${x.individuo.nomeCompleto.toUpperCase()}<br />
                      <strong>TELEFONE:</strong> ${x.individuo.telefoneCelular}<br />`,
              content: `<strong>Tipo da consulta: ${x.tipoDaConsulta}</strong><br />
              (${this.moment(horaInicial).format('HH:mm')} - ${this.moment(horaFinal).format('HH:mm')})`,
              class: `${((x.finalizado === true) && (x.presencaConfirmada === true) && (x.cancelado === false) ? 'Finalizado' : '') || ((x.cancelado === true || x.naoCompareceu === true) && (x.finalizado === true) ? 'canceladoAusente' : '') || ((x.presencaConfirmada == true) && (x.finalizado == false) ? 'aguardandoAtendimentoMedico' : '') || ((x.presencaConfirmada == false) && (x.finalizado == false) ? 'aguardandoTriagem' : '')}`
            }
          }
          if (x.tipoDaConsulta === 'Teleconsulta') this.total.teleconsulta = this.total.teleconsulta + 1
          if (x.tipoDaConsulta === 'Presencial') this.total.presencial = this.total.presencial + 1
          if (x.tipoDaConsulta === 'Totem') this.total.totem = this.total.totem + 1

          evento.class += ` width-${numEventos}`;
          evento.style = `width: ${width}%;`;

          this.eventos.push(evento)
        })
      },
      async getProfissionais () {
        this.loading.profissionais = true
        this.paramsProf = {
          ...this.paramsProf,
          estabelecimentoId: this.formAgendamento.estabelecimentoId,
          procedimentoId: this.formAgendamento.procedimentoId,
          dia: this.moment(this.formAgendamento.dia).subtract(1, 'days').format('YYYY-MM-DD'),
          hora: this.formAgendamento.hora
        }
        this.paramsProf.estabelecimentoId = this.formAgendamento.estabelecimentoId
        this.paramsProf.procedimentoId = this.formAgendamento.procedimentoId
        let { data, status } = await _api.profissionais.getProfissionaisByProcedimento(this.paramsProf)
        if (status !== 200) this.loading.profissionais = false
        this.api.profissionais = (status === 200) ? data : []
        this.loading.profissionais = false
      },
      async getIndividuos () {
        this.loading.individuos = true
        let { data, paginacao, status } = await _api.individuos.getAll(this.paramsIndividuos)
        if (status !== 200) this.loading.individuos = false
        this.api.individuos = (status === 200) ? data : []
        this.paramsIndividuos.skip = (status === 200) ? paginacao.skip : 0
        this.paramsIndividuos.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.individuos = false
      },
      async getEstabelecimentos () {
        this.loading.estabelecimentos = true
        let { data, paginacao, status } = await _api.estabelecimentos.get(this.paramsEstabelecimentos)
        if (status !== 200) this.loading.estabelecimentos = false
        this.api.estabelecimentos = (status === 200) ? data.filter(estab => estab.nomeFantasia ? estab : null) : []
        this.paramsEstabelecimentos.skip = (status === 200) ? paginacao.skip : 0
        this.paramsEstabelecimentos.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.estabelecimentos = false
      },
      async getProcedimentos () {
        this.loading.procedimentos = true
        this.paramsProcedimentos = {
          ...this.paramsProcedimentos,
          estabelecimentoId: this.formAgendamento.estabelecimentoId,
          tipoDaConsulta: this.formAgendamento.tipoDaConsulta,
          dia: this.moment(this.formAgendamento.dia).subtract(1, 'days').format('YYYY-MM-DD'),
          hora: this.formAgendamento.hora
        }
        let { data, status } = await _api.estabelecimentosProcedimentosHorarios.get(this.paramsProcedimentos)

        let arrTiposDeConsulta = this._.uniqBy(data, 'tipoDaConsulta')
        if (!this.formAgendamento.tipoDaConsulta) {
          arrTiposDeConsulta.forEach(item => {
            let found = this.enums.tiposDeConsulta.find(x => x.value === item.value)
            // foi solicitado que o tipo de consulta teleconsulta não apareça na lista de agendamento
            if (!found && item.tipoDaConsulta !== 'Teleconsulta') {
              this.enums.tiposDeConsulta.push({
                label: item.tipoDaConsulta,
                value: item.tipoDaConsulta
              })
            }
          })
        }
        if (data.length === 0) {
          this.$swal({
            title: "Atenção!",
            text: 'Este horário não está disponível para este Estabelecimento!',
            icon: 'warning',
          })
        }
        if (status !== 200) this.loading.procedimentos = false
        const array = []
        data.forEach(element => {
          const exist = array.some(obj => obj.procedimentoId === element.procedimentoId)
          if (!exist) { array.push(element) }
        })
        this.api.procedimentos = (status === 200) ? array : []
        this.loading.procedimentos = false
      },
      async getDias (mes, ano) {
        var count = this.moment().year(ano).month(mes).daysInMonth()

        var diasDoMes = []
        for (var i = 1; i < count + 1; i++) {
          diasDoMes.push(this.moment().year(ano).month(mes).date(i).format('YYYY-MM-DD'))
        }

        let params = {
          dataInicial: this.moment().year(ano).month(mes).startOf('month').format('YYYY-MM-DD'),
          dataFinal: this.moment().year(ano).month(mes).endOf('month').format('YYYY-MM-DD')
        }

        this.loading.dias = true
        let { data, status } = await _api.estabelecimentosProcedimentosHorarios.get(params)

        this.horariosPeriodoSelecionado = this._.uniqBy(data, 'dataHora').map(x => {
          return {
            dia: this.moment(x.dia).format('YYYY-MM-DD'),
            hora: x.hora,
            profissionalId: x.profissionalId
          }
        }).sort()

        if (status !== 200) this.loading.dias = false
        const array = []
        if (status !== 200) this.api.dias = []
        data.forEach(element => {
          const exist = array.some(obj => obj === element.dia)
          if (!exist) array.push(element.dia)
        })

        this.api.dias = array.map(element => {
          return this.moment(element).format('YYYY-MM-DD')
        })
        diasDoMes.forEach(dia => {
          let found = this.api.dias.find(x => x === dia)
          if (!found) {
            this.disabledDays.push(dia)
          }
        })
        this.loading.dias = false
      },
      async getHorariosDoDia (val) {
        let agrupePorDia = this._.groupBy(this.horariosPeriodoSelecionado, 'dia')
        let horarios = agrupePorDia[val]
        let dayNumber = this.moment(val).day()
        this.specialHours[dayNumber] = []
        if (this.mxHasAny('Recepcionista') && horarios && horarios.length > 0) {
          horarios.forEach(x => {
            let hour = parseInt(x.hora.split(':')[0])
            let minute = parseInt(x.hora.split(':')[1])
            this.specialHours[dayNumber].push(
              { from: hour * 60 + minute, to: hour * 60 + minute + this.$store.state.app.intervaloConsulta, class: 'horario-disponivel' }
            )
          })
        }
        if (horarios && horarios.length > 0) {
          horarios.forEach(x => {
            let hour = parseInt(x.hora.split(':')[0])
            let minute = parseInt(x.hora.split(':')[1])
            if (x.profissionalId === this.$auth.user().id) {
              this.specialHours[dayNumber].push(
                { from: hour * 60 + minute, to: hour * 60 + minute + this.$store.state.app.intervaloConsulta, class: 'horario-disponivel' }
              )
            }
          })
        }
      },
      disableBefore (date) {
        // return date < Date.now()
      },
      async handleCellFocus () {
        //
      },
      async handleViewChange (val) {
        this.disabledDays = []
        let mes = this.moment(val.startDate).month()
        let ano = this.moment(val.startDate).year()
        let data = this.moment(val.startDate).format('YYYY-MM-DD')
        await this.getDias(mes, ano)
        await this.getHorariosDoDia(data)
      },
      async handleCalReady (val) {
        let mes = this.moment(val.startDate).month()
        let ano = this.moment(val.startDate).year()
        let data = this.moment().format('YYYY-MM-DD')
        await this.getDias(mes, ano)
        await this.getHorariosDoDia(data)
      },

      async onEventClick (event, e) {
        // retornando dia e hora
        var diaHora = this.moment(`${event.start}`).format('YYYY-MM-DD HH:mm:ss')
        var splitDiaHora = diaHora.split(' ')
        var dia = this.moment(splitDiaHora[0]).format('YYYY-MM-DD HH:mm:ss').replace(' ', 'T')
        var hora = splitDiaHora[1]

        // fazendo o filter no api agendamentos para retornar o agendamento
        var agendamento = this.api.agendamentos.filter(agendamento => {
          if (
            (agendamento.dia === dia) &&
            (agendamento.hora === hora)
          ) {
            return agendamento
          }
        })

        if (agendamento.length > 0) {
          this.formDialogAgendamentoExistente = agendamento[0]
          this.formDiaHoraDialog.dia = agendamento[0].dia
          this.formDiaHoraDialog.hora = agendamento[0].hora
          this.converterDiaHoraDialog()

          this.getIndividuos()
          this.getEstabelecimentos()
          this.getProcedimentosByParam()
          this.getProfissionaisByParam()
          this.getDiasDialog()

          this.showDialog = true
        }
        // STOP NO EVENTO
        e.stopPropagation()
      },
      // CONVERTER DATA (teve que ser feito fora pois o moment dentro do evento quebrava o mesmo)
      converterDiaHoraDialog () {
        this.formDiaHoraDialog.dia = this.moment(this.formDiaHoraDialog.dia).format('DD/MM/YYYY')
      },

      // retorna todos os procedimentos do banco
      async getProcedimentosByParam () {
        var params = {
          skip: 1,
          take: 1000000,
          sort: 'Descricao ASC',
          total: 0
        }

        let { data, status } = await _api.procedimentos.get(params)
        this.api.procedimentosByParam = (status === 200) ? data : []
      },

      // retorna os profissionais do banco
      async getProfissionaisByParam () {
        var params = {
          skip: 1,
          take: 100000,
          sort: 'Nome ASC',
          total: 0
        }
        let { data, status } = await _api.profissionais.get(params)
        this.api.profissionaisByParam = (status === 200) ? data : []
      },

      // retorna os dias
      async getDiasDialog () {
        var form = {
          estabelecimentoId: this.formDialogAgendamentoExistente.estabelecimentoId,
          procedimentoId: this.formDialogAgendamentoExistente.procedimentoId,
          profissionalId: this.formDialogAgendamentoExistente.profissionalId,
          tipoDaConsulta: this.formDialogAgendamentoExistente.tipoDaConsulta
        }

        let { data, status } = await _api.estabelecimentosProcedimentosHorarios.get(form)

        const array = []
        if (status !== 200) this.api.diasDialog = []
        data.forEach(element => {
          const exist = array.some(obj => obj === element.dia)
          if (!exist) array.push(element.dia)
        })
        // console.log("array", array)

        array.forEach(element => {
          var data = this.moment(element).format('DD/MM/YYYY')
          this.api.diasDialog.push(data)
        })

        // console.log("this.api.diasDialog", this.api.diasDialog)
      },

      async selectDiaDialog () {
        this.disabledHoraDialog = false
        this.getHorasDialog()
      },

      async getHorasDialog () {
        this.loading.horas = true

        let dia = this.formDiaHoraDialog.dia.split('/')
        // console.log("dia", dia)
        let diaNovo = `${dia[2]}-${dia[1]}-${dia[0]}`

        let paramsHoras = {

          estabelecimentoId: this.formDialogAgendamentoExistente.estabelecimentoId,
          procedimentoId: this.formDialogAgendamentoExistente.procedimentoId,
          profissionalId: this.formDialogAgendamentoExistente.profissionalId,
          tipoDaConsulta: this.formDialogAgendamentoExistente.tipoDaConsulta,
          dia: diaNovo

        }
        let { data, status } = await _api.estabelecimentosProcedimentosHorarios.get(paramsHoras)
        if (status === 502) this.loading.horas = false
        const array = []
        if (status !== 200) this.api.horasDialog = []
        if (data.length > 0) {
          data.forEach(element => {
            const exist = array.some(obj => obj === element.hora)
            if (!exist) array.push(element.hora)
          })

          this.api.horasDialog = array
        }
        this.loading.horas = false
      },

      async remarcarAgendamentos () {
        if (this.remarcarAgendamento == true) {
          this.formDiaHoraDialog.dia = ''
          this.formDiaHoraDialog.hora = ''
        } else if (this.remarcarAgendamento == false) {
          this.disabledHoraDialog = true
          this.formDiaHoraDialog.dia = this.moment(this.formDialogAgendamentoExistente.dia).format('DD/MM/YYYY')
          this.formDiaHoraDialog.hora = this.formDialogAgendamentoExistente.hora
        } else {
          this.disabledHoraDialog = true
          this.formDiaHoraDialog.dia = this.moment(this.formDialogAgendamentoExistente.dia).format('DD/MM/YYYY')
          this.formDiaHoraDialog.hora = this.formDialogAgendamentoExistente.hora
        }
      },

      async onClickSalvarReagendamento () {
        this.loading.reagendar = true

        // se for remarcar agendamento setando o dia e hora
        if (this.remarcarAgendamento == true) {
          var diaSplit = this.formDiaHoraDialog.dia.split('/')
          var dia = `${diaSplit[2]}-${diaSplit[1]}-${diaSplit[0]}`

          // montando o form para setar como situação 0 o horario anterior
          var form = {
            dataInicial: this.formDialogAgendamentoExistente.dia,
            dataFinal: this.formDialogAgendamentoExistente.dia,
            dia: this.formDialogAgendamentoExistente.dia,
            hora: this.formDialogAgendamentoExistente.hora,
            tipoDaConsulta: this.formDialogAgendamentoExistente.tipoDaConsulta,
            profissionalId: this.formDialogAgendamentoExistente.profissionalId,
            procedimentoId: this.formDialogAgendamentoExistente.procedimentoId,
            estabelecimentoId: this.formDialogAgendamentoExistente.estabelecimentoId,
            situacao: 1
          }

          this.formDialogAgendamentoExistente.dia = dia
          this.formDialogAgendamentoExistente.hora = this.formDiaHoraDialog.hora

          // remarca e seta como situação 1 no estabelecimento procedimento horario
          let { data, status } = await _api.agendamentos.putAgendamento(this.formDialogAgendamentoExistente)

          if (status === 204) {
            // criando os params para recuperar o id do estabelecimentoProcedimentoHorario
            var params = {
              ...form,
              skip: 1,
              take: 100000
            }

            // retornando a lista com item do horario
            let { data: dataEPH, status: statusEPH } = await _api.estabelecimentosProcedimentosHorarios.get(params)
            // console.log("dataEPH", dataEPH)
            // console.log("statusEPH", statusEPH)

            // o data é para retornar só 1
            if (dataEPH.length === 1) {
              // o put(update) no repository esta forçando a ser liberado o horario (situação 0)
              let { data: dataPutEPH, status: statusPutEPH } = await _api.estabelecimentosProcedimentosHorarios.put(dataEPH[0].id, dataEPH[0])
              if (statusPutEPH != 200) {
                this.$swal({
                  title: "Erro!",
                  text: 'Ocorreu um erro ao liberar o horário anterior!',
                  icon: 'error',
                })
              }
              // console.log("statusPutEPH", statusPutEPH)
            } else {
              console.log('erro get horario')
            }

            this.formDiaHoraDialog.dia = ''
            this.formDiaHoraDialog.hora = ''
            this.formDialogAgendamentoExistente = {}
            this.disabledHoraDialog = true
            this.remarcarAgendamento = false
            this.api.diasDialog = []
            this.api.horasDialog = []
            this.api.agendamentos = []
            this.eventos = []

            await this.getAgendamentos()
            this.showDialog = false
          }
        }
        this.loading.reagendar = false
      },
      async handleCancelarReagendamento () {
        this.formDiaHoraDialog.dia = ''
        this.formDiaHoraDialog.hora = ''
        this.formDialogAgendamentoExistente = {}
        this.disabledHoraDialog = true
        this.remarcarAgendamento = false
        this.showDialog = false
      },

      async handleSelectDateTime (val) {
        if (!this.mxHasAny('Recepcionista')) return
        if (this.moment(val) < this.moment()) {
          this.$swal({
            title: "Atenção!",
            text: 'O horário escolhido é menor que o horário atual! Preencha novamente!',
            icon: 'warning',
          })
          return
        }
        let dataSelecionada = this.moment(val).add(1, 'days').format('YYYY-MM-DD')
        let horaSelecionada = this.moment(val).format('HH')
        let minutoSelecionado = this.moment(val).format('mm')
        if (minutoSelecionado < 15 && minutoSelecionado < 30) minutoSelecionado = 0
        if (minutoSelecionado > 15 && minutoSelecionado < 30) minutoSelecionado = 15
        if (minutoSelecionado > 30 && minutoSelecionado < 45) minutoSelecionado = 30
        if (minutoSelecionado > 45 && minutoSelecionado < 60) minutoSelecionado = 45
        this.formAgendamento.dia = dataSelecionada
        this.formAgendamento.hora = this.moment(`${horaSelecionada}:${minutoSelecionado}`, 'HH:mm').format('HH:mm')
        this.showModalAgendamento = true
        this.getIndividuos()
      },
      async onSelectPaciente () {
        await this.getEstabelecimentos()
      },
      async onSelectEstabelecimento () {
        this.enums.tiposDeConsulta = []
        this.formAgendamento.tipoDaConsulta = null
        this.formAgendamento.tipoDaConsulta = null
        this.formAgendamento.procedimentoId = null
        await this.getProcedimentos()
      },
      async onSelectTipoDaConsulta () {
        await this.getProcedimentos()
      },
      async onSelectProcedimento () {
        await this.getProfissionais()
      },
      async onSelectProfissional () {
        this.disabledRetorno = false
      },
      async onSelectRetorno (val) {
        // se for retorno
        if (val === true) {
          delete this.formAgendamento.retornoAgendamentoId
          // retorno -> true para entrar no if no repository
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
          let { data, status } = await _api.agendamentos.get(paramsRetorno)
          this.api.agendamentosByIndividuo = (status === 200) ? data : []
          // Alterando as informações que irá aparecer no el-select Agendamentos Anteriores
          this.api.agendamentosByIndividuo.forEach(item => {
            item.dia = 'Realizado em: ' + this.moment(item.dia).format('DD/MM/YYYY') + ' às ' + item.hora + ' - ' + item.tipoDaConsulta + ' - ' + 'Médico(a): ' + item.profissional.nome + ' - ' + 'Especialidade: ' + item.procedimento.descricao
          })
        } else {
          this.consultaProfissional = {}
          delete this.formAgendamento.retornoAgendamentoId
        }
      },
      handleAgendar (form) {
        this.loading.agendamentos = true
        this.formAgendamento = {
          ...this.formAgendamento,
          presencaConfirmada: (this.formAgendamento.tipoDaConsulta === 'Teleconsulta'),
          pago: !this.isRetorno,
          dia: this.moment(this.formAgendamento.dia).subtract(1, 'days').format('YYYY-MM-DD')
        }
        this.$refs[form].validate((valid) => {
          if (valid) {
            _api.agendamentos.post(this.formAgendamento).then(async (res) => {
              if (res.status === 201) {
                this.loading.agendamentos = false
                this.options = {
                  agendamentoId: res.data,
                  individuoId: this.formAgendamento.individuoId,
                  profissionalId: this.formAgendamento.profissionalId
                }
                await this.getConversationClient()

                this.eventos = []
                await this.getAgendamentos()

                this.showModalAgendamento = false
                this.formAgendamento = {}
                this.$refs.formAgendamento.resetFields()
              } else {
                this.$swal({
                  title: "Erro!",
                  text: 'Ocorreu um erro ao realizar o registro do agendamento!',
                  icon: 'error',
                })
                this.loading.agendamento = false
              }
            })
          } else {
            this.$swal({
              title: "Atenção!",
              text: 'Verifique o preenchimento do formulário!',
              icon: 'warning',
            })
            this.loading.modelos = false
            return false
          }
        })
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
          const joinedConversation = await this.conversationsClient.createConversation({ uniqueName: uniqueName })
          await joinedConversation.add(this.options.individuoId).catch(err => console.log(err))
          await joinedConversation.add(this.options.profissionalId).catch(err => console.log(err))
        } catch (e) {
          const joinedConversation = await this.conversationsClient.getConversationByUniqueName(uniqueName)
          await joinedConversation.add(this.options.individuoId).catch(err => console.log(err))
          await joinedConversation.add(this.options.profissionalId).catch(err => console.log(err))
        }
      },

      handleCancelaAgendar () {
        this.showModalAgendamento = false
        this.formAgendamento = {}
        this.$refs.formAgendamento.resetFields()
      }
    }
  }
</script>

<style>

    .tipos-atendimento {
      list-style: none;
      font-size: 14px;
      font-weight: 400;
    }

    .box-cor {
      width: 15px;
      height: 15px;
      border: 1px solid #1414149f;
      float: left;
      margin-right: 5px;
      margin-top: 2px;
    }

    .box-cor-canceladoAusente {
      background-color: #faafaf;
    }

    .box-cor-finalizado {
      background-color: #94e3a8;
    }

    .box-cor-aguardandoAtendimentoMedico {
      background-color: #90c7fc;
    }

    .box-cor-aguardandoTriagem {
      background-color: #b8ccde;
    }

    .box-cor-horariosLivres {
      background-color: #d5ffe0;
    }

    .cor-presencial {
      color: #1414149f;
    }

    .box-cor-presencial {
      background: #D9E8F2;
    }

    .cor-totem {
      color: #1414149f;
    }

    .box-cor-totem {
      background: #ECF9F9;
    }

    .cor-teleconsulta {
      color: #1414149f;
    }

    .box-cor-teleconsulta {
      background: #E9EDEF;
    }






    /*.tipo-consulta__presencial {background-color: #D9E8F2;}
  .tipo-consulta__totem {background-color:#ECF9F9; }
  .tipo-consulta__teleconsulta {background-color: #E9EDEF;}*/

  .vuecal__event.canceladoAusente {
    background-color: #faafaf;
  }

  .vuecal__event.Finalizado {
    background-color: #94e3a8;
  }

  .vuecal__event.aguardandoAtendimentoMedico {
    background-color: #90c7fc;
  }

  .vuecal__event.aguardandoTriagem {
    background-color: #b8ccde;
  }

  .vuecal__event.width-1 {
    width: 90% !important;
  }

  .vuecal__event.width-2 {
    width: 45% !important;
  }

  .vuecal__event.width-3 {
    width: 30% !important;
  }

  .vuecal__event.width-4 {
    width: 22.5% !important;
  }


    .vuecal__flex {
      font-size: 14px;
      font-family: 'Roboto', arial, sans-serif;
    }

    .vuecal__cell--disabled {
      text-decoration: line-through;
      color: #bbb;
    }

    .vuecal__weekdays-headings, .vuecal__all-day {
      padding-right: 0px !important;
    }

    .vuecal__event {
      cursor: pointer;
    }

    .vuecal__event-title {
      padding-top: 2px;
      font-size: 12px;
    }

    .vuecal__event .vuecal__event-time {
      display: none !important;
      visibility: hidden !important;
      height: 1px;
    }

    /* dots
  .vuecal__cell-events-count {
    width: 4px;
    min-width: 0;
    height: 4px;
    margin: 5px 0 0 0;
    color: transparent;
  }
   */
    .mini-calendario .vuecal__cell.vuecal__cell--has-events {
      background: #d3e7e7 !important;
    }

    .vuecal__cell-events-count {
      display: none;
    }

    /*.el-form-item {
      margin-bottom: 0 !important;
  }*/

    .horario-disponivel {
      background-color: #d5ffe0;
      /* background-color: rgba(0, 255, 55, 0.15); */
      /* border-bottom: 1px solid rgba(0, 255, 55, 0.3); */
      cursor: pointer;
    }

    .closed {
      background: #fff7f0 repeating-linear-gradient( -45deg, rgba(255, 162, 87, 0.25), rgba(255, 162, 87, 0.25) 5px, rgba(255, 255, 255, 0) 5px, rgba(255, 255, 255, 0) 15px );
      color: #f6984c;
    }

    .el-input.is-disabled .el-input__inner {
      color: black;
    }
</style>
