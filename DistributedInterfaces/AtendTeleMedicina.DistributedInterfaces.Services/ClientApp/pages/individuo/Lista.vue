<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">

    <el-card>

      <el-row>
        <el-col :span="14">
          <h2 v-show="listando" class="box-card--h2">Pacientes</h2>
          <h2 v-show="!listando" class="box-card--h2"> Cadastrar Paciente</h2>

        </el-col>
        <el-col :span="10" class="text-right">
          <el-form :inline="true">
            <el-form-item>
              <el-button v-if="listando" style="margin-right: -10px" icon="fas fa-user-plus" type="success" @click="onClickNovo"> Cadastrar Paciente</el-button>
              <el-button v-if="!listando" style="margin-right: -10px" icon="fas fa-undo-alt" type="warning" @click="onListar('formIndividuo')"> Voltar</el-button>
            </el-form-item>
          </el-form>
        </el-col>
      </el-row>

      <el-row v-show="listando">
        <el-col :span="24">
          <FiltroIndividuo :loading="loading.individuos" :params="params" @emit="onEmitFromFiltro" />
        </el-col>
      </el-row>

      <el-row v-show="listando && api.individuos.length > 0">
        <el-col :span="24">
          <el-table ref="tabela" :data="api.individuos"
                    highlight-current-row border
                    v-loading="loading.individuos"
                    class="table--profissionais table--row-click">
            <el-table-column label="CPF" prop="cpf" width="180" />
            <el-table-column label="CNS" prop="cns" width="180">
              <template slot-scope="scope">
                <span>{{ scope.row.cns ? scope.row.cns : 'Não informado' }}</span>
              </template>
            </el-table-column>
            <el-table-column label="Nome" prop="nomeCompleto" fixed />
            <el-table-column label="Email" prop="email" />
            <el-table-column header-align="center" align="right" width="140" fixed="right">

              <template slot-scope="scope">
                
                  <el-button type="primary" size="small" @click="onEditar(scope.$index, scope.row)">
                    <i class="fas fa-user-edit"></i> Editar
                  </el-button>

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

        <el-form :model="formIndividuo" status-icon :rules="metodo === 'POST' ? validacoesPOST : validacoesPUT" :disabled="isDisabled" ref="formIndividuo" label-width="120px" label-position="top" class="form--individuo">
          <el-row :gutter="24">
            <el-col :xs="24" :sm="24" :md="24" :lg="12" :xl="12">
              <el-form-item label="Nome Completo" prop="nomeCompleto">
                <el-input maxlength="100" show-word-limit v-model="formIndividuo.nomeCompleto" />
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="24" :lg="12" :xl="12">
              <el-form-item label="PIS/PASEP" prop="pisPasep">
                <el-input maxlength="20" show-word-limit v-model="formIndividuo.pisPasep" />
              </el-form-item>
            </el-col>
          </el-row>
          <el-row :gutter="24">
            <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12">
              <el-form-item label="Nome Social" prop="nomeSocial">
                <el-input maxlength="100" show-word-limit v-model="formIndividuo.nomeSocial" />
              </el-form-item>
            </el-col>

            <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12">
              <el-form-item label="Nome da Mãe" class="forms--checkbox-right" prop="nomeDaMae">
                <el-checkbox v-model="formIndividuo.temMaeDesconhecida" @change="onChangeNomeMae">Mãe Desconhecida</el-checkbox>
                <el-input maxlength="100" :disabled="disabledTemMaeDesconhecida == true" show-word-limit v-model="formIndividuo.nomeDaMae" />
              </el-form-item>
            </el-col>

          </el-row>

          <el-row :gutter="24">

            <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12">
              <el-form-item label="Nome do Pai" class="forms--checkbox-right" prop="nomeDoPai">
                <el-checkbox v-model="formIndividuo.temPaiDesconhecido" @change="onChangeNomePai">Pai Desconhecido</el-checkbox>
                <el-input maxlength="100" :disabled="disabledTemPaiDesconhecido === true" show-word-limit v-model="formIndividuo.nomeDoPai" />
              </el-form-item>
            </el-col>

            <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12">
              <el-form-item label="Email" prop="email">
                <el-input maxlength="80" show-word-limit v-model="formIndividuo.email" />
              </el-form-item>
            </el-col>

          </el-row>

          <el-row :gutter="24">

            <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12">
              <el-form-item label="CPF" prop="cpf">
                <el-input show-word-limit v-model="formIndividuo.cpf" masked="true" maxlength="14" v-mask="'###.###.###-##'" />
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12">
              <el-form-item label="CNS" prop="cns">
                <el-input v-model="formIndividuo.cns" masked="true" maxlength="15" show-word-limit v-mask="'###############'" />
              </el-form-item>
            </el-col>

          </el-row>

          <el-row :gutter="20">


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

          <!--Senhas-->
          <!--<el-row :gutter="20">
            <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6" v-show="metodo === 'POST'">
              <el-form-item label="Senha" prop="senha">
                <el-input v-model="formIndividuo.senha" type="password" autocomplete="off" />
              </el-form-item>
            </el-col>

            <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6" v-show="metodo === 'POST'">
              <el-form-item label="Confirmação da senha" prop="senhaConfirmacao">
                <el-input v-model="formIndividuo.senhaConfirmacao" type="password" autocomplete="off" />
              </el-form-item>
            </el-col>
          </el-row>-->

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

            <el-col :xs="12" :sm="12" :md="12" :lg="8" :xl="8">
              <el-form-item label="Número" prop="logradouroNumero">
                <el-input type="number" v-model="formIndividuo.logradouroNumero" show-word-limit />
              </el-form-item>
            </el-col>

            <el-col :xs="12" :sm="12" :md="12" :lg="8" :xl="8">
              <el-form-item label="Complemento" prop="logradouroComplemento">
                <el-input v-model="formIndividuo.logradouroComplemento" show-word-limit maxlength="30" />
              </el-form-item>
            </el-col>

            <el-col :xs="12" :sm="12" :md="12" :lg="8" :xl="8">
              <el-form-item label="Nacionalidade" prop="nacionalidadeCidadao">
                <el-select placeholder="Selecione sua Nacionalidade" v-model="formIndividuo.nacionalidadeCidadao" @change="onChangeNacionalidadeCidadao">
                  <el-option v-for="option in enums.nacionalidade" :value="option.value" :label="option.label" />
                </el-select>
              </el-form-item>
            </el-col>

          </el-row>

          <el-row :gutter="24">
            <el-col :xs="12" :sm="12" :md="12" :lg="7" :xl="7" v-show="formIndividuo.nacionalidadeCidadao != undefined && formIndividuo.nacionalidadeCidadao == 1">
              <el-form-item label="Estado de Nascimento" prop="ufDeNascimentoAbreviado">
                <el-select filterable placeholder="Selecione o Estado de Nascimento" v-model="formIndividuo.ufDeNascimentoAbreviado" @change="onSelectUfDeNascimento" :loading="loading.cidades">
                  <el-option v-for="option in api.ufDeNascimento" :value="option.ufAbreviado" :label="option.ufExtenso" :key="option.ufAbreviado" />
                </el-select>
              </el-form-item>
            </el-col>

            <el-col :xs="12" :sm="12" :md="12" :lg="7" :xl="7" v-show="formIndividuo.nacionalidadeCidadao != undefined && formIndividuo.nacionalidadeCidadao == 1">
              <el-form-item label="Cidade De Nascimento" prop="cidadeDeNascimentoIbge">
                <el-select filterable placeholder="Selecione a Cidade de Nascimento" v-model="formIndividuo.cidadeDeNascimentoIbge"  :loading="loading.cidades">
                  <el-option v-for="option in api.cidadesDeNascimento" :value="option.ibge" :label="option.nome" :key="option.ibge" />
                </el-select>
              </el-form-item>
            </el-col>

            <el-col :xs="12" :sm="12" :md="12" :lg="7" :xl="7" v-show="formIndividuo.nacionalidadeCidadao != undefined && formIndividuo.nacionalidadeCidadao == 2">
              <el-form-item label="Data da Naturalização" prop="naturalizacaoData">
                <el-date-picker prefix-icon="fas fa-calendar-day" v-model="formIndividuo.naturalizacaoData" type="date" format="dd-MM-yyyy" />
              </el-form-item>
            </el-col>

            <el-col :xs="12" :sm="12" :md="12" :lg="7" :xl="7" v-show="formIndividuo.nacionalidadeCidadao != undefined && formIndividuo.nacionalidadeCidadao == 2">
              <el-form-item label="Portaria de Naturalização" prop="naturalizacaoPortaria">
                <el-input v-model="formIndividuo.naturalizacaoPortaria" show-word-limit maxlength="30" />
              </el-form-item>
            </el-col>

            <el-col :xs="12" :sm="12" :md="12" :lg="7" :xl="7" v-show="formIndividuo.nacionalidadeCidadao != undefined && formIndividuo.nacionalidadeCidadao == 3">
              <el-form-item label="País de Nascimento" prop="paisDeNascimento">
                <el-select filterable placeholder="Selecione o País de Nascimento" v-model="formIndividuo.paisDeNascimento">
                  <el-option v-for="option in api.paises" :value="option.id" :label="option.nome" :key="option.id" />
                </el-select>
              </el-form-item>
            </el-col>

            <el-col :xs="12" :sm="12" :md="12" :lg="5" :xl="5" v-show="formIndividuo.nacionalidadeCidadao != undefined && formIndividuo.nacionalidadeCidadao == 3">
              <el-form-item label="Data da Entrada no Brasil" prop="dataEntradaNoPais">
                <el-date-picker prefix-icon="fas fa-calendar-day" v-model="formIndividuo.dataEntradaNoPais" type="date" format="dd-MM-yyyy" />
              </el-form-item>
            </el-col>

            <el-col :xs="12" :sm="12" :md="12" :lg="4" :xl="4">
              <el-form-item label="Frequenta Escola" prop="frequentaEscola" >
                <el-select filterable v-model="formIndividuo.frequentaEscola">
                  <el-option v-for="option in enums.simNao" :value="option.value" :label="option.label" :key="option.value" />
                </el-select>
              </el-form-item>
            </el-col>

            <el-col :xs="12" :sm="12" :md="12" :lg="6" :xl="6">
              <el-form-item label="Grau de Instrução" prop="grauDeInstrucao">
                <el-select filterable placeholder="Selecione o Grau de Instrução" v-model="formIndividuo.grauDeInstrucao">
                  <el-option v-for="option in enums.grauDeInstrucao" :value="option.value" :label="option.label" />
                </el-select>
              </el-form-item>
            </el-col>
            
          </el-row>

          <el-row :gutter="20">
            <el-col :xs="24">
              <el-form-item>
                <el-button flat icon="fas fa-save" type="success" @click="onClickSalvar('formIndividuo')" :loading="loading.individuos"> Salvar</el-button>
                <el-button flat icon="fas fa-undo-alt" type="warning" @click="onListar('formIndividuo')" :disabled="loading.individuos"> Voltar</el-button>
                <el-button flat icon="fas fa-eraser" v-if="metodo === 'POST'" type="default" @click="resetForm('formIndividuo')" :disabled="loading.individuos"> Limpar</el-button>
              </el-form-item>
            </el-col>
          </el-row>

        </el-form>

      </el-row>
    </el-card>

  </el-col>
</template>

<script>
  import VuePerfectScrollbar from 'vue-perfect-scrollbar'
  import { Notification, Row } from 'element-ui'
  import Utils from '../../mixins/Utils'
  import jQuery from 'jquery'
  import _api from '../../api'
  import _enums from '../../api/Enums'
  import FiltroIndividuo from '../../components/shared/FiltroIndividuo'
  import { mask } from 'vue-the-mask'
  import moment from 'moment'
  moment.locale('pt-br')
  export default {
    name: 'IndividuoLista',
    mixins: [Utils],
    components: { FiltroIndividuo, VuePerfectScrollbar },
    directives: { mask },
    data () {
      return {
        isDisabled: false,
        disabledTemPaiDesconhecido: false,
        disabledTemMaeDesconhecida: false,
        disabledUfPeloGetCep: false,
        disabledCidadePeloGetCep: false,

        metodo: 'POST',
        listando: true,
        erros: [],
        formIndividuo: {
          temMaeDesconhecida: false,
          temPaiDesconhecido: false
        },
  
        enums: {
          sexos: _enums.sexos,
          racaOuCor: _enums.racaOuCor,
          grauDeInstrucao: _enums.grauDeInstrucao,
          nacionalidade: _enums.nacionalidade,
          simNao: _enums.simNao
        },
        listaIndividuos: [],
        api: {
          individuos: [],
          ufs: [],
          cidades: []
        },
        loading: {
          individuos: false,
          cidades: false,
          ufs: false
        },
        scrollSettings: {
          maxScrollbarLength: 50
        },

        // validações
        validacoesPOST: {
          nomeCompleto: [
            { required: true, validator: this.mxValidaNomeIndividuo, trigger: ['blur', 'submit'] }
          ],
          pisPasep: [
            { required: false, validator: this.mxValidaPisPasep, trigger: ['blur', 'submit'] }
          ],
          cpf: [
            { required: true, validator: this.mxValidaCpfIndividuo, trigger: ['blur', 'submit'] }
          ],
          cns: [
            { required: false, validator: this.mxValidaCnsIndividuo, trigger: ['blur', 'submit'] }
          ],
          nomeDaMae: [
            { required: false, validator: this.mxValidaNomeDaMaeIndividuo, trigger: ['blur', 'submit'] }
          ],
          nomeDoPai: [
            { required: false, validator: this.mxValidaNomeDoPaiIndividuo, trigger: ['blur', 'submit'] }
          ],
          // senha: [
          //  { required: true, validator: this.mxValidaSenhaIndividuo, trigger: ['blur', 'submit'] },
          // ],
          // senhaConfirmacao: [
          //  { required: true, validator: validaSenhaConfirmacao, trigger: ['blur', 'submit'] }
          // ],
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
          nacionalidadeCidadao: [{ required: true, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],
          ufDeNascimentoAbreviado: [{ required: false, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],
          cidadeDeNascimentoIbge: [{ required: false, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],
          naturalizacaoData: [{ required: false, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],
          naturalizacaoPortaria: [{ required: false, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],
          paisDeNascimento: [{ required: false, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],
          dataEntradaNoPais: [{ required: false, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],
          frequentaEscola: [{ required: true, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }]
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
        validacoesPUT: {
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
          // senha: [
          //  { required: true, validator: this.mxValidaSenhaIndividuo, trigger: ['blur', 'submit'] },
          // ],
          // senhaConfirmacao: [
          //  { required: true, validator: validaSenhaConfirmacao, trigger: ['blur', 'submit'] }
          // ],
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
          nacionalidadeCidadao: [{ required: true, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],
          ufDeNascimentoAbreviado: [{ required: false, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],
          cidadeDeNascimentoIbge: [{ required: false, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],
          naturalizacaoData: [{ required: false, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],
          naturalizacaoPortaria: [{ required: false, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],
          paisDeNascimento: [{ required: false, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],
          dataEntradaNoPais: [{ required: false, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],
          grauDeInstrucao: [{ required: true, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }],
          frequentaEscola: [{ required: true, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }]

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

        params: {
          skip: 1,
          take: 10,
          sort: 'i.NomeCompleto ASC',
          total: 0
        },
        paramsPaises: {
          skip: 1,
          take: 10000,
          sort: 'p.Nome ASC',
          total: 0
        },
        paramsUfs: {
          skip: 1,
          take: 30,
          sort: '+UfAbreviado',
          total: 0
        },
        paramsUfsDeNascimento: {
          skip: 1,
          take: 30,
          sort: '+UfAbreviado',
          total: 0
        },
        paramsCidades: {
          skip: 1,
          take: 999,
          sort: '+Nome',
          total: 0,
          ufAbreviado: null
        },
        paramsCidadesDeNascimento: {
          skip: 1,
          take: 999,
          sort: '+Nome',
          total: 0,
          ufAbreviado: null
        }
      }
    },
    async mounted() {
      await this.getIndividuos()
      // await this.getUfs()
      await this.getUfsDeNascimento()
      await this.getPaises()
    },
    methods: {
      onEmitFromFiltro (val) {
        this.params = {
          ...this.params,
          ...val.params,
          page: 1
        }
        this.listando = true
        this.getIndividuos()
      },

      async getPaises () {
        let { data, status } = await _api.pais.get(this.paramsPaises)
        this.api.paises = (status === 200) ? data : []
      },

      async getIndividuos () {
        this.loading.individuos = true

        if (this.mxHasAccess('Recepcionista')) this.params.ativo = true

        let { data, paginacao, status } = await _api.individuos.getAll(this.params)
        if (status === 502) this.loading.individuos = false
        this.api.individuos = (status === 200) ? data : []
        this.params.skip = (status === 200) ? paginacao.skip : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.individuos = false
      },
  
      onClickNovo () {
        this.isDisabled = false
        this.listando = false
        this.metodo = 'POST'
      },
      onListar (form) {
        let i = this.api.individuos.findIndex(x => x.id === this.formIndividuo.id)
        this.$refs.tabela.setCurrentRow(this.api.individuos[i])
        this.$refs[form].resetFields()
        this.listando = true
      },
      async onClickSalvar (form) {
        this.erros = []
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
  
        // if (this.formIndividuo.cpf != undefined) this.formIndividuo.cpf = this.formIndividuo.cpf.replace(/[.-\s]/g, '')
        // if (this.formIndividuo.logradouroCep != undefined) this.formIndividuo.logradouroCep = this.formIndividuo.logradouroCep.replace(/[.-\s]/g, '')
  
        this.$refs[form].validate(async (valid) => {
          if (valid) {

            



            this.loading.individuos = true
            if (this.metodo === 'POST') {
              // caso o paciente não tenha senha o cpf tornasse a senha dele
              if (this.formIndividuo.senha == undefined || this.formIndividuo.senha === '') this.formIndividuo.senha = this.formIndividuo.cpf
              if (this.formIndividuo.senhaConfirmacao == undefined || this.formIndividuo.senhaConfirmacao === '') this.formIndividuo.senhaConfirmacao = this.formIndividuo.cpf

              this.formIndividuo = {
                ...this.formIndividuo,
                ativo: true,
                imagem: '',
                nomeCompleto: this.formIndividuo.nomeCompleto.toUpperCase(),
                nomeDaMae: this.formIndividuo.nomeDaMae.toUpperCase(),
                nacionalidade: this.formIndividuo.nacionalidadeCidadao,
                paisDeNascimento: this.formIndividuo.nacionalidadeCidadao === 1 ? 36 : this.formIndividuo.paisDeNascimento
              }

              _api.individuos.post(this.formIndividuo).then(res => {
                if (res.status === 201) {
                  this.loading.individuos = false
                  this.onListar(form)
                  this.resetForm(form)
                  this.getIndividuos()

                  this.$swal({
                    title: "Sucesso!",
                    text: 'Paciente cadastrado com sucesso!',
                    icon: 'success',
                  })

                } else {
                  this.$swal({
                    title: "Erro!",
                    text: 'Ocorreu um erro ao cadastrar o paciente!',
                    icon: 'error',
                  })

                  this.loading.individuos = false
                }
              })
            } else {


              this.$prompt('Descreva o motivo da edição do cadastro:', 'Alteração de dados cadastrados', {
                confirmButtonText: 'Confirmar',
                cancelButtonText: 'Cancelar',
                inputPattern: /^[A-Za-záàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ ]+$/,
                //  /[\w!#$%&'*+/=?^_`{|}~-]+(?:\.[\w!#$%&'*+/=?^_`{|}~-]+)*@(?:[\w](?:[\w-]*[\w])?\.)+[\w](?:[\w-]*[\w])?/
                inputErrorMessage: 'O campo pode conter apenas letras'
              }).then(({ value }) => {
                this.formIndividuo = {
                  ...this.formIndividuo,
                  justificativa: value,
                nacionalidade: this.formIndividuo.nacionalidadeCidadao
                }

                _api.individuos.put(this.formIndividuo).then(res => {
                if (res.status === 200) {
                  this.loading.individuos = false
                  this.onListar(form)
                  this.resetForm(form)
                  this.getIndividuos()
                  /* this.$message.success('Paciente Editado com sucesso.') */
                } else {
                  /* this.$message.warnig('Erro ao cadastrar paciente.') */

                  this.loading.individuos = false
                }
              })

              }).catch(() => {

              });
            }
          } else {
            this.$swal({
              title: "Atenção!",
              text: 'Preencha todos os campos obrigatórios!',
              icon: 'warning',
            })
            this.loading.individuos = false
          }
        })
        this.loading.individuos = false
      },
      limparSelecao (rows) {
        if (rows) {
          rows.forEach(row => {
            this.$refs.tabela.toggleRowSelection(row)
          })
        } else {
          this.$refs.tabela.clearSelection()
        }
      },
      resetForm (form) {
        this.$refs[form].resetFields()
      },
      handleSizeChange (val) {
        this.params.take = val
        this.getIndividuos()
      },
      handleCurrentChange (val) {
        this.params.skip = val
        this.getIndividuos()
      },

      async onEditar (index, row) {
        this.metodo = 'PUT'
        this.isDisabled = false
        this.listando = false
        console.log('row', row)
        this.getCep(row.logradouroCep)

        if (row.ufAbreviado === 'NI') {
          this.formIndividuo = {
            ...row,
            ufAbreviado: '',
            cidadeId: ''
          }
        } else {
          this.formIndividuo = {
            ...row
          }
        }
      },
  
      async getUfs () {
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

      async getUfsDeNascimento () {
        let { data, paginacao, status } = await _api.ufs.get(this.paramsUfsDeNascimento)
        this.api.ufDeNascimento = (status === 200) ? data : []
        if (this.api.ufDeNascimento.length === 1) {
          this.formIndividuo.ufDeNascimentoAbreviado = this.api.ufDeNascimento[0].ufAbreviado
          this.getCidadesDeNascimento()
        }
        this.paramsUfs.skip = (status === 200) ? paginacao.skip : 0
        this.paramsUfs.total = (status === 200) ? paginacao.totalCount : 0
      },

      async getCidadesDeNascimento () {
        this.loading.cidades = true
        let { data, paginacao, status } = await _api.cidades.get(this.paramsCidadesDeNascimento)
        this.api.cidadesDeNascimento = (status === 200) ? data : []
        if (this.api.cidadesDeNascimento.length === 1) {
          this.formIndividuo.cidadeDeNascimentoIbge = this.api.cidadesDeNascimento[0].ibge
        }
        this.paramsCidades.skip = (status === 200) ? paginacao.currentPage : 0
        this.paramsCidades.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.cidades = false
      },

      async getCidadesByUf () {
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

      async onSelectUf (val) {
        this.paramsCidades.ufAbreviado = val
        this.formIndividuo = {
          ...this.formIndividuo
  
        }
        await this.getCidadesByUf()
      },

      async onSelectUfDeNascimento (val) {
        this.paramsCidadesDeNascimento.ufAbreviado = val
        delete this.formIndividuo.cidadeDeNascimentoIbge
        this.formIndividuo = {
          ...this.formIndividuo
  
        }
        await this.getCidadesDeNascimento()
      },
      async getCep (cep) {
        if (cep != undefined) {
          cep = cep.replace(/[.-\s]/g, '')
          if (cep.length >= 8) {
            let { data, status } = await _api.auxiliar.getCep(cep)
            if (status === 200) {
              // caso ache alguma informação de uf e cidade atravez do getcep desativa os campos estado e cidade para não poder editar e causar divergencia
              this.disabledUfPeloGetCep = true
              this.disabledCidadePeloGetCep = true

              this.formIndividuo.cidadeId = data.ibge
              this.formIndividuo.logradouro = data.logradouro
              this.formIndividuo.logradouroBairro = data.bairro
              this.formIndividuo.ufAbreviado = data.uf
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
            console.log('erro cep')
          }
        } else {
          console.log('cep -> ', cep)
        }
      },

      async onChangeNomePai () {
        this.disabledTemPaiDesconhecido = !this.disabledTemPaiDesconhecido
        if (this.metodo === 'POST') {
          if (this.disabledTemPaiDesconhecido) {
            this.validacoesPOST.nomeDoPai[0].required = false
            this.validacoesPOST.nomeDoPai[0].validator = (rule, value, callback) => { return callback() }
            delete this.formIndividuo.nomeDoPai
          } else {
            this.validacoesPOST.nomeDoPai[0].required = true
            this.validacoesPOST.nomeDoPai[0].validator = this.mxValidaNomeDoPaiIndividuo
          }
        } else {
          if (this.disabledTemPaiDesconhecido) {
            this.validacoesPUT.nomeDoPai[0].required = false
            this.validacoesPUT.nomeDoPai[0].validator = (rule, value, callback) => { return callback() }
            delete this.formIndividuo.nomeDoPai
          } else {
            this.validacoesPUT.nomeDoPai[0].required = true
            this.validacoesPUT.nomeDoPai[0].validator = this.mxValidaNomeDoPaiIndividuo
          }
        }
      },

      async onChangeNomeMae () {
        this.disabledTemMaeDesconhecida = !this.disabledTemMaeDesconhecida

        if (this.metodo === 'POST') {
          if (this.disabledTemMaeDesconhecida) {
            this.validacoesPOST.nomeDaMae[0].required = false
            this.validacoesPOST.nomeDaMae[0].validator = (rule, value, callback) => { return callback() }
            delete this.formIndividuo.nomeDaMae
          } else {
            this.validacoesPOST.nomeDaMae[0].required = true
            this.validacoesPOST.nomeDaMae[0].validator = this.mxValidaNomeDaMaeIndividuo
          }
        } else {
          if (this.disabledTemMaeDesconhecida) {
            this.validacoesPUT.nomeDaMae[0].required = false
            this.validacoesPUT.nomeDaMae[0].validator = (rule, value, callback) => { return callback() }
            delete this.formIndividuo.nomeDaMae
          } else {
            this.validacoesPUT.nomeDaMae[0].required = true
            this.validacoesPUT.nomeDaMae[0].validator = this.mxValidaNomeDaMaeIndividuo
          }
        }
      },

      async onChangeNacionalidadeCidadao () {
        // VERIFICANDO A OPÇÃO SELECIONADA E DELETANDO OS CAMPOS QUE NÃO PODEM PERMANECER NO FORM
        if (this.formIndividuo.nacionalidadeCidadao == 1) {
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
        if (this.formIndividuo.nacionalidadeCidadao === 1) {
          if (this.metodo === 'POST') {
            // adiciona os campos obrigatórios
            this.validacoesPOST.ufDeNascimentoAbreviado[0].required = true
            this.validacoesPOST.cidadeDeNascimentoIbge[0].required = true

            // retira a obrigação dos demais
            this.validacoesPOST.naturalizacaoData[0].required = false
            this.validacoesPOST.naturalizacaoPortaria[0].required = false

            this.validacoesPOST.paisDeNascimento[0].required = false
            this.validacoesPOST.dataEntradaNoPais[0].required = false
          } else {
            // adiciona os campos obrigatórios
            this.validacoesPUT.ufDeNascimentoAbreviado[0].required = true
            this.validacoesPUT.cidadeDeNascimentoIbge[0].required = true

            // retira a obrigação dos demais
            this.validacoesPUT.naturalizacaoData[0].required = false
            this.validacoesPUT.naturalizacaoPortaria[0].required = false

            this.validacoesPUT.paisDeNascimento[0].required = false
            this.validacoesPUT.dataEntradaNoPais[0].required = false
          }
        } else if (this.formIndividuo.nacionalidadeCidadao == 2) {
          if (this.metodo === 'POST') {
            // adiciona os campos obrigatórios
            this.validacoesPOST.naturalizacaoData[0].required = true
            this.validacoesPOST.naturalizacaoPortaria[0].required = true

            // retira a obrigação dos demais
            this.validacoesPOST.ufDeNascimentoAbreviado[0].required = false
            this.validacoesPOST.cidadeDeNascimentoIbge[0].required = false

            this.validacoesPOST.paisDeNascimento[0].required = false
            this.validacoesPOST.dataEntradaNoPais[0].required = false
          } else {
            // adiciona os campos obrigatórios
            this.validacoesPUT.naturalizacaoData[0].required = true
            this.validacoesPUT.naturalizacaoPortaria[0].required = true

            // retira a obrigação dos demais
            this.validacoesPUT.ufDeNascimentoAbreviado[0].required = false
            this.validacoesPUT.cidadeDeNascimentoIbge[0].required = false

            this.validacoesPUT.paisDeNascimento[0].required = false
            this.validacoesPUT.dataEntradaNoPais[0].required = false
          }
        } else {
          // opção 3- Estrangeiro
          if (this.metodo === 'POST') {
            // adiciona os campos obrigatórios
            this.validacoesPOST.paisDeNascimento[0].required = true
            this.validacoesPOST.dataEntradaNoPais[0].required = true

            // retira a obrigação dos demais
            this.validacoesPOST.ufDeNascimentoAbreviado[0].required = false
            this.validacoesPOST.cidadeDeNascimentoIbge[0].required = false

            this.validacoesPOST.naturalizacaoData[0].required = false
            this.validacoesPOST.naturalizacaoPortaria[0].required = false
          } else {
            // adiciona os campos obrigatórios
            this.validacoesPUT.paisDeNascimento[0].required = true
            this.validacoesPUT.dataEntradaNoPais[0].required = true

            // retira a obrigação dos demais
            this.validacoesPUT.ufDeNascimentoAbreviado[0].required = false
            this.validacoesPUT.cidadeDeNascimentoIbge[0].required = false

            this.validacoesPUT.naturalizacaoData[0].required = false
            this.validacoesPUT.naturalizacaoPortaria[0].required = false
          }
        }
      },

    }
  }
</script>

<style scoped>

  .scroll-area {
    position: relative;
    margin: auto;
    width: 100%;
    height: 250px;
    overflow-x: hidden;
  }
</style>
