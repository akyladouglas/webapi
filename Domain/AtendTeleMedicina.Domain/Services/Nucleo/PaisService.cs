using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Services.Helpers;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
    public class PaisService : IPaisService
    {
        private readonly IPaisRepository _paisRepository;

        #region -- Constructor
        public PaisService(IPaisRepository paisRepository)
        {
            _paisRepository = paisRepository;
        }
        #endregion
        public Task<(IEnumerable<Pais>, int)> GetByParam(Pais filters, string sort, int? skip, int? take)
        {
            return _paisRepository.GetByParam(filters, sort, skip, take);
        }
    }
}