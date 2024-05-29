import AdminNotificacao from '../pages/admin/notificacoes/Notificacao'

export default {
  children: [
    {
      path: '/notificacao',
      name: 'notificacao',
      component: AdminNotificacao,
      meta: {
        auth: ['Administrador', 'GestorEstado', 'GestorMunicipio'],
        title: 'Notificacao',
        hidden: true
      }
    }
  ]
}
