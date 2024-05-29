using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using Microsoft.AspNetCore.Authorization;
using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class AvaliacaoController : Controller
    {

        private readonly IAvaliacaoApplication _avaliacaoApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IUser _user;
        private static IHostingEnvironment _environment;

        public AvaliacaoController(IAvaliacaoApplication avaliacaoApplication,
                                   IMapper mapper,
                                   ILogger<AvaliacaoController> logger,
                                   IUser user,
                                   IHostingEnvironment environment)
        {
            _avaliacaoApplication = avaliacaoApplication;
            _mapper = mapper;
            _logger = logger;
            _user = user;
            _environment = environment;
        }

        //POST api/avaliacao
        [HttpPost]
        [Authorize(Roles = "Retaguarda, Médico, Recepcionista, Triagem, Individuo")]
        public IActionResult Post([FromBody] AvaliacaoModel avaliacaoModel)
        {

            try
            {
                if (avaliacaoModel == null) return BadRequest("Necessário informar dados da Avaliação");

                if (avaliacaoModel.Invalid)
                {
                    return BadRequest(avaliacaoModel);
                }

                var avaliacao = _mapper.Map<Avaliacao>(avaliacaoModel);
                var avaliacaoSalva = _avaliacaoApplication.Add(avaliacao);
               /* if (agendamento.avaliacaoSalva != null)
                {
                    var exists = _agendamentoApplication.GetById(agendamento.RetornoAgendamentoId);

                    if (exists != null)
                    {

                        exists.VinculoRetorno = true;
                        _agendamentoApplication.Update(exists.Id, exists);
                    }
                }*/

               // agendamentoModel = _mapper.Map<AgendamentoModel>(agendamento);

                return Created("", "");
            }
            catch (Exception e)
            {
                _logger.LogError($"AgendamentoPost: {"dddd"}");
                return BadRequest(e.Message.ToString());
            }
        }

    }
}
