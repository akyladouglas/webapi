using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo
{
    public interface IAvaliacaoRepository
    {
        int Add(Avaliacao obj);

    }
}
