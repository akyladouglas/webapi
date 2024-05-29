using AtendTeleMedicina.Domain.Entities.Dashboard;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Application.Contracts.Nucleo
{
    public interface IDashboardApplication
    {
        Task<(IEnumerable<DashboardParams>, int)> GetContadorAtendimentos(DashboardParams dashParams);
        Task<(IEnumerable<DashboardParams>, int)> GetContadorAtendimentosPacientes(DashboardParams dashParams);
        
    }
}
