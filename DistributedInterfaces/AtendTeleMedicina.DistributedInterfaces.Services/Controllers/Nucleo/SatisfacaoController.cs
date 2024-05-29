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
    public class SatisfacaoController : Controller
    {
        private readonly ISatisfacaoApplication _satisfacaoApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public SatisfacaoController(ISatisfacaoApplication satisfacaoApplication, IMapper mapper, ILoggerFactory logger)
        {
            _satisfacaoApplication = satisfacaoApplication;
            _mapper = mapper;
            _logger = logger.CreateLogger<SatisfacaoController>();
        }

        //POST api/satisfacao
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Post([FromBody] SatisfacaoModel satisfacaoModel)
        {

            try
            {
                if (satisfacaoModel == null) return BadRequest("Necessário informar dados da Avaliação");
               

                var satisfacao = _mapper.Map<Satisfacao>(satisfacaoModel);

                _satisfacaoApplication.Add(satisfacao);

                satisfacaoModel = _mapper.Map<SatisfacaoModel>(satisfacao);

                return Created($"api/satisfacao/{satisfacaoModel.Id}", satisfacaoModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"NotificacaoPost: {satisfacaoModel}");
                return BadRequest(e.Message.ToString());
            }
        }

    }
}
