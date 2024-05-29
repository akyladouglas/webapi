 using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtendTeleMedicina.Application.Contracts.Nucleo
{
    public interface IIndividuoNotificacaoApplication
    {
        int Add(IndividuoNotificacao obj);
    }
}