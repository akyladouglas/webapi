using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Params;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Contracts.Nucleo
{
  public interface IAuditoriaApplication
  {
    Auditoria GetById(string id);
    Task<(IEnumerable<Auditoria>, int)> GetByParam(Auditoria filters, AppParams appParams, string sort, int? skip, int? take);
  }
}
