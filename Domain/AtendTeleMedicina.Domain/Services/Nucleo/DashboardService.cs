using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AtendTeleMedicina.Domain.Entities.Dashboard;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtendTeleMedicina.Domain.Services.Nucleo
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        #region -- Constructor
        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }
        #endregion

        public Task<(IEnumerable<DashboardParams>, int)> GetContadorAtendimentos(DashboardParams dashParams)
        {
            return _dashboardRepository.GetContadorAtendimentos(dashParams);
        }
        public Task<(IEnumerable<DashboardParams>, int)> GetContadorAtendimentosPacientes(DashboardParams dashParams)
        {
            return _dashboardRepository.GetContadorAtendimentosPacientes(dashParams);
        }
        

    }
}