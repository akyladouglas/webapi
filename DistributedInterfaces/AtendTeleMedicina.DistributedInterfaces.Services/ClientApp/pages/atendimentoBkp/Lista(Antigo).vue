<template>

  <el-row :gutter="24">
    <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
      <el-card shadow="always">

        <el-descriptions class="margin-top" :column="4" border>
          <template slot="title">



            <div v-if="api.individuo.corStatus === 1">
              <img v-if="api.individuo.imagem" alt="Imagem indisponível no momento" style="width: 110px; height: 110px; object-fit: cover; border-radius: 100%" :src="'../../' + api.individuo.imagem + '.jpg'" :title="api.individuo.nomeCompleto" />
              <div v-else class="paciente__missing_photo">
                <div width="500px">
                  <img :src="'../../assets/img/' + $store.state.app.empresa.fotoUsuario"
                       :title="$store.state.app.empresa.nome"
                       id="image" />
                </div>
              </div>

              <el-descriptions title="" direction="vertical" :column="4" border style="margin-top:20px">
                <el-descriptions-item label="Grau de Risco" label-class-name="verde">
                  <p>Caso com menor risco de problema respiratório</p>
                </el-descriptions-item>
              </el-descriptions>
            </div>



            <div v-if="api.individuo.corStatus === 2">
              <img v-if="api.individuo.imagem" alt="Imagem indisponível no momento" style="width: 110px; height: 110px; object-fit: cover; border-radius: 100%" :src="'../../' + api.individuo.imagem + '.jpg'" :title="api.individuo.nomeCompleto" />
              <div v-else class="paciente__missing_photo">
                <div width="500px">
                  <img :src="'../../assets/img/' + $store.state.app.empresa.fotoUsuario"
                       :title="$store.state.app.empresa.nome"
                       id="image" />
                </div>
              </div>
              <el-descriptions title="" direction="vertical" :column="4" border style="margin-top:20px">
                <el-descriptions-item label="Grau de Risco" label-class-name="amarelo">
                  <p style="font-size:12px">Caso suspeito de problema respiratório</p>
                </el-descriptions-item>
              </el-descriptions>

            </div>


            <div v-if="api.individuo.corStatus === 3">
              <img v-if="api.individuo.imagem" alt="Imagem indisponível no momento" style="width: 110px; height: 110px; object-fit: cover; border-radius: 100%" :src="'../../' + api.individuo.imagem + '.jpg'" :title="api.individuo.nomeCompleto" />
              <div v-else class="paciente__missing_photo">
                <div width="500px">
                  <img :src="'../../assets/img/' + $store.state.app.empresa.fotoUsuario"
                       :title="$store.state.app.empresa.nome"
                       id="image" />
                </div>
              </div>
              <el-descriptions title="" direction="vertical" :column="4" border style="margin-top:20px">
                <el-descriptions-item label="Grau de Risco" label-class-name="laranja">
                  <p style="font-size:12px">Caso Suspeito – Sintomático leve com risco mais elevado problema respiratório</p>
                </el-descriptions-item>
              </el-descriptions>
            </div>

            <div v-if="api.individuo.corStatus === 4">
              <img v-if="api.individuo.imagem" alt="Imagem indisponível no momento" style="width: 110px; height: 110px; object-fit: cover; border-radius: 100%" :src="'../../' + api.individuo.imagem + '.jpg'" :title="api.individuo.nomeCompleto" />
              <div v-else class="paciente__missing_photo">
                <div width="500px">
                  <img :src="'../../assets/img/' + $store.state.app.empresa.fotoUsuario"
                       :title="$store.state.app.empresa.nome"
                       id="image" />
                </div>
              </div>
              <el-descriptions title="" direction="vertical" :column="4" border style="margin-top: 20px">
                <el-descriptions-item label="Grau de Risco" label-class-name="vermelho" style="background-color: #e63946">
                  <p style="font-size:12px">Sintomatologia grave ou agravando para problema respiratório</p>
                </el-descriptions-item>
              </el-descriptions>
            </div>
          </template>

          <template slot="extra">
            <div>
              <p>
                Tempo de Consulta:
                <!--<el-tag id="timer" style="font-size: 25px; font-weight: bold;"></el-tag>-->
                <stopwatch :running="cronometrando" :resetWhenStart="false" :flag="false" />
              </p>
            </div>
            <div id="info-consulta"></div>
            <el-button @click="openModalVideo" icon="el-icon-video-camera" type="primary" size="small" id="btn-teleatendimento" v-if="!videoMinimizado && !isMinimizado">TeleAtendimento</el-button>
            <el-button @click="maximizar" icon="el-icon-video-camera" type="primary" size="small" id="btn-teleatendimento" v-if="isMinimizado">TeleAtendimento</el-button>
            <el-button @click="openModalMemed" type="primary" size="small" icon="el-icon-document-add">Prescrição</el-button>
            <el-button @click="openModalHistorico" type="primary" size="small" icon="el-icon-refresh-left">Histórico</el-button>
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
          <el-collapse-item v-if="agendamento.pressaoSanguinea != null " title="Sinais Vitais Pulseira" name="1">
            <div style="margin-bottom: 10px">
              <el-tag type="info">Data de envio: {{ moment(sinaisPulseira.dataAlteracao).format('DD/MM/YYYY HH:mm') }}</el-tag>
            </div>
            <el-descriptions :column="4" border>
              <el-descriptions-item label="Pre. Arterial">
                <el-tag size="small">{{ sinaisPulseira.pressaoSanguinea && sinaisPulseira.pressaoSanguinea + ' mmhg' }}</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Fre. Cardiaca">
                <el-tag size="small">{{ sinaisPulseira.batimentoCardiaco && sinaisPulseira.batimentoCardiaco + ' bpm' }}</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Saturação O2">
                <el-tag size="small">{{ sinaisPulseira.oxigenacaoSanguinea && sinaisPulseira.oxigenacaoSanguinea + ' %' }}</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Temperatura">
                <el-tag size="small">{{ sinaisPulseira.temperatura && sinaisPulseira.temperatura + ' °C' }}</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Altura">
                <el-tag size="small">{{ sinaisPulseira.altura && sinaisPulseira.altura + ' cm' }}</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Peso">
                <el-tag size="small">{{ sinaisPulseira.peso && sinaisPulseira.peso + ' kg' }}</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="IMC">
                <el-tag size="small">{{  sinaisPulseira.dadosImc }}</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="GRAU IMC">
                <el-tag size="small">{{  sinaisPulseira.statusImc }}</el-tag>
              </el-descriptions-item>
            </el-descriptions>
          </el-collapse-item>

          <el-collapse-item v-if="sinaisRecepcao.pressaoSanguineaRecepcao != null" title="Sinais Vitais Preenchido Pela Recepção" name="1">
            <div style="margin-bottom: 10px">
              <el-tag type="info">Data de envio: {{ moment(sinaisRecepcao.dataAlteracaoRecepcao).format('DD/MM/YYYY HH:mm') }}</el-tag>
            </div>
            <el-descriptions :column="4" border>
              <el-descriptions-item label="Pre. Arterial">
                <el-tag size="small">{{ sinaisRecepcao.pressaoSanguineaRecepcao && sinaisRecepcao.pressaoSanguineaRecepcao + ' mmhg' }}</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Fre. Cardiaca">
                <el-tag size="small">{{ sinaisRecepcao.batimentoCardiacoRecepcao && sinaisRecepcao.batimentoCardiacoRecepcao + ' bpm' }}</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Saturação O2">
                <el-tag size="small">{{ sinaisRecepcao.oxigenacaoSanguineaRecepcao && sinaisRecepcao.oxigenacaoSanguineaRecepcao + ' %' }}</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Temperatura">
                <el-tag size="small">{{ sinaisRecepcao.temperaturaRecepcao && sinaisRecepcao.temperaturaRecepcao + ' °C' }}</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Altura">
                <el-tag size="small">{{ sinaisRecepcao.alturaRecepcao && sinaisRecepcao.alturaRecepcao + ' cm' }}</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Peso">
                <el-tag size="small">{{ sinaisRecepcao.pesoRecepcao && sinaisRecepcao.pesoRecepcao + ' kg' }}</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="IMC">
                <el-tag size="small">{{  sinaisRecepcao.dadosImc }}</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="GRAU IMC">
                <el-tag size="small">{{  sinaisRecepcao.statusImc }}</el-tag>
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

          <el-collapse-item v-if="api.comoMeSinto.length > 0" title="Sintomas" name="3">
            <div style="margin-bottom: 10px">
              <span v-if="api.comoMeSinto[0].data" size="small">
                <el-tag type="info">Data de envio: {{ moment(api.comoMeSinto[0].data).format('DD/MM/YYYY HH:mm') }}</el-tag>
              </span>
            </div>
            <el-descriptions :column="4" border>
              <el-descriptions-item label="Temperatura">
                <el-tag v-if="api.comoMeSinto[0].temperatura" size="small" type="success">Sim</el-tag>
                <el-tag v-else size="small" type="danger">Não</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Tosse">
                <el-tag v-if="api.comoMeSinto[0].tosse" size="small" type="success">Sim</el-tag>
                <el-tag v-else size="small" type="danger">Não</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Coriza">
                <el-tag v-if="api.comoMeSinto[0].coriza" size="small" type="success">Sim</el-tag>
                <el-tag v-else size="small" type="danger">Não</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Dor no corpo">
                <el-tag v-if="api.comoMeSinto[0].dorCorpo" size="small" type="success">Sim</el-tag>
                <el-tag v-else size="small" type="danger">Não</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Dor abdominal">
                <el-tag v-if="api.comoMeSinto[0].dorAbdominal" size="small" type="success">Sim</el-tag>
                <el-tag v-else size="small" type="danger">Não</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Fraqueza">
                <el-tag v-if="api.comoMeSinto[0].fraqueza" size="small" type="success">Sim</el-tag>
                <el-tag v-else size="small" type="danger">Não</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Dor na garganta">
                <el-tag v-if="api.comoMeSinto[0].dorGarganta" size="small" type="success">Sim</el-tag>
                <el-tag v-else size="small" type="danger">Não</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Náusea/ Vômito">
                <el-tag v-if="api.comoMeSinto[0].nauseaVomito" size="small" type="success">Sim</el-tag>
                <el-tag v-else size="small" type="danger">Não</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Dor de cabeça">
                <el-tag v-if="api.comoMeSinto[0].dorCabeca" size="small" type="success">Sim</el-tag>
                <el-tag v-else size="small" type="danger">Não</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Tem saído de casa">
                <el-tag v-if="api.comoMeSinto[0].sairCasa" size="small" type="success">Sim</el-tag>
                <el-tag v-else size="small" type="danger">Não</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Contato com pessoas">
                <el-tag v-if="api.comoMeSinto[0].contatoPessoas" size="small" type="success">Sim</el-tag>
                <el-tag v-else size="small" type="danger">Não</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Dificuldade de respirar">
                <el-tag v-if="api.comoMeSinto[0].dificuldadeRespirar" size="small" type="success">Sim</el-tag>
                <el-tag v-else size="small" type="danger">Não</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Taquicardia">
                <el-tag v-if="api.comoMeSinto[0].taquicardia" size="small" type="success">Sim</el-tag>
                <el-tag v-else size="small" type="danger">Não</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Diarréia">
                <el-tag v-if="api.comoMeSinto[0].diarreia" size="small" type="success">Sim</el-tag>
                <el-tag v-else size="small" type="danger">Não</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Retornou A Ter Febre">
                <el-tag v-if="api.comoMeSinto[0].temperaturaRetornou" size="small" type="success">Sim</el-tag>
                <el-tag v-else size="small" type="danger">Não</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Atendido por serviço de saúde">
                <el-tag v-if="api.comoMeSinto[0].atendidoServicoSaude" size="small" type="success">Sim</el-tag>
                <el-tag v-else size="small" type="danger">Não</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Perda de olfato ou pladar">
                <el-tag v-if="api.comoMeSinto[0].perdaOlfatoPaladar" size="small" type="success">Sim</el-tag>
                <el-tag v-else size="small" type="danger">Não</el-tag>
              </el-descriptions-item>
            </el-descriptions>
          </el-collapse-item>
        </el-collapse>
      </el-card>
    </el-col>

    <el-form :model="formAtendimento" :rules="validacoes" labelPosition="top" ref="formAtendimento" label-width="170px">
      <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
        <el-card shadow="always" style="margin-top: 20px">
          <el-form-item label="Subjetivo" prop="subjetivo">
            <el-input :rules="validacoes" v-model="formAtendimento.subjetivo" :rows="4" type="textarea" placeholder="" />
          </el-form-item>
          <el-form-item label="Objetivo" prop="objetivo">
            <el-input :rules="validacoes" v-model="formAtendimento.objetivo" :rows="4" prop="objetivo" type="textarea" placeholder="" />
          </el-form-item>
          <el-form-item label="Avaliação" prop="avaliacao">
            <el-input :rules="validacoes" v-model="formAtendimento.avaliacao" :rows="4" type="textarea" placeholder="" />
          </el-form-item>
        </el-card>
      </el-col>

      <!-- Exames -->
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
          <!-- Menu Dos Exames -->
          <el-col>
            <!-- DIV DE AVALIAR -->
            <div style="margin-top: 20px" v-show="statusAvaliar">
              <el-form :model="formAvaliar" ref="tableAvaliar" label-width="170px" label-position="top">
                <div class="divButtonExame">
                  <el-button type="danger" size="small" @click="hideAvaliar" icon="el-icon-close"></el-button>
                </div>
                <!--<el-row :gutter="20">
                  <el-col :sm="24" :md="4" :lg="4" :xl="4">
                    <el-form-item label="Data Da Avaliação" prop="avaliado">
                      <el-input v-model="formAvaliar.avaliado" type="date">
                      </el-input>
                    </el-form-item>
                  </el-col>
                </el-row>-->
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

                <el-row v-show="formVisualizar.finalizado==true" :gutter="20">
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

                <!--<el-row :gutter="20">
                  <el-col :sm="24" :md="4" :lg="4" :xl="4">

                    <el-form-item label="Data Da Avaliação" prop="dataAvaliacao">
                      <el-input v-model="formEditar.avaliado" type="input" placeholder="Data Avaliacao" disabled>
                      </el-input>
                    </el-form-item>
                  </el-col>
                </el-row>-->
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


          <!--:disabled="!formAtendimento.ciap2 || !formAtendimento.cid10"-->
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

      <!-- PLANO -->
      <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
        <el-card shadow="always" style="margin-top: 20px">
          <el-form-item label="Plano de Tratamento" prop="plano">
            <el-input :rules="validacoes" v-model="formAtendimento.plano" :rows="4" type="textarea" placeholder="" />
          </el-form-item>
        </el-card>

        <!-- PLANO -->



        <div style="margin-top: 20px">
          <el-button type="primary" size="large" :disabled="!isValid" @click="submitForm('formAtendimento')" icon="el-icon-check">Finalizar Atendimento</el-button>
          <el-button type="danger" size="large" @click="onVoltarAgendamento" icon="el-icon-close">Cancelar Atendimento</el-button>
          <el-button @click="openModalHistorico" style="float: right" type="primary" size="small" icon="el-icon-refresh-left">Histórico</el-button>
          <el-button @click="openModalMemed" style="float: right" type="primary" size="small" icon="el-icon-document-add">Prescrição</el-button>
          <el-button @click="openModalVideo" style="float: right" icon="el-icon-video-camera" type="primary" size="small" id="btn-teleatendimento-bottom">Teleatendimento</el-button>
        </div>
      </el-col>
    </el-form>

    <modal name="modalMemed" :draggable="true" @opened="startMemed" @closed="closeMemed" :clickToClose="false" :width="1020" :height="690">
      <div class="window-header">
        <el-button type="danger" @click="hideModalMemed" icon="el-icon-close"></el-button>
      </div>
      <VuePerfectScrollbar id="pscroll" class="scroll-area-memed" :settings="scrollSettings" key="scroll-memed">
        <prescricao-memed v-if="usePrescricao && modalMemedOpen" :paciente="api.individuo" :idAgendamento="agendamento.id" :profissional="api.profissional" :procedimento="agendamento.procedimentoId" />
      </VuePerfectScrollbar>
    </modal>

    <modal :class="{ modalVideoMinizado: isMinimizado }" name="teleatendimento" :resizable="true" :draggable="true" :clickToClose="false" :min-width="280" :max-height="580" :height="580" :width="800" :adaptive="true">
      <el-tabs type="border-card" @tab-click="handleTabClick">
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
        <el-tab-pane label="Chat">
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
            <el-row v-else class="scroll-area--chat" >
             <el-empty description="Nenhum histórico de mensagens" />
            </el-row>
            <el-row :gutter="20">
              <el-col :span="21">
                <el-input v-model="chatMessage" />
              </el-col>
              <el-col :span="3">
                <el-button icon="el-icon-s-promotion" :disabled="!chatMessage" type="success" @click="onSendMessage" />
              </el-col>
            </el-row>         
          </div>
        </el-tab-pane>
      </el-tabs>
    </modal>




    <modal name="modalHistorico" :draggable="true" :clickToClose="false" :width="800" :height="600">
      <div class="window-header" style="">
        <el-button type="danger" @click="hideModalHistorico" icon="el-icon-close"></el-button>
        <el-button v-show="!listando" type="warning" @click="onVoltar" icon="el-icon-back"></el-button>
      </div>
      <VuePerfectScrollbar class="scroll-area2" :settings="scrollSettings" key="scrol-atendimento">
        <el-empty v-show="listando && api.historico.length === 0" description="Nenhum histórico encontrado"></el-empty>
        <el-row style="padding: 8px" v-show="listando && api.historico.length > 0">
          <el-col :span="24">
            <el-table ref="tabela" :data="api.historico"
                      highlight-current-row border
                      v-loading.body="loading.historico"
                      class="table--atendimento table--row-click">
              <el-table-column label="Data da Consulta" prop="dataCadastro" align="center" width="200" fixed />
              <el-table-column label="Individuo" prop="individuo.nomeCompleto" />
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
            <el-pagination @size-change="handleSizeChange"
                           @current-change="handleCurrentChange"
                           :current-page.sync="params.page"
                           :page-sizes="[10,25,50,100]"
                           :page-size="params.take"
                           :total="this.paginacaoExame.totalCount"
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
            <el-collapse-item v-if="sinaisPulseiraHistorico.pressaoArterial != null" title="Sinais Vitais Pulseira" name="1">
              <div style="margin-bottom: 10px">
                <el-tag type="info">Data de envio: {{ moment(sinaisPulseiraHistorico.dataAlteracao).format('DD/MM/YYYY HH:mm') }}</el-tag>
              </div>
              <el-descriptions :column="3" border>
                <el-descriptions-item label="Pre. Arterial">
                  <el-tag size="small">{{ sinaisPulseiraHistorico.pressaoArterial && sinaisPulseiraHistorico.pressaoArterial + ' mmhg' }}</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Fre. Cardiaca">
                  <el-tag size="small">{{ sinaisPulseiraHistorico.frequenciaCardiaca && sinaisPulseiraHistorico.frequenciaCardiaca + ' bpm' }}</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Saturação O2">
                  <el-tag size="small">{{ sinaisPulseiraHistorico.saturacaoO2 && sinaisPulseiraHistorico.saturacaoO2 + ' %' }}</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Temperatura">
                  <el-tag size="small">{{ sinaisPulseiraHistorico.temperatura && sinaisPulseiraHistorico.temperatura + ' °C' }}</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Altura">
                  <el-tag size="small">{{ sinaisPulseiraHistorico.altura && sinaisPulseiraHistorico.altura + ' cm' }}</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Peso">
                  <el-tag size="small">{{ sinaisPulseiraHistorico.peso && sinaisPulseiraHistorico.peso + ' kg' }}</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="IMC">
                  <el-tag size="small">{{  sinaisPulseiraHistorico.dadosImc }}</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="GRAU IMC">
                  <el-tag size="small">{{  sinaisPulseiraHistorico.statusImc }}</el-tag>
                </el-descriptions-item>
              </el-descriptions>
            </el-collapse-item>
            <!------------------------------SINAIS VITAIS HISTORICO RECEPÇÃO-------------------------------------------->
            <el-collapse-item v-if="sinaisRecepcaoHistorico.pressaoArterialRecepcao != null" title="Sinais Vitais Preenchido Pela Recepção" name="1">
              <div style="margin-bottom: 10px">
                <el-tag type="info">Data de envio: {{ moment(sinaisRecepcaoHistorico.dataAlteracaoRecepcao).format('DD/MM/YYYY HH:mm') }}</el-tag>
              </div>
              <el-descriptions :column="3" border>
                <el-descriptions-item label="Pre. Arterial">
                  <el-tag size="small">{{ sinaisRecepcaoHistorico.pressaoArterialRecepcao && sinaisRecepcaoHistorico.pressaoArterialRecepcao + ' mmhg' }}</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Fre. Cardiaca">
                  <el-tag size="small">{{ sinaisRecepcaoHistorico.frequenciaCardiacaRecepcao && sinaisRecepcaoHistorico.frequenciaCardiacaRecepcao + ' bpm' }}</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Saturação O2">
                  <el-tag size="small">{{ sinaisRecepcaoHistorico.saturacaoO2Recepcao && sinaisRecepcaoHistorico.saturacaoO2Recepcao + ' %' }}</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Temperatura">
                  <el-tag size="small">{{ sinaisRecepcaoHistorico.temperaturaRecepcao && sinaisRecepcaoHistorico.temperaturaRecepcao + ' °C' }}</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Altura">
                  <el-tag size="small">{{ sinaisRecepcaoHistorico.alturaRecepcao && sinaisRecepcaoHistorico.alturaRecepcao + ' cm' }}</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="Peso">
                  <el-tag size="small">{{ sinaisRecepcaoHistorico.pesoRecepcao && sinaisRecepcaoHistorico.pesoRecepcao + ' kg' }}</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="IMC">
                  <el-tag size="small">{{  sinaisRecepcaoHistorico.dadosImc }}</el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="GRAU IMC">
                  <el-tag size="small">{{  sinaisRecepcaoHistorico.statusImc }}</el-tag>
                </el-descriptions-item>
              </el-descriptions>
            </el-collapse-item>
            <!-------------------------------------------------------------------------->
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
  </el-row>




</template>

<style>
  .vermelho {
    background: #e63946 !important;
    color: #FFF !important
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
</style>



<script>
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
  import moment from 'moment'
  moment.locale('pt-br')

  export default {
    name: 'Atendimento',
    mixins: [Utils],
    components: { Integration, 'prescricao-memed': PrescricaoMemed, Log, Stopwatch, VuePerfectScrollbar },
    data() {
      return {
        socket: new WebSocket('ws://127.0.0.1:3099/NotifyAll'),
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

        isMinimizado: false,

        imgNotFound: false,
        historicoSelecionado: {},
        nomeMae: '',
        dadoImc: '',
        statusImc: '',
        agendamento: {},
        sinaisPulseiraHistorico: {},
        sinaisRecepcaoHistorico: {},
        comoMeSintoHistorico: {},
        sinaisPulseira: {},
        sinaisRecepcao: {},
        paginacaoExame: {},
        cid10Atendimento: [],
        ciap2Atendimento: [],

        videoMinimizado: false,
        statusAvaliar: false,
        statusEditar: false,
        statusVisualizar: false,
        statusVisualizarHistorico: false,
        statusEnviadoAvaliacao: false,
        usePrescricao: false,
        dialogVisible: false,
        formData: {},
        formAvaliar: {},
        formVisualizar: {},
        formVisualizarHistorico: {},
        formEditar: {},
        cronometrando: false,
        chatMessage: '',
        modalMemedOpen: false,
        scrollSettings: {
          maxScrollbarLength: 50
        },
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
        isValid: true,
        formAtendimento: {
          individuoCiap: [],
          individuoCid10: [],
          locomocao: null
        },
        cid10Ciap: [],
        filtroParams: {},
        erros: [],
        enums: {
          tipoExames: [
            ..._enums.tipoExames
          ],
          tipoDaConsulta: [
            ..._enums.tipoDaConsulta
          ]
        },
        cidCiap: [{}],
        ciap: [],
        cid10: [],
        arrExames: [],
        routeParams: {},

        validacoes: {
          subjetivo: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' },
            { min: 3, max: 1000, message: 'Nome não pode conter menos de 4 e mais que 1000 caracteres', trigger: 'submit' }
          ],
          objetivo: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' },
            { min: 3, max: 1000, message: 'Nome não pode conter menos de 4 e mais que 1000 caracteres', trigger: 'submit' }
          ],
          avaliacao: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' },
            { min: 3, max: 1000, message: 'Nome não pode conter menos de 4 e mais que 1000 caracteres', trigger: 'submit' }
          ],
          plano: [
            { required: true, message: 'Campo obrigatório', trigger: 'change' },
            { min: 3, max: 1000, message: 'Nome não pode conter menos de 4 e mais que 1000 caracteres', trigger: 'submit' }
          ],
          ciap2: [{ required: true, message: 'Campo obrigatório', trigger: ['blur', 'change'] }],
          cid10: [{ required: true, message: 'Campo obrigatório', trigger: ['blur', 'change'] }]
        },
        api: {
          sinaisVitaisPulseiraHistorico: {},
          sinaisVitaisRecepcaoHistorico: {},
          sinaisVitaisPulseira: {},
          sinaisVitaisRecepcao: {},
          cid10: [],
          ciap2: [],
          cid10Historico: [],
          ciapHistorico: [],
          estabelecimentos: [],
          motivoConsulta: [],
          ciapsCadastrados: [],
          comorbidades: {},
          comoMeSinto: {},
          comoMeSintoHistorico: {},
          individuo: {},
          profissional: {},
          historico: [],
          exames: [],
          examesHistorico: []
        },
        loading: {
          cid10: false,
          ciap: false,
          estabelecimentos: false,
          motivoConsulta: false,
          ciapsCadastrados: false,
          loadingCiaps: false,
          atendimentos: false,
          historico: false
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
          sort: '+Data ASC',
          total: 0
        },
        paramsAcompanhamentoHistorico: {
          skip: 1,
          take: 9999,
          sort: '+Data ASC',
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
          sort: 'IndividuoId ASC',
          total: 0,
        },
        paramsExamesHistorico: {
          skip: 1,
          take: 5,
          sort: 'IndividuoId ASC',
          total: 0,
        }
      }
    },
    async mounted() {
      // if (this.count == 1) {
      //  setInterval(this.updateTimer, 1000);
      // }

      this.links = this.loadAll()
      this.routeParams = this.$route.params
      this.agendamento = this.routeParams.agendamento
      //console.log(this.agendamento)
      if (!this.routeParams.agendamento) {
        this.$router.push({
          name: 'Agendamentos'
        })
        return
      }
      this.api.profissional.id = this.agendamento.profissionalId
      await this.getPaciente()
      this.paramsExames.individuoId = this.api.individuo.id

      await this.getCiap()
      await this.getCid10()

      await this.getSinaisVitaisRecepcao()
      await this.getSinaisVitaisPulseira()


      await this.getComorbidades()
      await this.getComoMeSinto()
      await this.tokenMemed()
      await this.getExamesHistorico()
      await this.getExames()
      await this.getTipoExames()
    },
    beforeUnmount () {
      ConversationsClient.shutdown()
      if (this.twilio.activeConversation) {
        this.twilio.activeConversation.leave()
      }
    },    
    async created() {
      window.addEventListener('beforeunload', this.leaveRoomIfJoined)
    },
    methods: {
      // PEGANDO SINAIS VITAIS DA RECEPCAO (ATENDIMENTO)
      async getSinaisVitaisRecepcao() {
        // retornando para api dados das aferições da recepcao
        // console.log('this.agendamento', this.agendamento)
        this.api.sinaisVitaisRecepcao.dataAlteracaoRecepcao = this.agendamento.dataAlteracaoRecepcao
        this.api.sinaisVitaisRecepcao.batimentoCardiacoRecepcao = this.agendamento.batimentoCardiacoRecepcao
        this.api.sinaisVitaisRecepcao.oxigenacaoSanguineaRecepcao = this.agendamento.oxigenacaoSanguineaRecepcao
        this.api.sinaisVitaisRecepcao.pressaoSanguineaRecepcao = this.agendamento.pressaoSanguineaRecepcao
        this.api.sinaisVitaisRecepcao.alturaRecepcao = this.agendamento.alturaRecepcao
        this.api.sinaisVitaisRecepcao.pesoRecepcao = this.agendamento.pesoRecepcao
        this.api.sinaisVitaisRecepcao.temperaturaRecepcao = this.agendamento.temperaturaRecepcao

        // IMC RECEPCAO
        var imcDados = (this.api.sinaisVitaisRecepcao.pesoRecepcao / ((this.api.sinaisVitaisRecepcao.alturaRecepcao / 100) * (this.api.sinaisVitaisRecepcao.alturaRecepcao / 100)))
        var imcFloatDados = parseFloat(imcDados).toFixed(1)
        this.api.sinaisVitaisRecepcao.dadosImc = imcFloatDados

        if (imcFloatDados >= 16 & imcFloatDados <= 16.9) {
          this.api.sinaisVitaisRecepcao.statusImc = 'Muito abaixo do Peso'
        }
        if (imcFloatDados >= 17 & imcFloatDados <= 18.4) {
          this.api.sinaisVitaisRecepcao.statusImc = 'Abaixo do Peso'
        }
        if (imcFloatDados >= 18.5 & imcFloatDados <= 24.9) {
          this.api.sinaisVitaisRecepcao.statusImc = 'Peso Normal'
        }
        if (imcFloatDados >= 25 & imcFloatDados <= 29.9) {
          this.api.sinaisVitaisRecepcao.statusImc = 'Acima do Peso'
        }
        if (imcFloatDados >= 30 & imcFloatDados <= 34.9) {
          this.api.sinaisVitaisRecepcao.statusImc = 'Obesidade Grau I'
        }
        if (imcFloatDados >= 35 & imcFloatDados <= 40) {
          this.api.sinaisVitaisRecepcao.statusImc = 'Obesidade Grau II'
        }
        if (imcFloatDados > 40) {
          this.api.sinaisVitaisRecepcao.statusImc = 'Obesidade Grau III'
        }
        // retirando da api e passando para um objeto no data e exibindo

        this.sinaisRecepcao = this.api.sinaisVitaisRecepcao
        // console.log('sinaisRecepcao', this.sinaisRecepcao)
      },

      // PEGANDO SINAIS VITAIS DA PULSEIRA (ATENDIMENTO)
      async getSinaisVitaisPulseira() {
        // retornando para api dados das aferições da Pulseira
        this.api.sinaisVitaisPulseira.dataAlteracao = this.agendamento.dataAlteracao
        this.api.sinaisVitaisPulseira.batimentoCardiaco = this.agendamento.batimentoCardiaco
        this.api.sinaisVitaisPulseira.oxigenacaoSanguinea = this.agendamento.oxigenacaoSanguinea
        this.api.sinaisVitaisPulseira.pressaoSanguinea = this.agendamento.pressaoSanguinea
        this.api.sinaisVitaisPulseira.altura = this.agendamento.altura
        this.api.sinaisVitaisPulseira.peso = this.agendamento.peso
        this.api.sinaisVitaisPulseira.temperatura = this.agendamento.temperatura

        var imcDados = (this.api.sinaisVitaisPulseira.peso / ((this.api.sinaisVitaisPulseira.altura / 100) * (this.api.sinaisVitaisPulseira.altura / 100)))
        var imcFloatDados = parseFloat(imcDados).toFixed(1)

        this.api.sinaisVitaisPulseira.dadosImc = imcFloatDados

        if (imcFloatDados >= 16 & imcFloatDados <= 16.9) {
          this.api.sinaisVitaisPulseira.statusImc = 'Muito abaixo do Peso'
        }
        if (imcFloatDados >= 17 & imcFloatDados <= 18.4) {
          this.api.sinaisVitaisPulseira.statusImc = 'Abaixo do Peso'
        }
        if (imcFloatDados >= 18.5 & imcFloatDados <= 24.9) {
          this.api.sinaisVitaisPulseira.statusImc = 'Peso Normal'
        }
        if (imcFloatDados >= 25 & imcFloatDados <= 29.9) {
          this.api.sinaisVitaisPulseira.statusImc = 'Acima do Peso'
        }
        if (imcFloatDados >= 30 & imcFloatDados <= 34.9) {
          this.api.sinaisVitaisPulseira.statusImc = 'Obesidade Grau I'
        }
        if (imcFloatDados >= 35 & imcFloatDados <= 40) {
          this.api.sinaisVitaisPulseira.statusImc = 'Obesidade Grau II'
        }
        if (imcFloatDados > 40) {
          this.api.sinaisVitaisPulseira.statusImc = 'Obesidade Grau III'
        }

        this.sinaisPulseira = this.api.sinaisVitaisPulseira
        // console.log('sinaisRecepcao', this.sinaisRecepcao)
      },

      querySearch(queryString, cb) {
        var links = this.newObj
        var results = queryString ? links.filter(this.createFilter(queryString)) : links
        // call callback function to return suggestions
        // console.log('RESULTS', results)
        cb(results)
      },

      createFilter(queryString) {
        return (link) => {
          return (link.value.toLowerCase().indexOf(queryString.toLowerCase()) === 0)
        }
      },

      async loadAll() {
        let { data, paginacao, status } = await _api.cid10.get(this.paramsCid10)
        this.api.cid10 = (status === 200) ? data : []
        // console.log(this.api.cid10)

        this.api.cid10.map(item => {
          this.newObj.push({
            value: item.codigo,
            codigo: item.id
          })
        })

        // console.log("NEW OBJ", this.newObj)

        return this.newObj
      },
      handleSelect(item) {
        // console.log(item)
      },

      async getCid10Historico(row) {
        this.loading.ciapCid = true
        this.params.agendamentoId = row.agendamentoId
        let { data, paginacao, status } = await _api.individuoCid10.get(this.params)
        if (status === 502) this.loading.ciapCid = false

        let dataFiltered = data.map(item => {
          return item.cid
        })
        this.api.cid10Historico = (status === 200) ? dataFiltered : []


        this.params.skip = (status === 200) ? paginacao.skip : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.ciapCid = false
      },

      // PEGANDO SINAIS VITAIS DA RECEPCAO (HISTORICO)
      async getSinaisVitaisRecepcaoHistorico(row) {
        // console.log('row -->', row)
        this.api.sinaisVitaisRecepcaoHistorico.dataAlteracaoRecepcao = row.agendamento.dataAlteracaoRecepcao
        this.api.sinaisVitaisRecepcaoHistorico.pressaoArterialRecepcao = row.agendamento.pressaoSanguineaRecepcao
        this.api.sinaisVitaisRecepcaoHistorico.frequenciaCardiacaRecepcao = row.agendamento.batimentoCardiacoRecepcao
        this.api.sinaisVitaisRecepcaoHistorico.temperaturaRecepcao = row.agendamento.temperaturaRecepcao
        this.api.sinaisVitaisRecepcaoHistorico.saturacaoO2Recepcao = row.agendamento.oxigenacaoSanguineaRecepcao
        this.api.sinaisVitaisRecepcaoHistorico.pesoRecepcao = row.agendamento.pesoRecepcao
        this.api.sinaisVitaisRecepcaoHistorico.alturaRecepcao = row.agendamento.alturaRecepcao
        //console.log('this.api.sinaisVitaisRecepcaoHistorico', this.api.sinaisVitaisRecepcaoHistorico)

        // IMC HISTORICO NÃO ALTERAR
        var imcDados = (this.api.sinaisVitaisRecepcaoHistorico.pesoRecepcao / ((this.api.sinaisVitaisRecepcaoHistorico.alturaRecepcao / 100) * (this.api.sinaisVitaisRecepcaoHistorico.alturaRecepcao / 100)))
        var imcFloatDados = parseFloat(imcDados).toFixed(1)
        // console.log('o imc', imcFloatDados)
        this.api.sinaisVitaisRecepcaoHistorico.dadosImc = imcFloatDados

        if (imcFloatDados >= 16 & imcFloatDados <= 16.9) {
          this.api.sinaisVitaisRecepcaoHistorico.statusImc = 'Muito abaixo do Peso'
        }
        if (imcFloatDados >= 17 & imcFloatDados <= 18.4) {
          this.api.sinaisVitaisRecepcaoHistorico.statusImc = 'Abaixo do Peso'
        }
        if (imcFloatDados >= 18.5 & imcFloatDados <= 24.9) {
          this.api.sinaisVitaisRecepcaoHistorico.statusImc = 'Peso Normal'
        }
        if (imcFloatDados >= 25 & imcFloatDados <= 29.9) {
          this.api.sinaisVitaisRecepcaoHistorico.statusImc = 'Acima do Peso'
        }
        if (imcFloatDados >= 30 & imcFloatDados <= 34.9) {
          this.api.sinaisVitaisRecepcaoHistorico.statusImc = 'Obesidade Grau I'
        }
        if (imcFloatDados >= 35 & imcFloatDados <= 40) {
          this.api.sinaisVitaisRecepcaoHistorico.statusImc = 'Obesidade Grau II'
        }
        if (imcFloatDados > 40) {
          this.api.sinaisVitaisRecepcaoHistorico.statusImc = 'Obesidade Grau III'
        }

        this.sinaisRecepcaoHistorico = this.api.sinaisVitaisRecepcaoHistorico

        // console.log('this.sinaisRecepcaoHistorico', this.sinaisRecepcaoHistorico)
      },

      // PEGANDO SINAIS VITAIS DA PULSEIRA (HISTORICO)
      async getSinaisVitaisPulseiraHistorico(row) {
        this.api.sinaisVitaisPulseiraHistorico.dataAlteracao = row.agendamento.dataAlteracao
        this.api.sinaisVitaisPulseiraHistorico.pressaoArterial = row.agendamento.pressaoSanguinea
        this.api.sinaisVitaisPulseiraHistorico.frequenciaCardiaca = row.agendamento.batimentoCardiaco
        this.api.sinaisVitaisPulseiraHistorico.temperatura = row.agendamento.temperatura
        this.api.sinaisVitaisPulseiraHistorico.saturacaoO2 = row.agendamento.oxigenacaoSanguinea
        this.api.sinaisVitaisPulseiraHistorico.peso = row.agendamento.peso
        this.api.sinaisVitaisPulseiraHistorico.altura = row.agendamento.altura
        // console.log('this.api.sinaisVitaisPulseiraHistorico', this.api.sinaisVitaisPulseiraHistorico)

        // IMC HISTORICO NÃO ALTERAR
        var imcDados = (this.api.sinaisVitaisPulseiraHistorico.peso / ((this.api.sinaisVitaisPulseiraHistorico.altura / 100) * (this.api.sinaisVitaisPulseiraHistorico.altura / 100)))
        var imcFloatDados = parseFloat(imcDados).toFixed(1)
        // console.log('o imc', imcFloatDados)
        this.api.sinaisVitaisPulseiraHistorico.dadosImc = imcFloatDados

        if (imcFloatDados >= 16 & imcFloatDados <= 16.9) {
          this.api.sinaisVitaisPulseiraHistorico.statusImc = 'Muito abaixo do Peso'
        }
        if (imcFloatDados >= 17 & imcFloatDados <= 18.4) {
          this.api.sinaisVitaisPulseiraHistorico.statusImc = 'Abaixo do Peso'
        }
        if (imcFloatDados >= 18.5 & imcFloatDados <= 24.9) {
          this.api.sinaisVitaisPulseiraHistorico.statusImc = 'Peso Normal'
        }
        if (imcFloatDados >= 25 & imcFloatDados <= 29.9) {
          this.api.sinaisVitaisPulseiraHistorico.statusImc = 'Acima do Peso'
        }
        if (imcFloatDados >= 30 & imcFloatDados <= 34.9) {
          this.api.sinaisVitaisPulseiraHistorico.statusImc = 'Obesidade Grau I'
        }
        if (imcFloatDados >= 35 & imcFloatDados <= 40) {
          this.api.sinaisVitaisPulseiraHistorico.statusImc = 'Obesidade Grau II'
        }
        if (imcFloatDados > 40) {
          this.api.sinaisVitaisPulseiraHistorico.statusImc = 'Obesidade Grau III'
        }

        this.sinaisPulseiraHistorico = this.api.sinaisVitaisPulseiraHistorico

        // console.log('this.sinaisPulseiraHistorico', this.sinaisPulseiraHistorico)
      },

      async getComoMeSintoHistorico(row) {
        if (row != null) {
          this.paramsAcompanhamentoHistorico.individuoId = row.individuo.id
          let { data } = await _api.acompanhamento.get(this.paramsAcompanhamentoHistorico)
          let comoMeSintoHistorico = (data !== null) ? data : {}
          this.api.comoMeSintoHistorico = comoMeSintoHistorico.slice(-2)

          // console.log('comoMeSintoHistorico', this.api.comoMeSintoHistorico)

          // this.paramsAcompanhamento.individuoId = this.agendamento.individuoId
          // let { data } = await _api.acompanhamento.get(this.paramsAcompanhamento)
          // let comoMeSinto = (data !== null) ? data : {}
          // this.api.comoMeSinto = comoMeSinto.slice(-1)
        }
      },

      async onVisualizar(index, rowHistorico) {
        this.isDisabled = true
        this.listando = false
        this.getExamesHistorico()

        this.getCiapHistorico(rowHistorico)
        this.getCid10Historico(rowHistorico)
        this.getSinaisVitaisRecepcaoHistorico(rowHistorico)

        this.getSinaisVitaisPulseiraHistorico(rowHistorico)
        this.getComoMeSintoHistorico(rowHistorico)
        this.historicoSelecionado = rowHistorico




        //this.formAtendimento = {
        //  ...row
        //}
      },
      async getHistorico() {
        this.loading.historico = true
        var myParams = {
          ...this.params, individuoId: this.agendamento.individuoId, profissionalId: this.agendamento.profissionalId
        }
        // console.log('myparams', myParams)
        let { data, paginacao, status } = await _api.atendimentos.get(myParams)
        if (status === 502) this.loading.historico = false
        this.api.historico = (status === 200) ? data : []
        this.api.historico.map((item) => {
          item.dataCadastro = moment(item.dataCadastro).format('DD/MM/YYYY hh:mm')
        })

        this.params.skip = (status === 200) ? paginacao.skip : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.historico = false
      },

      handleTabClick(tab, event) {
        if (tab.label === 'Chat') {
          setTimeout(() => {
            this.scrollChatToEnd()
          }, 500)
        }
      },

      async tokenMemed() {
        let { data } = await _api.memed.recoverTokenMedico()
        // console.log('data tokenMemed', data)
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
        //console.log('this.agendamento.individuoId', this.agendamento.individuoId)
        let { data, status } = await _api.individuos.getById(this.agendamento.individuoId)
        this.api.individuo = (status === 200) ? data : []
      },





      stopTimer() {
        this.cronometrando = false
      },
      startTimer() {
        this.cronometrando = true
      },
      async twCreateRoom() {
        let agendamento = {
          agendamentoId: this.agendamento.id
        }
        let { data } = await _api.teleConsulta.twCreateRoom(agendamento)
        if (data === 'Sala de atendimento ja criada.') {
          this.twilio.roomAlreadyCreated = true
          this.twEntrarNaSala()
          return
        }
        // console.log('data', data)
        this.twilio.roomName = data.roomVideo.unique_name
        this.twEntrarNaSala()
      },
      async twGetToken() {
        let { data } = await _api.teleConsulta.twGetToken(this.agendamento.id)
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
              // console.log('disconnecting')
              break
            case 'disconnected':
              // console.log('disconnected')
              break
            case 'denied':
              // console.log('denied')
              break
          }
        })
        const VueThis = this
        this.conversationsClient.on('messageAdded', function (message) {
          VueThis.onAddMessageToHistory(message.state)
        })
      },
      async createConversation() {
        const uniqueName = `${this.agendamento.id}-chat`
        try {
          const newConversation = await this.conversationsClient.createConversation({ uniqueName: uniqueName })
          newConversation.add(this.agendamento.individuoId);
          const joinedConversation = await newConversation.join().catch(err => console.log(err))
          this.twilio.activeConversation = joinedConversation
        } catch (e) {
          this.twilio.activeConversation = await this.conversationsClient.getConversationByUniqueName(uniqueName)
          this.twilio.activeConversation.add(this.agendamento.individuoId);

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
        }
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
          //publication.track.stop()
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
        //if (this.twilio.activeRoom) {
        //  this.twilio.activeRoom.disconnect()
        //}
      },
      minimizar() {
        this.isMinimizado = true
      },
      maximizar() {
        this.isMinimizado = false

      },
      async openModalVideo() {
        try {

          this.isMinimizado = false
          //console.log('this.videoMinimizado', this.videoMinimizado)
          await this.twCreateRoom()
          this.socket.send(`${this.agendamento.id};andamento-true`)

          setTimeout(async () => await _api.agendamentos.putAgendamento({ ...this.agendamento, emAndamento: true }), 10000);

          this.$modal.show('teleatendimento')
        } catch (e) {
          Notification({ title: 'Erro ao abrir a sala', message: 'Verifique sua conexão', type: 'error' })
        }
      },
      openModalMemed() {
        this.$modal.show('modalMemed')
      },
      hideModalMemed() {
        this.$modal.hide('modalMemed')
      },
      async hideDialogVideo() {
        this.videoMinimizado = false
        this.agendamento.emAndamento = false
        await _api.agendamentos.putAgendamento(this.agendamento)

        this.$modal.hide('teleatendimento')
        this.onVideoDisable()
        this.onMicrophoneDisable()
        this.leaveRoomIfJoined()
      },
      openModalHistorico() {
        this.$modal.show('modalHistorico')
        this.getHistorico()
      },
      hideModalHistorico() {
        this.$modal.hide('modalHistorico')
      },







      async getComorbidades() {
        let { data } = await _api.individuos.getById(this.agendamento.individuoId)

        this.api.comorbidades = (data !== null) ? data : {}
      },

      async getComoMeSinto() {
        if (this.agendamento.id != null) {
          this.paramsAcompanhamento.individuoId = this.agendamento.individuoId
          let { data } = await _api.acompanhamento.get(this.paramsAcompanhamento)
          let comoMeSinto = (data !== null) ? data : {}
          this.api.comoMeSinto = comoMeSinto.slice(-1)
        }
      },

      async submitForm(form) {

        if (this.twilio.activeRoom) {
          this.twilio.activeRoom.disconnect()
        }


        var examesGeral = this.api.exames.filter(exames => {
          if (exames.resultado == true && exames.finalizado == false) {
            exames.finalizado = true
            exames.profissionalId = this.$auth.user().id
            return this.api.exames
          }
        })
        //console.log('examesGeral', examesGeral)

        examesGeral.forEach(async function (exames) {
          let { data, status } = await _api.exames.putAvaliacao(exames)
          //console.log('data', data)
          //console.log('status', status)

          if (status !== 204) {
            this.$message.warning('Avaliação De Exames Não Enviado')
          }
        })


        if (this.cid10Ciap.length === 0) {
          this.$message.warning('Verifique o preenchimento dos campos CIAP 2 e CID10')
          return
        }
        // console.log('formAtendimento', this.formAtendimento)
        this.$refs[form].validate(valid => {
          // console.log('valid', valid)
          this.loading.atendimentos = true
          if (valid) {
            //// Inserção Cid10
            this.onAddCid10()
            //// Inserção Ciap
            this.onAddCiap()

            let putAgendamento = {}
            putAgendamento.id = this.agendamento.id
            putAgendamento.finalizado = true
            _api.agendamentos.putFinalizarMedico(putAgendamento).then(res => {
              //console.log(res)
              if (res.status === 204) {
                //console.log('ok agendamento')
              } else {
                //console.log('não ok agendamento')
              }
            })

            let atendimento = { ...this.formAtendimento, agendamentoId: this.agendamento.id }
            _api.atendimentos.postMedico(atendimento).then(res => {
              if (res.status === 201) {
                this.loading.atendimentos = false
              }
              this.loading.atendimentos = false
              this.$router.push({ name: 'Agendamentos' })
            })
          } else {
            this.$message.warning('Verifique o preenchimento dos campos obrigatórios')
          }
        })

      },





      onVoltarAgendamento() {
        this.$router.push({ name: 'Agendamentos' })
      },

      handleClose(done) {
        done()
      },

      resetForm(form) {
        this.$refs[form].resetFields()
      },
      scrollChatToEnd() {
        // console.log('acionou o scroll')
        jQuery('#pscroll').animate({ scrollTop: jQuery('ul#conversation li:last').offset().top + 30 }, 1000)
      },
      async onHistorico(row) {
        this.$router.push({
          name: 'Historico',
          params: { agendamento: row }
        })
      },
      handleSizeChangeExames(val) {
        this.paramsExames.take = val
        this.getExames()
      },
      handleCurrentChangeExames(val) {
        this.paramsExames.skip = val
        this.getExames()
      },

      handleSizeChange(val) {
        this.params.take = val
      },
      handleCurrentChange(val) {
        this.params.skip = val
        this.getHistorico()
      },
      onVoltar(form) {
        this.listando = true
      },




      //---------------------- EXAMES (ATENDIMENTO) ----------------------

      //RETORNANDO A LISTA DE EXAMES DO INDIVIDUO DO ATENDIMENTO ATUAL
      async getExames() {
        let { data, status, paginacao } = await _api.exames.getExames(this.paramsExames)
        var arrExame = (status === 200) ? data : []
        console.log("data arrExame", arrExame)
        this.paramsExames.skip = (status === 200) ? paginacao.skip : 0
        this.paramsExames.total = (status === 200) ? paginacao.totalCount : 0

        arrExame.forEach(exames => {
          if (exames.avaliado) {
            exames.avaliado = moment(exames.avaliado).format('DD/MM/YYYY')
          }
        })

        this.api.exames = arrExame


      },


      getTipoExames(val) {
        let tipo = this.enums.tipoExames.filter(x => x.value == val)[0]
        return tipo.label
      },


      async menuAvaliar(val) {
        this.formAvaliar = val
        this.statusAvaliar = true
        //this.formData.data = this.mxConverterData(new Date())

        //console.log('this.formAvaliar.avaliado', this.formAvaliar.avaliado)
        //console.log('this.formAvaliar', this.formAvaliar)
        //console.log('this.api.exames', this.api.exames)
      },
      async editarAvaliar(val) {
        this.formEditar = val
        this.statusEditar = true
      },

      //CLICK DE VISUALIZAR A AVALIAÇÃO DO MEDICO
      async visualizarAvaliacao(val) {
        //console.log(val)
        this.formVisualizar = val
        this.statusVisualizar = true
      },

      //CLICK PARA VISUALIZAR O PDF
      async visualizar(val) {
        var base64 = val.url
        this.debugBase64(base64, val)
      },

      //DESCRIPT BASE 64 E ABRINDO EM OUTRA TELA
      async debugBase64(base64URL, obj) {

        if (obj.formato === ".pdf") {
          var win = window.open();
          win.document.write('<iframe src="' + base64URL + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe>');
        }
        if (obj.formato === ".jpeg") {
          var win = window.open();
          win.document.write('<img src="' + base64URL + '" style="width:100%; height:100%;" >');
        }
        if (obj.formato === ".jpg") {
          var win = window.open();
          win.document.write('<img src="' + base64URL + '" style="width:100%; height:100%;" >');
        }
        if (obj.formato === ".png") {
          var win = window.open();
          win.document.write('<img src="' + base64URL + '" style="width:100%; height:100%;" >');
        }
        if (obj.formato === ".wav") {
          var win = window.open();
          win.document.write(`
              <div style="display: flex; justify-content: center; align-items: center; width: 100%; height: 100%;">
                <audio controls>
                   <source src="${base64URL}" type="audio/wav">
                   <p><a href="${base64URL}"link</a></p>
                </audio>
              </div>
          `);
        }
        if (obj.formato === ".mp3") {
          var win = window.open();
          win.document.write(`
              <div style="display: flex; justify-content: center; align-items: center; width: 100%; height: 100%;">
                <audio controls>
                   <source src="${base64URL}" type="audio/wav">
                   <p><a href="${base64URL}"link</a></p>
                </audio>
              </div>
          `);
        }

      },

      async onSalvarAvaliacao() {
        //console.log('val', val)
        this.formAvaliar.profissional = this.agendamento.profissional
        this.formAvaliar.resultado = true

        //console.log('this.formAvaliarr', this.formAvaliar)
        let { data, status } = await _api.exames.putAvaliacao(this.formAvaliar)
        if (status === 204) {
          //console.log('this.formAvaliar no onSalvarAvaliacao', this.formAvaliar)
          this.statusAvaliar = false
          this.statusEnviadoAvaliacao = true
          this.$message.success('Avaliação Enviada Com Sucesso!')
        } else {
          /*this.$message.danger('Avaliação Não Enviada!')*/
        }
      },

      async onSalvarAvaliacaoEditada() {
        //this.formAvaliar.profissional = this.agendamento.profissional
        this.formEditar.resultado = true
        let { data, status } = await _api.exames.putAvaliacao(this.formEditar)
        if (status === 204) {
          //console.log('this.formAvaliar no onSalvarAvaliacao', this.formEditar)
          this.statusEditar = false
          this.statusEnviadoAvaliacao = true
          this.$message.success('Avaliação Enviada Com Sucesso!')
        } else {
          this.$message.danger('Avaliação Não Enviada!')
        }
      },

      async hideVisualizar() {
        this.statusVisualizar = false
        this.statusEditar = false
      },

      async hideEditar() {
        this.statusVisualizar = false
        this.statusEditar = false

      },

      async hideAvaliar() {
        this.statusEditar = false
        this.statusAvaliar = false
        this.formAvaliar = {}
      },


      //----------- PARTE DOS EXAMES HISTORICO -----------//


      async getExamesHistorico() {

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

      async getTipoExamesHistorico(val) {

        let tipo = this.enums.tipoExames.filter(x => x.value == val)[0]

        return tipo.label
      },

      clickVisualizarHistorico(val) {

        this.statusVisualizarHistorico = true

        this.formVisualizarHistorico = val
      },

      async visualizarAvaliacaoHistorico(val) {
        var base64 = val.pdfExame
        this.debugBase64Historico(base64, val)
      },
      async debugBase64Historico(base64URL) {
        if (obj.formato === ".pdf") {
          var win = window.open();
          win.document.write('<iframe src="' + base64URL + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe>');
        }
        if (obj.formato === ".jpeg") {
          var win = window.open();
          win.document.write('<img src="' + base64URL + '" style="width:100%; height:100%;" >');
        }
        if (obj.formato === ".jpg") {
          var win = window.open();
          win.document.write('<img src="' + base64URL + '" style="width:100%; height:100%;" >');
        }
        if (obj.formato === ".png") {
          var win = window.open();
          win.document.write('<img src="' + base64URL + '" style="width:100%; height:100%;" >');
        }
        if (obj.formato === ".wav") {
          var win = window.open();
          win.document.write(`
              <div style="display: flex; justify-content: center; align-items: center; width: 100%; height: 100%;">
                <audio controls>
                   <source src="${base64URL}" type="audio/wav">
                   <p><a href="${base64URL}"link</a></p>
                </audio>
              </div>
          `);
        }
        if (obj.formato === ".mp3") {
          var win = window.open();
          win.document.write(`
              <div style="display: flex; justify-content: center; align-items: center; width: 100%; height: 100%;">
                <audio controls>
                   <source src="${base64URL}" type="audio/wav">
                   <p><a href="${base64URL}"link</a></p>
                </audio>
              </div>
          `);
        }
      },
      async hideVisualizarHistorico() {
        this.statusVisualizarHistorico = false
      },







      //-------------------- CID E CIAP --------------------



      //---------------------- CIAP ---------------

      // RETORNO DA LISTA DE CIAP DO BANCO
      async getCiap() {
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



      //FUNÇÃO DO CLICK NO CIAP PARA ADICIONAR NA TABLE
      onAddButtonCiap() {
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
            codCiap: findCiap2.codigo,

          })

        } else {
          this.$message({ message: `CIAP REPETIDO`, type: 'warning' })
        }
        this.formAtendimento.ciap2Atendimento = this.ciap2Atendimento
      },

      checkObjCiap2(arr, id) {
        let hasIdCiap2 = arr.some(item => item['idCiap'] === id)
        return hasIdCiap2
      },



      // FUNÇÃO QUE É CHAMADA DENTRO DA FUNÇÃO DO FINALIZAR ATENDIMENTO (submitform) / ADICIONA O ITEM DE CIAP NO FORM ATENDIMENTO
      onAddCiap() {
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
      selectIdCiap(arr) {
        let arrFiltered = []

        this.formAtendimento.ciap2Atendimento.map(function (item) {
          arrFiltered.push(item.idCiap)
        })

        this.ciap = arrFiltered
        return this.ciap
      },













      async getCiapHistorico(row) {
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
        //console.log('this.api.ciapHistorico', this.api.ciapHistorico)

        this.params.skip = (status === 200) ? paginacao.skip : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.ciapCid = false
      },



      //---------------------- FIM CIAP ---------------







      //---------------------- CID ---------------

      //RETORNO DA LISTA DE CID DO BANCO
      async getCid10() {
        this.loading.cid10 = true
        let { data, paginacao, status } = await _api.cid10.get(this.paramsCid10)
        this.api.cid10 = (status === 200) ? data : []

        this.paramsCid10.skip = (status === 200) ? paginacao.skip : 0
        this.paramsCid10.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.cid10 = false
      },


      //FUNÇÃO DO CLICK NO CID PARA ADICIONAR NA TABLE
      onAddButtonCid() {
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
            codCid10: findCid10.codigo,

          })

        } else {
          this.$message({ message: `CID REPETIDO`, type: 'warning' })
        }
        this.formAtendimento.cid10Atendimento = this.cid10Atendimento
      },

      checkObjCid10(arr, id) {
        let hasIdCid10 = arr.some(item => item['idCid10'] === id)
        return hasIdCid10

      },

      // FUNÇÃO QUE É CHAMADA DENTRO DA FUNÇÃO DO FINALIZAR ATENDIMENTO (submitform) / ADICIONA O ITEM DE CID NO FORM ATENDIMENTO
      onAddCid10() {
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
          console.log('this.formAtendimento.individuoCid10', this.formAtendimento.individuoCid10)
        } catch (e) {
          this.$message.warning('Ocorreu um erro ao tentar registrar o CID10')
        }
      },


      // RETORNA O ID DO CID PARA A FUNÇÃO ONADDCID10()
      selectIdCid10(arr) {
        let arrFilteredCid10 = []
        this.formAtendimento.cid10Atendimento.map(function (item) {
          return arrFilteredCid10.push(item.idCid10)
        })

        this.cid10 = arrFilteredCid10
        return this.cid10
      },










      async getCid10Historico(row) {
        this.loading.ciapCid = true
        this.params.agendamentoId = row.agendamentoId
        let { data, paginacao, status } = await _api.individuoCid10.get(this.params)
        if (status === 502) this.loading.ciapCid = false

        let dataFiltered = data.map(item => {
          return item.cid
        })
        this.api.cid10Historico = (status === 200) ? dataFiltered : []
        //console.log('this.api.cid10Historico', this.api.cid10Historico)

        this.params.skip = (status === 200) ? paginacao.skip : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.ciapCid = false
      },



      //---------------------- FIM CID ---------------




      //UTILITARIO PARA ARRAY DE CID CIAP
      findInArray(arr, field) {
        return arr.filter(item => item.codigo === field)
      },


      limparCid10() {
        this.cid10Atendimento = []
        this.formAtendimento.cid10Atendimento = []
      },
      limparCiap2() {
        this.ciap2Atendimento = []
        this.formAtendimento.ciap2Atendimento = []
      },

      //-------------------- FIM CID E CIAP --------------------
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

  .v--modal-overlay {
    background: transparent;
  }

    .v--modal-overlay[data-modal="hello-world"] {
      background: transparent;
    }

  .vm--overlay {
    display: flex;
    width: 14%;
    background-color: transparent;
    /*align-items: center;
    justify-content: center;*/
  }

  .vm--container {
    width: 300px;
    height: 600px;
    float: right;
  }
  /** Twilio classes */
  .local_video_container {
    width: 200px;
    height: 110px;
    position: absolute;
    bottom: 80px;
    right: 30px;
  }

  #localTrack video {
    width: 100%;
    height: 100%;
  }

  .remote_video_container {
    flex: 1;
    width: 100%;
    height: 500px;
    align-items: flex-end;
    justify-content: flex-end;
    background: #000;
  }

  #remoteTrack video {
    width: 100%;
    height: 500px;
  }


  .roomTitle {
    border: 1px solid rgb(124, 129, 124);
    padding: 4px;
    color: dodgerblue;
  }


  .scroll-area {
    position: relative;
    width: 100%;
    height: 340px;
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

  .modalVideoMinizado {
    display: none;
  }
</style>
