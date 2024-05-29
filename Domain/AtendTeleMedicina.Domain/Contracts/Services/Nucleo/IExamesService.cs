using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Services.Nucleo
{
    public interface IExamesService
    {
        int Add(Exames obj);
        int Update(string id, Exames obj);
        Task Delete(string id);
        Exames GetById(string id);
        Exames GetExame(Exames obj);
        Task<(IEnumerable<Exames>, int)> GetByParam(Exames filters, string sort, int? skip, int? take);
        int Count(Exames obj);
    }
}
