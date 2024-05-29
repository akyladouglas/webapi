using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Services.Nucleo
{
    public interface IBairroService
    {
        int Add(Bairro obj);
        int Update(string id, Bairro obj);
        int Delete(string id);
        Bairro GetById(string id);
        Task<(IEnumerable<Bairro>, int)> GetByParam(Bairro filters, string sort,
            int? skip, int? take);
    }
}