
using System;
using System.Collections.Generic;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Security;
using AtendTeleMedicina.Domain.Enums;
using AtendTeleMedicina.Infrastructure.CrossCutting.Notification.Validations;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class EstabelecimentoModel : BaseModel
    {
        public string Id { get; set; }
        public string NomeFantasia { get; set; }

        public string Cnpj { get; set; }

        public string Cnes { get; set; }

        public string ResponsavelCpf { get; set; }

        public string RamoAtividade { get; set; }

        public string TipoEmpresa { get; set; }

        public int CodEsfAdm { get; set; }

        public int TpUnidId { get; set; }

        public string Telefone1 { get; set; }

        public string Telefone2 { get; set; }

        public string Fax { get; set; }

        public string Email { get; set; }

        public string Cep { get; set; }

        public string Uf { get; set; }

        public string CodIbgeMun { get; set; }

        public string Bairro { get; set; }

        public string Logradouro { get; set; }

        public string Numero { get; set; }

        public string Complemento { get; set; }

        public string PontoReferencia { get; set; }

        public string SgComplexidade1 { get; set; }

        public string SgComplexidade2 { get; set; }

        public string TpEquipe { get; set; }

        public string SgEquipe { get; set; }

        public string DsEquipe { get; set; }

        public string CodIne { get; set; }

        public int CodArea { get; set; }

        public string DsArea { get; set; }

        public string NmReferencia { get; set; }

        public DateTime? DataDesativacao { get; set; }

        public DateTime DataCadastro { get; set; }
        public DateTime DataAlteracao { get; set; }

        public string Tipo { get; set; }

        public bool? Ativo { get; set; }
        public bool? Deletado { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }

        public IList<EstabelecimentoProcedimentoModel> Procedimentos { get; set; }
        public IList<ProfissionalModel> Profissionais { get; set; }


        public void ValidarAdicionar()
        {
            ValidarString();
            AddNotifications(new Contract()
                .Requires()
                .IsNotNullOrEmpty(NomeFantasia, "NomeFantasia", "O Campo NomeFantasia é Obrigatório"));
        }

        public void ValidarEditar()
        {
            ValidarString();
            AddNotifications(new Contract()
                .Requires()
                .IsNotNullOrEmpty(NomeFantasia, "NomeFantasia", "O Campo NomeFantasia é Obrigatório"));
        }

        public void ValidarString()
        {
            NomeFantasia = ValidateText(NomeFantasia, 255);
        }

        public void ValidarGet()
        {
            ValidarString();
        }
    }
}
