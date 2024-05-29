using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Authorization;
using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo;
using System.Collections.Generic;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Params;
using System.Threading.Tasks;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/profissionais/")]
    [Produces("application/json")]
    [Authorize]
    public class ProfissionalEscalasController : Controller
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IProfissionalApplication _profissionalApplication;



        public ProfissionalEscalasController(IProfissionalApplication profissionalApplication,
                                                    IMapper mapper,
                                                    ILogger<ProfissionaisProcedimentosController> logger)
        {
            _profissionalApplication = profissionalApplication;
            _mapper = mapper;
            _logger = logger;
        }

        // <summary>
        // Retorna as Escalas do Profissional por período.
        // </summary>
        // <param name = "dataInicial"></ param >
        // <param name = "dataFinal"></ param >
        [HttpGet("escalas")]
        [Authorize(Roles = "Retaguarda, App")]
        public async Task<IActionResult> Get([FromQuery] AppParams filters)
        {
            if (filters == null) return BadRequest();
            if (!filters.DataInicial.HasValue || !filters.DataFinal.HasValue) return BadRequest("Obrigatório a seleção de Data Inicial e Data Final");

            try
            {
                var list = await _profissionalApplication.GetProfissionalHorarios(filters).ConfigureAwait(false);
                if (list == null) return NoContent();

                return Ok(list);
            }
            catch (Exception e)
            {
                _logger.LogError($"ProfissionalEscalasGet: {DateTime.Now}, {filters}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        // <summary>
        // Retorna as Escalas do Profissional por período.
        // </summary>
        // <param name = "profissionalId"></ param >
        // <param name = "dataInicial"></ param >
        // <param name = "dataFinal"></ param >
        [HttpGet("{profissionalId}/escalas")]
        [Authorize(Roles = "Retaguarda, App")]
        public IActionResult GetById(AppParams filters)
        {
            try
            {
                var list = _mapper.Map<RelatorioProfissionalEscala>(_profissionalApplication.GetProfissionalHorarios(filters));

                if (list == null) return NoContent();

                return Ok(list);
            }
            catch (Exception e)
            {
                _logger.LogError($"ProfissionalEscalasGet: {DateTime.Now}, {filters}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }
    }
}
