using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class UnidadeFederativaController : Controller
    {
        private readonly IUnidadeFederativaApplication _ufApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public UnidadeFederativaController(IUnidadeFederativaApplication ufApplication,
            IMapper mapper,
            ILogger<UnidadeFederativaController> logger)
        {
            _ufApplication = ufApplication;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Retorna uma lista de Unidades Federativas
        /// </summary>
        /// <param name="ufModel"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns>Retorna uma lista de Unidades Federativas</returns>
        /// <response code="200">Estabelecimento(s) ecnontrada(s)</response>
        /// <response code="204">Nenhum Unidade Federativa encontrado</response>
        /// <response code="404">Erro ao procurar Unidade Federativa</response>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromQuery] UnidadeFederativaModel ufModel, string filters, string sort = "UfAbreviado",
            int skip = 1, int take = 10)
        {
            try
            {
                var (ufs, totalCount) = await _ufApplication.GetByParam(
                    _mapper.Map<UnidadeFederativa>(ufModel),
                    sort,
                    skip,
                    take).ConfigureAwait(false);

                if (ufs == null) return NoContent();
                if (!ufs.Any()) return NoContent();

                var totalPages = (int)Math.Ceiling((double)totalCount / take);
                var response = new
                {
                    items = _mapper.Map<IEnumerable<UnidadeFederativaModel>>(ufs),
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
                _logger.LogError($"UnidadeFederativaGet: {ufModel}, {e}");
                return NotFound();
            }
        }

        /// <summary>
        /// Retorna uma UnidadeFederativa específica.
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id}", Name = "UnidadeFederativaId")]
        [AllowAnonymous]
        public IActionResult Get(string id)
        {
            try
            {
                var ufModel = _mapper.Map<UnidadeFederativaModel>(_ufApplication.GetById(id));

                if (ufModel == null) return NoContent();

                return Ok(ufModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"UnidadeFederativaId: {DateTime.Now}, {id}, {e}");
                return NotFound();
            }

        }
        
        //[HttpGet]
        //[AllowAnonymous]
        //[Route("GetApp")]
        //public async Task<IActionResult> GetApp([FromQuery]UnidadeFederativaModel ufModel, string filters, string sort = "UfAbreviado",
        //  int skip = 1, int take = 10)
        //{
        //    try
        //    {
        //        var (ufs, totalCount) = await _ufApplication.GetByParam(
        //            _mapper.Map<UnidadeFederativa>(ufModel),
        //            sort,
        //            skip,
        //            take).ConfigureAwait(false);

        //        if (ufs == null) return NoContent();
        //        if (!ufs.Any()) return NoContent();

        //        var totalPages = (int)Math.Ceiling((double)totalCount / take);
        //        var response = new
        //        {
        //            items = _mapper.Map<IEnumerable<UnidadeFederativaModel>>(ufs),
        //            sort,
        //            skip,
        //            take,
        //            totalCount,
        //            totalPages
        //        };

        //        return Ok(response);

        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError($"UnidadeFederativaGetApp: {ufModel}, {e}");
        //        return NotFound();
        //    }
        //}

    }
}
