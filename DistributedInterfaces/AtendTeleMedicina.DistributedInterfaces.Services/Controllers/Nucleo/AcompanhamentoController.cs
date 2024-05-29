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

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class AcompanhamentoController : Controller
    {
        private readonly IAcompanhamentoApplication _acompanhamentoApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public AcompanhamentoController(IAcompanhamentoApplication acompanhamentoApplication,
                                        IMapper mapper,
                                        ILogger<AcompanhamentoController> logger)
        {
            _acompanhamentoApplication = acompanhamentoApplication;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Retorna uma lista de Acompanhamentos
        /// </summary>
        /// <param name="acompanhamentoModel"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns>Retorna uma lista de Agentes de Fiscalização</returns>
        /// <response code="200">Estabelecimento(s) ecnontrada(s)</response>
        /// <response code="204">Nenhum Acompanhamento encontrado</response>
        /// <response code="404">Erro ao procurar Acompanhamento</response>
        [HttpGet]
        [Authorize(Roles = "Administrador, Individuo, Médico, Recepcionista, MédicoEspecialista, MédicoAD")]
        public async Task<IActionResult> Get([FromQuery]AcompanhamentoModel acompanhamentoModel, string param, string sort = "a.Data DESC",
          int skip = 1, int take = 10)
        {

            if (param != null)
            {
                byte[] data = System.Convert.FromBase64String(param);
                var decodeString = System.Text.ASCIIEncoding.ASCII.GetString(data);
                acompanhamentoModel = JsonConvert.DeserializeObject<AcompanhamentoModel>(decodeString);
            }
            acompanhamentoModel?.ValidarGet();
            try
            {
                var (acompanhamentos, totalCount) = await _acompanhamentoApplication.GetByParam(
                    _mapper.Map<Acompanhamento>(acompanhamentoModel),
                    sort,
                    skip,
                    take).ConfigureAwait(false);

                var totalPages = (int)Math.Ceiling((double)totalCount / take);
                var response = new
                {
                    items = _mapper.Map<IEnumerable<AcompanhamentoModel>>(acompanhamentos),
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
                _logger.LogError($"AcompanhamentoGet: {acompanhamentoModel}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        /// <summary>
        /// Retorna um Acompanhamento específico.
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id}")]
        [Authorize(Roles = "Retaguarda, App, Médico, Recepcionista, MédicoEspecialista")]
        public IActionResult Get(string id)
        {
            try
            {
                var acompanhamentoModel = _mapper.Map<AcompanhamentoModel>(_acompanhamentoApplication.GetById(id));

                if (acompanhamentoModel == null) return NotFound();

                return Ok(acompanhamentoModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"AcompanhamentoGetById: {DateTime.Now}, {id}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        //POST api/acompanhamento
        [HttpPost]
        [Authorize(Roles = "Retaguarda, Individuo, Médico, Recepcionista, MédicoEspecialista")]
        public IActionResult Post([FromBody] AcompanhamentoModel acompanhamentoModel)
        {

            try
            {
                if (acompanhamentoModel == null) return BadRequest("Necessário informar dados do Acompanhamento");
                //acompanhamentoModel.ValidarAdicionar();
                if (acompanhamentoModel.Invalid)
                {
                    return BadRequest(acompanhamentoModel);
                }

                var acompanhamento = _mapper.Map<Acompanhamento>(acompanhamentoModel);

                _acompanhamentoApplication.Add(acompanhamento);

                acompanhamentoModel = _mapper.Map<AcompanhamentoModel>(acompanhamento);

                return Created($"api/acompanhamento/{acompanhamento.Id}", acompanhamentoModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"AcompanhamentoPost: {acompanhamentoModel}");
                return BadRequest(e.Message.ToString());
            }
        }

        //PUT api/acompanhamento/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Retaguarda, Recepcionista")]
        public IActionResult Put(string id, [FromBody] AcompanhamentoModel acompanhamentoModel)
        {
            try
            {
                if (acompanhamentoModel == null) return BadRequest("Informe o registro a ser alterado");

                //acompanhamentoModel.ValidarEditar();
                if (acompanhamentoModel.Invalid)
                {
                    return BadRequest(acompanhamentoModel);
                }

                var acompanhamentoExists = _acompanhamentoApplication.GetById(id);
                if (acompanhamentoExists == null)
                {
                    acompanhamentoModel.AddNotification("Acompanhamento.Id", "Registro não encontrado");
                    return NotFound();
                }

                _acompanhamentoApplication.Update(id, _mapper.Map<Acompanhamento>(acompanhamentoModel));

                return new NoContentResult();
            }
            catch (Exception e)
            {
                _logger.LogError($"AcompanhamentoPut: {acompanhamentoModel}");
                return BadRequest(e.Message.ToString());
            }
        }

        //PUT api/acompanhamento/AtualizarAcompanhamento/{id}
        [HttpPut("AtualizarAcompanhamento/{atendimentoId}")]
        [Authorize(Roles = "Retaguarda, Individuo, Médico, Recepcionista, MédicoEspecialista")]
        public IActionResult AtualizarAcompanhamento(string atendimentoId, [FromBody]AcompanhamentoModel acompanhamentoModel)
        {
            try
            {
                if (atendimentoId == null || acompanhamentoModel == null) return BadRequest("Informe o registsro a ser alterado");

                //individuoModel.ValidarEditar();
                //if (individuoModel.Invalid)
                //{
                //    return BadRequest(individuoModel);
                //}

                var acompanhamentoExists = _acompanhamentoApplication.GetById(acompanhamentoModel.Id);
                if (acompanhamentoExists == null)
                {
                    //individuoModel.AddNotification("Individuo.Id", "Registro não encontrado");
                    return NotFound();
                }

                acompanhamentoModel.AtendimentoId = atendimentoId;

                _acompanhamentoApplication.Update(atendimentoId, _mapper.Map<Acompanhamento>(acompanhamentoModel));

                return Ok("Atualizado com sucesso");

            }
            catch (Exception e)
            {
                _logger.LogError($"IndividuoPut: {e}");
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
                _acompanhamentoApplication.Delete(id);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"AcompanhamentoDelete: {id}");
                return BadRequest(e.Message.ToString());
            }
        }

    }
}
