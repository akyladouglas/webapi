using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo
{
    public interface ICiapRepository
    {
        Ciap GetById(string id);
        Task<(IEnumerable<Ciap>, int)> GetByParam(Ciap ciapFilters, string sort,
          int? skip, int? take);
        Task<IEnumerable<Ciap>> GetByManyIds(string[] ciapIds);
    }
}