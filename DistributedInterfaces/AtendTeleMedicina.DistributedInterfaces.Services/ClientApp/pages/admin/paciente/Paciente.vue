<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">

    <el-card class="box-card box-card--main-card">

      <el-row>
        <el-col :span="14">
          <h2 class="box-card--h2">{{$route.meta.title}}</h2>
        </el-col>
        <el-col :span="10" class="text-right">
          <el-form :inline="true">
            <el-form-item>
              <!--<el-button v-if="listando" style="margin-right: -10px" icon="fas fa-plus" type="success" @click="onClickNovo"> Novo</el-button>-->
              <el-button v-if="!listando" style="margin-right: -10px" icon="fas fa-undo-alt" type="warning" @click="onListar('formIndividuo')"> Voltar</el-button>
            </el-form-item>
          </el-form>
        </el-col>
      </el-row>

      <el-row v-show="listando">
        <el-col :span="24">
          <FiltroIndividuo :loading="loading.individuos" :params="params" @emit="onEmitFromFiltro" />
        </el-col>
      </el-row>

      <el-row v-show="listando && api.individuos.length > 0">
        <el-col :span="24">
          <el-table ref="tabela" :data="api.individuos"
                                  highlight-current-row border
                                  v-loading.body="loading.individuos"
                                  class="table--profissionais table--row-click">
            <el-table-column label="CPF" prop="cpf" width="180" />
            <el-table-column label="CNS" prop="cns" width="180">
              <template slot-scope="scope">
                <span>{{ scope.row.cns ? scope.row.cns : 'Não Informado' }}</span>
              </template>
            </el-table-column>
            <el-table-column label="Nome" prop="nomeCompleto" fixed />
            <el-table-column label="Email" prop="email" />
            <el-table-column header-align="center" align="right" width="140" fixed="right">
              <template slot-scope="scope">
                <el-dropdown>
                  <el-button type="primary" size="small">
                    Ações <i class="el-icon-arrow-down el-icon--right"></i>
                  </el-button>
                  <el-dropdown-menu slot="dropdown">
                    <ul class="list-unstyled">
                      <li @click="onProcedimento(scope.row)" class="el-dropdown-menu__item">
                        <i class="fas fa-calendar-plus"></i> Procedimentos
                      </li>
                      <!--<li @click="onEditar(scope.$index, scope.row)"  class="el-dropdown-menu__item"><i class="fas fa-user-edit"></i> Editar</li>
    <el-popconfirm v-if="mxHasAccess('Administrador','GestorMunicipio')"
      popper-class="forms--dropdown--popover"
      confirmButtonText='Sim, pode desativar'
      confirmButtonType="danger"
      cancelButtonType="primary"
      cancelButtonText='Não'
      icon="el-icon-info"
      iconColor="red"
      @onConfirm="onExcluir(scope.$index, scope.row)"
      title="Você tem certeza que deseja desativar o profissional?">-->

                      <li v-if="scope.row.ativo" @click="onAtivarDesativar(scope.$index, scope.row)" slot="reference" class="el-dropdown-menu__item text-danger">
                        <i class="fas fa-trash"></i>Desativar
                      </li>
                      <li v-else @click="onAtivarDesativar(scope.$index, scope.row)" slot="reference" class="el-dropdown-menu__item text-success">
                        <i class="fas fa-check-circle"></i>Ativar
                      </li>
                      <!--</el-popconfirm>-->
                    </ul>
                  </el-dropdown-menu>
                </el-dropdown>
              </template>
            </el-table-column>
          </el-table>
        </el-col>

        <el-col :span="24" v-show="listando">
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
      </el-row>

      <el-row v-show="!listando">

        <el-row :gutter="20" v-if="erros.length > 0">
          <el-col :xs="24">
            <div id="erros" class="alert alert-danger" role="alert">
              <ul>
                <li v-for="erro in erros" :key="erro">{{erro}}</li>
              </ul>
            </div>
          </el-col>
        </el-row>

      </el-row>
   </el-card>

  </el-col>
</template>

<script>
  import Utils from '../../../mixins/Utils'
  import jQuery from 'jquery'
  import _api from '../../../api'
  import _enums from '../../../api/Enums'
  import FiltroIndividuo from '../../../components/shared/FiltroIndividuo'
  import { mask } from 'vue-the-mask'
  export default {
    name: 'AdminPaciente',
    mixins: [Utils],
    components: { FiltroIndividuo },
    directives: { mask },
    data () {
      return {
        isDisabled: false,
        isValid: true,
        metodo: 'POST',
        metodoModelo: 'PUT',
        listando: true,
        erros: [],
        formIndividuo: {},
        enums: {
          perfis: _enums.perfis,
          claims: _enums.claims
        },
        api: {
          individuos: [],
        },
        loading: {
          profissionais: false,
        },
        params: {
          skip: 1,
          take: 10,
          sort: 'i.NomeCompleto ASC',
          total: 0
        },
      }
    },
    async mounted () {
      await this.getIndividuos()
    },
    methods: {
      onEmitFromFiltro (val) {
        this.params = {
          ...this.params,
          ...val.params,
          skip: 1
        }
        this.listando = true
        this.getIndividuos()
      },
      async getIndividuos () {
        this.loading.individuos = true
       // console.log('this.params', this.params)
        let { data, paginacao, status } = await _api.individuos.getAll(this.params)
        if (status === 502) this.loading.individuos = false
        this.api.individuos = (status === 200) ? data : []
        this.params.skip = (status === 200) ? paginacao.skip : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.individuos = false
       // console.log("DATA", data);
      },
      onListar(form) {
        let i = this.api.individuos.findIndex(x => x.id === this.formIndividuo.id)
        this.$refs.tabela.setCurrentRow(this.api.individuos[i])
        this.$refs[form].resetFields()
        this.listando = true
      },

      //Desativar/Ativar Paciente:
      async onAtivarDesativar(index, row) {
        
        row.ativo = !row.ativo;

        _api.individuos.put(row).then(res => {
          if (res.status === 204 || res.status === 200) {
            console.log('res status', res)
            let i = this.api.individuos.findIndex(x => x.id === estabelecimento.id)
            this.api.individuos[i] = estabelecimento
            this.$refs.tabela.setCurrentRow(estabelecimento)
            this.onListar(form)
          }
          else {
            console.log('statusErro', res.status)

            //res.data.forEach(i => {
              //this.erros.push(i)
            //}
            //)
            jQuery('.form--estabelecimento').get(0).scrollIntoView()
          }
          this.loading.individuos = false
        })

        console.log('index', index)
        console.log('row', row)


      },
      async onClickSalvar (form) {
        this.erros = []
        this.loading.individuos = true
        this.$refs[form].validate((valid) => {
          if (valid) {
            if (this.metodo === 'POST') {
              let claims = []
              this.formUsuario.userClaims.forEach(item => {
                claims.push({
                  id: this.mxGerarGuid(),
                  claimType: item.claimType,
                  claimValue: item.claimValue
                })
              })
              this.formUsuario.userClaims = claims
              _api.individuos.post(this.formIndividuo).then(res => {
                if (res.status === 201) {
                  this.onListar(form)
                  this.getIndividuos()
                  this.loading.individuos = false
                }
                this.loading.individuos = false
              })
            } else {
              var estabelecimento = this._.clone(this.formIndividuo)

              _api.individuos.put(this.formIndividuo).then(res => {
                if (res.status === 204) {
                  let i = this.api.individuos.findIndex(x => x.id === estabelecimento.id)
                  this.api.individuos[i] = estabelecimento
                  this.$refs.tabela.setCurrentRow(estabelecimento)
                  this.onListar(form)
                } else {
                  res.data.forEach(i => {
                    this.erros.push(i)
                  })
                   jQuery('.form--estabelecimento').get(0).scrollIntoView()
                }
                this.loading.individuos = false
              })


            }
          } else {
            this.$swal({
              title: "Atenção!",
              text: 'Verifique o preenchimento do formulário!',
              icon: 'warning',
            })
            this.loading.individuos = false
            jQuery('.el-form-item__error').get(0).scrollIntoView()
            return false
          }
        })
      },
      onProcedimento (row) {
        this.$router.push({
          name: 'PacienteAutorizacao',
          params: { individuo: row }
        })
      },
      limparSelecao (rows) {
        if (rows) {
          rows.forEach(row => {
            this.$refs.tabela.toggleRowSelection(row)
          })
        } else {
          this.$refs.tabela.clearSelection()
        }
      },
      resetForm (form) {
        this.$refs[form].resetFields()
      },
      handleSizeChange (val) {
        this.params.take = val
        this.getIndividuos()
      },
      handleCurrentChange (val) {
        this.params.skip = val
        this.getIndividuos()
      },
      frontEndDateFormat: function(date) {
        		return moment(date, 'YYYY-MM-DD').format('DD/MM/YYYY');
        	},
    }
  }
</script>
