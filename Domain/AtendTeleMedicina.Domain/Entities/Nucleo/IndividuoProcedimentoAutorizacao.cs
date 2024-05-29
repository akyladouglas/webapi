using System;
using AtendTeleMedicina.Domain.Entities.Base;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class IndividuoProcedimentoAutorizacao : EntidadeBase
    {
        public string IndividuoId { get; set; }
        public string ProcedimentoId { get; set; }
        public Procedimento Procedimento { get; set; }
        public DateTime DataCadastro { get; set; }

        public bool Autorizado { get; set; }

    }
}
