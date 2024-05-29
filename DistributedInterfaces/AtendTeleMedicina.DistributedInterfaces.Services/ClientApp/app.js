import Vue from 'vue'
import axios from 'axios'
import axiosRetry from 'axios-retry'
import VueAxios from 'vue-axios'
// import router from './router/off'
import router from './router/index'
import store from './store'
import Vuex from 'vuex'
import VueAuth from '@websanova/vue-auth'
import VueAuthCustoms from './lib/custom-auth'
import moment from 'moment'
import lodash from 'lodash'
import lodashGet from 'lodash/get'
import { sync } from 'vuex-router-sync'
import App from 'components/app-root'
import { FontAwesomeIcon } from './icons'
// element-ui
import ElementUI from 'element-ui'
import locale from 'element-ui/lib/locale/lang/pt-br'
import './assets/css/style.css'

// highcharts
import VueHighcharts from 'vue-highcharts'
import Highcharts from 'highcharts'
import loadHighchartsMore from 'highcharts/highcharts-more'
import loadSolidGauge from 'highcharts/modules/solid-gauge'

import VueNotify from 'vue-notifyjs'
import 'vue-notifyjs/themes/default.css';

import VueSweetalert2 from 'vue-sweetalert2'; // https://sweetalert2.github.io/
import 'sweetalert2/dist/sweetalert2.min.css';
const options = {
  confirmButtonColor: '#8fd9cd',
}

import money from 'v-money'
import VueCurrencyFilter from 'vue-currency-filter'

import VModal from 'vue-js-modal'

// Auth
import User from './api/services/User'
import $config from './api/config'

Vue.use(VModal)
Vue.use(User)
Vue.use(ElementUI, { locale })
Vue.use(Vuex)
Vue.use(VueNotify)
Vue.use(VueSweetalert2, options)
Vue.component('icon', FontAwesomeIcon)
Vue.use(money, {precision: 2})
Vue.use(VueCurrencyFilter,
  {
    symbol: 'R$',
    thousandsSeparator: '.',
    fractionCount: 2,
    fractionSeparator: ',',
    symbolPosition: 'front',
    symbolSpacing: true
  })

// highcharts
loadHighchartsMore(Highcharts)
loadSolidGauge(Highcharts)
Vue.use(VueHighcharts, { Highcharts })

Vue.use(VueAxios, axios)
axios.defaults.baseURL = '/'
// axios.defaults.headers.common['X-Requested-With'] = 'XMLHttpRequest'
// axios.defaults.headers = { 'Content-Type': 'application/json' }
// axios.defaults.data = {}

axiosRetry(axios, { retries: 3,
  retryDelay: (retryCount) => {
    return retryCount * 1000
  }})

// Necessário para o vue-auth
Vue.router = router

Vue.use(VueAuth, {
  tokenDefaultName: 'access_token',
  loginData: {url: '/api/v1/auth/login', method: 'POST', redirect: false, fetchUser: false},
  logoutData: {redirect: '/login', makeRequest: false},
  // fetchData False por conta da opção por parseUserData pegar os dados do localstorage
  fetchData: {url: '/api/v1/auth/userinfo', method: 'GET', enabled: false},
  refreshData: {url: '/api/v1/auth/refresh', method: 'POST', enabled: true, interval: 0},
  parseUserData: VueAuthCustoms.parseUserData,
  notFoundRedirect: false,
  auth: VueAuthCustoms.drivers.dotNetJWT,
  http: require('@websanova/vue-auth/drivers/http/axios.1.x.js'),
  router: require('@websanova/vue-auth/drivers/router/vue-router.2.x.js'),
  rolesVar: 'roles'
})

// Verifica se token expirou
Vue.user.checkToken()

Vue.router.beforeEach((to, from, next) => {
  if (to.meta.auth !== false) {
    if (Vue.user.checkToken()) {

      if (to.name === 'Atendimento') {
        // Entrando pela primeira vez no atendimento
        next();
      }
      // Se o usuário tentou navegar para outra tela que não seja a do atendimento
      else {
        // Verificando se ele está dentro do atendimento e se o atendimento está finalizado
        if (store.state.atendimento.dentroDoAtendimento == true && store.state.atendimento.finalizado == true) {
          next()
        }
        // Se o usuário tentou navegar para outra tela que não seja a do atendimento e o atendimento não está finalizado
        else if (store.state.atendimento.dentroDoAtendimento == true && store.state.atendimento.finalizado == false) {
          Vue.prototype.$swal({
            title: 'Atendimento não finalizado',
            text: 'Você precisa finalizar o atendimento antes de sair para outra tela',
            icon: 'warning',
          })
        }
        else {
          next()
        }
      }

    } else {
      // Token expirado, redirecione para a página de login
      next({ name: 'Login', params: { redirectFrom: to.name } });
    }
  } else {
    // Página de login
    next();
  }
});

// Response interceptor
axios.interceptors.response.use(function (response) {
  // Tratamento antes do response data
  return response
}, function (error) {
  // Tratamento depois do response data
  if (error.response.status === 401) {
    Vue.user.logout()
  }
  // Forma que o erro será apresentado
  return Promise.reject(error)
})

// Vue.prototype.$http = axios
Vue.prototype._ = lodash
Vue.prototype.$elvis = lodashGet
Vue.prototype.moment = moment
Vue.prototype.$config = $config

moment.locale('pt-br')

sync(store, router)

$config.getConfig()

const app = new Vue({
  store,
  router,
  ...App
})

export {
  app,
  router,
  store
}
