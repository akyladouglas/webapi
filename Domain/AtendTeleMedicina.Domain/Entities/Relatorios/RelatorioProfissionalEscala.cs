using System;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class RelatorioProfissionalEscala
    {
        public string ProfissionalId { get; set; }
        public Profissional Profissional { get; set; }

        public DateTime Dia { get; set; }
        public TimeSpan HoraInicial { get; set; }
        public TimeSpan HoraFinal { get; set; }
    }
}
