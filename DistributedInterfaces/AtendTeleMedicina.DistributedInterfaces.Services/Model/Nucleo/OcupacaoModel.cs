using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using System;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class OcupacaoModel : BaseModel
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
