using AtendTelemedicina.Integracao.Models.Helpers;
using AtendTelemedicina.Integracao.Models.Interfaces;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace AtendTelemedicina.Integracao.Models
{
    public class XmlFichaAtendimentoDomiciliar : XmlColetaBase, IXmlCadastro
    {
        #region Propriedades

        public XNamespace ns4 { get; set; }
        public int tipoDadoSerializado { get; set; }

        public bool? statusSituacaoRua { get; set; }
        #endregion

        #region FichaAtualizada

        public bool fichaAtualizada { get; set; }

        #endregion

        #region Identificacao Atendimento
        public string cnsCidadao { get; set; }
        public string cpfCidadao { get; set; }
        public DateTime dtNascimento { get; set; }
        public int? sexo { get; set; }
        public int? localAtendimento { get; set; }
        public string atencaoDomiciliarModalidade { get; set; }
        public int? tipoDeAtendimento { get; set; }
        public int? turno { get; set; }
        public string condicoesAvaliadas { get; set; }
        public string cid { get; set; }
        public string ciap { get; set; }
        public string condutaDesfecho { get; set; }
        private ICollection<IndividuoCid10> listaCid10 { get; set; }
        private ICollection<IndividuoCiap> listaCiap { get; set; }
        private ICollection<IndividuoProcedimento> listaProcedimentos { get; set; }
        private ICollection<ExamesF200> listaExamesF200 { get; set; }
        #endregion

        #region TipoCDSOrigem

        private object tpCdsOrigem { get { return 3; } }

        #endregion

        #region UUIDFichaOriginadora
        public string uuidFichaOriginadora { get; set; }
        #endregion

        #region UUID
        public string uuid { get; set; }

        #endregion

        public XmlFichaAtendimentoDomiciliar()
        {
            ns4 = "http://esus.ufsc.br/fichaatendimentodomiciliarmaster";
            tipoDadoSerializado = 10;
            listaCid10 = new HashSet<IndividuoCid10>();
            listaCiap = new HashSet<IndividuoCiap>();
            listaProcedimentos = new HashSet<IndividuoProcedimento>();
            listaExamesF200 = new HashSet<ExamesF200>();
        }

        public XElement GetColetaElement()
        {
            var element = new XElement(ns4 + "fichaAtendimentoDomiciliarMasterTransport",
                new XElement("uuidFicha", uuid),
                new XElement("tpCdsOrigem", tpCdsOrigem),
                GetHeaderTransport(),
                GetAtendimentosDomiciliaresElement()
                );
            return element;
        }

        private XElement GetAtendimentosDomiciliaresElement()
        {
            return new XElement("atendimentosDomiciliares",
                new XElement("turno", turno),
                !string.IsNullOrEmpty(cpfCidadao) ? new XElement("cpfCidadao", cpfCidadao) : new XElement("cnsCidadao", cnsCidadao),
                new XElement("dataNascimento", dtNascimento.DateTimeParaUnixTime()),
                new XElement("sexo", sexo),
                new XElement("localAtendimento", localAtendimento),
                new XElement("atencaoDomiciliarModalidade", atencaoDomiciliarModalidade),
                new XElement("tipoAtendimento", tipoDeAtendimento),
                GetProblemaCondicaoAvaliada(condicoesAvaliadas),
                listaCid10.Any() ? new XElement("cid", cid) : null,
                listaCiap.Any() ? new XElement("ciap", ciap) : null,
                GetProcedimentos(),
                new XElement("condutaDesfecho", condutaDesfecho));
        }

        private IEnumerable<XElement> GetProblemaCondicaoAvaliada(string condicoes)
        {
            var xElements = new List<XElement>();

            var listCondicoes = condicoes.Split(',');

            foreach (var condicao in listCondicoes)
            {
                xElements.Add(new XElement("condicoesAvaliadas", condicao));
            }

            return xElements;
        }

        private IEnumerable<XElement> GetProcedimentos()
        {

            if (!listaProcedimentos.Any())
                return null;

            var xElements = new List<XElement>();

            if (listaProcedimentos.Select(x => x.ProcedimentoCodigo).Any())
            {
                foreach (var codProcedimento in listaProcedimentos.Select(x => x.ProcedimentoCodigo))
                {
                        xElements.Add(new XElement("procedimentos", codProcedimento));
                }
            }

            return xElements;
        }

        public XElement GetHeaderTransport()
        {
            return new XElement("headerTransport",
                    new XElement("lotacaoFormPrincipal",
                    new XElement("profissionalCNS", profissionalCNS?.PadLeft(15, '0')),
                    new XElement("cboCodigo_2002", cboCodigo_2002),
                    new XElement("cnes", cnes?.PadLeft(7, '0')),
                    new XElement("ine", ine?.PadLeft(10, '0'))),
                new XElement("dataAtendimento", $"{new DateTimeOffset(dataAtendimento).ToUnixTimeMilliseconds()}"),
                new XElement("codigoIbgeMunicipio", codigoIbgeMunicipio));
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

            profissionalCNS = atendimento.Agendamento.Profissional.Cns;
            cboCodigo_2002 = atendimento.Agendamento.Profissional.Ocupacao.Codigo;
            cnes = atendimento.Agendamento.Estabelecimento.Cnes;
            ine = atendimento.Agendamento.Estabelecimento.CodIne;
            dataAtendimento = atendimento.DataAlteracao.AddMinutes(-10);

            cpfCidadao = atendimento.Individuo.Cpf;
            cnsCidadao = atendimento.Individuo.Cns;
            dtNascimento = atendimento.Individuo.DataNascimento.AddHours(12);
            sexo = atendimento.Individuo.Sexo;
            localAtendimento = (int)atendimento.LocalDeAtendimento;
            atencaoDomiciliarModalidade = atendimento.ModalidadeAD;
            tipoDeAtendimento = atendimento.AtendidoMedico ? int.Parse(atendimento.TipoAtendimento) : (int)TipoDeAtendimento.ESCUTA_INICIAL_ORIENTACAO;
            turno = GetTurno(dataAtendimento);
            condicoesAvaliadas = atendimento.CondicoesAvaliadas;
            condutaDesfecho = atendimento.CondutaDesfecho;

            listaCid10 = atendimento.IndividuoCid10.ToList();
            ValidarCid10(listaCid10, atendimento.Individuo);
            listaCiap = atendimento.IndividuoCiap.ToList();
            ValidarCiap(listaCiap);
            listaProcedimentos = atendimento.IndividuoProcedimentos.ToList();
            ValidarProcedimentos(listaProcedimentos);

            cid = listaCid10.Any() ? listaCid10.FirstOrDefault().Cid.Codigo : null;
            ciap = listaCiap.Any() ? listaCiap.FirstOrDefault().Ciap.Codigo : null;

            listaExamesF200 = atendimento.Agendamento.ExamesF200;

            uuid = atendimento.DataCadastro == atendimento.DataAlteracao ? $"{cnes}-{atendimento.Id}" : $"{cnes}-{Guid.NewGuid()}";
            uuidFichaOriginadora = $"{cnes}-{atendimento.Id}";
            fichaAtualizada = atendimento.DataCadastro != atendimento.DataAlteracao;

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

        private void Validar(Atendimento atendimento)
        {
            if (atendimento.Individuo == null)
                throw new Exception("Paciente não identificado.");
            if (atendimento.Agendamento.Profissional == null)
                throw new Exception("Profissional que realizaou o atendimento não identificado");
            if (string.IsNullOrEmpty(atendimento.Agendamento.Profissional.Cns))
                throw new Exception("CNS do profissional que realizaou o atendimento é obrigatório.");
            if ((int)atendimento.LocalDeAtendimento < 1 || (int)atendimento.LocalDeAtendimento > 13)
                    throw new Exception("Local De Atendimento possui um valor inválido. Apenas valores de 1 a 13");
            if (GetTurno(atendimento.DataAlteracao) == 0)
                throw new Exception("Código do turno onde aconteceu o atendimento é obrigatório.");

            if (!string.IsNullOrEmpty(atendimento.ModalidadeAD) && ((!string.IsNullOrEmpty(atendimento.TipoAtendimento)) && int.Parse(atendimento.TipoAtendimento) == (int)TipoDeAtendimento.VISITA_DOMICILIAR_POS_OBITO))
                throw new Exception("O campo ModalidadeAD não pode ser preenchido se o campo TipoAtendimento = 9: Visita domiciliar pós-óbito.");

            if (!string.IsNullOrEmpty(atendimento.ModalidadeAD)) {
                int atencaoDomiciliarModalidade = int.Parse(atendimento.ModalidadeAD);
                if (atencaoDomiciliarModalidade != 1 && atencaoDomiciliarModalidade != 2 && atencaoDomiciliarModalidade != 3)
                    throw new Exception("O campo ModalidadeAD aceita apenas as opções 1, 2 e 3");
            }

            if (!string.IsNullOrEmpty(atendimento.CondicoesAvaliadas) && ((!string.IsNullOrEmpty(atendimento.TipoAtendimento)) && int.Parse(atendimento.TipoAtendimento) == (int)TipoDeAtendimento.VISITA_DOMICILIAR_POS_OBITO))
            {
                throw new Exception("O campo CondicoesAvaliadas não pode ser preenchido se o campo TipoAtendimento = 9: Visita domiciliar pós-óbito.");
            }

            if (listaProcedimentos.Any() && ((!string.IsNullOrEmpty(atendimento.TipoAtendimento)) && int.Parse(atendimento.TipoAtendimento) == (int)TipoDeAtendimento.VISITA_DOMICILIAR_POS_OBITO))
            {
                throw new Exception("Código dos procedimentos do atendimento não pode ser preenchido se o campo TipoAtendimento = 9: Visita domiciliar pós-óbito.");
            }

            if (string.IsNullOrEmpty(atendimento.CondutaDesfecho))
                throw new Exception("Código do desfecho do atendimento do cidadão é obrigatório.");

            if (string.IsNullOrEmpty(atendimento.Agendamento.Estabelecimento.Cnes))
                throw new Exception("CNES do Estabelecimento não preencido.");

            if (string.IsNullOrEmpty(atendimento.Agendamento.Estabelecimento.CodIne))
                throw new Exception("Código INE do Estabelecimento não preencido.");

        }

        private static void ValidarCid10(ICollection<IndividuoCid10> lista, Individuo individuo)
        {
            if (individuo.Sexo == 0) {
                if (lista.Any(item => item.Cid.Sexo == "F"))
                    throw new Exception("Para esta ficha só devem ser apresentadas CID-10 permitidas para o sexo Masculino.");
            }
            if (individuo.Sexo == 1)
            {
                if (lista.Any(item => item.Cid.Sexo == "M"))
                    throw new Exception("Para esta ficha só devem ser apresentadas CID-10 permitidas para o sexo Feminino.");
            }

            if (lista.Any(item => item.Cid.Codigo == "0301040079"))
                throw new Exception("Não pode ser preenchido com o procedimento '03.01.04.007-9' - Escuta inicial / Orientação (acolhimento a demanda espontânea). Esta informação deve ser registrada através do campo statusEscutaInicialOrientacao");
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

            if (lista.Any(item => item.Procedimento.Codigo == "0301050104"))
            {
                throw new Exception("Não pode ser preenchido com o procedimento 03.01.05.010-4 - Visita domiciliar pós-óbito. Esta informação deve ser registrada através do campo tipoAtendimento, opção 9 - Visita domiciliar pós-óbito");
            }
        }

        public bool ValidarNome(string nome)
        {
            if (string.IsNullOrEmpty(nome)) return true;

            if (nome != null)
                while (nome.IndexOf("  ") >= 0)
                    nome = nome.Replace("  ", " ");

            //if (NomeCompleto.Length > 100 || NomeCompleto.Length < 5)
            if (nome.Length > 70 || nome.Length < 3)
                throw new Exception("O campo nome deve possuir no máximo 70 caracteres e no mínimo 3 caracteres.");

            var nomeAuxiliar = nome.Trim().Split(' ');

            //Verifica se o nome possui apenas o primeiro nome
            if (nomeAuxiliar.Length < 2)
                throw new Exception("O nome inserido não possui sobrenome.");

            //Verifica se o nome possui algum número
            if (nome.Any(char.IsDigit))
                throw new Exception("No campo nome não pode conter número.");

            if (nomeAuxiliar[0].Length == 1)
                throw new Exception("No campo nome, o primeiro nome não pode ser apenas uma letra.");

            //Verifica se o nome e sobrenome possui apenas 2 carateres em ambos
            if (nomeAuxiliar.Length == 2)
                if (nomeAuxiliar[0].Length == 2 && nomeAuxiliar[1].Length == 2)
                    throw new Exception("O nome e sobrenome não pode conter apenas 2 caracteres em ambos.");

            //Verifica se o primeiro e segundo termos possuem apenas um caractere em cada um deles
            if (nomeAuxiliar[0].Length == 1 && nomeAuxiliar[1].Length == 1)
                throw new Exception("O primeiro e segundo termo do nome, não pode possuir apenas 1 caractere.");

            //Verifica se o caractere isolado a partir do primeiro termo do nome possui é diferente de E e Y
            for (int i = 1; i < nomeAuxiliar.Length; i++)
            {
                if (nomeAuxiliar[i].Length == 1)
                    if (nomeAuxiliar[i] != "E" && nomeAuxiliar[i] != "Y")
                        throw new Exception("O caractere isolado do nome é inválido, só é aceitável caso seja E ou Y, Exemplo: JOAQUIM MARIA A SILVA => ERRADO! "
                                    + "| JOAQUIM MARIA E SILVA => CORRETO!");
            }
            return true;
        }
    }
}
