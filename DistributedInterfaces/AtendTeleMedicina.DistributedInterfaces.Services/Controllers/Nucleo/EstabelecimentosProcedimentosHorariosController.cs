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
    public class EstabelecimentosProcedimentosHorariosController : Controller
    {
        private readonly IEstabelecimentoProcedimentoHorarioApplication _estabelecimentoProcedimentoHorarioApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public EstabelecimentosProcedimentosHorariosController(IEstabelecimentoProcedimentoHorarioApplication estabelecimentoProcedimentoHorarioApplication,
                                                    IMapper mapper,
                                                    ILogger<EstabelecimentosProcedimentosHorariosController> logger)
        {
            _estabelecimentoProcedimentoHorarioApplication = estabelecimentoProcedimentoHorarioApplication;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Retorna uma lista de Estabelecimentos e seus Procedimentos
        /// </summary>
        /// <param name="estabelecimentoProcedimentoHorarioModel"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns>Retorna uma lista de EstabelecimentoProcedimentoHorarios</returns>
        /// <response code="200">EstabelecimentoProcedimentoHorario(s) encontrado(s)</response>
        /// <response code="204">Nenhum EstabelecimentoProcedimentoHorario encontrado</response>
        /// <response code="404">Erro ao procurar EstabelecimentoProcedimentoHorario</response>
        [HttpGet]
        [Authorize(Roles = "Retaguarda, App, Individuo")]
        public async Task<IActionResult> Get([FromQuery] EstabelecimentoProcedimentoHorarioModel estabelecimentoProcedimentoHorarioModel, [FromQuery] AppParams appParams, string param, string sort = "Dia, Hora",
          int skip = 1, int take = 10000)
        {
            if (param != null)
            {
                byte[] data = System.Convert.FromBase64String(param);
                var decodeString = System.Text.ASCIIEncoding.ASCII.GetString(data);
                estabelecimentoProcedimentoHorarioModel = JsonConvert.DeserializeObject<EstabelecimentoProcedimentoHorarioModel>(decodeString);
            }
            estabelecimentoProcedimentoHorarioModel?.ValidarGet();
            try
            {
                var (estabelecimentoProcedimentoHorarios, totalCount) = await _estabelecimentoProcedimentoHorarioApplication.GetByParam(
                    _mapper.Map<EstabelecimentoProcedimentoHorario>(estabelecimentoProcedimentoHorarioModel),
                    appParams,
                    sort,
                    skip,
                    take).ConfigureAwait(false);

                var totalPages = (int)Math.Ceiling((double)totalCount / take);
                var response = new
                {
                    items = _mapper.Map<IEnumerable<EstabelecimentoProcedimentoHorarioModel>>(estabelecimentoProcedimentoHorarios),
                    sort,
                    skip,
                    take,
                    totalCount,
                    totalPages
                };

                return Ok(response);

            }
            catch (Exception e)
            {
                _logger.LogError($"EstabelecimentoProcedimentoHorarioGet: {estabelecimentoProcedimentoHorarioModel}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        [HttpPost]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Post([FromBody] IEnumerable<EstabelecimentoProcedimentoHorarioModel> listModel)
        {

            try
            {
                if (listModel == null) return BadRequest("A Lista de Procedimentos está vazia.");
                foreach (var item in listModel)
                {
                    item.ValidarAdicionar();
                    if (!item.Invalid) BadRequest(item);
                }

                var list = _mapper.Map<IEnumerable<EstabelecimentoProcedimentoHorario>>(listModel);

                _estabelecimentoProcedimentoHorarioApplication.Add(list);

                return Created($"api/EstabelecimentosProcedimentoHorarios", "listModel");
            }
            catch (Exception e)
            {
                _logger.LogError($"EstabelecimentosProcedimentosHorariosPost: {listModel}");
                return BadRequest(e.Message.ToString());
            }
        }

        //PUT api/estabelecimentosProcedimentosHorarios/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Retaguarda, Médico, Recepcionista, Individuo")]
        public IActionResult Put(string id, [FromBody] EstabelecimentoProcedimentoHorarioModel estabelecimentoProcedimentoHorarioModel)
        {
            try
            {
                if (estabelecimentoProcedimentoHorarioModel == null) return BadRequest("Informe o registro a ser alterado");

                if (estabelecimentoProcedimentoHorarioModel.Invalid)
                {
                    return BadRequest(estabelecimentoProcedimentoHorarioModel);
                }

                var response = _estabelecimentoProcedimentoHorarioApplication.Update(id, _mapper.Map<EstabelecimentoProcedimentoHorario>(estabelecimentoProcedimentoHorarioModel));

                return Ok(response);
            }
            catch (Exception e)
            {
                _logger.LogError($"estabelecimentoProcedimentoHorarioPut: {estabelecimentoProcedimentoHorarioModel}");
                return BadRequest(e.Message.ToString());
            }
        }


    }
}
