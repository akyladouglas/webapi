using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.Domain.Entities.Security;
using AtendTeleMedicina.Infrastructure.CrossCutting.Notification.Validations;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class DashboardParamsModel : BaseModel
    {
        #region Propriedades
        public string Id { get; set; }
        public string Atendimentos { get; set; }
        public string ProfissionalNome { get; set; }
        public string IndividuoNome { get; set; }
        public string ProfissionalId { get; set; }

        public string AgendamentoId { get; set; }
        public string IndividuoId { get; set; }
        public string TipoDaConsulta { get; set; }
        public virtual ProfissionalModel Profissional { get; set; }
        public virtual AgendamentoModel Agendamento { get; set; }
        public virtual IndividuoModel Individuo { get; set; }

        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public string EstabelecimentoId { get; set; }


        #endregion


    }
}
