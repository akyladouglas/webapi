using System.Collections.Generic;
using AtendTeleMedicina.Domain.Contracts.Services.Base;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Services.Nucleo
{
    public interface IUnidadeFederativaService
    {
        UnidadeFederativa GetById(string id);
        Task<(IEnumerable<UnidadeFederativa>, int)> GetByParam(UnidadeFederativa cidadeFilters, string sort,
            int? skip, int? take);
    }
}