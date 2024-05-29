using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Infrastructure.CrossCutting.Notification.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class NotificacaoModel : BaseModel
    {
        #region Propriedades
        public string Id { get; set; }

        public string Titulo { get; set; }

        public string Descricao { get; set; }

        public DateTime DataCadastro { get; set; }

        public DateTime DataAlteracao { get; set; }

        public bool Deletado { get; set; }
        public string[] IndividuoIds { get; set; }


        public string IndividuoId { get; set; }

        public Individuo Individuo { get; set; }
        #endregion


        public void ValidarAdicionar()
        {
            ValidarString();
            AddNotifications(new Contract()
                .Requires()
                .IsNotNullOrEmpty(Titulo, "Titulo", "O Campo Titulo é Obrigatório"));
        }

        public void ValidarEditar()
        {
            ValidarString();
            AddNotifications(new Contract()
                .Requires()
                .IsNotNullOrEmpty(Titulo, "Titulo", "O Campo Titulo é Obrigatório"));
        }

        public void ValidarString()
        {
            Titulo = ValidateText(Titulo, 255);
            Descricao = ValidateText(Descricao, 255);
        }

        public void ValidarGet()
        {
            ValidarString();
        }
    }
}
