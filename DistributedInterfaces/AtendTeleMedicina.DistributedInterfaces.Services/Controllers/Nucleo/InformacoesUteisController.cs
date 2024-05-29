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
    public class InformacoesUteisController : Controller
    {
        private readonly IInformacoesUteisApplication _informacoesUteisApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public InformacoesUteisController(IInformacoesUteisApplication informacoesUteisApplication, IMapper mapper, ILoggerFactory logger)
        {
            _informacoesUteisApplication = informacoesUteisApplication;
            _mapper = mapper;
            _logger = logger.CreateLogger<InformacoesUteisController>();
        }


        /// <summary>
        /// Retorna uma Informação Útil.
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id}")]
        [Authorize(Roles = "Retaguarda, App")]
        public IActionResult Get(string id)
        {
            try
            {
                var informacaoModel = _mapper.Map<InformacoesUteisModel>(_informacoesUteisApplication.GetById(id));

                if (informacaoModel == null) return NotFound();

                return Ok(informacaoModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"InformacaoUtilId: {DateTime.Now}, {id}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        /// <summary>
        /// Retorna uma lista de Informacoes Uteis
        /// </summary>
        /// <param name="informacoesUteisModel"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns>Retorna uma lista de Agentes de Fiscalização</returns>
        /// <response code="200">Informações(s) Útil(eis) encontrada(s)</response>
        /// <response code="204">Nenhuma Informação Útil encontrada</response>
        /// <response code="404">Erro ao procurar Informação Útil</response>
        [HttpGet]
        [Authorize(Roles = "Administrador, Individuo")]
        public async Task<IActionResult> Get([FromQuery]InformacoesUteisModel informacoesUteisModel, string param, string sort = "Titulo",
          int skip = 1, int take = 10)
        {

            if (param != null)
            {
                byte[] data = System.Convert.FromBase64String(param);
                var decodeString = System.Text.ASCIIEncoding.ASCII.GetString(data);
                informacoesUteisModel = JsonConvert.DeserializeObject<InformacoesUteisModel>(decodeString);
            }
            informacoesUteisModel?.ValidarGet();
            informacoesUteisModel.Ativo = true;
            try
            {
                var (informacoesUteis, totalCount) = await _informacoesUteisApplication.GetByParam(
                    _mapper.Map<InformacoesUteis>(informacoesUteisModel),
                    sort,
                    skip,
                    take).ConfigureAwait(false);

                var totalPages = (int)Math.Ceiling((double)totalCount / take);
                var response = new
                {
                    items = _mapper.Map<IEnumerable<InformacoesUteisModel>>(informacoesUteis),
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
                _logger.LogError($"InformacoesUteisList: {informacoesUteisModel}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        [HttpPost]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Post([FromBody] InformacoesUteisModel informacoesUteisModel)
        {

            try
            {
                if (informacoesUteisModel == null) return BadRequest("Necessário informar dados das Informações Uteis");
                informacoesUteisModel.ValidarAdicionar();
                if (informacoesUteisModel.Invalid)
                {
                    return BadRequest(informacoesUteisModel);
                }

                var informacoesUteis = _mapper.Map<InformacoesUteis>(informacoesUteisModel);

                _informacoesUteisApplication.Add(informacoesUteis);

                informacoesUteisModel = _mapper.Map<InformacoesUteisModel>(informacoesUteis);

                return Created($"api/informacoesuteis/{informacoesUteisModel.Id}", informacoesUteisModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"InformacoesUteisPost: {informacoesUteisModel}");
                return BadRequest(e.Message.ToString());
            }
        }

        //PUT api/informacoesuteis/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Put(string id, [FromBody] InformacoesUteisModel informacoesUteisModel)
        {
            try
            {
                if (informacoesUteisModel == null) return BadRequest("Informe o registro a ser alterado");

                informacoesUteisModel.ValidarEditar();
                if (informacoesUteisModel.Invalid)
                {
                    return BadRequest(informacoesUteisModel);
                }

                var informacoesUteisExists = _informacoesUteisApplication.GetById(id);
                if (informacoesUteisExists == null)
                {
                    informacoesUteisModel.AddNotification("InformacoesUteis.Id", "Registro não encontrado");
                    return NotFound();
                }

                _informacoesUteisApplication.Update(id, _mapper.Map<InformacoesUteis>(informacoesUteisModel));

                return new NoContentResult();
            }
            catch (Exception e)
            {
                _logger.LogError($"InformacoesUteisPut: {informacoesUteisModel}");
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
                _informacoesUteisApplication.Delete(id);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"InformacoesUteisDelete: {id}");
                return BadRequest(e.Message.ToString());
            }
        }

    }
}
