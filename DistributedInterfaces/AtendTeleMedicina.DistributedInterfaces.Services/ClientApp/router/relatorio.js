//import RelatorioIndividuo from '../pages/relatorios/RelatorioIndividuo'
import RelatorioAtendimento from '../pages/relatorios/RelatorioAtendimento'
import RelatorioGlicemia from '../pages/relatorios/RelatorioGlicemia'
//import RelatorioAtendimentoPaciente from '../pages/relatorios/RelatorioAtendimentoPaciente'
//import RelatorioGrauRiscoPorUf from '../pages/relatorios/RelatorioGrauRiscoPorUf'
//import RelatorioCadastrosUf from '../pages/relatorios/RelatorioCadastrosUf'
//import RelatorioTotalCadastros from '../pages/relatorios/RelatorioTotalCadastros'
//import DashBoardRelatorio from '../pages/dashboard/DashBoardRelatorios'

export default {
  children: [
    //{
    //  path: '/relatorios/usuario',
    //  name: 'Relatorio de Indivíduos',
    //  component: RelatorioIndividuo,
    //  meta: {
    //    auth: ['Administrador'],
    //    title: 'Cidadãos',
    //    hidden: false
    //  }
    //},
    //{
    //  path: '/relatorios/total-de-cadastros',
    //  component: RelatorioTotalCadastros,
    //  name: 'Relatorio Total de Cadastros',
    //  meta: {
    //    auth: ['Administrador', 'GestorEstado', 'GestorMunicipio'],
    //    title: 'Total de Cadastros',
    //    hidden: false
    //  }
    //},
    //{
    //  path: '/relatorios/cadastros-por-uf',
    //  component: RelatorioCadastrosUf,
    //  name: 'Relatório Cadastros por UF',
    //  meta: {
    //    auth: ['Administrador', 'GestorEstado'],
    //    title: 'Cadastros por UF',
    //    hidden: false
    //  }
    //},
    //{
    //  path: '/relatorios/grau-de-risco-por-uf',
    //  component: RelatorioGrauRiscoPorUf,
    //  name: 'Relatório Grau de Risco UF',
    //  meta: {
    //    auth: ['Administrador', 'GestorEstado', 'GestorMunicipio'],
    //    title: 'Grau de Risco UF',
    //    hidden: false
    //  }
    //},
    //{
    //  path: '/relatorios/relatorio-atendimento-paciente',
    //  component: RelatorioAtendimentoPaciente,
    //  name: 'Relatorio Atendimento Paciente',
    //  meta: {
    //    auth: ['Administrador', 'GestorEstado', 'GestorMunicipio'],
    //    title: 'Relatório Paciente',
    //    hidden: false
    //  }
    //},
    {
      path: '/relatorios/relatorio-atendimento',
      component: RelatorioAtendimento,
      name: 'Relatorio Atendimentos',
      meta: {
        auth: ['Administrador', 'GestorEstado', 'GestorMunicipio', 'Médico'],
        title: 'Relatório Atendimentos',
        hidden: false
      }
    },{
      path: '/relatorios/relatorio-glicemia',
      component: RelatorioGlicemia,
      name: 'RelatorioGlicemia',
      meta: {
        auth: ['Administrador'],
        title: 'Relatório de Glicemias',
        hidden: false
      }
    },
    //{
    //  path: '/relatorios/dashboard',
    //  component: DashBoardRelatorio,
    //  name: 'DashBoard',
    //  meta: {
    //    auth: ['Administrador', 'GestorEstado', 'GestorMunicipio'],
    //    title: 'DashBoard',
    //    hidden: false
    //  }
    //}
  ]
}
