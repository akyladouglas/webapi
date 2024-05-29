<template>

  <el-row :gutter="24">

    <el-col :span="4">
      <!--<div style="display: flex; align-items: center; justify-content: center; flex-direction:column; min-height: 100vh">-->
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
            <div v-if="api.individuo.imagem">
              <img alt="Imagem indisponível no momento" style="width: 110px; height: 110px; object-fit: cover; border-radius: 100%" :src="'../../' + api.individuo.imagem + '.jpg'" :title="api.individuo.nomeCompleto" />
            </div>
            <div v-else class="paciente__missing_photo">
              <div width="500px">
                <img :src="'../../assets/img/' + $store.state.app.empresa.fotoUsuario"
                     :title="$store.state.app.empresa.nome"
                     id="image" />
              </div>
            </div>
          </div>

          <div class="container-nome-paciente">
            <h1>{{api.individuo.nomeCompleto}} | {{api.individuo.sexo === 0 ? ' Masculino ' : ' Feminino '}}</h1>

            <p>Idade: {{getIdade(api.individuo.dataNascimento)}}  - CPFs: {{api.individuo.cpf == undefined ? " Não preenchido " : api.individuo.cpf}} - CNS:{{api.individuo.cns == undefined ? " Não preenchido " : api.individuo.cns}} - Nome da Mãe: {{api.individuo.nomeDaMae == undefined ? " Não preenchido " : api.individuo.nomeDaMae}}</p>
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
                    <el-input type="textarea" v-model="atendimentoTriagem.queixaDoPaciente" disabled />
                  </el-card>
                </div>

                <!--Últimos Atendimentos Registrados-->
                <el-card v-show="listando && api.historico.length === 0">
                  <el-empty v-show="listando && api.historico.length === 0" description="Nenhum histórico encontrado"></el-empty>
                </el-card>

                <el-card style="margin-top:10px; padding-bottom: 10px">

                  <div>
                    <span v-show="api.historico.length > 0" style="font-weight: bold; font-size: 15px;">Últimos Atendimentos</span>
                  </div>

                  <el-table v-show="api.historico.length > 0" ref="tabela" :data="api.historico"
                            highlight-current-row border
                            v-loading.body="loading.historico"
                            :row-class-name="tableRow"
                            class="table--atendimento table--row-click"
                            :default-sort="{prop: 'dataCadastro', order: 'descending'}">
                    <el-table-column label="Data da Consulta" prop="dataCadastro" align="center" width="200" sortable />
                    <el-table-column label="Individuo" prop="individuo.nomeCompleto" align="center" />
                    <el-table-column label="Profissional" prop="profissional.nome" align="center" />
                    <el-table-column label="CRM" prop="profissional.crm" align="center" width="100" />
                  </el-table>
                  <el-col :span="24" v-show="listando">
                    <el-pagination @size-change="handleSizeChangeHistorico"
                                   @current-change="handleCurrentChangeHistorico"
                                   :current-page.sync="paramsHistorico.page"
                                   :page-sizes="[5,10,25]"
                                   :page-size="10"
                                   :total="paramsHistorico.total"
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
                <button class="botaoMenu" style="margin-top: 21px; margin-bottom: 0px; margin-left: 10px; width: 95%; display: flex; justify-content: flex-start; " size="large" @click="openModalAlergias">Informações Adicionais</button>
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
          <el-descriptions class="margin-top" :column="4" border>
            <template slot="title">

              <div v-if="agendamento.corStatusTriagem === 1">
                <div v-if="api.individuo.imagem">
                  <img alt="Imagem indisponível no momento" style="width: 110px; height: 110px; object-fit: cover; border-radius: 100%" :src="'../../' + api.individuo.imagem + '.jpg'" :title="api.individuo.nomeCompleto" />
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
                  </el-descriptions-item>
                </el-descriptions>
              </div>


              <div v-if="agendamento.corStatusTriagem === 2">
                <img v-if="api.individuo.imagem" alt="Imagem indisponível no momento" style="width: 110px; height: 110px; object-fit: cover; border-radius: 100%" :src="'../../' + api.individuo.imagem + '.jpg'" :title="api.individuo.nomeCompleto" />
                <div v-else class="paciente__missing_photo">
                  <div width="500px">
                    <img :src="'../../assets/img/' + $store.state.app.empresa.fotoUsuario"
                         :title="$store.state.app.empresa.nome"
                         id="image" />
                  </div>
                </div>
                <el-descriptions title="" direction="vertical" :column="4" border style="margin-top:20px">
                  <el-descriptions-item label="Pouco Urgente" label-class-name="verde">
                  </el-descriptions-item>
                </el-descriptions>
              </div>


              <div v-if="agendamento.corStatusTriagem === 3">
                <img v-if="api.individuo.imagem" alt="Imagem indisponível no momento" style="width: 110px; height: 110px; object-fit: cover; border-radius: 100%" :src="'../../' + api.individuo.imagem + '.jpg'" :title="api.individuo.nomeCompleto" />
                <div v-else class="paciente__missing_photo">
                  <div width="500px">
                    <img :src="'../../assets/img/' + $store.state.app.empresa.fotoUsuario"
                         :title="$store.state.app.empresa.nome"
                         id="image" />
                  </div>
                </div>
                <el-descriptions title="" direction="vertical" :column="4" border style="margin-top:20px">
                  <el-descriptions-item label="Urgente" label-class-name="amarelo">
                  </el-descriptions-item>
                </el-descriptions>
              </div>


              <div v-if="agendamento.corStatusTriagem === 4">
                <img v-if="api.individuo.imagem" alt="Imagem indisponível no momento" style="width: 110px; height: 110px; object-fit: cover; border-radius: 100%" :src="'../../' + api.individuo.imagem + '.jpg'" :title="api.individuo.nomeCompleto" />
                <div v-else class="paciente__missing_photo">
                  <div width="500px">
                    <img :src="'../../assets/img/' + $store.state.app.empresa.fotoUsuario"
                         :title="$store.state.app.empresa.nome"
                         id="image" />
                  </div>
                </div>
                <el-descriptions title="" direction="vertical" :column="4" border style="margin-top: 20px">
                  <el-descriptions-item label="Muito Urgente" label-class-name="laranja" style="background-color: #e63946">
                  </el-descriptions-item>
                </el-descriptions>
              </div>


              <div v-if="agendamento.corStatusTriagem === 5">
                <img v-if="api.individuo.imagem" alt="Imagem indisponível no momento" style="width: 110px; height: 110px; object-fit: cover; border-radius: 100%" :src="'../../' + api.individuo.imagem + '.jpg'" :title="api.individuo.nomeCompleto" />
                <div v-else class="paciente__missing_photo">
                  <div width="500px">
                    <img :src="'../../assets/img/' + $store.state.app.empresa.fotoUsuario"
                         :title="$store.state.app.empresa.nome"
                         id="image" />
                  </div>
                </div>
                <el-descriptions title="" direction="vertical" :column="4" border style="margin-top: 20px; ">
                  <el-descriptions-item label="Emergência" label-class-name="vermelho" />
                </el-descriptions>
              </div>

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
              <el-button @click="openModalVideo" icon="el-icon-video-camera" type="primary" size="small" id="btn-teleatendimento" v-if="!videoMinimizado && !isMinimizado && !emAndamentoVideoChamada">TeleAtendimento</el-button>

              <el-button @click="openModalMemed" type="primary" size="small" icon="el-icon-document-add">Prescrição</el-button>
            </template>
            <el-descriptions-item>
              <template slot="label">
                Nome
              </template>
              {{api.individuo.nomeCompleto}}
            </el-descriptions-item>
            <el-descriptions-item>
              <template slot="label">
                Cpf
              </template>
              {{api.individuo.cpf}}
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
              {{api.individuo.telefoneCelular}}
            </el-descriptions-item>
            <el-descriptions-item>
              <template slot="label">
                Email
              </template>
              {{api.individuo.email}}
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
          </el-descriptions>

          <el-collapse style="margin-top: 20px">
            <el-collapse-item v-if="agendamento.pressaoSanguinea != null " title="Sinais Vitais" name="1">
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
                <el-descriptions-item label="Saturação O2">
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

            <!--<pre>{{ api.comoMeSintoAtendimento}}</pre>-->
            <div class="container-sintomas">
              <p>Sintomas</p>
              <p class="data-envio">
                <el-tag type="info">Data de envio: {{ moment(api.comoMeSintoAtendimento.data).format('DD/MM/YYYY HH:mm') }}</el-tag>
              </p>

              <!--{{api.comoMeSintoAtendimento}}-->

              <div v-if="api.comoMeSintoAtendimento" class="list-sintomas">
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
            </div>



          </el-collapse>
        </el-card>



        <el-form :model="formAtendimento" :rules="validacoes" labelPosition="top" ref="formAtendimento" label-width="170px">
          <el-col :xl="24">
            <el-card shadow="always" style="margin-top: 20px">
              <el-form-item label="Subjetivo" prop="subjetivo">
                <el-input v-model="formAtendimento.subjetivo" :rows="4" type="textarea" placeholder="" />
              </el-form-item>
              <el-form-item label="Objetivo" prop="objetivo">
                <el-input v-model="formAtendimento.objetivo" :rows="4" prop="objetivo" type="textarea" placeholder="" />
              </el-form-item>
              <el-form-item label="Avaliação" prop="avaliacao">
                <el-input v-model="formAtendimento.avaliacao" :rows="4" type="textarea" placeholder="" />
              </el-form-item>
            </el-card>
          </el-col>

          <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
            <el-card shadow="always" style="margin-top: 20px">
              <div>
                Exames Solicitados e/ou Avaliados
                <el-table v-show="api.exames.length > 0" :data="api.exames" :default-sort="{prop: 'dataDeEnvio', order: 'descending'}" ref="table" border style="margin-top: 20px">
                  <el-table-column show-overflow-tooltip label="Data do envio" prop="dataDeEnvio" align="center" sortable>
                    <template slot-scope="scope">
                      <span>{{ moment(scope.row.dataDeEnvio).format('DD/MM/YYYY') }}</span>
                    </template>
                  </el-table-column>

                  <el-table-column show-overflow-tooltip label="Nome Do Exame" prop="tipoExameId" align="center">
                    <template slot-scope="scope">
                      <span>{{ getTipoExames(scope.row.tipoExameId) }}</span>
                    </template>
                  </el-table-column>

                  <el-table-column prop="resultado" label="Resultado / Avaliado" align="center">
                    <template slot-scope="scope">
                      <span>{{ scope.row.resultado == true ? 'SIM' : 'NÃO' }}</span>
                    </template>
                  </el-table-column>

                  <el-table-column label="Avaliar Exame" width="140">
                    <template slot-scope="scope">
                      <el-dropdown>
                        <el-button type="primary" size="small">
                          Ações <i class="el-icon-arrow-down el-icon--right"></i>
                        </el-button>
                        <el-dropdown-menu slot="dropdown">
                          <ul class="list-unstyled">
                            <li @click="editarAvaliar(scope.row)" v-if="scope.row.resultado == true && scope.row.finalizado == false && scope.row.avaliado != '0001-01-01T00:00:00'" class="el-dropdown-menu__item"><i class="fas fa-user-edit"></i> Editar</li>
                            <li @click="visualizarAvaliacao(scope.row)" v-if="scope.row.resultado == true && scope.row.avaliado != '0001-01-01T00:00:00'" class="el-dropdown-menu__item"><i class="fas fa-user-edit"></i>Visualizar Avaliação</li>
                            <li @click="menuAvaliar(scope.row)" v-if="scope.row.resultado == false && scope.row.finalizado == false" class="el-dropdown-menu__item"><i class="fas fa-user-edit"></i> Avaliar</li>
                            <li @click="visualizar(scope.row)" v-if="scope.row.formato === '.pdf'" class="el-dropdown-menu__item"><i class="far fa-file"></i><span>{{ scope.row.formato === ".pdf" ? "Visualizar PDF" : "Visualizar"}}</span> </li>
                            <li @click="visualizar(scope.row)" v-if="scope.row.formato === '.jpeg' || scope.row.formato === '.png' || scope.row.formato === '.jpg'" class="el-dropdown-menu__item"><i class="far fa-file"></i><span>{{ scope.row.formato === ".jpg" || scope.row.formato === ".jpeg" ||scope.row.formato === ".png" ? "Visualizar Imagem" : "Visualizar"}}</span> </li>
                            <li @click="visualizar(scope.row)" v-if="scope.row.formato === '.mp3' || scope.row.formato === '.wav'" class="el-dropdown-menu__item"><i class="far fa-file"></i><span>{{ scope.row.formato === ".wav" || scope.row.formato === ".mp3" ? "Visualizar Audio" : "Visualizar"}}</span> </li>
                          </ul>
                        </el-dropdown-menu>
                      </el-dropdown>
                    </template>
                  </el-table-column>
                </el-table>

                <el-col :span="24">
                  <el-pagination @size-change="handleSizeChangeExames"
                                 @current-change="handleCurrentChangeExames"
                                 :current-page.sync="paramsExames.page"
                                 :page-sizes="[5,10,25]"
                                 :page-size="5"
                                 :total="paramsExames.total"
                                 layout="total, sizes, prev, pager, next, jumper">
                  </el-pagination>
                </el-col>

              </div>
              <br />
              <!--Menu Dos Exames-->
              <el-col>
                <!--DIV DE AVALIAR-->
                <div style="margin-top: 20px" v-show="statusAvaliar">
                  <el-form :model="formAvaliar" ref="tableAvaliar" label-width="170px" label-position="top">
                    <div class="divButtonExame">
                      <el-button type="danger" size="small" @click="hideAvaliar" icon="el-icon-close"></el-button>
                    </div>
                    <div>
                      <h2 style="padding-bottom: 5px" class="box-card--h2"> Avaliar</h2>
                    </div>

                    <el-row>
                      <el-form-item label="Descrição" prop="descricao">
                        <el-input maxlength="255" type="textarea" v-model="formAvaliar.descricao" />
                      </el-form-item>
                      <el-button @click="onSalvarAvaliacao" type="primary" size="small" style="margin-bottom: 20px">Salvar</el-button>
                    </el-row>
                  </el-form>
                </div>
                <!--DIV DE VISUALIZAR-->
                <div style="margin-top: 20px" v-show="statusVisualizar">
                  <el-form :model="formVisualizar" ref="formVisualizar" label-width="170px" label-position="top">
                    <div class="divButtonExame">
                      <el-button type="danger" size="small" @click="hideVisualizar" icon="el-icon-close"></el-button>
                    </div>

                    <el-row :gutter="20">
                      <el-col :sm="24" :md="4" :lg="4" :xl="4">
                        <el-form-item label="Data Da Avaliação" prop="dataAvaliacao">
                          <el-input v-model="formVisualizar.avaliado" type="input" placeholder="Data Avaliacao" disabled>
                          </el-input>
                        </el-form-item>
                      </el-col>
                    </el-row>
                    <el-row>
                      <el-form-item label="Descrição" prop="descricao">
                        <el-input type="textarea" v-model="formVisualizar.descricao" disabled />
                      </el-form-item>
                    </el-row>
                  </el-form>
                </div>
                <!--DIV DE EDITAR-->
                <div style="margin-top: 20px" v-show="statusEditar">
                  <el-form :model="formEditar" ref="formEditar" label-width="170px" label-position="top">
                    <div class="divButtonExame">
                      <el-button type="danger" size="small" @click="hideEditar" icon="el-icon-close"></el-button>
                    </div>

                    <el-row :gutter="20">
                      <el-col :sm="24" :md="4" :lg="4" :xl="4">

                        <el-form-item label="Data Da Avaliação" prop="dataAvaliacao">
                          <el-input v-model="formEditar.avaliado" type="input" placeholder="Data Avaliacao" disabled>
                          </el-input>
                        </el-form-item>
                      </el-col>
                    </el-row>
                    <el-row>
                      <el-form-item label="Descrição" prop="descricao">
                        <el-input maxlength="255" type="textarea" v-model="formEditar.descricao" />
                      </el-form-item>
                      <el-button @click="onSalvarAvaliacaoEditada" type="primary" size="small" style="margin-bottom: 20px">Salvar</el-button>
                    </el-row>
                  </el-form>
                </div>
              </el-col>
            </el-card>
          </el-col>

          <el-col>
            <el-card shadow="always" style="margin-top: 20px">

              <p>CIAP2</p>

              <el-form-item label="" prop="ciap2">
                <el-select v-model="formAtendimento.ciap2"
                           filterable
                           clearable
                           remote
                           reserve-keyword
                           placeholder="Selecione">
                  <el-option v-for="item in api.ciap2"
                             :key="item.codigo"
                             :label="item.descricao"
                             :value="item.codigo">
                  </el-option>
                </el-select>
              </el-form-item>

              <el-button style="margin-top:15px" type="primary" size="small" @click="onAddButtonCiap" icon="el-icon-plus">Adicionar</el-button>
              <el-button type="danger" size="small" @click="limparCiap2" icon="el-icon-close">Remover</el-button>

              <el-table :data="ciap2Atendimento" border style="margin-top: 20px">
                <el-table-column prop="codCiap" label="CIAP2">
                </el-table-column>
                <el-table-column prop="descCiap" label="Descrição CIAP2">
                </el-table-column>
              </el-table>

            </el-card>




            <el-card shadow="always" style="margin-top: 20px">

              <p>CID10</p>

              <el-form-item style="margin-top: 20px" label="" prop="cid10">
                <el-select v-model="formAtendimento.cid10"
                           filterable
                           clearable
                           remote
                           reserve-keyword
                           placeholder="Selecione">
                  <el-option v-for="item in api.cid10"
                             :key="item.codigo"
                             :label="item.descricao"
                             :value="item.codigo">
                  </el-option>
                </el-select>
              </el-form-item>

              <el-button style="margin-top:15px" type="primary" size="small" @click="onAddButtonCid" icon="el-icon-plus">Adicionar</el-button>
              <el-button type="danger" size="small" @click="limparCid10" icon="el-icon-close">Remover</el-button>


              <el-table :data="cid10Atendimento" border style="margin-top: 20px">
                <el-table-column prop="codCid10" label="CID10">
                </el-table-column>
                <el-table-column prop="descCid10" label="Descrição CID10">
                </el-table-column>
              </el-table>
            </el-card>
          </el-col>
          <!--PLANO-->
          <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
            <el-card shadow="always" style="margin-top: 20px">
              <el-form-item label="Plano de Tratamento" prop="plano">
                <el-input v-model="formAtendimento.plano" :rows="4" type="textarea" placeholder="" />
              </el-form-item>
            </el-card>

            <div style="margin-top: 20px">
              <el-button type="primary" size="large" @click="submitForm('formAtendimento')" icon="el-icon-check">Finalizar Atendimento</el-button>
              <!--RETIRADO A FUNÇÃO DO BOTÃO CANCELAR ATENDIMENTO E RETORNAR PARA AGENDAMENTOS, AGORA O MEDICO TEM QUE FINALIZAR O ATENDIMENTO OBRIGATORIAMENTE
            <el-button type="danger" size="large" @click="onVoltarAgendamento" icon="el-icon-close" disabled>Cancelar Atendimento</el-button>-->
              <el-button @click="openModalMemed" style="float: right" type="primary" size="small" icon="el-icon-document-add">Prescrição</el-button>
              <el-button @click="openModalVideo" icon="el-icon-video-camera" type="primary" size="small" id="btn-teleatendimento" v-if="!videoMinimizado && !isMinimizado && !emAndamentoVideoChamada">TeleAtendimento</el-button>
            </div>
          </el-col>
        </el-form>
      </div>

      <!--<full :isMinimizado="isMinimizado" :emAndamentoVideoChamada="emAndamentoVideoChamada"></full>-->

    </el-col>

    <el-col v-show="telaHistorico == true && telaAtendimento == false && telaSinopse == false && telaCadastroPaciente == false" :span="19" style="border-left: 1px solid #43bec6; margin-top: 20px; min-height:100vh">
      <el-card style="padding-bottom: 30px">
        <div label="Histórico">
          <el-empty v-show="listando && api.historico.length === 0" description="Nenhum histórico encontrado"></el-empty>
          <div style="margin-top:10px">
            <span v-show="api.historico.length > 0" style="font-weight: bold; font-size: 15px;">Últimos Atendimentos</span>
          </div>
          <!--<pre>{{api.historico}}</pre>-->
          <el-table v-show="api.historico.length > 0" ref="tabela" :data="api.historico"
                    highlight-current-row border
                    v-loading.body="loading.historico"
                    :row-class-name="tableRow"
                    class="table--atendimento table--row-click"
                    :default-sort="{prop: 'dataCadastro', order: 'descending'}">
            <el-table-column label="Data da Consulta" prop="dataCadastro" align="center" width="200" sortable />
            <el-table-column label="Individuo" prop="individuo.nomeCompleto" align="center" />
            <el-table-column label="Profissional" prop="profissional.nome" align="center" />
            <el-table-column label="CRM" prop="profissional.crm" align="center" width="100" />
            <el-table-column header-align="center" align="center" width="100">
              <template slot-scope="scope">
                <el-dropdown>
                  <el-button @click="onModalHistoricos(scope.row)" type="primary" size="small">
                    Visualizar
                  </el-button>
                </el-dropdown>
              </template>
            </el-table-column>
          </el-table>
          <el-col :span="24" v-show="listando">
            <el-pagination @size-change="handleSizeChangeHistorico"
                           @current-change="handleCurrentChangeHistorico"
                           :current-page.sync="paramsHistorico.page"
                           :page-sizes="[5,10,25]"
                           :page-size="5"
                           :total="paramsHistorico.total"
                           layout="total, sizes, prev, pager, next, jumper">
            </el-pagination>
          </el-col>
        </div>
      </el-card>
      <modal name="resumoHistoricoModal" :draggable="true" :clickToClose="false" :width="800" :height="500">
        <div class="window-header" style="padding: 15px;padding-bottom: 0px;display: flex;justify-content: flex-end;">
          <el-button type="danger" @click="hideModal" icon="el-icon-close"></el-button>
        </div>

        <VuePerfectScrollbar class="scroll-area-historico" :settings="scrollSettings" key="scrol-atendimento">
          <el-row>
            <el-col :span="12">
              <div class="grid-content bg-purple">
                <div class="soap">
                  <h5 class="title-consulta">Consulta | {{formHistorico.dataCadastro}}</h5>

                  <div style="width:90%; margin-bottom:30px">
                    <div>
                      <p style="font-size: 18px;">Sinais Vitais | Data de envio: {{moment(api.sinaisVitaisHistorico.dataAlteracao).format('DD/MM/YYYY')}}</p>
                      <p>
                        <strong>Pressao Arterial:</strong> {{api.sinaisVitaisHistorico.pressaoArterial == undefined || api.sinaisVitaisHistorico.pressaoArterial == null ? 'Dados não informados' : api.sinaisVitaisHistorico.pressaoArterial + " mmHg"}}
                      </p>
                      <p>
                        <strong>Frequência Cardíaca:</strong> {{api.sinaisVitaisHistorico.frequenciaCardiaca == undefined || api.sinaisVitaisHistorico.frequenciaCardiaca == null ? 'Dados não informados' : api.sinaisVitaisHistorico.frequenciaCardiaca + " bpm"}}
                      </p>
                      <p>
                      <p>
                        <strong>Saturação O²:</strong> {{api.sinaisVitaisHistorico.saturacaoO2 == undefined || api.sinaisVitaisHistorico.saturacaoO2 == null ? 'Dados não informados' : api.sinaisVitaisHistorico.saturacaoO2 + "%"}}
                      </p>
                      <p>
                        <strong>Temperatura:</strong> {{api.sinaisVitaisHistorico.temperatura == undefined || api.sinaisVitaisHistorico.temperatura == null ? 'Dados não informados' : api.sinaisVitaisHistorico.temperatura + " °C"}}
                      </p>
                      <p>
                        <strong>Altura:</strong> {{api.sinaisVitaisHistorico.altura == undefined || api.sinaisVitaisHistorico.altura == null ? 'Dados não informados' : api.sinaisVitaisHistorico.altura + " cm"}}
                      </p>
                      <p>
                        <strong>Peso:</strong> {{api.sinaisVitaisHistorico.peso == undefined || api.sinaisVitaisHistorico.peso == null ? 'Dados não informados' : api.sinaisVitaisHistorico.peso + " kg"}}
                      </p>
                      <p>
                        <strong>IMC:</strong> {{api.sinaisVitaisHistorico.dadosImc == undefined || api.sinaisVitaisHistorico.dadosImc == null ? 'Dados não informados' : api.sinaisVitaisHistorico.dadosImc}}
                      </p>
                      <p>
                        <strong>Grau IMC:</strong> {{api.sinaisVitaisHistorico.dadosImc == undefined || api.sinaisVitaisHistorico.dadosImc == null ? 'Dados não informados' : getGrauImc(api.sinaisVitaisHistorico.dadosImc)}}
                      </p>
                    </div>
                  </div>

                  <p style="font-size: 18px;">Dados Do Atendimento</p>
                  <p>
                    <strong>Objetivo:</strong> {{formHistorico.objetivo == null ? 'Dados não informados' : formHistorico.objetivo}}
                  </p>
                  <p>
                    <strong>Subjetivo:</strong> {{formHistorico.subjetivo == null ? 'Dados não informados' : formHistorico.subjetivo}}
                  </p>
                  <p>
                    <strong>Avaliação:</strong> {{formHistorico.avaliacao == null ? 'Dados não informados' : formHistorico.avaliacao}}
                  </p>
                  <p>
                    <strong>Plano:</strong> {{formHistorico.plano == null ? 'Dados não informados' : formHistorico.plano}}
                  </p>

                  <div class="container-cid-ciap">
                    <p><strong>CID10</strong></p>
                    <div v-if="!formCid.length">Dados não informados</div>
                    <ul>
                      <li v-for="value in formCid">
                        - {{ value.descricao }}
                      </li>
                    </ul>

                    <p><strong>CIAP</strong></p>
                    <div v-if="!formCiap.length">Dados não informados</div>
                    <ul>
                      <li v-for="value in formCiap">
                        - {{ value.descricao }}
                      </li>
                    </ul>
                  </div>





                </div>
              </div>
            </el-col>
            <el-col :span="12">
              <div class="grid-content bg-purple-light">
                <h5 class="title-consulta">Dados do Profissional</h5>
                <h5 class="title-consulta"><strong>Profissional:</strong>{{formProfissional.nome == null ? '' : formProfissional.nome}}</h5>
                <p>{{formProcedimento.descricao == null ? '' : formProcedimento.descricao}}</p>
              </div>
            </el-col>
          </el-row>

          <!--<el-row>
          <el-col :span="24">
            <div class="container-comorbidades">
              <div v-if="formIndividuo.hipertenso ||
                   formIndividuo.diabetes ||
                   formIndividuo.fumante ||
                   formIndividuo.asma ||
                   formIndividuo.doencaPulmao ||
                   formIndividuo.doencaCoracao ||
                   formIndividuo.doencaCancer ||
                   formIndividuo.transplantado ||
                   formIndividuo.doencaComprometeImunidade ||
                   formIndividuo.doencaRins ||
                   formIndividuo.doencaFigado">
                <div>
                  <p>Comorbidades | Data de envio: {{moment(formIndividuo.dataAlteracao).format('DD/MM/YYYY')}}</p>
                </div>
                <div v-if="formIndividuo.hipertenso == true">
                  <span>Hipertenso: <el-tag v-if="formIndividuo.hipertenso" size="small" type="success">Sim</el-tag></span>
                </div>
                <div v-if="formIndividuo.diabetes == true">
                  <span>Diabetes: <el-tag v-if="formIndividuo.diabetes" size="small" type="success">Sim</el-tag></span>
                </div>
                <div v-if="formIndividuo.fumante == true">
                  <span>Fumante: <el-tag v-if="formIndividuo.fumante" size="small" type="success">Sim</el-tag></span>
                </div>
                <div v-if="formIndividuo.asma == true">
                  <span>Asma: <el-tag v-if="formIndividuo.asma" size="small" type="success">Sim</el-tag></span>
                </div>
                <div v-if="formIndividuo.doencaPulmao == true">
                  <span>Doença no Pulmão: <el-tag v-if="formIndividuo.doencaPulmao" size="small" type="success">Sim</el-tag></span>
                </div>
                <div v-if="formIndividuo.doencaCoracao == true">
                  <span>Doença no Coração: <el-tag v-if="formIndividuo.doencaCoracao" size="small" type="success">Sim</el-tag></span>
                </div>
                <div v-if="formIndividuo.doencaCancer == true">
                  <span>Câncer: <el-tag v-if="formIndividuo.doencaCancer" size="small" type="success">Sim</el-tag></span>
                </div>
                <div v-if="formIndividuo.transplantado == true">
                  <span>Transplantado: <el-tag v-if="formIndividuo.transplantado" size="small" type="success">Sim</el-tag></span>
                </div>
                <div v-if="formIndividuo.doencaComprometeImunidade == true">
                  <span>Doença Compromete Imunidade: <el-tag v-if="formIndividuo.doencaComprometeImunidade" size="small" type="success">Sim</el-tag></span>
                </div>

                <div v-if="formIndividuo.doencaRins == true">
                  <span>Doença Nos Rins: <el-tag v-if="formIndividuo.doencaRins" size="small" type="success">Sim</el-tag></span>
                </div>
                <div v-if="formIndividuo.doencaFigado == true">
                  <span>Doença No Figado: <el-tag v-if="formIndividuo.doencaFigado" size="small" type="success">Sim</el-tag></span>
                </div>

              </div>
              <div v-else>
                <p>Comorbidades: Paciente sem comorbidade | Data de envio: {{moment(formIndividuo.dataAlteracao).format('DD/MM/YYYY')}}</p>
              </div>

            </div>
          </el-col>
        </el-row>-->

          <el-row>
            <el-col :span="24">
              <div class="container-como-me-sinto">
                <div v-if="!api.comoMeSinto.length">
                  <p>Dados não informados</p>
                </div>
                <!--<div v-for="item in api.comoMeSintoHistorico">
                <p>Sintomas - Atendimento Anterior - Data de envio: {{moment(item.data).format('DD/MM/YYYY HH:mm')}}</p>
              </div>-->
                <!--<pre>{{api.comoMeSintoAtendimentoHistorico}}</pre>-->

                <div v-for="item in api.comoMeSintoAtendimentoHistorico">
                  <div v-if="item.temperatura ||
                       item.tosse ||
                       item.coriza ||
                       item.dorCorpo ||
                       item.dorAbdominal ||
                       item.fraqueza ||
                       item.dorGarganta ||
                       item.nauseaVomito ||
                       item.dorCabeca ||
                       item.sairCasa ||
                       item.contatoPessoas ||
                       item.dificuldadeRespirar ||
                       item.taquicardia ||
                       item.perdaOlfatoPaladar ||
                       item.diarreia ||
                       item.temperaturaRetornou ||
                       item.atendidoServicoSaude ||
                       item.sintomasGripais">
                    <p>Sintomas - Atendimento Anterior - Data de envio: {{moment(item.data).format('DD/MM/YYYY HH:mm')}}</p>
                    <ul>
                      <li v-if="item.temperatura"><span>Temperatura: <el-tag size="small" type="success">Sim</el-tag></span></li>
                      <li v-if="item.tosse"><span>Tosse: <el-tag size="small" type="success">Sim</el-tag></span></li>
                      <li v-if="item.coriza"><span>Coriza: <el-tag size="small" type="success">Sim</el-tag></span></li>
                      <li v-if="item.dorCorpo"><span>Dor no corpo: <el-tag size="small" type="success">Sim</el-tag></span></li>
                      <li v-if="item.dorAbdominal"><span>Dor abdominal: <el-tag size="small" type="success">Sim</el-tag></span></li>
                      <li v-if="item.fraqueza"><span>Fraqueza: <el-tag size="small" type="success">Sim</el-tag></span></li>
                      <li v-if="item.dorGarganta"><span>Dor de garganta: <el-tag size="small" type="success">Sim</el-tag></span></li>
                      <li v-if="item.nauseaVomito"><span>Náusea/Vômito: <el-tag size="small" type="success">Sim</el-tag></span></li>
                      <li v-if="item.dorCabeca"><span>Dor de cabeça: <el-tag size="small" type="success">Sim</el-tag></span></li>
                      <li v-if="item.sairCasa"><span>Tem saido de casa: <el-tag size="small" type="success">Sim</el-tag></span></li>
                      <li v-if="item.contatoPessoas"><span>Contato com pessoas: <el-tag size="small" type="success">Sim</el-tag></span></li>
                      <li v-if="item.dificuldadeRespirar"><span>Dificuldade de respirar: <el-tag size="small" type="success">Sim</el-tag></span></li>
                      <li v-if="item.taquicardia"><span>Taquicardia: <el-tag size="small" type="success">Sim</el-tag></span></li>
                      <li v-if="item.perdaOlfatoPaladar"><span>Perda de olfato ou paladar: <el-tag size="small" type="success">Sim</el-tag></span></li>
                      <li v-if="item.diarreia"><span>Diarréia: <el-tag size="small" type="success">Sim</el-tag></span></li>
                      <li v-if="item.temperaturaRetornou"><span>Temperatura retornou: <el-tag size="small" type="success">Sim</el-tag></span></li>
                      <li v-if="item.atendidoServicoSaude"><span>Atendido por serviço de saúde: <el-tag size="small" type="success">Sim</el-tag></span></li>
                      <li v-if="item.sintomasGripais"><span>Sintomas gripais: <el-tag size="small" type="success">Sim</el-tag></span></li>
                    </ul>
                    <!--Infos Adicionais:-->
                    <p><strong>Informações Adicionais</strong></p>

                    <p style="font-size: 14px;">
                      Alergias: {{(formHistorico.alergias == null || formHistorico.alergias ==  "") ? 'Dados não informados' : formHistorico.alergias}}
                    </p>
                    <p style="font-size: 14px;">
                      Antecedentes: {{(formHistorico.antecedentes == null || formHistorico.antecedentes  == "") ? 'Dados não informados' : formHistorico.antecedentes }}
                    </p>



                  </div>
                  <div v-else>
                    <p>Sintomas: Paciente sem sintomas - Data de envio: {{moment(item.data).format('DD/MM/YYYY HH:mm')}}</p>


                  </div>
                </div>
              </div>
            </el-col>
          </el-row>
        </VuePerfectScrollbar>
      </modal>
    </el-col>

    <el-col v-show="telaCadastroPaciente == true && telaHistorico == false && telaAtendimento == false && telaSinopse == false " :span="19" style="border-left: 1px solid #43bec6">
      <div style="margin-top:20px">
        <el-card>

          <el-row>
            <el-col :span="12">
              <h2 class="box-card--h2"> Dados Cadastrais </h2>
            </el-col>
          </el-row>

          <el-row>

            <el-form :model="formIndividuoCadastro" status-icon :rules="validacoes" :disabled="isDisabled"
                     ref="formIndividuoCadastro" label-width="120px" label-position="top" class="form--individuo">

              <el-row :gutter="24">

                <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="15">
                  <el-form-item label="Nome Completo" prop="nomeCompleto">
                    <el-input maxlength="100" show-word-limit v-model="formIndividuoCadastro.nomeCompleto" :disabled="disabledComorbidades" />
                  </el-form-item>
                </el-col>

                <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="15">
                  <el-form-item label="Nome Social" prop="nomeSocial">
                    <el-input maxlength="100" show-word-limit v-model="formIndividuoCadastro.nomeSocial" :disabled="disabledComorbidades" />
                  </el-form-item>
                </el-col>

                <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="15" v-if="!formIndividuoCadastro.temMaeDesconhecida">
                  <el-form-item label="Nome da Mãe" prop="nomeDaMae">
                    <el-input maxlength="100" show-word-limit v-model="formIndividuoCadastro.nomeDaMae" :disabled="disabledComorbidades" />
                  </el-form-item>
                </el-col>

              </el-row>

              <el-row :gutter="24">

                <el-col :xs="24" :sm="24" :md="12" :lg="10" :xl="10">
                  <el-form-item label="Email" prop="email">
                    <el-input maxlength="80" show-word-limit v-model="formIndividuoCadastro.email" :disabled="disabledComorbidades" />
                  </el-form-item>
                </el-col>

                <el-col :xs="24" :sm="24" :md="12" :lg="7" :xl="10">
                  <el-form-item label="CPF" prop="cpf">
                    <el-input show-word-limit v-model="formIndividuoCadastro.cpf" masked="true" maxlength="14" v-mask="'###.###.###-##'" :disabled="disabledComorbidades" />
                  </el-form-item>
                </el-col>

                <el-col :xs="24" :sm="24" :md="12" :lg="7" :xl="10">
                  <el-form-item label="CNS" prop="cns">
                    <el-input v-model="formIndividuoCadastro.cns" masked="true" maxlength="15" show-word-limit :disabled="disabledComorbidades" />
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
                    <el-select filterable placeholder="Selecione o Sexo" v-model="formIndividuoCadastro.sexo" :disabled="disabledComorbidades">
                      <el-option v-for="option in enums.sexos" :value="option.value" :label="option.label" :key="option.value" />
                    </el-select>
                  </el-form-item>
                </el-col>

                <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="10">
                  <el-form-item label="Raça ou Cor" prop="racaOuCor">
                    <el-select filterable placeholder="Selecione a Raça ou Cor" v-model="formIndividuoCadastro.racaOuCor" :disabled="disabledComorbidades">
                      <el-option v-for="option in enums.racaOuCor" :value="option.value" :label="option.label" :key="option.value" />
                    </el-select>
                  </el-form-item>
                </el-col>

              </el-row>

              <el-row :gutter="24">

                <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="10">
                  <el-form-item label="Data Nascimento" prop="dataNascimento">
                    <el-date-picker prefix-icon="fas fa-calendar-day" v-model="formIndividuoCadastro.dataNascimento" type="date" format="dd-MM-yyyy" :disabled="disabledComorbidades" />
                  </el-form-item>
                </el-col>

                <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="10">
                  <el-form-item label="Telefone Celular" prop="telefoneCelular">
                    <el-input v-model="formIndividuoCadastro.telefoneCelular" show-word-limit masked="true" maxlength="15" v-mask="'(##) #####-####'" :disabled="disabledComorbidades">
                      <i slot="prefix" class="fas fa-mobile-alt"></i>
                    </el-input>
                  </el-form-item>
                </el-col>

              </el-row>

              <el-row :gutter="24">

                <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="10">
                  <el-form-item label="CEP" prop="logradouroCep">
                    <el-input v-model="formIndividuoCadastro.logradouroCep" show-word-limit @input="getCep" masked="true" maxlength="9" v-mask="'#####-###'" :disabled="disabledComorbidades" />
                  </el-form-item>
                </el-col>

                <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="10">
                  <el-form-item label="Estado" prop="ufAbreviado">
                    <el-select filterable placeholder="Selecione o Estado" v-model="formIndividuoCadastro.ufAbreviado"
                               v-loading.body="loading.ufs" @change="onSelectUf" :disabled="disabledComorbidades">
                      <el-option v-for="option in api.ufs" :value="option.ufAbreviado" :label="option.ufExtenso" :key="option.ufAbreviado" />
                    </el-select>
                  </el-form-item>
                </el-col>

              </el-row>

              <el-row :gutter="24">

                <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="10">
                  <el-form-item label="Cidade" prop="cidadeId">
                    <el-select filterable placeholder="Selecione a Cidade" v-model="formIndividuoCadastro.cidadeId"
                               v-loading.body="loading.cidades" :disabled="disabledComorbidades">
                      <el-option v-for="option in api.cidades" :value="option.ibge" :label="option.nome" :key="option.ibge" />
                    </el-select>
                  </el-form-item>
                </el-col>

                <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="10">
                  <el-form-item label="Bairro" prop="logradouroBairro">
                    <el-input v-model="formIndividuoCadastro.logradouroBairro" :disabled="disabledComorbidades" />
                  </el-form-item>
                </el-col>

              </el-row>

              <el-row :gutter="24">

                <el-col :xs="24" :sm="24" :md="12" :lg="12" :xl="10">
                  <el-form-item label="Endereço" prop="logradouro">
                    <el-input v-model="formIndividuoCadastro.logradouro" :disabled="disabledComorbidades" />
                  </el-form-item>
                </el-col>


                <el-col :xs="24" :sm="24" :md="12" :lg="4" :xl="3">
                  <el-form-item label="Número" prop="logradouroNumero">
                    <el-input type="number" v-model="formIndividuoCadastro.logradouroNumero" show-word-limit :disabled="disabledComorbidades" />
                  </el-form-item>
                </el-col>

                <el-col :xs="24" :sm="24" :md="12" :lg="8" :xl="7">
                  <el-form-item label="Complemento" prop="logradouroComplemento">
                    <el-input v-model="formIndividuoCadastro.logradouroComplemento" show-word-limit maxlength="30" :disabled="disabledComorbidades" />
                  </el-form-item>
                </el-col>
              </el-row>

              <el-row :gutter="20">

                <el-col :xs="24">
                  <el-form-item>
                    <el-button flat icon="fas fa-save" type="success" @click="onClickSalvarIndividuo('formIndividuoCadastro')" v-loading.body="loading.individuos" :disabled="disabledComorbidades"> Salvar</el-button>
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

      <el-button v-if="isMinimizado" style="height:48px; width:49px" slot="reference" type="primary" icon="el-icon-video-camera" circle @click="maximizar"></el-button>

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
            <el-button type="danger" @click="hideDialogVideo" v-if="twilio.roomName" size="small" id="fechar-videochamada">Encerrar</el-button>
            <el-button type="default" @click="minimizar" size="small">Minimizar</el-button>

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

    <!-- MODAL HISTÓRICO-->
    <modal name="modalHistorico" :resizable="true" :draggable="true" :clickToClose="false" :min-width="800" :min-height="600" :max-height="700" :width="800" :height="600">
      <div class="window-header" style="">
        <el-button type="danger" @click="hideModalHistorico" icon="el-icon-close"></el-button>
        <el-button v-show="!listando" type="warning" @click="onVoltar" icon="el-icon-back"></el-button>
      </div>
      <VuePerfectScrollbar class="scroll-area2" :settings="scrollSettingsMedicoes" key="scrol-atendimento">
        <el-empty v-show="listando && api.historico.length === 0" description="Nenhum histórico encontrado"></el-empty>
        <el-row style="padding: 8px" v-show="listando && api.historico.length > 0">
          <el-col :span="24">
            <el-table ref="tabela" :data="api.historico"
                      highlight-current-row border
                      v-loading.body="loading.historico"
                      class="table--atendimento table--row-click">
              <el-table-column label="Data da Consulta" prop="dataCadastro" align="center" width="200" fixed sortable />
              <el-table-column label="Individuo" prop="individuo.nomeCompleto" />
              <el-table-column label="Atendido Medico" prop="atendidoMedico">
                <template slot-scope="scope">
                  <span>{{ scope.row.atendidoMedico == true ? 'Sim' : 'Não' }}</span>
                </template>
              </el-table-column>
              <el-table-column label="Atendido Triagem" prop="atendidoTriagem">
                <template slot-scope="scope">
                  <span>{{ scope.row.atendidoTriagem == true ? 'Sim' : 'Não' }}</span>
                </template>
              </el-table-column>
              <el-table-column label="Profissional" prop="profissional.nome" />
              <el-table-column label="CRM" prop="profissional.crm" />


              <el-table-column header-align="center" align="right" width="140" fixed="right">
                <template slot-scope="scope">
                  <el-dropdown>
                    <el-button @click="onVisualizar(scope.$index, scope.row)" type="primary" size="small">
                      Visualizar
                    </el-button>
                  </el-dropdown>
                </template>
              </el-table-column>
            </el-table>
          </el-col>


          <el-col :span="24" v-show="listando">
            <el-pagination @size-change="handleSizeChangeHistorico"
                           @current-change="handleSizeChangeHistorico"
                           :current-page.sync="paramsHistorico.page"
                           :page-sizes="[10,25,50,100]"
                           :page-size="paramsHistorico.take"
                           :total="paramsHistorico.totalCount"
                           layout="total, sizes, prev, pager, next, jumper">
            </el-pagination>
          </el-col>
        </el-row>
        <el-card shadow="always" v-show="!listando">
          <el-descriptions class="margin-top" :column="3" border>
            <template slot="title">
              <img v-if="api.individuo.imagem" style="width: 110px; height: 110px; object-fit: cover; border-radius: 100%" :src="'../../' + api.individuo.imagem + '.jpg'" :title="api.individuo.nomeCompleto" />
              <div v-else class="paciente__missing_photo">
                <div width="500px">
                  <img :src="'../../assets/img/' + $store.state.app.empresa.fotoUsuario"
                       :title="$store.state.app.empresa.nome"
                       id="image" />
                </div>
              </div>
            </template>
            <el-descriptions-item>
              <template slot="label">
                Nome
              </template>
              {{api.individuo.nomeCompleto}}
            </el-descriptions-item>
            <el-descriptions-item>
              <template slot="label">
                Cpf
              </template>
              {{api.individuo.cpf}}
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
              {{api.individuo.telefoneCelular}}
            </el-descriptions-item>
            <el-descriptions-item>
              <template slot="label">
                Email
              </template>
              {{api.individuo.email}}
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
          </el-descriptions>

          <!--SINAIS VITAIS HISTORICO NÃO ALTERAR-->
          <el-collapse style="margin-top: 20px">
            <el-collapse-item v-if="api.sinaisVitaisHistorico.pressaoArterial != null" title="Sinais Vitais" name="1">
              <div style="margin-bottom: 10px">
                <el-tag type="info">Data de envio: {{ moment(api.sinaisVitaisHistorico.dataAlteracao).format('DD/MM/YYYY HH:mm') }}</el-tag>
              </div>
              <el-descriptions :column="3" border>
                <el-descriptions-item label="Pre. Arterial">
                  <el-tag size="small">{{ api.sinaisVitaisHistorico.pressaoArterial && api.sinaisVitaisHistorico.pressaoArterial + ' mmhg' }}</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Fre. Cardiaca">
                  <el-tag size="small">{{ api.sinaisVitaisHistorico.frequenciaCardiaca && api.sinaisVitaisHistorico.frequenciaCardiaca + ' bpm' }}</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Saturação O2">
                  <el-tag size="small">{{ api.sinaisVitaisHistorico.saturacaoO2 && api.sinaisVitaisHistorico.saturacaoO2 + ' %' }}</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Temperatura">
                  <el-tag size="small">{{ api.sinaisVitaisHistorico.temperatura && api.sinaisVitaisHistorico.temperatura + ' °C' }}</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Altura">
                  <el-tag size="small">{{ api.sinaisVitaisHistorico.altura && api.sinaisVitaisHistorico.altura + ' cm' }}</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Peso">
                  <el-tag size="small">{{ api.sinaisVitaisHistorico.peso && api.sinaisVitaisHistorico.peso + ' kg' }}</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="IMC">
                  <el-tag size="small">{{  api.sinaisVitaisHistorico.dadosImc }}</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="GRAU IMC">
                  <el-tag size="small">{{  getGrauImc(api.sinaisVitaisHistorico.dadosImc) }}</el-tag>
                </el-descriptions-item>
              </el-descriptions>
            </el-collapse-item>

            <el-collapse-item title="Comorbidades" name="2">
              <div style="margin-bottom: 10px">
                <span v-if="api.comorbidades.dataCadastro" size="small">
                  <el-tag type="info">Data de envio: {{ moment(api.comorbidades.dataCadastro).format('DD/MM/YYYY HH:mm') }}</el-tag>
                </span>
              </div>
              <el-descriptions :column="4" border>
                <el-descriptions-item label="Hipertenso">
                  <el-tag v-if="api.comorbidades.hipertenso" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Diabetes">
                  <el-tag v-if="api.comorbidades.diabetes" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Fumante">
                  <el-tag v-if="api.comorbidades.fumante" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Asma">
                  <el-tag v-if="api.comorbidades.asma" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Doença no Pulmão">
                  <el-tag v-if="api.comorbidades.doencaPulmao" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Doença no Coração">
                  <el-tag v-if="api.comorbidades.doencaCoracao" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Câncer">
                  <el-tag v-if="api.comorbidades.doencaCancer" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Transplantado">
                  <el-tag v-if="api.comorbidades.transplantado" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Doença Compromete Imunidade">
                  <el-tag v-if="api.comorbidades.doencaComprometeImunidade" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Doença Nos Rins">
                  <el-tag v-if="api.comorbidades.doencaRins" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Doença No Figado">
                  <el-tag v-if="api.comorbidades.doencaFigado" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Sintomas Gripais">
                  <el-tag v-if="api.comorbidades.sintomasGripais" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <!--<el-descriptions-item label="Anemia Falciforme">
                <el-tag v-if="api.comorbidades.anemiaFalciforme" size="small" type="success">Sim</el-tag>
                <el-tag v-else size="small" type="danger">Não</el-tag>
              </el-descriptions-item>-->
              </el-descriptions>
            </el-collapse-item>


            <el-collapse-item v-if="api.comoMeSintoHistorico.length > 0" title="Sintomas Atendimento Atual" name="3">
              <div style="margin-bottom: 10px">
                <span v-if="api.comorbidades.dataCadastro" size="small">
                  <el-tag type="info">Data de envio: {{ moment(api.comoMeSintoHistorico[0].data).format('DD/MM/YYYY HH:mm') }}</el-tag>
                </span>
              </div>
              <el-descriptions :column="4" border>
                <el-descriptions-item label="Temperatura">
                  <el-tag v-if="api.comoMeSintoHistorico[0].temperatura" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Tosse">
                  <el-tag v-if="api.comoMeSintoHistorico[0].tosse" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Coriza">
                  <el-tag v-if="api.comoMeSintoHistorico[0].coriza" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Dor no corpo">
                  <el-tag v-if="api.comoMeSintoHistorico[0].dorCorpo" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Dor abdominal">
                  <el-tag v-if="api.comoMeSintoHistorico[0].dorAbdominal" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Fraqueza">
                  <el-tag v-if="api.comoMeSintoHistorico[0].fraqueza" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Dor na garganta">
                  <el-tag v-if="api.comoMeSintoHistorico[0].dorGarganta" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Náusea/ Vômito">
                  <el-tag v-if="api.comoMeSintoHistorico[0].nauseaVomito" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Dor de cabeça">
                  <el-tag v-if="api.comoMeSintoHistorico[0].dorCabeca" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Tem saído de casa">
                  <el-tag v-if="api.comoMeSintoHistorico[0].sairCasa" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Contato com pessoas">
                  <el-tag v-if="api.comoMeSintoHistorico[0].contatoPessoas" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Dificuldade de respirar">
                  <el-tag v-if="api.comoMeSintoHistorico[0].dificuldadeRespirar" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Taquicardia">
                  <el-tag v-if="api.comoMeSintoHistorico[0].taquicardia" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Diarréia">
                  <el-tag v-if="api.comoMeSintoHistorico[0].diarreia" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Temperatura retornou">
                  <el-tag v-if="api.comoMeSintoHistorico[0].temperaturaRetornou" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Atendido por serviço de saúde">
                  <el-tag v-if="api.comoMeSintoHistorico[0].atendidoServicoSaude" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Perda de olfato ou pladar">
                  <el-tag v-if="api.comoMeSintoHistorico[0].perdaOlfatoPaladar" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Sintomas gripais">
                  <el-tag v-if="api.comoMeSintoHistorico[0].sintomasGripais" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
              </el-descriptions>
            </el-collapse-item>

            <el-collapse-item v-if="api.comoMeSintoHistorico.length > 0" title="Sintomas Atendimento Anterior" name="19">
              <div style="margin-bottom: 10px">
                <span v-if="api.comorbidades.dataCadastro" size="small">
                  <el-tag type="info">Data de envio: {{ moment(api.comoMeSintoHistorico[0].data).format('DD/MM/YYYY HH:mm') }}</el-tag>
                </span>
              </div>
              <el-descriptions :column="4" border>
                <el-descriptions-item label="Temperatura">
                  <el-tag v-if="api.comoMeSintoHistorico[0].temperatura" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Tosse">
                  <el-tag v-if="api.comoMeSintoHistorico[0].tosse" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Coriza">
                  <el-tag v-if="api.comoMeSintoHistorico[0].coriza" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Dor no corpo">
                  <el-tag v-if="api.comoMeSintoHistorico[0].dorCorpo" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Dor abdominal">
                  <el-tag v-if="api.comoMeSintoHistorico[0].dorAbdominal" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Fraqueza">
                  <el-tag v-if="api.comoMeSintoHistorico[0].fraqueza" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Dor na garganta">
                  <el-tag v-if="api.comoMeSintoHistorico[0].dorGarganta" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Náusea/ vômito">
                  <el-tag v-if="api.comoMeSintoHistorico[0].nauseaVomito" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Dor de cabeça">
                  <el-tag v-if="api.comoMeSintoHistorico[0].dorCabeca" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Tem saído de casa">
                  <el-tag v-if="api.comoMeSintoHistorico[0].sairCasa" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Contato com pessoas">
                  <el-tag v-if="api.comoMeSintoHistorico[0].contatoPessoas" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Dificuldade de respirar">
                  <el-tag v-if="api.comoMeSintoHistorico[0].dificuldadeRespirar" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Taquicardia">
                  <el-tag v-if="api.comoMeSintoHistorico[0].taquicardia" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Diarréia">
                  <el-tag v-if="api.comoMeSintoHistorico[0].diarreia" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Temperatura retornou">
                  <el-tag v-if="api.comoMeSintoHistorico[0].temperaturaRetornou" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Atendido por serviço de saúde">
                  <el-tag v-if="api.comoMeSintoHistorico[0].atendidoServicoSaude" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Perda de olfato ou pladar">
                  <el-tag v-if="api.comoMeSintoHistorico[0].perdaOlfatoPaladar" size="small" type="success">Sim</el-tag>
                  <el-tag v-else size="small" type="danger">Não</el-tag>
                </el-descriptions-item>
              </el-descriptions>
            </el-collapse-item>
          </el-collapse>


          <el-form v-if="true" :model="historicoSelecionado" status-icon :disabled="isDisabled"
                   label-width="120px" style="line-height: 0 !important; margin-top: 20px" class="form--agendamento">
            <el-row>
              <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
                <div>
                  <h3>Objetivo</h3>
                  <el-input type="textarea" v-model="historicoSelecionado.objetivo" />
                </div>
              </el-col>
            </el-row>

            <el-row>
              <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
                <div>
                  <h3>Avaliação</h3>
                  <el-input type="textarea" v-model="historicoSelecionado.avaliacao" />
                </div>
              </el-col>
            </el-row>

            <el-row>
              <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
                <div>
                  <h3>Plano</h3>
                  <el-input type="textarea" v-model="historicoSelecionado.plano" />
                </div>
              </el-col>
            </el-row>

            <el-row>
              <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
                <div>
                  <h3>Subjetivo</h3>
                  <el-input type="textarea" v-model="historicoSelecionado.subjetivo" />
                </div>
              </el-col>
            </el-row>



            <el-row v-if="api.ciapHistorico != null">
              <el-form-item prop="ciap">
                <el-table :data="api.ciapHistorico" style="margin-left: -120px; width:100%;">
                  <el-table-column prop="descricao" label="Ciap">
                  </el-table-column>
                </el-table>
              </el-form-item>
            </el-row>

            <el-row v-if="api.cid10Historico != null">
              <el-form-item prop="ciap">

                <el-table :data="api.cid10Historico" style="margin-left: -120px;">
                  <el-table-column prop="descricao" label="Cid10">
                  </el-table-column>
                </el-table>
              </el-form-item>
            </el-row>

          </el-form>








          <el-row>
            <!-- Menu Dos Exames HISTORICO -->
            <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
              <div>
                <h3>Exames Solicitados e/ou Avaliados</h3>
                <el-table :data="api.examesHistorico" ref="table" border style="margin-top: 20px">

                  <el-table-column prop="avaliado" label="Avaliado" width="100">
                    <template slot-scope="scope">
                      <span>{{ scope.row.avaliado }}</span>
                    </template>
                  </el-table-column>

                  <el-table-column label="Nome Do Exame" prop="tipoExameId" align="center">
                    <template slot-scope="scope">
                      <span>{{ getTipoExames(scope.row.tipoExameId)}}</span>
                    </template>
                  </el-table-column>

                  <el-table-column prop="resultado" label="Resultado" width="90">
                    <template slot-scope="scope">
                      <span>{{ scope.row.resultado == true ? 'SIM' : 'NÃO' }}</span>
                    </template>
                  </el-table-column>

                  <el-table-column label="Avaliar Exame" width="150" header-align="center" align="center" fixed="right">
                    <template slot-scope="scope">
                      <el-dropdown>
                        <el-button type="primary" size="small">
                          Ações <i class="el-icon-arrow-down el-icon--right"></i>
                        </el-button>
                        <el-dropdown-menu slot="dropdown">
                          <ul class="list-unstyled">

                            <li @click="visualizarAvaliacaoHistorico(scope.row)" class="el-dropdown-menu__item"><i class="fas fa-user-edit"></i>Visualizar Avaliação</li>
                            <li @click="visualizarHistorico(scope.row)" v-if="scope.row.formato === '.pdf'" class="el-dropdown-menu__item"><i class="far fa-file"></i><span>{{ scope.row.formato === ".pdf" ? "Visualizar PDF" : "Visualizar"}}</span> </li>
                            <li @click="visualizarHistorico(scope.row)" v-if="scope.row.formato === '.jpeg' || scope.row.formato === '.png' || scope.row.formato === '.jpg'" class="el-dropdown-menu__item"><i class="far fa-file"></i><span>{{ scope.row.formato === ".jpg" || scope.row.formato === ".jpeg" ||scope.row.formato === ".png" ? "Visualizar Imagem" : "Visualizar"}}</span> </li>
                            <li @click="visualizarHistorico(scope.row)" v-if="scope.row.formato === '.mp3' || scope.row.formato === '.wav'" class="el-dropdown-menu__item"><i class="far fa-file"></i><span>{{ scope.row.formato === ".wav" || scope.row.formato === ".mp3" ? "Visualizar Audio" : "Visualizar"}}</span> </li>
                          </ul>
                        </el-dropdown-menu>
                      </el-dropdown>
                    </template>
                  </el-table-column>
                </el-table>
              </div>


            </el-col>
          </el-row>

          <div style="margin-top: 20px" v-if="statusVisualizarHistorico">
            <el-form :model="formVisualizarHistorico" ref="formVisualizarHistorico" label-width="170px" label-position="top">
              <div class="divButtonExame">
                <el-button type="danger" size="small" @click="hideVisualizarHistorico" icon="el-icon-close"></el-button>
              </div>

              <el-row :gutter="20">
                <el-col :sm="24" :md="4" :lg="4" :xl="4">

                  <el-form-item label="Data Da Avaliação" prop="dataAvaliacao">
                    <el-input v-model="formVisualizarHistorico.avaliado" type="input" placeholder="Data Avaliacao" disabled>
                    </el-input>
                  </el-form-item>
                </el-col>
              </el-row>
              <el-row>
                <el-form-item label="Descrição" prop="descricao">
                  <el-input type="textarea" v-model="formVisualizarHistorico.descricao" disabled />
                </el-form-item>
              </el-row>
            </el-form>
          </div>


        </el-card>
      </VuePerfectScrollbar>
    </modal>


    <!-- MODAL MEDIÇÕES INICIO loading.agendamentos || loading.auscultacao-->


    <modal name="modalMedicoes" :resizable="false" :draggable="true" :clickToClose="false" width="1000" height="600">

      <VuePerfectScrollbar class="scroll-areaModalMedicoes" :settings="scrollSettings" key="scrol-atendimento">
        <div class="window-header" style="padding: 5px;display: flex;justify-content: flex-end;">
          <el-button style="width:40px; display:flex; align-content:space-around; justify-content:center" type="danger" @click="hideModalMedicoes" icon="el-icon-close"></el-button>
        </div>



        <el-row style="margin-left:5px; ">
          <h2 style="padding-bottom: 5px" class="box-card--h2"> Histórico de Medições</h2>
        </el-row>



        <el-tabs type="border-card" @tab-click="handleTabClickMedicoesHeader">
          <!--Antropometria-->
          <el-tab-pane label="Antropometria">
            <div class="loading-spinner" v-if="loading.agendamentos"></div>
            <el-row v-if="!loading.agendamentos">
              <el-col :span="24">
                <el-table ref="tabela" :data="api.agendamentos"
                          highlight-current-row border
                          class="table--agendamentos table--row-click">
                  <el-table-column label="Data e Hora da medição" prop="dataCadastro" align="center">
                    <template slot-scope="scope">
                      {{scope.row.dataAlteracao != null || scope.row.dataAlteracao != "0001-01-01T00:00:00" ? moment(scope.row.dataAlteracao).format('DD/MM/YYYY HH:mm') : 'Erro na Exibição da Data'}}
                    </template>
                  </el-table-column>
                  <el-table-column label="Peso (Kg)" prop="peso" align="center">
                    <template slot-scope="scope">
                      {{scope.row.peso != null || scope.row.peso != "" ? scope.row.peso : 'Erro na Exibição do Peso'}}
                    </template>
                  </el-table-column>
                  <el-table-column label="Altura (cm)" prop="altura" align="center">
                    <template slot-scope="scope">
                      {{scope.row.altura != null || scope.row.altura != "" ? scope.row.altura : 'Erro na Exibição da Altura'}}
                    </template>
                  </el-table-column>
                  <el-table-column label="IMC (kg/m²)" prop="peso" align="center">
                    <template slot-scope="scope">
                      {{calcularIMC(scope.row.peso, scope.row.altura)}}
                    </template>
                  </el-table-column>
                </el-table>
              </el-col>
              <el-col :span="24" v-show="listando">
                <el-pagination @size-change="handleSizeChangeAgendamentos"
                               @current-change="handleCurrentChangeAgendamentos"
                               :current-page.sync="paramsAgendamentosTable.page"
                               :page-sizes="[3, 5]"
                               :page-size="3"
                               :total="paramsAgendamentosTable.total"
                               layout="total, sizes, prev, pager, next, jumper">
                </el-pagination>
              </el-col>
            </el-row>

            <div style="margin-top: 20px">
              <el-tabs type="border-card" @tab-click="handleTabClickMedicoesGrafico">
                <el-tab-pane label="Peso (Kg)">

                  <highcharts :options="graficoPeso"></highcharts>

                </el-tab-pane>
                <el-tab-pane label="Altura">

                  <highcharts :options="graficoAltura"></highcharts>

                </el-tab-pane>
                <el-tab-pane label="IMC">

                  <highcharts :options="graficoIMC"></highcharts>

                </el-tab-pane>
              </el-tabs>
            </div>

          </el-tab-pane>

          <!--Sinais Vitais-->
          <el-tab-pane label="Sinais Vitais">
            <div class="loading-spinner" v-if="loading.agendamentos"></div>
            <el-row v-if="!loading.agendamentos">
              <el-col :span="24">
                <el-table ref="tabela" :data="api.agendamentos"
                          highlight-current-row border
                          class="table--agendamentos table--row-click">
                  <el-table-column label="Data da Medição" prop="dataCadastro" align="center">
                    <template slot-scope="scope">
                      {{scope.row.dataCadastro != null || scope.row.dataCadastro != "0001-01-01T00:00:00" ? moment(scope.row.dataCadastro).format('DD/MM/YYYY HH:mm') : 'Erro na Exibição da Data'}}
                    </template>
                  </el-table-column>
                  <el-table-column label="Pressão Arterial(mmHg)" prop="pressaoSanguinea" align="center">
                    <template slot-scope="scope">
                      {{scope.row.pressaoSanguinea != null || scope.row.pressaoSanguinea != "" ? scope.row.pressaoSanguinea : 'Erro na Exibição da Pressao Sanguinea'}}
                    </template>
                  </el-table-column>
                  <el-table-column label="Freq. Cardíaca(bpm)" prop="batimentoCardiaco" align="center">
                    <template slot-scope="scope">
                      {{scope.row.batimentoCardiaco != null || scope.row.batimentoCardiaco != "" ? scope.row.batimentoCardiaco : 'Erro na Exibição da Freq. Cardíaca'}}
                    </template>
                  </el-table-column>
                  <el-table-column label="Saturação de O2(%)" prop="oxigenacaoSanguinea" align="center">
                    <template slot-scope="scope">
                      {{scope.row.oxigenacaoSanguinea != null || scope.row.oxigenacaoSanguinea != "" ? scope.row.oxigenacaoSanguinea : 'Erro na Exibição da Saturação'}}
                    </template>
                  </el-table-column>
                  <el-table-column label="Temperatura(°C)" prop="temperatura" align="center">
                    <template slot-scope="scope">
                      {{scope.row.temperatura != null || scope.row.temperatura != "" ? scope.row.temperatura : 'Erro na Exibição da Temperatura'}}
                    </template>
                  </el-table-column>
                </el-table>
              </el-col>
              <el-col :span="24" v-show="listando">
                <el-pagination @size-change="handleSizeChangeAgendamentos"
                               @current-change="handleCurrentChangeAgendamentos"
                               :current-page.sync="paramsAgendamentosTable.page"
                               :page-sizes="[3, 5, 10]"
                               :page-size="3"
                               :total="paramsAgendamentosTable.total"
                               layout="total, sizes, prev, pager, next, jumper">
                </el-pagination>
              </el-col>
            </el-row>

            <div style="margin-top: 20px">
              <el-tabs type="border-card" @tab-click="handleTabClickMedicoesGrafico">
                <el-tab-pane label="Pressão Arterial">

                  <highcharts :options="graficoPressaoArterial"></highcharts>

                </el-tab-pane>
              </el-tabs>
            </div>
          </el-tab-pane>

          <!--Auscultação-->
          <el-tab-pane label="Auscultação">
            <div class="loading-spinner" v-if="loading.auscultacao"></div>
            <el-row v-if="!loading.auscultacao">
              <el-col :span="24">
                <el-table ref="tabela" :data="api.auscultacao"
                          highlight-current-row border
                          class="table--row-click">
                  <el-table-column label="Data de Envio" prop="dataDeEnvio" align="center">
                    <template slot-scope="scope">
                      {{scope.row.dataDeEnvio != null || scope.row.dataDeEnvio != "0001-01-01T00:00:00" ? moment(scope.row.dataDeEnvio).format('DD/MM/YYYY HH:mm') : 'Erro na Exibição da Data'}}
                    </template>
                  </el-table-column>
                  <el-table-column label="Nome Do Exame" prop="Nome" align="center">
                    <template slot-scope="scope">
                      {{scope.row.nome}}
                    </template>
                  </el-table-column>
                  <el-table-column label="Formato" prop="Formato" align="center">
                    <template slot-scope="scope">
                      {{scope.row.formato}}
                    </template>
                  </el-table-column>


                  <el-table-column header-align="center" fixed="right" width="145">
                    <template slot-scope="scope">
                      <el-dropdown>
                        <el-button type="primary" size="small">
                          Ações <i class="el-icon-arrow-down el-icon--right"></i>
                        </el-button>
                        <el-dropdown-menu slot="dropdown">
                          <ul class="list-unstyled">
                            <li @click="onVisualizarAtendimento(scope.row)" class="el-dropdown-menu__item"><i class="fas fa-eye"></i> Visualizar</li>

                          </ul>
                        </el-dropdown-menu>

                      </el-dropdown>
                    </template>
                  </el-table-column>

                </el-table>
              </el-col>
              <el-col :span="24" v-show="listando">
                <el-pagination @size-change="handleSizeChangeAuscultacao"
                               @current-change="handleCurrentChangeAuscultacao"
                               :current-page.sync="paramsAuscultacao.page"
                               :page-sizes="[10, 15, 20]"
                               :page-size="10"
                               :total="paramsAuscultacao.total"
                               layout="total, sizes, prev, pager, next, jumper">
                </el-pagination>
              </el-col>
            </el-row>


          </el-tab-pane>
        </el-tabs>
      </VuePerfectScrollbar>
    </modal>
    <!-- MODAL MEDIÇÕES FIM -->
    <!-- MODAL COMORBIDADES INICIO-->
    <modal name="modalComorbidades" :resizable="false" :draggable="true" :clickToClose="false" width="800" height="550">
      <VuePerfectScrollbar class="scroll-areaModalMedicoes" :settings="scrollSettings" key="scrol-atendimento">
        <div class="window-header" style="padding: 5px;display: flex;justify-content: flex-end;">
          <el-button style="width:40px; display:flex; align-content:space-around; justify-content:center" type="danger" @click="hideModalComorbidades" icon="el-icon-close"></el-button>
        </div>



        <!--<pre>{{api.comorbidades}}</pre>-->
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
                <div v-if="api.comorbidades.hipertenso"><span>Hipertenso: <el-tag size="small" type="success">Sim</el-tag></span></div>
              </div>
              <div v-if="api.comorbidades">
                <div v-if="api.comorbidades.diabetes"><span>Diabetes: <el-tag size="small" type="success">Sim</el-tag></span></div>
              </div>
              <div v-if="api.comorbidades">
                <div v-if="api.comorbidades.fumante"><span>Fumante: <el-tag size="small" type="success">Sim</el-tag></span></div>
              </div>
              <div v-if="api.comorbidades">
                <div v-if="api.comorbidades.asma"><span>Asma: <el-tag size="small" type="success">Sim</el-tag></span></div>
              </div>
              <div v-if="api.comorbidades">
                <div v-if="api.comorbidades.doencaPulmao"><span>Doença no Pulmão: <el-tag size="small" type="success">Sim</el-tag></span></div>
              </div>
              <div v-if="api.comorbidades">
                <div v-if="api.comorbidades.doencaCoracao"><span>Doença no Coração: <el-tag size="small" type="success">Sim</el-tag></span></div>
              </div>
              <div v-if="api.comorbidades">
                <div v-if="api.comorbidades.doencaCancer"><span>Câncer: <el-tag size="small" type="success">Sim</el-tag></span></div>
              </div>
              <div v-if="api.comorbidades">
                <div v-if="api.comorbidades.transplantado"><span>Transplantado: <el-tag size="small" type="success">Sim</el-tag></span></div>
              </div>
              <div v-if="api.comorbidades">
                <div v-if="api.comorbidades.doencaComprometeImunidade"><span>Doença Compromete Imunidade: <el-tag size="small" type="success">Sim</el-tag></span></div>
              </div>
              <div v-if="api.comorbidades">
                <div v-if="api.comorbidades.doencaRins"><span>Doença Nos Rins: <el-tag size="small" type="success">Sim</el-tag></span></div>
              </div>
              <div v-if="api.comorbidades">
                <div v-if="api.comorbidades.doencaFigado"><span>Doença No Figado: <el-tag size="small" type="success">Sim</el-tag></span></div>
              </div>
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
      <VuePerfectScrollbar class="scroll-areaModalMedicoes" :settings="scrollSettings" key="scrol-atendimento">
        <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
          <h2 style="padding-bottom: 5px" class="box-card--h2"> Resultados de exames</h2>
        </el-col>

        <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">

          <div>

            <el-table v-show="api.exames.length > 0"
                      :data="api.exames"
                      :default-sort="{prop: 'dataDeEnvio', order: 'descending'}"
                      ref="table"
                      border style="margin-top: 20px">

              <el-table-column show-overflow-tooltip label="Data do envio" prop="dataDeEnvio" align="center" sortable>
                <template slot-scope="scope">
                  <span>{{ moment(scope.row.dataDeEnvio).format('DD/MM/YYYY') }}</span>
                </template>
              </el-table-column>

              <el-table-column show-overflow-tooltip label="Nome Do Exame" prop="tipoExameId" align="center">
                <template slot-scope="scope">
                  <span>{{ getTipoExames(scope.row.tipoExameId) }}</span>
                </template>
              </el-table-column>

              <el-table-column prop="resultado" label="Resultado / Avaliado#" align="center">
                <template slot-scope="scope">
                  <span>{{ scope.row.resultado == true ? 'SIM' : 'NÃO' }}</span>
                </template>
              </el-table-column>

              <el-table-column label="Avaliar Exame" width="140">
                <template slot-scope="scope">
                  <el-dropdown>
                    <el-button type="primary" size="small">
                      Ações <i class="el-icon-arrow-down el-icon--right"></i>
                    </el-button>
                    <el-dropdown-menu slot="dropdown">
                      <ul class="list-unstyled">
                        <!--v-if="statusEnviadoAvaliacao && scope.row.resultado == true"-->
                        <li @click="editarAvaliar(scope.row)" v-if="scope.row.resultado == true && scope.row.finalizado == false && scope.row.avaliado != '0001-01-01T00:00:00'" class="el-dropdown-menu__item"><i class="fas fa-user-edit"></i> Editar</li>
                        <li @click="visualizarAvaliacao(scope.row)" v-if="scope.row.resultado == true && scope.row.avaliado != '0001-01-01T00:00:00'" class="el-dropdown-menu__item"><i class="fas fa-user-edit"></i>Visualizar Avaliação</li>

                        <li @click="menuAvaliar(scope.row)" v-if="scope.row.resultado == false && scope.row.finalizado == false && !disabledComorbidades" class="el-dropdown-menu__item"><i class="fas fa-user-edit"></i> Avaliar</li>
                        <li @click="visualizar(scope.row)" v-if="scope.row.formato === '.pdf'" class="el-dropdown-menu__item"><i class="far fa-file"></i><span>{{ scope.row.formato === ".pdf" ? "Visualizar PDF" : "Visualizar"}}</span> </li>
                        <li @click="visualizar(scope.row)" v-if="scope.row.formato === '.jpeg' || scope.row.formato === '.png' || scope.row.formato === '.jpg'" class="el-dropdown-menu__item"><i class="far fa-file"></i><span>{{ scope.row.formato === ".jpg" || scope.row.formato === ".jpeg" ||scope.row.formato === ".png" ? "Visualizar Imagem" : "Visualizar"}}</span> </li>
                        <li @click="visualizar(scope.row)" v-if="scope.row.formato === '.mp3' || scope.row.formato === '.wav'" class="el-dropdown-menu__item"><i class="far fa-file"></i><span>{{ scope.row.formato === ".wav" || scope.row.formato === ".mp3" ? "Visualizar Audio" : "Visualizar"}}</span> </li>
                      </ul>
                    </el-dropdown-menu>
                  </el-dropdown>
                </template>
              </el-table-column>
            </el-table>

            <el-col :span="24">
              <el-pagination @size-change="handleSizeChangeExames"
                             @current-change="handleCurrentChangeExames"
                             :current-page.sync="paramsExames.page"
                             :page-sizes="[10,25,50]"
                             :page-size="10"
                             :total="paramsExames.total"
                             layout="total, sizes, prev, pager, next, jumper">
              </el-pagination>
            </el-col>

            <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
              <h2 style="padding-bottom: 5px; padding-top: 20px" class="box-card--h2"> Resultados de exames ECO F200</h2>
            </el-col>

            <el-table v-show="api.examesF200.length > 0"
                      :data="api.examesF200"
                      :default-sort="{prop: 'dataTransferenciaEcoPc', order: 'descending'}"
                      ref="table"
                      border style="margin-top: 20px">

              <el-table-column show-overflow-tooltip label="Data do envio" prop="dataExameEco" align="center" sortable>
                <template slot-scope="scope">
                  <span>{{ moment(scope.row.dataExameEco).format('DD/MM/YYYY') }}</span>
                </template>
              </el-table-column>

              <el-table-column show-overflow-tooltip label="Tipo do Exame" prop="tipoExameEco" align="center">
                <template slot-scope="scope">
                  <span>{{ (scope.row.tipoExameEco) }}</span>
                </template>
              </el-table-column>

              <el-table-column prop="resultadoExameEco" label="Resultado" align="center">
                <template slot-scope="scope">
                  <span>{{ scope.row.resultadoExameEco }}</span>
                </template>
              </el-table-column>

              <el-table-column label="Visualizar exame" width="140">
                <template slot-scope="scope">
                  <el-dropdown>
                    <el-button type="primary" size="small">
                      Ações <i class="el-icon-arrow-down el-icon--right"></i>
                    </el-button>
                    <el-dropdown-menu slot="dropdown">
                      <ul class="list-unstyled">
                        <li @click="visualizarExameF200(scope.row)" v-if="scope.row.formato === '.pdf'" class="el-dropdown-menu__item"><i class="far fa-file"></i><span>{{ scope.row.formato === ".pdf" ? "Visualizar PDF" : "Visualizar"}}</span> </li>
                      </ul>
                    </el-dropdown-menu>
                  </el-dropdown>
                </template>
              </el-table-column>
            </el-table>

            <el-col :span="24">
              <el-pagination @size-change="handleSizeChangeExamesF200"
                             @current-change="handleCurrentChangeExamesF200"
                             :current-page.sync="paramsExamesF200.page"
                             :page-sizes="[10,25,50]"
                             :page-size="10"
                             :total="paramsExamesF200.total"
                             layout="total, sizes, prev, pager, next, jumper">
              </el-pagination>
            </el-col>

          </div>
          <br />
          <!-- Menu Dos Exames -->
          <el-col>
            <!-- DIV DE AVALIAR -->
            <div style="margin-top: 20px" v-show="statusAvaliar">
              <el-form :model="formAvaliar" ref="tableAvaliar" label-width="170px" label-position="top">
                <div class="divButtonExame">
                  <el-button type="danger" size="small" @click="hideAvaliar" icon="el-icon-close"></el-button>
                </div>
                <el-row>
                  <el-form-item label="Descrição" prop="descricao">
                    <el-input maxlength="255" type="textarea" v-model="formAvaliar.descricao" />
                  </el-form-item>
                  <el-button @click="onSalvarAvaliacao" type="primary" size="small" style="margin-bottom: 20px">Salvar</el-button>
                </el-row>
              </el-form>
            </div>

            <!-- DIV DE VISUALIZAR-->
            <div style="margin-top: 20px" v-show="statusVisualizar">
              <el-form :model="formVisualizar" ref="formVisualizar" label-width="170px" label-position="top">
                <div class="divButtonExame">
                  <el-button type="danger" size="small" @click="hideVisualizar" icon="el-icon-close"></el-button>
                </div>

                <el-row :gutter="20">
                  <el-col :sm="24" :md="4" :lg="4" :xl="4">

                    <el-form-item label="Data Da Avaliação" prop="dataAvaliacao">
                      <el-input v-model="formVisualizar.avaliado" type="input" placeholder="Data Avaliacao" disabled>
                      </el-input>
                    </el-form-item>
                  </el-col>
                </el-row>
                <el-row>
                  <el-form-item label="Descrição" prop="descricao">
                    <el-input type="textarea" v-model="formVisualizar.descricao" disabled />
                  </el-form-item>
                </el-row>
              </el-form>
            </div>

            <!--DIV DE EDITAR-->
            <div style="margin-top: 20px" v-show="statusEditar">
              <el-form :model="formEditar" ref="formEditar" label-width="170px" label-position="top">
                <div class="divButtonExame">
                  <el-button type="danger" size="small" @click="hideEditar" icon="el-icon-close"></el-button>
                </div>

                <el-row :gutter="20">
                  <el-col :sm="24" :md="4" :lg="4" :xl="4">

                    <el-form-item label="Data Da Avaliação" prop="dataAvaliacao">
                      <el-input v-model="formEditar.avaliado" type="input" placeholder="Data Avaliacao" disabled>
                      </el-input>
                    </el-form-item>
                  </el-col>
                </el-row>
                <el-row>
                  <el-form-item label="Descrição" prop="descricao">
                    <el-input maxlength="255" type="textarea" v-model="formEditar.descricao" />
                  </el-form-item>
                  <el-button @click="onSalvarAvaliacaoEditada" type="primary" size="small" style="margin-bottom: 20px">Salvar</el-button>
                </el-row>
              </el-form>
            </div>
          </el-col>

        </el-col>

      </VuePerfectScrollbar>
    </modal>
    <!-- MODAL EXAMES FIM-->

    <!-- MODAL ALERGIAS
  <modal name="modalAlergias" :resizable="false" :draggable="true" :clickToClose="false" width="1000" height="600">
    <div class="window-header" style="padding: 5px;display: flex;justify-content: flex-end;">
      <el-button style="width:40px; display:flex; align-content:space-around; justify-content:center" type="danger" @click="hideModalAlergias" icon="el-icon-close"></el-button>
    </div>
    <VuePerfectScrollbar class="scroll-areaModalMedicoes" :settings="scrollSettings" key="scrol-atendimento">

      <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
        <h2 style="padding-bottom: 5px" class="box-card--h2"> Informações Adicionais</h2>
      </el-col>


      <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" v-if="api.individuo" style="margin-top: 20px">
        <el-form :model="formAlergiasEdit" status-icon ref="formAlergiasEdit" label-width="120px" label-position="top" class="forms--usuario">

          <el-form-item label="Alergias" prop="alergias">
            <el-input type="textarea" v-model="formAlergiasEdit.alergias" :disabled="disabledComorbidades" />
          </el-form-item>
          <el-form-item label="Antecedentes Familiares" prop="antecedentes">
            <el-input type="textarea" show-word-limit v-model="formAlergiasEdit.antecedentes" :disabled="disabledComorbidades" />
          </el-form-item>

          <el-button size="small" flat icon="fas fa-save" type="success" style="margin-top: 10px" @click="onClickAlergias(formAlergiasEdit)" :disabled="disabledComorbidades"> Salvar </el-button>
        </el-form>
      </el-col>

    </VuePerfectScrollbar>
  </modal>
  -->
    <!--Dialog Alergias-->
    <el-dialog title="" :visible.sync="dialogVisible" width="50%" :before-close="handleClose">
      <!--Title-->
      <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
        <h2 style="padding-bottom: 5px" class="box-card--h2">Informações Adicionais</h2>
      </el-col>
      <!--Form-->
      <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" style="margin-top: 20px">
        <el-form :model="formAlergiasEdit" status-icon ref="formAlergiasEdit" label-width="120px" label-position="top" class="forms--usuario">

          <el-form-item label="Alergias" prop="alergias">
            <el-input type="textarea" v-model="formAlergiasEdit.alergias" />
          </el-form-item>
          <el-form-item label="Antecedentes Familiares" prop="antecedentes">
            <el-input type="textarea" show-word-limit v-model="formAlergiasEdit.antecedentes" />
          </el-form-item>
        </el-form>
      </el-col>
      <span slot="footer" class="dialog-footer">
                <el-button @click="dialogVisible = false" size="small">Cancelar</el-button>
                <el-button type="primary" size="small" style="margin-top: 10px" @click="onClickAlergias(formAlergiasEdit)">Salvar</el-button>
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



            <!--<pre>{{agendamento}}</pre>-->
            <el-row>
              <el-col :xs="12" :sm="12" :md="3" :lg="3" :xl="3">
                <ge
              <div v-if="agendamento.corStatusTriagem === 1">
                <div v-if="api.individuo.imagem">
                  <img alt="Imagem indisponível no momento" style="width: 110px; height: 110px; object-fit: cover; border-radius: 100%" :src="'../../' + api.individuo.imagem + '.jpg'" :title="api.individuo.nomeCompleto" />
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
                <img v-if="api.individuo.imagem" alt="Imagem indisponível no momento" style="width: 110px; height: 110px; object-fit: cover; border-radius: 100%" :src="'../../' + api.individuo.imagem + '.jpg'" :title="api.individuo.nomeCompleto" />
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
                <img v-if="api.individuo.imagem" alt="Imagem indisponível no momento" style="width: 110px; height: 110px; object-fit: cover; border-radius: 100%" :src="'../../' + api.individuo.imagem + '.jpg'" :title="api.individuo.nomeCompleto" />
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
                <img v-if="api.individuo.imagem" alt="Imagem indisponível no momento" style="width: 110px; height: 110px; object-fit: cover; border-radius: 100%" :src="'../../' + api.individuo.imagem + '.jpg'" :title="api.individuo.nomeCompleto" />
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
                <img v-if="api.individuo.imagem" alt="Imagem indisponível no momento" style="width: 110px; height: 110px; object-fit: cover; border-radius: 100%" :src="'../../' + api.individuo.imagem + '.jpg'" :title="api.individuo.nomeCompleto" />
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















            <!--<pre>{{api.individuo}}</pre>-->
            <el-descriptions class="margin-top" :column="4" style="margin-top:30px" border>
              cr
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

<style>
  .vermelho {
    background: #e63946 !important;
    color: #FFF !important;
    padding-bottom: -12px
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
  import Integration from './Integration'
  import PrescricaoMemed from './PrescricaoMemed'
  import Stopwatch from '../../components/UIComponents/StopWatch.vue'
  import { Client as ConversationsClient } from '@twilio/conversations'
  import Twilio, { createLocalVideoTrack } from 'twilio-video'
  import { mask } from 'vue-the-mask'
  import Full from '../../components/containers/Full.vue'
  import audioNotificacao from '../../assets/audios/chat-notification.mp3'
  import Hub from '../../Hub'
  var somNotificao = new Audio(audioNotificacao)

  export default {
    name: 'Atendimento',
    mixins: [Utils],
    components: { Integration, 'prescricao-memed': PrescricaoMemed, Log, Stopwatch, VuePerfectScrollbar, 'full': Full },
    directives: { mask },
    data () {
      return {
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

        // inicio graficos
        graficoPeso: {
          chart: {
            type: 'line'
          },
          title: {
            text: 'Peso'
          },
          subtitle: {
            text: ''
          },
          xAxis: {
            categories: [],
            title: {
              text: 'Idade(Ano e Mês quando houve o atendimento)'
            }
          },
          yAxis: {
            title: {
              text: 'Peso (Kg)'
            }
          },
          plotOptions: {
            line: {
              dataLabels: {
                enabled: true
              },
              enableMouseTracking: true
            }
          },
          series: [{
            name: '',
            data: []
          }]
        },

        graficoAltura: {
          chart: {
            type: 'line'
          },
          title: {
            text: 'Altura'
          },
          subtitle: {
            text: ''
          },
          xAxis: {
            categories: [],
            title: {
              text: 'Idade(Ano e Mês quando houve o atendimento)'
            }
          },
          yAxis: {
            title: {
              text: 'Altura(cm)'
            }
          },
          plotOptions: {
            line: {
              dataLabels: {
                enabled: true
              },
              enableMouseTracking: true
            }
          },
          series: [{
            name: '',
            data: []
          }]
        },

        graficoIMC: {
          chart: {
            type: 'line'
          },
          title: {
            text: 'IMC'
          },
          subtitle: {
            text: ''
          },
          xAxis: {
            categories: [],
            title: {
              text: 'Idade(Ano e Mês quando houve o atendimento)'
            }
          },
          yAxis: {
            title: {
              text: 'IMC(kg/m²)'
            },
            plotBands: [{ // Laranja Baixo Peso
              from: 0.0,
              to: 18.4,
              color: 'rgba(255, 236, 220, 0.8)',
              label: {
                text: 'Baixo Peso',
                style: {
                  color: '#606060'
                }
              }
            },
            { // Verde Adequado ou Eutrófico
              from: 18.5,
              to: 24.9,
              color: 'rgba(221, 255, 221, 0.8)',
              label: {
                text: 'Adequado ou Eutrófico',
                style: {
                  color: '#606060'
                }
              }
            },
            { // Amarelo Sobrepeso
              from: 25.0,
              to: 29.9,
              color: 'rgba(255, 255, 221, 0.8)',
              label: {
                text: 'Sobrepeso',
                style: {
                  color: '#606060'
                }
              }
            },
            { // Vermelho Obesidade
              from: 30,
              to: 100,
              color: 'rgba(255, 221, 221, 0.8)',
              label: {
                text: 'Obesidade',
                style: {
                  color: '#606060'
                }
              }
            } ]
          },

          plotOptions: {
            line: {
              dataLabels: {
                enabled: true
              },
              enableMouseTracking: true
            }
          },
          series: [{
            name: '',
            data: []
          }]
        },

        graficoPressaoArterial: {
          chart: {
            type: 'line'
          },
          title: {
            text: 'Pressão Arterial'
          },
          subtitle: {
            text: ''
          },
          xAxis: {
            categories: [],
            title: {
              text: 'Dia'
            }
          },
          yAxis: {
            title: {
              text: 'Pressão Arterial(mmHg)'
            }
          },
          plotOptions: {
            line: {
              dataLabels: {
                enabled: true
              },
              enableMouseTracking: true
            }
          },
          series: [{
            name: 'Pressão Sistólica',
            data: []
          },
          {
            name: 'Pressão Diastólica',
            data: []
          }]
        },

        // fim graficos

        // modal comorbidades
        formComorbidades: {},

        formAgendamentoHistorico: {},
        formIndividuo: {},
        formIndividuoCadastro: {},
        formProfissional: {},
        formProcedimento: {},
        formCid: [],
        formCiap: [],
        formHistorico: {},
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

        comoMeSintoHistorico: {},
        historicoSelecionado: {},
        paginacaoExame: {},

        // mensagem de aviso header
        alert: true,

        // flag para aparecer o botão atender
        exibirAtendimento: true,

        // flag para desativar edição das comorbidades
        disabledComorbidades: false,

        // flags das telas
        telaSinopse: true,
        telaAtendimento: false,
        telaHistorico: false,
        telaCadastroPaciente: false,

        // EXAMES
        statusAvaliar: false,
        statusEditar: false,
        statusVisualizar: false,
        statusVisualizarHistorico: false,
        statusEnviadoAvaliacao: false,
        formAvaliar: {},
        formVisualizar: {},
        formVisualizarHistorico: {},
        formEditar: {},

        usePrescricao: false,

        // CONTADOR
        cronometrando: false,
        resetTimer: false,
        tempoDoContador: 0,
        tempoTotalDoAtendimento: null,

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
        isDisabled: false,
        listando: true,
        videoMinimizado: false,
        emAndamentoVideoChamada: false,
        isValid: false,
        formAtendimento: {
          individuoCiap: [],
          individuoCid10: [],
          locomocao: null
        },
        cid10Atendimento: [],
        ciap2Atendimento: [],
        filtroParams: {},
        erros: [],
        enums: {
          sexos: [
            ..._enums.sexos
          ],
          racaOuCor: [
            ..._enums.racaOuCor
          ],
          tipoExames: [
            ..._enums.tipoExames
          ],
          tipoDaConsulta: [
            ..._enums.tipoDaConsulta
          ]
        },
        ciap: [],
        cid10: [],
        arrExames: [],
        routeParams: {},

        validacoes: {
          subjetivo: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' },
            { min: 3, message: 'Nome não pode conter menos de 4', trigger: 'submit' }
          ],
          objetivo: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' },
            { min: 3, message: 'Nome não pode conter menos de 4', trigger: 'submit' }
          ],
          avaliacao: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' },
            { min: 3, message: 'Nome não pode conter menos de 4', trigger: 'submit' }
          ],
          plano: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' },
            { min: 3, message: 'Nome não pode conter menos de 4 ', trigger: 'submit' }
          ]
          // ciap2: [{ required: true, message: 'Campo obrigatório', trigger: ['blur', 'change'] }],
          // cid10: [{ required: true, message: 'Campo obrigatório', trigger: ['blur', 'change'] }]
        },
        api: {
          sinaisVitaisHistorico: {},
          sinaisVitais: {},
          cid10: [],
          ciap2: [],
          cid10Historico: [],
          ciapHistorico: [],
          estabelecimentos: [],
          motivoConsulta: [],
          ciapsCadastrados: [],
          comorbidades: {},
          comoMeSinto: {},
          comoMeSintoHistorico: [],
          individuo: {},
          profissional: {},
          historico: [],
          exames: [],
          examesHistorico: [],
          acompanhamento: {},
          cidades: [],
          comoMeSintoAtendimento: [],
          comoMeSintoAtendimentoHistorico: [],
          auscultacao: [],
          examesF200: []
        },
        loading: {
          individuos: false,
          cid10: false,
          ciap: false,
          estabelecimentos: false,
          motivoConsulta: false,
          ciapsCadastrados: false,
          loadingCiaps: false,
          atendimentos: false,
          historico: false,
          agendamentos: false,
          acompanhamento: false,
          modalMenuHistorico: false,
          auscultacao: false

        },
        paramsCid10: {
          skip: 1,
          take: 9999999,
          sort: '+Id ASC',
          total: 0
        },
        paramsCiap: {
          skip: 1,
          take: 9999999,
          sort: '+Id ASC',
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
        paramsAcompanhamentoHistorico: {
          skip: 1,
          take: 9999,
          sort: 'a.Data DESC',
          total: 0
        },
        params: {
          skip: 1,
          take: 10,
          sort: 'DataCadastro ASC',
          total: 0
        },
        paramsExames: {
          skip: 1,
          take: 5,
          sort: 'Nome ASC',
          total: 0
        },
        paramsExamesF200: {
          skip: 1,
          take: 5,
          sort: 'DataExameEco ASC',
          total: 0
        },
        paramsHistorico: {
          skip: 1,
          take: 10,
          sort: 'DataCadastro DESC',
          total: 0
        },
        paramsExamesHistorico: {
          skip: 1,
          take: 5,
          sort: 'IndividuoId ASC',
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
        },

        // modal inicio
        paramsAgendamentosTable: {
          skip: 1,
          take: 3,
          individuoId: this.$route.params.agendamento.individuoId,
          sort: 'a.DataCadastro DESC',
          total: 0
        },
        paramsAuscultacao: {
          skip: 1,
          sort: 'Nome ASC',
          take: 10
        }
        // modal fim
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
    created () {
      // video chamada
      window.addEventListener('beforeunload', this.leaveRoomIfJoined)

      // params para chamar os dados dos graficos

      var paramsCreated = {
        skip: 1,
        take: 9999,
        individuoId: this.$route.params.agendamento.individuoId,
        sort: 'a.DataCadastro ASC',
        total: 0
      }

      // dados dos graficos
      _api.agendamentos.get(paramsCreated)
        .then(res => {
          res.data.forEach(data => {
            // Nome do paciente na Linha,
            if (data.individuo.nomeCompleto != '') {
              this.graficoPeso.series[0].name = data.individuo.nomeCompleto
              this.graficoAltura.series[0].name = data.individuo.nomeCompleto
              this.graficoIMC.series[0].name = data.individuo.nomeCompleto
            }

            // Retorno dos dados do peso,
            if (data.peso != '' || data.peso != null || data.peso != undefined) {
              var peso = parseFloat(data.peso)
              this.graficoPeso.series[0].data.push(peso)
            }

            // Retorno da Altura
            if (data.altura != null || data.altura != '') {
              var altura = parseFloat(data.altura)
              this.graficoAltura.series[0].data.push(altura)
            }

            // Retorno do IMC
            if ((data.peso != null || data.peso != '') && (data.altura != null || data.altura != '')) {
              var imcInicial = (data.peso / ((data.altura / 100) * (data.altura / 100)))
              var imcFinal = parseFloat(imcInicial).toFixed(1)
              var imc = parseFloat(imcFinal)
              this.graficoIMC.series[0].data.push(imc)
            }

            // Retorno da Idade na epoca do atendimento do paciente
            var d2 = data.individuo.dataNascimento
            var d1 = moment(data.dataCadastro)
            var idadeMes = d1.diff(d2, 'Month')
            var idadeAno = parseFloat(idadeMes / 12).toFixed(1)
            var idadeString = idadeAno.toString()

            // adicionando idade aos eixos
            if (idadeString != '') {
              this.graficoPeso.xAxis.categories.push(idadeString)
              this.graficoAltura.xAxis.categories.push(idadeString)
              this.graficoIMC.xAxis.categories.push(idadeString)
            }

            // Pressao Arterial
            if (data.pressaoSanguinea != '' || data.pressaoSanguinea != null) {
              // Retornando a data para o eixo X
              const locale = 'pt-br'
              var dataFinal = new Date(data.dataCadastro).toLocaleDateString(locale)

              // Retornando os dados para o grafico
              var resultado = []
              if (data.pressaoSanguinea != undefined) {
                resultado = data.pressaoSanguinea.split('/')
              }

              // adicionando a data
              this.graficoPressaoArterial.xAxis.categories.push(dataFinal)

              // adicionando os dados
              this.graficoPressaoArterial.series[0].data.push(parseFloat(resultado[0]))
              this.graficoPressaoArterial.series[1].data.push(parseFloat(resultado[1]))
            }
          })
        }
        )
    },

    async mounted () {
      await this.clickButtonSinopse()

      // interval para fechar tela de alert exibida no inicio
      /* setTimeout(async () => { await this.close() }, 20000) */

      this.links = this.loadAll()
      this.routeParams = this.$route.params

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
      // caso ocorra um refresh na tela ele ira retornar para tela agendamentos
      if (!this.routeParams.agendamento) {
        exibirAtendimento = true
        this.$router.push({
          name: 'Agendamentos'
        })
        return
      }
      /* this.setIsCollapse(true) */
      // Passado o id do profissional para api profissional para chamada posterior da memed junto com o token
      this.api.profissional.id = this.agendamento.profissionalId

      // Retorno dos dados do paciente
      await this.getPaciente()

      // Retorno do atendimento da triagem
      await this.getAtendimentoTriagem()

      // Passado o individuo Id para retornar os exames do mesmo na lista
      this.paramsExames.individuoId = this.api.individuo.id

      // Passado o individuo Id para retornar os exames F200 do mesmo na lista
      this.paramsExamesF200.idPaciente = this.api.individuo.id

      // Retorno da lista do cid e ciap
      await this.getCiap()
      await this.getCid10()

      // Retorno dos atendimentos no historico
      await this.getHistorico()

      // Verificação e retorno dos sinais vitais
      await this.getSinaisVitais()

      // Retorno das comorbidades do paciente
      await this.getComorbidades()

      // Retorno dos sintomas do paciente
      await this.getComoMeSinto()

      // Chamada do token do medico para abrir modal memed
      await this.tokenMemed()

      // Retorno dos exames no atendimento atual
      await this.getExames()

      // Retorno dos exames no historico do paciente
      /* await this.getExamesHistorico() */

      // Retorno dos tipos de exames
      await this.getTipoExames()

      // Auscutacao
      // await this.getAuscultacao()

      await this.getExamesF200()

      // Retorno ultimo atendimento do Individuo
      await this.getMaxAtendimentoByIndividuoId()
    },

    methods: {

      onChangeComorbidades (val) {
        this.formComorbidades.respondeuComorbidade = true
        // this.formComorbidades.dataAlteracao = new Date();
        this.onClickComorbidades(this.formComorbidades)
      },
      // CLICK PARA ATUALIZAR COMORBIDADES
      async onClickComorbidades (val) {
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

      // ---------------- OPEN MODAL ALERGIAS ---------------------------
      openModalAlergias () {
        this.dialogVisible = true
      },

      // Modal Alergias e Antecedentes
      async onClickAlergiasAntecedentes (val) {
        // let { data, status } = await _api.individuos.atualizarAlergiasAntecedentes(this.agendamento.individuoId, val)

        if (status === 200) {
          this.$message.success('Informações adicionais editadas com sucesso')
          /* this.salvarComorbidades = true */
        } else {
          this.$message.warning('Erro no envio das informações')
        }
      },
      // ---------------- FIM MODAL ALERGIAS ---------------------------
      // Dados Cadastrais do Paciente
      async onClickSalvarIndividuo (form) {
        this.erros = []
        this.$refs[form].validate(async (valid) => {
          if (valid) {
            this.loading.individuos = true

            this.formIndividuoCadastro.ativo = true
            this.formIndividuoCadastro.nomeCompleto = this.formIndividuoCadastro.nomeCompleto.toUpperCase()
            this.formIndividuoCadastro.nomeDaMae = this.formIndividuoCadastro.nomeDaMae.toUpperCase()
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
          } else {
            this.$message.warning('Verifique o preenchimento do formulário')
            this.loading.individuos = false
            return false
          }
        })
        this.loading.individuos = false
      },

      async getCep (cep) {
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

      async onSelectUf (val) {
        this.paramsCidades.ufAbreviado = val
        this.formIndividuoCadastro = {
          ...this.formIndividuoCadastro,
          cidadeId: null
        }
        await this.getCidadesByUf()
      },
      async getCidadesByUf () {
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
      async clickButtonSinopse () {
        this.telaSinopse = true
        this.telaAtendimento = false
        this.telaHistorico = false
        this.telaCadastroPaciente = false
      },
      async clickButtonAtendimento () {
        await this.getComoMeSintoAtendimento(this.agendamento)

        this.telaAtendimento = true
        this.telaSinopse = false
        this.telaHistorico = false
        this.telaCadastroPaciente = false
      },

      async clickButtonHistorico () {
        this.getHistorico()
        this.hideModal()
        this.telaHistorico = true
        this.telaSinopse = false
        this.telaAtendimento = false
        this.telaCadastroPaciente = false
      },
      async clickButtonCadastro () {
        this.getPaciente()
        this.telaCadastroPaciente = true
        this.telaSinopse = false
        this.telaAtendimento = false
        this.telaHistorico = false
      },

      // RETORNANDO QUEIXA DO PACIENTE
      async getAtendimentoTriagem () {
        let { data, status } = await _api.atendimentos.getbyAgendamentoId(this.agendamento.id)
        this.atendimentoTriagem = (status === 200) ? data : {}

        if (this.atendimentoTriagem.queixaDoPaciente === '' || this.atendimentoTriagem.queixaDoPaciente === undefined || this.atendimentoTriagem.queixaDoPaciente === null) {
          this.atendimentoTriagem.queixaDoPaciente = 'Não Informado Pela Triagem!!'
        }
      },
      // RETORNA O ULTIMO ATENDIMENTO DO INDIVIDUO
      async getMaxAtendimentoByIndividuoId () {
        let { data, status } = await _api.atendimentos.getMaxAtendimentoByIndividuoId(this.api.individuo.id)
        let infosAdicionais = (status === 200) ? data : {}

        this.formAlergiasEdit.alergias = infosAdicionais.alergias
        this.formAlergiasEdit.antecedentes = infosAdicionais.antecedentes
      },
  
      // OPEN MODAL RESULTADO EXAMES
      openModalResultadoExames () {
        this.getComorbidades()
        this.$modal.show('modalResultadoExames')
      },
      hideModalResultadoExames () {
        this.api.comorbidades = {}
        this.$modal.hide('modalResultadoExames')
      },

      // OPEN MODAL COMORBIDADES
      async openModalComorbidades () {
        await this.getComorbidades()
        this.formComorbidades = this.api.comorbidades
        this.$modal.show('modalComorbidades')
      },
      hideModalComorbidades () {
        this.api.comorbidades = {}
        this.$modal.hide('modalComorbidades')
      },

      // CLICK PARA ATUALIZAR COMORBIDADES
      async onClickComorbidades (val) {
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
      minimizar () {
        this.isMinimizado = true
      },
      maximizar () {
        this.isMinimizado = false
      },

      // HISTÓRICO E GRÁFICOS MEDIÇÕES
      async openModalMedicoes () {
        this.$modal.show('modalMedicoes')
        this.getAgendamentosTable()
        // await this.getAuscultacao()
      },

      // caso necessite de algum evento na troca de tela no header
      handleTabClickMedicoesHeader (tab, event) {
        if (tab.paneName === '2') {
          this.getAuscultacao()
        }
      },

      // caso necessite de algum evento na troca de tela entre os graficos
      handleTabClickMedicoesGrafico (tab, event) {
        if (tab.label === 'Teste') {

        }
      },

      // fechar modal medições
      hideModalMedicoes () {
        this.$modal.hide('modalMedicoes')
      },

      // retornar agendamentos para a table
      async getAgendamentosTable () {
        this.loading.agendamentos = true
        let { data, paginacao, status } = await _api.agendamentos.get(this.paramsAgendamentosTable)
        if (status === 502) this.loading.agendamentos = false
        this.api.agendamentos = (status === 200) ? data : []

        this.paramsAgendamentosTable.skip = (status === 200) ? paginacao.skip : 0
        this.paramsAgendamentosTable.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.agendamentos = false
      },

      // calcular imc da table e do grafico
      calcularIMC (peso, altura) {
        if ((peso != null || peso != '') && (altura != null || altura != '')) {
          var imcInicial = (peso / ((altura / 100) * (altura / 100)))
          var imcFinal = parseFloat(imcInicial).toFixed(1)
          return imcFinal
        } else {
          return 'Erro no cálculo do IMC'
        }
      },
      // paginação da table Auscultacao

      async handleSizeChangeAuscultacao (val) {
        this.paramsAuscultacao.take = val
        await this.getAuscultacao()
      },
      // paginação da table Auscultacao

      async handleCurrentChangeAuscultacao (val) {
        this.paramsAuscultacao.skip = val
        await this.getAuscultacao()
      },

      // paginação da table medições
      handleSizeChangeAgendamentos (val) {
        this.paramsAgendamentosTable.take = val
        this.getAgendamentosTable()
      },

      // paginação da table medições
      handleCurrentChangeAgendamentos (val) {
        this.paramsAgendamentosTable.skip = val
        this.getAgendamentosTable()
      },

      // FIM HISTÓRICO E GRÁFICOS MEDIÇÕES

      // RETORNO DA IDADE DO PACIENTE
      getIdade (val) {
        var data = moment(val).format('DD/MM/YYYY')
        var idade = moment().diff(val, 'years')
        return idade
      },

      handleClose (key, keyPath) {

      },

      // ALTERAÇÃO DE COR NA TABLE
      tableRow ({ row }) {
        if (row.ativo === true) {
          return 'success-row'
        } else if (row.ativo === false) {
          return 'warning-row'
        }
        return ''
      },

      // CONTADOR DE TEMPO
      onEmitContador (val) {
        this.tempoDoContador = val.time
      },

      // BLOQUEIA O USUARIO AO DAR O CLICK DE VOLTAR NO BOTÃO DO NAVEGADOR
      block () {
        window.location.hash = 'no-back-button'
        window.location.hash = 'Again-No-back-button'// again because google chrome don't insert first hash into history
        window.onhashchange = function () { window.location.hash = 'no-back-button' }
      },

      // BLOQUEIA E OCULTA OS ICONES DO MENU LATERAL
      setIsCollapse (payLoad) {
        this.$store.dispatch('setIsCollapse', payLoad)
      },

      // Fechar tela de alert (ATENDIMENTO)
      close () {
        this.alert = false
      },

      // RETORNO DAS COMORBIDADES DO PACIENTE ATUAL (ATENDIMENTO)
      async getComorbidades () {
        let { data } = await _api.individuos.getById(this.agendamento.individuoId)

        this.api.comorbidades = (data !== null) ? data : {}
      },

      // RETORNO DOS SINAIS VITAIS DO PACIENTE ATUAL (ATENDIMENTO)
      async getComoMeSinto () {
        if (this.agendamento.id != null) {
          this.paramsAcompanhamento.individuoId = this.agendamento.individuoId
          let { data } = await _api.acompanhamento.get(this.paramsAcompanhamento)
          let comoMeSinto = (data !== null) ? data : {}
          this.api.comoMeSinto = comoMeSinto.slice(-1)
          this.formComoMeSinto = this.api.comoMeSinto
        }
      },
      async getComoMeSintoAtendimento (row) {
        if (this.agendamento.id != null) {
          this.paramsAcompanhamento.individuoId = this.agendamento.individuoId
          let { data } = await _api.acompanhamento.get(this.paramsAcompanhamento)
          let comoMeSintoAtendimento = (data !== null) ? data : []
          if (comoMeSintoAtendimento.length > 0) {
            this.api.comoMeSintoAtendimento = comoMeSintoAtendimento[0]
          }
        }
      },
      async getComoMeSintoAtendimentoHistorico (row) {
        if (this.agendamento.id != null) {
          this.paramsAcompanhamento.individuoId = this.agendamento.individuoId
          this.paramsAcompanhamento.atendimentoId = row.id
          let { data } = await _api.acompanhamento.get(this.paramsAcompanhamento)
          let comoMeSintoAtendimento = (data !== null) ? data : []
          if (comoMeSintoAtendimento.length > 0) {
            this.api.comoMeSintoAtendimentoHistorico = comoMeSintoAtendimento
          }
        }
      },
      // RETORNANDO SINAIS VITAIS DA PULSEIRA (ATENDIMENTO)
      async getSinaisVitais () {
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

      getGrauImc (row) {
        if (row <= 16.9) {
          return 'Muito abaixo do Peso'
        }
        if (row >= 17 && row <= 18.4) {
          return 'Abaixo do Peso'
        }
        if (row >= 18.5 && row <= 24.9) {
          return 'Peso Normal'
        }
        if (row >= 25 && row <= 29.9) {
          return 'Acima do Peso'
        }
        if (row >= 30 && row <= 34.9) {
          return 'Obesidade Grau I'
        }
        if (row >= 35 && row <= 40) {
          return 'Obesidade Grau II'
        }
        if (row > 40) {
          return 'Obesidade Grau III'
        }
      },

      querySearch (queryString, cb) {
        var links = this.newObj
        var results = queryString ? links.filter(this.createFilter(queryString)) : links
        // call callback function to return suggestions

        cb(results)
      },

      createFilter (queryString) {
        return (link) => {
          return (link.value.toLowerCase().indexOf(queryString.toLowerCase()) === 0)
        }
      },

      async loadAll () {
        let { data, paginacao, status } = await _api.cid10.get(this.paramsCid10)
        this.api.cid10 = (status === 200) ? data : []

        if (this.api.cid10.length > 0) {
          this.api.cid10.map(item => {
            this.newObj.push({
              value: item.codigo,
              codigo: item.id
            })
          })

          return this.newObj
        }
      },

      // ----------------------------------- MODAL HISTORICO -----------------------------------
      // MODAL
      async onModalHistoricos (row) {
        this.loading.modalMenuHistorico = true
        /* await this.getAtendimento(row.id) */
        // console.log('row historico', row)
        await this.getSinaisVitaisHistorico(row)
        await this.getComoMeSintoHistorico(row)
        await this.getCid10Historico(row)
        await this.getCiapHistorico(row)
        await this.getComoMeSintoAtendimentoHistorico(row)
        this.formIndividuo = row.individuo,
        this.formProfissional = row.profissional,
        this.formProcedimento = row.procedimento,
        this.formHistorico = row
  
        if (this.api.cid10Historico.length > 0) {
          this.api.cid10Historico.map(item => this.formCid.push(item))
        }

        if (this.api.ciapHistorico.length > 0) {
          this.api.ciapHistorico.map(item => this.formCiap.push(item))
        }

        this.formComoMeSinto = this.getComoMeSinto(row),
        this.$modal.show('resumoHistoricoModal')
        this.loading.modalMenuHistorico = false
      },

      hideModal (row) {
        this.formModal = {},
        this.api.sinaisVitaisHistorico = []
        this.api.comoMeSintoHistorico = []
        this.formIndividuo = {},
        this.formCid = [],
        this.formCiap = [],
        this.formProfissional = {},
        this.formProcedimento = {},
        this.formComoMeSinto = [],
        this.$modal.hide('resumoHistoricoModal')
      },
      // ----------------------------------- FIM MODAL HISTORICO -----------------------------------

      // ----------------------------------- FIM HISTORICO -----------------------------------

      // RETORNANDO OS ATENDIMENTOS ANTERIORES DO PACIENTE SELECIONADO
      async getHistorico () {
        this.loading.historico = true

        // OBS SOBRE MYPARAMS (CASO PRECISE RETORNAR SOMENTE OS ATENDIMENTOS ANTERIORES DO PRONTUARIO DO PACIENTE, QUE O PROFISSIONAL ATUAL FINALIZOU ADICIONAR-> profissionalId: this.agendamento.profissionalId)
        var myParams = {
          ...this.paramsHistorico, individuoId: this.agendamento.individuoId, atendidoMedico: true
        }
        let { data, paginacao, status } = await _api.atendimentos.get(myParams)
        if (status === 502) this.loading.historico = false
        this.api.historico = (status === 200) ? data : []

        this.api.historico.map((item) => {
          item.dataCadastro = moment(item.dataCadastro).format('DD/MM/YYYY HH:mm')
        })

        this.paramsHistorico.skip = (status === 200) ? paginacao.skip : 0
        this.paramsHistorico.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.historico = false
      },

      // RETORNA O QUE FOI PREENCHIDO EM ATENDIMENTO
      async getAtendimento (id) {
        this.loading.historico = true

        // OBS SOBRE MYPARAMS (CASO PRECISE RETORNAR SOMENTE OS ATENDIMENTOS ANTERIORES DO PRONTUARIO DO PACIENTE, QUE O PROFISSIONAL ATUAL FINALIZOU ADICIONAR-> profissionalId: this.agendamento.profissionalId)
        var myParams = {
          ...this.params, individuoId: this.agendamento.individuoId, agendamentoId: id
        }
        let { data, paginacao, status } = await _api.atendimentos.get(myParams)
        if (status === 502) this.loading.historico = false
        this.api.atendimento = (status === 200) ? data : []

        this.api.atendimento.map((item) => {
          item.dataCadastro = moment(item.dataCadastro).format('DD/MM/YYYY hh:mm')
        })

        this.params.skip = (status === 200) ? paginacao.skip : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.historico = false
      },

      // RETORNA O ACOMPANHAMENTO BASEADO NO INDIVIDUO ID(SINTOMAS)
      async getAcompanhamento (id) {
        this.loading.historico = true

        // OBS SOBRE MYPARAMS (CASO PRECISE RETORNAR SOMENTE OS ATENDIMENTOS ANTERIORES DO PRONTUARIO DO PACIENTE, QUE O PROFISSIONAL ATUAL FINALIZOU ADICIONAR-> profissionalId: this.agendamento.profissionalId)
        this.params.sort = 'a.DataCadastro ASC'
        var myParams = {
          ...this.params, individuoId: this.agendamento.individuoId
        }
        // let { data, paginacao, status } = await _api.atendimentos.get(myParams)
        let { data, paginacao, status } = await _api.acompanhamento.get(myParams)

        if (status === 502) this.loading.historico = false
        this.api.acompanhamento = (status === 200) ? data : []

        // this.api.atendimento.map((item) => {
        //  item.dataCadastro = moment(item.dataCadastro).format('DD/MM/YYYY hh:mm')
        // })

        this.params.skip = (status === 200) ? paginacao.skip : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.historico = false
      },

      // CLICK DE VISUALIZAR O ATENDIMENTO ANTERIOR NO HISTORICO
      async onVisualizar (index, rowHistorico) {
        this.isDisabled = true
        this.listando = false
        this.getExamesHistorico()

        this.getCiapHistorico(rowHistorico)
        this.getCid10Historico(rowHistorico)

        this.getSinaisVitaisHistorico(rowHistorico)
        this.getComoMeSintoHistorico(rowHistorico)
        this.historicoSelecionado = rowHistorico
      },

      // RETORNANDO SINAIS VITAIS DA PULSEIRA (HISTORICO)
      async getSinaisVitaisHistorico (row) {
        this.api.sinaisVitaisHistorico.dataAlteracao = row.agendamento.dataAlteracao
        this.api.sinaisVitaisHistorico.pressaoArterial = row.agendamento.pressaoSanguinea
        this.api.sinaisVitaisHistorico.frequenciaCardiaca = row.agendamento.batimentoCardiaco
        this.api.sinaisVitaisHistorico.temperatura = row.agendamento.temperatura
        this.api.sinaisVitaisHistorico.saturacaoO2 = row.agendamento.oxigenacaoSanguinea
        this.api.sinaisVitaisHistorico.peso = row.agendamento.peso
        this.api.sinaisVitaisHistorico.altura = row.agendamento.altura

        var imcDados = (this.api.sinaisVitaisHistorico.peso / ((this.api.sinaisVitaisHistorico.altura / 100) * (this.api.sinaisVitaisHistorico.altura / 100)))
        var imcFloatDados = parseFloat(imcDados).toFixed(1)
        this.api.sinaisVitaisHistorico.dadosImc = imcFloatDados
      },

      // RETORNANDO SINAIS VITAIS DO PACIENTE NO HISTORICO
      async getComoMeSintoHistorico (row) {
        if (row != null) {
          this.paramsAcompanhamentoHistorico.individuoId = row.individuo.id
          let { data } = await _api.acompanhamento.get(this.paramsAcompanhamentoHistorico)
          var comoMeSintoHistorico = (data !== null) ? data : []
          var comoMeSinto = comoMeSintoHistorico.slice(-2)
          var comoMeSintoLength = []
          comoMeSintoLength.push(comoMeSinto[0])

          if (comoMeSintoLength.length > 0) this.api.comoMeSintoHistorico = comoMeSintoLength
        }
      },

      // PAGINAÇÃO HISTORICO
      handleSizeChangeHistorico (val) {
        this.paramsHistorico.take = val
        this.getHistorico()
      },
      handleCurrentChangeHistorico (val) {
        this.paramsHistorico.skip = val
        this.getHistorico()
      },

      // CLICK VOLTAR HISTORICO
      onVoltar (form) {
        this.listando = true
      },

      // ----------------------------------- FIM HISTORICO -----------------------------------

      // ---------------- MEMED ---------------------------
      async tokenMemed () {
        let { data } = await _api.memed.recoverTokenMedico()
        this.api.profissional.tokenMemed = data.attributes.token
        this.usePrescricao = true
      },
      async startMemed () {
        this.modalMemedOpen = true
      },
      async closeMemed () {
        this.modalMemedOpen = false
      },
      async getPaciente () {
        let { data, status } = await _api.individuos.getById(this.agendamento.individuoId)
        this.formIndividuoCadastro = (status === 200) ? data : {}
        this.formAlergiasEdit = (status === 200) ? data : {}
        // await this.onSelectUf(this.formIndividuoCadastro.ufAbreviado)
        // await this.getCidadesByUf()

        if (this.formIndividuoCadastro.ufAbreviado != 'NI') {
          await this.onSelectUf(this.formIndividuoCadastro.ufAbreviado)
          await this.getCidadesByUf()
        } else {
          this.formIndividuoCadastro.ufAbreviado = ''
          this.formIndividuoCadastro.cidadeId = ''
        }
        this.api.individuo = (status === 200) ? data : {}
      },

      // ---------------- FIM MEMED ---------------------------

      // --------------------- VIDEO CHAMADA TWILIO ---------------------
      stopTimer () {
        this.cronometrando = false
      },
      startTimer () {
        if (this.cronometrando == false) this.cronometrando = true
      },
      async twCreateRoom () {
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
      async twGetToken () {
        let { data, status } = await _api.teleConsulta.twGetToken(this.agendamento.id)
        return data.token
      },
      async twEntrarNoChat () {
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
          if (this.activeTab !== '1') {
            this.handleNotifyChatMessage()
          }
          this.onAddMessageToHistory(message.state)
        })
      },
      handleNotifyChatMessage () {
        let element = document.getElementById('tab-chat')
        element.classList.add('chat--notify')
        somNotificao.play()
      },
      async createConversation () {
        const uniqueName = `${this.agendamento.id}-chat`
        try {
          const newConversation = await this.conversationsClient.createConversation({ uniqueName: uniqueName })
          const joinedConversation = await newConversation.join().catch(err => console.log(err))
          await joinedConversation.add(this.$auth.user().id).catch(err => console.log(err))
          await joinedConversation.add(this.agendamento.individuoId).catch(err => console.log(err))
          this.twilio.activeConversation = joinedConversation
        } catch (e) {
          this.twilio.activeConversation = await this.conversationsClient.getConversationByUniqueName(uniqueName)
          let participants = await this.twilio.activeConversation.getParticipants()
          if (participants.length < 2) {
            await this.twilio.activeConversation.add(this.agendamento.individuoId).catch(err => console.log(err))
          }
        } finally {
          await this.getMessages()
        }
      },
      async getMessages () {
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
      onSendMessage () {
        if (!this.chatMessage) return
        this.twilio.activeConversation.sendMessage(this.chatMessage)
          .catch((erro) => {
            this.$message.warning('Não foi possível enviar a mensagem')
          })
        this.chatMessage = ''
      },
      async onAddMessageToHistory (msg) {
        await this.twilio.chatMessages.push({
          body: msg.body,
          createdDate: msg.timestamp,
          memberSid: msg.author
        })
      },
      async twEntrarNaSala () {
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
          localMediaContainer.appendChild(cameraTrack.attach())
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
      onMicrophoneEnable () {
        // console.log('ativar microfone')
        this.twilio.activeRoom.localParticipant.audioTracks.forEach(publication => {
          publication.track.enable()
          // console.log('microfone', publication)
        })
        this.twilio.micEnabled = true
      },
      onMicrophoneDisable () {
        this.twilio.activeRoom.localParticipant.audioTracks.forEach(publication => {
          publication.track.disable()
          // publication.track.stop()
        })
        this.twilio.micEnabled = false
      },
      onVideoEnable () {
        createLocalVideoTrack().then(track => {
          let localMediaContainer = document.getElementById('localTrack')
          localMediaContainer.appendChild(track.attach())
          return this.twilio.activeRoom.localParticipant.publishTrack(track)
        })
        this.twilio.camEnabled = true
      },
      onVideoDisable () {
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
      dispatchLog (message) {
        EventBus.$emit('new_log', message)
      },

      // Attach the Tracks to the DOM.
      attachTracks (tracks, container) {
        tracks.forEach(function (track) {
          container.appendChild(track.attach())
        })
      },

      // Attach the Participant's Tracks to the DOM.
      attachParticipantTracks (participant, container) {
        let tracks = Array.from(participant.tracks.values())
        this.attachTracks(tracks, container)
      },

      // Detach the Tracks from the DOM.
      detachTracks (tracks) {
        tracks.forEach((track) => {
          track.detach().forEach((detachedElement) => {
            detachedElement.remove()
          })
        })
      },

      // Detach the Participant's Tracks from the DOM.
      detachParticipantTracks (participant) {
        let tracks = Array.from(participant.tracks.values())
        this.detachTracks(tracks)
      },

      // Leave Room.
      leaveRoomIfJoined () {
        document.getElementById('localTrack').innerHTML = ''
        document.getElementById('remoteTrack').innerHTML = ''
        if (this.twilio.activeRoom) {
          this.twilio.activeRoom.disconnect()
        }
      },

      handleTabClickTeleAtendimento (tab, event) {
        this.activeTab = tab.index
        if (tab.label === 'Chat') {
          let element = document.getElementById('tab-1')
          element.classList.remove('chat--notify')
          setTimeout(() => {
            this.scrollChatToEnd()
          }, 500)
        }
      },

      handleTabClick (tab, event) {
        if (tab.label === 'Teste') {

        }
      },

      scrollChatToEnd () {
        if (this.twilio.chatMessages.length === 0) return
        jQuery('#pscroll').animate({ scrollTop: jQuery('ul#conversation li:last').offset().top + 60 }, 200)
      },

      // --------------------- FIM VIDEO CHAMADA TWILIO ---------------------

      // ----------------------- MODALS -------------------------
      // MODAL VIDEO CHAMADA

      async openModalVideo () {
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
          this.emAndamentoVideoChamada = true
          this.$modal.show('teleatendimento')
        } catch (e) {
          Notification({ title: 'Erro ao abrir a sala', message: 'Verifique sua conexão', type: 'error' })
        }
      },

      async hideDialogVideo () {
        this.stopTimer()

        this.tempoTotalDoAtendimento = this.tempoDoContador

        this.agendamento.emAndamento = false
        await _api.agendamentos.putAgendamento(this.agendamento)
        this.emAndamentoVideoChamada = false
        this.$modal.hide('teleatendimento')

        this.onVideoDisable()
        this.onMicrophoneDisable()
        this.leaveRoomIfJoined()
      },

      // MODAL MEMED
      openModalMemed () {
        this.$modal.show('modalMemed')
      },
      hideModalMemed () {
        this.$modal.hide('modalMemed')
      },

      hideModalHistorico () {
        this.api.comorbidades = {}
        this.$modal.hide('modalHistorico')
      },

      // ----------------------- FIM MODALS -------------------------

      // -------------------- CID E CIAP --------------------

      // ---------------------- CIAP ---------------

      // RETORNO DA LISTA DE CIAP DO BANCO
      async getCiap () {
        this.loading.ciap = true
        let { data, paginacao, status } = await _api.ciap.get(this.paramsCiap)
        this.api.ciap2 = (status === 200) ? data : []
        // console.log('data', data)
        if (this.api.ciap2.length === 1) {
          this.formPaciente.ciap2 = this.api.ciap2[0].id
        }
        // console.log('this.api.ciap2', this.api.ciap2)
        this.paramsCiap.skip = (status === 200) ? paginacao.skip : 0
        this.paramsCiap.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.ciap = false
      },

      // FUNÇÃO DO CLICK NO CIAP PARA ADICIONAR NA TABLE
      onAddButtonCiap () {
        var findCiap = this.findInArray(this.api.ciap2, this.formAtendimento.ciap2)
        var objCiap = {
          codigo: findCiap[0].codigo,
          descricao: findCiap[0].descricao,
          id: findCiap[0].id
        }

        var findCiap2 = objCiap

        var returnCiap2 = this.checkObjCiap2(this.ciap2Atendimento, findCiap2.id)

        if (returnCiap2 == false) {
          this.ciap2Atendimento.push({
            idCiap: findCiap2.id,
            descCiap: findCiap2.descricao,
            codCiap: findCiap2.codigo

          })
        } else {
          this.$message({ message: `CIAP REPETIDO`, type: 'warning' })
        }
        this.formAtendimento.ciap2Atendimento = this.ciap2Atendimento
      },

      checkObjCiap2 (arr, id) {
        let hasIdCiap2 = arr.some(item => item['idCiap'] === id)
        return hasIdCiap2
      },

      // FUNÇÃO QUE É CHAMADA DENTRO DA FUNÇÃO DO FINALIZAR ATENDIMENTO (submitform) / ADICIONA O ITEM DE CIAP NO FORM ATENDIMENTO
      onAddCiap () {
        let ciapFiltered = this.selectIdCiap(this.formAtendimento.ciap2Atendimento)
        this.ciap = ciapFiltered
        try {
          this.ciap.map(item => {
            this.formAtendimento.individuoCiap.push({
              agendamentoId: this.agendamento.id,
              individuoId: this.agendamento.individuoId,
              profissionalId: this.agendamento.profissionalId,
              ciapId: item
            })
          })
        } catch (e) {
          this.$message.warning('Ocorreu um erro ao tentar registrar o Ciap')
        }
      },

      // RETORNA O ID DO CIAP PARA A FUNÇÃO ONADDCIAP()
      selectIdCiap (arr) {
        let arrFiltered = []
        if (this.formAtendimento.ciap2Atendimento) {
          this.formAtendimento.ciap2Atendimento.map(function (item) {
            if (item.idCiap) {
              arrFiltered.push(item.idCiap)
            } else { }
          })
        }

        this.ciap = arrFiltered
        return this.ciap
      },

      async getCiapHistorico (row) {
        // console.log('row', row)
        this.loading.ciapCid = true
        this.params.agendamentoId = row.agendamentoId
        let { data, paginacao, status } = await _api.individuoCiap.get(this.params)
        if (status === 502) this.loading.ciapCid = false

        let dataFiltered = data.map(item => {
          item.ciap.atendidoMedico = item.atendidoMedico
          item.ciap.atendidoTriagem = item.atendidoTriagem
          return item.ciap
        })
        this.api.ciapHistorico = (status === 200) ? dataFiltered : []
        // console.log('this.api.ciapHistorico', this.api.ciapHistorico)

        this.params.skip = (status === 200) ? paginacao.skip : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.ciapCid = false
      },

      // ---------------------- FIM CIAP ---------------

      // ---------------------- CID ---------------

      // RETORNO DA LISTA DE CID DO BANCO
      async getCid10 () {
        this.loading.cid10 = true
        let { data, paginacao, status } = await _api.cid10.get(this.paramsCid10)
        this.api.cid10 = (status === 200) ? data : []

        this.paramsCid10.skip = (status === 200) ? paginacao.skip : 0
        this.paramsCid10.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.cid10 = false
      },

      // FUNÇÃO DO CLICK NO CID PARA ADICIONAR NA TABLE
      onAddButtonCid () {
        var findCid = this.findInArray(this.api.cid10, this.formAtendimento.cid10)
        var objCid = {
          codigo: findCid[0].codigo,
          descricao: findCid[0].descricao,
          id: findCid[0].id
        }
        var findCid10 = objCid
        var returnCid10 = this.checkObjCid10(this.cid10Atendimento, findCid10.id)

        if (returnCid10 == false) {
          this.cid10Atendimento.push({
            idCid10: findCid10.id,
            descCid10: findCid10.descricao,
            codCid10: findCid10.codigo

          })
        } else {
          this.$message({ message: `CID REPETIDO`, type: 'warning' })
        }
        this.formAtendimento.cid10Atendimento = this.cid10Atendimento
      },

      checkObjCid10 (arr, id) {
        let hasIdCid10 = arr.some(item => item['idCid10'] === id)
        return hasIdCid10
      },

      // FUNÇÃO QUE É CHAMADA DENTRO DA FUNÇÃO DO FINALIZAR ATENDIMENTO (submitform) / ADICIONA O ITEM DE CID NO FORM ATENDIMENTO
      onAddCid10 () {
        let cid10Filtered = this.selectIdCid10(this.formAtendimento.cid10Atendimento)
        this.cid10 = cid10Filtered
        try {
          this.cid10.map(item => {
            this.formAtendimento.individuoCid10.push({
              agendamentoId: this.agendamento.id,
              individuoId: this.agendamento.individuoId,
              profissionalId: this.agendamento.profissionalId,
              cid10Id: item
            })
          })
        } catch (e) {
          this.$message.warning('Ocorreu um erro ao tentar registrar o CID10')
        }
      },

      // RETORNA O ID DO CID PARA A FUNÇÃO ONADDCID10()
      selectIdCid10 (arr) {
        let arrFilteredCid10 = []
        if (this.formAtendimento.cid10Atendimento) {
          this.formAtendimento.cid10Atendimento.map(function (item) {
            if (item.idCid10) {
              return arrFilteredCid10.push(item.idCid10)
            } else { }
          })
        } else { }

        this.cid10 = arrFilteredCid10
        return this.cid10
      },

      async getCid10Historico (row) {
        this.loading.ciapCid = true
        this.params.agendamentoId = row.agendamentoId
        let { data, paginacao, status } = await _api.individuoCid10.get(this.params)
        if (status === 502) this.loading.ciapCid = false

        let dataFiltered = data.map(item => {
          return item.cid
        })
        this.api.cid10Historico = (status === 200) ? dataFiltered : []
        // console.log('this.api.cid10Historico', this.api.cid10Historico)

        this.params.skip = (status === 200) ? paginacao.skip : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.ciapCid = false
      },

      // ---------------------- FIM CID ---------------

      // UTILITARIO PARA ARRAY DE CID CIAP
      findInArray (arr, field) {
        return arr.filter(item => item.codigo === field)
      },

      limparCid10 () {
        this.cid10Atendimento = []
        this.formAtendimento.cid10Atendimento = []
      },
      limparCiap2 () {
        this.ciap2Atendimento = []
        this.formAtendimento.ciap2Atendimento = []
      },

      // -------------------- FIM CID E CIAP --------------------

      // ---------------------- EXAMES (ATENDIMENTO) ----------------------

      // RETORNANDO A LISTA DE EXAMES DO INDIVIDUO DO ATENDIMENTO ATUAL
      async getExames () {
        let { data, status, paginacao } = await _api.exames.getExames(this.paramsExames)
        var arrExame = (status === 200) ? data : []

        this.paramsExames.skip = (status === 200) ? paginacao.skip : 0
        this.paramsExames.total = (status === 200) ? paginacao.totalCount : 0

        arrExame.forEach(exames => {
          if (exames.avaliado) {
            exames.avaliado = moment(exames.avaliado).format('DD/MM/YYYY')
          }
        })

        this.api.exames = arrExame
      },

      // RETORNANDO A LISTA DE EXAMES DA ECO F200
      async getExamesF200 () {
        let { data, status, paginacao } = await _api.examesF200.getExamesF200(this.paramsExamesF200)
        var arrExameF200 = (status === 200) ? data : []

        this.paramsExamesF200.skip = (status === 200) ? paginacao.skip : 0
        this.paramsExamesF200.total = (status === 200) ? paginacao.totalCount : 0
        this.api.examesF200 = arrExameF200
      },

      // RETORNANDO A LISTA DE TIPO DE EXAMES
      getTipoExames (val) {
        let tipo = this.enums.tipoExames.filter(x => x.value == val)[0]

        if (tipo != null && tipo != undefined && tipo != '') {
          if (tipo.label != null && tipo.label != undefined && tipo.label != '') {
            return tipo.label
          }
        }
      },

      // ABRIR O CAMPO AVALIAR EXAME
      async menuAvaliar (val) {
        this.formAvaliar = val
        this.statusAvaliar = true
      },

      // EDITAR AVALIAÇÃO (OBS SOMENTE NO ATENDIMENTO QUE ELE AVALIOU O EXAME, CASO ATENDIMENTO SEJA FINALIZADO NÃO PODE SER MAIS EDITADO)
      async editarAvaliar (val) {
        this.formEditar = val
        this.statusEditar = true
      },

      // CLICK DE VISUALIZAR A AVALIAÇÃO DO MEDICO
      async visualizarAvaliacao (val) {
        // console.log(val)
        this.formVisualizar = val
        this.statusVisualizar = true
      },

      // CLICK PARA VISUALIZAR O PDF ECO F200
      async visualizarExameF200 (val) {
        var base64 = val.url
        this.debugBase64F200(base64, val)
      },

      // DESCRIPT BASE 64 E ABRINDO EM OUTRA TELA
      async debugBase64F200 (base64URL, obj) {
        // console.log('baseurl: #1', base64URL)
        // console.log('obj: #1', obj)
        if (obj.formato === '.pdf') {
          var win = window.open()
          win.document.write('<iframe src="' + base64URL + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe>')
        }
        if (obj.formato === '.jpeg') {
          var win = window.open()
          win.document.write('<img src="' + base64URL + '" style="width:100%; height:100%;" >')
        }
        if (obj.formato === '.jpg') {
          var win = window.open()
          win.document.write('<img src="' + base64URL + '" style="width:100%; height:100%;" >')
        }
        if (obj.formato === '.png') {
          var win = window.open()
          win.document.write('<img src="' + base64URL + '" style="width:100%; height:100%;" >')
        }
        if (obj.formato === '.wav') {
          var win = window.open()
          win.document.write(`
              <div style="display: flex; justify-content: center; align-items: center; width: 100%; height: 100%;">
                <audio controls>
                   <source src="${base64URL}" type="audio/wav">
                   <p><a href="${base64URL}"link</a></p>
                </audio>
              </div>
          `)
        }
        if (obj.formato === '.mp3') {
          var win = window.open()
          win.document.write(`
              <div style="display: flex; justify-content: center; align-items: center; width: 100%; height: 100%;">
                <audio controls>
                   <source src="${base64URL}" type="audio/wav">
                   <p><a href="${base64URL}"link</a></p>
                </audio>
              </div>
          `)
        }
      },

      // CLICK PARA VISUALIZAR O PDF
      async visualizar (val) {
        var base64 = val.url
        this.debugBase64(base64, val)
      },

      // DESCRIPT BASE 64 E ABRINDO EM OUTRA TELA
      async debugBase64 (base64URL, obj) {
        if (obj.formato === '.pdf') {
          var win = window.open()
          win.document.write('<iframe src="' + base64URL + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe>')
        }
        if (obj.formato === '.jpeg') {
          var win = window.open()
          win.document.write('<img src="' + base64URL + '" style="width:100%; height:100%;" >')
        }
        if (obj.formato === '.jpg') {
          var win = window.open()
          win.document.write('<img src="' + base64URL + '" style="width:100%; height:100%;" >')
        }
        if (obj.formato === '.png') {
          var win = window.open()
          win.document.write('<img src="' + base64URL + '" style="width:100%; height:100%;" >')
        }
        if (obj.formato === '.wav') {
          var win = window.open()
          win.document.write(`
              <div style="display: flex; justify-content: center; align-items: center; width: 100%; height: 100%;">
                <audio controls>
                   <source src="${base64URL}" type="audio/wav">
                   <p><a href="${base64URL}"link</a></p>
                </audio>
              </div>
          `)
        }
        if (obj.formato === '.mp3') {
          var win = window.open()
          win.document.write(`
              <div style="display: flex; justify-content: center; align-items: center; width: 100%; height: 100%;">
                <audio controls>
                   <source src="${base64URL}" type="audio/wav">
                   <p><a href="${base64URL}"link</a></p>
                </audio>
              </div>
          `)
        }
      },

      // CLICK DE SALVAR A AVALIAÇÃO
      async onSalvarAvaliacao () {
        this.formAvaliar.profissionalId = this.agendamento.profissionalId
        this.formAvaliar.resultado = true
        this.formAvaliar.avaliado = moment(this.formAvaliar.avaliado).format('DD/MM/YYYY')
        let { data, status } = await _api.exames.putAvaliacao(this.formAvaliar)
        if (status === 204) {
          this.formAvaliar.avaliado = moment(this.formAvaliar.avaliado).format('DD/MM/YYYY')
          this.statusAvaliar = false
          this.statusEnviadoAvaliacao = true
          this.$message.success('Avaliação Enviada Com Sucesso!')
        } else {
          this.$message.danger('Avaliação Não Enviada!')
        }
      },

      // CLICK DE SALVAR A AVALIAÇÃO EDITADA
      async onSalvarAvaliacaoEditada () {
        this.formEditar.resultado = true
        this.formEditar.avaliado = moment(this.formEditar.avaliado).format('DD/MM/YYYY')
        let { data, status } = await _api.exames.putAvaliacao(this.formEditar)
        if (status === 204) {
          this.formEditar.avaliado = moment(this.formEditar.avaliado).format('DD/MM/YYYY')
          this.statusEditar = false
          this.statusEnviadoAvaliacao = true
          this.$message.success('Avaliação Enviada Com Sucesso!')
        } else {
          this.$message.danger('Avaliação Não Enviada!')
        }
      },

      // FECHAR VISUALIZAR
      async hideVisualizar () {
        this.statusVisualizar = false
        this.statusEditar = false
      },

      // FECHAR EDITAR
      async hideEditar () {
        this.statusVisualizar = false
        this.statusEditar = false
      },

      // FECHAR AVALIAR
      async hideAvaliar () {
        this.statusEditar = false
        this.statusAvaliar = false
        this.formAvaliar = {}
      },

      // PAGINAÇÃO DO MENU EXAMES
      handleSizeChangeExames (val) {
        this.paramsExames.take = val
        this.getExames()
      },
      handleCurrentChangeExames (val) {
        // console.log('val skip', val)
        this.paramsExames.skip = val
        this.getExames()
      },

      // PAGINAÇÃO DO MENU EXAMES F200
      handleSizeChangeExamesF200 (val) {
        this.paramsExamesF200.take = val
        this.getExamesF200()
      },
      handleCurrentChangeExamesF200 (val) {
        // console.log('val skip', val)
        this.paramsExamesF200.skip = val
        this.getExamesF200()
      },

      // ---------------------- EXAMES (ATENDIMENTO) ----------------------

      // RETORNANDO A LISTA DE EXAMES DO INDIVIDUO FINALIZADOS DO HISTORICO
      async getExamesHistorico () {
        this.paramsExamesHistorico.finalizado = true
        let { data, status, paginacao } = await _api.exames.getExames(this.paramsExamesHistorico)
        var arrExame = (status === 200) ? data : []

        this.paramsExamesHistorico.skip = (status === 200) ? paginacao.skip : 0
        this.paramsExamesHistorico.total = (status === 200) ? paginacao.totalCount : 0

        let individuoId = this.api.individuo.id

        this.api.examesHistorico = arrExame.filter(exames => {
          if (exames.individuoId === individuoId) {
            return arrExame
          } else {
          }
        })

        this.api.examesHistorico = this.api.examesHistorico.filter(exames => {
          if (exames.avaliado) {
            exames.avaliado = moment(exames.avaliado).format('DD/MM/YYYY')
            return this.api.examesHistorico
          }
        })
      },

      // RETORNANDO A LISTA DE TIPO DE EXAMES
      async getTipoExamesHistorico (val) {
        let tipo = this.enums.tipoExames.filter(x => x.value == val)[0]
        return tipo.label
      },

      // CLICK DE VISUALIZAR A AVALIAÇÃO DO MEDICO
      visualizarAvaliacaoHistorico (val) {
        this.statusVisualizarHistorico = true
        this.formVisualizarHistorico = val
      },

      // CLICK PARA VISUALIZAR O PDF
      async visualizarHistorico (val) {
        var base64 = val.pdfExame
        this.debugBase64Historico(base64, val)
      },

      // DESCRIPT BASE 64 E ABRINDO EM OUTRA TELA
      async debugBase64Historico (base64URL, obj) {
        if (obj.formato === '.pdf') {
          var win = window.open()
          win.document.write('<iframe src="' + base64URL + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe>')
        }
        if (obj.formato === '.jpeg') {
          var win = window.open()
          win.document.write('<img src="' + base64URL + '" style="width:100%; height:100%;" >')
        }
        if (obj.formato === '.jpg') {
          var win = window.open()
          win.document.write('<img src="' + base64URL + '" style="width:100%; height:100%;" >')
        }
        if (obj.formato === '.png') {
          var win = window.open()
          win.document.write('<img src="' + base64URL + '" style="width:100%; height:100%;" >')
        }
        if (obj.formato === '.wav') {
          var win = window.open()
          win.document.write(`
              <div style="display: flex; justify-content: center; align-items: center; width: 100%; height: 100%;">
                <audio controls>
                   <source src="${base64URL}" type="audio/wav">
                   <p><a href="${base64URL}"link</a></p>
                </audio>
              </div>
          `)
        }
        if (obj.formato === '.mp3') {
          var win = window.open()
          win.document.write(`
              <div style="display: flex; justify-content: center; align-items: center; width: 100%; height: 100%;">
                <audio controls>
                   <source src="${base64URL}" type="audio/wav">
                   <p><a href="${base64URL}"link</a></p>
                </audio>
              </div>
          `)
        }
      },

      // FECHAR VISUALIZAR
      async hideVisualizarHistorico () {
        this.statusVisualizarHistorico = false
      },

      // ---------------- OPEN MODAL ALERGIAS ---------------------------
      /* async openModalAlergias () {
         await this.getPaciente()
  
        this.formAlergiasEdit.alergias = this.api.individuo.alergias
        this.formAlergiasEdit.antecedentes = this.api.individuo.antecedentes

        As Informaçoes Adicionais agora irao ser salvas no Atendimento:

        this.$modal.show('modalAlergias')
      }, */

      hideModalAlergias () {
        // this.formAlergias = {}
        this.$modal.hide('modalAlergias')
      },

      async onClickAlergias (val) {
        this.dialogVisible = false

        let { data, status } = await _api.individuos.atualizarAlergiasAntecedentes(this.agendamento.individuoId, val)
        if (status === 200) {
          await this.getPaciente()
        } else {
          this.$message.warning('Erro no envio das informações')
        }
  
        this.formAtendimento.alergias = (this.formAlergiasEdit.alergias != null) ? this.formAlergiasEdit.alergias : ''
        this.formAtendimento.antecedentes = (this.formAlergiasEdit.antecedentes != null) ? this.formAlergiasEdit.antecedentes : ''

        this.$modal.hide('modalAlergias')

        if (this.formAtendimento.alergias != null || this.formAtendimento.antecedentes != null) {
          this.$message.success('Dados inseridos no atendimento')
        } else {
          this.$message.warning('Erro no envio das informações')
        }
      },
      // ---------------- FIM MODAL ALERGIAS ---------------------------

      // FUNÇÃO DO BOTÃO DE FINALIZAR ATENDIMENTO
      async submitForm (form) {
        if (this.emAndamentoVideoChamada === true) {
          this.$message.warning('Encerre a video chamada antes de finalizar o atendimento')
        } else {
          var examesGeral = this.api.exames.filter(exames => {
            if (exames.resultado == true && exames.finalizado == false) {
              exames.finalizado = true
              exames.profissionalId = this.$auth.user().id
              return this.api.exames
            }
          })

          examesGeral.forEach(async function (exames) {
            let { data, status } = await _api.exames.putAvaliacao(exames)

            if (status !== 204) {
              this.$message.warning('Avaliação De Exames Não Enviado')
            }
          })

          // Verificação se tem um cid ou um ciap
          if (this.ciap2Atendimento.length === 0 && this.cid10Atendimento.length === 0) {
            this.$message.warning('É necessário o preenchimento de pelo menos um CID ou um CIAP')
          } else {
            this.$refs[form].validate(valid => {
              this.loading.atendimentos = true
              if (valid) {
                this.isValid = true
                // Inserção Cid10
                this.onAddCid10()
                // Inserção Ciap
                this.onAddCiap()

                let putAgendamento = {}
                putAgendamento.id = this.agendamento.id
                putAgendamento.finalizado = true
                putAgendamento.emAtendimentoMedico = false
                _api.agendamentos.putFinalizarMedico(putAgendamento).then(res => {
                  if (res.status === 204) {

                  } else {

                  }
                })
                this.formAtendimento.tempoAtendimento = this.tempoTotalDoAtendimento
                this.formAtendimento.atendidoMedico = true

                // Add as Informaç~eos Adicionais:
                this.formAtendimento.alergias = this.formAlergiasEdit.alergias
                this.formAtendimento.antecedentes = this.formAlergiasEdit.antecedentes

                let atendimento = { ...this.formAtendimento, agendamentoId: this.agendamento.id }

                _api.atendimentos.postMedico(atendimento).then(res => {
                  if (res.status === 201) {
                    // console.log('res', res.data)
                    // console.log('this.agendamento', this.agendamento)
                    if (this.agendamento != null) {
                      this.getComoMeSintoAtendimento(this.agendamento)
                      // console.log('this.api.comoMeSintoAtendimento', this.api.comoMeSintoAtendimento)
                      // console.log('res.data.id', res.data.id)
                      let atendimentoId = res.data.id
                      /* console.log('atendimentoId', atendimentoId) */
                      if (atendimentoId != null) {
                        _api.acompanhamento.atualizarAcompanhamento(atendimentoId, this.api.comoMeSintoAtendimento).then(res => {
                          this.$message.success('Acompanhamento atualizado')
                        }).catch(e => {
                          this.$message.error('Erro Acompanhamento')
                        })
                      }
                    }
  
                    // if (this.agendamento != null) {
                    //  console.log('this.api.comoMeSintoAtendimento', this.api.comoMeSintoAtendimento)
                    // }
  
                    // _api.acompanhamento.atualizarAcompanhamento(res.data.id).then(res => {
                    //  this.$message.warning('Acompanhamento atualizado')
                    // }).catch(e => {
                    //  console.log('ERRO#', e);
                    //  this.$message.warning('Erro ao atualizar acompanhamento')
                    // })
                    // _api.acompanhamento.atualizarAcompanhamento(res.data.id)
                    this.loading.atendimentos = false
                  }

                  this.loading.atendimentos = false
                  this.$router.push({ name: 'Agendamentos' })
                })
              } else {
                this.isValid = false
                this.$message.warning('Verifique o preenchimento dos campos obrigatórios')
              }
            })
          }
        }
      },

      onClickVoltar () {
        this.ausentes === true
          ? this.$router.push({ name: 'Ausentes' })
          : this.$router.push({ name: 'Agendamentos' })
      },
      async getAuscultacao () {
        this.loading.auscultacao = true
        this.paramsAuscultacao.IndividuoId = this.api.individuo.id
        this.paramsAuscultacao.IndividuoCpf = this.api.individuo.cpf
        this.paramsAuscultacao.Formato = '.pdf'

        let { data, status, paginacao } = await _api.exames.getAuscultacao(this.paramsAuscultacao)
        this.api.auscultacao = (status === 200) ? data : []
        this.paramsAuscultacao.skip = (status === 200) ? paginacao.skip : 0
        this.paramsAuscultacao.total = (status === 200) ? paginacao.totalcount : 0
        this.loading.auscultacao = false
      },
      onVisualizarAtendimento (val) {
        if (val.formato === '.pdf') {
          var win = window.open()
          win.document.write(`<iframe src="${val.url}" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe>`)
        }
  
        if (val.formato === '.wav') {
          var win = window.open()
          win.document.write(`
              <div style="display: flex; justify-content: center; align-items: center; width: 100%; height: 100%;">
                <audio controls>
                   <source src="${val.url}" type="audio/wav">
                   <p><a href="${val.url}"link</a></p>
                </audio>
              </div>
          `)
        }
      }

    }
  }
</script>

<style>

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
    width: 110px;
    height: 110px;
    background-color: #f2f2f2;
    border-radius: 100px;
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

  .scroll-area-historico {
    position: relative;
    width: 96%;
    height: 100%;
    overflow-x: hidden;
    margin: 15px;
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
    padding-bottom: 20px;
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
    /*right: 0px;
    left: 1360px;
    z-index: 1030;
    margin-bottom: 0;
    border-width: 1px 0 0;
    text-align: right;
    padding-right: 25px;
    font-size: 11px;
    display: block;
    box-sizing: border-box;*/
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
