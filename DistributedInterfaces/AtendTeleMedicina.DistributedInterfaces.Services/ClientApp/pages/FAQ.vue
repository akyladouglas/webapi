<template>

      <el-row>
        <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" style="padding: 15px">
          <div style="margin-bottom: 15px">
            <img style="width: 100vw; border-radius:5px" src="../../wwwroot/assets/img/LogoPrimary.png"/>
          </div>
          <h1>Política de privacidade</h1>
          
          <p>
+	        Caros Usuários, Em atendimento a Lei nº 13.709 de 14/08/2019 do Governo
+	        Federal, que dispõe sobre tratamentos de dados pessoais, inclusive nos meios
+	        digitais, por pessoa natural ou por pessoa jurídica de direito público ou
+	        privado, com o objetivo de proteger os direitos fundamentais de liberdade,
+	        privacidade e livre desenvolvimento da personalidade da pessoa natural, é
+	        obrigatório que a partir de agora concordem com essas novas regras e aceitem
+	        as condições de uso. Essas medidas, visam assegurar a privacidade e
+	        inviolabilidade dos dados disponibilizados. Solicitamos que leiam
+	        atentamente o conteúdo desta declaração, e somente após a concordância seu
+	        acesso será viabilizado.
+          </p>

          <h2>
            Quais são e para que servem os dados que o aplicativo Meeds utiliza?
          </h2>

          <p>
            O aplicativo Meeds coleta três tipos de informação: dados
            pessoais de cadastro; dados sobre sua situação de saúde; e dados de
            sensores biometricos. Os dados de cadastro possuem duas finalidades. A
            primeira é identificar o usuário de forma inequívoca, para que as
            informações fornecidas possam integrar a base de dados deste usuário no
            Sistema Único de Saúde. Os dados coletados com esta finalidade são os
            seguintes: nome completo; data de nascimento; sexo; CPF; nome da mãe; e
            número do cartão SUS (opcional). A segunda finalidade é poder entrar em
            contato com o cidadão. Para esta finalidade, são coletados: número de
            celular; endereço eletrônico; CEP; e endereço residencial. Os dados sobre
            a situação de saúde e sensores biometricos, por sua vez, possuem uma
            finalidade apenas. Avaliar o risco do usuário e fornecer ao profissional
            médico esses dados para auxiliar no diagnóstico. Para ambas as
            finalidades, as informações solicitadas são as de comorbidades (doenças ou
            hábitos que o paciente tenha), que são respondidas apenas uma vez; e os
            sintomas que o paciente apresenta no momento (como febre, dor no corpo,
            desconforto ou dor ao respirar etc.), que o paciente pode atualizar sempre
            que houver piora ou novos sintomas.
          </p>

          <h2>Como os dados do cidadão são coletados?</h2>

          <p>
            Os dados pessoais, de contato e de endereço são coletados no momento do
            cadastro no aplicativo. Os dados sobre a situação de saúde são coletados
            no momento em que o usuário informa sobre as comorbidades e sempre que
            atualiza o quadro de sintomas. Os dados dos sensores biometricos são
            coletados instantes antes da consulta, podendo atualizar esses dados até o
            final do atendimento.
          </p>

          <h2>Onde os dados do cidadão são armazenados?</h2>

          <p>
            O datacenter do Meeds foi desenvolvido em uma plataforma de
            serviços de computação em nuvem que está em conformidade com todos os
            protocolos, nacionais e internacionais, de sigilo e proteção de dados.
          </p>

          <h2>É preciso responder o questionário todos os dias?</h2>

          <p>
            O objetivo do questionário é manter atualizada a sua situação de saúde
            para que, em caso de um novo atendimento, os médicos possam obter esses
            dados atualizados. Portanto, o questionário deve ser respondido sempre que
            houver piora ou surgimento de novos sintomas.
          </p>

          <h2>O aplicativo funciona em todas as regiões do Brasil?</h2>

          <p>O Aplicativo Meeds está disponível para todo o país.</p>

          <h2>Como os médicos entram em contato comigo?</h2>

          <p>
            Ao realizar um agendamento, o profissional médico será notificado e no
            horário estabelecido, estará disponível em uma vídeo chamada onde o
            usuário do aplicativo poderá ingressar na mesma e ter o contato virtual
            com o médico.
          </p>
        </el-col>
      </el-row>


</template>

<script>
  export default {
    name: 'FAQ',
    data() {
      return {
        loading: false
      }
    }
  }
</script>

<style scoped>

  html {
    background: #263238 !important;
  }

  .wrapper {
    background: #263238;
    padding-top: 90px;
  }

  .faq--logo--novetech {
    margin-top: 20px;
  }

  .faq--endereco {
    margin: 0;
    font-size: 0.8rem;
    color: #fff;
    font-weight: 300;
    line-height: 120%;
    padding-top: 15px;
    padding-bottom: 15px;
  }

    .faq--endereco a:link, .faq--endereco a:visited {
      color: #fff;
    }
</style>
