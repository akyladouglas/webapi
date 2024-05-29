using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AtendTeleMedicina.Application.Contracts.Nucleo;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class EstabelecimentosController : Controller
    {
        private readonly IEstabelecimentoApplication _estabelecimentoApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public EstabelecimentosController(IEstabelecimentoApplication estabelecimentoApplication, IMapper mapper, ILogger<EstabelecimentosController> logger)
        {
            _estabelecimentoApplication = estabelecimentoApplication;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Retorna uma lista de Estabelecimentos
        /// </summary>
        /// <param name="estabelecimentoModel"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns>Retorna uma lista de Estabelecimentos</returns>
        /// <response code="200">Estabelecimento(s) encontrado(s)</response>
        /// <response code="204">Nenhum Estabelecimento encontrado</response>
        /// <response code="404">Erro ao procurar Estabelecimento</response>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromQuery]EstabelecimentoModel estabelecimentoModel, string param, string sort = "NomeFantasia",
          int skip = 1, int take = 10)

        {
            if (param != null)
            {
                byte[] data = System.Convert.FromBase64String(param);
                var decodeString = System.Text.ASCIIEncoding.ASCII.GetString(data);
                estabelecimentoModel = JsonConvert.DeserializeObject<EstabelecimentoModel>(decodeString);
            }
            estabelecimentoModel?.ValidarGet();
            try
            {
                var (estabelecimentos, totalCount) = await _estabelecimentoApplication.GetByParam(
                    _mapper.Map<Estabelecimento>(estabelecimentoModel),
                    sort,
                    skip,
                    take).ConfigureAwait(false);

                var totalPages = (int)Math.Ceiling((double)totalCount / take);
                var response = new
                {
                    items = _mapper.Map<IEnumerable<EstabelecimentoModel>>(estabelecimentos),
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
                _logger.LogError($"EstabelecimentoGet: {estabelecimentoModel}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        /// <summary>
        /// Retorna um Estabelecimento específico.
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id}")]
        [Authorize(Roles = "Retaguarda, Individuo")]
        public IActionResult Get(string id)
        {
            try
            {
                var estabelecimentoModel = _mapper.Map<EstabelecimentoModel>(_estabelecimentoApplication.GetById(id));

                if (estabelecimentoModel == null) return NotFound();

                return Ok(estabelecimentoModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"EstabelecimentoGetById: {DateTime.Now}, {id}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }
        
        //POST api/estabelecimento
        [HttpPost]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Post([FromBody] EstabelecimentoModel estabelecimentoModel)
        {

            try
            {
                if (estabelecimentoModel == null) return BadRequest("Necessário informar dados do Estabelecimento");
                estabelecimentoModel.ValidarAdicionar();
                if (estabelecimentoModel.Invalid)
                {
                    return BadRequest(estabelecimentoModel);
                }

                var estabelecimento = _mapper.Map<Estabelecimento>(estabelecimentoModel);

                _estabelecimentoApplication.Add(estabelecimento);

                estabelecimentoModel = _mapper.Map<EstabelecimentoModel>(estabelecimento);

                return Created($"api/estabelecimento/{estabelecimentoModel.Id}", estabelecimentoModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"EstabelecimentoPost: {estabelecimentoModel}");
                return BadRequest(e.Message.ToString());
            }
        }

        //PUT api/estabelecimento/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Put(string id, [FromBody] EstabelecimentoModel estabelecimentoModel)
        {
            try
            {
                if (estabelecimentoModel == null) return BadRequest("Informe o registro a ser alterado");

                estabelecimentoModel.ValidarEditar();
                if (estabelecimentoModel.Invalid)
                {
                    return BadRequest(estabelecimentoModel);
                }

                var estabelecimentoExists = _estabelecimentoApplication.GetById(id);
                if (estabelecimentoExists == null)
                {
                    estabelecimentoModel.AddNotification("Estabelecimento.Id", "Registro não encontrado");
                    return NotFound();
                }

                _estabelecimentoApplication.Update(id, _mapper.Map<Estabelecimento>(estabelecimentoModel));

                return new NoContentResult();
            }
            catch (Exception e)
            {
                _logger.LogError($"EstabelecimentoPut: {estabelecimentoModel}");
                return BadRequest(e.Message.ToString());
            }
        }

        [HttpPut]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Excluir([FromBody] EstabelecimentoModel estabelecimento)
        {
            if (string.IsNullOrEmpty(estabelecimento.Id)) return BadRequest($"Dados incompletos");
            if(!estabelecimento.Ativo.HasValue) return BadRequest($"Dados incompletos");

            try
            {

                _estabelecimentoApplication.Delete(estabelecimento.Id, estabelecimento.Ativo);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"EstabelecimentoDelete: {estabelecimento.Id}");
                return BadRequest(e.Message.ToString());
            }
        }

    }
}
