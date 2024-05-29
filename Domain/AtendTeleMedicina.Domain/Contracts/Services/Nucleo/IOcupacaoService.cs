using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Services.Nucleo
{
    public interface IOcupacaoService
    {
        Task<Ocupacao> GetByCbo(string cbo);
        Task<(IEnumerable<Ocupacao>, int)> GetByParam(Ocupacao filters, bool cboEsus, string sort, int? skip, int? take);
    }
}