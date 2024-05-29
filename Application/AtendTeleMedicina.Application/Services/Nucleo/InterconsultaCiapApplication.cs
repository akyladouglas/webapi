using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Services.Nucleo
{
    public class InterconsultaCiapApplication : IInterconsultaCiapApplication
    {
        private readonly IInterconsultaCiapService _interconsultaCiapService;

        public InterconsultaCiapApplication(IInterconsultaCiapService interconsultaCiapService)
        {
            _interconsultaCiapService = interconsultaCiapService;
        }

        public async Task<string> Post(InterconsultaCiap ciap)
        {
            return await _interconsultaCiapService.Post(ciap);
        }
    }
}
