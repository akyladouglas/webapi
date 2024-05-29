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
    public class XmlIndividuoSimplificado : XmlColetaBase, IXmlCadastro
    {
        #region Propriedades

        public XNamespace ns4 { get; set; }
        public int tipoDadoSerializado { get; set; }

        public bool? statusSituacaoRua { get; set; }
        #endregion

        #region FichaAtualizada

        public bool fichaAtualizada { get; set; }

        #endregion

        #region IdentificacaoUsuarioCidadao
        public string nomeSocial { get; set; }
        public string codigoIbgeMunicipioNascimento { get; set; }
        public DateTime? dataNascimentoCidadao { get; set; }
        public bool? desconheceNomeMae { get; set; }
        public string emailCidadao { get; set; }
        public int? nacionalidadeCidadao { get; set; }
        public string nomeCidadao { get; set; }
        public string nomeMaeCidadao { get; set; }
        public string telefoneCelular { get; set; }
        public string cnsCidadao { get; set; }
        public string cpfCidadao { get; set; }
        public string numeroNisPisPasep { get; set; }
        public int? paisNascimento { get; set; }
        public int? racaCorCidadao { get; set; }
        public int? sexoCidadao { get; set; }
        public int? etnia { get; set; }
        public string nomePaiCidadao { get; set; }
        public bool? desconheceNomePai { get; set; }
        public DateTime? dtNaturalizacao { get; set; }
        public string portariaNaturalizacao { get; set; }
        public DateTime? dtEntradaBrasil { get; set; }
        public int microArea { get; set; }
        public bool? stForaArea { get; set; }

        #endregion

        #region InformacoesSocioDemograficas

        public int? grauInstrucaoCidadao { get; set; }
        public string ocupacaoCodigoCbo2002 { get; set; }
        public int? orientacaoSexualCidadao { get; set; }
        public int? situacaoMercadoTrabalhoCidadao { get; set; }
        public bool? statusDesejaInformarOrientacaoSexual { get; set; }
        public bool? statusFrequentaEscola { get; set; }
        public bool statusTemAlgumaDeficiencia { get; set; }
        public bool? statusDesejaInformarIdentidadeGenero { get; set; }
        public int? identidadeGeneroCidadao { get; set; }
        public int? idade { get; set; }
        #endregion

        #region SaidaCidadaoCadastro

        private int? saidaCidadaoCadastro;

        #endregion

        #region TermoRecusa

        public bool statusTermoRecusaCadastroIndividualAtencaoBasica { get; set; }

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

        #region SaidaCidadao
        public int? motivoSaidaCidadao { get; set; }
        public DateTime? dataObito { get; set; }
        public string numeroDO { get; set; }

        #endregion

        public XmlIndividuoSimplificado()
        {
            ns4 = "http://esus.ufsc.br/cadastroindividual";
            tipoDadoSerializado = 2;
        }

        public XElement GetColetaElement()
        {
            var element = new XElement(ns4 + "cadastroIndividualTransport",
                !statusTermoRecusaCadastroIndividualAtencaoBasica ? GetSituacaoDeRua() : null,
                new XElement("fichaAtualizada", fichaAtualizada),
                GetIdentificacaoUsuarioCidadao(),
                !statusTermoRecusaCadastroIndividualAtencaoBasica ? GetInformacoesSocioDemograficas() : null,
                GetSaidaCidadaoCadastro(),
                new XElement("statusTermoRecusaCadastroIndividualAtencaoBasica", statusTermoRecusaCadastroIndividualAtencaoBasica),
                new XElement("tpCdsOrigem", tpCdsOrigem),
                new XElement("uuid", uuid),
                new XElement("uuidFichaOriginadora", uuidFichaOriginadora),
                GetHeaderTransport()
                );
            return element;
        }

        private XElement GetSaidaCidadaoCadastro()
        {
            if (statusTermoRecusaCadastroIndividualAtencaoBasica) return new XElement("saidaCidadaoCadastro", null);

            return new XElement("saidaCidadaoCadastro",
                motivoSaidaCidadao != null ? new XElement("motivoSaidaCidadao", motivoSaidaCidadao) : null,
                dataObito != null ? new XElement("dataObito", $"{new DateTimeOffset(dataObito.Value).ToUnixTimeMilliseconds()}") : null,
                numeroDO != null ? new XElement("numeroDO", numeroDO) : null
                );
        }

        private XElement GetSituacaoDeRua()
        {
            if (statusTermoRecusaCadastroIndividualAtencaoBasica) return new XElement("emSituacaoDeRua", null);
            return new XElement("emSituacaoDeRua",
                statusSituacaoRua != null ? new XElement("statusSituacaoRua", statusSituacaoRua) : null
                );
        }

        private XElement GetIdentificacaoUsuarioCidadao()
        {
            if (statusTermoRecusaCadastroIndividualAtencaoBasica) return new XElement("identificacaoUsuarioCidadao", null);

            return new XElement("identificacaoUsuarioCidadao",
                new XElement("nomeSocial", nomeSocial),
                codigoIbgeMunicipioNascimento != null ? new XElement("codigoIbgeMunicipioNascimento", codigoIbgeMunicipioNascimento) : null,
                new XElement("dataNascimentoCidadao", dataNascimentoCidadao != null ? $"{new DateTimeOffset(dataNascimentoCidadao.Value).ToUnixTimeMilliseconds()}" : null),
                (desconheceNomeMae.HasValue && desconheceNomeMae == true) ? new XElement("desconheceNomeMae", desconheceNomeMae) : null,
                !string.IsNullOrEmpty(emailCidadao) ? new XElement("emailCidadao", emailCidadao) : null,
                new XElement("nacionalidadeCidadao", nacionalidadeCidadao),
                new XElement("nomeCidadao", nomeCidadao),
                !string.IsNullOrEmpty(nomeMaeCidadao) ? new XElement("nomeMaeCidadao", nomeMaeCidadao) : null,
                !string.IsNullOrEmpty(cpfCidadao) ? new XElement("cpfCidadao", cpfCidadao) : new XElement("cnsCidadao", cnsCidadao),
                new XElement("telefoneCelular", telefoneCelular),
                !string.IsNullOrEmpty(numeroNisPisPasep) ? new XElement("numeroNisPisPasep", numeroNisPisPasep) : null,
                nacionalidadeCidadao == 1 ? new XElement("paisNascimento", 31) : new XElement("paisNascimento", paisNascimento),
                new XElement("racaCorCidadao", racaCorCidadao),
                new XElement("sexoCidadao", sexoCidadao),
                racaCorCidadao == 5 && etnia != null ? new XElement("etnia", etnia) : null, //Este campo é obrigatório se a racaCorCidadao for 5
                (desconheceNomePai.HasValue && desconheceNomePai == true) ? new XElement("desconheceNomePai", desconheceNomePai) : null,
                !string.IsNullOrEmpty(nomePaiCidadao) ? new XElement("nomePaiCidadao", nomePaiCidadao) : null,
                dtNaturalizacao != null ? new XElement("dtNaturalizacao", dtNaturalizacao?.DateTimeParaUnixTime()) : null,
                !string.IsNullOrEmpty(portariaNaturalizacao) ? new XElement("portariaNaturalizacao", portariaNaturalizacao) : null,
                dtEntradaBrasil != null ? new XElement("dtEntradaBrasil", dtEntradaBrasil?.DateTimeParaUnixTime()) : null,
                stForaArea != true ? new XElement("microArea", $"{microArea}".PadLeft(2, '0')) : null,
                stForaArea != null && stForaArea == true ? new XElement("stForaArea", stForaArea) : null
                );
        }

        private XElement GetInformacoesSocioDemograficas()
        {
            if (statusTermoRecusaCadastroIndividualAtencaoBasica) return new XElement("informacoesSocioDemograficas", null);

            return new XElement("informacoesSocioDemograficas",
                grauInstrucaoCidadao != null ? new XElement("grauInstrucaoCidadao", grauInstrucaoCidadao) : null,
                ocupacaoCodigoCbo2002 != null && ocupacaoCodigoCbo2002 != "1" ? new XElement("ocupacaoCodigoCbo2002", ocupacaoCodigoCbo2002) : null,
                orientacaoSexualCidadao != null && statusDesejaInformarOrientacaoSexual == true ? new XElement("orientacaoSexualCidadao", orientacaoSexualCidadao) : null,
                situacaoMercadoTrabalhoCidadao != null ? new XElement("situacaoMercadoTrabalhoCidadao", situacaoMercadoTrabalhoCidadao) : null,
                statusDesejaInformarOrientacaoSexual != null ? new XElement("statusDesejaInformarOrientacaoSexual", statusDesejaInformarOrientacaoSexual) : null,
                statusFrequentaEscola != null ? new XElement("statusFrequentaEscola", statusFrequentaEscola) : null,
                new XElement("statusTemAlgumaDeficiencia", statusTemAlgumaDeficiencia),
                identidadeGeneroCidadao != null ? new XElement("identidadeGeneroCidadao", identidadeGeneroCidadao) : null,
                statusDesejaInformarIdentidadeGenero != null ? new XElement("statusDesejaInformarIdentidadeGenero", statusDesejaInformarIdentidadeGenero) : null
                );
        }

        public XElement GetHeaderTransport()
        {
            return new XElement("headerTransport",
                new XElement("profissionalCNS", profissionalCNS?.PadLeft(15, '0')),
                new XElement("cboCodigo_2002", cboCodigo_2002),
                new XElement("cnes", cnes?.PadLeft(7, '0')),
                new XElement("ine", ine?.PadLeft(10, '0')),
                new XElement("dataAtendimento", $"{new DateTimeOffset(dataAtendimento).ToUnixTimeMilliseconds()}"),
                new XElement("codigoIbgeMunicipio", codigoIbgeMunicipio.PadLeft(7, '0'))
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
            var individuo = entity as Individuo;

            if (individuo == null) return;
            Validar(individuo);

            numLote = $"{lote}";

            profissionalCNS = individuo.Profissional.Cns;
            cboCodigo_2002 = "223565"; // TODO substituir pelo CBO do profissional. Utilizando temporariamente um valor disponível em: //https://integracao.esusab.ufsc.br/ledi/documentacao/regras/cbo.html
            cnes = individuo.Estabelecimento.Cnes;
            ine = individuo.Estabelecimento.CodIne;
            dataAtendimento = individuo.DataAlteracao;
            codigoIbgeMunicipio = individuo.Estabelecimento.CodIbgeMun;

            uuidDadoSerializado = $"{id}";
            cnesDadoSerializado = cnes;
            ineDadoSerializado = ine;

            uuid = individuo.DataCadastro == individuo.DataAlteracao ? $"{cnes}-{individuo.Id}" : $"{cnes}-{Guid.NewGuid()}";
            uuidFichaOriginadora = $"{cnes}-{individuo.Id}";
            fichaAtualizada = individuo.DataCadastro != individuo.DataAlteracao;

            statusSituacaoRua = false;

            nomeSocial = !string.IsNullOrEmpty(individuo.NomeSocial) ? Encoding.ASCII.GetString(Encoding.GetEncoding("Cyrillic").GetBytes(individuo.NomeSocial)) : null;
            nomeSocial = Guardiao.RemoverCaracteresEspeciais(nomeSocial);
            nomeSocial = Guardiao.RemoverAcentos(nomeSocial);
            nomeSocial = nomeSocial?.Trim();

            codigoIbgeMunicipioNascimento = $"{individuo.CidadeDeNascimentoIbge}";
            dataNascimentoCidadao = individuo.DataNascimento;
            desconheceNomeMae = individuo.TemMaeDesconhecida;
            emailCidadao = individuo.Email?.ToString().ToLower();
            cpfCidadao = individuo.Cpf;
            cnsCidadao = individuo.Cns;

            nacionalidadeCidadao = (int?)individuo.Nacionalidade;

            nomeCidadao = Guardiao.RemoverAcentos(individuo.NomeCompleto);
            nomeCidadao = nomeCidadao?.Trim();

            nomeMaeCidadao = Guardiao.RemoverAcentos(individuo.NomeDaMae);
            nomeMaeCidadao = nomeMaeCidadao?.Trim();

            nomePaiCidadao = Guardiao.RemoverAcentos(individuo.NomeDoPai);
            nomePaiCidadao = nomePaiCidadao?.Trim();

            telefoneCelular = string.IsNullOrEmpty(individuo.TelefoneCelular?.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Replace("_", "")) ||
                              individuo.TelefoneCelular?.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Replace("_", "").Length < 10 ||
                              individuo.TelefoneCelular?.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Replace("_", "").Length > 11
                              ? null : individuo.TelefoneCelular.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Replace("_", "");

            numeroNisPisPasep = individuo.PisPasep?.Length == 11 ? individuo.PisPasep : null;
            paisNascimento = individuo.PaisDeNascimento;
            racaCorCidadao = (int?)individuo.RacaOuCor;

            etnia = racaCorCidadao == 5 ? (int)individuo.Etnia : etnia;

            sexoCidadao = (int?)individuo.Sexo;

            idade = individuo.Idade?.Anos;
            desconheceNomePai = individuo.TemPaiDesconhecido;
            statusFrequentaEscola = individuo.FrequentaEscola;
            microArea = 0;
            stForaArea = false;

            grauInstrucaoCidadao = (int?)individuo.GrauDeInstrucao;

            statusTermoRecusaCadastroIndividualAtencaoBasica = false;

            SetDadosInstalacao("Meeds", Guid.NewGuid().ToString(), cpfResponsavel, nomeResponsavel, "2.0.5", "MySql");
        }

        private bool CboInvalidos(string codigo)
        {
            var lista = new List<string>();
            lista.Add("1");
            lista.Add("521140");

            return lista.Contains(codigo);

        }

        private void Validar(Individuo individuo)
        {

            if (individuo.Nacionalidade == Nacionalidade.NATURALIZADO && string.IsNullOrWhiteSpace(individuo.NaturalizacaoPortaria))
                throw new Exception("O campo 'portariaNaturalizacao' é obrigatório.");

            if (individuo.Nacionalidade == 0)
                throw new Exception("O campo 'Nacionalidade' não pode conter o valor (0).");

            if (individuo.Nacionalidade == Nacionalidade.BRASILEIRA && individuo.CidadeDeNascimentoIbge == null)
                throw new Exception("O campo 'CidadeDeNascimentoIbge' é de preenchimento obrigatório quando 'Nacionalidade' for igual a Brasileira.");

            if (individuo.NomeSocial == null) individuo.NomeSocial = "";
            if (individuo.NomeSocial.Any(char.IsDigit))
                throw new Exception("No campo 'NomeSocial' não pode conter número.");

            if (individuo.NomeSocial.Any(char.IsPunctuation))
                throw new Exception("No campo 'NomeSocial' não pode conter pontuações.");

            if (individuo.TemMaeDesconhecida == false && string.IsNullOrEmpty(individuo.NomeDaMae))
                throw new Exception("'NomeDaMae' é um campo de preenchimento obrigatório quando 'TemMaeDesconhecida' for falso.");

            if (string.IsNullOrEmpty(individuo.Estabelecimento.CodIbgeMun))
                throw new Exception("'codigoIbgeMunicipio' não informado. Verifique o estabelecimento do atendimento.");

            if (individuo.TemPaiDesconhecido == false && string.IsNullOrEmpty(individuo.NomeDoPai))
                throw new Exception("'NomeDoPai' é um campo de preenchimento obrigatório quando 'TemPaiDesconhecido' for falso.");

            ValidarNome(individuo.NomeCompleto);

            if (individuo.TemMaeDesconhecida == false && !string.IsNullOrEmpty(individuo.NomeDaMae))
                ValidarNome(individuo.NomeDaMae);

            if (individuo.TemPaiDesconhecido == false && !string.IsNullOrEmpty(individuo.NomeDoPai))
                ValidarNome(individuo.NomeDoPai);

            if (!string.IsNullOrEmpty(individuo.Email))
            {
                if (!Guardiao.ValidaEmail(individuo.Email))
                    throw new Exception("O email informado não é um email válido!.");
            }

            if (individuo.RacaOuCor == RacaOuCor.INDIGENA && !individuo.Etnia.HasValue)
                throw new Exception("O campo 'Etnia' é de preenchimento obrigatório quando o indivíduo é indígena!.");

            if (!individuo.Nacionalidade.HasValue)
                throw new Exception("A 'Nacionalidade' é obrigatória!.");

            if (string.IsNullOrEmpty(individuo.TelefoneCelular))
                throw new Exception("O campo 'TelefoneCelular' é obrigatório.");
                    if (!individuo.TelefoneCelular.Any(char.IsDigit))
                        throw new Exception("O campo 'TelefoneCelular' só pode conter números.");

            individuo.Validar();

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
