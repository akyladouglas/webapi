<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
    <div class="container-fluid chat_container" id="chat">
        <div class="row">
          <Rooms />
          <Video />
          <Logs />
        </div>
    </div>
  </el-col>
</template>

<script>
// import components
import Rooms from './Rooms'
import Video from './Video'
import Logs from './Logs'
import AddRoom from './AddRoom'

export default {
  name: 'TeleAtendimento',
  components: {
    Rooms,
    Video,
    Logs,
    AddRoom
  },
  data () {
    return {
      username: '',
      authenticated: false
    }
  },
  methods: {
    submitUsername (username) {
      if (!username) {
        return alert('please provide a username')
      }

      this.authenticated = true
    }
  }
}
</script>

<style scoped>
 #chat {
   font-family: 'Avenir', Helvetica, Arial, sans-serif;
   -webkit-font-smoothing: antialiased;
   -moz-osx-font-smoothing: grayscale;
   text-align: center;
   color: #2c3e50;
   margin-top: 60px;
 }
 .box {
   border: 1px solid gray;
 }

.Botton {
   color: #fff;
   background-color: #4d555f;
   border-color: #303840;
   padding: 8px;
   font-weight: bolder;
 }
</style>