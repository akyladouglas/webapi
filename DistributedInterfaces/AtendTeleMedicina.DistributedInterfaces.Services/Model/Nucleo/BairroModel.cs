using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.Infrastructure.CrossCutting.Notification.Validations;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class BairroModel : BaseModel
    {
        #region Propriedades
        public string Id { get; set; }
        public string Nome { get; set; }
        public virtual CidadeModel Cidade { get; set; }
        public int CidadeId { get; set; }
        public bool Ativo { get; set; }
        #endregion

        public void ValidarGet()
        {
            ValidarString();
        }

        public void ValidarString()
        {
            Nome = ValidateText(Nome, 120);
        }

        public void ValidarAdicionar()
        {
            ValidarString();
            AddNotifications(new Contract()
                .Requires()
                .IsNotNullOrEmpty(Nome, "bairro.Nome", "Nome inválido"));
        }

        public void ValidarEditar()
        {
            ValidarString();
            AddNotifications(new Contract()
                .Requires()
                .IsNotNullOrEmpty(Nome, "bairro.Nome", "Nome inválido"));
        }
    }
}
