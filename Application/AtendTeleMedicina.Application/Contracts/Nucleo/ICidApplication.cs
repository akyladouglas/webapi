using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AtendTeleMedicina.Application.Contracts.Nucleo
{
    public interface ICidApplication
    {
        Task<(IEnumerable<Cid>, int)> GetByParam(Cid cidFilters, string sort,
          int? skip, int? take);
        Cid GetById(string id);
    }
}
