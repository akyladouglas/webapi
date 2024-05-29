<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">

    <el-card class="box-card box-card--main-card">
      <el-row :gutter="24">
        <el-col :xs="14" :sm="14" :md="14" :lg="14" :xl="14">
          <h2 class="box-card--h2">Relatório De Glicemias</h2>
        </el-col>
      </el-row>

      <el-row :gutter="24">
        <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
          <FiltroRelatorioGlicemia :loading="loading.glicemias" :gerarRelatorioParams="filtroParams" @emit="onEmitFromFiltro" />
        </el-col>
      </el-row>

      <el-row v-if="api.glicemias.length === 0">
        <el-empty description="Nenhum dado de glicemia encontrado"></el-empty>
      </el-row>

      <el-row v-if="api.glicemias.length > 0" :gutter="24">
        <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
          <el-table ref="tableGlicemias" :data="api.glicemias"
                    v-loading.body="loading.glicemias"
                    highlight-current-row border>

            <el-table-column label="Data do Registro" align="center">
              <template slot-scope="scope">
                {{ moment(scope.row.dataCadastro).format("DD/MM/YYYY") }}
              </template>
            </el-table-column>

            <el-table-column label="Paciente" prop="individuo.nomeCompleto" align="center" />

            <el-table-column label="Status" align="center">
              <template slot-scope="scope">
                <el-tag v-if="(scope.row.respondeuCafe != false && scope.row.respondeuCafe != undefined) &&
                        (scope.row.respondeuAlmoco != false && scope.row.respondeuAlmoco != undefined) &&
                        (scope.row.respondeuJanta != false && scope.row.respondeuJanta != undefined) &&
                        (scope.row.respondeuDormirMadrugada != false && scope.row.respondeuDormirMadrugada != undefined)"
                        type="success">Completo</el-tag>
                <el-tag v-else type="danger">Incompleto</el-tag>
              </template>
            </el-table-column>

          </el-table>
        </el-col>

        <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
          <el-pagination @size-change="handleSizeChange"
                         @current-change="handleCurrentChange"
                         :current-page.sync="filtroParams.page"
                         :page-sizes="[5, 10, 20, 50, 100]"
                         :page-size="filtroParams.take"
                         :total="filtroParams.total"
                         layout="total, sizes, prev, pager, next, jumper">
          </el-pagination>
        </el-col>
      </el-row>
    </el-card>
  </el-col>
</template>

<script>
  import _api from '../../api'
  import FiltroRelatorioGlicemia from '../../components/shared/FiltroRelatorioGlicemia'
  export default {
    name: 'RelatorioGlicemia',
    mixins: [],
    components: { FiltroRelatorioGlicemia },
    data() {
      return {
        api: {
          glicemias: [],
        },

        loading: {
          glicemias: false,
        },

        filtroParams: {
          skip: 1,
          take: 10,
          sort: 'ig.DataCadastro DESC'
        },
      }
    },
    async mounted() {
      await this.getGlicemias()
    },
    methods: {
      async onEmitFromFiltro(val) {
        this.filtroParams = {
          ...this.filtroParams,
          ...val.params,
        }
        await this.getGlicemias()
      },

      async getGlicemias() {
        this.loading.glicemias = true

        let { data, status, paginacao } = await _api.glicemias.get(this.filtroParams)
        this.api.glicemias = (status === 200) ? data : []

        this.filtroParams.skip = (status === 200) ? paginacao.skip : 0
        this.filtroParams.total = (status === 200) ? paginacao.totalCount : 0

        this.loading.glicemias = false
      },

      async handleSizeChange(val) {
        this.filtroParams.take = val
        await this.getGlicemias()
      },

      async handleCurrentChange(val) {
        this.filtroParams.skip = val
        await this.getGlicemias()
      },
    },
  }
</script>

<style scoped>

</style>
