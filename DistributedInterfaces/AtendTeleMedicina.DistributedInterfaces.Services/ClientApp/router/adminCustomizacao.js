import AdminCustomizacao from '../pages/admin/customizacao/Customizacao'

export default {
  children: [
    {
      path: '/customizacao',
      name: 'Customizacao',
      component: AdminCustomizacao,
      meta: {
        auth: ['Administrador', 'GestorEstado', 'GestorMunicipio'],
        title: 'Customizacao',
        hidden: true
      }
    },
  ]
}
