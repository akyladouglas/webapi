<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">

    <el-card class="box-card box-card--main-card">

      <div v-if="currentScreen === 'TableUsuarios'">
        <el-row>
          <el-col :span="14">
            <h2 class="box-card--h2">{{$route.meta.title}}</h2>
          </el-col>
          <el-col :span="10" class="text-right">
            <el-form :inline="true">
              <el-form-item>
                <el-button style="margin-right: -10px" icon="fas fa-user-plus" type="success" @click="onClickNovo"> Novo</el-button>
              </el-form-item>
            </el-form>
          </el-col>
        </el-row>

        <el-row>
          <el-col :span="24">
            <FiltroUsuario :loading="loading.usuarios" :params="params" @emit="onEmitFromFiltro" />
          </el-col>
        </el-row>


        <el-empty v-show="api.usuarios.length === 0" description="Nenhum usuário encontrado"></el-empty>
        <el-row v-show="api.usuarios.length > 0">
          <el-col :span="24">
            <el-table ref="tabela" :data="api.usuarios"
                      highlight-current-row border
                      v-loading.body="loading.usuarios"
                      class="table--usuario table--row-click">
              <el-table-column label="Nome" prop="nome" />
              <el-table-column label="Usuário" prop="username" />
              <el-table-column label="Ativo" prop="ativo" width="150">
                <template slot-scope="scope">
                  <span v-if="scope.row.ativo == true" style="color: green; font-weight: bold" >SIM</span>
                  <span v-if="scope.row.ativo == false" style="color: red; font-weight: bold" >NÃO</span>
                </template>
              </el-table-column>
              <el-table-column label="CPF" prop="cpf" width="130" />
              <el-table-column label="Email" prop="email" />
              <el-table-column label="Estado" prop="uf" width="100" />
              <el-table-column label="Cidade" prop="cidade.nome" />
              <el-table-column header-align="center" align="right">
                <template slot-scope="scope">

                  <el-dropdown>
                    <el-button type="primary" size="small">
                      Ações <i class="el-icon-arrow-down el-icon--right"></i>
                    </el-button>

                    <el-dropdown-menu slot="dropdown">
                      <ul class="list-unstyled">
                        <li @click="onEditar(scope.row)" class="el-dropdown-menu__item"><i class="fas fa-user-edit"></i> Editar</li>
                        <li @click="onDesativar(scope.row)" v-if="!loading.usuarios" v-show="scope.row.id !== $auth.user().id && scope.row.ativo === true" class="el-dropdown-menu__item text-danger"><i class="fas fa-trash"></i> Desativar</li>
                        <li @click="onAtivar(scope.row, 'formUsuario')" v-if="!loading.usuarios" v-show="!scope.row.ativo" slot="reference" class="el-dropdown-menu__item text-success"><i class="fas fa-check-circle"></i>Ativar</li>
                        <li v-if="loading.usuarios" class="el-dropdown-menu__item text-danger"><i class="fa fa-spinner fa-spin"></i> Aguarde</li>
                      </ul>
                    </el-dropdown-menu>
                  </el-dropdown>

                </template>
              </el-table-column>
            </el-table>
          </el-col>

          <el-col :span="24">
            <el-pagination @size-change="handleSizeChange"
                           @current-change="handleCurrentChange"
                           :current-page.sync="params.skip"
                           :page-sizes="[10,25,50,100]"
                           :page-size="params.pageSize"
                           :total="params.total"
                           layout="total, sizes, prev, pager, next, jumper">
            </el-pagination>
          </el-col>
        </el-row>
      </div>

      <!-- Novo Usuario-->
      <div v-if="currentScreen === 'NovoUsuario'">
        <component v-if="currentScreen === 'NovoUsuario'" :is="'novo-usuario'" @emit="returnEmitNovoUsuario" />
      </div>

      <!--Editar Usuario-->
      <div v-if="currentScreen === 'EditarUsuario'">
        <component v-if="currentScreen === 'EditarUsuario'" :is="'editar-usuario'" :usuario="formEditar" @emit="returnEmitEditarUsuario" />
      </div>
    </el-card>
  </el-col>
</template>

<script>
  import Utils from '../../../mixins/Utils'
  import _api from '../../../api'
  import FiltroUsuario from '../../../components/shared/FiltroUsuario'
  import NovoUsuario from '../usuario/NovoUsuario'
  import EditarUsuario from '../usuario/EditarUsuario'

  export default {
    name: 'AdminUsuario',
    directives: {},
    mixins: [Utils],
    components: { FiltroUsuario, NovoUsuario, EditarUsuario },
    data () {

      return {
        currentScreen: 'TableUsuarios',

        api: {
          usuarios: [],
        },

        loading: {
          usuarios: false,
        },

        params: {
          skip: 1,
          pageSize: 10,
          sort: 'Nome ASC',
          total: 0
        },
        paramsUfs: {
          skip: 1,
          take: 30,
          sort: '+UfAbreviado',
          total: 0
        },
        paramsCidades: {
          skip: 1,
          take: 999,
          sort: 'Nome',
          total: 0,
          ufAbreviado: null
        },
        paramsEstabelecimento: {
          skip: 1,
          take: 10,
          sort: 'e.NomeFantasia ASC',
          ativo: true,
          total: 0
        }
      }
    },

    async mounted () {
      await this.getUsuarios()
    },

    methods: {
      //RETORNO DOS USUARIOS PARA TABLE
      async getUsuarios () {
        this.loading.usuarios = true
        let { data, paginacao, status } = await _api.usuarios.get(this.params)
        this.api.usuarios = (status === 200) ? data : []
        this.params.skip = (status === 200) ? paginacao.currentPage : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.usuarios = false
      },

      //SCREEN DISPLAY CONTROL
      showScreen(screen) {
        this.currentScreen = screen;
      },

      //NOVO USUARIO
      async onClickNovo () {
        this.showScreen('NovoUsuario')
      },

      //EDITAR USUARIO
      async onEditar(row) {
        delete row.senha
        if (row.ocupacaoId == 0) delete row.ocupacaoId
        this.formEditar = row
        this.showScreen('EditarUsuario')

        // CASO SEJA PRECISO NÃO DESATIVAR O ADMINISTRADOR
        //var userClaim = this.formUsuario.userClaims
        //userClaim = userClaim.filter(claim => {
        //  if (claim.claimValue == 'Administrador') {
        //    this.disabledAtivoAdmin = true
        //  } else {
        //    return userClaim
        //  }
        //})
      },

      //EVENTO DE RETORNO DO EMIT NOVO USUÁRIO
      returnEmitNovoUsuario(val) {
        if (val === 'novo-usuario-close') {
          this.showScreen('TableUsuarios')
          this.getUsuarios()
        }
      },

      //EVENTO DE RETORNO DO EMIT EDITAR USUÁRIO
      returnEmitEditarUsuario(val) {
        if (val === 'editar-usuario-close') {
          this.showScreen('TableUsuarios')
          this.getUsuarios()
        }
      },

      //EVENTO DE RETORNO DO EMIT DO FILTRO
      onEmitFromFiltro (val) {
        this.params = {
          ...this.params,
          ...val.params,
          skip: 1
        }
        this.listando = true
        this.getUsuarios()
      },

      //PAGINAÇÕES
      handleSizeChange (val) {
        this.params.take = val
        this.getUsuarios()
      },
      handleCurrentChange (val) {
        this.params.currentPage = val
        this.getUsuarios()
      },


      // Desativar Usuario:
      async onDesativar(row) {
        this.$swal({
          title: "Desativar usuário",
          text: `Você tem certeza que deseja desativar o usuário: ${row.nome}?`,
          icon: 'warning',
          showCloseButton: true,
          confirmButtonText: "Sim",
          showCancelButton: true,
          cancelButtonText: "Cancelar",
        }).then(async (result) => {
          if (result.isConfirmed) {
            await _api.usuarios.delete(row.id)
            this.getUsuarios()

            this.$swal({
              title: "Sucesso!",
              text: 'O usuário foi desativado com sucesso!',
              icon: 'success',
            })
          }
        }).catch((result) => {

        })
      },

      // Ativar Usuario:
      async onAtivar(row, form) {

        // Ativar os campos:
        row = {
          ...row,
          ativo: true,
          cadastroEditado: true,
          descricaoEditado: 'Ativar Usuario',
          usuarioEditou: this.$auth.user().id,
          senha: null
        }

        this.formUsuario = row

        _api.usuarios.put(this.formUsuario).then(res => {
          if (res.status === 200) {
            this.getUsuarios()
          }
          this.loading.usuarios = false
        })
      },
    }
  }
</script>
