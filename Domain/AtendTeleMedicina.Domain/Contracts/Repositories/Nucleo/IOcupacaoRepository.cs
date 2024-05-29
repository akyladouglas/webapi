using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo
{
    public interface IOcupacaoRepository
    {
        Task<Ocupacao> GetByCbo(string cbo);
        Task<(IEnumerable<Ocupacao>, int)> GetByParam(Ocupacao filters, bool cboEsus, string sort, int? skip, int? take);
    }
}