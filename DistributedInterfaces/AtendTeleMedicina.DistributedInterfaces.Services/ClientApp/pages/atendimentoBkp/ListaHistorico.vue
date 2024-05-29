<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
    <el-card class="main-card">
      <el-row v-show="listando && api.historico.length > 0">
        <el-button @click="onBackAtendimento(agendamento)" class="float-left" type="warning">
          Voltar2
        </el-button>
        <el-col :span="24">
          <el-table ref="tabela" :data="api.historico"
                    highlight-current-row border
                    v-loading.body="loading.historico"
                    class="table--atendimento table--row-click">
            <el-table-column label="Data da Consulta" prop="dataCadastro" align="center" width="200" fixed />
            <el-table-column label="Profissional" prop="profissional.nome" />

            <el-table-column label="CRM" prop="profissional.crm" />

            <el-table-column header-align="center" align="right" width="140" fixed="right">
              <template slot-scope="scope">
                <el-dropdown>
                  <el-button type="primary" size="small">
                    Ação <i class="el-icon-arrow-down el-icon--right"></i>
                  </el-button>
                  <el-dropdown-menu slot="dropdown">
                    <ul class="list-unstyled">
                      <li @click="onVisualizar(scope.$index, scope.row)" class="el-dropdown-menu__item">
                        <i class="fas fa-clipboard-outline"></i>
                        Visualizar
                      </li>
                    </ul>
                  </el-dropdown-menu>
                </el-dropdown>
              </template>
            </el-table-column>
          </el-table>
        </el-col>


        <el-col :span="24" v-show="listando">
          <el-pagination @size-change="handleSizeChange"
                         @current-change="handleCurrentChange"
                         :current-page.sync="params.page"
                         :page-sizes="[10,25,50,100]"
                         :page-size="params.pageSize"
                         :total="params.total"
                         layout="total, sizes, prev, pager, next, jumper">
          </el-pagination>
        </el-col>
      </el-row>





      <el-row v-show="!listando">

        <el-row :gutter="20" v-if="erros.length > 0">
          <el-col :xs="24">
            <div id="erros" class="alert alert-danger" role="alert">
              <ul>
                <li v-for="erro in erros" :key="erro">{{erro}}</li>
              </ul>
            </div>
          </el-col>
        </el-row>


        <!-- SEGUNDA PAGE DO HISTORICO -->

        <el-row :gutter="24" style="padding-left: 0px; padding-right: 0px;">
          <el-col :span="14">
            <h2 class="box-card--h3">Dados Do Paciente</h2>
          </el-col>
          <el-col :span="10" class="text-right">
            <el-form :inline="true">
              <el-form-item>
                <!--<el-button v-if="listando" style="margin-right: -10px" icon="fas fa-plus" type="success" @click="onClickNovo"> Novo</el-button>-->
                <el-button v-if="!listando" style="margin-right: -60px;" icon="fas fa-undo-alt" type="warning" @click="onVoltar('formAtendimento')">Voltar</el-button>
                <!--De Acordo com michel não ira precisar da opção do medico imprimir o historico do paciente porque o que foi prescrito ja tem historico do mesmo no memed-->
                <!--<el-button v-if="!listando" style="margin-right: -60px;" icon="fas fa-print" type="success" @click="onPrint"> Imprimir</el-button>-->
              </el-form-item>
            </el-form>
          </el-col>
        </el-row>


        <el-form v-if="true" :model="historicoSelecionado" status-icon :disabled="isDisabled"
                 ref="api.historico" label-width="120px" style="line-height: 0 !important;" class="form--agendamento">
          <!--ref="api.historico" label-width="120px" style="line-height: 0 !important;" class="form--agendamento">-->




          <el-row :gutter="20" v-if="dadosGerais">
            <el-col :span="24" class="space-card">
              <el-form :inline="true" :model="dadosGerais" class="demo-form-inline">
                <el-form-item label="Foto:" class="el-form-item__content4">

                </el-form-item>
                <el-form-item label="Nome:" class="el-form-item__content2">
                  <strong>{{dadosGerais.nome}}</strong>
                </el-form-item>
                <el-form-item label="Cpf:" class="el-form-item__content2">
                  <strong>{{dadosGerais.cpf}}</strong>
                </el-form-item>
                <el-form-item label="Sexo:" class="el-form-item__content2">
                  <strong>{{dadosGerais.sexo}}</strong>
                </el-form-item>
                <el-form-item label="Telefone Celular:" class="el-form-item__content2">
                  <strong>{{dadosGerais.telefoneCelular}}</strong>
                </el-form-item>
                <el-form-item label="Email:" class="el-form-item__content2">
                  <strong>{{dadosGerais.email}}</strong>
                </el-form-item>
                <el-form-item label="Idade:" class="el-form-item__content2">
                  <strong>{{dadosGerais.idade}}</strong>
                </el-form-item>
                <el-form-item label="Nome Da Mae:" class="el-form-item__content2">
                  <strong>{{dadosGerais.nomeDaMae}}</strong>
                </el-form-item>
                <el-form-item label="Data Do Nascimento:" class="el-form-item__content2">
                  <strong>{{dadosGerais.dataNascimento}}</strong>
                </el-form-item>
              </el-form>
            </el-col>
          </el-row>

          <el-col :span="14">
            <h2 class="box-card--h3">Dados Biométricos</h2>
          </el-col>
          <el-row :gutter="20" v-if="dadosGerais">
            <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
              <el-form :inline="true" :model="dadosGerais" class="demo-form-inline">

                <el-form-item label="Pressão Arterial:" class="el-form-item__content2">
                  <strong>{{dadosGerais.pressaoArterial}}</strong>
                </el-form-item>

                <el-form-item label="Frequência Cardíaca:" class="el-form-item__content2">
                  <strong>{{dadosGerais.frequenciaCardiaca}} bpm</strong>
                </el-form-item>

                <el-form-item label="Saturação de O2:" class="el-form-item__content2">
                  <strong>{{dadosGerais.saturacaoO2}} %</strong>
                </el-form-item>

                <el-form-item label="Temperatura:" class="el-form-item__content2">
                  <strong>{{dadosGerais.temperatura}} °C</strong>
                </el-form-item>

                <el-form-item label="Altura:" class="el-form-item__content2">
                  <strong>{{dadosGerais.altura}} cm</strong>
                </el-form-item>

                <el-form-item label="Peso:" class="el-form-item__content2">
                  <strong>{{dadosGerais.peso}} kg</strong>
                </el-form-item>

                <el-form-item label="IMC:" class="el-form-item__content2">
                  <strong>{{dadosGerais.dadoImc}} </strong>
                </el-form-item>

                <el-form-item label="Grau IMC:" class="el-form-item__content2">
                  <strong>{{dadosGerais.statusImc}} </strong>
                </el-form-item>
              </el-form>
            </el-col>
          </el-row>

          <div v-else>
            <p>Sinais Vitais</p>
            <p>Não foi registrado nesse atendimento.</p>
          </div>

          <el-row>
            <el-col :span="14">
              <h2 class="box-card--h3">Dados Atendimento</h2>
            </el-col>
          </el-row>

          <el-row class="el-form-item__content3">
            <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
              <div>
                <h3>Objetivo</h3>
                <el-input type="textarea" v-model="historicoSelecionado.objetivo" />
              </div>
            </el-col>
          </el-row>

          <el-row class="el-form-item__content3">
            <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
              <div>
                <h3>Avaliação</h3>
                <el-input type="textarea" v-model="historicoSelecionado.avaliacao" />
              </div>
            </el-col>
          </el-row>

          <el-row class="el-form-item__content3">
            <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
              <div>
                <h3>Plano</h3>
                <el-input type="textarea" v-model="historicoSelecionado.plano" />
              </div>
            </el-col>
          </el-row>

          <el-row class="el-form-item__content3">
            <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
              <div>
                <h3>Subjetivo</h3>
                <el-input type="textarea" v-model="historicoSelecionado.subjetivo" />
              </div>
            </el-col>
          </el-row>

          <el-row class="el-form-item__content3" v-if="api.ciap != null">
            <el-col>
              <el-form-item prop="ciap">
                <h3 style="margin-left: -115px;">Ciap</h3>
                <el-table :data="api.ciap" style="margin-left: -120px; width:100%;">
                  <el-table-column prop="descricao" label="Descrição">
                  </el-table-column>
                </el-table>
              </el-form-item>
            </el-col>
          </el-row>

          <el-row class="el-form-item__content3" v-if="api.cid10 != null">
            <el-col>
              <el-form-item prop="ciap">
                <h3 style="margin-left: -115px;">Cid10</h3>
                <el-table :data="api.cid10" style="margin-left: -120px;">
                  <el-table-column prop="descricao" label="Descrição">
                  </el-table-column>
                </el-table>
              </el-form-item>
            </el-col>
          </el-row>

          <el-row class="el-form-item__content3" v-if="api.documentos != null">
            <el-col>
              <el-form-item prop="documentos">
                <h3 style="margin-left: -115px;">Documentos</h3>
                <el-table :data="api.documentos" style="margin-left: -120px;">
                  <el-table-column prop="url">
                    <template slot-scope="scope">
                      <a :href="scope.row.url">Documento gerado durante o atendimento</a>
                    </template>
                  </el-table-column>
                </el-table>
              </el-form-item>
            </el-col>
          </el-row>
         </el-form>
      </el-row>



    </el-card>
  </el-col>
</template>


<script>
  import Integration from './Integration.vue'
  import Utils from '../../mixins/Utils'
  import jQuery, { data } from 'jquery'
  import _api from '../../api'
  import _enums from '../../api/Enums'
  import FiltroAgendamento from '../../components/shared/FiltroAgendamento'
  import { mask } from 'vue-the-mask'
  import moment from 'moment'
  export default {
    name: 'ListaHistorico',
    mixins: [Utils],
    components: { FiltroAgendamento, Integration},
    directives: { mask },
    props: ['idPaciente'],
    data () {
      return {
        url: null,
        historicoSelecionado: {},
        dados: null,
        isDisabled: false,
        listando: true,
        listando2: true,
        isValid: true,
        modo: 'adicionar',
        metodo: 'POST',
        formAtendimento: {},
        erros: [],
        agendamento: {},
        dadosGerais: {
          pressaoArterial: '',
          frequenciaCardiaca: '',
          temperatura: '',
          saturacaoO2: '',
          peso: '',
          altura: '',
          dadoImc: '',
          statusImc: '',
          nome: '',
          sexo: '',
          cpf: '',
          email: '',
          telefoneCelular: '',
          idade: '',
          dataNascimento: '',
          nomeDaMae: ''
        },

        // api.agendamento
        api: {
          historico: [],
          agendamentos: [],
          ciap: [],
          cid10: [],
          documentos: []
        },

        loading: {
          historico: false,
          ciapCid: false
        },
        params: {
          skip: 1,
          take: 10,
          sort: 'DataCadastro ASC',
          total: 0,
          agendamentoId: ''
        },
        routeParams: {}
      }
    },
    async mounted () {
      this.routeParams = this.$route.params
      this.agendamento = this.routeParams.agendamento

      await this.getHistorico()
      await this.dadosAgendamentos()
      /* await this.dadosRotaAtendimento() */
  },
    methods: {
      // async dadosGerais() {
      //  if (this.dadosGerais.sexo == 0) {
      //    this.sexo = 'Masculino'
      //  } else {
      //    this.sexo = 'Feminino'
      //  }
      // }

      onPrint() {

        window.print("", "", "width=0, height=0");
        
        
        // console.log("PRINT");
      },

      // onEmitFromFiltro(val) {
      //  this.params = {
      //    ...this.params,
      //    ...val.params,
      //    skip: 1
      //  }
      //  this.listando = true
      //  //this.getAgendamentos()

      // },

      async getHistorico () {
        //console.log('agendamento', this.agendamento)
        this.loading.historico = true
        var myParams = {
          ...this.params, individuoId: this.agendamento.individuoId
        }
        let { data, paginacao, status } = await _api.atendimentos.get(myParams)
        // console.log('teste de alguma', this.routeParams.agendamentos)
        //console.log('exibindo data do atendimentos api', data)
        // console.log('exibindo paginacao', paginacao)
        if (status === 502) this.loading.historico = false

        this.api.historico = (status === 200) ? data : []
        this.api.historico.map((item) => {
          item.dataCadastro = moment(item.dataCadastro).format('DD/MM/YYYY hh:mm')
        })
        this.api.historico.map((medico) => {
          medico.profissional.crm = medico.profissional.crm
        })
        this.params.skip = (status === 200) ? paginacao.skip : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        // console.log(this.api.historico);
        this.loading.historico = false
      },

      async dadosAgendamentos () {
        let { data } = await _api.agendamentos.get(this.params)
        this.dados = data
       // console.log('exibindo api agendamentos', this.dados)
      },

      // async dadosRotaAtendimento() {
      //  let { data } = this.routeParams.atendimentos()
      //  this.teste = data
      //  console.log('exibindo data', teste)
      // },

      async getCiap (row) {
        this.loading.ciapCid = true
        this.params.agendamentoId = row.agendamentoId
        // console.log("AGENDAMENTOID", this.params)
        let { data, paginacao, status } = await _api.individuoCiap.get(this.params)
        if (status === 502) this.loading.ciapCid = false

        let dataFiltered = data.map(item => {
          return item.ciap
        })
        // console.log("DATA 2", dataFiltered)
        this.api.ciap = (status === 200) ? dataFiltered : []

        this.params.skip = (status === 200) ? paginacao.skip : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        // console.log("CIAP", this.api.ciap);
        this.loading.ciapCid = false
      },

      async getCid10 (row) {
        this.loading.ciapCid = true
        this.params.agendamentoId = row.agendamentoId
        let { data, paginacao, status } = await _api.individuoCid10.get(this.params)
        if (status === 502) this.loading.ciapCid = false

        let dataFiltered = data.map(item => {
          return item.cid
        })
        this.api.cid10 = (status === 200) ? dataFiltered : []

        this.params.skip = (status === 200) ? paginacao.skip : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.ciapCid = false
      },

      onVisualizar (index, row) {
        this.modo = 'editar'
        this.metodo = 'PUT'
        this.isDisabled = true
        this.listando = false
        this.getCiap(row)
        this.getCid10(row)
        this.getDocumentos(row)
        this.historicoSelecionado = row


        //SINAIS VITAIS
        this.dadosGerais.pressaoArterial = row.agendamento.pressaoSanguinea
        this.dadosGerais.frequenciaCardiaca = row.agendamento.batimentoCardiaco
        this.dadosGerais.temperatura = row.agendamento.temperatura
        this.dadosGerais.saturacaoO2 = row.agendamento.oxigenacaoSanguinea
        this.dadosGerais.peso = row.agendamento.peso
        this.dadosGerais.altura = row.agendamento.altura

        //IMC
        var imc = (row.agendamento.peso / ((row.agendamento.altura / 100) * (row.agendamento.altura / 100)))
        var imcFloat = parseFloat(imc).toFixed(1);
        //console.log('o imc', imcFloat)
        this.dadosGerais.dadoImc = imcFloat

        if (imcFloat >= 16 & imcFloat <= 16.9) {
          this.dadosGerais.statusImc = 'Muito abaixo do Peso'
        }
        if (imcFloat >= 17 & imcFloat <= 18.4) {
          this.dadosGerais.statusImc = 'Abaixo do Peso'
        }
        if (imcFloat >= 18.5 & imcFloat <= 24.9) {
          this.dadosGerais.statusImc = 'Peso Normal'
        }
        if (imcFloat >= 25 & imcFloat <= 29.9) {
          this.dadosGerais.statusImc = 'Acima do Peso'
        }
        if (imcFloat >= 30 & imcFloat <= 34.9) {
          this.dadosGerais.statusImc = 'Obesidade Grau I'
        }
        if (imcFloat >= 35 & imcFloat <= 40) {
          this.dadosGerais.statusImc = 'Obesidade Grau II'
        }
        if (imcFloat > 40) {
          this.dadosGerais.statusImc = 'Obesidade Grau III'
        }

        //DADOS PACIENTE
        this.dadosGerais.nome = row.individuo.nomeCompleto
        var sexo = row.individuo.sexo
        if (sexo == 0) {
          this.dadosGerais.sexo = 'Masculino'
        } else {
          this.dadosGerais.sexo = 'Feminino'
        };
        this.dadosGerais.cpf = row.individuo.cpf
        this.dadosGerais.email = row.individuo.email
        this.dadosGerais.telefoneCelular = row.individuo.telefoneCelular

        var idade = row.individuo.dataNascimento
        this.dadosGerais.idade = moment().diff(idade, 'years')

        this.dadosGerais.dataNascimento = moment(idade, 'YYYY-MM-DD').format('DD/MM/YYYY')

        let mae = row.individuo.nomeDaMae
        if (mae == '' || mae == null) {
          this.dadosGerais.nomeDaMae = 'Não Tem'
        } else {
          this.dadosGerais.nomeDaMae = mae
        };

        this.formAtendimento = {
          ...row
        }
      },
      // resetForm(form) {
      //  this.$refs[form].resetFields()
      // },
      onVoltar (form) {
        let i = this.api.historico.findIndex(x => x.id === this.formAtendimento.id)
        this.$refs.tabela.setCurrentRow(this.api.historico[i])
        // this.$refs[form].resetFields()
        this.listando = true
      },
      async onBackAtendimento (row) {
        this.$router.push({
          name: 'Atendimento',
          params: { agendamentos: row }
        })
      },
      async getDocumentos(row) {
        this.params.agendamentoId = row.agendamentoId
        let { data, paginacao, status } = await _api.documentos.get(this.params)
        this.api.documentos = (status === 200) ? data : []
        //console.log("DOCUMENTOS", data);
      },
      handleSizeChange (val) {
        this.params.pageSize = val
      },
      handleCurrentChange (val) {
        this.params.skip = val
      }
    }
  }
</script>

<style>
  .main-card{
      margin-top: 14px;
   }
  .space-card {
     margin-bottom: 20px;
     /*line-height: 0 !important;*/
   }
  .el-form-item {
     margin-bottom: 0px;
     /*line-height: 0 !important ;*/
   }
  .el-form-item__content {
     margin-right: 50px;
     vertical-align: top;
     display: flow-root;
     /*line-height: 0 !important;*/
   }
  .el-form-item__content2{
     box-sizing: border-box;
     align-items: center;
     border: ridge;
     border-bottom-width: 3px;
     border-top-width: 3px;
     border-right-width: 3px;
     border-left-width: 3px;
     margin-right: 0px;
     margin-bottom: 8px;
     margin-top: 1px;
     margin-left: 0px;
     padding: 5px;
     border-radius: 20px;
     /*line-height: 0 !important;*/
   }
  .el-form-item__content3 {
     border: ridge;
     border-bottom-width: 3px;
     border-top-width: 3px;
     border-right-width: 3px;
     border-left-width: 3px;
     padding: 10px;
     border-radius: 20px;
     /*line-height: 0 !important;*/
     
   }
  .el-form-item__content4 {
    float: left;
    border: ridge;
    border-bottom-width: 3px;
    border-top-width: 3px;
    border-right-width: 3px;
    border-left-width: 3px;
    width: 160px;
    height: 150px;
    margin-bottom: 8px
  }

</style>
