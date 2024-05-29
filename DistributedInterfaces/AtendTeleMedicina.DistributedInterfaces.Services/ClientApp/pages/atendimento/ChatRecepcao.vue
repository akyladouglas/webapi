<template>
 <el-row :gutter="24" class="p-2">
    <el-col :xs="17" :sm="17" :md="17" :lg="17" :xl="17">
      <div id="chat">
        <el-card class="mb-2"  v-show="activeConversation">Você está conversando com {{ destiny.nome}}</el-card>
        <el-card  v-show="activeConversation">
          <Conversation v-if="activeConversation" :active-conversation="activeConversation" :name="name" :destiny="destiny" @emitNotifyMsg="handleNotifyMsg" />
        </el-card>
      </div>
       <span>&nbsp;</span>
    </el-col>
    <el-col :xs="7" :sm="7" :md="7" :lg="7" :xl="7">
      <VuePerfectScrollbar class="scroll-area-recepcao" :settings="scrollSettings" key="scrol-recepcao">
      <el-card class="el-menu-chat-card">
        <h4 class="pl-3">Contatos</h4>
        <el-menu
          class="el-menu-chat"
          text-color="#000"
          active-text-color="#E6F3F0">
          
            <el-menu-item v-for="(item, index) in api.contatos" :key="item.id" :index="item.id" @click="twEntrarNoChat(item)">
              <i class="el-icon-user pb-1"></i>
              <el-tooltip :content="item.nome" placement="top">
                <span class="el-menu-chat--nome" v-html="getNomeContato(item.nome, 20)"></span>
              </el-tooltip>
            </el-menu-item>
        </el-menu>
      </el-card>
      </VuePerfectScrollbar>
    </el-col>
  </el-row>
</template>

<script>
import VuePerfectScrollbar from 'vue-perfect-scrollbar'
import {Client as ConversationsClient} from '@twilio/conversations'
import Utils from '../../mixins/Utils'
import _api from '../../api'
import Conversation from './Conversation'

export default {
  props: ['destinyId'],
  components: { VuePerfectScrollbar, Conversation },
  mixins: [Utils],
  data () {
    return {
      scrollSettings: {
        maxScrollbarLength: 50
      },
      statusString: '',
      activeConversation: null,
      name: '',
      destiny: {},
      nameRegistered: false,
      isConnected: false,
      api: {
        profissionais: [],
        usuarios: [],
        contatos: []
      },
      loading: {
        profissionais: false
      }
    }
  },
  watch: {
    destinyId (val) {
      let contato = this.api.contatos.find(x => x.id === val)
      if (contato) this.twEntrarNoChat(contato)
    }
  },
  async created () {
    await this.getProfissionais()
    await this.getUsuarios()
  },
  methods: {
    async getProfissionais () {
      this.loading.profissionais = true
      let params = {
        skip: 1,
        take: 999
      }
      let { data, status } = await _api.profissionais.get(params)
      if (status === 502) this.loading.profissionais = false
      this.api.profissionais = (status === 200) ? data : []
      this.api.profissionais.forEach(x => {
        if (x.id !== this.$auth.user().id) {
          this.api.contatos.push({
            id: x.id,
            nome: x.nome,
            username: x.username
          })
        }
      })
      this.loading.profissionais = false
    },
    async getUsuarios () {
      this.loading.usuarios = true
      let params = {
        skip: 1,
        take: 999
      }
      let { data, status } = await _api.usuarios.getSimplified(params)
      if (status === 502) this.loading.usuarios = false
      this.api.usuarios = (status === 200) ? data : []
      this.api.usuarios.forEach(x => {
        const isRecepcionista = x.claims.some(element => {
          if (element.claimValue === 'Recepcionista') {
            return true
          }
          return false
        })
        if (x.id !== this.$auth.user().id && isRecepcionista) {
          this.api.contatos.push({
            id: x.id,
            nome: x.nome,
            username: x.username
          })
        }
      })
      this.loading.usuarios = false
    },
    async initConversationsClient (userId) {
      window.conversationsClient = ConversationsClient
      const token = await this.getUserChatToken()
      this.conversationsClient = new ConversationsClient(token)
      this.statusString = 'Conectando…'
      this.conversationsClient.on('connectionStateChanged', async (state) => {
        switch (state) {
          case 'connected':
            this.statusString = 'Contectado.'
            this.isConnected = true
            await this.createConversation(userId)
            break
          case 'disconnecting':
            this.statusString = 'Disconnecting from Twilio…'
            break
          case 'disconnected':
            this.statusString = 'Disconnected.'
            break
          case 'denied':
            this.statusString = 'Failed to connect.'
            break
        }
      })
    },
    async getUserChatToken () {
      let { data } = await _api.teleConsulta.twGetChatToken(this.$auth.user().id)
      return data
    },
    async twEntrarNoChat (item) {
      this.destiny = item
      if (this.activeConversation) {
        this.activeConversation = null
        this.name = null
        this.nameRegistered = false
      }
      this.name = this.$auth.user().id
      this.nameRegistered = true
      await this.initConversationsClient(this.destiny.id)
      setTimeout(() => { // Temporario
        this.scrollChatToEnd()
      }, 4000)
    },
    scrollChatToEnd () {
      // var container = this.$el.querySelector('#conversation')
      // console.log('container', container)
      // container.scrollTop = container.scrollHeight
      // // jQuery('#conversation').animate({
      // //   scrollTop: jQuery(window).scrollTop() + jQuery('#conversation').prop('scrollHeight')
      // // }, 1000)
    },
    async createConversation () {
      this.name = this.$auth.user().id
      let uniqueName = ''
      if (this.mxHasAny('Profissional')) {
        uniqueName = `${this.destiny.id}_${this.$auth.user().id}-Recepcao`
      } else {
        uniqueName = `${this.$auth.user().id}_${this.destiny.id}-Recepcao`
      }
      try {
        const newConversation = await this.conversationsClient.createConversation({uniqueName: uniqueName})
        const joinedConversation = await newConversation.join().catch(err => console.log(err))
        await joinedConversation.add(this.$auth.user().id).catch(err => console.log(err))
        await joinedConversation.add(this.destiny.id).catch(err => console.log(err))
        this.activeConversation = joinedConversation
      } catch (e) {
        this.activeConversation = await this.conversationsClient.getConversationByUniqueName(uniqueName)
      }
    },
    getNomeContato (sentence, length) {
      if (sentence.length <= length) return sentence
      return `<span title="${sentence}">${sentence.substring(0, length)} ...</span>`
    },
    handleNotifyMsg (msg) {
      this.$emit('emitNotifyMsg', msg)
    }
  }
}
</script>

<style>
.scroll-area-recepcao {
  position: relative;
  margin: auto;
  width: 100%;
  height: 570px;
  overflow-x: hidden;
}

.chat--empty .el-empty__image img {
    padding-top: calc(15vh);
}

.el-menu-chat-card .el-card__body {
    padding: 10px 0;
}

.el-menu-chat-card .el-icon-user {
    color: #000;
}

.el-menu-chat {
  background: #FFF;
}

.el-menu-chat .el-menu-item {
  width: 100%;
  height: 35px;
  line-height: 35px;
  padding-left: 10px !important;
  margin-left: 1px !important;
  border-right: 1px solid #FFF;
}

.el-menu-chat .el-menu-item.is-active {
    color: #5eb126 !important;
}

.el-menu-chat--nome {
  font-size: 11px;
  text-transform: uppercase;
}

ul {
  list-style-type: none;
  padding: 0;
}

li {
/*  display: inline-block;*/
  margin: 0 10px;
}

a {
  color: #42b983;
}

.el-menu-chat-card .el-card__body {
    padding: 10px 0;
}
</style>
