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
    [Route("api/v{version:apiVersion}/EstabelecimentosProfissional/")]
    [Produces("application/json")]
    [Authorize]
    public class EstabelecimentosProfissionalController : Controller
    {
        private readonly IEstabelecimentoProfissionalApplication _estabelecimentoProfissionalApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public EstabelecimentosProfissionalController(IEstabelecimentoProfissionalApplication estabelecimentoProfissionalApplication,
                                                    IMapper mapper,
                                                    ILogger<EstabelecimentosProcedimentosController> logger)
        {
            _estabelecimentoProfissionalApplication = estabelecimentoProfissionalApplication;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Retorna uma lista de Estabelecimentos e seus Procedimentos
        /// </summary>
        /// <param name="estabelecimentoProfissionaisModel"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="appParams.Positivo"></param>
        /// <returns>Retorna uma lista de EstabelecimentoProcedimentos</returns>
        /// <response code="200">EstabelecimentoProcedimento(s) encontrado(s)</response>
        /// <response code="204">Nenhum EstabelecimentoProcedimento encontrado</response>
        /// <response code="404">Erro ao procurar EstabelecimentoProcedimento</response>
        [HttpGet]
        [Route("Estabelecimentos")]
        [Authorize(Roles = "Retaguarda, Individuo")]
        public async Task<IActionResult> Get([FromQuery] EstabelecimentoProfissionalModel estabelecimentoProfissionalModel, [FromQuery] AppParams appParams, string param, string sort = "Cota",
          int skip = 1, int take = 10)
        {
            if (param != null)
            {
                byte[] data = System.Convert.FromBase64String(param);
                var decodeString = System.Text.ASCIIEncoding.ASCII.GetString(data);
                estabelecimentoProfissionalModel = JsonConvert.DeserializeObject<EstabelecimentoProfissionalModel>(decodeString);
            }
            estabelecimentoProfissionalModel?.ValidarGet();
            try
            {
                var (estabelecimentoProfissional, totalCount) = await _estabelecimentoProfissionalApplication.GetByParam(
                    _mapper.Map<EstabelecimentoProfissional>(estabelecimentoProfissionalModel), appParams,
                    sort,
                    skip,
                    take).ConfigureAwait(false);

                var totalPages = (int)Math.Ceiling((double)totalCount / take);
                var response = new
                {
                    items = _mapper.Map<IEnumerable<EstabelecimentoProfissionalModel>>(estabelecimentoProfissional),
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
                _logger.LogError($"EstabelecimentoProfissionalGet: {estabelecimentoProfissionalModel}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        /// <summary>
        /// Retorna os Procedimentos do Estabelecimento.
        /// </summary>
        /// <param name="profissionalId"></param>
        [HttpGet("{profissionalId}/Estabelecimentos")]
        [Authorize(Roles = "Retaguarda, Individuo")]
        public IActionResult Get(string profissionalId)
        {
            try
            {
                var estabelecimentoProfissionalModel = _mapper.Map<EstabelecimentoModel>(_estabelecimentoProfissionalApplication.GetById(profissionalId));

                if (estabelecimentoProfissionalModel == null) return NotFound();

                return Ok(estabelecimentoProfissionalModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"EstabelecimentoGetById: {DateTime.Now}, {profissionalId}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        [HttpPost]
        //[Route("Adicionar")]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Post([FromBody] IEnumerable<EstabelecimentoProfissionalModel> estabelecimentoProfissionalModel)
        {

            try
            {
                if (estabelecimentoProfissionalModel == null) return BadRequest("A Lista de Profissionais está vazia.");
                foreach (var item in estabelecimentoProfissionalModel)
                {
                    item.ValidarAdicionar();
                    if (!item.Invalid) BadRequest(item);
                }

                var list = _mapper.Map<IEnumerable<EstabelecimentoProfissional>>(estabelecimentoProfissionalModel);

                _estabelecimentoProfissionalApplication.Add(list);

                return Created($"api/estabelecimentos/adicionar", "estabelecimentoProfissionaisModel");
            }
            catch (Exception e)
            {
                _logger.LogError($"EstabelecimentoProfissionalPost: {estabelecimentoProfissionalModel}");
                return BadRequest(e.Message.ToString());
            }
        }

        ////PUT api/estabelecimentoProcedimento/{id}
        //[HttpPut("{id}")]
        //[Authorize(Roles = "Retaguarda")]
        //public IActionResult Put(string id, [FromBody] EstabelecimentoProcedimentoModel estabelecimentoProcedimentoModel)
        //{
        //    try
        //    {
        //        if (estabelecimentoProcedimentoModel == null) return BadRequest("Informe o registro a ser alterado");

        //        estabelecimentoProcedimentoModel.ValidarEditar();
        //        if (estabelecimentoProcedimentoModel.Invalid)
        //        {
        //            return BadRequest(estabelecimentoProcedimentoModel);
        //        }

        //        _estabelecimentoProcedimentoApplication.Update(id, _mapper.Map<EstabelecimentoProcedimento>(estabelecimentoProcedimentoModel));

        //        return new NoContentResult();
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError($"EstabelecimentoProcedimentoPut: {estabelecimentoProcedimentoModel}");
        //        return BadRequest(e.Message.ToString());
        //    }
        //}

    }
}
