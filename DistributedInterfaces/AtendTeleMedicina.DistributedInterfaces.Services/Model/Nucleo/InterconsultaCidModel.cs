using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.Domain.Enums;
using AtendTeleMedicina.Infrastructure.CrossCutting.Notification.Validations;
using System;
using System.Collections.Generic;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class InterconsultaCidModel : BaseModel
    {
        #region Propriedades
        public string InterconsultaId { get; set; }
        public string CidId { get; set; }
        public DateTime? DataCadastro { get; set; }
        public DateTime? DataAlteracao { get; set; }
        #endregion

        #region Métodos de Validações

        public void ValidarAdicionar()
        {
            AddNotifications(new Contract().Requires()
                .IsNotNullOrEmpty(InterconsultaId, "InterconsultaId", "O id da interconsulta é obrigatório")
                .IsNotNullOrEmpty(CidId, "CidId", "O id do cid é obrigatório"));
        }

        public void ValidarEditar()
        {
            AddNotifications(new Contract()
                .Requires()
                .IsNotNullOrEmpty(InterconsultaId, "InterconsultaId", "O id da interconsulta é obrigatório")
                .IsNotNullOrEmpty(CidId, "CidId", "O id do cid é obrigatório"));
        }
        #endregion

    }
}
