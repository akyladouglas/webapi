<template>
  <section class="wrap-login content animated fadeIn">
    <div class="wrapper">

      <div class="login-form-container" v-if="$auth.ready()">
        <form class="login-form">
          <div class="login-logo-meeds-container">
            <img :src="'../../assets/img/' + $store.state.app.empresa.logo2"
                 class="img-responsive center-block"
                 :title="$store.state.app.empresa.nome" />
          </div>

          <div class="form-login">
            <label>Usuário</label>
            <input class="form-control" v-model="formData.username" placeholder="Digite seu usuário" type="text" maxlength="15" autofocus autocomplete="off" />
          </div>
          <div class="form-login">
            <label>Senha</label>
            <input class="form-control" v-model="formData.password" placeholder="Digite sua senha" type="password" maxlength="15" required autocomplete="off" />
          </div>

          <div class="form-infos">
            <div class="form-infos-items">
              <el-button class="form-infos-button" @click="redirectLoginPaciente()" type="primary" size="medium">
                <strong>Login Paciente</strong>
              </el-button>
            </div>
            <div class="form-infos-items">
              <el-button class="form-infos-button" @click="onResetSenha()" type="primary" size="medium">
                Esqueci a Senha
              </el-button>
            </div>
          </div>

          <div class="form-radio-button" v-show="screenWidth > 400">
            <el-radio-group fill="#008d95" v-model="formData.scope">
              <el-radio-button label="Profissional">Profissional</el-radio-button>
              <el-radio-button label="Usuario">Usuario</el-radio-button>
            </el-radio-group>
          </div>

          <div class="form-radio-button-mobile" v-show="screenWidth <= 400">
            <el-radio v-model="formData.scope" label="Profissional" border>Profissional</el-radio>
            <el-radio v-model="formData.scope" label="Usuario" border>Usuário</el-radio>
          </div>

          <div class="form-termo">
            <input type="checkbox" id="accept" v-model="user.accept" @change="checkBox()"><el-button class="login__bt-termo" @click="visualizarTermos()" type="primary" size="medium">Termo de Responsabilidade</el-button></input>
          </div>

          <div v-if="erro">
            <div class="error">{{ this.erro }}</div>
          </div>


        </form>
        <div class="login--botao">
          <el-button class="login-button" type="primary" @click="login()">{{ !loading ? 'Entrar' : 'Aguarde ...' }}</el-button>
        </div>
      </div>

      <footer style="width: 100%">
        <div class="novetech" style=" display: flex; flex-direction: row-reverse; justify-content: center ">
          <div class="l1">
            <img :src="'../../assets/img/logo-footer-novetech.png'"
                 class="img-responsive login--logo--novetech"
                 :title="'Novetech - Soluções Tecnológicas'" />
            <address class="login--endereco">
              <a href="http://www.novetech.com.br" target="_blank">www.novetech.com.br</a> <br />
              contato@novetech.com.br <br />
            </address>
          </div>
          <div class="l2">
            <img :src="'../../assets/img/logo-footer-novetech-p.png'"
                 class="img-responsive center-block login--logo--p"
                 :title="'Novetech - Soluções Tecnológicas'" />
          </div>
          <br clear="all" />
          <br clear="all" />
        </div>
      </footer>
      <footer class="navbar-fixed-bottom footer--empresa">
        <a>
          <img :src="'../../assets/img/logo-footer-novetech-p.png'"
               class="img-responsive center-block login--logo--novetech"
               :title="'Novetech - Soluções Tecnológicas'" /> Meeds - v2.0.5 - Novetech Soluções Tecnológicas
        </a>
      </footer>
    </div>

    <el-dialog title="" class="modal--termos" :visible.sync="showModalAceite" width="70%" :close-on-click-modal="false" :close-on-press-escape="false">
      <VuePerfectScrollbar class="scroll-area" :settings="scrollSettings" key="scrol-atendimento">
        <p align="center"> <strong>TERMO DE RESPONSABILIDADE, SIGILO E CONFIDENCIALIDADE - TELECONSULTA </strong> </p>
        <p>O presente termo firma o compromisso de responsabilidade, de sigilo e confidencialidade que assume o signatário deste quando da utilização do sistema MEEDS Telemedicina de propriedade da empresa Novetech Soluções Tecnológica, com sede em João Pessoa - PB. Seguem abaixo as disposições e condições que regem este termo: </p>
        <p>
          <strong> Cláusula 1ª. -  </strong>Para fins do presente termo entende-se por: - usuário: todo aquele que opera o sistema e cujo perfil de acesso define as restrições em face das informações cadastradas e as funcionalidades dos sistemas. Estes perfis podem ser: médico, recepcionista, triagem, ou administrador do sistema (usuário que parametriza o sistema sem acesso as informações sigilosas).
        </p>
        <p>
          <strong>Informação confidencial: </strong> toda e qualquer informação, seja verbal, escrita ou por meio digital, relativa ao prontuário do paciente, dados técnicos, pessoais ou não, banco de dados, metodologias, entre outros disponibilizados aos usuários ou aos quais estes tenham acesso em razão de sua relação empregatícia, de prestação de serviço ou de qualquer outra natureza. - RES - Registro Eletrônico em Saúde é um sistema que contempla todas as informações de saúde, clínicas e administrativas de um beneficiário. Tem como objetivo apoiar o ato médico no atendimento ao paciente e a operadora na gestão, promoção da saúde, prevenção de riscos e doenças. Neste contexto, existem vários sistemas que compõe o RES e entre eles estão os sistemas PEP – Prontuário Eletrônico do Paciente e SADTISS – Solicitador, Executor e Autorizador de consultas, procedimentos diagnósticos, terapêuticos, cirúrgicos e internações, que são objetos deste termo.
        </p>
        <p>
          <strong> Cláusula 2ª. -  </strong>A cada usuário será atribuída uma senha de acesso para um perfil, a qual é sigilosa, de uso pessoal e intransferível. A responsabilidade pelo uso e guarda é atribuída integralmente ao seu titular, inclusive pelos danos advindos da sua revelação indevida.
        </p>
        <p>
          <strong> 2.1 –  </strong>O usuário de perfil do administrador é o responsável por autorizar e bloquear o acesso aos sistemas dos usuários subordinados a ele, inclusive em funcionalidades exclusivas de médicos, assumindo assim toda a responsabilidade pelo uso inadequado da ferramenta.
        </p>
        <p>
          <strong> Cláusula 3ª. - </strong>A Novetech Soluções Tecnológicas não tolerará, em hipótese alguma, divulgação das informações confidenciais.
        </p>
        <p> <strong> Cláusula 4ª. - </strong> Aos usuários do RES, fica desde já proibido:</p>

        <p>
          <strong> 4.1 –  </strong>
          Utilizar informações internas em benefício próprio ou de terceiros, para finalidades outras que não as permitidas pela legislação;
        </p>

        <p>
          <strong> 4.2 -  </strong>
          Divulgar quaisquer informações relativas ao RES, aos seus clientes ou terceiros, ressalvando-se o disposto na legislação;
        </p>

        <p>
          <strong> 4.3 -  </strong>
          Divulgar quaisquer informações referentes aos projetos de informática, equipamentos, sistemas operacionais, softwares, sistemas de controles e outros aqui envolvidos;
        </p>

        <p>
          <strong> 4.4 -  </strong>
          Falar em nome do sistema RES ou da Novetech Soluções Tecnológicas sem a aquiescência expressa da administração da empresa;
        </p>

        <p>
          <strong> 4.5 -  </strong>
          Divulgar quaisquer informações disponíveis através do RES, sua estratégia, processos, entre outros, sem a devida autorização;
        </p>

        <p>
          <strong> 4.6 -  </strong>
          Reproduzirem no todo ou em parte, documentos, softwares ou qualquer outra informação, para uso próprio ou de terceiros, seja dentro ou fora do estabelecimento de trabalho;
        </p>

        <p>
          <strong> 4.7 -  </strong>
          Fazer transitar por qualquer meio, quaisquer informações que não sejam de domínio público, sem consentimento da administração ou fora dos procedimentos estabelecidos;
        </p>

        <p>
          <strong> 4.8 -  </strong>
          Fornecer a senha de acesso ao RES a terceiros ou não observar a devida cautela na sua guarda e sigilo.
        </p>
        <p> <strong> Cláusula 5ª. - </strong> O descumprimento do presente Termo, tendo sido previamente aceito, sujeitará o infrator às sanções na esfera administrativa, com prejuízo de eventuais ações nas esferas civil e criminal, em conformidade com a Lei de Proteção de Dados em vigor (LGPD), respondendo pela extensão dos danos direta ou indiretamente causados à Novetech Soluções Tecnológicas, seus clientes, parceiros e terceiros, inclusive por lucros cessantes, danos materiais e/ou morais, mesmo que a divulgação das informações confidenciais ocorram após a rescisão do contrato de trabalho de colaborador ou do contrato de prestação de serviços do(s) envolvido(s), ou ainda, após a utilização do sistema. </p>
        <p> <strong> Cláusula 6ª. - </strong> Os usuários médicos declaram-se cientes que todas as normas técnicas concernentes à digitalização e uso dos sistemas informatizados para a guarda e manuseio dos documentos dos prontuários dos pacientes, autorizando a eliminação do papel e a troca de informação identificada em saúde, estão definidas na resolução CFM nº 1821/2007 que fica fazendo parte integrante deste termo.  </p>
        <p> <strong> Cláusula 7ª. - </strong> Ficam excluídas do presente termo as informações: </p>
        <p> <strong> a) </strong> que sejam de domínio público (Lei nº 9.610, de 19 de fevereiro de 1998- http://www.dominiopublico.gov.br)  </p>
        <p> <strong> b) </strong> informações que venham a ser disponibilizadas para o público, de outra forma que não seja por meio de divulgação por parte dos usuários e prestadores de serviços da empresa Novetech Soluções Tecnológicas;   </p>
        <p> <strong> Cláusula 8ª. - </strong> As obrigações de confidencialidade contidas neste instrumento terão validade por prazo indeterminado, ficando os usuários e prestadores de serviços adstritos aos seus termos mesmo após o término do Contrato.  </p>
        <p> <strong> Cláusula 9ª. - </strong> No caso de dúvidas quanto ao correto procedimento, deverá ser consultado a administração da Novetech Soluções Tecnológicas.  </p>
        <p> <strong> 9.1 - </strong> No caso de término do contrato será permitido ao gestor da unidade de saúde a geração de um arquivo em meio magnético com todos os prontuários dos pacientes por ele atendidos. Um backup do banco de dados, onde este arquivo será entregue na forma presencial e com a assinatura do termo de recebimento. </p>
        <p> <strong> Cláusula 10ª. - </strong> A Novetech Soluções Tecnológicas tem o direito de, a qualquer momento, modificar, alterar ou retirar quaisquer políticas ou procedimentos, adicionar outros que se façam necessários para o perfeito funcionamento do sistema, mediante simples aviso por circular. Por concordar com a redação supra e do inequívoco compromisso firmado, declaro ciência e concordância, aderindo a todos os termos e condições ora estabelecidos. </p>

        <p align="center"> <strong>RESOLUÇÃO CFM Nº 1.821/07 (Publicada no Diário Oficial da União de 23 nov. 2007, Seção I, pg. 252) </strong> </p>
        <p> Aprova as normas técnicas concernentes à digitalização e uso dos sistemas informatizados para a guarda e manuseio dos documentos dos prontuários dos pacientes, autorizando a eliminação do papel e a troca de informação identificada em saúde.  </p>
        <p> <strong>O CONSELHO FEDERAL DE MEDICINA</strong>, no uso das atribuições que lhe confere a Lei nº 3.268, de 30 de setembro de 1957, alterada pela Lei nº 11.000, de 15 de dezembro de 2004, regulamentada pelo Decreto nº 44.045, de 19 de julho de 1958, e </p>
        <p> <strong>CONSIDERANDO</strong> que o médico tem o dever de elaborar um prontuário para cada paciente a que assiste; </p>
        <p> <strong>CONSIDERANDO</strong> que o Conselho Federal de Medicina (CFM) é a autoridade certificadora dos médicos do Brasil (AC) e distribuirá o CRM-Digital aos médicos interessados, que será um certificado padrão ICP-Brasil;</p>
        <p> <strong>CONSIDERANDO</strong> que as unidades de serviços de apoio, diagnóstico e terapêutica têm documentos próprios, que fazem parte dos prontuários dos pacientes;</p>
        <p> <strong>CONSIDERANDO</strong> o crescente volume de documentos armazenados pelos vários tipos de estabelecimentos de saúde, conforme definição de tipos de unidades do Cadastro Nacional de Estabelecimentos de Saúde, do Ministério da Saúde;</p>
        <p> <strong>CONSIDERANDO</strong> os avanços da tecnologia da informação e de telecomunicações, que oferecem novos métodos de armazenamento e transmissão de dados;</p>
        <p> <strong>CONSIDERANDO</strong> o teor das Resoluções CFM nos 1.605, de 29 de setembro de 2000, e 1.638, de 9 de agosto de 2002; </p>
        <p> <strong>CONSIDERANDO</strong> o teor do Parecer CFM nº 30/02, aprovado na sessão plenária de 10 de julho de 2002, que trata de prontuário elaborado em meio eletrônico;  </p>
        <p><strong>CONSIDERANDO</strong> que o prontuário do paciente, em qualquer meio de armazenamento, é propriedade física da instituição onde o mesmo é assistido - independente de ser unidade de saúde ou consultório -, a quem cabe o dever da guarda do documento;  </p>
        <p><strong>CONSIDERANDO</strong> que os dados ali contidos pertencem ao paciente e só podem ser divulgados com sua autorização ou a de seu responsável, ou por dever legal ou justa causa;</p>
        <p><strong>CONSIDERANDO</strong> que o prontuário e seus respectivos dados pertencem ao paciente e devem estar permanentemente disponíveis, de modo que quando solicitado por ele ou seu representante legal permita o fornecimento de cópias autênticas das informações pertinentes; </p>
        <p><strong>CONSIDERANDO</strong> que o sigilo profissional, que visa preservar a privacidade do indivíduo, deve estar sujeito às normas estabelecidas na legislação e no Código de Ética Médica, independente do meio utilizado para o armazenamento dos dados no prontuário, quer eletrônico quer em papel; </p>
        <p><strong>CONSIDERANDO</strong> o disposto no Manual de Certificação para Sistemas de Registro Eletrônico em Saúde, elaborado, conforme convênio, pelo Conselho Federal de Medicina e Sociedade Brasileira de Informática em Saúde; </p>
        <p><strong>CONSIDERANDO</strong> que a autorização legal para eliminar o papel depende de que os sistemas informatizados para a guarda e manuseio de prontuários de pacientes atendam integralmente aos requisitos do "Nível de garantia de segurança 2 (NGS2)", estabelecidos no referido manual;</p>
        <p><strong>CONSIDERANDO</strong> que toda informação em saúde identificada individualmente necessita de proteção em sua confidencialidade, por ser princípio basilar do exercício da medicina;</p>
        <p><strong>CONSIDERANDO</strong> os enunciados constantes nos artigos 102 a 109 do Capítulo IX do Código de Ética Médica, o médico tem a obrigação ética de proteger o sigilo profissional; </p>
        <p><strong>CONSIDERANDO</strong> o preceituado no artigo 5º, inciso X da Constituição da República Federativa do Brasil, nos artigos 153, 154 e 325 do Código Penal (Decreto-Lei nº 2.848, de 7 de dezembro de 1940) e no artigo 229, inciso I do Código Civil (Lei nº 10.406, de 10 de janeiro de 2002);</p>
        <p><strong>CONSIDERANDO</strong>, finalmente, o decidido em sessão plenária de 11/7/2007, resolve:</p>
        <p><strong>Art. 1º</strong> Aprovar o Manual de Certificação para Sistemas de Registro Eletrônico em Saúde, versão 3.0 e/ou outra versão aprovada pelo Conselho Federal de Medicina, anexo e também disponível nos sites do Conselho Federal de Medicina e Sociedade Brasileira de Informática em Saúde (SBIS), respectivamente, www.portalmedico.org.br e www.sbis.org.br.</p>
        <p><strong>Art. 2º</strong> Autorizar a digitalização dos prontuários dos pacientes, desde que o modo de armazenamento dos documentos digitalizados obedeça a norma específica de digitalização contida nos parágrafos abaixo e, após análise obrigatória da Comissão de Revisão de Prontuários, as normas da Comissão Permanente de Avaliação de Documentos da unidade médico-hospitalar geradora do arquivo.</p>
        <p><strong>§ 1º</strong> Os métodos de digitalização devem reproduzir todas as informações dos documentos originais.</p>
        <p><strong>§ 2º</strong> Os arquivos digitais oriundos da digitalização dos documentos do prontuário dos pacientes deverão ser controlados por sistema especializado (Gerenciamento eletrônico de documentos - GED), que possua, minimamente, as seguintes características:</p>
        <p><strong>a)</strong> Capacidade de utilizar base de dados adequada para o armazenamento dos arquivos digitalizados;</p>
        <p><strong>b)</strong> Método de indexação que permita criar um arquivamento organizado, possibilitando a pesquisa de maneira simples e eficiente;</p>
        <p><strong>c)</strong> Obediência aos requisitos do "Nível de garantia de segurança 2 (NGS2)", estabelecidos no Manual de Certificação para Sistemas de Registro Eletrônico em Saúde;</p>
        <p><strong>Art. 3º</strong> Autorizar o uso de sistemas informatizados para a guarda e manuseio de prontuários de pacientes e para a troca de informação identificada em saúde, eliminando a obrigatoriedade do registro em papel, desde que esses sistemas atendam integralmente aos requisitos do "Nível de garantia de segurança 2 (NGS2)", estabelecidos no Manual de Certificação para Sistemas de Registro Eletrônico em Saúde;</p>
        <p><strong>Art. 4º</strong> Não autorizar a eliminação do papel quando da utilização somente do "Nível de garantia de segurança 1 (NGS1)", por falta de amparo legal.</p>
        <p><strong>Art. 5º</strong> Como o "Nível de garantia de segurança 2 (NGS2)", exige o uso de assinatura digital, e conforme os artigos 2º e 3º desta resolução, está autorizada a utilização de certificado digital padrão ICP-Brasil, até a implantação do CRM Digital pelo CFM, quando então será dado um prazo de 360 (trezentos e sessenta) dias para que os sistemas informatizados incorporem este novo certificado.</p>
        <p><strong>Art. 6º</strong> No caso de microfilmagem, os prontuários microfilmados poderão ser eliminados de acordo com a legislação específica que regulamenta essa área e após análise obrigatória da Comissão de Revisão de Prontuários da unidade médico-hospitalar geradora do arquivo. </p>
        <p><strong>Art. 7º</strong> Estabelecer a guarda permanente, considerando a evolução tecnológica, para os prontuários dos pacientes arquivados eletronicamente em meio óptico, microfilmado ou digitalizado.</p>
        <p><strong>Art. 8º</strong> Estabelecer o prazo mínimo de 20 (vinte) anos, a partir do último registro, para a preservação dos prontuários dos pacientes em suporte de papel, que não foram arquivados eletronicamente em meio óptico, microfilmado ou digitalizado. </p>
        <p><strong>Art. 9º</strong> As atribuições da Comissão Permanente de Avaliação de Documentos em todas as unidades que prestam assistência médica e são detentoras de arquivos de prontuários de pacientes, tomando como base as atribuições estabelecidas na legislação arquivística brasileira, podem ser exercidas pela Comissão de Revisão de Prontuários. </p>
        <p><strong>Art. 10º</strong> Estabelecer que o Conselho Federal de Medicina (CFM) e a Sociedade Brasileira de Informática em Saúde (SBIS), mediante convênio específico, expedirão selo de qualidade dos sistemas informatizados que estejam de acordo com o Manual de Certificação para Sistemas de Registro Eletrônico em Saúde, aprovado nesta resolução. </p>
        <p><strong>Art. 11º</strong> Ficam revogadas as Resoluções CFM nos 1.331/89 e 1.639/02, e demais disposições em contrário. </p>
        <p><strong>Art. 12º</strong> Esta resolução entra em vigor na data de sua publicação (08/09/2020). </p>
        <p>Assim, ao paciente pertencem os dados ali contidos, os quais só podem ser divulgados com a sua autorização ou a de seu responsável, ou por dever legal ou justa causa. Estes dados devem estar permanentemente disponíveis, de modo que, quando solicitados por ele ou seu representante legal, permitam o fornecimento de cópias autênticas das informações a ele pertinentes. </p>
        <p> Data/Hora: {{ moment().format('D')}} de {{ moment().format('MMMM')}} de {{ moment().format('YYYY')}} {{moment().format('HH:mm')}}</p>
      </VuePerfectScrollbar>
    </el-dialog>
  </section>
</template>

<script>
  import VuePerfectScrollbar from 'vue-perfect-scrollbar'
  import _api from '../../api'
  import b64 from 'base-64'

  export default {
    name: 'login',
    components: { VuePerfectScrollbar },
    data() {
      return {
        screenWidth: window.innerWidth,

        showModalAceite: false,
        statusEntrarTermo: '',
        termoAceito: false,
        formAceite: {},
        user: {
          accept: false
        },
        formData: {
          username: '',
          password: '',
          scope: 'Profissional',
          audience: this.$config.audience
        },
        tokenData: {
          access_token: null,
          expires_in: 0,
          refresh_token: null
        },
        scrollSettings: {
          maxScrollbarLength: 50
        },
        erro: '',
        loading: false
      }
    },
    computed: {
      windowWidth() {
        return this.screenWidth;
      }
    },
    //beforeCreated() {
    //  this.$config.getConfig()
    //},
    created() {
      if (localStorage.getItem('access_token') === null) {
        this.clearAllCookies()
      }
    },

    mounted() {
      window.addEventListener('resize', this.updateScreenWidth);
    },

    beforeDestroy() {
      window.removeEventListener('resize', this.updateScreenWidth);
    },

    methods: {
      updateScreenWidth() {
        this.screenWidth = window.innerWidth;
      },

      redirectLoginPaciente() {
        window.location.href = this.$config.urlPaciente
      },

      async visualizarTermos() {
        this.showModalAceite = true
      },

      login() {
        if (this.termoAceito === true && this.user.accept === true) {
          this.loading = true
          let redirectFrom = this.$route.params.redirectFrom
          if (this.$auth.redirect()) {
            redirectFrom = this.$auth.redirect().from.name
          }
          let data = {
            username: b64.encode(this.formData.username),
            password: b64.encode(this.formData.password),
            scope: b64.encode(this.formData.scope),
            audience: b64.encode(this.$config.audience)
          }
          //let redirect = this.formData.scope === 'Profissional' ? 'Home' : 'Home'

          this.$auth.login({
            data: data,
            // redirect: { name: redirect },
            fetchUser: false, // true se dados recebidos vierem do endpoint /auth/user (gerenciado pelo vue-auth)
            async success(res) {
              this.tokenData = res.data
              await this.setUser(this.tokenData)
              this.loading = false
            },
            error(e) {
              this.loading = false
              var erro = e.response.status === 400 ? e.response.data : 'Erro No Processo de Login'
              this.$swal({
                title: "Erro!",
                text: "Verifique seus dados de login e tente novamente!",
                icon: 'error',
              })
            }
          })
        } else {
          this.$swal({
            title: "Erro!",
            text: "Verifique o termo de responsabilidade",
            icon: 'error',
          })
        }
      },

      async setUser(tokenData) {
        await this.$user.setUser(tokenData) // desnecessário em caso de fetchUser = true
        var string = JSON.stringify(this.$store.state.user.dados)
        // caso seja diferente de vazio chama a tela home
        if (string != '{}') this.$router.push({ name: 'Home' })
        // o else é para caso ocorra do store user dados mesmo com todo o processo não retorne o objeto do usuario ele fazer novamente o fluxo
        else {
          await this.$user.setUser(tokenData)
          var string = JSON.stringify(this.$store.state.user.dados)
          if (string != '{}') this.$router.push({ name: 'Home' })
        }
      },

      async checkBox() {
        if (this.user.accept === true && this.formData.scope === 'Usuario') {
          this.termoAceito = true
        } else if (this.user.accept === true && this.formData.scope === 'Profissional') {
          this.termoAceito = true
        } else if (this.user.accept === true && this.formData.scope === 'Paciente') {
          this.termoAceito = true
        } else {
          /* console.log('erro') */
        }
      },

      clearAllCookies() {
        var cookies = document.cookie.split('; ')
        for (var c = 0; c < cookies.length; c++) {
          var d = window.location.hostname.split('.')
          while (d.length > 0) {
            var cookieBase = encodeURIComponent(cookies[c].split(';')[0].split('=')[0]) + '=; expires=Thu, 01-Jan-1970 00:00:01 GMT; domain=' + d.join('.') + ' ;path='
            var p = location.pathname.split('/')
            document.cookie = cookieBase + '/'
            while (p.length > 0) {
              document.cookie = cookieBase + p.join('/')
              p.pop()
            };
            d.shift()
          }
        }
      },
      onResetSenha() {
        this.$router.push({
          name: 'RedefinirSenha'
        })
      }
    }
  }
</script>

<style scoped>

  html, body {
    background: #43bec6 !important;
    min-width: 350px !important;
  }

  .wrapper {
    min-width: 350px !important;
    background: #43bec6;
    display: flex;
    justify-content: center;
    align-items: center;
    flex-direction: column
  }

  .wrap-login p {
    word-break: normal !important;
  }

  .login__bt-termo {
    background-color: white;
    color: #43bec6;
    border: 0px;
    outline: none;
    padding: 0px;
    margin-left: 4px;
  }


  .login-form-container {
    display: flex;
    flex-direction: column;
    border-radius: 10px;
    max-width: 380px;
    width: 360px;
    background-color: #fff;
  }

  @media screen and (max-width: 400px) {
    .login-form-container {
      max-width: none;
      width: 75%;
    }
  }


  .login-form {
    border-radius: 10px;
    max-width: 380px;
    padding: 15px 35px 0px;
    width: 100%;
    margin: 0 auto 0 auto;
    background-color: #fff;
  }
  @media screen and (max-width: 400px) {
    .login-form{
      padding: 15px 15px 0px 15px;
    }
  }


    .login-form .btn-block {
      border-bottom-left-radius: 10px;
      border-bottom-right-radius: 10px;
      background: #008d95;
      text-transform: uppercase;
      font-size: 1.05em;
      border: 0;
      padding: 15px 0;
    }


  .login-logo-meeds-container {
    display: flex;
    flex-direction: row;
    justify-content: center;
    align-items: center;
    margin-bottom: 15px;
    width: 100%;
  }

    .login-logo-meeds-container img {
      width: 235px;
    }


  .login--logo {
    margin-bottom: 15px;
    margin-left: 25px;
    width: max-content;
    height: max-content;
  }

    .login--logo img {
      width: 235px;
    }

  .login--logo--novetech {
    margin-top: 20px;
    height: 55px
  }

  .login--endereco {
    margin: 0;
    font-size: 0.8rem;
    color: #fff;
    font-weight: 300;
    line-height: 120%;
    padding-top: 10px;
  }

    .login--endereco a:link, .login--endereco a:visited {
      color: #fff;
    }

  .novetech {
    width: 350px;
    margin: 0 auto;
    text-align: center;
  }

  .login--logo--p {
    width: 70px;
    padding-top: 24px;
    padding-right: 11px;
  }

  .l1, .l2 {
    float: right;
  }

  .scroll-area {
    position: relative;
    margin: auto;
    width: 95%;
    height: 710px;
    overflow-x: hidden;
  }

  .modal--termos {
    background: #43bec6;
  }

  .el-dialog__body {
    word-break: keep-all;
  }


  .navbar-fixed-bottom {
    position: fixed;
    right: 0;
    left: 0;
    z-index: 1030;
    bottom: 0;
    margin-bottom: -1px;
    border-width: 1px 0 0;
  }

  .footer--empresa {
    text-align: right;
    padding-right: 25px;
    font-size: 11px;
  }

    .footer--empresa a {
      text-decoration: none;
      text-align: center;
      display: flex;
      justify-content: flex-end;
      align-items: flex-end;
    }

    .footer--empresa img {
      width: 15px;
      height: 15px;
      margin-bottom: 3px;
    }

  .form-login {
    margin-bottom: 0.5rem
  }

  .form-infos {
    display: flex;
    flex-direction: row;
    justify-content: space-between;
    margin-bottom: 3px;
  }

  @media screen and (max-width: 400px) {
    .form-infos {
      flex-direction: row;
    }
  }

  .form-infos-items {
    display: flex;
    flex-direction: row;
    align-items: center;
    justify-content: center;
  }

  .form-infos-button {
    background-color: transparent;
    color: #43bec6;
    border: 0px;
    outline: none;
  }

  @media screen and (max-width: 400px) {
    .form-infos-button {
      padding: 10px 15px 10px 15px;
    }
  }

  .form-radio-button {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    margin-bottom: 0.5rem
  }

  .form-radio-button-mobile {
    display: flex;
    flex-direction: row;
    gap:5px;
    align-items: center;
    justify-content: center;
    margin-bottom: 0.5rem
  }

  .form-radio-button-mobile label {
      margin: 0px 0px 4px 0px !important;
  }

  .form-termo {
    display: flex;
    flex-direction: row;
    align-items: center;
    justify-content: center;
    margin-bottom: 1rem;
  }

  .error {
    color: #721c24;
    background-color: #f8d7da;
    border-color: #f5c6cb;
    padding: 0.75rem 1.25rem;
    margin-bottom: 0.5rem;
    border: 1px solid transparent;
    border-radius: 0.25rem
  }

  .login-button {
    width: 100%;
    background-color: #349b89;
    font-weight: bold;
    padding-top: 20px;
    padding-bottom: 20px;
    border-radius: 10px;
    border-top-left-radius: 0px;
    border-top-right-radius: 0px;
  }
  @media screen and (max-width: 400px) {
    .login-button {
      padding-top: 14px;
      padding-bottom: 14px;
    }
  }

</style>
