<template>
  <modal name="modalMedicoes" :resizable="false" :draggable="true" :clickToClose="false" width="1000" height="600">
    <VuePerfectScrollbar class="scroll-area-medicoes" :settings="scrollSettings" key="scrol-atendimento">
      <div class="window-header" style="padding: 5px;display: flex;justify-content: flex-end;">
        <el-button style="width:40px; display:flex; align-content:space-around; justify-content:center" type="danger" @click="hideModalMedicoes" icon="el-icon-close"></el-button>
      </div>
      <el-row style="margin-left:5px; ">
        <h2 style="padding-bottom: 5px" class="box-card--h2"> Histórico de Medições</h2>
      </el-row>
      <component v-if="controlMedicoes" :is="'graficos-das-medicoes'" :agendamento="agendamento" :origem="'Modal'" />
    </VuePerfectScrollbar>
  </modal>
</template>

<script>
  import _api from '../../../api'
  import VuePerfectScrollbar from 'vue-perfect-scrollbar'
  import GraficosDasMedicoes from '../../../components/shared/GraficosDasMedicoes'
  export default {
    name: 'ModalMedicoes',
    props: {
      agendamento: {},
      openMedicoes: ''
    },
    components: { VuePerfectScrollbar, GraficosDasMedicoes },

    data() {
      return {
        controlMedicoes: false,
        scrollSettings: {
          maxScrollbarLength: 200
        },
      }
    },

    async mounted() {
      if (this.openMedicoes == true) {
        this.$modal.show('modalMedicoes')
        this.controlMedicoes = true
      }
    },

    methods: {
      // HIDE MODAL
      hideModalMedicoes() {
        this.controlMedicoes = false
        this.$modal.hide('modalMedicoes')
        this.$emit('emit', 'medicoes-close')
      },
    }
  }
</script>

<style scoped>
  .scroll-area-medicoes {
    position: relative;
    width: 100%;
    height: 100%;
    padding-bottom: 10vh;
    overflow-x: hidden;
    overflow-y: auto;
  }
</style>
