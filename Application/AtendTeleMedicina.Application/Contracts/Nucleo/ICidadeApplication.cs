using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AtendTeleMedicina.Application.Contracts.Nucleo
{
    public interface ICidadeApplication
    {
        int Add(Cidade cidade);
        int Update(string id, Cidade cidade, string userId);
        Task<(IEnumerable<Cidade>, int)> GetByParam(Cidade cidadeFilters, string sort,
          int? skip, int? take);
        Cidade GetById(string id);
    }
}
