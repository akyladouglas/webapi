using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AtendTeleMedicina.Application.Contracts.Nucleo
{
    public interface IUnidadeFederativaApplication
    {
        Task<(IEnumerable<UnidadeFederativa>, int)> GetByParam(UnidadeFederativa unidadeFederativaFilters, string sort,
          int? skip, int? take);
        UnidadeFederativa GetById(string id);
    }
}
