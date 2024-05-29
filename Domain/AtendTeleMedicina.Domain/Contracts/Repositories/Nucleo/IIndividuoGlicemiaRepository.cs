using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo
{
    public interface IIndividuoGlicemiaRepository
    {
        string Add(IndividuoGlicemia obj);
        string Update(string id, IndividuoGlicemia obj);
        IndividuoGlicemia GetById(string id);
        Task<(IEnumerable<IndividuoGlicemia>, int)> GetByParam(IndividuoGlicemia filters, string sort, int? skip, int? take);
    }
}
