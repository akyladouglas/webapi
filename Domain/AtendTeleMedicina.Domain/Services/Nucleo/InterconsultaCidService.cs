using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
    public class InterconsultaCidService : IInterconsultaCidService
    {
        private readonly IInterconsultaCidRepository _interconsultaCidRepository;

        public InterconsultaCidService(IInterconsultaCidRepository interconsultaCidRepository)
        {
            _interconsultaCidRepository = interconsultaCidRepository;
        }

        public async Task<string> Post(InterconsultaCid cid)
        {
            cid.Id = Guid.NewGuid().ToString();
            cid.DataCadastro = DateTime.Now;
            return await _interconsultaCidRepository.Post(cid);
        }
    }
}
