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
    [Route("api/v{version:apiVersion}/Estabelecimentos/")]
    [Produces("application/json")]
    [Authorize]
    public class EstabelecimentosProcedimentosController : Controller
    {
        private readonly IEstabelecimentoProcedimentoApplication _estabelecimentoProcedimentoApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public EstabelecimentosProcedimentosController(IEstabelecimentoProcedimentoApplication estabelecimentoProcedimentoApplication,
                                                    IMapper mapper,
                                                    ILogger<EstabelecimentosProcedimentosController> logger)
        {
            _estabelecimentoProcedimentoApplication = estabelecimentoProcedimentoApplication;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Retorna uma lista de Estabelecimentos e seus Procedimentos
        /// </summary>
        /// <param name="estabelecimentoProcedimentoModel"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="appParams.Positivo"></param>
        /// <returns>Retorna uma lista de EstabelecimentoProcedimentos</returns>
        /// <response code="200">EstabelecimentoProcedimento(s) encontrado(s)</response>
        /// <response code="204">Nenhum EstabelecimentoProcedimento encontrado</response>
        /// <response code="404">Erro ao procurar EstabelecimentoProcedimento</response>
        [HttpGet]
        [Route("Procedimentos")]
        [Authorize(Roles = "Retaguarda, Individuo")]
        public async Task<IActionResult> Get([FromQuery] EstabelecimentoProcedimentoModel estabelecimentoProcedimentoModel, [FromQuery] AppParams appParams, string param, string sort = "Cota",
          int skip = 1, int take = 10)
        {
            if (param != null)
            {
                byte[] data = System.Convert.FromBase64String(param);
                var decodeString = System.Text.ASCIIEncoding.ASCII.GetString(data);
                estabelecimentoProcedimentoModel = JsonConvert.DeserializeObject<EstabelecimentoProcedimentoModel>(decodeString);
            }
            estabelecimentoProcedimentoModel?.ValidarGet();
            try
            {
                var (estabelecimentoProcedimentos, totalCount) = await _estabelecimentoProcedimentoApplication.GetByParam(
                    _mapper.Map<EstabelecimentoProcedimento>(estabelecimentoProcedimentoModel), appParams,
                    sort,
                    skip,
                    take).ConfigureAwait(false);

                var totalPages = (int)Math.Ceiling((double)totalCount / take);
                var response = new
                {
                    items = _mapper.Map<IEnumerable<EstabelecimentoProcedimentoModel>>(estabelecimentoProcedimentos),
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
                _logger.LogError($"EstabelecimentoProcedimentoGet: {estabelecimentoProcedimentoModel}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        /// <summary>
        /// Retorna os Procedimentos do Estabelecimento.
        /// </summary>
        /// <param name="estabelecimentoId"></param>
        [HttpGet("{estabelecimentoId}/Procedimentos")]
        [AllowAnonymous]
        public IActionResult Get(string estabelecimentoId)
        {
            try
            {
                var estabelecimentoProcedimentosModel = _mapper.Map<EstabelecimentoModel>(_estabelecimentoProcedimentoApplication.GetById(estabelecimentoId));

                if (estabelecimentoProcedimentosModel == null) return NotFound();

                return Ok(estabelecimentoProcedimentosModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"EstabelecimentoGetById: {DateTime.Now}, {estabelecimentoId}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        [HttpPost]
        [Route("Procedimentos")]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Post([FromBody] IEnumerable<EstabelecimentoProcedimentoModel> estabelecimentoProcedimentoModel)
        {

            try
            {
                if (estabelecimentoProcedimentoModel == null) return BadRequest("A Lista de Procedimentos está vazia.");
                foreach (var item in estabelecimentoProcedimentoModel)
                {
                    item.ValidarAdicionar();
                    if (!item.Invalid) BadRequest(item);
                }

                var list = _mapper.Map<IEnumerable<EstabelecimentoProcedimento>>(estabelecimentoProcedimentoModel);

                _estabelecimentoProcedimentoApplication.Add(list);

                return Created($"api/estabelecimento/procedimentos", "estabelecimentoProcedimentoModel");
            }
            catch (Exception e)
            {
                _logger.LogError($"EstabelecimentoProcedimentoPost: {estabelecimentoProcedimentoModel}");
                return BadRequest(e.Message.ToString());
            }
        }

        //vem um array e faz delete dos itens
        [HttpDelete("DeleteProcedimentos")]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Delete([FromBody] IEnumerable<EstabelecimentoProcedimentoModel> listModel)
        {

            try
            {
                if (listModel == null) return BadRequest("A Lista de Procedimentos está vazia.");
                foreach (var item in listModel)
                {
                    item.ValidarAdicionar();
                    if (!item.Invalid) BadRequest(item);
                }

                var list = _mapper.Map<IEnumerable<EstabelecimentoProcedimento>>(listModel);

                _estabelecimentoProcedimentoApplication.Delete(list);

                return Created($"api/EstabelecimentoProcedimento", "Procedimento do Domicilio Deletado com Sucesso!");
            }
            catch (Exception e)
            {
                _logger.LogError($"EstabelecimentoProcedimentoDelete: {listModel}");
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
