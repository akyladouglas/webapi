using System.Collections.Generic;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Entities.Nucleo;

namespace AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo
{
    public interface ISatisfacaoRepository
    {
        int Add(Satisfacao satisfacao);

    }
}
