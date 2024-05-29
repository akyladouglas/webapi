using AtendTeleMedicina.Domain.Entities.Base;
using AtendTeleMedicina.Domain.Enums;
using System;
using System.Collections.Generic;
using AtendTeleMedicina.Domain.Entities.Security;
using static System.Net.Mime.MediaTypeNames;
using AtendTeleMedicina.Domain.Helpers;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class IndividuoGlicemia : EntidadeBase
    {
        public string IndividuoId { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public bool? RespondeuCafe { get; set; }
        public int? CafeAntes { get; set; }
        public int? CafeDepois { get; set; }
        public bool? RespondeuAlmoco { get; set; }
        public int? AlmocoAntes { get; set; }
        public int? AlmocoDepois { get; set; }
        public bool? RespondeuJanta { get; set; }
        public int? JantaAntes { get; set; }
        public int? JantaDepois { get; set; }
        public bool? RespondeuDormirMadrugada { get; set; }
        public int? AntesDormirMadrugada { get; set; }
        public string Observacoes { get; set; }
        public virtual Individuo Individuo { get; set; }

        #region Construtores
        public IndividuoGlicemia()
        {
        }
        #endregion
    }
}
