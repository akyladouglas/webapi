using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AtendTeleMedicina.Application.Services.Nucleo
{
    public class CidApplication : ICidApplication
    {
        private readonly ICidService _cidService;
        #region -- Constructor
        public CidApplication(ICidService cidService)
        {
            _cidService = cidService;
        }
        #endregion

        public Cid GetById(string id)
        {
            return _cidService.GetById(id);
        }

        public Task<(IEnumerable<Cid>, int)> GetByParam(Cid cidFilter, string sort, int? skip, int? take)
        {
            return _cidService.GetByParam(cidFilter, sort, skip, take);
        }

    }
}
