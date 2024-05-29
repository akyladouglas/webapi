<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">

    <el-card class="box-card box-card--main-card">

      <el-row>
        <el-col :span="14">
          <h2 class="box-card--h2">{{$route.meta.title}}</h2>
        </el-col>
      </el-row>

      <el-row v-show="api.configuracao.length > 0">
        <el-col :span="24">
          <el-table ref="tabela" :data="api.configuracao"
                    highlight-current-row border
                    v-loading.body="loading.configuracao"
                    class="table--estabelecimentos table--row-click">

            <el-table-column label="Demanda Espontanea" prop="demandaEspontanea" align="center">
              <template slot-scope="scope">
                <el-switch v-model="scope.row.demandaEspontanea" active-color="#00ff00" inactive-color="#ff0000" active-text="SIM" inactive-text="NÃO" />
              </template>
            </el-table-column>
            <el-table-column label="Login com senha" prop="loginComSenha" align="center">
              <template slot-scope="scope">
                <el-switch v-model="scope.row.loginComSenha" active-color="#00ff00" inactive-color="#ff0000" active-text="SIM" inactive-text="NÃO" />
              </template>
            </el-table-column>

            <el-table-column label="Intrega com o Pec" prop="integraPec" align="center">
              <template slot-scope="scope">
                <el-switch v-model="scope.row.integraPec" active-color="#00ff00" inactive-color="#ff0000" active-text="SIM" inactive-text="NÃO" />
              </template>
            </el-table-column>

            <el-table-column label="Módulo" prop="modulo" align="center">
              <template slot-scope="scope">

                <div style="display: flex; justify-content: center">

                  <el-select filterable v-model="scope.row.modulo">
                    <el-option v-for="item in enums.moduloEnum" :value="item.id" :label="item.nome" :key="item.id" />
                  </el-select>

                </div>

              </template>
            </el-table-column>

            <el-table-column label="Tipo da consulta" prop="tipoConsulta" align="center">
              <template slot-scope="scope">

                <div style="display: flex; justify-content: center; flex-direction: row">

                  <el-select v-model="scope.row.tipoConsulta" collapse-tags multiple>
                    <el-option v-for="item in enums.tipoDaConsulta" :value="item.value" :label="item.label" :key="item.value" />
                  </el-select>

                </div>

              </template>
            </el-table-column>

            <el-table-column label="Url AtendSaude" prop="urlAtend" align="center">
              <template slot-scope="scope">

                <div style="display: flex; justify-content: center">
                  <div style="width: 80%">
                    <el-input v-model="scope.row.urlAtend" />
                  </div>
                </div>

              </template>
            </el-table-column>

            <el-table-column label="Data da última alteração" prop="dataAlteracao" align="center">
              <template slot-scope="scope">
                <span>{{moment(scope.row.dataAlteracao).format('DD/MM/YYYY HH:mm')}} por Usuário {{scope.row.usuarioAlterou.nome}}</span>
              </template>
            </el-table-column>

          </el-table>
        </el-col>
      </el-row>
      <div style="display: flex; justify-content: end; align-items: flex-end; margin-top: 10px">
        <el-button flat icon="fas fa-save" type="success"@click="onClickSalvar"> Salvar Configurações</el-button>
      </div>
    </el-card>

  </el-col>
</template>


<script>
  import Utils from '../../../mixins/Utils'
  import _api from '../../../api'
  import _enums from '../../../api/Enums'
  import { mask } from 'vue-the-mask'
  import moment from 'moment'
  moment.locale('pt-br')
  export default {
    name: 'Configuracao',
    mixins: [Utils],
    components: {  },
    directives: { mask },
    data () {

      return {
        previousTipoConsulta: null,
        enums: {
          moduloEnum: [
            { nome: 'Clinica', id: 1 },
            { nome: 'Agendamento', id: 2 },
            { nome: 'Demanda Espontanea', id: 3 },
            { nome: 'Home Care', id: 4 },
            { nome: 'Conde', id: 5 }
          ],
          tipoDaConsulta: _enums.tipoDaConsulta
        },
        api: {
          configuracao: [],
        },
        loading: {
          configuracao: false,
        },
      }
    },

    async mounted () {
      await this.getConfiguracao()
    },
    methods: {
      async getConfiguracao() {
        this.loading.configuracao = true
        let { data, status } = await _api.configuracoes.get()
        if (status === 502) this.loading.configuracao = false

        if (status === 200) {
          // Transforma a string em um array
          if (data.tipoConsulta) {
            data.tipoConsulta = data.tipoConsulta.split(',');
            this.previousTipoConsulta = data.tipoConsulta;
          }

          this.api.configuracao = [data];
          
        } else {
          this.api.configuracao = [];
        }

        this.loading.configuracao = false
      },

      async salvarConfig(row) {
        row.tipoConsulta = row.tipoConsulta.join(',');
        let { data, status } = await _api.configuracoes.put(row);
        if (status === 200) this.getConfiguracao();
      },

      async handleTipoConsultaChange(row) {
        if (row.tipoConsulta.length === 0) {
          this.$swal({
            title: "Atenção!",
            text: 'Selecione pelo menos um tipo da consulta!',
            icon: 'warning',
          })          
          row.tipoConsulta = this.previousTipoConsulta;
          return;
        }
        this.previousTipoConsulta = [...row.tipoConsulta];
        await this.salvarConfig(row);
      },

      async onClickSalvar() {
        this.handleTipoConsultaChange(this.api.configuracao[0])
      }
    }

  }
</script>
