<template>
  <div style="display: flex; align-items: center; justify-content: center;">

    <el-dropdown>
      <div class="container_perfil">
        <div style="flex-direction: column; display: flex; align-items: center; justify-content: center; cursor: pointer">
          <div id="container_imagem" v-if="mxHasAccess('Médico')" style="cursor:pointer; color:#FFF">
            <!--{{$auth.user()}}-->
            <img v-if="mxHasAccess('Médico')" id="imagemPerfil" :src="`../../${$auth.user().imagem ? $auth.user().imagem : fotoFallback}.jpg?${new Date()}`"
                 :title="$store.state.user.dados.login" />
            <!--<img v-if="mxHasAccess('Médico')" id="imagemPerfil" :src="`../../${fotoFallback}.jpg`"
     :title="$store.state.user.dados.login" />

  -->

          </div>
          <div id="container_nome" v-if="mxHasAccess('Médico')">
            <div style="color: #FFFFFF" id="nomePerfil">
              {{returnUser()}}
            </div>
          </div>

        </div>

          <div v-if="!mxHasAccess('Médico')">
            <div style="flex-direction: column; display: flex; align-items: center; justify-content: center; cursor: pointer">
              <div>
                <img id="imagemPerfilFallback" :src="`../../${fotoFallback}.jpg`"
                     :title="$store.state.user.dados.login" />
              </div>
              <div style="color: #FFFFFF">
                {{returnUser()}}
              </div>
            </div>
          </div>

          <el-dropdown-menu slot="dropdown">
            <el-dropdown-item>
              <div style="width: 100px" v-if="mxHasAccess(!'Recepcionista')">
                <router-link :to="{ name: 'Perfil' }" exact>Perfil</router-link>
              </div>
            </el-dropdown-item>
            <el-dropdown-item v-if="mxHasAccess('Médico') || mxHasAccess('MédicoAD')">
              <div @click="dialogVisible = true" style="width: 100px">
                <span>Carregar foto</span>
              </div>
            </el-dropdown-item>
            <el-dropdown-item v-if="mxHasAccess('Médico') || mxHasAccess('MédicoAD')">
              <div @click="dialogVisibleRemovePhoto = true" style="width: 100px">
                <span>Remover foto</span>
              </div>
            </el-dropdown-item>

            <el-dropdown-item @click="changeAccess" v-show="$store.state.user.selectRole != ''">
              <div @click="changeAccess" style="width: 100px">
                <span>Trocar Acesso</span>
              </div>
            </el-dropdown-item>

            <el-dropdown-item @click="logout">
              <div @click="logout" style="width: 100px">
                <span>Sair</span>
              </div>
            </el-dropdown-item>
          </el-dropdown-menu>
      </div>
    </el-dropdown>

    <el-dialog title="Carregar foto"
               :visible.sync="dialogVisible"
               width="30%"
               class="modalFoto">
      <el-upload class="avatar-uploader"
                 action=""
                 :show-file-list="false"
                 :on-success="encodeImageFileAsURL"
                 :before-upload="beforeAvatarUpload">
        <img v-if="imageUrl" :src="imageUrl" class="avatar">
        <i v-else class="el-icon-plus avatar-uploader-icon"></i>
      </el-upload>
      <el-button :disabled="disableButton" class="buttonSave" size="small" flat icon="fas fa-save" type="success" @click="submit"> Salvar</el-button>
    </el-dialog>

    <el-dialog title="Remover foto"
               :visible.sync="dialogVisibleRemovePhoto"
               width="30%"
               class="modalFoto">
      <span>Tem certeza que deseja remover a foto de perfil?</span>

      <el-button :disabled="disableButtonRemovePhoto" class="buttonSave" size="small" flat type="success" @click="submitRemovePhoto"> Sim</el-button>
      <el-button :disabled="disableButtonRemovePhoto" class="buttonSave" size="small" flat type="default" @click="dialogVisibleRemovePhoto = false"> Não</el-button>

    </el-dialog>

  </div>
</template>

<script>
  import Utils from '../../mixins/Utils'
  import _api from '../../api'
  import { forEach } from 'core-js/fn/dict'
  import moment from 'moment'
  moment.locale('pt-br')
  export default {
    name: 'MenuUsuario',
    mixins: [Utils],
    data() {
      return {
        disableButtonRemovePhoto: false,
        dialogVisibleRemovePhoto: false,
        disableButton: false,
        profissionalPost: null,
        profissionalGet: null,
        imageUrl: '',
        dialogVisible: false,
        activeIndex: '1',
        defaultOpeneds: ['0', '1'],
        fotoFallback: 'dist/foto-fallback',
        encodedImage: null
      }
    },
    async mounted() {
      setTimeout(() => {
        //console.log("auth", this.$auth.user())
        this.$auth.user().roles.forEach(role => {
          if (role.includes("Médico")) {
            this.profissionalId = this.$auth.user().id
            this.getProfissional(this.profissionalId)
            //console.log("this.$auth", this.$auth.user())
            return
          }
        })
      }, 2000)
     
      

       
      
    },

    methods: {
      changeAccess() {
        this.$router.push({
          name: 'SelecionarPerfil',
          params: { origem: 'TrocarAcesso' }
        })
      },


      returnUser() {
        if (this.mxHasAccess("Recepcionista") && this.mxHasAccess("Admin")) return "Recepcão / Admin"
        else if (this.mxHasAccess("Médico")) return "Médico"
        else if (this.mxHasAccess("MédicoAD")) return "MédicoAD"
        else if (this.mxHasAccess("Administrador")) return "Admin"
        else if (this.mxHasAccess("Triagem")) return "Triagem"
        else if (this.mxHasAccess("Recepcionista")) return "Recepcionista"
      },
      async getProfissional(profissionalId) {
        _api.profissionais.getById(profissionalId).then(res => {
          if (res.status == 200) {
            this.profissionalGet = res.data

          }
        })

      },
      handleOpen(key, keyPath) {

      },
      handleSelect(key, keyPath) {
        this.activeIndex = key
        this.defaultOpeneds = keyPath
      },
      handleClose(key, keyPath) {
        // console.log(key, keyPath)
      },
      async logout() {

        var perfil = this.$store.state.user.selectRole

        if (perfil === "Médico" || perfil === "MédicoAD" || perfil === "MédicoEspecialista") {
          let { data: dataGetById, status: statusGetById } = await _api.profissionais.getById(this.$store.state.user.dados.id)
          if (statusGetById == 200) {

            // LIMPANDO O ULTIMO PERFIL SELECIONADO PARA QUANDO O USUARIO FIZER O LOGIN NOVAMENTE NÃO IR PARA O STORE
            var newProfissional = {
              ...dataGetById,
              ultimoPerfilSelecionado: null,
              imagem: "",
              userClaims: null,
            }

            let { data: dataPut, status: statusPut } = await _api.profissionais.put(newProfissional, 'selecaoPerfil')
            if (statusPut === 204) {
              this.$store.dispatch('signOut')
              this.$router.push({ path: '/login' })
            } else {
              this.$swal({
                title: "Erro!",
                text: "Não foi possivel atualizar o perfil do profissional!",
                icon: 'error',
              })
            }
          }
        } else {
          //SE FOR USUARIO

          let { data: dataGetById, status: statusGetById } = await _api.usuarios.getById(this.$store.state.user.dados.id)
          if (statusGetById == 200) {

            // LIMPANDO O ULTIMO PERFIL SELECIONADO PARA QUANDO O USUARIO FIZER O LOGIN NOVAMENTE NÃO IR PARA O STORE
            var newUsuario = {
              ...dataGetById,
              ultimoPerfilSelecionado: null,
              userClaims: null,
            }
            delete newUsuario.senha
            let { data: dataPut, status: statusPut } = await _api.usuarios.put(newUsuario, 'selecaoPerfil')
            if (statusPut === 200) {
              this.$store.dispatch('signOut')
              this.$router.push({ path: '/login' })
            } else {
              this.$swal({
                title: "Erro!",
                text: "Não foi possivel atualizar o perfil do profissional!",
                icon: 'error',
              })
            }
          }
        }
      },
      encodeImageFileAsURL(response, file) {
        this.imageUrl = URL.createObjectURL(file.raw);


        let tipoExame = this.tipoExame
        var arq = file.raw;

        var reader = new FileReader();

        reader.onloadend = () => {
          let Url = reader.result
          let formato = ""


          if (arq.type === "image/jpeg") {
            Url = reader.result.replace("data:image/jpeg;base64,", "")
            formato = ".jpeg";
          }
          if (arq.type === "image/jpg") {
            Url = reader.result.replace("data:image/jpg;base64,", "")
            formato = ".jpg";
          }
          if (arq.type === "image/png") {
            Url = reader.result.replace("data:image/png;base64,", "")
            formato = ".png";
          }

          this.profissionalPost = { ...this.profissionalGet }

          this.profissionalPost.imagem = Url
        };
        reader.readAsDataURL(arq);

      },

      beforeAvatarUpload(file) {
        const isJPG = file.type === 'image/jpeg';
        const isLt2M = file.size / 1024 / 1024 < 2;

        if (!isJPG) {
          this.$swal({
            title: "Erro!",
            text: "O formato da imagem precisar ser em JPG!",
            icon: 'error',
          })
        }
        if (!isLt2M) {
          this.$swal({
            title: "Erro!",
            text: 'O tamanho da imagem não pode exceder mais do que 2MB!',
            icon: 'error',
          })
        }
        return isJPG && isLt2M;
      },
      submit() {
        this.disableButton = true
        _api.profissionais.put(this.profissionalPost).then(res => {
          if (res.status == 204) {
            this.dialogVisible = false
            this.disableButton = false
            this.imageUrl = ''

            _api.profissionais.getById(this.profissionalId).then(res => {

              if (res.status == 200) {
                this.profissionalGet = res.data
                var imagem = window.document.querySelector('#imagemPerfil')
                var container = window.document.querySelector('#container_imagem')

                var nomePerfil = window.document.querySelector('#nomePerfil')
                var containerNome = window.document.querySelector('#container_nome')
                

                container.removeChild(imagem)
                containerNome.removeChild(nomePerfil)
                const nome = nomePerfil.cloneNode(true);

                

                const img = imagem.cloneNode(true);
                img.src = `../../${this.profissionalGet.imagem}.jpg`
                
                container.appendChild(img);
                containerNome.appendChild(nome);
                window.location.reload();

                
              }
            })
          }
        })

      },
      submitRemovePhoto() {
        this.profissionalPost = null
        this.profissionalPost = { ...this.profissionalGet }
        this.profissionalPost.imagem = ''
        _api.profissionais.updateFoto(this.profissionalPost).then(res => {
          if (res.status == 200) {
            this.dialogVisibleRemovePhoto = false
            this.disableButtonRemovePhoto = false
            this.imageUrl = ''

            var imagem = window.document.querySelector('#imagemPerfil')
            var container = window.document.querySelector('#container_imagem')

            container.removeChild(imagem)

            const img = imagem.cloneNode(true);

            if (res.data.imagem != null) {
              this.fotoFallback = res.data.imagem
            } 

            img.src = `../../${this.fotoFallback}.jpg`
            container.appendChild(img);
            this.profissionalGet.imagem = ''
          }
        })

        //_api.profissionais.getById(this.profissionalId).then(res => {

        //  if (res.status == 200) {
        //    this.profissionalGet = res.data

        //  }
        //})



      }
    }
  }
</script>

<style>
  .container_perfil {
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
  }

  #imagemPerfil, #imagemPerfilFallback {
    width: 40px;
    height: 40px;
    transition: all 0.3s ease-in;
    margin: 5px;
    border-radius: 50%;
  }

  .avatar-uploader .el-upload {
    border: 1px dashed #d9d9d9;
    border-radius: 6px;
    cursor: pointer;
    position: relative;
    overflow: hidden;
  }

    .avatar-uploader .el-upload:hover {
      border-color: #409EFF;
    }

  .avatar-uploader-icon {
    font-size: 28px;
    color: #8c939d;
    width: 178px;
    height: 178px;
    line-height: 178px;
    text-align: center;
  }

  .avatar {
    width: 178px;
    height: 178px;
    display: block;
  }

  .buttonSave {
    margin: 10px;
  }
</style>
