const atendimento = {
  state: {
    dentroDoAtendimento: false,
    finalizado: false,
  },
  mutations: {
    SET_DENTRO_DO_ATENDIMENTO: (state, payload) => {
      state.dentroDoAtendimento = payload
    },
    SET_FINALIZADO: (state, payload) => {
      state.finalizado = payload
    },
  },
  actions: {
    setFinalizado({ commit }, userPayload) {
      commit('SET_FINALIZADO', userPayload)
    },
    setDentroDoAtendimento({ commit }, userPayload) {
      commit('SET_DENTRO_DO_ATENDIMENTO', userPayload)
    },
  }
}

export default atendimento
