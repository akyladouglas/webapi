<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
    <el-card class="box-card box-card--main-card">

      <el-row>
        <el-col :span="14" v-if="mxHasAccess('Médico')">
          <h2 class="box-card--h2">Agendamentos</h2>
        </el-col>
        <el-col :span="14" v-if="mxHasAccess('Recepcionista')">
          <h2 class="box-card--h2">Fila de Espera Médica</h2>
        </el-col>
      </el-row>

      <el-row v-show="listando">
        <el-col :span="24">
          <FiltroAgendamentoEsperaMedica :loading="loading.agendamentos" :params="params" @emit="onEmitFromFiltro" />
        </el-col>
      </el-row>
      <el-empty v-show="listando && api.agendamentos.length === 0" description="Nenhum Agendamento Encontrado"></el-empty>
      <el-row v-show="listando && api.agendamentos.length > 0">
        <el-col :span="24">
          <el-table ref="tabela" :data="api.agendamentos"
                    highlight-current-row border
                    :row-class-name="tableRow"
                    v-loading.body="loading.agendamentos"
                    class="table--agendamentos table--row-click">

            <el-table-column label="Paciente" prop="individuo.nomeCompleto" show-overflow-tooltip />
            <el-table-column label="Data" prop="individuo.dia" align="center" width="100">
              <template slot-scope="scope">
                {{moment(scope.row.dia).format('DD/MM/YYYY')}}
              </template>
            </el-table-column>
            <el-table-column label="Hora" prop="individuo.hora" align="center" width="90">
              <template slot-scope="scope">
                {{ scope.row.hora.substr(0, 5) }}
              </template>
            </el-table-column>
            <el-table-column label="Especialidade" prop="procedimento.descricao" show-overflow-tooltip />
            <el-table-column v-if="!mxHasAccess('Médico') || !mxHasAccess('MédicoAD')" label="Profissional" prop="profissional.nome">
              <template slot-scope="scope">
                <span>{{scope.row.profissional ? scope.row.profissional.nome : 'Sem vinculo'}}</span>
              </template>
            </el-table-column>


            <el-table-column label="Tipo" prop="tipoDaConsulta" width="125" v-if="$config.configDB.modulo != 3" />
            <el-table-column v-if="mxHasAccess('Recepcionista') && isDemandaEspontanea()" label="Em Atendimento" prop="emAtendimentoMedico" width="150">
              <template slot-scope="scope">
                {{scope.row.emAtendimentoMedico === true ? 'Sim' : 'Não'}}
              </template>
            </el-table-column>

            <el-table-column prop="corStatusTriagem" width="150" align="center" v-if="$config.configDB.modulo != 3">
              <template slot="header" slot-scope="scope">
                <el-tooltip content="O Grau de risco se deu com base na seleção da triagem" placement="top">
                  <span style="font-size: 16px">Grau de Risco</span>
                </el-tooltip>
              </template>
              <template slot-scope="scope">
                <div style="display: flex; justify-content: center; flex-direction: column">
                  <i class="fas fa-circle" :style="{ color: getCorGrauDeRisco(scope.row) }"></i>
                </div>
              </template>
            </el-table-column>

            <el-table-column v-if="isDemandaEspontanea()" label="Prioridade" prop="corStatusTriagem" width="150" sortable>
              <template slot-scope="scope">
                <span v-if="scope.row.corStatusTriagem === 1" style="color:dodgerblue">{{ prioridadeCor(scope.row.corStatusTriagem) }} </span>
                <span v-if="scope.row.corStatusTriagem === 2" style="color:forestgreen">{{ prioridadeCor(scope.row.corStatusTriagem) }} </span>
                <span v-if="scope.row.corStatusTriagem === 3" style="color: #cfcf00">{{ prioridadeCor(scope.row.corStatusTriagem) }} </span>
                <span v-if="scope.row.corStatusTriagem === 4" style="color:orange">{{ prioridadeCor(scope.row.corStatusTriagem) }} </span>
                <span v-if="scope.row.corStatusTriagem === 5" style="color:red">{{ prioridadeCor(scope.row.corStatusTriagem) }} </span>
              </template>
            </el-table-column>

            <el-table-column label="Retorno" prop="retorno" show-overflow-tooltip width="130" v-if="$config.configDB.modulo != 3">
              <template slot-scope="scope">
                <span>{{scope.row.retorno === true ? 'SIM' : 'NÃO'}}</span>
              </template>
            </el-table-column>
            <el-table-column label="Aguardando" prop="waitingForDoctor" width="130" v-if="$config.configDB.modulo != 3">
              <template slot-scope="scope">

                <div style="display: flex; justify-content: center; flex-direction: column">
                  <i v-if="scope.row.waitingForDoctor == true" class="fas fa-check-circle" :style="{ color: '#c6e5b1' }"></i>
                  <i v-else class="fas fa-times-circle" :style="{ color: 'gray' }"></i>
                </div>

              </template>
            </el-table-column>
            <!--<el-table-column v-if="mxHasAccess('Recepcionista')" header-align="center" width="180px">
    <template slot-scope="scope">
      <el-dropdown>
        <el-button @click="onCancelar(scope.row)" v-if="mxHasAccess('Recepcionista')" type="danger" size="small">
          Cancelar Agendamento
        </el-button>
      </el-dropdown>
    </template>
  </el-table-column>-->
            <el-table-column v-if="mxHasAccess('Médico') || mxHasAccess('MédicoAD')" align="center" width="130">
              <template slot-scope="scope">
                <el-dropdown>
                  <el-button type="primary" size="small">
                    Ações <i class="el-icon-arrow-down el-icon--right"></i>
                  </el-button>
                  <el-dropdown-menu slot="dropdown">
                    <ul class="list-unstyled">

                      <li v-if="mxHasAccess('Médico') || mxHasAccess('MédicoAD')" @click="irParaAtendimento(scope.row, 'atendimento')" class="el-dropdown-menu__item"><i class="fas fa-user-md"></i> Atendimento</li>
                      <li v-if="mxHasAccess('Médico') || mxHasAccess('MédicoAD')" @click="onVisualizarPreviaAtendimento(scope.row)" class="el-dropdown-menu__item"><i class="fas fa-eye"></i> Visualizar</li>

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

    </el-card>
  </el-col>
</template>



<style>
  .el-table .warning-row {
    background: #FFDCDC;
  }

  .el-table .success-row {
    background: #D5FED3;
  }

  .item-comorbidade {
    margin-bottom: 5px;
  }

  ul.item-sintomas li {
    margin-bottom: 5px;
  }
</style>

<script>
  import VuePerfectScrollbar from 'vue-perfect-scrollbar'
  import Utils from '../../mixins/Utils'
  import _api from '../../api'
  import _enums from '../../api/Enums'
  import FiltroAgendamentoEsperaMedica from '../../components/shared/FiltroAgendamentoEsperaMedica'
  import { mask } from 'vue-the-mask'
  import moment from 'moment'
  import Hub from '../../Hub'
  import audioNotificacao from '../../assets/audios/campainha-2.mp3'
  var somNotificao = new Audio(audioNotificacao)

  var _hub = new Hub()

  export default {
    name: 'Agendamentos',
    mixins: [Utils],
    components: { FiltroAgendamentoEsperaMedica, VuePerfectScrollbar },
    directives: { mask },
    data() {
      return {
        isDisabled: false,
        listando: true,
        isValid: true,
        modo: 'adicionar',
        metodo: 'POST',

        erros: [],
        enums: {
          racaOuCor: [
            ..._enums.racaOuCor
          ],
          tipoDaConsulta: [
            ..._enums.tipoDaConsulta
          ],
          sexos: _enums.sexos
        },
        validacoes: {

        },

        // api.agendamento
        api: {
          agendamentos: [],
          usuarios: []
        },

        loading: {
          agendamentos: false
        },
        params: {
          skip: 1,
          take: 10,
          finalizado: false,
          sort: 'a.Dia ASC, a.Hora ASC',
          total: 0
        },

      }
    },
    async created() {
      _hub.connection.start()
        .then(() => {
          console.log('Hub connected Agendamentos')
          _hub.connection.on('ReceivedWaitingForDoctor', async (msg) => {
            console.log('ReceivedWaitingForDoctor')
            const specialtyId = msg.split(':')[1];

            console.log('Mensagem: ', msg);
            console.log('Especialidade da mensagem: ', specialtyId);
            console.log('Especialidade do médico: ', this.$auth.user().procedimentoId);

            console.log('Inclui sinal: ', msg.includes('sinal'));

            if (msg.includes('sinal') && specialtyId == this.$auth.user().procedimentoId) {
              somNotificao.play()
              console.log('ReceivedWaitingForDoctor sinal')
              await this.getAgendamentos()
              return
            }

            var [status, agendamentoId] = msg.split(":");

            this.api.agendamentos.forEach(agendamento => {

              if (agendamento.id === agendamentoId && status === 'open') {
                this.$set(agendamento, 'waitingForDoctor', true)
              } else {
                this.$set(agendamento, 'waitingForDoctor', false)
              }

            })

          })

          _hub.connection.on('ReceivedRefreshTable', async () => {
            console.log('ReceivedRefreshTable hub')
            await this.getAgendamentos()
            return
          })

        }).catch(e => {
          console.log('Error connection to Hub', e)
        })
    },

    async mounted() {
      if (this.mxHasAccess('Médico') || this.mxHasAccess('MédicoAD')) {
        setTimeout(async () => { await this.setIsCollapse(false) }, 800)
        if (this.$config.configDB.modulo === 3) await this.getProcedimentosDoProfissional()
        else await this.getAgendamentos();
        //this.intervalAgendamentos()
      } else if (this.mxHasAccess('Recepcionista')) {
        await this.getUsuario()
        await this.getAgendamentosRecepcao()
        //this.intervalAgendamentosMedicos()
      } else {
        if (this.$config.configDB.modulo === 3) await this.getProcedimentosDoProfissional()
        else await this.getAgendamentos()
      }
    },

    methods: {
      //METODO DE RETORNO DOS PROCEDIMENTOS DO PROFISSIONAL PARA O FILTRO E PARA O $AUTH
      async getProcedimentosDoProfissional() {
        
        var params = {
          skip: 1,
          dataInicial: this.moment().startOf('day').format('YYYY-MM-DD HH:mm:ss'),
          profissionalId: this.$auth.user().id
        }

        let { data, status } = await _api.estabelecimentosProcedimentosHorarios.get(params)

        //percorrendo o array e retornando o procedimento para o getAgendamentos ser filtrado com o procedimento do profissional
        if (status === 200) {
          this.params.procedimentoId = data[0].procedimentoId
          this.$auth.user().procedimentoId = data[0].procedimentoId
          await this.getAgendamentos()
        }
      },

      setIsCollapse(payLoad) {
        this.$store.dispatch('setIsCollapse', payLoad)
        //console.log('this.$store', this.$store)
      },
      renderIcon(status) {
        return status ? '<i class="el-icon-check"></i>' : '<i class="el-icon-check"></i>'
      },

      //FILTRO
      onEmitFromFiltro(val) {
        this.params = {
          ...this.params,
          ...val.params,
          skip: 1
        }
        this.listando = true
        this.getAgendamentos()
      },


      //PARTE DO MEDICO
      async getAgendamentos() {

        // Aguarda até que this.$store.state.user.selectRole tenha algum dado
        while (!this.$store.state.user.selectRole) {
          // Pausa a execução do código por 1 segundo antes de verificar novamente
          await new Promise(resolve => setTimeout(resolve, 100));
        }

        if (this.$store.state.user.selectRole == 'Médico' || this.$store.state.user.selectRole == 'MédicoAD') {
          //se não for demanda espontanea
          if(this.$config.configDB.modulo !== 3) this.params.profissionalId = this.$auth.user().id

          if (this.$route.params.dashboard == true && this.$route.params.tipoDaConsulta == undefined ||
            this.$route.params.dashboard == true && this.$route.params.tipoDaConsulta != undefined) {
            this.params.dataInicial = new Date()
            this.params.dataFinal = new Date()
          }
          this.params.dataInicial = new Date()
          this.params.dataFinal = new Date()
        }


        if (this.$route.params.tipoDaConsulta != null) {
          this.params.tipoDaConsulta = this.$route.params.tipoDaConsulta
          this.params.dataInicial = new Date()
          this.params.dataFinal = new Date()
        }



        if (this.params.skip == 0) this.params.skip = 1

        this.params = {
          ...this.params,
          cancelado: false,
          presencaConfirmada: true,
          finalizado: false,
        }
        
        this.loading.agendamentos = true
        //console.log('this.params', this.params)

        this.api.agendamentos = []

        let { data, paginacao, status } = await _api.agendamentos.get(this.params)
        if (status === 502) {
          return this.$swal({
            title: 'Erro!',
            text: 'Falha ao carregar os agendamentos!',
            icon: 'error'
          })
          this.loading.agendamentos = false
        }

        if(status === 204) {
          this.loading.agendamentos = false
          return this.$swal({
            title: 'Atenção!',
            timer: 2000,
            timerProgressBar: true,
            text: 'Nenhum agendamento encontrado!',
            icon: 'warning'
          })
        }
        if (status === 200) {

          //data vai ser os agendamentos
          var filtro = data.filter(agendamento => {
            //SÓ VAI FUNCIONAR SE O PROFISSIONAL TIVER UM UNICO PROCEDIMENTO (ESPECIALIDADE)
            if (!this.$auth.user().procedimentoId) this.$auth.user().procedimentoId = agendamento.procedimentoId
            if (!agendamento.profissionalId || agendamento.profissionalId == this.$auth.user().id) return agendamento
          })

          console.log('this.$auth.user()', this.$auth.user())

          this.api.agendamentos = filtro
          this.params.skip = (status === 200) ? paginacao.skip : 0
          this.params.total = (status === 200) ? paginacao.totalCount : 0
          this.loading.agendamentos = false
        }
      },

      prioridadeCor(val) {
        if (val === 1) {
          return 'Não Urgente'
        }
        if (val === 2) {
          return 'Pouco Urgente'
        }
        if (val === 3) {
          return 'Urgente'
        }
        if (val === 4) {
          return 'Muito Urgente'
        }
        if (val === 5) {
          return 'Emergência'
        }
      },
      getRacaCor(val) {
        if (val === 1) {
          return 'Branco'
        }
        if (val === 2) {
          return 'Negro'
        }
        if (val === 3) {
          return 'Amarelo'
        }
        if (val === 4) {
          return 'Pardo'
        }
        if (val === 5) {
          return 'Indigena'
        }
        if (val === 6) {
          return 'Sem Informação'
        }
      },
      getIdade(val) {
        var data = moment(val).format("DD/MM/YYYY")
        var idade = moment().diff(val, 'years')
        return idade
      },

      handleSizeChange(val) {
        this.params.take = val
        this.getAgendamentos()
      },
      handleCurrentChange(val) {
        this.params.skip = val
        this.getAgendamentos()
      },



      //CLICK DO BOTÃO PARA UMA PREVIEW DOS DADOS DO PACIENTE NO ATENDIMENTO
      async onVisualizarPreviaAtendimento(row) {
        this.$router.push({
          name: 'Atendimento',
          params: { agendamento: row, preVisualizar: true }
        })
      },

      //CLICK DO BOTÃO PARA IR PARA ATENDIMENTO
      async irParaAtendimento(row) {
        let form = {
          id: row.id,
          emAtendimentoMedico: true,
          profissionalId: this.$auth.user().id
        }
        let { status } = await _api.agendamentos.putEmAtendimentoMedico(form)
        if (status === 204) {

          await _hub.connection.invoke('SendRefreshTable')
          await _hub.connection.stop()

          this.$router.push({
            name: 'Atendimento',
            params: { agendamento: row }
          })
        } else {
          this.$swal({            
            title: "Erro!",
            text: "Falha ao registrar o vinculo agendamento para o médico!",
            icon: "error"
            })
        }
      },



      //INTERVALO PARA ATUALIZAR A TABLE DE EM AGENDAMENTO MÉDICO DA RECEPCAO
      async intervalAgendamentos() {
        setInterval(async () => { await this.getAgendamentos() }, 30000)
      },

      //INTERVALO PARA ATUALIZAR A TABLE DE EM AGENDAMENTO MÉDICO
      async intervalAgendamentosMedicos() {
        setInterval(async () => { await this.getAgendamentosRecepcao() }, 30000)
      },


      // PARTE DA RECEPCAO

      async getUsuario() {
        let { data, status } = await _api.usuarios.getById(this.$auth.user().id)
        this.api.usuarios = (status === 200) ? data : []
      },

      //metodo para recepcao ver os agendamentos do medico e ver qual ele esta atendendo no momento
      async getAgendamentosRecepcao() {

        if (this.$route.params.dashboard == true && this.$route.params.tipoDaConsulta == undefined ||
          this.$route.params.dashboard == true && this.$route.params.tipoDaConsulta != undefined) {
          this.params.dataInicial = new Date()
          this.params.dataFinal = new Date()
          this.params.tipoDaConsulta = this.$route.params.tipoDaConsulta
        }


        this.params.dataInicial = new Date()
        this.params.dataFinal = new Date()
        this.params.cancelado = false
        this.params.presencaConfirmada = true
        this.params.finalizado != true
        this.params.estabelecimentoId = this.api.usuarios.estabelecimentoId
        this.loading.agendamentos = true
        let { data, paginacao, status } = await _api.agendamentos.get(this.params)
        if (status === 502) this.loading.agendamentos = false
        this.api.agendamentos = (status === 200) ? data : []
        this.params.skip = (status === 200) ? paginacao.skip : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.agendamentos = false
        this.api.agendamentos.forEach(array => {
          array.corStatusTriagem = parseInt(array.corStatusTriagem)
        })
      },
      tableRow({ row }) {
        if (this.demandaEspontanea === true) {
          if (row.emAtendimentoMedico === true) {
            return 'success-row';
          } else if (row.emAtendimentoMedico === false) {
            return 'warning-row';
          }
          return '';
        } else { }

      },

      async onCancelar(val) {
        this.$confirm(`Deseja cancelar o agendamento do paciente ${val.individuo.nomeCompleto}?`, 'Warning', {
          confirmButtonText: 'Sim',
          cancelButtonText: 'Cancelar',
          type: 'warning'
        }).then(() => {
          this.cancelarPresenca(val)
        }).catch(() => { });
      },

      cancelarPresenca(val) {
        this.loading.agendamentos = true
        if (val != {}) {
          let formCancelado = {}
          formCancelado.id = val.id
          formCancelado.cancelado = true
          formCancelado.naoCompareceu = false
          formCancelado.finalizado = true
          _api.agendamentos.putTriagem(formCancelado).then(res => {

            if (res.status === 204) {
              this.loading.agendamentos = false
            }
            this.getAgendamentos()
          })
        } else {

          this.$swal({
            title: "Atenção!",
            text: 'Preencha todos os campos obrigatórios!',
            icon: 'warning',
          })

          this.loading.agendamentos = false
          return false
        }
      },

      getCorGrauDeRisco(row) {
        if (row.corStatusTriagem != undefined && row.corStatusTriagem == 1) {
          return "#1E90FF"
        } else if (row.corStatusTriagem != undefined && row.corStatusTriagem == 2) {
          return "#2a9d8f"
        } else if (row.corStatusTriagem != undefined && row.corStatusTriagem == 3) {
          return "#ffca3a"
        } else if (row.corStatusTriagem != undefined && row.corStatusTriagem == 4) {
          return "#e76f51"
        } else if (row.corStatusTriagem != undefined && row.corStatusTriagem == 5) {
          return "#e63946"
        } else {
          return "#808080"
        }
      },
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

  i {
    min-width: auto !important
  }
</style>
