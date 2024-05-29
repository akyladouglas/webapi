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
using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Params;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class AuditoriaController : Controller
    {
        private readonly IAuditoriaApplication _auditoriaApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public AuditoriaController(IAuditoriaApplication auditoriaApplication,
                                        IMapper mapper,
                                        ILogger<AuditoriaController> logger)
        {
            _auditoriaApplication = auditoriaApplication;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Retorna uma lista de Auditorias
        /// </summary>
        /// <param name="auditoriaModel"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns>Retorna uma lista de Agentes de Fiscalização</returns>
        /// <response code="200">Estabelecimento(s) ecnontrada(s)</response>
        /// <response code="204">Nenhum Auditoria encontrado</response>
        /// <response code="404">Erro ao procurar Auditoria</response>
        [HttpGet]
        [Authorize(Roles = "Administrador, Individuo, Médico")]
        public async Task<IActionResult> Get([FromQuery] AuditoriaModel auditoriaModel, [FromQuery] AppParams appParams, string sort = "DataHora",
          int skip = 1, int take = 10)
        {

            auditoriaModel?.ValidarGet();
            try
            {
                var (auditorias, totalCount) = await _auditoriaApplication.GetByParam(
                    _mapper.Map<Auditoria>(auditoriaModel), appParams,
                    sort,
                    skip,
                    take).ConfigureAwait(false);

                var totalPages = (int)Math.Ceiling((double)totalCount / take);
                var response = new
                {
                    items = _mapper.Map<IEnumerable<AuditoriaModel>>(auditorias),
                    sort,
                    skip,
                    take,
                    totalCount,
                    totalPages
                };

                return Ok(response);

            }
            catch (Exception e)
            {
                _logger.LogError($"AuditoriaGet: {auditoriaModel}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        /// <summary>
        /// Retorna um Auditoria específico.
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id}")]
        [Authorize(Roles = "Retaguarda, App, Médico")]
        public IActionResult Get(string id)
        {
            try
            {
                var auditoriaModel = _mapper.Map<AuditoriaModel>(_auditoriaApplication.GetById(id));

                if (auditoriaModel == null) return NotFound();

                return Ok(auditoriaModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"AuditoriaGetById: {DateTime.Now}, {id}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

    }
}
