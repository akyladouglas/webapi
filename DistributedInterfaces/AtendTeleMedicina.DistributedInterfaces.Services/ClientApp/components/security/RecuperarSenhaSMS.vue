<template>
  <section class="content animated fadeIn">
    <div class="wrapper">
      <form id="formReset" class="login-form" v-if="$auth.ready()">
        <div class="text-center login--logo">
          <img :src="'../../assets/img/' + $store.state.app.empresa.logo2"
               class="img-responsive center-block"
               :title="$store.state.app.empresa.nome" />
        </div>
        <el-form v-show="sms" :model="formSenha" status-icon :rules="validacoes" ref="formSenha" label-width="120px" label-position="top" class="form--individuo--senha">
          <el-row :gutter="20">
            <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
              <h4 class="text-center">Codigo Enviado Via SMS, Verifique Suas Mensagens:</h4><br />
            </el-col>
            <input v-mask="'#########'" class="form-control" v-model="formSMS.codigo" placeholder="Digite seu código" autofocus />
            <h4 style="margin-top: 10px; margin-left: 3px">Não recebeu o código? <el-button class="re-enviar" @click="onReenviar()" type="primary" size="medium">Re-enviar</el-button></h4>
            <br />
            <el-button style=" margin-top:0px; margin-bottom:0px; margin-left:0px; margin-right:10px" @click="onProximo()" type="primary" size="large">
              Próximo
            </el-button>
          </el-row>
        </el-form>

        <FormInputSenha :show="!sms" @emit="onConfirmar" />
      </form>
    </div>
  </section>
</template>

<script>
  import { Notification } from 'element-ui'
  import _api from '../../api'
  import b64 from 'base-64'
  import { mask } from 'vue-the-mask'
  import FormInputSenha from '../shared/FormInputSenha'

  export default {
    name: 'login',
    directives: { mask },
    components: { FormInputSenha },
    data() {
      var validaSenha = (rule, value, callback) => {
        var regexPwd = /^(?=.*[0-9])(?=.*[a-z]).{8,}/
        if (regexPwd.test(value) === false) {
          alert('Erro da senha')
        } else {
          alert('else')
        }
      }
      var validaSenhaConfirmacao = (rule, value, callback) => {
        var regexPwd = /^(?=.*[0-9])(?=.*[a-z]).{8,}/
        if (regexPwd.test(value) === false) {
          alert('Erro da senha')
        } else {
          alert('else')
        }
      }
      return {
        paramsSMS: {},
        sms: true,
        loading: false,
        listando: false,
        showPassword: false,
        showPasswordConfirm: false,
        routeParams: {},
        validacoes: {
          senha: [
            { validator: validaSenha, trigger: ['blur', 'change'] },
            { required: true, message: 'Campo obrigatório', trigger: 'submit' }
          ],
          senhaConfirmacao: [
            { validator: validaSenhaConfirmacao, trigger: ['blur', 'change'] },
            { required: true, message: 'Campo obrigatório', trigger: 'submit' }
          ],
        },
        formSenha: {
          senha: '',
          senhaConfirmacao: '',
          token: ''
        },
        formSMS: {
          codigo: ''
        },
        formGeral: {

        },
        erro: ''
      }
    },
    async mounted() {
      this.routeParams = this.$route.params
      this.paramsSMS = this.routeParams
    },

    methods: {
      onInicio() {
        this.$router.push({
          name: 'Login',
        })
      },

      async onProximo() {
        if (this.formSMS.codigo === this.paramsSMS.codigo) {
          this.sms = false

        } else {
          this.$swal({
            title: "Atenção!",
            text: 'Código de autenticação informado é inválido!',
            icon: 'warning',
          })
          this.sms = true
        }

      },

      async onReenviar() {
        var cpf = this.paramsSMS.RecuperarSMS.cpf ? this.paramsSMS.RecuperarSMS.cpf.replace(/[.-\s]/g, '') : null
        var metodo = this.paramsSMS.RecuperarSMS.metodo
        let { data, status } = await _api.usuarios.getRecuperarSenha(cpf, metodo)
        let teste = (status === 200) ? data : []

      },
      async onConfirmar(val) {
        if (val.params.senha.match(/^(?=.*[0-9])(?=.*[a-z]).{8,}/)) {


          if (val.params.senha == val.params.senhaConfirmacao) {
            var cpf = this.paramsSMS.RecuperarSMS.cpf ? this.paramsSMS.RecuperarSMS.cpf.replace(/[.-\s]/g, '') : null
            var cpfNovo = cpf.toString()
            var senha = val.params.senha.toString()
            var confirmacao = val.params.senhaConfirmacao.toString()
            var codigoAutenticacao = this.formSMS.codigo.toString()

            this.formGeral.cpf = cpfNovo,
              this.formGeral.senha = senha,
              this.formGeral.confirmacao = confirmacao,
              this.formGeral.codigoAutenticacao = codigoAutenticacao

            if (this.paramsSMS.RecuperarSMS.tipo === 'Usuario') {
              let { data, status } = await _api.usuarios.redefinir(this.formGeral)
              let teste = (status === 200) ? data : []
              if (status === 200) {
                this.$router.push({
                  name: 'Home'
                })
              }
            } else if (this.paramsSMS.RecuperarSMS.tipo === 'Profissional') {
              let { data, status } = await _api.profissionais.redefinir(this.formGeral)
              let DataProfissionais = (status === 200) ? data : []
              let StatusProfissionais = status
              if (StatusProfissionais === 200) {
                this.$router.push({
                  name: 'Home'
                })
              }
            } else { }
          } else {
            this.$swal({
              title: "Atenção!",
              text: "As senhas digitadas são diferentes! Informe senhas iguais!",
              icon: 'warning',
            })
          }
        } else {
          this.$swal({
            title: "Erro!",
            text: 'Senha inválida! A senha deve conter 8 caracteres, contendo pelo menos 1 número!',
            icon: 'error',
          })
        }
      },
    }
  }
</script>

<style scoped>

  html, body {
    background: #43bec6 !important;
  }

  .wrapper {
    background: #43bec6;
    padding-top: 90px;
  }

  .wrap-login p {
    word-break: normal !important;
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
      background: #008d95;
      text-transform: uppercase;
      font-size: 1.05em;
      border: 0;
      padding: 15px 0;
    }

  .login--logo {
    margin-bottom: 15px;
    margin-left: 25px;
    width: max-content;
    height: max-content;
  }


    .login--logo img {
      width: 235px;
    }

  .login--botao {
    margin: 35px -36px -46px;
  }


  .login--logo--novetech {
    margin-top: 20px;
    height: 55px
  }


  .login--endereco {
    margin: 0;
    font-size: 0.8rem;
    color: #fff;
    font-weight: 300;
    line-height: 120%;
    padding-top: 10px;
  }

    .login--endereco a:link, .login--endereco a:visited {
      color: #fff;
    }

  .novetech {
    width: 350px;
    margin: 0 auto;
    text-align: center;
  }

  .login--logo--p {
    width: 70px;
    padding-top: 24px;
    padding-right: 11px;
  }

  .l1, .l2 {
    float: right;
  }

  .scroll-area {
    position: relative;
    margin: auto;
    width: 95%;
    height: 710px;
    overflow-x: hidden;
  }

  .modal--termos {
    background: #01826C;
  }

  .el-dialog__body {
    word-break: keep-all;
  }

  .re-enviar {
    background: transparent;
    border: none;
    outline: none;
    font-size: 16px;
    color: #43bec6;
    margin: 0px;
    padding: 0px
  }

  .container-senha {
    /*    background-color: red;*/
    border: 1px solid black;
    width: 90%;
    border-radius: 3px;
    display: flex;
    justify-content: space-around;
    align-items: center;
    text-align: center;
  }

    .container-senha:hover {
      cursor: pointer;
    }

  .senha {
    border: none;
    padding: 5px;
  }

    .senha:focus-visible {
      box-shadow: 0 0 0 0;
      border: 0 none;
      outline: 0;
    }

  .container-password {
    display: flex;
    justify-content: center;
    align-items: center;
    flex-direction: column;
  }
</style>
