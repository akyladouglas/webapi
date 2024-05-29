using System;
using System.Collections.Generic;
using System.Text;

namespace AtendTeleMedicina.Domain.Params
{
    public class AppParams
    {
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public string ProfissionalId { get; set; }
        public string EstabelecimentoId { get; set; }
        public string ProcedimentoId { get; set; }
        public string TipoDaConsulta { get; set; }
        public int? Filtro { get; set; }
        public bool? Positivo { get; set; }
        public string GruposProcedimento { get; set; }

    }
}
