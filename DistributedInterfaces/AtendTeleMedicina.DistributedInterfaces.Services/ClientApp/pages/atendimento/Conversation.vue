<template>
  <div id="conversation-container">
    <div id="containerChat">
      <ul class="list-unstyled">
        <li v-for="(item, index) in groupedMessages" :key="index">
          <div class="text-center pb-2">{{moment(item.date).format("DD [de] MMMM [de] YYYY")}}</div>

            <ul class="list-unstyled mt-2">
              <li v-for="(message, index) in item.messages" :key="`c-${index}`"
              :class="message.state.author !== name ? `msg__paciente index-${index}` : `msg__chat index-${index}`">
                <span>
                  {{message.body}}
                  <i class="msg__hora">{{ moment(message.state.dateUpdated).format('HH:mm') }}</i>
                </span>
              </li>
            </ul>

        </li>
      </ul> 
    </div>
    <el-row :gutter="20" class="mt-4">
      <el-col :span="20">
        <el-input id="nova-mensagem" @keyup.enter.native="sendMessage" v-model="messageText" />
      </el-col>
      <el-col :span="3">
        <el-button icon="el-icon-s-promotion" :disabled="messageText.length < 1" type="success" class="chat--button" @click="sendMessage" />
      </el-col>
    </el-row>
  </div>
</template>

<script>
  import jQuery from 'jquery'
  import VuePerfectScrollbar from 'vue-perfect-scrollbar'

  export default {
    components: { VuePerfectScrollbar },
    props: ['activeConversation', 'name', 'destiny'],
    data () {
      return {
        socket: null,
        messages: [],
        groupedMessages: [],
        messageText: '',
        isSignedInUser: false
      }
    },
    watch: {
      messages: function (val) {
        if (val) {
          setTimeout(() => {
            this.scrollChatToEnd()
          }, 1000)
        }
      }
    },
    async mounted () {
      this.activeConversation.getMessages()
        .then((newMessages) => {
          this.messages = [...this.messages, ...newMessages.items]
          var groupByDays = this._(this.messages)
            .groupBy(item => this.moment(item.state.dateUpdated).startOf('day').format('YYYY-MM-DD'))
            .map((value, key) => ({date: key, messages: value}))
            .value();
          this.groupedMessages = groupByDays
        })
      this.activeConversation.on('messageAdded', (message) => {
        if (!message.conversation.channelState.uniqueName.includes("Recepcao")) return
        let today = this.moment().format('YYYY-MM-DD')
        let index = this.groupedMessages.findIndex(x => x.date === today)
        if (index === -1) {
          this.groupedMessages.push({
            date: today,
            messages: [message]
          })
        } else {
          this.groupedMessages[index].messages.push(message)
        }
        this.messages.push(message)
        this.scrollChatToEnd()
      })
    },
    methods: {
      async sendMessage () {
        this.activeConversation.sendMessage(this.messageText)
          .then(() => {
            this.messageText = ''
          })
        let msg = `${this.destiny.id},${this.$auth.user().id}`
        this.$emit('emitNotifyMsg', msg)
      },
      scrollChatToEnd () {
        jQuery('#containerChat').animate({
          scrollTop: jQuery(window).scrollTop() + jQuery('#containerChat').prop('scrollHeight')
        }, 1000)
      }
    }
  }
</script>

<style scoped>
#containerChat {
  max-height:400px; 
  min-height: 400px;
  overflow-y: auto;
}

.chat--button {
  height: 37px !important
}

.scroll-area {
  margin: auto;
  width: 100%;
  max-height: 340px;
  background: #FFF;
  padding: 15px;
}

  .space-card {
    margin-bottom: 20px;
  }

  .align-center {
    display: flex !important;
    align-items: center !important;
    justify-content: center !important;
  }

  .space-top{
      margin-top: 20px;
  }

  .v--modal-overlay[data-modal="hello-world"] {
    background: transparent;
  }

  .vm--overlay {
    display: flex;
    width: 14%;
    background-color: transparent;
    /*align-items: center;
    justify-content: center;*/
  }
  .vm--container { 
    width: 300px;
    height: 600px;
    float: right;
  }
  /** Twilio classes */
  .local_video_container {
    width: 200px;
    height: 110px;
    position: absolute;
    bottom: 40px;
    right: 30px;
  }

  #localTrack video {
    width: 100%;
    height: 100%;
  }

  .remote_video_container {
    flex: 1;
    width: 100%;
    height: 380px;
    align-items: flex-end;
    justify-content: flex-end;
    background: #000;
  }

  #remoteTrack video {
    width: 100%;
    height: 380px;
  }


    .roomTitle {
      border: 1px solid rgb(124, 129, 124);
      padding: 4px;
      color: dodgerblue;
    }


.scroll-area {
  position: relative;
  width: 100%;
  height: 340px;
  overflow-x: hidden;
}
  .scroll-area2 {
    position: relative;
    width: 100%;
    height: 600px;
    padding-bottom:50px;
    overflow-x: hidden;
  }

  .scroll-area-memed {
    position: relative;
    width: 100%;
    height: 700px;
    padding-bottom:50px;
    overflow-x: hidden;
  }

#chat .el-input__inner {
  height: 36px;
}

#chat li {
  font-size: 12px;
  font-weight: 400;
  display: block;
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

.tempo__atendimento {
  position: absolute;
  color: #fff;
}
.divButtonExame {
  display: flex;
  flex-direction: row;
  justify-content: flex-end;
}
  .div-container-fichas {
    transform: translate(-50%, -50%);
  }

  .container-fichas {
    color: red;
    font-size: 16px;
    font-family: serif;
    text-align: center;
    animation: animation-fichas 2.5s linear infinite;
  }
  @keyframes animation-fichas {
    0% {
      opacity: 0;
    }

    50% {
      opacity: 0.7;
    }

    100% {
      opacity: 0;
    }
  }














.conversation-container {
 margin: 0 auto;
 max-width: 400px;
 height: 600px;
 padding: 0 20px;
 border: 3px solid #f1f1f1;
 overflow: scroll;
}
 
.bubble-container {
 text-align: left;
}
 
.bubble {
 border: 2px solid #f1f1f1;
 background-color: #fdfbfa;
 border-radius: 5px;
 padding: 10px;
 margin: 10px 0;
 width: 230px;
 float: right;
}
 
.myMessage .bubble {
 background-color: #abf1ea;
 border: 2px solid #87E0D7;
 float: left;
}
 
.name {
 padding-right: 8px;
 font-size: 11px;
}
 
::-webkit-scrollbar {
 width: 10px;
}
 
::-webkit-scrollbar-track {
 background: #f1f1f1;
}
 
::-webkit-scrollbar-thumb {
 background: #888;
}
 
::-webkit-scrollbar-thumb:hover {
 background: #555;
}
</style>
