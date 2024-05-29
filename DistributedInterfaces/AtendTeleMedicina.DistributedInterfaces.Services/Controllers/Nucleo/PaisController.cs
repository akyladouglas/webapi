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
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class PaisController : Controller
    {

        private readonly ILogger _logger;
        private readonly IPaisApplication _paisApplication;
        private readonly IMapper _mapper;
        private static IHostingEnvironment _environment;
        private readonly IConfiguration _config;


        public PaisController(
                                  IPaisApplication paisApplication,
                                  IMapper mapper, IConfiguration config,
                                  ILogger<PaisController> logger, IHostingEnvironment environment)
        {
            _mapper = mapper;
            _paisApplication = paisApplication;
            _logger = logger;
            _environment = environment;
            _config = config;
        }

        /// <summary>
        /// Retorna uma lista de Paises
        /// </summary>
        /// <param name="paisModel"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns>Retorna uma lista de Paises</returns>
        /// <response code="200">Estabelecimento(s) ecnontrada(s)</response>
        /// <response code="204">Nenhum Indivíduo encontrado</response>
        /// <response code="404">Erro ao procurar Indivíduo</response>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromQuery] PaisModel paisModel, string param, string sort = "p.Nome",
          int skip = 1, int take = 10000)
        {


            var filePath = _config["AppSettings:FilePath"];

            var file3 = filePath;

            if (param != null)
            {
                byte[] data = System.Convert.FromBase64String(param);
                var decodeString = System.Text.ASCIIEncoding.ASCII.GetString(data);
                paisModel = JsonConvert.DeserializeObject<PaisModel>(decodeString);
            }
            try
            {
                var (paises, totalCount) = await _paisApplication.GetByParam(
                    _mapper.Map<Pais>(paisModel),
                    sort,
                    skip,
                    take).ConfigureAwait(false);

                var totalPages = (int)Math.Ceiling((double)totalCount / take);
                var response = new
                {
                    items = _mapper.Map<IEnumerable<Pais>>(paises),
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
                _logger.LogError($"PaisGet: {paisModel}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }
    }
}
