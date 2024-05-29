const app = {
  state: {
    empresa: {
      logoNove: 'LOGO-branco.png',
      logoNoveCollapsed: 'logo-image-collapse.png',
      fotoUsuario: 'foto-usuario.png',
      logo2: 'logo-atendtelemedicina2.png'
    },
    pathFotos: '../../Fotos/',
    demandaEspontanea: false,
    intervaloConsulta: 15,
    agenda: {
      horaInicial: 7 * 60,
      horaFinal: 18 * 60
    },
    isCollapse: true,
    keyPath: ['0', '1'],
    tableOnly: false
  },
  mutations: {
    SET_IS_COLLAPSE: (state, payLoad) => {
      state.isCollapse = payLoad
      localStorage.setItem('isCollapse', payLoad)
    },
    SET_KEYPATH: (state, payLoad) => {
      state.keyPath = payLoad
      localStorage.setItem('path', JSON.stringify(payLoad))
    },
    SET_TABLE_ONLY: (state, payLoad) => {
      state.tableOnly = payLoad
    },
    SET_DEMANDA_ESPONTANEA: (state, payLoad) => {
      state.demandaEspontanea = payLoad
    },
    SET_AGENDA: (state, payLoad) => {
      state.agenda = payLoad
    }
  },
  actions: {
    setIsCollapse ({commit}, payLoad) {
      commit('SET_IS_COLLAPSE', payLoad)
    },
    setKeyPath ({commit}, payLoad) {
      commit('SET_KEYPATH', payLoad)
    },
    setTableOnly ({commit}, payLoad) {
      commit('SET_TABLE_ONLY', payLoad)
    },
    setDemandaEspontanea ({commit}, payLoad) {
      commit('SET_DEMANDA_ESPONTANEA', payLoad)
    },
    setAgenda ({commit}, payLoad) {
      commit('SET_AGENDA', payLoad)
    }
  },
  getters: {
    isCollapse (state) {
      return state.isCollapse
    },
    demandaEspontanea (state) {
      return state.demandaEspontanea
    },
    keyPath (state) {
      return state.keyPath
    },
    tableOnly (state) {
      return state.tableOnly
    },
    agenda (state) {
      return state.agenda
    }
  }
}

export default app
