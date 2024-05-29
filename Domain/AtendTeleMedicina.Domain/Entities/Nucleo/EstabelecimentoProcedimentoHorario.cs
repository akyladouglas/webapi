using System;
using AtendTeleMedicina.Domain.Entities.Base;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class EstabelecimentoProcedimentoHorario : EntidadeBase
    {
        public Estabelecimento Estabelecimento { get; set; }
        public string EstabelecimentoId { get; set; }
        public Procedimento Procedimento { get; set; }
        public string ProcedimentoId { get; set; }
        public Profissional Profissional { get; set; }
        public string ProfissionalId { get; set; }
        public string FichaDeReferenciaId { get; set; }
        public string TipoDaConsulta { get; set; }
        public DateTime? DataHora { get; set; }
        public DateTime? Dia { get; set; }
        public TimeSpan? Hora { get; set; }
        public int? Situacao { get; set; }

        public virtual string NomeFantasia { get; set; }

    }
}
