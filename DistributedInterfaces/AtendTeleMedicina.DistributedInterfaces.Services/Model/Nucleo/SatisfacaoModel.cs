using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Infrastructure.CrossCutting.Notification.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class SatisfacaoModel : BaseModel
    {
        #region Propriedades
        public string Id { get; set; }

        public int Avaliacao { get; set; }

        public string AplicacaoId { get; set; }

        #endregion

    }
}
