using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Params;

namespace AtendTeleMedicina.Application.Contracts.Nucleo
{
  public interface IEstabelecimentoProfissionalApplication
    {
    void Add(IEnumerable<EstabelecimentoProfissional> list);
    int Update(string id, EstabelecimentoProfissional obj);
    Estabelecimento GetById(string estabelecimentoId);
    Task<(IEnumerable<EstabelecimentoProfissional>, int)> GetByParam(EstabelecimentoProfissional filters, AppParams appParas, string sort, int? skip, int? take);
    }
}
