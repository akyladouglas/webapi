<template>
  <div>
    <span>Memed container2</span>

    <div class="memed-container2" id="memed-container2">chamando</div>
  </div>
</template>

<script>
  export default {
    name: "Integration2",
    props: {},
    data: () => {
      return {
        body: document.querySelector("body"),
        token: 'eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.WzQwNDM0LCIzZTM4MmVmYWNkNzk5YTdjN2I4ZmJiMTk5ZWJjNDgzYyIsIjIwMjItMDQtMDEiLCJzaW5hcHNlLnByZXNjcmljYW8iLCJwYXJ0bmVyLjMuMzU2ODkiXQ.5wZ7KyGxJZnU08PZ6FRAGvTVOEdx8o2OLuyhpXR7hfo' // INSIRA TOKEN DO MÉDICO
      };
    },
    methods: {
      initMemed: function () {
        let script = document.createElement("script");
        script.setAttribute("type", "text/javascript");
        script.setAttribute("data-color", "#1abc9c");
        script.setAttribute(
          "token",
          this.token
        );
        script.setAttribute("data-container2", "memed-container2");
        script.src =
          "https://integracao.api.memed.com.br/v1/prescricoes";
        script.onload = function data (data) {
          data.path[3].defaultView.MdSinapsePrescricao.event.add(
            "core:moduleInit",
            function dados (moduleData) {
              console.log(moduleData);
              if (moduleData.name === "plataforma.prescricao") {
                MdHub.event.add('prescricaoImpressa', function Prescricao(prescriptionData) {
                  var info = new XMLHttpRequest();
                  info.onreadystatechange = function () {
                    if (this.readyState == 4 && this.status == 200) {
                      var dados = JSON.parse(this.responseText);
                      document.getElementById("memed-container2").innerHTML = `
                        <p> medicos_id: ${new Date(dados.cadastro)} </p>
                        <p> nome_medico: ${dados.nome_medico} </p>
                        <p> endereco_medico: ${dados.endereco_medico} </p>
                       `
                      
                    }
                  }
                })
              }
            }
          );
        };
        
      },
    },
    mounted() {
      this.initMemed();
    },
  }
</script>

<style scoped>
  .memed-container2 {
    margin: 0 auto;
    border: 2px solid black;
    /* Largura mínima 820px */
    width: 820px;
    height: 710px;
  }
</style>
