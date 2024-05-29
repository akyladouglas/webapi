using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo
{
    public interface IIndividuoNotificacaoRepository
    {
        int Add(IndividuoNotificacao obj);
    }
}
