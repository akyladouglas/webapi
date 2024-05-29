
using System;
using System.Collections.Generic;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Security;
using AtendTeleMedicina.Domain.Enums;
using AtendTeleMedicina.Infrastructure.CrossCutting.Notification.Validations;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class IndividuoCid10Model : BaseModel
    {
        public string Id { get; set; }
        public string IndividuoId { get; set; }
        public IndividuoModel Individuo { get; set; }
        public string ProfissionalId { get; set; }
        public ProfissionalModel Profissional { get; set; }
        public string AgendamentoId { get; set; }
        public AgendamentoModel Agendamento { get; set; }
        public int Cid10Id { get; set; }
        public CidModel Cid { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataAlteracao { get; set; }
        public bool AtendidoTriagem { get; set; }
        public bool AtendidoMedico { get; set; }

        public void ValidarAdicionar()
        {
            ValidarString();
            AddNotifications(new Contract()
                .Requires());
        }

        public void ValidarEditar()
        {
            ValidarString();
            AddNotifications(new Contract()
                .Requires()
                .IsNotNullOrEmpty(IndividuoId, "IndividuoId", "O Id do Individuo é Obrigatório")
                .IsNotNullOrEmpty(ProfissionalId, "ProfissionalId", "O Id do Profissional é Obrigatório"));
        }

        public void ValidarString()
        {
            IndividuoId = ValidateText(IndividuoId, 36);
            ProfissionalId = ValidateText(ProfissionalId, 36);
        }

        

        public void ValidarGet()
        {
            ValidarString();
        }
    }
}
