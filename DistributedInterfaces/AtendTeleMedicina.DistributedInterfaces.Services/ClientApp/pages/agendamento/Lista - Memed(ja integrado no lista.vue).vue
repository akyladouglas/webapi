<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
    <el-card class="box-card box-card--main-card">
      <el-row>
        <Integration />
      </el-row>
    </el-card>
  </el-col>
</template>

<script>
  import Integration from './Integration.vue'

  export default {
    name: 'Memed',
    components: {
      Integration
    }
  }
</script>
