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
    public class CiapController : Controller
    {
        private readonly ICiapApplication _ciapApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public CiapController(ICiapApplication ciapApplication,
            IMapper mapper,
            ILogger<CiapController> logger)
        {
            _ciapApplication = ciapApplication;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Retorna uma lista de CIAPs
        /// </summary>
        /// <param name="ciapModel"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns>Retorna uma lista de CIAP</returns>
        /// <response code="200">CIAP(s) ecnontrada(s)</response>
        /// <response code="204">Nenhum CIAP encontrado</response>
        /// <response code="404">Erro ao procurar CIAP</response>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromQuery] CiapModel ciapModel, string filters, string sort = "Codigo",
            int skip = 1, int take = 10)
        {
            try
            {
                var (ciap, totalCount) = await _ciapApplication.GetByParam(
                    _mapper.Map<Ciap>(ciapModel),
                    sort,
                    skip,
                    take).ConfigureAwait(false);

                if (ciap == null) return NoContent();
                if (!ciap.Any()) return NoContent();

                var totalPages = (int)Math.Ceiling((double)totalCount / take);
                var response = new
                {
                    items = _mapper.Map<IEnumerable<CiapModel>>(ciap),
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
                _logger.LogError($"CiapGet: {ciapModel}, {e}");
                return NotFound();
            }
        }

        /// <summary>
        /// Retorna uma CIAP
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id}", Name = "IdCiap")]
        [AllowAnonymous]
        public IActionResult Get(string id)
        {
            try
            {
                var ciapModel = _mapper.Map<CiapModel>(_ciapApplication.GetById(id));

                if (ciapModel == null) return NoContent();

                return Ok(ciapModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"CiapId: {DateTime.Now}, {id}, {e}");
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

        
        [Authorize(Roles = "Retaguarda, Individuo, Médico, MédicoEspecialista")]
        [Route("GetByManyIds")]
        public async Task<IActionResult> GetByManyIds([FromQuery] string ids)
        {
            if (ids is null) return BadRequest();

            var idsArray = ids.Split(',');
            
            try
            {
                var ciaps = await _ciapApplication.GetByManyIds(idsArray).ConfigureAwait(true);

                if (ciaps == null || !ciaps.Any()) return NoContent();
                
                var response = ciaps.Select(c => new { key = c.Codigo, label = c.Descricao });


                return Ok(response);
            }
            catch (Exception e)
            {
                _logger.LogError($"GetByManyIds: {DateTime.Now}, {ids}, {e}");
                return NotFound();
            }
        }
    }
}
