using AtendTeleMedicina.Domain.Entities.Base;
using AtendTeleMedicina.Domain.Enums;
using System;
using System.Collections.Generic;
using AtendTeleMedicina.Domain.Entities.Security;
using static System.Net.Mime.MediaTypeNames;
using AtendTeleMedicina.Domain.Helpers;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class Individuo : EntidadeBase
    {
        public string EstabelecimentoId { get; set; }
        public string Cpf { get; set; }
        public string Cns { get; set; }
        public virtual string Username => Cns;
        public virtual string Audience { get; set; }
        public string Email { get; set; }
        public string EmailToken { get; set; }
        public string CodigoAutenticacao { get; set; }
        public string Senha { get; set; }
        public string TelefoneCelular { get; set; }
        public string NomeCompleto { get; set; }
        public string NomeDaMae { get; set; }
        public string NomeSocial { get; set; }
        public DateTime DataNascimento { get; set; }
        public int Sexo { get; set; }
        public RacaOuCor RacaOuCor { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataAlteracao { get; set; }
        public DateTime DataInicioSintomas { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Logradouro { get; set; }
        public string LogradouroNumero { get; set; }
        public string LogradouroComplemento { get; set; }
        public string LogradouroCep { get; set; }
        public string LogradouroBairro { get; set; }
        public string UfAbreviado { get; set; }
        public UnidadeFederativa UnidadeFederativa { get; set; }
        public string CidadeId { get; set; }
        public Cidade Cidade { get; set; }
        public DateTime DataDeIntegraçaoGov { get; set; }
        public bool? RespondeuComorbidade { get; set; }
        public string Comorbidades { get; set; }
        public bool? Hipertenso { get; set; }
        public bool? Diabetes { get; set; }
        public bool? Fumante { get; set; }
        public bool? Asma { get; set; }
        public bool? DoencaCoracao { get; set; }
        public bool? DoencaPulmao { get; set; }
        public bool? DoencaRins { get; set; }
        public bool? DoencaFigado { get; set; }
        public bool? DoencaCancer { get; set; }
        public bool? Transplantado { get; set; }
        public bool? DoencaComprometeImunidade { get; set; }
        public bool? LugarComCasosCorona { get; set; }
        public bool? Obesidade { get; set; }
        public bool? Gestante { get; set; }
        public bool? DoencaNeurologica { get; set; }
        public bool? AnemiaFalciforme { get; set; }
        public bool? Ativo { get; set; }
        public bool? Deletado { get; set; }
        public IList<UserClaim> UserClaims { get; set; }
        public IList<IndividuoProcedimentoAutorizacao> Autorizacoes { get; set; }
        public IList<IndividuoCiap> IndividuoCiap { get; set; }
        public string NotificacaoToken { get; set; }
        public string Imagem { get; set; }
        public string CorMensagem { get; set; }
        public string FaceToken { get; set; }
        public DateTime? FaceTokenValidade { get; set; }
        public string Face { get; set; }
        public string DocumentFront { get; set; }
        public string DocumentBack { get; set; }
        public int? CorStatus { get; set; }
        public string DataDoNascimento { get; set; }

        // Adicionados após integrador
        public Guid? DadoSerializado { get; set; }
        public int? LoteIntegracaoId { get; set; }
        public bool? TemMaeDesconhecida { get; set; }
        public string NomeDoPai { get; set; }
        public bool? TemPaiDesconhecido { get; set; }

        public string PisPasep { get; set; }
        public int? PaisDeNascimento { get; set; }
        public int? CidadeDeNascimentoIbge { get; set; }
        public string UfDeNascimentoAbreviado { get; set; } // akyla
        public Nacionalidade? Nacionalidade { get; set; }
        public string NaturalizacaoPortaria { get; set; }
        public DateTime? NaturalizacaoData { get; set; } // akyla
        public DateTime? DataEntradaNoPais { get; set; } // akyla

        public virtual Idade Idade => new Idade(DataNascimento);
        public Etnia? Etnia { get; set; }
        public GrauDeInstrucao? GrauDeInstrucao { get; set; }
        public bool? FrequentaEscola { get; set; }


        public Estabelecimento Estabelecimento { get; set; } // Último estabelecimento que o paciente foi atendido
        public Profissional Profissional { get; set; } // Último profissional que realizou atendimento

        public string Justificativa { get; set; }

        public bool? Valido { get; set; }
        public void Validar()
        {
            Valido = true;
        }

        public void Invalidar()
        {
            Valido = false;
        }


        #region Construtores
        public Individuo()
        {
            UserClaims = new List<UserClaim>();
        }
        #endregion
    }
}
