using AtendTeleMedicina.Domain.Entities.Base;
using AtendTeleMedicina.Domain.Entities.Security;
using System;
using System.Collections.Generic;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class ProfissionaisInterconsulta : EntidadeBase
    {
        #region Propriedades

        public string Nome { get; set; }
        public string Participante { get; set; }
        public string Tipo { get; set; }

        #endregion

        #region Construtores
        public ProfissionaisInterconsulta()
        {

        }
        #endregion
    }
}
