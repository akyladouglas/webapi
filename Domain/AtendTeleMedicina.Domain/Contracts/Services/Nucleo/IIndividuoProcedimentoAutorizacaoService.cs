using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Services.Nucleo
{
  public interface IIndividuoProcedimentoAutorizacaoService
  {
    void Add(IEnumerable<IndividuoProcedimentoAutorizacao> obj);
    void Delete(IEnumerable<IndividuoProcedimentoAutorizacao> obj);
  }
}