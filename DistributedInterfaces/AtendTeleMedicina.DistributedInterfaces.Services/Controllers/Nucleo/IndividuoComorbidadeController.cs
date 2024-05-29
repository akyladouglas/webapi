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
    [Route("api/v{version:apiVersion}/IndividuoComorbidade")]
    [Produces("application/json")]
    [Authorize]
    public class IndividuoComorbidadeController : Controller
    {
        private readonly IIndividuoApplication _individuoApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public IndividuoComorbidadeController(IIndividuoApplication individuoApplication,
                                  IMapper mapper,
                                  ILogger<IndividuoComorbidadeController> logger)
        {
            _individuoApplication = individuoApplication;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Atualiza as Comorbidades do Indivíduo
        /// </summary>
        /// <param name="individuoId"></param>
        /// <param name="individuoComorbidade"></param>
        [HttpPut("{individuoId}/Comorbidades")]
        [Authorize(Roles = "Retaguarda, Individuo, Triagem")]
        public IActionResult Put(string individuoId, Individuo individuoComorbidade)
        {

            //return null;
            try
            {
                if (string.IsNullOrEmpty(individuoId)) return BadRequest();
                if (individuoComorbidade == null) return BadRequest();

                _individuoApplication.AtualizarComorbidadeTriagem(individuoId, individuoComorbidade);

                return Ok(individuoComorbidade);
            }
            catch (Exception e)
            {
                _logger.LogError($"Erro ao atualizar Comorbidade: {DateTime.Now}, {individuoId}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }


    }
}
