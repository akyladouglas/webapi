import Vue from 'vue'
import Vuex from 'vuex'
import app from './modules/app'
import user from './modules/user'
import atendimento from './modules/atendimento'
import getters from './getters'
// import createPersistedState from 'vuex-persistedstate'

Vue.use(Vuex)

const store = new Vuex.Store({
  modules: {
    user,
    app,
    atendimento
  },
  getters,
  // plugins: [createPersistedState()]
})

export default store
