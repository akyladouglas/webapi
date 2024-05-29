<template>
  <modal name="modalMemed" :draggable="true" @opened="startMemed" @closed="closeMemed" :clickToClose="false" :width="1020" :height="690">
    <div class="window-header">
      <el-button type="danger" @click="hideModalMemed" icon="el-icon-close"></el-button>
    </div>
    <VuePerfectScrollbar class="scroll-area-memed" :settings="scrollSettings" key="scroll-memed">
      <prescricao-memed v-if="usePrescricao && componentMemed" :paciente="agendamento.individuo" :idAgendamento="agendamento.id" :profissional="agendamento.profissional" />
    </VuePerfectScrollbar>
  </modal>
</template>

<script>
  import _api from '../../../api'
  import VuePerfectScrollbar from 'vue-perfect-scrollbar'
  import PrescricaoMemed from '../PrescricaoMemed'
  export default {
    name: 'ModalMemed',
    props: {
      agendamento: {},
      openMemed: ''
    },
    components: { VuePerfectScrollbar, PrescricaoMemed },

    data() {
      return {
        usePrescricao: false,
        componentMemed: false,

        scrollSettings: {
          maxScrollbarLength: 200
        },
      }
    },

    async mounted() {
      await this.tokenMemed()
      if (this.openMemed == true) this.$modal.show('modalMemed')
    },

    methods: {
      hideModalMemed() {
        this.$modal.hide('modalMemed')
        this.$emit('emit', 'memed-close')
      },
      startMemed() {
        this.componentMemed = true
      },
      closeMemed() {
        this.componentMemed = false
      },

      async tokenMemed() {
        let { data } = await _api.memed.recoverTokenMedico()
        this.agendamento.profissional.tokenMemed = data.attributes.token
        this.usePrescricao = true
        this.componentMemed = true
      },
    }
  }
</script>

<style scoped>
  .scroll-area-memed {
    position: relative;
    width: 100%;
    height: 700px;
    padding-bottom: 50px;
    overflow-x: hidden;
  }
</style>
