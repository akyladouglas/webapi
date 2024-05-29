<template>
    <router-view></router-view>
</template>

<script>
  export default {
    name: 'App',
    data () {
      return {}
    },
    beforeMount () {
      this.$auth.ready()
    }
  }
</script>

    