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
    [Route("api/v{version:apiVersion}/IndividuoCid10/")]
    [Produces("application/json")]
    [Authorize]
    public class IndividuoCid10Controller : Controller
    {
        private readonly IIndividuoCid10Application _individuoCid10Application;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public IndividuoCid10Controller(IIndividuoCid10Application individuoCid10Application,
                                                    IMapper mapper,
                                                    ILogger<IndividuoCid10Controller> logger)
        {
            _individuoCid10Application = individuoCid10Application;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Retaguarda, Individuo, Médico, MédicoEspecialista, MédicoAD")]
        public async Task<IActionResult> Get([FromQuery] IndividuoCid10Model cidModel, string param, string sort = "Atendimento.Id",
          int skip = 1, int take = 10)
        {

            if (param != null)
            {
                byte[] data = System.Convert.FromBase64String(param);
                var decodeString = System.Text.ASCIIEncoding.ASCII.GetString(data);
                cidModel = JsonConvert.DeserializeObject<IndividuoCid10Model>(decodeString);
            }
            //atendimentoModel?.ValidarGet();
            cidModel?.ValidarAdicionar();
            //string userId = _user.GetUserId();
            //ciapModel.ProfissionalId = userId;
            try
            {
                var (cid, totalCount) = await _individuoCid10Application.GetByParam(
                    _mapper.Map<IndividuoCid10>(cidModel),
                    sort,
                    skip,
                    take).ConfigureAwait(false);

                var totalPages = (int)Math.Ceiling((double)totalCount / take);
                var response = new
                {
                    items = _mapper.Map<IEnumerable<IndividuoCid10Model>>(cid),
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
                _logger.LogError($"IndividuoCidList: {cidModel}, {e}, Usuario: ");
                return BadRequest(e.Message.ToString());
            }
        }

        /// <summary>
        /// Retorna os IndividuoCid10
        /// </summary>
        /// <param name="individuoCiapId"></param>
        [HttpGet("{individuoCid10Id}/")]
        [Authorize(Roles = "Retaguarda, Individuo, MédicoEspecialista")]
        public IActionResult Get(string individuoCid10Id)
        {
            try
            {
                var individuoCid10Model = _mapper.Map<IndividuoCiapModel>(_individuoCid10Application.GetById(individuoCid10Id));

                if (individuoCid10Model == null) return NotFound();

                return Ok(individuoCid10Model);
            }
            catch (Exception e)
            {
                _logger.LogError($"IndividuoCid10ById: {DateTime.Now}, {individuoCid10Id}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        [HttpPost]
        [Authorize(Roles = "Retaguarda, Médico, MédicoEspecialista")]
        public IActionResult Post([FromBody] IEnumerable<IndividuoCid10Model> individuoCid10Model)
        {

            try
            {
                if (individuoCid10Model == null) return BadRequest("A Lista de Individuo Cid10 está vazia.");
                foreach (var item in individuoCid10Model)
                {
                    item.ValidarAdicionar();
                    if (!item.Invalid) BadRequest(item);
                }

                var list = _mapper.Map<IEnumerable<IndividuoCid10>>(individuoCid10Model);

                _individuoCid10Application.Add(list);

                return Created($"api/individuocid/", "individuoCid10Model");
            }
            catch (Exception e)
            {
                _logger.LogError($"IndividuoCid10Post: {individuoCid10Model}");
                return BadRequest(e.Message.ToString());
            }
        }

    }
}
