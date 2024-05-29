<template>
  <div label="Interconsulta">
    <el-form :model="formInterconsulta" :rules="validacoes" labelPosition="top" ref="formInterconsulta" label-width="170px">
      <el-card shadow="always" style="margin-top: 20px">
        <el-form-item label="Avaliação" prop="avaliacao">
          <el-input v-model="formInterconsulta.avaliacao" :rows="1" type="textarea" placeholder="" autosize />
        </el-form-item>
      </el-card>
    </el-form>

    <div style="margin-top: 20px">
      <el-button type="primary" size="large" icon="el-icon-check" v-show="!loading.atendimentos" @click="submitForm('formInterconsulta')">Finalizar Interconsulta</el-button>
      <el-button v-show="loading.atendimentos" type="info" icon="fa fa-spinner fa-spin" disabled> Finalizando...</el-button>
    </div>
  </div>
</template>
<script>
  import _api from '../../../api'
  import _enums from '../../../api/Enums'
  export default {
    name: 'FichaInterconsulta',
    props: {
      agendamento: {},
    },

    data () {
      return {
        // DADOS E CONFIGS
        formInterconsulta: {},

        loading: {
          atendimentos: false
        },

        // VALIDAÇÕES
        validacoes: {
           avaliacao: [
             { required: true, message: 'Campo obrigatório', trigger: 'change' },
             { min: 3, message: 'Avaliacao não pode conter menos de 4 caracteres', trigger: 'submit' }
           ],
        },

        // PARAMETROS
        params: {
          skip: 1,
          take: 10000,
          sort: 'a.Dia DESC',
          total: 0,
          interconsulta: true,
          agendamentoId: this.agendamento.id,
          profissionalId: this.$auth.user().id,
        },
      }
    },

    async mounted() {},

    methods: {
      // FUNÇÃO DO BOTÃO DE FINALIZAR ATENDIMENTO
      async submitForm(form) {
        this.$refs[form].validate(async valid => {
          this.loading.atendimentos = true
          if (valid) {
            try {

              let { data: dataGet, status: statusGet } = await _api.interconsultas.get(this.params)

              if (statusGet === 200 && dataGet.length > 0) {
                var form = {
                  avaliacao: this.formInterconsulta.avaliacao,
                  agendamentoId: this.agendamento.id,
                  profissionalId: this.$auth.user().id,
                  participouAtendimento: true,
                  id: dataGet[0].id,
                  convidado: dataGet[0].convidado,
                  aceitou: dataGet[0].aceitou,
                }
                console.log('form', form)
                let { data: dataPut, status: statusPut } = await _api.interconsultas.putAvaliacaoMedico(form)

                if (statusPut === 204) {
                  this.$swal({
                    title: "Sucesso!",
                    text: 'Interconsulta finalizada com sucesso!',
                    icon: 'success',
                  })
                  this.$router.push({ name: 'Interconsulta' })
                }

              } else{
                console.log('error: não retornou dados da interconsulta')
              }
            } catch (e) {
              console.log('error', e)
            }
          } else {
            this.$swal({
              title: "Atenção!",
              text: 'Preencha os campos obrigatórios!',
              icon: 'warning',
            })
            this.loading.atendimentos = false
          }
          this.loading.atendimentos = false
        })
      }
    }
  }
</script>
<style scoped>
</style>
