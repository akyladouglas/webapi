import Perfil from '../pages/admin/Perfil'

export default {
  children: [
    {
      path: '/admin/perfil',
      name: 'Perfil',
      component: Perfil,
      meta: {
        auth: ['Administrador', 'GestorEstado', 'GestorMunicipio', 'AdmMunicipio', 'Atendente', 'Médico', 'Recepcionista', 'Triagem'],
        title: 'Perfil',
        hidden: true
      }
    },
  ]
}
