using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Params;

namespace AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo
{
  public interface IEstabelecimentoProfissionalRepository
  {
    void Add(IEnumerable<EstabelecimentoProfissional> list);
    int Update(string id, EstabelecimentoProfissional obj);
    Estabelecimento GetById(string profissionalId);
    Task<(IEnumerable<EstabelecimentoProfissional>, int)> GetByParam(EstabelecimentoProfissional filters, AppParams appParas, string sort, int? skip, int? take);
  }
}