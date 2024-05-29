<template>
  <modal name="modalInformacoesAdicionais" :resizable="false" :draggable="true" :clickToClose="false" width="600" height="350">
    <div style="padding: 10px">
      <el-row :gutter="24">
        <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
          <h2 style=" padding-bottom: 5px" class="box-card--h2">Informações Adicionais</h2>
        </el-col>
      </el-row>

      <el-row :gutter="24">
        <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" style="margin-top: 20px">
          <el-form :model="formInformacoesAdicionais" status-icon ref="formInformacoesAdicionais" label-width="120px" label-position="top" class="forms--usuario">
            <el-form-item label="Alergias" prop="alergias">
              <el-input type="input" v-model="formInformacoesAdicionais.alergias" />
            </el-form-item>
            <el-form-item label="Antecedentes Familiares" prop="antecedentes">
              <el-input type="input" v-model="formInformacoesAdicionais.antecedentes" />
            </el-form-item>
          </el-form>
        </el-col>
      </el-row>

      <div style="display: flex; flex-direction: row; justify-content: flex-end; margin-top: 0px">
        <el-button type="default" size="large" @click="hideModalInformacoesAdicionais()" >Cancelar</el-button>
        <el-button type="primary" size="large" @click="onClickSalvar()">Salvar</el-button>
      </div>

    </div>
  </modal>
</template>

<script>
  import _api from '../../../api'
  import VuePerfectScrollbar from 'vue-perfect-scrollbar'
  import moment from 'moment'
  moment.locale('pt-br')

  export default {
    name: 'ModalInformacoesAdicionais',
    props: {
      agendamento: {},
      openInformacoesAdicionais: ''
    },
    components: { VuePerfectScrollbar },

    data() {
      return {
        formInformacoesAdicionais: {
          alergias: '',
          antecedentes: '',
        },
        scrollSettings: {
          maxScrollbarLength: 200
        }, 
      }
    },

    async created() {
      //verificando se tem no store
      if (Object.keys(this.$store.state.user.formModalInformacoesAdicionais).length > 0) {
        let storeObj = this.$store.state.user.formModalInformacoesAdicionais

        //adicionando alergias do store
        if (storeObj.alergias != null && storeObj.alergias != '' && storeObj.alergias != undefined) {
          this.formInformacoesAdicionais.alergias = storeObj.alergias
        }

        //adicionando antecedentes do store
        if (storeObj.antecedentes != null && storeObj.antecedentes != '' && storeObj.antecedentes != undefined) {
          this.formInformacoesAdicionais.antecedentes = storeObj.antecedentes
        }
      }
    },

    async mounted() {
      if (this.openInformacoesAdicionais == true) {

        if (Object.keys(this.$store.state.user.formModalInformacoesAdicionais).length == 0) {
          await this.getUltimoAtendimento()
          this.$modal.show('modalInformacoesAdicionais')
        } else {
          this.$modal.show('modalInformacoesAdicionais')
        }
      }
    },

    methods: {

      // RETORNA O ULTIMO ATENDIMENTO DO PACIENTE
      async getUltimoAtendimento() {
        let { data, status } = await _api.atendimentos.getMaxAtendimentoByIndividuoId(this.agendamento.individuoId)
        let ultimoAtendimento = (status === 200) ? data : {}

        if (Object.keys(ultimoAtendimento).length > 0) {
          var obj = {}
          if (ultimoAtendimento.alergias) {
            obj.alergias = ultimoAtendimento.alergias
            this.$set(this.formInformacoesAdicionais, 'alergias', ultimoAtendimento.alergias);
            this.formInformacoesAdicionais.alergias = ultimoAtendimento.alergias
          }
          if (ultimoAtendimento.antecedentes) {
            obj.antecedentes = ultimoAtendimento.antecedentes;
            this.$set(this.formInformacoesAdicionais, 'antecedentes', ultimoAtendimento.antecedentes);
            this.formInformacoesAdicionais.antecedentes = ultimoAtendimento.antecedentes
          }
          this.$store.dispatch('setFormInformacoes', obj)
        }

      },

      async onClickSalvar(val) {

        let obj = {
          alergias: (this.formInformacoesAdicionais.alergias != null && this.formInformacoesAdicionais.alergias != '' && this.formInformacoesAdicionais.alergias != undefined) ? this.formInformacoesAdicionais.alergias.trim() : null,
          antecedentes: (this.formInformacoesAdicionais.antecedentes != null && this.formInformacoesAdicionais.antecedentes != '' && this.formInformacoesAdicionais.alergias != undefined) ? this.formInformacoesAdicionais.antecedentes.trim() : null
        }
        this.$store.dispatch('setFormInformacoes', obj)
        this.$modal.hide('modalInformacoesAdicionais')
        this.$swal({
          title: "Sucesso!",
          text: 'Os dados foram inseridos no atendimento!',
          icon: 'success',
        })
        this.$emit('emit', 'informacoes-adicionais-close')
        
      },

      // HIDE MODAL
      hideModalInformacoesAdicionais() {
        this.$modal.hide('modalInformacoesAdicionais')
        this.$emit('emit', 'informacoes-adicionais-close')
      },
    }
  }
</script>

<style scoped>
  .scroll-area-comorbidades {
    position: relative;
    width: 100%;
    height: 100%;
    padding-bottom: 10vh;
    overflow-x: hidden;
    overflow-y: auto;
  }

  .comorbidades > div {
    flex-direction: column;
    justify-content: space-around !important;
    display: flex !important;
    align-items: center !important;
  }

    .comorbidades > div > * {
      display: flex;
      flex-wrap: wrap;
      word-break: break-all;
      width: 75%;
      justify-content: space-between;
      align-items: center;
      border: 1px solid #dcdfe6;
      margin: 8px 0;
      padding: 0 10px;
      box-shadow: 3px 3px 3px #dcdfe6;
      border-radius: 8px;
      font-weight: bold;
    }

  .comorbidades-autorreferidas {
    margin-bottom: 5px;
  }

  .coluna-comorbidades {
    padding: 5px;
  }
</style>
