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
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CustomizacaoController : Controller
    {
        private readonly ICustomizacaoApplication _customizacaoApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public CustomizacaoController(ICustomizacaoApplication customizacaoApplication, IMapper mapper, ILoggerFactory logger)
        {
            _customizacaoApplication = customizacaoApplication;
            _mapper = mapper;
            _logger = logger.CreateLogger<CustomizacaoController>();
        }

        /// <summary>
        /// Retorna uma Customizacao.
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id}")]
        [Authorize(Roles = "Retaguarda, App")]
        public IActionResult Get(string id)
        {
            try
            {
                var estabelecimentoModel = _mapper.Map<CustomizacaoModel>(_customizacaoApplication.GetById(id));

                if (estabelecimentoModel == null) return NotFound();

                return Ok(estabelecimentoModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"CustomizacaoId: {DateTime.Now}, {id}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        /// <summary>
        /// Retorna uma lista de Customizacao
        /// </summary>
        /// <param name="customizacaoModel"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns>Retorna uma lista de Customizacao</returns>
        /// <response code="200">Customizacao ecnontrada(s)</response>
        /// <response code="204">Nenhum Customizacao encontrado</response>
        /// <response code="404">Erro ao procurar Customizacao</response>
        [HttpGet]
        [Authorize(Roles = "Retaguarda, App, Individuo")]
        public async Task<IActionResult> Get([FromQuery] CustomizacaoModel customizacaoModel, string param, string sort = "DataCadastro",
          int skip = 1, int take = 100)
        {

            if (param != null)
            {
                byte[] data = System.Convert.FromBase64String(param);
                var decodeString = System.Text.ASCIIEncoding.ASCII.GetString(data);
                customizacaoModel = JsonConvert.DeserializeObject<CustomizacaoModel>(decodeString);
            }
            try
            {
                var (customizacoes, totalCount) = await _customizacaoApplication.GetByParam(
                    _mapper.Map<Customizacao>(customizacaoModel),
                    sort,
                    skip,
                    take).ConfigureAwait(false);

                var totalPages = (int)Math.Ceiling((double)totalCount / take);
                var response = new
                {
                    items = _mapper.Map<IEnumerable<CustomizacaoModel>>(customizacoes),
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
                _logger.LogError($"AgendamentoGet: {customizacaoModel}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        //POST api/customizacao
        [HttpPost]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Post([FromBody] CustomizacaoModel customizacaoModel)
        {

            try
            {
                if (customizacaoModel == null) return BadRequest("Necessário informar dados da Customizacao");


                var customizacao = _mapper.Map<Customizacao>(customizacaoModel);

                _customizacaoApplication.Add(customizacao);

                customizacaoModel = _mapper.Map<CustomizacaoModel>(customizacao);

                return Created($"api/customizacao/{customizacaoModel.Id}", customizacaoModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"CustomizacaoPost: {customizacaoModel}");
                return BadRequest(e.Message.ToString());
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Put(string id, [FromBody] CustomizacaoModel customizacaoModel)
        {
            try
            {
                if (customizacaoModel == null) return BadRequest("Informe o registro a ser alterado");

                var customizacaoExists = _customizacaoApplication.GetById(id);
                if (customizacaoExists == null)
                {
                    return NotFound();
                }

                _customizacaoApplication.Update(id, _mapper.Map<Customizacao>(customizacaoModel));

                return new NoContentResult();
            }
            catch (Exception e)
            {
                _logger.LogError($"CustomizacaoPut: {customizacaoModel}");
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
                _customizacaoApplication.Delete(id);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"CustomizacaoDelete: {id}");
                return BadRequest(e.Message.ToString());
            }
        }

    }
}
