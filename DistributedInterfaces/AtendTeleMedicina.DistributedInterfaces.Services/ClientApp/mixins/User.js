 import axios from 'axios'
import b64 from 'base-64'

export default {
  data () {
    return {
      usuario: {},
      tokenData: {
        access_token: null,
        expires_in: 0,
        refresh_token: null
      },
      tokenRefresh: false,
      tokenErro: null
    }
  },
  beforeMount () {
    this.$auth.ready()
  },
  methods: {
    userRefreshToken () {
      axios({
        method: 'post',
        url: '/auth/refresh',
        data: { grant_type: 'refresh_token' },
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer ' + localStorage.getItem('refresh_token')
        }
      })
        .then(response => {
          this.tokenData = response.data
          this.tokenRefresh = true
          this.userSet(this.tokenData)
          // location.reload() // após implantação do store remover e passar para o tokentimer o valor do store.user.expires_in
        })
        .catch((e) => {
          this.tokenData = {}
          this.tokenErro = 'Refresh Token já havia expirado, faça a autenticação novamente.'
        })
    },
    userSet (token) {
      this.tokenData = token
      axios.get('/api/v1/auth/userinfo', {
        headers: {'Authorization': 'Bearer ' + token.access_token}
      })
        .then(res => {
          let userInfo = JSON.stringify(res.data)
          this.$auth.user(res.data) // sem stringify
          localStorage.setItem('userInfo', b64.encode(userInfo))
          this.userSetStorage(this.tokenData)
        })
        .catch(e => {
          console.log(e.response.data)
          this.$auth.logout()
        })
    },
    userSetStorage () {
      if (this.tokenRefresh) {
        this.$auth.token(null, this.tokenData.access_token) // vue-auth seta o token no header
        localStorage.setItem('refresh_token', this.tokenData.refresh_token)
        localStorage.setItem('expires_in', this.tokenData.expires_in)
      }
      // TODO Salvar os dados no store aqui
    },
    userLogout () {
      localStorage.clear()
      this.$auth.logout()
    }
  }
}
