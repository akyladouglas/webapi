const user = {
  state: {
    userName: null, // temporario para permitir restaurar o state do vuex no f5 (ver app.js)
    dados: {},
    roles: [],
    claims: [],
    selectRole: "",
    filters: {},
    formModalInformacoesAdicionais: {} //form do modal de informações adicionais dentro do atendimento
  },
  mutations: {
    SET_FORM: (state, payload) => {
      state.formModalInformacoesAdicionais = payload
    },
    LIMPAR_FORM: (state) => {
      state.formModalInformacoesAdicionais = {}
    },
    REHYDRATE_USER: (state, payLoad) => {
      state = { ...payLoad }
    },

    SIGN_IN: (state, userPayload) => {
      state.dados = userPayload
      /*state.roles = userPayload.roles*/
    },

    SET_INFO: (state, userPayload) => {
      state.dados = userPayload
    },

    SET_ROLES: (state, roles) => {
      state.roles = roles
    },

    SET_CLAIMS: (state, userPayload) => {
      var arrClaims = []
      
      state.roles.forEach(stringRole => {
        if (stringRole != "Retaguarda" && stringRole != "Profissional" && stringRole != "Usuario") {
          arrClaims.push(stringRole)
        }
      })
      state.dados.userClaims = arrClaims
      state.claims = arrClaims
    },

    SET_FILTERS: (state, filters) => {
      state.filters = filters
    },

    SIGN_OUT: (state) => {
      state.dados = {}
      state.roles = []
      state.claims = []
      state.selectRole = ""
      localStorage.clear()
      location.reload()
    },

    SELECT_PERFIL: (state, role) => {
      state.selectRole = role
    },

  },
  actions: {
    setFormInformacoes({ commit }, userPayload) {
      commit('SET_FORM', userPayload)
    },

    limparFormInformacoes({ commit }) {
      commit('LIMPAR_FORM')
    },

    async selectPerfil({ commit }, role) {
      await commit('SELECT_PERFIL', role)
    },

    rehydrateUser({ commit }, userPayload) {
      commit('REHYDRATE_USER', userPayload)
    },

    async signIn({ commit }, userPayload) {

      if (userPayload.ultimoPerfilSelecionado != "" && userPayload.ultimoPerfilSelecionado != undefined) {
         await commit('SELECT_PERFIL', userPayload.ultimoPerfilSelecionado)
      } 

      commit('SIGN_IN', userPayload)
      commit('SET_ROLES', userPayload.roles)
      commit('SET_CLAIMS', userPayload)
    },

    // Utilizado para setar os dados de volta no store em caso de F5
    // ESSE ACTION NÃO ESTA SENDO CHAMADO NO F5 E SIM O SIGNIN
    setInfo({ commit }, userPayload) {
      commit('SET_INFO', userPayload.dados)
      commit('SET_ROLES', userPayload.roles)
      commit('SET_CLAIMS', userPayload.claims)
    },

    setFilters ({commit}, userPayload) {
      commit('SET_FILTERS', userPayload)
    },

    async signOut({ commit }) {
      commit('SIGN_OUT')
    },
  }
}

export default user
