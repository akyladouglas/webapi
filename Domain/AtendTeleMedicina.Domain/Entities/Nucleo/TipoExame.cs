using AtendTeleMedicina.Domain.Entities.Base;
using System;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class TipoExame : EntidadeBase
    {
        #region Propriedades
        public string Nome { get; set; }

        #endregion

        #region Construtores
        public TipoExame()
        {

        }
        #endregion
    }
}
