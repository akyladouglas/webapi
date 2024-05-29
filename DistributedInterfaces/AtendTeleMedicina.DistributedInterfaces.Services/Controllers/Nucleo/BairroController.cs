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
using Microsoft.AspNetCore.Http;
using AtendTeleMedicina.Application.Contracts.Nucleo;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class BairroController : Controller
    {
        private readonly IBairroApplication _bairroApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public BairroController(IBairroApplication bairroApplication,
                                IMapper mapper,
                                ILogger<BairroController> logger)
        {
            _bairroApplication = bairroApplication;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Retorna uma lista de Bairros
        /// </summary>
        /// <param name="bairroModel"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns>Retorna uma lista de Bairros</returns>
        /// <response code="200">Bairro(s) encontrada(s)</response>
        /// <response code="204">Nenhum Bairro encontrado</response>
        /// <response code="404">Erro ao procurar Bairro</response>
        [HttpGet]
        [Authorize(Roles = "Retaguarda, App")]
        public async Task<IActionResult> Get([FromQuery]BairroModel bairroModel, string param, string sort = "Nome",
          int skip = 1, int take = 10)
        {

            if (param != null)
            {
                byte[] data = System.Convert.FromBase64String(param);
                var decodeString = System.Text.ASCIIEncoding.ASCII.GetString(data);
                bairroModel = JsonConvert.DeserializeObject<BairroModel>(decodeString);
            }
            bairroModel?.ValidarGet();
            try
            {
                var (bairros, totalCount) = await _bairroApplication.GetByParam(
                    _mapper.Map<Bairro>(bairroModel),
                    sort,
                    skip,
                    take).ConfigureAwait(false);

                var totalPages = (int)Math.Ceiling((double)totalCount / take);
                var response = new
                {
                    items = _mapper.Map<IEnumerable<BairroModel>>(bairros),
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
                _logger.LogError($"BairroGet: {bairroModel}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        /// <summary>
        /// Retorna um Bairro específico.
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id}")]
        [Authorize(Roles = "Retaguarda, App")]
        public IActionResult Get(string id)
        {
            try
            {
                var bairroModel = _mapper.Map<BairroModel>(_bairroApplication.GetById(id));

                if (bairroModel == null) return NotFound();

                return Ok(bairroModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"BairroGetById: {DateTime.Now}, {id}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

       //POST api/bairo
        [HttpPost]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Post([FromBody] BairroModel bairroModel)
        {

            try
            {
                if (bairroModel == null) return BadRequest("Necessário informar dados do Bairro");
                bairroModel.ValidarAdicionar();
                if (bairroModel.Invalid)
                {
                    return BadRequest(bairroModel);
                }

                var estabelecimento = _mapper.Map<Bairro>(bairroModel);

                _bairroApplication.Add(estabelecimento);

                bairroModel = _mapper.Map<BairroModel>(estabelecimento);

                return Created($"api/bairro/{bairroModel.Id}", bairroModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"BairroPost: {bairroModel}");
                return BadRequest(e.Message.ToString());
            }
        }

        //PUT api/bairro/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Put(string id, [FromBody] BairroModel bairroModel)
        {
            try
            {
                if (bairroModel == null) return BadRequest("Informe o registro a ser alterado");

                bairroModel.ValidarEditar();
                if (bairroModel.Invalid)
                {
                    return BadRequest(bairroModel);
                }

                var estabelecimentoExists = _bairroApplication.GetById(id);
                if (estabelecimentoExists == null)
                {
                    bairroModel.AddNotification("bairroModel.Id", "Registro não encontrado");
                    return NotFound();
                }

                _bairroApplication.Update(id, _mapper.Map<Bairro>(bairroModel));

                bairroModel = _mapper.Map<BairroModel>(_bairroApplication.GetById(id));
                return new NoContentResult();
            }
            catch (Exception e)
            {
                _logger.LogError($"BairroPut: {bairroModel}");
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
                _bairroApplication.Delete(id);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"BairroDelete: {id}");
                return BadRequest(e.Message.ToString());
            }
        }
    }
}
