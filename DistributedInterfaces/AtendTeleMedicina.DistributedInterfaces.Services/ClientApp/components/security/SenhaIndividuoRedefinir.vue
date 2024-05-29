<template>
  <section class="content animated fadeIn">
    <div class="wrapper">
      <div v-show="opcoes">
        <form id="formData" v-show="listando" class="login-form" v-if="$auth.ready()">
          <div class="text-center login--logo">
            <img :src="'../../assets/img/' + $store.state.app.empresa.logo2"
                 class="img-responsive center-block"
                 :title="$store.state.app.empresa.nome" />
          </div>
          <!--<el-form :model="formSenha" status-icon :rules="validacoes" ref="formSenha" label-width="120px" label-position="top" class="form--individuo--senha">
            <el-row :gutter="20">
              <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
                <h6 class="text-center">Digite sua nova senha</h6><br />
              </el-col>
              <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
                <el-form-item label="Senha" prop="senha">
                  <el-input v-model="formSenha.senha" type="password" autocomplete="off" />
                </el-form-item>
              </el-col>
              <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
                <el-form-item label="Confirmação" prop="senhaConfirmacao">
                  <el-input v-model="formSenha.senhaConfirmacao" type="password" autocomplete="off" />
                </el-form-item>
              </el-col>
            </el-row>

          </el-form>-->
        </form>

        <form id="formReset" v-show="!listando" class="login-form" v-if="$auth.ready()">
          <div class="text-center login--logo">
            <img :src="'../../assets/img/' + $store.state.app.empresa.logo2"
                 class="img-responsive center-block"
                 :title="$store.state.app.empresa.nome" />
          </div>
          <el-form :model="formSenha" status-icon :rules="validacoes" ref="formSenha" label-width="120px" label-position="top" class="form--individuo--senha">
            <el-row :gutter="20">
              <el-col :xs="0" :sm="1" :md="1" :lg="24" :xl="24">
                <h3 class="text-left">Devido Seu Administrador Ter Resetado Sua Senha Defina Uma Senha Nova</h3><br />
              </el-col>
              <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="15">
                <h4 style=" margin-top: -20px; margin-bottom: -10px">Escolha o Método de Recuperação:</h4><br />
              </el-col>
              <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
                <el-button class="fa fa-envelope" @click="onClickEmail('Email')" type="primary" size="medium" style="background-color: #01826C; color: white; border: 0px; margin-bottom: 10px">
                  EMAIL
                </el-button>
              </el-col>
              <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
                <el-button class="fa fa-envelope" @click="onClickSMS('SMS')" type="primary" size="medium" style="background-color: #01826C; color: white; border: 0px; margin-bottom: 10px">
                  SMS
                </el-button>
              </el-col>
            </el-row>
          </el-form>
          <!--<el-button @click="onInicio()" type="danger" size="medium">
            Voltar
          </el-button>-->
        </form>
      </div>

      <div v-show="!opcoes">
        <form id="formReset" class="login-form" v-if="$auth.ready()">
          <div class="text-center login--logo">
            <img :src="'../../assets/img/' + $store.state.app.empresa.logo2"
                 class="img-responsive center-block"
                 :title="$store.state.app.empresa.nome" />
          </div>
          <el-form :model="formSenha" status-icon :rules="validacoes" ref="formSenha" label-width="120px" label-position="top" class="form--individuo--senha">
            <el-row :gutter="20">
              <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
                <h4 class="text-center">Insira os dados para recuperação de senha:</h4><br />
              </el-col>
              <input v-mask="'###.###.###-##'" class="form-control" v-model="formData.cpf" placeholder="Digite seu CPF" autofocus />
            </el-row>
            <el-row :gutter="20">
              <el-button style=" margin-top:10px; margin-bottom:10px; margin-left:50px; margin-right:10px" @click="onConfirmar(formData)" v-show="!opcoes" type="primary" size="medium">
                Confirmar
              </el-button>
              <el-button style="margin-top: 10px; margin-bottom:10px; margin-left: 10px; margin-right:10px " @click="onClickBack()" v-show="!opcoes" type="danger" size="medium">
                Voltar
              </el-button>
            </el-row>
          </el-form>
        </form>
      </div>
    </div>
  </section>
</template>

<script>
  import { Notification } from 'element-ui'
  import _api from '../../api'
  import b64 from 'base-64'
  import { mask } from 'vue-the-mask'

  export default {
    name: 'login',
    directives: { mask },
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
        opcoes: true,
        loading: false,
        listando: false,
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
        formData: {
          metodo: '',
          cpf: '',
          tipo: '',
        },
        erro: ''
      }
    },
    methods: {
      onInicio() {
        this.$router.push({
          name: 'Login',
        })
      },
      async onClickSMS(val) {
        let sms = val
        this.formData.metodo = sms
        this.opcoes = false

      },
      async onClickEmail(val) {
        let email = val
        this.formData.metodo = email
        this.opcoes = false

      },
      async onConfirmar(val) {

        var cpf = val.cpf ? val.cpf.replace(/[.-\s]/g, '') : null

        //PROCURANDO NA TABELA USUARIO
        let { data, status } = await _api.usuarios.getRecuperarSenha(cpf, val.metodo)
        let DataUsuario = (status === 200) ? data : []
        let StatusUsuario = status

        if (StatusUsuario === 200) {
          if (val.cpf) {
            if (val.metodo === "SMS") {
              val.tipo = 'Usuario'
              this.$router.push({
                name: 'RecuperarSenhaSMS',
                params: { RecuperarSMS: val, codigo: DataUsuario.codigoAutenticacao }
              })
            } else if (val.metodo === "Email") {
              val.tipo = 'Usuario'
              this.$router.push({
                name: 'RecuperarSenhaEmail',
                params: { RecuperarEmail: val, codigo: DataUsuario.codigoAutenticacao }
              })
            } else {
              this.$swal({
                title: "Erro!",
                text: 'Escolha um metodo válido!',
                icon: 'error',
              })
            }
          } else {
            this.$swal({
              title: "Erro!",
              text: 'Escolha um metodo válido!',
              icon: 'error',
            })
          }
        } else if (StatusUsuario === 400) {

          //PROFURANDO NA TABELA PROFISSIONAL
          let { data, status } = await _api.profissionais.getRecuperarSenha(cpf, val.metodo)
          let DataProfissionais = (status === 200) ? data : []

          let StatusProfissionais = status
          if (StatusProfissionais === 200) {
            if (val.cpf) {
              if (val.metodo === "SMS") {
                val.tipo = 'Profissional'
                this.$router.push({
                  name: 'RecuperarSenhaSMS',
                  params: { RecuperarSMS: val, codigo: DataProfissionais.codigoAutenticacao }
                })
              } else if (val.metodo === "Email") {
                val.tipo = 'Profissional'
                this.$router.push({

                  name: 'RecuperarSenhaEmail',
                  params: { RecuperarEmail: val, codigo: DataProfissionais.codigoAutenticacao }
                })
              } else {
                this.$swal({
                  title: "Erro!",
                  text: 'Escolha um metodo válido!',
                  icon: 'error',
                })
              }
            } else {
              this.$swal({
                title: "Erro!",
                text: 'Escolha um metodo válido!',
                icon: 'error',
              })
            }
          } else {

          }
        } else {
          this.$swal({
            title: "Erro!",
            text: 'Senha inválida! A senha deve conter 8 caracteres, contendo pelo menos 1 número!',
            icon: 'error',
          })
        }
      },
      async onClickBack() {
        this.opcoes = true
        this.email = false
        this.cpf = false
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
</style>
