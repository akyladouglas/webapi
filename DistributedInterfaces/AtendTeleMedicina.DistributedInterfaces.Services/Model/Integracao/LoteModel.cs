using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Integracao
{
    public class LoteModel
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public int Mes { get; set; }
        public int Ano { get; set; }
        public string Estabelecimento_Id { get; set; }
        public EstabelecimentoModel Estabelecimento { get; set; }
        public string Periodo { get; set; }
        public int Validos { get; set; }
        public int Erros { get; set; }
    }
}
