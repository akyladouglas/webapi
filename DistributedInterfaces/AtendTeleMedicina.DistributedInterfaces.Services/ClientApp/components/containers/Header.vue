<template>
  <el-row>
    <!--<el-col :span="1">
        <el-button v-if="!mxHasAccess('Médico') && $store.state.app.isCollapse" type="info" circle icon="el-icon-arrow-right" v-on:click="setIsCollapse(false)" />
        <el-button v-if="!mxHasAccess('Médico') && !$store.state.app.isCollapse" type="info" circle icon="el-icon-arrow-left" v-on:click="setIsCollapse(true)" />
    </el-col>-->
    <div class="aside--logo">

      <div style="width: 205px">
        <router-link :to="{ name: 'Home' }">
          <img :src="'../../assets/img/' + $store.state.app.empresa.logoNove"
               :title="$store.state.app.empresa.nome"
               id="image" />
        </router-link>
      </div>

        <el-menu v-show="$store.state.user.selectRole != ''" class="menus-container"
                   :default-active="activeIndex" :default-openeds="defaultOpeneds"
                   @open="handleOpen" @close="handleClose"
                   @select="handleSelect" mode="horizontal">

            <router-link :to="{ name: 'Home' }">
              <el-menu-item index="1">
                <router-link :to="{ name: 'Home' }">
                  <i class="fas fa-house-user"></i><span class="root-title"> Home</span>
                </router-link>
              </el-menu-item>
            </router-link>

            <router-link :to="{ name: 'AgendaCalendario' }">
              <el-menu-item index="1" v-if="!mxHasAny('Triagem')">
                <router-link :to="{ name: 'AgendaCalendario' }">
                  <i class="fas fa-calendar-alt fa-lg icone"></i><span class="root-title"> Agenda</span>
                </router-link>
              </el-menu-item>
            </router-link>

            <router-link :to="{ name: 'Agendamentos' }">
              <el-menu-item index="2" v-if="mxHasAccess('Médico') || mxHasAccess('MédicoAD') || mxHasAccess('MédicoEspecialista')">
                <router-link :to="{ name: 'Agendamentos' }">
                  <i class="fa fa-notes-medical icone"></i><span class="root-title"> Atender</span>
                </router-link>
              </el-menu-item>
            </router-link>

            <router-link :to="{ name: 'Interconsulta' }">
              <el-menu-item index="2" v-if="mxHasAccess('Médico') || mxHasAccess('MédicoAD') || mxHasAccess('MédicoEspecialista')">
                <router-link :to="{ name: 'Interconsulta' }">
                  <i class="fa fa-notes-medical icone"></i><span class="root-title"> Interconsulta</span>
                </router-link>
              </el-menu-item>
            </router-link>

            <router-link :to="{ name: 'MenuHistorico' }">
              <el-menu-item index="3" v-if="mxHasAccess('Médico') || mxHasAccess('MédicoAD')">
                <router-link :to="{ name: 'MenuHistorico' }">
                  <i class="fas fa-laptop fa-lg"></i> <span class="root-title">Histórico</span>
                </router-link>
              </el-menu-item>
            </router-link>

            <el-submenu index="4" v-if="mxHasAccess('Recepcionista') || mxHasAccess('Triagem')">
              <template slot="title">
                <i class="fas fa-users"></i>
                <span slot="title">Pacientes</span>
              </template>
              <div v-for="(nivel1, i) in menu.paciente" :key="i+1">
                <router-link :to="{ name: nivel1.name }">
                  <el-menu-item v-if="!nivel1.children && !nivel1.meta.hidden && mxHasAccess(...nivel1.meta.auth)" :index="`10-${i+1}`">
                    <router-link :to="{ name: nivel1.name, params: { agendaMenu: true } }">{{nivel1.meta.title}}</router-link>
                  </el-menu-item>
                </router-link>
                <el-submenu v-if="nivel1.children" :index="`4-${i+1}`">
                  <span slot="title">{{nivel1.meta.title}}</span>
                  <div v-for="(nivel2, j) in nivel1.children" :key="i+j+1">
                    <router-link :to="{ name: nivel2.name }" exact>
                      <el-menu-item v-if="!nivel2.children && !nivel2.meta.hidden" :index="`4-${i+1}-${j+1}`">
                        <router-link :to="{ name: nivel2.name }" exact>{{nivel2.meta.title}}</router-link>
                      </el-menu-item>
                    </router-link>

                    <el-submenu v-if="nivel2.children" :index="`4-${i+1}-${j+1}`">
                      <span slot="title">{{nivel2.meta.title}}</span>
                      <div v-for="(nivel3, k) in nivel2.children" :key="i+j+k+1">
                        <router-link :to="{ name: nivel3.name }" exact>
                          <el-menu-item v-if="!nivel3.meta.hidden" :index="`4-${i+1}-${j+1}-${k+1}`">
                            <router-link :to="{ name: nivel3.name }" exact>{{nivel3.meta.title}}</router-link>
                          </el-menu-item>
                        </router-link>
                      </div>
                    </el-submenu>
                  </div>
                </el-submenu>
              </div>
            </el-submenu>

            <router-link :to="{ name: 'Agendamentos', params: { filaEspera: true } }">
              <el-menu-item index="5" v-if="mxHasAccess('Recepcionista')">
                <i style="font-size:22px; margin-right: 5px" class="far fa-clock"></i><span class="root-title">Fila de Espera Médica</span>
              </el-menu-item>
            </router-link>

            <el-submenu index="10" v-if="mxHasAccess('Administrador')">
              <template slot="title">
                <i class="fas fa-file-excel fa-lg"></i>
                <span slot="title">Relatórios</span>
              </template>
              <div v-for="(nivel1, i) in menu.relatorio" :key="i+1">
                <router-link :to="{ name: nivel1.name }">
                  <el-menu-item v-if="!nivel1.children&& !nivel1.meta.hidden && mxHasAccess(...nivel1.meta.auth)" :index="`10-${i+1}`">
                    <router-link :to="{ name: nivel1.name }">{{nivel1.meta.title}}</router-link>
                  </el-menu-item>
                </router-link>
                <el-submenu v-if="nivel1.children" :index="`10-${i+1}`">
                  <span slot="title">{{nivel1.meta.title}}</span>
                  <div v-for="(nivel2, j) in nivel1.children" :key="i+j+1">
                    <router-link :to="{ name: nivel2.name }">
                      <el-menu-item v-if="!nivel2.children && !nivel2.meta.hidden" :index="`10-${i+1}-${j+1}`">
                        <router-link :to="{ name: nivel2.name }" exact>{{nivel2.meta.title}}</router-link>
                      </el-menu-item>
                    </router-link>

                    <el-submenu v-if="nivel2.children" :index="`10-${i+1}-${j+1}`">
                      <span slot="title">{{nivel2.meta.title}}</span>
                      <div v-for="(nivel3, k) in nivel2.children" :key="i+j+k+1">
                        <router-link :to="{ name: nivel3.name }">
                          <el-menu-item v-if="!nivel3.meta.hidden" :index="`10-${i+1}-${j+1}-${k+1}`">
                            <router-link :to="{ name: nivel3.name }" exact>{{nivel3.meta.title}}</router-link>
                          </el-menu-item>
                        </router-link>
                      </div>
                    </el-submenu>
                  </div>
                </el-submenu>
              </div>
            </el-submenu>

            <!--ADMINISTRAÇÃO-->
            <el-submenu index="20" v-if="mxHasAccess('Administrador')">
              <template slot="title">
                <i class="fas fa-file-excel fa-lg"></i>
                <span slot="title">Administração</span>
              </template>
              <div v-for="(nivel1, i) in menu.administracao" :key="i+1">
                <router-link :to="{ name: nivel1.name }">
                  <el-menu-item v-if="!nivel1.children&& !nivel1.meta.hidden && mxHasAccess(...nivel1.meta.auth)" :index="`20-${i+1}`">
                    <router-link :to="{ name: nivel1.name }" exact>{{nivel1.meta.title}}</router-link>
                  </el-menu-item>
                </router-link>
                <el-submenu v-if="nivel1.children" :index="`20-${i+1}`">
                  <span slot="title">{{nivel1.meta.title}}</span>
                  <div v-for="(nivel2, j) in nivel1.children" :key="i+j+1">
                    <router-link :to="{ name: nivel2.name }">
                      <el-menu-item v-if="!nivel2.children && !nivel2.meta.hidden" :index="`20-${i+1}-${j+1}`">
                        <router-link :to="{ name: nivel2.name }" exact>{{nivel2.meta.title}}</router-link>
                      </el-menu-item>
                    </router-link>

                    <el-submenu v-if="nivel2.children" :index="`20-${i+1}-${j+1}`">
                      <span slot="title">{{nivel2.meta.title}}</span>
                      <div v-for="(nivel3, k) in nivel2.children" :key="i+j+k+1">
                        <router-link :to="{ name: nivel3.name }">
                          <el-menu-item v-if="!nivel3.meta.hidden" :index="`20-${i+1}-${j+1}-${k+1}`">
                            <router-link :to="{ name: nivel3.name }" exact>{{nivel3.meta.title}}</router-link>
                          </el-menu-item>
                        </router-link>
                      </div>
                    </el-submenu>
                  </div>
                </el-submenu>
              </div>
            </el-submenu>
            <!--FIM MENU ADMIN-->
          </el-menu>
        <MenuUsuario />
      </div>

        

  </el-row>
</template>

<script>
  import Utils from '../../mixins/Utils'
  import MenuAdministracao from '../../router/admin'
  import MenuRelatorio from '../../router/relatorio'
  import MenuPaciente from '../../router/paciente'
  import MenuUsuario from './MenuUsuario'

  export default {
    name: 'Header',
    mixins: [Utils],
    components: { MenuUsuario },
    data () {
      return {
        screenWidth: window.innerWidth,
        filtroParams: {},
        tipoDaConsulta: null,
        profissionalId: null,
        defaultOpeneds: ['0', '1'],
        activeIndex: '1',
        activeIndex2: '1',
        menu: {
          administracao: MenuAdministracao.children,
          relatorio: MenuRelatorio.children,
          paciente: MenuPaciente.children

        }
      }
    },
    computed: {
      dynamicWidth() {
        return this.screenWidth;
      }
    },

    beforeMount() {
      this.updateScreenWidth()
    },

    async mounted() {
      window.addEventListener('resize', this.updateScreenWidth);
      this.updateScreenWidth();
    },

    beforeDestroy() {
      window.removeEventListener('resize', this.updateScreenWidth);
    },

   
    methods: {
      updateScreenWidth() {
        this.screenWidth = window.innerWidth;
      },

      returnUser () {
        if (this.mxHasAccess('Médico')) {
          return 'Médico'
        } else if (this.mxHasAccess('Admin')) {
          return 'Admin'
        } else if (this.mxHasAccess('Triagem')) {
          return 'Triagem'
        } else if (this.mxHasAccess('Recepcionista')) {
          return 'Recepcionista'
        }
      },
      handleOpen (key, keyPath) {
  
      },
      handleClose (key, keyPath) {
  
      },
      handleSelect (key, keyPath) {
        this.$store.dispatch('setKeyPath', keyPath)
        this.activeIndex = key
        this.defaultOpeneds = keyPath
      },
  
      setIsCollapse (payLoad) {
        this.$store.dispatch('setIsCollapse', payLoad)
      },
      onPerfil () {
        this.$router.push({ name: 'AdminPerfil' })
      },
      logout () {
        this.$store.dispatch('signOut')
        this.$router.push({ path: '/login' })
      }

      // async fonte(e) {
      //  console.log("e", e)
      //  let elemento = jQuery(".acessibilidade");
      //  console.log("elemento", elemento)
      //  let fonte = elemento.css('font-size');
      //  console.log("fonte", fonte)

      //  if (e == 'a') {
      //    elemento.css("fontSize", parseInt(fonte) + 1);
      //  } else if ('d') {
      //    elemento.css("fontSize", parseInt(fonte) - 1);
      //  }
      // },
    }
  }
</script>

<style scoped>
  .aside--logo {
    display: flex !important;
    align-items: center;
    width: 100%;
    height: 50%;
    flex-direction: row;
    justify-content: space-between;
    padding: 0px 20px 0px 30px;
    background: #43bec6;
  }
  @media screen and (max-width: 450px) {
    .aside--logo {
      padding: 0px 10px 0 10px;
    }
  }


  #image {
    width: max-content;
    min-width: 205px;
    height: 80px;
  }

  @media screen and (max-width: 450px) {
    #image {
        width: 150px;
        min-width: 150px;
        height: 50px;
    }
  }

  .menus-container {
    display: flex;
    align-content: center;
    align-items: center;
    justify-content: center;
    width: auto;
  }

  /*a:visited {
    color: #000000;
  }*/
  /*a:link {
    color: #000000;
  }*/
  /* .el-menu--horizontal ul > * {
    color: #fff !important
  }*/
  /*  div.el-menu--horizontal > ul > div > a {
    color: #fff !important
  }*/
  .el-menu--horizontal .el-menu .el-menu-item {
    color: #ff0000
  }
/*
  .el-menu--horizontal .el-submenu.is-active .el-submenu__title {
    border: none;
    color: #303133;
  }*/
  .el-menu--horizontal .el-submenu .el-submenu__title {
    height: 60px;
    line-height: 60px;
    border: none !important;
    color: #fff;
  }
  .el-menu.el-menu--horizontal {
    border: none;
    display: flex;
  }

  /*container retangulo quando nao selecionado*/
  .el-menu--horizontal .el-menu-item:not(.is-disabled):hover {
    background-color: #43bec6;
    color:red
  }
  .el-menu--horizontal .el-menu-item.is-active {
    border:none;
    background-color: #43bec6;
  }

  
  .router-link-exact-active li > * {
    color: #015f6b !important
  }

  ul.el-menu a.router-link-exact-active li span {
    color: #015f6b
  }



  ul.el-menu--horizontal a li > * {
    transition: color 0.2s ease;
  }

  ul.el-menu--horizontal a li:hover > * {
    color: #015f6b
  }


</style>
