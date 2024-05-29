using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Security;
using Microsoft.Extensions.Configuration;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Individuo")]
    [Produces("application/json")]
    [Authorize]
    public class IndividuoFaceTokenController : Controller
    {
        private readonly IIndividuoApplication _individuoApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public IndividuoFaceTokenController(IIndividuoApplication individuoApplication,
                                  IMapper mapper,
                                  ILogger<IndividuoSenhaController> logger,
                                  IConfiguration config)
        {
            _individuoApplication = individuoApplication;
            _mapper = mapper;
            _logger = logger;
            _config = config;
        }

        /// <summary>
        /// Gera Token para autenticação facial
        /// </summary>
        /// <param name="usuario"></param>
        /// <remarks>
        /// ### Observação ###
        /// <p>O objeto deve ser enviado totalmente em Base64: </p>
        /// </remarks>
        [HttpPost("{id}/FaceToken")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public IActionResult Post(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id)) return BadRequest();

                var faceToken= _individuoApplication.GenerateFaceToken(id);

                return Ok(faceToken);
            }
            catch (Exception e)
            {
                _logger.LogError($"FaceToken: {DateTime.Now}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }
    }
}
