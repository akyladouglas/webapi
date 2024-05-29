using AtendTeleMedicina.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class IndividuoNotificacao : EntidadeBase
    {
        public string NotificacaoId { get; set; }
        public string IndividuoId { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}
