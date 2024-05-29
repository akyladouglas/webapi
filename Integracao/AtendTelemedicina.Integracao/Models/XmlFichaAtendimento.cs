using AtendTelemedicina.Integracao.Models.Helpers;
using AtendTelemedicina.Integracao.Models.Interfaces;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Enums;
using AtendTeleMedicina.Domain.Services.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace AtendTelemedicina.Integracao.Models
{
    public class XmlFichaAtendimento : XmlColetaBase, IXmlCadastro
    {
        #region Propriedades

        public XNamespace ns4 { get; set; }
        public int tipoDadoSerializado { get; set; }

        public bool? statusSituacaoRua { get; set; }
        #endregion

        #region FichaAtualizada

        public bool fichaAtualizada { get; set; }

        #endregion

        #region Identificacao Agendamento
        public string numProntuario { get; set; }
        public string cnsCidadao { get; set; }
        public string cpfCidadao { get; set; }
        public DateTime dtNascimento { get; set; }
        public int? sexo { get; set; }
        public int? localAtendimento { get; set; }
        public bool? atendimentoTriagem { get; set; }
        public bool? atendimentoMedico { get; set; }
        public int? turno { get; set; }
        public int? tipoDeAtendimento { get; set; }
        public DateTime dataHoraInicialAtendimento { get; set; }
        public DateTime dataHoraFinalAtendimento { get; set; }
        public DateTime dumDaGestante { get; set; }
        public int? idadeGestacional { get; set; }
        public string pesoAcompanhamentoNutricional { get; set; }
        public string alturaAcompanhamentoNutricional { get; set; }
        public int? aleitamentoMaterno { get; set; }
        public string condicoesAvaliadas { get; set; }
        public string condutas { get; set; }
        private ICollection<IndividuoCid10> listaCid10 { get; set; }
        private ICollection<IndividuoCiap> listaCiap { get; set; }
        private ICollection<IndividuoProcedimento> listaProcedimentos { get; set; }
        private ICollection<Atendimento> listaAtendimentos { get; set; }
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

        public XmlFichaAtendimento()
        {
            ns4 = "http://esus.ufsc.br/fichaatendimentoindividualmaster";
            tipoDadoSerializado = 4;
            listaCid10 = new HashSet<IndividuoCid10>();
            listaCiap = new HashSet<IndividuoCiap>();
            listaProcedimentos = new HashSet<IndividuoProcedimento>();
            listaExamesF200 = new HashSet<ExamesF200>();
        }

        public XElement GetColetaElement()
        {
            var element = new XElement(ns4 + "fichaAtendimentoIndividualMasterTransport",
                GetHeaderTransport(),
                GetAtendimentosIndividuaisElement(),
                new XElement("tpCdsOrigem", tpCdsOrigem),
                new XElement("uuidFicha", uuid)

                );
            return element;
        }

        private XElement GetAtendimentosIndividuaisElement()
        {
            return new XElement("atendimentosIndividuais",
                new XElement("numeroProntuario", numProntuario),
                !string.IsNullOrEmpty(cpfCidadao) ? new XElement("cpfCidadao", cpfCidadao) : new XElement("cnsCidadao", cnsCidadao),
                new XElement("dataNascimento", dtNascimento.DateTimeParaUnixTime()),
                new XElement("localDeAtendimento", localAtendimento),
                new XElement("sexo", sexo),
                new XElement("turno", turno),
                new XElement("tipoAtendimento", tipoDeAtendimento),
                pesoAcompanhamentoNutricional != null ? new XElement("pesoAcompanhamentoNutricional", pesoAcompanhamentoNutricional) : null,
                alturaAcompanhamentoNutricional != null ? new XElement("alturaAcompanhamentoNutricional", alturaAcompanhamentoNutricional) : null,
                new XElement("problemaCondicaoAvaliada", GetProblemaCondicaoAvaliada()),
                GetCondutasElements(),
                new XElement("dataHoraInicialAtendimento", new DateTimeOffset(dataHoraInicialAtendimento).ToUnixTimeMilliseconds()),
                new XElement("dataHoraFinalAtendimento", new DateTimeOffset(dataHoraFinalAtendimento).ToUnixTimeMilliseconds()));
        }

        private IEnumerable<XElement> GetProblemaCondicaoAvaliada()
        {
            var xElements = new List<XElement>();

            if (!string.IsNullOrEmpty(condicoesAvaliadas))
            {
                var listCondicoes = condicoesAvaliadas.Split(',');
                foreach (var ciap in listCondicoes)
                {
                    xElements.Add(new XElement("ciaps", ciap));
                }
            }

            // Removendo CIAPS da lista de condicoes avaliadas
            var listaCiapCondicaoAvaliada = new List<string>
            { "ABP018","ABP024","ABP004","ABP009","ABP019","ABP005","ABP008","ABP006","ABP023","ABP003","ABP017","ABP010","ABP012","ABP022","ABP002","ABP020","ABP014","ABP015","ABP011","ABP001","ABP013","ABP007",
              "R96","A77","T91","T90","R95","A78","K86","T82","W78","A98","W18","K22","57","P17","A70","P16","P19"};

            var listaCiapCodigos = new List<string>();

            foreach (var item in listaCiap)
            {
                if (!listaCiapCondicaoAvaliada.Contains(item.Ciap.Codigo))
                    listaCiapCodigos.Add(item.Ciap.Codigo);
            }

            if (listaCiapCodigos.Any())
            {
                xElements.Add(new XElement("outroCiap1", listaCiapCodigos.ElementAt(0)));
                if (listaCiapCodigos.Count > 1)
                    xElements.Add(new XElement("outroCiap2", listaCiapCodigos.ElementAt(1)));
            }

            if (listaCid10.Any())
            {
                xElements.Add(new XElement("cid10", listaCid10.ElementAt(0).Cid.Codigo));
                if (listaCid10.Count > 1)
                    xElements.Add(new XElement("cid10_2", listaCid10.ElementAt(1).Cid.Codigo));

            }

            return xElements;
        }

        private IEnumerable<XElement> GetCondutasElements()
        {
            if (condutas == null || !condutas.Any())
                return null;

            var listaCondutas = condutas.Split(',').ToList();

            if (listaCondutas.Count != listaCondutas.Distinct().Count())
                throw new Exception("Não deve conter duas condutas iguais.");


            var codigosCondutas = EnumService<CondutaEncaminhamento>.ParseList(listaCondutas).ToList();

            var xElements = new List<XElement>();

            xElements.AddRange(codigosCondutas.Select(conduta => new XElement("condutas", (int)conduta)));

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
            numProntuario = atendimento.NumProntuario;
            cnes = atendimento.Agendamento.Estabelecimento.Cnes;
            ine = atendimento.Agendamento.Estabelecimento.CodIne;
            cpfCidadao = atendimento.Individuo.Cpf;
            cnsCidadao = atendimento.Individuo.Cns;
            dtNascimento = atendimento.Individuo.DataNascimento.AddHours(12);
            sexo = atendimento.Individuo.Sexo;
            dataAtendimento = atendimento.DataAlteracao;
            localAtendimento = (int)atendimento.LocalDeAtendimento;
            turno = GetTurno(dataAtendimento);
            tipoDeAtendimento = atendimento.AtendidoMedico ? int.Parse(atendimento.TipoAtendimento) : (int)TipoDeAtendimento.ESCUTA_INICIAL_ORIENTACAO;
            condutas  = atendimento.CondutaDesfecho;
            condicoesAvaliadas = atendimento.CondicoesAvaliadas;

            dataHoraInicialAtendimento = dataAtendimento.AddMinutes(10); // TODO adicionar Inicio e Fim dos Atendimentos na própria ficha
            dataHoraFinalAtendimento = atendimento.DataAlteracao.AddMinutes(10);
            pesoAcompanhamentoNutricional = atendimento.Agendamento.Peso;
            alturaAcompanhamentoNutricional = atendimento.Agendamento.Altura;

            listaCid10 = atendimento.IndividuoCid10.ToList();
            ValidarCid10(listaCid10);

            listaCiap = atendimento.IndividuoCiap.ToList();
            ValidarCiap(listaCiap);
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
            if ((int)atendimento.LocalDeAtendimento < 1 || (int)atendimento.LocalDeAtendimento > 10)
                throw new Exception("Local De Atendimento possui um valor inválido. Apenas valores de 1 a 10");
            if (GetTurno(atendimento.DataAlteracao) == 0)
                throw new Exception("Código do turno onde aconteceu o atendimento é obrigatório.");

            if (string.IsNullOrEmpty(atendimento.TipoAtendimento))
                throw new Exception("TipoAtendimento não preenchido.");

            if (!string.IsNullOrEmpty(atendimento.TipoAtendimento))
            {
                int tipoAtendimento = int.Parse(atendimento.TipoAtendimento);
                if (tipoAtendimento != 1 && tipoAtendimento != 2 && tipoAtendimento != 3 && tipoAtendimento != 4 && tipoAtendimento != 5 && tipoAtendimento != 6)
                    throw new Exception("O TipoAtendimento aceita apenas as opções 1, 2, 3, 4, 5 e 6");
            }

            if (!string.IsNullOrEmpty(atendimento.Agendamento.Peso))
            {
                var regex = new Regex(@"^[\d.]*\d$");
                if (!regex.IsMatch(atendimento.Agendamento.Peso))
                    throw new Exception("Apenas números e ponto são permitidos.");
            }

            if (!string.IsNullOrEmpty(atendimento.Agendamento.Altura))
            {
                var regex = new Regex(@"^[\d.]*\d$");
                if (!regex.IsMatch(atendimento.Agendamento.Altura))
                    throw new Exception("Apenas números e ponto são permitidos.");

                if (int.Parse(atendimento.Agendamento.Altura) < 20 || int.Parse(atendimento.Agendamento.Altura) > 250)
                    throw new Exception("Valor mínimo 20 e máximo 250");
            }

            if (string.IsNullOrEmpty(atendimento.CondicoesAvaliadas))
                throw new Exception("Obrigatório o preenchimento do campo condicoesAvaliadas");

            if (string.IsNullOrEmpty(atendimento.CondutaDesfecho))
                throw new Exception("Obrigatório o preenchimento do campo condutas");

            if (string.IsNullOrEmpty(atendimento.Agendamento.Estabelecimento.Cnes))
                throw new Exception("CNES do Estabelecimento não preencido.");

            if (string.IsNullOrEmpty(atendimento.Agendamento.Estabelecimento.CodIne))
                throw new Exception("Código INE do Estabelecimento não preencido.");

        }

        private static void ValidarCid10(ICollection<IndividuoCid10> lista)
        {
            //if (lista.Count > 2)
            //    throw new Exception("Não é permitido ter mais de 2 CID10 para um atendimento");
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
