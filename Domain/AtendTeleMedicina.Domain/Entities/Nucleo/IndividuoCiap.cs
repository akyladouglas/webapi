using System;
using AtendTeleMedicina.Domain.Entities.Base;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class IndividuoCiap : EntidadeBase
    {
        public string IndividuoId { get; set; }
        public Individuo Individuo { get; set; }
        public string ProfissionalId { get; set; }
        public Profissional Profissional { get; set; }

        public Agendamento Agendamento { get; set; }
        public string AgendamentoId { get; set; }
        public int CiapId { get; set; }
        public virtual Ciap Ciap { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataAlteracao { get; set; }
        public bool AtendidoTriagem { get; set; }
        public bool AtendidoMedico { get; set; }
        public Guid? DadoSerializado { get; set; }

    }
}
