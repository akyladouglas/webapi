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
                <el-form-item label="Individuo:" prop="individuo">
                  <el-input v-model="individuo.nomeCompleto" disabled />
                </el-form-item>
                <!--<el-form-item label="Estabelecimento:" prop="estabelecimento">
                  <el-row :gutter="20">
                    <el-col :span="12">
                      <el-input class="filter-item" v-model="procedimentoQuery.descricao" />
                    </el-col>
                    <el-col :span="4">
                      <el-button type="primary" icon="fas fa-search" @click="getEstabalecimentos(procedimentoQuery.descricao)"></el-button>
                    </el-col>
                  </el-row>
                </el-form-item>-->
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
                  </el-row>
                </el-form-item>

              </el-col>
              <el-col :span="24" />
              <el-col :span="12">
                <el-table key="tab1"
                          v-loading.body="loading.procedimentos"
                          border fit highlight-current-row
                          ref="tab1"
                          :data="api.procedimentos"
                          style="width: 100%"
                          @selection-change="handleSelectionChangeTab1">
                  <el-table-column type="selection" width="40" />
                  <el-table-column prop="codigo" label="Código" width="95" />
                  <el-table-column show-overflow-tooltip prop="descricao" label="Procedimento">
                  </el-table-column>
                </el-table>
                <el-row v-show="!loading.procedimentos">
                  <el-pagination @size-change="handleSizeChange"
                                 @current-change="handleCurrentChange"
                                 :current-page.sync="procedParams.page"
                                 :page-sizes="[10,25,50,100]"
                                 :page-size="procedParams.pageSize"
                                 :total="procedParams.total"
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
                <h3 class="estabelecimento__titulo--mt-negativo">Procedimentos adicionados ao Paciente:</h3>
                <el-table key='tab2'
                          height="470"
                          border fit highlight-current-row
                          ref="tab2"
                          :data="api.procedimentosCadastrados"
                          style="width: 100%"
                          class="table--expandable"
                          @selection-change="handleSelectionChangeTab2">
                  <el-table-column :selectable="rowSelectable" type="selection" width="40" />
                  <el-table-column prop="codigo" label="Código" width="95" />
                  <el-table-column show-overflow-tooltip prop="descricao" label="Procedimento" />
                </el-table>

                <el-row>
                  <el-col>
                    <el-form-item class="float-right mt-4">
                      <el-button flat icon="fas fa-undo-alt" type="warning" @click="onVoltar"> {{ $config.txt.btVoltar }}</el-button>
                      <el-button flat icon="fas fa-save" type="success" :disabled="!estadoBotao" v-loading.body="loading.salvar" @click="procedimentoIndividuoAdd('formData')"> {{ $config.txt.btSalvar }}</el-button>
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
import individuo from '../../../router/individuo'
  export default {
    name: 'PacienteAutorizacao',
    mixins: [Utils],
    data() {
      return {
        cnes: [],
        estadoBotao: false,
        loadingCnes: false,
        procedimentoQuery: {
          //tipo: null,
          descricao: null,
          codigo: null
        },
 
        formData: {},
        estabelecimento: '',
        tipoDoProcedimento: '',
        tempSelect: [],
        tempSelect2: [],
        filtroTipo: [
          { label: 'Codigo', value: 'codigo' },
          { label: 'Descrição', value: 'descricao' }
        ],

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
          procedimentos: [],
          individuos: [],
          procedimentosCadastrados: [],
          unique: []
        },
        loading: {
          procedimentos: false,
          procedimentosCadastrados: false
        },
        params: {
          skip: 1,
          take: 10,
          sort: 'Individuo.NomeCompleto ASC',
          total: 0
        },
        procedParams: {
          skip: 1,
          take: 10,
          sort: 'Procedimento.Descricao ASC',
          total: 0
        },

      }
    },
    async created() {
      if (this.$route.params.individuo === undefined) {
        this.$router.push({ name: 'AdminPaciente' })
      } else {
        this.individuo = this.$route.params.individuo
        await this.getProcedimentos()
        await this.getIndividuos()
        await this.getProcedimentoIndividuos()
      }
    },
    methods: {
      async getProcedimentos() {
        this.loading.procedimentos = true
        let { data, paginacao, status } = await _api.procedimentos.get(this.procedParams)
        //console.log('data', data)
        if (status === 502) this.loading.procedimentos = false
        this.api.procedimentos = (status === 200) ? data : []
        this.procedParams.skip = (status === 200) ? paginacao.skip : 0
        this.procedParams.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.procedimentos = false
      },
      async getIndividuos() {
        this.loading.individuos = true
        let { data, paginacao, status } = await _api.individuos.getById(this.individuo.id)
        if (status === 502) this.loading.individuos = false
        this.api.individuos = (status === 200) ? data : []
        //this.params.skip = (status === 200) ? paginacao.skip : 0
        //this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.individuos = false
      },
      async getProcedimentoIndividuos() {
       // console.log('this.api.individuos', this.api.individuos)
        for (var i = 0; i < this.api.individuos.autorizacoes.length; i++) {
         // console.log("MEU LOG", i);
          this.api.procedimentosCadastrados.push(this.api.individuos.autorizacoes[i].procedimento);
        }

        this.api.procedimentosCadastrados.forEach(i => {
          i.jaCadastrado = true
        })

        this.api.unique = [...new Set(this.api.procedimentosCadastrados)];
        this.api.procedimentosCadastrados = this.api.unique;
        //console.log("UNIQUE", uniqueChars);
      //  console.log('procedimentos cadastrados usuario api', this.api.procedimentosCadastrados)

        //alterar aqui direto funcionou
        /*this.api.procedimentosCadastrados[0].jaCadastrado = false*/
        /*console.log('teste', this.api.procedimentosCadastrados)*/

        this.loading.procedimentosCadastrados = false
      },
      handleTipoChange(val) {
        this.procParams.tipo = val
        this.getProcedimentos()
      },
      handleFilter() {
        this.getProcedimentos()
      },
      handleSizeChange(val) {
        this.procParams.pageSize = val
        this.getProcedimentos()
      },
      handleCurrentChange(val) {
        this.procedParams.skip = val
        this.getProcedimentos()
      },
      handleSelectionChangeTab1(val) {
        this.tempSelect = val
        //console.log('eita', this.tempSelect)
      },
      handleSelectionChangeTab2(val) {
      //  console.log('val dentro do handleSelectionChangeTab2: ', val)
        this.tempSelect2 = val
      },
      onVoltar() {
        this.$router.push({ name: 'AdminPaciente' })
      },
      onAdicionar() {
        if (!this.api.procedimentosCadastrados) this.api.procedimentosCadastrados = []
        //console.log('entrou aqui', this.api.procedimentosCadastrados)
        this.tempSelect.forEach(item => {
          if (!this._.some(this.api.procedimentosCadastrados, ['id', item.id])) {
            this.api.procedimentosCadastrados.push(item)
          } else {
            this.$swal({
              title: "Atenção!",
              text: 'Item já cadastrado para este procedimento!',
              icon: 'warning',
            })
          }
        })
        this.clearSelectionTab1()
        this.estadoBotao = true
      },
      //rowselectable(row) {
      //  if (!row.jacadastrado) return true
      //},
      onRemover() {
        this.$confirm('Tem certeza que deseja remover o procedimento ? ', 'ATENÇÃO!', {
          confirmButtonText: 'OK',
          cancelButtonText: 'Cancelar',
          type: 'warning',
          center: true
        }).then(() => {


        //  console.log('api.procedimentosCadastrados antes', this.api.procedimentosCadastrados)
         // console.log('this.tempSelect2 antes', this.tempSelect2)


          this.tempSelect2.forEach(item => {
          //  console.log('item', item)
            item.procedimentoId = item.id,
            item.individuoId = this.individuo.id
          //  console.log('item final', item)
            this.removeDoArray(this.api.procedimentosCadastrados, item)
            
          })
          //console.log('this.tempSelect2', this.tempSelect2)

          let arrayDeletados = []
          this.tempSelect2.forEach(item => {
           // console.log('item', item)
            arrayDeletados.push({
                procedimentoId: item.procedimentoId,
                individuoId: item.individuoId
            })
          })
         // console.log('arrayDeletados', arrayDeletados)

          _api.individuos.deleteProcedimentos(arrayDeletados).then(res => {
          //  console.log("Deletados", arrayDeletados)
          //  console.log('res', res)
            /*if (res.status === 201) this.getProcedimentoIndividuos()*/
            this.loading.salvar = false
            this.estadoBotao = false
          })


          //console.log('api.procedimentosCadastrados depois', this.api.procedimentosCadastrados)

          //this.tempSelect2.forEach(item => {
          //  var a = this.api.procedimentosCadastrados.map(procedimento => {
          //    if (procedimento.id == item.id) {
          //      procedimento.jaCadastrado = false
          //      this.removeDoArray(this.api.procedimentosCadastrados, item)
          //      return procedimento
          //    } else {
          //      console.log('ignora')
          //    }
          //  })
          //  return a
          //})
          //console.log('onRemover this.tempSelect2 depois', this.tempSelect2)
          //console.log('onRemover this.api.procedimentosCadastrados depois', this.api.procedimentosCadastrados)


          this.$swal({
            title: "Sucesso!",
            text: 'Os procedimentos foram removidos com sucesso!',
            icon: 'success',
          })

        }).catch(() => {

        });
      
        
        
      },
      clearSelectionTab1() {
        this.$refs.tab1.clearSelection()
      },
      toggleSelectionTab1() {
        // para ser adicionado em um botão separado 'selecionar tudo'
        this.api.procedimentos.forEach(row => {
          this.$refs.tab1.toggleRowSelection(row)
        })
      },
      procedimentoIndividuoAdd(formData) {
        this.loading.salvar = true
        this.$refs[formData].validate(valid => {
          let aSerAdicionado
          var procedimentoIndividuo = []
          if (valid) {
            //não mexer para não dar duplicidade de rows no banco
            procedimentoIndividuo = this.api.procedimentosCadastrados.filter((item) => item.jaCadastrado == true)
            aSerAdicionado = this.api.procedimentosCadastrados.filter((item) => item.jaCadastrado !== true)
           // console.log('aSerAdicionado antes foreach', aSerAdicionado)

            aSerAdicionado.forEach(item => {
              procedimentoIndividuo.push({
                procedimentoId: item.id,
                individuoId: this.individuo.id
              })
            })
            //console.log('aSerAdicionado', aSerAdicionado)

            //adicionando um array de objetos com os procedimentos novos adicionados
            let arrayPacientesPost = procedimentoIndividuo.filter(item => {
              if (item.jaCadastrado == true) {
                //console.log('if')
               // console.log('não faz nada')
              } else {
                return item
              }
            })
          //  console.log('arrayPacientesPost', arrayPacientesPost)

            //dando o post do array com os objetos(procedimentos) novos adicionados
            _api.individuos.postPacientes(arrayPacientesPost).then(res => {
             // console.log("PACIENTES", arrayPacientesPost)
            //  console.log('res', res)
              if (res.status === 201) {
                this.$router.push({
                  name: 'AdminPaciente',
                })
                /*this.getProcedimentoIndividuos()*/
              }
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
      },
      handleTipoDoFiltroChange(val) {
        if (val == 'descricao') {
          this.procedimentoQuery.codigo = null
          this.procedParams.codigo = null
          this.getProcedimentos()
        } else {
          this.procedimentoQuery.descricao = null
          this.procedParams.descricao = null
          this.getProcedimentos()
        }
      },
      async clickProcedimentos(val) {
        if (this.tipoDoProcedimento == 'descricao') {
          this.procedParams.descricao = val
        }
        if (this.tipoDoProcedimento == 'codigo') {
          this.procedParams.codigo = val
        }
        this.loading.procedimentos = true
        this.procedParams.skip = 1
        let { data, paginacao, status } = await _api.procedimentos.get(this.procedParams)
        if (status === 502) this.loading.procedimentos = false
        this.api.procedimentos = (status === 200) ? data : []
        this.procedParams.skip = (status === 200) ? paginacao.skip : 0
        this.procedParams.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.procedimentos = false
      },
    }
  }
</script>

<style scoped>
  .vertical-center {
    min-height: 70%; /* Fallback for browsers do NOT support vh unit */
    min-height: 70vh; /* These two lines are counted as one :-)       */
    display: flex;
    align-items: center;
  }
</style>
