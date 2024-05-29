using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Params;

namespace AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo
{
  public interface IIndividuoProcedimentoAutorizacaoRepository
  {
    void Add(IEnumerable<IndividuoProcedimentoAutorizacao> list);
    void Delete(IEnumerable<IndividuoProcedimentoAutorizacao> list);
  }
}