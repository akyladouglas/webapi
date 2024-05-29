using AtendTeleMedicina.Domain.Entities.Base;
using AtendTeleMedicina.Domain.Enums;
using AtendTeleMedicina.Domain.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class AgendamentoParticipante : EntidadeBase
    {
        #region Propriedades
        public string Id { get; set; }
        public string AgendamentoId { get; set; }
        public string ProfissionalId { get; set; }
        public bool Convidado { get; set; }
        public bool? Aceitou { get; set; }
        public bool ParticipouAtendimento { get; set; }
        public string Avaliacao { get; set; }

        public Guid? DadoSerializado { get; set; }
        public int? LoteIntegracaoId { get; set; }

        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public bool? Interconsulta { get; set; }

        public virtual Agendamento Agendamento { get; set; }
        public virtual Profissional Profissional { get; set; }

        #endregion

        #region Construtores
        public AgendamentoParticipante()
        {

        }
        #endregion
    }
}
