using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.Domain.Entities.Security;
using AtendTeleMedicina.Infrastructure.CrossCutting.Notification.Validations;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class ProfissionalMemedModel : BaseModel
    {
        #region Propriedades
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Cpf { get; set; }
        public string Conselho { get; set; }
        public string Crm { get; set; }
        public string CrmUF { get; set; }
        public string Uf { get; set; }
        public DateTime DataNascimento { get; set; }

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
                .IsNotNullOrEmpty(Nome, "Profissional.Nome", "Nome inválido"));
        }

        public void ValidarEditar()
        {
            ValidarString();
            AddNotifications(new Contract()
                .Requires()
                .IsNotNullOrEmpty(Nome, "Profissional.Nome", "Nome inválido"));
        }
    }
}
