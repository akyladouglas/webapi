using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AtendTeleMedicina.Application.Contracts.Integracao;
using AtendTeleMedicina.Domain.Entities.Integracao;
using AutoMapper;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Integracao
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/ExamesAfinion2")]
    [Produces("application/json")]

    public class ExamesAfinion2Controller : Controller
    {
        private readonly IExamesAfinion2Application _examesAfinion2Application;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ExamesAfinion2Controller(IExamesAfinion2Application examesAfinion2Application, IMapper mapper)
        {
            _examesAfinion2Application = examesAfinion2Application;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("GetExamesAfinion2")]
        [Authorize(Roles = "Retaguarda, Médico, Individuo, Recepcionista, MédicoEspecialista")]
        public async Task<IActionResult> Get([FromQuery] ExamesAfinion2 exameAfinion2Model, string sort = "Nome ASC",
          int skip = 1, int take = 5)
        {
            try
            {
                var (tipoExame, totalCount) = await _examesAfinion2Application.GetByParam(
                     _mapper.Map<ExamesAfinion2>(exameAfinion2Model),
                     sort,
                     skip,
                     take).ConfigureAwait(false);

                var totalPages = (int)Math.Ceiling((double)totalCount / take);

                if (totalCount == 0) return NoContent();

                var response = new
                {
                    items = _mapper.Map<IEnumerable<ExamesAfinion2>>(tipoExame),
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
                _logger.LogError("ERRO");
                return BadRequest(e.Message);
            }
        }
    }
}
