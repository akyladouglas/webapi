<template>
<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="20">

  <el-card class="box-card box-card--main-card">
    <el-row :gutter="20" style="min-height: 600px">
      <el-col :span="24" class="text-center" style="padding-top:50px">
         <h3><i class="fa fa-warning text-red"></i> Oops! Acesso Negado.</h3>
         <h5>Você não tem permissão de acesso para esta página ou operação.</h5>
      </el-col>
    </el-row>
  </el-card>

</el-col>
</template>

<script>
  export default {
    name: 'Erro403'
  }
</script>

