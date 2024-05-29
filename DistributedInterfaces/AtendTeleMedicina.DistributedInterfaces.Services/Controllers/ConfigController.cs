using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using KissLog;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo;
using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers
{
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class ConfigController : Controller
    {
        private readonly IConfiguracaoApplication _configuracaoApplication;
        private readonly IConfiguration _config;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private readonly KissLog.ILogger _kissLogger;
        private readonly IMapper _mapper;

        public ConfigController(IConfiguration configuration, IConfiguracaoApplication configuracaoApplication, IMapper mapper, ILoggerFactory logger, KissLog.ILogger kissLogger)
        {
            _config = configuration;
            _mapper = mapper;
            _logger = logger.CreateLogger<ConfigController>();
            _kissLogger = kissLogger;
            _configuracaoApplication = configuracaoApplication;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Get(Configuracao configModel)
        {
            try
            {
                var config = _configuracaoApplication.Get(configModel);

                return Ok(config);
            }
            catch (Exception e)
            {
                _logger.LogError($"ERRO: {configModel} , {e}");

                return BadRequest($"ERRO: {e}");
            }
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Put([FromBody] ConfiguracaoModel configModel)
        {
            try
            {
                if (configModel == null) return BadRequest("Dados incompletos");

                configModel.UsuarioAlterouId = User.FindFirstValue("uid");

                _configuracaoApplication.Update(_mapper.Map<Configuracao>(configModel), configModel.UsuarioAlterouId);

                return Ok(configModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"ERRO: {configModel} , {e}");

                return BadRequest($"ERRO: {e}");
            }
        }

    }
}
