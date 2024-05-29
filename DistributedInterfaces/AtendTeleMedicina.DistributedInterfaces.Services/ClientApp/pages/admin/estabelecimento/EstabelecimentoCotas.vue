<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">

    <el-card class="box-card box-card--main-card">
      <el-row>
        <el-col :span="14">
          <h2 class="box-card--h2">{{$route.meta.title}}</h2>
        </el-col>
      </el-row>

      <el-row class="estabelecimento">
        <el-col :span="24">
          <el-form :model="formData" labelPosition="top" ref="formData" label-width="170px">
            <el-row :gutter="20">
              <el-col :span="16">
                <el-form-item label="Estabelecimento:" prop="estabelecimentoId">
                  <el-input v-model="estabelecimento.nomeFantasia" disabled/>
                </el-form-item>
                <el-form-item label="Procedimento:" prop="procedimentos">
                  <el-row :gutter="20">
                    <el-col :span="12">
                      <el-input class="filter-item" v-model="procedimentoQuery.descricao" @input="getProcedimentos(procedimentoQuery.descricao)" />
                    </el-col>
                    <el-col :span="4">
                      <el-button type="primary" icon="fas fa-search" size="small"></el-button>
                    </el-col>
                  </el-row>
                </el-form-item>
              </el-col>
              <el-col :span="8"></el-col>
            </el-row>
            <el-row>
              <el-col :span="16">
                <el-table key="tab1"
                        border fit highlight-current-row
                        ref="tab1"
                        :data="filteredProcedimentos"
                        style="width: 100%">
                  <el-table-column prop="procedimento.codigo" label="Código" width="95" />
                  <el-table-column show-overflow-tooltip prop="procedimento.descricao" label="Descrição" />
                  <el-table-column prop="cota" label="Cota Atual" width="110">
                    <template slot-scope="scope">
                      <el-input v-model="scope.row.cota" disabled style="width: 60px; margin-left: 10px"></el-input>
                    </template>
                  </el-table-column>
                  <el-table-column prop="cotaTotal" label="Disponível" width="110">
                    <template slot-scope="scope">
                      <el-input v-model="scope.row.procedimento.cotaTotal" disabled style="width: 60px; margin-left: 10px"></el-input>
                    </template>
                  </el-table-column>
                  <el-table-column prop="Quantidade" label="Qt" width="70">
                    <template slot-scope="scope">
                      <el-input
                        v-model="scope.row.quantidade"
                        :max="scope.row.cotaTotal">
                      </el-input>
                    </template>
                  </el-table-column>
                  <el-table-column prop="Acao" label="Ação" width="80">
                    <template slot-scope="scope">
                      <el-button
                        :disabled="scope.row.procedimento.cotaTotal === 0 || scope.row.quantidade > scope.row.procedimento.cotaTotal"
                        size="mini"
                        type="success"
                        @click="onSalvar(scope.$index, scope.row)">
                        <i class="fas fa-save"></i>
                      </el-button>
                    </template>
                  </el-table-column>
                </el-table>

              <ul class="list-unstyled estabelecimento__legenda pt-1">
                <li>COTA ATUAL: Cota já distribuida para utilização do estabelecimento</li>
                <li>DISPONÍVEL: (Cota Total) disponível para o estabelecimento ainda não distribuída</li>
              </ul>
<!-- 
              // TODO a api precisa de atualização para fornecer a paginação.
              <el-pagination @size-change="handleSizeChange"
                            small
                            @current-change="handleCurrentChange"
                            :current-page.sync="params.page"
                            :page-sizes="[10,20,50]"
                            :page-size="params.pageSize"
                            layout="total, sizes, prev, pager, next"
                            :total="params.total">
              </el-pagination> -->

              <el-form-item class="pull-right">
                <el-button flat icon="fas fa-undo-alt" type="warning" @click="onVoltar"> {{ $config.txt.btVoltar }}</el-button>
              </el-form-item>
              </el-col>
              <el-col :span="8"></el-col>

            </el-row>

          </el-form>
        </el-col>
      </el-row>
   </el-card>

  </el-col>
</template>

<script>
  import Utils from '../../../mixins/Utils'
  import _api from '../../../api'
  import _enums from '../../../api/Enums'
  export default {
    name: 'EstabelecimentoCotas',
    mixins: [Utils],
    data () {
      return {
        formData: {
          estabalecimentoId: [],
          procedimentos: [],
          quantidade: ''
        },
        estabelecimento: '',
        filteredProcedimentos: [],
        api: {
          procedimentosCadastrados: []
        },
        loading: {
          procedimentosCadastrados: false
        },
        filtroSelect: [
          {label: 'Tudo', value: null},
          {label: 'Procedimento', value: 'procedimento'},
          {label: 'Especialidade', value: 'especialidade'}
        ],
        procedimentoQuery: {
          tipo: null,
          descricao: null
        },
        enums: {},
        params: {
          skip: 1,
          take: 10,
          sort: 'Descricao ASC',
          total: 0
        },
        procParams: {
          skip: 1,
          take: 10,
          descricao: '',
          estabelecimentoId: null,
          sort: 'Descricao ASC',
          total: 0
        }
      }
    },
    async created () {
      if (this.$route.params.estabelecimento === undefined) {
        this.$router.push({name: 'AdminEstabelecimento'})
      } else {
        this.estabelecimento = this.$route.params.estabelecimento
        await this.getEstabelecimentoProcedimentos()
      }
    },
    methods: {
      async getProcedimentos (val) {
        this.filteredProcedimentos = this.api.procedimentosCadastrados.filter(x => x.procedimento.descricao.includes(val.toUpperCase()))
      },
      async getEstabelecimentoProcedimentos () {
        let filters = {
          estabelecimentoId: this.estabelecimento.id,
          ...this.params
        }
        this.loading.procedimentosCadastrados = true
        let { data, paginacao, status } = await _api.estabelecimentos.getEstabelecimentoProcedimentos(filters)
        if (status === 502) this.loading.procedimentosCadastrados = false
        this.api.procedimentosCadastrados = (status === 200) ? data : []
        this.filteredProcedimentos = this.api.procedimentosCadastrados.slice(0)
        this.loading.procedimentosCadastrados = false
      },
      handleTipoChange (val) {
        this.params.tipo = val
        this.getProcedimentos()
      },
      handleFilter () {
        // this.getProcedimentos()
      },
      handleSizeChange (val) {
        this.params.pageSize = val
        this.getProcedimentos()
      },
      handleCurrentChange (val) {
        this.params.skip = val
        this.getProcedimentos()
      },
      async onSalvar (index, row) {
        let params = {
          procedimentoId: row.procedimentoId,
          estabelecimentoId: row.estabelecimentoId,
          quantidade: parseInt(row.quantidade)
        }
        await _api.estabelecimentos.distribuirCota(params)
        await this.getEstabelecimentoProcedimentos()
      },
      onVoltar () {
        this.$router.push({ name: 'Estabelecimentos' })
      }
    }
  }
</script>

<style scoped>
  .vertical-center {
    min-height: 70%;  /* Fallback for browsers do NOT support vh unit */
    min-height: 70vh; /* These two lines are counted as one :-)       */

    display: flex;
    align-items: center;
  }
</style>
