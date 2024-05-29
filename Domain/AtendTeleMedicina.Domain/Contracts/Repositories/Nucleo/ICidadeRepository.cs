using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo
{
    public interface ICidadeRepository
    {
        int Add(Cidade cidade);
        int Update(string id, Cidade cidade, string userId);
        Cidade GetById(string id);
        Task<(IEnumerable<Cidade>, int)> GetByParam(Cidade cidadeFilters, string sort,
          int? skip, int? take);
    }
}