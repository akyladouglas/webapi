using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;

namespace AtendTeleMedicina.Domain.Entities.Dashboard
{
    public class DashboardParams
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
        public virtual Profissional Profissional { get; set; }
        public virtual Agendamento Agendamento { get; set; }
        public virtual Individuo Individuo { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public string EstabelecimentoId { get; set; }

        #endregion

        #region Construtores
        public DashboardParams()
        {

        }
        #endregion
    }
}
