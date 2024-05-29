import Vue from 'vue'
import Router from 'vue-router'

import Full from 'components/containers/Full'
import Offline from 'components/security/Offline'
import SenhaIndividuo from 'components/security/SenhaIndividuo'
import Home from '../pages/Home'

Vue.use(Router)

export default new Router({
  mode: 'history',
  routes: [
    {
      path: '/',
      redirect: '/home',
      name: 'Inicio',
      component: Full,
      meta: {
        auth: ['Offline'],
        titulo: 'Home',
        oculta: false
      },
      children: [
        {
          path: '/',
          component: Home,
          name: 'Home',
          meta: {
            auth: ['Offline'],
            titulo: 'Home',
            oculta: false
          }
        }
      ]
    },
    // FORA DO CONTAINER
    {
      path: '/403',
      name: 'Offline',
      component: Offline,
      meta: {auth: false}
    },
    {
      path: '/login',
      name: 'Offline',
      component: Offline,
      meta: {auth: false}
    },
    {
      path: '/individuo/redefinir-senha',
      name: 'SenhaIndividuo',
      component: SenhaIndividuo,
      meta: {auth: false}
    }
  ]
})
