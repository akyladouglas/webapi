using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Net;
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
    public class IndividuoNotificacaoController : Controller
    {
        private readonly IIndividuoApplication _individuoApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public IndividuoNotificacaoController(IIndividuoApplication individuoApplication,
                                  IMapper mapper,
                                  ILogger<IndividuoController> logger)
        {
            _individuoApplication = individuoApplication;
            _mapper = mapper;
            _logger = logger;
        }

        //PUT api/individuoNotificacao/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Retaguarda, Individuo")]
        public IActionResult Put(string id, [FromBody] string notificacaoToken)
        {
            try
            {
                if (notificacaoToken == null) return BadRequest("Informe o registro a ser alterado");


                    var individuoExists = _individuoApplication.GetById(id);
                    if (individuoExists == null)
                    {
                        return NotFound();
                    }

                    _individuoApplication.UpdateNotificacaoToken(id, notificacaoToken);

                    return Ok(notificacaoToken);

            }
            catch (Exception e)
            {
                _logger.LogError($"IndividuoPut: {notificacaoToken}");
                return BadRequest(e.Message.ToString());
            }
        }
    }
}
