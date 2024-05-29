<template>
  <div :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
    <el-card class="box-card box-card--main-card">
      <el-form :model="filtroParams" ref="filtroParams" label-width="120px" label-position="top" class="forms--filtro-estabelecimento">

        <el-row :gutter="20" v-if="!mxHasAccess('Administrador')">
          <div style="display:flex; margin-left:20px; margin-top:20px">
            <span style="font-size:24px">Pacientes:</span>
            <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="7">
              <el-form-item label="" prop="tipoDaConsulta">
                <el-select v-model="filtroParams.tipoDaConsulta" placeholder="Selecione um tipo de consulta" @change='onFiltrar'>
                  <el-option v-for="option in enums.tipoDaConsultaTelaHome" :label="option.label" :value="option.value" :key="option.value" />
                </el-select>
              </el-form-item>
            </el-col>
          </div>
        </el-row>

      </el-form>

      <el-card v-if="!mxHasAccess('Administrador')" v-loading="loading.agendamentos">
        <div style="width: 100%; border:2px solid lightgrey; border-radius:10px">
          <el-row>
            <el-col class="agendados" :span="8">

              <router-link :to="{ name: 'Agenda', params: { tipoDaConsulta: filtroParams.tipoDaConsulta, dashboard: true } }" title="Clique para ver a fila de espera da triagem">
                <h1 style="font: 3.5vw arial, verdada; margin-bottom: -10px; text-shadow: 1px 1px 2px black;">{{ pacientesAgendados.length }}</h1>
                <h1 style="font: 26px arial, verdada; ">Fila de espera triagem</h1>
                <i style="margin-bottom:15px; font-size:25px" class="far fa-calendar-alt"></i>
              </router-link>

            </el-col>

            <el-col class="confirmados" :span="8">

              <router-link :to="{ name: 'Agendamentos', params: { tipoDaConsulta: filtroParams.tipoDaConsulta, dashboard: true } } " title="Clique para ver os pacientes aguardando atendimento">
                <h1 style="font: 3.5vw arial, verdada; margin-bottom: -10px; text-shadow: 1px 1px 2px black;">{{ pacientesConfirmados.length }}</h1>
                <h1 style="font: 26px arial, verdada; ">Aguardando atendimento</h1>
                <i style="margin-bottom: 15px; font-size: 25px" class="far fa-check-circle"></i>
              </router-link>
            </el-col>

            <el-col class="atendidos" :span="8">

              <router-link :to="{ name: 'MenuHistorico', params: { tipoDaConsulta: filtroParams.tipoDaConsulta,  dashboard: true }}" title="Clique para ver os pacientes atendidos">
                <h1 style="font: 3.5vw arial, verdada; margin-bottom: -10px; text-shadow: 1px 1px 2px black; ">{{ pacientesAtendidos.length }}</h1>
                <h1 style="font: 26px arial, verdada; ">Pacientes atendidos</h1>
                <i style="margin-bottom: 15px; font-size: 25px" class="far fa-check-circle"></i>
              </router-link>

            </el-col>

            <!--<el-col class="ausentes" :span="6">
              <router-link :to="{ name: 'Ausentes', params: { tipoDaConsulta: filtroParams.tipoDaConsulta,  dashboard: true } }">
                <h1 style="font: 70px arial, verdada; margin-bottom: -10px; text-shadow: 1px 1px 2px black;">{{ pacientesAusentes.length }}</h1>
                <h1 style="font: 26px arial, verdada; ">Ausentes</h1>

                <i style="margin-bottom: 15px; font-size: 25px" class="far fa-times-circle"></i>
              </router-link>
            </el-col>-->
          </el-row>
        </div>
      </el-card>

      <div style="margin-top:10px">
          <!-- DASHBOARD MEDICO INICIO -->
          <el-col :span="12" v-if="!mxHasAccess('Administrador')">
            <el-card>

              <div v-if="filtroParams.tipoDaConsulta == null || filtroParams.tipoDaConsulta == undefined || filtroParams.tipoDaConsulta == ''" style="display:flex; margin-bottom:20px; justify-content:center"><span style="font-size:18px; font-weight:bold; color:gray">Todos</span></div>
              <div v-else style="display:flex; margin-bottom:20px; justify-content:center"><span style="font-size:18px; font-weight:bold; color:gray">{{filtroParams.tipoDaConsulta}}</span></div>

              <div v-if="api.arrayGraficoGeral.length === 0" style="height:100%; width:100%">
                <el-empty description="Nenhum dado encontrado"></el-empty>
              </div>

              <div v-if="api.arrayGraficoGeral.length > 0">

                <highcharts :options="graficoGeral"></highcharts>

              </div>
            </el-card>
          </el-col>

          <el-col v-if="!mxHasAccess('Administrador')" :span="12" style=" display: flex; flex-direction: column; align-items: center; ">
            <el-card v-if="mxHasAccess('Recepcionista')" style="width:98%">
              <div>
                <div style="margin-bottom:20px; display:flex; justify-content:center"><span style="font-size:18px; font-weight:bold; color:gray">Pacientes por sexo</span></div>
              </div>
              <div style="width:98%">
                <highcharts :options="graficoQuantidadePorSexo"></highcharts>
              </div>
            </el-card>


            <el-card style=" width:99%" v-if="mxHasAccess('Médico') || mxHasAccess('MédicoAD')">

              <div>
                <div style="margin-bottom:20px; display:flex; justify-content:center"><span style="font-size:18px; font-weight:bold; color:gray">Média de Duração de Tempo no Atendimento (mês atual)</span></div>
                <div style="display: flex; justify-content:center">
                  <span style="font-size: 24px"> <i class="far fa-clock"></i> {{duracaoAtendimento === "Sem Dados" ? "Sem Dados" : duracaoAtendimento + "min"}}</span>

                </div>
              </div>

              <div style="margin-top:20px">
                <div style="display: flex; justify-content:center">
                  <div style="margin-bottom:20px"><span style="font-size:18px; font-weight:bold; color:gray">Tipo de atendimento (mês atual)</span></div>
                </div>
                <div>
                  <highcharts :options="graficoTipoDeAtendimento"></highcharts>
                </div>
              </div>

            </el-card>

          </el-col>
       </div>

      <!-- DASHBOARD MEDICO FIM -->

          <!-- DASHBOARD ADMIN INICIO -->
          <!-- TOTAL TIPO DE ATENDIMENTO ADMIN-->

          <div>

            <el-row v-if="mxHasAccess('Administrador')" style="margin-bottom: 15px;">
              <el-col :span="24" >
                <FiltroDashboardAdmin :loading="loading.profissionais" :params="filtroParams" @emit="onEmitFromFiltro" />
              </el-col>
            </el-row>

            <el-card>
              <div style="display:flex; margin-bottom:20px; justify-content:center"><span style="font-size:20px; font-weight:bold; color:gray">Total por Tipo de Atendimento</span></div>

              <div v-if="api.totalTipoAtendimento.length === 0" style="height:100%; width:100%">
                <el-empty description="Nenhum dado encontrado"></el-empty>
              </div>

              <div style="height:90%; width:100%">

                <highcharts :options="graficoTotalTipoAtendimento"></highcharts>

              </div>
            </el-card>
            <div v-if="mxHasAccess('Administrador')" class="container__rank_todos_pacientes_sexo">
              <!-- RANK ADMIN -->
              <el-col :xs="24" :sm="24" :md="8" :lg="8" :xl="8">
                <el-card style="margin-right:10px">
                  <div style="display:flex; margin-bottom:20px; justify-content:center"><span style="font-size: 20px; font-weight: bold; color: gray">Rank</span></div>

                  <div v-show="api.profissionais.length === 0" style="height:100%; width:100%">
                    <el-empty description="Nenhum profissional encontrado"></el-empty>
                  </div>

                  <div>
                    <el-table ref="tabela" :data="api.profissionais"
                              highlight-current-row border
                              v-loading.body="loading.profissionais"
                              :default-sort="{prop: 'nome', order: 'descending'}"
                              class="table--atendimento table--row-click">

                      <el-table-column label="Profissional" prop="nome" />
                      <el-table-column label="Atendidos" prop="profissionalId" sortable>
                        <template slot-scope="scope">
                          {{ getAtendidosTable(scope.row.id) }}
                        </template>
                      </el-table-column>
                      <el-table-column label="Ausentes" prop="profissionalId" sortable>
                        <template slot-scope="scope">
                          {{ getAusentesTable(scope.row.id) }}
                        </template>
                      </el-table-column>
                      <el-table-column label="Tempo Médio (min)" prop="profissionalId" sortable>
                        <template slot-scope="scope">
                          {{ getTempoAtendimentoPorMedico(scope.row.id) }}
                        </template>
                      </el-table-column>
                    </el-table>
                  </div>
                </el-card>
              </el-col>

               <!--TODOS ADMIN--> 
              <el-col :xs="24" :sm="24" :md="8" :lg="8" :xl="8">
                <el-card>
                  <div style="display:flex; margin-bottom:20px; justify-content:center"><span style="font-size: 20px; font-weight: bold; color: gray">Todos</span></div>

                  <div v-if="api.arrayGraficoGeral.length === 0" style="height:100%; width:100%">
                    <el-empty description="Nenhum dado encontrado"></el-empty>
                  </div>

                  <div v-if="api.arrayGraficoGeral.length > 0" style="height:100%; width:100%">

                    <highcharts :options="graficoGeral"></highcharts>

                  </div>
                </el-card>
              </el-col>

               <!--TOTAL POR SEXO ADMIN-->
              <el-col :xs="24" :sm="24" :md="8" :lg="8" :xl="8">
                <el-card>
                  <div>
                    <div style="margin-bottom:20px; display:flex; justify-content:center">
                      <span style="font-size: 20px; font-weight: bold; color: gray">Pacientes por sexo</span>
                    </div>
                  </div>
                  <div style="width:98%">
                    <highcharts :options="graficoQuantidadePorSexo"></highcharts>
                  </div>
                </el-card>
              </el-col>

            </div>
          </div>
          <!-- DASHBOARD ADMIN FIM -->
        
    </el-card>
    <!--<h3 v-if="!mxHasAccess('Médico')"> Modo: {{ isDemandaEspontanea() ? 'Demanda Espontânea' : 'Agendamentos' }} </h3>-->
  </div>
</template>

<script>
  import jQuery from 'jquery'
  import FiltroDashboardAdmin from '../../ClientApp/components/shared/FiltroDashboardAdmin'
  import { getGraficoTotalTipoAtendimento } from '../../ClientApp/components/GraficosTemplate/GraficoTotalTipoAtendimento.js'
  import { getGraficoGeral } from '../../ClientApp/components/GraficosTemplate/GraficoGeral.js'
  import { getGraficoPorSexo } from '../../ClientApp/components/GraficosTemplate/GraficoQuantidadePorSexo.js'
  import { getGraficoTipoAtendimento } from '../../ClientApp/components/GraficosTemplate/GraficoTipoAtendimento.js'
  import Utils from '../mixins/Utils'
  import ScreenWidth from '../mixins/ScreenWidth'
  import { mapGetters } from 'vuex'
  import _api from '../api'
  import _enums from '../api/Enums'
  import moment from 'moment'
  moment.locale('pt-br')
  export default {
    name: 'Home',
    mixins: [ScreenWidth],
    components: { FiltroDashboardAdmin },
    data () {
      return {

        duracaoAtendimento: '',
        filtroParams: {},
        pacientesAgendados: [],
        pacientesConfirmados: [],
        pacientesAtendidos: [],
        pacientesAusentes: [],

        api: {
          arrayGraficoGeral: [],
          agendamentos: [],
          profissionais: [],
          atendimentos: [],
          agendamentos: [],
          totalTipoAtendimento: []
        },
        enums: {
          tipoDaConsultaTelaHome: [
            ..._enums.tipoDaConsultaTelaHome
          ]
        },
        loading: {
          profissionais: false,
          agendamentos: false
        },

        graficoGeral: getGraficoGeral(),

        graficoQuantidadePorSexo: getGraficoPorSexo(),

        graficoTotalTipoAtendimento: getGraficoTotalTipoAtendimento('', 'Total por Tipo de Atendimento'),

        graficoTipoDeAtendimento: getGraficoTipoAtendimento('', 'Quantidade de atendimento por tipo'),

        params: {
          skip: 1,
          take: 100000
        },

        paramsAgendamento: {
          skip: 1,
          take: 100000,
          finalizado: false,
          sort: 'a.Dia DESC, a.Hora DESC',
          total: 0
        },

        paramsPaciente: {
          skip: 1,
          take: 10000,
          sort: 'i.NomeCompleto ASC',
          total: 0
        },

        paramsGetProfissionais: {
          skip: 1,
          take: 100000
        },
        paramsGetTotalTipoAtendimento: {
          skip: 1,
          take: 100000
        },
        paramsGetAgendamentos: {
          skip: 1,
          take: 10000
        },

        paramsAtendimento: {
          skip: 1,
          take: 10
        }
      }
    },

    mixins: [Utils],
    computed: {
      ...mapGetters([
        'user'
      ])
    },
    watch: {
      user: {
        immediate: true,
        deep: false,
        handler (newValue) {
          // Faça aqui o que for preciso ou chame uma função
          if (newValue.senhaAlterada) this.isSenhaAlterada()
        }
      }
    },
    created () {
      if (this.$store.state.user.selectRole === '' || this.$store.state.user.selectRole === undefined || this.$store.state.user.selectRole == null) {
        this.$router.push('SelecionarPerfil')
      }

      var graficoAltura = window.innerHeight
      this.graficoGeral.chart.height = graficoAltura * (40 / 100)
      this.graficoTipoDeAtendimento.chart.height = graficoAltura * (26 / 100)
      this.graficoQuantidadePorSexo.chart.height = graficoAltura * (40 / 100)

      const primeiraDataDoAno = new Date(new Date().getFullYear(), 0, 1)
      const primeiraDataFormatada = primeiraDataDoAno.toLocaleDateString()
      const dataSplit = primeiraDataFormatada.split('/')
      const data = `${dataSplit[2]}-${dataSplit[1]}-${dataSplit[0]}`
      this.paramsGetTotalTipoAtendimento.dataInicial = data

      const ultimaDataDoAno = new Date(new Date().getFullYear(), 11, 0)
      const ultimaDataFormatada = ultimaDataDoAno.toLocaleDateString()
      const ultimaDataSplit = ultimaDataFormatada.split('/')
      const ultimaData = `${ultimaDataSplit[2]}-${ultimaDataSplit[1]}-${ultimaDataSplit[0]}`
      this.paramsGetTotalTipoAtendimento.dataFinal = ultimaData
    },

    async mounted () {
      // Aguarda a mudança do estado user.selectRole no Vuex
      await this.$store.watch(
        state => state.user.selectRole,
        (newVal, oldVal) => {
          if (newVal && newVal.trim() !== '') {
            console.log('chamou', this.$store.state.user.selectRole)
            /// / Quando o valor selectRole não é vazio ou indefinido, navegue para a rota "Home"
            this.$router.push({
              name: 'Home'
            })
          }
        }
      )

      // console.log("this.$store.state.user.dados", this.$store.state.user.dados)
      // console.log("this.$auth.user()", this.$auth.user())

      await this.getProfissionais()
      await this.getAgendamentos()
      await this.getAtendimentos()

      await this.getTotalTipoAtendimento()
      await this.getTipoDaConsultaAgendamentosGrafico()

      await this.getIndividuos()
    },

    methods: {

      dadosMensaisGraficoAdmin (dados, mes) {
        if (dados.length > 0) {
          var tipo = []
          dados.forEach(obj => {
            tipo.push(obj.tipoDaConsulta)
          })

          // adicionando um item no graficoTotalTipoAtendimento.series[x].data
          // exemplo Mes janeiro 0; Mes fevereiro 0; para caso nao tenha nada do tipo totem ou presencial ou teleconsulta incluida para nao quebrar os espaços em branco do grafico
          if (!tipo.includes('Totem')) {
            // console.log("não tem Totem no ", mes)
            this.graficoTotalTipoAtendimento.series[0].data.push(0)
          }
          if (!tipo.includes('Presencial')) {
            // console.log("não tem Presencial no ", mes)
            this.graficoTotalTipoAtendimento.series[1].data.push(0)
          }
          if (!tipo.includes('Teleconsulta')) {
            // console.log("não tem Teleconsulta no ", mes)
            this.graficoTotalTipoAtendimento.series[2].data.push(0)
          }

          // dentro dos arrays tem os objetos se tiver totem teleconsulta ou presencial
          dados.forEach(obj => {
            // retornando os tipo TOTEM para SERIES[0]
            if (obj.tipoDaConsulta == 'Totem') {
              if (obj.total > 0) {
                this.graficoTotalTipoAtendimento.series[0].data.push(obj.total)
              } else {
                this.graficoTotalTipoAtendimento.series[0].data.push(0)
              }
            }

            // retornando os tipo PRESENCIAL para SERIES[1]
            if (obj.tipoDaConsulta == 'Presencial') {
              if (obj.total > 0) {
                this.graficoTotalTipoAtendimento.series[1].data.push(obj.total)
              } else {
                this.graficoTotalTipoAtendimento.series[1].data.push(0)
              }
            }

            // retornando os tipo TELECONSULTA para SERIES[2]
            if (obj.tipoDaConsulta == 'Teleconsulta') {
              if (obj.total > 0) {
                this.graficoTotalTipoAtendimento.series[2].data.push(obj.total)
              } else {
                this.graficoTotalTipoAtendimento.series[2].data.push(0)
              }
            }
          })
          this.graficoTotalTipoAtendimento.xAxis.categories.push(mes)
        } else {
          // se o dados nao for > 0
          this.graficoTotalTipoAtendimento.xAxis.categories.push(mes)
          this.graficoTotalTipoAtendimento.series[0].data.push(0)
          this.graficoTotalTipoAtendimento.series[1].data.push(0)
          this.graficoTotalTipoAtendimento.series[2].data.push(0)
        }
      },

      // DASHBOARD TOTAL TIPO ATENDIMENTO ADMIN
      async getTotalTipoAtendimento (val) {
        this.graficoTotalTipoAtendimento.series[0].data = []
        this.graficoTotalTipoAtendimento.series[1].data = []
        this.graficoTotalTipoAtendimento.series[2].data = []
        this.graficoTotalTipoAtendimento.xAxis.categories = []
        let { data, status } = await _api.atendimentos.getContadorTotalTipoAtendimento(this.paramsGetTotalTipoAtendimento)
        var totalTipoAtendimento = (status === 200) ? data : []
        this.api.totalTipoAtendimento = totalTipoAtendimento
        if (totalTipoAtendimento.length > 0) {
          const monthName = item => {
            var a = moment(item.periodo, 'MM-YYYY').format('MMM')
            a.toString()
            return a
          }

          var tiposDeAtendimentoPorMes = _.groupBy(totalTipoAtendimento, monthName)
          // VERIFICAR OS ATENDIMENTOS QUE ESTAO RETORNANDO
          var resultado = tiposDeAtendimentoPorMes

          // o resultado é uma lista que contém array dentro, e nos arrays tem no máximo 3 objetos ("TOTEM, PRESENCIAL, TELECONSULTA")
          // console.log("resultado", resultado)

          if (resultado.jan != undefined) {
            await this.dadosMensaisGraficoAdmin(resultado.jan, 'Jan')
          } else {
            this.graficoTotalTipoAtendimento.xAxis.categories.push('Jan')
            this.graficoTotalTipoAtendimento.series[0].data.push(0)
            this.graficoTotalTipoAtendimento.series[1].data.push(0)
            this.graficoTotalTipoAtendimento.series[2].data.push(0)
          }
          if (resultado.fev != undefined) {
            await this.dadosMensaisGraficoAdmin(resultado.fev, 'Fev')
          } else {
            this.graficoTotalTipoAtendimento.xAxis.categories.push('Fev')
            this.graficoTotalTipoAtendimento.series[0].data.push(0)
            this.graficoTotalTipoAtendimento.series[1].data.push(0)
            this.graficoTotalTipoAtendimento.series[2].data.push(0)
          }
          if (resultado.mar != undefined) {
            await this.dadosMensaisGraficoAdmin(resultado.mar, 'Mar')
          } else {
            this.graficoTotalTipoAtendimento.xAxis.categories.push('Mar')
            this.graficoTotalTipoAtendimento.series[0].data.push(0)
            this.graficoTotalTipoAtendimento.series[1].data.push(0)
            this.graficoTotalTipoAtendimento.series[2].data.push(0)
          }
          if (resultado.abr != undefined) {
            await this.dadosMensaisGraficoAdmin(resultado.abr, 'Abr')
          } else {
            this.graficoTotalTipoAtendimento.xAxis.categories.push('Abr')
            this.graficoTotalTipoAtendimento.series[0].data.push(0)
            this.graficoTotalTipoAtendimento.series[1].data.push(0)
            this.graficoTotalTipoAtendimento.series[2].data.push(0)
          }
          if (resultado.mai != undefined) {
            await this.dadosMensaisGraficoAdmin(resultado.mai, 'Mai')
          } else {
            this.graficoTotalTipoAtendimento.xAxis.categories.push('Mai')
            this.graficoTotalTipoAtendimento.series[0].data.push(0)
            this.graficoTotalTipoAtendimento.series[1].data.push(0)
            this.graficoTotalTipoAtendimento.series[2].data.push(0)
          }
          if (resultado.jun != undefined) {
            await this.dadosMensaisGraficoAdmin(resultado.jun, 'Jun')
          } else {
            this.graficoTotalTipoAtendimento.xAxis.categories.push('Jun')
            this.graficoTotalTipoAtendimento.series[0].data.push(0)
            this.graficoTotalTipoAtendimento.series[1].data.push(0)
            this.graficoTotalTipoAtendimento.series[2].data.push(0)
          }
          if (resultado.jul != undefined) {
            await this.dadosMensaisGraficoAdmin(resultado.jul, 'Jul')
          } else {
            this.graficoTotalTipoAtendimento.xAxis.categories.push('Jul')
            this.graficoTotalTipoAtendimento.series[0].data.push(0)
            this.graficoTotalTipoAtendimento.series[1].data.push(0)
            this.graficoTotalTipoAtendimento.series[2].data.push(0)
          }
          if (resultado.ago != undefined) {
            await this.dadosMensaisGraficoAdmin(resultado.ago, 'Ago')
          } else {
            this.graficoTotalTipoAtendimento.xAxis.categories.push('Ago')
            this.graficoTotalTipoAtendimento.series[0].data.push(0)
            this.graficoTotalTipoAtendimento.series[1].data.push(0)
            this.graficoTotalTipoAtendimento.series[2].data.push(0)
          }
          if (resultado.set != undefined) {
            await this.dadosMensaisGraficoAdmin(resultado.set, 'Set')
          } else {
            this.graficoTotalTipoAtendimento.xAxis.categories.push('Set')
            this.graficoTotalTipoAtendimento.series[0].data.push(0)
            this.graficoTotalTipoAtendimento.series[1].data.push(0)
            this.graficoTotalTipoAtendimento.series[2].data.push(0)
          }
          if (resultado.out != undefined) {
            await this.dadosMensaisGraficoAdmin(resultado.out, 'Out')
          } else {
            this.graficoTotalTipoAtendimento.xAxis.categories.push('Out')
            this.graficoTotalTipoAtendimento.series[0].data.push(0)
            this.graficoTotalTipoAtendimento.series[1].data.push(0)
            this.graficoTotalTipoAtendimento.series[2].data.push(0)
          }
          if (resultado.nov != undefined) {
            await this.dadosMensaisGraficoAdmin(resultado.nov, 'Nov')
          } else {
            this.graficoTotalTipoAtendimento.xAxis.categories.push('Nov')
            this.graficoTotalTipoAtendimento.series[0].data.push(0)
            this.graficoTotalTipoAtendimento.series[1].data.push(0)
            this.graficoTotalTipoAtendimento.series[2].data.push(0)
          }
          if (resultado.dez != undefined) {
            await this.dadosMensaisGraficoAdmin(resultado.dez, 'Dez')
          } else {
            this.graficoTotalTipoAtendimento.xAxis.categories.push('Dez')
            this.graficoTotalTipoAtendimento.series[0].data.push(0)
            this.graficoTotalTipoAtendimento.series[1].data.push(0)
            this.graficoTotalTipoAtendimento.series[2].data.push(0)
          }

          // console.log("this.graficoTotalTipoAtendimento.xAxis.categories", this.graficoTotalTipoAtendimento.xAxis.categories)
          // console.log("this.graficoTotalTipoAtendimento.series", this.graficoTotalTipoAtendimento.series)
        }
      },

      // RETORNANDO TEMPO DE ATENDIMENTO POR MEDICO TABLE ADMIN
      getTempoAtendimentoPorMedico (val) {
        if (this.api.atendimentos.length > 0) {
          var tempoDosAtendimentos = []
          this.api.atendimentos.forEach(atendimento => {
            
            if (atendimento.profissional.id === val) {
              if (atendimento.tempoAtendimento != null && atendimento.tempoAtendimento != undefined && atendimento.tempoAtendimento != '') {
                var tempoAtendimentoInt = parseInt(atendimento.tempoAtendimento)
                tempoDosAtendimentos.push(tempoAtendimentoInt)
              }
            }
          })
          // se tiver tempo de atendimento
          if (tempoDosAtendimentos.length > 0) {
            // fazendo a soma de todos os segundos dentro do array
            var tempoTotalAtendimentos = 0
            for (var i = 0; i < tempoDosAtendimentos.length; i++) {
              tempoTotalAtendimentos += tempoDosAtendimentos[i]
            }

            // tempo medio em segundos para minutos
            var tempoEmSegundosMedia = tempoTotalAtendimentos / tempoDosAtendimentos.length
            var tempoMinutosMedia = moment.utc(tempoEmSegundosMedia * 1000).format('mm')
            return tempoMinutosMedia
          } else {
            var tempo = '00'
            return tempo
          }
        }
      },

      // RETORNANDO COLUNA DA TABLE DO MEDICO AUSENTES
      getAusentesTable (val) {
        if (this.api.agendamentos.length > 0) {
          var agendamentosDoMedico = this.api.agendamentos.filter(agendamento => {
            if (agendamento.profissionalId === val) {
              if (agendamento.cancelado == true || agendamento.naoCompareceu == true) {
                return agendamento
              }
            }
          })
          var agendamentoString = agendamentosDoMedico.length.toString()
          return agendamentoString
        }
      },

      // RETORNANDO COLUNA DA TABLE DO MEDICO ATENDIDOS
      getAtendidosTable (val) {
        if (this.api.agendamentos.length > 0) {
          var agendamentosDoMedico = this.api.agendamentos.filter(agendamento => {
            if (agendamento.profissionalId === val) {
              if (agendamento.presencaConfirmada == true && agendamento.finalizado == true) {
                return agendamento
              }
            }
          })
          var agendamentoString = agendamentosDoMedico.length.toString()
          return agendamentoString
        }
      },

      // RETORNANDO OS PROFISSIONAIS PARA A TABLE DO ADMIN
      async getProfissionais () {
        let { data, paginacao, status } = await _api.profissionais.get(this.paramsGetProfissionais)
        this.api.profissionais = (status === 200) ? data : []
        this.paramsGetProfissionais.skip = (status === 200) ? paginacao.skip : 0
        this.paramsGetProfissionais.total = (status === 200) ? paginacao.totalCount : 0
      },

      // RETORNANDO SEXO DO INDIVIDUOS PARA GRAFICO SEXO
      async getIndividuos () {
        let { data, status } = await _api.individuos.getAll(this.params)
        var dataArray = (status === 200) ? data : []
        var masculino = []
        var feminino = []
        dataArray.forEach(paciente => {
          if (paciente.sexo === 1) {
            feminino.push(paciente)
          } else {
            masculino.push(paciente)
          }
        })
        var masculinoObj = { 'name': 'Masculino', 'cadastros': masculino.length }
        var femininoObj = { 'name': 'Feminino', 'cadastros': feminino.length }
        var array = []
        array.push(masculinoObj)
        array.push(femininoObj)
        // prop name para aparecer o nome quando passar o mouse e prop y para pegar no contador / propriedades do highchart
        array.forEach(x => {
          x.name = `${x.name}`
          x.y = parseInt(x.cadastros)
        })
        this.graficoQuantidadePorSexo.series[0].data = array
      },

      // RETORNANDO TEMPO DOS ATENDIMENTOS
      async getAtendimentos(val) {
        var myParams = {}
        if (this.mxHasAccess('Médico') || this.mxHasAccess('MédicoAD')) {
          // parametros
          myParams = {
            ...this.params, atendidoMedico: true, atendidoTriagem: false, profissionalId: this.$auth.user().id
          }
        }

        if (val != null || val != undefined) {
          myParams = { ...myParams, tipoDaConsulta: this.filtroParams.tipoDaConsulta }
        }

        // retornando atendimentos
        _api.atendimentos.get(myParams ? myParams : { ...this.params, atendidoMedico: true, take: 100000 }).then(res => {
          if (res.status === 200) {
            // declaração das variaveis
            var tempoDosAtendimentos = []
            this.api.atendimentos = res.data
            var atendimentos = res.data
            var mesAtual = moment(new Date()).format('MM')

            // retornando para o array o tempo em segundos do mês atual
            atendimentos.forEach(atendimento => {
              if (atendimento.dataCadastro != '' && atendimento.dataCadastro != undefined && atendimento.dataCadastro != null) {
                // puxando o mes do atendimento
                var mesAtendimento = moment(atendimento.dataCadastro).format('MM')

                // comparando e retornando tempo e retornando tempo

                if (mesAtual == mesAtendimento) {
                  if (atendimento.tempoAtendimento != '' && atendimento.tempoAtendimento != undefined && atendimento.tempoAtendimento != null) {
                    var retornoInt = parseInt(atendimento.tempoAtendimento)
                    tempoDosAtendimentos.push(retornoInt)
                  }
                }
              }
            })

            if (tempoDosAtendimentos.length > 0) {
              // fazendo a soma de todos os segundos dentro do array
              var tempoTotalAtendimentos = 0
              for (var i = 0; i < tempoDosAtendimentos.length; i++) {
                tempoTotalAtendimentos += tempoDosAtendimentos[i]
              }

              // tempo medio em segundos e depois em minutos
              var tempoEmSegundosMedia = tempoTotalAtendimentos / tempoDosAtendimentos.length
              var tempoMinutosMedia = moment.utc(tempoEmSegundosMedia * 1000).format('mm')
              this.duracaoAtendimento = tempoMinutosMedia
            } else {
              this.duracaoAtendimento = 'Sem Dados'
            }
          }
        })
      },

      async isSenhaAlterada () {
        if (this.user.senhaAlterada === true) {
          this.$router.push({
            name: 'RedefinirSenhaIndividuo'
          })
        } else {

        }
      },

      async onFiltrar () {
        this.pacientesAgendados = []
        this.pacientesConfirmados = []
        this.pacientesAtendidos = []
        this.pacientesAusentes = []
        this.graficoGeral.series[0].data = []

        this.getAgendamentos(this.filtroParams.tipoDaConsulta)
        this.getAtendimentos(this.filtroParams.tipoDaConsulta)
      },

      // GRAFICO POR TIPO DO ATENDIMENTO
      async getTipoDaConsultaAgendamentosGrafico (val) {
        var params = {
          profissionalId: this.$auth.user().id,
          skip: 1,
          take: 10000
        }
        _api.agendamentos.get(params).then(res => {
          if (res.status === 200) {
            var mesAtual = moment(new Date()).format('MM')
            var agendamentos = res.data
            var agendamentoTotem = []
            var agendamentoTeleconsulta = []
            var agendamentoPresencial = []

            agendamentos.forEach(agendamento => {
              if (agendamento.dia != '' && agendamento.dia != undefined && agendamento.dia != null) {
                var mesAtendimento = moment(agendamento.dia).format('MM')

                if (mesAtual == mesAtendimento) {
                  if (agendamento.tipoDaConsulta == 'Totem') {
                    // console.log("totem: ", agendamento)
                    agendamentoTotem.push(agendamento)
                  }
                  if (agendamento.tipoDaConsulta == 'Teleconsulta') {
                    agendamentoTeleconsulta.push(agendamento)
                  }
                  if (agendamento.tipoDaConsulta == 'Presencial') {
                    agendamentoPresencial.push(agendamento)
                  }
                }
              }
            })

            var totem = ['Totem', agendamentoTotem.length]
            var teleConsulta = ['Teleconsulta', agendamentoTeleconsulta.length]
            var presencial = ['Presencial', agendamentoPresencial.length]

            this.graficoTipoDeAtendimento.series[0].data.push(totem)
            this.graficoTipoDeAtendimento.series[0].data.push(teleConsulta)
            this.graficoTipoDeAtendimento.series[0].data.push(presencial)
          }
        })
      },

      async getAgendamentos (val) {
        // RETORNO AGENDAMENTOS
        // se o usuario logado for o médico
        if (this.$config.configDB.modulo !== 3 && this.mxHasAccess('Médico') || this.mxHasAccess('MédicoAD')) this.paramsGetAgendamentos.profissionalId = this.$auth.user().id
        if (this.mxHasAccess('Recepcionista')) this.paramsGetAgendamentos.estabelecimentoId = this.$store.state.user.dados.estabelecimentoId
        if (this.mxHasAccess('Triagem')) this.paramsGetAgendamentos.estabelecimentoId = this.$store.state.user.dados.estabelecimentoId

        if (val != null || val != undefined) {
          this.paramsGetAgendamentos = { ...this.paramsGetAgendamentos, 'tipoDaConsulta': this.filtroParams.tipoDaConsulta }
        }
        _api.agendamentos.get(this.paramsGetAgendamentos).then(res => {
          if (res.status === 200) {
            this.api.agendamentos = res.data
            var agendamentos = res.data
            this.pacientesAgendamentos = []
            this.pacientesConfirmados = []
            this.pacientesAtendidos = []
            this.pacientesAusentes = []

            agendamentos.forEach(agendamento => {
              // push nos pacientes agendados POREM NÂO CONFIRMADOS
              let dateNowFormatted = moment(new Date()).format('DD-MM-YYYY')
              let agendamentoDia = moment(agendamento.dia).format('DD-MM-YYYY')
              if (agendamento.presencaConfirmada == false && agendamento.finalizado == false && agendamento.naoCompareceu == false && agendamento.cancelado == false && agendamentoDia == dateNowFormatted) {
                this.pacientesAgendados.push(agendamento)
              }

              // push nos pacientes confirmados
              if (agendamento.presencaConfirmada == true && agendamento.finalizado == false && agendamento.naoCompareceu == false && agendamento.cancelado == false && agendamentoDia == dateNowFormatted) {

                if (this.$config.configDB.modulo === 3 && (!agendamento.profissionalId || agendamento.profissionalId == this.$auth.user().id)) this.pacientesConfirmados.push(agendamento)
                else if (this.$config.configDB.modulo !== 3) this.pacientesConfirmados.push(agendamento)
              }

              // push nos pacientes atendidos
              if (agendamento.presencaConfirmada == true && agendamento.finalizado == true && agendamento.naoCompareceu == false && agendamento.cancelado == false) {
                this.pacientesAtendidos.push(agendamento)
              }

              // push nos pacientes cancelados ou que nao compareceram
              if ((agendamento.cancelado == true || agendamento.naoCompareceu == true) && agendamento.finalizado == true) {
                this.pacientesAusentes.push(agendamento)
              }
            })

            this.loading.agendamentos = false;

            this.getGrafico()
          } else if (res.status === 204) {
            this.pacientesAgendados = []
            this.pacientesConfirmados = []
            this.pacientesAtendidos = []
            this.pacientesAusentes = []
            this.graficoGeral.series[0].data = []
            this.loading.agendamentos = false;
          } else { }
        })
      },

      // GRÁFICOS
      async getGrafico () {
        var pacientesAgendados = { 'name': 'Agendados', 'atendimentos': this.pacientesAgendados.length }
        var pacientesConfirmados = { 'name': 'Confirmados', 'atendimentos': this.pacientesConfirmados.length }
        var pacientesAtendidos = { 'name': 'Atendidos', 'atendimentos': this.pacientesAtendidos.length }
        var pacientesAusentes = { 'name': 'Ausentes', 'atendimentos': this.pacientesAusentes.length }

        var data = []
        data.push(pacientesAgendados)
        data.push(pacientesConfirmados)
        data.push(pacientesAtendidos)
        data.push(pacientesAusentes)

        if (data != []) {
          // prop name para aparecer o nome quando passar o mouse e prop y para pegar no contador / propriedades do highchart
          data.forEach(x => {
            x.name = `${x.name}`
            x.y = parseInt(x.atendimentos)
          })

          this.api.arrayGraficoGeral = data
          this.graficoGeral.series[0].data = data
        }
      },

      onEmitFromFiltro (val) {
        this.paramsGetAgendamentos = {
          ...this.paramsGetAgendamentos,
          ...val.params,
          skip: 1
        }

        this.paramsGetProfissionais = {
          ...this.paramsGetProfissionais,
          ...val.params,
          skip: 1
        }

        this.paramsGetTotalTipoAtendimento = {
          ...this.paramsGetTotalTipoAtendimento,
          ...val.params,
          skip: 1
        }

        this.getAgendamentos()
        this.getTotalTipoAtendimento()
      }
    }
  }
</script>

<style>
  .acessibilidade {
  }

  .ausentes a {
    display: flex;
    flex-direction: column;
    align-items: center;
    color: red;
  }

  .ausentes {
    display: flex;
    flex-direction: column;
    align-items: center;
    color: red;
  }

  .agendados a {
    display: flex;
    flex-direction: column;
    align-items: center;
    color: lightslategray
  }

  .confirmados a {
    display: flex;
    flex-direction: column;
    align-items: center;
    color: dodgerblue
  }

  .atendidos a {
    display: flex;
    flex-direction: column;
    align-items: center;
    color: forestgreen
  }

  .atendidos {
    display: flex;
    flex-direction: column;
    align-items: center;
    color: forestgreen
  }


  .acumulativo {
    display: flex;
    justify-content: center;
  }

    .acumulativo span {
      font-size: 16px;
    }


  .container__rank_todos_pacientes_sexo {
    display: flex;
    flex-direction: row;
  }

  @media screen and (max-width: 590px) {
    .container__rank_todos_pacientes_sexo {
      flex-direction: column-reverse;
    }
  }
</style>
