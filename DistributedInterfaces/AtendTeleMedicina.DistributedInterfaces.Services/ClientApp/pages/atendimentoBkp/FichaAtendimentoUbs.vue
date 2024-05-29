<template>
  <div label="Atendimento">
    <el-form :model="formAtendimento" :rules="validacoes" labelPosition="top" ref="formAtendimento" label-width="170px">
      <el-card shadow="always" style="margin-top: 20px">

        <el-form-item label="Tipo de atendimento" prop="tipoAtendimento">
          <el-radio-group style="display: flex; flex-direction: row; flex-wrap: wrap;" v-model="formAtendimento.tipoAtendimento">
            <el-radio :label="'1'" style="width: 40%;">Consulta agendada programada / Cuidado continuado</el-radio>
            <el-radio :label="'2'" style="width: 20%;">Consulta agendada</el-radio>
            <el-radio :label="'4'" style="width: 40%;">Escuta inicial / Orientação</el-radio>
            <el-radio :label="'5'" style="width: 25%;">Consulta no dia</el-radio>
            <el-radio :label="'6'" style="width: 25%;">Atendimento de urgência</el-radio>
          </el-radio-group>
        </el-form-item>

        <el-form-item label="Subjetivo" prop="subjetivo">
          <el-input v-model="formAtendimento.subjetivo" :rows="1" type="textarea" placeholder="" autosize />
        </el-form-item>
        <el-form-item label="Objetivo" prop="objetivo">
          <el-input v-model="formAtendimento.objetivo" :rows="1" prop="objetivo" type="textarea" placeholder="" autosize />
        </el-form-item>
        <el-form-item label="Avaliação" prop="avaliacao">
          <el-input v-model="formAtendimento.avaliacao" :rows="1" type="textarea" placeholder="" autosize />
        </el-form-item>
      </el-card>

      <el-card shadow="always" style="margin-top: 20px">
        <component v-if="exibirExamesTabsAtendimento" :is="'exames-tabs'" :agendamento="agendamentoProps" @emit="retornoExamesTabs" />
      </el-card>

      <el-card shadow="always" style="margin-top: 20px">
        <el-form-item label="Problema / Condição Avaliada" prop="checkedCondAval">
          <el-checkbox-group v-model="formAtendimento.checkedCondAval" style="display: flex; flex-direction:row; flex-wrap: wrap;">
            <el-checkbox v-for="item in api.ciapCondAval" :label="item.codigo" :key="item.codigo" style="width: 25%;">{{item.descricao}}</el-checkbox>
          </el-checkbox-group>
        </el-form-item>
      </el-card>

      <el-card shadow="always" style="margin-top: 20px">
        <p>CID10</p>
        <el-form-item label="" prop="cid10">
          <el-select v-model="formAtendimento.cid10"
                     filterable
                     clearable
                     remote
                     :remote-method="searchCid10"
                     reserve-keyword
                     placeholder="Selecione">
            <el-option v-for="item in api.cid10"
                       :key="item.codigo"
                       :label="`Código: ${item.codigo} / Descrição: ${item.descricao.length > 120 ? item.descricao.substring(0, 120)+'...' : item.descricao}`"
                       :value="item.codigo">
            </el-option>
          </el-select>
        </el-form-item>

        <el-button style="margin-top:15px" type="primary" size="small" @click="onAddButtonCid" :disabled="formAtendimento.cid10Adicionados.length >= 2" icon="el-icon-plus">Adicionar</el-button>
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
                     remote
                     :remote-method="searchCiap"
                     reserve-keyword
                     placeholder="Selecione">
            <el-option v-for="item in api.ciap2"
                       :key="item.codigo"
                       :label="'Código: '+ item.codigo + ' /  Descrição: ' + item.descricao"
                       :value="item.codigo">
            </el-option>
          </el-select>
        </el-form-item>

        <el-button style="margin-top:15px" type="primary" size="small" @click="onAddButtonCiap" :disabled="formAtendimento.ciap2Adicionados.length >= 2" icon="el-icon-plus">Adicionar</el-button>
        <el-button type="danger" size="small" @click="limparCiap2" icon="el-icon-close">Remover</el-button>

        <el-table :data="ciap2Adicionados" border style="margin-top: 20px">
          <el-table-column prop="codCiap" label="CIAP2">
          </el-table-column>
          <el-table-column prop="descCiap" label="Descrição CIAP2">
          </el-table-column>
        </el-table>
      </el-card>



      <!--PLANO-->
      <el-card shadow="always" style="margin-top: 20px">
        <el-form-item label="Plano de Tratamento" prop="plano">
          <el-input v-model="formAtendimento.plano" :rows="1" type="textarea" placeholder="" autosize />
        </el-form-item>
      </el-card>

      <el-card shadow="always" style="margin-top: 20px">
        <el-row :gutter="24">
          <el-col :xs="12" :sm="12" :md="12" :lg="12" :xl="12">
            <el-form-item label="Conduta/Encaminhamento" prop="condutaDesfecho">
              <el-checkbox-group v-model="formAtendimento.condutaDesfecho">
                <h4>Conduta</h4>
                  <div  style="display:flex; flex-direction:row; gap: 20px">
                    <el-checkbox label="1">Retorno para consulta agendada</el-checkbox>
                    <el-checkbox label="3">Agendamento para NASF</el-checkbox>
                    <el-checkbox label="12">Agendamento para grupos</el-checkbox>
                  </div>
                  <div  style="display:flex; flex-direction:row; gap: 20px">  
                    <el-checkbox label="2">Retorno para consulta programada / cuidado continuado</el-checkbox>
                    <el-checkbox label="9">Alta do episódio</el-checkbox>
                  </div>                
                <h4>Encaminhamentos</h4>
                  <div  style="display:flex; flex-direction:row; gap: 20px">
                    <el-checkbox label="5">CAPS</el-checkbox>
                    <el-checkbox label="7">Urgência</el-checkbox>
                    <el-checkbox label="10">Intersetorial</el-checkbox>
                    <el-checkbox label="6">Internação Hospitalar</el-checkbox>
                    <el-checkbox label="8">Serviço de Atenção Domiciliar</el-checkbox>
                  </div>
              </el-checkbox-group>
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
  import ExamesTabs from '../../components/shared/ExamesTabs'
  export default {
    name: 'FichaAtendimentoUbs',
    props: {
      agendamentoProps: {}
    },
    components: { ExamesTabs },

    data () {
      return {
        // DADOS E CONFIGS
        agendamento: {},
        formAtendimento: {
          ciap2: [],
          cid10: [],
          checkedCondAval: [],
          ciap2Adicionados: [],
          cid10Adicionados: [],
          individuoCiap: [],
          individuoCid10: [],
          condutaDesfecho: []
        },
        ciap2Adicionados: [],
        cid10Adicionados: [],

        // DADOS API GERAIS
        api: {
          individuo: {},
          exames: [],
          ciap2: [],
          cid10: [],
          ciapCondAval: []
        },

        loading: {
          atendimentos: false
        },

        // CONTROLE DE FLAGS
        exibirExamesTabsAtendimento: true,

        // VALIDAÇÕES
        validacoes: {
          tipoAtendimento: [
            { required: true, message: 'Campo obrigatório', trigger: 'submit' }
          ],
          condutaDesfecho: [
            { required: true, message: 'Campo obrigatório', trigger: 'submit' }
          ],
          // subjetivo: [
          //  { required: true, message: 'Campo obrigatório', trigger: 'change' },
          //  { min: 3, message: 'Nome não pode conter menos de 4', trigger: 'submit' }
          // ],
          // objetivo: [
          //  { required: true, message: 'Campo obrigatório', trigger: 'change' },
          //  { min: 3, message: 'Nome não pode conter menos de 4', trigger: 'submit' }
          // ],
          avaliacao: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' },
            { min: 3, message: 'Nome não pode conter menos de 4', trigger: 'submit' }
          ]
          // plano: [
          //  { required: true, message: 'Campo obrigatório', trigger: 'change' },
          //  { min: 3, message: 'Nome não pode conter menos de 4 ', trigger: 'submit' }
          // ]
        },

        // PARAMS
        paramsCid10: {
          skip: 1,
          // take: 9999999,
          take: 10,
          sort: 'Descricao',
          total: 0
        },
        paramsCiap: {
          skip: 1,
          take: 10,
          // take: 9999999,
          sort: 'Descricao',
          total: 0
        },
        paramsCondAval: {
          skip: 1,
          take: 99,
          ai: true,
          // take: 9999999,
          sort: 'Descricao',
          total: 0
        }
      }
    },

    async mounted () {
      this.agendamento = (this.agendamentoProps != undefined ? this.agendamentoProps : {})

      await this.getPaciente()

      // Retorno da lista do cid e ciap
      await this.getCiapCondAval()
      await this.getCiap()
      await this.getCid10()
    },

    methods: {

      // RETORNANDO O OBJETO DO PACIENTE
      async getPaciente () {
        let { data, status } = await _api.individuos.getById(this.agendamento.individuoId)
        this.api.individuo = (status === 200) ? data : {}
      },

      // RETORNO DOS EXAMES DO COMPONENTE DOS EXAMES PARA A TELA PAI
      retornoExamesTabs (val) {
        // val.exames é o array com todos os exames do componente
        if (val.exames != undefined) {
          if (val.exames.length > 0) {
            this.api.exames = val.exames
          }
        }
      },

      // RETORNO DA LISTA DE CIAP PARA CONDIÇÃO AVALIADA DO BANCO
      async getCiapCondAval () {
        this.paramsCondAval.sexo = this.api.individuo.sexo === 0 ? 'M' : 'F'
        let { data, status } = await _api.ciap.get(this.paramsCondAval)
        this.api.ciapCondAval = (status === 200) ? data : []
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
          this.ciap2Adicionados.push({
            idCiap: findCiap2.id,
            descCiap: findCiap2.descricao,
            codCiap: findCiap2.codigo
          })
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
          this.cid10Adicionados.push({
            idCid10: findCid10.id,
            descCid10: findCid10.descricao,
            codCid10: findCid10.codigo
          })
        } else {
          this.$message({ message: `CID REPETIDO`, type: 'warning' })
        }
        this.formAtendimento.cid10Adicionados = this.cid10Adicionados
      },

      // UTILITARIO PARA ARRAY DE CID CIAP
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

      limparCid10 () {
        this.cid10Adicionados = []
        this.formAtendimento.cid10Adicionados = []
      },
      limparCiap2 () {
        this.ciap2Adicionados = []
        this.formAtendimento.ciap2Adicionados = []
      },

      // FUNÇÃO QUE É CHAMADA DENTRO DA FUNÇÃO DO FINALIZAR ATENDIMENTO (submitform) / ADICIONA O ITEM DE CID NO FORM ATENDIMENTO
      onAddCid10 () {
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

      // FUNÇÃO DO BOTÃO DE FINALIZAR ATENDIMENTO
      async submitForm (form) {
        if (this.api.exames.length > 0) {
          var examesGeral = this.api.exames.filter(exames => {
            if (exames.resultado == true && exames.finalizado == false) {
              exames.finalizado = true
              exames.profissionalId = this.$auth.user().id
              return this.api.exames
            }
          })

          examesGeral.forEach(async function (exames) {
            var avaliadoSplit = exames.avaliado.split('/')
            var avaliado = `${avaliadoSplit[2]}-${avaliadoSplit[1]}-${avaliadoSplit[0]}`
            exames.avaliado = avaliado.replaceAll('/', '-')
            let { status } = await _api.exames.putAvaliacao(exames)

            if (status != 204) {
              this.$message.warning('Avaliação De Exames Não Enviado')
            }
          })
        }

        // Verificação se tem um cid ou um ciap
        if (this.ciap2Adicionados.length === 0 && this.cid10Adicionados.length === 0) return this.$message.warning('É necessário o preenchimento de pelo menos um CID ou um CIAP')

        this.$refs[form].validate(async valid => {
          this.loading.atendimentos = true
          if (valid) {
            try {
              // Inserção Cid10
              await this.onAddCid10()
              // Inserção Ciap
              await this.onAddCiap()

              let putAgendamento = { id: this.agendamento.id, finalizado: true, emAtendimentoMedico: false }
              let { status } = await _api.agendamentos.putFinalizarMedico(putAgendamento)

              let atendimento = {
                agendamentoId: this.agendamento.id,
                atendidoMedico: true,
                avaliacao: this.formAtendimento.avaliacao,
                condutaDesfecho: this.formAtendimento.condutaDesfecho.join(','),
                condicoesAvaliadas: this.formAtendimento.checkedCondAval.join(','),
                individuoCiap: this.formAtendimento.individuoCiap,
                individuoCid10: this.formAtendimento.individuoCid10,
                objetivo: this.formAtendimento.objetivo,
                plano: this.formAtendimento.plano,
                subjetivo: this.formAtendimento.subjetivo,
                tempoAtendimento: this.tempoTotalDoAtendimento,
                tipoAtendimento: this.formAtendimento.tipoAtendimento,
                localDeAtendimento: 1
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
            this.$message.warning('Verifique o preenchimento dos campos obrigatórios')
            this.loading.atendimentos = false
          }
          this.loading.atendimentos = false
        })
      }
    }
  }
</script>
<style scoped>
</style>
