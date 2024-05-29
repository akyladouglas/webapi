using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class AvaliacaoModel : BaseModel
    {
        public string IndividuoId { get; set; }
        public string ProfissionalId{ get; set; }
        public DateTime DataAvaliacao { get; set; }
        public string AtendimentoId { get; set; }
        public int Questao1 { get; set; }
        public int Questao2 { get; set; }
    }
}
