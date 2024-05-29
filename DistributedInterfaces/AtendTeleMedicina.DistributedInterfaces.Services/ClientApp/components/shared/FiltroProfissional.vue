<template>

  <el-form :model="filtroParams" ref="filtroParams" :rules="validacoes" label-width="120px" label-position="top" class="forms--filtro-estabelecimento">
    <div class="card">
    <div class="card-header">
      <el-row :gutter="20">
        <el-col :xs="24" :sm="24" :md="5" :lg="5" :xl="5">
          <el-form-item label="CNS" prop="cns">
            <el-input v-model="filtroParams.cns" maxlength="15" placeholder="Filtre pelo cns "/>
          </el-form-item>
        </el-col>
        <el-col :xs="24" :sm="24" :md="13" :lg="13" :xl="13">
          <el-form-item label="Nome" prop="nome">
            <el-input v-model="filtroParams.nome" maxlength="50" placeholder="Filtre o nome do profissional"/>
          </el-form-item>
        </el-col>
        <el-col :xs="24" :sm="24" :md="12" :lg="3" :xl="3">
          <el-form-item label="Ativo" prop="deletado">
            <el-select v-model="filtroParams.ativo" placeholder="Todos ...">
              <el-option v-for="item in enums.simNaoTodos" :label="item.label" :value="item.value" :key="item.value" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
    </div>
    </div>
    <el-row :gutter="20">
      <el-col :span="24" class="text-right">
        <el-form-item class="forms--margin-xs-from-top">
          <el-button v-if="!loading" type="info" icon="fas fa-filter"  @click="onClickFiltrar('filtroParams')"> Filtrar</el-button>
          <el-button v-if="loading"  type="info" icon="fa fa-spinner fa-spin" disabled> Aguarde ...</el-button>
        </el-form-item>
      </el-col>
    </el-row>
  </el-form>

</template>

<script>
  import Utils from '../../mixins/Utils'
  import { mask } from 'vue-the-mask'
  import _api from '../../api'
  import _enums from '../../api/Enums'

export default {
    name: 'FiltroEstabelecimento',
    props: ['loading', 'params'],
    mixins: [Utils],
    directives: { mask },
    data() {
      var validaNome = (rule, value, callback) => {
        if (!value) return callback()
        if (this.mxValidaNome(value) === false) {
          return callback(new Error('O campo pode conter apenas letras'))
        } else {
          callback()
        }
      }

      var validaCns = (rule, value, callback) => {
        if (!value) return callback()
        if (this.mxValidaCnpj(value) === false) {
          return callback(new Error('O campo pode conter apenas números'))
        } else {
          callback()
        }
      }

      return {
        filtroParams: {},
        filtroValidacoes: {},
        validacoes: {
          cns: [
            { validator: validaCns, trigger: ['blur', 'change'] },
          ],
          nome: [
            { validator: validaNome, trigger: ['blur', 'change'] },
          ],
        },
        enums: {
          
          simNaoTodos: [
            { label: 'Todos', value: null },
            ..._enums.simNao
          ],
          
          tipoProcedimento: [
            { label: 'Todos', value: null },
            ..._enums.tipoProcedimento
          ]
        },
      }
    },
    async created () {
    },
    methods: {
      async onClickFiltrar (form) {
        this.$refs[form].validate((valid) => {
          if (valid) {
            this.$emit('emit', { params: this.filtroParams })
          } else {
            this.$swal({
              title: "Atenção!",
              text: "Verifique o preenchimento dos filtros!",
              icon: 'warning',
            })
          }
        })
      },
    }
  }
</script>
