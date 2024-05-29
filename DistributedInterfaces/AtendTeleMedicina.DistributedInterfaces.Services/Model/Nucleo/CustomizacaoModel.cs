using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Infrastructure.CrossCutting.Notification.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class CustomizacaoModel : BaseModel
    {
        #region Propriedades
        public string Id { get; set; }

        public string Logomarca { get; set; }
        #endregion

    }
}
