<template>
  <modal name="modalResultadoExames" :resizable="false" :draggable="true" :clickToClose="false" width="820" height="550">
    <VuePerfectScrollbar class="scroll-area-resultado-exames" :settings="scrollSettings" key="scrol-atendimento">

      <div class="window-header" style="padding: 5px;display: flex;justify-content: flex-end;">
        <el-button style="width:40px; display:flex; align-content:space-around; justify-content:center" type="danger" @click="hideModalResultadoExames" icon="el-icon-close"></el-button>
      </div>

      <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
        <div>
          <h2 style="padding-bottom: 5px" class="box-card--h2"> Resultados de exames</h2>
        </div>
        <component v-if="controlResultadoExames" :is="'exames-tabs'" :agendamento="agendamento" @emit="" />
      </el-col>

    </VuePerfectScrollbar>
  </modal>
</template>

<script>
  import _api from '../../../api'
  import VuePerfectScrollbar from 'vue-perfect-scrollbar'
  import ExamesTabs from '../../../components/shared/ExamesTabs'
  import moment from 'moment'
  moment.locale('pt-br')

  export default {
    name: 'ModalResultadoExames',
    props: {
      agendamento: {},
      openResultadoExames: ''
    },
    components: { VuePerfectScrollbar, ExamesTabs },

    data() {
      return {
        controlResultadoExames: false,
        scrollSettings: {
          maxScrollbarLength: 200
        },
      }
    },

    async mounted() {
      if (this.openResultadoExames == true) {
        this.controlResultadoExames = true
        this.$modal.show('modalResultadoExames')
      }
    },

    methods: {
      // HIDE MODAL
      hideModalResultadoExames() {
        this.controlComorbidades = false
        this.$modal.hide('modalResultadoExames')
        this.$emit('emit', 'resultado-exames-close')
      },
    }
  }
</script>

<style scoped>
  .scroll-area-resultado-exames {
    position: relative;
    width: 100%;
    height: 100%;
    padding-bottom: 10vh;
    overflow-x: hidden;
    overflow-y: auto;
  }
</style>
