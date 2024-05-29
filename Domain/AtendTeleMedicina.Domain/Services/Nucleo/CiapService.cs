using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
    public class CiapService : ICiapService
    {
        private readonly ICiapRepository _ciapRepository;

        #region -- Constructor
        public CiapService(ICiapRepository ciapRepository)
        {
            _ciapRepository = ciapRepository;
        }
        #endregion

        public Ciap GetById(string id)
        {
            return _ciapRepository.GetById(id);
        }
        public Task<(IEnumerable<Ciap>, int)> GetByParam(Ciap ciapFilters, string sort, int? skip, int? take)
        {
            return _ciapRepository.GetByParam(ciapFilters, sort, skip, take);
        }

        public Task<IEnumerable<Ciap>> GetByManyIds(string[] ciapIds)
        {
            return _ciapRepository.GetByManyIds(ciapIds);
        }
    }
}