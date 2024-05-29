using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Services.Nucleo
{
    public class PaisApplication : IPaisApplication
    {
        private readonly IPaisService _paisService;
        #region -- Constructor
        public PaisApplication(IPaisService paisService)
        {
            _paisService = paisService;
        }
        #endregion
        public Task<(IEnumerable<Pais>, int)> GetByParam(Pais filters, string sort, int? skip, int? take)
        {
            return _paisService.GetByParam(filters, sort, skip, take);
        }
    }
}
