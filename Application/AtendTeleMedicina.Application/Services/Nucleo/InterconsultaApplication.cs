using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Services.Nucleo
{
    public class InterconsultaApplication : IInterconsultaApplication
    {
        private readonly IInterconsultaService _interconsultaService;

        public InterconsultaApplication(IInterconsultaService interconsultaService)
        {
            _interconsultaService = interconsultaService;
        }

        public async Task<string> Post(Interconsulta interconsulta)
        {
            return await _interconsultaService.Post(interconsulta);
        }
    }
}
