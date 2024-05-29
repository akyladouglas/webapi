<template>

  <el-col :gutter="24" class="teste">
    <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
      <el-card shadow="always">

        <el-descriptions class="margin-top" :column="4" border>
          <template slot="title">

            <div>
              <img v-if="api.individuo.imagem" alt="Imagem indisponível no momento" style="width: 110px; height: 110px; object-fit: cover; border-radius: 100%" :src="'../../' + api.individuo.imagem + '.jpg'" :title="api.individuo.nomeCompleto" />
              <div v-else class="paciente__missing_photo">
                <div width="500px">
                  <img :src="'../../assets/img/' + $store.state.app.empresa.fotoUsuario"
                       :title="$store.state.app.empresa.nome"
                       id="image" />
                </div>
              </div>
              <!--v-if="isDemandaEspontanea()"-->
              <div style="margin-top:15px">
                <el-select v-model="grauDeRisco" placeholder="Selecione o Grau de Risco" size="small">
                  <el-option v-for="item in classificacaoDeRisco"
                             :key="item.value"
                             :label="item.label"
                             :value="item.value" />
                </el-select>

                <div v-if="grauDeRisco === 1">

                  <el-descriptions title="" direction="vertical" :column="4" border style="margin-top:20px">
                    <el-descriptions-item label="Não Urgente" label-class-name="dodgerblue">
                      <!--<p style="font-size:12px">Caso com menor risco de problema respiratório</p>-->
                    </el-descriptions-item>
                  </el-descriptions>

                </div>

                <div v-if="grauDeRisco === 2">

                  <el-descriptions title="" direction="vertical" :column="4" border style="margin-top:20px">
                    <el-descriptions-item label="Pouco Urgente" label-class-name="verde">
                      <!--<p style="font-size:12px">Caso suspeito de problema respiratório</p>-->
                    </el-descriptions-item>
                  </el-descriptions>

                </div>

                <div v-if="grauDeRisco === 3">

                  <el-descriptions title="" direction="vertical" :column="4" border style="margin-top:20px">
                    <el-descriptions-item label="Urgente" label-class-name="amarelo">
                      <!--<p style="font-size:12px">Caso Suspeito – Sintomático leve com risco mais elevado problema respiratório</p>-->
                    </el-descriptions-item>
                  </el-descriptions>

                </div>

                <div v-if="grauDeRisco === 4">

                  <el-descriptions title="" direction="vertical" :column="4" border style="margin-top:20px">
                    <el-descriptions-item label="Muito Urgente" label-class-name="laranja">
                      <!--<p style="font-size:12px">Caso suspeito de problema respiratório</p>-->
                    </el-descriptions-item>
                  </el-descriptions>

                </div>

                <div v-if="grauDeRisco === 5">

                  <el-descriptions title="" direction="vertical" :column="4" border style="margin-top:20px">
                    <el-descriptions-item label="Emergência" label-class-name="vermelho">
                      <!-- <p style="font-size:12px">Caso suspeito de problema respiratório</p>-->
                    </el-descriptions-item>
                  </el-descriptions>

                </div>

              </div>
            </div>
          </template>


          <template slot="extra">
            <!--<div>
            <p>Tempo de Consulta:-->
            <!--<el-tag id="timer" style="font-size: 25px; font-weight: bold;"></el-tag>-->
            <!--<stopwatch :running="cronometrando" :resetWhenStart="false"  :flag="false"/>
              </p>
            </div>-->
            <div id="info-consulta"></div>
            <!--<el-button @click="openModalVideo" icon="el-icon-video-camera" type="primary" size="small" id="btn-teleatendimento">TeleAtendimento</el-button>
            <el-button @click="openModalMemed" type="primary" size="small" icon="el-icon-document-add">Prescrição</el-button>-->
            <!--<el-button @click="openModalHistorico" type="primary" size="small" icon="el-icon-refresh-left">Histórico</el-button>-->
          </template>
          <el-descriptions-item>
            <template slot="label">
              <strong>Nome</strong>
            </template>
            {{api.individuo.nomeCompleto}}
          </el-descriptions-item>
          <el-descriptions-item>
            <template slot="label">
              <strong>CPF</strong>
            </template>
            {{api.individuo.cpf ? api.individuo.cpf : 'Não cadastrado'}}
          </el-descriptions-item>
          <el-descriptions-item>
            <template slot="label">
              <strong>CNS</strong>
            </template>
            {{api.individuo.cns ? api.individuo.cns : 'Não cadastrado'}}
          </el-descriptions-item>
          <el-descriptions-item>
            <template slot="label">
              <strong>Sexo</strong>
            </template>
            {{api.individuo.sexo === 0 ? 'Masculino' : 'Feminino'}}
          </el-descriptions-item>
          <el-descriptions-item>
            <template slot="label">
              <strong>Idade</strong>
            </template>
            {{moment().diff(this.api.individuo.dataNascimento, 'years')}}
          </el-descriptions-item>
          <el-descriptions-item>
            <template slot="label">
              <strong>Celular</strong>
            </template>
            {{api.individuo.telefoneCelular}}
          </el-descriptions-item>
          <el-descriptions-item>
            <template slot="label">
              <strong>Email</strong>
            </template>
            {{api.individuo.email}}
          </el-descriptions-item>
          <el-descriptions-item>
            <template slot="label">
              <strong>Nome da Mãe</strong>
            </template>
            {{api.individuo.nomeDaMae ? api.individuo.nomeDaMae : 'Não informado'}}
          </el-descriptions-item>
          <el-descriptions-item>
            <template slot="label">
              <strong>Data Nascimento</strong>
            </template>
            {{moment(this.api.individuo.dataNascimento, 'YYYY-MM-DD').format('DD/MM/YYYY')}}
          </el-descriptions-item>
        </el-descriptions>

        <el-collapse>

          <el-collapse-item v-if="agendamento.pressaoSanguinea != null " title="Sinais Vitais" name="1">
            <div style="margin-bottom: 10px">
              <el-tag type="info">Data de envio: {{ moment(sinaisPulseira.dataAlteracao).format('DD/MM/YYYY HH:mm') }}</el-tag>
            </div>
            <el-descriptions :column="4" border>
              <el-descriptions-item label="Pressão Arterial (mmHg)">
                <el-tag size="small">{{ sinaisPulseira.pressaoSanguinea && sinaisPulseira.pressaoSanguinea + ' mmhg' }}</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Batimento Cardiaco (bpm)">
                <el-tag size="small">{{ sinaisPulseira.batimentoCardiaco && sinaisPulseira.batimentoCardiaco + ' bpm' }}</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Oxigenação (O²)">
                <el-tag size="small">{{ sinaisPulseira.oxigenacaoSanguinea && sinaisPulseira.oxigenacaoSanguinea + ' %' }}</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Temperatura (°C)">
                <el-tag size="small">{{ sinaisPulseira.temperatura && sinaisPulseira.temperatura + ' °C' }}</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Altura (cm)">
                <el-tag size="small">{{ sinaisPulseira.altura && sinaisPulseira.altura + ' cm' }}</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="Peso (Kg)">
                <el-tag size="small">{{ sinaisPulseira.peso && sinaisPulseira.peso + ' kg' }}</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="IMC">
                <el-tag size="small">{{  sinaisPulseira.dadosImc }}</el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="GRAU IMC">
                <el-tag size="small">{{  getGrauImc(sinaisPulseira.dadosImc) }}</el-tag>
              </el-descriptions-item>
            </el-descriptions>
          </el-collapse-item>


          <!--SINAIS VITAIS AFERIDOS PELA RECEPCAO, NÃO DELETAR!!!-->
          <el-collapse-item title="Sinais Vitais Aferidos" name="5">
            <div>
              <el-checkbox v-model="alterarSinaisVitais" @change="onAlterarSinaisVitais">Alterar Sinais Vitais</el-checkbox>
              <el-form :model="formUsuario" status-icon :rules="validacoes"
                       ref="formUsuario" label-width="120px" label-position="top" class="form--individuo">
                <el-row :gutter="24">
                  <el-col :xs="3" :sm="3" :md="3" :lg="8" :xl="8">
                    <el-form-item label="Pressão Arterial (mmHg)" prop="pressaoSanguinea">
                      <el-input maxlength="7" show-word-limit v-model="formUsuario.pressaoSanguinea" :disabled="!alterarSinaisVitais" />
                    </el-form-item>
                  </el-col>
                  <el-col :xs="3" :sm="3" :md="3" :lg="8" :xl="8">
                    <el-form-item label="Batimento Cardiaco (bpm)" prop="batimentoCardiaco">
                      <el-input maxlength="3" show-word-limit v-model="formUsuario.batimentoCardiaco" :disabled="!alterarSinaisVitais" />
                    </el-form-item>
                  </el-col>
                  <el-col :xs="3" :sm="3" :md="3" :lg="8" :xl="8">
                    <el-form-item label="Oxigenação (O²)" prop="oxigenacaoSanguinea">
                      <el-input maxlength="3" show-word-limit v-model="formUsuario.oxigenacaoSanguinea" :disabled="!alterarSinaisVitais" />
                    </el-form-item>
                  </el-col>
                </el-row>
                <el-row :gutter="24">
                  <el-col :xs="3" :sm="3" :md="3" :lg="8" :xl="8">
                    <el-form-item label="Temperatura (°C)" prop="temperatura">
                      <el-input maxlength="4" show-word-limit v-model="formUsuario.temperatura" :disabled="!alterarSinaisVitais" />
                    </el-form-item>
                  </el-col>
                  <el-col :xs="3" :sm="3" :md="3" :lg="8" :xl="8">
                    <el-form-item label="Altura (cm)" prop="altura">
                      <el-input maxlength="3" show-word-limit v-model="formUsuario.altura" :disabled="!alterarSinaisVitais" />
                    </el-form-item>
                  </el-col>

                  <el-col :xs="3" :sm="3" :md="3" :lg="8" :xl="8">
                    <el-form-item label="Peso (Kg)" prop="peso">
                      <el-input maxlength="3" show-word-limit v-model="formUsuario.peso" :disabled="!alterarSinaisVitais" />
                    </el-form-item>
                  </el-col>
                </el-row>
              </el-form>
              <el-button size="small" flat icon="fas fa-save" type="success" @click="onClickSalvarSinaisVitais('formUsuario')" :disabled="!alterarSinaisVitais || !formUsuario.batimentoCardiaco || !formUsuario.oxigenacaoSanguinea || !formUsuario.pressaoSanguinea || !formUsuario.altura || !formUsuario.peso || !formUsuario.temperatura"> Salvar Sinais Vitais</el-button>
            </div>
          </el-collapse-item>

          <div v-if="!api.individuo.respondeuComorbidade">
            <el-collapse-item title="Comorbidades" name="2">
              <div style="margin-bottom: 10px">
              </div>
              <el-descriptions :column="4" border>
                <el-descriptions-item style="align-items: center" label="Hipertenso">
                  <el-switch :disabled="salvarComorbidades" v-model="formComorbidades.hipertenso"
                             inactive-text="Não"
                             active-text="Sim"
                             inactive-value="false"
                             active-value="true"
                             style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949" />
                </el-descriptions-item>
                <el-descriptions-item label="Diabetes">
                  <el-switch :disabled="salvarComorbidades" v-model="formComorbidades.diabetes"
                             inactive-text="Não"
                             active-text="Sim"
                             inactive-value="false"
                             active-value="true"
                             style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949"
                             @change="" />
                </el-descriptions-item>
                <el-descriptions-item label="Fumante">
                  <el-switch :disabled="salvarComorbidades" v-model="formComorbidades.fumante"
                             inactive-text="Não"
                             active-text="Sim"
                             inactive-value="false"
                             active-value="true"
                             style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949"
                             @change="" />
                </el-descriptions-item>
                <el-descriptions-item label="Asma">
                  <el-switch :disabled="salvarComorbidades" v-model="formComorbidades.asma"
                             inactive-text="Não"
                             active-text="Sim"
                             inactive-value="false"
                             active-value="true"
                             style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949"
                             @change="" />
                </el-descriptions-item>
                <el-descriptions-item label="Doença no Pulmão">
                  <el-switch :disabled="salvarComorbidades" v-model="formComorbidades.doencaPulmao"
                             inactive-text="Não"
                             active-text="Sim"
                             inactive-value="false"
                             active-value="true"
                             style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949"
                             @change="" />
                </el-descriptions-item>
                <el-descriptions-item label="Doença no Coração">
                  <el-switch :disabled="salvarComorbidades" v-model="formComorbidades.doencaCoracao"
                             inactive-text="Não"
                             active-text="Sim"
                             inactive-value="false"
                             active-value="true"
                             style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949"
                             @change="" />
                </el-descriptions-item>
                <el-descriptions-item label="Câncer">
                  <el-switch :disabled="salvarComorbidades" v-model="formComorbidades.doencaCancer"
                             inactive-text="Não"
                             active-text="Sim"
                             inactive-value="false"
                             active-value="true"
                             style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949"
                             @change="" />
                </el-descriptions-item>
                <el-descriptions-item label="Transplantado">
                  <el-switch :disabled="salvarComorbidades" v-model="formComorbidades.transplantado"
                             inactive-text="Não"
                             active-text="Sim"
                             inactive-value="false"
                             active-value="true"
                             style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949"
                             @change="" />
                </el-descriptions-item>
                <el-descriptions-item label="Doença Compromete Imunidade">
                  <el-switch :disabled="salvarComorbidades" v-model="formComorbidades.doencaComprometeImunidade"
                             inactive-text="Não"
                             active-text="Sim"
                             inactive-value="false"
                             active-value="true"
                             style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949"
                             @change="" />
                </el-descriptions-item>
                <el-descriptions-item label="Doenca Nos Rins">
                  <el-switch :disabled="salvarComorbidades" v-model="formComorbidades.doencaRins"
                             inactive-text="Não"
                             active-text="Sim"
                             inactive-value="false"
                             active-value="true"
                             style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949"
                             @change="" />
                </el-descriptions-item>
                <el-descriptions-item label="Doenca No Figado">
                  <el-switch :disabled="salvarComorbidades" v-model="formComorbidades.doencaFigado"
                             inactive-text="Não"
                             active-text="Sim"
                             inactive-value="false"
                             active-value="true"
                             style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949"
                             @change="" />
                </el-descriptions-item>


              </el-descriptions>
              <!--<el-button size="small" flat icon="fas fa-save" type="success" style="margin-top: 10px" @click="onClickComorbidades(formComorbidades)" :disabled="salvarComorbidades"> Salvar Comorbidades</el-button>-->
            </el-collapse-item>

          </div>

          <el-collapse-item title="Sintomas" name="3">
            <div style="margin-bottom: 10px">
              <!--<span v-if="api.comoMeSinto[0].data" size="small">
                <el-tag type="info">Data de envio: {{ moment(api.comoMeSinto[0].data).format('DD/MM/YYYY HH:mm') }}</el-tag>
              </span>-->
            </div>
            <el-descriptions :column="4" border>
              v>
              <el-descriptions-item label="Temperatura Alta">
                <el-switch :disabled="salvarSintomas" v-model="formSintomas.temperatura"
                           inactive-text="Não"
                           active-text="Sim"
                           inactive-value="false"
                           active-value="true"
                           style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949"
                           @change="" />
              </el-descriptions-item>
              <el-descriptions-item label="Tosse">
                <el-switch :disabled="salvarSintomas" v-model="formSintomas.tosse"
                           inactive-text="Não"
                           active-text="Sim"
                           inactive-value="false"
                           active-value="true"
                           style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949"
                           @change="" />
              </el-descriptions-item>
              <el-descriptions-item label="Coriza">
                <el-switch :disabled="salvarSintomas" v-model="formSintomas.coriza"
                           inactive-text="Não"
                           active-text="Sim"
                           inactive-value="false"
                           active-value="true"
                           style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949"
                           @change="" />
              </el-descriptions-item>
              <el-descriptions-item label="Dor No Corpo">
                <el-switch :disabled="salvarSintomas" v-model="formSintomas.dorCorpo"
                           inactive-text="Não"
                           active-text="Sim"
                           inactive-value="false"
                           active-value="true"
                           style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949"
                           @change="" />
              </el-descriptions-item>
              <el-descriptions-item label="Dor Abdominal">
                <el-switch :disabled="salvarSintomas" v-model="formSintomas.dorAbdominal"
                           inactive-text="Não"
                           active-text="Sim"
                           inactive-value="false"
                           active-value="true"
                           style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949"
                           @change="" />
              </el-descriptions-item>
              <el-descriptions-item label="Fraqueza">
                <el-switch :disabled="salvarSintomas" v-model="formSintomas.fraqueza"
                           inactive-text="Não"
                           active-text="Sim"
                           inactive-value="false"
                           active-value="true"
                           style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949"
                           @change="" />
              </el-descriptions-item>
              <el-descriptions-item label="Dor Garganta">
                <el-switch :disabled="salvarSintomas" v-model="formSintomas.dorGarganta"
                           inactive-text="Não"
                           active-text="Sim"
                           inactive-value="false"
                           active-value="true"
                           style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949"
                           @change="" />
              </el-descriptions-item>
              <el-descriptions-item label="Náusea/ Vômito">
                <el-switch :disabled="salvarSintomas" v-model="formSintomas.nauseaVomito"
                           inactive-text="Não"
                           active-text="Sim"
                           inactive-value="false"
                           active-value="true"
                           style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949"
                           @change="" />
              </el-descriptions-item>
              <el-descriptions-item label="Dor de Cabeça">
                <el-switch :disabled="salvarSintomas" v-model="formSintomas.dorCabeca"
                           inactive-text="Não"
                           active-text="Sim"
                           inactive-value="false"
                           active-value="true"
                           style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949"
                           @change="" />
              </el-descriptions-item>
              <el-descriptions-item label="Tem saído de casa">
                <el-switch :disabled="salvarSintomas" v-model="formSintomas.sairCasa"
                           inactive-text="Não"
                           active-text="Sim"
                           inactive-value="false"
                           active-value="true"
                           style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949"
                           @change="" />
              </el-descriptions-item>
              <el-descriptions-item label="Contato com pessoas">
                <el-switch :disabled="salvarSintomas" v-model="formSintomas.contatoPessoas"
                           inactive-text="Não"
                           active-text="Sim"
                           inactive-value="false"
                           active-value="true"
                           style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949"
                           @change="" />
              </el-descriptions-item>
              <el-descriptions-item label="Dificuldade de respirar">
                <el-switch :disabled="salvarSintomas" v-model="formSintomas.dificuldadeRespirar"
                           inactive-text="Não"
                           active-text="Sim"
                           inactive-value="false"
                           active-value="true"
                           style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949"
                           @change="" />
              </el-descriptions-item>
              <el-descriptions-item label="Taquicardia">
                <el-switch :disabled="salvarSintomas" v-model="formSintomas.taquicardia"
                           inactive-text="Não"
                           active-text="Sim"
                           inactive-value="false"
                           active-value="true"
                           style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949"
                           @change="" />
              </el-descriptions-item>
              <el-descriptions-item label="Diarréia">
                <el-switch :disabled="salvarSintomas" v-model="formSintomas.diarreia"
                           inactive-text="Não"
                           active-text="Sim"
                           inactive-value="false"
                           active-value="true"
                           style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949"
                           @change="" />
              </el-descriptions-item>
              <el-descriptions-item label="Retornou A Ter Febre">
                <el-switch :disabled="salvarSintomas" v-model="formSintomas.temperaturaRetornou"
                           inactive-text="Não"
                           active-text="Sim"
                           inactive-value="false"
                           active-value="true"
                           style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949"
                           @change="" />
              </el-descriptions-item>
              <el-descriptions-item label="Atendido por serviço de saúde">
                <el-switch :disabled="salvarSintomas" v-model="formSintomas.atendidoServicoSaude"
                           inactive-text="Não"
                           active-text="Sim"
                           inactive-value="false"
                           active-value="true"
                           style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949"
                           @change="" />
              </el-descriptions-item>
              <el-descriptions-item label="Perda de olfato ou pladar">
                <el-switch :disabled="salvarSintomas" v-model="formSintomas.perdaOlfatoPaladar"
                           inactive-text="Não"
                           active-text="Sim"
                           inactive-value="false"
                           active-value="true"
                           style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949"
                           @change="" />
              </el-descriptions-item>
              <el-descriptions-item label="Sintomas Gripais">
                <el-switch :disabled="salvarSintomas" v-model="formSintomas.sintomasGripais"
                           inactive-text="Não"
                           active-text="Sim"
                           inactive-value="false"
                           active-value="true"
                           style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949"
                           @change="" />
              </el-descriptions-item>
            </el-descriptions>
            <!--<el-button size="small" flat icon="fas fa-save" type="success" style="margin-top: 10px" @click="onClickSintomas(formSintomas)" :disabled="salvarSintomas"> Salvar Sintomas</el-button>-->
          </el-collapse-item>


          <el-collapse-item title="Anexar exames" name="4">

            <el-upload style="display: flex; flex-direction:column"
                       action="" ref="upload"
                       :file-list="fileList"
                       :before-upload="isPdf"
                       :on-success="encodeImageFileAsURL"
                       :on-remove="onDeleteDocs"
                       accept="application/pdf"
                       multiple>
              <div style="display:flex; align-items:flex-start;flex-direction:column">

                <el-select v-model="value" placeholder="Selecione o tipo de exame" @change="handleSelectChange(value)" style="width: 30%">
                  <el-option v-for="item in this.api.exames"
                             :key="item.id"
                             :label="item.nome"
                             :value="item.id">
                  </el-option>
                </el-select>
                <el-button v-if="showBtn" type="primary" style="margin-top: 5px">Clique para selecionar o exame</el-button>
              </div>

            </el-upload>
            <el-button v-if="showEnviar" v-loading.body="loading.exames" @click="uploadDocs" type="primary" :disabled="salvarExames">Enviar Exames</el-button>

          </el-collapse-item>

        </el-collapse>
      </el-card>
    </el-col>

    <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">

      <el-form :model="formAtendimento" labelPosition="top" ref="formAtendimento" label-width="170px">
        <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
          <el-card shadow="always" style="margin-top: 20px">
            <el-form-item label="Queixa Do Paciente" prop="queixaDoPaciente">
              <el-input v-model="formAtendimento.queixaDoPaciente" type="textarea" autosize :rows="1" placeholder="" size="mini" />
            </el-form-item>
          </el-card>
        </el-col>
        <el-col>
          <div style="margin-top: 20px">
            <el-button type="primary" size="large" :loading="loading.atendimentos" @click="submitForm('formAtendimento')" icon="el-icon-check">Finalizar Atendimento</el-button>
          </div>
        </el-col>
      </el-form>
    </el-col>

  </el-col>
</template>

<style>
  #loading-spinner {
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

  .loading-container {
    text-align: center;
  }

  .texto-modal {
    font-size: 20px;
    font-weight: 300;
    text-indent: 50px;
    text-align: justify;
  }

  .modal-content {
    width: 70%;
    margin: 0 auto;
    border: none;
    word-break: normal;
  }

  .centered-button {
    display: block;
    margin: 20px auto;
  }

  .separator {
    height: 1px;
    width: 100%;
    background-color: #ddd;
    margin: 10px 0;
  }

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

  .dodgerblue {
    background: dodgerblue !important;
    color: #FFF !important
  }
</style>



<script>
  import jQuery from 'jquery'
  import { EventBus } from '../../Event'
  import Log from './Log'
  import Utils from '../../mixins/Utils'
  import _api from '../../api'
  import _enums from '../../api/Enums'
  import Stopwatch from '../../components/UIComponents/StopWatch.vue'
  import moment from 'moment'
  import { mask } from 'vue-the-mask'
  moment.locale('pt-br')

  export default {
    name: 'Atendimento',
    mixins: [Utils],
    components: { Log, Stopwatch },
    directives: { mask },
    data () {
      var validaPressaoArterial = (rule, value, callback) => {
        var pattern = /\d{2,3}\/\d{2,3}/
        if (value === undefined) {
          return callback(new Error('Informe a pressão arterial (ex: 120/90)'))
        } else if (value.match(pattern)) {
          return callback()
        } else {
          return callback(new Error('Pressão arterial inválida (ex: 120/90).'))
        }
      }
      var validaBatimentoCardiaco = (rule, value, callback) => {
        var pattern = /\b\d{2,3}\b/
        if (value > 300) return callback(new Error('Batimento Cardiaco não pode ser maior que 300'))
        if (value === undefined) {
          return callback(new Error('Informe o batimento cardiaco (ex: 100, 90)'))
        } else if (value.match(pattern)) {
          return callback()
        } else {
          return callback(new Error('Batimento cardiaco inválida (ex: 100, 90).'))
        }
      }
      var validaOxigenacaoSanguinea = (rule, value, callback) => {
        var pattern = /\b\d{2,3}\b/
        if (value > 100) return callback(new Error('Oxigenação não pode ser maior que 100'))
        if (value === undefined) {
          return callback(new Error('Informe a oxigenação sanguinea (ex: 93, 99)'))
        } else if (value.match(pattern)) {
          return callback()
        } else {
          return callback(new Error('Oxigenação sanguinea inválida (ex: 93, 99).'))
        }
      }
      var validaTemperatura = (rule, value, callback) => {
        var pattern = /\b\d{2}\b/
        if (value === undefined) {
          return callback(new Error('Informe a temperatura (ex: 35,9, 36,7)'))
        } else if (value.match(pattern)) {
          return callback()
        } else {
          return callback(new Error('Temperatura inválida (ex: 35,9, 36,7).'))
        }
      }
      var validaAltura = (rule, value, callback) => {
        var pattern = /\b\d{2,3}\b/
        if (value === undefined) {
          return callback(new Error('Informe a altura (ex: 168, 179)'))
        } else if (value.match(pattern)) {
          return callback()
        } else {
          return callback(new Error('Altura inválida (ex: 168, 179).'))
        }
      }
      var validaPeso = (rule, value, callback) => {
        var pattern = /\b\d{2,3}\b/
        if (value === undefined) {
          return callback(new Error('Informe o peso (ex: 78, 98)'))
        } else if (value.match(pattern)) {
          return callback()
        } else {
          return callback(new Error('Peso inválido (ex: 78, 98).'))
        }
      }

      return {
        statusPutRecepcao: null,
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
        showModal: false,
        imgNotFound: false,
        nomeMae: '',
        dadoImc: '',
        statusImc: '',
        agendamento: {},
        sinaisPulseira: {},
        /*      sinaisRecepcao: {}, */
        paginacaoExame: {},
        formUsuario: {},
        formRow: {},
        formComorbidades: {},
        formSintomas: {},
        classificacaoDeRisco: [],
        grauDeRisco: null,
        exames: [],
        tipoExame: 0,

        showBtn: false,
        showEnviar: false,
        statusAvaliar: false,
        salvarExames: false,
        salvarSintomas: false,
        salvarComorbidades: false,
        alterarSinaisVitais: false,
        enviouSinaisVitais: false,
        statusEditar: false,
        statusVisualizar: false,
        statusEnviadoAvaliacao: false,
        usePrescricao: false,
        dialogVisible: false,
        formData: {},
        formAvaliar: {},
        formVisualizar: {},
        formEditar: {},
        cronometrando: false,
        chatMessage: '',
        modalMemedOpen: false,
        scrollSettings: {
          maxScrollbarLength: 50
        },
        fileList: [],
        value: '',
        isDisabled: false,
        listando: true,
        isValid: true,
        formAtendimento: {
          individuoCiap: [],
          individuoCid10: [],
          locomocao: null
        },

        filtroParams: {},
        erros: [],
        enums: {
          classificacaoDeRisco: [
            ..._enums.classificacaoDeRisco
          ],
          tipoExames: [
            ..._enums.tipoExames
          ],
          tipoDaConsulta: [
            ..._enums.tipoDaConsulta
          ]
        },

        arrExames: [],
        routeParams: {},

        validacoes: {
          pressaoSanguinea: [
            { required: false, validator: validaPressaoArterial, trigger: ['blur', 'submit'] }
          ],
          batimentoCardiaco: [
            { required: false, validator: validaBatimentoCardiaco, trigger: ['blur', 'submit'] }
          ],
          oxigenacaoSanguinea: [
            { required: false, validator: validaOxigenacaoSanguinea, trigger: ['blur', 'submit'] }
          ],
          temperatura: [
            { required: false, validator: validaTemperatura, trigger: ['blur', 'submit'] }
          ],
          altura: [
            { required: false, validator: validaAltura, trigger: ['blur', 'submit'] }
          ],
          peso: [
            { required: false, validator: validaPeso, trigger: ['blur', 'submit'] }
          ]
        },
        api: {

          sinaisVitaisPulseira: {},
          /*          sinaisVitaisRecepcao: {}, */

          estabelecimentos: [],
          motivoConsulta: [],
          comorbidades: {},
          comoMeSinto: {},
          individuo: {},
          profissional: {},
          exames: [],
          examesF200: [],
          classificacaoDeRisco: []
        },
        loading: {
          exames: false,
          estabelecimentos: false,
          motivoConsulta: false,
          atendimentos: false,
          f200: false
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
          total: 0
        }
      }
    },
    async mounted () {
      // if (this.count == 1) {
      //  setInterval(this.updateTimer, 1000);
      // }

      this.links = this.loadAll()
      this.routeParams = this.$route.params
      this.agendamento = this.routeParams.agendamento
      // console.log("this.routeParams.agendamento", this.routeParams.agendamento)
      if (this.routeParams.agendamento == undefined) {
        this.$router.push({
          name: 'Agenda'
        })
        return
      }

      // VERIFICAÇÃO DOS FINAIS VITAIS CASO SEJA PREENCHIDO PELA PULSEIRA, LIBERAR ACESSO PARA FINALIZAR TRIAGEM
      if (this.agendamento.batimentoCardiaco != undefined && this.agendamento.altura != undefined && this.agendamento.oxigenacaoSanguinea != undefined && this.agendamento.peso != undefined && this.agendamento.pressaoSanguinea != undefined && this.agendamento.temperatura != undefined) {
        this.enviouSinaisVitais = true
      }

      this.formRow = this.routeParams.agendamento

      this.api.profissional.id = this.agendamento.profissionalId
      await this.getPaciente()
      await this.getClassificacaoDeRisco()
      this.paramsExames.individuoId = this.api.individuo.id
      await this.getSinaisVitaisPulseira()
      await this.getExames()

      await this.getComorbidades()
      await this.getComoMeSinto()
    },

    methods: {
      async getClassificacaoDeRisco () {
        this.classificacaoDeRisco = this.enums.classificacaoDeRisco
        // console.log('this.classificacaoDeRisco', this.classificacaoDeRisco)
      },
      async onClickComorbidades (val) {
        // val é this.formComorbidades
        val.RespondeuComorbidade = true
        val.DataAlteracao = new Date()
        //console.log('this.formComorbidades#VAL', val)

        let { data, status } = await _api.individuos.atualizarComorbidade(this.agendamento.individuoId, val)
        // console.log('data', data)
        // console.log('status', status)
        if (status === 200) {
          // this.$message.success('Comorbidades Enviadas Com Sucesso')
          this.salvarComorbidades = true
        } else {
          this.$swal({
            title: "Erro!",
            text: 'Ocorreu um erro no envio das comorbidades',
            icon: 'error',
          })
        }
      },
      async onClickSintomas (val) {
        // console.log('this.formSintomas', val)
        val.individuoId = this.agendamento.individuoId
        let { data, status } = await _api.acompanhamento.post(val)
        // console.log('data', data)
        // console.log('status', status)
        if (status === 201) {
          // this.$message.success('Sintomas Enviados Com Sucesso')
          this.salvarSintomas = true
        } else {
          this.$swal({
            title: "Erro!",
            text: 'Ocorreu um erro no envio dos sintomas',
            icon: 'error',
          })
        }
      },
      // async receberDadosEcoF200() {
      //  this.loading.f200 = true
      //  //console.log('Clicou no botão "RECEBER EXAMES (ECO F200)"')
      //  //console.log('Loading F200: ', this.loading.f200)
      //  //console.log('Show Modal: ', this.showModal)
      //  let { status } = await _api.examesF200.postExamesF200(this.api.individuo.cpf)
      //  if (status === 200) {
      //    this.showModal = false
      //    this.loading.f200 = false
      //  }
      //  //console.group('Resultados do recebimento de exames da F200')
      //  //console.log('Loading F200: ', this.loading.f200)
      //  //console.log('Show Modal: ', this.showModal)
      //  //console.log('Status: ', status)
      //  //console.groupEnd()
      // },

      // fecharModal() {
      // this.showModal = false
      // this.loading.f200 = false
      // console.log('Clicou no botão "Fechar" do Modal')
      // console.log('Loading F200: ', this.loading.f200)
      // console.log('Show Modal: ', this.showModal)
      // },
      // PEGANDO SINAIS VITAIS DA RECEPCAO (ATENDIMENTO)
      // async getSinaisVitaisRecepcao () {
      //  // retornando para api dados das aferições da recepcao
      //  // console.log('this.agendamento', this.agendamento)
      //  this.api.sinaisVitaisRecepcao.dataAlteracaoRecepcao = this.agendamento.dataAlteracaoRecepcao
      //  this.api.sinaisVitaisRecepcao.batimentoCardiacoRecepcao = this.agendamento.batimentoCardiacoRecepcao
      //  this.api.sinaisVitaisRecepcao.oxigenacaoSanguineaRecepcao = this.agendamento.oxigenacaoSanguineaRecepcao
      //  this.api.sinaisVitaisRecepcao.pressaoSanguineaRecepcao = this.agendamento.pressaoSanguineaRecepcao
      //  this.api.sinaisVitaisRecepcao.alturaRecepcao = this.agendamento.alturaRecepcao
      //  this.api.sinaisVitaisRecepcao.pesoRecepcao = this.agendamento.pesoRecepcao
      //  this.api.sinaisVitaisRecepcao.temperaturaRecepcao = this.agendamento.temperaturaRecepcao

      //  // IMC RECEPCAO
      //  var imcDados = (this.api.sinaisVitaisRecepcao.pesoRecepcao / ((this.api.sinaisVitaisRecepcao.alturaRecepcao / 100) * (this.api.sinaisVitaisRecepcao.alturaRecepcao / 100)))
      //  var imcFloatDados = parseFloat(imcDados).toFixed(1)
      //  this.api.sinaisVitaisRecepcao.dadosImc = imcFloatDados

      //  if (imcFloatDados >= 16 & imcFloatDados <= 16.9) {
      //    this.api.sinaisVitaisRecepcao.statusImc = 'Muito abaixo do Peso'
      //  }
      //  if (imcFloatDados >= 17 & imcFloatDados <= 18.4) {
      //    this.api.sinaisVitaisRecepcao.statusImc = 'Abaixo do Peso'
      //  }
      //  if (imcFloatDados >= 18.5 & imcFloatDados <= 24.9) {
      //    this.api.sinaisVitaisRecepcao.statusImc = 'Peso Normal'
      //  }
      //  if (imcFloatDados >= 25 & imcFloatDados <= 29.9) {
      //    this.api.sinaisVitaisRecepcao.statusImc = 'Acima do Peso'
      //  }
      //  if (imcFloatDados >= 30 & imcFloatDados <= 34.9) {
      //    this.api.sinaisVitaisRecepcao.statusImc = 'Obesidade Grau I'
      //  }
      //  if (imcFloatDados >= 35 & imcFloatDados <= 40) {
      //    this.api.sinaisVitaisRecepcao.statusImc = 'Obesidade Grau II'
      //  }
      //  if (imcFloatDados > 40) {
      //    this.api.sinaisVitaisRecepcao.statusImc = 'Obesidade Grau III'
      //  }
      //  // retirando da api e passando para um objeto no data e exibindo

      //  this.sinaisRecepcao = this.api.sinaisVitaisRecepcao
      //  // console.log('sinaisRecepcao', this.sinaisRecepcao)
      // },

      // PEGANDO SINAIS VITAIS DA PULSEIRA (ATENDIMENTO)
      async getSinaisVitaisPulseira () {
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

        // else {
        //  this.api.sinaisVitaisPulseira.statusImc = 'ERRO'
        // }

        this.sinaisPulseira = this.api.sinaisVitaisPulseira
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
        // console.log('RESULTS', results)
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
      handleSelect (item) {
        // console.log(item)
      },

      handleTabClick (tab, event) {
        if (tab.label === 'Chat') {
          setTimeout(() => {
            this.scrollChatToEnd()
          }, 500)
        }
      },

      async getPaciente () {
        // console.log('this.agendamento.individuoId', this.agendamento.individuoId)
        let { data, status } = await _api.individuos.getById(this.agendamento.individuoId)
        this.api.individuo = (status === 200) ? data : []
        let { nomeCompleto, nomeDaMae } = this.api.individuo
        if (nomeCompleto && nomeDaMae) {
          this.api.individuo = {
            ...this.api.individuo,
            nomeCompleto: nomeCompleto.toUpperCase(),
            nomeDaMae: nomeDaMae.toUpperCase()
          }
        }
      },

      async getComorbidades () {
        let { data } = await _api.individuos.getById(this.agendamento.individuoId)

        this.api.comorbidades = (data !== null) ? data : {}
      },

      async getComoMeSinto () {
        if (this.agendamento.id != null) {
          this.paramsAcompanhamento.individuoId = this.agendamento.individuoId
          let { data } = await _api.acompanhamento.get(this.paramsAcompanhamento)
          let comoMeSinto = (data !== null) ? data : {}
          this.api.comoMeSinto = comoMeSinto.slice(-1)
        }
      },

      async submitForm (form) {
        this.$refs[form].validate(valid => {
          // console.log('valid', valid)
          this.loading.atendimentos = true
          if (valid) {
            if (this.enviouSinaisVitais == true) {
              // se for demanda esponanea
              if (this.demandaEspontanea == true) {
                let putAgendamento = {}
                putAgendamento.id = this.agendamento.id
                putAgendamento.presencaConfirmada = true
                putAgendamento.corStatusTriagem = this.grauDeRisco == null ? 1 : this.grauDeRisco

                if (putAgendamento.corStatusTriagem <= 2) {
                  // CASO O GRAU DE RISCO DELE SEJA ABAIXO DE 3 SERÁ ENCAMINHADO O CHAMADO PARA O MEDICO VIRTUAL
                  // console.log('status <= 2')
                  if (putAgendamento.corStatusTriagem != null) {
                    _api.agendamentos.putConfirmarPresenca(putAgendamento).then(res => {
                      if (res.status === 204) {
                        // console.log('ok agendamento')
                      } else {
                        // console.log('não ok agendamento')
                      }
                    })

                    this.formAtendimento.atendidoTriagem = true
                    let atendimento = { ...this.formAtendimento, agendamentoId: this.agendamento.id }
                    _api.atendimentos.post(atendimento).then(res => {
                      if (res.status === 201) {
                        // console.log('ok atendimento')
                        // console.log('formSintomas', this.formSintomas)
                        let atendimentoId = res.data.id
                        this.formSintomas.atendimentoId = atendimentoId
                        // console.log('this.formSintomas', this.formSintomas)
                        this.onClickSintomas(this.formSintomas)
                        if (this.api.individuo.respondeuComorbidade == null || this.api.individuo.respondeuComorbidade == false) {
                          this.onClickComorbidades(this.formComorbidades)
                        }
                        this.loading.atendimentos = false
                      } else {
                        // console.log('não ok atendimento')
                      }
                      this.loading.atendimentos = false
                      this.$router.push({ name: 'Agenda' })
                    })
                  } else {
                    this.$swal({
                      title: "Atenção!",
                      text: 'Selecione um grau de risco!',
                      icon: 'warning',
                    })
                  }
                } else {
                  // CASO O GRAU DE RISCO DELE SEJA ACIMA DE 3 SERÁ FINALIZADO O CHAMADO PELA TRIAGEM MESMO E NÃO IRA PARA O MEDICO VIRTUAL
                  // console.log('status >= 3')

                  this.$alert('O Paciente atendido tem um Grau de Risco Muito Alto, Encaminhe para o Atendimento Presencial!', 'ATENÇÃO!', {
                    confirmButtonText: 'OK',
                    type: 'warning',
                    center: true
                  }).then(() => {
                    this.$message({
                      type: 'success',
                      message: 'Finalizado'
                    })
                  })

                  putAgendamento.finalizado = true
                  if (putAgendamento.corStatusTriagem != null) {
                    _api.agendamentos.putFinalizarTriagem(putAgendamento).then(res => {
                      if (res.status === 204) {
                        // console.log('ok agendamento')
                      } else {
                        // console.log('não ok agendamento')
                      }
                    })

                    this.formAtendimento.atendidoTriagem = true
                    let atendimento = { ...this.formAtendimento, agendamentoId: this.agendamento.id }
                    _api.atendimentos.post(atendimento).then(res => {
                      if (res.status === 201) {
                        // console.log('ok atendimento')
                        this.loading.atendimentos = false
                      } else {
                        // console.log('não ok atendimento')
                      }
                      this.loading.atendimentos = false
                      this.$router.push({ name: 'Agenda' })
                    })
                  } else {
                    this.$swal({
                      title: "Atenção!",
                      text: 'Verifique o grau de risco!',
                      icon: 'warning',
                    })
                  }
                }
              } else {
                // se não for demanda espontanea
                let putAgendamento = {}
                putAgendamento.id = this.agendamento.id
                putAgendamento.presencaConfirmada = true
                putAgendamento.corStatusTriagem = this.grauDeRisco == null ? this.agendamento.individuo.corStatus : this.grauDeRisco

                _api.agendamentos.putConfirmarPresenca(putAgendamento)

                this.formAtendimento.atendidoTriagem = true
                let atendimento = { ...this.formAtendimento, agendamentoId: this.agendamento.id }
                atendimento.localDeAtendimento = this.agendamento.tipoDaConsulta === 'Presencial' ? 4 : 1
                _api.atendimentos.post(atendimento).then(res => {
                  if (res.status === 201) {
                    let atendimentoId = res.data.id
                    this.formSintomas.atendimentoId = atendimentoId
                    this.onClickSintomas(this.formSintomas)
                    if (this.api.individuo.respondeuComorbidade == null || this.api.individuo.respondeuComorbidade == false) {
                      this.onClickComorbidades(this.formComorbidades)
                    }
                    this.loading.atendimentos = false
                  } else {
                    // console.log('não enviado atendimento')
                  }
                  this.loading.atendimentos = false
                  this.$router.push({ name: 'Agenda' })
                })
              }
            } else {
              this.$swal({
                title: "Atenção!",
                text: 'O envio dos sinais vitais do paciente é obrigatório!',
                icon: 'warning',
              })
            }
          } else {
            this.$swal({
              title: "Atenção!",
              text: 'Preencha os campos obrigatórios!',
              icon: 'warning',
            })
          }
        })
      },

      handleClose (done) {
        done()
      },

      resetForm (form) {
        this.$refs[form].resetFields()
      },
      scrollChatToEnd () {
        // console.log('acionou o scroll')
        jQuery('#pscroll').animate({ scrollTop: jQuery('ul#conversation li:last').offset().top + 30 }, 1000)
      },

      handleSizeChangeExames (val) {
        // console.log('val take',val)
        this.paramsExames.take = val
        this.getExames()
      },
      handleCurrentChangeExames (val) {
        // console.log('val skip', val)
        this.paramsExames.skip = val
        this.getExames()
      },

      handleSizeChange (val) {
        this.params.take = val
      },

      onAlterarSinaisVitais () {
        if (this.alterarSinaisVitais == true) {
          // console.log("this.validacoes", this.validacoes)

          this.formUsuario = {
            ...this.formUsuario,
            id: this.formRow.id,
            individuoId: this.formRow.individuoId,
            profissionalId: this.formRow.profissionalId,
            estabelecimentoId: this.formRow.estabelecimentoId,
            procedimentoId: this.formRow.procedimentoId,
            dia: this.formRow.dia,
            hora: this.formRow.hora,
            tipoDaConsulta: this.formRow.tipoDaConsulta,
            presencaConfirmada: this.formRow.presencaConfirmada
          }

          this.validacoes.pressaoSanguinea[0].required = true
          this.validacoes.batimentoCardiaco[0].required = true
          this.validacoes.oxigenacaoSanguinea[0].required = true
          this.validacoes.temperatura[0].required = true
          this.validacoes.altura[0].required = true
          this.validacoes.peso[0].required = true
        } else {
          this.validacoes.pressaoSanguinea[0].required = false
          this.validacoes.batimentoCardiaco[0].required = false
          this.validacoes.oxigenacaoSanguinea[0].required = false
          this.validacoes.temperatura[0].required = false
          this.validacoes.altura[0].required = false
          this.validacoes.peso[0].required = false
        }
      },

      // ----------- PARTE DOS EXAMES ATENDIMENTO -----------//

      async getExames () {
        // console.log('entrei no get exames')
        let { data, status, paginacao } = await _api.exames.getExames(this.paramsExames)
        var arrExame = (status === 200) ? data : []
        this.paramsExames.skip = (status === 200) ? paginacao.skip : 0
        this.paramsExames.total = (status === 200) ? paginacao.totalCount : 0

        let individuoId = this.api.individuo.id
        this.api.exames = arrExame.filter(exames => {
          if (exames.individuoId === individuoId) {
            return arrExame
          } else {
          }
        })

        this.api.exames = this.api.exames.filter(exames => {
          if (exames.avaliado) {
            exames.avaliado = moment(exames.avaliado).format('DD/MM/YYYY')
            return this.api.exames
          }
        })
        // console.log('this.api.exames', this.api.exames)
      },

      getTipoExames (val) {
        let tipo = this.enums.tipoExames.filter(x => x.value == val)[0]
        return tipo.label
      },

      confirmarPresenca () {
        this.loading.usuario = true
        if (this.formUsuario != '') {
          this.formUsuario.id = this.formRow.id
          this.formUsuario.presencaConfirmada = true
          _api.agendamentos.putRecepcao(this.formUsuario).then(res => {
            this.statusPutRecepcao = res.status
            if (res.status === 204) {
              // console.log(res)
              this.loading.usuario = false
              // console.log('this.loading.usuario', this.loading.usuario)
            }
            if (this.statusPutRecepcao === 204) {
              this.$modal.hide('sinaisModal')
              this.getAgendamentos()
            }
          })
        } else {
          this.$swal({
            title: "Atenção!",
            text: 'Preencha os campos obrigatórios!',
            icon: 'warning',
          })
          this.loading.usuario = false
          return false
        }
      },

      negarPresenca () {
        this.loading.usuario = true
        if (this.formUsuario != '') {
          // console.log('presença negada if')
          this.formUsuario.id = this.formRow.id
          this.formUsuario.naoCompareceu = true
          this.formUsuario.finalizado = true
          // console.log('this.formusuario', this.formUsuario)
          _api.agendamentos.putRecepcao(this.formUsuario).then(res => {
            if (res.status === 204) {
              // console.log(res)
              this.loading.usuario = false
              // console.log('this.loading.usuario', this.loading.usuario)
            }

            this.$modal.hide('sinaisModal')
            this.getAgendamentos()
          })
        } else {
          this.$swal({
            title: "Atenção!",
            text: 'Preencha os campos obrigatórios!',
            icon: 'warning',
          })
          this.loading.usuario = false
          return false
        }
      },

      async onClickSalvarSinaisVitais (form) {
        this.$refs[form].validate(async (valid) => {
          this.loading.usuario = true
          if (valid) {
            let { data, status, paginacao } = await _api.agendamentos.putSinaisVitais(this.formUsuario)
            if (status === 204) {
              this.loading.usuario = false
              this.alterarSinaisVitais = false
              this.enviouSinaisVitais = true
            }
          } else {
            this.$swal({
              title: "Atenção!",
              text: 'Preencha os campos obrigatórios dos sinais vitais!',
              icon: 'warning',
            })
          }
        })
      },

      // ENVIO DOS EXAMES
      isPdf (file) {
        // console.log('file.type', file.type)
        const isPdf = file.type === 'application/pdf' || file.type === 'image/jpeg' || file.type === 'image/jpg' || file.type === 'image/png' || file.type === 'audio/mp3' || file.type === 'audio/wav'
        if (!isPdf) {
          this.$swal({
            title: "Atenção!",
            text: 'O arquivo deve estar no formato PDF, JPEG, JPG, PNG, MP3 ou WAV!',
            icon: 'warning',
          })
        }
        return isPdf
      },

      encodeImageFileAsURL (response, file) {
        // console.log('file', file)

        let tipoExame = this.tipoExame
        // console.log(tipoExame)
        var arq = file.raw

        var reader = new FileReader()

        reader.onloadend = () => {
          // console.log('entreiiiiii')
          // console.log("reader.result", reader.result)
          // console.log("arq", arq)
          let Url = reader.result
          let formato = ''

          if (arq.type === 'application/pdf') {
            Url = reader.result.replace('data:application/pdf;base64,', '')
            formato = '.pdf'
          }
          if (arq.type === 'image/jpeg') {
            Url = reader.result.replace('data:image/jpeg;base64,', '')
            formato = '.jpeg'
          }
          if (arq.type === 'image/jpg') {
            Url = reader.result.replace('data:image/jpg;base64,', '')
            formato = '.jpg'
          }
          if (arq.type === 'image/png') {
            Url = reader.result.replace('data:image/png;base64,', '')
            formato = '.png'
          }
          if (arq.type === 'audio/mp3') {
            Url = reader.result.replace('data:audio/mp3;base64,', '')
            formato = '.mp3'
          }
          if (arq.type === 'audio/wav') {
            Url = reader.result.replace('data:audio/wav;base64,', '')
            formato = '.wav'
          }

          let individuoId = this.api.individuo.id
          let individuoCpf = this.api.individuo.cpf
          // console.log('this.api.individuo.cpf', this.api.individuo)
          let name = file.name
          let tipoExame = this.tipoExame

          // console.log('individuoId', individuoId)
          // console.log('individuoId', individuoCpf)
          // console.log('individuoId', name)
          // console.log('individuoId', tipoExame)
          this.exames.push({ 'individuoId': individuoId, 'individuoCpf': individuoCpf, tipoExame: tipoExame, 'Url': Url, 'nome': name, 'formato': formato })
          // console.log('this.exames no encodeImageFileAsUrl', this.exames)
        }
        reader.readAsDataURL(arq)
        this.showBtn = false
        this.showEnviar = true
      },

      onDeleteDocs (file) {
        this.exames = this.exames.filter(exame => {
          // console.log('exame.name -> ', exame.name)
          // console.log('file.name -> ', file.name)
          return exame.name !== file.name
          // console.log(exame.name !== file.name)
        })
        // console.log("exames", exames)
      },

      handleSelectChange (value) {
        this.showBtn = true
        // this.tipoExame = value;
        this.api.exames.forEach(exame => {
          if (exame.id === value) {
            this.tipoExame = exame
          }
        })
        // console.log('this.tipoExame', this.tipoExame)
        // console.log('TIPO EXAMEE', this.tipoExame)
      },

      uploadDocs () {
        this.loading.exames = true

        _api.exames.postExames(this.exames).then(res => {
          // console.log(res.status)
          if (res.status === 201 || res.status === 200) {
            this.$swal({
              title: "Sucesso!",
              text: 'Os exames foram enviados com sucesso!',
              icon: 'success',
            })
            this.loading.exames = false
            this.salvarExames = true
          } else {
            this.$swal({
              title: "Erro!",
              text: 'Ocorreu um erro ao enviar os exames!',
              icon: 'error',
            })
            this.loading.exames = false
          }
        })
      },

      async getExames () {
        // console.log('entrei')
        // console.log(this.params)
        let { data, paginacao, status } = await _api.exames.getTipoExames({
          skip: 1,
          take: 100,
          sort: 'Nome ASC',
          total: 0
        })

        // console.log(status)
        this.api.exames = (status === 200) ? data : []
        // console.log('this.api.exames', this.api.exames)
        this.paramsExames.skip = (status === 200) ? paginacao.currentPage : 0
        this.paramsExames.total = (status === 200) ? paginacao.totalCount : 0
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
    height: 600px;
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
</style>
