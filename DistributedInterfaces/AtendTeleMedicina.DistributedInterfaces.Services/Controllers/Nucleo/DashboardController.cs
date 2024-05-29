using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using AtendTeleMedicina.Domain.Entities.Dashboard;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IAtendimentoApplication _atendimentoApplication;
        private readonly IIndividuoCiapApplication _individuoCiapApplication;
        private readonly IIndividuoCid10Application _individuoCid10Application;
        private readonly IAgendamentoApplication _agendamentoApplication;
        private readonly IDashboardApplication _dashboardApplication;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private readonly IUser _user;
        private readonly IMapper _mapper;
        private static IHostingEnvironment _environment;

        public DashboardController(IAtendimentoApplication atendimentoApplication, IIndividuoCiapApplication individuoCiapApplication, IIndividuoCid10Application individuoCid10Application, IAgendamentoApplication agendamentoApplication, IDashboardApplication dashboardApplication, IUser user, IMapper mapper, ILoggerFactory logger, IHostingEnvironment environment)
        {
            _atendimentoApplication = atendimentoApplication;
            _individuoCiapApplication = individuoCiapApplication;
            _individuoCid10Application = individuoCid10Application;
            _agendamentoApplication = agendamentoApplication;
            _dashboardApplication = dashboardApplication;
            _user = user;
            _mapper = mapper;
            _logger = logger.CreateLogger<AtendimentoController>();
            _environment = environment;
        }

        [HttpGet]
        [Route("ContadorAtendimentos")]
        [Authorize(Roles = "Retaguarda")]
        public async Task<IActionResult> GetContadorAtendimentos([FromQuery] DashboardParams dashParams)
        {

            try
            {
                var (dashboard, totalCount) = await _dashboardApplication.GetContadorAtendimentos(
                    _mapper.Map<DashboardParams>(dashParams));


                return Ok(dashboard);

            }
            catch (Exception e)
            {
                _logger.LogError($"ContadorAtividades: {dashParams}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }




        


    }
}
