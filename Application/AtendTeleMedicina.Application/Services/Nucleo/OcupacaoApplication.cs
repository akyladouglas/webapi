using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Params;
using System.Collections.Generic;
using System.Threading.Tasks;
using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;

namespace AtendTeleMedicina.Application.Services.Nucleo
{
    public class OcupacaoApplication : IOcupacaoApplication
    {
        private readonly IOcupacaoService _ocupacaoService;

        #region Construtor

        public OcupacaoApplication(IOcupacaoService ocupacaoService)
        {
            _ocupacaoService = ocupacaoService;
        }

        #endregion
        public async Task<Ocupacao> GetByCbo(string cbo)
        {
            return await _ocupacaoService.GetByCbo(cbo);
        }

        public async Task<(IEnumerable<Ocupacao>, int)> GetByParam(Ocupacao filter, bool cboEsus, string sort, int? skip, int? take)
        {
            return await _ocupacaoService.GetByParam(filter, cboEsus, sort, skip, take);
        }
    }
}