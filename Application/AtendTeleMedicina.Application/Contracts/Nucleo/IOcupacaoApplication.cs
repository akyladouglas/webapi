using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Params;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Contracts.Nucleo
{
    public interface IOcupacaoApplication
    {
        Task<Ocupacao> GetByCbo(string cbo);
        Task<(IEnumerable<Ocupacao>, int)> GetByParam(Ocupacao filters, bool cboEsus, string sort, int? skip, int? take);
    }
}