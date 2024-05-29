using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo
{
    public interface IConfiguracaoRepository
    {
        Configuracao Get(Configuracao configFilters);
        int Update(Configuracao configuracao, string userId);
    }
}