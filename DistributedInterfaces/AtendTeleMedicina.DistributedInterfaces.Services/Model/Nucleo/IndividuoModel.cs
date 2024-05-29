using System;
using System.Collections;
using System.Collections.Generic;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Security;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Entities.Security;
using AtendTeleMedicina.Domain.Enums;
using AtendTeleMedicina.Domain.Helpers;
using AtendTeleMedicina.Infrastructure.CrossCutting.Notification.Validations;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class IndividuoModel : BaseModel
    {
        public string Id { get; set; }
        public string EstabelecimentoId { get; set; }
        public string Cpf { get; set; }
        public string Cns { get; set; }
        public virtual string Username { get; set; }
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
        public bool RespondeuSintomasNoPeriodo { get; set; }
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
        public virtual IList Roles { get; set; }
        public IList<UserClaimModel> UserClaims { get; set; }
        public IList<IndividuoProcedimentoAutorizacao> Autorizacoes { get; set; }
        public IList<IndividuoCiap> IndividuoCiap { get; set; }
        public string NotificacaoToken { get; set; }
        public string Imagem { get; set; }
        public int? CorStatus { get; set; }
        public string CorMensagem { get; set; }
        public string FaceToken { get; set; }
        public DateTime? FaceTokenValidade { get; set; }
        public string Face { get; set; }
        public string DocumentFront { get; set; }
        public string DocumentBack { get; set; }
        public string DataDoNascimento { get; set; }

        // Adicionados após integrador
        public Guid? DadoSerializado { get; set; }
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

        public bool? Valido { get; set; }

        public string Justificativa { get; set; }

        #region Construtores
        public IndividuoModel()
        {
            UserClaims = new List<UserClaimModel>();
        }
        #endregion

        public void ValidarAdicionar()
        {
             //RETIRADO AS VALIDAÇÕES
            //.IsNotNullOrEmpty(Logradouro, "Logradouro", "Logradouro é Obrigatório")
            //.IsNotNullOrEmpty(LogradouroCep, "Cep", "Cep é Obrigatório")
            ValidarString();
            AddNotifications(new Contract()
                .Requires()
                //.IsNotNullOrEmpty(Cpf, "Cpf", "Cpf Inválido")
                //.HasLen(Cpf, 11, "Cpf", "Cpf deve conter 11 caracteres")
                .IsNotNullOrEmpty(TelefoneCelular, "TelefoneCelular", "Telefone Celular Inválido")
                .HasLen(TelefoneCelular, 11, "TelefoneCelular", "Telefone Celular deve conter 11 caracteres")
                .IsBetween(DataNascimento, new DateTime(1900, 1, 1), DateTime.Now, "DataNascimento", "Data Nascimento Inválida"));
                AddNotifications(new Contract()
                //.IsEmail(Email, "Email", "Email inválido")
                );
        }

        public void ValidarEditar()
        {
            //ValidarString();
            //AddNotifications(new Contract()
            //    .Requires()
            //    .IsNotNullOrEmpty(CidadeId, "Cidade_Id", "Cidade é Obrigatório")
            //    .IsNotNullOrEmpty(UfAbreviado, "UfAbreviado", "Estado é Obrigatório")
            //    .IsNotNullOrEmpty(Logradouro, "Logradouro", "Logradouro é Obrigatório")
            //    .IsNotNullOrEmpty(LogradouroBairro, "Bairro", "Bairro é Obrigatório")
            //    .IsNotNullOrEmpty(LogradouroCep, "Cep", "Cep é Obrigatório")
            //    .IsNotNullOrEmpty(Cpf, "Cpf", "Cpf Inválido")
            //    .HasLen(Cpf, 11, "Cpf", "Cpf deve conter 11 caracteres")
            //    .IsNotNullOrEmpty(TelefoneCelular, "TelefoneCelular", "Telefone Celular Inválido")
            //    .HasLen(TelefoneCelular, 11, "TelefoneCelular", "Telefone Celular deve conter 11 caracteres")
            //    .IsBetween(DataNascimento, new DateTime(1900, 1, 1), DateTime.Now, "DataNascimento", "Data Nascimento Inválida"));
            //    AddNotifications(new Contract()
            //    .IsEmail(Email, "Email", "Email inválido"));
        }

        public void ValidarString()
        {
            Cns = ValidateText(Cns, 15);
            //Cpf = ValidateText(Cpf, 11);
            Email = ValidateText(Email, 255);
            TelefoneCelular = ValidateText(TelefoneCelular, 11);
            NomeCompleto = ValidateText(NomeCompleto, 255);
        }

        public void ValidarGet()
        {
            ValidarString();
        }

    }
}
