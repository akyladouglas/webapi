<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">

    <el-card class="box-card box-card--main-card">

      <el-row>
        <el-col :span="14">
          <h2 class="box-card--h2">{{$route.meta.title}}</h2>
        </el-col>
      </el-row>

      <el-row>
        <el-form :model="formUsuario" status-icon :rules="validacoes" ref="formUsuario" label-width="120px" label-position="top" class="forms--usuario">
          <el-row :gutter="20">
            <el-col :xs="24" :sm="24" :md="12" :lg="8" :xl="6">
              <el-form-item label="Nome" prop="nome">
                <el-input v-model="formUsuario.nome" :disabled="mxHasAccess('Médico', 'Admin' )" />
              </el-form-item>
            </el-col>
            <el-col :xs="2" :sm="24" :md="12" :lg="4" :xl="2">
              <el-form-item label="Usuário" prop="username">
                <el-input v-model="formUsuario.username" disabled />
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
              <el-form-item label="Senha" v-if="!mxHasAccess('Medico')" class="forms--checkbox-right" prop="senha">
                <el-checkbox v-model="alterarSenha" @change="onAlterarSenha" :disabled="mxHasAccess('Médico')">Alterar Senha</el-checkbox>
                <el-input v-model="formUsuario.senha" type="password" :disabled="!alterarSenha" minlength="8" maxlength="16"/>
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
              <el-form-item label="Confirmação" v-if="!mxHasAccess('Medico')" prop="senhaConfirmacao">
                <el-input v-model="formUsuario.senhaConfirmacao" type="password" :disabled="!alterarSenha" minlength="8" maxlength="16"/>
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="3">
              <el-form-item label="CPF" prop="cpf" v-if="!mxHasAccess('Admin')">
                <el-input v-model="formUsuario.cpf" masked="true" v-mask="'###.###.###-##'" disabled />
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="4">
              <el-form-item label="CNS" prop="cns" v-if="!mxHasAccess('Admin')">
                <el-input v-model="formUsuario.cns" disabled />
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="4">
              <el-form-item label="CRM" prop="crm" v-if="!mxHasAccess('Admin')">
                <el-input v-model="formUsuario.crm" disabled />
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="3">
              <el-form-item label="Data De Nascimento" prop="dataNascimento" v-if="!mxHasAccess('Admin')">
                <el-input v-model="formUsuario.dataNascimento" disabled />
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="2">
              <el-form-item label="Sexo" prop="sexo" v-if="!mxHasAccess('Admin')">
                <el-input v-model="formUsuario.sexoIndividuo" disabled />
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="3">
              <el-form-item label="Telefone" prop="telefone" v-if="!mxHasAccess('Admin')">
                <el-input v-model="formUsuario.telefone" disabled />
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="9" :xl="5">
              <el-form-item label="Email" prop="email">
                <el-input v-model="formUsuario.email" :disabled="mxHasAccess('Médico', 'Admin' )" />
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="2">
              <el-form-item label="Estado" prop="uf">
                <el-select filterable placeholder="Selecione o Estado" v-model="formUsuario.uf"
                           v-loading.body="loading.ufs" disabled @change="onSelectUf">
                  <el-option v-for="option in api.ufs" :value="option.ufAbreviado" :label="option.ufExtenso" :key="option.ufAbreviado" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="3">
              <el-form-item label="Cidade" prop="cidade">
                <el-select filterable placeholder="Selecione a Cidade" v-model="formUsuario.cidade"
                           v-loading.body="loading.cidades" disabled>
                  <el-option v-for="option in api.cidades" :value="option.id" :label="option.nome" :key="option.id" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="6">
              <el-form-item label="Logradouro" prop="logradouro" v-if="!mxHasAccess('Admin')">
                <el-input v-model="formUsuario.logradouro" disabled />
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="4">
              <el-form-item label="Bairro" prop="bairro" v-if="!mxHasAccess('Admin')">
                <el-input v-model="formUsuario.logradouroBairro" disabled />
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="2">
              <el-form-item label="Número" prop="numero" v-if="!mxHasAccess('Admin')">
                <el-input v-model="formUsuario.logradouroNumero" disabled />
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="2">
              <el-form-item label="CEP" prop="cep" v-if="!mxHasAccess('Admin')">
                <el-input v-model="formUsuario.logradouroCep" disabled />
              </el-form-item>
            </el-col>

          </el-row>

          <el-row :gutter="20">
            <el-col :xs="24">
              <el-form-item>
                <el-button flat icon="fas fa-save" v-if="!mxHasAccess('Medico')" type="success" @click="onClickSalvar('formUsuario')" v-loading.body="loading.usuario"> Salvar</el-button>
                <!--<el-button flat icon="fas fa-undo-alt" v-if="!mxHasAccess('Medico')" type="warning" @click="$router.go(-1)" :disabled="loading.usuario"> Cancelar</el-button>-->
                <el-button flat icon="fas fa-undo-alt" v-if="mxHasAccess('Admin')" type="warning" @click="goHome" :disabled="loading.usuario"> Voltar</el-button>
              </el-form-item>
            </el-col>
          </el-row>

        </el-form>
      </el-row>
    </el-card>

  </el-col>
</template>

<script>
  import Utils from '../../mixins/Utils'
  import jQuery from 'jquery'
  import { mask } from 'vue-the-mask'
  import _api from '../../api'
  import _enums from '../../api/Enums'
  import moment from 'moment'
  export default {
    name: 'AdminPerfil',
    directives: { mask },
    mixins: [Utils],
    data() {
      var validaSenha = (rule, value, callback) => {
        if (value.length > 6) {
          callback(new Error('A senha deve ter mais que 7(sete) caracteres'))
        } else {
          if (this.formUsuario.senhaConfirmacao !== '') {
            this.$refs.formUsuario.validateField('senhaConfirmacao')
          }
          callback()
        }
      }
      var validaSenhaConfirmacao = (rule, value, callback) => {
        if (value === '') {
          callback(new Error('Digite a confirmação da senha'))
        } else if (value !== this.formUsuario.senha) {
          callback(new Error('Senha não confere'))
        } else {
          callback()
        }
      }
      return {
        cidadeUsuarioLogado: null,
        profissional: false,
        isDisabled: false,
        alterarSenha: false,
        metodo: 'POST',
        listando: true,
        formUsuario: {},
        validacoes: {
          nome: [
            { required: true, message: 'Campo obrigatório', trigger: 'submit' },
            { min: 3, max: 255, message: 'Nome não pode conter menos de 4 e mais que 255 caracteres', trigger: 'submit' }
          ],
          senha: [
            { validator: validaSenha, trigger: ['submit'] },
            { required: true, message: 'A senha deve ter mais que 7(sete) caracteres', trigger: ['blur', 'submit'] }
          ],
          senhaConfirmacao: [{ validator: validaSenhaConfirmacao, trigger: ['blur', 'submit'] }],
          email: [
            { required: false, message: 'Campo Obrigatório', trigger: ['submit'] },
            { type: 'email', message: 'Email inválido', trigger: ['submit'] }
          ],
        },
        api: {
          usuario: [],
          ufs: [],
          cidades: []
        },
        storeSenha: '',
        loading: {
          usuario: false,
          ufs: false,
          cidades: false
        },
        params: {
          skip: 1,
          pageSize: 10,
          sort: 'Nome ASC',
          total: 0,
          ufs: {
            skip: 1,
            take: 30,
            sort: '+UfAbreviado',
            total: 0
          },
          cidades: {
            skip: 1,
            take: 999,
            sort: 'Nome',
            total: 0,
            ufAbreviado: null
          }
        }
      }
    },
    async mounted() {
      await this.getUsuario()
    },
    methods: {
      onAlterarSenha() {
        if (this.alterarSenha) {
          this.formUsuario.senha = '',
            this.formUsuario.senhaConfirmacao = ''
        } else {
          this.formUsuario.senha = this.storeSenha,
            this.formUsuario.senhaConfirmacao = this.storeSenha
        }
      },
      async getUsuario() {
        if (this.$auth.user().username === 'admin') {
          this.loading.usuario = true
          await this.onSelectUf(this.$auth.user().uf)
          await this.getCidadesByUf()
          let { data, status } = await _api.usuarios.getById(this.$auth.user().id)
          //console.log('data --> ', data)
          this.formUsuario = {
            ...data,
            senhaConfirmacao: data.senha,
            cidade: this.cidadeUsuarioLogado
          }
          //console.log('this.formUsuario dentro do if', this.formUsuario)
          //this.storeSenha = data.senha.slice(0)
          this.formUsuario.senha = ''
          this.formUsuario.senhaConfirmacao = ''
          this.loading.usuario = false
        }
        else {
          //console.log('$auth.user porque não é admin', this.$auth.user())
          //this.loading.usuario = true
          await this.onSelectUf(this.$auth.user().uf)
          await this.getCidadesByUf()
          let { data, paginacao, status } = await _api.profissionais.getById(this.$auth.user().id)
          var nascimento = data.dataNascimento
          var dataNascimento = moment(nascimento, 'YYYY-MM-DD').format('DD/MM/YYYY')
          var sexo = data.sexo
          var sexoIndividuo = sexo == 0 ? 'Masculino' : 'Feminino'
          this.formUsuario = {
            ...data,
            uf: data.ufAbreviado,
            cidade: this.cidadeUsuarioLogado,
            cpf: data.cpf,
            dataNascimento: dataNascimento,
            sexoIndividuo: sexoIndividuo
          }
          this.formUsuario.senha = ''
          this.formUsuario.senhaConfirmacao = ''
         // console.log('this.formUsuario dentro do else', this.formUsuario)
          //console.log('usuario logado --> ', this.$auth.user())
        }
      },

      async getUfs() {
        this.loading.ufs = true
        let { data, paginacao, status } = await _api.ufs.get(this.params.us)
        //console.log('this.params.us --> ', this.params.us)
        //console.log('data do this.ufs->> ', data)
        this.api.ufs = (status === 200) ? data : []
        this.params.ufs.skip = (status === 200) ? paginacao.currentPage : 0
        this.params.ufs.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.ufs = false
      },
      async getCidadesByUf() {
        this.loading.cidades = true
        let { data, paginacao, status } = await _api.cidades.getById(this.$auth.user().cidadeId)
        this.ufAbreviadoMedico = data.ufAbreviado
        this.cidadeUsuarioLogado = data.nome
        this.api.cidades = (status === 200) ? data : []
        //this.params.cidades.skip = (status === 200) ? paginacao.currentPage : 0
        //this.params.cidades.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.cidades = false
      },
      async onSelectUf(val) {
        this.params.cidades.ufAbreviado = val
        //console.log('teste onSelectUf', this.params.cidades.ufAbreviado)
        this.formUsuario = {
          ...this.formUsuario,
          cidade_Id: null
        }
        await this.getCidadesByUf()
      },
      onClickSalvar(form) {
        this.loading.usuario = true
        this.$refs[form].validate((valid) => {
          if (valid) {
            
            this.formUsuario.cidade = {
                ibge: this.formUsuario.cidadeId,
                ufAbreviado: this.formUsuario.uf
            }

            _api.usuarios.put(this.formUsuario).then(res => {
              if (res.status === 200) {
                this.$router.go(-1)
                this.loading.usuario = false
              }
            }).catch((e) => console.log(e))
          } else {
            this.$swal({
              title: "Atenção!",
              text: 'Verifique o preenchimento do formulário!',
              icon: 'warning',
            })
            this.loading.usuario = false
            return false
          }
        })
      },
      goHome() {
        this.$router.push({
          name: 'Home',
        })
      }
    }
  }
</script>
