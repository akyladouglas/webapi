using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Params;

namespace AtendTeleMedicina.Application.Contracts.Nucleo
{
  public interface IEstabelecimentoProcedimentoApplication
  {
    void Add(IEnumerable<EstabelecimentoProcedimento> list);
    int Update(string id, EstabelecimentoProcedimento obj);
    Estabelecimento GetById(string estabelecimentoId);
    Task<(IEnumerable<EstabelecimentoProcedimento>, int)> GetByParam(EstabelecimentoProcedimento filters, AppParams appParas, string sort, int? skip, int? take);
    void Delete(IEnumerable<EstabelecimentoProcedimento> list);
  }
}
