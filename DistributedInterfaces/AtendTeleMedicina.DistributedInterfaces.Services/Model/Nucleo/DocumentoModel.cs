using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.Infrastructure.CrossCutting.Notification.Validations;
using System;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class DocumentoModel : BaseModel
    {
        #region Propriedades
        public string Id { get; set; }
        public string IndividuoId { get; set; }
        public string ProcedimentoId { get; set; }
        public string AgendamentoId { get; set; }
        public string ProfissionalId { get; set; }
        public string Url { get; set; }
        public DateTime? DataCadastro { get; set; }

        public virtual IndividuoModel Individuo { get; set; }
        public virtual ProcedimentoModel Procedimento { get; set; }
        public virtual AgendamentoModel Agendamento { get; set; }
        public virtual ProfissionalModel Profissional { get; set; }
        
        
        #endregion
    }
}
