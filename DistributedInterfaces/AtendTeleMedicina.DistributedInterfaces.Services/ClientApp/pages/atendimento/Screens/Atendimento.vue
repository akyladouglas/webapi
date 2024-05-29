<template>
  <el-row :gutter="24">
    <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
      <el-card shadow="always">
        <!--HEADER-->
        <el-row :gutter="24">
          <el-col :xs="19" :sm="19" :md="19" :lg="19" :xl="19">
            <component v-if="showHeader" :is="'header-atendimento'" :agendamento="agendamento" />
          </el-col>



          <el-col :xs="5" :sm="5" :md="5" :lg="5" :xl="5" style="padding-right: 20px">
            <el-button @click="emitModal('teleatendimento')"
                       icon="el-icon-video-camera"
                       type="primary"
                       size="small"
                       class="button-header"
                       v-if="inTeleatendimento == false">
              TeleAtendimento
            </el-button>

            <el-button class="button-header"
                       @click="emitModal('memed')"
                       type="primary"
                       size="small"
                       icon="el-icon-document-add"
                       v-if="agendamento.interconsulta == undefined || agendamento.interconsulta == false">
              Prescrição
            </el-button>

            <!--<el-button class="button-header"
                       @click="openModalMonitoramento"
                       type="primary"
                       icon="el-icon-document-add"
                       size="small">
              Monitoramento
            </el-button>-->

          </el-col>
        </el-row>

        <el-row :gutter="24" style="padding: 10px">

          <el-collapse style="margin-top: 20px" @change="onChangeMenusCollapse">
            <!--GRÁFICOS-->
            <el-collapse-item title="" name="graficos" style="border-radius: 5px">
              <template slot="title">
                <span><strong>Gráficos das medições</strong></span>
              </template>
              <component v-if="showGraficos" :is="'graficos-das-medicoes'" :agendamento="agendamento" :origem="'Atendimento'" />
            </el-collapse-item>

            <!--SINAIS VITAIS-->
            <el-collapse-item v-if="existVitalSigns(agendamento)" name="sinaisVitais">
              <template slot="title">
                <span><strong>Sinais Vitais</strong></span>
              </template>
              <div style="padding: 10px">
                <div style="margin-bottom: 10px">
                  <el-tag type="info">Data de envio: {{ moment(agendamento.dataAlteracao).format('DD/MM/YYYY HH:mm') }}</el-tag>
                </div>
                <el-descriptions :column="4" border>
                  <el-descriptions-item v-for="sign in vitalSigns" :key="sign.key" :label="sign.label">
                    <el-tag size="small">{{ agendamento[sign.key] + ' ' + sign.unit }}</el-tag>
                  </el-descriptions-item>
                  <el-descriptions-item label="IMC">
                    <el-tag size="small">{{ parseFloat(agendamento.peso / ((agendamento.altura / 100) * (agendamento.altura / 100))).toFixed(1) }}</el-tag>
                  </el-descriptions-item>
                  <el-descriptions-item label="GRAU IMC">
                    <el-tag size="small">{{ getGrauImc(parseFloat(agendamento.peso / ((agendamento.altura / 100) * (agendamento.altura / 100))).toFixed(1)) }}</el-tag>
                  </el-descriptions-item>
                </el-descriptions>
              </div>
            </el-collapse-item>

            <!--SINTOMAS-->
            <div class="container-sintomas">
              <p>Sintomas</p>
              <p class="data-envio" v-if="existSintomas == true">
                <el-tag type="info">Data de envio: {{ moment(api.sintomas.data).format('DD/MM/YYYY HH:mm') }}</el-tag>
              </p>
              <div v-if="existSintomas == true" class="list-sintomas">
                <div v-for="sintoma in sintomasList" :key="sintoma.prop">
                  <div v-if="api.sintomas[sintoma.prop]">
                    <span v-if="sintoma.prop !== 'outros' ">{{ sintoma.label }}: <el-tag size="small" type="success">Sim</el-tag></span>
                    <span v-else>Outros: {{api.sintomas[sintoma.prop]}}</span>
                  </div>
                </div>
              </div>
              <div v-else style="width: 40%">
                <el-tooltip effect="dark" content="Paciente não tem sintomas cadastrados sem vinculo para esse atendimento" placement="top-start">
                  <p>Paciente sem sintomas</p>
                </el-tooltip>
              </div>
            </div>
          </el-collapse>
        </el-row>
      </el-card>

      <!--COMPONENT FICHA ATENDIMENTO-->
      <component v-if="showFichaAtendimentoUbs && mxHasAccess('Médico') && (agendamento.interconsulta == undefined || agendamento.interconsulta == false)" :is="'ficha-atendimento-ubs'" :agendamento="agendamento" :sintomas="api.sintomas" :existSintomas="existSintomas" :countTimer="countTimerTeleatendimento" :inicioDoAtendimento="inicioDoAtendimento" :fimDoAtendimento="fimDoAtendimento" @emit="" />
      <component v-if="showFichaAtendimentoDomiliciar && mxHasAccess('MédicoAD') && (agendamento.interconsulta == undefined || agendamento.interconsulta == false)" :is="'ficha-atendimento-domiciliar'" :agendamentoProps="agendamento" :sintomas="api.sintomas" :existSintomas="existSintomas" :countTimer="countTimerTeleatendimento" :inicioDoAtendimento="inicioDoAtendimento" :fimDoAtendimento="fimDoAtendimento" @emit="eventRetornoFichaAtendimento" />

      <component v-if="agendamento.interconsulta != undefined && agendamento.interconsulta == true" :is="'ficha-interconsulta'" :agendamento="agendamento" @emit=""/>

    </el-col>

    <!-- MODAL MONITORAMENTO-->
    <!--<modal name="modalMonitoramento" :resizable="false" :draggable="true" :clickToClose="false" width="1000" height="600">
      <div class="window-header" style="padding: 5px;display: flex;justify-content: flex-end;">
        <el-button style="width:40px; display:flex; align-content:space-around; justify-content:center" type="danger" @click="hideModalMonitoramento" icon="el-icon-close"></el-button>
      </div>
      <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
        <div>
          <h2 style="padding-bottom: 5px" class="box-card--h2"> Monitoramento</h2>
        </div>
        <el-scrollbar wrap-style="max-height: 500px;">
          <component v-if="showModalMonitoramento" :is="'graficos-monitoramento-individual'" :origem="'atendimento'" :agendamento="agendamento" />
        </el-scrollbar>
      </el-col>
    </modal>-->

  </el-row>
</template>

<script>
  import moment from 'moment'
  moment.locale('pt-br')
  import _api from '../../../api'
  import Utils from '../../../mixins/Utils'
  //COMPONENTS
  import HeaderAtendimento from '../../../components/shared/DadosHeaderAtendimento'
  import GraficosDasMedicoes from '../../../components/shared/GraficosDasMedicoes'
  import FichaAtendimentoUbs from '../FichasAtendimento/FichaAtendimentoUbs'
  import FichaInterconsulta from '../FichasAtendimento/FichaInterconsulta'
  import FichaAtendimentoDomiciliar from '../FichasAtendimento/FichaAtendimentoDomiciliar'
  //import GraficosMonitoramentoIndividual from '../../monitoramento/GraficosMonitoramentoIndividual'

  export default {
    name: 'Atendimento',
    props: {
      agendamento: {},
      inTeleatendimento: '',
      countTimerTeleatendimento: '',
      inicioDoAtendimento: '',
      fimDoAtendimento: '',
    },
    mixins: [Utils],
    components: { HeaderAtendimento, GraficosDasMedicoes, FichaAtendimentoUbs, FichaInterconsulta, FichaAtendimentoDomiciliar },
    data() {
      return {

        //SCREENS CONTROL
        showHeader: true,
        showGraficos: false,
        showFichaAtendimentoUbs: false,
        showFichaAtendimentoDomiliciar: false,
        showModalMonitoramento: false,

        existSintomas: false,
        //DATA
        api: {
          sintomas: {},
        },

        //CONFIGS
        vitalSigns: [
          { label: 'Pre. Arterial', unit: 'mmhg', key: 'pressaoSanguinea' },
          { label: 'Fre. Cardiaca', unit: 'bpm', key: 'batimentoCardiaco' },
          { label: 'Saturação O²', unit: '%', key: 'oxigenacaoSanguinea' },
          { label: 'Temperatura', unit: '°C', key: 'temperatura' },
          { label: 'Altura', unit: 'cm', key: 'altura' },
          { label: 'Peso', unit: 'kg', key: 'peso' }
        ],
        sintomasList: [
          { label: 'Temperatura', prop: 'temperatura' },
          { label: 'Tosse', prop: 'tosse' },
          { label: 'Coriza', prop: 'coriza' },
          { label: 'Dor no corpo', prop: 'dorCorpo' },
          { label: 'Dor abdominal', prop: 'dorAbdominal' },
          { label: 'Fraqueza', prop: 'fraqueza' },
          { label: 'Dor na garganta', prop: 'dorGarganta' },
          { label: 'Nausea ou vomito', prop: 'nauseaVomito' },
          { label: 'Dor de cabeça', prop: 'dorCabeca' },
          { label: 'Saiu de casa', prop: 'sairCasa' },
          { label: 'Teve contato com pessoas', prop: 'contatoPessoas' },
          { label: 'Dificuldade de respirar', prop: 'dificuldadeRespirar' },
          { label: 'Taquicardia', prop: 'taquicardia' },
          { label: 'Diarreia', prop: 'diarreia' },
          { label: 'Temperatura alta retornando/retornou', prop: 'temperaturaRetornou' },
          { label: 'Atendido pelo serviço de saúde', prop: 'atendidoServicoSaude' },
          { label: 'Perda de olfato ou paladar', prop: 'perdaOlfatoPaladar' },
          { label: 'Sintomas gripais', prop: 'sintomasGripais' },
          { label: 'Outros', prop: 'outros' },
        ],

        //PARAMS
        paramsSintomas: {
          skip: 1,
          take: 100,
          sort: 'a.Data DESC',
          total: 0
        },

      }
    },

    computed: {
      //PROP PARA VERFICAR SE EXISTE SINAIS VITAIS
      existVitalSigns() {
        return function (agendamento) {
          return agendamento.pressaoSanguinea &&
            agendamento.batimentoCardiaco &&
            agendamento.oxigenacaoSanguinea &&
            agendamento.temperatura &&
            agendamento.altura &&
            agendamento.peso;
        }
      }
    },

    async created() {
      await this.getSintomas()
      if(this.mxHasAccess('Médico')) this.showFichaAtendimentoUbs = true
      if(this.mxHasAccess('MédicoAD')) this.showFichaAtendimentoDomiciliar = true
    },


    methods: {

      //REQUESTS APIS
      async getSintomas() {
        this.paramsSintomas.individuoId = this.agendamento.individuoId
        this.paramsSintomas.atendimentoId = null
        let { data } = await _api.acompanhamento.get(this.paramsSintomas)
        if (data.length > 0) {
          this.api.sintomas = data[0]
          this.existSintomas = true
        } else {
          this.api.sintomas = {}
          this.existSintomas = false
        }
      },

      //EMIT DOS MODALS
      emitModal(stringNameModal) {
        this.$emit('emit', `${stringNameModal}-open`)
      },

      // MODAL MONITORAMENTO
      openModalMonitoramento() {
        this.showModalMonitoramento = true
        this.$modal.show('modalMonitoramento')
      },
      hideModalMonitoramento() {
        this.showModalMonitoramento = false
        this.$modal.hide('modalMonitoramento')
      },

      //AUXILIAR CONFIGS
      onChangeMenusCollapse(val) {
        if (val.length > 0) {
          if (val[0] == 'graficos') {
            this.showGraficos = true
          }
        }
      },
      getGrauImc(row) {
        if (row <= 16.9) return 'Muito abaixo do Peso'
        else if (row >= 17 && row <= 18.4) return 'Abaixo do Peso'
        else if (row >= 18.5 && row <= 24.9) return 'Peso Normal'
        else if (row >= 25 && row <= 29.9) return 'Acima do Peso'
        else if (row >= 30 && row <= 34.9) return 'Obesidade Grau I'
        else if (row >= 35 && row <= 40) return 'Obesidade Grau II'
        else if (row > 40) return 'Obesidade Grau III'
        else return ''
      },


      //PARTICULARIDADE DO CONDE
      async eventRetornoFichaAtendimento(val) {

        if (val.atendimentoFinalizado != undefined && val.atendimentoId) {
          if (val.atendimentoFinalizado == true) {
            // se houver exames avaliados no modal dos exames irá subir aqui
            if (this.api.exames.length > 0) {
              await this.enviarAvaliacaoExames()
            }

            // atualizando as alergias e antecedentes
            if (this.formAlergias.alergias != undefined || this.formAlergias.antecedentes != undefined) {
              let { data, status } = await _api.atendimentos.getById(val.atendimentoId)
              if (status === 200 && data != {}) {
                var obj = data
                obj.alergias = this.formAlergias.alergias
                obj.antecedentes = this.formAlergias.antecedentes
                let { data: dataPut, status: statusPut } = await _api.atendimentos.putAtendimento(obj)
                if (statusPut === 200) {
                  this.$swal({
                    title: "Sucesso!",
                    text: 'As alergias foram registradas com sucesso!',
                    icon: 'success',
                  })
                }
              }
            }

            // atualizar o sintomas (que vai estar no banco com o ID do atendimento da triagem para o ID do atendimento do médico)

            // retirando o observer de reatividade do vue
            delete this.api.comoMeSintoAtendimento.__ob__

            //verificando se tem algum dado apos tirar o observer do vue, e atualizando os sintomas
            if (Object.keys(this.api.comoMeSintoAtendimento).length > 0) {
              let { data: dataAcompanhamento, status: statusAcompanhamento } = await _api.acompanhamento.atualizarAcompanhamento(val.atendimentoId, this.api.comoMeSintoAtendimento)
              if (statusAcompanhamento === 200 || statusAcompanhamento === 201 || statusAcompanhamento === 204) {
                //this.$swal({
                //  title: "Sucesso!",
                //  text: 'Os sintomas foram atualizados com sucesso!',
                //  icon: 'success',
                //})
              }
              else {
                this.$swal({
                  title: "Erro!",
                  text: 'Ocorreu um erro na atualização dos sintomas!',
                  icon: 'error',
                })
              }
            }
            this.$router.push({ name: 'Agendamentos' })
          }
        }
      },
    }
  }
</script>

<style scoped>
  .button-header {
    width: 100% !important;
    margin-left: 0 !important;
    margin: 5px;
  }

  .container-sintomas {
    padding: 20px 0
  }

    .container-sintomas span {
      font-size: 14px;
    }

    .container-sintomas p.data-envio {
      font-size: 14px;
    }

    .container-sintomas .list-sintomas span {
      font-size: 14px;
    }

  /*CONTROLE DA PARTE DE ESTILO DO BACKGROUND DO MODAL*/
  /*retirar o fundo disabled do modal*/
  .v--modal-overlay[data-modal="hello-world"] {
    background: transparent;
  }
</style>
