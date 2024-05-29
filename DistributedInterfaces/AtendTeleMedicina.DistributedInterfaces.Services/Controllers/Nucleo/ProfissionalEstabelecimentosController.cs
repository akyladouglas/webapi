using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Authorization;
using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo;
using System.Collections.Generic;
using AtendTeleMedicina.Domain.Entities.Nucleo;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Profissionais/")]
    [Produces("application/json")]
    [Authorize]
    public class ProfissionalEstabelecimentosController : Controller
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IEstabelecimentoProfissionalApplication _estabelecimentoProfissionalApplication;



        public ProfissionalEstabelecimentosController(IEstabelecimentoProfissionalApplication estabelecimentoProfissionalApplication,
                                                    IMapper mapper,
                                                    ILogger<ProfissionalEstabelecimentosController> logger)
        {
            _estabelecimentoProfissionalApplication = estabelecimentoProfissionalApplication;
            _mapper = mapper;
            _logger = logger;
        }


        /// <summary>
        /// Retorna o Profissional com seus Estabelecimentos.
        /// </summary>
        /// <param name="profissionalId"></param>
        //[HttpGet("{profissionalId}/Estabelecimentos")]
        //[Authorize(Roles = "Retaguarda, App")]
        //public IActionResult Get(string profissionalId)
        //{
        //    try
        //    {
        //        var profissionalEstabelecimentosModel = _mapper.Map<EstabelecimentoModel>(_estabelecimentoProfissionalApplication.GetById(profissionalId));

        //        if (profissionalEstabelecimentosModel == null) return NotFound();

        //        return Ok(profissionalEstabelecimentosModel);
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError($"ProfissionalEstabelecimentosGet: {DateTime.Now}, {profissionalId}, {e}");
        //        return BadRequest(e.Message.ToString());
        //    }
        //}

    }
}
