<template>
  <el-tabs type="border-card" @tab-click="handleTabClickMedicoesHeader">
    <!-- Glicemia -->
    <el-tab-pane label="Glicemia">
      <el-row>
        <el-col>
          <el-table ref="tableGlicemias" :data="glicemias"
                    v-loading.body="loading.glicemias"
                    highlight-current-row border>

            <el-table-column type="expand">
              <template slot-scope="props">
                <GraficoGlicemia :row="props.row" />
              </template>
            </el-table-column>

            <el-table-column label="Data do Registro" align="center">
              <template slot-scope="scope">
                {{ moment(scope.row.dataCadastro).format("DD/MM/YYYY") }}
              </template>
            </el-table-column>

            <el-table-column label="Paciente" prop="individuo.nomeCompleto" align="center" />

            <el-table-column label="Status" align="center">
              <template slot-scope="scope">

                <el-tag v-if="(scope.row.respondeuCafe != false && scope.row.respondeuCafe != undefined) &&
                              (scope.row.respondeuAlmoco != false && scope.row.respondeuAlmoco != undefined) &&
                              (scope.row.respondeuJanta != false && scope.row.respondeuJanta != undefined) &&
                              (scope.row.respondeuDormirMadrugada != false && scope.row.respondeuDormirMadrugada != undefined)"
                        type="success">Completo</el-tag>

                <el-tag v-else type="danger">Incompleto</el-tag>

              </template>
            </el-table-column>
          </el-table>
        </el-col>
        <el-col>
          <el-pagination @size-change="handleSizeChangeGlicemia"
                         @current-change="handleCurrentChangeGlicemia"
                         :current-page.sync="paramsGlicemia.page"
                         :page-sizes="[5]"
                         :page-size="paramsGlicemia.take"
                         :total="paramsGlicemia.total"
                         layout="total, sizes, prev, pager, next, jumper">
          </el-pagination>
        </el-col>
      </el-row>
      
    </el-tab-pane>

    <!--Antropometria-->
    <el-tab-pane label="Antropometria">
      <el-row v-show="origemProps == 'Modal'">
        <el-col :span="24">
          <el-table ref="tabela" :data="agendamentosTable"
                    highlight-current-row border
                    class="table--agendamentos table--row-click">
            <el-table-column label="Data e Hora da medição" prop="dataCadastro" align="center">
              <template slot-scope="scope">
                {{scope.row.dataAlteracao != null || scope.row.dataAlteracao != "0001-01-01T00:00:00" ? moment(scope.row.dataAlteracao).format('DD/MM/YYYY HH:mm') : 'Erro na Exibição da Data'}}
              </template>
            </el-table-column>
            <el-table-column label="Peso (Kg)" prop="peso" align="center">

              <template slot-scope="scope">
                <span :style="{ color: scope.row.peso == '0' || scope.row.peso == undefined ? '#ff0000' : '#606266' }"> {{scope.row.peso != null && scope.row.peso != "" ? scope.row.peso : 'Peso não informado'}}</span>
              </template>

            </el-table-column>
            <el-table-column label="Altura (cm)" prop="altura" align="center">
              <template slot-scope="scope">
                <span :style="{ color: scope.row.altura == '0' || scope.row.altura == undefined  ? '#ff0000' : '#606266' }">{{scope.row.altura != null && scope.row.altura != "" ? scope.row.altura : 'Altura não informada'}}</span>
              </template>
            </el-table-column>
            <el-table-column label="IMC (kg/m²)" prop="imc" align="center">
              <template slot-scope="scope">
               <span :style="{ color: (scope.row.peso == '0' || scope.row.altura == '0') || (scope.row.altura == undefined || scope.row.peso == undefined) ? '#ff0000' : '#606266' }"> {{calcularIMC(scope.row.peso, scope.row.altura)}} </span>
              </template>
            </el-table-column>
          </el-table>
        </el-col>
        <el-col :span="24">
          <el-pagination @size-change="handleSizeChangeAgendamentosTable"
                         @current-change="handleCurrentChangeAgendamentosTable"
                         :current-page.sync="paramsAgendamentosTable.page"
                         :page-sizes="[3, 5]"
                         :page-size="3"
                         :total="paramsAgendamentosTable.total"
                         layout="total, sizes, prev, pager, next, jumper">
          </el-pagination>
        </el-col>
      </el-row>

      <div style="margin-top: 20px">
        <el-tabs type="border-card" @tab-click="handleTabClickMedicoesGrafico">
          <el-tab-pane label="Peso (Kg)">
            <highcharts :options="graficoPeso"></highcharts>
          </el-tab-pane>

          <el-tab-pane label="Altura">
            <highcharts :options="graficoAltura"></highcharts>
          </el-tab-pane>

          <el-tab-pane label="IMC">
            <highcharts :options="graficoIMC"></highcharts>
          </el-tab-pane>
        </el-tabs>
      </div>
    </el-tab-pane>

    <!--Sinais Vitais-->
    <el-tab-pane label="Sinais Vitais">
      <el-row v-show="origemProps == 'Modal'">
        <el-col :span="24">
          <el-table ref="tabela" :data="agendamentosTable"
                    highlight-current-row border
                    class="table--agendamentos table--row-click">
            <el-table-column label="Data da Medição" prop="dataCadastro" align="center">
              <template slot-scope="scope">
                {{scope.row.dataCadastro != null || scope.row.dataCadastro != "0001-01-01T00:00:00" ? moment(scope.row.dataCadastro).format('DD/MM/YYYY HH:mm') : 'Erro na Exibição da Data'}}
              </template>
            </el-table-column>
            <el-table-column label="Pressão Arterial(mmHg)" prop="pressaoSanguinea" align="center">
              <template slot-scope="scope">
                <span :style="{ color: (validaPressaoSanguinea(scope.row.pressaoSanguinea) == 'Invalido') || (scope.row.pressaoSanguinea == '0') || (scope.row.pressaoSanguinea == undefined) ? '#ff0000' : '#606266' }">{{scope.row.pressaoSanguinea != null && scope.row.pressaoSanguinea != "" ? scope.row.pressaoSanguinea : 'Pressao Sanguinea não informada'}}</span>
              </template>
            </el-table-column>
            <el-table-column label="Freq. Cardíaca(bpm)" prop="batimentoCardiaco" align="center">
              <template slot-scope="scope">
                <span :style="{ color: (scope.row.batimentoCardiaco <= 20) || (scope.row.batimentoCardiaco == undefined) ? '#ff0000' : '#606266' }">{{scope.row.batimentoCardiaco != null && scope.row.batimentoCardiaco != "" ? scope.row.batimentoCardiaco : 'Freq. Cardíaca não informada'}}</span>
              </template>
            </el-table-column>
            <el-table-column label="Saturação de O2(%)" prop="oxigenacaoSanguinea" align="center">
              <template slot-scope="scope">
                <span :style="{ color: (scope.row.oxigenacaoSanguinea <= '10') || (scope.row.oxigenacaoSanguinea == undefined ) ? '#ff0000' : '#606266' }">{{scope.row.oxigenacaoSanguinea != null && scope.row.oxigenacaoSanguinea != "" ? scope.row.oxigenacaoSanguinea : 'Saturação não informada'}}</span>
              </template>
            </el-table-column>
            <el-table-column label="Temperatura(°C)" prop="temperatura" align="center">
              <template slot-scope="scope">
                <span :style="{ color: (scope.row.temperatura <= '30' || scope.row.temperatura == undefined) ? '#ff0000' : '#606266' }"> {{scope.row.temperatura != null && scope.row.temperatura != "" ? scope.row.temperatura : 'Temperatura não informada'}}</span>
              </template>
            </el-table-column>
          </el-table>
        </el-col>
        <el-col :span="24">
          <el-pagination @size-change="handleSizeChangeAgendamentosTable"
                         @current-change="handleCurrentChangeAgendamentosTable"
                         :current-page.sync="paramsAgendamentosTable.page"
                         :page-sizes="[3, 5, 10]"
                         :page-size="3"
                         :total="paramsAgendamentosTable.total"
                         layout="total, sizes, prev, pager, next, jumper">
          </el-pagination>
        </el-col>
      </el-row>

      <div style="margin-top: 20px">
        <el-tabs type="border-card" @tab-click="handleTabClickMedicoesGrafico">
          <el-tab-pane label="Pressão Arterial">
            <highcharts :options="graficoPressaoArterial"></highcharts>
          </el-tab-pane>

          <el-tab-pane label="Frequência Cardíaca">
            <highcharts :options="graficoFrequenciaCardiaca"></highcharts>
          </el-tab-pane>

          <el-tab-pane label="Saturação de O²">
            <highcharts :options="graficoSaturacao"></highcharts>
          </el-tab-pane>

          <el-tab-pane label="Temperatura">
            <highcharts :options="graficoTemperatura"></highcharts>
          </el-tab-pane>
        </el-tabs>
      </div>
    </el-tab-pane>

    <!--Auscultação-->
    <el-tab-pane label="Auscultação">
      <div class="loading-spinner" v-if="loading.auscultacao"></div>
      <el-row v-if="!loading.auscultacao">
        <el-col :span="24">
          <el-table ref="tabela" :data="auscultacao"
                    highlight-current-row border
                    class="table--row-click">
            <el-table-column label="Data de Envio" prop="dataDeEnvio" align="center">
              <template slot-scope="scope">
                {{scope.row.dataDeEnvio != null || scope.row.dataDeEnvio != "0001-01-01T00:00:00" ? moment(scope.row.dataDeEnvio).format('DD/MM/YYYY HH:mm') : 'Erro na Exibição da Data'}}
              </template>
            </el-table-column>
            <el-table-column label="Nome Do Exame" prop="Nome" align="center">
              <template slot-scope="scope">
                {{scope.row.nome}}
              </template>
            </el-table-column>
            <el-table-column label="Formato" prop="Formato" align="center">
              <template slot-scope="scope">
                {{scope.row.formato}}
              </template>
            </el-table-column>

            <el-table-column header-align="center" fixed="right" width="145">
              <template slot-scope="scope">
                <el-dropdown>
                  <el-button type="primary" size="small">
                    Ações <i class="el-icon-arrow-down el-icon--right"></i>
                  </el-button>
                  <el-dropdown-menu slot="dropdown">
                    <ul class="list-unstyled">
                      <li @click="onVisualizarAtendimento(scope.row)" class="el-dropdown-menu__item"><i class="fas fa-eye"></i> Visualizar</li>
                    </ul>
                  </el-dropdown-menu>
                </el-dropdown>
              </template>
            </el-table-column>
          </el-table>
        </el-col>
        <el-col :span="24">
          <el-pagination @size-change="handleSizeChangeAuscultacao"
                         @current-change="handleCurrentChangeAuscultacao"
                         :current-page.sync="paramsAuscultacao.page"
                         :page-sizes="[10, 15, 20]"
                         :page-size="10"
                         :total="paramsAuscultacao.total"
                         layout="total, sizes, prev, pager, next, jumper">
          </el-pagination>
        </el-col>
      </el-row>
    </el-tab-pane>
  </el-tabs>
</template>

<script>
  import _api from '../../api'
  import moment from 'moment'
  moment.locale('pt-br')
  import { getGraficoPesoAltura } from '../GraficosTemplate/GraficoPesoAltura.js'
  import { getGraficoIMC } from '../GraficosTemplate/GraficoIMC.js'
  import { getGraficoPressaoArterial} from '../GraficosTemplate/GraficoPressaoArterial.js'
  import { getGraficoFrequenciaSaturacaoTemperatura } from '../GraficosTemplate/GraficoFrequenciaSaturacaoTemperatura.js'
  import GraficoGlicemia from '../shared/GraficoGlicemia'
  export default {
    name: 'GraficosDasMedicoes',
    props: {
      agendamento: {},
      origem: '',
    },
    components: { GraficoGlicemia },
    data() {
      return {
        // inicio graficos
        graficoPeso: getGraficoPesoAltura('Peso', 'Peso (Kg)'),
        graficoAltura: getGraficoPesoAltura('Altura', 'Altura(cm)'),
        graficoIMC: getGraficoIMC('IMC', 'IMC(kg/m²)'),
        graficoPressaoArterial: getGraficoPressaoArterial('Pressão Arterial', 'Pressão Arterial(mmHg)', 'Dia'),
        graficoFrequenciaCardiaca: getGraficoFrequenciaSaturacaoTemperatura('Frequência Cardíaca', 'Frequência Cardíaca (bpm)', 'Dia'),
        graficoSaturacao: getGraficoFrequenciaSaturacaoTemperatura('Saturação de O²', 'Saturação de O²', 'Dia'),
        graficoTemperatura: getGraficoFrequenciaSaturacaoTemperatura('Temperatura', 'Temperatura (°C)', 'Dia'),
        //fim graficos

        //armazenando dados da rota
        agendamentoProps: {},

        //armazenando dados da origem
        origemProps: {},

        //dados da table
        agendamentosTable: [],

        //dados dos agendamentos para os graficos
        agendamentosGrafico: [],

        //dados da auscutação
        auscultacao: [],

        //dados da glicemia
        glicemias: [],

        loading: {
          auscultacao: false,
          glicemias: false
        },

        // Params Inicio
        paramsAgendamentosTable: {
          skip: 1,
          take: 3,
          individuoId: null,
          sort: 'a.DataCadastro DESC',
          total: 0,
          interconsulta: false
        },

        paramsAuscultacao: {
          skip: 1,
          sort: 'Nome ASC',
          take: 10
        },

        paramsCreated: {
          skip: 1,
          take: 9999,
          individuoId: null,
          sort: 'a.DataCadastro ASC',
          total: 0,
          interconsulta: false
        },

        paramsGlicemia: {
          skip: 1,
          take: 10,
          sort: 'ig.DataCadastro DESC'
        },
        // Params Fim
      }
    },

    async created() {
      this.agendamentoProps = this.agendamento

      //console.log("this.agendamentoProps created", this.agendamentoProps)
      this.origemProps = this.origem

      //passando individuo id para os params da table
      this.paramsAgendamentosTable.individuoId = this.agendamentoProps.individuoId

      //passando individuo id para os params da glicemia
      this.paramsGlicemia.individuoId = this.agendamentoProps.individuoId

      //passando individuo id para os params do created para os graficos
      this.paramsCreated.individuoId = this.agendamentoProps.individuoId

      // metodo para retorno das glicemias
      await this.getGlicemias()

      // metodo para retorno dos agendamentos para a table
      await this.getAgendamentosTable()

      // metodo para retorno dos agendamentos para dados dos graficos
      await this.getAgendamentosGrafico()

      // metodos dos graficos
      await this.graficoPesoMethods()
      await this.graficoAlturaMethods()
      await this.graficoIMCMethods()
      await this.graficoPressaoArterialMethods()
      await this.graficoFrequenciaCardiacaMethods()
      await this.graficoSaturacaoMethods()
      await this.graficoTemperaturaoMethods()
    },

    methods: {
      // retornar as glicemias para o grafico
      async getGlicemias() {
        this.loading.glicemias = true

        let { data, status, paginacao } = await _api.glicemias.get(this.paramsGlicemia)
        this.glicemias = (status === 200) ? data : []

        this.paramsGlicemia.skip = (status === 200) ? paginacao.skip : 0
        this.paramsGlicemia.total = (status === 200) ? paginacao.totalCount : 0

        this.loading.glicemias = false
      },

      // retornar agendamentos para a table
      async getAgendamentosTable() {
        this.paramsAgendamentosTable = { ...this.paramsAgendamentosTable, sinaisVitaisGrafico: true }

        let { data, paginacao, status } = await _api.agendamentos.get(this.paramsAgendamentosTable)
        if (status === 200 && data != undefined) {
          var jsonString = JSON.stringify(data)
          var array = JSON.parse(jsonString);
          this.agendamentosTable = array
        }

        //console.log("this.agendamentosTable", this.agendamentosTable)

        this.paramsAgendamentosTable.skip = (status === 200) ? paginacao.skip : 0
        this.paramsAgendamentosTable.total = (status === 200) ? paginacao.totalCount : 0
      },

      // retornar agendamentos para os dados dos graficos
      async getAgendamentosGrafico() {
        this.paramsCreated = { ...this.paramsCreated, sinaisVitaisGrafico: true }
        let { data, status } = await _api.agendamentos.get(this.paramsCreated)
        var array = (status === 200) ? data : []

        array.forEach(agendamento => {
          if (agendamento.peso == 0) agendamento.peso = null
          if (agendamento.altura == 0) agendamento.altura = null
          this.agendamentosGrafico.push(agendamento)
        })

        //console.log("this.agendamentosGrafico", this.agendamentosGrafico)
      },

      //METODOS DOS GRAFICOS
      // gráfico de peso
      async graficoPesoMethods() {
        this.agendamentosGrafico.forEach(data => {
          // Nome do paciente na Linha,
          if (data.individuo.nomeCompleto != '') {
            this.graficoPeso.series[0].name = data.individuo.nomeCompleto
          }

          // Retorno dos dados do peso,
          if (data.peso != '' || data.peso != null || data.peso != undefined) {
            var peso = parseFloat(data.peso)
            this.graficoPeso.series[0].data.push(peso)
          }

          // Retorno da idade do paciente na época do atendimento
          var d2 = data.individuo.dataNascimento
          var d1 = moment(data.dataCadastro)
          var idadeMes = d1.diff(d2, 'Month')
          var idadeAno = parseFloat(idadeMes / 12).toFixed(1)
          var idadeString = idadeAno.toString()

          // Adicionando idade aos eixos
          if (idadeString != '' && data.peso > "0") {
            this.graficoPeso.xAxis.categories.push(idadeString)
          }
        })
      },

      // gráfico de altura
      async graficoAlturaMethods() {
        this.agendamentosGrafico.forEach(data => {
          // Nome do paciente na Linha,
          if (data.individuo.nomeCompleto != '') {
            this.graficoAltura.series[0].name = data.individuo.nomeCompleto
          }

          // Retorno da Altura
          if (data.altura != null || data.altura != '') {
            var altura = parseFloat(data.altura)
            this.graficoAltura.series[0].data.push(altura)
          }

          // Retorno da idade do paciente na época do atendimento
          var d2 = data.individuo.dataNascimento
          var d1 = moment(data.dataCadastro)
          var idadeMes = d1.diff(d2, 'Month')
          var idadeAno = parseFloat(idadeMes / 12).toFixed(1)
          var idadeString = idadeAno.toString()

          // Adicionando idade aos eixos
          if (idadeString != '' && data.altura > "0") {
            this.graficoAltura.xAxis.categories.push(idadeString)
          }
        })
      },

      // gráfico do IMC
      async graficoIMCMethods() {
        this.agendamentosGrafico.forEach(data => {
          // Nome do paciente na Linha,
          if (data.individuo.nomeCompleto != '') {
            this.graficoIMC.series[0].name = data.individuo.nomeCompleto
          }

          // Retorno do IMC
          if ((data.peso != null && data.peso != '' && data.peso > 0) && (data.altura != null && data.altura != '' && data.altura > 0)) {
            var imcInicial = (data.peso / ((data.altura / 100) * (data.altura / 100)))
            var imcFinal = parseFloat(imcInicial).toFixed(1)
            var imc = parseFloat(imcFinal)
            this.graficoIMC.series[0].data.push(imc)
          }

          // Retorno da idade do paciente na época do atendimento
          var d2 = data.individuo.dataNascimento
          var d1 = moment(data.dataCadastro)
          var idadeMes = d1.diff(d2, 'Month')
          var idadeAno = parseFloat(idadeMes / 12).toFixed(1)
          var idadeString = idadeAno.toString()

          // Adicionando idade aos eixos
          if (idadeString != '') {
            this.graficoIMC.xAxis.categories.push(idadeString)
          }
        })
      },

      // gráfico de pressao arterial
      async graficoPressaoArterialMethods() {
        this.agendamentosGrafico.forEach(data => {
          // Pressao Arterial
          if (data.pressaoSanguinea != '' || data.pressaoSanguinea != null) {
            // Retornando a data para o eixo X
            const locale = 'pt-br'
            var dataCadastroFormatada = new Date(data.dataCadastro).toLocaleDateString(locale)

            // Retornando os dados para o grafico
            var resultado = []
            if (data.pressaoSanguinea != undefined) {

              var retorno = this.validaPressaoSanguinea(data.pressaoSanguinea)
              if (retorno == "Valido") {
                resultado = data.pressaoSanguinea.split('/')

                // adicionando a data
                this.graficoPressaoArterial.xAxis.categories.push(dataCadastroFormatada)

                // adicionando os dados
                //sistólica
                this.graficoPressaoArterial.series[0].data.push(parseFloat(resultado[0]))
                //diastólica
                this.graficoPressaoArterial.series[1].data.push(parseFloat(resultado[1]))

              }
              else { }
            }

            
          }
        })
      },

      // grafico frequencia cardiaca
      async graficoFrequenciaCardiacaMethods() {
        this.agendamentosGrafico.forEach(data => {
          // Nome do paciente na Linha,
          if (data.individuo.nomeCompleto != '') {
            this.graficoFrequenciaCardiaca.series[0].name = data.individuo.nomeCompleto
          }

          // Frequencia Cardiaca
          if (data.batimentoCardiaco != '' && data.batimentoCardiaco != null && data.batimentoCardiaco > '20') {
            // Retornando a data para o eixo X
            const locale = 'pt-br'
            let dataCadastroFormatada = new Date(data.dataCadastro).toLocaleDateString(locale)

            // adicionando a data
            this.graficoFrequenciaCardiaca.xAxis.categories.push(dataCadastroFormatada)

            // adicionando os dados
            let batimentoCardiaco = parseFloat(data.batimentoCardiaco)

            this.graficoFrequenciaCardiaca.series[0].data.push(batimentoCardiaco)
          }
        })
      },

      // grafico saturação
      async graficoSaturacaoMethods() {
        this.agendamentosGrafico.forEach(data => {
          // Nome do paciente na Linha,
          if (data.individuo.nomeCompleto != '') {
            this.graficoSaturacao.series[0].name = data.individuo.nomeCompleto
          }

          // Saturacao de O²
          if (data.oxigenacaoSanguinea != '' && data.oxigenacaoSanguinea != null && data.oxigenacaoSanguinea > "10") {
            // Retornando a data para o eixo X
            const locale = 'pt-br'
            let dataFinal = new Date(data.dataCadastro).toLocaleDateString(locale)

            // adicionando a data
            this.graficoSaturacao.xAxis.categories.push(dataFinal)

            // adicionando os dados
            let saturacao = parseFloat(data.oxigenacaoSanguinea)

            this.graficoSaturacao.series[0].data.push(saturacao)
          }
        })
      },

      // grafico temperatura
      async graficoTemperaturaoMethods() {
        this.agendamentosGrafico.forEach(data => {
          // Nome do paciente na Linha,
          if (data.individuo.nomeCompleto != '') {
            this.graficoTemperatura.series[0].name = data.individuo.nomeCompleto
          }

          // Temperatura
          if (data.temperatura != '' && data.temperatura != null && data.temperatura > "30") {
            // Retornando a data para o eixo X
            const locale = 'pt-br'
            let dataFinal = new Date(data.dataCadastro).toLocaleDateString(locale)

            // adicionando a data
            this.graficoTemperatura.xAxis.categories.push(dataFinal)

            // adicionando os dados
            let temperatura = parseFloat(data.temperatura)

            this.graficoTemperatura.series[0].data.push(temperatura)
          }
        })
      },
      //FIM DOS METODOS DOS GRAFICOS

      //AUSCULTAÇÃO
      async getAuscultacao() {
        this.paramsAuscultacao.skip = this.paramsAuscultacao.skip === 0 ? 1 : this.paramsAuscultacao.skip; 
        this.loading.auscultacao = true
        this.paramsAuscultacao.IndividuoId = this.agendamento.individuoId
        this.paramsAuscultacao.IndividuoCpf = this.agendamento.individuo.cpf
        this.paramsAuscultacao.Formato = '.pdf'

        let { data, status, paginacao } = await _api.exames.getAuscultacao(this.paramsAuscultacao)
        this.auscultacao = (status === 200 && data != undefined) ? data : []
        this.paramsAuscultacao.skip = (status === 200) ? paginacao.skip : 0
        this.paramsAuscultacao.total = (status === 200) ? paginacao.totalcount : 0
        this.loading.auscultacao = false
      },

      //EVENTOS
      // caso necessite de algum evento na troca de menu do header
      handleTabClickMedicoesHeader(tab, event) {
        if (tab.paneName === '3') {
          this.getAuscultacao()
        }
      },

      // caso necessite de algum evento na troca de menu entre os graficos
      handleTabClickMedicoesGrafico(tab, event) {
        if (tab.label === 'Peso (Kg)') {

        }
      },
      //FIM EVENTOS


      //FUNÇÕES AUXILIARES
      //validar a pressao sanguinea
      validaPressaoSanguinea(pressaoSanguinea) {
        if (pressaoSanguinea != undefined && pressaoSanguinea != 0 && pressaoSanguinea != null && pressaoSanguinea != "") {
          var pressaoSplit = pressaoSanguinea.split("/")
          let pressaoSistolica = pressaoSplit[0]
          let pressaoDiastolica = pressaoSplit[1]

          //validando
          var sistolica = false
          var diastolica = false
          //validar sistolica
          if (pressaoSistolica > 3) sistolica = true
          if (pressaoDiastolica.toString() > "3") diastolica = true

          if (sistolica == true && diastolica == true) return "Valido"
          else return "Invalido"
        }
      },


      // calcular imc da table e do grafico
      calcularIMC(peso, altura) {
        if ((peso != null && peso != '' && peso != 0) && (altura != null && altura != '' && altura != 0)) {
          var imcInicial = (peso / ((altura / 100) * (altura / 100)))
          var imcFinal = parseFloat(imcInicial).toFixed(1)
          return imcFinal
        } else {
          return 'Erro no cálculo do IMC'
        }
      },

      // paginação das tables
      handleSizeChangeAgendamentosTable(val) {
        this.paramsAgendamentosTable.take = val
        this.getAgendamentosTable()
      },

      // paginação das tables
      handleCurrentChangeAgendamentosTable(val) {
        this.paramsAgendamentosTable.skip = val
        this.getAgendamentosTable()
      },

      // paginação da table Auscultacao

      async handleSizeChangeAuscultacao(val) {
        this.paramsAuscultacao.take = val
        await this.getAuscultacao()
      },
      // paginação da table Auscultacao

      async handleCurrentChangeAuscultacao(val) {
        this.paramsAuscultacao.skip = val
        await this.getAuscultacao()
      },

      async handleSizeChangeGlicemia(val) {
        this.filtroParams.take = val
        await this.getGlicemias()
      },

      async handleCurrentChangeGlicemia(val) {
        this.filtroParams.skip = val
        await this.getGlicemias()
      },

      // visualizar Auscultação
      onVisualizarAtendimento(val) {
        if (val.formato === '.pdf') {
          var win = window.open()
          win.document.write(`<iframe src="${val.url}" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe>`)
        }
        if (val.formato === '.wav') {
          var win = window.open()
          win.document.write(`
              <div style="display: flex; justify-content: center; align-items: center; width: 100%; height: 100%;">
                <audio controls>
                   <source src="${val.url}" type="audio/wav">
                   <p><a href="${val.url}"link</a></p>
                </audio>
              </div>
          `)
        }
      }
      //FIM FUNÇÕES AUXILIARES
    }
  }
</script>
