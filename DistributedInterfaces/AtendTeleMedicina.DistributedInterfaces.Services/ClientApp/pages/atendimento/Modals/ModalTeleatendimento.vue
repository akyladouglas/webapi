<template>
  <modal name="modalTeleatendimento" :class="{ modalTeleatendimentoMinimizado: isTeleAtendimentoMinimizado }"
         :draggable="true" :clickToClose="false"
         :width="700" :height="450" :adaptive="true">

    <div class="header-modal-tele-atendimento">
      <!--componente de contrador só para renderizar na DOM-->
      <stopwatch :running="timing" :countTimer="countTimer" :resetWhenStart="resetTimer" :flag="false" @emit="onEmitContador" />
      <el-button size="small" @click="minimizeModalTeleatendimento" icon="el-icon-minus" />
      <el-popconfirm title="Você tem certeza que deseja encerrar a chamada?"
                     confirm-button-text='Sim'
                     confirmButtonType="danger"
                     cancel-button-text='Não'
                     icon="el-icon-info"
                     icon-color="red"
                     @confirm="closeModalTeleatendimento">
        <el-button size="small"
                   icon="el-icon-close"
                   type="danger"
                   slot="reference">
        </el-button>
      </el-popconfirm>
    </div>

    <el-tabs type="border-card" @tab-click="handleTabClickTeleAtendimento">
      <el-tab-pane label="Video">
        <div class="remote_video_container">
          <div id="remoteTrack"></div>
        </div>
        <div class="local_video_container">
          <div id="localTrack"></div>
        </div>

        <div style="position: absolute; bottom: 40px; left: 30px">
          <el-button type="danger" v-if="!twilio.micEnabled" icon="el-icon-microphone" @click="onMicrophoneEnable" size="small" />
          <el-button type="success" v-if="twilio.micEnabled" icon="el-icon-microphone" @click="onMicrophoneDisable" size="small" />
          <el-button type="danger" v-if="!twilio.camEnabled" icon="el-icon-camera-solid" @click="onVideoEnable" size="small" />
          <el-button type="success" v-if="twilio.camEnabled" icon="el-icon-camera-solid" @click="onVideoDisable" size="small" />
          <el-popconfirm title="Você tem certeza que deseja encerrar a chamada?"
                         confirm-button-text='Sim'
                         confirmButtonType="danger"
                         cancel-button-text='Não'
                         icon="el-icon-info"
                         icon-color="red"
                         @confirm="closeModalTeleatendimento">
            <el-button size="small"
                       type="danger"
                       slot="reference">
              Encerrar
            </el-button>
          </el-popconfirm>
        </div>
      </el-tab-pane>
      <el-tab-pane name="chat" label="Chat">
        <div id="chat">
          <el-row v-if="twilio.chatMessages.length > 0">
            <el-col :span="24">
              <VuePerfectScrollbar id="pscroll" class="scroll-area--chat" :settings="scrollSettings" key="scrol-atendimento">
                <ul id="conversation" class="list-unstyled mt-2">
                  <li v-for="(message, i) of twilio.chatMessages" :key="i" :class="message.memberSid === agendamento.individuoId ? 'msg__paciente' : 'msg__chat'">
                    <span>
                      {{message.body}}
                      <i class="msg__hora">{{ moment(message.createdDate).format('HH:mm') }}</i>
                    </span>
                  </li>
                </ul>
              </VuePerfectScrollbar>
            </el-col>
          </el-row>
          <el-row v-else class="scroll-area--chat">
            <el-empty description="Nenhum histórico de mensagens" />
          </el-row>
          <el-row :gutter="20">
            <el-col :span="21">
              <el-input v-model="chatMessage" @keyup.enter.native="onSendMessage" />
            </el-col>
            <el-col :span="3">
              <el-button icon="el-icon-s-promotion" :disabled="!chatMessage" type="success" class="chat--button" @click="onSendMessage" />
            </el-col>
          </el-row>
        </div>
      </el-tab-pane>

    </el-tabs>
  </modal>
</template>

<script>
  import { Client as ConversationsClient } from '@twilio/conversations'
  import Twilio, { createLocalVideoTrack } from 'twilio-video'
  import _api from '../../../api'
  import VuePerfectScrollbar from 'vue-perfect-scrollbar'
  import Stopwatch from '../../../components/UIComponents/StopWatch.vue'
  import audioNotificacao from '../../../assets/audios/chat-notification.mp3'
  var somNotificao = new Audio(audioNotificacao)
  import { EventBus } from '../../../event'
  import jQuery from 'jquery'
  import Hub from '../../../Hub'
  import moment from 'moment'
  moment.locale('pt-br')
  var _hub = new Hub()

  export default {
    name: 'ModalTeleatendimento',
    props: {
      agendamento: {},
      openTeleatendimento: ''
    },
    components: { VuePerfectScrollbar, Stopwatch },

    data() {
      return {

        activeTab: '0',

        scrollSettings: {
          maxScrollbarLength: 200
        },
        inicioDoAtendimento: '',
        fimDoAtendimento: '',
        //TWILIO
        isTeleAtendimentoMinimizado: false,
        inProgressTeleatendimento: false,
        // CONTADOR
        timing: false,
        resetTimer: false,
        countTimer: 0,
        countTimerTotal: null,

        chatMessage: '',

        twilio: {
          token: '',
          loading: false,
          username: '',
          authenticated: false,
          data: {},
          localTrack: false,
          remoteTrack: '',
          activeRoom: '',
          previewTracks: '',
          identity: '',
          roomName: null,
          roomAlreadyCreated: false,
          participants: [],
          tracks: [],
          localVideoTrack: '',
          camEnabled: false,
          micEnabled: false,
          activeConversation: '',
          conversationsClient: null,
          chatMessages: []
        },
      }
    },

    beforeDestroy() {
      this.leaveRoomIfJoined()
    },

    async mounted() {
      if (this.openTeleatendimento == true) {
        this.openModalTeleatendimento()
      }
    },

    methods: {

      //CONTADOR
      stopTimer() {
        this.timing = false
      },
      startTimer() {
        if (this.timing == false) this.timing = true
      },
      onEmitContador(val) {
        this.countTimer = val.time
      },


      //OPEN
      async openModalTeleatendimento() {

        try {
          const result = await navigator.mediaDevices.enumerateDevices().then(devices => {
            if (Array.isArray(devices)) {
              if (!devices.some(device => device.kind === 'videoinput')) {
                this.$swal({
                  title: "Erro!",
                  text: 'Não foi possível iniciar a chamada! Sua câmera está indisponível!',
                  icon: 'error',
                })
                return false
              } else {
                return true
              }
            }
          })
          if (!result) {
            return
          }

          //CRIAR SALA
          await this.twCreateRoom()

          setTimeout(async () => await _api.agendamentos.putAgendamento({ ...this.agendamento, emAndamento: true, profissionalId: this.$auth.user().id }), 10000)
          this.inProgressTeleatendimento = true
          
          this.$modal.show('modalTeleatendimento')
        } catch (e) {
          Notification({ title: 'Erro ao abrir a sala', message: 'Verifique sua conexão', type: 'error' })
        }

      },

      async notifyOpenRoom(event) {
        _hub.connection.start()
          .then(() => {
            console.log(`${event} - ${this.agendamento.id}`)
            _hub.connection.invoke('SendOpenRoom', `${event}-${this.agendamento.id}`)
          }).catch(e => console.log('Error connection to Hub', e));
      },

      async notifyCloseRoom(event) {
        await _hub.connection.invoke('SendOpenRoom', `${event}-close`)
        await _hub.connection.stop();
        console.log("Hub VideoChamada Desconectado");
      },


      //CLOSE
      async closeModalTeleatendimento() {
        if (document.pictureInPictureElement) {
          document.exitPictureInPicture()
        }

        //contador
        this.stopTimer()
        this.countTimerTotal = this.countTimer

        this.agendamento.emAndamento = false
        await _api.agendamentos.putAgendamento(this.agendamento)
        this.inProgressTeleatendimento = false

        this.notifyCloseRoom('schedule-update-event')

        this.$modal.hide('modalTeleatendimento')

        this.onVideoDisable()
        this.onMicrophoneDisable()
        this.leaveRoomIfJoined()

        this.fimDoAtendimento = moment().format('YYYY-MM-DD HH:mm:ss')

        //verificar o countTime no arquivo pai
        this.$emit('emit', { close: 'teleatendimento-close', countTime: this.countTimerTotal, inicioDoAtendimento: this.inicioDoAtendimento, fimDoAtendimento: this.fimDoAtendimento })

      },

      // MINIMIZE
      minimizeModalTeleatendimento() {
        try {
          if (navigator.userAgent.indexOf('Firefox') != -1) {
            this.$swal({
              title: "Atenção!",
              text: 'Este navegador não suporta este recurso!',
              icon: 'warning',
            })
            return
            // Usuário está usando o Mozilla Firefox"
          }
          document.getElementById('video').requestPictureInPicture({ width: 400, height: 300, aspectRatio: '16:9' })
          this.isTeleAtendimentoMinimizado = true
        } catch (e) {
          this.$swal({
            title: "Erro!",
            text: 'Ocorreu um problema ao minimizar a chamada!',
            icon: 'error',
          })
        }
      },

      //MAXIMIZAR
      maximizar() {
        this.isTeleAtendimentoMinimizado = false
      },

      //CRIAR SALA
      async twCreateRoom() {
        let agendamento = {
          agendamentoId: this.agendamento.id
        }

        let { data } = await _api.teleConsulta.twCreateRoom(agendamento)
        if (data === 'Sala de atendimento ja criada.') {
          this.resetTimer = true
          this.twilio.roomAlreadyCreated = true
          this.twEntrarNaSala()
          return
        }
        this.resetTimer = false
        this.twilio.roomName = data.roomVideo.unique_name
        this.twEntrarNaSala()
      },

      //ENTRAR NA SALA
      async twEntrarNaSala() {
        this.twilio.loading = true
        const VueThis = this

        let agendamentoId = this.agendamento.id

        let { data } = await _api.teleConsulta.twGetToken(agendamentoId)
        this.twilio.token = data.token
        this.twEntrarNoChat()
        if (this.twilio.roomAlreadyCreated) {
          this.twilio.roomName = data.videoRoom.unique_name
        }
        let connectOptions = {
          name: this.twilio.roomName,
          automaticSubscription: true,
          audio: true,
          video: { width: 400 }
        }

        // before a user enters a new room,
        // disconnect the user from they joined already
        this.leaveRoomIfJoined()

        // remove any remote track when joining a new room
        document.getElementById('remoteTrack').innerHTML = ''

        Twilio.connect(data.token, connectOptions).then(function (room) {
          VueThis.dispatchLog(`Sala do Atedimento: ${VueThis.twilio.roomName} aberta `)
          const cameraTrack = [...room.localParticipant.videoTracks.values()][0].track
          let localMediaContainer = document.getElementById('localTrack')
          
          // this.video = localMediaContainer;

          localMediaContainer.appendChild(cameraTrack.attach())
          document.getElementById('localTrack').children[0].setAttribute('style', 'width: 60%')
          VueThis.notifyOpenRoom('schedule-update-event')
          document.addEventListener('leavepictureinpicture', function (event) {
            // Vídeo sai de Picture-in-Picture.
            // O usuário reproduziu um vídeo Picture-in-Picture em outra página.

            // if (document.pictureInPictureElement) {
            //  document
            //    .exitPictureInPicture()
            //    .then(() => this.isMinimizado = false)
            //    .catch((err) => console.error(err));
            // } else {
            //  video.requestPictureInPicture();
            // }
            VueThis.maximizar()
          })

          VueThis.twilio.localVideoTrack = cameraTrack
          VueThis.twilio.camEnabled = true
          VueThis.twilio.micEnabled = true
          // set active toom
          VueThis.twilio.activeRoom = room
          VueThis.twilio.loading = false

          let videoParticipants = []
          // Attach the Tracks of all the remote Participants.
          room.participants.forEach(async participant => {
            //console.log('participant conectado antes', participant)
            let previewContainer = document.getElementById('remoteTrack')
            //console.log('array from simulando tracks -> ', Array.from(participant.tracks.values()))
            videoParticipants = [...videoParticipants, participant]

            participant.on('trackSubscribed', track => {
              if (track.kind === 'audio' || track.kind === 'video') {
                document.getElementById('remoteTrack').appendChild(track.attach())

                if (document.getElementById('remoteTrack').children[0].localName.includes('video')) {
                  document.getElementById('remoteTrack').children[0].setAttribute('id', 'video')
                } else {
                  document.getElementById('remoteTrack').children[1].setAttribute('id', 'video')
                }
              }
            })

            //await VueThis.attachParticipantTracks(participant, previewContainer)
          })
          //console.log('videoParticipants', videoParticipants)
          // When a Participant joins the Room, log the event.
          room.on('participantConnected', function (participant) {
            //console.log('participant entrou na sala', participant)
            participant.tracks.forEach(publication => {
              if (publication.isSubscribed) {
                const track = publication.track
                document.getElementById('remoteTrack').appendChild(track.attach())
              }
            })
            participant.on('trackSubscribed', track => {
              if (track.kind === 'audio' || track.kind === 'video') {
                document.getElementById('remoteTrack').appendChild(track.attach())

                if (document.getElementById('remoteTrack').children[0].localName.includes('video')) {
                  document.getElementById('remoteTrack').children[0].setAttribute('id', 'video')
                  //document.getElementById('remoteTrack').children[0].setAttribute('style', 'width: 43%')
                } else {
                  document.getElementById('remoteTrack').children[1].setAttribute('id', 'video')
                  //document.getElementById('remoteTrack').children[1].setAttribute('style', 'width: 43%')
                }
              }
            })
            VueThis.dispatchLog("Joining: '" + participant.identity + "'")

            VueThis.inicioDoAtendimento = moment().format('YYYY-MM-DD HH:mm:ss')
            VueThis.startTimer()
          })

          // When a Participant adds a Track, attach it to the DOM.
          room.on('trackAdded', function (track, participant) {
            //console.log('trackAdded ORIGINAL', track)
            VueThis.dispatchLog(participant.identity + ' added track: ' + track.kind)
            let previewContainer = document.getElementById('remoteTrack')
            VueThis.attachTracks([track], previewContainer)
            document.getElementById('remoteTrack').children[0].setAttribute('id', 'video')
          })

          // When a Participant removes a Track, detach it from the DOM.
          room.on('trackRemoved', function (track, participant) {
            // console.log('trackRemoved - track', track)
            VueThis.dispatchLog(participant.identity + ' removed track: ' + track.kind)
            VueThis.detachTracks([track])
          })

          // When a Participant leaves the Room, detach its Tracks.
          room.on('participantDisconnected', function (participant) {
            VueThis.stopTimer()
            // console.log('participantDisconnected', participant)
            VueThis.dispatchLog("Participant '" + participant.identity + "' left the room")
          })

          room.on('trackUnsubscribed', function (track) {
            // console.log('trackUnsubscribed', track)
            if (track.kind === 'audio' || track.kind === 'video') {
              const htmlElements = track.detach()
              for (let htmlElement of htmlElements) {
                htmlElement.remove()
              }
            }
            VueThis.dispatchLog("Track '" + track + "' trackUnsubscribed")
          })
        })
      },

      //ENTRAR NO CHAT
      async twEntrarNoChat() {
        window.conversationsClient = ConversationsClient
        this.conversationsClient = new ConversationsClient(this.twilio.token)
        this.conversationsClient.on('connectionStateChanged', (state) => {
          switch (state) {
            case 'connected':
              this.createConversation()
              break
            case 'disconnecting':
              break
            case 'disconnected':
              break
            case 'denied':
              break
          }
        })
        this.conversationsClient.on('messageAdded', (message) => {
          console.log('messageAdded')
          if (message.conversation.channelState.uniqueName.includes('Recepcao')) return
          if (this.activeTab !== '1') {
            this.handleNotifyChatMessage()
          }
          this.onAddMessageToHistory(message.state)
        })
        this.conversationsClient.on('participantJoined', (participant) => {
          // console.log('paciente entrou na sala')
        })
      },

      //CRIAR A CONVERSA DO CHAT
      async createConversation() {
        const uniqueNameChat = `${this.agendamento.id}-chat`
        try {
          const newConversation = await this.conversationsClient.createConversation({ uniqueName: uniqueNameChat })
          const joinedConversation = await newConversation.join().catch(err => console.log(err))
          await joinedConversation.add(this.$auth.user().id).catch(err => console.log(err))
          await joinedConversation.add(this.agendamento.individuoId).catch(err => console.log(err))
          this.twilio.activeConversation = joinedConversation
        } catch (e) {
          const oldConversation = await this.conversationsClient.getConversationByUniqueName(uniqueNameChat).catch(err => console.log('erro getConversationByUniqueName', err))
          //console.log('oldConversation', oldConversation)
          let count = oldConversation.getParticipantsCount()
          if (count < 2) {
            await oldConversation.add(this.$auth.user().id).catch(err => console.log(err))
            await oldConversation.add(this.agendamento.individuoId).catch(err => console.log(err))
          }
          this.twilio.activeConversation = oldConversation
        } finally {
          await this.getMessages()
        }
      },
      async getMessages() {
        if (!this.twilio.activeConversation) {
          return this.$swal({
            title: "Atenção!",
            text: 'Não foi possivel conectar e retornar as mensagens do chat!',
            icon: 'warning',
          })
        }

        this.twilio.activeConversation.getMessages().then(msgs => {
          const allMessages = msgs.items.map(msg => {
            return {
              body: msg.body,
              createdDate: msg.dateCreated,
              memberSid: msg.author
            }
          })
          let filteredMsgs = allMessages.filter(function (msg) {
            return msg.body.length > 0
          })
          this.twilio.chatMessages = filteredMsgs
        })
      },

      onSendMessage() {
        if (!this.chatMessage) return
        if (!this.twilio.activeConversation) {
          return this.$swal({
            title: "Atenção!",
            text: 'Não foi possivel conectar e enviar a mensagem no chat!',
            icon: 'warning',
          })
        }
        this.twilio.activeConversation.sendMessage(this.chatMessage)
          .catch((erro) => {

            this.$swal({
              title: "Atenção!",
              text: 'Não foi possível enviar a mensagem!',
              icon: 'warning',
            })

          })
        this.chatMessage = ''
      },

      //NOTIFICAR NOVAS MENSAGENS DO CHAT
      handleNotifyChatMessage() {
        let element = document.getElementById('tab-chat')
        //console.log('element', element)
        element.classList.add('chat--notify')
        somNotificao.play()
      },

      //ADICIONAR MENSAGEM NO HISTORICO
      async onAddMessageToHistory(msg) {
        await this.twilio.chatMessages.push({
          body: msg.body,
          createdDate: msg.timestamp,
          memberSid: msg.author
        })
      },

      //SAIR DA SALA.
      leaveRoomIfJoined() {
        document.getElementById('localTrack').innerHTML = ''
        document.getElementById('remoteTrack').innerHTML = ''
        if (this.twilio.activeRoom) {
          this.twilio.activeRoom.disconnect()
        }
      },


      // Trigger log events
      dispatchLog(message) {
        EventBus.$emit('new_log', message)
      },

      // Attach the Tracks to the DOM.
      attachTracks(tracks, container) {
        //console.log('attachTracks tracks: ', tracks)
        //console.log('attachTracks container: ', container)
        tracks.forEach(function (track) {
          container.appendChild(track.attach())
        })
      },

      // Attach the Participant's Tracks to the DOM.
      async attachParticipantTracks(participant, container) {
        //console.log('tipo da lista', typeof participant.tracks)
        if (!participant.tracks) {
          console.log('não tem participante')
          return
        }

        const trackFromList = Array.from(participant.tracks.values())
        const arrayTracks = trackFromList.map(publication => {
          //console.log('publication', publication)
          //console.log('publication.isSubscribed', publication.isSubscribed)
          if (publication.isSubscribed) {
            //console.log('publication.track', publication.track)
            return publication.track
          }

          //console.log('lista de tracks montada', arrayTracks)
        })
        
        
        //let tracks = Array.from(participant.tracks.values())
        //if (!tracks) return console.log('error array traks attachParticipantTracks ->', tracks)

        
      },

      // Detach the Tracks from the DOM.
      detachTracks(tracks) {
        tracks.forEach((track) => {
          track.detach().forEach((detachedElement) => {
            detachedElement.remove()
          })
        })
      },

      // Detach the Participant's Tracks from the DOM.
      detachParticipantTracks(participant) {
        let tracks = Array.from(participant.tracks.values())
        this.detachTracks(tracks)
      },

      handleTabClickTeleAtendimento(tab, event) {
        this.activeTab = tab.index
        if (tab.label === 'Chat') {
          let element = document.getElementById('tab-1')
          element.classList.remove('chat--notify')
          setTimeout(() => {
            this.scrollChatToEnd()
          }, 500)
        }
      },

      scrollChatToEnd() {
        if (this.twilio.chatMessages.length === 0) return
        jQuery('#pscroll').animate({ scrollTop: jQuery('ul#conversation li:last').offset().top + 60 }, 200)
      },

      onMicrophoneEnable() {
        this.twilio.activeRoom.localParticipant.audioTracks.forEach(publication => {
          publication.track.enable()
        })
        this.twilio.micEnabled = true
      },

      onMicrophoneDisable() {
        this.twilio.activeRoom.localParticipant.audioTracks.forEach(publication => {
          publication.track.disable()
        })
        this.twilio.micEnabled = false
      },

      onVideoEnable() {
        createLocalVideoTrack().then(track => {
          let localMediaContainer = document.getElementById('localTrack')
          localMediaContainer.appendChild(track.attach())
          localMediaContainer.children[0].setAttribute('style', 'width: 39%')
          return this.twilio.activeRoom.localParticipant.publishTrack(track)
        })
        this.twilio.camEnabled = true
      },

      onVideoDisable() {
        this.twilio.activeRoom.localParticipant.videoTracks.forEach(publication => {
          publication.track.disable()
          publication.track.stop()
          publication.unpublish()
        })
        document.getElementById('localTrack').innerHTML = ''
        this.twilio.camEnabled = false
      },
      
    }
  }
</script>

<style>
  .chat--notify {
    background: #E6A23C;
    color: #fff !important;
  }
</style>

<style scoped>
  /*MODAL TWILIO*/
  .modalTeleatendimentoMinimizado {
    display: none;
  }

  .header-modal-tele-atendimento {
    margin-bottom: 20px;
    position: absolute !important;
    right: 0 !important;
    z-index: 9;
  }

  /** Twilio classes */
  .local_video_container {
    display: flex;
    justify-content: flex-end;
    position: absolute;
    bottom: 28px;
    right: 15px;
    width: 100%;
  }

  #localTrack {
    display: flex;
    justify-content: flex-end;
    width: max-content;
  }
  .remote_video_container {
    flex: 1;
    width: 100%;
    height: 380px;
    align-items: flex-end;
    justify-content: flex-end;
    background: #000;
  }

  #remoteTrack {
      width: 100%;
      height: 100%;
      display: flex;
      justify-content: center;
  }
  #remoteTrack video {
      height: 100%
  }

  .roomTitle {
    border: 1px solid rgb(124, 129, 124);
    padding: 4px;
    color: dodgerblue;
  }

  .scroll-area {
    position: relative;
    width: 100%;
    height: 700px;
    overflow-x: hidden;
  }

  .scroll-area--chat {
    position: relative;
    width: 100%;
    height: 330px;
    overflow-x: hidden;
  }

  #chat .el-input__inner {
    height: 36px;
  }

  #chat li {
    font-size: 12px;
    font-weight: 400;
  }

  li.msg__chat {
    text-align: right;
    margin-bottom: 15px;
  }

  .msg__chat span {
    border-radius: 10px;
    border-bottom-right-radius: 0;
    background: #008d95;
    padding: 5px 10px 5px 15px;
    color: #fff;
    font-size: 14px;
    font-weight: 400;
  }

  li.msg__paciente {
    text-align: left;
    margin-bottom: 15px;
  }

  .msg__paciente span {
    border-radius: 10px;
    border-bottom-left-radius: 0;
    background: #225457;
    padding: 5px 15px 5px 10px;
    color: #fff;
    font-size: 14px;
    font-weight: 400;
  }

  .msg__hora {
    font-size: 10px;
  }

</style>
