<template>
  <div>

    <el-button id="botaoShowPrescricao" type="success" style="margin-left: 10px">
      Prescrição
    </el-button>

    <!--<div> teste info {{ nomePaciente }} {{ cpfPaciente }} {{ idPaciente }} {{ idAgendamento }} {{ idProfissional }}</div>-->
    <div class="memed-container" id="memed-container"></div>
  </div>
</template>

<script>
  import _api from '../../api'
  export default {
    name: "Integration",
    props: ["idIndividuo", "idAgendamento", "idProfissional"],
    data () {
      return {
        body: document.querySelector("body"),
        tokenAbrirMemed: "",
        
        api: {
          profissionais: [],
          individuo: {},
        },
      };
    },
    methods: {
      async getPaciente() {
        let { data, status } = await _api.individuos.getById(this.idIndividuo)
        if (data !== null) {
          this.api.individuo = data
        }
      },

      async tokenMemed() {
        //INSERE O TOKEN QUE VEM DA API E VERIFICA SE IRA ABRIR O MEMED
        let tokenInicial = await _api.memed.recoverTokenMedico()
        this.tokenAbrirMemed = tokenInicial.data.data.attributes.token
        //console.log('exibindo tokenn!!!!!!!!!!!! ', tokenInicial.data.data.attributes.token)
        //console.log('exibindo token inicial', tokenInicial)
      },
      async initMemed() {
        //var token = this.tokenAbrirMemed
        var self = this
        let script = document.createElement("script");
        script.setAttribute("type", "text/javascript");
        script.setAttribute("data-color", "#1abc9c");
        script.setAttribute(
          "data-token",
          self.tokenAbrirMemed
        );
        script.setAttribute("data-container", "memed-container");
        script.src =
          "https://integrations.memed.com.br/modulos/plataforma.sinapse-prescricao/build/sinapse-prescricao.min.js";
        script.onload = function (data) {
          //console.log('data', data)
          data.path[3].defaultView.MdSinapsePrescricao.event.add(
            "core:moduleInit",
            function moduleInitHandler(module) {
              // console.log(module);
              if (module.name === "plataforma.prescricao") {
                //console.log('vai chamar')
                document
                  .getElementById("botaoShowPrescricao")
                  .addEventListener("click", function (e) {
                    console.log('acionou o botão')
                    data.path[3].defaultView.MdHub.command
                      .send("plataforma.prescricao", "setPaciente", {
                        nome: self.api.individuo.nomeCompleto,
                        cpf: self.api.individuo.cpf,
                        idExterno: self.api.individuo.id,
                        //idExterno: 144123,
                        //nome: 'Otavio da Silva',
                        //cpf: '692.370.774-91',
                        //data_nascimento: '10/10/2010',
                        //nome_social: 'Otavio da Silva',
                        //endereco: 'Rua Gustavo Torres Trocolli, 67',
                        //cidade: 'Paraiba',
                        //telefone: '83987452751',
                        //peso: 75,
                        //altura: 1.8,
                        //nome_mae: 'Maria da Silva',
                        //dificuldade_locomocao: false
                      })
                      .then(function () {
                        data.path[3].defaultView.MdHub.module.show("plataforma.prescricao");

                          data.path[3].defaultView.MdHub.command.send('plataforma.prescricao', 'setFeatureToggle', {
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
                            dropdownSync: false,
                            // Habilita o Campo Protocolos, para ser usado como textos padrões: Atestados, Encaminhamentos e Prescrições, ou Qualquer outro Texto Padrão
                            showProtocol: true,
                            // Habilita o Campo Ajuda com videos que ajuda o usuário nas funcionalidades
                            showHelpMenu: true,
                            // Permite a edição de dados do paciente (após feita a prescrição/atestado/encaminhamento) na base memed
                            editIdentification: false,
                          });

                        MdHub.event.add('prescricaoImpressa', function (prescriptionData) {
                          // No objeto "prescriptionData", é retornado as informações da prescrição gerada.
                          // Implementar ações, callbacks, etc.
                          //console.log('console log prescriptionData quando termina ', prescriptionData)

                          let dados = prescriptionData.prescricao
                          let documentos = prescriptionData.prescricao.documents
                          if (documentos.length > 0) {
                            var idPrescricao = dados.id
                            var documentsArray = prescriptionData.prescricao.documents
                            //console.log('IDS', {idPaciente: self.idPaciente, idProfissional: self.idProfissional, idAgendamento: self.idAgendamento})
                            //console.log('dentro do if não é vazio')
                            //console.log('retorno prescricaoID do if', idPrescricao)
                            //console.log('retorno DocumentsArray ', documentsArray)
                            //console.log('passando idprescricao para api', idPrescricao)
                            //console.log('console do this.tokenAbrirMemed', self.tokenAbrirMemed)
                            _api.memed.getPrescriptionsById(idPrescricao, self.tokenAbrirMemed, self.api.individuo.id, self.idProfissional, self.idAgendamento)
                          }

                          
                        });

                        MdHub.command.send('plataforma.prescricao', 'getPrescricao').then(function (prescricao) {
                          //console.log('console log quando a prescricao memed inicia ', prescricao)
                        });
                      });
                  });
              }
            }
          );
        };
        this.body.appendChild(script);
      },


      async getProfissional() {
 

        let { data, status } = await _api.profissionais.getById(this.$auth.user().id)
        
        this.api.profissionais = (status === 200) ? data : []

        //console.log('console na api', this.api.profissionais)
      },

    },
    created() {
      //console.log('this individuo created', this.props.individuoId)
    },
    async mounted() {
      await this.tokenMemed();
      await this.initMemed();
      await this.getPaciente();
      //this.getProfissional()
      this.getProfissional();
    },
    //unmounted() {
    //  document.getElementById("botaoShowPrescricao").removeEventListener("click", (e) => { console.log('evento removido')})
    //}
  };
</script>


<style scoped>
  .memed-container {
    margin: 0 auto;
    /* Largura mínima 820px */
    min-width: 820px;
    width: 100%;
    height: 710px;
  }
</style>

