<template>
  <el-submenu :index="`${index}`">
    <template slot="title">
      <i :class="`${icone}`"></i>
      <span slot="title">{{titulo}}</span>
    </template>
      <div v-for="(nivel1, i) in menu.atendimento" :key="i+1">
        <el-menu-item v-if="!nivel1.children" :index="`${index}-${+i}`">
          <router-link :to="{ name: nivel1.name }" exact>{{nivel1.name}}</router-link>
        </el-menu-item>

        <el-submenu v-if="nivel1.children" :index="`${index}-${+i}`">
          <span slot="title">{{nivel1.name}}</span>
          <div v-for="(nivel2, j) in nivel1.children" :key="j+i+1">
            <el-menu-item v-if="!nivel2.children" :index="`2-${i}-${j}${+1}`">
              <router-link :to="{ name: nivel2.name }" exact>{{nivel2.name}}</router-link>
            </el-menu-item>
            
          <el-submenu v-if="nivel2.children" :index="`${index}-${i}-${+j}`">
            <span slot="title">{{nivel2.name}}</span>
            <el-menu-item v-for="(nivel3, j) in nivel2.children" :index="`2-${i}-${j}${+1}`" :key="j+i+1">
              <router-link :to="{ name: nivel3.name }" exact>{{nivel3.name}}</router-link>
            </el-menu-item>
          </el-submenu>
          </div>
        </el-submenu>
      </div>
  </el-submenu>
</template>

<script>
  export default {
    name: 'MenuMultiNivel',
    props: {
      titulo: String,
      menu: Array,
      icone: String,
      index: String,
      funcao: Function
    },
    created () {
    //  console.log('menu', this.menu)
     // console.log('icone', this.icone)
  //    console.log('index', this.index)
    }
  }
</script>
