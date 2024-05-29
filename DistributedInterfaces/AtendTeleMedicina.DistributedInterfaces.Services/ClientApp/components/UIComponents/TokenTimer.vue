<template>
  <div class="alert alert-warning">
    {{ tokenExpires }}
    <countdown v-if="tokenAtivado" :time="tokenTempo" :interval="tokenAtualiza" @countdownend="countdownend">
      <template slot-scope="props">Token expira em: {{ props.days }} dias, {{ props.hours }}h, {{ props.minutes }}m, {{ props.seconds }}s</template>
    </countdown>
    <div class="alert alert-danger" v-if="tokenTempo < 1">Token Expirado.</div>
  </div>
</template>

<script>
  import VueCountdown from 'vue-countdown'
  import user from '../../mixins/User'
  export default {
    props: ['tokenExpires'],
    name: 'token-timer',
    components: {'countdown': VueCountdown},
    mixins: [user],
    data () {
      return {
        expires_in: localStorage.getItem('expires_in'),
        error: null,
        tokenTempo: this.tokenExpires,
        tokenAtivado: false,
        tokenAtualiza: 0
      }
    },
    mounted () {
      this.expires_in = localStorage.getItem('expires_in')
      if (this.expires_in) {
        this.tokenTempo = new Date(parseInt(this.expires_in)) - new Date()
        this.tokenAtivado = true
      }
    },
    methods: {
      countdownend: function () {
        this.tokenAtivado = false
//        this.userLogout()
      }
    },
    watch: {
      tokenExpires: function (val) {
        this.tokenAtualiza = parseInt(val)
        //console.log('o valor mudou para', val)
      }
    }
  }
</script>
