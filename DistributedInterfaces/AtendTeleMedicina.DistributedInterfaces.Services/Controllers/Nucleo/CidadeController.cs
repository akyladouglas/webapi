using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class CidadeController : Controller
    {
        private readonly ICidadeApplication _cidadeApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public CidadeController(ICidadeApplication cidadeApplication, IMapper mapper, ILogger<CidadeController> logger)
        {
            _cidadeApplication = cidadeApplication;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Retorna uma lista de Cidade
        /// </summary>
        /// <param name="cidadeModel"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns>Retorna uma lista de Cidades</returns>
        /// <response code="200">Cidade(s) ecnontrada(s)</response>
        /// <response code="204">Nenhuma Cidade Encontrada</response>
        /// <response code="404">Erro ao procurar Cidade</response>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromQuery]CidadeModel cidadeModel, string param, string filters, string sort = "Nome",
          int skip = 1, int take = 10)
        {
            if (param != null)
            {
                byte[] data = System.Convert.FromBase64String(param);
                var decodeString = System.Text.ASCIIEncoding.ASCII.GetString(data);
                cidadeModel = JsonConvert.DeserializeObject<CidadeModel>(decodeString);
            }
            cidadeModel?.ValidarGet();
            try
            {
                var (cidades, totalCount) = await _cidadeApplication.GetByParam(
                    _mapper.Map<Cidade>(cidadeModel),
                    sort,
                    skip,
                    take).ConfigureAwait(false);

                if (cidades == null) return NoContent();
                if (!cidades.Any()) return NoContent();

                var totalPages = (int)Math.Ceiling((double)totalCount / take);
                var response = new
                {
                    items = _mapper.Map<IEnumerable<CidadeModel>>(cidades),
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
                _logger.LogError($"CidadeGet: {cidadeModel}, {e}");
                return NotFound();
            }
        }

        /// <summary>
        /// Retorna uma Cidade específica.
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id}", Name = "CidadesId")]
        [AllowAnonymous]
        public IActionResult Get(string id)
        {
            try
            {
                var cidadeModel = _mapper.Map<CidadeModel>(_cidadeApplication.GetById(id));

                if (cidadeModel == null) return NoContent();

                return Ok(cidadeModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"CidadeGetById: {DateTime.Now}, {id}, {e}");
                Debug.WriteLine(e);
                return NotFound();
            }

        }

        //[HttpGet]
        //[AllowAnonymous]
        //[Route("GetApp")]
        //public async Task<IActionResult> GetApp([FromQuery]CidadeModel cidadeModel, string param, string filters, string sort = "Nome",
        //  int skip = 1, int take = 10)
        //{

        //    if (param != null)
        //    {
        //        byte[] data = System.Convert.FromBase64String(param);
        //        var decodeString = System.Text.ASCIIEncoding.ASCII.GetString(data);
        //        cidadeModel = JsonConvert.DeserializeObject<CidadeModel>(decodeString);
        //    }
        //    cidadeModel?.ValidarGet();
        //    try
        //    {
        //        var (cidades, totalCount) = await _cidadeApplication.GetByParam(
        //            _mapper.Map<Cidade>(cidadeModel),
        //            sort,
        //            skip,
        //            take).ConfigureAwait(false);

        //        if (cidades == null) return NoContent();
        //        if (!cidades.Any()) return NoContent();

        //        var totalPages = (int)Math.Ceiling((double)totalCount / take);
        //        var response = new
        //        {
        //            items = _mapper.Map<IEnumerable<CidadeModel>>(cidades),
        //            sort,
        //            skip,
        //            take,
        //            totalCount,
        //            totalPages
        //        };

        //        return Ok(response);

        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError($"CidadeGetApp: {cidadeModel}, {e}, Usuario: ");
        //        return NotFound();
        //    }
        //}

        //POST api/cidades
        [HttpPost]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Post([FromBody] CidadeModel cidadeModel)
        {
            try
            {
                if (cidadeModel == null) return BadRequest("Necessário informar dados da cidade");
                cidadeModel.ValidarAdicionar();
                if (cidadeModel.Invalid)
                {
                    Debug.WriteLine(cidadeModel);
                    return BadRequest(cidadeModel);
                }

                var cidade = _mapper.Map<Cidade>(cidadeModel);

                _cidadeApplication.Add(cidade);

                cidadeModel = _mapper.Map<CidadeModel>(cidade);

                return Created($"api/cidades/{cidadeModel.Ibge}", cidadeModel);
            }
            catch (Exception e)
            {
                // TODO Refatorar as exceptions em local mais adequado: possivelmente dentro do presentation model
                _logger.LogError($"CidadesPost: {cidadeModel}");
                if (e.Message.Contains("PK_Cidade")) cidadeModel?.AddNotification("Cidade.Id", "Erro ao salvar. Tente novamente");
                if (e.Message.Contains("UX_CidadeIbge")) cidadeModel?.AddNotification("Cidade.Ibge", "Ibge já cadastrado");
                if (e.Message.Contains("UX_CidadeNomeUf")) cidadeModel?.AddNotification("Cidade.Nome", "Cidade já cadastrada para essa UF");

                return BadRequest(cidadeModel);
            }
        }

        //PUT api/cidades/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Put(string id, [FromBody] CidadeModel cidadeModel)
        {
            try
            {
                if (!Guid.TryParse(id, out var newGuid)) return BadRequest($"Id ({id}) inválido");
                if (cidadeModel == null) return BadRequest("Informe o registro a ser alterado");

                cidadeModel.ValidarEditar();
                if (cidadeModel.Invalid)
                {
                    Debug.WriteLine(cidadeModel);
                    return BadRequest(cidadeModel);
                }

                var cidadeExists = _cidadeApplication.GetById(id);
                if (cidadeExists == null)
                {
                    cidadeModel.AddNotification("cidadeModel.Id", "Registro não encontrado");
                    return NoContent();
                }

                _cidadeApplication.Update(id, _mapper.Map<Cidade>(cidadeModel), "");

                cidadeModel = _mapper.Map<CidadeModel>(_cidadeApplication.GetById(id));
                return Ok(cidadeModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"CidadesPost: {cidadeModel}");
                if (e.Message.Contains("PK_Cidade")) cidadeModel?.AddNotification("Cidade.Id", "Erro ao salvar. Tente novamente");
                if (e.Message.Contains("UX_CidadeIbge")) cidadeModel?.AddNotification("Cidade.Ibge", "Ibge já cadastrado");
                if (e.Message.Contains("UX_CidadeNomeUf")) cidadeModel?.AddNotification("Cidade.Nome", "Cidade já cadastrada para essa UF");
                return BadRequest(cidadeModel);
            }
        }
    }
}
