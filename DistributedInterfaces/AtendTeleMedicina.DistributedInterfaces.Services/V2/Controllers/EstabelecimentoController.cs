using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AtendTeleMedicina.DistributedInterfaces.Services.V2.Controllers
{
    [Produces("application/json")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [AllowAnonymous]
    public class EstabelecimentoController : Controller
    {
        private readonly IEstabelecimentoApplication _estabelecimentoApplication;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private readonly IMapper _mapper;

        public EstabelecimentoController(IEstabelecimentoApplication estabelecimentoApplication, IMapper mapper, ILoggerFactory logger)
        {
            _estabelecimentoApplication = estabelecimentoApplication;
            _mapper = mapper;
            _logger = logger.CreateLogger<EstabelecimentoController>();
        }

        /// <summary>
        /// Retorna um Indivíduo específico.
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id}")]
        [Authorize(Roles = "Retaguarda, App, Individuo")]
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
                _logger.LogError($"EstabelecimentoId: {DateTime.Now}, {id}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        /// <summary>
        /// Retorna uma lista de Indivíduos
        /// </summary>
        /// <param name="estabelecimentoModel"></param>
        /// <param name="sort"></param>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <returns>Retorna uma lista de Agentes de Fiscalização</returns>
        /// <response code="200">Estabelecimento(s) ecnontrada(s)</response>
        /// <response code="204">Nenhum Indivíduo encontrado</response>
        /// <response code="404">Erro ao procurar Indivíduo</response>
        //[Route("[controller]/[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(EstabelecimentoModel estabelecimentoModel, string param, string sort = "NomeFantasia",
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
                _logger.LogError($"EstabelecimentoList: {estabelecimentoModel}, {e}, Usuario: ");
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
                if (estabelecimentoModel == null) return BadRequest("Necessário informar dados do Indivíduo");
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

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Excluir(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest($"Dados incompletos");
            try
            {
                //_estabelecimentoApplication.Delete(id);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"EstabelecimentoDelete: {id}");
                return BadRequest(e.Message.ToString());
            }
        }

    }
}
