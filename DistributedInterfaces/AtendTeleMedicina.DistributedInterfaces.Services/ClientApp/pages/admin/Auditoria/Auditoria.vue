<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">

    <el-card class="box-card box-card--main-card">
      <el-row>
        <el-col :span="14">
          <h2 class="box-card--h2">{{$route.meta.title}}</h2>
        </el-col>
        <!--<el-col :span="10" class="text-right">
          <el-form :inline="true">
            <el-form-item>
              <el-button v-if="!loading.auditoria" style="margin-right: -10px" icon="fas fa-undo-alt" type="warning" @click="onVoltar"> Voltar</el-button>
            </el-form-item>
          </el-form>
        </el-col>-->
      </el-row>

      <el-row>
        <el-col :span="24">
          <FiltroAuditoria :loading="loading.auditoria" :params="params" @emit="onEmitFromFiltro" />
        </el-col>
      </el-row>

      <el-row>
        <el-col :span="24">
          <el-table 
            ref="tabela"
            :data="api.auditoria"
            highlight-current-row border
            v-loading.body="loading.auditoria"
            class="table--row-click">
            <el-table-column label="Id" prop="id" width="100" show-overflow-tooltip />
            <el-table-column label="Ação" prop="acao" fixed show-overflow-tooltip />
            <el-table-column label="Ip" prop="ip" width="100" show-overflow-tooltip />
            <el-table-column label="Utilizador" width="120">
              <template slot-scope="props">
                <span v-if="props.row.usuario !== undefined">{{ props.row.usuario }}</span>
                <span v-if="props.row.profissional !== undefined">{{ props.row.profissional }}</span>
                <span v-if="props.row.individuo !== undefined">{{ props.row.individuo }}</span>
              </template>
            </el-table-column>
            <el-table-column label="Registro" prop="tabelaId" width="100" show-overflow-tooltip />
            <el-table-column label="Tabela" prop="tabela"  width="100" show-overflow-tooltip />
            <el-table-column label="Origem" prop="origem" />
            <el-table-column label="Data" width="100">
              <template slot-scope="props">
              {{ moment(props.row.dataHora).format('DD-MM-YYYY') }}
              </template>
            </el-table-column>
            <el-table-column label="Hora" width="70">
              <template slot-scope="props">
              {{ moment(props.row.dataHora).format('HH:mm') }}
              </template>
            </el-table-column>
          </el-table>
        </el-col>

        <el-col :span="24" v-show="api.auditoria.length > 0">
          <el-pagination
            @size-change="handleSizeChange"
            @current-change="handleCurrentChange"
            :current-page.sync="params.page"
            :page-sizes="[10,25,50,100]"
            :page-size="params.pageSize"
            :total="params.total"
            layout="total, sizes, prev, pager, next, jumper">
          </el-pagination>
        </el-col>

         <el-col :span="24" class="text-right">
          <el-form :inline="true">
            <el-form-item>
              <el-button title="Download arquivo do arquivo de auditoria" class="mt-2" :disabled="api.auditoria.length === 0" icon="fas fa-download" type="primary" @click="onDownload"> Download JSON</el-button>
              <el-button v-if="!loading.relatorio" title="Download arquivo do arquivo de auditoria Excel" class="mt-2" :disabled="api.auditoria.length === 0" icon="fas fa-filter" type="primary" @click="onDownloadExcel"> Download EXCEL</el-button>
              <el-button v-if="loading.relatorio" type="info" icon="fa fa-spinner fa-spin" disabled> Aguarde ...</el-button>
            </el-form-item>
          </el-form>
        </el-col> 
      </el-row>
   </el-card>

  </el-col>
</template>

<script>
  import FiltroAuditoria from '../../../components/shared/FiltroAuditoria'

  import Utils from '../../../mixins/Utils'
  import _api from '../../../api'
  export default {
    name: 'Auditoria',
    components: { FiltroAuditoria },
    mixins: [Utils],
    data () {
      return {
        formData: {},
        loading: {
          auditoria: false,
          relatorio: false
        },
        api: {
          auditoria: []
        },
        params: {
          skip: 1,
          take: 10,
          sort: 'DataHora DESC',
          total: 0
        },
        paramsExcel: {
          sort: 'DataHora DESC',
        }
      }
    },
    async created () {
      await this.getAuditoria()
    },
    methods: {
      async getAuditoria () {
        this.loading.auditoria = true
        let { data, paginacao, status } = await _api.auditoria.get(this.params)
        if (status === 502) this.loading.auditoria = false
        this.api.auditoria = (status === 200) ? data : []
        this.params.skip = (status === 200) ? paginacao.skip : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.auditoria = false
      },
      onEmitFromFiltro (val) {
        //console.log('veio pra ca', val)
        this.params = {
          ...this.params,
          ...val.params,
          skip: 1
        }
        this.paramsExcel = {
          ...this.paramsExcel,
          ...val.params,
        }
        this.getAuditoria()
      },
      handleSizeChange (val) {
        this.params.take = val
        this.getAuditoria()
      },
      handleCurrentChange (val) {
        this.params.skip = val
        this.getAuditoria()
      },
      onDownload () {
        let blob = window.URL.createObjectURL(new Blob([JSON.stringify(this.api.auditoria)]))
        let link = document.createElement('a')
        link.href = blob
        link.setAttribute('download', 'auditoria.json')
        document.body.appendChild(link)
        link.click()
      },
      async onDownloadExcel() {
        this.loading.relatorio = true
        await _api.relatorios.excelRelatorioAuditoria(this.paramsExcel)
        this.loading.relatorio = false
      },
      onVoltar () {
        this.$router.push({ name: 'Estabelecimentos' })
      }
    }
  }
</script>

<style>

</style>
