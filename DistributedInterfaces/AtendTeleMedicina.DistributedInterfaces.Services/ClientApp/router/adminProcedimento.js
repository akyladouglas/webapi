import AdminProcedimento from '../pages/admin/Procedimento/Procedimento'

export default {
  children: [
    {
      path: '/procedimento',
      name: 'Procedimentos',
      component: AdminProcedimento,
      meta: {
        auth: ['Administrador', 'GestorEstado', 'GestorMunicipio'],
        title: 'Procedimentos',
        hidden: true
      }
    }
  ]
}
