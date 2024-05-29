<template>
  <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
    <el-card class="box-card--main-card" v-show="listando" v-loading="loading.visualizarAtendimento">

      <el-row>
        <el-col :span="14">
          <h2 class="box-card--h2">Histórico De Atendimentos</h2>
        </el-col>
      </el-row>

      <el-row>
        <el-col :span="24">
          <FiltroHistorico :loading="loading.historico" :params="params" @emit="onEmitFromFiltro" />
        </el-col>
      </el-row>

      <el-empty v-show="listando && api.historico.length === 0" description="Nenhum Atendimento Encontrado"></el-empty>
      <el-row v-show="listando && api.historico.length > 0">
        <el-col :span="24">
          <el-table ref="tabela" :data="api.historico"
                    highlight-current-row border
                    v-loading="loading.historico"
                    :default-sort="{prop: 'dataCadastro', order: 'descending'}"
                    class="table--atendimento table--row-click">
            <el-table-column label="Data da Consulta" prop="dataCadastro" align="center" width="170" fixed sortable>
              <template slot-scope="scope">

                <span>{{moment(scope.row.dataCadastro).format('DD/MM/YYYY')}}</span>
              </template>
            </el-table-column>
            <el-table-column label="Hora da Consulta" prop="dataCadastro" align="center" width="170">
              <template slot-scope="scope">
                <span>{{moment(scope.row.dataCadastro).format('HH:mm')}}</span>
              </template>
            </el-table-column>
            <el-table-column label="Paciente" prop="individuo.nomeCompleto" align="center" />
            <el-table-column label="CPF" prop="individuo.cpf" align="center">
              <template slot-scope="scope">
                <span>{{scope.row.individuo.cpf ? scope.row.individuo.cpf : 'Não Cadastrado'}}</span>
              </template>
            </el-table-column>
            <el-table-column label="CNS" prop="individuo.cns" align="center">
              <template slot-scope="scope">
                <span>{{scope.row.individuo.cns ? scope.row.individuo.cns : 'Não Cadastrado'}}</span>
              </template>
            </el-table-column>
            <el-table-column label="Ativo" prop="ativo" width="90" align="center">
              <template slot-scope="scope">
                <span>{{scope.row.ativo === true ? 'SIM' : 'NÃO'}}</span>
              </template>
            </el-table-column>
            <el-table-column label="Atendido por" prop="atendidoTriagem" width="110" align="center">
              <template slot-scope="scope">
                <span>{{scope.row.atendidoTriagem === true ? 'TRIAGEM' : 'MÉDICO'}}</span>
              </template>
            </el-table-column>
            <el-table-column label="Profissional" prop="profissional.nome" align="center">
              <template slot-scope="scope">
                <span>{{scope.row.atendidoTriagem === true ? 'TRIAGEM' : scope.row.profissional.nome}}</span>
              </template>
            </el-table-column>
            <el-table-column label="CRM" prop="profissional.crm" align="center">
              <template slot-scope="scope">
                <span>{{scope.row.atendidoTriagem === true ? 'Não Possue' : scope.row.profissional.crm}}</span>
              </template>
            </el-table-column>
            <el-table-column label="Retornos" prop="agendamento.retorno" align="center">
              <template slot-scope="scope">
                <span>{{scope.row.agendamento.retorno === true ? 'SIM' : 'NÃO'}}</span>
              </template>
            </el-table-column>

            <el-table-column header-align="center" align="center" width="140" fixed="right" v-if="!mxHasAccess('Recepcionista')">
              <template slot-scope="scope">
                <el-dropdown>
                  <el-button type="primary" size="small">
                    Ação <i class="el-icon-arrow-down el-icon--right"></i>
                  </el-button>
                  <el-dropdown-menu slot="dropdown">
                    <ul class="list-unstyled">
                      <li @click="onVisualizarAtendimentoHistorico(scope.$index, scope.row)" class="el-dropdown-menu__item">
                        <i class="fas fa-notes-medical"></i>
                        Visualizar
                      </li>
                      <!--<li v-if="scope.row.ativo === true" @click="onDesativar(scope.row)" class="el-dropdown-menu__item text-danger">
              <i class="fas fa-trash"></i>
              Inativar
            </li>-->
                    </ul>
                  </el-dropdown-menu>
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
                         :page-size="params.pageSize"
                         :total="params.total"
                         layout="total, sizes, prev, pager, next, jumper">
          </el-pagination>
        </el-col>
      </el-row>





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
    <el-card shadow="always" v-show="!listando">
      <div style="display: flex; justify-content: flex-end">
        <el-button v-if="!listando" type="warning" @click="onVoltar('formAtendimento')"> Voltar</el-button>
      </div>

      <component
                 v-if="exibirDadosHeaderHistorico"
                 :is="'dados-header-historico'"
                 :individuo="api.individuo"
                 :formAtendimento="formAtendimento"
                 :profissionalAnteriorInfo="api.profissionalAnteriorInfo"
                 :agendamentoRetornoInfo="api.agendamentoRetornoInfo"
                 :tipoAtendimento="formAtendimento.tipoAtendimento"
                 :modalidade="formAtendimento.modalidadeAD"
                 :condutaDesfechoEncaminhamento="formAtendimento.condutaDesfecho"
                 :condicoesAvaliadas="formAtendimento.condicoesAvaliadas"
                 :procedimentosRealizados="formAtendimento.procedimentosRealizados"
                 />

      <div>
        <el-row>
          <!--{{formAtendimento.agendamento}}-->
        </el-row>
      </div>

      <!--SINAIS VITAIS HISTORICO NÃO ALTERAR-->
      <el-collapse style="margin-top: 20px">
        <el-collapse-item v-if="sinaisHistorico.pressaoArterial != null" title="Sinais Vitais" name="1">
          <div style="margin-bottom: 10px">
            <el-tag type="info">Data de envio: {{ moment(sinaisHistorico.dataAlteracao).format('DD/MM/YYYY HH:mm') }}</el-tag>
          </div>
          <el-descriptions :column="3" border>
            <el-descriptions-item label="Pre. Arterial">
              <el-tag size="small">{{ sinaisHistorico.pressaoArterial && sinaisHistorico.pressaoArterial + ' mmhg' }}</el-tag>
            </el-descriptions-item>
            <el-descriptions-item label="Fre. Cardiaca">
              <el-tag size="small">{{ sinaisHistorico.frequenciaCardiaca && sinaisHistorico.frequenciaCardiaca + ' bpm' }}</el-tag>
            </el-descriptions-item>
            <el-descriptions-item label="Saturação O2">
              <el-tag size="small">{{ sinaisHistorico.saturacaoO2 && sinaisHistorico.saturacaoO2 + ' %' }}</el-tag>
            </el-descriptions-item>
            <el-descriptions-item label="Temperatura">
              <el-tag size="small">{{ sinaisHistorico.temperatura && sinaisHistorico.temperatura + ' °C' }}</el-tag>
            </el-descriptions-item>
            <el-descriptions-item label="Altura">
              <el-tag size="small">{{ sinaisHistorico.altura && sinaisHistorico.altura + ' cm' }}</el-tag>
            </el-descriptions-item>
            <el-descriptions-item label="Peso">
              <el-tag size="small">{{ sinaisHistorico.peso && sinaisHistorico.peso + ' kg' }}</el-tag>
            </el-descriptions-item>
            <el-descriptions-item label="IMC">
              <el-tag size="small">{{  sinaisHistorico.dadosImc }}</el-tag>
            </el-descriptions-item>
            <el-descriptions-item label="GRAU IMC">
              <el-tag size="small">{{  sinaisHistorico.statusImc }}</el-tag>
            </el-descriptions-item>
          </el-descriptions>
        </el-collapse-item>


        <el-collapse-item title="Sintomas" name="19">
          <div v-if="api.comoMeSintoAtendimentoHistorico.length === 0">
            <p>Paciente sem sintomas</p>
          </div>
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
            </div>
          </div>
        </el-collapse-item>
      </el-collapse>

      <!--<el-row :gutter="24" style="margin-top:25px">
        <el-col :xs="10" :sm="10" :md="10" :lg="10" :xl="10" style="display: flex; flex-direction: row; justify-content: center" v-show="formAtendimento.tipoAtendimento != undefined">
          <h3>Tipo de Atendimento: <strong>{{formAtendimento.tipoAtendimento != undefined ? exibirTipoAtendimento(formAtendimento.tipoAtendimento) : 'Erro'}}</strong></h3>
        </el-col>
        <el-col v-if="formAtendimento.modalidadeAD" :xs="4" :sm="4" :md="4" :lg="4" :xl="4" style="display: flex; flex-direction: row; justify-content: center" >
          <h3>Modalidade: <strong>{{formAtendimento.modalidadeAD ? `AD${formAtendimento.modalidadeAD}` : 'Erro ao retornar a modalidade AD'}}</strong></h3>
        </el-col>
        <el-col :xs="10" :sm="10" :md="10" :lg="10" :xl="10" style="display: flex; flex-direction: row; justify-content: center" v-show="formAtendimento.condutaDesfecho != undefined">
          <h3 v-if="formAtendimento.tipoAtendimento != 2">Conduta/Desfecho/Encaminhamento: <strong>{{formAtendimento.condutaDesfecho != undefined ? exibirCondutaDesfecho(formAtendimento.condutaDesfecho) : 'Erro ao retornar a conduta e desfecho'}}</strong></h3>
          <h3 v-else>Conduta/Desfecho: <strong>Tipo de atendimento Visita domiciliar pós parto não possui conduta/desfecho</strong></h3>
        </el-col>
        </el-col>
      </el-row>-->
      <!--<el-row :gutter="24" style="margin-top:10px" v-show="formAtendimento.condicoesAvaliadas != undefined">
        <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" style="display: flex; flex-direction: row; justify-content: center">
          <h3>Condições Avaliadas: <strong>{{formAtendimento.condicoesAvaliadas != undefined ? formAtendimento.condicoesAvaliadas : 'Erro ao retornar as condições avaliadas'}}</strong></h3>
        </el-col>
      </el-row>-->
      <!--<el-row :gutter="24" style="margin-top:10px" v-show="formAtendimento.procedimentosRealizados != undefined">
        <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
          <h3 v-if="formAtendimento.tipoAtendimento != 2">Procedimentos Realizados: <br /> <strong>{{formAtendimento.procedimentosRealizados != undefined ? formAtendimento.procedimentosRealizados : 'Erro ao retornar os procedimentos realizados'}}</strong></h3>
          <h3 v-else>Procedimentos Realizados: <strong>Tipo de atendimento Visita domiciliar pós parto não possui procedimentos realizados</strong></h3>
        </el-col>
      </el-row>-->



      <!--<pre>{{formAtendimento}}</pre>-->
    <el-form v-if="true" :model="formAtendimento" status-icon :disabled="isDisabled"
             label-width="120px" style="line-height: 0 !important; margin-top: 20px" class="form--agendamento">

      <el-row>
        <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
          <div>
            <h3>Objetivo</h3>
            <el-input type="textarea" v-model="formAtendimento.objetivo" />
          </div>
        </el-col>
      </el-row>

      <el-row>
        <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
          <div>
            <h3>Avaliação</h3>
            <el-input type="textarea" v-model="formAtendimento.avaliacao" />
          </div>
        </el-col>
      </el-row>

      <el-row>
        <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
          <div>
            <h3>Plano</h3>
            <el-input type="textarea" v-model="formAtendimento.plano" />
          </div>
        </el-col>
      </el-row>

      <el-row>
        <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
          <div>
            <h3>Subjetivo</h3>
            <el-input type="textarea" v-model="formAtendimento.subjetivo" />
          </div>
        </el-col>
      </el-row>


      <el-row v-if="api.cid10Historico != null">
        <el-form-item prop="ciap">
          <el-table :data="api.cid10Historico" style="margin-left: -120px;">
            <el-table-column prop="descricao" label="Cid10">
            </el-table-column>
          </el-table>
        </el-form-item>
      </el-row>
      <el-row v-if="api.ciapHistorico != null">
        <el-form-item prop="ciap">
          <el-table :data="api.ciapHistorico" style="margin-left: -120px; width:100%;">
            <el-table-column prop="descricao" label="Ciap">
            </el-table-column>
          </el-table>
        </el-form-item>
      </el-row>
      <el-row v-if="api.procedimentoSigtapHistorico != null">
        <el-form-item prop="ciap">
          <el-table :data="api.procedimentoSigtapHistorico" style="margin-left: -120px; width:100%;">
            <el-table-column prop="label" label="SIGTAP">
            </el-table-column>
          </el-table>
        </el-form-item>
      </el-row>


      <!--Informações Adicionais-->

      <el-row v-if="formAtendimento != null" style="margin-left: 0px; margin-bottom: 15px;">
        <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
          <div>
            <h3>Informações Adicionais</h3>
          </div>
        </el-col>
      </el-row>

      <el-row v-if="formAtendimento != null" style="margin-left: 0px; margin-bottom: 15px;">
        <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
          <div>
            <p> Alergias</p>
            <el-input v-model="formAtendimento.alergias" type="input" disabled></el-input>
          </div>
        </el-col>
      </el-row>

      <el-row v-if="formAtendimento != null" style="margin-left: 0px; ">
        <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
          <div>
            <p>Antecedentes</p>
            <el-input v-model="formAtendimento.antecedentes" type="input" disabled></el-input>
          </div>
        </el-col>
      </el-row>

    </el-form>

      <!--<el-row>-->
      <!-- Menu Dos Exames HISTORICO -->
      <!--<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24">
  <el-card shadow="always" style="margin-top: 20px">
    <div>
      Exames Solicitados e/ou Avaliados
      <el-table :data="api.examesHistorico" ref="tableHistorico" border style="margin-top: 20px">



        <el-table-column prop="avaliado" label="Avaliado" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.avaliado == "01/01/0001" ? 'Ainda Não Avaliado' : scope.row.avaliado }}</span>
          </template>

        </el-table-column>

        <el-table-column show-overflow-tooltip label="Nome Do Exame" prop="tipoExameId" align="center">
          <template slot-scope="scope">
            <span>{{ getTipoExamesHistorico(scope.row.tipoExameId) }}</span>
          </template>
        </el-table-column>

        <el-table-column prop="resultado" label="Resultado" align="center">
          <template slot-scope="scope">
            <span>{{ scope.row.resultado == true ? 'SIM' : 'NÃO' }}</span>
          </template>
        </el-table-column>



        <el-table-column label="Avaliar Exame" width="150" header-align="center" align="center" fixed="right">
          <template slot-scope="scope">
            <el-dropdown align="center">
              <el-button type="primary" size="small">
                Ações <i class="el-icon-arrow-down el-icon--right"></i>
              </el-button>
              <el-dropdown-menu slot="dropdown">
                <ul class="list-unstyled">-->
      <!--<li @click="editarAvaliar(scope.row)" v-if="scope.row.resultado == true && scope.row.finalizado == false && scope.row.avaliado != '0001-01-01T00:00:00'" class="el-dropdown-menu__item"><i class="fas fa-user-edit"></i> Editar</li>-->
      <!--v-if="scope.row.resultado == true && scope.row.avaliado != '0001-01-01T00:00:00'"-->
      <!--<li @click="clickVisualizarHistorico(scope.row)" class="el-dropdown-menu__item"><i class="fas fa-user-edit"></i>Visualizar Avaliação</li>-->
      <!--<li @click="menuAvaliar(scope.row)" v-if="scope.row.resultado == false && scope.row.finalizado == false" class="el-dropdown-menu__item"><i class="fas fa-user-edit"></i> Avaliar</li>-->
      <!--<li @click="visualizarPdfHistorico(scope.row)" v-if="" class="el-dropdown-menu__item"><i class="far fa-file"></i> Visualizar PDF</li>
            </ul>
          </el-dropdown-menu>
        </el-dropdown>
      </template>
    </el-table-column>
  </el-table>-->
      <!-- DIV DE VISUALIZAR-->
      <!--<div style="margin-top: 20px" v-show="statusVisualizar">
            <el-form :model="formVisualizar" ref="formVisualizar" label-width="170px" label-position="top">
              <div class="divButtonExame">
                <el-button type="danger" size="small" @click="hideVisualizar" icon="el-icon-close"></el-button>
              </div>

              <el-row :gutter="20">
                <el-col>

                  <el-form-item label="Data Da Avaliação" prop="dataAvaliacao">
                    <el-input v-model="formVisualizar.avaliado" type="input" placeholder="Data Avaliacao" disabled style="width: 150px; margin-bottom: 10px">
                    </el-input>
                  </el-form-item>
                </el-col>
              </el-row>
              <el-row>
                <el-form-item label="Descrição" prop="descricao">
                  <el-input type="textarea" v-model="formVisualizar.descricao" disabled label-width="170px" />
                </el-form-item>
              </el-row>
            </el-form>
          </div>


        </div>
      </el-card>
    </el-col>
  </el-row>-->



    </el-card>
  </el-col>
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

  .el-table .warning-row {
    background: #FFDCDC;
  }

  .el-table .success-row {
    background: #D5FED3;
  }

  .title-consulta {
    font-size: 18px;
  }

  .item-comorbidades-sintomas {
    margin-bottom: 5px;
  }
</style>

<script>
  import Utils from '../../mixins/Utils'
  import _api from '../../api'
  import _enums from '../../api/Enums'
  import FiltroHistorico from '../../components/shared/FiltroHistorico'
  import DadosHeaderHistorico from '../../components/shared/DadosHeaderHistorico'
  import { mask } from 'vue-the-mask'
  import moment from 'moment'
  moment.locale('pt-br')
  export default {
    name: 'ListaHistorico',
    mixins: [Utils],
    components: { FiltroHistorico, DadosHeaderHistorico },
    directives: { mask },
    props: ['idPaciente'],
    data() {
      return {
        url: null,
        formInativar: {},
        historicoSelecionado: {},
        sinaisHistorico: {},
        formVisualizar: {},
        isDisabled: false,
        listando: true,
        listando2: true,
        isValid: true,
        statusVisualizar: false,
        exibirDadosHeaderHistorico: false,
        modo: 'adicionar',
        metodo: 'POST',
        formAtendimento: {},
        erros: [],
        agendamento: {},


        api: {
          procedimentos: [],
          comoMeSintoHistorico: {},
          sinaisVitaisHistorico: {},
          ciapHistorico: [],
          cid10Historico: [],
          historico: [],
          agendamento: {},
          ciap: [],
          cid10: [],
          documentos: [],
          individuo: {},
          comorbidades: {},
          examesHistorico: [],
          acompanhamento: {},
          comoMeSintoAtendimentoHistorico: [],
          agendamentoRetornoInfo: [],
          profissionalAnteriorInfo: [],
          procedimentoSigtapHistorico: [],
        },

        enums: {
          tipoExames: [
            ..._enums.tipoExames
          ],
          condicoesAvaliadas: [
            ..._enums.condicoesAvaliadas
          ],
        },
        loading: {
          historico: false,
          ciapCid: false,
          visualizarAtendimento: false
        },
        params: {
          skip: 1,
          take: 10,
          sort: 'DataCadastro DESC',
          total: 0,
          agendamentoId: ''
        },
        paramsIndividuoProcedimento: {
          skip: 1,
          take: 10,
          sort: 'DataCadastro DESC',
          total: 0,
          agendamentoId: ''
        },
        paramsAcompanhamentoHistorico: {
          skip: 1,
          take: 9999,
          sort: '+Data ASC',
          total: 0
        },
        paramsExames: {
          skip: 1,
          take: 5,
          sort: 'IndividuoId ASC',
          total: 0,
        },
        paramsAcompanhamento: {
          individuoId: null,
          atendidoMedico: true,
          skip: 1,
          take: 9999,
          sort: '+Data ASC',
          total: 0
        },
        paramsAllProcedimentos: {
          skip: 1,
          take: 10000,
          total: 0
        },
        
        routeParams: {}
      }
    },

    async mounted() {
      await this.perfilSelecionado()

      if (this.$route.params.tipoDaConsulta != null) {
        this.params.tipoDaConsulta = this.$route.params.tipoDaConsulta
      }

      if (this.mxHasAccess('Triagem') || this.mxHasAccess('Recepcionista')) {


        if (!this.isDemandaEspontanea) {
          //if (this.$route.params.dashboard == true && this.$route.params.tipoDaConsulta == undefined ||
          //  this.$route.params.dashboard == true && this.$route.params.tipoDaConsulta != undefined) {
          //  this.params.dataInicial = new Date()
          //  this.params.dataFinal = new Date()
          //}
          //if (this.$route.params.dashboard == true && this.$route.params.tipoDaConsulta != undefined) {
          //  this.params.dataInicial = new Date()
          //  this.params.dataFinal = new Date()
          //  this.params.tipoDaConsulta = this.$route.params.tipoDaConsulta
          //}
        }

      } else if (this.mxHasAccess("Médico") || this.mxHasAccess('MédicoAD')) {
        this.params.profissionalId = this.$auth.user().id
      } else { }

      await this.getHistorico();
      await this.getProcedimentos();
      //await this.getComorbidades();

    },
    methods: {

      async perfilSelecionado() {
        while (this.$store.state.user.selectRole == "") {
           await new Promise(resolve => setTimeout(resolve, 100));
        }
      },
      
      tableRow({ row }) {
        if (row.ativo === true) {
          return 'success-row';
        } else if (row.ativo === false) {
          return 'warning-row';
        }
        return '';
      },
      async onDesativar(row) {
        // console.log('row', row)
        this.$prompt('Descreva o motivo da inativação do atendimento:', 'Inativação do registro de atendimento', {
          confirmButtonText: 'Confirmar',
          cancelButtonText: 'Cancelar',
          inputPattern: /^[A-Za-záàâãéèêíïóôõöúçñÁÀÂÃÉÈÍÏÓÔÕÖÚÇÑ ]+$/,
          inputErrorMessage: 'O campo pode conter apenas letras'
        }).then(({ value }) => {
          this.formInativar.descricaoEditado = value
          this.formInativar.cadastroEditado = true
          this.formInativar.usuarioEditou = this.$auth.user().id
          this.formInativar.ativo = false
          this.formInativar.id = row.id

          this.Salvar(row)
        }).catch(() => {

        });
      },

      Salvar(row) {
        //console.log(row)

        _api.atendimentos.putInativar(this.formInativar).then(res => {
          // console.log('res', res)
          if (res.status === 204) {

            this.getHistorico()
          }
          this.loading.usuarios = false
        })
      },
      onEmitFromFiltro(val) {
        //if (this.params.individuo.nomeCompleto != null) {
        //  this.params.individuo
        //}
        this.params = {
          ...this.params,
          ...val.params,
          skip: 1
        }
        this.listando = true
        this.getHistorico()

      },

      handleSizeChange(val) {
        this.params.take = val
        this.getHistorico()
      },
      handleCurrentChange(val) {
        this.params.skip = val
        this.getHistorico()
      },

      async getHistorico() {
        this.loading.historico = true
        //get historico pelo medico
        if (this.mxHasAccess("Médico") || this.mxHasAccess('MédicoAD')) {
          this.params.profissionalId = this.$auth.user().id
          
          var myParams = {
            ...this.params, atendidoMedico: true, presencaConfirmada: true, naoCompareceu: false, cancelado: false,
          }
          if (myParams.agendamentoId != undefined || myParams.agendamentoId != null) delete myParams.agendamentoId
          let { data, paginacao, status } = await _api.atendimentos.get(myParams)
          if (status === 502) this.loading.historico = false
          this.api.historico = (status === 200) ? data : []
          //console.log("data", data)
          this.params.skip = (status === 200) ? paginacao.skip : 0
          this.params.total = (status === 200) ? paginacao.totalCount : 0

        }

        //get historico pela recepcao
        if (this.mxHasAccess("Recepcionista")) {

          var myParams = {
            ...this.params, atendidoMedico: true, presencaConfirmada: true, naoCompareceu: false, cancelado: false, 
          }
          //console.log("myParams", myParams)
          let { data, paginacao, status } = await _api.atendimentos.get(myParams)
          if (status === 502) this.loading.historico = false
          this.api.historico = (status === 200) ? data : []
          
          this.params.skip = (status === 200) ? paginacao.skip : 0
          this.params.total = (status === 200) ? paginacao.totalCount : 0

        }


        if (this.mxHasAccess("Triagem")) {

          var myParams = {
            ...this.params, atendidoMedico: true, presencaConfirmada: true, naoCompareceu: false, cancelado: false,
          }
          let { data, paginacao, status } = await _api.atendimentos.get(myParams)
          if (status === 502) this.loading.historico = false
          this.api.historico = (status === 200) ? data : []

          this.params.skip = (status === 200) ? paginacao.skip : 0
          this.params.total = (status === 200) ? paginacao.totalCount : 0

        }

        

        if (this.mxHasAccess("Administrador")) {
          var myParams = {
            ...this.params, atendidoMedico: true, presencaConfirmada: true, naoCompareceu: false, cancelado: false,
          }
          
          let { data, paginacao, status } = await _api.atendimentos.get(myParams)
          if (status === 502) this.loading.historico = false
          this.api.historico = (status === 200) ? data : []

          this.params.skip = (status === 200) ? paginacao.skip : 0
          this.params.total = (status === 200) ? paginacao.totalCount : 0

        }
        this.loading.historico = false
      },


      async onVisualizarAtendimentoHistorico(index, row) {
        this.loading.visualizarAtendimento = true;
        this.modo = 'editar'
        this.metodo = 'PUT'
        this.isDisabled = true
        
        this.api.agendamento = row.agendamento

        if (row.agendamento.retornoAgendamentoId != undefined) {
          await this.getAgendamentoRetornoInfo(row);
          await this.getNomeProfissionalAnteriorRetorno();
        }
        this.formAtendimento = {
          ...row
        }

        if (this.formAtendimento.alergias == undefined) this.formAtendimento.alergias = "Paciente sem alergias"
        if (this.formAtendimento.antecedentes == undefined) this.formAtendimento.antecedentes = "Paciente sem antecedentes"


        this.exibirDadosHeaderHistorico = true
        //await this.getComoMeSintoHistorico(row);
        this.api.atendimento = row

        await this.getCiapHistorico2(row);
        await this.getCid10Historico2(row);
        await this.getProcedimentoSigtapHistorico(row);
          

        await this.getSinaisVitaisHistorico(row);
        await this.getComoMeSintoAtendimentoHistorico(row)
        //if (row.condicoesAvaliadas != undefined) await this.recuperarCondicoesAvaliadas(row.condicoesAvaliadas)
        this.listando = false
        this.loading.visualizarAtendimento = false;

      },


      exibirTipoAtendimento(val) {
        if (val == 0) return "Erro"
        else if (val == 1) return "Consulta agendada programada / Cuidado continuado"
        else if (val == 2) return "Consulta agendada"
        else if (val == 4) return "Escuta inicial / Orientação"
        else if (val == 5) return "Consulta no dia"
        else if (val == 6) return "Atendimento de urgência"
        else if (val == 7) return "Atendimento programado"
        else if (val == 8) return "Atendimento não programado"
        else if (val == 9) return "Visita domiciliar pós-óbito"
        else "ERRO"
      },


      recuperarCondicoesAvaliadas(val) {
        var array = val.split(",")

        var arrayCondicoesAvaliadas = []
        array.forEach(item => {
          var label = this.findCondicoesAvaliadasEnum(item)
          arrayCondicoesAvaliadas.push(label)
        })

        this.formAtendimento.condicoesAvaliadas = arrayCondicoesAvaliadas.join(" / ");
      },

      findCondicoesAvaliadasEnum(string) {
        var stringRetorno = ""
        this.enums.condicoesAvaliadas.forEach(item => {
          if (item.value === string) {
            stringRetorno = item.label;
          } else {
            return null; // Caso a string não seja encontrada
          }
        })
        return stringRetorno
      },


      

      

      

      exibirCondutaDesfecho(val) {
        if (val == 1) return "Alta clínica"
        else if (val == 2) return "Atenção Básica (AD1)"
        else if (val == 3) return "Alta administrativa"
        else if (val == 4) return "Serviço de urgência e emergência"
        else if (val == 5) return "Serviço de internação hospitalar"
        else if (val == 7) return "Permanência"
        else if (val == 7) return "Óbito"
        else return ""
      },


      async getAgendamentoRetornoInfo(row) {
        //console.log('ROW: ', row)
        this.loading.agendamentos = true
        let { data, paginacao, status } = await _api.agendamentos.getById(row.agendamento.retornoAgendamentoId)
        if (status === 502) this.loading.agendamentos = false
        this.api.agendamentoRetornoInfo = (status === 200) ? data : []
        //console.log('agendamento retorno info: ', data)
        this.loading.agendamentos = false
      },
      async getNomeProfissionalAnteriorRetorno() {
        this.loading.agendamentos = true
        let { data, paginacao, status } = await _api.profissionais.getById(this.api.agendamentoRetornoInfo.profissionalId)
        if (status === 502) this.loading.agendamentos = false
        this.api.profissionalAnteriorInfo = (status === 200) ? data : []
        //console.log('agendamento retorno profissional info: ', data)
        this.loading.agendamentos = false
      },

      //RETORNA O QUE FOI PREENCHIDO EM ATENDIMENTO
      async getAtendimento(row) {
        this.loading.historico = true

        //OBS SOBRE MYPARAMS (CASO PRECISE RETORNAR SOMENTE OS ATENDIMENTOS ANTERIORES DO PRONTUARIO DO PACIENTE, QUE O PROFISSIONAL ATUAL FINALIZOU ADICIONAR-> profissionalId: this.agendamento.profissionalId)
        var myParams = {
          ...this.params, individuoId: row.individuo.id, agendamentoId: row.agendamento.id
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



      async getProcedimentoSigtapHistorico(row) {
        this.paramsIndividuoProcedimento.agendamentoId = row.agendamentoId
        let { data, status } = await _api.individuoProcedimento.get(this.params)

        if (status == 200) {
          if (data.length > 0) {
            var arrayString = []
            data.forEach(obj => {
              if (obj.procedimentoCodigo != undefined) {
                arrayString.push(obj.procedimentoCodigo)
              }
            })

            this.recuperarProcedimentos(arrayString)
          }
        }
      },


      async getProcedimentos() {
        let { data, status } = await _api.procedimentos.get(this.paramsAllProcedimentos)
        if (status === 200) this.api.procedimentos = data
      },

      recuperarProcedimentos(arrayString) {
        var arrayProcedimentos = []
        arrayString.forEach(item => {
          var label = this.findProcedimentos(item)
          var objLabel = {label}
          arrayProcedimentos.push(objLabel)
        })
        this.api.procedimentoSigtapHistorico = arrayProcedimentos;
      },

      findProcedimentos(string) {
        var stringRetorno = ""
        this.api.procedimentos.forEach(item => {
          if (item.codigo === string) {
            stringRetorno = item.descricao;
          } else {
            return null; // Caso a string não seja encontrada
          }
        })
        var stringFormatada = this.formatarString(stringRetorno)
        return stringFormatada
      },

      formatarString(string) {
        var resultado = string.toLowerCase().replace(/(^|\s)\S/g, (letter) => {
          return letter.toUpperCase();
        });
        return resultado
      },



      async getCiapHistorico2(row) {
        this.loading.ciapCid = true
        this.params.agendamentoId = row.agendamentoId
        let { data, paginacao, status } = await _api.individuoCiap.get(this.params)
        if (status === 502) this.loading.ciapCid = false

        let dataFiltered = data.map(item => {
          return item.ciap
        })
        this.api.ciapHistorico = (status === 200) ? dataFiltered : []

        this.params.skip = (status === 200) ? paginacao.skip : 0
        this.params.total = (status === 200) ? paginacao.totalCount : 0
        this.loading.ciapCid = false
      },

      async getCid10Historico2(row) {
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

      onVoltar(form) {
        this.getHistorico()
        //let i = this.api.historico.findIndex(x => x.id === this.formAtendimento.id)
        //this.$refs.tabela.setCurrentRow(this.api.historico[i])
        this.listando = true
        this.exibirDadosHeaderHistorico = false
      },
      async getDocumentos(row) {
        this.params.agendamentoId = row.agendamentoId
        let { data, paginacao, status } = await _api.documentos.get(this.params)
        this.api.documentos = (status === 200) ? data : []
      },
   


      // PEGANDO SINAIS VITAIS (HISTORICO)
      async getSinaisVitaisHistorico(row) {

        this.api.sinaisVitaisHistorico = {
          dataAlteracao: row.agendamento.dataAlteracao,
          pressaoArterial: row.agendamento.pressaoSanguinea,
          frequenciaCardiaca: row.agendamento.batimentoCardiaco,
          temperatura: row.agendamento.temperatura,
          saturacaoO2: row.agendamento.oxigenacaoSanguinea,
          peso: row.agendamento.peso,
          altura: row.agendamento.altura
        }

        // IMC HISTORICO NÃO ALTERAR
        var imcDados = (this.api.sinaisVitaisHistorico.peso / ((this.api.sinaisVitaisHistorico.altura / 100) * (this.api.sinaisVitaisHistorico.altura / 100)))
        var imcFloatDados = parseFloat(imcDados).toFixed(1)

        this.api.sinaisVitaisHistorico.dadosImc = imcFloatDados

        if (imcFloatDados >= 16 & imcFloatDados <= 16.9) {
          this.api.sinaisVitaisHistorico.statusImc = 'Muito abaixo do Peso'
        }
        if (imcFloatDados >= 17 & imcFloatDados <= 18.4) {
          this.api.sinaisVitaisHistorico.statusImc = 'Abaixo do Peso'
        }
        if (imcFloatDados >= 18.5 & imcFloatDados <= 24.9) {
          this.api.sinaisVitaisHistorico.statusImc = 'Peso Normal'
        }
        if (imcFloatDados >= 25 & imcFloatDados <= 29.9) {
          this.api.sinaisVitaisHistorico.statusImc = 'Acima do Peso'
        }
        if (imcFloatDados >= 30 & imcFloatDados <= 34.9) {
          this.api.sinaisVitaisHistorico.statusImc = 'Obesidade Grau I'
        }
        if (imcFloatDados >= 35 & imcFloatDados <= 40) {
          this.api.sinaisVitaisHistorico.statusImc = 'Obesidade Grau II'
        }
        if (imcFloatDados > 40) {
          this.api.sinaisVitaisHistorico.statusImc = 'Obesidade Grau III'
        }

        this.sinaisHistorico = this.api.sinaisVitaisHistorico
      },


      async getComoMeSintoHistorico(row) {
        if (row != null) {
          this.paramsAcompanhamentoHistorico.individuoId = row.individuo.id
          let { data } = await _api.acompanhamento.get(this.paramsAcompanhamentoHistorico)
          let comoMeSintoHistorico = (data !== null) ? data : {}
          //console.log('data comoMeSintoHistorico', data)
          this.api.comoMeSintoHistorico = comoMeSintoHistorico.slice(-1)
          //console.log('comoMeSintoHistorico', this.api.comoMeSintoHistorico)

          // this.paramsAcompanhamento.individuoId = this.agendamento.individuoId
          // let { data } = await _api.acompanhamento.get(this.paramsAcompanhamento)
          // let comoMeSinto = (data !== null) ? data : {}
          // this.api.comoMeSinto = comoMeSinto.slice(-1)
        }
      },

      async getComoMeSintoAtendimentoHistorico(row) {
        /*console.log('this.agendamento.id', row.agendamentoId)*/
        if (row.agendamentoId != null) {
          this.paramsAcompanhamento.individuoId = this.agendamento.individuoId
          this.paramsAcompanhamento.atendimentoId = row.id
          let { data } = await _api.acompanhamento.get(this.paramsAcompanhamento)
          this.api.comoMeSintoAtendimentoHistorico = (data !== null) ? data : []
        }
      },


      /*EXAMES HISTORICOS*/

      async getExamesHistorico() {
        this.paramsExames.finalizado = true
        let { data, status, paginacao } = await _api.exames.getExames(this.paramsExames)
        var arrExame = (status === 200) ? data : []
        this.paramsExames.skip = (status === 200) ? paginacao.skip : 0
        this.paramsExames.total = (status === 200) ? paginacao.totalCount : 0

        let individuoId = this.api.individuo.id
        //console.log("individuoId", individuoId)
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
        //console.log('this.api.examesHistorico', this.api.examesHistorico)
      },

      getTipoExamesHistorico(val) {
        let tipo = this.enums.tipoExames.filter(x => x.value == val)[0]
        return tipo.label

      },

      async clickVisualizarHistorico(val) {
        //console.log(val)
        this.formVisualizar = val
        this.statusVisualizar = true
      },

      async visualizarPdfHistorico(val) {
        var pdfBase64 = val.pdfExame
        this.debugBase64Historico(pdfBase64)
      },
      async debugBase64Historico(base64URL) {
        var win = window.open();
        win.document.write('<iframe src="' + base64URL + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe>');
      },





      async hideVisualizar() {
        this.statusVisualizar = false
        this.statusEditar = false
      },

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

  .divButtonExame {
    display: flex;
    flex-direction: row;
    justify-content: flex-end;
  }
  /*.el-button:focus {
    background-color: #e6f3f0
  }
  .el-button:hover {
    color: #01826c;
    background-color: #e6f3f0;
  }*/
</style>
