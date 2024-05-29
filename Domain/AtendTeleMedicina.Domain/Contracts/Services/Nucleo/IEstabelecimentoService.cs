using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Services.Nucleo
{
  public interface IEstabelecimentoService
  {
    int Add(Estabelecimento obj);
    int Update(string id, Estabelecimento obj);
    int Delete(string id, bool? ativo);
    Estabelecimento GetById(string id);
    Task<(IEnumerable<Estabelecimento>, int)> GetByParam(Estabelecimento filters, string sort, int? skip, int? take);
    int DistribuirCota(string estabelecimentoId, string procedimentoId, int quantidade, string usuario);

  }
}