<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">

    <h1>Twilio Serverless Video Calling</h1>
    <form>
      <div>
        <el-button @click="addLocalVideo">Entrar como Médico</el-button>
        <el-button @click="connect">Entrar como Paciente</el-button>
      </div>
      <!-- Name: <input type="text" id="username">
      <button id="join_leave">Join call</button> -->
    </form>
    <p id="count"></p>
    <div id="container" class="container">
      <div id="local" class="participant">
        <div></div>
        <div>Me</div>
      </div>
      <!-- more participants will be added dynamically here -->
    </div>
  </el-col>
</template>

<script>
import _api from '../../api'
import Twilio, { connect, createLocalTracks, createLocalVideoTrack } from 'twilio-video'

export default {
  name: 'VideoChat',
  data () {
    return {
      localConnected: false,
      remoteConnected: false,
      room: null
    }
  },
  async created () {
    // this.diasDaSemana(this.filtroParams.dataInicial)
  },
  methods: {
    async addLocalVideo () {
      const track = await Twilio.createLocalVideoTrack()
      const video = document.getElementById('local').firstElementChild
     // console.log('o que é track', track)
      video.appendChild(track.attach())
      this.localConnected = true
    },
    async connect () {
      let {data} = await _api.teleConsulta.twGetToken('dc8338d9-67a2-4ed9-b86b-54e5529cc995')
      this.room = await connect(data.token)
      this.room.participants.forEach(this.participantConnected)
      this.room.on('participantConnected', this.participantConnected)
      this.room.on('participantDisconnected', this.participantDisconnected)
      this.connected = true
      this.updateParticipantCount()
    },
    async disconnect () {
      const container = document.getElementById('container')
      this.room.disconnect()
      while (container.lastChild.id !== 'local') {
        container.removeChild(container.lastChild)
      }
      //console.log('exibir Join Call')
      // button.innerHTML = 'Join call';
      this.connected = false
      this.updateParticipantCount()
    },
    participantConnected (participant) {
      const container = document.getElementById('container')
      const participantDiv = document.createElement('div')
      participantDiv.setAttribute('id', participant.sid)
      participantDiv.setAttribute('class', 'participant')

      const tracksDiv = document.createElement('div')
      participantDiv.appendChild(tracksDiv)

      const labelDiv = document.createElement('div')
      labelDiv.innerHTML = participant.identity
      participantDiv.appendChild(labelDiv)

      container.appendChild(participantDiv)

      participant.tracks.forEach(publication => {
        if (publication.isSubscribed) {
          this.trackSubscribed(tracks_div, publication.track)
        }
      })
      participant.on('trackSubscribed', track => this.trackSubscribed(tracksDiv, track))
      participant.on('trackUnsubscribed', this.trackUnsubscribed)
      this.updateParticipantCount()
    },
    updateParticipantCount () {
      const count = document.getElementById('count')
      if (!this.connected) {
        count.innerHTML = 'Disconnected.'
      } else {
        count.innerHTML = (this.room.participants.size + 1) + ' participants online.'
      }
    },
    participantDisconnected (participant) {
      document.getElementById(participant.sid).remove()
      this.updateParticipantCount()
    },
    trackSubscribed (div, track) {
      div.appendChild(track.attach())
    }
  }
}
</script>

<style scoped>
.container {
    margin-top: 20px;
    width: 100%;
    display: flex;
    flex-wrap: wrap;
}
.participant {
    margin-bottom: 5px;
    margin-right: 5px;
}
.participant div {
    text-align: center;
}
.participant div:first-child {
    width: 240px;
    height: 180px;
    background-color: #ccc;
    border: 1px solid black;
}
.participant video {
    width: 100%;
    height: 100%;
}
</style>
