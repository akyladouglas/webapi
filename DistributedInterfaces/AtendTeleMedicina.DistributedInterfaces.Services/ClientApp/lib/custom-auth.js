import b64 from 'base-64'
import Vue from 'vue'

export default {
  drivers: {
    dotNetJWT: {
      request: function (req, token) {
        // Token antes do Refresh
        token = token.split(';')
        token = req.url.indexOf('refresh') > -1 ? token[1] : token[0]

        this.options.http._setHeaders.call(this, req, {Authorization: `Bearer ${localStorage.getItem('refresh_token')}`})
      },
      response: function (res) {
        // Token depois do refresh
        // console.log('acionou o refresh token')
        if (res.data.access_token && res.data.refresh_token && res.data.expires_in) {
          Vue.user.setUserStorage(res.data)
          return res.data.access_token + ';' + res.data.refresh_token + ';' + res.data.expires_in
        }
        // Funcionando como deveria mas só location.reload
        // if (res.status === 401 || res.status === 400) {
        //   localStorage.clear()
        //   Vue.auth.user()
        //   location.reload()
        // }
      }
    }
  },
  tokenExpired: function () {
    // console.log(Router.history.current)
    // Quase OK sem utilizar essa função, o problema que ocorre é que não consigo atualizar os valores do localstorage com os novos dados do refreshToken
    // var expires = localStorage.getItem('expires_in')
    // // if (Date.now() < expires) return false
    // if (Date.now() > expires) {
    //   localStorage.clear()
    //   return false
    // } else {
    //   return true
    // }
  },
  // Buscar os dados no localstore evitando consulta ao banco
  parseUserData: function () {
    let b64Data = b64.decode(localStorage.getItem('userInfo'))
    // Evitando o uso do base-64 no boot da aplicação, caso contrário um erro é gerado no JSON.parse
    if (b64Data.length > 10) var data = JSON.parse(b64Data)
    return data
  }
  // Opção padrão para buscar os dados no endpoint /auth/user precisa ser tratado pois
  // o parser padrão busca os dados do token em data.data
  // parseUserData: function (data) {
  //   return data
  // },
}
