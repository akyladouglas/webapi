<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">

    <el-card class="box-card box-card--main-card">
      <el-row>
        <el-col :span="14">
          <h2 class="box-card--h2">{{$route.meta.title}}</h2>
        </el-col>
      </el-row>

      <el-row>
        <el-col :span="24">
          <el-form :model="formData" :rules="validacoes" labelPosition="top" ref="formData" label-width="170px">
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="Profissional:" prop="profissionalId">
                  <el-input v-model="profissional.nome" disabled />
                </el-form-item>
                <el-form-item label="Estabelecimento:" prop="estabelecimento">
                  <el-row :gutter="20">
                    <el-col :span="12">
                      <el-input class="filter-item" v-model="procedimentoQuery.nomeFantasia" />
                    </el-col>
                    <el-col :span="4">
                      <el-button type="primary" icon="fas fa-search" @click="getEstabelecimentosFilter(procedimentoQuery)"></el-button>
                    </el-col>

                  </el-row>
                </el-form-item>
              </el-col>
              <el-col :span="24" />
              <el-col :span="12">
                <el-table key="tab1"
                          v-loading.body="loading.estabelecimentos"
                          border fit highlight-current-row
                          ref="tab1"
                          :data="api.estabelecimentos"
                          style="width: 100%"
                          @selection-change="handleSelectionChangeTab1">
                  <el-table-column type="selection" width="40" />
                  <el-table-column prop="cnes" label="CNES" width="95" />
                  <el-table-column show-overflow-tooltip prop="nomeFantasia" label="Estabelecimento">
                  </el-table-column>
                </el-table>
                <el-row v-show="!loading.estabelecimentos">
                  <el-pagination @size-change="handleSizeChange"
                                 @current-change="handleCurrentChange"
                                 :current-page.sync="estabParams.page"
                                 :page-sizes="[10,25,50,100]"
                                 :page-size="estabParams.pageSize"
                                 :total="estabParams.total"
                                 layout="total, sizes, prev, pager, next, jumper">
                  </el-pagination>
                </el-row>
              </el-col>
              <el-col :span="2">
                <div class="vertical-center">
                  <div style="text-align: center">
                    <el-button type="primary" size="mini" @click="onAdicionar()" icon="el-icon-arrow-right" style="margin: 0 auto 3px;"></el-button>
                    <el-button type="danger" size="mini" @click="onRemover()" icon="el-icon-arrow-left" style="margin: 0 auto 3px;"></el-button>
                  </div>
                </div>
              </el-col>
              <el-col :span="10">
                <h3 class="estabelecimento__titulo--mt-negativo">Estabelecimento(s) Cadastrado(s):</h3>
                <el-table key='tab2'
                          height="470"
                          border fit highlight-current-row
                          ref="tab2"
                          :data="api.estabelecimentosCadastrados"
                          style="width: 100%"
                          class="table--expandable"
                          @selection-change="handleSelectionChangeTab2">
                  <el-table-column :selectable="rowSelectable" type="selection" width="40" />
                  <el-table-column prop="cnes" label="CNES" width="95" />
                  <el-table-column show-overflow-tooltip prop="nomeFantasia" label="Estabelecimento" />
                </el-table>

                <el-row>
                  <el-col>
                    <el-form-item class="float-right mt-4">
                      <el-button flat icon="fas fa-undo-alt" type="warning" @click="onVoltar"> {{ $config.txt.btVoltar }}</el-button>
                      <el-button flat icon="fas fa-save" type="success" v-loading.body="loading.salvar" @click="estabelecimentoProfissionalAdd('formData')"> {{ $config.txt.btSalvar }}</el-button>
                    </el-form-item>
                  </el-col>
                </el-row>

              </el-col>
              
            </el-row>
          </el-form>
        </el-col>
      </el-row>
   </el-card>

  </el-col>
</template>

<script>
  import Utils from '../../../mixins/Utils'
  import jQuery from 'jquery'
  import _api from '../../../api'
  import _enums from '../../../api/Enums'
  export default {
    name: 'ProfissionalEstabelecimento',
    mixins: [Utils],
    data () {
      return {
        cnes: [],
        loadingCnes: false,
        procedimentoQuery: {
          //tipo: null,
          nomeFantasia: null,
        },
        // formData: {
        //   estabalecimentoId: [],
        //   procedimentos: []
        // },
        formData: {},
        estabelecimento: '',
        estabelecimentosJaCadastrados: [],
        tempSelect: [],
        tempSelect2: [],
        
        validacoes: {
          estabelecimentoId: [
            { required: false, type: 'string', message: this.$config.validacoes.cnesRequired, trigger: 'submit' }
          ],
          procedimento: [
            { required: false, type: 'string', message: this.$config.validacoes.procedimentoRequired, trigger: 'submit' }
          ]
        },
        enums: {},
        api: {
          estabelecimentos: [],
          procedimentos: [],
          profissionais: [],
          estabelecimentosCadastrados: [],
          unique: []
        },
        loading: {
          estabelecimentos: false,
          procedimentos: false,
          estabelecimentosCadastrados: false
        },
        params: {
          skip: 1,
          take: 10,
          sort: 'Nome ASC',
          total: 0
        },
        estabParams: {
          skip: 1,
          take: 10,
          sort: 'e.NomeFantasia ASC',
          total: 0
        },
       
      }
    },
    async created () {
      if (this.$route.params.profissional === undefined) {
        this.$router.push({name: 'AdminProfissional'})
      } else {
        this.profissional = this.$route.params.profissional
        await this.getEstabelecimentos()
        await this.getProfissionais()
        await this.getEstabelecimentoProfissionais()
      }
    },
    methods: {
      async getEstabelecimentos () {
        this.loading.estabelecimentos = true
        let { data, paginacao, status } = await _api.estabelecimentos.get(this.estabParams)
        //console.log('data', data)
        if (status === 502) this.loading.estabelecimentos = false
        this.api.estabelecimentos = (status === 200) ? data : []
        this.estabParams.skip = (status === 200) ? paginacao.skip : 0
        this.estabParams .total = (status === 200) ? paginacao.totalCount : 0
        this.loading.estabelecimentos = false
      },

      async getEstabelecimentosFilter (procedimentoQuery) {
        this.loading.estabelecimentos = true
        let { data, paginacao, status } = await _api.estabelecimentos.get(procedimentoQuery)
        //console.log('data', data)
        if (status === 502) this.loading.estabelecimentos = false
        this.api.estabelecimentos = (status === 200) ? data : []
        this.estabParams.skip = (status === 200) ? paginacao.skip : 0
        this.estabParams .total = (status === 200) ? paginacao.totalCount : 0
        this.loading.estabelecimentos = false
      },
      async getProfissionais () {
        this.loading.profissionais = true
        let { data, paginacao, status } = await _api.profissionais.get(this.params)
        if (status === 502) this.loading.profissionais = false
        this.api.profissionais = (status === 200) ? data : []
        this.params.skip = (status === 200) ? paginacao.skip : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.profissionais = false
      },
      async getEstabelecimentoProfissionais() {

        for (var i = 0; i < this.api.estabelecimentos.length; i++) {
          if (this.api.estabelecimentos[i].profissionais.some(profissional => profissional.id === this.profissional.id)) {
            this.api.estabelecimentosCadastrados.push(this.api.estabelecimentos[i]);
            
          }
        }

        this.api.estabelecimentosCadastrados.forEach(i => {
          i.jaCadastrado = true
        })

        this.api.unique = [...new Set(this.api.estabelecimentosCadastrados)];
        this.api.estabelecimentosCadastrados = this.api.unique;
        //console.log("UNIQUE", uniqueChars);

        this.loading.estabelecimentosCadastrados = false
      },
      handleTipoChange (val) {
        this.procParams.tipo = val
        this.getEstabelecimentos()
      },
      handleFilter () {
         this.getProcedimentos()
      },
      handleSizeChange (val) {
        this.procParams.pageSize = val
        this.getEstabelecimentos()
      },
      handleCurrentChange (val) {
        this.estabParams.skip = val
        this.getEstabelecimentos()
      },
      handleSelectionChangeTab1 (val) {
        this.tempSelect = val
        //console.log('eita', this.tempSelect)
      },
      handleSelectionChangeTab2 (val) {
        this.tempSelect2 = val
      },
      onVoltar () {
        this.$router.push({ name: 'Profissionais' })
      },
      onAdicionar() {
        // TODO Remover linha abaixo após implementar API que retorna os estabelecimentos cadastrados.
        if (!this.api.estabelecimentosCadastrados) this.api.estabelecimentosCadastrados = []
        //console.log('entrou aqui', this.api.estabelecimentosCadastrados)
        this.tempSelect.forEach(item => {
          if (!this._.some(this.api.estabelecimentosCadastrados, ['id', item.id])) {
            this.api.estabelecimentosCadastrados.push(item)
          } else {
            this.$swal({
              title: "Atenção!",
              text: 'O profissional já foi cadastrado para este estabelecimento!',
              icon: 'warning',
            })           
          }
        })
        this.clearSelectionTab1()
      },
      rowSelectable (row) {
        if (!row.jaCadastrado) return true
      },
     onRemover () {
       this.tempSelect2.forEach(item => {
         this.removeDoArray(this.api.estabelecimentosCadastrados, item)
        })
      },
      clearSelectionTab1 () {
        this.$refs.tab1.clearSelection()
      },
      toggleSelectionTab1 () {
        // para ser adicionado em um botão separado 'selecionar tudo'
        this.api.estabelecimentos.forEach(row => {
          this.$refs.tab1.toggleRowSelection(row)
        })
      },
      estabelecimentoProfissionalAdd (formData) {
        this.loading.salvar = true
        this.$refs[formData].validate(valid => {
          let aSerAdicionado
          var estabelecimentoProfissional = []
          if (valid) {
            

            aSerAdicionado = this.api.estabelecimentosCadastrados.filter((item) => item.jaCadastrado !== true)

            aSerAdicionado.forEach(item => {
              estabelecimentoProfissional.push({
                estabelecimentoId: item.id,
                profissionalId: this.profissional.id,
              })
            })

            _api.profissionais.postEstabelecimentoProfissionais(estabelecimentoProfissional).then(res => {
                if (res.status === 201) this.getEstabelecimentoProfissionais()
                this.loading.salvar = false
            })
          } else {
            this.loading.salvar = false
            this.$swal({
              title: "Atenção!",
              text: `${this.$config.txt.formInvalido}!`,
              icon: 'warning',
            })
            return false
          }
        })
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
