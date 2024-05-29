<template>
  <div>
    <span>Memed container</span>
    <button id="botaoShowPrescricao" style="margin-left: 10px">
      Dados Memed
    </button>
    <!--<div class="memed-container" id="memed-container"></div>-->
  </div>
</template>

<script>
  export default {
    name: "Integration",
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
          "data-token",
          this.token
        );
        script.setAttribute("data-container", "memed-container");
        script.src =
          "https://integrations.api.memed.com.br/v1/prescricoes/";
        script.onload = function (data) {
          data.path[3].defaultView.MdSinapsePrescricao.event.add(
            "core:moduleInit",
            function moduleInitHandler(module) {
               console.log(module);
              if (module.name === "plataforma.prescricao") {
                document
                  .getElementById("botaoShowPrescricao")
                  .addEventListener("click", function () {
                    data.path[3].defaultView.MdHub.command
                      .send("plataforma.prescricao", {
                        
                      })
                      .then(function () {
                        data.path[3].defaultView.MdHub.module.show("plataforma.prescricao");
                        data.path[3].defaultView.MdHub.command.send('plataforma.prescricao', {
                          
                        });
                      });
                  });
              }
            }
          );
        };
        
      },
    },
    mounted() {
      this.initMemed();
    },
  };
</script>


<style scoped>
  .memed-container {
    margin: 0 auto;
    border: 2px solid black;
    /* Largura mínima 820px */
    width: 820px;
    height: 710px;
  }
</style>

