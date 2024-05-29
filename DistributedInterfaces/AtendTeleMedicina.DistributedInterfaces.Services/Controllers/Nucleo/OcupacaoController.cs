using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using AtendTeleMedicina.Application.Services.Nucleo;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class OcupacaoController : Controller
    {
        #region Injecao de dependencias e construtor
        
        private readonly IOcupacaoApplication _ocupacaoApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public OcupacaoController(IOcupacaoApplication ocupacaoApplication, ILogger<OcupacaoController> logger, IMapper mapper)
        {
            _ocupacaoApplication = ocupacaoApplication;
            _logger = logger;
            _mapper = mapper;
        }
        
        #endregion

        // Retorna uma ocupacao
        [HttpGet("{cbo}", Name = "CodOcupacao")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(string cbo)
        {
            try
            {
                var ocupacaoModel = _mapper.Map<OcupacaoModel>(await _ocupacaoApplication.GetByCbo(cbo));

                if (ocupacaoModel == null) return NoContent();

                return Ok(ocupacaoModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"OcupacaoCBO: {DateTime.Now}, {cbo}, {e}");
                return NotFound();
            }

        }

        // Retorna uma lista de ocupacoes (objeto completo)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromQuery] OcupacaoModel ocupacaoModel, bool cboEsus = true, string sort = "Descricao",
            int skip = 1, int take = 999)
        {
            try
            {
                var (ocupacao, totalCount) = await _ocupacaoApplication.GetByParam(
                    _mapper.Map<Ocupacao>(ocupacaoModel),
                    cboEsus,
                    sort,
                    skip,
                    take).ConfigureAwait(false);

                if (ocupacao == null) return NoContent();
                if (!ocupacao.Any()) return NoContent();

                var totalPages = (int)Math.Ceiling((double)totalCount / take);
                var response = new
                {
                    items = _mapper.Map<IEnumerable<OcupacaoModel>>(ocupacao),
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
                _logger.LogError($"OcupacaoGet: {ocupacaoModel}, {e}");
                return NotFound();
            }
        }
    }
}
