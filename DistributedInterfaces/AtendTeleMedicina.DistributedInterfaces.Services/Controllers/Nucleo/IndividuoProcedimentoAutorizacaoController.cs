using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo;
using AtendTeleMedicina.Domain.Params;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class IndividuoProcedimentoAutorizacaoController : Controller
    {
        private readonly IIndividuoProcedimentoAutorizacaoApplication _individuoProcedimentoAutorizacaoApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public IndividuoProcedimentoAutorizacaoController(IIndividuoProcedimentoAutorizacaoApplication individuoProcedimentoAutorizacaoApplication,
                                                    IMapper mapper,
                                                    ILogger<IndividuoProcedimentoAutorizacaoController> logger)
        {
            _individuoProcedimentoAutorizacaoApplication = individuoProcedimentoAutorizacaoApplication;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Post([FromBody] IEnumerable<IndividuoProcedimentoAutorizacaoModel> listModel)
        {

            try
            {
                if (listModel == null) return BadRequest("A Lista de Procedimentos está vazia.");
                foreach (var item in listModel)
                {
                    item.ValidarAdicionar();
                    if (!item.Invalid) BadRequest(item);
                }

                var list = _mapper.Map<IEnumerable<IndividuoProcedimentoAutorizacao>>(listModel);

                _individuoProcedimentoAutorizacaoApplication.Add(list);

                return Created($"api/IndividuoProcedimentoAutorizacao", "Procedimento Adicionado ao Paciente com Sucesso!");
            }
            catch (Exception e)
            {
                _logger.LogError($"IndividuoProcedimentoAutorizacaoPost: {listModel}");
                return BadRequest(e.Message.ToString());
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Delete([FromBody] IEnumerable<IndividuoProcedimentoAutorizacaoModel> listModel)
        {

            try
            {
                if (listModel == null) return BadRequest("A Lista de Procedimentos está vazia.");
                foreach (var item in listModel)
                {
                    item.ValidarAdicionar();
                    if (!item.Invalid) BadRequest(item);
                }

                var list = _mapper.Map<IEnumerable<IndividuoProcedimentoAutorizacao>>(listModel);

                _individuoProcedimentoAutorizacaoApplication.Delete(list);

                return Created($"api/IndividuoProcedimentoAutorizacao", "Procedimento do Paciente Deletado com Sucesso!");
            }
            catch (Exception e)
            {
                _logger.LogError($"IndividuoProcedimentoAutorizacaoPost: {listModel}");
                return BadRequest(e.Message.ToString());
            }
        }

    }
}
