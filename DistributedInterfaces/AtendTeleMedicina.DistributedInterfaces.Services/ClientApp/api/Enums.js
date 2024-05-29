export default {
  claims: [
    { claimType: 'Role', claimValue: 'Administrador' },
    { claimType: 'Role', claimValue: 'GestorEstado' },
    { claimType: 'Role', claimValue: 'GestorMunicipio' },
    { claimType: 'Role', claimValue: 'Agente' },
    { claimType: 'Role', claimValue: 'MedicoAtencaoPrimaria' },
    { claimType: 'Role', claimValue: 'MédicoEspecialista' },
    { claimType: 'Role', claimValue: 'ProfissionalNaoMedico' },
    { claimType: 'Role', claimValue: 'Recepcionista' },
    { claimType: 'Role', claimValue: 'Triagem' }
  ],
  claimsMedico: [
    { claimType: 'Role', claimValue: 'Médico' }
  ],
  claimsMedicoConde: [
    { claimType: 'Role', claimValue: 'Médico' },
    { claimType: 'Role', claimValue: 'MédicoEspecialista' },
    { claimType: 'Role', claimValue: 'MédicoAD' }
  ],
  claimsGestorEstado: [
    { claimType: 'Role', claimValue: 'Administrador' },
    { claimType: 'Role', claimValue: 'GestorEstado' },
    { claimType: 'Role', claimValue: 'GestorMunicipio' },
    { claimType: 'Role', claimValue: 'Agente' }
  ],
  claimsAdmMunicipio: [
    { claimType: 'Role', claimValue: 'Administrador' },
    { claimType: 'Role', claimValue: 'GestorEstado' },
    { claimType: 'Role', claimValue: 'GestorMunicipio' },
    { claimType: 'Role', claimValue: 'Agente' }
  ],
  sexos: [
    { label: 'Masculino', value: 0 },
    { label: 'Feminino', value: 1 }
  ],
  genero: [
    { label: 'Masculino', value: 'M' },
    { label: 'Feminino', value: 'F' }
  ],
  simNao: [
    { label: 'Sim', value: true },
    { label: 'Não', value: false }
  ],
  simNaoTodos: [
    { label: 'Todos', value: null },
    { label: 'Sim', value: 1 },
    { label: 'Não', value: 0 }
  ],
  retorno: [
    { label: 'Sim', value: true },
    { label: 'Não', value: false }
  ],
  tipoDaConsultaTelaHome: [
    { label: 'Todos', value: '' },
    { label: 'Presencial', value: 'Presencial' },
    { label: 'Teleconsulta', value: 'Teleconsulta' },
    { label: 'Totem', value: 'Totem' }
  ],
  tiposDaConsulta: [
    { label: 'Presencial', value: 'Presencial' },
    { label: 'Teleconsulta', value: 'Teleconsulta' },
    { label: 'Totem', value: 'Totem' }
  ],
  motivosCadastro: [
    { label: 'Monitora Corona', value: 1 }
  ],
  coresStatus: [
    { label: 'Cinza', value: 0, cor: '#DFDFDF' },
    { label: 'Verde', value: 1, cor: '#00CC00' },
    { label: 'Amarelo', value: 2, cor: '#FFCC00' },
    { label: 'Laranja', value: 3, cor: '#FF6600' },
    { label: 'Vermelho', value: 4, cor: '#990000' }
  ],
  coresStatusAtendimento: [
    { label: 'Verde', value: 1, cor: '#00CC00' },
    { label: 'Amarelo', value: 2, cor: '#FFCC00' },
    { label: 'Laranja', value: 3, cor: '#FF6600' },
    { label: 'Vermelho', value: 4, cor: '#990000' }
  ],
  grausDePrioridade: [
    { label: 'Nenhum', value: 0 },
    { label: 'Baixa', value: 1 },
    { label: 'Média', value: 2 },
    { label: 'Alta', value: 3 }
  ],
  classificacaoDeRisco: [
    { label: 'Não Urgente', value: 1 },
    { label: 'Pouco Urgente', value: 2 },
    { label: 'Urgente', value: 3 },
    { label: 'Muito Urgente', value: 4 },
    { label: 'Emergência', value: 5 }
  ],
  condicoesAvaliadas: [
    { label: 'Acamado', value: '1' },
    { label: 'Domiciliado', value: '2' },
    { label: 'Úlceras/Feridas (grau III ou IV)', value: '3' },
    { label: 'Acompanhamento nutricional', value: '4' },
    { label: 'Uso de sonda nasogástrica - SNG', value: '5' },
    { label: 'Uso de sonda naso-enteral - SNE', value: '6' },
    { label: 'Uso de gastrostomia', value: '7' },
    { label: 'Uso de colostomia', value: '8' },
    { label: 'Uso de cistostomia', value: '9' },
    { label: 'Uso de sonda vesical de demora - SVD', value: '10' },
    { label: 'Acompanhamento pré-operatório', value: '11' },
    { label: 'Acompanhamento pós-operatório', value: '12' },
    { label: 'Adaptação ao uso de órtese/prótese', value: '13' },
    { label: 'Reabilitação domiciliar', value: '14' },
    { label: 'Cuidados paliativos oncológicos', value: '15' },
    { label: 'Cuidados paliativos não oncológicos', value: '16' },
    { label: 'Oxigenoterapia domiciliar', value: '17' },
    { label: 'Uso de traqueostomia', value: '18' },
    { label: 'Uso de aspirador de vias aéreas para higiene brônquica', value: '19' },
    { label: 'Suporte ventilatório não invasivo - CPAP', value: '20' },
    { label: 'Suporte ventilatório não invasivo - BIPAP', value: '21' },
    { label: 'Diálise peritonial', value: '22' },
    { label: 'Parecentese', value: '23' },
    { label: 'Medicação parenteral', value: '24' }
  ],
  tipoExames: [
    { label: 'Nenhum', value: 0 },
    { label: 'COVID', value: 13916 },
    { label: 'INFLUENZA', value: 13917 },
    { label: 'CK-MB', value: 13918 },
    { label: 'PSA', value: 13919 },
    { label: 'HEPATITE', value: 13920 },
    { label: 'PNEUMONIA', value: 13921 },
    { label: 'PCT(INFECCAO POR BACTERIAS)', value: 13922 },
    { label: 'DENGUE IGL/LGM', value: 13923 },
    { label: 'HCV AB', value: 13924 },
    { label: 'AUDIO', value: 13926 },
    { label: 'HEMOGLOBINA GLICADA', value: 13927 },
    { label: 'AUSCULTACAO', value: 13928 },
    { label: 'TROPONINA I', value: 13929 },
    { label: 'HS TROPONINA I*', value: 13930 },
    { label: 'HSPCR', value: 13931 },
    { label: 'D-DIMERO', value: 13932 },
    { label: 'NT-PROBNP', value: 13933 },
    { label: 'B-HCG', value: 13934 },
    { label: 'LH', value: 13935 },
    { label: 'TSH', value: 13936 },
    { label: 'T4 TOTAL*', value: 13937 },
    { label: 'T4 LIVRE', value: 13938 },
    { label: 'T3*', value: 13939 },
    { label: 'PSA', value: 13940 },
    { label: 'SANGUE OCULTO (FOB)', value: 13941 },
    { label: 'AFP*', value: 13942 },
    { label: 'CEA*', value: 13943 },
    { label: 'HBA1C', value: 13944 },
    { label: 'U-ALBUMINA', value: 13945 },
    { label: 'CISTATINA C*', value: 13946 },
    { label: 'PCT', value: 13947 },
    { label: 'PCR', value: 13948 },
    { label: 'CALPROTECTINA*', value: 13949 },
    { label: 'VITAMINA D*', value: 13950 },
    { label: 'HCV AB', value: 13951 },
    { label: 'HIV AB/AG', value: 13952 },
    { label: 'SIFILIS', value: 13953 },
    { label: 'ANTI HBS', value: 13954 },
    { label: 'HBSAG', value: 13955 },
    { label: 'LYME IGG/IGM', value: 13956 },
    { label: 'DENGUE IGG/IGM', value: 13957 },
    { label: 'DENGUE NS1', value: 13958 },
    { label: 'CHIKV IGG/IGM', value: 13959 },
    { label: 'ZIKA AG', value: 13960 },
    { label: 'ZIKA IGG/IGM', value: 13961 },
    { label: 'MALARIA AG - ICT', value: 13962 },
    { label: 'FILARIOSE - ICT', value: 13963 },

    { label: 'OUTROS', value: 13925 }
  ],
  grauParentescoResponsavel: [
    { label: 'Pai/Mae', value: 1 },
    { label: 'Tio/Tia', value: 2 },
    { label: 'Avô/Avó', value: 3 },
    { label: 'Outro', value: 4 }
  ],
  racaOuCor: [
    { label: 'Branca', value: 1 },
    { label: 'Preta', value: 2 },
    { label: 'Amarela', value: 3 },
    { label: 'Parda', value: 4 },
    { label: 'Indígena', value: 5 },
    { label: 'Sem Informação', value: 6 }
  ],
  resultadosExame: [
    { label: 'Positivo', value: 'Positivo' },
    { label: 'Negativo', value: 'Negativo' }
  ],
  unidadesFederativas: [
    {
      ufAbreviado: 'AC',
      ufExtenso: 'Acre'
    },
    {
      ufAbreviado: 'AL',
      ufExtenso: 'Alagoas'
    },
    {
      ufAbreviado: 'AM',
      ufExtenso: 'Amazonas'
    },
    {
      ufAbreviado: 'AP',
      ufExtenso: 'Amapá'
    },
    {
      ufAbreviado: 'BA',
      ufExtenso: 'Bahia'
    },
    {
      ufAbreviado: 'CE',
      ufExtenso: 'Ceara'
    },
    {
      ufAbreviado: 'DF',
      ufExtenso: 'Distrito Federal'
    },
    {
      ufAbreviado: 'ES',
      ufExtenso: 'Espírito Santo'
    },
    {
      ufAbreviado: 'GO',
      ufExtenso: 'Goiás'
    },
    {
      ufAbreviado: 'MA',
      ufExtenso: 'Maranhão'
    },
    {
      ufAbreviado: 'MG',
      ufExtenso: 'Minas Gerais'
    },
    {
      ufAbreviado: 'MS',
      ufExtenso: 'Mato Grosso do Sul'
    },
    {
      ufAbreviado: 'MT',
      ufExtenso: 'Mato Grosso'
    },
    {
      ufAbreviado: 'PA',
      ufExtenso: 'Pará'
    },
    {
      ufAbreviado: 'PB',
      ufExtenso: 'Paraíba'
    },
    {
      ufAbreviado: 'PE',
      ufExtenso: 'Pernambuco'
    },
    {
      ufAbreviado: 'PI',
      ufExtenso: 'Piauí'
    },
    {
      ufAbreviado: 'PR',
      ufExtenso: 'Paraná'
    },
    {
      ufAbreviado: 'RJ',
      ufExtenso: 'Rio de Janeiro'
    },
    {
      ufAbreviado: 'RN',
      ufExtenso: 'Rio Grande do Norte'
    },
    {
      ufAbreviado: 'RO',
      ufExtenso: 'Rondônia'
    },
    {
      ufAbreviado: 'RR',
      ufExtenso: 'Roraima'
    },
    {
      ufAbreviado: 'RS',
      ufExtenso: 'Rio Grande do Sul'
    },
    {
      ufAbreviado: 'SC',
      ufExtenso: 'Santa Catarina'
    },
    {
      ufAbreviado: 'SE',
      ufExtenso: 'Sergipe'
    },
    {
      ufAbreviado: 'SP',
      ufExtenso: 'São Paulo'
    },
    {
      ufAbreviado: 'TO',
      ufExtenso: 'Tocantins'
    }
  ],
  tipoProcedimento: [
    { label: 'Procedimento', value: 'Procedimento' },
    { label: 'Especialidade', value: 'Especialidade' }
  ],

  tipoDaConsulta: [
    { label: 'Presencial', value: 'Presencial' },
    { label: 'Teleconsulta', value: 'Teleconsulta' },
    { label: 'Totem', value: 'Totem' }
  ],
  nacionalidade: [
    { label: 'Brasileiro', value: 1 },
    { label: 'Naturalizado', value: 2 },
    { label: 'Estrangeiro', value: 3 }
  ],
  grauDeInstrucao: [
    { label: 'Creche', value: 51 },
    { label: 'Pré-Escola (Exceto CA)', value: 52 },
    { label: 'Classe de alfabetização - CA', value: 53 },
    { label: 'Ensino Fundamental 1ª a 4ª séries', value: 54 },
    { label: 'Ensino Fundamental 5ª a 8ª séries', value: 55 },
    { label: 'Ensino Fundamental Completo', value: 56 },
    { label: 'Ensino Fundamental Especial', value: 61 },
    { label: 'Ensino Fundamental EJA - Séries Iniciais (Supletivo 1ª a 4ª)', value: 58 },
    { label: 'Ensino Fundamental EJA - Séries Finais (Supletivo 5ª a 8ª)', value: 59 },
    { label: 'Ensino Médio, Médio 2º Ciclo (Científico, Técnico e etc)', value: 60 },
    { label: 'Ensino Médio Especial', value: 57 },
    { label: 'Ensino Médio EJA (Supletivo)', value: 62 },
    { label: 'Superior, Aperfeiçoamento, Especialização, Mestrado, Doutorado', value: 63 },
    { label: 'Alfabetização para Adultos (Mobral, etc)', value: 64 },
    { label: 'Nenhum', value: 65 }
  ],
  origens: [
    { label: 'Retaguarda', value: 'Retaguarda' },
    { label: 'Individuo', value: 'Individuo' }
  ],
  tabelas: [
    { label: 'Profissional', value: 'Profissional' },
    { label: 'Individuo', value: 'Individuo' },
    { label: 'Acompanhamento', value: 'Acompanhamento' },
    { label: 'Usuario', value: 'Usuario' },
    { label: 'Agendamento', value: 'Agendamento' },
    { label: 'Atendimento', value: 'Atendimento' },
    { label: 'Procedimento', value: 'Procedimento' },
    { label: 'Especialidade', value: 'Especialidade' },
    { label: 'Estabelecimento', value: 'Estabelecimento' },
    { label: 'Fiscalizacao', value: 'Fiscalizacao' },
    { label: 'Intimacao', value: 'Intimacao' },
    { label: 'Visita', value: 'Visita' }
  ],
  tipoConselho: [
    { label: 'CRM', value: 'CRM' },
    { label: 'COREM', value: 'COREN' },
    { label: 'CRO', value: 'CRO' }
  ]
}
