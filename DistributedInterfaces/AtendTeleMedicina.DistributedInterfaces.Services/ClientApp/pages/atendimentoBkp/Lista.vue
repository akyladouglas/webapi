<template>
  <el-row :gutter="24">
    <el-col :span="4">
      <div style="display: flex;  flex-direction:column; margin-top: 15vh">
        <button class="botaoMenu" :class="{ active: telaSinopse }" @click="clickButtonSinopse">Sinopse</button>
        <button v-if="exibirAtendimento" class="botaoMenu" :class="{ active: telaAtendimento }" @click="clickButtonAtendimento">Atendimento</button>
        <button class="botaoMenu" :class="{ active: telaHistorico }" @click="clickButtonHistorico">Histórico</button>
        <button class="botaoMenu" :class="{ active: telaCadastroPaciente }" @click="clickButtonCadastro">Cadastro do Paciente</button>
      </div>
    </el-col>


    <el-col v-show="telaSinopse == true && telaAtendimento == false && telaHistorico == false && telaCadastroPaciente == false" :span="20" style="border-left: 1px solid #43bec6; ">

      <!--SINOPSE-->
      <!--<div v-if="alert" class="alert alert-warning container-fichas">
        <div style="display: flex; flex-direction: row-reverse">
          <el-button @click="close" class="" size="small" icon="el-icon-close"></el-button>
        </div>
        <h4 style="font-family: 'Roboto', arial, sans-serif;"><strong>Atenção Médico</strong></h4>
        <h4 style="font-family: 'Roboto', arial, sans-serif;"><strong>Ao sair desta tela de atendimento sem antes finalizar o mesmo, ocorrerá perda dos dados preenchidos nos campos!</strong></h4>
      </div>-->
      <!-- Dados Do Paciente No Header -->

      <div>

        <div style="display: flex; align-items: center; min-height: 15vh;">

          <div>
            <div v-if="api.individuo.imagem && error == false" :style="{ borderColor: getBorderPerfil(routeParams.agendamento) }">
              <img alt="Imagem de perfil indisponível no momento" @error="handleImageError" class="imagemPaciente" :style="{ borderColor: getBorderPerfil(routeParams.agendamento)}" :src="`../../${api.individuo.imagem}.jpg?${api.individuo.dataAlteracao}`" :title="api.individuo.nomeCompleto" />
            </div>

            <div v-else class="paciente__missing_photo" width="500px" :style="{ borderColor: getBorderPerfil(routeParams.agendamento) }">
              <img :src="'../../assets/img/' + $store.state.app.empresa.fotoUsuario"
                   :title="$store.state.app.empresa.nome"
                   id="image" />
            </div>

          </div>



          <div class="container-nome-paciente">
            <h1>{{api.individuo.nomeCompleto}} | {{api.individuo.sexo === 0 ? ' Masculino ' : ' Feminino '}}</h1>

            <p>
              <strong>Idade:&nbsp; </strong> {{getIdade(api.individuo.dataNascimento)}}  - &nbsp;
              <strong>CPF: &nbsp;</strong> {{api.individuo.cpf == undefined ? " Não preenchido " : api.individuo.cpf}} -&nbsp;
              <strong>Data de : &nbsp;</strong> {{api.individuo.dataNascimento ? moment(api.individuo.dataNascimento).format("DD/MM/YYYY") : "Não informado."}}
              <!--<strong>CNS: &nbsp;</strong>{{api.individuo.cns == undefined ? " Não preenchido " : api.individuo.cns}} - -->
              <!--<strong>Nome da Mãe: &nbsp;</strong> {{api.individuo.nomeDaMae == undefined ? " Não preenchido " : api.individuo.nomeDaMae}}-->
            </p>
          </div>

          <div style="display: flex;justify-content: flex-end;position: absolute;top: 20px;right: 30px;">
            <el-button v-if="exibirAtendimento == false" size="small" icon="fas fa-undo-alt" type="warning" @click="onClickVoltar"> Voltar</el-button>
          </div>
        </div>


        <div>
          <el-row>
            <!--LEFT CONTENT-->
            <el-col :span="16" style="min-height:100vh">
              <div class="grid-content">

                <!--QUEIXA DO PACIENTE TRIAGEM-->
                <div style="margin-top: 10px">
                  <el-card>
                    <span style="margin-left:5px; color:black; font-weight: bold">Triagem</span>
                    <br />
                    <span style="margin-left:5px;">{{atendimentoTriagem.queixaDoPaciente}}</span>
                  </el-card>
                </div>

                <!--Últimos Atendimentos Registrados-->
                <el-card v-show="listando && api.historicoSinopse.length === 0">
                  <el-empty v-show="listando && api.historicoSinopse.length === 0" description="Nenhum histórico encontrado"></el-empty>
                </el-card>

                <el-card style="margin-top:10px; padding-bottom: 10px">

                  <div>
                    <span v-show="api.historicoSinopse.length > 0" style="font-weight: bold; font-size: 15px;">Últimos Atendimentos</span>
                  </div>

                  <el-table v-show="api.historicoSinopse.length > 0" ref="tabela" :data="api.historicoSinopse"
                            highlight-current-row border
                            v-loading.body="loading.historicoSinopse"
                            :row-class-name="tableRow"
                            class="table--atendimento table--row-click"
                            :default-sort="{prop: 'dataCadastro', order: 'descending'}">
                    <el-table-column label="Data da Consulta" prop="dataCadastro" align="center" width="200" sortable>
                      <template slot-scope="scope">
                        {{moment(scope.row.dataCadastro).format('DD/MM/YYYY HH:mm')}}
                      </template>
                    </el-table-column>
                    <el-table-column label="Individuo" prop="individuo.nomeCompleto" align="center" />
                    <el-table-column label="Profissional" prop="profissional.nome" align="center" />
                    <el-table-column label="CRM" prop="profissional.crm" align="center" width="100" />
                  </el-table>
                  <el-col :span="24" v-show="listando">
                    <el-pagination @size-change="handleSizeChangeHistoricoSinopse"
                                   @current-change="handleCurrentChangeHistoricoSinopse"
                                   :current-page.sync="paramsHistoricoSinopse.page"
                                   :page-sizes="[5,10,25]"
                                   :page-size="10"
                                   :total="paramsHistoricoSinopse.total"
                                   layout="total, sizes, prev, pager, next, jumper">
                    </el-pagination>
                  </el-col>
                </el-card>

              </div>
            </el-col>
            <!--RIGHT CONTENT-->
            <el-col :span="8">
              <div class="grid-content bg-purple-light">
                <button class="botaoMenu" v-if="!loading.individuos" style="margin-top: 12px; margin-bottom: 0px; margin-left: 10px; width: 95%; display: flex; justify-content: flex-start;" size="large" @click="openModalMedicoes">Medições</button>
                <button class="botaoMenu" v-if="loading.individuos" disabled style="margin-top: 12px; margin-bottom: 0px; margin-left: 10px; width: 95%; display: flex; justify-content: flex-start;" size="large" @click="openModalMedicoes">Medições</button>

                <button class="botaoMenu" style="margin-top: 21px; margin-bottom: 0px; margin-left: 10px; width: 95%; display: flex; justify-content: flex-start; " size="large" @click="openModalComorbidades">Condições Autorreferidas / Condições Detectadas</button>
                <button class="botaoMenu" style="margin-top: 21px; margin-bottom: 0px; margin-left: 10px; width: 95%; display: flex; justify-content: flex-start; " size="large" @click="openDialogAlergias">Informações Adicionais</button>
                <button class="botaoMenu" style="margin-top: 21px; margin-bottom: 0px; margin-left: 10px; width: 95%; display: flex; justify-content: flex-start; " size="large" @click="openModalResultadoExames">Resultados de Exames</button>

              </div>
            </el-col>
          </el-row>
        </div>

      </div>
    </el-col>

    <el-col v-show="telaAtendimento == true && exibirAtendimento == true && telaSinopse == false && telaHistorico == false && telaCadastroPaciente == false " :span="19" style="border-left: 1px solid #43bec6">

      <!--ATENDIMENTO-->
      <div label="Atendimento">
        <el-card shadow="always">
          <el-descriptions class="margin-top" :column="3" border>
            <template slot="title">
              <!-- DADOS DE IMAGEM E GRAU DE RISCO -->
              <component v-if="exibirDadosHeaderAtendimento" :is="'dados-header-atendimento'" :individuo="api.individuo" :agendamento="agendamento" />
            </template>
            <template slot="extra">
              <stopwatch :running="cronometrando" :resetWhenStart="resetTimer" :flag="false" @emit="onEmitContador" />
              <!--<div v-if="!isDemandaEspontanea">
                <p>Tempo de Consulta:
                    <el-tag id="timer" style="font-size: 25px; font-weight: bold;"></el-tag>
                    <stopwatch :running="cronometrando" :resetWhenStart="false" :flag="false" />
                </p>
              </div>
              <div id="info-consulta"></div>-->
              <div>

                <el-row :gutter="20" type="flex" justify="center" >
                  <el-col>

                    <el-button @click="openModalVideo"
                               icon="el-icon-video-camera"
                               type="primary"
                               size="small"
                               id="btn-teleatendimento"
                               v-if="
                           !videoMinimizado
                           &&
                           !isMinimizado
                           &&
                           !emAndamentoVideoChamada">
                      TeleAtendimento
                    </el-button>

                  </el-col>

                  <el-col>

                    <el-button @click="openModalTytoCare"
                               icon="el-icon-video-camera"
                               type="primary"
                               size="small"
                               id="btn-teleatendimento"
                               v-show="!minimizadoTytoCare">
                      TytoCare
                    </el-button>

                  </el-col>
                  <el-col v-show="minimizadoTytoCare" >

                    <el-button
                             @click="returnModalTytoCare"
                             icon="el-icon-video-camera"
                             type="primary"
                             size="small"
                             id="btn-teleatendimento"
                             v-show="minimizadoTytoCare">
                  Retornar ao TytoCare
                  </el-button>

                    </el-col>
                  <el-col>

                    <el-button @click="openModalMemed"
                               type="primary"
                               size="small"
                               icon="el-icon-document-add">
                      Prescrição
                    </el-button>

                  </el-col>



                </el-row>
              </div>
            </template>
          </el-descriptions>

          <el-collapse style="margin-top: -20px" @change="openCollapse">
            <!--GRÁFICOS ATENDIMENTO-->
            <el-collapse-item title="Gráficos das medições" name="graficos">
              <component v-if="exibirGraficosNoAtendimento" :is="'graficos-das-medicoes'" :agendamento="agendamento" :origem="'Atendimento'" />
            </el-collapse-item>
            <!--SINAIS VITAIS ATENDIMENTO-->
            <el-collapse-item v-if="agendamento.pressaoSanguinea != undefined " title="Sinais Vitais" name="sinaisVitais">
              <div style="margin-bottom: 10px">
                <el-tag type="info">Data de envio: {{ moment(api.sinaisVitais.dataAlteracao).format('DD/MM/YYYY HH:mm') }}</el-tag>
              </div>
              <el-descriptions :column="4" border>
                <el-descriptions-item label="Pre. Arterial">
                  <el-tag size="small">{{ api.sinaisVitais.pressaoSanguinea && api.sinaisVitais.pressaoSanguinea + ' mmhg' }}</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Fre. Cardiaca">
                  <el-tag size="small">{{ api.sinaisVitais.batimentoCardiaco && api.sinaisVitais.batimentoCardiaco + ' bpm' }}</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Saturação O²">
                  <el-tag size="small">{{ api.sinaisVitais.oxigenacaoSanguinea && api.sinaisVitais.oxigenacaoSanguinea + ' %' }}</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Temperatura">
                  <el-tag size="small">{{ api.sinaisVitais.temperatura && api.sinaisVitais.temperatura + ' °C' }}</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Altura">
                  <el-tag size="small">{{ api.sinaisVitais.altura && api.sinaisVitais.altura + ' cm' }}</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Peso">
                  <el-tag size="small">{{ api.sinaisVitais.peso && api.sinaisVitais.peso + ' kg' }}</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="IMC">
                  <el-tag size="small">{{ api.sinaisVitais.dadosImc }}</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="GRAU IMC">
                  <el-tag size="small">{{  getGrauImc(api.sinaisVitais.dadosImc) }}</el-tag>
                </el-descriptions-item>
              </el-descriptions>
            </el-collapse-item>

            <!--SINTOMAS ATENDIMENTO-->
            <div class="container-sintomas">
              <p>Sintomas</p>
              <p class="data-envio">
                <el-tag type="info">Data de envio: {{ moment(api.comoMeSintoAtendimento.data).format('DD/MM/YYYY HH:mm') }}</el-tag>
              </p>

              <div v-if="api.comoMeSintoAtendimento.temperatura ||
                       api.comoMeSintoAtendimento.tosse ||
                       api.comoMeSintoAtendimento.coriza ||
                       api.comoMeSintoAtendimento.dorCorpo ||
                       api.comoMeSintoAtendimento.dorAbdominal ||
                       api.comoMeSintoAtendimento.fraqueza ||
                       api.comoMeSintoAtendimento.dorGarganta ||
                       api.comoMeSintoAtendimento.nauseaVomito ||
                       api.comoMeSintoAtendimento.dorCabeca ||
                       api.comoMeSintoAtendimento.sairCasa ||
                       api.comoMeSintoAtendimento.contatoPessoas ||
                       api.comoMeSintoAtendimento.dificuldadeRespirar ||
                       api.comoMeSintoAtendimento.taquicardia ||
                       api.comoMeSintoAtendimento.perdaOlfatoPaladar ||
                       api.comoMeSintoAtendimento.diarreia ||
                       api.comoMeSintoAtendimento.temperaturaRetornou ||
                       api.comoMeSintoAtendimento.atendidoServicoSaude ||
                       api.comoMeSintoAtendimento.sintomasGripais" class="list-sintomas">

                <div v-if="api.comoMeSintoAtendimento.temperatura"><span>Temperatura: <el-tag size="small" type="success">Sim</el-tag></span></div>
                <div v-if="api.comoMeSintoAtendimento.tosse"><span>Tosse: <el-tag size="small" type="success">Sim</el-tag></span></div>
                <div v-if="api.comoMeSintoAtendimento.coriza"><span>Coriza: <el-tag size="small" type="success">Sim</el-tag></span></div>
                <div v-if="api.comoMeSintoAtendimento.dorCorpo"><span>Dor no corpo: <el-tag size="small" type="success">Sim</el-tag></span></div>
                <div v-if="api.comoMeSintoAtendimento.dorAbdominal"><span>Dor abdominal: <el-tag size="small" type="success">Sim</el-tag></span></div>
                <div v-if="api.comoMeSintoAtendimento.fraqueza"><span>Fraqueza: <el-tag size="small" type="success">Sim</el-tag></span></div>
                <div v-if="api.comoMeSintoAtendimento.dorGarganta"><span>Dor na garganta: <el-tag size="small" type="success">Sim</el-tag></span></div>
                <div v-if="api.comoMeSintoAtendimento.nauseaVomito"><span>Náusea/ Vômito: <el-tag size="small" type="success">Sim</el-tag></span></div>
                <div v-if="api.comoMeSintoAtendimento.dorCabeca"><span>Dor de cabeça: <el-tag size="small" type="success">Sim</el-tag></span></div>
                <div v-if="api.comoMeSintoAtendimento.sairCasa"><span>Tem saído de casa: <el-tag size="small" type="success">Sim</el-tag></span></div>
                <div v-if="api.comoMeSintoAtendimento.contatoPessoas"><span>Contato com pessoas: <el-tag size="small" type="success">Sim</el-tag></span></div>
                <div v-if="api.comoMeSintoAtendimento.dificuldadeRespirar"><span>Dificuldade de respirar: <el-tag size="small" type="success">Sim</el-tag></span></div>
                <div v-if="api.comoMeSintoAtendimento.taquicardia"><span>Taquicardia: <el-tag size="small" type="success">Sim</el-tag></span></div>
                <div v-if="api.comoMeSintoAtendimento.diarreia"><span>Diarréia: <el-tag size="small" type="success">Sim</el-tag></span></div>
                <div v-if="api.comoMeSintoAtendimento.temperaturaRetornou"><span>Retornou A Ter Febre: <el-tag size="small" type="success">Sim</el-tag></span></div>
                <div v-if="api.comoMeSintoAtendimento.atendidoServicoSaude"><span>Atendido por serviço de saúde: <el-tag size="small" type="success">Sim</el-tag></span></div>
                <div v-if="api.comoMeSintoAtendimento.perdaOlfatoPaladar"><span>Perda de olfato ou paladar: <el-tag size="small" type="success">Sim</el-tag></span></div>
                <div v-if="api.comoMeSintoAtendimento.sintomasGripais"><span>Sintomas gripais: <el-tag size="small" type="success">Sim</el-tag></span></div>
              </div>
              <div v-else>
                <p>Paciente sem sintomas</p>
              </div>
            </div>
          </el-collapse>
        </el-card>
        <component v-if="exibirFichaAtendimentoUbs && mxHasAccess('Médico') == true && mxHasAccess('MédicoAD') == false" :is="'ficha-atendimento-ubs'" :agendamentoProps="agendamento" @emit="eventRetornoFichaAtendimento" />
        <component v-if="exibirFichaAtendimentoDomiciliar && mxHasAccess('Médico') == false && mxHasAccess('MédicoAD') == true" :is="'ficha-atendimento-domiciliar'" :agendamentoProps="agendamento" @emit="eventRetornoFichaAtendimento" />

      </div>
      <!--<full :isMinimizado="isMinimizado" :emAndamentoVideoChamada="emAndamentoVideoChamada"></full>-->
    </el-col>

    <el-col v-show="telaHistorico == true && telaAtendimento == false && telaSinopse == false && telaCadastroPaciente == false" :span="19" style="border-left: 1px solid #43bec6; margin-top: 20px; min-height:100vh">
      <component v-if="exibirMenuHistorico" :is="'historico-no-atendimento'" :agendamento="agendamento" />
    </el-col>

    <el-col v-show="telaCadastroPaciente == true && telaHistorico == false && telaAtendimento == false && telaSinopse == false " :span="19" style="border-left: 1px solid #43bec6">
      <div style="margin-top:20px">
        <el-card>
          <el-row type="flex" :span="24" style="padding:15px;">
            <el-col :span="12">
              <h2 class="box-card--h2"> Dados Cadastrais </h2>

            </el-col>
            <el-col :span="12" align="right">
              <el-button type="primary" v-show="!editarPaciente" @click="editarPaciente = !editarPaciente">Editar</el-button>
              <el-button type="warning" v-show="editarPaciente" @click="editarPaciente = !editarPaciente">Voltar</el-button>
            </el-col>
          </el-row>

          <el-row>
            <el-form :model="formIndividuoCadastro" status-icon :rules="validacoes" :disabled="!editarPaciente"
                     ref="formIndividuoCadastro" label-width="120px" label-position="top" class="form--individuo">
              <el-row :gutter="24">
                <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="15">
                  <el-form-item label="Nome Completo" prop="nomeCompleto">
                    <el-input maxlength="100" show-word-limit v-model="formIndividuoCadastro.nomeCompleto" />
                  </el-form-item>
                </el-col>

                <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="15">
                  <el-form-item label="Nome Social" prop="nomeSocial">
                    <el-input maxlength="100" show-word-limit v-model="formIndividuoCadastro.nomeSocial" />
                  </el-form-item>
                </el-col>

                <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="15" v-if="!formIndividuoCadastro.temMaeDesconhecida">
                  <el-form-item label="Nome da Mãe" prop="nomeDaMae">
                    <el-input maxlength="100" show-word-limit v-model="formIndividuoCadastro.nomeDaMae" />
                  </el-form-item>
                </el-col>
              </el-row>

              <el-row :gutter="24">
                <el-col :xs="24" :sm="24" :md="12" :lg="10" :xl="10">
                  <el-form-item label="Email" prop="email">
                    <el-input maxlength="80" show-word-limit v-model="formIndividuoCadastro.email" />
                  </el-form-item>
                </el-col>

                <el-col :xs="24" :sm="24" :md="12" :lg="7" :xl="10">
                  <el-form-item label="CPF" prop="cpf">
                    <el-input show-word-limit v-model="formIndividuoCadastro.cpf" masked="true" maxlength="14" v-mask="'###.###.###-##'" />
                  </el-form-item>
                </el-col>

                <el-col :xs="24" :sm="24" :md="12" :lg="7" :xl="10">
                  <el-form-item label="CNS" prop="cns">
                    <el-input v-model="formIndividuoCadastro.cns" masked="true" maxlength="15" show-word-limit />
                  </el-form-item>
                </el-col>
              </el-row>

              <!--<el-row :gutter="24">

                <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="10">
                  <el-form-item label="Senha" prop="senha">
                    <el-input v-model="formIndividuoCadastro.senha" type="password" autocomplete="off" />
                  </el-form-item>
                </el-col>

                <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="10">
                  <el-form-item label="Confirmação da senha" prop="confirmacao">
                    <el-input v-model="formIndividuoCadastro.confirmacao" type="password" autocomplete="off" />
                  </el-form-item>
                </el-col>

              </el-row>-->

              <el-row :gutter="24">
                <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="10">
                  <el-form-item label="Sexo" prop="sexo">
                    <el-select filterable placeholder="Selecione o Sexo" v-model="formIndividuoCadastro.sexo">
                      <el-option v-for="option in enums.sexos" :value="option.value" :label="option.label" :key="option.value" />
                    </el-select>
                  </el-form-item>
                </el-col>

                <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="10">
                  <el-form-item label="Raça ou Cor" prop="racaOuCor">
                    <el-select filterable placeholder="Selecione a Raça ou Cor" v-model="formIndividuoCadastro.racaOuCor">
                      <el-option v-for="option in enums.racaOuCor" :value="option.value" :label="option.label" :key="option.value" />
                    </el-select>
                  </el-form-item>
                </el-col>
              </el-row>

              <el-row :gutter="24">
                <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="10">
                  <el-form-item label="Data Nascimento" prop="dataNascimento">
                    <el-date-picker prefix-icon="fas fa-calendar-day" v-model="formIndividuoCadastro.dataNascimento" type="date" format="dd-MM-yyyy" />
                  </el-form-item>
                </el-col>

                <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="10">
                  <el-form-item label="Telefone Celular" prop="telefoneCelular">
                    <el-input v-model="formIndividuoCadastro.telefoneCelular" show-word-limit masked="true" maxlength="15" v-mask="'(##) #####-####'">
                      <i slot="prefix" class="fas fa-mobile-alt"></i>
                    </el-input>
                  </el-form-item>
                </el-col>
              </el-row>

              <el-row :gutter="24">
                <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="10">
                  <el-form-item label="CEP" prop="logradouroCep">
                    <el-input v-model="formIndividuoCadastro.logradouroCep" show-word-limit @input="getCep" masked="true" maxlength="9" v-mask="'#####-###'" />
                  </el-form-item>
                </el-col>

                <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="10">
                  <el-form-item label="Estado" prop="ufAbreviado">
                    <el-select filterable placeholder="Selecione o Estado" v-model="formIndividuoCadastro.ufAbreviado"
                               v-loading.body="loading.ufs" @change="onSelectUf">
                      <el-option v-for="option in api.ufs" :value="option.ufAbreviado" :label="option.ufExtenso" :key="option.ufAbreviado" />
                    </el-select>
                  </el-form-item>
                </el-col>
              </el-row>

              <el-row :gutter="24">
                <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="10">
                  <el-form-item label="Cidade" prop="cidadeId">
                    <el-select filterable placeholder="Selecione a Cidade" v-model="formIndividuoCadastro.cidadeId"
                               v-loading.body="loading.cidades">
                      <el-option v-for="option in api.cidades" :value="option.ibge" :label="option.nome" :key="option.ibge" />
                    </el-select>
                  </el-form-item>
                </el-col>

                <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="10">
                  <el-form-item label="Bairro" prop="logradouroBairro">
                    <el-input v-model="formIndividuoCadastro.logradouroBairro" />
                  </el-form-item>
                </el-col>
              </el-row>

              <el-row :gutter="24">
                <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="10">
                  <el-form-item label="Endereço" prop="logradouro">
                    <el-input v-model="formIndividuoCadastro.logradouro" />
                  </el-form-item>
                </el-col>

                <el-col :xs="24" :sm="24" :md="12" :lg="4" :xl="3">
                  <el-form-item label="Número" prop="logradouroNumero">
                    <el-input type="number" v-model="formIndividuoCadastro.logradouroNumero" show-word-limit />
                  </el-form-item>
                </el-col>

                <el-col :xs="24" :sm="24" :md="12" :lg="8" :xl="7">
                  <el-form-item label="Complemento" prop="logradouroComplemento">
                    <el-input v-model="formIndividuoCadastro.logradouroComplemento" show-word-limit maxlength="30" />
                  </el-form-item>
                </el-col>
              </el-row>

              <el-row :gutter="20">
                <el-col :xs="24">
                  <el-form-item>
                    <el-button flat icon="fas fa-save" type="success" @click="onClickSalvarIndividuo('formIndividuoCadastro')" :loading="loading.individuos" :disabled="!editarPaciente"> Salvar</el-button>

                    <!--<el-button flat icon="fas fa-eraser" type="default" @click="" :disabled="loading.individuos"> Limpar</el-button>-->
                  </el-form-item>
                </el-col>
              </el-row>
            </el-form>
          </el-row>
        </el-card>
      </div>
    </el-col>
    <footer class="barra">

      <!--<el-button v-if="isMinimizado" style="height:48px; width:49px" slot="reference" type="primary" icon="el-icon-video-camera" circle @click="maximizar"></el-button>-->

    </footer>
    <!-- MODAL MEMED -->
    <modal name="modalMemed" :draggable="true" @opened="startMemed" @closed="closeMemed" :clickToClose="false" :width="1020" :height="690">
      <div class="window-header">
        <el-button type="danger" @click="hideModalMemed" icon="el-icon-close"></el-button>
      </div>
      <VuePerfectScrollbar id="pscroll" class="scroll-area-memed" :settings="scrollSettings" key="scroll-memed">
        <prescricao-memed v-if="usePrescricao && modalMemedOpen" :paciente="api.individuo" :idAgendamento="agendamento.id" :profissional="api.profissional" />
      </VuePerfectScrollbar>
    </modal>

    <!-- MODAL TELEATENDIMENTO-->
    <modal name="teleatendimento" :class="{ modalVideoMinizado: isMinimizado }" :resizable="true" :draggable="true" :clickToClose="false" :min-width="280" :min-height="520" :max-width="700" :max-height="520" :width="700" :height="450" :adaptive="true">
      <div style="margin-bottom: 20px; position:absolute !important; right:0 !important; z-index:9;">
        <el-button size="small" @click="minimizar" icon="el-icon-minus" />
        <el-popconfirm title="Você tem certeza que deseja encerrar a chamada?"
                       confirm-button-text='Sim'
                       confirmButtonType="danger"
                       cancel-button-text='Não'
                       icon="el-icon-info"
                       icon-color="red"
                       @confirm="hideDialogVideo">
          <el-button size="small"
                     icon="el-icon-close"
                     type="danger"
                     slot="reference">
          </el-button>
        </el-popconfirm>
      </div>

      <el-tabs type="border-card" @tab-click="handleTabClickTeleAtendimento">
        <el-tab-pane label="Video">
          <div class="remote_video_container">
            <div id="remoteTrack"></div>
          </div>
          <div class="local_video_container">
            <div id="localTrack"></div>
          </div>
          <div style="position: absolute; bottom: 40px; left: 30px">
            <el-button type="danger" v-if="!twilio.micEnabled" icon="el-icon-microphone" @click="onMicrophoneEnable" size="small" />
            <el-button type="success" v-if="twilio.micEnabled" icon="el-icon-microphone" @click="onMicrophoneDisable" size="small" />
            <el-button type="danger" v-if="!twilio.camEnabled" icon="el-icon-camera-solid" @click="onVideoEnable" size="small" />
            <el-button type="success" v-if="twilio.camEnabled" icon="el-icon-camera-solid" @click="onVideoDisable" size="small" />
            <el-popconfirm title="Você tem certeza que deseja encerrar a chamada?"
                           confirm-button-text='Sim'
                           confirmButtonType="danger"
                           cancel-button-text='Não'
                           icon="el-icon-info"
                           icon-color="red"
                           @confirm="hideDialogVideo">
              <el-button size="small"
                         type="danger"
                         slot="reference">
                Encerrar
              </el-button>
            </el-popconfirm>
          </div>
        </el-tab-pane>
        <el-tab-pane name="chat" label="Chat">
          <div id="chat">
            <el-row v-if="twilio.chatMessages.length > 0">
              <el-col :span="24">
                <VuePerfectScrollbar id="pscroll" class="scroll-area--chat" :settings="scrollSettings" key="scrol-atendimento">
                  <ul id="conversation" class="list-unstyled mt-2">
                    <li v-for="(message, i) of twilio.chatMessages" :key="i" :class="message.memberSid === agendamento.individuoId ? 'msg__paciente' : 'msg__chat'">
                      <span>
                        {{message.body}}
                        <i class="msg__hora">{{ moment(message.createdDate).format('HH:mm') }}</i>
                      </span>
                    </li>
                  </ul>
                </VuePerfectScrollbar>
              </el-col>
            </el-row>
            <el-row v-else class="scroll-area--chat">
              <el-empty description="Nenhum histórico de mensagens" />
            </el-row>
            <el-row :gutter="20">
              <el-col :span="21">
                <el-input v-model="chatMessage" @keyup.enter.native="onSendMessage" />
              </el-col>
              <el-col :span="3">
                <el-button icon="el-icon-s-promotion" :disabled="!chatMessage" type="success" class="chat--button" @click="onSendMessage" />
              </el-col>
            </el-row>
          </div>
        </el-tab-pane>
      </el-tabs>
    </modal>


    <modal v-show="showTytoCare" name="tytocare" :draggable="true" :clickToClose="false" :width="width" :height="height">
      <div style="margin-bottom: 20px; position:absolute !important; right:0 !important; z-index:9;">
        <el-button size="small" @click="minimizarModalTytoCare" icon="el-icon-minus" />
        <el-button size="small" icon="el-icon-close" type="danger" slot="reference" @click="closeModalTytoCare"></el-button>
      </div>
      <iframe id="myIframe" src="https://cloud.tytocare.com/" width="100%" height="100%" allowfullscreen allow="camera; microphone"></iframe>
    </modal>

    <!-- MODAL MEDIÇÕES INICIO -->
    <modal name="modalMedicoes" :resizable="false" :draggable="true" :clickToClose="false" width="1000" height="600">
      <VuePerfectScrollbar class="scroll-areaModalMedicoes" :settings="scrollSettings" key="scrol-atendimento">
        <div class="window-header" style="padding: 5px;display: flex;justify-content: flex-end;">
          <el-button style="width:40px; display:flex; align-content:space-around; justify-content:center" type="danger" @click="hideModalMedicoes" icon="el-icon-close"></el-button>
        </div>
        <el-row style="margin-left:5px; ">
          <h2 style="padding-bottom: 5px" class="box-card--h2"> Histórico de Medições</h2>
        </el-row>
        <component v-show="exibirGraficosNoModalMedicoes" :is="'graficos-das-medicoes'" :agendamento="agendamento" :origem="'Modal'" />
      </VuePerfectScrollbar>
    </modal>
    <!-- MODAL MEDIÇÕES FIM -->
    <!-- MODAL COMORBIDADES INICIO-->
    <modal name="modalComorbidades" :resizable="false" :draggable="true" :clickToClose="false" width="800" height="550">
      <VuePerfectScrollbar class="scroll-areaModalMedicoes" :settings="scrollSettings" key="scrol-atendimento">
        <div class="window-header" style="padding: 5px;display: flex;justify-content: flex-end;">
          <el-button style="width:40px; display:flex; align-content:space-around; justify-content:center" type="danger" @click="hideModalComorbidades" icon="el-icon-close"></el-button>
        </div>
        <el-row style="padding: 10px">
          <el-col :span="12">
            <h5>Comorbidades Autorreferidas</h5>
            <div style="margin-bottom: 10px">
              <span v-if="api.comorbidades.dataAlteracao" size="small">
                <el-tag type="info">Data de envio: {{ moment(api.comorbidades.dataAlteracao).format('DD/MM/YYYY HH:mm') }}</el-tag>
              </span>
            </div>
            <div v-if="api.comorbidades">
              <div v-if="api.comorbidades.hipertenso">
                -                <div v-if="api.comorbidades.hipertenso"><el-tag size="small" type="success">Hipertenso</el-tag></div>
              </div>
              <div v-if="api.comorbidades">
                -                <div v-if="api.comorbidades.diabetes"><el-tag size="small" type="success">Diabetes</el-tag></div>
              </div>
              <div v-if="api.comorbidades">
                -                <div v-if="api.comorbidades.fumante"><el-tag size="small" type="success">Fumante</el-tag></div>
              </div>
              <div v-if="api.comorbidades">
                -                <div v-if="api.comorbidades.asma"><el-tag size="small" type="success">Asma</el-tag></div>
              </div>
              <div v-if="api.comorbidades">
                -                <div v-if="api.comorbidades.doencaPulmao"><el-tag size="small" type="success">Doença no Pulmão</el-tag></div>
              </div>
              <div v-if="api.comorbidades">
                -                <div v-if="api.comorbidades.doencaCoracao"><el-tag size="small" type="success">Doença no Coração</el-tag></div>
              </div>
              <div v-if="api.comorbidades">
                -                <div v-if="api.comorbidades.doencaCancer"><el-tag size="small" type="success">Câncer</el-tag></div>
              </div>
              <div v-if="api.comorbidades">
                -                <div v-if="api.comorbidades.transplantado"><el-tag size="small" type="success">Transplantado</el-tag></div>
              </div>
              <div v-if="api.comorbidades">
                -                <div v-if="api.comorbidades.doencaComprometeImunidade"><el-tag size="small" type="success">Doença que compromete imunidade</el-tag></div>
              </div>
              <div v-if="api.comorbidades">
                -                <div v-if="api.comorbidades.doencaRins"><el-tag size="small" type="success">Doença Nos Rins</el-tag></div>
              </div>
              <div v-if="api.comorbidades">
                -                <div v-if="api.comorbidades.doencaFigado"><el-tag size="small" type="success">Doença No Figado</el-tag></div>
              </div>

              <!-- Exibição de duas respostas nas comorbidades -->
              <!-- <div v-if="api.comorbidades">
                <div >
                  <el-tag size="small" type="success" v-if="api.comorbidades.doencaFigado.respostaPaciente">Doença No Figado</el-tag>
                  <el-tag style="margin-left: 8px;" v-if="api.comorbidades.doencaFigado.respostaProfissional " size="small" type="danger">Profissional afirma Doença No Figado</el-tag>
                </div>
              </div> -->
            </div>
          </el-col>
          <el-col :span="12">
            <h5>Identificadas pelo Médico</h5>
            <!--PErfil Prontuário: routeParams.preVisualizar-->
            <el-form>
              <div>
                <el-form-item label="Hipertenso">
                  <el-radio-group @change="onChangeComorbidades" fill="#015f6b" v-model="formComorbidades.hipertenso" :disabled="disabledComorbidades">
                    <el-radio-button :label="true">Sim</el-radio-button>
                    <el-radio-button :label="false">Não</el-radio-button>
                  </el-radio-group>
                </el-form-item>
              </div>
              <div>
                <el-form-item label="Diabetes">
                  <el-radio-group @change="onChangeComorbidades" fill="#015f6b" v-model="formComorbidades.diabetes" :disabled="disabledComorbidades">
                    <el-radio-button :label="true">Sim</el-radio-button>
                    <el-radio-button :label="false">Não</el-radio-button>
                  </el-radio-group>
                </el-form-item>
              </div>
              <div>
                <el-form-item label="Fumante">
                  <el-radio-group @change="onChangeComorbidades" fill="#015f6b" v-model="formComorbidades.fumante" :disabled="disabledComorbidades">
                    <el-radio-button :label="true">Sim</el-radio-button>
                    <el-radio-button :label="false">Não</el-radio-button>
                  </el-radio-group>
                </el-form-item>
              </div>
              <div>
                <el-form-item label="Asma">
                  <el-radio-group @change="onChangeComorbidades" fill="#015f6b" v-model="formComorbidades.asma" :disabled="disabledComorbidades">
                    <el-radio-button :label="true">Sim</el-radio-button>
                    <el-radio-button :label="false">Não</el-radio-button>
                  </el-radio-group>
                </el-form-item>
              </div>
              <div>
                <el-form-item label="Doença no Pulmão">
                  <el-radio-group @change="onChangeComorbidades" fill="#015f6b" v-model="formComorbidades.doencaPulmao" :disabled="disabledComorbidades">
                    <el-radio-button :label="true">Sim</el-radio-button>
                    <el-radio-button :label="false">Não</el-radio-button>
                  </el-radio-group>
                </el-form-item>
              </div>
              <div>
                <el-form-item label="Doença no Coração">
                  <el-radio-group @change="onChangeComorbidades" fill="#015f6b" v-model="formComorbidades.doencaCoracao" :disabled="disabledComorbidades">
                    <el-radio-button :label="true">Sim</el-radio-button>
                    <el-radio-button :label="false">Não</el-radio-button>
                  </el-radio-group>
                </el-form-item>
              </div>
              <div>
                <el-form-item label="Câncer">
                  <el-radio-group @change="onChangeComorbidades" fill="#015f6b" v-model="formComorbidades.doencaCancer" :disabled="disabledComorbidades">
                    <el-radio-button :label="true">Sim</el-radio-button>
                    <el-radio-button :label="false">Não</el-radio-button>
                  </el-radio-group>
                </el-form-item>
              </div>
              <div>
                <el-form-item label="Transplantado">
                  <el-radio-group @change="onChangeComorbidades" fill="#015f6b" v-model="formComorbidades.transplantado" :disabled="disabledComorbidades">
                    <el-radio-button :label="true">Sim</el-radio-button>
                    <el-radio-button :label="false">Não</el-radio-button>
                  </el-radio-group>
                </el-form-item>
              </div>
              <div>
                <el-form-item label="Doença Compromete Imunidade">
                  <el-radio-group @change="onChangeComorbidades" fill="#015f6b" v-model="formComorbidades.doencaComprometeImunidade" :disabled="disabledComorbidades">
                    <el-radio-button :label="true">Sim</el-radio-button>
                    <el-radio-button :label="false">Não</el-radio-button>
                  </el-radio-group>
                </el-form-item>
              </div>
              <div>
                <el-form-item label="Doença nos Rins">
                  <el-radio-group @change="onChangeComorbidades" fill="#015f6b" v-model="formComorbidades.doencaRins" :disabled="disabledComorbidades">
                    <el-radio-button :label="true">Sim</el-radio-button>
                    <el-radio-button :label="false">Não</el-radio-button>
                  </el-radio-group>
                </el-form-item>
              </div>
              <div>
                <el-form-item label="Doença no Fígado">
                  <el-radio-group @change="onChangeComorbidades" fill="#015f6b" v-model="formComorbidades.doencaFigado" :disabled="disabledComorbidades">
                    <el-radio-button :label="true">Sim</el-radio-button>
                    <el-radio-button :label="false">Não</el-radio-button>
                  </el-radio-group>
                </el-form-item>
              </div>
            </el-form>

          </el-col>
        </el-row>
      </VuePerfectScrollbar>
    </modal>
    <!-- MODAL COMORBIDADES FIM-->
    <!-- MODAL EXAMES INICIO-->
    <modal name="modalResultadoExames" :resizable="false" :draggable="true" :clickToClose="false" width="1000" height="600">

      <div class="window-header" style="padding: 5px;display: flex;justify-content: flex-end;">
        <el-button style="width:40px; display:flex; align-content:space-around; justify-content:center" type="danger" @click="hideModalResultadoExames" icon="el-icon-close"></el-button>
      </div>
      <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
        <div>
          <h2 style="padding-bottom: 5px" class="box-card--h2"> Resultados de exames</h2>
        </div>
        <el-card>
          <el-scrollbar wrap-style="max-height: 500px;">
            <component v-if="exibirExamesTabsModal" :is="'exames-tabs'" :agendamento="agendamento" @emit="retornoExamesTabs" />
          </el-scrollbar>
        </el-card>
      </el-col>
    </modal>
    <!-- MODAL EXAMES FIM-->
    <!--Dialog Alergias-->
    <el-dialog title="" :visible.sync="dialogVisible" width="50%" :before-close="hideModalAlergias">
      <!--Title-->
      <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
        <h2 style="padding-bottom: 5px" class="box-card--h2">Informações Adicionais</h2>
      </el-col>
      <!--Form-->
      <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" style="margin-top: 20px">
        <el-form :model="formAlergiasEdit" status-icon ref="formAlergiasEdit" label-width="120px" label-position="top" class="forms--usuario">
          <el-form-item label="Alergias" prop="alergias">
            <el-input v-if="!exibirAtendimento" type="textarea" v-model="formAlergiasEdit.alergias" disabled />
            <el-input v-else type="textarea" v-model="formAlergiasEdit.alergias" />

          </el-form-item>

          <el-form-item label="Antecedentes Familiares" prop="antecedentes">
            <el-input v-if="!exibirAtendimento" type="textarea" show-word-limit v-model="formAlergiasEdit.antecedentes" disabled />
            <el-input v-else type="textarea" show-word-limit v-model="formAlergiasEdit.antecedentes" />
          </el-form-item>

        </el-form>

      </el-col>
      <span slot="footer" class="dialog-footer">
        <el-button @click="dialogVisible = false" size="small">Cancelar</el-button>
        <el-button type="primary" size="small" style="margin-top: 10px" v-if="exibirAtendimento" @click="onClickAlergias(formAlergiasEdit)">Salvar</el-button>
      </span>
    </el-dialog>
    <!-- FIM MODAL ALERGIAS-->
    <!-- MODAL CADASTRO PACIENTE-->
    <modal name="modalCadastroPaciente" :resizable="false" :draggable="true" :clickToClose="false" width="1300" height="600">
      <VuePerfectScrollbar class="scroll-areaModalMedicoes" :settings="scrollSettings" key="scrol-atendimento">
        <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
          <h2 style="padding-bottom: 5px" class="box-card--h2"> Cadastro do Paciente</h2>
        </el-col>
        <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" v-if="api.individuo">
          <el-row>
            <el-col :xs="12" :sm="12" :md="3" :lg="3" :xl="3">

              <div v-if="agendamento.corStatusTriagem === 1">
                <div v-if="api.individuo.imagem">
                  <img alt="Imagem" style="width: 110px; height: 110px; object-fit: cover; border-radius: 100%" :src="`../../${api.individuo.imagem}.jpg?${api.individuo.dataAlteracao}`" :title="api.individuo.nomeCompleto" />
                </div>

                <div v-else class="paciente__missing_photo">
                  <div width="500px">
                    <img :src="'../../assets/img/' + $store.state.app.empresa.fotoUsuario"
                         :title="$store.state.app.empresa.nome"
                         id="image" />
                  </div>
                </div>

                <el-descriptions title="" direction="vertical" :column="4" border style="margin-top:20px">
                  <el-descriptions-item label="Não Urgente" label-class-name="azul">
                    <!--<p>Caso com menor risco de problema respiratório</p>-->
                  </el-descriptions-item>
                </el-descriptions>
              </div>

              <div v-if="agendamento.corStatusTriagem === 2">
                <img v-if="api.individuo.imagem" alt="Imagem" style="width: 110px; height: 110px; object-fit: cover; border-radius: 100%" :src="`../../${api.individuo.imagem}.jpg?${api.individuo.dataAlteracao}`" :title="api.individuo.nomeCompleto" />
                <div v-else class="paciente__missing_photo">
                  <div width="500px">
                    <img :src="'../../assets/img/' + $store.state.app.empresa.fotoUsuario"
                         :title="$store.state.app.empresa.nome"
                         id="image" />
                  </div>
                </div>
                <el-descriptions title="" direction="vertical" :column="4" border style="margin-top:20px">
                  <el-descriptions-item label="Pouco Urgente" label-class-name="verde">
                    <!--<p style="font-size:12px">Caso suspeito de problema respiratório</p>-->
                  </el-descriptions-item>
                </el-descriptions>
              </div>

              <div v-if="agendamento.corStatusTriagem === 3">
                <img v-if="api.individuo.imagem" alt="Imagem" style="width: 110px; height: 110px; object-fit: cover; border-radius: 100%" :src="`../../${api.individuo.imagem}.jpg?${api.individuo.dataAlteracao}`" :title="api.individuo.nomeCompleto" />
                <div v-else class="paciente__missing_photo">
                  <div width="500px">
                    <img :src="'../../assets/img/' + $store.state.app.empresa.fotoUsuario"
                         :title="$store.state.app.empresa.nome"
                         id="image" />
                  </div>
                </div>
                <el-descriptions title="" direction="vertical" :column="4" border style="margin-top:20px">
                  <el-descriptions-item label="Urgente" label-class-name="amarelo">
                    <!--<p style="font-size:12px">Caso Suspeito – Sintomático leve com risco mais elevado problema respiratório</p>-->
                  </el-descriptions-item>
                </el-descriptions>
              </div>

              <div v-if="agendamento.corStatusTriagem === 4">
                <img v-if="api.individuo.imagem" alt="Imagem" style="width: 110px; height: 110px; object-fit: cover; border-radius: 100%" :src="`../../${api.individuo.imagem}.jpg?${api.individuo.dataAlteracao}`" :title="api.individuo.nomeCompleto" />
                <div v-else class="paciente__missing_photo">
                  <div width="500px">
                    <img :src="'../../assets/img/' + $store.state.app.empresa.fotoUsuario"
                         :title="$store.state.app.empresa.nome"
                         id="image" />
                  </div>
                </div>
                <el-descriptions title="" direction="vertical" :column="4" border style="margin-top: 20px">
                  <el-descriptions-item label="Muito Urgente" label-class-name="laranja" style="background-color: #e63946">
                    <!--<p style="font-size:12px">Sintomatologia grave ou agravando para problema respiratório</p>-->
                  </el-descriptions-item>
                </el-descriptions>
              </div>

              <div v-if="agendamento.corStatusTriagem === 5">
                <img v-if="api.individuo.imagem" alt="Imagem" style="width: 110px; height: 110px; object-fit: cover; border-radius: 100%" :src="`../../${api.individuo.imagem}.jpg?${api.individuo.dataAlteracao}`" :title="api.individuo.nomeCompleto" />
                <div v-else class="paciente__missing_photo">
                  <div width="500px">
                    <img :src="'../../assets/img/' + $store.state.app.empresa.fotoUsuario"
                         :title="$store.state.app.empresa.nome"
                         id="image" />
                  </div>
                </div>
                <el-descriptions title="" direction="vertical" :column="4" border style="margin-top: 20px; ">
                  <el-descriptions-item label="Emergência" label-class-name="vermelho" />
                  <!--<p style="font-size:12px">Sintomatologia grave ou agravando para problema respiratório</p>-->
                </el-descriptions>
              </div>
            </el-col>
          </el-row>
          <el-descriptions class="margin-top" :column="4" style="margin-top:30px" border>
            <el-descriptions-item>
              <template slot="label">
                Nome
              </template>
              {{api.individuo.nomeCompleto ? api.individuo.nomeCompleto : 'Não informado' }}
            </el-descriptions-item>
            <el-descriptions-item>
              <template slot="label">
                Cpf
              </template>
              {{api.individuo.cpf ? api.individuo.cpf : 'Não informado' }}
            </el-descriptions-item>
            <el-descriptions-item>
              <template slot="label">
                Sexo
              </template>
              {{api.individuo.sexo === 0 ? 'Masculino' : 'Feminino'}}
            </el-descriptions-item>
            <el-descriptions-item>
              <template slot="label">
                Idade
              </template>
              {{moment().diff(this.api.individuo.dataNascimento, 'years')}}
            </el-descriptions-item>
            <el-descriptions-item>
              <template slot="label">
                Celular
              </template>
              {{api.individuo.telefoneCelular ? api.individuo.telefoneCelular : 'Não informado' }}
            </el-descriptions-item>
            <el-descriptions-item>
              <template slot="label">
                Email
              </template>
              {{api.individuo.email ? api.individuo.email : 'Não informado'}}
            </el-descriptions-item>
            <el-descriptions-item>
              <template slot="label">
                Nome da Mãe
              </template>
              {{api.individuo.nomeDaMae ? api.individuo.nomeDaMae : 'Não informado'}}
            </el-descriptions-item>
            <el-descriptions-item>
              <template slot="label">
                Data Nascimento
              </template>
              {{moment(this.api.individuo.dataNascimento, 'YYYY-MM-DD').format('DD/MM/YYYY')}}
            </el-descriptions-item>
            <el-descriptions-item>
              <template slot="label">
                Logradouro
              </template>
              {{api.individuo.logradouro ? api.individuo.logradouro : 'Não informado'}}
            </el-descriptions-item>
            <el-descriptions-item>
              <template slot="label">
                Número
              </template>
              {{api.individuo.logradouroNumero ? api.individuo.logradouroNumero : 'Não informado' }}
            </el-descriptions-item>
            <el-descriptions-item>
              <template slot="label">
                CEP
              </template>
              {{api.individuo.logradouroCep ? api.individuo.logradouroCep : 'Não informado' }}
            </el-descriptions-item>
            <el-descriptions-item>
              <template slot="label">
                Cidade
              </template>
              {{api.individuo.cidadeNome ? api.individuo.cidadeNome : 'Não informado'}}
            </el-descriptions-item>
            <el-descriptions-item>
              <template slot="label">
                Estado
              </template>
              {{api.individuo.ufAbreviado ? api.individuo.ufAbreviado : 'Não informado'}}
            </el-descriptions-item>
            <el-descriptions-item>
              <template slot="label">
                Raça/Cor
              </template>
              {{api.individuo.racaOuCorLabel ? api.individuo.racaOuCorLabel : 'Não informado'}}
            </el-descriptions-item>
          </el-descriptions>
        </el-col>
      </VuePerfectScrollbar>
    </modal>
    <!-- MODAL CADASTRO PACIENTE-->
  </el-row>
</template>

<style scoped>
  .vermelho {
    background: #e63946 !important;
    color: #FFF !important;
    padding-bottom: -12px
  }

  .inputQueixa .textarea {
    color: black
  }

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

  .textarea-block {
    margin: 18px 0 0 0
  }

  .caret-wrapper i {
    width: auto
  }

  .container-sintomas {
    padding: 20px 0
  }

    .container-sintomas span {
      font-size: 14px;
    }

    .container-sintomas p.data-envio {
      font-size: 14px;
    }

    .container-sintomas .list-sintomas span {
      font-size: 14px;
    }

  .list-sintomas {
    display: flex;
    flex-direction: row;
    gap: 10px;
    flex-wrap: wrap;
  }

  .active:hover {
    color: #FFF
  }

  .active {
    background-color: #01826c !important;
    color: #FFF !important;
    display: inline-block;
    line-height: 1;
    white-space: nowrap;
    cursor: pointer;
    text-align: center;
    box-sizing: border-box;
    outline: 0;
    margin: 0;
    transition: .1s;
    font-weight: 500;
    -webkit-user-select: none;
    font-size: 14px;
    border-radius: 4px;
    padding: 12px;
    margin-top: 10px;
    margin-bottom: 10px;
    margin-left: 0px;
  }

  .el-radio-button__inner {
    padding: 6px 15px;
  }

  .el-form-item__content {
    align-items: center;
  }
</style>



<script>
  import moment from 'moment'
  moment.locale('pt-br')
  import jQuery from 'jquery'
  import { Notification } from 'element-ui'
  import VuePerfectScrollbar from 'vue-perfect-scrollbar'
  import { EventBus } from '../../Event'
  import Log from './Log'
  import Utils from '../../mixins/Utils'
  import _api from '../../api'
  import _enums from '../../api/Enums'
  import PrescricaoMemed from './PrescricaoMemed'
  import Stopwatch from '../../components/UIComponents/StopWatch.vue'
  import { Client as ConversationsClient } from '@twilio/conversations'
  import Twilio, { createLocalVideoTrack } from 'twilio-video'
  import { mask } from 'vue-the-mask'
  import Full from '../../components/containers/Full.vue'
  import audioNotificacao from '../../assets/audios/chat-notification.mp3'
  var somNotificao = new Audio(audioNotificacao)
  import Hub from '../../Hub'

  // componentes
  import DadosHeaderAtendimento from '../../components/shared/DadosHeaderAtendimento'
  import GraficosDasMedicoes from '../../components/shared/GraficosDasMedicoes'
  import HistoricoNoAtendimento from '../../components/shared/HistoricoNoAtendimento'
  import ExamesTabs from '../../components/shared/ExamesTabs'
  import FichaAtendimentoUbs from './FichaAtendimentoUbs'
  import FichaAtendimentoDomiciliar from './FichaAtendimentoDomiciliar'

  var _hub = new Hub()

  export default {
    name: 'Atendimento',
    mixins: [Utils],
    components: { 'prescricao-memed': PrescricaoMemed, Log, Stopwatch, VuePerfectScrollbar, 'full': Full, DadosHeaderAtendimento, GraficosDasMedicoes, HistoricoNoAtendimento, ExamesTabs, FichaAtendimentoUbs, FichaAtendimentoDomiciliar },
    directives: { mask },
    data() {
      return {
        error: false,

        // TYTOCARE
        width: (window.screen.width * 0.8),
        height: (window.screen.height * 0.8),
        showTytoCare: false,
        minimizadoTytoCare: false,

        video: null,
        // Edição comorbidades médico
        dialogVisible: false,
        formComorbidades: {},
        salvarComorbidades: false,
        formAlergias: {},
        formAlergiasEdit: {},
        atendimentoIndividuo: {},
        activeTab: '0',
        notifyChatMessage: false,
        // modal inicio
        scrollSettingsMedicoes: {
          maxScrollbarLength: 20
        },
        // modal comorbidades
        formComorbidades: {},
        formIndividuo: {},
        formIndividuoCadastro: {},
        formProfissional: {},
        formProcedimento: {},
        atendimentoTriagem: {},
        formComoMeSinto: [],
        links: [],
        newObj: [],
        state1: '',
        state2: '',
        // countdown
        count: 1,
        oldDateObj: new Date(),
        newDateObj: new Date(),
        diff: '',
        days: '',
        hours: '',
        mins: '',
        secs: '',
        d: '',
        h: '',
        m: '',
        s: '',
        future: '',
        imgNotFound: false,
        isMinimizado: false,

        dadoImc: '',
        statusImc: '',

        agendamento: {},

        // mensagem de aviso header
        alert: true,

        // flag para aparecer o botão atender
        exibirAtendimento: true,

        // flag para desativar edição das comorbidades
        disabledComorbidades: false,

        // FLAG DE CONTROLE DOS COMPONENTES
        // flag dos dados do paciente no header do atendimento
        exibirDadosHeaderAtendimento: false,
        // flag da exibição dos graficos no modal das medições
        exibirGraficosNoModalMedicoes: false,
        // flag da exibição dos graficos no modal das medições
        exibirGraficosNoAtendimento: false,
        // flag da exibição do menu historico
        exibirMenuHistorico: false,
        // flag da exibição do examesTabs no atendimento
        exibirExamesTabsAtendimento: false,
        // flag da exibição do examesTabs no modal
        exibirExamesTabsModal: false,
        // flag da exibição da ficha atendimento ubs
        exibirFichaAtendimentoUbs: false,
        // flag da exibição da ficha atendimento domiciliar
        exibirFichaAtendimentoDomiciliar: false,

        // flags das telas
        telaSinopse: true,
        telaAtendimento: false,
        telaHistorico: false,
        telaCadastroPaciente: false,

        // EXAMES
        statusAvaliar: false,
        statusEditar: false,
        statusVisualizar: false,
        statusEnviadoAvaliacao: false,
        formAvaliar: {},
        formVisualizar: {},
        formEditar: {},

        usePrescricao: false,

        // CONTADOR
        cronometrando: false,
        resetTimer: false,
        tempoDoContador: 0,
        tempoTotalDoAtendimento: null,

        // FLAG DADOS CADASTRAIS
        editarPaciente: false,
        chatMessage: '',

        // FLAG DE ABERTURA MODAL MEMED
        modalMemedOpen: false,

        scrollSettings: {
          maxScrollbarLength: 200
        },

        // TWILIO
        twilio: {
          token: '',
          loading: false,
          username: '',
          authenticated: false,
          data: {},
          localTrack: false,
          remoteTrack: '',
          activeRoom: '',
          previewTracks: '',
          identity: '',
          roomName: null,
          roomAlreadyCreated: false,
          participants: [],
          tracks: [],
          localVideoTrack: '',
          camEnabled: false,
          micEnabled: false,
          activeConversation: '',
          conversationsClient: null,
          chatMessages: []
        },
        value: '',
        listando: true,
        videoMinimizado: false,
        emAndamentoVideoChamada: false,
        procedimentosAtendimento: [],
        filtroParams: {},
        erros: [],
        enums: {
          condicoesAvaliadas: [
            ..._enums.condicoesAvaliadas
          ],
          sexos: [
            ..._enums.sexos
          ],
          racaOuCor: [
            ..._enums.racaOuCor
          ],
          tipoDaConsulta: [
            ..._enums.tipoDaConsulta
          ]
        },
        validacoes: {},
        routeParams: {},
        api: {
          sinaisVitais: {},
          exames: [],
          estabelecimentos: [],
          motivoConsulta: [],
          comorbidades: {},
          comoMeSinto: {},
          individuo: {},
          profissional: {},
          acompanhamento: {},
          cidades: [],
          comoMeSintoAtendimento: {},
          historicoSinopse: []
        },
        loading: {
          individuos: false,
          estabelecimentos: false,
          motivoConsulta: false,
          atendimentos: false,
          agendamentos: false,
          acompanhamento: false,
          historicoSinopse: false

        },
        paramsHistoricoSinopse: {
          skip: 1,
          take: 10,
          sort: 'DataCadastro DESC',
          total: 0
        },

        paramsMotivoConsulta: {
          skip: 1,
          take: 10,
          descricao: '',
          sort: 'Id ASC',
          total: 0
        },
        procParams: {
          skip: 1,
          take: 10,
          descricao: '',
          sort: 'Descricao ASC',
          total: 0
        },
        estabParams: {
          skip: 1,
          take: 10,
          sort: 'e.NomeFantasia ASC',
          total: 0
        },
        paramsAcompanhamento: {
          skip: 1,
          take: 9999,
          sort: 'a.Data DESC',
          total: 0
        },
        paramsAllProcedimentos: {
          skip: 1,
          take: 10000,
          total: 0
        },
        params: {
          skip: 1,
          take: 10,
          sort: 'DataCadastro ASC',
          total: 0
        },
        paramsUfs: {
          skip: 1,
          take: 30,
          sort: '+UfAbreviado',
          total: 0
        },
        paramsCidades: {
          skip: 1,
          take: 999,
          sort: '+Nome',
          total: 0,
          ufAbreviado: null
        }
      }
    },
    watch: {
      'twilio.chatMessages': function (val) {
        if (val) {
          setTimeout(() => {
            this.scrollChatToEnd()
          }, 200)
        }
      }
    },
    created() {
      // video chamada
      window.addEventListener('beforeunload', this.leaveRoomIfJoined)

      this.routeParams = this.$route.params
      // caso ocorra um refresh na tela ele ira retornar para tela agendamentos
      if (!this.routeParams.agendamento) {
        this.exibirAtendimento = true
        this.$router.push({
          name: 'Agendamentos'
        })
        return
      }
    },

    async mounted() {
      await this.clickButtonSinopse()

      // interval para fechar tela de alert exibida no inicio
      /* setTimeout(async () => { await this.close() }, 20000) */

      // Desativar Comorbidades e Informacoes Adicionais(Visualizar PRontuario):
      this.disabledComorbidades = !!((this.routeParams.preVisualizar === true || this.routeParams.preVisualizar !== undefined))

      if (this.routeParams.preVisualizar === true) {
        this.exibirAtendimento = false
      }

      if (this.routeParams.ausentes === true) {
        this.ausentes = true
      }
      this.agendamento = this.routeParams.agendamento
      await this.getComoMeSintoAtendimento(this.agendamento)
      this.block()

      /* this.setIsCollapse(true) */
      // Passado o id do profissional para api profissional para chamada posterior da memed junto com o token
      this.api.profissional.id = this.agendamento.profissionalId

      // Retorno dos dados do paciente
      await this.getPaciente()

      // Retorno do atendimento da triagem
      await this.getAtendimentoTriagem()

      // Retorno dos atendimentos anteriores no sinopse
      await this.getHistoricoSinopse()

      // Retorno da lista dos procedimentos
      await this.getProcedimentos()

      // Verificação e retorno dos sinais vitais
      await this.getSinaisVitais()

      // Retorno das comorbidades do paciente
      await this.getComorbidades()

      // Chamada do token do medico para abrir modal memed
      await this.tokenMemed()

      // Retorno ultimo atendimento do Individuo
      await this.getMaxAtendimentoByIndividuoId()
    },
    beforeDestroy() {
      this.leaveRoomIfJoined();
    },
    methods: {
      async eventRetornoFichaAtendimento(val) {

        if (val.atendimentoFinalizado != undefined && val.atendimentoId) {
          if (val.atendimentoFinalizado == true) {
            // se houver exames avaliados no modal dos exames irá subir aqui
            if (this.api.exames.length > 0) {
              await this.enviarAvaliacaoExames()
            }

            // atualizando as alergias e antecedentes
            if (this.formAlergias.alergias != undefined || this.formAlergias.antecedentes != undefined) {
              let { data, status } = await _api.atendimentos.getById(val.atendimentoId)
              if (status === 200 && data != {}) {
                var obj = data
                obj.alergias = this.formAlergias.alergias
                obj.antecedentes = this.formAlergias.antecedentes
                let { data: dataPut, status: statusPut } = await _api.atendimentos.putAtendimento(obj)
                if (statusPut === 200) this.$message.success('Alergias cadastradas')
              }
            }

            // atualizar o sintomas (que vai estar no banco com o ID do atendimento da triagem para o ID do atendimento do médico)

            // retirando o observer de reatividade do vue
            delete this.api.comoMeSintoAtendimento.__ob__

            //verificando se tem algum dado apos tirar o observer do vue, e atualizando os sintomas
            if (Object.keys(this.api.comoMeSintoAtendimento).length > 0) {
              let { data: dataAcompanhamento, status: statusAcompanhamento } = await _api.acompanhamento.atualizarAcompanhamento(val.atendimentoId, this.api.comoMeSintoAtendimento)
              if (statusAcompanhamento === 200 || statusAcompanhamento === 201 || statusAcompanhamento === 204) this.$message.success('Acompanhamento atualizado')
              else this.$message.error('Erro Acompanhamento')
            }

            this.$router.push({ name: 'Agendamentos' })
          }
        }
      },

      async enviarAvaliacaoExames() {
        var examesArr = this.api.exames.filter(exames => {
          if (exames.resultado == true && exames.finalizado == false) {
            exames.finalizado = true
            exames.profissionalId = this.$auth.user().id
            return this.api.exames
          }
        })

        examesArr.forEach(async function (exames) {
          var avaliadoSplit = exames.avaliado.split('/')
          var avaliado = `${avaliadoSplit[2]}-${avaliadoSplit[1]}-${avaliadoSplit[0]}`
          exames.avaliado = avaliado.replaceAll('/', '-')
          let { data, status } = await _api.exames.putAvaliacao(exames)

          if (status != 204) {
            this.$message.warning('Avaliação De Exames Não Enviado')
          }
        })
      },

      verificarNavegador() {
        return navigator.userAgent.indexOf('Firefox') == -1
      },

      async getProcedimentos() {
        let { data, status } = await _api.procedimentos.get(this.paramsAllProcedimentos)
        if (status === 200) this.api.procedimentos = data
      },

      onChangeComorbidades(val) {
        this.formComorbidades.respondeuComorbidade = true
        // this.formComorbidades.dataAlteracao = new Date();
        this.onClickComorbidades(this.formComorbidades)
      },
      existCNS(cns) {
        if (cns == "" || cns == undefined) {
          return "Nao cadastado";
        }
        return cns;
      },
      // CLICK PARA ATUALIZAR COMORBIDADES
      async onClickComorbidades(val) {
        // val é this.formComorbidades
        val.RespondeuComorbidade = true
        val.dataAlteracao = new Date()
        let { data, status } = await _api.individuos.atualizarComorbidade(this.agendamento.individuoId, val)
        this.getComorbidades()

        if (status === 200) {
          this.$message.success('Comorbidades Enviadas Com Sucesso')
          // this.salvarComorbidades = true
        } else {
          this.$message.warning('Erro No Envio Das Comorbidades')
        }
      },

      // Dados Cadastrais do Paciente
      async onClickSalvarIndividuo(form) {


        this.erros = []
        this.$refs[form].validate(async (valid) => {
          if (valid) {


            this.$prompt('Descreva o motivo da edição do cadastro:', 'Alteração de dados cadastrados', {
              confirmButtonText: 'Confirmar',
              cancelButtonText: 'Cancelar',
              inputPattern: /^[A-Za-záàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ ]+$/,
              //  /[\w!#$%&'*+/=?^_`{|}~-]+(?:\.[\w!#$%&'*+/=?^_`{|}~-]+)*@(?:[\w](?:[\w-]*[\w])?\.)+[\w](?:[\w-]*[\w])?/
              inputErrorMessage: 'O campo pode conter apenas letras'
            }).then(({ value }) => {
              this.formIndividuoCadastro = {
                ...this.formIndividuoCadastro,
                imagem: null,
                Justificativa: value,
                cadastroEditado: true,
                usuarioEditou: this.$auth.user().id,
              }



              this.loading.individuos = true

              this.formIndividuoCadastro.ativo = true
              this.formIndividuoCadastro.nomeCompleto = this.formIndividuoCadastro.nomeCompleto.toUpperCase()
              this.formIndividuoCadastro.nomeDaMae = this.formIndividuoCadastro.nomeDaMae ? this.formIndividuoCadastro.nomeDaMae.toUpperCase() : this.formIndividuoCadastro.nomeDaMae
              if (this.api.individuo.imagem != undefined && this.api.individuo.imagem != '') {
                this.formIndividuoCadastro.imagem = this.api.individuo.imagem
              } else {
                this.formIndividuoCadastro.imagem = ''
              }

              _api.individuos.put(this.formIndividuoCadastro).then(res => {
                if (res.status === 200) {
                  this.loading.individuos = false
                  /* this.$message.success('Alterado os dados do paciente com sucesso.') */
                } else {
                  /* this.$message.warning('Erro ao editar o paciente.') */

                  this.loading.individuos = false
                }
              })



            }).catch((e) => {
              console.log(e)
              this.loading.individuos = false

            });

          this.editarPaciente = false;
          }








          else {
            this.$message.warning('Verifique o preenchimento do formulário')
            this.loading.individuos = false
            return false
          }
        })
        this.loading.individuos = false
      },

      async getCep(cep) {
        if (cep.length > 8) {
          let { data, status } = await _api.auxiliar.getCep(cep)
          if (status === 200) {
            this.formIndividuoCadastro.cidadeId = data.ibge
            this.formIndividuoCadastro.logradouro = data.logradouro
            this.formIndividuoCadastro.logradouroBairro = data.bairro
            this.formIndividuoCadastro.ufAbreviado = data.uf
            this.paramsCidades.ibge = data.ibge
            await this.onSelectUf(data.uf)
            await this.getCidadesByUf()
          } else {
            this.paramsCidades.ibge = null
            this.formIndividuoCadastro = {
              ...this.formIndividuoCadastro,
              ufAbreviado: null,
              cidadeId: null,
              logradouro: null,
              bairro: null
            }
          }
        }
      },

      async onSelectUf(val) {
        this.paramsCidades.ufAbreviado = val
        this.formIndividuoCadastro = {
          ...this.formIndividuoCadastro,
          cidadeId: null
        }
        await this.getCidadesByUf()
      },
      async getCidadesByUf() {
        this.loading.cidades = true
        let { data, paginacao, status } = await _api.cidades.get(this.paramsCidades)
        this.api.cidades = (status === 200) ? data : []
        if (this.api.cidades.length === 1) {
          this.formIndividuoCadastro.cidadeId = this.api.cidades[0].ibge
        }
        this.paramsCidades.skip = (status === 200) ? paginacao.currentPage : 0
        this.paramsCidades.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.cidades = false
      },

      // CONTROLE DAS TELAS

      // TELA SINOPSE
      async clickButtonSinopse() {
        this.telaSinopse = true
        this.telaAtendimento = false
        this.telaHistorico = false
        this.telaCadastroPaciente = false

        // forçando o estado do exame tabs modal false
        this.exibirExamesTabsModal = false

        // forçando o estado do header atendimento false
        this.exibirDadosHeaderAtendimento = false

        // forçando o estado do componente dos graficos false
        this.exibirGraficosNoAtendimento = false

        // forçando o estado do menu historico false
        this.exibirMenuHistorico = false

        // forçando o estado do exame tabs atendimento false
        this.exibirExamesTabsAtendimento = false
      },

      // TELA ATENDIMENTO
      async clickButtonAtendimento() {
        await this.getComoMeSintoAtendimento(this.agendamento)
        this.telaAtendimento = true
        if (this.mxHasAccess('Médico')) this.exibirFichaAtendimentoUbs = true
        if (this.mxHasAccess('MédicoAD')) this.exibirFichaAtendimentoDomiciliar = true
        this.telaSinopse = false
        this.telaHistorico = false
        this.telaCadastroPaciente = false

        // exibir o componente header com foto e dados no atendimento
        this.exibirDadosHeaderAtendimento = true

        // exibir o componente do exame tabs no atendimento
        this.exibirExamesTabsAtendimento = true

        // forçando o estado do exame tabs modal false
        this.exibirExamesTabsModal = false

        // forçando o estado do menu historico false
        this.exibirMenuHistorico = false
      },

      async clickButtonHistorico() {
        // this.getHistorico()
        // this.hideModal()
        this.telaHistorico = true
        this.telaSinopse = false
        this.telaAtendimento = false
        this.telaCadastroPaciente = false

        // forçando o estado do header atendimento false
        this.exibirDadosHeaderAtendimento = false

        // exibindo componente historico
        this.exibirMenuHistorico = true

        // forçando o estado do exame tabs atendimento false
        this.exibirExamesTabsAtendimento = false

        // forçando o estado do exame tabs modal false
        this.exibirExamesTabsModal = false
      },

      async clickButtonCadastro() {
        this.getPaciente()
        this.telaCadastroPaciente = true
        this.telaSinopse = false
        this.telaAtendimento = false
        this.telaHistorico = false

        // forçando o estado do header atendimento false
        this.exibirDadosHeaderAtendimento = false

        // forçando o estado do menu historico false
        this.exibirMenuHistorico = false

        // forçando o estado do exame tabs atendimento false
        this.exibirExamesTabsAtendimento = false

        // forçando o estado do exame tabs modal false
        this.exibirExamesTabsModal = false
      },

      // RETORNANDO QUEIXA DO PACIENTE
      async getAtendimentoTriagem() {
        let { data, status } = await _api.atendimentos.getbyAgendamentoId(this.agendamento.id)
        this.atendimentoTriagem = (status === 200) ? data : {}
        if (this.atendimentoTriagem.queixaDoPaciente === '' || this.atendimentoTriagem.queixaDoPaciente === undefined || this.atendimentoTriagem.queixaDoPaciente === null) {
          this.atendimentoTriagem.queixaDoPaciente = 'Não Informado Pela Triagem!!'
        }
      },

      // RETORNANDO ATENDIMENTOS ANTERIORES NA TELA SINOPSE
      // RETORNANDO OS ATENDIMENTOS ANTERIORES DO PACIENTE SELECIONADO
      async getHistoricoSinopse() {
        this.loading.historicoSinopse = true

        var myParams = {
          ...this.paramsHistoricoSinopse, individuoId: this.agendamento.individuoId, atendidoMedico: true
        }

        let { data, paginacao, status } = await _api.atendimentos.get(myParams)
        if (status === 502) this.loading.historicoSinopse = false
        this.api.historicoSinopse = (status === 200) ? data : []

        this.paramsHistoricoSinopse.skip = (status === 200) ? paginacao.skip : 0
        this.paramsHistoricoSinopse.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.historicoSinopse = false
      },

      // PAGINAÇÃO HISTÓRICO SINOPSE
      handleSizeChangeHistoricoSinopse(val) {
        this.paramsHistoricoSinopse.take = val
        this.getHistoricoSinopse()
      },
      handleCurrentChangeHistoricoSinopse(val) {
        this.paramsHistoricoSinopse.skip = val
        this.getHistoricoSinopse()
      },

      // RETORNA O ULTIMO ATENDIMENTO DO INDIVIDUO
      async getMaxAtendimentoByIndividuoId() {
        let { data, status } = await _api.atendimentos.getMaxAtendimentoByIndividuoId(this.api.individuo.id)
        let infosAdicionais = (status === 200) ? data : {}

        this.formAlergiasEdit = {
          alergias: infosAdicionais.alergias,
          antecedentes: infosAdicionais.antecedentes
        }

        if (Object.keys(infosAdicionais).length > 0) {
          this.formAlergiasEdit.alergias = infosAdicionais.alergias
          this.formAlergiasEdit.antecedentes = infosAdicionais.antecedentes
        }
      },

      // OPEN MODAL RESULTADO EXAMES
      openModalResultadoExames() {
        this.exibirExamesTabsModal = true
        this.$modal.show('modalResultadoExames')
      },
      hideModalResultadoExames() {
        this.exibirExamesTabsModal = false
        this.$modal.hide('modalResultadoExames')
      },

      // OPEN MODAL COMORBIDADES
      async openModalComorbidades() {
        await this.getComorbidades()
        this.formComorbidades = this.api.comorbidades
        this.$modal.show('modalComorbidades')
      },
      hideModalComorbidades() {
        this.api.comorbidades = {}
        this.$modal.hide('modalComorbidades')
      },

      // CLICK PARA ATUALIZAR COMORBIDADES
      async onClickComorbidades(val) {
        // val é this.formComorbidades
        val.RespondeuComorbidade = true
        val.dataAlteracao = new Date()
        let { data, status } = await _api.individuos.atualizarComorbidade(this.agendamento.individuoId, val)
        this.getComorbidades()
        if (status === 200) {
          this.$message.success('Comorbidades Enviadas Com Sucesso')
          // this.salvarComorbidades = true
        } else {
          this.$message.warning('Erro No Envio Das Comorbidades')
        }
      },

      // BOTAO DE MINIMIZAR E MAXIMIZAR MODAL TELEATENDIMENTO
      minimizar() {
        // document.getElementById("localTrack").children[0].setAttribute("id", "video")
        try {
          if (navigator.userAgent.indexOf('Firefox') != -1) {
            this.$message.error('Este navegador não suporta este recurso!')
            Notification({
              title: 'Aviso!', message: 'Para minimizar, clique com o botão direito sobre o vídeo e clique em Assistir em picture-in-picture.', type: 'warning'
            })
            return
            // Usuário está usando o Mozilla Firefox"
          }
          document.getElementById('video').requestPictureInPicture({ width: 400, height: 300, aspectRatio: '16:9' })
          this.isMinimizado = true
        } catch (e) {
          // console.log(e)
          this.$message.error('Erro ao minimizar chamada!')
        }
      },
      maximizar() {
        this.isMinimizado = false

        // if (document.pictureInPictureElement) {
        //  document.exitPictureInPicture();
        // } else if (document.pictureInPictureEnabled) {
        //  video.requestPictureInPicture();
        // }
      },

      // EL-COLLAPSE NO ATENDIMENTO
      async openCollapse(val) {
        if (val.length > 0) {
          if (val[0] == 'graficos') {
            this.exibirGraficosNoAtendimento = true
          }
        }
      },

      // HISTÓRICO E GRÁFICOS MEDIÇÕES
      async openModalMedicoes() {
        this.$modal.show('modalMedicoes')
        this.exibirGraficosNoModalMedicoes = true
      },

      // caso necessite de algum evento na troca de tela no header
      handleTabClickMedicoesHeader(tab, event) {
        if (tab.paneName === '2') {
        }
      },

      // caso necessite de algum evento na troca de tela entre os graficos
      handleTabClickMedicoesGrafico(tab, event) {
        if (tab.label === 'Teste') {

        }
      },

      // fechar modal medições
      hideModalMedicoes() {
        this.$modal.hide('modalMedicoes')
        this.exibirGraficosNoModalMedicoes = false
      },

      // FIM HISTÓRICO E GRÁFICOS MEDIÇÕES

      // RETORNO DA IDADE DO PACIENTE
      getIdade(val) {
        var data = moment(val).format('DD/MM/YYYY')
        var idade = moment().diff(val, 'years')
        return idade
      },

      handleClose(key, keyPath) {

      },
      // ALTERAÇÃO DE COR NA TABLE
      tableRow({ row }) {
        if (row.ativo === true) return 'success-row'
        else if (row.ativo === false) return 'warning-row'
        else return ''
      },

      // CONTADOR DE TEMPO
      onEmitContador(val) {
        this.tempoDoContador = val.time
      },

      // BLOQUEIA O USUARIO AO DAR O CLICK DE VOLTAR NO BOTÃO DO NAVEGADOR
      block() {
        window.location.hash = 'no-back-button'
        window.location.hash = 'Again-No-back-button'// again because google chrome don't insert first hash into history
        window.onhashchange = function () { window.location.hash = 'no-back-button' }
      },

      // BLOQUEIA E OCULTA OS ICONES DO MENU LATERAL
      setIsCollapse(payLoad) {
        this.$store.dispatch('setIsCollapse', payLoad)
      },

      // Fechar tela de alert (ATENDIMENTO)
      close() {
        this.alert = false
      },

      // RETORNO DAS COMORBIDADES DO PACIENTE ATUAL (ATENDIMENTO)
      async getComorbidades() {
        let { data } = await _api.individuos.getById(this.agendamento.individuoId)
        this.api.comorbidades = (data !== null) ? data : {}
      },

      // RETORNO DOS SINAIS VITAIS DO PACIENTE ATUAL (ATENDIMENTO)
      async getComoMeSintoAtendimento(row) {
        if (this.agendamento.id != null) {
          this.paramsAcompanhamento.individuoId = this.agendamento.individuoId
          let { data } = await _api.acompanhamento.get(this.paramsAcompanhamento)
          let comoMeSintoAtendimento = (data !== null) ? data : []
          if (comoMeSintoAtendimento.length > 0) {
            this.api.comoMeSintoAtendimento = comoMeSintoAtendimento[0]
          }
        }
      },
      // RETORNANDO SINAIS VITAIS DA PULSEIRA (ATENDIMENTO)
      async getSinaisVitais() {
        this.api.sinaisVitais.dataAlteracao = this.agendamento.dataAlteracao
        this.api.sinaisVitais.batimentoCardiaco = this.agendamento.batimentoCardiaco
        this.api.sinaisVitais.oxigenacaoSanguinea = this.agendamento.oxigenacaoSanguinea
        this.api.sinaisVitais.pressaoSanguinea = this.agendamento.pressaoSanguinea
        this.api.sinaisVitais.altura = this.agendamento.altura
        this.api.sinaisVitais.peso = this.agendamento.peso
        this.api.sinaisVitais.temperatura = this.agendamento.temperatura

        var imcDados = (this.api.sinaisVitais.peso / ((this.api.sinaisVitais.altura / 100) * (this.api.sinaisVitais.altura / 100)))
        var imcFloatDados = parseFloat(imcDados).toFixed(1)

        this.api.sinaisVitais.dadosImc = imcFloatDados
      },

      getGrauImc(row) {
        if (row <= 16.9) return 'Muito abaixo do Peso'
        else if (row >= 17 && row <= 18.4) return 'Abaixo do Peso'
        else if (row >= 18.5 && row <= 24.9) return 'Peso Normal'
        else if (row >= 25 && row <= 29.9) return 'Acima do Peso'
        else if (row >= 30 && row <= 34.9) return 'Obesidade Grau I'
        else if (row >= 35 && row <= 40) return 'Obesidade Grau II'
        else if (row > 40) return 'Obesidade Grau III'
        else return ''
      },

      querySearch(queryString, cb) {
        var links = this.newObj
        var results = queryString ? links.filter(this.createFilter(queryString)) : links
        // call callback function to return suggestions
        cb(results)
      },

      createFilter(queryString) {
        return (link) => {
          return (link.value.toLowerCase().indexOf(queryString.toLowerCase()) === 0)
        }
      },

      // ---------------- MEMED ---------------------------
      async tokenMemed() {
        let { data } = await _api.memed.recoverTokenMedico()
        console.log('data', data)
        this.api.profissional.tokenMemed = data.attributes.token
        this.usePrescricao = true
      },
      async startMemed() {
        this.modalMemedOpen = true
      },
      async closeMemed() {
        this.modalMemedOpen = false
      },
      async getPaciente() {
        let { data, status } = await _api.individuos.getById(this.agendamento.individuoId)
        this.formIndividuoCadastro = (status === 200) ? data : {}
        if (this.formIndividuoCadastro.ufAbreviado != 'NI') {
          await this.onSelectUf(this.formIndividuoCadastro.ufAbreviado)
          await this.getCidadesByUf()
        } else {
          this.formIndividuoCadastro.ufAbreviado = ''
          this.formIndividuoCadastro.cidadeId = ''
        }
        this.api.individuo = (status === 200) ? data : {}
        let { nomeDaMae } = this.api.individuo
        this.api.individuo = { ...this.api.individuo, nomeDaMae: nomeDaMae }

      },

      // ---------------- FIM MEMED ---------------------------

      // --------------------- VIDEO CHAMADA TWILIO ---------------------
      stopTimer() {
        this.cronometrando = false
      },
      startTimer() {
        if (this.cronometrando == false) this.cronometrando = true
      },
      async twCreateRoom() {
        let agendamento = {
          agendamentoId: this.agendamento.id
        }
        let { data } = await _api.teleConsulta.twCreateRoom(agendamento)
        if (data === 'Sala de atendimento ja criada.') {
          this.resetTimer = true
          this.twilio.roomAlreadyCreated = true
          this.twEntrarNaSala()
          return
        }
        this.resetTimer = false
        this.twilio.roomName = data.roomVideo.unique_name
        this.twEntrarNaSala()
      },
      async twGetToken() {
        let { data, status } = await _api.teleConsulta.twGetToken(this.agendamento.id)
        return data.token
      },
      async twEntrarNoChat() {
        window.conversationsClient = ConversationsClient
        this.conversationsClient = new ConversationsClient(this.twilio.token)
        this.conversationsClient.on('connectionStateChanged', (state) => {
          switch (state) {
            case 'connected':
              this.createConversation()
              break
            case 'disconnecting':

              break
            case 'disconnected':

              break
            case 'denied':

              break
          }
        })
        this.conversationsClient.on('messageAdded', (message) => {
          if (message.conversation.channelState.uniqueName.includes('Recepcao')) return
          if (this.activeTab !== '1') {
            this.handleNotifyChatMessage()
          }
          this.onAddMessageToHistory(message.state)
        })
        this.conversationsClient.on('participantJoined', (participant) => {
          // console.log('paciente entrou na sala')
        })
      },
      handleNotifyChatMessage() {
        let element = document.getElementById('tab-chat')
        element.classList.add('chat--notify')
        somNotificao.play()
      },
      async createConversation() {
        const uniqueName = `${this.agendamento.id}-chat`
        try {
          const activeConnection = await this.conversationsClient.createConversation({ uniqueName: uniqueName })
          // console.log('active', activeConnection)
          const joinedConversation = await activeConnection.join()
          // console.log('joined', joinedConversation)
          await joinedConversation.add(this.agendamento.profissionalId).catch(err => console.log(err))
          await joinedConversation.add(this.agendamento.individuoId).catch(err => console.log(err))
        } catch (e) {
          const oldConversation = await this.conversationsClient.getConversationByUniqueName(uniqueName)
          let count = oldConversation.getParticipantsCount()
          if (count < 2) {
            await oldConversation.add(this.agendamento.individuoId).catch(err => console.log(err))
          }
          // console.log('old conversation', oldConversation)
          this.twilio.activeConversation = oldConversation
        } finally {
          await this.getMessages()
        }
      },
      async getMessages() {
        console.log('this.twilio.activeConversation', this.twilio.activeConversation)
        this.twilio.activeConversation.getMessages().then(msgs => {
          const allMessages = msgs.items.map(msg => {
            return {
              body: msg.body,
              createdDate: msg.dateCreated,
              memberSid: msg.author
            }
          })
          let filteredMsgs = allMessages.filter(function (msg) {
            return msg.body.length > 0
          })
          this.twilio.chatMessages = filteredMsgs
        })
      },
      onSendMessage() {
        if (!this.chatMessage) return
        this.twilio.activeConversation.sendMessage(this.chatMessage)
          .catch((erro) => {
            this.$message.warning('Não foi possível enviar a mensagem')
          })
        this.chatMessage = ''
      },
      async onAddMessageToHistory(msg) {
        await this.twilio.chatMessages.push({
          body: msg.body,
          createdDate: msg.timestamp,
          memberSid: msg.author
        })
      },
      async twEntrarNaSala() {
        this.twilio.loading = true
        const VueThis = this
        let { data } = await _api.teleConsulta.twGetToken(this.agendamento.id)
        this.twilio.token = data.token
        this.twEntrarNoChat()
        if (this.twilio.roomAlreadyCreated) {
          this.twilio.roomName = data.videoRoom.unique_name
        }
        let connectOptions = {
          name: this.twilio.roomName,
          automaticSubscription: true,
          audio: true,
          video: { width: 400 }
        }

        // before a user enters a new room,
        // disconnect the user from they joined already
        this.leaveRoomIfJoined()

        // remove any remote track when joining a new room
        document.getElementById('remoteTrack').innerHTML = ''

        Twilio.connect(data.token, connectOptions).then(function (room) {
          VueThis.dispatchLog(`Sala do Atedimento: ${VueThis.twilio.roomName} aberta `)
          const cameraTrack = [...room.localParticipant.videoTracks.values()][0].track
          let localMediaContainer = document.getElementById('localTrack')
          // this.video = localMediaContainer;

          localMediaContainer.appendChild(cameraTrack.attach())
          document.addEventListener('leavepictureinpicture', function (event) {
            // Vídeo sai de Picture-in-Picture.
            // O usuário reproduziu um vídeo Picture-in-Picture em outra página.

            // if (document.pictureInPictureElement) {
            //  document
            //    .exitPictureInPicture()
            //    .then(() => this.isMinimizado = false)
            //    .catch((err) => console.error(err));
            // } else {
            //  video.requestPictureInPicture();
            // }
            VueThis.maximizar()
          })

          VueThis.twilio.localVideoTrack = cameraTrack
          VueThis.twilio.camEnabled = true
          VueThis.twilio.micEnabled = true
          VueThis.startTimer()
          // set active toom
          VueThis.twilio.activeRoom = room
          VueThis.twilio.loading = false

          // Attach the Tracks of all the remote Participants.
          room.participants.forEach(function (participant) {
            let previewContainer = document.getElementById('remoteTrack')
            VueThis.attachParticipantTracks(participant, previewContainer)
          })

          // When a Participant joins the Room, log the event.
          room.on('participantConnected', function (participant) {
            // console.log('participant entrou na sala', participant)
            participant.tracks.forEach(publication => {
              if (publication.isSubscribed) {
                const track = publication.track
                document.getElementById('remoteTrack').appendChild(track.attach())
              }
            })
            participant.on('trackSubscribed', track => {
              if (track.kind === 'audio' || track.kind === 'video') {
                document.getElementById('remoteTrack').appendChild(track.attach())

                if (document.getElementById('remoteTrack').children[0].localName.includes('video')) {
                  document.getElementById('remoteTrack').children[0].setAttribute('id', 'video')
                } else {
                  document.getElementById('remoteTrack').children[1].setAttribute('id', 'video')
                }
              }
            })
            VueThis.dispatchLog("Joining: '" + participant.identity + "'")
          })

          // When a Participant adds a Track, attach it to the DOM.
          room.on('trackAdded', function (track, participant) {
            // console.log('trackAdded', track)
            VueThis.dispatchLog(participant.identity + ' added track: ' + track.kind)
            let previewContainer = document.getElementById('remoteTrack')
            VueThis.attachTracks([track], previewContainer)
            document.getElementById('remoteTrack').children[0].setAttribute('id', 'video')
          })

          // When a Participant removes a Track, detach it from the DOM.
          room.on('trackRemoved', function (track, participant) {
            // console.log('trackRemoved - track', track)
            VueThis.dispatchLog(participant.identity + ' removed track: ' + track.kind)
            VueThis.detachTracks([track])
          })

          // When a Participant leaves the Room, detach its Tracks.
          room.on('participantDisconnected', function (participant) {
            // console.log('participantDisconnected', participant)
            VueThis.dispatchLog("Participant '" + participant.identity + "' left the room")
          })

          room.on('trackUnsubscribed', function (track) {
            // console.log('trackUnsubscribed', track)
            if (track.kind === 'audio' || track.kind === 'video') {
              const htmlElements = track.detach()
              for (let htmlElement of htmlElements) {
                htmlElement.remove()
              }
            }
            VueThis.dispatchLog("Track '" + track + "' trackUnsubscribed")
          })
        })
      },
      onMicrophoneEnable() {
        // console.log('ativar microfone')
        this.twilio.activeRoom.localParticipant.audioTracks.forEach(publication => {
          publication.track.enable()
          // console.log('microfone', publication)
        })
        this.twilio.micEnabled = true
      },
      onMicrophoneDisable() {
        this.twilio.activeRoom.localParticipant.audioTracks.forEach(publication => {
          publication.track.disable()
          // publication.track.stop()
        })
        this.twilio.micEnabled = false
      },
      onVideoEnable() {
        createLocalVideoTrack().then(track => {
          let localMediaContainer = document.getElementById('localTrack')
          localMediaContainer.appendChild(track.attach())
          return this.twilio.activeRoom.localParticipant.publishTrack(track)
        })
        this.twilio.camEnabled = true
      },
      onVideoDisable() {
        this.twilio.activeRoom.localParticipant.videoTracks.forEach(publication => {
          publication.track.disable()
          publication.track.stop()
          publication.unpublish()
        })
        document.getElementById('localTrack').innerHTML = ''
        this.twilio.camEnabled = false
      },
      // Metodos Twilio
      // Trigger log events
      dispatchLog(message) {
        EventBus.$emit('new_log', message)
      },

      // Attach the Tracks to the DOM.
      attachTracks(tracks, container) {
        tracks.forEach(function (track) {
          container.appendChild(track.attach())
        })
      },

      // Attach the Participant's Tracks to the DOM.
      attachParticipantTracks(participant, container) {
        let tracks = Array.from(participant.tracks.values())
        this.attachTracks(tracks, container)
      },

      // Detach the Tracks from the DOM.
      detachTracks(tracks) {
        tracks.forEach((track) => {
          track.detach().forEach((detachedElement) => {
            detachedElement.remove()
          })
        })
      },

      // Detach the Participant's Tracks from the DOM.
      detachParticipantTracks(participant) {
        let tracks = Array.from(participant.tracks.values())
        this.detachTracks(tracks)
      },

      // Leave Room.
      leaveRoomIfJoined() {
        document.getElementById('localTrack').innerHTML = ''
        document.getElementById('remoteTrack').innerHTML = ''
        if (this.twilio.activeRoom) {
          this.twilio.activeRoom.disconnect()
        }
      },

      handleTabClickTeleAtendimento(tab, event) {
        this.activeTab = tab.index
        if (tab.label === 'Chat') {
          let element = document.getElementById('tab-1')
          element.classList.remove('chat--notify')
          setTimeout(() => {
            this.scrollChatToEnd()
          }, 500)
        }
      },

      handleTabClick(tab, event) {
        if (tab.label === 'Teste') {

        }
      },

      scrollChatToEnd() {
        if (this.twilio.chatMessages.length === 0) return
        jQuery('#pscroll').animate({ scrollTop: jQuery('ul#conversation li:last').offset().top + 60 }, 200)
      },

      // --------------------- FIM VIDEO CHAMADA TWILIO ---------------------

      // ----------------------- MODALS -------------------------
      // MODAL VIDEO CHAMADA

      async openModalVideo() {
        try {
          const result = await navigator.mediaDevices.enumerateDevices().then(devices => {
            if (Array.isArray(devices)) {
              if (!devices.some(device => device.kind === 'videoinput')) {
                this.$message({
                  showClose: true,
                  message: 'Não foi possível iniciar a chamada. Câmera indisponível.',
                  type: 'error'
                })
                return false
              } else {
                return true
              }
            }
          })
          if (!result) {
            return
          }

          await this.twCreateRoom()
          setTimeout(async () => await _api.agendamentos.putAgendamento({ ...this.agendamento, emAndamento: true }), 10000)
          await this.notifyWhatsapp()
          this.emAndamentoVideoChamada = true
          this.notifyOpenRoom('schedule-update-event')
          this.$modal.show('teleatendimento')

          // this.video.setAttribute('ref', 'video');

          // this.$refs.video.addEventListener('canplay', () => {
          //  this.$refs.video.requestPictureInPicture();
          // });

          // this.$refs.video.addEventListener('enterpictureinpicture', () => {
          //  this.$refs.video.style.width = '200px';
          //  this.$refs.video.style.height = 'auto';
          //  this.$refs.video.style.position = 'fixed';
          //  this.$refs.video.style.top = '10px';
          //  this.$refs.video.style.right = '10px';
          // });
        } catch (e) {
          // console.log(e)
          Notification({ title: 'Erro ao abrir a sala', message: 'Verifique sua conexão', type: 'error' })
        }
      },

      async notifyWhatsapp() {
        let response = await _api.agendamentos.notifyWhatsapp(this.api.individuo.nomeCompleto, this.api.individuo.telefoneCelular)
      },

      async openModalTytoCare() {
        this.$modal.show('tytocare')
        this.showTytoCare = true
      },

      async closeModalTytoCare() {
        this.$modal.hide('tytocare')
        this.showTytoCare = false
      },

      async minimizarModalTytoCare() {
        this.showTytoCare = false
        this.minimizadoTytoCare = true
      },

      async returnModalTytoCare() {
        this.showTytoCare = true
        this.minimizadoTytoCare = false
      },

      async notifyOpenRoom(event) {
        _hub.connection.start()
          .then(() => {
            _hub.connection.invoke('SendOpenRoom', `${event}-${this.agendamento.id}`)
            console.log(`${event}-${this.agendamento.id}`)
          }).catch(e => console.log('Error connection to Hub', e))
      },

      async hideDialogVideo() {
        if (document.pictureInPictureElement) {
          document.exitPictureInPicture()
        }
        this.stopTimer()

        this.tempoTotalDoAtendimento = this.tempoDoContador

        this.agendamento.emAndamento = false
        await _api.agendamentos.putAgendamento(this.agendamento)
        this.emAndamentoVideoChamada = false
        this.showTytoCare = false
        this.$modal.hide('teleatendimento')

        this.onVideoDisable()
        this.onMicrophoneDisable()
        this.leaveRoomIfJoined()
      },

      // MODAL MEMED
      openModalMemed() {
        this.$modal.show('modalMemed')
      },
      hideModalMemed() {
        this.$modal.hide('modalMemed')
      },

      // ----------------------- FIM MODALS -------------------------

      // ---------------- OPEN MODAL ALERGIAS ---------------------------
      openDialogAlergias() {
        this.dialogVisible = true
      },

      hideModalAlergias() {
        this.dialogVisible = false
      },

      async onClickAlergias() {
        this.dialogVisible = false

        this.formAlergias.alergias = null
        this.formAlergias.antecedentes = null

        this.formAlergias.alergias = (this.formAlergiasEdit.alergias != null, this.formAlergiasEdit.alergias != '') ? this.formAlergiasEdit.alergias.trim() : null
        this.formAlergias.antecedentes = (this.formAlergiasEdit.antecedentes != null, this.formAlergiasEdit.antecedentes != '') ? this.formAlergiasEdit.antecedentes.trim() : null

        this.$modal.hide('modalAlergias')
        let hasAlergias = this.formAlergias.hasOwnProperty('alergias')
        let hasAntecedentes = this.formAlergias.hasOwnProperty('antecedentes')

        if (hasAlergias || hasAntecedentes) {
          this.$message.success('Dados inseridos no atendimento')
        } else {
          this.$message.warning('Erro no envio das informações')
        }
      },
      // ---------------- FIM MODAL ALERGIAS ---------------------------

      // RETORNO DOS EXAMES DO COMPONENTE DOS EXAMES PARA A TELA PAI
      retornoExamesTabs(val) {
        // val.exames é o array com todos os exames do componente
        if (val.exames != undefined) {
          if (val.exames.length > 0) {
            this.api.exames = val.exames
          }
        }
      },

      onClickVoltar() {
        this.ausentes === true
          ? this.$router.push({ name: 'Ausentes' })
          : this.$router.push({ name: 'Agendamentos' })
      },

      handleImageError() {
        this.error = true
      },

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
    }
  }
</script>

<style>

  .imagemPaciente {
    width: 110px;
    height: 110px;
    object-fit: cover;
    border-radius: 100px;
    border-width: 6px;
    border-style: solid;
  }

  .chat--notify {
    background: #E6A23C;
    color: #fff !important;
  }

  .chat--button {
    height: 37px !important
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

  .el-form-item__content2 {
    box-sizing: border-box;
    align-items: center;
    border: ridge;
    border-bottom-width: 3px;
    border-top-width: 3px;
    border-right-width: 3px;
    border-left-width: 3px;
    margin-right: 0px;
    margin-bottom: 8px;
    margin-top: 1px;
    margin-left: 0px;
    padding: 2px;
    border-radius: 20px;
    /*line-height: 0 !important;*/
  }

  .el-form-item__content3 {
    box-sizing: border-box;
    align-items: center;
    border: ridge;
    border-bottom-width: 3px;
    border-top-width: 3px;
    border-right-width: 3px;
    border-left-width: 3px;
    margin-right: 0px;
    margin-bottom: 8px;
    margin-top: 1px;
    margin-left: 0px;
    padding-top: 2px;
    padding-bottom: 2px;
    padding-right: 12px;
    padding-left: 12px;
    border-radius: 20px;
  }

  .el-form-item__content4 {
    float: left;
    border: ridge;
    border-bottom-width: 3px;
    border-top-width: 3px;
    border-right-width: 3px;
    border-left-width: 3px;
    width: 160px;
    height: 150px;
    margin-bottom: 8px
  }

  .space-card {
    margin-bottom: 20px;
  }

  .align-center {
    display: flex !important;
    align-items: center !important;
    justify-content: center !important;
  }

  .space-top {
    margin-top: 20px;
  }

  .v--modal-overlay[data-modal="hello-world"] {
    background: transparent;
  }

  .vm--overlay {
    display: flex;
    width: 6%;
    height: 1%;
    background-color: transparent;
  }

  .vm--container {
    width: 100px;
    height: 100px;
    float: right;
    background-color: transparent;
  }
  /** Twilio classes */
  .local_video_container {
    width: 200px;
    height: 110px;
    position: absolute;
    bottom: 40px;
    right: 30px;
  }

  #localTrack video {
    width: 100%;
    height: 100%;
  }

  .remote_video_container {
    flex: 1;
    width: 100%;
    height: 380px;
    align-items: flex-end;
    justify-content: flex-end;
    background: #000;
  }

  #remoteTrack video {
    width: 100%;
    height: 380px;
  }

  .roomTitle {
    border: 1px solid rgb(124, 129, 124);
    padding: 4px;
    color: dodgerblue;
  }

  .scroll-area {
    position: relative;
    width: 100%;
    height: 700px;
    overflow-x: hidden;
  }

  .scroll-area--chat {
    position: relative;
    width: 100%;
    height: 400px;
    overflow-x: hidden;
  }

  .scroll-area2 {
    position: relative;
    width: 100%;
    height: 600px;
    padding-bottom: 50px;
    overflow-x: hidden;
  }

  .scroll-area-memed {
    position: relative;
    width: 100%;
    height: 700px;
    padding-bottom: 50px;
    overflow-x: hidden;
  }

  #chat .el-input__inner {
    height: 36px;
  }

  #chat li {
    font-size: 12px;
    font-weight: 400;
  }

  li.msg__chat {
    text-align: right;
    margin-bottom: 15px;
  }

  .msg__chat span {
    border-radius: 10px;
    border-bottom-right-radius: 0;
    background: #008d95;
    padding: 5px 10px 5px 15px;
    color: #fff;
    font-size: 14px;
    font-weight: 400;
  }

  li.msg__paciente {
    text-align: left;
    margin-bottom: 15px;
  }

  .msg__paciente span {
    border-radius: 10px;
    border-bottom-left-radius: 0;
    background: #225457;
    padding: 5px 15px 5px 10px;
    color: #fff;
    font-size: 14px;
    font-weight: 400;
  }

  .msg__hora {
    font-size: 10px;
  }

  .tempo__atendimento {
    position: absolute;
    color: #fff;
  }

  .divButtonExame {
    display: flex;
    flex-direction: row;
    justify-content: flex-end;
  }

  .div-container-fichas {
    transform: translate(-50%, -50%);
  }

  .container-fichas {
    color: red;
    font-size: 16px;
    font-family: serif;
    text-align: center;
    animation: animation-fichas 2.5s linear infinite;
  }

  /*key frame para animação do alert para o médico*/
  /*@keyframes animation-fichas {
    0% {
      opacity: 0;
    }

    50% {
      opacity: 1.0;
    }

    100% {
      opacity: 0;
    }
  }*/

  .container-nome-paciente {
    display: flex;
    flex-direction: column;
    margin-left: 10px;
    /*width: 100%*/
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

  .container-antecedentes {
    margin-top: 20px;
  }

  .title-consulta {
    font-size: 18px;
  }

  .soap li {
    list-style-type: none;
  }

  .soap {
    font-size: 14px;
  }

  .container-cid-ciap ul {
    padding: 0;
  }

  .container-cid-ciap p {
    margin-bottom: 0;
  }

  .container-comorbidades {
    margin-bottom: 20px;
    margin-top: 20px;
  }

    .container-comorbidades span {
      font-size: 14px;
    }

  .container-como-me-sinto {
    padding-bottom: 40px;
    margin-bottom: 50px;
  }

    .container-como-me-sinto ul {
      padding: 0px;
    }

      .container-como-me-sinto ul li {
        list-style-type: none;
      }

        .container-como-me-sinto ul li span {
          font-size: 14px;
        }

  /*modal style*/
  .scroll-areaModalMedicoes {
    position: relative;
    width: 100%;
    height: 100%;
    padding-bottom: 10vh;
    overflow-x: hidden;
    overflow-y: auto;
  }

  .scroll-areaModalExames {
    position: relative;
    width: 100%;
    height: 100%;
    padding-bottom: 10vh;
    overflow-x: hidden;
    overflow-y: auto;
  }

  .modalVideoMinizado {
    display: none;
  }

  .botaoMenu {
    display: inline-block;
    line-height: 1;
    white-space: nowrap;
    cursor: pointer;
    background: #b5e6e5;
    text-align: center;
    box-sizing: border-box;
    outline: 0;
    margin: 0;
    transition: .1s;
    font-weight: 500;
    -webkit-user-select: none;
    font-size: 14px;
    border-radius: 4px;
    padding: 12px;
    margin-top: 10px;
    margin-bottom: 10px;
    margin-left: 0px;
    border: 1px solid #093434;
  }

    .botaoMenu:focus {
      color: black;
      background-color: #188B8A;
    }

    .botaoMenu:hover {
      color: black;
      background-color: #B5E6E5;
    }

  .el-main > div {
    margin-left: 0px !important;
    margin-right: 0px !important;
  }

  i {
    min-width: auto !important;
  }

  .botaoMenu {
    transition: background-color 0.3s ease;
  }

    .botaoMenu:hover {
      background-color: #01826c;
      color: #FFF;
    }

  .barra {
    position: fixed;
    bottom: 2%;
    margin-bottom: 2px;
    left: 90%;
  }

  .loading-spinner {
    width: 60px;
    height: 60px;
    position: relative;
    margin: 50px auto;
    border: 3px solid #f3f3f3;
    border-radius: 50%;
    border-top-color: #3498db;
    animation: spin 1s linear infinite;
  }



  @keyframes spin {
    0% {
      transform: rotate(0deg);
    }

    100% {
      transform: rotate(360deg);
    }
  }
</style>
