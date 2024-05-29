using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo
{
    public interface IDocumentoRepository
    {
        int Add(Documento obj);
        int Update(string id, Documento obj);
        int Delete(string id);
        Documento GetById(string id);
        Task<(IEnumerable<Documento>, int)> GetByParam(Documento filters, string sort, int? skip, int? take);
    }
}
