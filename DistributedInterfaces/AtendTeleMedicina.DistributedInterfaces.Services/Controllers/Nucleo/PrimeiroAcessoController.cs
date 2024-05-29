using AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo;
using AutoMapper;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtendTeleMedicina.Application.Contracts.Nucleo;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class PrimeiroAcessoController : Controller
    {
        private IPrimeiroAcessoApplication _primeiroAcesso;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public PrimeiroAcessoController(IPrimeiroAcessoApplication primeiroAcesso, ILogger<PrimeiroAcessoController> logger, IMapper mapper)
        {
            _primeiroAcesso = primeiroAcesso;
            _logger = logger;
            _mapper = mapper;
        }


        /// <summary>
        /// Valida dados do usuário com api do governo
        /// </summary>
        /// <param name="individuoModel"></param>
        [HttpPost]
        public IActionResult Post([FromBody] IndividuoModel individuoModel)
        {
            var individuo = _mapper.Map<Individuo>(individuoModel);
            try
            {
                if (individuoModel == null) return BadRequest("Necessário informar seus dados para continuar com acesso");
                _primeiroAcesso.Add(individuo);
            } catch (Exception e)
            {
                _logger.LogError($"BairroPost: {individuoModel}");
                return BadRequest(e.Message.ToString());
            }
            return Ok(individuoModel);

        }

        [HttpPost("confirm")]
        public IActionResult Confirm([FromBody] IndividuoModel individuoModel)
        {
            var individuo = _mapper.Map<Individuo>(individuoModel);
            try
            {
                if (individuo == null) return BadRequest("Necessário informar seus dados para continuar com acesso");

                var usuarioConfirmado = _primeiroAcesso.ConfirmData(individuo);
            }
            catch (Exception e)
            {
                _logger.LogError($"BairroPost: {individuoModel}");
                return BadRequest(e.Message.ToString());
            }
            return Ok(individuo);
        }
    }
}
