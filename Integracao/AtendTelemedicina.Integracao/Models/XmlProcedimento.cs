using AtendTelemedicina.Integracao.Models.Helpers;
using AtendTelemedicina.Integracao.Models.Interfaces;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace AtendTelemedicina.Integracao.Models
{
    public class XmlProcedimento : XmlColetaBase, IXmlCadastro
    {
        #region Propriedades

        public XNamespace ns4 { get; set; }
        public int tipoDadoSerializado { get; set; }

        public bool? statusSituacaoRua { get; set; }
        #endregion

        #region FichaAtualizada

        public bool fichaAtualizada { get; set; }

        #endregion

        #region IdentificacaoAtendimento
        public string numProtuario { get; set; }
        public string cnsCidadao { get; set; }
        public string cpfCidadao { get; set; }
        public DateTime dtNascimento { get; set; }
        public int? sexo { get; set; }
        public int? localAtendimento { get; set; }
        public bool atendimentoTriagem { get; set; }
        public bool atendimentoMedico { get; set; }
        public int? turno { get; set; }
        public bool statusEscutaInicialOrientacao { get; set; }
        public DateTime dataHoraInicialAtendimento { get; set; }
        public DateTime dataHoraFinalAtendimento { get; set; }
        public string pesoAcompanhamentoNutricional { get; set; }
        public string alturaAcompanhamentoNutricional { get; set; }
        public int? numTotalAfericaoPa { get; set; }
        public int? numTotalGlicemiaCapilar { get; set; }
        public int? numTotalAfericaoTemperatura { get; set; }
        public int? numTotalMedicaoAltura { get; set; }
        public int? numTotalCurativoSimples { get; set; }
        public int? numTotalMedicaoPeso { get; set; }
        public int? numTotalColetaMaterialParaExameLaboratorial { get; set; }
        private ICollection<IndividuoCid10> listaCid10 { get; set; }
        private ICollection<IndividuoCiap> listaCiap { get; set; }
        private ICollection<IndividuoProcedimento> listaProcedimentos { get; set; }
        private ICollection<ExamesF200> listaExamesF200 { get; set; }
        #endregion

        #region TermoRecusa

        public bool statusTermoRecusaCadastroIndividualAtencaoBasica { get; set; }

        #endregion

        #region TipoCDSOrigem

        private object tpCdsOrigem { get { return 3; } }

        #endregion

        #region UUID
        public string uuid { get; set; }

        #endregion

        #region SaidaCidadao
        public int? motivoSaidaCidadao { get; set; }
        public DateTime? dataObito { get; set; }
        public string numeroDO { get; set; }

        #endregion

        public XmlProcedimento()
        {
            ns4 = "http://esus.ufsc.br/fichaprocedimentomaster";
            tipoDadoSerializado = 7;
            listaCid10 = new HashSet<IndividuoCid10>();
            listaCiap = new HashSet<IndividuoCiap>();
            listaProcedimentos = new HashSet<IndividuoProcedimento>();
            listaExamesF200 = new HashSet<ExamesF200>();
        }

        public XElement GetColetaElement()
        {
            var element = new XElement(ns4 + "fichaProcedimentoMasterTransport",
                GetHeaderTransport(),
                GetAtendProcedimentosElement(),
                new XElement("uuidFicha", uuid),
                new XElement("tpCdsOrigem", tpCdsOrigem),
                GetMedicoes()
                );
            return element;
        }

        private XElement GetAtendProcedimentosElement()
        {
            // TODO
            // 1 - Adicionar campos dataInicio, DataFim (usar lógica do v4)
            return new XElement("atendProcedimentos",
                new XElement("numProntuario", numProtuario),
                !string.IsNullOrEmpty(cpfCidadao) ? new XElement("cpfCidadao", cpfCidadao) : new XElement("cnsCidadao", cnsCidadao),
                new XElement("dtNascimento", dtNascimento.DateTimeParaUnixTime()),
                new XElement("sexo", sexo),
                new XElement("localAtendimento", localAtendimento),
                new XElement("turno", turno),
                new XElement("statusEscutaInicialOrientacao", statusEscutaInicialOrientacao),
                GetProcedimentos(),
                new XElement("dataHoraInicialAtendimento", new DateTimeOffset(dataHoraInicialAtendimento).ToUnixTimeMilliseconds()),
                new XElement("dataHoraFinalAtendimento", new DateTimeOffset(dataHoraFinalAtendimento).ToUnixTimeMilliseconds()),
                pesoAcompanhamentoNutricional != null ? new XElement("pesoAcompanhamentoNutricional", pesoAcompanhamentoNutricional) : null,
                alturaAcompanhamentoNutricional != null ? new XElement("alturaAcompanhamentoNutricional", alturaAcompanhamentoNutricional) : null);
        }

        private IEnumerable<XElement> GetProcedimentos()
        {
            var xElements = new List<XElement>();

            if (atendimentoMedico)
                xElements.Add(new XElement("procedimentos", "0301010072"));
            if (atendimentoTriagem)
                xElements.Add(new XElement("procedimentos", "0301010048"));
            if (numTotalAfericaoTemperatura > 0)
                xElements.Add(new XElement("procedimentos", "0301100250"));
            if (numTotalMedicaoAltura > 0)
                xElements.Add(new XElement("procedimentos", "0101040075"));
            if (numTotalMedicaoPeso > 0)
                xElements.Add(new XElement("procedimentos", "0101040083"));

            if (!listaProcedimentos.Any())
                return xElements;

            if (listaProcedimentos.Select(x => x.ProcedimentoCodigo).Any())
            {
                foreach (var codProcedimento in listaProcedimentos.Select(x => x.ProcedimentoCodigo))
                {
                    xElements.Add(new XElement("procedimentos", codProcedimento));
                }
            }

            return xElements;
        }

        private IEnumerable<XElement> GetMedicoes()
        {
            var xElements = new List<XElement>();

            if (numTotalAfericaoPa > 0)
                xElements.Add(new XElement("numTotalAfericaoPa", numTotalAfericaoPa));
            if (numTotalAfericaoTemperatura > 0)
                xElements.Add(new XElement("numTotalAfericaoTemperatura", numTotalAfericaoTemperatura));
            if (numTotalColetaMaterialParaExameLaboratorial > 0)
                xElements.Add(new XElement("numTotalColetaMaterialParaExameLaboratorial", numTotalColetaMaterialParaExameLaboratorial));
            if (numTotalMedicaoAltura > 0)
                xElements.Add(new XElement("numTotalMedicaoAltura", numTotalMedicaoAltura));
            if (numTotalMedicaoPeso > 0)
                xElements.Add(new XElement("numTotalMedicaoPeso", numTotalMedicaoPeso));

            return xElements;
        }

        public XElement GetHeaderTransport()
        {
            return new XElement("headerTransport",
                new XElement("profissionalCNS", profissionalCNS?.PadLeft(15, '0')),
                new XElement("cboCodigo_2002", cboCodigo_2002),
                new XElement("cnes", cnes?.PadLeft(7, '0')),
                new XElement("ine", ine?.PadLeft(10, '0')),
                new XElement("dataAtendimento", $"{new DateTimeOffset(dataAtendimento).ToUnixTimeMilliseconds()}"),
                new XElement("codigoIbgeMunicipio", codigoIbgeMunicipio)
                );
        }

        public void SetDadosInstalacao(string contraChave, string uuidInstalacao, string cpfOuCnpj,
             string nomeOuRazaoSocial, string versaoSistema = null, string nomeBancoDados = null, string fone = null,
             string email = null)
        {
            ContraChave = contraChave;
            UuidInstalacao = uuidInstalacao;
            CpfOuCnpj = cpfOuCnpj;
            NomeOuRazaoSocial = nomeOuRazaoSocial;
            VersaoSistema = versaoSistema;
            NomeBancoDados = nomeBancoDados;
        }

        public override void PopularXml(Guid id, int lote, object entity, string cpfResponsavel, string nomeResponsavel)
        {
            var atendimento = entity as Atendimento;

            if (atendimento == null) return;
            Validar(atendimento);

            uuidDadoSerializado = $"{id}";
            codigoIbgeMunicipio = atendimento.Agendamento.Estabelecimento.CodIbgeMun;
            cnesDadoSerializado = atendimento.Agendamento.Estabelecimento.Cnes;
            ineDadoSerializado = atendimento.Agendamento.Estabelecimento.CodIne;
            numLote = $"{lote}";

            profissionalCNS = atendimento.Usuario.Cns;
            cboCodigo_2002 = atendimento.Usuario.Ocupacao.Codigo;

            numProtuario = atendimento.Agendamento.NumProntuario;
            cnes = atendimento.Agendamento.Estabelecimento.Cnes;
            ine = atendimento.Agendamento.Estabelecimento.CodIne;
            cpfCidadao = atendimento.Individuo.Cpf;
            cnsCidadao = atendimento.Individuo.Cns;
            dtNascimento = atendimento.Individuo.DataNascimento.AddHours(12);
            sexo = atendimento.Individuo.Sexo;
            dataAtendimento = atendimento.DataAlteracao;
            atendimentoMedico = atendimento.AtendidoMedico;
            atendimentoTriagem = atendimento.AtendidoTriagem;
            localAtendimento = (int)atendimento.LocalDeAtendimento;
            codigoIbgeMunicipio = atendimento.Agendamento.Estabelecimento.CodIbgeMun.ToString();
            statusEscutaInicialOrientacao = atendimento.AtendidoTriagem;

            dataHoraInicialAtendimento = dataAtendimento.AddMinutes(10);
            dataHoraFinalAtendimento = atendimento.DataAlteracao.AddMinutes(10);

            turno = GetTurno(dataAtendimento);

            pesoAcompanhamentoNutricional = atendimento.Agendamento.Peso;
            alturaAcompanhamentoNutricional = atendimento.Agendamento.Altura;

            listaCid10 = atendimento.IndividuoCid10.ToList();
            ValidarCid10(listaCid10);

            listaCiap = atendimento.IndividuoCiap.ToList();
            ValidarCiap(listaCiap);

            listaProcedimentos = atendimento.IndividuoProcedimentos.ToList();
            ValidarProcedimentos(listaProcedimentos);


            numTotalAfericaoPa = !string.IsNullOrEmpty(atendimento.Agendamento.PressaoSanguinea) ? 1 : 0;
            numTotalAfericaoTemperatura = !string.IsNullOrEmpty(atendimento.Agendamento.Temperatura) ? 1 : 0;
            numTotalMedicaoAltura = !string.IsNullOrEmpty(atendimento.Agendamento.Altura) ? 1 : 0;
            numTotalAfericaoPa = !string.IsNullOrEmpty(atendimento.Agendamento.PressaoSanguinea) ? 1 : 0;
            numTotalMedicaoPeso = !string.IsNullOrEmpty(atendimento.Agendamento.Peso) ? 1 : 0;
            listaExamesF200 = atendimento.Agendamento.ExamesF200;
            numTotalColetaMaterialParaExameLaboratorial = GetQuantidadeExames();

            uuid = atendimento.Agendamento.DataCadastro == atendimento.Agendamento.DataAlteracao ? $"{cnes}-{atendimento.AgendamentoId}" : $"{cnes}-{Guid.NewGuid()}";
            fichaAtualizada = atendimento.Agendamento.DataCadastro != atendimento.Agendamento.DataAlteracao;

            statusTermoRecusaCadastroIndividualAtencaoBasica = false;

            SetDadosInstalacao("Meeds", Guid.NewGuid().ToString(), cpfResponsavel, nomeResponsavel, "2.0.5", "MySql");
        }

        private int GetTurno(DateTime horaAtendimento)
        {
            int turno;

            if (horaAtendimento.Hour >= 5 && horaAtendimento.Hour < 12)
            {
                turno = 1;
            }
            else if (horaAtendimento.Hour >= 12 && horaAtendimento.Hour < 18)
            {
                turno = 2;
            }
            else
            {
                turno = 3;
            };
            return turno;
        }

        private int GetQuantidadeExames()
        {
            int totalF200 = listaExamesF200.Count();
            int totalAfynion = 0;
            return totalF200 + totalAfynion;
        }

        private void Validar(Atendimento atendimento)
        {
            if (atendimento.Individuo == null)
                throw new Exception("Paciente não identificado.");
            if (atendimento.Usuario == null)
                throw new Exception("Profissional que realizaou o atendimento não identificado");
            if (string.IsNullOrEmpty(atendimento.Usuario.Cns))
                    throw new Exception("CNS do profissional que realizaou o atendimento é obrigatório.");
            if ((int)atendimento.LocalDeAtendimento < 1 && (int)atendimento.LocalDeAtendimento > 10)
                throw new Exception("Local De Atendimento possui um valor inválido. Apenas valores de 1 a 10");
            if (GetTurno(atendimento.DataAlteracao) == 0)
                throw new Exception("Código do turno onde aconteceu o atendimento é obrigatório.");
            if (string.IsNullOrEmpty(atendimento.Agendamento.Estabelecimento.Cnes))
                throw new Exception("CNES do Estabelecimento não preencido.");
            if (string.IsNullOrEmpty(atendimento.Agendamento.Estabelecimento.CodIne))
                throw new Exception("Código INE do Estabelecimento não preencido.");
        }

        private static void ValidarCid10(ICollection<IndividuoCid10> lista)
        {
            if (lista.Any(item => item.Cid.Codigo == "0301040079"))
            {
                throw new Exception("Não pode ser preenchido com o procedimento '03.01.04.007-9' - Escuta inicial / Orientação (acolhimento a demanda espontânea). Esta informação deve ser registrada através do campo statusEscutaInicialOrientacao");
            }
        }

        private static void ValidarCiap(ICollection<IndividuoCiap> lista)
        {
            if (lista.Any(item => item.Ciap.Codigo == "0301040079"))
            {
                throw new Exception("Não pode ser preenchido com o procedimento '03.01.04.007-9' - Escuta inicial / Orientação (acolhimento a demanda espontânea). Esta informação deve ser registrada através do campo statusEscutaInicialOrientacao");
            }
        }

        private static void ValidarProcedimentos(ICollection<IndividuoProcedimento> lista)
        {
            if (lista.Count != lista.Distinct().Count())
                throw new Exception("Lista de procedimentos não pode conter procedimentos repetidos");
            foreach (var item in lista)
            {
                if (item.Procedimento.Grupo != 1 && item.Procedimento.Grupo != 2 && item.Procedimento.Grupo != 3 && item.Procedimento.Grupo != 4)
                    throw new Exception("Só podem ser informados os procedimentos pertencentes aos grupos 01 - Ações de promoção e prevenção em saúde, 02 - Procedimentos com finalidade diagnóstica, 03 - Procedimentos clínicos, 04 - Procedimentos cirúrgicos");
            }
        }

    }
}
