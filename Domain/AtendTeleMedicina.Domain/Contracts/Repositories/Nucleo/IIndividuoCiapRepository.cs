using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Params;

namespace AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo
{
  public interface IIndividuoCiapRepository
    {
    void Add(IEnumerable<IndividuoCiap> list);
    void AddTriagem(IEnumerable<IndividuoCiap> list);
    int Update(string id, IndividuoCiap obj);
    Individuo GetById(string individuoId);
    Task<(IEnumerable<IndividuoCiap>, int)> GetByParam(IndividuoCiap filters, string sort, int? skip, int? take);
  }
}