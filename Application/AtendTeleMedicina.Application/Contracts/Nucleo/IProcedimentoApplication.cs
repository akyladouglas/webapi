using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Params;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Contracts.Nucleo
{
  public interface IProcedimentoApplication
  {
    int Add(Procedimento obj);
    int Update(string id, Procedimento obj);
    int Delete(string id);
    Procedimento GetById(string id);
    Task<(IEnumerable<Procedimento>, int)> GetByParam(Procedimento filters, AppParams appParams, string sort, int? skip, int? take);

    void AdicionarCota(string id, int quantidade, string usuario);
    void DistribuirCota(string procedimentoId, string estabelecimentoId, int quantidade, string usuario);
    void DistribuirCotaExecutor(string procedimentoId, string estabelecimentoId, int quantidade, string usuario);
    void SubtrairCota(string id, int quantidade, string usuario);
    void UpdateCotaProfissional(string id, int quantidade, string usuario);
  }
}
