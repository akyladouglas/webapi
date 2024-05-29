using System;
using System.Collections.Generic;
using System.Text;

namespace AtendTeleMedicina.Domain.Entities.Dashboard
{
    public class DashboardTipoAtendimento 
    {
        public string Periodo { get; set; }
        public string TipoDaConsulta { get; set; }
        public int Total { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public string EstabelecimentoId { get; set; }
        public string ProfissionalId { get; set; }
    }
}
