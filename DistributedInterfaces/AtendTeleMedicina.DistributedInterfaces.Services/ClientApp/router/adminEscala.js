import AdminEscala from '../pages/admin/Escala/Escala'

export default {
  children: [
    {
      path: '/escala',
      name: 'Escala',
      component: AdminEscala,
      meta: {
        auth: ['Administrador', 'GestorEstado', 'GestorMunicipio'],
        title: 'Escala',
        hidden: true
      }
    }
  ]
}
