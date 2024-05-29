import AdminProfissional from '../pages/admin/Profissional/Profissional'
import AdminProfissionalEstabelecimento from '../pages/admin/Profissional/ProfissionalEstabelecimento'

export default {
  children: [
    {
      path: '/profissional',
      name: 'Profissionais',
      component: AdminProfissional,
      meta: {
        auth: ['Administrador', 'GestorEstado', 'GestorMunicipio'],
        title: 'Profissional',
        hidden: true
      }
    },
    {
      path: '/profissional-estabelecimento',
      component: AdminProfissionalEstabelecimento,
      name: 'ProfissionalEstabelecimento',
      meta: {
        auth: ['Administrador', 'GestorEstado', 'GestorMunicipio'],
        title: 'Profissional - Estabelecimento',
        hidden: true
      }
    }
  ]
}
