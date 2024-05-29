<template>
  <el-tabs type="border-card" @tab-click="handleTabClickExamesAtendimento">
    <!--Exames Solicitados e/ou Avaliados-->
    <el-tab-pane label="Exames Solicitados e/ou Avaliados">
      <div>
        <el-empty v-show="api.exames.length === 0" description="Nenhum exame encontrado"></el-empty>
        <el-table v-show="api.exames.length > 0" :data="api.exames" :default-sort="{prop: 'date', order: 'descending'}" ref="table" border style="margin-top: 20px">
          <el-table-column show-overflow-tooltip label="Data do Envio" prop="dataDeEnvio" align="center" sortable>
            <template slot-scope="scope">
              <span>{{ moment(scope.row.dataDeEnvio).format('DD/MM/YYYY HH:mm') }}</span>
            </template>
          </el-table-column>

          <el-table-column show-overflow-tooltip label="Data da Avaliação" prop="avaliado" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.resultado === false ? 'Exame Não Avaliado' : moment(scope.row.avaliado).format('DD/MM/YYYY HH:mm') }}</span>
            </template>
          </el-table-column>

          <el-table-column show-overflow-tooltip label="Nome Do Exame" prop="tipoExameId" align="center">
            <template slot-scope="scope">
              <span>{{ getTipoExames(scope.row.tipoExameId) }}</span>
            </template>
          </el-table-column>

          <el-table-column prop="resultado" label="Resultado / Avaliado" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.resultado == true ? 'SIM' : 'NÃO' }}</span>
            </template>
          </el-table-column>

          <el-table-column prop="deletado" label="Deletado" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.deletado == true ? 'SIM' : 'NÃO' }}</span>
            </template>
          </el-table-column>

          <el-table-column label="Avaliar Exame" width="140">
            <template slot-scope="scope">
              <el-dropdown>
                <el-button type="primary" size="small">Ações <i class="el-icon-arrow-down el-icon--right"></i></el-button>
                <el-dropdown-menu slot="dropdown">
                  <ul>
                    <li @click="editarAvaliar(scope.row)" v-if="scope.row.resultado == true && scope.row.finalizado == false" class="el-dropdown-menu__item"><i class="fas fa-user-edit"></i> Editar</li>
                    <li @click="visualizarAvaliacao(scope.row)" v-if="scope.row.resultado == true && scope.row.avaliado != '0001-01-01T00:00:00'" class="el-dropdown-menu__item"><i class="fas fa-user-edit"></i>Visualizar Avaliação</li>
                    <li @click="menuAvaliar(scope.row)" v-if="scope.row.resultado == false && scope.row.finalizado == false" class="el-dropdown-menu__item"><i class="fas fa-user-edit"></i> Avaliar</li>
                    <li @click="visualizarExames(scope.row)" v-if="scope.row.formato === '.pdf'" class="el-dropdown-menu__item"><i class="far fa-file"></i><span>{{ scope.row.formato === ".pdf" ? "Visualizar PDF" : "Visualizar"}}</span> </li>
                    <li @click="visualizarExames(scope.row)" v-if="scope.row.formato === '.jpeg' || scope.row.formato === '.png' || scope.row.formato === '.jpg'" class="el-dropdown-menu__item"><i class="far fa-file"></i><span>{{ scope.row.formato === ".jpg" || scope.row.formato === ".jpeg" ||scope.row.formato === ".png" ? "Visualizar Imagem" : "Visualizar"}}</span> </li>
                    <li @click="visualizarExames(scope.row)" v-if="scope.row.formato === '.mp3' || scope.row.formato === '.wav'" class="el-dropdown-menu__item"><i class="far fa-file"></i><span>{{ scope.row.formato === ".wav" || scope.row.formato === ".mp3" ? "Visualizar Audio" : "Visualizar"}}</span> </li>
                    <li @click="inativarExames(scope.row)" class="el-dropdown-menu__item text-danger"><i class="fas fa-trash-alt"></i><span>Inativar Exame</span> </li>
                  </ul>
                </el-dropdown-menu>
              </el-dropdown>
            </template>
          </el-table-column>
        </el-table>
        <el-col :span="24">
          <el-pagination @size-change="handleSizeChangeExames"
                         @current-change="handleCurrentChangeExames"
                         :current-page.sync="paramsExames.page"
                         :page-sizes="[5,10,25]"
                         :page-size="5"
                         :total="paramsExames.total"
                         layout="total, sizes, prev, pager, next, jumper">
          </el-pagination>
        </el-col>
      </div>
      <br />
      <!--Menu Dos Exames-->
      <el-col>
        <!--DIV DE AVALIAR-->
        <div style="margin-top: 20px" v-show="statusAvaliar">
          <el-form :model="formAvaliar" ref="tableAvaliar" label-width="170px" label-position="top">
            <div class="divButtonExame">
              <el-button type="danger" size="small" @click="hideAvaliar" icon="el-icon-close"></el-button>
            </div>
            <div>
              <h2 style="padding-bottom: 5px" class="box-card--h2"> Avaliar</h2>
            </div>
            <el-row>
              <el-form-item label="Descrição" prop="descricao">
                <el-input maxlength="255" type="textarea" v-model="formAvaliar.descricao" />
              </el-form-item>
              <el-button @click="onSalvarAvaliacaoExame('Avaliar')" type="primary" size="small" style="margin-bottom: 20px">Salvar</el-button>
            </el-row>
          </el-form>
        </div>
        <!--DIV DE VISUALIZAR-->
        <div style="margin-top: 20px" v-show="statusVisualizar">
          <el-form :model="formVisualizar" ref="formVisualizar" label-width="170px" label-position="top">
            <div class="divButtonExame">
              <el-button type="danger" size="small" @click="hideVisualizar" icon="el-icon-close"></el-button>
            </div>
            <el-row :gutter="20">
              <el-col :sm="24" :md="4" :lg="4" :xl="4" style="margin-bottom: 10px">
                <span class="editar-exame-title">Data Da Avaliação</span>
                <span class="editar-exame-data">{{ moment(formVisualizar.avaliado).format('DD/MM/YYYY HH:mm') }}</span>
              </el-col>
            </el-row>
            <el-row>
              <span class="editar-exame-title">Descrição</span>
              <el-input type="textarea" v-model="formVisualizar.descricao" disabled />
            </el-row>
          </el-form>
        </div>
        <!--DIV DE EDITAR-->
        <div style="margin-top: 20px" v-show="statusEditar">
          <el-form :model="formEditar" ref="formEditar" label-width="170px" label-position="top">
            <div class="divButtonExame">
              <el-button type="danger" size="small" @click="hideEditar" icon="el-icon-close"></el-button>
            </div>
            <el-row :gutter="20">
              <el-col :sm="24" :md="4" :lg="4" :xl="4" style="margin-bottom: 10px">
                <span class="editar-exame-title">Data Da Avaliação</span>
                <span class="editar-exame-data">{{ moment(formEditar.avaliado).format('DD/MM/YYYY HH:mm') }}</span>
              </el-col>
            </el-row>
            <el-row>
              <span class="editar-exame-title">Descrição</span>
              <el-input maxlength="255" type="textarea" v-model="formEditar.descricao" />
              <el-button @click="onSalvarAvaliacaoExame('Editar')" type="primary" size="small" style="margin-top: 10px; margin-bottom: 10px">Salvar</el-button>
            </el-row>
          </el-form>
        </div>
      </el-col>
    </el-tab-pane>

    <!-- Exames ECG -->
    <el-tab-pane label="Exames ECG">
      <div>
        <el-empty v-show="api.examesEcg.length === 0" description="Nenhum ECG encontrado"></el-empty>
        <el-table v-show="api.examesEcg.length > 0"
                :data="api.examesEcg"
                ref="table"
                border style="margin-top: 20px">

          <el-table-column show-overflow-tooltip label="Data do envio" prop="dataCadastro" align="center" sortable>
            <template slot-scope="scope">
              <span>{{ moment(scope.row.dataCadastro).format('DD/MM/YYYY HH:mm') }}</span>
            </template>
          </el-table-column>

          <el-table-column label="Visualizar ECG" width="140">
            <template slot-scope="scope">
                <el-button type="primary" size="small" @click="visualizarECG(scope.row)">
                  <span>Visualizar</span>
                </el-button>
            </template>
          </el-table-column>
        </el-table>

        <!--<el-col :span="24">
          <el-pagination @size-change="handleSizeChangeExamesF200"
                         @current-change="handleCurrentChangeExamesF200"
                         :current-page.sync="paramsExamesF200.page"
                         :page-sizes="[10,25,50]"
                         :page-size="10"
                         :total="paramsExamesF200.total"
                         layout="total, sizes, prev, pager, next, jumper">
          </el-pagination>
        </el-col>-->
      </div>
    </el-tab-pane>

    <el-tab-pane label="Exames F200">
      <div>
        <el-empty v-show="api.examesF200.length === 0" description="Nenhum exame f200 encontrado"></el-empty>
        <el-table v-show="api.examesF200.length > 0"
                  :data="api.examesF200"
                  :default-sort="{prop: 'dataTransferenciaEcoPc', order: 'descending'}"
                  ref="table"
                  border style="margin-top: 20px">

          <el-table-column show-overflow-tooltip label="Data do envio" prop="dataExameEco" align="center" sortable>
            <template slot-scope="scope">
              <span>{{ moment(scope.row.dataExameEco).format('DD/MM/YYYY') }}</span>
            </template>
          </el-table-column>

          <el-table-column show-overflow-tooltip label="Tipo do Exame" prop="tipoExameEco" align="center">
            <template slot-scope="scope">
              <span>{{ (scope.row.tipoExameEco) }}</span>
            </template>
          </el-table-column>

          <el-table-column prop="resultadoExameEco" label="Resultado" align="center">
            <template slot-scope="scope">
              <span>{{ scope.row.resultadoExameEco }}</span>
            </template>
          </el-table-column>

          <el-table-column label="Visualizar exame" width="140">
            <template slot-scope="scope">
              <el-dropdown>
                <el-button type="primary" size="small">
                  Ações <i class="el-icon-arrow-down el-icon--right"></i>
                </el-button>
                <el-dropdown-menu slot="dropdown">
                  <ul class="list-unstyled">
                    <li @click="visualizarExames(scope.row)" v-if="scope.row.formato === '.pdf'" class="el-dropdown-menu__item"><i class="far fa-file"></i><span>{{ scope.row.formato === ".pdf" ? "Visualizar PDF" : "Visualizar"}}</span> </li>
                  </ul>
                </el-dropdown-menu>
              </el-dropdown>
            </template>
          </el-table-column>
        </el-table>

        <el-col :span="24">
          <el-pagination @size-change="handleSizeChangeExamesF200"
                         @current-change="handleCurrentChangeExamesF200"
                         :current-page.sync="paramsExamesF200.page"
                         :page-sizes="[10,25,50]"
                         :page-size="10"
                         :total="paramsExamesF200.total"
                         layout="total, sizes, prev, pager, next, jumper">
          </el-pagination>
        </el-col>
      </div>
    </el-tab-pane>
    <el-tab-pane label="Exames Afinion">
      <el-empty v-show="api.examesAfinion2.length === 0" description="Nenhum exame encontrado"></el-empty>
      <el-table v-show="api.examesAfinion2.length > 0"
                :data="api.examesAfinion2"
                :default-sort="{prop: 'observationDate', order: 'descending'}"
                ref="table"
                border style="margin-top: 20px; margin-bottom:auto; scroll-margin-block:inherit">

        <el-table-column show-overflow-tooltip label="Data do envio" prop="observationDate" align="center" sortable>
          <template slot-scope="scope">
            <span>{{ moment(scope.row.observationDate).format('DD/MM/YYYY') }}</span>
          </template>
        </el-table-column>

        <el-table-column show-overflow-tooltip label="Tipo do Exame" prop="examName" align="center">
          <template slot-scope="scope">
            <span>{{ (scope.row.examName) }}</span>
          </template>
        </el-table-column>

        <el-table-column prop="examValue" label="Resultado" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.examValue }}</span>
          </template>
        </el-table-column>

        <el-table-column label="Visualizar exame" width="140">
          <template slot-scope="scope">
            <el-dropdown>
              <el-button type="primary" size="small">
                Ações <i class="el-icon-arrow-down el-icon--right"></i>
              </el-button>
              <el-dropdown-menu slot="dropdown">
                <ul class="list-unstyled">
                  <li @click="visualizarExames(scope.row)" v-if="scope.row.formato === '.pdf'" class="el-dropdown-menu__item"><i class="far fa-file"></i><span>{{ scope.row.formato === ".pdf" ? "Visualizar PDF" : "Visualizar"}}</span> </li>
                </ul>
              </el-dropdown-menu>
            </el-dropdown>
          </template>
        </el-table-column>
      </el-table>
      <el-col :span="24">
        <el-pagination @size-change="handleSizeChangeExamesAfinion2"
                       @current-change="handleCurrentChangeExamesAfinion2"
                       :current-page.sync="paramsExamesAfinion2.page"
                       :page-sizes="[10,25,50]"
                       :page-size="10"
                       :total="paramsExamesAfinion2.total"
                       layout="total, sizes, prev, pager, next, jumper">
        </el-pagination>
      </el-col>
    </el-tab-pane>
  </el-tabs>
</template>

<script>
  import moment from 'moment'
  moment.locale('pt-br')
  import Utils from '../../mixins/Utils'
  import _api from '../../api'
  import _enums from '../../api/Enums'

  export default {
    name: 'ExamesTabs',
    props: {
      agendamento: {},
    },
    mixins: [Utils],
    data() {
      return {
        agendamentoProps: {},

        //FORMS
        formAvaliar: {},
        formEditar: {},
        formVisualizar: {},

        statusAvaliar: false,
        statusEditar: false,
        statusVisualizar: false,

        api: {
          exames: [],
          examesEcg: [],
          examesF200: [],
          examesAfinion2: [],
        },
        enums: {
          tipoExames: _enums.tipoExames,
        },
        paramsExames: {
          skip: 1,
          take: 5,
          sort: 'DataDeEnvio DESC',
          deletado: false,
          total: 0
        },
        paramsExamesEcg: {
          skip: 1,
          take: 5,
          graficoEcg: 'IS NOT NULL',
          total: 0
        },
        paramsExamesF200: {
          skip: 1,
          take: 5,
          sort: 'DataExameEco ASC',
          total: 0
        },
        paramsExamesAfinion2: {
          skip: 1,
          take: 5,
          sort: 'ObservationDate ASC',
          total: 0
        },
      }
    },

    async created() {
      this.agendamentoProps = this.agendamento
    },
    async mounted() {
      // RETORNO DOS EXAMES PARA O PACIENTE PASSADO VIA PROP
      this.paramsExames.individuoId = this.agendamentoProps.individuoId
      this.paramsExamesEcg.id = this.agendamentoProps.id
      this.paramsExamesF200.idPaciente = this.agendamentoProps.individuoId


      //EXAMES DA AFINION VAI TER ERRO CASO TIRE A OBRIGATORIEDADE DO CPF POIS É IDENTIFICADO PELO CPF
      this.paramsExamesAfinion2.PatientId = this.agendamentoProps.individuo.cpf.replace(/[.-\s]/g, '')
      this.getExamesAfinion2()

      this.getExames()
      this.getExamesF200()
      await this.getExamesEcg()
    },

    methods: {
      // RETORNANDO A LISTA DE EXAMES DO INDIVIDUO DO ATENDIMENTO ATUAL
      async getExames() {
        let { data, status, paginacao } = await _api.exames.getExames(this.paramsExames)
        this.api.exames = (status === 200) ? data : []
        //console.log("this.api.exames", this.api.exames)
        this.paramsExames.skip = (status === 200) ? paginacao.skip : 0
        this.paramsExames.total = (status === 200) ? paginacao.totalCount : 0
      },

      // RETORNANDO A LISTA DE ECG DO INDIVIDUO DO ATENDIMENTO ATUAL
      async getExamesEcg() {
        let { data, status, paginacao } = await _api.agendamentos.get(this.paramsExamesEcg)
        this.api.examesEcg = (status === 200) ? data : []
        console.log('data', data)
        //this.paramsExames.skip = (status === 200) ? paginacao.skip : 0
        //this.paramsExames.total = (status === 200) ? paginacao.totalCount : 0
      },

      // RETORNANDO A LISTA DE EXAMES DA ECO F200 DO ATENDIMENTO ATUAL
      async getExamesF200() {
        let { data, status, paginacao } = await _api.examesF200.getExamesF200(this.paramsExamesF200)
        this.api.examesF200 = (status === 200) ? data : []
        this.paramsExamesF200.skip = (status === 200) ? paginacao.skip : 0
        this.paramsExamesF200.total = (status === 200) ? paginacao.totalCount : 0
      },

      // RETORNANDO A LISTA DE EXAMES DA AFINION
      async getExamesAfinion2() {
        let { data, status, paginacao } = await _api.examesAfinion2.getExamesAfinion2(this.paramsExamesAfinion2)
        this.api.examesAfinion2 = (status === 200) ? data : []
        this.paramsExamesAfinion2.skip = (status === 200) ? paginacao.skip : 0
        this.paramsExamesAfinion2.total = (status === 200) ? paginacao.totalCount : 0
      },

      // RETORNANDO O NOME DOS TIPOS DE EXAMES
      getTipoExames(val) {
        let tipo = this.enums.tipoExames.filter(x => x.value == val)[0]
        if (tipo != null && tipo != undefined && tipo != '') {
          if (tipo.label != null && tipo.label != undefined && tipo.label != '') {
            return tipo.label
          }
        }
      },

      // EVENTO DE CLICK PARA SALVAR A AVALIAÇÃO
      async onSalvarAvaliacaoExame(modo) {
        if (modo === 'Avaliar') {
          this.formAvaliar = { ...this.formAvaliar, profissionalId: this.agendamentoProps.profissionalId, resultado: true}
        } 
        let { data, status } = (modo === 'Avaliar') ? await _api.exames.putAvaliacao(this.formAvaliar) : await _api.exames.putAvaliacao(this.formEditar)
        if (status === 204) {

          if (modo === 'Avaliar') {
            this.statusAvaliar = false
            this.formAvaliar = {}
          } else if (modo === 'Editar') {
            this.statusEditar = false
            this.formEditar = {}
          }
          
          //await this.getExames()
          this.retornarDadosComponentePai()
          this.$swal({
            title: "Sucesso!",
            text: "Avaliação enviada e registrada com sucesso!",
            icon: 'success',
          })
        } else {
          this.$swal({
            title: "Erro!",
            text: "Ocorreu um erro no envio da avaliação!",
            icon: 'error',
          })
        }
      },


      // FUNÇÕES AUXILIARES
      // EVENTO DE CLICK NO HEADER DA TABLE
      async handleTabClickExamesAtendimento(tab, event) {
        if (tab.label === 'Exames Solicitados e/ou Avaliados') {
          await this.getExames()
        }
        if (tab.label === 'Exames F200') {
          //await this.getExamesF200
        }
        if (tab.label === 'Exames Afinion') {
          //await this.getExamesAfinion2
        }
      },

      // EVENTO DE CLICK PARA ABRIR O CAMPO AVALIAR EXAME
      async menuAvaliar(val) {
        this.formAvaliar = val
        this.statusAvaliar = true
      },

      // EVENTO DE CLICK PARA EDITAR AVALIAÇÃO (OBS SOMENTE NO ATENDIMENTO QUE ELE AVALIOU O EXAME, CASO ATENDIMENTO SEJA FINALIZADO NÃO PODE SER MAIS EDITADO)
      async editarAvaliar(val) {
        this.formEditar = val
        this.statusEditar = true
      },

      // EVENTO DE CLICK PARA VISUALIZAR A AVALIAÇÃO DO MEDICO
      async visualizarAvaliacao(val) {
        this.formVisualizar = val
        this.statusVisualizar = true
      },

      // EVENTO DE CLICK PARA FECHAR O CAMPO AVALIAR EXAME
      async hideAvaliar() {
        this.statusEditar = false
        this.statusAvaliar = false
        this.formAvaliar = {}
      },

      // EVENTO DE CLICK PARA FECHAR O CAMPO EDITAR EXAME
      async hideEditar() {
        this.statusVisualizar = false
        this.statusEditar = false
      },

      // EVENTO DE CLICK PARAFECHAR VISUALIZAR EXAME
      async hideVisualizar() {
        this.statusVisualizar = false
      },

      // EVENTO DE CLICK PARA VISUALIZAR OS PDFS
      async visualizarExames(val) {
        var url = val.url
        this.openExames(url, val)
      },

      // EVENTO DE CLICK PARA VISUALIZAR OS ECGS
      async visualizarECG(val) {
        var url = val.graficoEcg
        var win = window.open()
        win.document.write('<div><img src="' + url + '.png"></div>')
      },

      // ABRINDO OS PDFS EM OUTRA TELA
      async openExames(url, obj) {
        if (obj.formato === '.pdf') {
          var win = window.open()
          win.document.write('<iframe src="' + url + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe>')
        }
        if (obj.formato === '.jpeg') {
          var win = window.open()
          win.document.write('<img src="' + url + '" style="width:100%; height:100%;" >')
        }
        if (obj.formato === '.jpg') {
          var win = window.open()
          win.document.write('<img src="' + url + '" style="width:100%; height:100%;" >')
        }
        if (obj.formato === '.png') {
          var win = window.open()
          win.document.write('<img src="' + url + '">')
        }
        if (obj.formato === '.wav') {
          var win = window.open()
          win.document.write(`
              <div style="display: flex; justify-content: center; align-items: center; width: 100%; height: 100%;">
                <audio controls>
                   <source src="${url}" type="audio/wav">
                   <p><a href="${url}"link</a></p>
                </audio>
              </div>
          `)
        }
        if (obj.formato === '.mp3') {
          var win = window.open()
          win.document.write(`
              <div style="display: flex; justify-content: center; align-items: center; width: 100%; height: 100%;">
                <audio controls>
                   <source src="${url}" type="audio/wav">
                   <p><a href="${url}"link</a></p>
                </audio>
              </div>
          `)
        }
      },

      // PAGINAÇÃO DO MENU EXAMES SOLICITADOS
      handleSizeChangeExames(val) {
        this.paramsExames.take = val
        this.getExames()
      },

      // PAGINAÇÃO DO MENU EXAMES SOLICITADOS
      handleCurrentChangeExames(val) {
        this.paramsExames.skip = val
        this.getExames()
      },

      // PAGINAÇÃO DO MENU EXAMES F200
      handleSizeChangeExamesF200(val) {
        this.paramsExamesF200.take = val
        this.getExamesF200()
      },

      // PAGINAÇÃO DO MENU EXAMES F200
      handleCurrentChangeExamesF200(val) {
        // console.log('val skip', val)
        this.paramsExamesF200.skip = val
        this.getExamesF200()
      },

      // PAGINAÇÃO DO MENU EXAMES AFINION 2
      handleSizeChangeExamesAfinion2(val) {
        this.paramsExamesAfinion2.take = val
        this.getExamesAfinion2()
      },

      // PAGINAÇÃO DO MENU EXAMES AFINION 2
      handleCurrentChangeExamesAfinion2(val) {
        this.paramsExamesAfinion2.skip = val
        this.getExamesAfinion2()
      },

      retornarDadosComponentePai() {
        this.$emit('emit', { exames: this.api.exames })
      },

      async inativarExames(row) {
        row.deletado = true
        await _api.exames.putAvaliacao(row)
        await this.getExames()
      }
    }
  }
</script>

<style>
  .divButtonExame {
    display: flex;
    flex-direction: row;
    justify-content: flex-end;
  }
  .editar-exame-data {
    font-weight: bold;
    margin-left: 2px;
    margin-bottom: 20px;
    font-size: 15px
  }
  .editar-exame-title {
    font-weight: bold;
    font-size: 17px;
    margin-bottom: 10px;
  }
</style>
