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
    [AllowAnonymous]
    public class DocumentoController : Controller
    {
        private readonly IDocumentoApplication _documentoApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public DocumentoController(IDocumentoApplication documentoApplication,
                                    IMapper mapper,
                                    ILogger<DocumentoController> logger)
        {
            _documentoApplication = documentoApplication;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Retorna uma lista de Documento
        /// </summary>
        /// <param name="documentoModel"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns>Retorna uma lista de Documentos</returns>
        /// <response code="200">Documento(s) ecnontrada(s)</response>
        /// <response code="204">Nenhum Procedimento encontrado</response>
        /// <response code="404">Erro ao procurar Procedimento</response>
        [HttpGet]
        [Authorize(Roles = "Retaguarda, Individuo")]
        public async Task<IActionResult> Get([FromQuery] DocumentoModel documentoModel, string param, string sort = "d.DataCadastro",
          int skip = 1, int take = 10000)
        {

            if (param != null)
            {
                byte[] data = System.Convert.FromBase64String(param);
                var decodeString = System.Text.ASCIIEncoding.ASCII.GetString(data);
                documentoModel = JsonConvert.DeserializeObject<DocumentoModel>(decodeString);
            }
            try
            {
                var (documentos, totalCount) = await _documentoApplication.GetByParam(
                    _mapper.Map<Documento>(documentoModel),
                    sort,
                    skip,
                    take).ConfigureAwait(false);

                var totalPages = (int)Math.Ceiling((double)totalCount / take);
                var response = new
                {
                    items = _mapper.Map<IEnumerable<DocumentoModel>>(documentos),
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
                _logger.LogError($"DocumentoGet: {documentoModel}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        /// <summary>
        /// Retorna um Documento específico.
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id}")]
        [Authorize(Roles = "Retaguarda, Individuo")]
        public IActionResult Get(string id)
        {
            try
            {
                var documentoModel = _mapper.Map<DocumentoModel>(_documentoApplication.GetById(id));

                if (documentoModel == null) return NotFound();

                return Ok(documentoModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"DocumentoGetById: {DateTime.Now}, {id}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        //POST api/documento
        [HttpPost]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Post([FromBody] DocumentoModel documentoModel)
        {

            try
            {
                if (documentoModel == null) return BadRequest("Necessário informar dados do Documento");

                if (documentoModel.Invalid)
                {
                    return BadRequest(documentoModel);
                }

                var documento = _mapper.Map<Documento>(documentoModel);

                _documentoApplication.Add(documento);

                documentoModel = _mapper.Map<DocumentoModel>(documento);

                return Created($"api/documento/{documentoModel.Id}", documentoModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"AgendamentoPost: {documentoModel}");
                return BadRequest(e.Message.ToString());
            }
        }

        //PUT api/documento/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Put(string id, [FromBody] DocumentoModel documentoModel)
        {
            try
            {
                if (documentoModel == null) return BadRequest("Informe o registro a ser alterado");

                if (documentoModel.Invalid)
                {
                    return BadRequest(documentoModel);
                }

                var agendamentoExists = _documentoApplication.GetById(id);
                if (agendamentoExists == null)
                {
                    documentoModel.AddNotification("Agendamento.Id", "Registro não encontrado");
                    return NotFound();
                }

                _documentoApplication.Update(id, _mapper.Map<Documento>(documentoModel));

                return new NoContentResult();
            }
            catch (Exception e)
            {
                _logger.LogError($"AgendamentoPut: {documentoModel}");
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
                _documentoApplication.Delete(id);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"AgendamentoDelete: {id}");
                return BadRequest(e.Message.ToString());
            }
        }
    }
}
