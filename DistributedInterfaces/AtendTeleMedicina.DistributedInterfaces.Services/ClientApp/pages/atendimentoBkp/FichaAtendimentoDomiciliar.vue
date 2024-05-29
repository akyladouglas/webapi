<template>
  <div label="Atendimento">
    <el-form :model="formAtendimento" :rules="validacoes" labelPosition="top" ref="formAtendimento" label-width="170px">
      <el-card shadow="always" style="margin-top: 20px">

        <el-form-item label="Modalidade AD" prop="modalidadeAD">
          <el-radio-group v-model="formAtendimento.modalidadeAD">
            <el-radio :label="'1'">AD1</el-radio>
            <el-radio :label="'2'">AD2</el-radio>
            <el-radio :label="'3'">AD3</el-radio>
          </el-radio-group>
        </el-form-item>

        <el-form-item label="Tipo de atendimento" prop="tipoAtendimento">
          <el-radio-group style="display:flex; flex-direction:row; gap: 5px" v-model="formAtendimento.tipoAtendimento" @change="selectTipoAtendimento">
            <el-radio :label="'7'">Atendimento programado</el-radio>
            <el-radio :label="'8'">Atendimento não programado</el-radio>
            <el-radio :label="'9'">Visita domiciliar pós-óbito</el-radio>
          </el-radio-group>
        </el-form-item>

        <el-form-item label="Condição(ões) avaliada(s)" prop="condicoesAvaliadas">
          <el-checkbox-group v-model="formAtendimento.condicoesAvaliadas" :disabled="formAtendimento.tipoAtendimento == 2" style="display: flex; flex-direction: row; flex-wrap: wrap;">
            <el-checkbox label="Acamado" style="width: 25%;"></el-checkbox>
            <el-checkbox label="Domiciliado" style="width: 25%;"></el-checkbox>
            <el-checkbox label="Úlceras/Feridas (grau III ou IV)" style="width: 25%;"></el-checkbox>
            <el-checkbox label="Acompanhamento nutricional" style="width: 25%;"></el-checkbox>
            <el-checkbox label="Uso de sonda nasogástrica - SNG" style="width: 25%;"></el-checkbox>
            <el-checkbox label="Uso de sonda nasoenteral - SNE" style="width: 25%;"></el-checkbox>
            <el-checkbox label="Uso de gastrostomia" style="width: 25%;"></el-checkbox>
            <el-checkbox label="Uso de colostomia" style="width: 25%;"></el-checkbox>
            <el-checkbox label="Uso de cistostomia" style="width: 25%;"></el-checkbox>
            <el-checkbox label="Uso de sonda vesical de demora - SVD" style="width: 25%;"></el-checkbox>
            <el-checkbox label="Acompanhamento pré-operatório" style="width: 25%;"></el-checkbox>
            <el-checkbox label="Acompanhamento pós-operatório" style="width: 25%;"></el-checkbox>
            <el-checkbox label="Adaptação ao uso de órtese/prótese" style="width: 25%;"></el-checkbox>
            <el-checkbox label="Reabilitação domiciliar" style="width: 25%;"></el-checkbox>
            <el-checkbox label="Cuidados paliativos não oncológicos" style="width: 25%;"></el-checkbox>
            <el-checkbox label="Oxigenoterapia domiciliar" style="width: 25%;"></el-checkbox>
            <el-checkbox label="Uso de traqueostomia" style="width: 25%;"></el-checkbox>
            <el-checkbox label="Uso de aspirador de vias aéreas para higiene brônquica" style="width: 25%;"></el-checkbox>
            <el-checkbox label="Suporte ventilatório não invasivo - CPAP" style="width: 25%;"></el-checkbox>
            <el-checkbox label="Suporte ventilatório não invasivo - BIPAP" style="width: 25%;"></el-checkbox>
            <el-checkbox label="Diálise peritonial" style="width: 25%;"></el-checkbox>
            <el-checkbox label="Parecentese" style="width: 25%;"></el-checkbox>
            <el-checkbox label="Medicação parenteral" style="width: 25%;"></el-checkbox>
          </el-checkbox-group>
        </el-form-item>

        <!--<el-form-item label="Avaliação" prop="avaliacao">
          <el-input v-model="formAtendimento.avaliacao" :rows="4" type="textarea" placeholder="" />
        </el-form-item>-->
      </el-card>

      <!--<el-card shadow="always" style="margin-top: 20px">
        <component v-if="exibirExamesTabsAtendimento" :is="'exames-tabs'" :agendamento="agendamento" @emit="retornoExamesTabs" />
      </el-card>-->


      <el-card shadow="always" style="margin-top: 20px">
        <p>CID10</p>
        <el-form-item label="" prop="cid10">
          <el-select v-model="formAtendimento.cid10"
                     filterable
                     clearable
                     :remote-method="searchCid10"
                     remote
                     reserve-keyword
                     placeholder="Selecione">
            <el-option v-for="item in api.cid10"
                       :key="item.codigo"
                       :label="`Código: ${item.codigo} / Descrição: ${item.descricao.length > 120 ? item.descricao.substring(0, 120)+'...' : item.descricao}`"
                       :value="item.codigo">
            </el-option>
          </el-select>
        </el-form-item>

        <el-button style="margin-top:15px" type="primary" size="small" @click="onAddButtonCid" :disabled="formAtendimento.cid10Adicionados.length >= 1" icon="el-icon-plus">Adicionar</el-button>
        <el-button type="danger" size="small" @click="limparCid10" icon="el-icon-close">Remover</el-button>

        <el-table :data="cid10Adicionados" border style="margin-top: 20px">
          <el-table-column prop="codCid10" label="CID10">
          </el-table-column>
          <el-table-column prop="descCid10" label="Descrição CID10">
          </el-table-column>
        </el-table>
      </el-card>

      <el-card shadow="always" style="margin-top: 20px">
          <p>CIAP2</p>
          <el-form-item label="" prop="ciap2">
            <el-select v-model="formAtendimento.ciap2"
                       filterable
                       clearable
                       :remote-method="searchCiap"
                       remote
                       reserve-keyword
                       placeholder="Selecione">
              <el-option v-for="item in api.ciap2"
                         :key="item.codigo"
                         :label="'Código: '+ item.codigo + ' /  Descrição: ' + item.descricao"
                         :value="item.codigo">
              </el-option>
            </el-select>
          </el-form-item>

        <el-button style="margin-top:15px" type="primary" size="small" @click="onAddButtonCiap" :disabled="formAtendimento.ciap2Adicionados.length >= 1" icon="el-icon-plus">Adicionar</el-button>
        <el-button type="danger" size="small" @click="limparCiap2" icon="el-icon-close">Remover</el-button>

        <el-table :data="ciap2Adicionados" border style="margin-top: 20px">
          <el-table-column prop="codCiap" label="CIAP2">
          </el-table-column>
          <el-table-column prop="descCiap" label="Descrição CIAP2">
          </el-table-column>
        </el-table>
      </el-card>

      <el-card shadow="always" style="margin-top: 20px">
        <p>Procedimentos (Código do SIGTAP)</p>
        <el-form-item label="" prop="procedimentos">
          <el-select v-model="formAtendimento.procedimentos"
                     filterable
                     clearable
                     :remote-method="searchProcedimentos"
                     remote
                     reserve-keyword
                     placeholder="Selecione"
                     :disabled="formAtendimento.tipoAtendimento == 2">
            <el-option v-for="item in api.procedimentos"
                       :key="item.codigo"
                       :label="`Código: ${item.codigo} / Descrição: ${item.descricao.length > 120 ? item.descricao.substring(0, 120)+'...' : item.descricao}`"
                       :value="item.codigo">
            </el-option>
          </el-select>
        </el-form-item>

        <el-button :disabled="formAtendimento.tipoAtendimento == 2" style="margin-top:15px" type="primary" size="small" @click="onAddButtonProcedimento" icon="el-icon-plus">Adicionar</el-button>
        <el-button :disabled="formAtendimento.tipoAtendimento == 2" type="danger" size="small" @click="limparProcedimento" icon="el-icon-close">Limpar Procedimentos</el-button>

        <el-table :data="procedimentosAdicionados" border style="margin-top: 20px" :disabled="formAtendimento.tipoAtendimento == 2">
          <el-table-column prop="codigo" label="Código">
          </el-table-column>
          <el-table-column prop="descricao" label="Descrição">
          </el-table-column>
        </el-table>
      </el-card>

      <el-card shadow="always" style="margin-top: 20px">
        <el-row :gutter="24">
          <el-col :xs="12" :sm="12" :md="12" :lg="12" :xl="12">
            <el-form-item label="Conduta/Desfecho" prop="condutaDesfecho">
              <el-radio-group style="display:flex; flex-direction:column; gap: 5px" v-model="formAtendimento.condutaDesfecho">
                <el-radio :label="'7'">Permanência</el-radio>
                <el-radio :label="'3'">Alta administrativa</el-radio>
                <el-radio :label="'1'">Alta clínica</el-radio>
                <el-radio :label="'9'">Óbito</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
          <el-col :xs="12" :sm="12" :md="12" :lg="12" :xl="12">
            <el-form-item label="Encaminhamento" prop="condutaDesfecho">
              <el-radio-group style="display:flex; flex-direction:column; gap: 20px" v-model="formAtendimento.condutaDesfecho">
                <el-radio :label="'2'">Atenção Básica (AD1)	</el-radio>
                <el-radio :label="'4'">Serviço de urgência e emergência	</el-radio>
                <el-radio :label="'5'">Serviço de internação hospitalar	</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
      </el-card>
    </el-form>
    <div style="margin-top: 20px">
      <el-button type="primary" size="large" icon="el-icon-check" v-show="!loading.atendimentos" @click="submitForm('formAtendimento')">Finalizar Atendimento</el-button>
      <el-button v-show="loading.atendimentos" type="info" icon="fa fa-spinner fa-spin" disabled> Finalizando...</el-button>
    </div>
  </div>
</template>
<script>
  import _api from '../../api'
  import _enums from '../../api/Enums'

  export default {
    name: 'FichaAtendimentoUbs',
    props: {
      agendamentoProps: {}
    },

    data () {
      return {
        // DADOS E CONFIGS
        agendamento: {},
        ciap2Adicionados: [],
        cid10Adicionados: [],
        procedimentosAdicionados: [],

        formAtendimento: {
          individuoCiap: [],
          individuoCid10: [],
          individuoProcedimentos: [],
          condicoesAvaliadas: [],
          cid10Adicionados: [],
          ciap2Adicionados: []
        },

        loading: {
          atendimentos: false
        },

        enums: {
          condicoesAvaliadas: [
            ..._enums.condicoesAvaliadas
          ]
        },

        // DADOS API GERAIS
        api: {
          individuo: {},
          ciap2: [],
          cid10: [],
          procedimentos: []
        },

        validacoes: {
          modalidadeAD: [
            { required: true, message: 'Campo obrigatório', trigger: 'submit' }
          ],
          tipoAtendimento: [
            { required: true, message: 'Campo obrigatório', trigger: 'submit' }
          ],
          condicoesAvaliadas: [
            { required: true, message: 'Campo obrigatório', trigger: 'submit' }
          ],
          condutaDesfecho: [
            { required: true, message: 'Campo obrigatório', trigger: 'submit' }
          ]
          // avaliacao: [
          //  { required: true, message: 'Campo obrigatório', trigger: 'change' },
          //  { min: 3, message: 'Nome não pode conter menos de 4', trigger: 'submit' }
          // ],
          // ciap2: [{ required: true, message: 'Campo obrigatório', trigger: ['blur', 'change'] }],
          // cid10: [{ required: true, message: 'Campo obrigatório', trigger: ['blur', 'change'] }]
        },

        // PARAMS
        paramsCid10: {
          skip: 1,
          take: 10,
          sort: 'Descricao ASC',
          total: 0
        },
        paramsCiap: {
          skip: 1,
          take: 10,
          ad: true,
          sort: 'Descricao ASC',
          total: 0
        },
        paramsAllProcedimentos: {
          skip: 1,
          gruposProcedimento: `'1','2','3','4'`,
          take: 10,
          total: 0
        }
      }
    },

    async mounted () {
      this.agendamento = (this.agendamentoProps != undefined ? this.agendamentoProps : {})

      await this.getPaciente()

      // Retorno da lista do cid e ciap
      await this.getCiap()
      await this.getCid10()

      // Retorno da lista dos procedimentos
      await this.getProcedimentos()
    },

    methods: {

      // RETORNANDO O OBJETO DO PACIENTE
      async getPaciente () {
        let { data, status } = await _api.individuos.getById(this.agendamento.individuoId)
        this.api.individuo = (status === 200) ? data : {}
      },

      // RETORNO DA LISTA DE PROCEDIMENTOS
      async getProcedimentos () {
        this.paramsAllProcedimentos.sexo = this.api.individuo.sexo === 0 ? 'M' : 'F'
        let { data, status } = await _api.procedimentos.get(this.paramsAllProcedimentos)
        if (status === 200) this.api.procedimentos = data
      },

      // RETORNO DA LISTA DE CIAP DO BANCO
      async getCiap () {
        this.paramsCiap.sexo = this.api.individuo.sexo === 0 ? 'M' : 'F'
        let { data, status } = await _api.ciap.get(this.paramsCiap)
        this.api.ciap2 = (status === 200) ? data : []
      },

      // RETORNO DA LISTA DE CID DO BANCO
      async getCid10 () {
        this.paramsCid10.sexo = this.api.individuo.sexo === 0 ? 'M' : 'F'
        let { data, status } = await _api.cid10.get(this.paramsCid10)
        this.api.cid10 = (status === 200) ? data : []
      },

      // FUNÇÃO DO CLICK NO CIAP PARA ADICIONAR NA TABLE
      onAddButtonCiap () {
        var findCiap = this.findInArray(this.api.ciap2, this.formAtendimento.ciap2)
        var objCiap = {
          codigo: findCiap[0].codigo,
          descricao: findCiap[0].descricao,
          id: findCiap[0].id
        }
        var findCiap2 = objCiap
        var returnCiap2 = this.checkObjCiap2(this.ciap2Adicionados, findCiap2.id)
        if (returnCiap2 == false) {
          this.ciap2Adicionados = [{
            idCiap: findCiap2.id,
            descCiap: findCiap2.descricao,
            codCiap: findCiap2.codigo
          }]
        } else {
          this.$message({ message: `CIAP REPETIDO`, type: 'warning' })
        }
        this.formAtendimento.ciap2Adicionados = this.ciap2Adicionados
      },

      // FUNÇÃO DO CLICK NO CID PARA ADICIONAR NA TABLE
      onAddButtonCid () {
        var findCid = this.findInArray(this.api.cid10, this.formAtendimento.cid10)
        var objCid = {
          codigo: findCid[0].codigo,
          descricao: findCid[0].descricao,
          id: findCid[0].id
        }
        var findCid10 = objCid
        var returnCid10 = this.checkObjCid10(this.cid10Adicionados, findCid10.id)
        if (returnCid10 == false) {
          this.cid10Adicionados = [{
            idCid10: findCid10.id,
            descCid10: findCid10.descricao,
            codCid10: findCid10.codigo
          }]
        } else {
          this.$message({ message: `CID REPETIDO`, type: 'warning' })
        }
        this.formAtendimento.cid10Adicionados = this.cid10Adicionados
      },

      // FUNÇÃO DO CLICK NO PROCEDIMENTO PARA ADICIONAR NA TABLE
      onAddButtonProcedimento () {
        var findProcedimento = this.findInArray(this.api.procedimentos, this.formAtendimento.procedimentos)
        if (findProcedimento.length === 0) return this.$message.warning('Erro ao adicionar o procedimento a lista')
        var objProcedimento = {
          codigo: findProcedimento[0].codigo,
          descricao: findProcedimento[0].descricao,
          id: findProcedimento[0].id
        }

        var returnProcedimento = this.checkObjProcedimento(this.procedimentosAdicionados, objProcedimento.id)
        if (returnProcedimento == false) {
          this.procedimentosAdicionados.push({
            id: objProcedimento.id,
            descricao: objProcedimento.descricao,
            codigo: objProcedimento.codigo
          })
        } else {
          this.$message({ message: `Procedimento repetido`, type: 'warning' })
        }
        this.formAtendimento.procedimentosAdicionados = this.procedimentosAdicionados
      },

      findInArray (arr, field) {
        return arr.filter(item => item.codigo === field)
      },

      checkObjCiap2 (arr, id) {
        let hasIdCiap2 = arr.some(item => item['idCiap'] === id)
        return hasIdCiap2
      },

      checkObjCid10 (arr, id) {
        let hasIdCid10 = arr.some(item => item['idCid10'] === id)
        return hasIdCid10
      },

      checkObjProcedimento (arr, id) {
        let hasIdProcedimento = arr.some(item => item['id'] === id)
        return hasIdProcedimento
      },

      limparCid10 () {
        this.cid10Adicionados = []
        this.formAtendimento.cid10Adicionados = []
      },
      limparCiap2 () {
        this.ciap2Adicionados = []
        this.formAtendimento.ciap2Adicionados = []
      },
      limparProcedimento () {
        this.procedimentosAdicionados = []
        this.formAtendimento.procedimentosAdicionados = []
      },

      // SELEÇÃO DE OPÇÕES DE TIPO ATENDIMENTO
      selectTipoAtendimento (val) {
        if (val === 2) {
          this.validacoes.condicoesAvaliadas[0].required = false
        } else {
          this.validacoes.condicoesAvaliadas[0].required = true
        }
      },
      async searchCid10 (query) {
        if (query.length >= 3) {
          this.paramsCid10 = {
            ...this.paramsCid10,
            descricao: query,
            codigo: query
          }
          let { data, status } = await _api.cid10.get(this.paramsCid10)
          this.api.cid10 = (status === 200) ? data : []
        }
      },
      async searchCiap (query) {
        if (query.length >= 3) {
          this.paramsCiap = {
            ...this.paramsCiap,
            descricao: query,
            codigo: query
          }
          let { data, status } = await _api.ciap.get(this.paramsCiap)
          this.api.ciap2 = (status === 200) ? data : []
        }
      },
      async searchProcedimentos (query) {
        if (query.length >= 3) {
          this.paramsAllProcedimentos = {
            ...this.paramsAllProcedimentos,
            descricao: query,
            codigo: query
          }
          let { data, status } = await _api.procedimentos.get(this.paramsAllProcedimentos)
          if (status === 200) this.api.procedimentos = data
        }
      },

      // FUNÇÃO QUE É CHAMADA DENTRO DA FUNÇÃO DO FINALIZAR ATENDIMENTO (submitform) / ADICIONA O ITEM DE CID NO FORM ATENDIMENTO
      onAddCid10 () {
        if (this.formAtendimento.cid10Adicionados.length > 0) {
          let cid10Filtered = this.selectIdCid10(this.formAtendimento.cid10Adicionados)
          var arr = cid10Filtered
          try {
            arr.map(item => {
              this.formAtendimento.individuoCid10.push({
                agendamentoId: this.agendamento.id,
                individuoId: this.agendamento.individuoId,
                profissionalId: this.agendamento.profissionalId,
                cid10Id: item
              })
            })
          } catch (e) {
            this.$message.warning('Ocorreu um erro ao tentar registrar o CID10')
          }
        }
      },

      // RETORNA O ID DO CID PARA A FUNÇÃO ONADDCID10()
      selectIdCid10 (arr) {
        let arrFilteredCid10 = []
        if (this.formAtendimento.cid10Adicionados) {
          this.formAtendimento.cid10Adicionados.map(function (item) {
            if (item.idCid10) {
              return arrFilteredCid10.push(item.idCid10)
            } else { }
          })
        } else { }

        var arr = arrFilteredCid10
        return arr
      },

      // FUNÇÃO QUE É CHAMADA DENTRO DA FUNÇÃO DO FINALIZAR ATENDIMENTO (submitform) / ADICIONA O ITEM DE CIAP NO FORM ATENDIMENTO
      onAddCiap () {
        if (this.formAtendimento.ciap2Adicionados.length > 0) {
          let ciapFiltered = this.selectIdCiap(this.formAtendimento.ciap2Adicionados)
          var arr = ciapFiltered
          try {
            arr.map(item => {
              this.formAtendimento.individuoCiap.push({
                agendamentoId: this.agendamento.id,
                individuoId: this.agendamento.individuoId,
                profissionalId: this.agendamento.profissionalId,
                ciapId: item
              })
            })
          } catch (e) {
            this.$message.warning('Ocorreu um erro ao tentar registrar o Ciap')
          }
        }
      },

      // RETORNA O ID DO CIAP PARA A FUNÇÃO ONADDCIAP()
      selectIdCiap (arr) {
        let arrFiltered = []
        if (this.formAtendimento.ciap2Adicionados) {
          this.formAtendimento.ciap2Adicionados.map(function (item) {
            if (item.idCiap) {
              arrFiltered.push(item.idCiap)
            } else { }
          })
        }
        var arr = arrFiltered
        return arr
      },

      // FUNÇÃO QUE É CHAMADA DENTRO DA FUNÇÃO DO FINALIZAR ATENDIMENTO (submitform) / ADICIONA O ITEM DE PROCEDIMENTO NO FORM ATENDIMENTO
      onAddProcedimento () {
        if (this.formAtendimento.procedimentosAdicionados.length > 0) {
          let procedimentoFiltered = this.selectCodigoProcedimento(this.formAtendimento.procedimentosAdicionados)
          var arr = procedimentoFiltered
          try {
            arr.map(item => {
              this.formAtendimento.individuoProcedimentos.push({
                agendamentoId: this.agendamento.id,
                individuoId: this.agendamento.individuoId,
                profissionalId: this.$auth.user().id,
                procedimentoCodigo: item
              })
            })
          } catch (e) {
            this.$message.warning('Ocorreu um erro ao tentar registrar os procedimentos')
          }
        }
      },

      // RETORNA O ID DO CIAP PARA A FUNÇÃO ONADDCIAP()
      selectCodigoProcedimento (arr) {
        let arrFiltered = []
        if (this.formAtendimento.procedimentosAdicionados) {
          this.formAtendimento.procedimentosAdicionados.map(function (item) {
            if (item.codigo) {
              arrFiltered.push(item.codigo)
            } else { }
          })
        }
        var arr = arrFiltered
        return arr
      },

      findCondicoesAvaliadasEnum (string) {
        var stringRetorno = ''
        this.enums.condicoesAvaliadas.forEach(item => {
          if (item.label === string) {
            stringRetorno = item.value
          } else {
            return null // Caso a string não seja encontrada
          }
        })
        return stringRetorno
      },

      // FUNÇÃO DO BOTÃO DE FINALIZAR ATENDIMENTO
      async submitForm (form) {
        /// / Verificação se tem um cid ou um ciap
        // if (this.ciap2Adicionados.length === 0 && this.cid10Adicionados.length === 0) return this.$message.warning('É necessário o preenchimento de pelo menos um CID ou um CIAP')

        if (this.formAtendimento.tipoAtendimento != 2 && this.procedimentosAdicionados.length === 0) return this.$message.warning('É necessário o preenchimento de pelo menos um Procedimento')

        this.$refs[form].validate(async valid => {
          this.loading.atendimentos = true
          if (valid) {
            try {
              // criando as strings
              if (this.formAtendimento.tipoAtendimento != 2) {
                // string do condicoes avaliadas separado por ,
                var arrCondicoesAvaliadas = []
                this.formAtendimento.condicoesAvaliadas.forEach(item => {
                  var value = this.findCondicoesAvaliadasEnum(item)
                  arrCondicoesAvaliadas.push(value)
                })
                var stringFinal = arrCondicoesAvaliadas.join(',')
                this.formAtendimento.condicoesAvaliadas = stringFinal
              }

              // Inserção Cid10
              await this.onAddCid10()
              // Inserção Ciap
              await this.onAddCiap()
              // Inserção Procedimento
              await this.onAddProcedimento()

              let putAgendamento = { id: this.agendamento.id, finalizado: true, emAtendimentoMedico: false }
              let { status } = await _api.agendamentos.putFinalizarMedico(putAgendamento)

              let atendimento = {
                agendamentoId: this.agendamento.id,
                atendidoMedico: true,
                condicoesAvaliadas: this.formAtendimento.condicoesAvaliadas,
                condutaDesfecho: this.formAtendimento.condutaDesfecho,
                individuoCiap: this.formAtendimento.individuoCiap,
                individuoCid10: this.formAtendimento.individuoCid10,
                modalidadeAD: this.formAtendimento.modalidadeAD,
                individuoProcedimentos: this.formAtendimento.individuoProcedimentos,
                tempoAtendimento: this.tempoTotalDoAtendimento,
                tipoAtendimento: this.formAtendimento.tipoAtendimento,
                localDeAtendimento: 4
              }
              let { data: dataPostMedico, status: statusPostMedico } = await _api.atendimentos.postMedico(atendimento)
              if (statusPostMedico === 201) {
                this.loading.atendimentos = false
                this.$emit('emit', { atendimentoFinalizado: true, atendimentoId: dataPostMedico.id })
              }

              this.loading.atendimentos = false
            } catch (e) {
              console.log(e)
              this.$message.error('Erro no registro do atendimento')
              this.loading.atendimentos = false
            }
          } else {
            this.loading.atendimentos = false
            this.$message.warning('Verifique o preenchimento dos campos obrigatórios')
          }
        })
      }

    }
  }
</script>
<style scoped>
</style>
