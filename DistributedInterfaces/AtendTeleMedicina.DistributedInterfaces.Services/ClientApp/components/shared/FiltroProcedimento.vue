<template>

  <el-form :model="filtroParams" ref="filtroParams" :rules="validacoes" label-width="120px" label-position="top" class="forms--filtro-estabelecimento">
    <div class="card">
    <div class="card-header">
    <el-row :gutter="20">
      <el-col :xs="24" :sm="24" :md="6" :lg="6" :xl="6">
        <el-form-item label="Tipo" prop="tipo">
          <el-select v-model="filtroParams.tipo" placeholder="Todos...">
            <el-option v-for="option in enums.tipoProcedimento" :value="option.value" :label="option.label" :key="option.value" />
          </el-select>
        </el-form-item>
      </el-col>
      <el-col :xs="24" :sm="24" :md="5" :lg="5" :xl="5">
        <el-form-item label="Código" prop="codigo">
          <el-input v-model="filtroParams.codigo" maxlength="9"/>
        </el-form-item>
      </el-col>
      <el-col :xs="24" :sm="24" :md="13" :lg="13" :xl="13">
        <el-form-item label="Descrição" prop="descricao">
          <el-input v-model="filtroParams.descricao" maxlength="50"/>
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
      var validaCodigo = (rule, value, callback) => {
        if (!value) return callback()
        if (this.mxValidaCodigo(value) === false) {
          return callback(new Error('O campo pode conter apenas letras e números'))
        } else {
          callback()
        }
      }

      var validaDescricao = (rule, value, callback) => {
        if (!value) return callback()
        if (this.mxValidaDescricao(value) === false) {
          return callback(new Error('Caracteres inseridos no campo estão invalidos'))
        } else {
          callback()
        }
      }

      return {
        filtroParams: {},
        filtroValidacoes: {},
        validacoes: {
          codigo: [
            { validator: validaCodigo, trigger: ['blur', 'change'] },
          ],
          descricao: [
            { validator: validaDescricao, trigger: ['blur', 'change'] },
          ],
        },
        enums: {
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
