<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
    <el-card class="box-card box-card--main-card">

      <el-row>
        <el-col :span="14">
          <h2 class="box-card--h2">Informações Úteis</h2>
        </el-col>

        <el-col :span="10" class="text-right">
          <el-form :inline="true">
            <el-form-item>
              <el-button v-if="!listando" style="margin-right: -10px" icon="fas fa-plus" type="success" @click="listando = !listando"> Novo</el-button>
              <el-button v-if="listando" style="margin-right: -10px" icon="fas fa-undo-alt" type="warning" @click="listando = !listando"> Voltar</el-button>
            </el-form-item>
          </el-form>
        </el-col>
      </el-row>


      <el-row v-show="!listando && api.informacoes.length > 0">
        <el-col :span="24">
          <el-table ref="tabela" :data="api.informacoes"
                    highlight-current-row border
                    v-loading.body="loading.informacoes"
                    class="table--informacoes table--row-click">

            <el-table-column label="Título" prop="titulo" align="center" width="auto" sortable />
            <el-table-column label="Conteúdo" prop="descricao" align="center" width="auto" sortable />
            <el-table-column label="Data De Envio" prop="dataCadastro" align="center" width="auto" sortable>
              <template slot-scope="scope">
                <span>{{ moment(scope.row.dataCadastro).format('DD/MM/YYYY') }} </span>
              </template>
            </el-table-column>

            <el-table-column width="140" fixed="right" align="center">
              <template slot-scope="scope">
                <el-button size="small" type="danger" @click="onClickExcluir(scope.row)">Excluir</el-button>
              </template>
            </el-table-column>

          </el-table>
        </el-col>
      </el-row>

      <el-row>

        <el-col :span="14" v-if="listando">
          <h3>Título</h3>
          <el-input type="textarea"
                    placeholder="Digite aqui o título"
                    v-model="params.titulo"
                    style="margin-bottom:10px;">
          </el-input>

          <h4>Descrição</h4>
          <el-input type="textarea"
                    placeholder="Digite aqui o conteúdo da notificação"
                    maxlength="4000"
                    :autosize="{ minRows: 2, maxRows: 4}"
                    v-model="params.descricao"
                    style="margin-bottom:10px;">
          </el-input>

          <!--<h4>Selecionar pacientes:</h4>
          <el-select filterable multiple clearable placeholder="Todos..." v-model="params.individuoIds"
                     v-loading.body="loading.individuos">
            <el-option v-for="option in api.pacientes" :value="option.id" :label="option.nomeCompleto" :key="option.id" />
          </el-select>-->

          <el-button v-loading.body="loading.informacoes" v-if="listando" style="margin-top:10px;" type="success" @click="onEnviar()">Enviar</el-button>

        </el-col>

      </el-row>
    </el-card>


  </el-col>
</template>

<script>
  import _api from '../../../api'
  import moment from 'moment'
  moment.locale('pt-br')

  export default {
    name: 'Informacao',
    data () {
      return {
        visible: false,
        listando: false,
        textarea: '',
        titulo: '',
        descricao: '',
        params: {
          skip: 1,
          take: 100,
          sort: 'titulo ASC',
          individuoId: null,
          ativo: 1
        },
        paramsIndividuos: {
          skip: 1,
          take: 100,
          sort: 'Nome ASC'
        },
        api: {
          informacoes: [],
          pacientes: []

        },
        loading: {
          notificacao: false,
          individuos: false
        }
      }
    },
    async mounted () {
      await this.getInformacoes()
      await this.getPacientes()
    },
    methods: {
      async getInformacoes () {
        let { data, paginacao, status } = await _api.informacao.get(this.params)
        /*console.log('informacoes', data)*/
        if (status === 502) this.loading.informacoes = false
        this.api.informacoes = (status === 200) ? data : []
        this.loading.informacoes = false
      },
      async onEnviar () {
        this.loading.informacoes = true
        _api.informacao.post(this.params).then(res => {
          if (res.status === 201) {
            this.$swal({
              title: 'Sucesso!',
              text: 'A informação foi cadastrada!',
              icon: 'success'
            })
            
            this.loading.informacoes = false
            this.onLimpar()
            this.listando = false
            this.getInformacoes()
          } else {
            this.$swal({
              title: 'Erro!',
              text: 'Ocorreu um erro ao cadastrar informação!',
              icon: 'error'
            })
            this.loading.informacoes = false
          }
        })
        // document.location.reload(true);
      },
      onLimpar () {
        this.params.titulo = null
        this.params.descricao = null
        this.params.individuoIds = []
      },
      async getPacientes () {
        this.loading.individuos = true
        let { data, paginacao, status } = await _api.individuos.getAll(this.paramsIndividuos)
        if (status === 502) this.loading.individuos = false
        this.api.pacientes = (status === 200) ? data : []
        if (this.api.pacientes.length > 0) {
          this.api.pacientes.unshift({ id: null, nomeCompleto: 'Todos' })
        }

        this.loading.individuos = false
      },

      async onClickExcluir (row) {
        this.$confirm('Tem certeza que deseja excluir essa Informação?', 'Warning', {
          confirmButtonText: 'OK',
          cancelButtonText: 'Cancelar',
          type: 'warning'
        }).then(async () => {
          await this.onExcluir(row)
          await this.getInformacoes()
        }).catch(() => {
        })
      },
      async onExcluir (row) {
        this.loading.notificacao = true
        row.ativo = false
        console.log('debug', row);
        _api.informacao.put(row).then(res => {
          // console.log('res', res)
          if (res.status === 204) {
            this.$swal({
              title: 'Sucesso!',
              text: 'A Informação foi excluída com sucesso!',
              icon: 'success'
            })
            this.loading.notificacao = false
            this.listando = false
          } else {
            this.$swal({
              title: 'Erro!',
              text: 'Ocorreu um erro ao excluir a Informação',
              icon: 'error'
            })
            this.loading.notificacao = false
          }
        })
        await this.getInformacoes()
      }
    }
  }
</script>
<style>
</style>
