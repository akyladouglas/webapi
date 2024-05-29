using AtendTeleMedicina.Domain.Entities.Nucleo;

namespace AtendTeleMedicina.Domain.Contracts.Services.Nucleo
{
    public interface IConfiguracaoService
    {
        Configuracao Get(Configuracao configFilters);
        int Update(Configuracao configuracao, string userId);
    }
}