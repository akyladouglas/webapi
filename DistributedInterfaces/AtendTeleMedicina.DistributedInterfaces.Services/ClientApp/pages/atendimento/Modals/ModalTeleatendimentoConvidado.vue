<template>
  <modal name="modalTeleatendimento" :class="{ modalTeleatendimentoMinimizado: isTeleAtendimentoMinimizado }"
         :draggable="true" :clickToClose="false"
         :width="700" :height="modalHeight">

    <div style="height: 100%" id="areaModal">

      <div class="header-modal-tele-atendimento">
        <el-button size="small" @click="minimizeModalTeleatendimento" icon="el-icon-minus" />
        <el-popconfirm title="Você tem certeza que deseja encerrar a chamada?"
                       confirm-button-text='Sim'
                       confirmButtonType="danger"
                       cancel-button-text='Não'
                       icon="el-icon-info"
                       icon-color="blue"
                       @confirm="closeModalTeleatendimento">
          <el-button size="small"
                     icon="el-icon-close"
                     type="danger"
                     slot="reference">
          </el-button>
        </el-popconfirm>
      </div>

      <el-tabs type="card" style="height: 100%">
        <el-tab-pane label="Video" style="height: 100%">

          <div class="video-grid" :style="gridStyle">
            <div v-for="(video, index) in videos" :key="index" class="grid-cell" :style="{'background-color': video.backgroundColor}">

              <div v-if="video.id == 'localTrack'">
                <div :id="video.id"></div>
              </div>

              <div v-else :id="video.id"></div>
            </div>
          </div>

          <div class="buttons">
            <el-button type="danger" v-if="!twilio.micEnabled" icon="el-icon-microphone" @click="onMicrophoneEnable" size="small" style="margin: 0px" />
            <el-button type="success" v-if="twilio.micEnabled" icon="el-icon-microphone" @click="onMicrophoneDisable" size="small" style="margin: 0px" />
            <el-button type="danger" v-if="!twilio.camEnabled" icon="el-icon-camera-solid" @click="onVideoEnable" size="small" style="margin: 0px" />
            <el-button type="success" v-if="twilio.camEnabled" icon="el-icon-camera-solid" @click="onVideoDisable" size="small" style="margin: 0px" />
            <el-popconfirm title="Você tem certeza que deseja encerrar a chamada?"
                           confirm-button-text='Sim'
                           confirmButtonType="danger"
                           cancel-button-text='Não'
                           icon="el-icon-info"
                           icon-color="red"
                           @confirm="closeModalTeleatendimento">
              <el-button size="small"
                         type="danger"
                         slot="reference" style="padding: 11px 15px 11px 15px">
                Encerrar
              </el-button>
            </el-popconfirm>
          </div>
        </el-tab-pane>
      </el-tabs>

    </div>
  </modal>
</template>

<script>
  import { EventBus } from '../../../event'
  import Twilio, { createLocalVideoTrack } from 'twilio-video'
  import _api from '../../../api'
  import VuePerfectScrollbar from 'vue-perfect-scrollbar'

  export default {
    name: 'ModalTeleatendimentoConvidado',
    props: {
      agendamento: {},
      openTeleatendimento: ''
    },
    components: { VuePerfectScrollbar },

    data() {
      return {
        videos: [
          /*{ backgroundColor: '#F3F3F3', id: 'localTracks' },*/
          //{ backgroundColor: '#F3F3F3', id: 'localTracks' },
          //{ backgroundColor: '#F3F3F3', id: 'localTracks' },
        ],

        activeTab: '0',

        scrollSettings: {
          maxScrollbarLength: 200
        },

        // TWILIO
        isTeleAtendimentoMinimizado: false,
        inProgressTeleatendimento: false,
        // CONTADOR
        timing: false,
        resetTimer: false,
        countTimer: 0,
        countTimerTotal: null,

        chatMessage: '',
        numRemoteTracks: 0,
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
        numGuests: 0,
        guests: []
      }
    },

    computed: {
      gridStyle() {
        const numVideos = this.videos.length;
        let columns;

        if (numVideos <= 3) {
          columns = numVideos; // Uma coluna por vídeo
        } else if (numVideos <= 6) {
          columns = 3; // Até 3 colunas
        }

        const columnWidth = 100 / columns;

        // Calcule a altura com base no número de vídeos e na altura de um vídeo
        const videoHeight = 200; // Altura de um vídeo em pixels (ajuste conforme necessário)
        const numRows = Math.ceil(numVideos / columns);

        //const gridHeight = numRows * videoHeight;

        if (numRows == 1) {
          return {
            'grid-template-columns': `repeat(${columns}, ${columnWidth}%)`,
          };
        } else {
          return {
            'grid-template-columns': `repeat(${columns}, ${columnWidth}%)`,
            'grid-template-rows': `repeat(${numRows}, ${videoHeight}px)`, // Ajuste a altura aqui
          };
        }
      },

      modalHeight() {
        return 500; // Altura
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
      // OPEN
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

          // TODO ENTRAR NA SALA
          await this.twEntrarNaSala()
          this.$modal.show('modalTeleatendimento')
        } catch (e) {
          // Notification({ title: 'Erro ao abrir a sala', message: 'Verifique sua conexão', type: 'error' })
        }
      },

      // CLOSE
      async closeModalTeleatendimento() {
        if (document.pictureInPictureElement) {
          document.exitPictureInPicture()
        }

        this.inProgressTeleatendimento = false

        this.$modal.hide('modalTeleatendimento')

        this.onVideoDisable()
        this.onMicrophoneDisable()
        this.leaveRoomIfJoined()

        this.$emit('emit', { close: 'teleatendimento-close'})
      },

      // MINIMIZE
      minimizeModalTeleatendimento() {
        try {
          if (navigator.userAgent.indexOf('Firefox') !== -1) {

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

      // MAXIMIZAR
      maximizar() {
        this.isTeleAtendimentoMinimizado = false
      },

      // ENTRAR NA SALA
      async twEntrarNaSala() {
        try {


          this.twilio.loading = true
          const VueThis = this

          let agendamento = this.agendamento.id

          let { data } = await _api.teleConsulta.twGetToken(agendamento)
          this.twilio.token = data.token
          this.twilio.roomName = data.videoRoom.unique_name

          let connectOptions = {
            name: this.twilio.roomName,
            automaticSubscription: true,
            audio: true,
            video: { width: 400 }
          }

          this.numRemoteTracks = this.numRemoteTracks + 1

          // before a user enters a new room,
          // disconnect the user from they joined already
          this.leaveRoomIfJoined()

          // remove any remote track when joining a new room
          // for (let index = 0; index < this.numRemoteTracks; index++) {
          //   console.log('vai remover o: remoteTrack' + [index + 1])
          //   document.getElementById('remoteTrack' + [index + 1]).innerHTML = ''
          // }
          Twilio.connect(data.token, connectOptions).then(function (room) {
            VueThis.videos.push({ backgroundColor: '#F3F3F3', id: 'localTrack' })

            VueThis.dispatchLog(`Sala do Atedimento: ${VueThis.twilio.roomName} aberta `)

            //esperando o proximo tick de atualização do vue para conseguir retornar a localTrack e adicionar o video
            VueThis.$nextTick(() => {
              const cameraTrack = [...room.localParticipant.videoTracks.values()][0].track
              let localMediaContainer = document.getElementById('localTrack')
              //this.video = localMediaContainer;

              localMediaContainer.appendChild(cameraTrack.attach())
              document.getElementById('localTrack').children[0].setAttribute('style', 'min-width: 100%', 'min-height: max-content', 'object-fit: cover')
              let localTrackDiv = document.getElementById('localTrack')
              let localVideoElement = localTrackDiv.querySelector('video')
              if (localVideoElement) {
                localVideoElement.style.maxHeight = '100%'
                localVideoElement.style.maxWidth = '100%'
                localVideoElement.style.minWidth = '100%'
                localVideoElement.style.border = '1px solid #000'
              }

              //  document.addEventListener('leavepictureinpicture', function (event) {
              //    // Vídeo sai de Picture-in-Picture.
              //    // O usuário reproduziu um vídeo Picture-in-Picture em outra página.

              //    // if (document.pictureInPictureElement) {
              //    //  document
              //    //    .exitPictureInPicture()
              //    //    .then(() => this.isMinimizado = false)
              //    //    .catch((err) => console.error(err));
              //    // } else {
              //    //  video.requestPictureInPicture();
              //    // }
              //    VueThis.maximizar()
              //  })

              VueThis.twilio.localVideoTrack = cameraTrack
              VueThis.twilio.camEnabled = true
              VueThis.twilio.micEnabled = true
              // set active toom
              VueThis.twilio.activeRoom = room
              VueThis.twilio.loading = false
            })


            // Attach the Tracks of all the remote Participants.
            //FAZ O ATTACH DOS PARTICIPANTES QUE JA ESTAO NA SALA
            room.participants.forEach(function (participant) {

              // SE FOR O PACIENTE ENTRA AQUI
              if (participant.identity === VueThis.agendamento.individuoId) {
                VueThis.videos.push({ backgroundColor: 'gray', id: 'remoteTrack' })

                VueThis.$nextTick(() => {
                  participant.tracks.forEach(publication => {
                    if (publication.isSubscribed) {
                      const track = publication.track
                      document.getElementById('remoteTrack').appendChild(track.attach())
                    }
                  })
                })
                participant.on('trackSubscribed', track => {
                  if (track.kind === 'audio' || track.kind === 'video') {
                    document.getElementById('remoteTrack').appendChild(track.attach())
                    if (document.getElementById('remoteTrack').children[0].localName.includes('video')) {
                      document.getElementById('remoteTrack').children[0].setAttribute('id', 'videoPaciente')
                    } else {
                      document.getElementById('remoteTrack').children[1].setAttribute('id', 'videoPaciente')
                    }
                  }
                })
                VueThis.dispatchLog("Joining: '" + participant.identity + "'")
                
              } else {
                VueThis.numGuests = VueThis.numGuests + 1
                VueThis.guests.push(participant.identity)
                /*console.log('entrou no foreach não paciente')*/
                VueThis.videos.push({ backgroundColor: '#F3F3F3', id: `guestTrack${participant.identity}` })

                VueThis.$nextTick(() => {
                  participant.tracks.forEach(publication => {
                    if (publication.isSubscribed) {
                      const track = publication.track
                      document.getElementById(`guestTrack${participant.identity}`).appendChild(track.attach())
                    }
                  })
                })

                participant.on('trackSubscribed', track => {
                  if (track.kind === 'audio' || track.kind === 'video') {
                    let guestTrackDiv = document.getElementById(`guestTrack${participant.identity}`)
                    guestTrackDiv.appendChild(track.attach())
                    let micStatus = document.createElement('div')
                    micStatus.id = `micStatus-${participant.identity}`
                    micStatus.style.color = '#fff'
                    micStatus.style.position = 'absolute'
                    track.on('disabled', () => {
                      if (track.kind === 'audio') {
                        micStatus.innerHTML = '<i class="fa fa-microphone-slash" style="position: absolute; bottom: 11px; left: 5px"></i>'
                        guestTrackDiv.appendChild(micStatus)
                      }
                    })
                    track.on('enabled', () => {
                      if (track.kind === 'audio') {
                        micStatus.innerHTML = '<i class="fa fa-microphone" style="position: absolute; bottom: 11px; left: 11px"></i>'
                        guestTrackDiv.appendChild(micStatus)
                      }
                    })
                  }
                  if (track.kind === 'video') {
                    let guestTrackDiv = document.getElementById(`guestTrack${participant.identity}`)
                    let videoElement = guestTrackDiv.querySelector('video')
                    if (videoElement) {
                      videoElement.style.maxHeight = '100%'
                      videoElement.style.minWidth = '100%'
                      videoElement.style.maxWidth = '100%'
                      videoElement.style.border = '1px solid #000'
                    }
                  }
                })

              }
            })



            // When a Participant joins the Room, log the event.

            //FAZ O ATTACH DOS PARTICIPANTES QUE ENTRAM NA SALA
            room.on('participantConnected', function (participant) {

              //CASO OCORRA DE ALGUEM SAIR E ENTRAR NOVAMENTE, REMOVE A POSIÇÃO ANTIGA DO ARRAY PARA PODER RENDERIZAR UMA POSIÇÃO NOVA NA LISTA
              VueThis.videos.forEach((video, index) => {
                if (video.id == `guestTrack${participant.identity}`) {
                  VueThis.videos.splice(index, 1);
                } else if (video.id == 'remoteTrack') {
                  VueThis.videos.splice(index, 1);
                } else { }
              });


              VueThis.videos.push({ backgroundColor: '#F3F3F3', id: `guestTrack${participant.identity}` })

              VueThis.$nextTick(() => {

                participant.tracks.forEach(publication => {
                  if (publication.isSubscribed) {
                    const track = publication.track
                    document.getElementById(`guestTrack${participant.identity}`).appendChild(track.attach())
                  }
                })



                participant.on('trackSubscribed', track => {
                  if (track.kind === 'audio' || track.kind === 'video') {
                    document.getElementById(`guestTrack${participant.identity}`).appendChild(track.attach())

                    if (document.getElementById(`guestTrack${participant.identity}`).children[0].localName.includes('video')) {
                      /*console.log('if')*/
                      document.getElementById(`guestTrack${participant.identity}`).children[0].setAttribute('id', `guestTrack${participant.identity}`)

                      let guestTrackDiv = document.getElementById(`guestTrack${participant.identity}`)
                      let guestVideoElement = guestTrackDiv.querySelector('video')
                      if (guestVideoElement) {
                        guestVideoElement.style.maxHeight = '100%'
                        guestVideoElement.style.minWidth = '100%'
                        guestVideoElement.style.maxWidth = '100%'
                        guestVideoElement.style.border = '1px solid #000'
                      }

                      //document.getElementById(`guestTrack${participant.identity}`).children[0].setAttribute('style', 'width: 43%')
                    } else {
                      /*console.log('else')*/

                      let guestTrackDiv = document.getElementById(`guestTrack${participant.identity}`)
                      let guestVideoElement = guestTrackDiv.querySelector('video')
                      if (guestVideoElement) {
                        guestVideoElement.style.maxHeight = '100%'
                        guestVideoElement.style.maxWidth = '100%'
                        guestVideoElement.style.minWidth = '100%'
                        guestVideoElement.style.border = '1px solid #000'
                      }
                      //document.getElementById(`guestTrack${participant.identity}`).children[1].setAttribute('style', 'width: 43%')
                    }
                  }
                })
                VueThis.dispatchLog("Joining: '" + participant.identity + "'")

              })
            })


            // When a Participant adds a Track, attach it to the DOM.
            room.on('trackAdded', function (track, participant) {
              // console.log('trackAdded', track)
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
              /*console.log('participantDisconnected', participant)*/

              //REMOVENDO O QUADRADO DO PARTICIPANTE QUE SAIU DA SALA
              VueThis.videos = VueThis.videos.filter(video => video.id !== `guestTrack${participant.identity}`)

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

        } catch(e) {
          console.log('error twEntrarNaSala', e)
          this.$emit('emit', { close: 'teleatendimento-close' })
        }
    },
      leaveRoomIfJoined() {
        let elLocalTrack = document.getElementById('localTrack')
        /*console.log('elLocalTrack', elLocalTrack)*/
        if (elLocalTrack) {
          /*console.log('exist')*/
          elLocalTrack.innerHTML = ''
        }
        console.log('this.twilio.activeRoom', this.twilio.activeRoom)
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
        tracks.forEach(function (track) {
          container.appendChild(track.attach())
        })
      },
      // Attach the Participant's Tracks to the DOM.
      attachParticipantTracks(participant, container) {
        let tracks = Array.from(participant.tracks.values())
        this.attachTracks(tracks, container)
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
          localMediaContainer.children[0].setAttribute('style', 'max-width: 100%', 'min-width: 100%', 'max-height: 100%', 'border: 1px solid #000')
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
      }

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

  .video-grid {
    display: grid;
    /*grid-template-columns: repeat(1, 1fr);*/
    gap: 2px;
    height: 83%;
    overflow: visible; /* Adiciona overflow: hidden para ocultar o conteúdo que ultrapassa a altura */
  }

  .grid-cell {
    display: flex;
    align-items: center;
    justify-content: center;
    border: 1px solid #ccc;
    padding: 2px;
    overflow: hidden;
    position: relative;
    max-height: 100%; /* Adicione max-height para evitar que a div cresça além do contêiner pai */
  }

  .buttons {
    /*margin-top: 1%;*/
    display: inline-flex;
    gap: 10px;
    height: 35px;
    position: absolute;
    bottom: 5px;
    left: 242px;
  }


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
    width: 50%;
    height: 380px;
    background-color: yellow
  }

  #localTrack video {
    width: 100%;
    height: 100%; /* Define a altura e a largura do vídeo para preencher o contêiner */
    object-fit: cover; /* Controla como o vídeo se encaixa no contêiner */
    position: absolute;
    top: 0;
  }

  .remote_video_container {
    flex: 1;
    justify-content: flex-end;
    width: 50%;
    height: 380px;
    align-items: flex-end;
    /*background: #000;*/
    background-color: green
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

  #guests_video_container {
    display: flex;
    justify-content: flex-end;
    position: absolute;
    bottom: 155px;
    right: 15px;
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
