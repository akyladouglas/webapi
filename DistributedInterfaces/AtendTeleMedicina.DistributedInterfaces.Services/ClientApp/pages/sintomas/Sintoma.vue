<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
    <el-card>

      <el-row>
        <el-col :span="14">
          <h2 class="box-card--h2">Histórico de Sintomas</h2>
        </el-col>
        <el-col :span="10" class="text-right">
          <el-button style="margin-right: -10px" icon="fas fa-undo-alt" type="warning" v-if="agendamento === undefined || agendamento === {}" @click="onVoltar()"> Voltar</el-button>
        </el-col>
      </el-row>

      <el-row>
        <el-col :span="24">
          <FiltroSintomas @emit="onEmitFromFiltro" />
        </el-col>
      </el-row>

      <el-empty v-show="api.sintomas.length == 0" description="Nenhum Registro de Sintomas Encontrado"></el-empty>
      <el-row v-show="api.sintomas.length > 0">
        <el-col :span="24">
          <el-table :data="api.sintomas" highlight-current-row v-loading="loading.sintomas">

            <el-table-column prop="data" label="Data de Registro" align="center" width="155">
              <template slot-scope="scope">
                <span>{{moment(scope.row.data).format('DD/MM/YYYY HH:mm')}}</span>
              </template>
            </el-table-column>

            <el-table-column label="Sintomas" align="center">
              <template slot-scope="scope">

                <template v-if="hasSymptoms(scope.row)">
                  <div class="symptoms-container">
                    <div v-for="sintoma in sintomasList" :key="sintoma.prop" class="symptom-item" v-if="scope.row[sintoma.prop]">
                      <el-tag size="small" type="success" v-if="sintoma.prop !== 'outros' ">{{ `${sintoma.label}: Sim` }}</el-tag>
                      <el-tag size="small" type="warning" v-else>Outros: {{ scope.row[sintoma.prop] }}</el-tag>
                    </div>
                  </div>
                </template>

                <template v-else>
                  <div style="width: 40%; display: flex; justify-content: flex-start;">
                    <el-tag size="small" type="warning">Paciente sem sintomas</el-tag>
                  </div>
                </template>

              </template>
            </el-table-column>

          </el-table>
        </el-col>

        <el-col :span="24">
          <el-pagination @size-change="handleSizeChange"
                         @current-change="handleCurrentChange"
                         :current-page.sync="paramsSintomas.page"
                         :page-sizes="[10,25,50,100]"
                         :page-size="paramsSintomas.pageSize"
                         :total="paramsSintomas.total"
                         layout="total, sizes, prev, pager, next, jumper">
          </el-pagination>
        </el-col>
      </el-row>

    </el-card>
  </el-col>
</template>
<script>
  import moment from 'moment'
  moment.locale('pt-br')
  import _api from "../../api"
  import FiltroSintomas from '../../components/shared/FiltroSintomas'
  export default {
    name: "Sintomas",
    components: { FiltroSintomas },
    props: {
      agendamento: {},
    },

    data() {
      return {

        loading: {
          sintomas: false
        },

        api: {
          sintomas: [],
        },

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

        paramsSintomas: {
          skip: 1,
          take: 10,
          sort: 'Data DESC',
          dataInicial: null,
          dataFinal: null,
          total: 0,
        },

      }
    },
    async mounted() {
      //se vier pela rota
      if (this.agendamento == undefined || this.agendamento === {}) {
        if (!this.$route.params.individuoId) {
          this.$router.push({
            name: 'ListaPacientes'
          })
          return
        } else {
          this.individuo = {
            id: this.$route.params.individuoId
          }
        }
      }
      
      //se vier como component
      if (this.agendamento != undefined) {
        if (this.agendamento.individuo != undefined) {
          this.individuo = this.agendamento.individuo
        }
      }

      await this.getSintomas()
    },

    methods: {

      async onVoltar(row) {
        this.$router.push({
          name: 'ListaPacientes',
        })
      },

      hasSymptoms(sintomasObj) {
        for (const prop in sintomasObj) {
          if (sintomasObj[prop] && this.isSymptomKey(prop)) {
            return true;
          }
        }
        return false;
      },

      isSymptomKey(key) {
        // Função auxiliar para verificar se a chave é uma chave de sintoma válida
        return this.sintomasList.some(sintoma => sintoma.prop === key);
      },

      async onEmitFromFiltro(val) {
        console.log('val', val)
        this.paramsSintomas = {
          ...this.paramsSintomas,
          ...val.params,
        }
        console.log('this.paramsSintomas', this.paramsSintomas)
        await this.getSintomas()
      },


      //REQUESTS APIS
      async getSintomas() {
        this.loading.sintomas = true

        this.paramsSintomas = {
          ...this.paramsSintomas,
          individuoId: this.individuo.id,
          atendimentoId: null,
          origem: "web"
        }

        let { data, paginacao } = await _api.acompanhamento.get(this.paramsSintomas)

        if (data.length > 0) {
          this.api.sintomas = data
        } else {
          this.api.sintomas = []
        }

        this.paramsSintomas.skip = (data.length > 0) ? paginacao.skip : 0
        this.paramsSintomas.total = (data.length > 0) ? paginacao.totalCount : 0
        this.loading.sintomas = false
      },

      

      handleSizeChange(val) {
        this.paramsSintomas.take = val
        this.getSintomas()
      },

      handleCurrentChange(val) {
        this.paramsSintomas.skip = val
        this.getSintomas()
      },
    }
  }
</script>

<style scoped>

  .symptoms-container {
    display: flex;
    flex-wrap: wrap;
    justify-content: flex-start;
    gap:5px
  }

  .symptom-item {
    
  }

</style>
