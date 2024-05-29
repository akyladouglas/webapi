import AdminInformacao from '../pages/admin/informacoes/Informacao'

export default {
  children: [
    {
      path: '/informacao',
      name: 'informacao',
      component: AdminInformacao,
      meta: {
        auth: ['Administrador', 'GestorEstado', 'GestorMunicipio'],
        title: 'Informacao',
        hidden: true
      }
    }
  ]
}
