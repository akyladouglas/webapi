import AdminEstabelecimento from '../pages/admin/Estabelecimento/Estabelecimento'
import AdminEstabelecimentoProcedimentos from '../pages/admin/Estabelecimento/EstabelecimentoProcedimentos'
import AdminEstabelecimentoCotas from '../pages/admin/Estabelecimento/EstabelecimentoCotas'
import AdminEstabelecimentoHorarios from '../pages/admin/Estabelecimento/EstabelecimentoHorarios'
import AdminEstabelecimentoProcedimentoHorario from '../pages/admin/Estabelecimento/EstabelecimentoProcedimentoProfissional'

export default {
  children: [
    {
      path: '/estabelecimento',
      name: 'Estabelecimentos',
      component: AdminEstabelecimento,
      meta: {
        auth: ['Administrador', 'GestorEstado', 'GestorMunicipio'],
        title: 'Estabelecimentos',
        hidden: true
      }
    },
    {
      path: '/estabelecimento-procedimentos',
      component: AdminEstabelecimentoProcedimentos,
      name: 'EstabelecimentoProcedimentos',
      meta: {
        auth: ['Administrador', 'GestorEstado', 'GestorMunicipio'],
        title: 'Estabelecimento - Procedimentos',
        hidden: true
      }
    },
    {
      path: '/estabelecimento-cotas',
      component: AdminEstabelecimentoCotas,
      name: 'EstabelecimentoCotas',
      meta: {
        auth: ['Administrador', 'GestorEstado', 'GestorMunicipio'],
        title: 'Estabelecimento - Cotas',
        hidden: true
      }
    },
    {
      path: '/estabelecimento-procedimento-horario',
      component: AdminEstabelecimentoProcedimentoHorario,
      name: 'EstabelecimentoProcedimentoProfissional',
      meta: {
        auth: ['Administrador', 'GestorEstado', 'GestorMunicipio'],
        title: 'Estabelecimento Procedimento Profissional',
        hidden: true
      }
    },
    {
      path: '/estabelecimento-horarios',
      component: AdminEstabelecimentoHorarios,
      name: 'EstabelecimentoHorarios',
      meta: {
        auth: ['Administrador', 'GestorEstado', 'GestorMunicipio'],
        title: 'Estabelecimento - Horários',
        hidden: true
      }
    }
  ]
}
