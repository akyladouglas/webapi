using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AtendTeleMedicina.Application.Contracts.Nucleo
{
    public interface IConfiguracaoApplication
    {
        Configuracao Get(Configuracao configFilters);
        int Update(Configuracao configuracao, string userId);
    }
}
