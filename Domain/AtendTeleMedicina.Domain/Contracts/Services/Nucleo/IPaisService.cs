using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Services.Nucleo
{
  public interface IPaisService
  {
    Task<(IEnumerable<Pais>, int)> GetByParam(Pais filters, string sort, int? skip, int? take);
  }
}