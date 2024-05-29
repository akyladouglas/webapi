using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Services.Nucleo;
using AtendTeleMedicina.Domain.Entities.Dashboard;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AtendTeleMedicina.Application.Services.Nucleo
{
    public class DashboardApplication : IDashboardApplication
    {
        private readonly IDashboardService _dashboardService;
        #region -- Constructor
        public DashboardApplication(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        #endregion
        public Task<(IEnumerable<DashboardParams>, int)> GetContadorAtendimentos(DashboardParams dashParams)
        {
            return _dashboardService.GetContadorAtendimentos(dashParams);
        }

        public Task<(IEnumerable<DashboardParams>, int)> GetContadorAtendimentosPacientes(DashboardParams dashParams)
        {
            return _dashboardService.GetContadorAtendimentosPacientes(dashParams);
        }
       
    }
}
