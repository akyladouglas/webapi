
using System;
using System.Collections.Generic;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Security;
using AtendTeleMedicina.Domain.Enums;
using AtendTeleMedicina.Infrastructure.CrossCutting.Notification.Validations;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class EstabelecimentoProcedimentoModel : BaseModel
    {
        public string Id { get; set; }
        public string EstabelecimentoId { get; set; }
        public EstabelecimentoModel Estabelecimento { get; set; }
        public string ProcedimentoId { get; set; }
        public ProcedimentoModel Procedimento { get; set; }
        public int Cota { get; set; }
        public int CotaExecutor { get; set; }
        public DateTime DataAlteracao { get; set; }

        public void ValidarAdicionar()
        {
            ValidarString();
            AddNotifications(new Contract()
                .Requires());
        }

        public void ValidarEditar()
        {
            ValidarString();
            AddNotifications(new Contract()
                .Requires()
                .IsNotNullOrEmpty(EstabelecimentoId, "EstabelecimentoId", "O Id do Estabelecimento é Obrigatório")
                .IsNotNullOrEmpty(ProcedimentoId, "ProcedimentoId", "O Id do ProcedimentoId é Obrigatório"));
        }

        public void ValidarString()
        {
            EstabelecimentoId = ValidateText(EstabelecimentoId, 36);
        }

        public void ValidarGet()
        {
            ValidarString();
        }
    }
}
