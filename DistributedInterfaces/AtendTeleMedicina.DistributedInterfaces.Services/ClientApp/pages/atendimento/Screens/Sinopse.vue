<template>
  <div>
    <div class="container-sinopse">
      <component v-if="showHeader" :is="'header-sinopse'" :agendamento="agendamento" />
    </div>
    <el-row>
      <!--LEFT CONTENT-->
      <el-col :span="16">
        <div class="grid-content">

          <!--QUEIXA DO PACIENTE TRIAGEM-->
          <div style="margin-top: 10px" v-if="agendamento.interconsulta != undefined && agendamento.interconsulta == false">
            <el-card>
              <span style="margin-left:5px; color:black; font-weight: bold">Triagem</span>
              <br />
              <span style="margin-left:5px;">{{atendimentoTriagem.queixaDoPaciente}}</span>
            </el-card>
          </div>

          <!--Últimos Atendimentos Registrados-->
          <el-card v-show="api.historico.length === 0"  style="margin-top:10px;">
            <el-empty description="Nenhum histórico encontrado"></el-empty>
          </el-card>

          <el-card v-show="api.historico.length > 0" style="margin-top:10px; padding-bottom: 10px">

            <div>
              <span style="font-weight: bold; font-size: 15px;">Últimos Atendimentos</span>
            </div>

            <el-table ref="tabela" :data="api.historico"
                      highlight-current-row border
                      v-loading.body="loading.historico"
                      :row-class-name="tableRow"
                      class="table--atendimento table--row-click"
                      :default-sort="{prop: 'dataCadastro', order: 'descending'}">
              <el-table-column label="Data da Consulta" prop="dataCadastro" align="center" width="200" sortable>
                <template slot-scope="scope">
                  {{moment(scope.row.dataCadastro).format('DD/MM/YYYY HH:mm')}}
                </template>
              </el-table-column>
              <el-table-column label="Individuo" prop="individuo.nomeCompleto" align="center" />
              <el-table-column label="Profissional" prop="profissional.nome" align="center" />
              <el-table-column label="CRM" prop="profissional.crm" align="center" width="100" />
            </el-table>
            <el-col :span="24">
              <el-pagination @size-change="handleSizeChangeHistorico"
                             @current-change="handleCurrentChangeHistorico"
                             :current-page.sync="paramsHistorico.page"
                             :page-sizes="[5,10,25]"
                             :page-size="10"
                             :total="paramsHistorico.total"
                             layout="total, sizes, prev, pager, next, jumper">
              </el-pagination>
            </el-col>
          </el-card>

        </div>
      </el-col>
      <!--RIGHT CONTENT-->
      <el-col :span="8">
        <div class="grid-content bg-purple-light">

          <button class="botaoMenu" style="margin-top: 12px; margin-bottom: 0px; margin-left: 10px; width: 95%; display: flex; justify-content: flex-start;" size="large" @click="openModalMedicoes">Medições</button>
          <button class="botaoMenu" style="margin-top: 21px; margin-bottom: 0px; margin-left: 10px; width: 95%; display: flex; justify-content: flex-start; " size="large" @click="openModalComorbidades">Condições Autorreferidas / Condições Detectadas</button>
          <button class="botaoMenu" style="margin-top: 21px; margin-bottom: 0px; margin-left: 10px; width: 95%; display: flex; justify-content: flex-start; " size="large" @click="openModalInformacoesAdicionais">Informações Adicionais</button>
          <button class="botaoMenu" style="margin-top: 21px; margin-bottom: 0px; margin-left: 10px; width: 95%; display: flex; justify-content: flex-start; " size="large" @click="openModalResultadoExames">Resultados de Exames</button>

        </div>
      </el-col>
    </el-row>

    <component v-if="controlModalMedicoes" :is="'modal-medicoes'" :agendamento="agendamento" :openMedicoes="controlModalMedicoes" @emit="returnEmitModalMedicoes"/>
    <component v-if="controlModalComorbidades" :is="'modal-comorbidades'" :agendamento="agendamento" :openComorbidades="controlModalComorbidades" @emit="returnEmitModalComorbidades"/>
    <component v-if="controlModalInformacoesAdicionais" :is="'modal-informacoes-adicionais'" :agendamento="agendamento" :openInformacoesAdicionais="controlModalInformacoesAdicionais" @emit="returnEmitModalInformacoesAdicionais"/>
    <component v-if="controlModalResultadoExames" :is="'modal-resultado-exames'" :agendamento="agendamento" :openResultadoExames="controlModalResultadoExames" @emit="returnEmitModalResultadoExames"/>
  </div>
</template>

<script>
  import _api from '../../../api'
  import HeaderSinopse from '../../../components/shared/DadosHeaderAtendimento'
  import ModalMedicoes from '../Modals/ModalMedicoes'
  import ModalComorbidades from '../Modals/ModalComorbidades'
  import ModalInformacoesAdicionais from '../Modals/ModalInformacoesAdicionais'
  import ModalResultadoExames from '../Modals/ModalResultadoExames'
  import moment from 'moment'
  moment.locale('pt-br')

  export default {
    name: 'Sinopse',
    props: {
      agendamento: {},
    },
    components: { HeaderSinopse, ModalMedicoes, ModalComorbidades, ModalInformacoesAdicionais, ModalResultadoExames},
    data() {
      return {

        //MODAL CONTROLS
        showHeader: true,
        controlModalMedicoes: false,
        controlModalComorbidades: false,
        controlModalInformacoesAdicionais: false,
        controlModalResultadoExames: false,

        //DATA
        atendimentoTriagem: {},
        api: {
          historico: [],
        },
        loading: {
          historico: false,
        },

        //PARAMS
        paramsHistorico: {
          skip: 1,
          take: 10,
          atendidoMedico: true,
          sort: 'DataCadastro DESC',
          total: 0
        },

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
      // Retorno do atendimento da triagem
      await this.getQueixaDoPaciente()
      // Retorno dos ultimos atendimentos
      await this.getHistorico()
    },

    methods: {
      //RETORNANDO QUEIXA DO PACIENTE
      async getQueixaDoPaciente() {
        let { data, status } = await _api.atendimentos.getbyAgendamentoId(this.agendamento.id)
        if (status === 200) {
          if (data.queixaDoPaciente === undefined || data.queixaDoPaciente === '' || data.queixaDoPaciente === null) {
            this.atendimentoTriagem.queixaDoPaciente = 'Não Informado Pela Triagem!!'
          }
          else {
            this.atendimentoTriagem = data
          }
        } else {
          this.atendimentoTriagem.queixaDoPaciente = 'Não Informado Pela Triagem!!'
        }
      },

      // RETORNO DOS HISTORICOS
      async getHistorico() {
        this.loading.historico = true

        var params = {
          ...this.paramsHistorico, individuoId: this.agendamento.individuoId
        }

        let { data, paginacao, status } = await _api.atendimentos.get(params)
        if (status === 502) this.loading.historico = false
        this.api.historico = (status === 200) ? data : []

        this.paramsHistorico.skip = (status === 200) ? paginacao.skip : 0
        this.paramsHistorico.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.historico = false
      },

      // HISTÓRICO E GRÁFICOS MEDIÇÕES
      async openModalMedicoes() {
        this.controlModalMedicoes = true
      },

      //EVENTO DE RETORNO DO EMIT MODAL MEDICOES
      returnEmitModalMedicoes(val) {
        if (val == 'medicoes-close') this.controlModalMedicoes = false
      },

      // COMORBIDADES
      async openModalComorbidades() {
        this.controlModalComorbidades = true
      },

      //EVENTO DE RETORNO DO EMIT MODAL COMORBIDADES
      returnEmitModalComorbidades(val) {
        if (val === 'comorbidades-close') this.controlModalComorbidades = false
      },

      // Informacoes Adicionais
      async openModalInformacoesAdicionais() {
        this.controlModalInformacoesAdicionais = true
      },

      //EVENTO DE RETORNO DO EMIT MODAL COMORBIDADES
      returnEmitModalInformacoesAdicionais(val) {
        if (val === 'informacoes-adicionais-close') this.controlModalInformacoesAdicionais = false
      },

      // RESULTADO DOS EXAMES
      async openModalResultadoExames() {
        this.controlModalResultadoExames = true
      },

      //EVENTO DE RETORNO DO EMIT MODAL COMORBIDADES
      returnEmitModalResultadoExames(val) {
        if (val === 'resultado-exames-close') this.controlModalResultadoExames = false
      },

      // PAGINAÇÃO HISTÓRICO SINOPSE
      handleSizeChangeHistorico(val) {
        this.paramsHistorico.take = val
        this.getHistorico()
      },
      handleCurrentChangeHistorico(val) {
        this.paramsHistorico.skip = val
        this.getHistorico()
      },

      tableRow({ row }) {
        if (row.ativo === true) return 'success-row'
        else if (row.ativo === false) return 'warning-row'
        else return ''
      },
    }
  }
</script>

<style scoped>
  .container {
    display: flex;
    flex-direction: column;
  }
  .container-sinopse {
    display: flex;
    flex-direction: row;
    justify-content: flex-start;
    align-items: flex-start;
  }
  .botaoMenu {
    display: inline-block;
    line-height: 1;
    white-space: nowrap;
    cursor: pointer;
    background: #b5e6e5;
    text-align: center;
    box-sizing: border-box;
    outline: 0;
    margin: 0;
    transition: .1s;
    font-weight: 500;
    -webkit-user-select: none;
    font-size: 14px;
    border-radius: 4px;
    padding: 12px;
    margin-top: 10px;
    margin-bottom: 10px;
    margin-left: 0px;
    border: 1px solid #093434;
  }

    .botaoMenu:focus {
      color: black;
      background-color: #188B8A;
    }

    .botaoMenu:hover {
      color: black;
      background-color: #B5E6E5;
    }
</style>
