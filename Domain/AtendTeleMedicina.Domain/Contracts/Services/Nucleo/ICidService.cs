using System.Collections.Generic;
using AtendTeleMedicina.Domain.Contracts.Services.Base;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Services.Nucleo
{
    public interface ICidService
    {
        Cid GetById(string id);
        Task<(IEnumerable<Cid>, int)> GetByParam(Cid cidFilters, string sort,
            int? skip, int? take);
    }
}