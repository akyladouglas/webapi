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
                           :disabled="api.profissionais.length === 0"
                           filterable remote clearable
                           placeholder="Selecione o Profissional"
                           value-key="id"
                           v-loading.body="loading.profissionais">
                  <el-option v-for="item in api.profissionais" :key="item.id"
                             :label="item.nome" :value="item.id" />
                </el-select>
              </el-form-item>
            </el-col>
          </el-row>
         
          <el-row :gutter="24">
            <el-col :span="24">
              <div style="display:flex; justify-content:center">
                <el-button v-show="!formData.dia" flat icon="fas fa-undo-alt" type="warning" @click="onVoltar"> {{ $config.txt.btVoltar }}</el-button>
                <el-button v-show="!formData.dia" flat icon="fas fa-save" type="success" v-loading.body="loading.salvar" @click="submitForm('formData')"> Salvar Alterações </el-button>
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
          estabProcHor: false
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
      onVoltar () {
        this.$router.push({ name: 'Estabelecimentos' })
      },

      async submitForm(formData) {

        var params = {
          skip: 1,
          dataInicial: this.moment().startOf('day').format('YYYY-MM-DD HH:mm:ss'),
          estabelecimentoId: this.formData.estabelecimento.id,
          procedimentoId: this.formData.procedimento,
          profissionalId: this.formData.profissional

        }

        let { data, status } = await _api.estabelecimentosProcedimentosHorarios.get(params)
        console.log('data', data)
        console.log('status', status)

        if (status === 200 && data.length > 0) {
          return this.$swal({
            title: "Atenção!",
            text: `O profissional adicionado ja tem vinculo com o procedimento!`,
            icon: 'warning',
          })
        }

        var newObj = {
          estabelecimentoId: this.formData.estabelecimento.id,
          procedimentoId: this.formData.procedimento,
          profissionalId: this.formData.profissional
        }
        console.log("newObj", newObj)

        _api.estabelecimentosProcedimentosHorarios.post([newObj]).then(res => {
            if (res.status === 201) {
              this.resetForm('formData')
            }
          })

      },
      resetForm (form) {
        this.formData.procedimento = ''
        this.formData.profissional = ''
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
