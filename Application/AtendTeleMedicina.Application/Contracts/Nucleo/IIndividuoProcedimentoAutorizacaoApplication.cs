using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Params;

namespace AtendTeleMedicina.Application.Contracts.Nucleo
{
  public interface IIndividuoProcedimentoAutorizacaoApplication
  {
    //Task<(IEnumerable<EstabelecimentoProcedimentoHorario>, int)> GetByParam(EstabelecimentoProcedimentoHorario filters, string sort, int? skip, int? take);
    void Add(IEnumerable<IndividuoProcedimentoAutorizacao> list);
    void Delete(IEnumerable<IndividuoProcedimentoAutorizacao> list);
    }
}