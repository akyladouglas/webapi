<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">

    <el-card class="box-card box-card--main-card">

      <el-row>
        <el-row>
          <el-col :span="14">
            <h2 v-show="listando" class="box-card--h2">Pacientes</h2>
            <h2 v-show="!listando" class="box-card--h2">Cadastrar Paciente</h2>
          </el-col>
          <el-col :span="10" class="text-right">
            <el-form :inline="true">
              <el-form-item>
                <el-button v-if="!loading.usuarios" v-show="listando" style="margin-right: -10px" icon="fas fa-user-plus" type="success" @click="onClickNovo"> Novo</el-button>
                <el-button v-if="!listando" style="margin-right: -10px" icon="fas fa-undo-alt" type="warning" @click="onListar('formUsuario')"> Voltar</el-button>
              </el-form-item>
            </el-form>
          </el-col>
        </el-row>

        <el-col>
        </el-col>

        <el-row v-show="listando && api.individuos.length > 0">
          <el-col :span="24">
            <el-table ref="tabela" :data="api.individuos"
                      highlight-current-row border
                      v-loading.body="loading.individuos"
                      class="table--profissionais table--row-click">
              <el-table-column label="CPF" prop="cpf" width="180" />
              <el-table-column label="Nome" prop="nomeCompleto" fixed />
              <el-table-column label="Email" prop="email" />
            </el-table>
          </el-col>

          <el-col :span="24" v-show="listando">
            <el-pagination @size-change="handleSizeChange"
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
          <el-form :model="formIndividuo" status-icon :rules="validacoes" :disabled="isDisabled"
                   ref="formIndividuo" label-width="120px" label-position="top">
            <el-row :gutter="20">
              <el-col :xs="24" :sm="24" :md="12" :lg="8" :xl="8">
                <el-form-item label="CPF" prop="cpf">
                  <el-input v-model="formIndividuo.cpf" v-mask="'###.###.###-##'" />
                </el-form-item>
              </el-col>
              <el-col :xs="24" :sm="24" :md="12" :lg="8" :xl="8">
                <el-form-item label="CNS" prop="cns">
                  <el-input v-model="formIndividuo.cns" v-mask="'###############'" />
                </el-form-item>
              </el-col>
              <el-col :xs="24" :sm="24" :md="12" :lg="8" :xl="8">
                <el-form-item label="Nome" prop="nome">
                  <el-input v-model="formIndividuo.nome" />
                </el-form-item>
              </el-col>
              <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="5">
                <el-form-item label="Nome Social" prop="nomeSocial">
                  <el-input v-model="formIndividuo.nomeSocial" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :xs="24" :sm="24" :md="12" :lg="8" :xl="8">
                <el-form-item label="Nome Da Mãe" prop="nomeDaMae">
                  <el-input v-model="formIndividuo.nomeDaMae" />
                </el-form-item>
              </el-col>

              <el-col :xs="24" :sm="24" :md="12" :lg="8" :xl="8">
                <el-form-item label="Email" prop="email">
                  <el-input v-model="formIndividuo.email" />
                </el-form-item>
              </el-col>
              <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="5">
                <el-form-item label="Data De Nascimento" prop="dataNascimento">
                  <el-input v-model="formIndividuo.dataNascimento" v-mask="'##/##/####'" :value="`${moment(formIndividuo.dataNascimento).format('L')}`" />

                </el-form-item>
              </el-col>
              <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="5">
                <el-form-item label="Número Celular" prop="telefoneCelular">
                  <el-input v-model="formIndividuo.telefoneCelular" v-mask="'(##) #####-####'" />
                </el-form-item>
              </el-col>
              <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="5">
                <el-form-item label="Raça" prop="raca">
                  <el-select filterable placeholder="Selecione sua raça" v-model="formIndividuo.raca"
                             v-loading.body="loading.individuos">
                    <el-option v-for="option in enums.raca" :value="option.value" :label="option.label" :key="option.value" />
                  </el-select>
                </el-form-item>
              </el-col>

              <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="5">
                <el-form-item label="Sexo" prop="sexo">
                  <el-select filterable placeholder="Selecione o sexo" v-model="formIndividuo.sexo"
                             v-loading.body="loading.individuos">
                    <el-option v-for="option in enums.sexo" :value="option.value" :label="option.label" :key="option.label" />
                  </el-select>
                </el-form-item>
              </el-col>
            </el-row>

            <el-row :gutter="20">
              <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
                <el-form-item label="CEP" prop="logradouroCep">
                  <el-input v-model="formIndividuo.logradouroCep" @input="getCep" masked="true" maxlength="9" v-mask="'#####-###'" />
                </el-form-item>
              </el-col>
              <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="5">
                <el-form-item label="Estado" prop="ufAbreviado">
                  <el-select filterable placeholder="Selecione o Estado" v-model="formIndividuo.ufAbreviado"
                             v-loading.body="loading.ufs" :disabled="loading.cidades" @change="onSelectUf">
                    <el-option v-for="option in api.ufs" :value="option.ufAbreviado" :label="option.ufExtenso" :key="option.ufAbreviado" />
                  </el-select>
                </el-form-item>
              </el-col>
              <el-col :xs="24" :sm="24" :md="12" :lg="7" :xl="7">
                <el-form-item label="Cidade" prop="cidadeId">
                  <el-select filterable placeholder="Selecione a Cidade" v-model="formIndividuo.cidadeId"
                             v-loading.body="loading.cidades">
                    <el-option v-for="option in api.cidades" :value="option.ibge" :label="option.nome" :key="option.ibge" />
                  </el-select>
                </el-form-item>
              </el-col>
              <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
                <el-form-item label="Bairro" prop="logradouroBairro">
                  <el-input v-model="formIndividuo.logradouroBairro" />
                </el-form-item>
              </el-col>
            </el-row>




            <el-row :gutter="20">
              <el-col :xs="24">
                <el-form-item>
                  <el-button flat icon="fas fa-save" type="success" @click="onClickSalvar('formIndividuo')" v-loading.body="loading.usuarios"> Salvar</el-button>
                  <el-button flat icon="fas fa-undo-alt" type="warning" @click="onListar('formIndividuo')" :disabled="loading.usuarios"> Cancelar</el-button>
                  <el-button flat icon="fas fa-eraser" v-if="metodo === 'POST'" type="default" @click="resetForm('formIndividuo')" :disabled="loading.usuarios"> Limpar</el-button>
                </el-form-item>
              </el-col>
            </el-row>

          </el-form>
        </el-row>
      </el-row>
    </el-card>

  </el-col>
</template>
<script>
  import _api from '../../api'
  import _enums from '../../api/Enums'
  import { mask } from 'vue-the-mask'


  export default {
    name: 'CadastroIndividuo',
    directives: { mask },
    data() {
      var validaCpf = (rule, value, callback) => {
        if (!this.formIndividuo.cpf) return callback(new Error('Cpf Obrigatório'))
        if (this.mxValidaCPF(this.formIndividuo.cpf) === false) {
          return callback(new Error('Cpf Inválido'))
        } else {
          return callback()
        }
      }

      var validaSenha = (rule, value, callback) => {
        if (value === '') {
          callback(new Error('Digite a senha'))
        } else {
          if (this.formIndividuo.senhaConfirmacao !== '') {
            this.$refs.formIndividuo.validateField('senhaConfirmacao')
          }
        }
      }
      var validaSenhaConfirmacao = (rule, value, callback) => {
        if (value === '') {
          callback(new Error('Digite a confirmação da senha'))
        } else if (value !== this.formProfissional.senha) {
          callback(new Error('Senha não confere'))
        } else {
        }
      }
      var validacaoEmail = (field) => {
        //usuario = field.value.substring(0, field.value.indexOf("@"));
        //dominio = field.value.substring(field.value.indexOf("@") + 1, field.value.length);

        //if ((usuario.length >= 1) &&
        //  (dominio.length >= 3) &&
        //  (usuario.search("@") == -1) &&
        //  (dominio.search("@") == -1) &&
        //  (usuario.search(" ") == -1) &&
        //  (dominio.search(" ") == -1) &&
        //  (dominio.search(".") != -1) &&
        //  (dominio.indexOf(".") >= 1) &&
        //  (dominio.lastIndexOf(".") < dominio.length - 1)) {
        //  console.log('if')
        //}
        //else {
        //  console.log('else')
        //}
       // console.log('field', field)
      }
      return {
        listando: true,
        metodo: 'POST',
        formIndividuo: {},
        validacoes: {
          nome: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' },
            { min: 3, max: 255, message: 'Nome não pode conter menos de 4 e mais que 255 caracteres', trigger: 'submit' }
          ],
          nomeDaMae: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' },
            { min: 3, max: 255, message: 'Nome não pode conter menos de 4 e mais que 255 caracteres', trigger: 'submit' }
          ],
          email: [
            { required: true, validator: validacaoEmail, trigger: ['blur', 'change', 'submit'] },
            { required: true, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }
          ],
          dataNascimento: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' },
          ],
          ufAbreviado: [{ required: true, message: 'Campo obrigatório', trigger: ['blur', 'change'] }],
          cidadeId: [{ required: true, message: 'Campo obrigatório', trigger: ['blur', 'change'] }],
          logradouroCep: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' }
          ],
          senha: [{ required: true, validator: validaSenha, trigger: ['blur', 'change'] }],
          senhaConfirmacao: [{ required: true, validator: validaSenhaConfirmacao, trigger: ['blur', 'change'] }],
          logradouro: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' },
            { min: 1, max: 150, message: 'Endereço não pode conter menos de 1 e mais que 150 caracteres', trigger: 'change' }
          ],
          logradouroNumero: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' }
          ],
          logradouroBairro: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' },
            { min: 1, max: 100, message: 'Bairro não pode conter menos de 1 e mais que 100 caracteres', trigger: 'change' }
          ],
          cpf: [
            { required: true, validator: validaCpf, trigger: ['blur', 'change', 'submit'] },
            { required: true, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }
          ],
          telefoneCelular: [
            { required: true,trigger: ['blur', 'change', 'submit'] },
            { required: true, message: 'Campo obrigatório', trigger: ['blur', 'change', 'submit'] }
          ],
          raca: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' },
          ],
          sexo: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' },
          ],
        },

        api: {
          individuos: [],
          ufs: [],
          cidades: []
        },
        enums: {
          raca: _enums.racaOuCor,
          sexo: _enums.sexos
        },
        loading: {
          individuos: false,
          cidades: false
        },
        params: {
          skip: 1,
          take: 10,
          sort: 'i.NomeCompleto ASC',
          total: 0
        },
        paramsCidades: {
          skip: 1,
          take: 999,
          sort: '+Nome',
          total: 0,
          ufAbreviado: null
        },
      }
    },
    async mounted() {
      await this.getIndividuos()
    },
    methods: {
      async getIndividuos() {
        this.loading.individuos = true
        let { data, paginacao, status } = await _api.individuos.getAll(this.params)
        if (status === 502) this.loading.individuos = false
        this.api.individuos = (status === 200) ? data : []
        this.params.skip = (status === 200) ? paginacao.skip : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.individuos = false
      },
      async getCidadesByUf() {
        this.loading.cidades = true
        let { data, paginacao, status } = await _api.cidades.get(this.paramsCidades)
        this.api.cidades = (status === 200) ? data : []
        if (this.api.cidades.length === 1) {
          this.formIndividuo.cidadeId = this.api.cidades[0].ibge
        }
        this.paramsCidades.skip = (status === 200) ? paginacao.currentPage : 0
        this.paramsCidades.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.cidades = false
      },
      async onSelectUf(val) {
        this.paramsCidades.ufAbreviado = val
        this.formIndividuo = {
          ...this.formIndividuo,
          cidadeId: null
        }
        await this.getCidadesByUf()
      },
      async getCep(cep) {
        if (this.listando) return
        if (cep.length > 8) {
          //console.log('cep', cep)
          let { data, status } = await _api.auxiliar.getCep(cep)

          if (status === 200) {
            this.formIndividuo.logradouro = data.logradouro
            this.formIndividuo.logradouroBairro = data.bairro
            this.formIndividuo.ufAbreviado = data.uf
            this.paramsCidades.ibge = data.ibge
            await this.onSelectUf(data.uf)
            await this.getCidadesByUf()
          } else {
            this.paramsCidades.ibge = null
            this.formIndividuo = {
              ...this.formIndividuo,
              ufAbreviado: null,
              cidadeId: null,
              logradouro: null,
              bairro: null
            }
          }
        }
      },
      handleSizeChange(val) {
        this.params.pageSize = val
      },
      handleCurrentChange(val) {
        this.params.skip = val
        this.getIndividuos()
      },
      async onClickNovo() {
        this.listando = false
        this.metodo = 'POST'
      },
      onListar(form) {
        //let i = this.api.individuos.findIndex(x => x.id === this.formUsuario.id)
        //this.$refs.tabela.setCurrentRow(this.api.individuos[i])
        //this.$refs[form].resetFields()
        this.listando = true
      },
      async onClickSalvar(form) {
        this.erros = []
        this.loading.profissionais = true
        this.$refs[form].validate((valid) => {
          if (valid) {
            if (this.metodo === 'POST') {
              //let newCpf = this.formIndividuo.cpf.match(/\d/g).join('')
              //let sobrenomeMemed = this.formProfissional.sobrenome
              //let nomeCompleto = this.formProfissional.nome + ' ' + this.formProfissional.sobrenome
              //this.formProfissional.nome = nomeCompleto
              //this.formProfissional.cpf = newCpf

              //console.log('FORM PROFISSIONAL', this.formProfissional)

              //let claims = []
              //this.formProfissional.userClaims.forEach(item => {
              //  claims.push({
              //    id: this.mxGerarGuid(),
              //    claimType: item.claimType,
              //    claimValue: item.claimValue
              //  })
              //})
              //this.formProfissional.userClaims = claims
              //console.log('FORM', this.formUsuario)
              //_api.profissionais.post(this.formProfissional).then(res => {
              //  console.log('RES FORM ADD', res.data)

              //  let formatedCpf = res.data.cpf.match(/\d/g).join('')
              //  this.paramsAddProfissional = {
              //    id: res.data.id,
              //    nome: nomeMemed,
              //    sobrenome: sobrenomeMemed,
              //    crm: res.data.crm.replace(/[^0-9]/g, ''),
              //    uf: res.data.crm.replace(/[^a-zA-Z]+/g, '').toUpperCase(),
              //    dataNascimento: res.data.dataNascimento,
              //    cpf: newCpf
              //  }

              //  this.addProfissionalMemed(this.paramsAddProfissional)

              //  console.log('NEW OBJ', this.addProfissionalMemed)
              //  console.log('PROFISSIONAL MEMED', this.paramsAddProfissional)

              //  this.onListar(form)
              //  this.getProfissionais()
              //  this.loading.profissionais = false
              //})
            } else {
              let claims = []
              this.formProfissional.userClaims.forEach(item => {
                claims.push({
                  id: this.mxGerarGuid(),
                  claimType: item.claimType,
                  claimValue: item.claimValue
                })
              })
              this.formProfissional.userClaims = claims
              //this.formProfissional.nome = this.formProfissional.nome
              _api.profissionais.put(this.formProfissional).then(res => {
                if (res.status === 200) {
                  let i = this.api.profissionais.findIndex(x => x.id === this.formProfissional.id)
                  this.perfils = []
                  this.api.usuarios[i] = res.data
                  this.$refs.tabela.setCurrentRow(res.data)
                  this.formProfissional = {}
                  this.onListar(form)
                }
                this.onListar(form)
                this.getProfissionais()
                this.loading.profissionais = false
              })

            }

          } else {
            this.$swal({
              title: "Atenção!",
              text: 'Preencha os campos obrigatórios!',
              icon: 'warning',
            })
            this.loading.profissionais = false
            jQuery('.el-form-item__error').get(0).scrollIntoView()


            return false
          }
        })
      },
      resetForm(form) {
        this.$refs[form].resetFields()
      },
    }
  }
/*
                <Item gridArea="cpf">
                            <InputMask name="cpf" title="CPF" mask="999.999.999-99" />
                          </Item>
                          <Item gridArea="name">
                            <Input name="name" title="Nome Completo" />
                          </Item>
                          <Item gridArea="sName">
                            <Input name="sName" title="Nome Social" />
                          </Item>
                          <Item gridArea="bday">
                            <Input name="bday" title="Data Nascimento" type="date" />
                          </Item>
                          <Item gridArea="email">
                            <Input name="email" title="Email" />
                          </Item>
                          <Item gridArea="phone">
                            <InputMask name="phone" title="Telefone" mask="(99) 9 9999-9999" />
                          </Item>
                          <Item gridArea="genre">
                            <InputSelect name="genre" title="Gênero" options={genreList} />
                          </Item>
                          <Item gridArea="breed">
                            <InputSelect name="breed" title="Raça" options={breedList} />
                          </Item>
                          <Item gridArea="motherName">
                            <Input name="motherName" title="Nome da Mãe" />
                          </Item>
                          <Item gridArea="cep">
                            <InputMask
                              name="cep"
                              title="CEP"
                              mask="99.999-999"
                              onChange={({ target }) => handleCEPChange(target.value)}
                            />
                          </Item>
                          <Item gridArea="state">
                            <InputSelect name="state" title="Estado" options={UfList} />
                          </Item>
                          <Item gridArea="city">
                            <InputSelectAlternative
                              name="city"
                              title="Cidade"
                              options={cityList}
                              onInputChange={value => handleCityInputChange(value)}
                            />
                          </Item>
                          <Item gridArea="neighborhood">
                            <Input name="neighborhood" title="Bairro" />
                          </Item>
                          <Item gridArea="street">
                            <Input name="street" title="Logradouro" />
                          </Item>
                          <Item gridArea="numberAddress">
                            <Input name="numberAddress" title="Número" />
                          </Item>
                          <Item gridArea="addressComplement">
                            <Input name="addressComplement" title="Complemento" />
                          </Item>
                */
</script>
