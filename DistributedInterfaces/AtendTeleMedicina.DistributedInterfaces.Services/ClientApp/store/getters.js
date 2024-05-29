const getters = {
  token: state => state.user.token,
  roles: state => state.user.roles,
  claims: state => state.user.claims,
  selectRole: state => state.user.selectRole,
  user: state => state.user,
  filters: state => state.user.filters,
  isDemandaEspontanea: state => state.app.demandaEspontanea,
  //avatar: state => state.user.dados.imagem,
  // name: state => state.user.dados.nome,
  // uid: state => state.user.dados.sub,
  // rotas: state => state.user.dados.rotas,
  // isCollapse: state => state.app.isCollapse
}

export default getters
