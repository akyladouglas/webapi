import MenuHistorico from '../pages/atendimento/MenuHistorico'

export default {
  children: [
    {
      path: '/historico',
      name: 'historico',
      component: MenuHistorico,
      meta: {
        auth: ['Administrador', 'GestorEstado', 'GestorMunicipio'],
        title: 'historico',
        hidden: true
      }
    },
  ]
}
