using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
    public class UnidadeFederativaService : IUnidadeFederativaService
    {
        private readonly IUnidadeFederativaRepository _cidadeRepository;

        #region -- Constructor
        public UnidadeFederativaService(IUnidadeFederativaRepository cidadeRepository)
        {
            _cidadeRepository = cidadeRepository;
        }
        #endregion

        public UnidadeFederativa GetById(string id)
        {
            return _cidadeRepository.GetById(id);
        }
        public Task<(IEnumerable<UnidadeFederativa>, int)> GetByParam(UnidadeFederativa cidadeFilters, string sort, int? skip, int? take)
        {
            return _cidadeRepository.GetByParam(cidadeFilters, sort, skip, take);
        }


    }
}