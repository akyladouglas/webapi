using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Entities.Dashboard;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo
{
    public interface IDashboardRepository
    {
        Task<(IEnumerable<DashboardParams>, int)> GetContadorAtendimentos(DashboardParams dashParams);
        Task<(IEnumerable<DashboardParams>, int)> GetContadorAtendimentosPacientes(DashboardParams dashParams);
        

    }
}