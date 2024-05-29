<template>
<el-row>
  <el-menu :default-active="activeIndex"
           :default-openeds="defaultOpeneds"
           unique-opened
           class="menu-vertical"
           @open="handleOpen"
           @close="handleClose"
           @select="handleSelect"
           :collapse="$store.state.app.isCollapse">

    <!--<el-menu-item index="1" v-if="mxHasAccess('Administrador')">
    <router-link :to="{ name: 'DashBoardGeral' }" exact>
      <i class="fas fa-laptop fa-lg"></i><span class="root-title">DashBoard</span>
    </router-link>
  </el-menu-item>-->

    <router-link :to="{ name: 'Home' }">
      <el-menu-item index="1">
        <router-link :to="{ name: 'Home' }">
          <i class="fas fa-house-user" :style="{ 'margin-left': $store.state.app.isCollapse ? '10px' : '6px' }"></i>
          <span class="root-title" v-if="!$store.state.app.isCollapse">Home</span>
        </router-link>
      </el-menu-item>
    </router-link>

    <router-link :to="{ name: 'Agenda' }" exact>
      <el-menu-item index="2" v-if="mxHasAccess('Recepcionista') && isDemandaEspontanea()">
        <router-link :to="{ name: 'Agenda' }" exact>
          <i class="fas fa-laptop fa-lg"></i><span class="root-title"> Agenda</span>
        </router-link>
      </el-menu-item>
    </router-link>

    <router-link v-if="!$store.state.app.isCollapse" :to="{ name: 'Agendamentos' }">
      <el-menu-item index="3" v-if="mxHasAccess('Médico') && !$store.state.app.isCollapse">
        <router-link v-if="!$store.state.app.isCollapse" :to="{ name: 'Agendamentos' }">
          <i class="fas fa-calendar-alt fa-lg" v-if="!$store.state.app.isCollapse"></i>
          <span class="root-title" v-if="!$store.state.app.isCollapse">Agenda</span>
        </router-link>
      </el-menu-item>
    </router-link>

    <router-link :to="{ name: 'Agenda' }" exact>
      <el-menu-item index="4" v-if="mxHasAccess('Triagem') && isDemandaEspontanea()">
        <router-link :to="{ name: 'Agenda' }" exact>
          <i class="fas fa-laptop fa-lg"></i><span class="root-title"> Agenda</span>
        </router-link>
      </el-menu-item>
    </router-link>

    <router-link :to="{ name: 'MenuHistorico' }" exact>
      <el-menu-item index="5" v-if="mxHasAccess('Médico')">
        <router-link :to="{ name: 'MenuHistorico' }" exact>
          <i class="fas fa-laptop fa-lg" v-if="!$store.state.app.isCollapse"></i>
          <span class="root-title" v-if="!$store.state.app.isCollapse">Histórico</span>
        </router-link>
      </el-menu-item>
    </router-link>

    <!--RELATORIOS-->
    <el-submenu index="15" v-if="mxHasAccess('Administrador', 'GestorEstado', 'AdmMunicipio', 'DownloadCSV', 'Médico')">
      <div slot="title">
        <i class="fas fa-file-excel fa-lg"> <span v-if="!$store.state.app.isCollapse" class="title">Relatórios</span></i>
      </div>
      <div v-for="(nivel1, i) in menu.relatorio" :key="i+1">

        <router-link :to="{ name: nivel1.name }" exact>
          <el-menu-item v-if="!nivel1.children&& !nivel1.meta.hidden && mxHasAccess(...nivel1.meta.auth)" :index="`5-${i+1}`">
            <router-link :to="{ name: nivel1.name }" exact>{{nivel1.meta.title}}</router-link>
          </el-menu-item>
        </router-link>

        <el-submenu v-if="nivel1.children" :index="`5-${i+1}`">
          <span slot="title">{{nivel1.meta.title}}</span>
          <div v-for="(nivel2, j) in nivel1.children" :key="i+j+1">

            <router-link :to="{ name: nivel2.name }" exact>
              <el-menu-item v-if="!nivel2.children && !nivel2.meta.hidden" :index="`5-${i+1}-${j+1}`">
                <router-link :to="{ name: nivel2.name }" exact>{{nivel2.meta.title}}</router-link>
              </el-menu-item>
            </router-link>

            <el-submenu v-if="nivel2.children" :index="`5-${i+1}-${j+1}`">
              <span slot="title">{{nivel2.meta.title}}</span>
              <div v-for="(nivel3, k) in nivel2.children" :key="i+j+k+1">

                <router-link :to="{ name: nivel3.name }" exact>
                  <el-menu-item v-if="!nivel3.meta.hidden" :index="`5-${i+1}-${j+1}-${k+1}`">
                    <router-link :to="{ name: nivel3.name }" exact>{{nivel3.meta.title}}</router-link>
                  </el-menu-item>
                </router-link>

              </div>
            </el-submenu>
          </div>
        </el-submenu>
      </div>
    </el-submenu>


    <el-submenu index="6" v-if="mxHasAccess('Administrador', 'GestorEstado', 'AdmMunicipio', 'DownloadCSV')">
      <template slot="title">
        <i class="fas fa-users fa-lg"><span v-if="!$store.state.app.isCollapse" class="title">Administração</span></i>
      </template>
      <div v-for="(nivel1, i) in menu.administracao" :key="i+1">

        <router-link :to="{ name: nivel1.name }" exact>
          <el-menu-item v-if="!nivel1.children&& !nivel1.meta.hidden && mxHasAccess(...nivel1.meta.auth)" :index="`4-${i+1}`">
            <router-link :to="{ name: nivel1.name }" exact>{{nivel1.meta.title}}</router-link>
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


    <router-link :to="{ name: 'Agenda' }" exact>
      <el-menu-item index="8" v-if="mxHasAccess('Recepcionista')&& !isDemandaEspontanea()">
        <router-link :to="{ name: 'Agenda' }" exact>
          <i class="fas fa-laptop fa-lg"></i><span class="root-title"> Agenda</span>
        </router-link>
      </el-menu-item>
    </router-link>



    <router-link :to="{ name: 'Agendamentos' }" exact>
      <el-menu-item index="10" v-if="mxHasAccess('Recepcionista') && isDemandaEspontanea()">
        <router-link :to="{ name: 'Agendamentos' }" exact>
          <i style="font-size:22px; margin-right: 5px" class="far fa-clock"></i><span class="root-title">Fila de Espera Médica</span>
        </router-link>
      </el-menu-item>
    </router-link>

    <router-link :to="{ name: 'Download' }" exact>
      <el-menu-item index="12" v-if="mxHasAccess('Recepcionista') && isDemandaEspontanea()">
        <router-link :to="{ name: 'Download' }" exact>
          <i style="font-size:22px; margin-right: 5px" class="fas fa-cloud-download-alt"></i><span class="root-title">Download</span>
        </router-link>
      </el-menu-item>
    </router-link>

  </el-menu>
</el-row>
</template>

<script>
  import Utils from '../../mixins/Utils'
  import MenuIndividuo from '../../router/individuo'
  import MenuAdministracao from '../../router/admin'
  import MenuRelatorio from '../../router/relatorio'
  export default {
    name: 'Menu',
    mixins: [Utils],
    data () {
      return {
        activeIndex: '1',
        defaultOpeneds: ['0', '1'],
        windowWidth: 0,
        windowHeight: 0,
        menu: {
          individuo: MenuIndividuo.children,
          administracao: MenuAdministracao.children,
          relatorio: MenuRelatorio.children
        }
      }
    },
    mounted () {
    },
    methods: {
      handleOpen (key, keyPath) {
        // console.log('handle open: key', key)
        // console.log('handle open: keyPath', keyPath)
      },
      handleSelect (key, keyPath) {
        this.$store.dispatch('setKeyPath', keyPath)
        this.activeIndex = key
        this.defaultOpeneds = keyPath
      },
      handleClose (key, keyPath) {
        // console.log(key, keyPath)
      }
    }
  }
</script>
<style scoped>

  li.el-menu-item.is-active {
    background-color: rgb(67, 190, 198);
  }

  li.el-menu-item.is-active a {
    color: #015f6b !important;
  }

  .el-menu {
    border: none !important;
  }

  li.el-menu-item {
    background-color: #54cec6;
    border-radius: 5px;
    padding: 0px 10px 0px 10px !important;
  }

  li.el-menu-item .el-submenu {
      padding: 0px 0px 0px 0px;
  }

  .title {
    margin-left: 4px;
    box-sizing: border-box;
    color: rgb(255, 255, 255);
    cursor: pointer;
    display: inline;
    font-family: 'Roboto', 'arial', sans-serif;
    font-size: 14px;
    font-weight: 400;
    height: auto;
    line-height: 60px;
    list-style-image: none;
    list-style-position: outside;
    list-style-type: none;
    text-align: left;
    text-size-adjust: 100%;
    text-transform: uppercase;
    vertical-align: middle;
    width: auto;
  }

    .title:hover {
      color: #015f6b !important
    }

</style>
