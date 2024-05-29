<template>
  <!--<div>
    <p>{{ time | secondsInMinutes }}</p>
  </div>-->
</template>

<script>
  import moment from 'moment'

export default {
    name: 'stopwatch',
    props: {
      countTimer: {
        type: Number,
        default: false
      },
      running: {
        type: Boolean,
        default: false
      },
      resetWhenStart: {
        type: Boolean,
        default: false
      },
      restWhenStop: {
        type: Boolean,
        default: false
      },
      flag: false
    },
    watch: {
      running: function (newVal, oldVal) {
        if (newVal) this.startT()
        else this.stopT()
      }
    },
    mounted () {
      if (this.running) this.startT()
    },
    unmouted () {
      this.stopT()
    },
    data () {
      return {
        time: 0,
        timer: null
      }
  },
    methods: {
      stopT: function () {
        clearInterval(this.timer)
        if (this.restWhenStop) this.resetT()
      },
      startT: function () {
        if (this.countTimer != 0) this.time = this.countTimer
        this.timer = setInterval(() => {
          this.time++
          this.$emit('emit', { time: this.time })
          //console.log('this.time', this.time)
          //console.log('TIME', this.time)

          //POC CAMPINAS O MEDICO NÃO TEM TEMPO ESPECIFICO LIMITANDO
          //if (this.time == 1140) {
          //  this.info1Min()
          //}
          //if (this.time == 1200) {
          //  this.stopT()
          //  this.info()
          //}
        }, 1000)
      },
      resetT: function () {
        this.time = 0
      },
      info () {
        this.$alert('O tempo limite para atendimento chegou ao limite de 20 minutos', 'Informação', {
          confirmButtonText: 'OK'
        })
        document.getElementById('btn-teleatendimento').setAttribute('disabled', 'disabled')
        document.getElementById('btn-teleatendimento-bottom').setAttribute('disabled', 'disabled')
        document.getElementById('fechar-videochamada').click()
      },
      info1Min () {
        this.$alert('Resta 1 minuto para encerrar a vídeo chamada!', 'Informação', {
          confirmButtonText: 'OK'
        })
      }
    },
    filters: {
      secondsInMinutes: function (seconds) {
        return moment('2015-10-01')
          .startOf('day')
          .seconds(seconds)
          .format('mm:ss')
      }
    }
  }
</script>

<style scoped>
  @import url('https://fonts.googleapis.com/css?family=Share+Tech+Mono');

  p {
    font-family: 'Share Tech Mono', sans-serif;
    font-weight: bold;
    font-size: x-large;
    color: #000;
  }
</style>
