using System.Collections.Generic;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
    public class OcupacaoService : IOcupacaoService
    {
        #region Injecao de dependencia e construtor

        private readonly IOcupacaoRepository _ocupacaoRepository;

        public OcupacaoService(IOcupacaoRepository ocupacaoRepository)
        {
            _ocupacaoRepository = ocupacaoRepository;
        }

        #endregion

        public async Task<Ocupacao> GetByCbo(string cbo)
        {
            return await _ocupacaoRepository.GetByCbo(cbo);
        }

        public async Task<(IEnumerable<Ocupacao>, int)> GetByParam(Ocupacao filters, bool cboEsus, string sort, int? skip, int? take)
        {
            return await _ocupacaoRepository.GetByParam(filters, cboEsus, sort, skip, take);
        }
    }
}