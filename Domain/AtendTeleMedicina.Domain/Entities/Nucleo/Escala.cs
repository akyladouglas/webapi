using AtendTeleMedicina.Domain.Entities.Base;
using System;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class Escala : EntidadeBase
    {
        public string EstabelecimentoId { get; set; }
        public Estabelecimento Estabelecimento { get; set; }
        public string ProfissionalId { get; set; }
        public Profissional Profissional { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataAlteracao { get; set; }
        public TimeSpan HoraInicial { get; set; }
        public TimeSpan HoraFinal { get; set; }
    }
}
