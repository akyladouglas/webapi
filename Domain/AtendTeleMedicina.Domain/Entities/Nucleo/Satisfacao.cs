using System;
using AtendTeleMedicina.Domain.Entities.Base;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class Satisfacao : EntidadeBase
    {
        #region Propriedades
        public int Avaliacao { get; set; }

        public string AplicacaoId { get; set; }

        #endregion

        #region Construtores
        public Satisfacao()
        {

        }
        #endregion
    }
}
