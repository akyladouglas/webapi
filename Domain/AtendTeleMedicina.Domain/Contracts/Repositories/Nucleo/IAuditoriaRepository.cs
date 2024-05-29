using System.Collections.Generic;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Params;

namespace AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo
{
  public interface IAuditoriaRepository
  {
    Auditoria GetById(string id);
    Task<(IEnumerable<Auditoria>, int)> GetByParam(Auditoria filters, AppParams appParams, string sort, int? skip, int? take);
  }
}
