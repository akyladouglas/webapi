using System.Collections.Generic;
using AtendTeleMedicina.Domain.Contracts.Services.Base;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Services.Nucleo
{
    public interface ICiapService
    {
        Ciap GetById(string id);
        Task<(IEnumerable<Ciap>, int)> GetByParam(Ciap ciapFilters, string sort,
            int? skip, int? take);
        Task<IEnumerable<Ciap>> GetByManyIds(string[] ciapIds);
    }
}