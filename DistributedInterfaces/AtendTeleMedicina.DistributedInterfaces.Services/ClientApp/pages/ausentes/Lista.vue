<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
    <el-card class="box-card box-card--main-card">

      <el-row>
        <el-col :span="14">
          <h2 class="box-card--h2">Pacientes Ausentes</h2>
        </el-col>
      </el-row>

      <el-row v-show="listando">
        <el-col :span="24">
          <FiltroAusentes :loading="loading.agendamentos" :params="params" @emit="onEmitFromFiltro" />
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
            <el-table-column label="Tipo" prop="tipoDaConsulta" width="125" align="center" />
            <el-table-column v-if="isDemandaEspontanea()" label="Prioridade" prop="corStatusTriagem" align="center" sortable>
              <template slot-scope="scope">
                <span v-if="scope.row.corStatusTriagem === 1" style="color:dodgerblue">{{ prioridadeCor(scope.row.corStatusTriagem) }} </span>
                <span v-if="scope.row.corStatusTriagem === 2" style="color:forestgreen">{{ prioridadeCor(scope.row.corStatusTriagem) }} </span>
                <span v-if="scope.row.corStatusTriagem === 3" style="color: #cfcf00">{{ prioridadeCor(scope.row.corStatusTriagem) }} </span>
                <span v-if="scope.row.corStatusTriagem === 4" style="color:orange">{{ prioridadeCor(scope.row.corStatusTriagem) }} </span>
                <span v-if="scope.row.corStatusTriagem === 5" style="color:red">{{ prioridadeCor(scope.row.corStatusTriagem) }} </span>
              </template>
            </el-table-column>
            <el-table-column label="Especialidade" prop="procedimento.descricao" show-overflow-tooltip align="center" />
            <el-table-column label="Paciente" prop="individuo.nomeCompleto" show-overflow-tooltip align="center" />
            <el-table-column label="Data" prop="individuo.dia" align="center">
              <template slot-scope="scope">
                {{moment(scope.row.dia).format('DD/MM/YYYY')}}
              </template>
            </el-table-column>
            <el-table-column label="Hora" prop="individuo.hora" align="center">
              <template slot-scope="scope">
                {{ scope.row.hora.substr(0, 5) }}
              </template>
            </el-table-column>
            <el-table-column label="Profissional" prop="profissional.nome" align="center"/>
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
    /*background: #D5FED3;*/
    background: #FFDCDC;
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
  import FiltroAusentes from '../../components/shared/FiltroAusentes'
  import { mask } from 'vue-the-mask'
  import moment from 'moment'
  export default {
    name: 'Agendamentos',
    mixins: [Utils],
    components: { FiltroAusentes, VuePerfectScrollbar },
    directives: { mask },
    data() {
      return {
        isDisabled: false,
        listando: true,
        isValid: true,
        modo: 'adicionar',
        metodo: 'POST',
        formAgendamento: {},
        ausentes: true,


        formAgendamentoPaciente: {},
        formAgendamentoEspecialidade: {},

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
        },

        loading: {
          agendamentos: false
        },
        params: {
          skip: 1,
          take: 10,
          finalizado: true,
          sort: 'a.Dia DESC, a.Hora DESC',
          total: 0
        },

      }
    },
    async mounted() {
      if (this.$route.params.tipoDaConsulta != null) {
        this.params.tipoDaConsulta = this.$route.params.tipoDaConsulta
      }
      await this.getAgendamentosAusentes()
    },
    methods: {

      setIsCollapse(payLoad) {
        this.$store.dispatch('setIsCollapse', payLoad)
        //console.log('this.$store', this.$store)
      },

      //FILTRO
      onEmitFromFiltro(val) {
        this.params = {
          ...this.params,
          ...val.params,
          skip: 1
        }
        this.listando = true
        this.api.agendamentos = []
        this.getAgendamentosAusentes()
      },


      //PARTE DO MEDICO
      async getAgendamentosAusentes() {
        this.api.agendamentos = []

        if (this.mxHasAccess('Médico')) this.params.profissionalId = this.$auth.user().id

        if (this.params.skip == 0) this.params.skip = 1

        this.loading.agendamentos = true
        let { data, paginacao, status } = await _api.agendamentos.getAusentes(this.params)
        if (status === 502) this.loading.agendamentos = false
        var arrayData = (status === 200) ? data : []
        this.params.skip = (status === 200) ? paginacao.skip : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.agendamentos = false

        arrayData.forEach(array => {
          array.corStatusTriagem = parseInt(array.corStatusTriagem)
          //if (array.cancelado === true || array.naoCompareceu === true) {
          //  this.api.agendamentos.push(array)
          //}
        })
        this.api.agendamentos = arrayData

        console.log("this.api.agendamentos", this.api.agendamentos)
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
        this.getAgendamentosAusentes()
      },
      handleCurrentChange(val) {
        this.params.skip = val
        this.getAgendamentosAusentes()
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
