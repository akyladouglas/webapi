using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo
{
    public interface IUnidadeFederativaRepository
    {
        UnidadeFederativa GetById(string id);
        Task<(IEnumerable<UnidadeFederativa>, int)> GetByParam(UnidadeFederativa cidadeFilters, string sort,
          int? skip, int? take);
    }
}