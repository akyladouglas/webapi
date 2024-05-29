using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AtendTeleMedicina.Application.Contracts.Nucleo
{
    public interface ICiapApplication
    {
        Task<(IEnumerable<Ciap>, int)> GetByParam(Ciap cidFilters, string sort,
          int? skip, int? take);
        Ciap GetById(string id);
        Task<IEnumerable<Ciap>> GetByManyIds(string[] ciapIds);
    }
}
