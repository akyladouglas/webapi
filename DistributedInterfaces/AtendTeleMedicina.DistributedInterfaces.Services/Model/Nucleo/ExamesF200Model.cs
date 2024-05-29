using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using System;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class ExamesF200Model : BaseModel
    {
        #region Propriedades

        public string Id { get; set; }
        public string IdPaciente { get; set; }
        public DateTime DataTransferenciaEcoPc { get; set; }
        public string CpfPaciente { get; set; }
        public string TipoExameEco { get; set; }
        public string ResultadoExameEco { get; set; }
        public string UnidadeResultadoEco { get; set; }
        public string ValorReferenciaResultadoEco { get; set; }
        public DateTime DataExameEco { get; set; }
        public string OperadorEco { get; set; }
        public string Url { get; set; }


        public virtual IndividuoModel Individuo { get; set; }
        //public virtual TipoExameModel TipoExame { get; set; }
        //public virtual ProfissionalModel Profissional { get; set; }

        #endregion
    }
}
