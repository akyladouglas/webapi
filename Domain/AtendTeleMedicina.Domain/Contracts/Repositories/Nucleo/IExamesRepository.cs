using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo
{
    public interface IExamesRepository
    {
        int Add(Exames obj);
        int Count(Exames obj);
        int Update(string id, Exames obj);
        Task Delete(string id);
        Exames GetById(string id);
        Exames GetExame(Exames obj);
        Task<(IEnumerable<Exames>, int)> GetByParam(Exames filters, string sort, int? skip, int? take);
    }
}
