<template>
  <div label="Atendimento">
    <el-form :model="formAtendimento" :rules="validacoes" labelPosition="top" ref="formAtendimento" label-width="170px">
      <el-card shadow="always" style="margin-top: 20px">

        <el-form-item label="Tipo de atendimento" prop="tipoAtendimento">
          <el-radio-group style="display: flex; flex-direction: row; flex-wrap: wrap;" v-model="formAtendimento.tipoAtendimento" @change="selectTipoAtendimento">
            <el-radio :label="'1'">Consulta agendada programada/Cuidado continuado</el-radio>
            <el-radio :label="'2'">Consulta agendada</el-radio>
            <el-radio :label="'4'">Esculta inicial/Orientação</el-radio>
            <el-radio :label="'5'">Consulta no dia</el-radio>
            <el-radio :label="'6'">Atendimento de urgência</el-radio>
          </el-radio-group>
        </el-form-item>

      </el-card>
      <el-card shadow="always" style="margin-top: 20px">

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
        <component v-if="showExamesTabs" :is="'exames-tabs'" :agendamento="agendamento" />
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
                       :label="formatLabel(item)"
                       :value="item.codigo">
            </el-option>
          </el-select>
        </el-form-item>

        <el-button style="margin-top:15px" type="primary" size="small" @click="onAddButtonCid" icon="el-icon-plus">Adicionar</el-button>
        <el-button type="danger" size="small" @click="limparCid10" icon="el-icon-close">Remover</el-button>

        <el-table :data="cid10Adicionados" border style="margin-top: 20px">
          <el-table-column prop="codCid10" label="CID10"></el-table-column>
          <el-table-column prop="descCid10" label="Descrição CID10"></el-table-column>
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
                <div style="display:flex; flex-direction:row; gap: 20px">
                  <el-checkbox label="1">Retorno para consulta agendada</el-checkbox>
                  <el-checkbox label="3">Agendamento para NASF</el-checkbox>
                  <el-checkbox label="12">Agendamento para grupos</el-checkbox>
                </div>
                <div style="display:flex; flex-direction:row; gap: 20px">
                  <el-checkbox label="2">Retorno para consulta programada / cuidado continuado</el-checkbox>
                  <el-checkbox label="9">Alta do episódio</el-checkbox>
                </div>
                <h4>Encaminhamentos</h4>
                <div style="display:flex; flex-direction:row; gap: 20px">
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
  import _api from '../../../api'
  import _enums from '../../../api/Enums'
  import ExamesTabs from '../../../components/shared/ExamesTabs'
  export default {
    name: 'FichaAtendimentoUbs',
    props: {
      agendamento: {},
      sintomas: {},
      countTimer: '',
      inicioDoAtendimento: '',
      fimDoAtendimento: '',
      existSintomas: Boolean
    },
    components: { ExamesTabs },

    data() {
      return {
        // DADOS E CONFIGS
        formAtendimento: {
          ciap2: [],
          cid10: [],
          checkedCondAval: [],
          ciap2Adicionados: [],
          cid10Adicionados: [],
          individuoCiap: [],
          individuoCid10: [],
          condutaDesfecho: [],
        },
        ciap2Adicionados: [],
        cid10Adicionados: [],

        // DADOS API GERAIS
        api: {
          exames: [],
          ciap2: [],
          cid10: [],
          ciapCondAval: []
        },

        loading: {
          atendimentos: false
        },

        enums: {
          condicoesAvaliadas: [
            ..._enums.condicoesAvaliadas
          ]
        },

        //SCREENS CONTROL
        showExamesTabs: true,

        // VALIDAÇÕES
        validacoes: {
          subjetivo: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' },
            { min: 3, message: 'Subjetivo não pode conter menos de 4 caracteres', trigger: 'submit' }
          ],
          objetivo: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' },
            { min: 3, message: 'Objetivo não pode conter menos de 4 caracteres', trigger: 'submit' }
          ],
          avaliacao: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' },
            { min: 3, message: 'Avaliacao não pode conter menos de 4 caracteres', trigger: 'submit' }
          ],
          plano: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' },
            { min: 3, message: 'Plano não pode conter menos de 4 caracteres', trigger: 'submit' }
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
        },

        // PARAMS
        paramsCid10: {
          skip: 1,
          take: 10,
          sort: 'Descricao',
          total: 0
        },
        paramsCiap: {
          skip: 1,
          take: 10,
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

    async mounted() {
      await this.getPaciente()
      // Retorno da lista do cid e ciap
      await this.getCiapCondAval()
      await this.getCiap()
      await this.getCid10()

    },

    methods: {
      //FORMATAR A LABEL DO CID10
      formatLabel(item) {
        return `Código: ${item.codigo} / Descrição: ${item.descricao.length > 120 ? item.descricao.substring(0, 120) + '...' : item.descricao}`;
      },

      // RETORNANDO O OBJETO DO PACIENTE
      async getPaciente() {
        let { data, status } = await _api.individuos.getById(this.agendamento.individuoId)
        this.api.individuo = (status === 200) ? data : {}
      },

      // RETORNO DA LISTA DE CIAP PARA CONDIÇÃO AVALIADA DO BANCO
      async getCiapCondAval() {
        this.paramsCondAval.sexo = this.api.individuo.sexo === 0 ? 'M' : 'F'
        let { data, status } = await _api.ciap.get(this.paramsCondAval)
        this.api.ciapCondAval = (status === 200) ? data : []
      },

      // RETORNO DA LISTA DE CIAP DO BANCO
      async getCiap() {
        this.paramsCiap.sexo = (this.agendamento.individuo) ? (this.agendamento.individuo.sexo === 0 ? 'M' : 'F') : ''
        let { data, status } = await _api.ciap.get(this.paramsCiap)
        this.api.ciap2 = (status === 200) ? data : []
      },

      // RETORNO DA LISTA DE CID DO BANCO
      async getCid10() {
        this.paramsCid10.sexo = (this.agendamento.individuo) ? (this.agendamento.individuo.sexo === 0 ? 'M' : 'F') : ''
        let { data, status } = await _api.cid10.get(this.paramsCid10);
        this.api.cid10 = (status === 200) ? data : [];
      },

      // FUNÇÃO DO CLICK NO CIAP PARA ADICIONAR NA TABLE
      onAddButtonCiap() {
        const selectedCiap = this.api.ciap2.find(ciap => ciap.codigo === this.formAtendimento.ciap2);
        if (!this.isCiap2Added(selectedCiap.id)) {
          this.ciap2Adicionados.push({
            idCiap: selectedCiap.id,
            descCiap: selectedCiap.descricao,
            codCiap: selectedCiap.codigo
          });
          this.formAtendimento.ciap2Adicionados = this.ciap2Adicionados;
        } else {
          this.$swal({
            title: "Atenção!",
            text: 'O ciap está repetido selecione outro!',
            icon: 'warning',
          })

        }
      },

      // FUNÇÃO DO CLICK NO CID PARA ADICIONAR NA TABLE
      onAddButtonCid() {
        const selectedCid = this.api.cid10.find(cid => cid.codigo === this.formAtendimento.cid10);
        if (!this.isCid10Added(selectedCid.id)) {
          this.cid10Adicionados.push({
            idCid10: selectedCid.id,
            descCid10: selectedCid.descricao,
            codCid10: selectedCid.codigo
          });
          this.formAtendimento.cid10Adicionados = this.cid10Adicionados;
        } else {
          this.$swal({
            title: "Atenção!",
            text: 'O cid está repetido selecione outro!',
            icon: 'warning',
          })
        }
      },

      // UTILITARIO PARA ARRAY DE CID CIAP
      findInArray(arr, field) {
        return arr.filter(item => item.codigo === field)
      },

      //CID
      isCid10Added(id) {
        return this.cid10Adicionados.some(item => item.idCid10 === id);
      },

      limparCid10() {
        this.cid10Adicionados = [];
        this.formAtendimento.cid10Adicionados = [];
      },

      //CIAP
      isCiap2Added(id) {
        return this.ciap2Adicionados.some(item => item.idCiap === id);
      },

      limparCiap2() {
        this.ciap2Adicionados = []
        this.formAtendimento.ciap2Adicionados = []
      },

      // FUNÇÃO QUE É CHAMADA DENTRO DA FUNÇÃO DO FINALIZAR ATENDIMENTO (submitform) / ADICIONA O ITEM DE CID NO FORM ATENDIMENTO
      onAddCid10() {
        const cid10Ids = this.formAtendimento.cid10Adicionados.map(item => item.idCid10);
        cid10Ids.forEach(cid10Id => {
          this.formAtendimento.individuoCid10.push({
            agendamentoId: this.agendamento.id,
            individuoId: this.agendamento.individuoId,
            profissionalId: this.agendamento.profissionalId,
            cid10Id: cid10Id
          });
        });
      },

      // RETORNA O ID DO CID PARA A FUNÇÃO ONADDCID10()
      selectIdCid10(arr) {
        let arrFilteredCid10 = []
        if (this.formAtendimento.cid10Adicionados) {
          this.formAtendimento.cid10Adicionados.map(function (item) {
            if (item.idCid10) {
              return arrFilteredCid10.push(item.idCid10)
            }
          })
        }

        var arr = arrFilteredCid10
        return arr
      },

      // FUNÇÃO QUE É CHAMADA DENTRO DA FUNÇÃO DO FINALIZAR ATENDIMENTO (submitform) / ADICIONA O ITEM DE CIAP NO FORM ATENDIMENTO
      onAddCiap() {
        const ciapIds = this.formAtendimento.ciap2Adicionados.map(item => item.idCiap);
        ciapIds.forEach(ciapId => {
          this.formAtendimento.individuoCiap.push({
            agendamentoId: this.agendamento.id,
            individuoId: this.agendamento.individuoId,
            profissionalId: this.agendamento.profissionalId,
            ciapId: ciapId
          });
        });
      },

      // RETORNA O ID DO CIAP PARA A FUNÇÃO ONADDCIAP()
      selectIdCiap(arr) {

        let arrFilteredCiap = []
        if (this.formAtendimento.ciap2Adicionados) {
          this.formAtendimento.ciap2Adicionados.map(function (item) {
            if (item.idCiap) {
              return arrFilteredCiap.push(item.idCiap)
            } else { }
          })
        } else { }

        var arr = arrFilteredCiap
        return arr

      },

      // PROCURANDO OPÇÕES NA API DE CID10
      async searchCid10(query) {

        const paramsCid10 = { ...this.paramsCid10 }
        if (query.length >= 3) {
          paramsCid10.descricao = query;
          paramsCid10.codigo = query;
        }

        let { data, status } = await _api.cid10.get(paramsCid10)

        this.api.cid10 = (status === 200) ? data : []
        return;
      },

      // PROCURANDO OPÇÕES NA API DE CIAP
      async searchCiap(query) {
        const paramsCiap = { ...this.paramsCiap }
        if (query.length >= 3) {
          paramsCiap.descricao = query;
          paramsCiap.codigo = query;
        }
        let { data, status } = await _api.ciap.get(paramsCiap)
        this.api.ciap2 = (status === 200) ? data : []
        return;

      },

      // SELEÇÃO DE OPÇÕES DE TIPO ATENDIMENTO
      selectTipoAtendimento(val) {
        if (val === 2) {
          this.validacoes.condicoesAvaliadas[0].required = false
        } else {
          this.validacoes.condicoesAvaliadas[0].required = true
        }
      },

      findCondicoesAvaliadasEnum(string) {
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
      async submitForm(form) {

        // if (!this.inicioDoAtendimento || !this.fimDoAtendimento) {
          // return this.$swal({
            // title: "Atenção!",
           // text: 'É necessário realizar a teleconsulta antes de finalizar o atendimento!',
           // icon: 'warning',
          //})
        // }

        // Verificação se tem um cid ou um ciap
        if (this.ciap2Adicionados.length === 0 && this.cid10Adicionados.length === 0) {
          return this.$swal({
            title: "Atenção!",
            text: 'É necessário o preenchimento de pelo menos um Procedimento!',
            icon: 'warning',
          })
        }

        this.$refs[form].validate(async valid => {
          this.loading.atendimentos = true
          if (valid) {
            try {
              //if (this.formAtendimento.tipoAtendimento != 2) {
              //    // string do condicoes avaliadas separado por ,
              //    var arrCondicoesAvaliadas = []
              //    this.formAtendimento.condicoesAvaliadas.forEach(item => {
              //        var value = this.findCondicoesAvaliadasEnum(item)
              //        arrCondicoesAvaliadas.push(value)
              //    })
              //    var stringFinal = arrCondicoesAvaliadas.join(',')
              //    this.formAtendimento.condicoesAvaliadas = stringFinal
              //}
              // Inserção Cid10
              await this.onAddCid10()
              // Inserção Ciap
              await this.onAddCiap()

              let { status } = await _api.agendamentos.putFinalizarMedico({ id: this.agendamento.id, finalizado: true, emAtendimentoMedico: false })

              console.log('Condições avaliadas: ', this.formAtendimento.checkedCondAval.join(','));

              let atendimento = {
                agendamentoId: this.agendamento.id,
                atendidoMedico: true,
                condicoesAvaliadas: this.formAtendimento.checkedCondAval.join(','),
                condutaDesfecho: this.formAtendimento.condutaDesfecho.join(','),
                avaliacao: this.formAtendimento.avaliacao,
                individuoCiap: this.formAtendimento.individuoCiap,
                individuoCid10: this.formAtendimento.individuoCid10,
                objetivo: this.formAtendimento.objetivo,
                plano: this.formAtendimento.plano,
                subjetivo: this.formAtendimento.subjetivo,
                tempoAtendimento: this.countTimer,
                inicioDoAtendimento: this.inicioDoAtendimento,
                fimDoAtendimento: this.fimDoAtendimento,
                tipoAtendimento: this.formAtendimento.tipoAtendimento,
                localDeAtendimento: 1
              }


              //verificando o form das alergias e antecedentes
              if (Object.keys(this.$store.state.user.formModalInformacoesAdicionais).length > 0) {
                let storeObj = this.$store.state.user.formModalInformacoesAdicionais

                //adicionando alergias do store
                if (storeObj.alergias != null && storeObj.alergias != '' && storeObj.alergias != undefined) {
                  atendimento.alergias = storeObj.alergias
                }

                //adicionando antecedentes do store
                if (storeObj.antecedentes != null && storeObj.antecedentes != '' && storeObj.antecedentes != undefined) {
                  atendimento.antecedentes = storeObj.antecedentes
                }
              }

              let { data: dataPostMedico, status: statusPostMedico } = await _api.atendimentos.postMedico(atendimento)

              //verificando se tem algum dado apos tirar o observer do vue, e atualizando os sintomas
              if (this.existSintomas && Object.keys(this.sintomas) && dataPostMedico) {

                try {
                  let { data: dataAcompanhamento, status: statusAcompanhamento } = await _api.acompanhamento.atualizarAcompanhamento(dataPostMedico.id, this.sintomas)
                  if (statusAcompanhamento === 200 || statusAcompanhamento === 201 || statusAcompanhamento === 204) {
                    console.log('acompanhamento atualizado')
                  }
                } catch (e) {
                  console.log('error acompanhamento', e)

                  this.$swal({
                    title: "Erro!",
                    text: 'Erro no registro do acompanhamento para o atendimento atual',
                    icon: 'error',
                  })

                }

              }
              this.$store.dispatch('setFinalizado', true)
              this.$route.params.atendimento = {}
              this.$route.params.atendimento.atendimento = true

              this.$router.push({ name: 'Agendamentos' })
              this.loading.atendimentos = false

            } catch (e) {
              console.log(e)

              this.$swal({
                title: "Erro!",
                text: 'Ocorreu um erro na criação do registro do atendimento',
                icon: 'error',
              })

              this.loading.atendimentos = false
            }
          } else {

            this.$swal({
              title: "Atenção!",
              text: 'Preencha todos os campos obrigatórios',
              icon: 'warning',
            })

            this.loading.atendimentos = false
          }
          this.loading.atendimentos = false
        })
      }
    }
  }</script>
<style scoped>
</style>
