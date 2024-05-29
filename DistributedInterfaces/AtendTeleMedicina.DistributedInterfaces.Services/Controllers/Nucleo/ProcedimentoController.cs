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
    [AllowAnonymous]
    public class ProcedimentoController : Controller
    {
        private readonly IProcedimentoApplication _procedimentoApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public ProcedimentoController(IProcedimentoApplication procedimentoApplication,
                                    IMapper mapper,
                                    ILogger<ProcedimentoController> logger)
        {
            _procedimentoApplication = procedimentoApplication;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Retorna uma lista de Procedimentos
        /// </summary>
        /// <param name="procedimentoModel"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns>Retorna uma lista de Procedimentos</returns>
        /// <response code="200">Procedimento(s) ecnontrada(s)</response>
        /// <response code="204">Nenhum Procedimento encontrado</response>
        /// <response code="404">Erro ao procurar Procedimento</response>
        [HttpGet]
        [Authorize(Roles = "Retaguarda, App")]
        public async Task<IActionResult> Get([FromQuery]ProcedimentoModel procedimentoModel, [FromQuery] AppParams appParams, string param, string sort = "Descricao",
          int skip = 1, int take = 10)
        {

            if (param != null)
            {
                byte[] data = System.Convert.FromBase64String(param);
                var decodeString = System.Text.ASCIIEncoding.ASCII.GetString(data);
                procedimentoModel = JsonConvert.DeserializeObject<ProcedimentoModel>(decodeString);
            }
            procedimentoModel?.ValidarGet();
            try
            {
                var (procedimentos, totalCount) = await _procedimentoApplication.GetByParam(
                    _mapper.Map<Procedimento>(procedimentoModel), appParams,
                    sort,
                    skip,
                    take).ConfigureAwait(false);

                var totalPages = (int)Math.Ceiling((double)totalCount / take);
                var response = new
                {
                    items = _mapper.Map<IEnumerable<ProcedimentoModel>>(procedimentos),
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
                _logger.LogError($"ProcedimentoGet: {procedimentoModel}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        /// <summary>
        /// Retorna um Procedimento específico.
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id}")]
        [Authorize(Roles = "Retaguarda, App")]
        public IActionResult Get(string id)
        {
            try
            {
                var procedimentoModel = _mapper.Map<ProcedimentoModel>(_procedimentoApplication.GetById(id));

                if (procedimentoModel == null) return NotFound();

                return Ok(procedimentoModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"ProcedimentoGetById: {DateTime.Now}, {id}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        //POST api/procedimento
        [HttpPost]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Post([FromBody] ProcedimentoModel procedimentoModel)
        {

            try
            {
                if (procedimentoModel == null) return BadRequest("Necessário informar dados do Procedimento");
                procedimentoModel.ValidarAdicionar();
                if (procedimentoModel.Invalid)
                {
                    return BadRequest(procedimentoModel);
                }

                var procedimento = _mapper.Map<Procedimento>(procedimentoModel);

                _procedimentoApplication.Add(procedimento);

                procedimentoModel = _mapper.Map<ProcedimentoModel>(procedimento);

                return Created($"api/procedimento/{procedimentoModel.Id}", procedimentoModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"ProcedimentoPost: {procedimentoModel}");
                return BadRequest(e.Message.ToString());
            }
        }

        //PUT api/procedimento/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Put(string id, [FromBody] ProcedimentoModel procedimentoModel)
        {
            try
            {
                if (procedimentoModel == null) return BadRequest("Informe o registro a ser alterado");

                procedimentoModel.ValidarEditar();
                if (procedimentoModel.Invalid)
                {
                    return BadRequest(procedimentoModel);
                }

                var procedimentoExists = _procedimentoApplication.GetById(id);
                if (procedimentoExists == null)
                {
                    procedimentoModel.AddNotification("Procedimento.Id", "Registro não encontrado");
                    return NotFound();
                }

                _procedimentoApplication.Update(id, _mapper.Map<Procedimento>(procedimentoModel));

                return new NoContentResult();
            }
            catch (Exception e)
            {
                _logger.LogError($"ProcedimentoPut: {procedimentoModel}");
                return BadRequest(e.Message.ToString());
            }
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Excluir(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest($"Dados incompletos");
            try
            {
                _procedimentoApplication.Delete(id);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"ProcedimentoDelete: {id}");
                return BadRequest(e.Message.ToString());
            }
        }

    }
}
