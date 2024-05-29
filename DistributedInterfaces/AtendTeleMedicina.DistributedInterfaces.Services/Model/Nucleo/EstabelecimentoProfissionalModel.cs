
using System;
using System.Collections.Generic;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Security;
using AtendTeleMedicina.Domain.Enums;
using AtendTeleMedicina.Infrastructure.CrossCutting.Notification.Validations;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class EstabelecimentoProfissionalModel : BaseModel
    {
        public string EstabelecimentoId { get; set; }
        public EstabelecimentoModel Estabelecimento { get; set; }
        public string ProfissionalId { get; set; }
        public ProfissionalModel Profissional { get; set; }

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
                .IsNotNullOrEmpty(ProfissionalId, "ProfissionalId", "O Id do Profissional é Obrigatório"));
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
