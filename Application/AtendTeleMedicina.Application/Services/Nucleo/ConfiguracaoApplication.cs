using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AtendTeleMedicina.Application.Services.Nucleo
{
    public class ConfiguracaoApplication : IConfiguracaoApplication
    {
        private readonly IConfiguracaoService _configuracaoService;
        #region -- Constructor
        public ConfiguracaoApplication(IConfiguracaoService configuracaoService)
        {
            _configuracaoService = configuracaoService;
        }
        #endregion

        public Configuracao Get(Configuracao configFilters)
        {
            return _configuracaoService.Get(configFilters);
        }

        public int Update(Configuracao configuracao, string userId)
        {
            return _configuracaoService.Update(configuracao, userId);
        }

    }
}
