<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
    <el-card class="box-card box-card--main-card">
      <el-row>
        <el-col :span="14">
          <h2 class="box-card--h2">Interconsulta</h2>
        </el-col>
        <el-col :span="10" class="text-right">
          <el-form :inline="true">
            <el-form-item>
              <el-button style="margin-right: -10px" type="success" @click="onClickAgendar"> Agendar Interconsulta</el-button>
            </el-form-item>
          </el-form>
        </el-col>
      </el-row>

      <!--<el-row>
        <el-col :span="24">
          <FiltroProfissional :loading="loading.profissionais" :params="params" @emit="onEmitFromFiltro" />
        </el-col>
      </el-row>-->
      <el-empty v-show="api.interconsulta.length === 0" description="Nenhuma interconsulta encontrada"></el-empty>
      <el-row v-show="api.interconsulta.length > 0">
        <el-col :span="24">
          <el-table ref="tabela" :data="api.interconsulta"
                    highlight-current-row border
                    v-loading.body="loading.interconsultas"
                    class="table--profissionais table--row-click">
            <el-table-column label="Paciente" prop="agendamento.individuo.nomeCompleto" />
            <el-table-column label="Motivo da Consulta" prop="agendamento.motivo">
              <template slot-scope="scope">
                <span><strong>{{scope.row.agendamento.motivo}}</strong></span>
              </template>
            </el-table-column>

            <el-table-column label="Profissional Anfitrião">
              <template slot-scope="scope">
                <span>{{returnNameProfissional(scope.row.agendamento.profissionalId)}}</span>
              </template>
            </el-table-column>

            <el-table-column label="Data" prop="agendamento.dia" width="120">
              <template slot-scope="scope">
                <span>{{ moment(scope.row.agendamento.dia).format('DD/MM/YYYY') }}</span>
              </template>
            </el-table-column>
            <el-table-column label="Hora" prop="agendamento.hora" width="120">
              <template slot-scope="scope">
                <span>{{ scope.row.agendamento.hora.substr(0, 5) }}</span>
              </template>
            </el-table-column>
            <el-table-column label="Paciente Presente" prop="agendamento.pacientePresente" width="150" align="center">
              <template slot-scope="scope">
                <span v-if="scope.row.agendamento.pacientePresente == true" style="color: green"><strong>SIM</strong></span>
                <span v-else style="color: red"><strong>NÃO</strong></span>
              </template>
            </el-table-column>

            <el-table-column label="Confirmar ou Negar Presença" header-align="center" align="center" width="220px">
              <template slot-scope="scope">

                <!-- se não tiver scope.row.aceitou quer dizer que ele não respondeu se vai participar ou não-->
                <div v-if="scope.row.aceitou === undefined">
                  <el-button type="default" size="small" @click="onClickButton(scope.row, true)" style="padding: 5px"><i class="fa fa-check-square" style="color: #00ff00;" /></el-button>
                  <el-button type="default" size="small" @click="onClickButton(scope.row, false)" style="padding: 5px"><i class="fa fa-window-close" style="color: #ff0000;" /></el-button>
                </div>

                <!-- v-else é sé ele ja tiver respondido com sim ou não -->
                <div v-if="scope.row.aceitou != undefined">
                  <div v-if="scope.row.aceitou == true" style="display: flex; flex-direction: row; align-items: center; justify-content: center">
                    <span>Presença confirmada</span><i class="fa fa-check-square" style="color: #00ff00;" />
                  </div>

                  <div v-if="scope.row.aceitou == false" style="display: flex; flex-direction: row; align-items: center; justify-content: center">
                    <span>Presença não confirmada</span><i class="fa fa-window-close" style="color: #ff0000;" />
                  </div>
                </div>

              </template>
            </el-table-column>

            <el-table-column label="Status" header-align="center" align="center">
              <template slot-scope="scope">

                <!-- status da presença -->
                <el-button size="small" type="success" @click="onClickEntrar(scope.row)" v-if="scope.row.aceitou == true && scope.row.avaliacao == undefined"> Entrar na interconsulta </el-button>
                <el-tag type="danger" effect="dark" v-if="scope.row.aceitou == false && scope.row.avaliacao == undefined"> Negou a presença</el-tag>
                <el-tag type="info" effect="dark" v-if="scope.row.aceitou === undefined && scope.row.avaliacao == undefined"> Verifique sua presença</el-tag>

                <!-- finalizada a interconsulta-->
                <el-tag type="info" effect="dark" v-if="scope.row.avaliacao != undefined"> Interconsulta Finalizada</el-tag>
              </template>
            </el-table-column>
          </el-table>
        </el-col>

        <el-col :span="24">
          <el-pagination @size-change="handleSizeChange"
                         @current-change="handleCurrentChange"
                         :current-page.sync="params.page"
                         :page-sizes="[10,25,50]"
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
</style>

<script>
  import Utils from '../../mixins/Utils'
  import _api from '../../api'
  import _enums from '../../api/Enums'
  import { mask } from 'vue-the-mask'
  import moment from 'moment'
  moment.locale('pt-br')
  export default {
    name: 'Interconsulta',
    mixins: [Utils],
    components: { },
    directives: { mask },
    data() {
      return {
        api: {
          interconsulta: [],
          profissionais: [],
        },
        loading: {
          interconsultas: false,
        },
        params: {
          skip: 1,
          take: 10,
          sort: 'a.Dia DESC',
          total: 0,
          interconsulta: true,
          dataInicial: moment().format('YYYY-MM-DD HH:mm:ss'),
          profissionalId: this.$auth.user().id,
        },

        paramsProfissionais: {
          skip: 1,
          take: 10000,
          sort: 'Nome ASC',
          total: 0
        }
      }
    },
    async mounted() {
      await this.getProfissionais()
      await this.getInterconsulta()
    },
    methods: {
      async getInterconsulta() {
        this.loading.interconsulta = true

        let { data, paginacao, status } = await _api.interconsultas.get(this.params)
        this.api.interconsulta = (status === 200) ? data : []
        this.params.skip = (status === 200) ? paginacao.skip : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0

        this.loading.interconsulta = false
      },

      async onClickButton(row, status) {
        row.aceitou = status
        let { data, status: statusPut } = await _api.interconsultas.aceiteInterconsulta(row)
        if (statusPut === 200) await this.getInterconsulta()
      },

      async onClickEntrar(row) {
        // retornando o agendamento completo
        let { data, status } = await _api.agendamentos.getById(row.agendamento.id)
        if (status === 200) {

          // comparando o row.profissionalId com o id do profissional retornado pelo getById
          if (row.profissionalId == row.agendamento.profissionalId) {
            this.$router.push({
              name: 'Atendimento',
              params: { agendamento: data, participante: 'Anfitrião' }
            })
          }
          else {
            this.$router.push({
              name: 'Atendimento',
              params: { agendamento: data, participante: 'Convidado' }
            })
          }

        }
        
      },

      handleSizeChange(val) {
        this.params.take = val
        this.getInterconsulta()
      },

      handleCurrentChange(val) {
        this.params.skip = val
        this.getInterconsulta()
      },

      onClickAgendar() {
        this.$router.push({ name: 'AgendarInterconsulta' })
      },

      returnNameProfissional(id) {
        let profissional = this.api.profissionais.find(x => x.id == id)
        if (profissional) return profissional.nome
        else return ''
      },


      async getProfissionais() {
        let { data, paginacao, status } = await _api.profissionais.get(this.paramsProfissionais)
        this.api.profissionais = (status === 200) ? data : []
      },
    }
  }
</script>
