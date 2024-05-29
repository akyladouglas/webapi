using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Authorization;
using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo;
using System.Collections.Generic;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Params;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Profissionais/")]
    [Produces("application/json")]
    [Authorize]
    public class ProfissionaisProcedimentosController : Controller
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IProfissionalApplication _profissionalApplication;



        public ProfissionaisProcedimentosController(IProfissionalApplication profissionalApplication,
                                                    IMapper mapper,
                                                    ILogger<ProfissionaisProcedimentosController> logger)
        {
            _profissionalApplication = profissionalApplication;
            _mapper = mapper;
            _logger = logger;
        }


        // <summary>
        // Retorna o Profissional com seus Procedimentos.
        // </summary>
        // <param name = "profissionalId"></ param >
        [HttpGet("{profissionalId}/Procedimentos")]
        [Authorize(Roles = "Retaguarda, App")]
        public IActionResult Get(string profissionalId)
        {
            try
            {
                var profissional = _mapper.Map<ProfissionalModel>(_profissionalApplication.GetById(profissionalId));

                if (profissional == null) return NotFound();

                return Ok(profissional);
            }
            catch (Exception e)
            {
                _logger.LogError($"ProfissionalEstabelecimentosGet: {DateTime.Now}, {profissionalId}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        [HttpGet("Procedimentos")]
        [Authorize(Roles = "Retaguarda, Individuo")]
        public async Task<IActionResult> Get([FromQuery] AppParams filters, string sort = "p.Nome", int skip = 1, int take = 10)
        {
            try
            {
                var list = await _profissionalApplication.GetProfissionalProcedimentos(filters, sort, skip, take).ConfigureAwait(false);
                if (list == null) return NoContent();

                return Ok(list);
            }
            catch (Exception e)
            {
                _logger.LogError($"ProfissionalProcedimentosGet: {DateTime.Now}, {filters}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

    }
}
