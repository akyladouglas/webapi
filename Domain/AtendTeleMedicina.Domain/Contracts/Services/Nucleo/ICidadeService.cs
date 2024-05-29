using System.Collections.Generic;
using AtendTeleMedicina.Domain.Contracts.Services.Base;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Services.Nucleo
{
    public interface ICidadeService
    {
        int Add(Cidade obj);
        int Update(string id, Cidade obj, string userId);
        Cidade GetById(string id);
        Task<(IEnumerable<Cidade>, int)> GetByParam(Cidade filters, string sort,
            int? skip, int? take);
    }
}