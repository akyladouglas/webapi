using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.Infrastructure.CrossCutting.Notification.Validations;
using System;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class IndividuoProcedimentoAutorizacaoModel : BaseModel
    {
        public string Id { get; set; }
        public string IndividuoId { get; set; }
        public string ProcedimentoId { get; set; }
        public ProcedimentoModel Procedimento { get; set; }
        public DateTime DataCadastro { get; set; }

        public bool Autorizado { get; set; }

        public void ValidarAdicionar()
        {
            ValidarString();
            AddNotifications(new Contract()
                .Requires()
                .IsNotNullOrEmpty(IndividuoId, "IndividuoId", "IndividuoId é Obrigatório")
                .IsNotNullOrEmpty(ProcedimentoId, "ProcedimentoId", "ProcedimentoId é Obrigatório"));
            
        }

        public void ValidarEditar()
        {
            ValidarString();
            AddNotifications(new Contract()
                .Requires()
                .IsNotNullOrEmpty(IndividuoId, "IndividuoId", "IndividuoId é Obrigatório")
                .IsNotNullOrEmpty(ProcedimentoId, "ProcedimentoId", "ProcedimentoId é Obrigatório"));

        }

        public void ValidarString()
        {
            IndividuoId = ValidateText(IndividuoId, 36);
            ProcedimentoId = ValidateText(ProcedimentoId, 36);
        }

        public void ValidarGet()
        {
            ValidarString();
        }
    }
}
