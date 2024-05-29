using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
    public class InterconsultaService : IInterconsultaService
    {
        private readonly IInterconsultaRepository _interconsultaRepository;

        public InterconsultaService(IInterconsultaRepository interconsultaRepository)
        {
            _interconsultaRepository = interconsultaRepository;
        }

        public async Task<string> Post(Interconsulta interconsulta)
        {
            interconsulta.Id = Guid.NewGuid().ToString();
            interconsulta.DataCadastro = DateTime.Now;
            return await _interconsultaRepository.Post(interconsulta);
        }
    }
}
