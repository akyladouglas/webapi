using System;
using AtendTeleMedicina.Domain.Entities.Base;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class Customizacao : EntidadeBase
    {
        #region Propriedades
        public string Logomarca { get; set; }

        #endregion

        #region Construtores
        public Customizacao()
        {

        }
        #endregion
    }
}
