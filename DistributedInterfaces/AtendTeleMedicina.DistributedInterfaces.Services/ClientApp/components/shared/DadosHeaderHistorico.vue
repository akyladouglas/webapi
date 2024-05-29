<template>

  <el-row :gutter="20" style="margin:10px;">


    <el-col :span="4" style="display: flex; flex-direction: column; align-content: center; justify-content: center; align-items: center;">


      <div style="margin:5px;">

        <div v-if="individuoProps.imagem">
          <img alt="Imagem de perfil indisponível no momento" style="width: 110px; height: 110px; object-fit: cover; border-radius: 100%" :src="`../../${individuoProps.imagem}.jpg?${individuoProps.dataAlteracao}`" :title="individuoProps.nomeCompleto" />
        </div>
        <div v-else class="paciente__missing_photo">
          <div width="500px">
            <img :src="'../../assets/img/' + $store.state.app.empresa.fotoUsuario"
                 :title="$store.state.app.empresa.nome"
                 id="image" />
          </div>
        </div>
      </div>

      <div style="width:100%;">

        <el-descriptions title="" direction="vertical" :column="4" border>
          <el-descriptions-item v-if="formAtendimentoProps.agendamento.corStatusTriagem === 1" label="Não Urgente" label-class-name="azul" />
          <el-descriptions-item v-else-if="formAtendimentoProps.agendamento.corStatusTriagem == 2" label="Pouco Urgente" label-class-name="verde" />
          <el-descriptions-item v-else-if="formAtendimentoProps.agendamento.corStatusTriagem == 3" label="Urgente" label-class-name="amarelo" />
          <el-descriptions-item v-else="formAtendimentoProps.agendamento.corStatusTriagem == 4" label="Muito Urgente" label-class-name="laranja" />
        </el-descriptions>

        <div v-if="formAtendimentoProps.agendamento.retorno === true">
          <el-tag type="success" style="margin-top: 10px">Retorno: {{formAtendimentoProps.agendamento.retorno === true ? 'SIM' : 'NÃO'}}</el-tag> <br />
          <el-tag type="success" style="margin-top: 10px">Profissional anterior: {{profissionalAnteriorInfoProp.nome}}</el-tag> <br />
          <el-tag type="success" style="margin-top: 10px">Data: {{moment(agendamentoRetornoInfo.dia).format('DD/MM/YYYY')}}</el-tag> <br /> <br />
        </div>

      </div>
    </el-col>

    <el-col :span="10">

      <div style="display: flex; flex-direction: row; justify-content: space-between">
        <div style="display: flex; flex-direction: row; width: 50%">
          <h3>Consulta: {{moment(formAtendimentoProps.dataCadastro).format('DD/MM/YYYY')}}</h3>
        </div>
        <div style="display: flex; flex-direction: row; width: 50%; align-items: center; justify-content: space-between">
          <div>
            <h3>Inicio: {{moment(formAtendimentoProps.inicioDoAtendimento).format('DD/MM/YYYY HH:mm')}}</h3>
          </div>
          <div>
            <h3>Fim: {{moment(formAtendimentoProps.fimDoAtendimento).format('DD/MM/YYYY HH:mm')}}</h3>
          </div>          
        </div>
      </div>

      <el-descriptions border :column="1">
        <el-descriptions-item label="Nome">{{formAtendimentoProps.individuo.nomeCompleto ? formAtendimentoProps.individuo.nomeCompleto : 'Não informado' }}</el-descriptions-item>
        <el-descriptions-item label="Sexo">{{formAtendimentoProps.individuo.sexo === 0 ? 'Masculino' : 'Feminino'}}</el-descriptions-item>
        <el-descriptions-item label="CPF">{{formAtendimentoProps.individuo.cpf != undefined ? formatCPF(formAtendimentoProps.individuo.cpf) : 'Não cadastrado'}}</el-descriptions-item>
        <el-descriptions-item label="CNS">{{formAtendimentoProps.individuo.cns != undefined && formAtendimentoProps.individuo.cns != "" ? formAtendimentoProps.individuo.cns : 'Não cadastrado'}}</el-descriptions-item>
        <el-descriptions-item label="Idade">{{moment().diff(formAtendimentoProps.individuo.dataNascimento, 'years')}}</el-descriptions-item>
        <el-descriptions-item label="Nome da mãe">{{formAtendimentoProps.individuo.nomeDaMae ? formAtendimentoProps.individuo.nomeDaMae : 'Não informado'}}</el-descriptions-item>
        <el-descriptions-item label="Data de Nascimento">{{formAtendimentoProps.individuo.dataNascimento ? moment(formAtendimentoProps.individuo.dataNascimento).format('DD/MM/YYYY') : 'Não informado'}}</el-descriptions-item>
      </el-descriptions>
    </el-col>



    <el-col :span="10">
      <el-descriptions border :column="1" title="Informações administrativas" >
        <el-descriptions-item  label="Profissional">{{formAtendimentoProps.profissional.nome == null ? '' : formAtendimentoProps.profissional.nome}}</el-descriptions-item>
        <el-descriptions-item  label="Procedimento">{{formAtendimentoProps.procedimento.descricao == null ? '' : formAtendimentoProps.procedimento.descricao}}</el-descriptions-item>
        <el-descriptions-item  label="Tipo de atendimento">{{tipoAtendimento != undefined ? exibirTipoAtendimento(tipoAtendimento) : 'Erro'}}</el-descriptions-item>
        <el-descriptions-item  v-if="modalidade" label="Modalidade">{{modalidade ? `AD${modalidade}` : 'Erro ao retornar a modalidade AD'}}</el-descriptions-item>
        <el-descriptions-item  v-if="tipoAtendimento != 2" label="Conduta/Desfecho/Encaminhamento">
          {{condutaDesfechoEncaminhamento != undefined ? exibirCondutaDesfecho(condutaDesfechoEncaminhamento) : 'Erro ao retornar a conduta e desfecho'}}
        </el-descriptions-item>
        <el-descriptions-item  v-if="condicoesAvaliadas != undefined" label="Condições Avaliadas">
          {{condicoesAvaliadas != undefined ? recuperarCondicoesAvaliadas(condicoesAvaliadas) : 'Erro ao retornar as condições avaliadas'}}
        </el-descriptions-item>
        <!--<el-descriptions-item v-if="tipoAtendimento != 2" label="Procedimentos Realizados">
          {{procedimentosRealizados != undefined ? procedimentosRealizados : 'Erro ao retornar os procedimentos realizados'}}
        </el-descriptions-item>-->
      </el-descriptions>
    </el-col>



  </el-row>


</template>

<style>
  .laranja {
    background: #e76f51 !important;
    color: #FFF !important
  }

  .amarelo {
    background: #ffca3a !important;
    color: #000 !important
  }

  .verde {
    background: #2a9d8f !important;
    color: #FFF !important
  }

  .azul {
    background: dodgerblue !important;
    color: #FFF !important
  }
</style>

<script>
  import Utils from '../../mixins/Utils'
  import { mask } from 'vue-the-mask'
  import _api from '../../api'
  import _enums from '../../api/Enums'

  export default {
    name: 'DadosHeaderHistorico',
    props: {
      individuo: {},
      formAtendimento: {},
      profissionalAnteriorInfo: {},
      agendamentoRetornoInfo: {},
      tipoAtendimento: {},
      modalidade: {},
      condutaDesfechoEncaminhamento: {},
      condicoesAvaliadas: {},
      procedimentosRealizados: {}
    },
    mixins: [Utils],
    directives: { mask },
    data() {
      return {
        individuoProps: {},
        formAtendimentoProps: {},
        profissionalAnteriorInfoProp: {},
        agendamentoRetornoInfoProp: {},
        enums: {

        condicoesAvaliadasEnums: [
          ..._enums.condicoesAvaliadas
        ],
        }

      }
    },

    async created() {
      this.individuoProps = this.individuo
      this.formAtendimentoProps = this.formAtendimento
      this.profissionalAnteriorInfoProp = this.profissionalAnteriorInfo
      this.agendamentoRetornoInfoProp = this.agendamentoRetornoInfo
      console.log('this.formAtendimentoProps', this.formAtendimentoProps)
    },
    async mounted() {
    },

    methods: {
      exibirTipoAtendimento(val) {
        if (val == 0) return "Erro"
        else if (val == 1) return "Consulta agendada programada / Cuidado continuado"
        else if (val == 2) return "Consulta agendada"
        else if (val == 4) return "Escuta inicial / Orientação"
        else if (val == 5) return "Consulta no dia"
        else if (val == 6) return "Atendimento de urgência"
        else if (val == 7) return "Atendimento programado"
        else if (val == 8) return "Atendimento não programado"
        else if (val == 9) return "Visita domiciliar pós-óbito"
        else "ERRO"
      },
      exibirCondutaDesfecho(val) {
        if (val == 1) return "Alta clínica"
        else if (val == 2) return "Atenção Básica (AD1)"
        else if (val == 3) return "Alta administrativa"
        else if (val == 4) return "Serviço de urgência e emergência"
        else if (val == 5) return "Serviço de internação hospitalar"
        else if (val == 7) return "Permanência"
        else if (val == 7) return "Óbito"
        else return ""
      },
      recuperarCondicoesAvaliadas(val) {
        var array = val.split(",")

        var arrayCondicoesAvaliadas = []
        array.forEach(item => {
          var label = this.findCondicoesAvaliadasEnum(item)
          arrayCondicoesAvaliadas.push(label)
        })
        return arrayCondicoesAvaliadas.join(" / ").toString();
      },
      findCondicoesAvaliadasEnum(string) {
        var stringRetorno = ""
        this.enums.condicoesAvaliadasEnums.forEach(item => {
          if (item.value === string) {
            stringRetorno = item.label;
          } else {
            return null; // Caso a string não seja encontrada
          }
        })
        return stringRetorno
      },
      formatCPF(cpf) {
        // Remove any non-digit characters from the input
        cpf = cpf.replace(/\D/g, '');

        // Check if the CPF length is valid
        if (cpf.length !== 11) {
          return "CPF inválido. O CPF deve conter 11 dígitos.";
        }

        // Format the CPF
        return cpf.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
      }
    }
  }
</script>

<style>
  .paciente__missing_photo {
    display: flex;
    justify-content: center;
    align-items: center;
    width: 110px;
    height: 110px;
    background-color: #f2f2f2;
    border-radius: 100px;
  }
</style>
