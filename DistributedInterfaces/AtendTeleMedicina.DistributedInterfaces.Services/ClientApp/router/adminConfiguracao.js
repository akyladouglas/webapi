import AdminConfiguracao from '../pages/admin/Configuracao/Configuracao'

export default {
  children: [
    {
      path: '/configuracao',
      name: 'Configuracao',
      component: AdminConfiguracao,
      meta: {
        auth: ['Administrador'],
        title: 'Configuração',
        hidden: true
      }
    }
  ]
}
