using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo
{
    public interface ICidRepository
    {
        Cid GetById(string id);
        Task<(IEnumerable<Cid>, int)> GetByParam(Cid cidFilters, string sort,
          int? skip, int? take);
    }
}