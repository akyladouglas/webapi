<template>

  <el-container>
    <el-container v-show="mxDynamicWidth() > 590">
      <Header />
      <el-main class="main">
        <transition name="fade" mode="out-in">
          <router-view></router-view>
        </transition>
      </el-main>
    </el-container>

    <el-container v-show="mxDynamicWidth() <= 590" style="display: flex; flex-direction: row; position: relative">
      <SideBar />
      <el-main class="main" :style="{ position: 'relative', 'max-height': mxDynamicHeight() + 'px' }">
        <transition name="fade" mode="out-in">
          <router-view></router-view>
        </transition>
      </el-main>
    </el-container>

    <footer class="navbar-fixed-bottom footer--empresa">
      <a>
        <img :src="'../../assets/img/logo-footer-novetech-p.png'"
             class="img-responsive center-block login--logo--novetech"
             :title="'Novetech - Soluções Tecnológicas'" /> Meeds - v2.0.5 - Novetech Soluções Tecnológicas
      </a>

      <div class="box--chat" v-if="mxHasAccess('Recepcionista', 'Médico', 'MédicoAD')">
        <el-badge :hidden="chatIcon === 'primary'" value="nova" class="chat-item">
          <el-popover placement="top-start"
                      width="800"
                      @after-leave="onHideChat"
                      trigger="click">
            <el-row>
              <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
                <div class="box--chat__container">
                  <ChatRecepcao :destinyId="destinyId" @emitNotifyMsg="handleNotifyMsg" />
                </div>
              </el-col>
            </el-row>
            <el-button slot="reference" :type="chatIcon" icon="fas fa-comments fa-2xl" circle @click="handleOpenChat"></el-button>
          </el-popover>
        </el-badge>
      </div>
    </footer>
  </el-container>

</template>

<script>
  import jQuery from 'jquery'
  import Countdown from '../UIComponents/Countdown.vue'
  import ChatRecepcao from '../../pages/atendimento/ChatRecepcao'
  import VuePerfectScrollbar from 'vue-perfect-scrollbar'
  import Header from './Header'
  import SideBar from './SideBar'
  import MenuUsuario from './MenuUsuario'
  import Utils from '../../mixins/Utils'
  import ScreenWidth from '../../mixins/ScreenWidth'
  import ScreenHeight from '../../mixins/ScreenHeight'
  import audioNotificacao from '../../assets/audios/chat-notification.mp3'
  import Hub from '../../Hub'

  var _hub = new Hub()
  var somNotificao = new Audio(audioNotificacao)

  export default {
    name: 'Esqueleto',
    mixins: [Utils, ScreenWidth, ScreenHeight],
    components: { Header, SideBar, MenuUsuario, VuePerfectScrollbar, Countdown, ChatRecepcao },
    data () {
      return {
        socket: null,
        destinyId: null,
        chatIcon: 'primary',
        chatIsOpen: false,
      }
    },

    async created () {
      _hub.connection.start()
        .then(() => {
          console.log('Hub connected Full')
          _hub.connection.on('ReceivedMessage', (msg) => {
            this.handleNotifyMessage(msg)
          })
        }).catch(e => {
          console.log('Error connection to Hub', e)
        })
    },

    methods: {

      handleNotifyMessage (msg) {
        const chatters = msg.split(',')
        if (chatters[0] === this.$auth.user().id) {
          if (!this.chatIsOpen) {
            this.destinyId = chatters[1]
          }
          this.notifyClient(msg)
        }
      },
      notifyClient (msg) {
        if (!this.chatIsOpen) {
          this.chatIcon = 'warning'
          this.playSound()
        }
      },
      playSound () {
        somNotificao.play()
      },
      handleOpenChat () {
        this.chatIsOpen = true
        this.chatIcon = 'primary'
      },
      handleNotifyMsg (msg) {
        _hub.connection.invoke('SendMessage', msg)
      },
      onHideChat () {
        this.chatIsOpen = false
      },
      refreshToken () {
        this.$user.refreshToken()
          .then(res => {
            this.tokenExpires = this.moment(res.expires_in).format('YYYY-MM-DD HH:mm:ss')
          })
          .catch((e) => {
            console.log(e.response.data)
          })
      },
      atualiza () {
        this.tokenExpires = '2018-09-07 16:00:00'
      },
      logout () {
        this.$user.logout()
      },
    }
  }
</script>


<style>






  .chat-item {
    margin-top: 10px;
    margin-right: 40px;
  }


 
  
  .navbar-fixed-top, .navbar-fixed-bottom {
    position: fixed;
    right: 0;
    left: 0;
    z-index: 1030;
    bottom: 0;
    margin-bottom: 0;
    border-width: 1px 0 0;
  }
  .footer--empresa {
    text-align: right;
    padding-right: 25px;
    font-size: 11px;
  }
  .footer--empresa a {
    text-decoration: none;
  }
  .footer--empresa img {
    width: 15px;
    height: 15px;
    margin-bottom: 3px;
  }

  .box--chat {
    position: fixed;
    bottom: 18px;
    right: 0px;
  }

  .box--chat__container {
    height: 600px;
    background-image: url('../../assets/img/chat-background.jpg');
    background-color: #F7F7F7;
  }

  a:visited {
    color: #015f6b;
  }

  a:link {
    color: #015f6b;
  }

  .el-menu--horizontal > .el-submenu.is-active .el-submenu__title {
    border: none;
    color: #303133;
  }
  .el-submenu__title i {
      color: #FFF
  }

  ul.el-menu > li.el-submenu:hover > div > i {
    color: #015f6b
  }

  ul.el-menu > li.el-submenu > div > i {
      transition: color 0.2s ease;
  }
  .is-active > div > i {
      color: #015f6b
  }

  .el-menu--horizontal > .el-submenu .el-submenu__title {
    height: 60px;
    line-height: 60px;
    border: none !important;
    color: #fff;
  }

  .el-menu.el-menu--horizontal {
    border: none;
  }

  .el-menu--horizontal .el-menu-item:not(.is-disabled):hover {
    background-color: #43bec6;
  }

  .el-menu-item{
      margin: 0px !important
  }

  .el-menu-item a {
    color: #FFFFFF
  }
    .el-menu-item a:hover {
      color: #015f6b !important
    }

  .el-menu a:hover > li > a {
    color: #015f6b
  }
  .el-menu a li > * {
      color: #FFF
  }
  /*
  .el-menu .router-link-active > li > a > i > span {
    color: #015f6b !important
  }*/
  /*.router-link-active li a i {
    color: #015f6b !important
  }*/

  /*.el-menu .router-link-active > li > a > i {
    color: #015f6b !important
  }*/
    a: visited {
    color: #FFFFFF
  }
</style>
