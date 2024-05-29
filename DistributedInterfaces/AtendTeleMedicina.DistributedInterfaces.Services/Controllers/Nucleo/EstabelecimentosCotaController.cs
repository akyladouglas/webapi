using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Estabelecimentos")]
    [Produces("application/json")]
    [AllowAnonymous]
    public class EstabelecimentosCotaController : Controller
    {
        private readonly IEstabelecimentoApplication _estabelecimentoApplication;
        private readonly ILogger _logger;
        private readonly IUser _user;

        public EstabelecimentosCotaController(IEstabelecimentoApplication estabelecimentoApplication,
                                    ILogger<ProcedimentoController> logger,
                                    IUser user
            )
        {
            _estabelecimentoApplication = estabelecimentoApplication;
            _logger = logger;
            _user = user;
        }


        /// <summary>
        /// Atualiza as cotas para o Procedimento.
        /// </summary>
        /// <param name="estabelecimentoId"></param>
        /// <param name="procedimentoId"></param>
        /// <param name="quantidade"></param>
        [HttpPut("{estabelecimentoId}/distribuir-cota")]
        [Authorize(Roles = "Retaguarda, App")]
        public IActionResult Put(string estabelecimentoId, string procedimentoId, int quantidade)
        {
            try
            {
                if (string.IsNullOrEmpty(estabelecimentoId)) return BadRequest();
                if (quantidade == 0) return BadRequest();

                string userId = _user.GetUserId();

                _estabelecimentoApplication.DistribuirCota(estabelecimentoId, procedimentoId, quantidade, userId);

                return Ok("Cota Distribuída com sucesso.");
            }
            catch (Exception e)
            {
                _logger.LogError($"distribuir-cota: {DateTime.Now}, estabelecimento: {estabelecimentoId}, procedimento: {procedimentoId}:  {e}");
                return BadRequest(e.Message.ToString());
            }
        }
    }
}
