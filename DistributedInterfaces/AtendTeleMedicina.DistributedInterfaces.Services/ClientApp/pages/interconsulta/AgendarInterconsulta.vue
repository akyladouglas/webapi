<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
    <el-card class="box-card box-card--main-card">
      <el-row>
        <el-col :span="14">
          <h2 class="box-card--h2">Agendar Interconsulta</h2>
        </el-col>
        <el-col :span="10" class="text-right">
          <el-form :inline="true">
            <el-form-item>
              <el-button style="margin-right: -10px" type="warning" @click="onClickVoltar"> Voltar</el-button>
            </el-form-item>
          </el-form>
        </el-col>
      </el-row>


      <el-form :model="formInterconsulta" :rules="validacoes" ref="formInterconsulta" label-width="170px" status-icon>

        <el-form-item label="Motivo:" prop="motivo">
          <el-input v-model="formInterconsulta.motivo" :rows="2" type="textarea" placeholder="Digite o motivo da interconsulta" autosize maxlength="250" />
        </el-form-item>

        <el-form-item label="Paciente presente:" prop="pacientePresente" label-width="180px">
          <el-radio-group style="padding-top: 10px" v-model="formInterconsulta.pacientePresente" @change="changePacientePresente">
            <el-radio :label="true">Sim</el-radio>
            <el-radio :label="false">Não</el-radio>
          </el-radio-group>
        </el-form-item>


        <el-row :gutter="24">
          <el-col :xs="12" :sm="12" :md="12" :lg="12" :xl="12">
            <el-form-item label="Paciente:" prop="individuoId">
              <el-select filterable placeholder="Selecione o paciente" v-model="formInterconsulta.individuoId"
                         v-loading.body="loading.individuos" @change="selectedIndividuo">
                <el-option v-for="option in api.individuos" :value="option.id" :label="option.nomeCompleto" :key="option.id" />
              </el-select>
            </el-form-item>
          </el-col>

          <el-col :xs="12" :sm="12" :md="12" :lg="12" :xl="12">
            <el-form-item label="Estabelecimento:" prop="estabelecimentoId" label-width="130px">
              <el-select filterable placeholder="Selecione o estabelecimento" v-model="formInterconsulta.estabelecimentoId"
                         v-loading.body="loading.estabelecimentos" @change="selectedEstabelecimento" :disabled="disabled.estabelecimento || formInterconsulta.profissionaisIds.length > 0">
                <el-option v-for="option in api.estabelecimentos" :value="option.id" :label="option.nomeFantasia" :key="option.id" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="24">
          <el-col :xs="12" :sm="12" :md="12" :lg="12" :xl="12">
            <el-form-item label="Procedimento:" prop="procedimentoId">
              <el-select filterable placeholder="Selecione o procedimento" v-model="formInterconsulta.procedimentoId"
                         v-loading.body="loading.procedimentos" @change="selectedProcedimento" :disabled="disabled.procedimento || formInterconsulta.profissionaisIds.length > 0">
                <el-option v-for="option in api.procedimentos" :value="option.procedimento.id" :label="option.procedimento.descricao" :key="option.procedimento.id" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :xs="12" :sm="12" :md="12" :lg="12" :xl="12">
            <el-form-item label="Data do evento:" prop="data" label-width="130px">

              <el-date-picker v-model="formInterconsulta.dia"
                              type="date"
                              placeholder="Selecione a data"
                              @change="selectedData"
                              :disabled="disabled.dia || formInterconsulta.profissionaisIds.length > 0"
                              disabledDate
                              :picker-options="pickerOptions"
                              format="dd/MM/yyyy">
              </el-date-picker>
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="24">
          <el-col :xs="12" :sm="12" :md="12" :lg="12" :xl="12">
            <el-form-item label="Horário:" prop="hora">
              <el-select filterable placeholder="Selecione a hora" v-model="formInterconsulta.hora"
                         v-loading.body="loading.horas" @change="selectedHora" :disabled="disabled.hora || formInterconsulta.profissionaisIds.length > 0">
                <el-option v-for="option in api.horas" :value="option" :label="`${option.substr(0, 5)}`" :key="option" />
              </el-select>
            </el-form-item>
          </el-col>

          <el-col :xs="12" :sm="12" :md="12" :lg="12" :xl="12">
            <el-form-item label="Profissionais:" prop="profissionais" label-width="130px">
              <el-select filterable multiple placeholder="Selecione os profissionais" v-model="formInterconsulta.profissionaisIds" @remove-tag="remove"
                         v-loading.body="loading.profissionais" @change="selectedProfissionais" :disabled="disabled.profissional">
                <el-option v-for="option in api.profissionais" :value="option.id" :label="option.nome" :key="option.id" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>

        
      </el-form>

      <el-row :gutter="24" v-if="api.profissionaisSelecionados.length > 0" style="margin-top: 5px">
        <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
          <h5>Participantes</h5>
          <el-table ref="tableProfissionaisSelecionados" :data="api.profissionaisSelecionados"
                    highlight-current-row border
                    class="table--agendamentos table--row-click">
            <el-table-column label="Nome" prop="nome" />
            <el-table-column label="Tipo" prop="tipo" />        
            <el-table-column label="Participante" prop="participante" width="130" />
          </el-table>
        </el-col>
      </el-row>

      <el-row :gutter="24" style="margin-top: 15px">
        <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" style="display: flex; flex-direction: row; align-items: center; justify-content: center;">
          <el-button flat icon="fas fa-save" type="success" :disabled="disabled.button" @click="onClickSalvar()"> Salvar</el-button>
          <el-tooltip content="Botão para limpar todo o formulário e tabela de profissionais selecionados" placement="top">
            <el-button flat icon="fas fa-eraser" type="default" @click="resetForm()"> Limpar</el-button>
          </el-tooltip>
        </el-col>
      </el-row>
    </el-card>

  </el-col>
</template>

<script>
  import _api from '../../api'
  import { Client as ConversationsClient } from '@twilio/conversations'
  import moment from 'moment'
  moment.locale('pt-br')
  export default {
    name: 'Interconsulta',
    data() {
      var validaProfissionais = (rule, value, callback) => {
        return callback()
      }

      var validaData = (rule, value, callback) => {
        if(this.formInterconsulta.dia) return callback()
        else return callback(new Error('Campo obrigatório'))
      }


      return {
        pickerOptions: {
          disabledDate: this.getDatasDisponiveis
        },

        conversationsClient: null,
        options: {},

        formInterconsulta: {
          pacientePresente: false,
          profissionaisIds: [],
        },

        api: {
          profissionais: [],
          dias: [],
          horas: [],
          profissionaisSelecionados: [],
          individuos: [],
          estabelecimentoProcedimentoHorario: [],
          estabelecimentos: [],
          procedimentos: [],
        },

        loading: {
          profissionais: false,
          dias: false,
          horas: false,
          individuos: false,
          estabelecimentos: false,
          procedimentos: false,
        },

        disabled: {
          profissional: true,
          dia: true,
          hora: true,
          button: true,
          estabelecimento: true,
          procedimento: true
        },

        validacoes: {
          motivo: [
            { required: true, message: 'Campo obrigatório', trigger: ['submit']},
          ],
          pacientePresente: [
            { required: true, message: 'Campo obrigatório', trigger: ['blur', 'submit']},
          ],
          individuoId: [
            { required: true, message: 'Campo obrigatório', trigger: ['blur', 'submit'] },
          ],
          estabelecimentoId: [
            { required: true, message: 'Campo obrigatório', trigger: ['blur', 'submit'] },
          ],
          procedimentoId: [
            { required: true, message: 'Campo obrigatório', trigger: ['blur', 'submit'] },
          ],
          data: [
            { required: true, validator: validaData, trigger: ['submit']},
          ],
          hora: [
            { required: true, message: 'Campo obrigatório', trigger: ['blur', 'submit']},
          ],
          profissionais: [
            { required: true, validator: validaProfissionais, trigger: ['blur', 'submit']},
          ],
        },

        paramsProfissional: {
          skip: 1,
          take: 10000,
          total: 0
        },

        paramsIndividuo: {
          skip: 1,
          take: 10000,
          sort: 'i.NomeCompleto ASC',
          total: 0,
          ativo: true,
        },

        paramsEst: {
          skip: 1,
          take: 10000,
          sort: 'e.NomeFantasia ASC',
          total: 0,
          ativo: true,
        },

        paramsProcedimentos: {
          skip: 1,
          take: 10000,
          total: 0
        },
      };
    },

    watch: {
      'api.profissionaisSelecionados': function (newProfissionais, oldProfissionais) {
        let temAnfitriao = newProfissionais.some(profissional => profissional.participante === "Anfitrião")

        if (temAnfitriao) this.disabled.button = false
        else this.disabled.button = true
      },
    },

    async mounted() {
      await this.getEstabelecimentoProcedimentoHorario()
      await this.getIndividuos()
    },

    methods: {
      async getEstabelecimentoProcedimentoHorario() {
        let { data, status } = await _api.estabelecimentosProcedimentosHorarios.get({ skip: 1, take: 100000, estabelecimentoId: this.formInterconsulta.estabelecimentoId })
        if (data.length === 0) {
          return this.$swal({
            title: "Atenção!",
            text: 'Não há horarios liberados!',
            icon: 'warning',
          })
        }
        if (data.length > 0) {
          this.api.estabelecimentoProcedimentoHorario = data
        }
      },
      
      async getEstabelecimentos() {
        this.loading.estabelecimentos = true
        let { data, status } = await _api.estabelecimentos.get(this.paramsEst)
        if (status === 502) this.loading.estabelecimentos = false
        this.api.estabelecimentos = (status === 200) ? data : []
        this.loading.estabelecimentos = false
      },

      async getProcedimentos() {
        this.loading.procedimentos = true
        this.paramsProcedimentos.estabelecimentoId = this.formInterconsulta.estabelecimentoId
        let { data, status } = await _api.estabelecimentosProcedimentosHorarios.get(this.paramsProcedimentos)

        if (status === 502) this.loading.procedimentos = false

        const array = []
        data.forEach(element => {
          const exist = array.some(obj => obj.procedimentoId === element.procedimentoId)
          if (!exist) { array.push(element) }
        })

        this.api.procedimentos = (status === 200) ? array : []
        this.loading.procedimentos = false
      },

      async selectedData() {
        this.api.horas = []
        if (this.formInterconsulta.hora) {
          delete this.formInterconsulta.hora
        }

        await this.getHora()
        if (this.api.horas.length > 0) this.disabled.hora = false
        else {
          this.disabled.hora = true
          this.$swal({
            title: "Atenção!",
            text: 'Não há horários disponíveis para este dia!',
            icon: 'warning',
          })
        }
      },

      getHora() {
        this.loading.horas = true
        this.api.estabelecimentoProcedimentoHorario.forEach(obj => {
          //traz todas as horas baseado no dia selecionado
          if (obj.dia === this.moment(this.formInterconsulta.dia).format('YYYY-MM-DDTHH:mm:ss')) {
            if (!this.api.horas.some(hora => hora === obj.hora)) {
              this.api.horas.push(obj.hora)
            }
          }
        })
        this.loading.horas = false
      },

      async selectedHora() {
        this.api.profissionais = []

        await this.getProfissionais()
        if (this.api.profissionais.length > 0) this.disabled.profissional = false
        else {
          this.disabled.profissional = true
          this.$swal({
            title: "Atenção!",
            text: 'Não há profissionais disponíveis para este dia e hora!',
            icon: 'warning',
          })
        }
      },

      getProfissionais() {
        this.loading.profissionais = true

        this.api.estabelecimentoProcedimentoHorario.forEach(obj => {
          //traz todos os profissionais baseado no dia e hora selecionado
          if (obj.dia === this.moment(this.formInterconsulta.dia).format('YYYY-MM-DDTHH:mm:ss') && obj.hora === this.formInterconsulta.hora) {
            if (!this.api.profissionais.some(profissional => profissional.id === obj.profissional.id)) {
              this.api.profissionais.push(obj.profissional)
            }
          }
        })

        this.loading.profissionais = false
      },

      selectedProfissionais() {
        const selectedProfissionais = this.api.estabelecimentoProcedimentoHorario
          .filter(obj => obj.dia === this.moment(this.formInterconsulta.dia).format('YYYY-MM-DDTHH:mm:ss') &&
            obj.hora === this.formInterconsulta.hora &&
            this.formInterconsulta.profissionaisIds.some(profissionalId => profissionalId === obj.profissional.id))
          .map(obj => ({
            id: obj.profissional.id,
            nome: obj.profissional.nome,
            tipo: obj.procedimento.descricao,
            participante: (obj.profissional.id === this.$auth.user().id) ? 'Anfitrião' : 'Convidado'
          }));

        // Adicione novos profissionais sem duplicatas
        selectedProfissionais.forEach(profissional => {
          if (!this.api.profissionaisSelecionados.some(item => item.nome === profissional.nome)) {
            this.api.profissionaisSelecionados.push(profissional);
          }
        });
      },

      async changePacientePresente() {
      },

      getDatasDisponiveis(date) {
        const indisponivel = date.getDay()
        return indisponivel === 6 || indisponivel === 0 || date < Date.now() - 86400000
      },

      async getIndividuos() {
        this.loading.individuos = true
        let { data, status } = await _api.individuos.getAll(this.paramsIndividuo)
        if (status === 502) this.loading.individuos = false
        this.api.individuos = (status === 200) ? data : []
        this.loading.individuos = false
      },


      async selectedIndividuo() {
        // Encontre o indivíduo selecionado
        const individuo = this.api.individuos.find(individuo => individuo.id === this.formInterconsulta.individuoId);
        const selectedPaciente = {
          id: individuo.id,
          nome: individuo.nomeCompleto,
          tipo: 'Paciente',
          participante: 'Convidado'
        };

        // Encontre o índice do paciente, se existir
        const pacienteIndex = this.api.profissionaisSelecionados.findIndex(item => item.tipo === 'Paciente');

        if (pacienteIndex !== -1) {
          // Substitua o paciente existente
          this.api.profissionaisSelecionados.splice(pacienteIndex, 1, selectedPaciente);
        } else {
          // Adicione um novo paciente, se nenhum estiver presente
          this.api.profissionaisSelecionados.push(selectedPaciente);
        }

        this.disabled.estabelecimento = false
        await this.getEstabelecimentos()
      },

      async selectedEstabelecimento() {
        //limpandos os proximos campos
        this.api.procedimentos = []
        this.api.profissionais = []
        this.api.horas = []
        this.api.dias = []
        this.formInterconsulta.profissionaisIds = []
        delete this.formInterconsulta.procedimentoId
        delete this.formInterconsulta.dia
        delete this.formInterconsulta.hora
        this.disabled.profissional = true
        this.disabled.hora = true
        this.disabled.dia = true

        this.disabled.procedimento = false
        await this.getProcedimentos()
      },

      selectedProcedimento() {
        //limpandos os proximos campos
        this.api.profissionais = []
        this.api.horas = []
        this.api.dias = []
        this.formInterconsulta.profissionaisIds = []
        delete this.formInterconsulta.dia
        delete this.formInterconsulta.hora
        this.disabled.profissional = true
        this.disabled.hora = true

        this.disabled.dia = false
      },

      remove(removeId) {
        // Encontre o índice do profissional no array de seleção
        var index = this.api.profissionaisSelecionados.findIndex(
          (item) => item.id === removeId
        );
        if (index !== -1) {
          // Remova o profissional do array
          this.api.profissionaisSelecionados.splice(index, 1);
        }
        else if (index === -1) {
          this.api.profissionaisSelecionados = []
        }else console.log('erro')
      },

      resetForm() {
        //limpando a table
        this.api.profissionaisSelecionados = []
        //limpando o form
        this.formInterconsulta = {
          pacientePresente: false
        }

        //limpando os arrays
        this.api.profissionais = []
        this.api.estabelecimento = []
        this.api.procedimento = []
        this.api.dias = []
        this.api.horas = []
        this.api.estabelecimentoProcedimentoHorario = []
        //limpando os loading
        this.loading.profissionais = false
        this.loading.dias = false
        this.loading.horas = false
        this.loading.individuos = false
        this.loading.estabelecimentos = false
        this.loading.procedimentos = false
        //limpando os disabled
        this.disabled.profissional = true
        this.disabled.dia = true
        this.disabled.hora = true
        this.disabled.estabelecimento = true
        this.disabled.procedimento = true
      },

      async onClickSalvar() {
        this.$refs.formInterconsulta.validate(async (valid) => {
          if (valid) {

            this.formInterconsulta = {
              ...this.formInterconsulta,
              dia: this.moment(this.formInterconsulta.dia).format('YYYY-MM-DD'),
              interconsulta: true,
              tipoDaConsulta: "Teleconsulta",
              profissionalId: this.$auth.user().id,
              profissionaisInterconsulta: this.api.profissionaisSelecionados.filter(obj => obj.tipo != "Paciente"),
              estabelecimentoId: this.api.estabelecimentoProcedimentoHorario.find(obj => obj.profissionalId === this.$auth.user().id).estabelecimentoId,
              procedimentoId: this.api.estabelecimentoProcedimentoHorario.find(obj => obj.profissionalId === this.$auth.user().id).procedimentoId
            }

            console.log('FormInterconsulta: ', this.formInterconsulta)
            let { data, status } = await _api.interconsultas.post(this.formInterconsulta)
            console.log('data', data)

            if (status === 201) {

              this.options = {
                agendamentoId: data,
                individuoId: this.formInterconsulta.individuoId,
                profissionalId: this.formInterconsulta.profissionalId
              }

              //// CRIANDO A SALA DA INTERCONSULTA
              await this.getConversationClient()
              await this.createConversation()
              this.resetForm()
              this.$router.push({ name: 'Interconsulta' })
            }
            
          } else {
            this.$swal({
              title: "Atenção!",
              text: 'Verifique o preenchimento do formulário!',
              icon: 'warning',
            })
          }
        })
      },

      async getConversationClient() {
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
      async createConversation() {
        console.log('this.options', this.options)
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


      onClickVoltar() {
        this.$router.push({ name: 'Interconsulta' })
      },
    }
  };
</script>

<style scoped>
</style>
