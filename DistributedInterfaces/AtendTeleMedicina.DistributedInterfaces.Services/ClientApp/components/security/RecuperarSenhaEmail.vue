<template>
  <section class="content animated fadeIn">
    <div class="wrapper">
      <form id="formReset" class="login-form" v-if="$auth.ready()">
        <div class="text-center login--logo">
          <img :src="'../../assets/img/' + $store.state.app.empresa.logo2"
               class="img-responsive center-block"
               :title="$store.state.app.empresa.nome" />
        </div>
        <el-form v-show="email" :model="formSenha" status-icon :rules="validacoes" ref="formSenha" label-width="120px" label-position="top" class="form--individuo--senha">
          <el-row :gutter="20">
            <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
              <h4 class="text-center">Codigo Enviado Via EMAIL, Verifique Suas Mensagens:</h4><br />
            </el-col>
            <input v-mask="'#########'" class="form-control" v-model="formEmail.codigo" placeholder="Digite seu código" autofocus />
            <h4 style="margin-top: 10px; margin-left: 3px">Não recebeu o código? <el-button class="re-enviar" @click="onReenviar()" type="primary" size="medium">Re-enviar</el-button></h4>
            <br />
            <el-button style=" margin-top:0px; margin-bottom:0px; margin-left:0px; margin-right:10px" @click="onProximo()" type="primary" size="large">
              Próximo
            </el-button>
          </el-row>
        </el-form>
        <FormInputSenha :show="!email" @emit="onConfirmar" />
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
        if (value === '') {
          callback(new Error('Digite a senha'))
        } else {
          if (this.formSenha.senhaConfirmacao !== '') {
            this.$refs.formSenha.validateField('senhaConfirmacao')
          }
          callback()
        }
      }
      var validaSenhaConfirmacao = (rule, value, callback) => {
        if (value === '') {
          callback(new Error('Digite a confirmação da senha'))
        } else if (value !== this.formSenha.senha) {
          callback(new Error('Senha não confere'))
        } else {
          callback()
        }
      }
      return {
        paramsEmail: {},
        email: true,
        loading: false,
        listando: false,
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
        formEmail: {
          codigo: ''
        },
        formGeral: {

        },
        erro: ''
      }
    },
    async mounted() {
      this.routeParams = this.$route.params
      this.paramsEmail = this.routeParams
    },

    methods: {
      onInicio() {
        this.$router.push({
          name: 'Login',
        })
      },

      async onProximo() {
        this.email = false
      },

      async onReenviar() {
        var cpf = this.paramsEmail.RecuperarEmail.cpf ? this.paramsEmail.RecuperarEmail.cpf.replace(/[.-\s]/g, '') : null
        var metodo = this.paramsEmail.RecuperarEmail.metodo
        let { data, status } = await _api.usuarios.getRecuperarSenha(cpf, metodo)
        let teste = (status === 200) ? data : []

      },
      async onConfirmar(val) {

        if (val.params.senha.match(/^(?=.*[0-9])(?=.*[a-z]).{8,}/)) {

          if (val.params.senha == val.params.senhaConfirmacao) {
            var cpf = this.paramsEmail.RecuperarEmail.cpf ? this.paramsEmail.RecuperarEmail.cpf.replace(/[.-\s]/g, '') : null
            var cpfNovo = cpf.toString()
            var senha = val.params.senha.toString()
            var confirmacao = val.params.senhaConfirmacao.toString()
            var codigoAutenticacao = this.formEmail.codigo.toString()

            this.formGeral.cpf = cpfNovo,
              this.formGeral.senha = senha,
              this.formGeral.confirmacao = confirmacao,
              this.formGeral.codigoAutenticacao = codigoAutenticacao
            if (this.paramsEmail.RecuperarEmail.tipo === 'Usuario') {
              let { data, status } = await _api.usuarios.redefinir(this.formGeral)
              let teste = (status === 200) ? data : []
              let StatusUsuario = status

              if (status === 200) {
                this.$router.push({
                  name: 'Home'
                })
              }
            } else if (this.paramsEmail.RecuperarEmail.tipo === 'Profissional') {
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
</style>
