using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Services.Helpers;

namespace AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo
{
    public interface IPaisRepository
    {
        Task<(IEnumerable<Pais>, int)> GetByParam(Pais filters, string sort, int? skip, int? take);
    }
}