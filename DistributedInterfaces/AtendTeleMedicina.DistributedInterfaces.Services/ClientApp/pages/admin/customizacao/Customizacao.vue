<!--<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
    <el-card class="box-card box-card--main-card">

      <el-row>
        <el-col :span="14">
          <h2 class="box-card--h2">Customização</h2>
        </el-col>-->

        <!--<el-col :span="10" class="text-right">
          <el-form :inline="true">
            <el-form-item>
              <el-button v-if="!listando" style="margin-right: -10px" icon="fas fa-plus" type="success" @click="listando = !listando"> Novo</el-button>
              <el-button v-if="listando" style="margin-right: -10px" icon="fas fa-undo-alt" type="warning" @click="listando = !listando"> Voltar</el-button>
            </el-form-item>
          </el-form>
        </el-col>-->
      <!--</el-row>


      

      <el-row>

          <el-upload style="display: flex; flex-direction:column"
                         action="" ref="upload"
                         :before-upload="isPNG"
                         :on-success="encodeImageFileAsURL"
                         :on-remove="onDeleteDocs"
                         accept="image/png">

            <el-button v-if="showBtn" type="primary" style="margin-top: 5px" >Selecionar Imagem</el-button>

          </el-upload>

        <el-button style="margin-top: 10px" v-loading.body="loading.exames" @click="uploadDocs" type="primary">Enviar Imagem</el-button>-->
          <!--multiple-->
          <!--<el-button  @click="" type="primary">Enviar Imagem</el-button>-->

          <!--<h4>Enviar para:</h4>
          <input type="file" />
          <br />
          <el-button v-loading.body="loading.notificacao" style="margin-top:10px;" type="success" @click="onEnviar()">Enviar</el-button>-->

      <!--</el-row>
    </el-card>
  </el-col>
</template>

<script>
  import _api from '../../../api'
  import moment from 'moment'
  moment.locale('pt-br')

  export default {
    name: 'Customizacao',
    data() {
      return {
        showBtn: true,
        imagem: {},
        params: {
          skip: 1,
          take: 100,
          sort: 'titulo ASC',
        },
        api: {

        },
        loading: {
          notificacao: false,
          individuos: false
        }
      }
    },
    async mounted() {
      console.log(this.$store.state)
    },
    methods: {
     

    }
  }
</script>
<style>
</style>-->
