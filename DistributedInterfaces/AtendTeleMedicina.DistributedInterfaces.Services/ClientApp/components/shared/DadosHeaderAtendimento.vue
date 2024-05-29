<template>
  <div class="container-header" v-if="agendamento.individuo">

    <div>
      <div v-if="agendamento.individuo.imagem && errorIndividuoImage == false" :style="{ borderColor: getBorderPerfil(agendamento) }">
        <img alt="Imagem de perfil indisponível no momento" @error="handleIndividuoImageError" class="imagemPaciente" :style="{ borderColor: getBorderPerfil(agendamento)}" :src="`../../${agendamento.individuo.imagem}.jpg?${agendamento.individuo.dataAlteracao}`" :title="agendamento.individuo.nomeCompleto" />
      </div>

      <div v-else class="paciente__missing_photo" width="500px" :style="{ borderColor: getBorderPerfil(agendamento) }">
        <img :src="'../../assets/img/' + $store.state.app.empresa.fotoUsuario"
             :title="$store.state.app.empresa.nome"
             id="image" />
      </div>
    </div>

    <div class="container-nome-paciente">
      <h1>{{agendamento.individuo.nomeCompleto}} | {{agendamento.individuo.sexo === 0 ? ' Masculino ' : ' Feminino '}}</h1>

      <p>
        <strong>Idade:&nbsp; {{agendamento.individuo.idade.anos}} </strong> &nbsp; - &nbsp;
        <strong>CPF: &nbsp; {{agendamento.individuo.cpf != undefined ? agendamento.individuo.cpf : " Não preenchido "}} </strong> &nbsp; - &nbsp;
        <strong>Data de nascimento: &nbsp; {{agendamento.individuo.dataNascimento ? moment(agendamento.individuo.dataNascimento).format("DD/MM/YYYY") : "Não informado."}} </strong>
      </p>
    </div>

  </div>
</template>

<script>
  import moment from 'moment'
  moment.locale('pt-br')

  export default {
    name: 'DadosHeaderAtendimento',
    props: {
      agendamento: {}
    },
    data() {
      return {
        errorIndividuoImage: false,
      }
    },

    methods: {

      //RETURN BORDER
      getBorderPerfil(row) {
        if (row != undefined) {
          if (row.corStatusTriagem == 1) {
            return "#1E90FF"
          } else if (row.corStatusTriagem == 2) {
            return "#2a9d8f"
          } else if (row.corStatusTriagem == 3) {
            return "#ffca3a"
          } else if (row.corStatusTriagem == 4) {
            return "#e76f51"
          } else if (row.corStatusTriagem == 5) {
            return "#e63946"
          } else {
            return "#808080"
          }
        }
      },

      //IF IMAGE ERROR
      handleIndividuoImageError() {
        this.errorIndividuoImage = true
      },
    }
  }
</script>

<style>

  .container-header {
    display: flex;
    flex-direction: row;
    align-items: center;
  }

  .imagemPaciente {
    width: 110px;
    height: 110px;
    object-fit: cover;
    border-radius: 100px;
    border-width: 6px;
    border-style: solid;
  }

  .paciente__missing_photo {
    display: flex;
    justify-content: center;
    align-items: center;
    background-color: #f2f2f2;
    border-radius: 100px;
    border-width: 4px;
    border-style: solid;
  }


  .container-nome-paciente {
    display: flex;
    flex-direction: column;
    margin-left: 10px;
  }

    .container-nome-paciente h1 {
      display: flex;
      font-size: 20px;
    }

    .container-nome-paciente span {
      padding-left: 5px;
    }

    .container-nome-paciente h1, .container-nome-paciente span {
      vertical-align: middle;
    }

    .container-nome-paciente p {
      display: flex;
      font-size: 12px;
      margin-bottom: -18px;
    }
</style>

