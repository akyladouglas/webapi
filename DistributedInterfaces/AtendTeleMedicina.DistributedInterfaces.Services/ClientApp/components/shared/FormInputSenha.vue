<template>
  <div v-show="show" :model="formSenha" status-icon ref="formSenha" label-width="120px" label-position="top" class="form--individuo--senha">

    <div class="container-password">

      <h3 class="text-center">Criação De Senha</h3>
      <h4 class="text-center">Senha: </h4>
      <div class="container-senha">

        <input v-if="!showPassword" type="password" ref="formSenha" v-model="formSenha.senha" placeholder="" autofocus class="senha" maxlength="15" />

        <input v-if="showPassword" type="text" ref="formSenha" v-model="formSenha.senha" placeholder="" autofocus class="senha" maxlength="15" />

        <i v-if="showPassword" class="far fa-eye-slash" @click="togglePassword()"></i>
        <i v-if="!showPassword" class="far fa-eye" @click="togglePassword()"></i>
      </div>
      <h4 style="padding-top:10px" class="text-center">Confirmar Senha: </h4>
      <div class="container-senha">

        <input v-if="!showPasswordConfirm" type="password" ref="formSenha" v-model="formSenha.senhaConfirmacao" placeholder="" autofocus class="senha" maxlength="15" />

        <input v-if="showPasswordConfirm" type="text" ref="formSenha" v-model="formSenha.senhaConfirmacao" placeholder="" autofocus class="senha" maxlength="15" />
        <i v-if="showPasswordConfirm" class="far fa-eye-slash" @click="togglePasswordConfirm()"></i>
        <i v-if="!showPasswordConfirm" class="far fa-eye" @click="togglePasswordConfirm()"></i>
      </div>
      <el-button style="margin-top:10px" @click="onConfirmar()" type="primary" size="large">
        Confirmar
      </el-button>
    </div>
  </div>
</template>

<script>
  export default {
    name: 'FormInputSenha',
    props: ['show'],
    data() {

      return {

        showPassword: false,
        showPasswordConfirm: false,
        formSenha: {
          senha: '',
          senhaConfirmacao: '',
          token: ''
        },
      }
    },
    methods: {

      onConfirmar() {
        this.$emit('emit', { params: this.formSenha })
      },

      togglePassword() {
        if (this.showPassword === true) {
          this.showPassword = false
        } else {
          this.showPassword = true
        }
      },
      togglePasswordConfirm() {
        if (this.showPasswordConfirm === true) {
          this.showPasswordConfirm = false
        } else {
          this.showPasswordConfirm = true
        }
      }
    }
  }
</script>

<style>
  .container-senha {
    /* background-color: red;*/
    border: 1px solid #ddd;
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
