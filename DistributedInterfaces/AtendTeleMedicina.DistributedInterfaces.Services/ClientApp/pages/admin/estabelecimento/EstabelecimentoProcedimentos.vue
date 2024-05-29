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
          <el-form :model="formData" :rules="validacoes" labelPosition="top" ref="formData" label-width="170px">
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="Estabelecimento:" prop="estabelecimentoId">
                  <el-input v-model="estabelecimento.nomeFantasia" disabled />
                </el-form-item>
                <el-form-item label="Tipo:" prop="tipo">
                  <el-input :value="estabelecimento.tipo.toUpperCase()" disabled />
                </el-form-item>
                <el-form-item label="Tipo De Filtro:" prop="procedimentos">
                  <el-row :gutter="20">
                    <el-col :span="8">
                      <el-select @change='handleTipoDoFiltroChange'
                                 v-model="tipoDoProcedimento"
                                 placeholder="Código ou Descrição">
                        <el-option v-for="item in filtroTipo"
                                   :key="item.value"
                                   :label="item.label"
                                   :value="item.value">
                        </el-option>
                      </el-select>
                    </el-col>
                  </el-row>
                </el-form-item>
                <el-form-item v-show="tipoDoProcedimento == 'descricao'" label="Procedimento(Descrição):" prop="descricao">
                  <el-row :gutter="20">
                    <el-col :span="12">
                      <el-input class="filter-item" v-model="procedimentoQuery.descricao" />
                    </el-col>
                    <el-col :span="4">
                      <el-button type="primary" icon="fas fa-search" @click="clickProcedimentos(procedimentoQuery.descricao)"></el-button>
                    </el-col>
                    <el-col :span="8">
                      <el-select @change='handleTipoChange'
                                 v-model="procedimentoQuery.tipo"
                                 placeholder="Filtro">
                        <el-option v-for="item in filtroSelect"
                                   :key="item.value"
                                   :label="item.label"
                                   :value="item.value">
                        </el-option>
                      </el-select>
                    </el-col>
                  </el-row>
                </el-form-item>
                <el-form-item v-show="tipoDoProcedimento == 'codigo'" label="Procedimento(Código):" prop="codigo">
                  <el-row :gutter="20">
                    <el-col :span="12">
                      <el-input class="filter-item" v-model="procedimentoQuery.codigo" />
                    </el-col>
                    <el-col :span="4">
                      <el-button type="primary" icon="fas fa-search" @click="clickProcedimentos(procedimentoQuery.codigo)"></el-button>
                    </el-col>
                    <el-col :span="8">
                      <el-select @change='handleTipoChange'
                                 v-model="procedimentoQuery.tipo"
                                 placeholder="Filtro">
                        <el-option v-for="item in filtroSelect"
                                   :key="item.value"
                                   :label="item.label"
                                   :value="item.value">
                        </el-option>
                      </el-select>
                    </el-col>
                  </el-row>
                </el-form-item>
              </el-col>
              <el-col :span="24" />
              <el-col :span="12">
                <el-table key="tab1"
                          v-loading.body="loading.loadingProcedimentos"
                          border fit highlight-current-row
                          ref="tab1"
                          :data="api.procedimentos"
                          style="width: 100%"
                          @selection-change="handleSelectionChangeTab1">
                  <el-table-column type="selection" width="40" />
                  <el-table-column prop="codigo" label="Código" width="95" />
                  <el-table-column show-overflow-tooltip prop="descricao" label="Descrição">
                  </el-table-column>
                  <!--<el-table-column-->
                    <!--align="center"-->
                    <!--show-overflow-tooltip-->
                    <!--prop="cotaTotal"-->
                    <!--label="Cota Total"-->
                    <!--width="100">-->
                  <!--</el-table-column>-->
                </el-table>
                <el-row v-show="!loading.procedimentos">
                  <el-pagination @size-change="handleSizeChange"
                                small
                                @current-change="handleCurrentChange"
                                :current-page.sync="procParams.page"
                                :page-sizes="[10,20,50]"
                                :page-size="procParams.pageSize"
                                layout="total, sizes, prev, pager, next"
                                :total="procParams.total">
                  </el-pagination>
                </el-row>
              </el-col>
              <el-col :span="2">
                <div class="estabelecimento__setas">
                  <div class="text-center">
                    <el-button type="primary" size="mini" @click="onAdicionar()" icon="el-icon-arrow-right" style="margin: 0 auto 3px;"></el-button>
                    <el-button type="danger" size="mini" @click="onRemover()" icon="el-icon-arrow-left" style="margin: 0 auto 3px;"></el-button>
                  </div>
                </div>
              </el-col>
              <el-col :span="10">
                <h3 class="estabelecimento__titulo--mt-negativo">Procedimentos Cadastrados:</h3>
                <el-table key='tab2'
                          height="470"
                          border fit highlight-current-row
                          ref="tab2"
                          :data="api.procedimentosCadastrados"
                          style="width: 100%"
                          class="table--expandable"
                          @selection-change="handleSelectionChangeTab2">
                    <el-table-column :selectable="rowSelectable" type="selection" width="40" />
                    <el-table-column type="expand">
                      <template slot-scope="props">
                        <ul v-if="props.row.cotaTotal > 0" class="list-unstyled">
                           <li>COTA DISTRIBUIDA: {{ props.row.cota }}</li>
                           <li>COTA DISPONÍVEL: {{ props.row.procedimento.cotaTotal - props.row.cota }}</li>
                           <li>COTA TOTAL: {{props.row.procedimento.cotaTotal}}</li>
                        </ul>
                      </template>
                    </el-table-column>
                    <el-table-column prop="procedimento.codigo" label="Código" width="95" />
                    <el-table-column show-overflow-tooltip prop="procedimento.descricao" label="Descrição" />
                    <el-table-column prop="procedimento.cotaTotal" label="C. Total" width="70" />
                  </el-table>

                  <el-row>
                    <el-col>
                      <el-form-item class="float-right mt-4">
                        <el-button flat icon="fas fa-undo-alt" type="warning" @click="onVoltar"> {{ $config.txt.btVoltar }}</el-button>
                        <el-button flat icon="fas fa-save" type="success" v-loading.body="loading.salvar" :disabled="!estadoBotao" @click="estabelecimentoProcedimentoAdd('formData')"> {{ $config.txt.btSalvar }}</el-button>
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
  import _api from '../../../api'
  import _enums from '../../../api/Enums'
  export default {
    name: 'EstabelecimentoProcedimentos',
    mixins: [Utils],
    data() {

      var validaCodigo = (rule, value, callback) => {
        if (!value) return callback()
        if (value.length > 7) {
          if (this.mxValidaCodigo(value) === false) {
            return callback(new Error('O campo pode conter apenas letras e números'))
          } else {
            callback()
          }
        } else {
          return callback(new Error('Digite no minimo 8 caracteres'))
        }
      }

      var validaDescricao = (rule, value, callback) => {
        if (!value) return callback()
        if (value.length > 5) {
          if (this.mxValidaDescricao(value) === false) {
            return callback(new Error('Caracteres inseridos no campo estão invalidos'))
          } else {
            callback()
          }
        } else {
          return callback(new Error('Digite no minimo 6 caracteres'))
        }

      }


      return {
        formData: {},
        tipoDoProcedimento: '',
        estabelecimento: '',
        estadoBotao: false,
        api: {
          estabelecimentos: [],
          procedimentos: [],
          procedimentosCadastrados: []
        },
        loading: {
          estabelecimentos: false,
          procedimentos: false,
          procedimentosCadastrados: false
        },
        procedimentosJaCadastrados: [], //?
        tempSelect: [],
        tempSelect2: [],
        filtroSelect: [
          {label: 'Tudo', value: null},
          {label: 'Procedimento', value: 'procedimento'},
          {label: 'Especialidade', value: 'especialidade'}
        ],
        filtroTipo: [
          { label: 'Código', value: 'codigo' },
          { label: 'Descrição', value: 'descricao' }
        ],
        procedimentoQuery: {
          tipo: null,
          descricao: null,
          codigo: null
        },
        validacoes: {
          codigo: [
            { required: true, validator: validaCodigo, trigger: ['blur', 'change'] },
          ],
          descricao: [
            { required: true, validator: validaDescricao, trigger: ['blur', 'change'] },
          ],
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
        },
        loading: {
          estabelecimentos: false,
          procedimentos: false,
          salvar: false,
        },
        params: {
          skip: 1,
          take: 10,
          sort: 'e.NomeFantasia ASC',
          total: 0
        },
        procParams: {
          skip: 1,
          take: 10,
          descricao: '',
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
        // this.estabelecimento.tipo = this.estabelecimento.tipo === 'T' ? 'Todos' : 'Executante'
        switch (this.estabelecimento.tipo) {
          case 'E':
            this.estabelecimento.tipo = 'EXECUTANTE'
            break
          case 'S':
            this.estabelecimento.tipo = 'SOLICITANTE'
            break
          default:
            this.estabelecimento.tipo = 'TODOS'
        }
        await this.getEstabelecimentoProcedimentos()
        await this.getEstabelecimentos()
        await this.getProcedimentos()
      }
    },
    methods: {
      async getEstabelecimentos () {
        this.loading.estabelecimentos = true
        let { data, paginacao, status } = await _api.estabelecimentos.get(this.params)
        if (status === 502) this.loading.estabelecimentos = false
        this.api.estabelecimentos = (status === 200) ? data : []
        this.params.skip = (status === 200) ? paginacao.skip : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.estabelecimentos = false
      },
      async getProcedimentos(val) {
       // console.log('val', val)
        this.procParams.descricao = val
        this.loading.procedimentos = true
        //console.log('this.procParams', this.procParams)
        let { data, paginacao, status } = await _api.procedimentos.get(this.procParams)
        if (status === 502) this.loading.procedimentos = false
        this.api.procedimentos = (status === 200) ? data : []
        this.procParams.skip = (status === 200) ? paginacao.skip : 0
        this.procParams.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.procedimentos = false
      },
      async clickProcedimentos(val) {
        if (this.tipoDoProcedimento == 'descricao') {
          this.procParams.descricao = val
        }
        if (this.tipoDoProcedimento == 'codigo') {
          this.procParams.codigo = val
        }
        this.loading.procedimentos = true
        this.procParams.skip = 1
        let { data, paginacao, status } = await _api.procedimentos.get(this.procParams)
        if (status === 502) this.loading.procedimentos = false
        this.api.procedimentos = (status === 200) ? data : []
        this.procParams.skip = (status === 200) ? paginacao.skip : 0
        this.procParams.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.procedimentos = false
      },
      async getEstabelecimentoProcedimentos () {
        let filters = {
          estabelecimentoId: this.estabelecimento.id,
          skip: 1,
          take: 999,
          descricao: '',
          sort: 'Descricao ASC',
          total: 0
        }
        //console.log("FILTERS PROCEDIMENTO", filters)
        this.loading.procedimentosCadastrados = true
        let { data, paginacao, status } = await _api.estabelecimentos.getEstabelecimentoProcedimentos(filters)
        if (status === 502) this.loading.procedimentosCadastrados = false
        this.api.procedimentosCadastrados = (status === 200) ? data : []
        this.api.procedimentosCadastrados.forEach(i => {
          i.jaCadastrado = true
        })
        this.loading.procedimentosCadastrados = false
      },

      rowSelectable(row) {
        if (!row.jaCadastrado) return true
      },

      handleTipoChange (val) {
        this.procParams.tipo = val
        this.getProcedimentos()
      },
      handleTipoDoFiltroChange(val) {
        if (val == 'descricao') {
          this.procedimentoQuery.codigo = null

          this.getProcedimentos()
        } else {
          this.procedimentoQuery.descricao = null

          this.getProcedimentos()
        }
      },
      handleFilter () {
        // this.getProcedimentos()
      },
      handleSizeChange (val) {
        this.procParams.take = val
        this.getProcedimentos()
      },
      handleCurrentChange (val) {
        this.procParams.skip = val
        this.getProcedimentos()
      },
      handleSelectionChangeTab1 (val) {
        this.tempSelect = val
      },
      handleSelectionChangeTab2 (val) {
        this.tempSelect2 = val
      },
      onAdicionar () {
        this.tempSelect.forEach(item => {
       //   console.log("ITEM", item.id);
      //    console.log("ESTAB ID", this.estabelecimento.id);
          if (!this._.some(this.api.procedimentosCadastrados, ['procedimentoId', item.id])) {
            this.api.procedimentosCadastrados.push({
              estabelecimentoId:  this.estabelecimento.id,
              procedimentoId: item.id,
              procedimento: {
                ...item,
                // codigo: item.codigo,
                // cota: 0,
                // cotaExecutor: 0,
                // descricao: item.descricao,
                // cotaTotal: item.cotaTotal
              }
            })
          } else {
            this.$swal({
              title: "Atenção!",
              text: 'Item já cadastrado para este estabelecimento!',
              icon: 'warning',
            })
          }
        })
        this.clearSelectionTab1()
        this.estadoBotao = true
      },
      onRemover () {
        this.$swal({
          title: "Atenção",
          text: `Tem certeza que deseja remover o procedimento?`,
          icon: 'warning',
          showCloseButton: true,
          confirmButtonText: "Sim",
          showCancelButton: true,
          cancelButtonText: "Cancelar",
        }).then(async (result) => {
          if (result.isConfirmed) {
            this.tempSelect2.forEach(item => {
              this.removeDoArray(this.api.procedimentosCadastrados, item)
            })

            let arrayDeletados = []
            this.tempSelect2.forEach(item => {
              // console.log('item', item)
              arrayDeletados.push({
                procedimentoId: item.procedimentoId,
                estabelecimentoId: item.estabelecimentoId
              })
            })

            _api.estabelecimentos.deleteEstabelecimentoProcedimentos(arrayDeletados).then(res => {
              this.loading.salvar = false
              this.estadoBotao = false
            })


            this.$swal({
              title: "Sucesso!",
              text: 'Procedimentos removidos e salvos com sucesso!',
              icon: 'success',
            })
          }
        }).catch((result) => {

        })

      },
      onVoltar () {
        this.$router.push({ name: 'Estabelecimentos' })
      },
      clearSelectionTab1 () {
        this.$refs.tab1.clearSelection()
      },
      toggleSelectionTab1 () {
        // para ser adicionado em um botão separado 'selecionar tudo'
        this.procedimentos.forEach(row => {
          this.$refs.tab1.toggleRowSelection(row)
        })
      },
      estabelecimentoProcedimentoAdd (formData) {
        this.loading.salvar = true
        this.$refs[formData].validate(valid => {
          let aSerAdicionado
          var estabelecimentoProcedimentos = []
          if (valid) {
            aSerAdicionado = this.api.procedimentosCadastrados.filter((item) => item.jaCadastrado !== true)
            aSerAdicionado.forEach(item => {
              estabelecimentoProcedimentos.push({
                procedimentoId: item.procedimento.id,
                estabelecimentoId: this.estabelecimento.id,
                cota: 0,
                cotaExecutor: 0
              })
            })
            _api.estabelecimentos.postEstabelecimentoProcedimentos(estabelecimentoProcedimentos).then(res => {
                if (res.status === 201) this.getEstabelecimentoProcedimentos()
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
