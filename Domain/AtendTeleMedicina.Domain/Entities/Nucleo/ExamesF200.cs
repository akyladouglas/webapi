using AtendTeleMedicina.Domain.Entities.Base;
using System;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class ExamesF200 : EntidadeBase
    {
        #region Propriedades
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
        public string Formato { get; set; }

        public virtual Individuo Individuo { get; set; }
        //public virtual TipoExame TipoExame { get; set; }
        // public virtual Profissional Profissional { get; set; }

        #endregion

        #region Construtores
        public ExamesF200()
        {
            //
        }

        #endregion
    }
}