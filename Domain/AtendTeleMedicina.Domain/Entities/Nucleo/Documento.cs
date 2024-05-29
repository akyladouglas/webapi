using AtendTeleMedicina.Domain.Entities.Base;
using System;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class Documento : EntidadeBase
    {
        #region Propriedades
        public string IndividuoId { get; set; }
        public string ProcedimentoId { get; set; }
        public string AgendamentoId { get; set; }
        public string ProfissionalId { get; set; }
        public string Url { get; set; }
       
        public DateTime? DataCadastro { get; set; }
        public virtual Individuo Individuo { get; set; }
        public virtual Procedimento Procedimento { get; set; }
        public virtual Agendamento Agendamento { get; set; }
        public virtual Profissional Profissional { get; set; }
        #endregion

        #region Construtores
        public Documento()
        {

        }
        #endregion
    }
}
