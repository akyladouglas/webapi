import Lista from '../pages/individuo/Lista'

export default {
  children: [
    {
      path: '/individuo/lista',
      name: 'Lista de Indivíduos',
      component: Lista,
      meta: {
        auth: ['Retaguarda', 'GestorEstado', 'GestorMunicipio', 'Retaguarda', 'Atendente'],
        title: 'Lista',
        hidden: false
      }
    }
  ]
}
