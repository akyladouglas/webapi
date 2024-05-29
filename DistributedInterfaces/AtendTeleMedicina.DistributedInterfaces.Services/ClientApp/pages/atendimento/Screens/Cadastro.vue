<template>
  <div style="margin-top:20px">
    <el-card>
      <el-row>
        <el-col :span="12">
          <h2 class="box-card--h2"> Dados Cadastrais </h2>
        </el-col>
      </el-row>

      <el-row>
        <el-form :model="formIndividuo" status-icon :rules="validacoes" ref="formIndividuo" label-width="120px" label-position="top" class="form--individuo">
          <el-row :gutter="24">
            <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12">
              <el-form-item label="Nome Completo" prop="nomeCompleto">
                <el-input maxlength="100" show-word-limit v-model="formIndividuo.nomeCompleto" />
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12">
              <el-form-item label="Nome Social" prop="nomeSocial">
                <el-input maxlength="100" show-word-limit v-model="formIndividuo.nomeSocial" />
              </el-form-item>
            </el-col>
          </el-row>

          <el-row :gutter="24">
            <el-col :xs="24" :sm="24" :md="4" :lg="4" :xl="4">
              <el-form-item label="PIS/PASEP" prop="pisPasep">
                <el-input maxlength="20" show-word-limit v-model="formIndividuo.pisPasep" />
              </el-form-item>
            </el-col>

            <el-col :xs="24" :sm="24" :md="10" :lg="10" :xl="10">
              <el-form-item label="Nome da Mãe" class="forms--checkbox-right" prop="nomeDaMae">
                <el-checkbox v-model="formIndividuo.temMaeDesconhecida" @change="onChangeNomeMae">Mãe Desconhecida</el-checkbox>
                <el-input maxlength="100" :disabled="disabledTemMaeDesconhecida == true" show-word-limit v-model="formIndividuo.nomeDaMae" />
              </el-form-item>
            </el-col>

            <el-col :xs="24" :sm="24" :md="10" :lg="10" :xl="10">
              <el-form-item label="Nome do Pai" class="forms--checkbox-right" prop="nomeDoPai">
                <el-checkbox v-model="formIndividuo.temPaiDesconhecido" @change="onChangeNomePai">Pai Desconhecido</el-checkbox>
                <el-input maxlength="100" :disabled="disabledTemPaiDesconhecido === true" show-word-limit v-model="formIndividuo.nomeDoPai" />
              </el-form-item>
            </el-col>
          </el-row>

          <el-row :gutter="24">
            <el-col :xs="24" :sm="24" :md="8" :lg="8" :xl="8">
              <el-form-item label="Email" prop="email">
                <el-input maxlength="80" show-word-limit v-model="formIndividuo.email" />
              </el-form-item>
            </el-col>

            <el-col :xs="24" :sm="24" :md="8" :lg="8" :xl="8">
              <el-form-item label="CPF" prop="cpf">
                <el-input show-word-limit v-model="formIndividuo.cpf" masked="true" maxlength="14" v-mask="'###.###.###-##'" />
              </el-form-item>
            </el-col>

            <el-col :xs="24" :sm="24" :md="8" :lg="8" :xl="8">
              <el-form-item label="CNS" prop="cns">
                <el-input v-model="formIndividuo.cns" masked="true" maxlength="15" show-word-limit v-mask="'###############'" />
              </el-form-item>
            </el-col>
          </el-row>


          <el-row :gutter="24">
            <el-col :xs="12" :sm="12" :md="12" :lg="7" :xl="7">
              <el-form-item label="Sexo" prop="sexo">
                <el-select filterable placeholder="Selecione o Sexo" v-model="formIndividuo.sexo">
                  <el-option v-for="option in enums.sexos" :value="option.value" :label="option.label" :key="option.value" />
                </el-select>
              </el-form-item>
            </el-col>

            <el-col :xs="12" :sm="12" :md="12" :lg="5" :xl="5">
              <el-form-item label="Raça ou Cor" prop="racaOuCor">
                <el-select filterable placeholder=" " v-model="formIndividuo.racaOuCor">
                  <el-option v-for="option in enums.racaOuCor" :value="option.value" :label="option.label" :key="option.value" />
                </el-select>
              </el-form-item>
            </el-col>

            <el-col :xs="12" :sm="12" :md="12" :lg="6" :xl="6">
              <el-form-item label="Data Nascimento" prop="dataNascimento">
                <el-date-picker prefix-icon="fas fa-calendar-day" v-model="formIndividuo.dataNascimento" type="date" format="dd-MM-yyyy" />
              </el-form-item>
            </el-col>

            <el-col :xs="12" :sm="12" :md="12" :lg="6" :xl="6">
              <el-form-item label="Telefone Celular" prop="telefoneCelular">
                <el-input v-model="formIndividuo.telefoneCelular" show-word-limit masked="true" maxlength="11" v-mask="'###########'">
                  <i slot="prefix" class="fas fa-mobile-alt"></i>
                </el-input>
              </el-form-item>
            </el-col>
          </el-row>

          <el-row :gutter="20">
            <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
              <el-form-item label="CEP" prop="logradouroCep">
                <el-input v-model="formIndividuo.logradouroCep" show-word-limit @input="getCep" masked="true" maxlength="9" v-mask="'#####-###'" />
              </el-form-item>
            </el-col>
            <el-col :xs="12" :sm="12" :md="12" :lg="6" :xl="6">
              <el-form-item label="Estado" prop="ufAbreviado">
                <el-select :disabled="disabledUfPeloGetCep" filterable placeholder="Selecione o Estado" v-model="formIndividuo.ufAbreviado"
                           :loading="loading.ufs" @change="onSelectUf">
                  <el-option v-for="option in api.ufs" :value="option.ufAbreviado" :label="option.ufExtenso" :key="option.ufAbreviado" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :xs="12" :sm="12" :md="12" :lg="12" :xl="12">
              <el-form-item label="Cidade" prop="cidadeId">
                <el-select :disabled="disabledCidadePeloGetCep" filterable placeholder="Selecione a Cidade" v-model="formIndividuo.cidadeId"
                           :loading="loading.cidades">
                  <el-option v-for="option in api.cidades" :value="option.ibge" :label="option.nome" :key="option.ibge" />
                </el-select>
              </el-form-item>
            </el-col>
          </el-row>
          <el-row :gutter="24">
            <el-col :xs="12" :sm="12" :md="12" :lg="12" :xl="12">
              <el-form-item label="Bairro" prop="logradouroBairro">
                <el-input v-model="formIndividuo.logradouroBairro" />
              </el-form-item>
            </el-col>
            <el-col :xs="12" :sm="12" :md="12" :lg="12" :xl="12">
              <el-form-item label="Endereço" prop="logradouro">
                <el-input v-model="formIndividuo.logradouro" />
              </el-form-item>
            </el-col>
          </el-row>

          <el-row :gutter="24">

            <el-col :xs="12" :sm="12" :md="3" :lg="3" :xl="3">
              <el-form-item label="Número" prop="logradouroNumero">
                <el-input type="number" v-model="formIndividuo.logradouroNumero" show-word-limit />
              </el-form-item>
            </el-col>

            <el-col :xs="12" :sm="12" :md="4" :lg="4" :xl="4">
              <el-form-item label="Complemento" prop="logradouroComplemento">
                <el-input v-model="formIndividuo.logradouroComplemento" show-word-limit maxlength="30" />
              </el-form-item>
            </el-col>

            <el-col :xs="12" :sm="12" :md="4" :lg="4" :xl="4">
              <el-form-item label="Frequenta Escola" prop="frequentaEscola">
                <el-select filterable v-model="formIndividuo.frequentaEscola">
                  <el-option v-for="option in enums.simNao" :value="option.value" :label="option.label" :key="option.value" />
                </el-select>
              </el-form-item>
            </el-col>

            <el-col :xs="12" :sm="12" :md="6" :lg="6" :xl="6">
              <el-form-item label="Grau de Instrução" prop="grauDeInstrucao">
                <el-select filterable placeholder="Selecione o Grau de Instrução" v-model="formIndividuo.grauDeInstrucao">
                  <el-option v-for="option in enums.grauDeInstrucao" :value="option.value" :label="option.label" />
                </el-select>
              </el-form-item>
            </el-col>

            <el-col :xs="12" :sm="12" :md="7" :lg="7" :xl="7">
              <el-form-item label="Nacionalidade" prop="nacionalidade">
                <el-select placeholder="Selecione sua Nacionalidade" v-model="formIndividuo.nacionalidade" @change="onChangeNacionalidade">
                  <el-option v-for="option in enums.nacionalidade" :value="option.value" :label="option.label" />
                </el-select>
              </el-form-item>
            </el-col>
          </el-row>

          <el-row :gutter="24">
            <el-col :xs="12" :sm="12" :md="12" :lg="7" :xl="7" v-show="formIndividuo.nacionalidade != undefined && formIndividuo.nacionalidade == 1">
              <el-form-item label="Estado de Nascimento" prop="ufDeNascimentoAbreviado">
                <el-select filterable placeholder="Selecione o Estado de Nascimento" v-model="formIndividuo.ufDeNascimentoAbreviado" @change="onSelectUfDeNascimento" :loading="loading.cidades">
                  <el-option v-for="option in api.ufDeNascimento" :value="option.ufAbreviado" :label="option.ufExtenso" :key="option.ufAbreviado" />
                </el-select>
              </el-form-item>
            </el-col>

            <el-col :xs="12" :sm="12" :md="12" :lg="7" :xl="7" v-show="formIndividuo.nacionalidade != undefined && formIndividuo.nacionalidade == 1">
              <el-form-item label="Cidade De Nascimento" prop="cidadeDeNascimentoIbge">
                <el-select filterable placeholder="Selecione a Cidade de Nascimento" v-model="formIndividuo.cidadeDeNascimentoIbge" :loading="loading.cidades">
                  <el-option v-for="option in api.cidadesDeNascimento" :value="option.ibge" :label="option.nome" :key="option.ibge" />
                </el-select>
              </el-form-item>
            </el-col>

            <el-col :xs="12" :sm="12" :md="12" :lg="7" :xl="7" v-show="formIndividuo.nacionalidade != undefined && formIndividuo.nacionalidade == 2">
              <el-form-item label="Data da Naturalização" prop="naturalizacaoData">
                <el-date-picker prefix-icon="fas fa-calendar-day" v-model="formIndividuo.naturalizacaoData" type="date" format="dd-MM-yyyy" />
              </el-form-item>
            </el-col>

            <el-col :xs="12" :sm="12" :md="12" :lg="7" :xl="7" v-show="formIndividuo.nacionalidade != undefined && formIndividuo.nacionalidade == 2">
              <el-form-item label="Portaria de Naturalização" prop="naturalizacaoPortaria">
                <el-input v-model="formIndividuo.naturalizacaoPortaria" show-word-limit maxlength="30" />
              </el-form-item>
            </el-col>

            <el-col :xs="12" :sm="12" :md="12" :lg="7" :xl="7" v-show="formIndividuo.nacionalidade != undefined && formIndividuo.nacionalidade == 3">
              <el-form-item label="País de Nascimento" prop="paisDeNascimento">
                <el-select filterable placeholder="Selecione o País de Nascimento" v-model="formIndividuo.paisDeNascimento">
                  <el-option v-for="option in api.paises" :value="option.id" :label="option.nome" :key="option.id" />
                </el-select>
              </el-form-item>
            </el-col>

            <el-col :xs="12" :sm="12" :md="12" :lg="5" :xl="5" v-show="formIndividuo.nacionalidade != undefined && formIndividuo.nacionalidade == 3">
              <el-form-item label="Data da Entrada no Brasil" prop="dataEntradaNoPais">
                <el-date-picker prefix-icon="fas fa-calendar-day" v-model="formIndividuo.dataEntradaNoPais" type="date" format="dd-MM-yyyy" />
              </el-form-item>
            </el-col>
          </el-row>

          <el-row :gutter="24">
            <el-col :xs="24">
              <el-form-item>
                <el-button flat icon="fas fa-save" type="success" @click="onClickSalvar('formIndividuo')" :loading="loading.individuos"> Salvar</el-button>
              </el-form-item>
            </el-col>
          </el-row>

        </el-form>
      </el-row>
    </el-card>
  </div>
</template>

<script>
  import _api from '../../../api'
  import _enums from '../../../api/Enums'
  import { mask } from 'vue-the-mask'
  import moment from 'moment'
  moment.locale('pt-br')

  export default {
    name: 'Cadastro',
    props: {
      agendamento: {},
    },
    directives: { mask },
    components: {  },
    data() {
      return {
        //DATAS
        formIndividuo: {},
        api: {
          cidades: [],
          ufDeNascimento: [],
          cidadesDeNascimento: [],
          paises: [],
        },

        //CONTROLS
        disabledUfPeloGetCep: false,
        disabledCidadePeloGetCep: false,
        disabledTemMaeDesconhecida: false,
        disabledTemPaiDesconhecido: false,

        //LOADINGS
        loading: {
          ufs: false,
          cidades: false,
          individuos: false,
        },

        //AUXS
        enums: {
          sexos: _enums.sexos,
          racaOuCor: _enums.racaOuCor,
          grauDeInstrucao: _enums.grauDeInstrucao,
          nacionalidade: _enums.nacionalidade,
          simNao: _enums.simNao
        },

        //VALIDATE
        validacoes: {
          
          nomeCompleto: [
            { required: true, validator: this.mxValidaNomeIndividuo, trigger: ['blur', 'submit'] }
          ],
          cpf: [
            { required: false, validator: this.mxValidaCpfIndividuo, trigger: ['blur', 'submit'] }
          ],
          cns: [
            { required: false, validator: this.mxValidaCnsIndividuo, trigger: ['blur', 'submit'] }
          ],
          pisPasep: [
            { required: false, validator: this.mxValidaPisPasep, trigger: ['blur', 'submit'] }
          ],
          nomeDaMae: [
            { required: false, validator: this.mxValidaNomeDaMaeIndividuo, trigger: ['blur', 'submit'] }
          ],
          nomeDoPai: [
            { required: false, validator: this.mxValidaNomeDoPaiIndividuo, trigger: ['blur', 'submit'] }
          ],
          telefoneCelular: [
            { required: true, validator: this.mxValidaCelularIndividuo, trigger: ['blur', 'submit'] }
          ],
          email: [
            { required: true, validator: this.mxValidaEmailIndividuo, trigger: ['blur', 'submit'] }
          ],
          dataNascimento: [
            { required: true, validator: this.mxValidaDataNascimentoIndividuo, trigger: ['blur', 'submit'] }
          ],
          racaOuCor: [
            { required: true, message: 'Campo obrigatório', trigger: ['blur', 'submit'] }
          ],
          sexo: [
            { required: true, message: 'Campo obrigatório', trigger: ['blur', 'submit'] }
          ],
          // dependendo da opção de nacionalidade escolhida eles troca para true
          nacionalidade: [{ required: false, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],
          ufDeNascimentoAbreviado: [{ required: false, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],
          cidadeDeNascimentoIbge: [{ required: false, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],
          naturalizacaoData: [{ required: false, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],
          naturalizacaoPortaria: [{ required: false, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],
          paisDeNascimento: [{ required: false, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],
          dataEntradaNoPais: [{ required: false, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],
          grauDeInstrucao: [{ required: false, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],
          frequentaEscola: [{ required: false, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }]

          // logradouroCep: [{ required: true, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],
          // logradouroBairro: [
          //  { required: true, message: 'Campo obrigatório', trigger: ['change', 'submit'] },
          //  { min: 1, max: 100, message: 'Não pode conter menos de 1 e mais que 100 caracteres', trigger: 'change' }
          // ],
          // logradouro: [
          //  { required: true, message: 'Campo obrigatório', trigger: ['change', 'submit'] },
          //  { min: 1, max: 150, message: 'Não pode conter menos de 1 e mais que 150 caracteres', trigger: 'change' }
          // ],
          // cidadeId: [{ required: true, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],
          // ufAbreviado: [{ required: true, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],
          
        },

        //PARAMS
        paramsCidades: {
          skip: 1,
          take: 999,
          sort: '+Nome',
          total: 0,
          ufAbreviado: null
        },
        paramsUfsDeNascimento: {
          skip: 1,
          take: 30,
          sort: '+UfAbreviado',
          total: 0
        },
        paramsCidadesDeNascimento: {
          skip: 1,
          take: 999,
          sort: '+Nome',
          total: 0,
          ufAbreviado: null
        },
        paramsPaises: {
          skip: 1,
          take: 10000,
          sort: 'p.Nome ASC',
          total: 0
        },
      }
    },
    async created() {
      if (!this.agendamento.individuo) {
        this.$router.push({
          name: 'Agendamentos'
        })
        return
      }

      await this.getIndividuo()
    },

    async mounted() {
      await this.getCidadesDeNascimento()
      await this.getUfsDeNascimento()
      await this.getPaises()
    },

    methods: {

      async getIndividuo() {
        this.loading.individuos = true
        let { data, status } = await _api.individuos.getById(this.agendamento.individuoId)
        if (status === 502) this.loading.individuos = false
        this.formIndividuo = (status === 200) ? data : []
        this.loading.individuos = false

        if (this.formIndividuo.logradouroCep) await this.getCep(this.formIndividuo.logradouroCep)
        if (this.formIndividuo.ufAbreviado == 'NI') delete this.formIndividuo.ufAbreviado
        if (this.formIndividuo.cidadeId == '1100014') delete this.formIndividuo.cidadeId

        if (this.formIndividuo.temMaeDesconhecida == true)
        {
          this.disabledTemMaeDesconhecida = true
        }

        if (this.formIndividuo.temPaiDesconhecido == true) {
          this.disabledTemPaiDesconhecido = true
        }

        if (this.formIndividuo.ufDeNascimentoAbreviado) {

          this.paramsCidadesDeNascimento.ufAbreviado = this.formIndividuo.ufDeNascimentoAbreviado
          await this.getCidadesDeNascimento()
        } 
      },

      async onSelectUfDeNascimento(val) {
        this.paramsCidadesDeNascimento.ufAbreviado = val
        delete this.formIndividuo.cidadeDeNascimentoIbge
        this.formIndividuo = {
          ...this.formIndividuo

        }
        await this.getCidadesDeNascimento()
      },

      async onChangeNomeMae() {
        this.disabledTemMaeDesconhecida = !this.disabledTemMaeDesconhecida
        if (this.disabledTemMaeDesconhecida) {
          this.validacoes.nomeDaMae[0].required = false
          this.validacoes.nomeDaMae[0].validator = (rule, value, callback) => { return callback() }
          delete this.formIndividuo.nomeDaMae
        } else {
          this.validacoes.nomeDaMae[0].required = true
          this.validacoes.nomeDaMae[0].validator = this.mxValidaNomeDaMaeIndividuo
        }
      },

      async onChangeNomePai() {
        this.disabledTemPaiDesconhecido = !this.disabledTemPaiDesconhecido
        if (this.disabledTemPaiDesconhecido) {
          this.validacoes.nomeDoPai[0].required = false
          this.validacoes.nomeDoPai[0].validator = (rule, value, callback) => { return callback() }
          delete this.formIndividuo.nomeDoPai
        } else {
          this.validacoes.nomeDoPai[0].required = true
          this.validacoes.nomeDoPai[0].validator = this.mxValidaNomeDoPaiIndividuo
        }
      },

      async onChangeNacionalidade() {

        // VERIFICANDO A OPÇÃO SELECIONADA E DELETANDO OS CAMPOS QUE NÃO PODEM PERMANECER NO FORM
        if (this.formIndividuo.nacionalidade == 1) {
          // campos do 2
          delete this.formIndividuo.naturalizacaoData
          delete this.formIndividuo.naturalizacaoPortaria

          // campos do 3
          delete this.formIndividuo.paisDeNascimento
          delete this.formIndividuo.dataEntradaNoPais

        } else if (this.formIndividuo.naturalidade == 2) {
          // campos do 1
          delete this.formIndividuo.ufDeNascimentoAbreviado
          delete this.formIndividuo.cidadeDeNascimentoIbge

          // campos do 3
          delete this.formIndividuo.paisDeNascimento
          delete this.formIndividuo.dataEntradaNoPais

        } else {
          // campos do 1
          delete this.formIndividuo.ufDeNascimentoAbreviado
          delete this.formIndividuo.cidadeDeNascimentoIbge
          // campos do 2
          delete this.formIndividuo.naturalizacaoData
          delete this.formIndividuo.naturalizacaoPortaria
        }

        // ATIVANDO AS VALIDAÇÕES DOS CAMPOS DE ACORDO COM A SELEÇÃO
        if (this.formIndividuo.nacionalidade === 1) {
         
          // adiciona os campos obrigatórios
          this.validacoes.ufDeNascimentoAbreviado[0].required = true
          this.validacoes.cidadeDeNascimentoIbge[0].required = true

          // retira a obrigação dos demais
          this.validacoes.naturalizacaoData[0].required = false
          this.validacoes.naturalizacaoPortaria[0].required = false

          this.validacoes.paisDeNascimento[0].required = false
          this.validacoes.dataEntradaNoPais[0].required = false
          
        } else if (this.formIndividuo.nacionalidade == 2) {
          
          // adiciona os campos obrigatórios
          this.validacoes.naturalizacaoData[0].required = true
          this.validacoes.naturalizacaoPortaria[0].required = true

          // retira a obrigação dos demais
          this.validacoes.ufDeNascimentoAbreviado[0].required = false
          this.validacoes.cidadeDeNascimentoIbge[0].required = false

          this.validacoes.paisDeNascimento[0].required = false
          this.validacoes.dataEntradaNoPais[0].required = false
          
        } else {
          // opção 3- Estrangeiro
         
          // adiciona os campos obrigatórios
          this.validacoes.paisDeNascimento[0].required = true
          this.validacoes.dataEntradaNoPais[0].required = true

          // retira a obrigação dos demais
          this.validacoes.ufDeNascimentoAbreviado[0].required = false
          this.validacoes.cidadeDeNascimentoIbge[0].required = false

          this.validacoes.naturalizacaoData[0].required = false
          this.validacoes.naturalizacaoPortaria[0].required = false
        }
      },


      async getPaises() {
        let { data, status } = await _api.pais.get(this.paramsPaises)
        this.api.paises = (status === 200) ? data : []
      },

      async getUfsDeNascimento() {
        let { data, paginacao, status } = await _api.ufs.get(this.paramsUfsDeNascimento)
        this.api.ufDeNascimento = (status === 200) ? data : []
        if (this.api.ufDeNascimento.length === 1) {
          this.formIndividuo.ufDeNascimentoAbreviado = this.api.ufDeNascimento[0].ufAbreviado
          this.getCidadesDeNascimento()
        }
        this.paramsUfsDeNascimento.skip = (status === 200) ? paginacao.skip : 0
        this.paramsUfsDeNascimento.total = (status === 200) ? paginacao.totalCount : 0
      },

      async getCidadesDeNascimento() {
        this.loading.cidades = true
        let { data, paginacao, status } = await _api.cidades.get(this.paramsCidadesDeNascimento)
        this.api.cidadesDeNascimento = (status === 200) ? data : []
        if (this.api.cidadesDeNascimento.length === 1) {
          this.formIndividuo.cidadeDeNascimentoIbge = this.api.cidadesDeNascimento[0].ibge
        }
        this.paramsCidadesDeNascimento.skip = (status === 200) ? paginacao.currentPage : 0
        this.paramsCidadesDeNascimento.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.cidades = false
      },


      async getCep(cep) {
        if (cep != undefined) {
          cep = cep.replace(/[.-\s]/g, '')
          if (cep.length >= 8) {
            let { data, status } = await _api.auxiliar.getCep(cep)
            if (status === 200) {
              // caso ache alguma informação de uf e cidade atravez do getcep desativa os campos estado e cidade para não poder editar e causar divergencia
              this.disabledUfPeloGetCep = true
              this.disabledCidadePeloGetCep = true

              this.formIndividuo = {
                ...this.formIndividuo,
                cidadeId: data.ibge,
                logadouro: data.logradouro,
                logradouroBairro: data.bairro,
                ufAbreviado: data.uf,
              }

              this.$set(this.formIndividuo, 'logadouro', data.logradouro);
              this.$set(this.formIndividuo, 'logradouroBairro', data.bairro);

              this.paramsCidades.ibge = data.ibge

              await this.onSelectUf(data.uf)
              await this.getCidadesByUf(data.uf)

            } else {

              this.paramsCidades.ibge = null
              this.formIndividuo = {
                ...this.formIndividuo,
                ufAbreviado: null,
                cidadeId: null,
                logradouro: null,
                bairro: null
              }

            }
          } else {
            console.log('cep < 8')
          }
        } else {
          console.log('cep undefined')
        }
      },

      async getUfs() {
        this.loading.ufs = true
        let { data, paginacao, status } = await _api.ufs.get(this.paramsUfs)
        this.api.ufs = (status === 200) ? data : []
        if (this.api.ufs.length === 1) {
          this.formIndividuo.ufAbreviado = this.api.ufs[0].ufAbreviado
          this.getCidadesByUf()
        }
        this.paramsUfs.skip = (status === 200) ? paginacao.skip : 0
        this.paramsUfs.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.ufs = false
      },

      async onSelectUf(val) {
        this.paramsCidades.ufAbreviado = val
        this.formIndividuo = {
          ...this.formIndividuo
        }
        await this.getCidadesByUf()
      },

      async getCidadesByUf() {
        this.loading.cidades = true
        let { data, paginacao, status } = await _api.cidades.get(this.paramsCidades)
        this.api.cidades = (status === 200) ? data : []
        if (this.api.cidades.length === 1) {
          this.formIndividuo.cidadeId = this.api.cidades[0].ibge
        }
        this.paramsCidades.skip = (status === 200) ? paginacao.currentPage : 0
        this.paramsCidades.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.cidades = false
      },


      async onClickSalvar(form) {

        if (this.formIndividuo.imagem == undefined) this.formIndividuo.imagem = ''
        if (this.formIndividuo.dataNascimento != undefined) {
          var data = new Date(this.formIndividuo.dataNascimento)
          var ano = data.getFullYear()
          var mes = ('0' + (data.getMonth() + 1)).slice(-2)
          var dia = ('0' + data.getDate()).slice(-2)
          var dataFormatada = ano + '-' + mes + '-' + dia
          this.formIndividuo.dataNascimento = dataFormatada
        }
        if (this.formIndividuo.logradouroCep != undefined) this.formIndividuo.logradouroCep = this.formIndividuo.logradouroCep.replace(/[.-\s]/g, '')
        if (this.formIndividuo.cpf != undefined) this.formIndividuo.cpf = this.formIndividuo.cpf.replace(/[.-\s]/g, '')

        this.$refs[form].validate(async (valid) => {
          if (valid) {
            this.loading.individuos = true

              this.$prompt('Descreva o motivo da edição do cadastro:', 'Alteração de dados cadastrados', {
                confirmButtonText: 'Confirmar',
                cancelButtonText: 'Cancelar',
                inputPattern: /^[A-Za-záàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ ]+$/,
                inputErrorMessage: 'O campo pode conter apenas letras'
              }).then(({ value }) => {
                  this.formIndividuo = {
                    ...this.formIndividuo,
                    justificativa: value,
                    nacionalidade: this.formIndividuo.nacionalidade
                  }

                _api.individuos.put(this.formIndividuo).then(res => {
                  if (res.status === 200) {
                    this.loading.individuos = false
                  } else {
                    this.loading.individuos = false
                  }
                })
              }).catch(() => {

              });
          } else {
              this.$swal({
                title: "Atenção!",
                text: 'Preencha os campos obrigatórios!',
                icon: 'warning',
              })
            this.loading.individuos = false
          }
        })
        this.loading.individuos = false
      },
    }
  }
</script>

<style scoped>
  
</style>
