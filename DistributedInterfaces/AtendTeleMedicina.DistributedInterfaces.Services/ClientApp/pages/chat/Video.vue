<template>
   <div class="col-md-9 box">
       <div class="roomTitle">
           <span v-if="loading"> Loading... {{roomName}}</span>
           <span v-else-if="!loading && roomName"> Connected to {{roomName}}</span>
           <span v-else>Select a room to get started</span>
       </div>
       <div class="row">
        <div class="col-md-6">
            <div id="remoteTrack"></div>
            <span>Paciente</span>
        </div>
        <div class="col-md-6">
            <div id="localTrack"></div>
            <span>Médico</span>
        </div>
       </div>
   </div>
</template>

<script>
import { EventBus } from '../../Event'
import Twilio, { connect, createLocalTracks, createLocalVideoTrack } from 'twilio-video'
import _api from '../../api'

export default {
  name: 'Video',
  data () {
    return {
      loading: false,
      data: {},
      localTrack: false,
      remoteTrack: '',
      activeRoom: '',
      previewTracks: '',
      identity: '',
      roomName: null
    }
  },
  props: ['username'], // props that will be passed to this component
  created () {
    EventBus.$on('show_room', (room_name) => {
      this.createChat(room_name)
    })

    // When a user is about to transition away from this page,
    // disconnect from the room, if joined.
    window.addEventListener('beforeunload', this.leaveRoomIfJoined)
  },
  methods: {
    // Generate access token
    async getAccessToken () {
      let {data} = await _api.teleConsulta.twGetToken('dc8338d9-67a2-4ed9-b86b-54e5529cc995')
      return data
    },

    // Trigger log events
    dispatchLog (message) {
      EventBus.$emit('new_log', message)
    },

    // Attach the Tracks to the DOM.
    attachTracks (tracks, container) {
      tracks.forEach(function (track) {
        container.appendChild(track.attach())
      })
    },

    // Attach the Participant's Tracks to the DOM.
    attachParticipantTracks (participant, container) {
      // let tracks = Array.from(participant.tracks.values())
      let tracks = Array.from(participant.videoTracks.values())[0]
     // console.log('as tracks', tracks)
      this.attachTracks(tracks, container)
    },

    // Detach the Tracks from the DOM.
    detachTracks (tracks) {
      tracks.forEach((track) => {
        track.detach().forEach((detachedElement) => {
          detachedElement.remove()
        })
      })
    },

    // Detach the Participant's Tracks from the DOM.
    detachParticipantTracks (participant) {
      let tracks = Array.from(participant.tracks.values())
      this.detachTracks(tracks)
    },

    // Leave Room.
    leaveRoomIfJoined () {
      if (this.activeRoom) {
        this.activeRoom.disconnect()
      }
    },

    createChat (room_name) {
      this.loading = true
      const VueThis = this

      this.getAccessToken().then((data) => {
        VueThis.roomName = null
        const token = data.token
        let connectOptions = {
          name: room_name,
          automaticSubscription: true,
          // logLevel: 'debug',
          audio: true,
          video: { width: 400 }
        }
        // before a user enters a new room,
        // disconnect the user from they joined already
        this.leaveRoomIfJoined()

        // remove any remote track when joining a new room
        document.getElementById('remoteTrack').innerHTML = ''

        Twilio.connect(token, connectOptions).then(function (room) {
          //console.log('Successfully joined a Room: ', room)
          VueThis.dispatchLog('Successfully joined a Room: ' + room_name)

          // set active toom
          VueThis.activeRoom = room
          VueThis.roomName = room_name
          VueThis.loading = false

          // Attach the Tracks of all the remote Participants.
          room.participants.forEach(function (participant) {
           // console.log('forEach', participant)
            let previewContainer = document.getElementById('remoteTrack')
            VueThis.attachParticipantTracks(participant, previewContainer)
          })

          // When a Participant joins the Room, log the event.
          room.on('participantConnected', function (participant) {
           // console.log('participantConnected', participant)
            // let previewContainer = document.getElementById('remoteTrack')
            participant.tracks.forEach(publication => {
              if (publication.isSubscribed) {
                const track = publication.track
                document.getElementById('remoteTrack').appendChild(track.attach())
              }
            })
            participant.on('trackSubscribed', track => {
             // console.log('subscreveu-se', track)
              if (track.kind === 'audio' || track.kind === 'video') {
               // console.log('era audio')
                document.getElementById('remoteTrack').appendChild(track.attach())
              }
            })
            // VueThis.attachTracks(participant.tracks, previewContainer)
            // participant.tracks.forEach(publication => {
            //   console.log('vai no foreach', publication)
            //   const track = publication.track
            //   previewContainer.appendChild(track.attach())
            // })

            VueThis.dispatchLog("Joining: '" + participant.identity + "'")
          })

          // When a Participant adds a Track, attach it to the DOM.
          room.on('trackAdded', function (track, participant) {
           // console.log('trackAdded', track)
            VueThis.dispatchLog(participant.identity + ' added track: ' + track.kind)
            let previewContainer = document.getElementById('remoteTrack')
            VueThis.attachTracks([track], previewContainer)
          })

          // When a Participant removes a Track, detach it from the DOM.
          room.on('trackRemoved', function (track, participant) {
            //console.log('trackRemoved', track)
            VueThis.dispatchLog(participant.identity + ' removed track: ' + track.kind)
            VueThis.detachTracks([track])
          })
          // When a Participant leaves the Room, detach its Tracks.
          room.on('participantDisconnected', function (participant) {
            //console.log('participantDisconnected', participant)
            VueThis.dispatchLog("Participant '" + participant.identity + "' left the room")
            VueThis.detachParticipantTracks(participant)
          })

          // if local preview is not active, create it
          if (!VueThis.localTrack) {
            createLocalVideoTrack().then(track => {
              let localMediaContainer = document.getElementById('localTrack')
             // console.log('localMediaContainer', localMediaContainer)
             // console.log('track', track)
              localMediaContainer.appendChild(track.attach())
              VueThis.localTrack = true
            })
          }
        })
      })
    }
  }
}
</script>

<style >
   #localTrack {
      border: 3px solid rgb(124, 129, 124);
      background: #a2fcae;
      width: 100%;
      height: 160px;
   } 
   #localTrack video {
       margin: 0px;
       max-width: 50% !important;
       background-repeat: no-repeat;
   }

   #remoteTrack {
       border: 3px solid rgb(124, 129, 124);
       background: #f1baba;
       width: 100%;
       height: 255px;
   }

   #remoteTrack video {
       margin: 0px;
       max-width: 50% !important;
       background-repeat: no-repeat;
   }
   .spacing {
     padding: 20px;
     width: 100%;
   }
   .roomTitle {
       border: 1px solid rgb(124, 129, 124);
       padding: 4px;
       color: dodgerblue;
   }
</style>
