<template>
  <div>
    <div class="pl-2" v-if="loadingMemed">
      <small>Carregando módulo ... </small>
    </div>
    <!--<div class="text-right">
      <el-button v-if="!loadingMemed" :disabled="iniciado" id="botaoShowPrescricao" type="success" class="mr-4 mb-4" @click="setPaciente">
        Preencher Prescrição
      </el-button>
    </div>-->
    <div class="memed-container" id="memed-container"></div>
  </div>
</template>

<script>
import _api from '../../api'
export default {
  name: 'PrescricaoMemed',
  props: {
    idAgendamento: '',
    paciente: '',
    profissional: ''
  },
  data: () => {
    return {
      cidade: '',
      body: document.querySelector('body'),
      loadingMemed: true,
      iniciado: false
    }
  },
  async mounted () {
    await this.cidadePrescricao()
    await this.initMemed()
    //setTimeout(() => {
    //  this.loadingMemed = false
    //}, 12000)

    // Inicie a verificação
    this.checkPaciente();

    if (!window.MdHub && !window.MdSinapsePrescricao) {
      setTimeout(() => {
        console.log('tentando conectar memed novamente');
        this.checkPaciente();
      }, 4000); // Tenta novamente em 4 segundos
    }

  },
  beforeDestroy () {
    this.closeMemed()
  },
    methods: {
      checkPaciente() {
        console.log('window.MdHub', window.MdHub)
        if (window.MdHub && window.MdSinapsePrescricao) {

            console.log('abrindo memed');
            setTimeout(() => {
              this.setPaciente();
              this.loadingMemed = false;
            }, 4000);
          }
      },

    async cidadePrescricao () {
      let { data } = await _api.cidades.getById(this.paciente.cidadeId)
      this.cidade = data
    },
    async initMemed () {
      var VueThis = this
      let script = document.createElement('script')
      script.setAttribute('type', 'text/javascript')
      script.setAttribute('data-color', '#43bec6')
      script.setAttribute(
        'data-token',
        this.profissional.tokenMemed
      )
      script.setAttribute('data-container', 'memed-container')
      script.src =
        'https://memed.com.br/modulos/plataforma.sinapse-prescricao/build/sinapse-prescricao.min.js'
      script.onload = function (data) {
        VueThis.initEventsMemed()
      }
      this.body.appendChild(script)
    },
    async setPaciente () {
      this.iniciado = true
      var VueThis = this
      window.MdHub.command
        .send('plataforma.prescricao', 'setPaciente', {
          nome: VueThis.paciente.nomeCompleto,
          cpf: VueThis.paciente.cpf,
          idExterno: VueThis.paciente.id,
          endereco: VueThis.paciente.logradouro,
          cidade: VueThis.cidade.nome,
          telefone: VueThis.paciente.telefoneCelular,
          peso: null,
          altura: null,
          nome_mae: null,
          dificuldade_locomocao: false
        })
        .then(function () {
          window.MdHub.module.show('plataforma.prescricao')
          window.MdHub.command.send('plataforma.prescricao', 'setFeatureToggle', {
            // Habilitar o EXCLUIR PACIENTE após clicar no campo do nome
            deletePatient: false, // ou true para ativar
            // Habilitar o X para remover o paciente da prescrição
            removePatient: false,
            // Habilita o campo DADOS DO PACIENTE após clicar no campo do nome
            editPatient: false,
            // Habilita o Historico de Prescrições geradas do paciente
            historyPrescription: true,
            // Habilita a opção de Modificar o Style do Receituario e fazer conforme suas modificações
            optionsPrescription: true,
            // Habilita a opção de Deletar uma prescrição do Historico
            removePrescription: false,
            // Habilita a opção de adicionar Alergias ao Paciente
            setPatientAllergy: false,
            // Habilita o Auto Complemento no Campo Nomes Comerciais
            autocompleteIndustrialized: true,
            // Habilita o Auto Complemento no Campo Formulas
            autocompleteManipulated: true,
            // Habilita o Auto Complemento no Campo Nome Genéricos
            autocompleteCompositions: true,
            // Habilita o Auto Complemento no Campo Outros
            autocompletePeripherals: true,
            // Habilita o Auto Complemento no Campo Exames
            autocompleteExams: true,
            // Habilita a opção de Copiar os dados da prescrição para a Área de Transferencia (clipboard)
            copyMedicalRecords: true,
            // Habilita o botão para fechar o Modulo
            buttonClose: false,
            // Habilita a opção de Nova Formula Manipulada após clicado em Nova Fórmula Manipulada
            newFormula: true,
            // Habilita a opção de Compartilhar o link da prescrição através do Whatsapp e SMS
            allowShareModal: true,
            // Habilita videos de guias rápidos no rodapé da página
            guidesOnboarding: false,
            // Habilita após a conclusão da Prescrição, um campo de editar receita
            conclusionModalEdit: true,
            // Habilita o botão de Perfil para vincular o profissional a uma conta Memed
            dropdownSync: true,
            // Habilita o Campo Protocolos, para ser usado como textos padrões: Atestados, Encaminhamentos e Prescrições, ou Qualquer outro Texto Padrão
            showProtocol: true,
            // Habilita o Campo Ajuda com videos que ajuda o usuário nas funcionalidades
            showHelpMenu: true,
            // Permite a edição de dados do paciente (após feita a prescrição/atestado/encaminhamento) na base memed
            editIdentification: false
          })

          window.MdHub.event.add('prescricaoImpressa', function (prescriptionData) {
            // No objeto "prescriptionData", é retornado as informações da prescrição gerada.
            // Implementar ações, callbacks, etc.

            let dados = prescriptionData.prescricao
            let documentos = prescriptionData.prescricao.documents
            if (documentos.length > 0) {
              var idPrescricao = dados.id
              _api.memed.getPrescriptionsById(idPrescricao, VueThis.profissional.tokenMemed, VueThis.paciente.id, VueThis.profissional.id, VueThis.idAgendamento)
            }
          })
        })
    },
    initEventsMemed () {
      window.MdSinapsePrescricao.event.add(
        'core:moduleInit',
        function moduleInitHandler (module) {
          if (module.name === 'plataforma.prescricao') {
            // console.log('modulo prescricao iniciado')
          }
        }
      )
    },
    closeMemed () {
      window.MdHub.server.unbindEvents()
      delete window.MdHub
      delete window.MdSinapsePrescricao

      const scripts = Array.from(document.getElementsByTagName('script'))
      if (scripts && scripts.length > 0) {
        scripts.forEach(script => {
          if (script.src.includes('memed.com.br')) {
            if (script.parentNode) {
              script.parentNode.removeChild(script)
            }
          }
        })
      }
    }
  }
}
</script>

<style scoped>
.memed-container {
  /* Largura mínima 820px */
  /*width: 900px;*/
  height: 600px !important;
  position: relative;
}
</style>
