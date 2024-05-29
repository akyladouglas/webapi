<template>

  <el-form :model="filtroParams" ref="filtroParams" :rules="filtroValidacoes"
           label-width="120px" label-position="top" class="forms--filtro">
    <div class="card">
      <div class="card-header">
        <el-row :gutter="20">
          <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="5">
            <el-form-item label="Tabela" prop="tabela">
              <el-select v-model="filtroParams.tabela" placeholder="Todos ..." clearable>
                <el-option v-for="item in enums.tabelas" :label="item.label" :value="item.value" :key="item.value" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="24" :md="12" :lg="5" :xl="5">
            <el-form-item label="Origem" prop="origem">
              <el-select v-model="filtroParams.origem" placeholder="Todos ..." clearable>
                <el-option v-for="item in enums.origens" :label="item.label" :value="item.value" :key="item.value" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
            <el-form-item label="Data Inicial" prop="dataInicial">
              <el-date-picker v-model="filtroParams.dataInicial"
                              type="date"
                              placeholder="Data Inicial"
                              format="dd/MM/yyyy">
              </el-date-picker>
            </el-form-item>
          </el-col>

          <el-col :xs="24" :sm="24" :md="12" :lg="6" :xl="6">
            <el-form-item label="Data Final" prop="dataFinal">
              <el-date-picker v-model="filtroParams.dataFinal"
                              type="date"
                              format="dd/MM/yyyy"
                              placeholder="Data Final">
              </el-date-picker>
            </el-form-item>
          </el-col>

        </el-row>

      </div>
    </div>
    <el-row :gutter="20">
      <el-col :span="24" class="text-right">
        <el-form-item class="forms--margin-xs-from-top">
          <el-button v-if="!loading" type="info" icon="fas fa-filter" @click="onClickFiltrar('filtroParams')"> Filtrar</el-button>
          <el-button v-if="loading" type="info" icon="fa fa-spinner fa-spin" disabled> Aguarde ...</el-button>
        </el-form-item>

      </el-col>


    </el-row>

  </el-form>
</template>

<script>
  import Utils from '../../mixins/Utils'
  import _enums from '../../api/Enums'
  import _api from '../../api'

  export default {
    mixins: [Utils],
    name: 'FiltroAuditoria',
    props: ['loading', 'params'],
    data () {
      return {
        filtroValidacoes: {},
        filtroParams: {},
        api: {
        },
        carregando: {
        },
        enums: {
          tabelas: _enums.tabelas,
          origens: _enums.origens
        }
      }
    },
    methods: {
      async onClickFiltrar (form) {
        this.$emit('emit', { params: this.filtroParams })
      }
    }
  }
</script>
