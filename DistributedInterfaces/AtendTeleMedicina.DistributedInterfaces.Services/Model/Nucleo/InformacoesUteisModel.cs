
using System;
using System.Collections.Generic;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Security;
using AtendTeleMedicina.Domain.Enums;
using AtendTeleMedicina.Infrastructure.CrossCutting.Notification.Validations;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class InformacoesUteisModel : BaseModel
    {
        public string Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataAlteracao { get; set; }
        public bool Ativo { get; set; }



        public void ValidarAdicionar()
        {
            ValidarString();
            AddNotifications(new Contract()
                .Requires()
                .IsNotNullOrEmpty(Titulo, "Titulo", "O Campo Titulo é Obrigatório")
                .IsNotNullOrEmpty(Descricao, "Descricao", "O Campo Descricao é Obrigatório"));
            
        }

        public void ValidarEditar()
        {
            ValidarString();
            AddNotifications(new Contract()
                .Requires()
                .IsNotNullOrEmpty(Titulo, "Titulo", "O Campo Titulo é Obrigatório")
                .IsNotNullOrEmpty(Descricao, "Descricao", "O Campo Descricao é Obrigatório"));

        }

        public void ValidarString()
        {
            Titulo = ValidateText(Titulo, 255);
            Descricao = ValidateText(Descricao, 99999);
        }

        public void ValidarGet()
        {
            ValidarString();
        }
    }
}
