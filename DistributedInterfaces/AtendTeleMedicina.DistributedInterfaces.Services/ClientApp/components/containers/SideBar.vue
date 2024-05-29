<template>
  <el-aside class="horizontal-collapse-transition-menu"
            :style="{ 'min-height': `${dynamicHeight}px` }"
            :class="[$store.state.app.isCollapse ? 'container__collapse' : 'container__unCollapsed']">

    <div :class="[$store.state.app.isCollapse ? 'container__collapse__logo' : 'container__unCollapsed__logo']">

      <img v-show="$store.state.app.isCollapse == false" :src="'../../assets/img/' + $store.state.app.empresa.logoNove"
           class="unCollapse-image"
           :title="$store.state.app.empresa.nome" />

      <img v-show="$store.state.app.isCollapse == true" :src="'../../assets/img/' + $store.state.app.empresa.logoNoveCollapsed"
           class="collapse-image"
           :title="$store.state.app.empresa.nome" />

      <div class="text-center">
        <span class="aside--logo--empresa">{{$store.state.app.empresa.nome}}</span>
        <span class="aside--logo--titulo">{{$store.state.app.empresa.titulo}}</span>
      </div>
    </div>

    <div class="aside--divider"></div>
    <MenuUsuario />

    <!--<div style="display: flex; flex-direction: row; justify-content: center; align-items: center">
      <el-button size="small" @click="setIsCollapse()">Click</el-button>
    </div>-->


    <div class="aside--divider"></div>
    <!--<div style="display: flex; flex-direction: row; justify-content: center; align-items: center"><span>{{$store.state.app.isCollapse}}</span></div>-->
    <VuePerfectScrollbar :class="[!$store.state.app.isCollapse ? 'scroll-area' : 'no-ps']"
                         :settings="scrollSettings"
                         :key="$store.state.app.isCollapse"
                         @ps-scroll-y="scrollHandle">

      <Menu />

    </VuePerfectScrollbar>

    <div class="aside--divider"></div>
    <!--<el-row>
      <countdown :tokenExpires="tokenExpires" />
    </el-row>-->
  </el-aside>
</template>

<script>
  import MenuUsuario from './MenuUsuario'
  import Menu from './Menu'
  import Countdown from '../UIComponents/Countdown.vue'
  import VuePerfectScrollbar from 'vue-perfect-scrollbar'
  import moment from 'moment'
  import ScreenHeight from '../../mixins/ScreenHeight'
  moment.locale('pt-br')

  export default {
    name: 'SideBar',
    mixins: [ScreenHeight],
    components: { MenuUsuario, Menu, Countdown, VuePerfectScrollbar },
    data() {
      return {
        scrollSettings: {
          maxScrollbarLength: 60,
        },
        tokenExpires: null,
      }
    },

    created() {
      let expiresIn = parseInt(localStorage.getItem('expires_in'))
      this.tokenExpires = this.moment(expiresIn).format('YYYY-MM-DD HH:mm:ss')
    },

    methods: {

      setIsCollapse() {
        this.$store.commit('SET_IS_COLLAPSE', !this.$store.state.app.isCollapse);
        console.log('this.$store.state.app.isCollapse', this.$store.state.app.isCollapse)
      },

      scrollHandle(e) {
        // console.log(e)
      },
    }
  }

</script>

<style scoped>

  /* UnCollapsed */

  .container__unCollapsed {
    display: flex !important;
    flex-direction: column;
    background: #43bec6;
    width: 300px !important;
    position: absolute;
    z-index: 20;
  }

  @media screen and (max-width: 590px) {
    .container__unCollapsed {
      width: 200px !important;
      position: absolute;
    }
  }

  .container__unCollapsed__logo {
    display: flex !important;
    align-items: center;
    width: 100%;
    height: 80px;
    flex-direction: row;
    justify-content: space-between;
    padding: 0px 20px 0px 20px;
    background: #43bec6;
  }

  @media screen and (max-width: 590px) {
    .container__unCollapsed__logo {
      padding: 0px 10px 0px 10px;
      height: 80px;
    }
  }

  .unCollapse-image {
    display: flex;
    justify-content: center;

    width: 100%;
    min-width: 205px;
  }

  @media screen and (max-width: 590px) {
    .unCollapse-image {
      min-width: 80px;
    }
  }

  

  


  /* Collapse (SIDE BAR CLOSE) */
  .container__collapse {
    display: flex !important;
    flex-direction: column;
    background: #43bec6;
    width: 150px !important;
  }
  @media screen and (max-width: 590px) {
    .container__collapse {
      width: 80px !important;
    }
  }


  .container__collapse__logo {
    display: flex !important;
    align-items: center;
    width: 100%;
    height: 80px;
    flex-direction: row;
    justify-content: center;
    padding: 5px 20px 0px 20px;
    background: #43bec6;
  }

  @media screen and (max-width: 590px) {
    .container__collapse__logo {
      padding: 5px 0px 0px 0px;
      height: 80px;
    }
  }

  .collapse-image {
    display: flex;
    justify-content: center;
    width: 50%;
    min-width: 90px;
    height: 70px;
  }

  @media screen and (max-width: 590px) {
    .collapse-image {
      width: 50px;
      min-width: 50%;
      height: 50px;
    }
  }



  .aside__colapse .scroll-area {
    width: 50px;
    overflow-x: hidden;
    overflow-y: hidden;
  }

  .aside__colapse .menu-vertical {
    margin-left: -10px;
  }

  .aside__colapse .ps__scrollbar-y-rail {
    visibility: hidden;
  }


  .scroll-area {
    position: relative;
    margin-top: 0px;
    margin-bottom: auto;
    width: 100%;
    min-height: auto;
    overflow-x: hidden;
  }
</style>
