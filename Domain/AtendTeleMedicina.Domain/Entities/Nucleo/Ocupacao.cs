using AtendTeleMedicina.Domain.Entities.Base;
using System;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class Ocupacao : EntidadeBase
    {
        #region Propriedades

        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataAlteracao { get; set; }

        #endregion
    }
}