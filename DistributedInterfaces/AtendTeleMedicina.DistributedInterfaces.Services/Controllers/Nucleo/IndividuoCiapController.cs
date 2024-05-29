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
    [Route("api/v{version:apiVersion}/IndividuoCiap/")]
    [Produces("application/json")]
    [Authorize]
    public class IndividuoCiapController : Controller
    {
        private readonly IIndividuoCiapApplication _individuoCiapApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public IndividuoCiapController(IIndividuoCiapApplication individuoCiapApplication,
                                                    IMapper mapper,
                                                    ILogger<IndividuoCiapController> logger)
        {
            _individuoCiapApplication = individuoCiapApplication;
            _mapper = mapper;
            _logger = logger;
        }


        
        [HttpGet]
        [Authorize(Roles = "Retaguarda, Individuo, Médico, MédicoEspecialista, MédicoAD")]
        public async Task<IActionResult> Get([FromQuery] IndividuoCiapModel ciapModel, string param, string sort = "Atendimento.Id",
          int skip = 1, int take = 10)
        {

            if (param != null)
            {
                byte[] data = System.Convert.FromBase64String(param);
                var decodeString = System.Text.ASCIIEncoding.ASCII.GetString(data);
                ciapModel = JsonConvert.DeserializeObject<IndividuoCiapModel>(decodeString);
            }
            //atendimentoModel?.ValidarGet();
            ciapModel?.ValidarAdicionar();
            //string userId = _user.GetUserId();
            //ciapModel.ProfissionalId = userId;
            try
            {
                var (ciap, totalCount) = await _individuoCiapApplication.GetByParam(
                    _mapper.Map<IndividuoCiap>(ciapModel),
                    sort,
                    skip,
                    take).ConfigureAwait(false);

                var totalPages = (int)Math.Ceiling((double)totalCount / take);
                var response = new
                {
                    items = _mapper.Map<IEnumerable<IndividuoCiapModel>>(ciap),
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
                _logger.LogError($"IndividuoCiapList: {ciapModel}, {e}, Usuario: ");
                return BadRequest(e.Message.ToString());
            }
        }

        /// <summary>
        /// Retorna os IndividuoCiap.
        /// </summary>
        /// <param name="individuoCiapId"></param>
        [HttpGet("{individuoCiapId}/")]
        [Authorize(Roles = "Retaguarda, Individuo, 'MédicoEspecialista'")]
        public IActionResult Get(string individuoCiapId)
        {
            try
            {
                var individuoCiapModel = _mapper.Map<IndividuoCiapModel>(_individuoCiapApplication.GetById(individuoCiapId));

                if (individuoCiapModel == null) return NotFound();

                return Ok(individuoCiapModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"IndividuoCiapById: {DateTime.Now}, {individuoCiapId}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        [HttpPost]
        //[Route("Procedimentos")]
        [Authorize(Roles = "Retaguarda, Médico, 'MédicoEspecialista'")]
        public IActionResult Post([FromBody] IEnumerable<IndividuoCiapModel> individuoCiapModel)
        {

            try
            {
                if (individuoCiapModel == null) return BadRequest("A Lista de Individuo Ciap está vazia.");
                foreach (var item in individuoCiapModel)
                {
                    item.ValidarAdicionar();
                    if (!item.Invalid) BadRequest(item);
                }

                var list = _mapper.Map<IEnumerable<IndividuoCiap>>(individuoCiapModel);

                _individuoCiapApplication.Add(list);

                return Created($"api/individuocial/", "individuoCiapModel");
            }
            catch (Exception e)
            {
                _logger.LogError($"IndividuoCiapPost: {individuoCiapModel}");
                return BadRequest(e.Message.ToString());
            }
        }

    }
}
