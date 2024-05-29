<template>
  <el-row class="text-center">
    <p class="digit"><small>Sessão expira em: {{ hours }}:{{ minutes }}:{{ seconds }}</small></p>
    <!-- <p class="digit">{{ days }} Dias, {{ hours }}h, {{ minutes }}m, {{ seconds }}s</p> -->
  </el-row>
</template>

<script>
  export default {
    props: {
      tokenExpires: null
    },
    data () {
      return {
        now: Math.trunc((new Date()).getTime() / 1000),
        tempoLimite: this.tokenExpires
      }
    },
    watch: {
      now: function (val) {
        if (val > this.tempoLimite) {
         // console.log('expirou')
          this.$user.logout()
        }
      }
    },
    computed: {
      calculatedDate () {
        this.tempoLimite = Math.trunc(Date.parse(this.tempoLimite) / 1000)
        return this.tempoLimite
      },
      seconds () {
        return (this.calculatedDate - this.now) % 60
      },
      minutes () {
        return Math.trunc((this.calculatedDate - this.now) / 60) % 60
      },
      hours () {
        return Math.trunc((this.calculatedDate - this.now) / 60 / 60) % 24
      },
      days () {
        return Math.trunc((this.calculatedDate - this.now) / 60 / 60 / 24)
      }
    },
    mounted () {
      window.setInterval(() => {
        this.now = Math.trunc((new Date()).getTime() / 1000)
      }, 1000)
    }
  }
</script>

<style>
  .digit {
    color: #fff;
  }
</style>
