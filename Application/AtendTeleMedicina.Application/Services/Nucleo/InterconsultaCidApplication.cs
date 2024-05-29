using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Services.Nucleo
{
    public class InterconsultaCidApplication : IInterconsultaCidApplication
    {
        private readonly IInterconsultaCidService _interconsultaCidService;

        public InterconsultaCidApplication(IInterconsultaCidService interconsultaCidService)
        {
            _interconsultaCidService = interconsultaCidService;
        }

        public async Task<string> Post(InterconsultaCid cid)
        {
            return await _interconsultaCidService.Post(cid);
        }
    }
}
