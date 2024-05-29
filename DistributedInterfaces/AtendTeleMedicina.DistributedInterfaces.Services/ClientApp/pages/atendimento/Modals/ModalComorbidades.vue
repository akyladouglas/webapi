<template>
  <modal name="modalComorbidades" :resizable="false" :draggable="true" :clickToClose="false" width="800" height="550">
    <VuePerfectScrollbar class="scroll-area-comorbidades" :settings="scrollSettings" key="scrol-atendimento">
      <div class="window-header" style="padding: 5px;display: flex;justify-content: flex-end;">
        <el-button size="mini"
                   type="danger"
                   @click="hideModalComorbidades"
                   icon="el-icon-close"></el-button>
      </div>

      <el-row style="padding: 10px">
        <el-col :span="12" class="coluna-comorbidades">
          <h5>Comorbidades autorreferidas</h5>
          <div style="margin-bottom: 10px">
            <span v-if="api.comorbidades.dataAlteracao" size="small">
              <el-tag type="info">Data de envio: {{ moment(api.comorbidades.dataAlteracao).format('DD/MM/YYYY HH:mm') }}</el-tag>
            </span>
          </div>

          <div v-if="api.comorbidades" v-for="(value, key, index) in api.comorbidades">
            <div>
              <el-tag class="comorbidades-autorreferidas"
                      size="small" type="success"
                      v-if="
                          value === true
                          &&
                          key !== 'ativo'
                          &&
                          key !== 'valid'
                          &&
                          key !== 'respondeuComorbidade'
                          ">{{enumComorbidade[key]}}
              </el-tag>
            </div>
          </div>
        </el-col>

        <el-col :span="12" class="coluna-comorbidades" style="border-left: 1px solid #e3e3e3">
          <h5>Identificadas pelo médico</h5>

          <el-form class="comorbidades">
            <el-row>
              <div>
                <el-form-item label="Hipertenso">
                  <el-radio-group @change="onChangeComorbidades" fill="#015f6b" v-model="formComorbidades.hipertenso" :disabled="disabledComorbidades">
                    <el-radio-button :label="true">Sim</el-radio-button>
                    <el-radio-button :label="false">Não</el-radio-button>
                  </el-radio-group>
                </el-form-item>
              </div>
              <div>
                <el-form-item label="Diabetes">
                  <el-radio-group @change="onChangeComorbidades" fill="#015f6b" v-model="formComorbidades.diabetes" :disabled="disabledComorbidades">
                    <el-radio-button :label="true">Sim</el-radio-button>
                    <el-radio-button :label="false">Não</el-radio-button>
                  </el-radio-group>
                </el-form-item>
              </div>
              <div>
                <el-form-item label="Fumante">
                  <el-radio-group @change="onChangeComorbidades" fill="#015f6b" v-model="formComorbidades.fumante" :disabled="disabledComorbidades">
                    <el-radio-button :label="true">Sim</el-radio-button>
                    <el-radio-button :label="false">Não</el-radio-button>
                  </el-radio-group>
                </el-form-item>
              </div>
              <div>
                <el-form-item label="Asma">
                  <el-radio-group @change="onChangeComorbidades" fill="#015f6b" v-model="formComorbidades.asma" :disabled="disabledComorbidades">
                    <el-radio-button :label="true">Sim</el-radio-button>
                    <el-radio-button :label="false">Não</el-radio-button>
                  </el-radio-group>
                </el-form-item>
              </div>
              <div>
                <el-form-item label="Doença no Pulmão">
                  <el-radio-group @change="onChangeComorbidades" fill="#015f6b" v-model="formComorbidades.doencaPulmao" :disabled="disabledComorbidades">
                    <el-radio-button :label="true">Sim</el-radio-button>
                    <el-radio-button :label="false">Não</el-radio-button>
                  </el-radio-group>
                </el-form-item>
              </div>
              <div>
                <el-form-item label="Doença no Coração">
                  <el-radio-group @change="onChangeComorbidades" fill="#015f6b" v-model="formComorbidades.doencaCoracao" :disabled="disabledComorbidades">
                    <el-radio-button :label="true">Sim</el-radio-button>
                    <el-radio-button :label="false">Não</el-radio-button>
                  </el-radio-group>
                </el-form-item>
              </div>
              <div>
                <el-form-item label="Câncer">
                  <el-radio-group @change="onChangeComorbidades" fill="#015f6b" v-model="formComorbidades.doencaCancer" :disabled="disabledComorbidades">
                    <el-radio-button :label="true">Sim</el-radio-button>
                    <el-radio-button :label="false">Não</el-radio-button>
                  </el-radio-group>
                </el-form-item>
              </div>
              <div>
                <el-form-item label="Transplantado">
                  <el-radio-group @change="onChangeComorbidades" fill="#015f6b" v-model="formComorbidades.transplantado" :disabled="disabledComorbidades">
                    <el-radio-button :label="true">Sim</el-radio-button>
                    <el-radio-button :label="false">Não</el-radio-button>
                  </el-radio-group>
                </el-form-item>
              </div>
              <div>
                <el-form-item label="Doença Compromete Imunidade">
                  <el-radio-group @change="onChangeComorbidades" fill="#015f6b" v-model="formComorbidades.doencaComprometeImunidade" :disabled="disabledComorbidades">
                    <el-radio-button :label="true">Sim</el-radio-button>
                    <el-radio-button :label="false">Não</el-radio-button>
                  </el-radio-group>
                </el-form-item>
              </div>
              <div>
                <el-form-item label="Doença nos Rins">
                  <el-radio-group @change="onChangeComorbidades" fill="#015f6b" v-model="formComorbidades.doencaRins" :disabled="disabledComorbidades">
                    <el-radio-button :label="true">Sim</el-radio-button>
                    <el-radio-button :label="false">Não</el-radio-button>
                  </el-radio-group>
                </el-form-item>
              </div>
              <div>
                <el-form-item label="Doença no Fígado">
                  <el-radio-group @change="onChangeComorbidades" fill="#015f6b" v-model="formComorbidades.doencaFigado" :disabled="disabledComorbidades">
                    <el-radio-button :label="true">Sim</el-radio-button>
                    <el-radio-button :label="false">Não</el-radio-button>
                  </el-radio-group>
                </el-form-item>
              </div>
            </el-row>
          </el-form>
        </el-col>
      </el-row>
    </VuePerfectScrollbar>
  </modal>
</template>

<script>
  import _api from '../../../api'
  import VuePerfectScrollbar from 'vue-perfect-scrollbar'
  import moment from 'moment'
  moment.locale('pt-br')

  export default {
    name: 'ModalComorbidades',
    props: {
      agendamento: {},
      openComorbidades: ''
    },
    components: { VuePerfectScrollbar },

    data() {
      return {
        disabledComorbidades: false,
        controlComorbidades: false,
        api: {
          comorbidades: {},
        },
        formComorbidades: {},

        scrollSettings: {
          maxScrollbarLength: 200
        },
        enumComorbidade: {
          doencaPulmao: 'DOENÇA NO PULMÃO',
          doencaCoracao: 'DOENÇA NO CORAÇÃO',
          doencaCancer: 'CÂNCER',
          doencaComprometeImunidade: 'DOENÇA QUE COMPROMETE IMUNIDADE',
          doencaRins: 'DOENÇA NOS RINS',
          doencaFigado: 'DOENÇA NO FÍGADO',
          hipertenso: 'HIPERTENSO',
          diabetes: 'DIABETES',
          fumante: 'FUMANTE',
          asma: 'ASMA',
          transplantado: 'TRANSPLANTADO',
        },
      }
    },

    async mounted() {
      if (this.openComorbidades == true) {
        await this.getComorbidades()
        this.formComorbidades = this.api.comorbidades
        this.controlComorbidades = true
        this.$modal.show('modalComorbidades')
      }
    },

    methods: {
      // RETORNO DAS COMORBIDADES DO PACIENTE
      async getComorbidades() {
        let { data } = await _api.individuos.getById(this.agendamento.individuoId)
        this.api.comorbidades = (data !== null) ? data : {}
      },

      onChangeComorbidades(val) {
        this.formComorbidades = { ...this.formComorbidades, respondeuComorbidade: true }
        this.onClickComorbidades()
      },

      // ATUALIZAR COMORBIDADES
      async onClickComorbidades() {
        let { data, status } = await _api.individuos.atualizarComorbidade(this.agendamento.individuoId, this.formComorbidades)
        if (status === 200) {
          this.$swal({
            title: "Sucesso!",
            text: 'Comorbidades enviadas com sucesso!',
            icon: 'success',
          })
          await this.getComorbidades()
        } else {
          this.$swal({
            title: "Erro!",
            text: 'Erro no envio do registro das comorbidades!',
            icon: 'error',
          })
        }
      },

      // HIDE MODAL
      hideModalComorbidades() {
        this.controlComorbidades = false
        this.$modal.hide('modalComorbidades')
        this.$emit('emit', 'comorbidades-close')
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
