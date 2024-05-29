using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/procedimento")]
    [Produces("application/json")]
    [AllowAnonymous]
    public class ProcedimentoCotaController : Controller
    {
        private readonly IProcedimentoApplication _procedimentoApplication;
        private readonly ILogger _logger;
        private readonly IUser _user;

        public ProcedimentoCotaController(IProcedimentoApplication procedimentoApplication,
                                    ILogger<ProcedimentoController> logger,
                                    IUser user
            )
        {
            _procedimentoApplication = procedimentoApplication;
            _logger = logger;
            _user = user;
        }


        /// <summary>
        /// Atualiza as cotas para o Procedimento.
        /// </summary>
        /// <param name="procedimentoId"></param>
        /// <param name="quantidade"></param>
        [HttpPut("{procedimentoId}/cota")]
        [Authorize(Roles = "Retaguarda, App")]
        public IActionResult Put(string procedimentoId, int quantidade)
        {
            try
            {
                if (string.IsNullOrEmpty(procedimentoId)) return BadRequest();
                if (quantidade == 0) return BadRequest();

                string userId = _user.GetUserId();

                _procedimentoApplication.AdicionarCota(procedimentoId, quantidade, userId);

                return new NoContentResult();
            }
            catch (Exception e)
            {
                _logger.LogError($"Procedimento cota: {DateTime.Now}, {procedimentoId}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }
    }
}
