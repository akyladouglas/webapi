using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
    public class ConfiguracaoService : IConfiguracaoService
    {
        private readonly IConfiguracaoRepository _configuracaoRepository;

        #region -- Constructor
        public ConfiguracaoService(IConfiguracaoRepository configuracaoRepository)
        {
            _configuracaoRepository = configuracaoRepository;
        }
        #endregion

        public Configuracao Get(Configuracao configFilters)
        {
            return _configuracaoRepository.Get(configFilters);
        }

        public int Update(Configuracao configuracao, string userId)
        {
            return _configuracaoRepository.Update(configuracao, userId);
        }

    }
}