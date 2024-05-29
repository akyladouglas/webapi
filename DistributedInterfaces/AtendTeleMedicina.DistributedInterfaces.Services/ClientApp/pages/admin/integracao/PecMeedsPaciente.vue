<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">

    <el-card class="box-card box-card--main-card">

      <el-row>
        <el-col :span="14">
          <h2 class="box-card--h2">{{$route.meta.title}}</h2>
        </el-col>
      </el-row>

      <el-row>
        <el-form label-width="120px" label-position="top" class="forms--import-paciente">
            <el-col :span="24">
                <el-form-item label="Arquivo de Lote" prop="arquivo">
                <el-upload 
                            action="" ref="upload"
                            :before-upload="checkFileType"
                            :on-success="onSuccess"
                            :limit="1"
                            accept="application/zip">
                            <el-button type="primary" style="margin-top: 5px" >Selecionar Arquivo</el-button>
                </el-upload>
                </el-form-item>
            </el-col>
        </el-form>
      </el-row>
      <el-row v-show="!listando && api.lotes.length > 0">
        <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
            Importa Paciente
          <!-- <el-table ref="tabela" :data="api.lotes" highlight-current-row border  v-loading.body="loading.lotes">
            <el-table-column label="Estabelecimento" prop="estabelecimento.nomeFantasia" width="350"  show-overflow-tooltip />
            <el-table-column label="Lote #" prop="id" width="100" />
            <el-table-column label="Data">
              <template slot-scope="props">
                {{ moment(props.row.data, 'YYYY-MM-DD').format('DD/MM/YYYY') }}
              </template>
            </el-table-column>
            <el-table-column label="Data">
              <template slot-scope="props">
                {{ props.row.mes }}/{{ props.row.ano }}
              </template>
            </el-table-column>
            <el-table-column label="Válidas" width="100" prop="validos" />
            <el-table-column label="Inválidas" width="100" prop="erros" />
            <el-table-column header-align="center" align="center" width="100" fixed="right">
              <template slot-scope="props">
                <el-button icon="fas fa-download" size="small" @click="onBaixarZip(props.row)" />
              </template>
            </el-table-column>
          </el-table> -->
        </el-col>
      </el-row>
    
    </el-card>

  </el-col>
</template>

<script>
  import axios from 'axios'
  import Utils from '../../../mixins/Utils'
  import _api from '../../../api'

  export default {
    name: 'PecMeedsPaciente',
    mixins: [Utils],
    data () {
      return {
        metodo: 'POST',
        listando: false,
        api: {
          lotes: []
        },
        loading: {
          lotes: false
        },
        params: {
          page: 1,
          pageSize: 10
        }
      }
    },
    methods: {
      checkFileType (file) {
        const valid = file.type === 'application/zip' || file.type === 'application/x-zip-compressed' || file.type === 'application/x-compressed'
        if (!valid) {
          this.$swal({
            title: "Erro!",
            text: 'O arquivo deve estar no formato ZIP!',
            icon: 'error',
          })
        }
        return valid
      }, 
      async onSuccess(response, file) {
        var arq = file.raw;
        var reader = new FileReader();
        reader.onloadend = () => {};
        reader.readAsDataURL(arq);
        console.log("arquivo", reader)
      },      
      onBaixarZip (lote) {
        let params = {
          id: lote.id
        }
        let fileName = `lote_${lote.id}_${lote.estabelecimento.cnes}-${lote.estabelecimento.nomeFantasia}.zip`
        this.loading = true
        axios({
          method: 'GET',
          url: `/api/v1/Integracao/BaixarZip`,
          params: params,
          responseType: 'blob'
        })
          .then((res) => {
            if (res.status === 200) {
              let blob = window.URL.createObjectURL(new Blob([res.data], { type: 'application/application/zip' }))
              let link = document.createElement('a')
              link.href = blob
              link.setAttribute(`download`, fileName)
              document.body.appendChild(link)
              link.click()
              this.loading = false
            }
          })
          .catch((e) => {
            console.log(e)
            this.erroMsg = 'Erro, tenta efetuar a operação novamente ou entre em contato com o suporte'
            this.loading.relatorio = false
          })
      },
      async onFiltrar (val) {
        this.listando = true
        this.params = {
          ...val.params
        }
        let { data, status, paginacao } = await _api.integracao.listarLotes(this.params)
        this.api.lotes = data
        this.params.skip = (status === 200) ? paginacao.skip : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.listando = false
      },
      async onGerar (val) {
        this.params = {
          ...val.params
        }
        let { data, status, paginacao } = await _api.integracao.listarLotes(this.params)
        this.api.lotes = data.items
        this.params.skip = (status === 200) ? paginacao.skip : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.listando = true
      }
    }
  }
</script>

<style>

</style>
