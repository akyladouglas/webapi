<template>
  <div>
    <component v-if="exibirSintomas" :is="'sintomas'" :agendamento="agendamento" />
  </div>
</template>

<script>
  import _api from '../../../api'
  import Sintomas from '../../sintomas/Sintoma'
  import moment from 'moment'
  moment.locale('pt-br')

  export default {
    name: 'HistoricoSintomas',
    props: {
      agendamento: {},
    },
    components: { Sintomas },
    data() {
      return {
        //MODAL CONTROLS
        exibirSintomas: false
      }
    },
    created() {

    },

    beforeDestroy() { this.exibirSintomas = false },

    async mounted() {
      this.exibirSintomas = true
    },

    methods: {
      
    }
  }
</script>

<style scoped>
  
</style>
