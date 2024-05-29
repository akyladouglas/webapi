using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AtendTeleMedicina.Application.Services.Nucleo
{
    public class CiapApplication : ICiapApplication
    {
        private readonly ICiapService _ciapService;
        #region -- Constructor
        public CiapApplication(ICiapService ciapService)
        {
            _ciapService = ciapService;
        }
        #endregion

        public Ciap GetById(string id)
        {
            return _ciapService.GetById(id);
        }

        public Task<(IEnumerable<Ciap>, int)> GetByParam(Ciap ciapFilter, string sort, int? skip, int? take)
        {
            return _ciapService.GetByParam(ciapFilter, sort, skip, take);
        }

        public Task<IEnumerable<Ciap>> GetByManyIds(string[] ciapIds)
        {
            return _ciapService.GetByManyIds(ciapIds);
        }

    }
}
