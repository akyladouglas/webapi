<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
    <el-card class="box-card box-card--main-card">

      <el-row>
        <el-col :span="14">
          <h2 class="box-card--h2">Notificações</h2>
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


      <el-row v-show="!listando && api.notificacoes.length > 0">
        <el-col :span="24">
          <el-table ref="tabela" :data="api.notificacoes"
                    highlight-current-row border
                    v-loading.body="loading.notificacoes"
                    class="table--notificacoes table--row-click">

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

          <h4>Notificação</h4>
          <el-input type="textarea"
                    placeholder="Digite aqui o conteúdo da notificação"
                    maxlength="4000"
                    :autosize="{ minRows: 2, maxRows: 4}"
                    v-model="params.descricao"
                    style="margin-bottom:10px;">
          </el-input>

          <h4>Selecionar pacientes:</h4>
          <el-select filterable multiple clearable placeholder="Todos..." v-model="params.individuoIds"
                     v-loading.body="loading.individuos">
            <el-option v-for="option in api.pacientes" :value="option.id" :label="option.nomeCompleto" :key="option.id" />
          </el-select>

          <el-button v-loading.body="loading.notificacao" v-if="listando" style="margin-top:10px;" type="success" @click="onEnviar()">Enviar</el-button>

        </el-col>

      </el-row>









    </el-card>


  </el-col>
</template>

<script>
  import _api from '../../../api'
  import Hub from '../../../Hub'
  import moment from 'moment'
  moment.locale('pt-br')

  var _hub = new Hub()

  export default {
    name: 'Notificacao',
    data() {
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
          individuoId: null
        },
        paramsIndividuos: {
          skip: 1,
          take: 100,
          sort: 'Nome ASC'
        },
        api: {
          notificacoes: [],
          pacientes: []

        },
        loading: {
          notificacao: false,
          individuos: false
        }
      }
    },
    async mounted() {
      await this.getNotificacoes()
      await this.getPacientes()
    },
    methods: {
      async getNotificacoes() {
        let { data, paginacao, status } = await _api.notificacao.get(this.params)
        if (status === 502) this.loading.notificacao = false;
        this.api.notificacoes = (status === 200) ? data : []
        //console.log("get funcionu: ", this.api.notificacoes)
        this.loading.notificacao = false
      },
      async onEnviar() {
        this.loading.notificacao = true
        _api.notificacao.post(this.params).then(res => {
          //   console.log(res)
          if (res.status === 201) {
            this.$swal({
              title: "Sucesso!",
              text: 'A notificação enviada para o destino!',
              icon: 'success',
            })
            this.notifyHub('notification-update-event')
            this.loading.notificacao = false
            this.onLimpar();
            this.listando = false
            this.getNotificacoes()
          } else {
            this.$swal({
              title: "Erro!",
              text: 'Ocorreu um erro ao enviar a notificação!',
              icon: 'error',
            })
            this.loading.notificacao = false
          }
        })
        //document.location.reload(true);
      },
      onLimpar() {
        this.params.titulo = null;
        this.params.descricao = null;
        this.params.individuoIds = [];
      },
      async notifyHub(event) {
        _hub.connection.start()
          .then(() => {
            _hub.connection.invoke('SendOpenRoom', event)
          }).catch(e => {
            console.log('Error connection to Hub', e)
          })
      },
      async getPacientes() {
        this.loading.individuos = true
        let { data, paginacao, status } = await _api.individuos.getAll(this.paramsIndividuos)
        if (status === 502) this.loading.individuos = false
        this.api.pacientes = (status === 200) ? data : []
        if (this.api.pacientes.length > 0) {
          this.api.pacientes.unshift({ id: null, nomeCompleto: 'Todos' })
        }

        this.loading.individuos = false
      },

      async onClickExcluir(row) {
        this.$confirm('Tem certeza que deseja excluir essa Notificação?', 'Warning', {
          confirmButtonText: 'OK',
          cancelButtonText: 'Cancelar',
          type: 'warning'
        }).then(async () => {
          await this.onExcluir(row)
          await this.getNotificacoes()
        }).catch(() => {
        });
      },
      async onExcluir(row) {
        this.loading.notificacao = true
        row.deletado = true;
        _api.notificacao.put(row).then(res => {
          // console.log('res', res)
          if (res.status === 204) {
            this.$swal({
              title: "Sucesso!",
              text: 'A notificação foi excluída com sucesso!',
              icon: 'success',
            })
            this.loading.notificacao = false
            this.listando = false
          } else {
            this.$swal({
              title: "Erro!",
              text: 'Ocorreu um erro ao excluir a notificação',
              icon: 'error',
            })
            this.loading.notificacao = false
          }
        })
        await this.getNotificacoes()
      },
    }
  }
</script>
<style>
</style>
