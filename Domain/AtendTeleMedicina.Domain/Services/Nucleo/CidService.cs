using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
    public class CidService : ICidService
    {
        private readonly ICidRepository _cidRepository;

        #region -- Constructor
        public CidService(ICidRepository cidRepository)
        {
            _cidRepository = cidRepository;
        }
        #endregion

        public Cid GetById(string id)
        {
            return _cidRepository.GetById(id);
        }
        public Task<(IEnumerable<Cid>, int)> GetByParam(Cid cidFilters, string sort, int? skip, int? take)
        {
            return _cidRepository.GetByParam(cidFilters, sort, skip, take);
        }


    }
}