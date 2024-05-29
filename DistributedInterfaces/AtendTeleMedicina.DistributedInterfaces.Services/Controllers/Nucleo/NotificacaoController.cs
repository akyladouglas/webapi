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
    public class NotificacaoController : Controller
    {
        private readonly INotificacaoApplication _notificacaoApplication;
        private readonly IIndividuoNotificacaoApplication _individuoNotificacaoApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public NotificacaoController(
            INotificacaoApplication notificacaoApplication,
            IMapper mapper,
            ILoggerFactory logger,
            IIndividuoNotificacaoApplication individuoNotificacaoApplication
            )
        {
            _notificacaoApplication = notificacaoApplication;
            _individuoNotificacaoApplication = individuoNotificacaoApplication;
            _mapper = mapper;
            _logger = logger.CreateLogger<NotificacaoController>();
        }

        /// <summary>
        /// Retorna uma Notificação.
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id}")]
        [Authorize(Roles = "Retaguarda, App")]
        public IActionResult Get(string id)
        {
            try
            {
                var estabelecimentoModel = _mapper.Map<NotificacaoModel>(_notificacaoApplication.GetById(id));

                if (estabelecimentoModel == null) return NotFound();

                return Ok(estabelecimentoModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"NotificacaoId: {DateTime.Now}, {id}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        /// <summary>
        /// Retorna uma lista de Notificacao
        /// </summary>
        /// <param name="notificacaoModel"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns>Retorna uma lista de Notificacao</returns>
        /// <response code="200">Notificacoes ecnontrada(s)</response>
        /// <response code="204">Nenhum Notificacao encontrado</response>
        /// <response code="404">Erro ao procurar Notificacao</response>
        [HttpGet]
        [Authorize(Roles = "Retaguarda, App, Individuo")]
        public async Task<IActionResult> Get([FromQuery] NotificacaoModel notificacaoModel, string param, string sort = "DataCadastro",
          int skip = 1, int take = 100)
        {

            if (param != null)
            {
                byte[] data = System.Convert.FromBase64String(param);
                var decodeString = System.Text.ASCIIEncoding.ASCII.GetString(data);
                notificacaoModel = JsonConvert.DeserializeObject<NotificacaoModel>(decodeString);
            }
            try
            {
                var (notificacoes, totalCount) = await _notificacaoApplication.GetByParam(
                    _mapper.Map<Notificacao>(notificacaoModel),
                    sort,
                    skip,
                    take).ConfigureAwait(false);

                var totalPages = (int)Math.Ceiling((double)totalCount / take);
                var response = new
                {
                    items = _mapper.Map<IEnumerable<NotificacaoModel>>(notificacoes),
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
                _logger.LogError($"AgendamentoGet: {notificacaoModel}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        //POST api/notificacao
        [HttpPost]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Post([FromBody] NotificacaoModel notificacaoModel)
        {

            try
            {
                if (notificacaoModel == null) return BadRequest("Necessário informar dados da Notificação");
                notificacaoModel.ValidarAdicionar();
                if (notificacaoModel.Invalid)
                {
                    return BadRequest(notificacaoModel);
                }

                var notificacao = _mapper.Map<Notificacao>(notificacaoModel);

                _notificacaoApplication.Add(notificacao);

                notificacaoModel = _mapper.Map<NotificacaoModel>(notificacao);

                IndividuoNotificacao individuoNotificacao = new IndividuoNotificacao
                {

                    NotificacaoId = notificacaoModel.Id,

                };

                if (notificacaoModel.IndividuoIds.Length != 0)
                {
                    foreach (var individuoId in notificacaoModel.IndividuoIds)
                    {
                        individuoNotificacao.IndividuoId = individuoId;
                        _individuoNotificacaoApplication.Add(individuoNotificacao);
                    }
                }
                else
                {

                    individuoNotificacao.IndividuoId = null;
                    _individuoNotificacaoApplication.Add(individuoNotificacao);


                }

                return Created($"api/notificacao/{notificacaoModel.Id}", notificacaoModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"NotificacaoPost: {notificacaoModel}");
                return BadRequest(e.Message.ToString());
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Put(string id, [FromBody] NotificacaoModel notificacaoModel)
        {
            try
            {
                if (notificacaoModel == null) return BadRequest("Informe o registro a ser alterado");

                notificacaoModel.ValidarEditar();
                if (notificacaoModel.Invalid)
                {
                    return BadRequest(notificacaoModel);
                }

                var notificacaoExists = _notificacaoApplication.GetById(id);
                if (notificacaoExists == null)
                {
                    notificacaoModel.AddNotification("Notificacao.Id", "Registro não encontrado");
                    return NotFound();
                }

                _notificacaoApplication.Update(id, _mapper.Map<Notificacao>(notificacaoModel));

                return new NoContentResult();
            }
            catch (Exception e)
            {
                _logger.LogError($"NotificacaoPut: {notificacaoModel}");
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
                _notificacaoApplication.Delete(id);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"NotificacaoDelete: {id}");
                return BadRequest(e.Message.ToString());
            }
        }

    }
}
