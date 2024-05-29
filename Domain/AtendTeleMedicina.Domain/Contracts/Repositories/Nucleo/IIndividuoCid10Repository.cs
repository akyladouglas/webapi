using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Params;

namespace AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo
{
  public interface IIndividuoCid10Repository
    {
    void Add(IEnumerable<IndividuoCid10> list);
    void AddTriagem(IEnumerable<IndividuoCid10> list);
    int Update(string id, IndividuoCid10 obj);
    Individuo GetById(string individuoId);
    Task<(IEnumerable<IndividuoCid10>, int)> GetByParam(IndividuoCid10 filters, string sort, int? skip, int? take);
  }
}