<template>
  <div>
    <el-card style="padding-bottom: 30px">
      <div label="Histórico">
        <el-empty v-show="api.historico.length === 0" description="Nenhum histórico encontrado"></el-empty>
        <div style="margin-top:10px">
          <span v-show="api.historico.length > 0" style="font-weight: bold; font-size: 15px;">Últimos Atendimentos</span>
        </div>
        <el-table v-show="api.historico.length > 0" ref="tabela" :data="api.historico"
                  highlight-current-row border
                  v-loading.body="loading.historico"
                  :row-class-name="tableRow"
                  class="table--atendimento table--row-click"
                  :default-sort="{prop: 'dataCadastro', order: 'descending'}">
          <el-table-column label="Data da Consulta" prop="dataCadastro" align="center" width="200" sortable>
            <template slot-scope="scope">
              <span>{{moment(scope.row.dataCadastro).format('DD/MM/YYYY HH:mm')}}</span>
            </template>
          </el-table-column>
          <el-table-column label="Individuo" prop="individuo.nomeCompleto" align="center" />
          <el-table-column label="Profissional" prop="profissional.nome" align="center" />
          <el-table-column label="CRM" prop="profissional.crm" align="center" width="100" />
          <el-table-column header-align="center" align="center" width="100">
            <template slot-scope="scope">
              <el-dropdown>
                <el-button @click="openModalHistorico(scope.row)" type="primary" size="small">
                  Visualizar
                </el-button>
              </el-dropdown>
            </template>
          </el-table-column>
        </el-table>
        <el-col :span="24">
          <el-pagination @size-change="handleSizeChangeHistorico"
                         @current-change="handleCurrentChangeHistorico"
                         :current-page.sync="paramsHistorico.page"
                         :page-sizes="[10,25]"
                         :page-size="10"
                         :total="paramsHistorico.total"
                         layout="total, sizes, prev, pager, next, jumper">
          </el-pagination>
        </el-col>
      </div>
    </el-card>

    <modal name="resumoHistoricoModal" :draggable="true" :clickToClose="false" :width="800" :height="500">
      <div class="window-header" style="padding: 15px;padding-bottom: 0px;display: flex;justify-content: flex-end;">
        <el-button type="danger" @click="hideModalHistorico" icon="el-icon-close"></el-button>
      </div>
      <VuePerfectScrollbar class="scroll-area-historico" :settings="scrollSettings" key="scrol-atendimento">
        <el-row>
          <el-col :span="12">
            <div class="grid-content bg-purple">
              <div class="soap">
                <h5 class="title-consulta">Consulta | {{moment(formHistorico.dataCadastro).format('DD/MM/YYYY HH:mm')}}</h5>

                <div style="width:90%; margin-bottom:30px">
                  <div>
                    <p style="font-size: 18px;">Sinais Vitais | Data de envio: {{moment(sinaisVitaisHistorico.dataAlteracao).format('DD/MM/YYYY')}}</p>
                    <p>
                      <strong>Pressao Arterial:</strong> {{sinaisVitaisHistorico.pressaoSanguinea == undefined || sinaisVitaisHistorico.pressaoSanguinea == null ? 'Dados não informados' : sinaisVitaisHistorico.pressaoSanguinea + " mmHg"}}
                    </p>
                    <p>
                      <strong>Frequência Cardíaca:</strong> {{sinaisVitaisHistorico.batimentoCardiaco == undefined || sinaisVitaisHistorico.batimentoCardiaco == null ? 'Dados não informados' : sinaisVitaisHistorico.batimentoCardiaco + " bpm"}}
                    </p>
                    <p>
                    <p>
                      <strong>Saturação O²:</strong> {{sinaisVitaisHistorico.oxigenacaoSanguinea == undefined || sinaisVitaisHistorico.oxigenacaoSanguinea == null ? 'Dados não informados' : sinaisVitaisHistorico.oxigenacaoSanguinea + "%"}}
                    </p>
                    <p>
                      <strong>Temperatura:</strong> {{sinaisVitaisHistorico.temperatura == undefined || sinaisVitaisHistorico.temperatura == null ? 'Dados não informados' : sinaisVitaisHistorico.temperatura + " °C"}}
                    </p>
                    <p>
                      <strong>Altura:</strong> {{sinaisVitaisHistorico.altura == undefined || sinaisVitaisHistorico.altura == null ? 'Dados não informados' : sinaisVitaisHistorico.altura + " cm"}}
                    </p>
                    <p>
                      <strong>Peso:</strong> {{sinaisVitaisHistorico.peso == undefined || sinaisVitaisHistorico.peso == null ? 'Dados não informados' : sinaisVitaisHistorico.peso + " kg"}}
                    </p>
                    <p>
                      <strong>IMC:</strong> {{sinaisVitaisHistorico.dadosImc == undefined || sinaisVitaisHistorico.dadosImc == null ? 'Dados não informados' : sinaisVitaisHistorico.dadosImc}}
                    </p>
                    <p>
                      <strong>Grau IMC:</strong> {{sinaisVitaisHistorico.dadosImc == undefined || sinaisVitaisHistorico.dadosImc == null ? 'Dados não informados' : getGrauImc(sinaisVitaisHistorico.dadosImc)}}
                    </p>
                  </div>
                </div>
                <p style="font-size: 18px;">Dados Do Atendimento</p>
                <p>
                  <strong>Objetivo:</strong> {{formHistorico.objetivo == null ? 'Dados não informados' : formHistorico.objetivo}}
                </p>
                <p>
                  <strong>Subjetivo:</strong> {{formHistorico.subjetivo == null ? 'Dados não informados' : formHistorico.subjetivo}}
                </p>
                <p>
                  <strong>Avaliação:</strong> {{formHistorico.avaliacao == null ? 'Dados não informados' : formHistorico.avaliacao}}
                </p>
                <p>
                  <strong>Plano:</strong> {{formHistorico.plano == null ? 'Dados não informados' : formHistorico.plano}}
                </p>
                <div class="container-cid-ciap">
                  <p><strong>CID10</strong></p>
                  <div v-if="!formCid.length">Dados não informados</div>
                  <ul>
                    <li v-for="value in formCid">
                      - {{ value.descricao }}
                    </li>
                  </ul>
                  <p><strong>CIAP</strong></p>
                  <div v-if="!formCiap.length">Dados não informados</div>
                  <ul>
                    <li v-for="value in formCiap">
                      - {{ value.descricao }}
                    </li>
                  </ul>
                </div>
              </div>
            </div>
          </el-col>
          <el-col :span="12">
            <div class="grid-content bg-purple-light">
              <h5 class="title-consulta">Dados do Profissional</h5>
              <h5 class="title-consulta"><strong>Profissional:</strong>{{formProfissional.nome == null ? '' : formProfissional.nome}}</h5>
              <p>{{formProcedimento.descricao == null ? '' : formProcedimento.descricao}}</p>
            </div>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="24">
            <div>
              <!--Infos Adicionais:-->
              <p><strong>Informações Adicionais</strong></p>
              <p style="font-size: 14px;">
                Alergias: {{(formHistorico.alergias == null || formHistorico.alergias ==  "" || formHistorico.alergias == undefined) ? 'Dados não informados' : formHistorico.alergias}}
              </p>
              <p style="font-size: 14px;">
                Antecedentes: {{(formHistorico.antecedentes == null || formHistorico.antecedentes  == "" || formHistorico.antecedentes == undefined) ? 'Dados não informados' : formHistorico.antecedentes }}
              </p>
            </div>
          </el-col>
        </el-row>
        <el-row>
          <el-col :span="24">
            <div class="container-como-me-sinto">
              <div v-if="!api.comoMeSintoHistorico.length">
                <p>Dados não informados</p>
              </div>
              <div v-for="item in api.comoMeSintoHistorico">
                <div v-if="item.temperatura ||
                     item.tosse ||
                     item.coriza ||
                     item.dorCorpo ||
                     item.dorAbdominal ||
                     item.fraqueza ||
                     item.dorGarganta ||
                     item.nauseaVomito ||
                     item.dorCabeca ||
                     item.sairCasa ||
                     item.contatoPessoas ||
                     item.dificuldadeRespirar ||
                     item.taquicardia ||
                     item.perdaOlfatoPaladar ||
                     item.diarreia ||
                     item.temperaturaRetornou ||
                     item.atendidoServicoSaude ||
                     item.sintomasGripais">
                  <p>Sintomas - Atendimento Anterior - Data de envio: {{moment(item.data).format('DD/MM/YYYY HH:mm')}}</p>
                  <ul>
                    <li v-if="item.temperatura"><span>Temperatura: <el-tag size="small" type="success">Sim</el-tag></span></li>
                    <li v-if="item.tosse"><span>Tosse: <el-tag size="small" type="success">Sim</el-tag></span></li>
                    <li v-if="item.coriza"><span>Coriza: <el-tag size="small" type="success">Sim</el-tag></span></li>
                    <li v-if="item.dorCorpo"><span>Dor no corpo: <el-tag size="small" type="success">Sim</el-tag></span></li>
                    <li v-if="item.dorAbdominal"><span>Dor abdominal: <el-tag size="small" type="success">Sim</el-tag></span></li>
                    <li v-if="item.fraqueza"><span>Fraqueza: <el-tag size="small" type="success">Sim</el-tag></span></li>
                    <li v-if="item.dorGarganta"><span>Dor de garganta: <el-tag size="small" type="success">Sim</el-tag></span></li>
                    <li v-if="item.nauseaVomito"><span>Náusea/Vômito: <el-tag size="small" type="success">Sim</el-tag></span></li>
                    <li v-if="item.dorCabeca"><span>Dor de cabeça: <el-tag size="small" type="success">Sim</el-tag></span></li>
                    <li v-if="item.sairCasa"><span>Tem saido de casa: <el-tag size="small" type="success">Sim</el-tag></span></li>
                    <li v-if="item.contatoPessoas"><span>Contato com pessoas: <el-tag size="small" type="success">Sim</el-tag></span></li>
                    <li v-if="item.dificuldadeRespirar"><span>Dificuldade de respirar: <el-tag size="small" type="success">Sim</el-tag></span></li>
                    <li v-if="item.taquicardia"><span>Taquicardia: <el-tag size="small" type="success">Sim</el-tag></span></li>
                    <li v-if="item.perdaOlfatoPaladar"><span>Perda de olfato ou paladar: <el-tag size="small" type="success">Sim</el-tag></span></li>
                    <li v-if="item.diarreia"><span>Diarréia: <el-tag size="small" type="success">Sim</el-tag></span></li>
                    <li v-if="item.temperaturaRetornou"><span>Temperatura retornou: <el-tag size="small" type="success">Sim</el-tag></span></li>
                    <li v-if="item.atendidoServicoSaude"><span>Atendido por serviço de saúde: <el-tag size="small" type="success">Sim</el-tag></span></li>
                    <li v-if="item.sintomasGripais"><span>Sintomas gripais: <el-tag size="small" type="success">Sim</el-tag></span></li>
                  </ul>
                </div>
                <div v-else>
                  <p>Sintomas: Paciente sem sintomas - Data de envio: {{moment(item.data).format('DD/MM/YYYY HH:mm')}}</p>
                </div>
              </div>
            </div>
          </el-col>
        </el-row>
      </VuePerfectScrollbar>
    </modal>
  </div>
</template>

<script>
  import moment from 'moment'
  moment.locale('pt-br')
  import Utils from '../../mixins/Utils'
  import _api from '../../api'
  import _enums from '../../api/Enums'
  import VuePerfectScrollbar from 'vue-perfect-scrollbar'
  export default {
    name: 'HistoricoNoAtendimento',
    components: { VuePerfectScrollbar },
    props: {
      agendamento: {},
    },
    mixins: [Utils],
    data() {
      return {
        
        historicoModal: false,

        agendamentoProps: {},

        //dados dos sinais vitais
        sinaisVitaisHistorico: {},

        //form com todos os dados do atendimento quando clicar em visualizar
        formHistorico: {},

        //form com os dados do profissional quando clicar em visualizar
        formProfissional: {},

        //form comos dados do procedimento quando clicar em visualizar
        formProcedimento: {},

        // dados do cid e ciap no atendimento quando clicar em visualizar
        formCid: [],
        formCiap: [],

        loading: {
          historico: false,
        },

        scrollSettings: {
          maxScrollbarLength: 200
        },

        api: {
          historico: [],
          comoMeSintoHistorico: [],
          acompanhamento: {},
          cid10Historico: [],
          ciapHistorico: [],
        },

        paramsHistorico: {
          skip: 1,
          take: 10,
          sort: 'DataCadastro DESC',
          total: 0
        },
        paramsAcompanhamentoHistorico: {
          skip: 1,
          take: 9999,
          sort: 'a.Data DESC',
          total: 0
        },
        paramsCid10: {
          skip: 1,
          take: 10,
          sort: 'DataCadastro DESC',
          total: 0
        },
        paramsCiap: {
          skip: 1,
          take: 10,
          sort: 'DataCadastro DESC',
          total: 0
        },
      }
    },

    async created() {
      this.agendamentoProps = this.agendamento
      await this.getHistorico()
    },

    methods: {

      // RETORNANDO OS ATENDIMENTOS ANTERIORES DO PACIENTE SELECIONADO
      async getHistorico() {
        this.loading.historico = true

        var myParams = {
          ...this.paramsHistorico, individuoId: this.agendamentoProps.individuoId, atendidoMedico: true
        }

        let { data, paginacao, status } = await _api.atendimentos.get(myParams)
        if (status === 502) this.loading.historico = false
        this.api.historico = (status === 200) ? data : []

        this.paramsHistorico.skip = (status === 200) ? paginacao.skip : 0
        this.paramsHistorico.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.historico = false
      },

      //EXIBINDO O MODAL
      async openModalHistorico(row) {
        this.historicoModal = false
        await this.getSinaisVitaisHistorico(row)
        await this.getComoMeSintoHistorico(row)
        await this.getCid10Historico(row)
        await this.getCiapHistorico(row)
        this.formHistorico = row
        this.formProfissional = row.profissional 
        this.formProcedimento = row.procedimento

        //console.log("this.formHistorico", this.formHistorico)

        if (this.api.cid10Historico.length > 0) {
          this.api.cid10Historico.map(item => this.formCid.push(item))
        }

        if (this.api.ciapHistorico.length > 0) {
          this.api.ciapHistorico.map(item => this.formCiap.push(item))
        }

        this.$modal.show('resumoHistoricoModal')
      },

      //FECHANDO O MODAL
      hideModalHistorico(row) {
        this.sinaisVitaisHistorico = {}
        this.api.comoMeSintoHistorico = []
        this.cid10Historico = []
        this.ciapHistorico = [],
        this.formCid = [],
        this.formCiap = [],
        this.formProfissional = {},
        this.formProcedimento = {},
        this.formHistorico = {},
        this.$modal.hide('resumoHistoricoModal')
      },


      // RETORNANDO SINAIS VITAIS DO AGENDAMENTO
      async getSinaisVitaisHistorico(row) {
        if (row != null) {
          var { dataAlteracao, pressaoSanguinea, batimentoCardiaco, temperatura, oxigenacaoSanguinea, peso, altura } = row.agendamento;
          this.sinaisVitaisHistorico = { dataAlteracao, pressaoSanguinea, batimentoCardiaco, temperatura, oxigenacaoSanguinea, peso, altura };
          var imcDados = (this.sinaisVitaisHistorico.peso / ((this.sinaisVitaisHistorico.altura / 100) * (this.sinaisVitaisHistorico.altura / 100)))
          var imcFloatDados = parseFloat(imcDados).toFixed(1)
          this.sinaisVitaisHistorico.dadosImc = imcFloatDados
        }
      },

      // RETORNANDO OS SINTOMAS(COMO ME SINTO) DO PACIENTE NO HISTORICO
      async getComoMeSintoHistorico(row) {
        if (row != null) {

          var paramsAtendimento = {
            skip: 1,
            take: 10,
            agendamentoId: row.agendamentoId,
            atendidoMedico: false,
            atendidoTriagem: true,
            total: 0
          }

          // Retornando atendimento da triagem
          let { data, status } = await _api.atendimentos.get(paramsAtendimento)
          if (status === 200) {
            //é obrigatorio voltar 1
            if (data.length === 1) {
              let { data: dataAcompanhamento, status: statusAcompanhamento } = await _api.acompanhamento.get({ atendimentoId: data[0].id })
              if (statusAcompanhamento === 200) {
                //se tiver sintomas
                if (dataAcompanhamento != undefined && dataAcompanhamento.length > 0) {
                  this.api.comoMeSintoHistorico = dataAcompanhamento
                } else {
                  //se não tiver sintomas
                  this.api.comoMeSintoHistorico = []
                }
              } 
            } else {
              //caso não volte o atendimento da triagem para retornar o acompanhamento
              this.$swal({
                title: "Erro!",
                text: "Não foi possivel retornar o atendimento da triagem para retornar os sintomas!",
                icon: 'error',
              })
            }
          }
        }
      },

      // RETORNO DO CID10
      async getCid10Historico(row) {
        this.paramsCid10.agendamentoId = row.agendamentoId
        let { data, paginacao, status } = await _api.individuoCid10.get(this.paramsCid10)
        let dataFiltered = data.map(item => {
          return item.cid
        })
        this.api.cid10Historico = (status === 200) ? dataFiltered : []
        this.paramsCid10.skip = (status === 200) ? paginacao.skip : 0
        this.paramsCid10.total = (status === 200) ? paginacao.totalCount : 0
      },

      //RETORNO DO CIAP
      async getCiapHistorico(row) {
        this.paramsCiap.agendamentoId = row.agendamentoId
        let { data, paginacao, status } = await _api.individuoCiap.get(this.paramsCiap)
        let dataFiltered = data.map(item => {
          return item.ciap
        })
        this.api.ciapHistorico = (status === 200) ? dataFiltered : []
        this.paramsCiap.skip = (status === 200) ? paginacao.skip : 0
        this.paramsCiap.total = (status === 200) ? paginacao.totalCount : 0
      },








      //FUNÇÕES AUXILIARES
      // PAGINAÇÃO HISTORICO
      handleSizeChangeHistorico(val) {
        this.paramsHistorico.take = val
        this.getHistorico()
      },
      handleCurrentChangeHistorico(val) {
        this.paramsHistorico.skip = val
        this.getHistorico()
      },

      // ALTERAÇÃO DE COR NA TABLE
      tableRow({ row }) {
        if (row.ativo === true) return 'success-row'
        else if (row.ativo === false) return 'warning-row'
        else return ''
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
      //FIM FUNÇÕES AUXILIARES
    }
  }
</script>

<style>
  .paciente__missing_photo {
    display: flex;
    justify-content: center;
    align-items: center;
    width: 110px;
    height: 110px;
    background-color: #f2f2f2;
    border-radius: 100px;
  }

  .container-como-me-sinto {
    padding-bottom: 40px;
    margin-bottom: 50px;
  }

    .container-como-me-sinto ul {
      padding: 0px;
    }

      .container-como-me-sinto ul li {
        list-style-type: none;
      }

        .container-como-me-sinto ul li span {
          font-size: 14px;
        }

  .soap {
    font-size: 14px;
  }

    .soap li {
      list-style-type: none;
    }

  .container-cid-ciap ul {
    padding: 0;
  }

  .container-cid-ciap p {
    margin-bottom: 0;
  }

  .scroll-area-historico {
    position: relative;
    width: 96%;
    height: 100%;
    overflow-x: hidden;
    margin: 15px;
  }

</style>
