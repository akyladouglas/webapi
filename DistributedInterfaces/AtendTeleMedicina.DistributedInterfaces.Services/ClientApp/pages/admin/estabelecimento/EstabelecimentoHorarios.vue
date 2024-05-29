<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">

    <el-card class="box-card box-card--main-card">
      <el-row>
        <el-col :span="14">
          <h2 class="box-card--h2">{{$route.meta.title}}</h2>
        </el-col>
      </el-row>

      <el-row>
        <el-form :model="formData" :rules="validacoes" ref="formData" label-width="170px">
          <el-row :gutter="20">
            <el-col :span="12" class="formHorarios">
              <el-form-item label="Estabelecimento:" prop="estabelecimento">
                <el-select v-model="formData.estabelecimento"
                           placeholder="Selecione o Estabelecimento"
                           filterable remote
                           :remote-method="searchEstabelecimentos"
                           @change="getEstabelecimentoProcedimentos"
                           v-loading.body="loading.estabelecimentos"
                           value-key="id">
                  <el-option v-for="item in api.estabelecimentos"
                             :key="item.id"
                             :label="item.nomeFantasia"
                             :value="item">
                  </el-option>
                </el-select>
              </el-form-item>
              <el-form-item label="Especialidade:" prop="procedimento">
                <el-select v-model="formData.procedimento"
                           :disabled="api.procedimentos.length === 0"
                           filterable remote clearable
                           placeholder="Selecione o Procedimento"
                           :remote-method="getEstabelecimentoProcedimentos"
                           @change="getProfissionais"
                           value-key="codigo"
                           v-loading.body="loading.procedimentos">
                  <el-option v-for="item in api.procedimentos"
                             :key="item.id"
                             :label="item.procedimento.descricao"
                             :value="item.procedimento.id">
                  </el-option>
                </el-select>
              </el-form-item>
              <el-form-item label="Profissional:" prop="profissional">
                <el-select v-model="formData.profissional"
                           :disabled="api.profissionais.length === 0 || formData.procEstHor.length > 0"
                           filterable remote clearable
                           placeholder="Selecione o Profissional"
                           value-key="id"
                           @change="getEstabelecimentoDatas"
                           v-loading.body="loading.profissionais">
                  <el-option v-for="item in api.profissionais" :key="item.id"
                             :label="item.nome" :value="item.id" />
                </el-select>
              </el-form-item>

              <!-- campinas só tem um tipo da consulta que é totem nao precisa da seleção aqui -->
              <el-form-item  v-if="!isDemandaEspontanea()" label="Tipo da Consulta" prop="tipoDaConsulta">
                <el-select v-model="formData.tipoDaConsulta" placeholder="Selecione...">
                  <el-option v-for="option in enums.tiposDaConsulta" :value="option.value" :label="option.label" :key="option.value" />
                </el-select>
              </el-form-item>

              <el-form-item label="Dia:" prop="dia">
                <el-date-picker v-model="formData.dia"
                                :disabled="formData.procedimento.length < 1"
                                type="date"
                                format="dd-MM-yyyy"
                                clearable
                                disabledDate
                                :picker-options="pickerOptions"
                                placeholder="Selecione o Dia">
                </el-date-picker>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <div v-show="formData.procEstHor.length > 0">
                <h3>Datas e Horários a serem adicionados</h3>
                <el-table border fit :data="formData.procEstHor">
                  <el-table-column type="expand">
                    <template slot-scope="props">
                      <ul class="list-inline pl-2">
                        <li v-for="item in props.row.procedimentoHorarios" :key="item.id">{{ item.hora }}</li>
                      </ul>
                    </template>
                  </el-table-column>
                  <el-table-column label="Data">
                    <template slot-scope="props">
                      {{ moment(props.row.dia, 'YYYY-MM-DD').format('DD-MM-YYYY') }}
                    </template>
                  </el-table-column>
                </el-table>
              </div>
            </el-col>
          </el-row>
          <el-row :gutter="24">
            <el-col :span="24">
              <div v-show="formData.dia">
                <el-form-item label="Intervalo Atendimento:" prop="hora">
                  <template>
                    <el-transfer v-model="formData.hora"
                                 :data="horaDisponivel"
                                 :format="{
                                    noChecked: '${total} horário(s)',
                                    hasChecked: '${checked}/${total} hora(s)'
                                  }"
                                 :titles="['Horários', 'Disponíveis']">
                    </el-transfer>
                    
                  </template>
                </el-form-item>
              </div>
            </el-col>
          </el-row>
          <el-row :gutter="24">
            <el-col :span="24">
              <div style="display:flex; justify-content:center">
                <el-button v-show="formData.dia" type="success" @click="onFinalizarDia(formData)">Finalizar Dia</el-button>
                <el-button v-show="!formData.dia" flat icon="fas fa-undo-alt" type="warning" @click="onVoltar"> {{ $config.txt.btVoltar }}</el-button>
                <el-button v-show="!formData.dia" icon="fas fa-save" type="success" :loading="loading.salvar" @click="submitForm('formData')"> Salvar Alterações </el-button>
              </div>
            </el-col>
          </el-row>
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
    name: 'EstabelecimentoHorarios',
    mixins: [Utils],
    data () {
      return {
        formData: {
          estabelecimento: '',
          procedimento: '',
          procEstHor: [],
          dia: '',
          hora: [],
        },
        api: {
          estabelecimentos: [],
          procedimentos: [],
          profissionais: [],
          estabProcHor: []
        },
        procedimentoHorariosDb: [],
        procedimentoHorarios: [],
        loading: {
          estabelecimentos: false,
          procedimentos: false,
          profissionais: false,
          estabProcHor: false,
          salvar: false
        },
        procedimentoQuery: {
          tipo: null,
          descricao: null
        },
        pickerOptions: {
          disabledDate: this.getDatasDisponiveis
        },
        horaDisponivel: this.$config.intervalo(),
        validacoes: {
          estabelecimento: [
            {required: true, type: 'object', message: this.$config.validacoes.cnesRequired, trigger: 'submit'}
          ],
          procedimento: [
            {required: true, type: 'string', message: this.$config.validacoes.procedimentoRequired, trigger: 'submit'}
          ],
          profissional: [
            {required: false, type: 'string', message: this.$config.validacoes.profissionalRequired, trigger: 'submit'}
          ],
          tipoDaConsulta: [
            {required: true, type: 'string', message: this.$config.validacoes.tipoDaConsultaRequired, trigger: 'submit'}
          ]
        },
        enums: {
          tiposDaConsulta: _enums.tiposDaConsulta
        },
        params: {
          skip: 1,
          take: 10,
          sort: 'e.NomeFantasia ASC',
          total: 0
        },
        procParams: {
          skip: 1,
          take: 10,
          descricao: '',
          sort: 'Descricao ASC',
          total: 0
        },
        profParams: {
          skip: 1,
          take: 9999999,
          descricao: '',
          sort: 'Nome ASC',
          total: 0
        }
      }
    },
    async created () {
      await this.getEstabelecimentos()
      this.estabelecimento = this.$route.params.estabelecimento
      if (this.estabelecimento) {
        this.formData.estabelecimento = this.estabelecimento
        await this.getEstabelecimentoProcedimentos()
      }
      // await this.getProcedimentos()
    },
    methods: {
      async getEstabelecimentos() {
        this.params.ativo = true
        this.loading.estabelecimentos = true
        let { data, paginacao, status } = await _api.estabelecimentos.get(this.params)
        if (status === 502) this.loading.estabelecimentos = false
        this.api.estabelecimentos = (status === 200) ? data : []
        this.params.skip = (status === 200) ? paginacao.skip : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.estabelecimentos = false
      },
      async searchEstabelecimentos (query) {
        if (query.length >= 3) {
          let { data, paginacao, status } = await _api.estabelecimentos.get({nomeFantasia: query})
          if (status === 502) this.loading.estabelecimentos = false
          this.api.estabelecimentos = (status === 200) ? data : []
          this.params.skip = (status === 200) ? paginacao.skip : 0
          this.params.total = (status === 200) ? paginacao.totalCount : 0
          this.loading.estabelecimentos = false
        }
      },
      async getEstabelecimentoProcedimentos () {
       // console.log('1')
        let filters = {
          estabelecimentoId: this.formData.estabelecimento.id,
          skip: 1,
          take: 999,
          descricao: '',
          sort: 'Descricao ASC',
          total: 0
        }
        this.loading.procedimentos = true
        let { data, paginacao, status } = await _api.estabelecimentos.getEstabelecimentoProcedimentos(filters)
        if (status === 502) this.loading.procedimentos = false
        this.api.procedimentos = (status === 200) ? data : []
        if (this.api.procedimentos.length === 0) {
          this.$swal({
            title: "Atenção!",
            text: `${this.$config.txt.estabelecimento.semProcedimento}!`,
            icon: 'warning',
          })
        }
        this.loading.procedimentos = false
      },
      async getProfissionais () {
        this.loading.profissionais = true
        this.profParams.ativo = true
        let { data, paginacao, status } = await _api.profissionais.get(this.profParams)
        if (status === 502) this.loading.profissionais = false
        this.api.profissionais = (status === 200) ? data : []
        this.profParams.skip = (status === 200) ? paginacao.skip : 0
        this.profParams.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.profissionais = false
      },
      async getEstabelecimentoDatas () {
        this.loading.estabProcHor = true
        let filters = {
          estabelecimentoId: this.formData.estabelecimento.id,
          procedimentoId: this.formData.procedimento,
          profissionalId: this.formData.profissional
        }
        let { data, paginacao, status } = await _api.estabelecimentosProcedimentosHorarios.get(filters)
        this.api.estabProcHor = (status === 200) ? data : []
        if (data.length > 0) {
          data.map(item => {
            let estabelecimentoHorario = {
              estabelecimentoId: item.estabelecimentoId,
              procedimentoId: item.procedimentoId,
              profissionalId: item.profissionalId,
              tipoDaConsulta: item.tipoDaConsulta,
              dia: this.moment(item.dataHora, 'YYYY-MM-DD HH:mm:ss').format('YYYY-MM-DD'),
              hora: this.moment(item.dataHora, 'YYYY-MM-DD HH:mm:ss').format('HH:mm'),
              dataHora: this.moment(item.dataHora, 'YYYY-MM-DD HH:mm:ss').format('YYYY-MM-DD HH:mm:ss')
            }
            this.procedimentoHorariosDb.push(estabelecimentoHorario)
          })
        }
        this.loading.estabProcHor = false
      },
      getDatasDisponiveis (date) {
        const indisponivel = date.getDay()
        return indisponivel === 6 || indisponivel === 0 || date < Date.now() - 86400000
      },
      onClear () {
        // this.getEstabelecimentos()
        this.formData.procedimento = ''
      },
      onVoltar () {
        this.$router.push({ name: 'Estabelecimentos' })
      },
      onFinalizarDia (formData) {
        let diaProcedimentoHorario = []
        // para cada horario adicionando montar o objeto completo
        formData.hora.forEach(hora => {
          let formatDia = this.moment(formData.dia).format('YYYY-MM-DD')
          let obj = {}
          if (this.isDemandaEspontanea == true) {
            obj = {
              estabelecimentoId: this.formData.estabelecimento.id,
              procedimentoId: this.formData.procedimento,
              profissionalId: this.formData.profissional,
              tipoDaConsulta: "Totem",
              dia: formatDia,
              hora: this.moment(hora, 'HH:mm').format('HH:mm'),
              dataHora: this.moment(formatDia + ' ' + hora, 'YYYY-MM-DD HH:mm:ss').format('YYYY-MM-DD HH:mm:ss')
            }
          } else {
            obj = {
              estabelecimentoId: this.formData.estabelecimento.id,
              procedimentoId: this.formData.procedimento,
              profissionalId: this.formData.profissional,
              tipoDaConsulta: this.formData.tipoDaConsulta,
              dia: formatDia,
              hora: this.moment(hora, 'HH:mm').format('HH:mm'),
              dataHora: this.moment(formatDia + ' ' + hora, 'YYYY-MM-DD HH:mm:ss').format('YYYY-MM-DD HH:mm:ss')
            }
          }
          
        //  console.log('obj', obj)
          this.procedimentoHorarios.push(obj)
          // Verfica se o horário já consta no Db
          if (this._.some(this.procedimentoHorariosDb, obj)) {
            this._.remove(this.procedimentoHorarios, obj)
            this.$swal({
              title: "Atenção!",
              text: `O horário ${obj.hora} do dia: ${this.moment(formData.dia).format('DD-MM-YYYY')} já havia sido cadastrado anteriormente!`,
              icon: 'warning',
            })
          }
        })
        // monta um objeto para permitir o agrupamento por dia
        diaProcedimentoHorario = {
          dia: this.moment(formData.dia, 'DD-MM-YYYY').format('YYYY-MM-DD'),
          procedimentoHorarios: this.procedimentoHorarios
        }
        this.procedimentoHorarios = [] // zerando os procedimentos para novos horarios
        // verifica se a data já foi agendada, se foi remove o agendamento anterior antes de reagendar
        if (this._.some(this.formData.procEstHor, {dia: diaProcedimentoHorario.dia})) {
          this._.remove(this.formData.procEstHor, {dia: diaProcedimentoHorario.dia})
        }
        this.formData.procEstHor.push(diaProcedimentoHorario)
        this.formData.procEstHor = this._.orderBy(this.formData.procEstHor, ['dia'], ['asc'])
        // zerando o transfer para novo preenchimento
        this.formData.dia = ''
        this.formData.hora = []
      },
      submitForm(formData) {
        this.loading.salvar = true;
        this.$refs[formData].validate(valid => {
          let listProcEstHor = []
          if (valid) {
            this.formData.procEstHor.forEach(item => {
              item.procedimentoHorarios.forEach(horario => {
                listProcEstHor.push(horario)
              })
            })
            _api.estabelecimentosProcedimentosHorarios.post(listProcEstHor).then(res => {
              if (res.status === 201) {
                this.loading.salvar = false;
                this.resetForm('formData')
              }
              this.loading.salvar = false;
            })
          }
        })
       

      },
      resetForm (form) {
        this.formData.procEstHor = []
        this.procedimentoHorariosDb = []
        this.procedimentoHorarios = []
        this.formData.procedimento = ''
        this.formData.profissional = ''
        //this.formData.tipoDaConsulta = ''
        // this.$refs[form].resetFields()
      }
    }
  }
</script>

<style>

.el-transfer__buttons button {
  display: block !important;
  margin-left: 0 !important;
}

.el-transfer-panel {
    width: 220px !important;
}
</style>
