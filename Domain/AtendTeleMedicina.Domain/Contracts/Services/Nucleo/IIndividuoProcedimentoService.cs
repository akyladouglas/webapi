using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Params;

namespace AtendTeleMedicina.Domain.Contracts.Services.Nucleo
{
  public interface IIndividuoProcedimentoService
  {
    //void Add(IEnumerable<IndividuoProcedimento> list);
    void AddTriagem(IEnumerable<IndividuoProcedimento> list);
    //int Update(string id, IndividuoProcedimento obj);
    //Individuo GetById(string individuoId);
    Task<(IEnumerable<IndividuoProcedimento>, int)> GetByParam(IndividuoProcedimento filters, string sort, int? skip, int? take);
  }
}