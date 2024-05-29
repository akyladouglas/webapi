<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">

    <el-card class="box-card box-card--main-card">

      <el-row>
        <el-col :span="14">
          <h2 class="box-card--h2">{{$route.meta.title}}</h2>
        </el-col>
      </el-row>

      <el-row v-show="!listando">
        <el-col :span="24">
          <FiltroIntegracao :params="params" :loadingLotes="loading.lotes" @filtrar="onFiltrar" @gerar="onGerar" />
        </el-col>
      </el-row>
      <el-row v-show="!listando && api.lotes.length > 0">
        <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
          <el-table ref="tabela" :data="api.lotes" highlight-current-row border  v-loading.body="loading.lotes">
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
            <el-table-column label="Lote" header-align="center" align="center" width="100">
              <template slot-scope="props">
                <el-button icon="fas fa-download" :disabled="props.row.validos === 0" class="bt-lote" size="mini" @click="onBaixarZip(props.row)" />
              </template>
            </el-table-column>
            <el-table-column label="Relatórios" header-align="center" align="center" width="100">
              <template slot-scope="props">
                <el-tooltip content="Relatório com o Total de Fichas Geradas por tipo" placement="top">
                  <el-button icon="fas fa-file-excel" :disabled="props.row.validos === 0" class="bt-lote-totais" type="success" size="mini" @click="onRelatorioFichasGeradas(props.row)" />
                </el-tooltip>
                <el-tooltip content="Relatório de Erros" placement="top">
                  <el-button icon="fas fa-file-excel" :disabled="props.row.erros === 0" class="bt-lote-erro" type="danger" size="mini" @click="onRelatorioErros(props.row)" />
                </el-tooltip>
              </template>
            </el-table-column>
            
          </el-table>
        </el-col>
      </el-row>
    
    </el-card>

  </el-col>
</template>

<script>
  import axios from 'axios'
  import Utils from '../../../mixins/Utils'
  import _api from '../../../api'
  import FiltroIntegracao from '../../../components/shared/FiltroIntegracao'
  export default {
    name: 'Integracao',
    mixins: [Utils],
    components: { FiltroIntegracao },
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
      async onRelatorioErros (lote) {
        let params = {
          loteId: lote.id
        }
        await _api.integracao.getRelatorioErros(params)
      },
      async onRelatorioFichasGeradas (lote) {
        let params = {
          loteId: lote.id
        }
        await _api.integracao.getRelatorioFichasGeradas(params)
      },
      async onFiltrar (val) {
        this.loading.lotes = true
        this.params = val
        let { data, status, paginacao } = await _api.integracao.listarLotes(this.params)
        this.api.lotes = data
        if (data.length === 0) {
          this.$swal({
            title: "Atenção!",
            text: 'Lote não disponível no período selecionado!',
            icon: 'warning',
          })
        }
        this.params.skip = (status === 200) ? paginacao.skip : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.listando = false
        this.loading.lotes = false
      },
      async onGerar (val) {
        this.loading.lotes = true
        this.params = val
        let { status } = await _api.integracao.gerarLote(this.params)
        if (status === 200) {
          this.onFiltrar(this.params)
        }
        this.loading.lotes = false
      }
    }
  }
</script>

<style>

.bt-lote {
  background: transparent !important;
  border: 0;
  padding: 0 !important;
  max-width: 20px !important;
}

.bt-lote-erro {
  background: transparent !important;
  border: 0;
  color: red !important;
  padding: 0 !important;
}

.bt-lote-totais {
  background: transparent !important;
  border: 0;
  color: rgb(13, 184, 132) !important;
  padding: 0 !important;
}


</style>
