using AtendTeleMedicina.Domain.Entities.Base;
using System;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class Avaliacao : EntidadeBase
    {

        public string IndividuoId { get; set; }
        public string ProfissionalId { get; set; }
        public DateTime DataAvaliacao { get; set; }
        public string AtendimentoId { get; set; }
        public int Questao1 { get; set; }
        public int Questao2 { get; set; }

        public Avaliacao() 
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

    }

}