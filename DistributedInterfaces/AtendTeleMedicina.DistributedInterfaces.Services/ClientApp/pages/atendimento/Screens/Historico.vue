<template>
  <div>
    <component v-if="exibirHistorico" :is="'historico-no-atendimento'" :agendamento="agendamento" />
  </div>
</template>

<script>
  import _api from '../../../api'
  import HistoricoNoAtendimento from '../../../components/shared/HistoricoNoAtendimento'
  import moment from 'moment'
  moment.locale('pt-br')

  export default {
    name: 'Historico',
    props: {
      agendamento: {},
    },
    components: { HistoricoNoAtendimento },
    data() {
      return {
        //MODAL CONTROLS
        exibirHistorico: false
      }
    },
    created() {
      if (!this.agendamento.individuo) {
        this.$router.push({
          name: 'Agendamentos'
        })
        return
      }
    },

    async mounted() {
      this.exibirHistorico = true
    },

    methods: {
      
    }
  }
</script>

<style scoped>
  
</style>
