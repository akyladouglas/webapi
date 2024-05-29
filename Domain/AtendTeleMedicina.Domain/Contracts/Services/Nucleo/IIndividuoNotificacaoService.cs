using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtendTeleMedicina.Domain.Contracts.Services.Nucleo
{
    public interface IIndividuoNotificacaoService
    {
        int Add(IndividuoNotificacao obj);
    }
}
