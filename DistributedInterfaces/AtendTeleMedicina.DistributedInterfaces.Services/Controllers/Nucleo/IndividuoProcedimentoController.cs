using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo;
using AtendTeleMedicina.Domain.Params;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/IndividuoProcedimento/")]
    [Produces("application/json")]
    [Authorize]
    public class IndividuoProcedimentoController : Controller
    {
        private readonly IIndividuoProcedimentoApplication _individuoProcedimentoApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public IndividuoProcedimentoController(IIndividuoProcedimentoApplication individuoProcedimentoApplication,
                                                    IMapper mapper,
                                                    ILogger<IndividuoProcedimentoController> logger)
        {
            _individuoProcedimentoApplication = individuoProcedimentoApplication;
            _mapper = mapper;
            _logger = logger;
        }


        
        [HttpGet]
        [Authorize(Roles = "Retaguarda, Individuo, Médico, MédicoEspecialista, MédicoAD")]
        public async Task<IActionResult> Get([FromQuery] IndividuoProcedimento individuoProcedimentoModel, string param, string sort = "DataCadastro.Id",
          int skip = 1, int take = 10)
        {

            if (param != null)
            {
                byte[] data = System.Convert.FromBase64String(param);
                var decodeString = System.Text.ASCIIEncoding.ASCII.GetString(data);
                individuoProcedimentoModel = JsonConvert.DeserializeObject<IndividuoProcedimento>(decodeString);
            }
           
            try
            {
                var (individuoProcedimento, totalCount) = await _individuoProcedimentoApplication.GetByParam(
                    _mapper.Map<IndividuoProcedimento>(individuoProcedimentoModel),
                    sort,
                    skip,
                    take).ConfigureAwait(false);

                var totalPages = (int)Math.Ceiling((double)totalCount / take);
                var response = new
                {
                    items = _mapper.Map<IEnumerable<IndividuoProcedimentoModel>>(individuoProcedimento),
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
                _logger.LogError($"IndividuoProcedimentoList: {individuoProcedimentoModel}, {e}, Usuario: ");
                return BadRequest(e.Message.ToString());
            }
        }

        ///// <summary>
        ///// Retorna os IndividuoCiap.
        ///// </summary>
        ///// <param name="individuoCiapId"></param>
        //[HttpGet("{individuoProcedimentoId}/")]
        //[Authorize(Roles = "Retaguarda, Individuo, 'MédicoEspecialista'")]
        //public IActionResult Get(string individuoCiapId)
        //{
        //    try
        //    {
        //        var individuoCiapModel = _mapper.Map<IndividuoCiapModel>(_individuoProcedimentoApplication.GetById(individuoCiapId));

        //        if (individuoCiapModel == null) return NotFound();

        //        return Ok(individuoCiapModel);
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError($"IndividuoCiapById: {DateTime.Now}, {individuoCiapId}, {e}");
        //        return BadRequest(e.Message.ToString());
        //    }
        //}

        //[HttpPost]
        ////[Route("Procedimentos")]
        //[Authorize(Roles = "Retaguarda, Médico, 'MédicoEspecialista'")]
        //public IActionResult Post([FromBody] IEnumerable<IndividuoCiapModel> individuoCiapModel)
        //{

        //    try
        //    {
        //        if (individuoCiapModel == null) return BadRequest("A Lista de Individuo Ciap está vazia.");
        //        foreach (var item in individuoCiapModel)
        //        {
        //            item.ValidarAdicionar();
        //            if (!item.Invalid) BadRequest(item);
        //        }

        //        var list = _mapper.Map<IEnumerable<IndividuoCiap>>(individuoCiapModel);

        //        _individuoProcedimentoApplication.Add(list);

        //        return Created($"api/individuocial/", "individuoCiapModel");
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError($"IndividuoCiapPost: {individuoCiapModel}");
        //        return BadRequest(e.Message.ToString());
        //    }
        //}

    }
}
