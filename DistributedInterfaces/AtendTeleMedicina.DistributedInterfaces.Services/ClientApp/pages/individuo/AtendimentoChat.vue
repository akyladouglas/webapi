<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">

    <el-card class="box-card box-card--main-card">

      <el-row>
        <el-col :span="14">
          <h2 class="box-card--h2">Atendimento</h2>
        </el-col>
        <el-col :span="10" class="text-right">
          <el-form :inline="true">
            <el-form-item>
              <el-button v-if="!listando" style="margin-right: -10px" icon="fas fa-undo-alt" type="warning" @click="onListar('formIndividuo')"> Voltar</el-button>
            </el-form-item>
          </el-form>
        </el-col>
      </el-row>

      <el-row v-show="listando">
        <el-col :span="24">
          <FiltroAtendimento :loading="loading.individuos" :params="params" @emit="onEmitFromFiltro" />
        </el-col>
      </el-row>

      <el-row v-show="listando && api.individuos.length > 0">
        <el-col :span="24">
          <el-table ref="tabela" :data="api.individuos"
                                  highlight-current-row border
                                  v-loading.body="loading.individuos"
                                  @selection-change="onSelectIndividuo"
                                  class="table--atendimentos table--row-click">
            <el-table-column align="center" type="selection" width="55" />
            <el-table-column align="center" label="Grau de Risco" prop="corStatus" width="130">
              <template slot-scope="scope">
                <el-tag effect="dark" :class="corRisco(scope.row.corStatus)">{{ scope.row.corStatus }}</el-tag>
              </template>
            </el-table-column>
            <el-table-column label="Nome" fixed width="320">
              <template slot-scope="scope">
                <el-badge is-dot value="Em Atendimento" title="Em Atendimento" class="item" v-if="scope.row.emAtendimentoPor">
                  {{ scope.row.nomeCompleto }}
                </el-badge>
                <span v-else>{{ scope.row.nomeCompleto }}</span>
              </template>
            </el-table-column>
            <el-table-column label="Sexo" align="center" width="65">
              <template slot-scope="scope">
                <i v-if="scope.row.sexo === 0" class="fas fa-male"></i>
                <i v-if="scope.row.sexo === 1" class="fas fa-female"></i>
              </template>
            </el-table-column>
            <el-table-column label="Idade" width="65">
              <template slot-scope="scope"> {{ moment().diff(scope.row.dataNascimento, 'years') }}</template>
            </el-table-column>
            <el-table-column label="Atendimentos" width="130" align="center" show-overflow-tooltip>
              <template slot-scope="scope">
                {{ scope.row.atendimentos.length }}
              </template>
            </el-table-column>
            <el-table-column label="Último Atendimento" width="160">
              <template slot-scope="scope">
                <span v-if="scope.row.atendimentos[0]">{{ moment(scope.row.atendimentos[0].dataCadastro).format("DD/MM/YYYY") }}</span>
                <span v-if="!scope.row.atendimentos[0]">-</span>
              </template>
            </el-table-column>
            <el-table-column label="Atendido Por" width="170" show-overflow-tooltip>
              <template slot-scope="scope">
                <span v-if="scope.row.atendimentos[0]">{{ scope.row.atendimentos[0].usuario.nome }}</span>
                <span v-if="!scope.row.atendimentos[0]">-</span>
              </template>
            </el-table-column>
            <!-- <el-table-column header-align="center" align="right" width="140" fixed="right">
              <template slot-scope="scope">
                <el-button size="small" icon="fas fa-file-signature" type="primary" v-loading.body="loading.individuo" @click="onVerificaAtendimento(scope.row, $event)"> Atender</el-button>
              </template>
            </el-table-column> -->
            <el-table-column header-align="center" align="right" width="140" fixed="right">
              <template slot-scope="scope">
                <el-dropdown>
                  <el-button type="primary" size="small">
                    Ações <i class="el-icon-arrow-down el-icon--right"></i>
                  </el-button>
                  <el-dropdown-menu slot="dropdown">
                    <ul class="list-unstyled">
                      <li @click="onIniciarChat(scope.row)" class="el-dropdown-menu__item">
                        <i v-if="!loading.chat" class="fas fa-comments"></i>
                        <i v-if="loading.chat" class="fa fa-spinner fa-spin"></i>
                        Iniciar Chat</li>
                      <li @click="onAtendimento(scope.row)" class="el-dropdown-menu__item"><i class="fas fa-file-signature"></i> Atender</li>
                    </ul>
                  </el-dropdown-menu>
                </el-dropdown>
              </template>
            </el-table-column>
          </el-table>
        </el-col>

        <el-col :span="24" v-show="listando">
          <el-pagination
            @size-change="handleSizeChange"
            @current-change="handleCurrentChange"
            :current-page.sync="params.page"
            :page-sizes="[10,25,50,100]"
            :page-size="params.pageSize"
            :total="params.total"
            layout="total, sizes, prev, pager, next, jumper">
          </el-pagination>
        </el-col>
      </el-row>

      <el-row :gutter="20" v-show="listando && api.individuos.length > 0">
        <el-col :xs="24" :sm="24" :md="12" :lg="10" :xl="10" class="forms--margin-xs-from-top">
          <el-button icon="fas fa-paper-plane" type="info" @click="onEnviarMensagem"> Enviar Mensagem</el-button>
        </el-col>
      </el-row>

      <el-dialog title="Em Atendimento" :visible.sync="showEmAtendimento" width="30%" v-if="individuo.emAtendimentoPor">
        <span>Cidadão já está em atendimento por: <strong>{{individuo.emAtendimentoPor.nome}}</strong></span>
        <span slot="footer" class="dialog-footer">
          <el-button type="danger" @click="showEmAtendimento = false">Fechar</el-button>
          <el-button type="success" :disabled="individuo.emAtendimentoPor.id !== $auth.user().id" @click="onAtendimento">Continuar</el-button>
        </span>
      </el-dialog>

      <el-dialog title="Enviar Mensagem" :visible.sync="showModalMensagem" width="40%">
        <el-form :model="formMensagem" :rules="formMensagemValidacoes" ref="formMensagem" label-width="120px" label-position="top" class="forms--mensagem">
          <el-row :gutter="20">
            <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
              <el-form-item label="Mensagem" prop="mensagem">
                <el-input type="textarea" :rows="4" v-model="formMensagem.mensagem" />
              </el-form-item>
              Esta mensagem será enviada à {{ listaIndividuos.length }} indivíduo(s).
            </el-col>
          </el-row>
        </el-form>
        <span slot="footer" class="dialog-footer">
          <el-button @click="showModalMensagem = false">Cancelar</el-button>
          <el-button v-if="!loading.notificacoes" type="success" icon="fas fa-paper-plane" :disabled="!formMensagem.mensagem || listaIndividuos.length === 0" @click="doEnviarMensagem('formMensagem')"> Enviar</el-button>
          <el-button v-if="loading.notificacoes" type="success" icon="fa fa-spinner fa-spin" disabled> Aguarde ...</el-button>
        </span>
      </el-dialog>

      <el-dialog title="Atendimento" :visible.sync="showModalAtendimento" width="40%">
        <el-form :model="formAtendimento" :rules="formAtendimentoValidacoes" ref="formAtendimento" label-width="120px" label-position="top" class="forms--atendimento">
          <el-row :gutter="20">
            <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12">
              <el-form-item label="Nome Completo" prop="nomeCompleto">
                <el-input v-model="formAtendimento.nomeCompleto" disabled />
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12">
              <el-form-item label="Data Nascimento" prop="dataNascimento">
                <el-date-picker prefix-icon="fas fa-calendar-day" v-model="formAtendimento.dataNascimento" type="date" format="dd-MM-yyyy" disabled/>
              </el-form-item>
            </el-col>
          </el-row>
          <el-row :gutter="20">
            <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="12">
              <el-form-item label="Telefone Celular" prop="telefoneCelular">
                <el-input v-model="formAtendimento.telefoneCelular" masked="true" maxlength="15" v-mask="'(##) #####-####'" disabled>
                  <i slot="prefix" class="fas fa-mobile-alt"></i>
                </el-input>
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="24" :lg="12" :xl="12">
              <el-form-item label="Classificação de Risco" prop="corStatus">
                <el-select filterable placeholder="Selecione a Classificação de Risco" v-model="formAtendimento.corStatus">
                  <el-option v-for="option in enums.coresStatus" :value="option.value" :label="option.label" :key="option.value" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
              <el-form-item label="Detalhes do Atendimento" prop="mensagem">
                <el-input type="textarea" :rows="4" v-model="formAtendimento.mensagem" />
              </el-form-item>
            </el-col>
          </el-row>
        </el-form>
        <span slot="footer" class="dialog-footer">
          <el-button @click="showModalAtendimento = false">Cancelar</el-button>
          <el-button v-if="!loading.atendimento" type="success" icon="fas fa-paper-plane" :disabled="!formAtendimento.mensagem" @click="doInserirAtendimento('formAtendimento')"> Enviar</el-button>
          <el-button v-if="loading.atendimento" type="success" icon="fa fa-spinner fa-spin" disabled> Aguarde ...</el-button>
        </span>
      </el-dialog>

      <el-dialog title="Foto do Documento" :visible.sync="showModalDocumento" width="540px">
        <el-image
        style="width: 500px; height: 700px"
        :src="imagemDocumento"
        fit="true" />
      </el-dialog>

      <el-dialog :title="'Bate-papo: ' + qbDialog.name" :visible.sync="showModalChat" width="75%" @close="qbFechatChat" >
        <div id="containerChat" style="max-height:400px; overflow-y: auto;">
          <!--<video id="oppnentVideo" autoplay playsinline />-->
          <div id="callinfo" v-if="inCall">
            <i class="fa fa-phone" style="margin: 0 10px 0 0"></i>
            <span v-if="isCalling"> Ligando... </span>
            <span v-if="isCall"> Em chamada </span>
            <el-button style="margin: 0 0 0 auto" type="danger" size="mini" @click="qbDesligarChamada()">DESLIGAR</el-button>
          </div>
          <div :gutter="20" v-for="(item, index) in qbMensagens" :key="index">
            <div class="qbMessageOwner" v-if="item.sender_id == qbUsuario.id">
              <h6>Você</h6>
              <p class="qbMessageP">{{item.message}}</p>
            </div>
            <div class="qbMessageOccupant" v-else>
              <h6>{{qbDialog.name}}</h6>
              <p class="qbMessageP">{{item.message}}</p>
            </div>
          </div>
        </div>
        <span slot="footer" class="dialog-footer">
          <el-form :model="formChatMessage" @submit.prevent.native="qbSendMessage">
            <el-row type="flex">
              <el-col :span="21">
                <el-input type="text" placeholder="Digite uma mensagem" v-model="formChatMessage.message" clearable />
              </el-col>
              <el-col :span="3">
                <el-button type="success" icon="fas fa-paper-plane" @keyup.enter="qbSendMessage" @click="qbSendMessage"> Enviar</el-button>
              </el-col>
              <el-col :span="3">
                <el-button type="success" icon="fas fa-phone" :disabled="isCall || isCalling" @click="qbIniciarChamada"> Ligar</el-button>
              </el-col>
            </el-row>
          </el-form>
        </span>
      </el-dialog>

      <el-row v-show="!listando">

        <el-row :gutter="20" v-if="erros.length > 0">
          <el-col :xs="24">
            <div id="erros" class="alert alert-danger" role="alert">
              <ul>
                <li v-for="erro in erros" :key="erro">{{erro}}</li>
              </ul>
            </div>
          </el-col>
        </el-row>

      </el-row>
   </el-card>

  </el-col>
</template>

<script>
  import axios from 'axios'
  import Utils from '../../mixins/Utils'
  import jQuery from 'jquery'
  import _api from '../../api'
  import _enums from '../../api/Enums'
  import FiltroAtendimento from '../../components/shared/FiltroAtendimento'
  import { Notification } from 'element-ui'
  import { mask } from 'vue-the-mask'
  import jwt from 'jwt-decode'
  export default {
    name: 'AtendimentoChat',
    mixins: [Utils],
    components: { FiltroAtendimento },
    directives: { mask },
    data () {
      var validaCpf = (rule, value, callback) => {
        if (this.mxValidaCPF(this.formIndividuo.cpf) === false) {
          return callback(new Error('Cpf Inválido'))
        } else {
          callback()
        }
      }
      var validaSenha = (rule, value, callback) => {
        if (value === '') {
          callback(new Error('Digite a senha'))
        } else {
          if (this.formIndividuo.senhaConfirmacao !== '') {
            this.$refs.formIndividuo.validateField('senhaConfirmacao')
          }
          callback()
        }
      }
      var validaSenhaConfirmacao = (rule, value, callback) => {
        if (value === '') {
          callback(new Error('Digite a confirmação da senha'))
        } else if (value !== this.formIndividuo.senha) {
          callback(new Error('Senha não confere'))
        } else {
          callback()
        }
      }
      return {
        isDisabled: false,
        el_aguarde: false,
        isValid: true,
        metodo: 'POST',
        listando: true,
        erros: [],
        showModalMensagem: false,
        showModalAtendimento: false,
        showModalDocumento: false,
        showModalChat: false,
        showEmAtendimento: false,
        formIndividuo: {},
        formMensagem: {},
        formAtendimento: {},
        imagemDocumento: '',
        formChatMessage: {
          message: ''
        },
        qbUsuario: null,
        qbDialog: {
          nome: ''
        },
        qbUserOpponent: null,
        qbMensagens: [],
        sessionCall: null,
        inCall: false,
        isCalling: false,
        isCall: false,
        validacoes: {
          nomeCompleto: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' },
            { min: 3, max: 255, message: 'Nome não pode conter menos de 4 e mais que 255 caracteres', trigger: 'submit' }
          ],
          cpf: [
            { validator: validaCpf, trigger: ['blur', 'change'] },
            { required: true, message: 'Campo obrigatório', trigger: 'change' }
          ],
          cartaoSUS: [
            { min: 15, max: 15, message: 'Cartão SUS inválido', trigger: 'change' }
          ],
          senha: [ { validator: validaSenha, trigger: ['blur', 'change'] } ],
          senhaConfirmacao: [ { validator: validaSenhaConfirmacao, trigger: ['blur', 'change'] } ],
          nomeDaMae: [
            { required: '!this.formIndividuo.temMaeDesconhecida', message: 'Campo obrigatório', trigger: ['blur', 'change'] },
            { min: 3, max: 48, message: 'Nome da mãe não pode conter menos de 4 e mais que 8 letras', trigger: ['blur', 'change'] }
          ],
          email: [
            { required: true, message: 'Campo Obrigatório', trigger: ['blur', 'change'] },
            { type: 'email', message: 'Email inválido', trigger: ['blur', 'change'] }
          ],
          dataNascimento: [{ required: true, message: 'Campo obrigatório', trigger: ['blur', 'change'] }],
          cep: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' }
          ],
          ufAbreviado: [{ required: true, message: 'Campo obrigatório', trigger: ['blur', 'change'] }],
          cidade_Id: [{ required: true, message: 'Campo obrigatório', trigger: ['blur', 'change'] }],
          sexo: [{ required: true, message: 'Campo obrigatório', trigger: ['blur', 'change'] }],
          bairro: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' },
            { min: 1, max: 100, message: 'Bairro não pode conter menos de 1 e mais que 100 caracteres', trigger: 'change' }
          ],
          logradouro: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' },
            { min: 1, max: 150, message: 'Endereço não pode conter menos de 1 e mais que 150 caracteres', trigger: 'change' }
          ],
          transplantado: [{ required: true, message: 'Campo obrigatório', trigger: ['submit', 'change'] }],
          doencaComprometeImunidade: [{ required: true, message: 'Campo obrigatório', trigger: ['submit', 'change'] }],
          fumante: [{ required: true, message: 'Campo obrigatório', trigger: ['submit', 'change'] }],
          hipertenso: [{ required: true, message: 'Campo obrigatório', trigger: ['submit', 'change'] }],
          asma: [{ required: true, message: 'Campo obrigatório', trigger: ['submit', 'change'] }],
          diabetes: [{ required: true, message: 'Campo obrigatório', trigger: ['submit', 'change'] }],
          doencaCoracao: [{ required: true, message: 'Campo obrigatório', trigger: ['submit', 'change'] }],
          doencaRins: [{ required: true, message: 'Campo obrigatório', trigger: ['submit', 'change'] }],
          doencaFigado: [{ required: true, message: 'Campo obrigatório', trigger: ['submit', 'change'] }],
          doencaCancer: [{ required: true, message: 'Campo obrigatório', trigger: ['submit', 'change'] }],
        },
        formMensagemValidacoes: {
          mensagem: [
            { required: true, message: 'Campo obrigatório', trigger: 'submit' }
          ]
        },
        formAtendimentoValidacoes: {
          mensagem: [
            { required: true, message: 'Campo obrigatório', trigger: 'submit' }
          ]
        },
        enums: {
          sexos: _enums.sexos,
          simNao: _enums.simNao,
          motivosCadastro: _enums.motivosCadastro,
          coresStatus: _enums.coresStatus,
          resultadosExame: _enums.resultadosExame
        },
        listaIndividuos: [],
        individuo: {},
        api: {
          individuos: [],
          ufs: [],
          cidades: [],
          mensagens: [],
          notificacoes: []
        },
        loading: {
          individuos: false,
          individuo: false,
          ufs: false,
          cidades: false,
          mensagens: false,
          notificacoes: false,
          atendimento: false,
          relatorio: false,
          chat: false
        },
        params: {
          page: 1,
          pageSize: 10,
          // sort: 'a.DataCadastro ASC, i.CorStatus DESC, i.NomeCompleto ASC',
          // Remover NomeCompleto após testes, estava vindo aleatório o nome porque muitos tem a dataUltimoAtendimento NULL
          sort: 'i.DataUltimoAtendimento ASC, i.CorStatus DESC, i.NomeCompleto ASC',
          statusIn: [0, 1, 2, 3, 4],
          total: 0,
          ufs: {
            skip: 1,
            take: 30,
            sort: '+UfAbreviado',
            total: 0
          },
          cidades: {
            skip: 1,
            take: 999,
            sort: '+Nome',
            total: 0,
            ufAbreviado: null
          }
        },
        qbCredentials: {
          appId: 81654,
          authKey: 'AEKF26TkvUQNeLY',
          authSecret: 'reBfwKNzv7WfGsn'
        }
      }
    },
    async mounted () {
      await this.getIndividuos()
      await this.getUfs()
      await this.getMensagens()
      await this.qbInit()
    },
    methods: {
      qbInit() {
        QB.init(this.qbCredentials.appId, this.qbCredentials.authKey, this.qbCredentials.authSecret, );
        var token = this.$auth.token()
        var profissional = jwt(token)
        var profissionalCpf = profissional['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
        QB.createSession((err, res) => {
          if(err) {
            Notification({message: 'Não foi possível iniciar o serviço de chat', type: 'error'});
          } else {
            QB.login({login: `u-${profissionalCpf}`, password: 'monitoraCovid19'}, (err, user) => {
              if(err) {
                Notification({message: 'Não foi possível iniciar o serviço de chat', type: 'error'});
              } else {
                this.qbUsuario = user
              }
            })
          }
        })
      },
      onIniciarChat (row) {
        this.loading.chat = true
        QB.chat.connect({userId: this.qbUsuario.id, password: 'monitoraCovid19'}, (err, contactList) => {
          if(err){
            Notification({message: 'Não foi possível conectar no serviço de chat', type: 'error'});
            this.loading.chat = false
          } else {
            QB.users.get({login: `i-${row.cpf}`}, (err, user) => {
              if(err) {
                if(row.corStatus <= 1) {
                  Notification({message: `Só é possível iniciar bate-papo com cidadães que tenham classificação de risco acima de verde.`, type: 'info', duration: 10000});
                } else {
                  Notification({message: `Não foi possível iniciar o chat com ${row.nomeCompleto}`, type: 'error'});
                }
                this.loading.chat = false
              } else {
                // console.log({user}) Dados do individuo selecionado
                let asd = QB.chat.dialog.create({type: 3, occupants_ids: [user.id]}, (error, conversation) => {
                  if(error) {
                    Notification({message: `Não foi possível criar o chat com ${row.nomeCompleto}`, type: 'error'});
                    this.loading.chat = false
                  } else {
                    // console.log({conversation}) Dados da conversa
                    QB.chat.message.list({
                      chat_dialog_id: conversation._id,
                      limit: 100,
                      skip: 0
                    }, (error, messages) => {
                      if(error) {
                        Notification({message: `Não foi possível carregar o histórico de mensagem`, type: 'error'});
                        this.loading.chat = false
                      } else {
                        this.loading.chat = false
                        this.showModalChat = true
                        this.qbUserOpponent = user
                        this.qbDialog = conversation
                        this.qbMensagens = messages.items
                        QB.chat.onMessageListener = this.onRecievedNewMessage
                        setTimeout(() => { // Temporario
                          this.scrollChatToEnd()
                        }, 1000);
                      }
                    })
                  }
                })
              }
            })
          }
        })
      },
      onRecievedNewMessage(user, message) {
        if(message.dialog_id === this.qbDialog._id) {
          this.qbMensagens = [...this.qbMensagens, {...message, message: message.body, sender_id: user}]
          this.scrollChatToEnd()
        }
      },
      scrollChatToEnd () {
        jQuery('#containerChat').animate({
          scrollTop: jQuery(window).scrollTop() + jQuery('#containerChat').prop("scrollHeight")
        }, 1000)
      },
      qbSendMessage() {
        if(!this.formChatMessage.message) return;

        var message = {
          type: "chat",
          body: this.formChatMessage.message,
          extension: {
            save_to_history: 1,
            dialog_id: this.qbDialog._id
          },
          markable: 1
        };

        var opponentId = this.qbUserOpponent.id;
        var messageId = QB.chat.send(opponentId, message);
        this.qbMensagens = [...this.qbMensagens, {...message, message: this.formChatMessage.message, sender_id: this.qbUsuario.id}]
        this.formChatMessage.message = ''
        this.scrollChatToEnd()
      },
      qbFechatChat() {
        QB.chat.onMessageListener = () => {};
        QB.chat.disconnect();
      },
      qbIniciarChamada() {
        var calleesIds = [this.qbUserOpponent.id];
        var sessionType = QB.webrtc.CallType.AUDIO;
        var additionalOptions = {};
        var session = QB.webrtc.createNewSession(calleesIds, sessionType, null, additionalOptions);
        var mediaParams = {
          audio: true,
          video: false,
          options: {
            muted: false,
            mirror: false
          },
        };
        QB.webrtc.onCallListener = this.qbOnCallListener
        QB.webrtc.onUserNotAnswerListener = this.qbOnUserNotAnswerListener
        QB.webrtc.onAcceptCallListener = this.qbOnAcceptCallListener
        QB.webrtc.onRemoteStreamListener = this.qbOnRemoteStreamListener
        QB.webrtc.onRejectCallListener = this.qbOnRejectCallListener
        QB.webrtc.onStopCallListener = this.qbOnStopCallListener

        session.getUserMedia(mediaParams, (err, stream) => {
          if (err) {
            Notification({message: 'Para iniciar a ligação é necessário permitir o uso dos recursos solicitados.', type: 'error'});
          } else {
            var extension = {};
            session.call(extension, (error, asd, asdd) => {
              if(error) {
                Notification({message: 'Não foi possível iniciar a chamada.', type: 'error'});
              }
              else {
                this.sessionCall = session;
                this.inCall = true;
                this.isCalling = true;
              }
            });
          }
        });
      },
      qbDesligarChamada() {
        this.sessionCall.stop({});
        this.sessionCall = null;
        this.inCall = false;
        this.isCalling = false;
        this.isCall = false;
      },
      qbOnCallListener(session, extension) {
        this.sessionCall = session;
        this.inCall = true;
        this.isCalling = true;
        this.isCall = false;
      },
      qbOnUserNotAnswerListener(session, userId) {
        this.sessionCall = null;
        this.inCall = false;
        this.isCalling = false;
        this.isCall = false;
      },
      qbOnAcceptCallListener(session, userId, extension) {
        this.sessionCall = session;
        this.inCall = true;
        this.isCalling = false;
        this.isCall = true;
      },
      qbOnRemoteStreamListener(session, userID, remoteStream) { // Não esta sendo chamdao, é responsavel pelo Audio vindo doapp
        //console.log({qbOnRemoteStreamListener: {session, userId, remoteStream}})
        session.attachMediaStream("oppnentVideo", remoteStream);
        this.sessionCall = session;
      },
      qbOnRejectCallListener(session, userId, extension) {
        this.sessionCall = null;
        this.inCall = false;
        this.isCalling = false;
        this.isCall = false;
      },
      qbOnStopCallListener(session, userId, extension) {
        this.sessionCall = null;
        this.inCall = false;
        this.isCalling = false;
        this.isCall = false;
      },
      onEmitFromFiltro (val) {
        this.params = {
          ...this.params,
          ...val.params,
          page: 1
        }
        this.listando = true
        this.getIndividuos()
      },
      async getIndividuos () {
        this.loading.individuos = true
        let { data, paginacao, status } = await _api.individuos.get(this.params)
        if (data.length === 0) {
          return this.$swal({
            title: "Atenção!",
            text: 'Nenhum resultado encontrado com base nos filtros utilizados!',
            icon: 'warning',
          })
        }
        this.api.individuos = (status === 200) ? data : []
        this.params.page = (status === 200) ? paginacao.currentPage : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.individuos = false
      },
      async getUfs () {
        this.loading.ufs = true
        let { data, paginacao, status } = await _api.ufs.get(this.params.ufs)
        this.api.ufs = (status === 200) ? data : []
        if (this.api.ufs.length === 1) {
          this.formIndividuo.ufAbreviado = this.api.ufs[0].ufAbreviado
          this.getCidadesByUf()
        }
        this.params.ufs.skip = (status === 200) ? paginacao.currentPage : 0
        this.params.ufs.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.ufs = false
      },
      async getCidadesByUf () {
        this.loading.cidades = true
        let { data, paginacao, status } = await _api.cidades.get(this.params.cidades)
        this.api.cidades = (status === 200) ? data : []
        if (this.api.cidades.length === 1) {
          this.formIndividuo.cidade_Id = this.api.cidades[0].id
          this.getCidadesByUf()
        }
        this.params.cidades.skip = (status === 200) ? paginacao.currentPage : 0
        this.params.cidades.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.cidades = false
      },
      async onSelectUf (val) {
        this.params.cidades.ufAbreviado = val
        this.formIndividuo = {
          ...this.formIndividuo,
          cidade_Id: null
        }
        await this.getCidadesByUf()
      },
      async getMensagens () {
        this.loading.mensagens = true
        let params = { tipo: 'Encerramento' }
        let { data, status } = await _api.mensagens.get(params)
        this.api.mensagens = (status === 200) ? data : []
        this.loading.mensagens = false
      },
      diasQuarentena (dataInternacao) {
        if (dataInternacao) {
          return this.moment().diff(dataInternacao, 'days')
        } else {
          return '-'
        }
      },
      onClickNovo () {
        this.isDisabled = false
        this.listando = false
        this.metodo = 'POST'
        this.formIndividuo = {
          ...this.formIndividuo,
          corStatusAnterior: 1,
          origemCadastro: 'Retaguarda',
          ativo: true,
          confirmado: true,
          dataCadastro: this.moment()
        }
      },
      async onEditar (index, row) {
        this.metodo = 'PUT'
        this.isDisabled = false
        this.listando = false
        await this.onSelectUf(row.ufAbreviado)
        await this.getCidadesByUf()
        this.formIndividuo = {
          ...row,
          senhaConfirmacao: row.senha
        }
      },
      async onExcluir (index, row) {
        await _api.individuos.delete(row.id)
        this.getIndividuos()
      },
      onListar (form) {
        let i = this.api.individuos.findIndex(x => x.id === this.formIndividuo.id)
        this.$refs.tabela.setCurrentRow(this.api.individuos[i])
        this.$refs[form].resetFields()
        this.listando = true
      },
      onClickSalvar (form) {
        this.erros = []
        this.loading.individuos = true
        this.$refs[form].validate((valid) => {
          if (valid) {
            if (this.metodo === 'POST') {
              _api.individuos.post(this.formIndividuo).then(res => {
                if (res.status === 200) {
                  this.onListar(form)
                  this.getIndividuos()
                  this.loading.individuos = false
                }
                this.loading.individuos = false
              })
            } else {
              _api.individuos.put(this.formIndividuo).then(res => {
                if (res.status === 200) {
                  let i = this.api.individuos.findIndex(x => x.id === this.formIndividuo.id)
                  this.api.individuos[i] = res.data
                  this.$refs.tabela.setCurrentRow(res.data)
                  this.onListar(form)
                } else {
                  res.data.forEach(i => {
                    this.erros.push(i)
                  })
                  jQuery('.form--individuo').get(0).scrollIntoView()
                }
                this.loading.individuos = false
              })
            }
          } else {
            this.$swal({
              title: "Atenção!",
              text: 'Preencha os campos obrigatórios!',
              icon: 'warning',
            })
            this.loading.individuos = false
            return false
          }
        })
      },
      onSelectIndividuo (val) {
        this.listaIndividuos = val
      },
      onEnviarMensagem () {
        this.showModalMensagem = true
      },
      async doEnviarMensagem () {
        this.loading.notificacoes = true
        let individuos = []
        this.listaIndividuos.map(item => {
          individuos.push(
            {
              id: this.gerarGuid(),
              individuoCorona_Id: item.id,
              mensagem: this.formMensagem.mensagem,
              dataCadastro: this.moment()
            }
          )
        })
        await _api.notificacoes.post(individuos)
        this.loading.notificacoes = false
        this.limparSelecao()
        this.formMensagem.mensagem = ''
        this.showModalMensagem = false
      },
      async onVerificaAtendimento (row, e) {
        if (e) this.abrirLoading(e)
        let {data, status} = await _api.individuos.getById(row.id)
        if (status !== 200) {
          this.$swal({
            title: "Erro!",
            text: 'Paciente não encontrado!',
            icon: 'error',
          })
          this.onListar('formIndividuo')
        }
        this.individuo = data
        if (this.individuo.emAtendimentoPor) {
          if (this.individuo.emAtendimentoPor.id === this.$auth.user().id) {
            this.onAtendimento()
            return
          }
          this.showEmAtendimento = true
        } else {
          this.onAtendimento()
        }
        this.el_aguarde.close()
      },
      abrirLoading (e) {
        this.el_aguarde = this.$loading({
          text: 'Aguarde',
          target: e.path[2]
        })
      },
      onAtendimento () {
        this.$router.push({
          name: 'AtendimentoAtender',
          params: { individuo: this.individuo }
        })
        this.showModalAtendimento = true
      },
      async doInserirAtendimento () {
        this.loading.atendimento = true
        var atendimento = {
          id: this.gerarGuid(),
          individuoCorona_Id: this.formAtendimento.id,
          mensagem: this.formAtendimento.mensagem,
          dataCadastro: this.moment(),
          corStatus: this.formAtendimento.corStatus,
          corStatusAnterior: this.formAtendimento.corStatusAnterior
        }
        let {status } = await _api.atendimentos.post(atendimento)
        if (status === 201) {
          this.getIndividuos()
        }
        this.loading.atendimento = false
        this.showModalAtendimento = false
      },
      corRisco (val) {
        let cor = ''
        switch (val) {
          case 0:
            cor = 'grau--cinza'
            break
          case 1:
            cor = 'grau--verde'
            break
          case 2:
            cor = 'grau--amarelo'
            break
          case 3:
            cor = 'grau--laranja'
            break
          case 4:
            cor = 'grau--vermelho'
            break
          default:
            cor = 'grau--cinza'
            break
        }
        return cor
      },
      gerarGuid () {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
          let r = Math.random() * 16 | 0
          let v = c === 'x' ? r : (r & 0x3 | 0x8)
          return v.toString(16)
        })
      },
      limparSelecao (rows) {
        if (rows) {
          rows.forEach(row => {
            this.$refs.tabela.toggleRowSelection(row)
          })
        } else {
          this.$refs.tabela.clearSelection()
        }
      },
      resetForm (form) {
        this.$refs[form].resetFields()
      },
      handleSizeChange (val) {
        this.params.pageSize = val
      },
      handleCurrentChange (val) {
        this.params.currentPage = val
        this.getIndividuos()
      }
    }
  }
</script>

<style scoped>
  .grau--cinza {
    background: #C0C0C0 !important;
    border: 1px solid #a99696 !important;
    color: #000;
  }
  .grau--verde {
    background: #5cbf5c !important;
    border: 1px solid #5cbf5c !important;
  }
  .grau--amarelo {
    background: #FFFF66 !important;
    border: 1px solid #FFFF33 !important;
    color: #000;
  }
  .grau--laranja {
    background: #FF8000 !important;
    border: 1px solid #FF8000 !important;
    color: #000;
  }
  .grau--vermelho {
    background: #FF0000 !important;
    border: 1px solid #FF0000 !important;
    color: #FFF;
  }
  .qbMessageOwner {
    color: #fff212;
    background-color: #2B47A2;
    padding: 10px;
    margin: 10px 10px 10px 15%;
    border-radius: 18px;
    border-bottom-right-radius: 0;
  }
  .qbMessageOccupant {
    color: #fff212;
    background-color: #2B47A2;
    padding: 10px;
    margin: 10px 15% 10px 10px;
    border-radius: 18px;
    border-bottom-left-radius: 0;
  }

  .qbMessageP {
    color: white;
    margin-left: 5px;
    margin-bottom: 0;
  }

  #callinfo {
    display: flex;
    flex: 1;
    background-color: green;
    justify-items: center;
    align-items: center;
    padding: 10px 10px;
    color: white;
    border-radius: 8px;
  }

</style>
