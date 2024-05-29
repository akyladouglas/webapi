using AtendTeleMedicina.Domain.Entities.Base;
using AtendTeleMedicina.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class InterconsultaCiap : EntidadeBase
    {
        #region Propriedades
        public string InterconsultaId { get; set; }
        public string CiapId { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataAlteracao { get; set; }
        #endregion

        #region Construtor
        public InterconsultaCiap()
        {

        }
        #endregion
    }
}
