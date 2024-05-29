import axios from 'axios'
import { Notification } from 'element-ui'

export default {
  urlApi: '/api/v1',
  urlHub: '/',
  urlPaciente: 'https://portalcondepb.meeds.com.br',
  audience: 'atendtelemedicina',

  // Configurações
  minBusca: 2,
  configDB: {},

  // Configurações DB
  getConfig() {
    return axios({
      method: 'GET',
      url: `${this.urlApi}/Config`
    })
      .then(res => {
        this.configDB = res.data
      })
      .catch(e => {
        console.log('erro', e.response)
        let erroMsg = e.response ? e.response.data.message : 'Erro de comunicação com a API'
        Notification({ title: 'Erro ao retornar as configurações', message: erroMsg, type: 'error' })
        return { status: 400, error: erroMsg }
      })
  },


  // Mensagens
  txt: {
    btSalvar: 'Salvar',
    btVoltar: 'Voltar',
    formInvalido: 'Verifique o preenchimento do formulário',
    enviando: 'Aguarde enquanto os dados são enviandos ...',
    operacaoOk: 'Operação realizada com sucesso',
    operacaoErro: 'Falha ao processar operação',
    resultadoVazio: 'Nenhum resultado encontrado',
    estabelecimento: {
      semProcedimento: 'Estabelecimento sem procedimentos cadastrados.'
    }
  },
  // Banco de Dados
  dbErro: 'Erro na comunicação com a base de dados',

  // VALIDAÇÕES
  validacoes: {
    descricaoRequired: 'A descrição é obrigatória',
    nomeFantasiaRequired: 'Nome Fantasia é obrigatório',
    cnesOrigemRequired: 'Selecione o Estabelecimento de Origem',
    cnesRequired: 'Estabelecimento é obrigatório',
    cnesFormato: 'CNS precisa conter um total de 7 números',
    pacienteRequired: 'Selecione o Paciente',
    suposicaoDiagnosticoRequired: 'O campo Suposição Diagnóstica e/ou Diagnóstico Sindrômico é obrigatório',
    cidRequired: 'Selecione pelo menos um Cid10',
    especialidadeRequired: 'Selecione pelo menos uma Especialidade',
    procedimentoRequired: 'Selecione pelo menos um Procedimento',
    procedimentoToAddRequired: 'Selecione pelo menos 1 procedimento a ser adicionado',
    procedimentoCotaExcedida: 'Cota execedida para o procedimento',
    procedimentoCotaQuantidade: 'Digite a quantidade de cotas a adicionar no procedimento',
    dataRequired: 'Selecione a data',
    horaRequired: 'Selecione a hora',
    usuarioRequired: 'Preencha o Nome de Usuário',
    senhaRequired: 'Preencha a senha do Usuário',
    nomeRequired: 'Preencha o Nome',
    sobreNomeRequired: 'Preencha o Sobrenome',
    sexoRequired: 'Selecione o Sexo',
    cpfRequired: 'O campo CPF  é obrigatório',
    cpfFormato: 'CPF precisa conter um total de 11 números',
    cnsRequired: 'O campo CNS  é obrigatório',
    cnsFormato: 'CNS precisa conter um total de 16 números',
    dataNascimentoRequired: 'Data de Nascimento é obrigatória',
    estadoRequired: 'Selecione o Estado',
    cidadeRequired: 'Selecione a Cidade',
    bairroRequired: 'Selecione o Bairro',
    emailRequired: 'O campo email é obrigatório',
    emailInvalido: 'Digite um email válido',
    tipoRequired: 'O Tipo é obrigatório',
    codigoRequired: 'O Código é obrigatório',
    diaRequired: 'Selecione pelo menos um dia',
    justificativaRequired: 'A justificativa para a solicitação é obrigatória',
    classificacaoRequired: 'Classificação de risco é obrigatória',
    profissionalRequired: 'Selecione o Profissional',
    tipoDaConsultaRequired: 'Tipo da Consulta é obrigatório'
  },
  // FUNCOES
  intervalo () {
    let hora = []
    let i, j
    for (i = 7; i < 18; i++) {
      for (j = 0; j < 4; j++) {
        hora.push({
          label: i + ':' + (j === 0 ? '00' : 15 * j),
          key: i + ':' + (j === 0 ? '00' : 15 * j)
        })
      }
    }
    return hora
  },
  helpers: {
    money: {
      decimal: ',',
      thousands: '.',
      prefix: '',
      suffix: '',
      precision: 2,
      masked: true
    }
  }
}
