<template>
  <el-card>
    <!--<div class="container">-->
    <div class="title">
      <el-tooltip content="Os perfis que estão aparecendo são cadastrados previamente pelo seu administrador." placement="top">
        <h1>Selecione o Seu Perfil Desejado</h1>
      </el-tooltip>
    </div>
    <div class="sub-container">
      <el-button class="card" v-for="perfil in $store.state.user.claims" :key="perfil" @click="selectPerfil(perfil)">
        <h1>{{ perfil == 'MédicoAD' ? "Atendimento Domiciliar" : perfil }}</h1>
      </el-button>
    </div>
    <!--</div>-->
  </el-card>
</template>

<script>
  import _api from '../../api'
  import Utils from '../../mixins/Utils'
  export default {
    name: 'SelecionarPerfil',
    mixins: [Utils],
    data () {
      return {
  
      }
    },
    async created () {
      if (this.$route.params.origem) {
        if (this.$route.params.origem == 'TrocarAcesso') {
          this.$store.state.user.selectRole = ''
        }
      } else {
        /* await this.temPerfilSelecionado() */
      }
    },
    methods: {
      // async temPerfilSelecionado() {
      //  console.log("entrou");

      //  // Aguarda a mudança do estado user.selectRole no Vuex
      //  await this.$store.watch(
      //    state => state.user.selectRole,
      //    (newVal, oldVal) => {
      //      if (newVal && newVal.trim() !== "") {
      //        console.log("chamou")
      //        // Quando o valor selectRole não é vazio ou indefinido, navegue para a rota "Home"
      //        this.$router.push({
      //          name: 'Home',
      //        });
      //      }
      //    }
      //  );
      // },

      async selectPerfil (perfil) {
        await this.$store.dispatch('selectPerfil', perfil)
        if (perfil === 'Médico' || perfil === 'MédicoAD' || perfil === 'MédicoEspecialista') {
          let { data: dataGetById, status: statusGetById } = await _api.profissionais.getById(this.$store.state.user.dados.id)
          if (statusGetById == 200) {
            var newProfissional = {
              ...dataGetById,
              ultimoPerfilSelecionado: perfil,
              imagem: '',
              userClaims: null
            }

            let { data: dataPut, status: statusPut } = await _api.profissionais.put(newProfissional, 'selecaoPerfil')
            if (statusPut === 204) {
              let ehMedico = this.mxHasAccess('Médico')
              let ehMedicoAD = this.mxHasAccess('MédicoAD')

              this.$router.push({
                name: 'Home'
              })
            } else {
              this.$swal({
                title: "Erro!",
                text: "Não foi possivel atualizar o perfil do profissional!",
                icon: 'error',
              })
            }
          }
        } else {
          // ENTROU NO ELSE É USUARIO
          let { data: dataGetById, status: statusGetById } = await _api.usuarios.getById(this.$store.state.user.dados.id)
          if (statusGetById == 200) {
            var newUsuario = {
              ...dataGetById,
              ultimoPerfilSelecionado: perfil,
              userClaims: null
            }
            delete newUsuario.senha
            let { data: dataPut, status: statusPut } = await _api.usuarios.put(newUsuario, 'selecaoPerfil')
            //console.log('statusPut', statusPut)
            if (statusPut === 200) {
              this.$router.push({
                name: 'Home'
              })
            } else {
              this.$swal({
                title: "Erro!",
                text: 'Não foi possivel atualizar o perfil do profissional!',
                icon: 'error',
              })
            }
          }
        }
      }
    }
  }
</script>

<style scoped>

  .title {
    display:flex;
    justify-content:center
  }

  .sub-container {
    display: flex;
    flex-direction: column;
    gap: 20px;
    justify-content: center
  }

  .card {
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
  }
  
</style>
