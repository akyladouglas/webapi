export default {
  fatorRH: [
    {value: 'A+', label: 'A+'},
    {value: 'A-', label: 'A-'},
    {value: 'B+', label: 'B+'},
    {value: 'B-', label: 'B-'},
    {value: 'AB+', label: 'AB+'},
    {value: 'AB-', label: 'AB-'},
    {value: 'O+', label: 'O+'},
    {value: 'O-', label: 'O-'}
  ],
  pacientes: [
    {
      id_Paciente: 1,
      nome: 'André Marques Menezes',
      dataNascimento: '10/01/1980',
      cpf: '51776642287',
      documentoIdentificacaoTipo: 'rg',
      documentoIdentificacaoNumero: '2637896',
      documentoIdentificacaoEmissor: 'SSP-PB',
      documentoIdentificacaoDataEmissao: '01/01/1990',
      sexo: 'Masculino',
      foto: 'face-2.jpg',
      telefone: '8333219392',
      celular: '83998031463',
      email: 'andre@ig.com.br',
      estadoCivil: 'Solteiro',
      nacionalidade: 'Brasileiro',
      naturalidade: 'São Paulo',
      escolaridade: 'Fundamental Completo',
      cor: 'Caucasiano',
      profissao: 'Locutor',
      grupoSanguineo: 'A',
      fatorRh: 'Positivo',
      doador: false,
      nomeMae: 'Joselia Lima',
      nomePai: 'Paulo Marques',
      falecido: false,
      dataFalecimento: null,
      endereco: {
        id: '45c24-b5f4v-s4d5v-sdfsf',
        logradouro: 'Rua da Varzea',
        bairro: 'Catolé',
        cidade: {
          ibge: 154234,
          nome: 'Campinas',
          ufAbreviado: 'SP',
          ufExtenso: 'São Paulo'
        },
        cep: '58100000'
      },
      enderecoNumero: 300,
      enderecoComplemento: 'Ap. 202',
      convenio: {
        razaoSocial: 'Unimed convênio ltda',
        nomeFantasia: 'Unimed',
        cnpj: 13723172000135,
        endereco: {
          id: '45c24-b5f4v-s4d5v-arfsf',
          logradouro: 'Rua do Nunca',
          bairro: 'Ipés',
          cidade: {
            ibge: 154444,
            nome: 'São Paulo',
            ufAbreviado: 'SP',
            ufExtenso: 'São Paulo'
          },
          cep: '58100000'
        },
        enderecoNumero: 10,
        enderecoComplemento: null,
        convenioDetalheEmpresa: {}
      },
      convenioCartaoNumero: 'SP-002451246-00',
      convenioCartaoValidade: '20/01/2022',
      responsavelNome: null,
      responsavelCpf: null,
      responsavelDataNascimento: null,
      responsavelParentesco: null,
      responsavelProfissao: null,
      observacao: 'Paciente retorna após realização de exames.',
      historico: [
        {
          titulo: '2',
          subTitulo: 'Consultas Realizadas'
        },
        {
          titulo: '09/17',
          subTitulo: 'Última consulta'
        },
        {
          titulo: '09/18',
          subTitulo: 'Próxima Consulta'
        }
      ]
    },
    {
      id_Paciente: 2,
      nome: 'João Evangelista de Souza',
      dataNascimento: '10/10/1970',
      cpf: '32376642287',
      documentoIdentificacaoTipo: 'rg',
      documentoIdentificacaoNumero: '4637896',
      documentoIdentificacaoEmissor: 'SSP-PE',
      documentoIdentificacaoDataEmissao: '31/01/1990',
      sexo: 'Masculino',
      foto: 'joao.jpg',
      telefone: '8333219392',
      celular: '83998031463',
      email: 'andre@ig.com.br',
      estadoCivil: 'Solteiro',
      nacionalidade: 'Brasileiro',
      naturalidade: 'São Paulo',
      escolaridade: 'Fundamental Completo',
      cor: 'Caucasiano',
      profissao: 'Advogado',
      grupoSanguineo: 'A',
      fatorRh: 'Positivo',
      doador: true,
      nomeMae: 'Maria Nazaré',
      nomePai: 'Thiago Albuquerque',
      falecido: false,
      dataFalecimento: null,
      endereco: {
        id: '45c24-b5f4v-s4d5v-sdfsf',
        logradouro: 'Rua da Varzea',
        bairro: 'São José',
        cidade: {
          ibge: 154234,
          nome: 'Campinas',
          ufAbreviado: 'SP',
          ufExtenso: 'São Paulo'
        },
        cep: '58100000'
      },
      enderecoNumero: 200,
      enderecoComplemento: 'Ap. 203',
      convenio: {
        razaoSocial: 'Unimed convênio ltda',
        nomeFantasia: 'Unimed',
        cnpj: 13723172000135,
        endereco: {
          id: '45c24-b5f4v-s4d5v-arfsf',
          logradouro: 'Rua do Nunca',
          bairro: 'Ipés',
          cidade: {
            ibge: 154444,
            nome: 'São Paulo',
            ufAbreviado: 'SP',
            ufExtenso: 'São Paulo'
          },
          cep: '58100000'
        },
        enderecoNumero: 10,
        enderecoComplemento: null,
        convenioDetalheEmpresa: {}
      },
      convenioCartaoNumero: 'SP-003421246-00',
      convenioCartaoValidade: '20/01/2022',
      responsavelNome: null,
      responsavelCpf: null,
      responsavelDataNascimento: null,
      responsavelParentesco: null,
      responsavelProfissao: null,
      observacao: 'Paciente possui Alergia a Lactose',
      historico: [
        {
          titulo: '3',
          subTitulo: 'Consultas Realizadas'
        },
        {
          titulo: '08/17',
          subTitulo: 'Última consulta'
        },
        {
          titulo: '10/18',
          subTitulo: 'Próxima Consulta'
        }
      ]
    },
    {
      id_Paciente: 3,
      nome: 'Maria do Carmos Gouveia',
      dataNascimento: '10/01/1992',
      cpf: '98776642287',
      documentoIdentificacaoTipo: 'rg',
      documentoIdentificacaoNumero: '2637896',
      documentoIdentificacaoEmissor: 'SSP-PB',
      documentoIdentificacaoDataEmissao: '01/08/200',
      sexo: 'Feminino',
      foto: 'maria.jpg',
      telefone: '8333219392',
      celular: '83998031463',
      email: 'maria@ig.com.br',
      estadoCivil: 'Solteiro',
      nacionalidade: 'Brasileiro',
      naturalidade: 'João Pessoa',
      escolaridade: 'Superior Completo',
      cor: 'Caucasiano',
      profissao: 'Professor',
      grupoSanguineo: 'B',
      fatorRh: 'Negativo',
      doador: false,
      nomeMae: 'Maria Luiza',
      nomePai: 'Estevão Onofre',
      falecido: false,
      dataFalecimento: null,
      endereco: {
        id: '45c24-b5f4v-s4d5v-sdfsf',
        logradouro: 'Av. Rio Grande do Sul',
        bairro: 'Estados',
        cidade: {
          ibge: 254234,
          nome: 'João pessoa',
          ufAbreviado: 'JP',
          ufExtenso: 'João Pessoa'
        },
        cep: '58100000'
      },
      enderecoNumero: 1000,
      enderecoComplemento: 'Ap. 202',
      convenio: {
        razaoSocial: 'Smile Convênio ltda',
        nomeFantasia: 'Smile',
        cnpj: 23723172000135,
        endereco: {
          id: '54c24-b5f4v-s4d5v-arfsf',
          logradouro: 'Epitácio Pessoa',
          bairro: 'Centro',
          cidade: {
            ibge: 234444,
            nome: 'São Paulo',
            ufAbreviado: 'SP',
            ufExtenso: 'São Paulo'
          },
          cep: '58030020'
        },
        enderecoNumero: 1000,
        enderecoComplemento: null,
        convenioDetalheEmpresa: {}
      },
      convenioCartaoNumero: 'SM-4524451246-92',
      convenioCartaoValidade: '20/01/2030',
      responsavelNome: null,
      responsavelCpf: null,
      responsavelDataNascimento: null,
      responsavelParentesco: null,
      responsavelProfissao: null,
      observacao: 'Paciente retorna após solicitação de exames.',
      historico: [
        {
          titulo: '4',
          subTitulo: 'Consultas Realizadas'
        },
        {
          titulo: '05/18',
          subTitulo: 'Última consulta'
        },
        {
          titulo: '11/18',
          subTitulo: 'Próxima Consulta'
        }
      ]
    },
    {
      id_Paciente: 4,
      nome: 'Marcos Olegário da Costa',
      dataNascimento: '23/07/1985',
      cpf: '34676642287',
      documentoIdentificacaoTipo: 'CNH',
      documentoIdentificacaoNumero: '8637896',
      documentoIdentificacaoEmissor: 'SSP-PB',
      documentoIdentificacaoDataEmissao: '01/01/2010',
      sexo: 'Masculino',
      foto: 'face-3.jpg',
      telefone: '8333221392',
      celular: '83898021463',
      email: 'marcos@ig.com.br',
      estadoCivil: 'Casado',
      nacionalidade: 'Brasileiro',
      naturalidade: 'São Paulo',
      escolaridade: 'Superior Incompleto',
      cor: 'Negro',
      profissao: 'Professor',
      grupoSanguineo: 'O',
      fatorRh: 'Positivo',
      doador: true,
      nomeMae: 'Silva Nunes',
      nomePai: 'Eliezer Silva',
      falecido: false,
      dataFalecimento: null,
      endereco: {
        id: '45c24-b5f4v-s4d5v-sdfsf',
        logradouro: 'Av. José Martins de Andrade',
        bairro: 'Santa Rosa',
        cidade: {
          ibge: 154222,
          nome: 'Campina Grande',
          ufAbreviado: 'PB',
          ufExtenso: 'Paraíba'
        },
        cep: '58100100'
      },
      enderecoNumero: 353,
      enderecoComplemento: null,
      convenio: {
        razaoSocial: 'Particular',
        nomeFantasia: 'Particular',
        cnpj: null,
        endereco: null,
        enderecoNumero: null,
        enderecoComplemento: null,
        convenioDetalheEmpresa: {}
      },
      convenioCartaoNumero: null,
      convenioCartaoValidade: null,
      responsavelNome: null,
      responsavelCpf: null,
      responsavelDataNascimento: null,
      responsavelParentesco: null,
      responsavelProfissao: null,
      observacao: 'Paciente se queixava de fortes dores abdominais.',
      historico: [
        {
          titulo: '1',
          subTitulo: 'Consultas Realizadas'
        },
        {
          titulo: '09/17',
          subTitulo: 'Última consulta'
        }
      ]
    },
    {
      id_Paciente: 5,
      nome: 'Paula Fernandes Soares',
      dataNascimento: '10/08/1990',
      cpf: '23476642287',
      documentoIdentificacaoTipo: 'rg',
      documentoIdentificacaoNumero: '3647896',
      documentoIdentificacaoEmissor: 'SSP-SP',
      documentoIdentificacaoDataEmissao: '01/08/2000',
      sexo: 'Feminino',
      foto: 'paula-fernandes.jpg',
      telefone: '1122219392',
      celular: '11898031463',
      email: 'paula@ig.com.br',
      estadoCivil: 'Solteiro',
      nacionalidade: 'Brasileiro',
      naturalidade: 'Brotas',
      escolaridade: 'Superior Completo',
      cor: 'Caucasiano',
      profissao: 'Cantora',
      grupoSanguineo: 'B',
      fatorRh: 'Negativo',
      doador: true,
      nomeMae: 'Amelia Fernandes',
      nomePai: 'Carlos Batista',
      falecido: false,
      dataFalecimento: null,
      endereco: {
        id: '45c24-b5f4v-s4d5v-sdfsf',
        logradouro: 'Rua do Sol Nascente',
        bairro: 'Mangabeira',
        cidade: {
          ibge: 254234,
          nome: 'Brotas',
          ufAbreviado: 'MG',
          ufExtenso: 'Minas Gerais'
        },
        cep: '35100000'
      },
      enderecoNumero: 200,
      enderecoComplemento: null,
      convenio: {
        razaoSocial: 'Unimed convênio ltda',
        nomeFantasia: 'Unimed',
        cnpj: 13723172000135,
        endereco: {
          id: '45c24-b5f4v-s4d5v-arfsf',
          logradouro: 'Rua do Nunca',
          bairro: 'Ipés',
          cidade: {
            ibge: 154444,
            nome: 'São Paulo',
            ufAbreviado: 'SP',
            ufExtenso: 'São Paulo'
          },
          cep: '58100000'
        },
        enderecoNumero: 10,
        enderecoComplemento: null,
        convenioDetalheEmpresa: {}
      },
      convenioCartaoNumero: 'UN-4524451246-92',
      convenioCartaoValidade: '20/01/2020',
      responsavelNome: null,
      responsavelCpf: null,
      responsavelDataNascimento: null,
      responsavelParentesco: null,
      responsavelProfissao: null,
      observacao: 'Paciente retorna após cirurgia realizada dia 05/08/18',
      historico: [
        {
          titulo: '1',
          subTitulo: 'Consultas Realizadas'
        },
        {
          titulo: '08/18',
          subTitulo: 'Última consulta'
        }
      ]
    }
  ],
  hospitais: [
    { value: 1, label: 'Hospital Antonio Targino' },
    { value: 2, label: 'Hospital da Clipsi' },
    { value: 3, label: 'Hospital da FAP' },
    { value: 3, label: 'Hospital das Clínicas da FMUSP' },
    { value: 3, label: 'Hospital das Panelas' },
    { value: 3, label: 'Hospital de Olhos' },
    { value: 3, label: 'Hospital de Trauma' }
  ]
}
