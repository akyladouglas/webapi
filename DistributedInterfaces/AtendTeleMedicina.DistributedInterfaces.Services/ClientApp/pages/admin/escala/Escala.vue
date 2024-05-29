<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">

    <el-card class="box-card box-card--main-card">
      <el-row>
        <el-col :span="14">
          <h2 class="box-card--h2">{{$route.meta.title}}</h2>
        </el-col>
      </el-row>

      <el-row>
        <el-form :model="formData" ref="formData" label-width="170px" label-position="top">
          <!--<el-row :gutter="20">
            <el-col :sm="24" :md="4" :lg="4" :xl="4">
              <el-form-item label="Semana do Dia" prop="dataInicial">
                <el-date-picker v-model="filtroParams.dataInicial" type="date" placeholder="Data Inicial" format="dd/MM/yyyy" />
               </el-form-item>
            </el-col>
            <el-col :sm="24" :md="4" :lg="4" :xl="4">
              <el-form-item class="pt-4">
                <el-button v-if="!loading.horarios" type="info" icon="fas fa-filter" @click="onClickFiltrar('filtroParams')"> Filtrar</el-button>
                <el-button v-if="loading.loading" type="info" icon="fa fa-spinner fa-spin" disabled> Aguarde ...</el-button>
              </el-form-item>
            </el-col>
          </el-row>-->

          <div class="card-header">
            <el-row :gutter="20">

              <el-col :xs="24" :sm="24" :md="24" :lg="8" :xl="5">
                <el-form-item label="Data:" prop="dataInicial">
                  <el-date-picker v-model="filtroParams.dataInicial" type="date" placeholder="Data Inicial" format="dd/MM/yyyy" />
                </el-form-item>
              </el-col>

            </el-row>

          </div>
          <div style="display:flex; justify-content:flex-end">
            <el-row :gutter="20">
              <el-col :sm="24" :md="4" :lg="4" :xl="4">
                <el-form-item class="pt-4">
                  <el-button style="margin-top:-20px" v-if="!loading.horarios" type="info" icon="fas fa-filter" @click="onClickFiltrar('filtroParams')"> Filtrar</el-button>
                  <el-button style="margin-top:-20px"v-if="loading.loading" type="info" icon="fa fa-spinner fa-spin" disabled> Aguarde ...</el-button>
                </el-form-item>
              </el-col>
            </el-row>
          </div>
          

             <el-row>
                <el-col class="escala__celula" :sm="3" :md="3" :lg="3" :xl="3" v-for="(x, i) in semana" :key="i+1">
                  {{x.diaExt}} ({{moment(x.dia).format('DD/MM')}})
                </el-col>
              </el-row>
             <el-row>
               <el-col class="escala__celula" v-for="i in 7" :key="i" :sm="3" :md="3" :lg="3" :xl="3">
                <template v-if="_.get(api.horarios[i-1], '[0]profissional.nome')">
                  <ul class="list-unstyled"  v-for="(j, i) in api.horarios[i-1]" >
                    <li class="escala__titulo">{{j.profissional.nome}}</li>
                    <li class="escala__horario">({{`${j.horaInicial} às ${j.horaFinal}`}})</li>
                  </ul>
                </template>
                <div v-else class="text-center">-</div>
               </el-col>
             </el-row>

          <!--NÂO DESCOMENTAR-->
           <!--<el-row :gutter="20">
            <el-col :span="12" class="formHorarios">
              <table class="table table-bordered">
                <thead>
                  <tr>
                    <th scope="col" class="cel-dia" v-for="(x, i) in semana" :key="i+1">{{x.diaExt}} ({{moment(x.dia).format('DD/MM')}})</th>
                  </tr>
                </thead>
                <tbody>
                  <tr>
                    <th scope="row" v-for="(x, i) in semana" :key="i+1" @click="onClickDia(moment(x.dia.format('YYYY-MM-DD')))" class="cel-data"></th>
                  </tr>
                  <tr>
                    <th scope="row"  class="cel-data"></th>
                    <th scope="row"  class="cel-data">Romulo Loureiro</th>
                    <th scope="row"  class="cel-data">Cauã Victor</th>
                    <th scope="row"  class="cel-data">Priscila Mara</th>
                    <th scope="row"  class="cel-data">Eduardo Vasconcelos</th>
                    <th scope="row"  class="cel-data">Victor Augusto</th>
                  </tr>
                </tbody>
              </table>
            </el-col>
          </el-row>--> 


      </el-form>  
    </el-row>



   </el-card>

  </el-col>
</template>

<script>
  import Utils from '../../../mixins/Utils'
  import _api from '../../../api'
  import _enums from '../../../api/Enums'
  export default {
    name: 'Escala',
    mixins: [Utils],
    data () {
      return {
        formData: {},
        semana: [],
        api: {
          horarios: []
        },
        loading: {
          horarios: false
        },
        enums: {},
        filtroParams: {
          dataInicial: this.moment().startOf('week'),
          dataFinal: this.moment().endOf('week')
        }
      }
    },
    async created () {
      // this.diasDaSemana(this.filtroParams.dataInicial)
      this.onClickFiltrar()
    },
    methods: {
      diasDaSemana (data) {
        for (var i = 0; i <= 6; i++) {
          this.semana.push({
            diaExt: this.moment(data).day(i).format('ddd'),
            dia: this.moment(data).day(i).format('YYYY-MM-DD[T]HH:mm:ss')
          })
        }
      },
      onClickDia (val) {
        //console.log('val', this.moment(val).format('YYYY-MM-DD'))
      },
      async getEstabelecimentos () {
        this.loading.estabelecimentos = true
        let { data, paginacao, status } = await _api.estabelecimentos.get(this.params)
        if (status === 502) this.loading.estabelecimentos = false
        this.api.estabelecimentos = (status === 200) ? data : []
        if (this.api.estabelecimentos.length === 1) this.formData.estabelecimento = this.api.estabelecimentos[0]
        this.params.skip = (status === 200) ? paginacao.skip : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.estabelecimentos = false
      },
      async getProfissionais () {
        this.loading.profissionais = true
        let { data, paginacao, status } = await _api.profissionais.get(this.profParams)
        if (status === 502) this.loading.profissionais = false
        this.api.profissionais = (status === 200) ? data : []
        this.profParams.skip = (status === 200) ? paginacao.skip : 0
        this.profParams.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.profissionais = false
      },
      async onClickFiltrar () {
        this.semana = []
        this.api.horarios = []
        this.diasDaSemana(this.filtroParams.dataInicial)
        let params = {
          dataInicial: this.moment(this.filtroParams.dataInicial).startOf('week').format('YYYY-MM-DD'),
          dataFinal: this.moment(this.filtroParams.dataInicial).endOf('week').format('YYYY-MM-DD')
        }
        let { data } = await _api.profissionais.getProfissionalHorarios(params)
        // let diasAgrupados = this._.groupBy(data, (x) => x.dia)
        this.semana.forEach(x => {
          let diasComAgenda = data.filter(y => y.dia === x.dia)
          if (diasComAgenda.length > 0) {
            this.api.horarios.push(diasComAgenda)
          } else {
            this.api.horarios.push([{
              dia: x.dia
            }])
          }
        })
      //  console.log('allDays', this.api.horarios)
      },
      onVoltar () {
        this.$router.push({ name: 'Estabelecimentos' })
      }
    }
  }
</script>

<style>
.el-calendar-table thead th {
    padding: 12px 0;
    color: #606266;
    font-weight: 400;
    border: 1px solid #e8e8e8;
    padding: 5px;
}

.cel-data {
  cursor: pointer;
  height: 60px;
}

.cel-dia {
  min-width: 150px !important;
}

</style>
