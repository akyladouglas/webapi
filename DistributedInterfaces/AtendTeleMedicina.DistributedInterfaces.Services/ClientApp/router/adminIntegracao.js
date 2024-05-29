import AdminIntegracao from '../pages/admin/Integracao/Integracao'

export default {
  children: [
    {
      path: '/integracao',
      name: 'Integracao',
      component: AdminIntegracao,
      meta: {
        auth: ['Administrador'],
        title: 'Integração',
        hidden: true
      }
    }
  ]
}
