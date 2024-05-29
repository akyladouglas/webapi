<template>
  <section class="content animated fadeIn">
    <div class="wrapper">
      <form id="formData" class="login-form" v-if="$auth.ready()">
        <div class="text-center login--logo">
          <img 
          :src="'../../assets/img/' + $store.state.app.empresa.logo" 
          class="img-responsive center-block" 
          :title="$store.state.app.empresa.nome" />
        </div>
        <div class="form-group text-center">
          BackOffice Indisponível
        </div>
      </form>

    </div>

  </section>
</template>

<script>
import { Notification } from 'element-ui'
import b64 from 'base-64'

export default {
  name: 'login',
  data () {
    return {
      erro: '',
      loading: false
    }
  },
  created () {
    this.clearAllCookies()
  },
  methods: {
    clearAllCookies () {
      var cookies = document.cookie.split('; ')
      for (var c = 0; c < cookies.length; c++) {
        var d = window.location.hostname.split('.')
        while (d.length > 0) {
          var cookieBase = encodeURIComponent(cookies[c].split(';')[0].split('=')[0]) + '=; expires=Thu, 01-Jan-1970 00:00:01 GMT; domain=' + d.join('.') + ' ;path='
          var p = location.pathname.split('/')
          document.cookie = cookieBase + '/'
          while (p.length > 0) {
            document.cookie = cookieBase + p.join('/')
            p.pop()
          };
          d.shift()
        }
      }
    },
  }
}
</script>

<style scoped>

html {
  background: #263238 !important;
}

.wrapper {
  background: #263238;
  padding-top: 90px;
}

.login-form {
  max-width: 380px;
  padding: 15px 35px 45px;
  width: 360px;
  margin: 0 auto 0 auto;
  background-color: #fff;
  border: 1px solid rgba(0, 0, 0, 0.1);
}

.login-form .btn-block {
    border-radius: 0;
    background: #2B54A3;
    text-transform: uppercase;
    font-size: 1.05em;
    border: 0;
    padding: 15px 0;
}

.login--logo {
  margin-bottom: 35px;
}

.login--logo img {
  width: 235px;
}

.login--botao {
  margin: 35px -36px -46px;
}

</style>
