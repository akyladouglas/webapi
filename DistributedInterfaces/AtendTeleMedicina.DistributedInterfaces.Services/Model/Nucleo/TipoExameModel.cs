using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.Infrastructure.CrossCutting.Notification.Validations;
using System;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class TipoExameModel : BaseModel
    {
        #region Propriedades
        public string Id { get; set; }
        public string Nome { get; set; }
       
        #endregion
    }
}
