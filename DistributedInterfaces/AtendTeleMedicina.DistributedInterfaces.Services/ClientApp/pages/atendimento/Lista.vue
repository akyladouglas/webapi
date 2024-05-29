<template>
  <el-row :gutter="24">
    <el-col :xs="4" :sm="4" :md="4" :lg="4" :xl="4">
      <div class="container-menu-buttons">

        <div style="width: 100%" v-if="agendamento.interconsulta != undefined && agendamento.interconsulta == true">
          <el-tag style="background-color: #c4c104; font-size: 14px; font-weight: bold; width: 100%; display: flex; justify-content: center "
                  effect="dark">
            Atendimento Interconsulta
          </el-tag>
        </div>

        <button :class="{ menuButton: true, menuActive: isScreenActive('Sinopse') }" @click="showScreen('Sinopse')">Sinopse</button>
        <button :class="{ menuButton: true, menuActive: isScreenActive('Atendimento') }" @click="showScreen('Atendimento')" v-if="showButtonAtendimento">Atendimento</button>
        <button :class="{ menuButton: true, menuActive: isScreenActive('Historico') }" @click="showScreen('Historico')">Histórico</button>
        <button :class="{ menuButton: true, menuActive: isScreenActive('HistoricoSintomas') }" @click="showScreen('HistoricoSintomas')">Historico de Sintomas</button>
        <button :class="{ menuButton: true, menuActive: isScreenActive('Cadastro') }" @click="showScreen('Cadastro')">Cadastro do Paciente</button>
      </div>
    </el-col>

    <el-col :xs="20" :sm="20" :md="20" :lg="20" :xl="20" style="border-left: 1px solid #43bec6; ">
      <component v-show="currentScreen === 'Sinopse'" :is="'screen-sinopse'" :agendamento="agendamento" @emit="" />
      <component v-show="currentScreen === 'Atendimento'" :is="'screen-atendimento'" :agendamento="agendamento" :inTeleatendimento="inTeleatendimento" :countTimerTeleatendimento="countTimerTeleatendimento" :inicioDoAtendimento="inicioDoAtendimento" :fimDoAtendimento="fimDoAtendimento" @emit="returnEmitModalAtendimento" />
      <component v-show="currentScreen === 'Historico'" :is="'screen-historico'" :agendamento="agendamento" @emit="" />
      <component v-show="currentScreen === 'Cadastro'" :is="'screen-cadastro'" :agendamento="agendamento" @emit="" />
      <component v-show="currentScreen === 'HistoricoSintomas'" :is="'screen-historico-sintomas'" :agendamento="agendamento" @emit="" />
    </el-col>

    <!-- MODALS -->
    <component v-if="controlModalMemed" :is="'modal-memed'" :agendamento="agendamento" :openMemed="controlModalMemed" @emit="returnEmitModalMemed" />
    
    <!-- MODAL TELEATENDIMENTO (PROFISSIONAL) -->
    <component v-if="controlModalTeleatendimento && agendamento.interconsulta == false" :is="'modal-teleatendimento'" :agendamento="agendamento" :openTeleatendimento="controlModalTeleatendimento" @emit="returnEmitModalTeleatendimento" />


    <!-- MODAL TELEATENDIMENTO INTERCONSULTA (ANFITRIÃO) -->
    <component v-if="controlModalTeleatendimento && agendamento.interconsulta == true && isAnfitriao == true" :is="'modal-teleatendimento-interconsulta'" :agendamento="agendamento" :openTeleatendimento="controlModalTeleatendimento" @emit="returnEmitModalTeleatendimento" />

    <!-- MODAL TELEATENDIMENTO INTERCONSULTA (CONVIDADO)-->
    <component v-if="controlModalTeleatendimento && agendamento.interconsulta == true && isConvidado == true" :is="'modal-teleatendimento-convidado'" :agendamento="agendamento" :openTeleatendimento="controlModalTeleatendimento" @emit="returnEmitModalTeleatendimento" />
  </el-row>
</template>

<script>
  import _api from '../../api'
  import { Client as ConversationsClient } from '@twilio/conversations'
  import ScreenSinopse from './Screens/Sinopse'
  import ScreenAtendimento from './Screens/Atendimento'
  import ScreenHistorico from './Screens/Historico'
  import ScreenCadastro from './Screens/Cadastro'
  import ScreenHistoricoSintomas from './Screens/HistoricoSintomas'

  //MODALS COMPONENTS
  import ModalMemed from './Modals/ModalMemed'
  import ModalTeleatendimento from './Modals/ModalTeleatendimento'
  import ModalTeleatendimentoConvidado from './Modals/ModalTeleatendimentoConvidado'
  import ModalTeleatendimentoInterconsulta from './Modals/ModalTeleatendimentoInterconsulta'

  export default {
    name: 'Atendimento',
    components: { ScreenSinopse, ScreenAtendimento, ScreenHistorico, ScreenCadastro, ScreenHistoricoSintomas, ModalMemed, ModalTeleatendimento, ModalTeleatendimentoConvidado, ModalTeleatendimentoInterconsulta },
    data() {
      return {

        //CONTROL MENU
        showButtonAtendimento: true,
        inTeleatendimento: false,

        //MODALS CONTROL
        controlModalMemed: false,
        controlModalTeleatendimento: false,

        //SCREENS CONTROL
        currentScreen: 'Sinopse',

        //DATA
        agendamento: {},
        isAnfitriao: false,
        isConvidado: false,
        conversationsClient: null,
        twilio: {},

        countTimerTeleatendimento: '',
        inicioDoAtendimento: '',
        fimDoAtendimento: '',
      }
    },

    computed: {
      //COMPUTED PARA CONTROLAR A COR DO MENU ATUAL
      isScreenActive() {
        return screen => this.currentScreen === screen;
      }
    },

    async created() {
      //console.log("this.$router", this.$router)
      // caso ocorra um refresh na tela ele ira retornar para tela agendamentos
      if (!this.$route.params.agendamento) {
        this.$router.push({
          name: 'Agendamentos'
        })
        return
      }
      
      this.agendamento = this.$route.params.agendamento
      if (this.$route.params.participante != undefined && this.$route.params.participante == "Anfitrião") {
        this.isAnfitriao = true
      } else if (this.$route.params.participante != undefined && this.$route.params.participante == "Convidado") {
        this.isConvidado = true
      }


      let { data, status } = await _api.agendamentos.getById(this.$route.params.agendamento.id)
      if (status === 200 && data.profissionalId != this.$auth.user().id) {

        this.$swal({
          title: 'Atenção',
          text: 'O agendamento selecionado já esta com vinculo para outro médico!',
          icon: 'warning',
        })

      }
      //SE O PROFISSIONAL LOGADO FOR O MESMO QUE O PROFISSIONAL QUE ASSUMIU O AGENDAMENTO
      else if (status === 200 && data.profissionalId == this.$auth.user().id) {
        this.agendamento.profissionalId = data.profissionalId
        this.$store.dispatch('setDentroDoAtendimento', true)

        if (this.$config.configDB.modulo == 3) {
          //CRIANDO A CONVERSA DA TWILIO
          await this.getConversationClient()
        }

      }

    },

    async mounted() {
      
    },

    beforeDestroy() {
      this.$store.dispatch('setDentroDoAtendimento', false)
    },

    methods: {
      async getConversationClient() {
        if (this.conversationsClient == null) {

          window.conversationsClient = ConversationsClient
          let { data } = await _api.teleConsulta.twGetChatToken(this.agendamento.individuoId)
          let token = data
          this.conversationsClient = new ConversationsClient(token)
          this.conversationsClient.on('connectionStateChanged', (state) => {
            switch (state) {
              case 'connected':
                this.createConversation()
                break
            }
          })

        }
      },

      async createConversation() {
        const uniqueName = `${this.agendamento.id}-chat`
        try {
          let activeConnection = await this.conversationsClient.createConversation({ uniqueName: uniqueName }).catch(err => console.log('erro ao criar a conversation -> ', err))

          if (activeConnection) {
            let joinedConversation = await activeConnection.join()
            if (joinedConversation) {
              await joinedConversation.add(this.agendamento.profissionalId).catch(err => console.log('erro no add profisisonal', err))
              await joinedConversation.add(this.agendamento.individuoId).catch(err => console.log('erro no add individuo', err))
            }
          }
          
          
        } catch (e) {
          if (e.message == "Conflict") {
            return
          }
          //console.log('Não foi possível criar a conversa do chat! ->', e)
          //this.$swal({
          //  title: 'Atenção',
          //  text: 'Não foi possível criar a conversa do chat!',
          //  icon: 'warning',
          //  confirmButtonText: 'OK'
          //})
        }
      },


      //SCREEN DISPLAY CONTROL
      showScreen(screen) {
        this.currentScreen = screen;
      },

      returnEmitModalAtendimento(val) {
        if (val == 'memed-open') this.controlModalMemed = true
        if (val == 'teleatendimento-open') {
          this.inTeleatendimento = true
          this.controlModalTeleatendimento = true
        }
      },

      returnEmitModalMemed(val) {
        if (val == 'memed-close') this.controlModalMemed = false
      },

      returnEmitModalTeleatendimento(val) {
        if (val.close == 'teleatendimento-close') {
          this.inTeleatendimento = false
          this.controlModalTeleatendimento = false
          this.countTimerTeleatendimento = val.countTime
          this.inicioDoAtendimento = val.inicioDoAtendimento
          this.fimDoAtendimento = val.fimDoAtendimento

          console.log('this.inicioDoAtendimento', this.inicioDoAtendimento)
          console.log('this.fimDoAtendimento', this.fimDoAtendimento)
        }
      }

    }
  }
</script>

<style>
  .vm--container {
    width: 100px !important;
    height: 100px !important;
    float: right !important;
    background-color: transparent !important;
  }

  .vm--overlay {
    display: flex !important;
    width: 6% !important;
    height: 1% !important;
    background-color: transparent !important;
  }
</style>

<style scoped>
  .container-menu-buttons {
    display: flex;
    flex-direction: column;
    margin-top: 15vh;
  }

  .menuButton {
    display: inline-block;
    line-height: 1;
    white-space: nowrap;
    cursor: pointer;
    background: #b5e6e5;
    text-align: center;
    box-sizing: border-box;
    outline: 0;
    margin: 0;
    transition: .1s;
    font-weight: 500;
    -webkit-user-select: none;
    font-size: 14px;
    border-radius: 4px;
    padding: 12px;
    margin-top: 10px;
    margin-bottom: 10px;
    margin-left: 0px;
    border: 1px solid #093434;
  }

    .menuButton:focus {
      color: black;
      background-color: #188B8A;
    }

    .menuButton:hover {
      color: black;
      background-color: #B5E6E5;
    }

  .menuActive {
    background-color: #01826c !important;
    color: #FFF !important;
    display: inline-block;
    line-height: 1;
    white-space: nowrap;
    cursor: pointer;
    text-align: center;
    box-sizing: border-box;
    outline: 0;
    margin: 0;
    transition: .1s;
    font-weight: 500;
    -webkit-user-select: none;
    font-size: 14px;
    border-radius: 4px;
    padding: 12px;
    margin-top: 10px;
    margin-bottom: 10px;
    margin-left: 0px;
  }
    .menuActive:hover {
      color: #FFF
    }
</style>
