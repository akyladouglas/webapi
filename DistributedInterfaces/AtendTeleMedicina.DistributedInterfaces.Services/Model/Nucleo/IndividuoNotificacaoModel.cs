using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.Domain.Entities.Base;
using System;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class IndividuoNotificacaoModel : BaseModel
    {
        public string NotificacaoId { get; set; }
        public string IndividuoId { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}
