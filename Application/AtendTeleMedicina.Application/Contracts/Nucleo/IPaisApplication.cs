using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Contracts.Nucleo
{
  public interface IPaisApplication
  {
    Task<(IEnumerable<Pais>, int)> GetByParam(Pais filters, string sort, int? skip, int? take);
  }
}
