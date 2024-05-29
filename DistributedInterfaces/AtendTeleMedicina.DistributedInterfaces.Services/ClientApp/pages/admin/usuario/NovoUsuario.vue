<template>
  <div>
    <el-row>
      <el-col :span="14">
        <h2 class="box-card--h2">Cadastrar Usuario</h2>
      </el-col>
      <el-col :span="10" class="text-right">
        <el-form :inline="true">
          <el-form-item>
            <el-button style="margin-right: -10px" icon="fas fa-undo-alt" type="warning" @click="onClickVoltar"> Voltar</el-button>
          </el-form-item>
        </el-form>
      </el-col>
    </el-row>


    <el-row>
      <el-form :model="formUsuario" status-icon :rules="validacoes" ref="formUsuario" label-width="120px" label-position="top" class="forms--usuario">
        <el-row :gutter="24">
          <el-col :xs="24" :sm="24" :md="24" :lg="10" :xl="10">
            <el-form-item label="Nome" prop="nome">
              <el-input v-model="formUsuario.nome" minlength="10" maxlength="50" placeholder="Digite o nome"/>
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="24" :md="24" :lg="4" :xl="4">
            <el-form-item label="Usuário" prop="username">
              <el-input v-model="formUsuario.username" minlength="10" maxlength="20" placeholder="Digite o usuário para login"/>
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="24" :md="24" :lg="5" :xl="5">
            <el-form-item label="Senha" class="forms--checkbox-right" prop="senha">
              <el-input v-model="formUsuario.senha" type="password" minlength="8" maxlength="16" placeholder="Digite a senha"/>
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="24" :md="24" :lg="5" :xl="5">
            <el-form-item label="Confirmação" prop="senhaConfirmacao">
              <el-input v-model="formUsuario.senhaConfirmacao" type="password" minlength="8" maxlength="16" placeholder="Confirme a senha digitada"/>
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="24">
          <el-col :xs="24" :sm="24" :md="24" :lg="5" :xl="5">
            <el-form-item label="CPF" prop="cpf">
              <el-input v-model="formUsuario.cpf" masked="true" v-mask="'###.###.###-##'" placeholder="Digite o CPF"/>
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="24" :md="24" :lg="5" :xl="5">
            <el-form-item label="CNS" prop="cns">
              <el-input v-model="formUsuario.cns" v-mask="'###############'" maxlength="15" placeholder="Digite o CNS"/>
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="24" :md="24" :lg="8" :xl="8">
            <el-form-item label="Email" prop="email">
              <el-input v-model="formUsuario.email" maxlength="30" placeholder="Digite o email"/>
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="24" :md="24" :lg="6" :xl="6">
            <el-form-item label="Telefone" prop="telefone">
              <el-input v-model="formUsuario.telefone" maxlength="15" v-mask="'(##)#####-####'" placeholder="Digite o número de telefone"/>
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="24">
          <el-col :xs="24" :sm="24" :md="24" :lg="5" :xl="5">
            <el-form-item label="Estado" prop="uf">
              <el-select filterable placeholder="Selecione o Estado" v-model="formUsuario.uf"
                         v-loading.body="loading.ufs" @change="onSelectUf">
                <el-option v-for="option in api.ufs" :value="option.ufAbreviado" :label="option.ufExtenso" :key="option.ufAbreviado" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="24" :md="24" :lg="5" :xl="5">
            <el-form-item label="Cidade" prop="cidadeId">
              <el-select filterable placeholder="Selecione a Cidade" v-model="formUsuario.cidadeId" v-loading.body="loading.cidades" :disabled="disabled.cidade">
                <el-option v-for="option in api.cidades" :value="option.ibge" :label="option.nome" :key="option.ibge" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="24" :md="24" :lg="7" :xl="7">
            <el-form-item label="Estabelecimento" prop="estabelecimentoId">
              <el-select filterable placeholder="Selecione o Estabelecimento" v-model="formUsuario.estabelecimentoId"
                         v-loading.body="loading.estabelecimentos">
                <el-option v-for="option in api.estabelecimentos" :value="option.id" :label="option.nomeFantasia" :key="option.id" />
              </el-select>
            </el-form-item>
          </el-col>

          <el-col :xs="24" :sm="24" :md="24" :lg="7" :xl="7">
            <el-form-item label="Ocupação" prop="ocupacaoId">
              <el-select filterable placeholder="Selecione a ocupação" v-model="formUsuario.ocupacaoId" v-loading.body="loading.cbos">
                <el-option v-for="option in api.cboEsus" :value="option.id" :label="`${option.descricao} (CBO: ${option.codigo})`" :key="option.id" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="24">
          <el-col :xs="24" :sm="24" :md="45" :lg="5" :xl="5">
            <el-form-item label="Perfis" prop="userClaims">
              <el-select v-if="mxHasAccess('Administrador')"
                         clearable
                         collapse-tags
                         multiple
                         value-key="claimValue"
                         placeholder="Selecione"
                         v-model="formUsuario.userClaims"
                         :validate-state="formUsuario.userClaims ? '' : 'error'"
                         >

                <el-option v-for="(option, i) in enums.claims"
                           :value="option"
                           :label="option.claimValue"
                           :key="`${i+1}`" />

              </el-select>
            </el-form-item>
          </el-col>


          <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="5">
            <el-form-item prop="ativo" class="forms--label-no-title">
              <el-checkbox v-model="formUsuario.ativo" label="Ativo" border style="margin-top: 2px" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="24">
          <el-col :xs="24">
            <el-form-item>
              <el-button :disabled="loading.usuarios == true" type="success" icon="fas fa-save" @click="onClickSalvar()"> Salvar</el-button>
              <el-button flat icon="fas fa-undo-alt" type="warning" @click="onClickVoltar" :disabled="loading.usuarios"> Cancelar</el-button>
              <el-button flat icon="fas fa-eraser" type="default" @click="resetForm('formUsuario')" :disabled="loading.usuarios"> Limpar</el-button>
            </el-form-item>
          </el-col>
        </el-row>

      </el-form>
    </el-row>
  </div>
</template>

<script>
  import Utils from '../../../mixins/Utils'
  import { mask } from 'vue-the-mask'
  import _api from '../../../api'
  import _enums from '../../../api/Enums'
  export default {
    name: 'NovoUsuario',
    directives: { mask },
    mixins: [Utils],
    components: { },
    data() {

      var validaSenhaConfirmacao = (rule, value, callback) => {
        if (value === undefined) {
          return callback(new Error('Campo obrigatorio'))
        } else if (value != this.formUsuario.senha) {
          return callback(new Error('A senha não confere.'))
        } else {
          return callback()
        }
      }


      var validaUserClaims = (rule, value, callback) => {
        if (value === undefined) {
          return callback(new Error('Campo obrigatorio'))
        } else if (value.length === 0) {
          return callback(new Error('Selecione pelo menos um perfil.'))
        } else {
          return callback()
        }
      }

      return {

        formUsuario: {
          ativo: true,
        },

        validacoes: {
          nome: [
            { required: true, validator: this.mxValidaNomeUsuario, trigger: ['blur', 'change', 'submit'] }
          ],
          username: [
            { required: true, validator: this.mxValidaUsernameUsuario, trigger: ['blur', 'change', 'submit'] }
          ],
          senha: [
            { required: true, validator: this.mxValidaSenhaUsuario, trigger: ['blur', 'submit'] }
          ],
          senhaConfirmacao: [
            { required: true, validator: validaSenhaConfirmacao, trigger: ['blur', 'submit'] }
          ],
          cpf: [
            { required: true, validator: this.mxValidaCpfUsuario, trigger: ['blur', 'change', 'submit'] }
          ],
          cns: [
            { required: false, validator: this.mxValidaCnsUsuario, trigger: ['blur', 'change', 'submit'] }
          ],
          email: [
            { required: true, validator: this.mxValidaEmailUsuario, trigger: ['blur', 'change', 'submit'] }
          ],
          telefone: [
            { required: true, validator: this.mxValidaTelefoneUsuario, trigger: ['blur', 'change', 'submit'] }
          ],
          uf: [
            { required: true, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }
          ],
          cidadeId: [
            { required: true, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }
          ],
          estabelecimentoId: [
            { required: true, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }
          ],
          ocupacaoId: [
            { required: true, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }
          ],
          userClaims: [
            { required: true, validator: validaUserClaims, trigger: ['submit'] }
          ],
        },

        enums: {
          claims: _enums.claims,
        },

        api: {
          usuarios: [],
          ufs: [],
          cidades: [],
          estabelecimentos: [],
        },

        loading: {
          usuarios: false,
          ufs: false,
          cidades: false,
          estabelecimentos: false,
          cbos: false,
        },

        disabled: {
          cidade: true,
        },

        paramsUfs: {
          skip: 1,
          take: 100,
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
          take: 100,
          sort: 'e.NomeFantasia ASC',
          ativo: true,
          total: 0
        },
      }
    },

    async mounted() {
      await this.getUfs()
      await this.getEstabelecimentos()
      await this.getCbos()
    },

    methods: {
      async getUfs() {
        this.loading.ufs = true
        let { data, status } = await _api.ufs.get(this.paramsUfs)
        this.api.ufs = (status === 200) ? data : []
        this.loading.ufs = false
      },

      async onSelectUf(val) {
        this.paramsCidades.ufAbreviado = val
        this.disabled.cidade = false
        delete this.formUsuario.cidadeId
        await this.getCidadesByUf()
      },

      async getCidadesByUf() {
        this.loading.cidades = true
        let { data, status } = await _api.cidades.get(this.paramsCidades)
        this.api.cidades = (status === 200) ? data : []
        this.loading.cidades = false
      },

      async getEstabelecimentos() {
        this.loading.estabelecimentos = true
        let { data, status } = await _api.estabelecimentos.get(this.paramsEstabelecimento)
        this.api.estabelecimentos = (status === 200) ? data : []
        this.loading.estabelecimentos = false
      },

      async getCbos() {
        this.loading.cbos = true
        let { data, status } = await _api.ocupacao.get()
        this.api.cboEsus = (status === 200) ? data : []
        this.loading.cbos = false
      },

      async onClickSalvar() {
        this.loading.usuarios = true
        this.$refs.formUsuario.validate(async (valid) => {
          if (valid) {
            let { data, status } = await _api.usuarios.post(this.formUsuario)
            if (status === 201) {
              this.$refs.formUsuario.resetFields()
              this.$emit('emit', 'novo-usuario-close')

            } else {
              this.$swal({
                title: "Erro!",
                text: 'Ocorreu um erro ao salvar o cadastro do usuário!',
                icon: 'error',
              })
            }
          } else {
            this.$swal({
              title: "Atenção!",
              text: 'Preencha todos os campos obrigatórios!',
              icon: 'warning',
            })
          }
        })
        this.loading.usuarios = false
      },

      onClickVoltar() {
        this.$emit('emit', 'novo-usuario-close')
      },

      resetForm(formName) {
        this.$refs[formName].resetFields()
      },
    }
  }
</script>
