using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo
{
    public interface IExamesF200Repository
    {
        int Add(ExamesF200 obj);
        int Update(string id, ExamesF200 obj);
        //int Delete(string id);
        ExamesF200 GetById(string id);
        Task<(IEnumerable<ExamesF200>, int)> GetByParam(ExamesF200 filters, string sort, int? skip, int? take);
        Task<(IEnumerable<ExamesF200>, int)> GetByCpfIndividuo(string cpf);
    }
}