using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
    public class InterconsultaCiapService : IInterconsultaCiapService
    {
        private readonly IInterconsultaCiapRepository _interconsultaCiapRepository;

        public InterconsultaCiapService(IInterconsultaCiapRepository interconsultaCiapRepository)
        {
            _interconsultaCiapRepository = interconsultaCiapRepository;
        }

        public async Task<string> Post(InterconsultaCiap ciap)
        {
            ciap.Id = Guid.NewGuid().ToString();
            ciap.DataCadastro = DateTime.Now;
            return await _interconsultaCiapRepository.Post(ciap);
        }
    }
}
