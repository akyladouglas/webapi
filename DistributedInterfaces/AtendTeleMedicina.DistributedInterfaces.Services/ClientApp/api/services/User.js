import Vue from 'vue'
import axios from 'axios'
import b64 from 'base-64'
import Router from '../../router'
import store from '../../store'

var UserPlugin = {


  refreshToken() {
    console.log("erro")
    return axios({
      method: 'post',
      url: '/api/auth/refresh',
      // data: { grant_type: 'refresh_token' },
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + localStorage.getItem('refresh_token')
      }
    })
      .then(res => {
        this.setUser(res.data)
        return res.data
      })
      .catch((e) => {
        console.log(e)
      })
  },
  async setUser(token) {
    if (!token) {
      this.logout()
    }
    await axios.get('/api/v1/auth/userinfo', {
      headers: { 'Authorization': 'Bearer ' + token.access_token }
    })
      .then(res => {
        let userInfo = JSON.stringify(res.data)
        Vue.prototype.$auth.user(res.data) // sem stringify
        localStorage.setItem('userInfo', b64.encode(userInfo))
        var newUserInfo = JSON.parse(userInfo)
        store.dispatch('signIn', newUserInfo)
        this.setUserStorage(token, 'vimDoSetUser')
      })
      .catch(e => {
        console.log(e.response)
        return e
        // Vue.prototype.$auth.logout()
      })
  },
  async setUserStorage(token, origem) {
    /*console.log("CHAMOU O SET USER STORAGE DE ALGUM LUGAR")*/

    if (origem == "vimDoSetUser") {
      Vue.auth.token(null, token.access_token) // vue-auth seta o token no header
      localStorage.setItem('access_token', token.access_token)
      localStorage.setItem('refresh_token', token.refresh_token)
      localStorage.setItem('expires_in', token.expires_in)
      //let userInfo = localStorage.getItem('userInfo')
      //if (userInfo) {
      //  store.dispatch('signIn', JSON.parse(b64.decode(userInfo)))
      //}
    } else {

      //SÓ VAI CAIR NO ELSE QUANDO ELE FOR DAR O REFRESH NA TELA
      //POR CAUSA QUE QUANDO DA O REFRESH ELE NÃO CHEGA NO METODO setUser ELE CAI DIRETO NO setUserStorage
      //POR ISSO O IF ELSE
      this.recoverUser(token)
    }
  },

  async recoverUser(token) {
    await axios.get('/api/v1/auth/userinfo', {
      headers: { 'Authorization': 'Bearer ' + token.access_token }
    })
      .then(res => {
        let userInfo = JSON.stringify(res.data)
        Vue.prototype.$auth.user(res.data) // sem stringify
        localStorage.setItem('userInfo', b64.encode(userInfo))
        var newUserInfo = JSON.parse(userInfo)
        store.dispatch('signIn', newUserInfo)

        //UM VIMDOSETUSER FAKE PARA ELE ENTRAR NO IF DENTRO DA FUNÇÃO DELE
        this.setUserStorage(token, 'vimDoSetUser')
      })
      .catch(e => {})
  },

  checkToken () {
    let expires = localStorage.getItem('expires_in')
    if (!expires) return false
    if (Date.now() > expires) {
      localStorage.clear()
      Vue.prototype.$auth.user()
      return false
    } else {
      return true
    }
  },
  logout () {
    localStorage.clear()
    Vue.prototype.$auth.user(null)
    Router.replace({name: 'Login', params: { redirectFrom: Router.history.current.name }})
    // Vue.auth.logout()
  }

  // loginOld (user) {
  //   let tokenData
  //   tokenData = jwtDecode(user.access_token)
  //   this.setToken(user.access_token, tokenData.exp)
  //   // 2ª chamada para dados adicionais do usuário
  //   HTTP({
  //     method: 'get',
  //     url: 'userinfo',
  //     headers: {
  //       'Content-Type': 'application/json',
  //       'Authorization': 'Bearer ' + user.access_token
  //     }
  //   })
  //     .then(response => {
  //       let claims = []
  //       let userPayload
  //       // Fazendo um foreach para tratar os claims, que não estão retornando do token e sim da consulta ao banco
  //       response.data.userClaims.forEach(item => {
  //         claims.push({
  //           claimType: item.claimType,
  //           claimValue: item.claimValue
  //         })
  //       })
  //       userPayload = {
  //         dados: response.data,
  //         roles: tokenData.role,
  //         claims: claims,
  //         token: user.access_token
  //       }
  //       store.dispatch('signIn', userPayload)
  //       // Setando dados no localStorage para recuperação em caso de F5
  //       localStorage.setItem('userRegulacao', JSON.stringify(userPayload.dados))
  //       localStorage.setItem('rolesRegulacao', JSON.stringify(userPayload.roles))
  //       localStorage.setItem('claimsRegulacao', JSON.stringify(userPayload.claims))
  //       // TODO confirmar se ainda é utilizado essa verificação
  //       // if (userPayload.dados.cnes != null) {
  //       //   localStorage.setItem('userCnes', userPayload.dados.cnes)
  //       // }
  //       // setando informações adicionais
  //       store.dispatch('setFichasTotal')
  //       store.dispatch('setPacientesTotal')
  //       $config.abreToast('Bem vindo(a)', userPayload.dados.nomeCompleto, 'success')
  //     })
  //     .catch(e => {
  //       console.log(e)
  //       $config.abreToast('Erro', 'Erro na recuperação do token.', 'error')
  //       router.push({name: 'Login'})
  //     })
  // },
  // logOutOld () {
  //   localStorage.clear()
  //   store.dispatch('signOut')
  //   store.dispatch('clearApp')
  //   router.replace('/auth/login')
  // },
  // // v7 utilizado para recuperar os dados em caso de F5
  // getUser () {
  //   let userPayload = {
  //     dados: JSON.parse(localStorage.getItem('userRegulacao')),
  //     roles: JSON.parse(localStorage.getItem('rolesRegulacao')),
  //     claims: JSON.parse(localStorage.getItem('claimsRegulacao'))
  //   }
  //   return userPayload
  // },
  // getToken () {
  //   let token = localStorage.getItem('access_token')
  //   let expiration = localStorage.getItem('expires_in')
  //
  //   if (!token || !expiration) return null
  //
  //   // TODO checar expiração do token, o valor retornado é inválido ou incompleto, adicionando '000' para tornar o formato válido
  //   if (Date.now() > parseInt(localStorage.getItem('expires_in'))) {
  //     // console.log('token expirado')
  //     this.logOut()
  //     return null
  //   } else {
  //     return token
  //   }
  // },
  // loggedIn () {
  //   if (this.getToken()) {
  //     return true
  //   } else {
  //     return false
  //   }
  // }
}

export default function (Vue) {
  Vue.user = UserPlugin

  Object.defineProperties(Vue.prototype, {
    $user: {
      get () {
        return Vue.user
      }
    }
  })
}
// força o refresh do token $auth.refresh()
