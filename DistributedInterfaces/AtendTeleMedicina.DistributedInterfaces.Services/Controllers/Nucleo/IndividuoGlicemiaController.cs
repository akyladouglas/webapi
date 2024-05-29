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
using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using System.Linq;
using AtendTeleMedicina.Application.Services.Nucleo;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class IndividuoGlicemiaController : Controller
    {

        private readonly IIndividuoApplication _individuoApplication;
        private readonly IIndividuoGlicemiaApplication _individuoGlicemiaApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IUser _user;

        public IndividuoGlicemiaController(
                                    IIndividuoApplication individuoApplication,
                                    IIndividuoGlicemiaApplication individuoGlicemiaApplication,
                                    IMapper mapper,
                                    ILogger<IndividuoGlicemiaController> logger,
                                    IUser user)
        {
            _individuoApplication = individuoApplication;
            _individuoGlicemiaApplication = individuoGlicemiaApplication;
            _mapper = mapper;
            _logger = logger;
            _user = user;
        }

        [HttpPost]
        [Authorize(Roles = "Retaguarda, Médico, MédicoAD, Individuo")]
        public async Task<IActionResult> Post([FromBody] IndividuoGlicemiaModel individuoGlicemiaModel)
        {

            try
            {
                if (individuoGlicemiaModel == null) return BadRequest("Necessário informar dados da glicemia");

                if (individuoGlicemiaModel.Invalid)
                {
                    return BadRequest(individuoGlicemiaModel);
                }

                var individuoGlicemiaEntity = _mapper.Map<IndividuoGlicemia>(individuoGlicemiaModel);

                //VERIFICA SE EXISTE REGISTRO COM O FILTRO DE DATA CADASTRO PARA O DIA (OU SEJA SE TEM ALGUM REGISTRO DE REFEIÇÃO)
                var (individuoGlicemiaArray, totalCount) = await _individuoGlicemiaApplication.GetByParam(
                    _mapper.Map<IndividuoGlicemia>(individuoGlicemiaModel),
                    "ig.DataCadastro DESC",
                    1,
                    1).ConfigureAwait(false);


                // SE EXISTIR REGISTRO, VERIFICA OS DADOS E ATUALIZA O REGISTRO
                if (totalCount > 0) {
                    var individuoGlicemiaDB = individuoGlicemiaArray.FirstOrDefault();

                    // VERIFICA SE JA ESTA RESPONDIDO ALGUM TIPO DE REFEIÇÃO, E SE ESTA TENTANDO RESPONDER NOVAMENTE
                    if(individuoGlicemiaDB.RespondeuCafe == true && (individuoGlicemiaModel.CafeDepois != null && individuoGlicemiaModel.CafeAntes != null)) return BadRequest("Já existe um registro de glicemia para o café da manhã");
                    if(individuoGlicemiaDB.RespondeuAlmoco == true && (individuoGlicemiaModel.AlmocoDepois != null && individuoGlicemiaModel.AlmocoAntes != null)) return BadRequest("Já existe um registro de glicemia para o almoço");
                    if(individuoGlicemiaDB.RespondeuJanta == true && (individuoGlicemiaModel.JantaDepois != null && individuoGlicemiaModel.JantaAntes != null)) return BadRequest("Já existe um registro de glicemia para a janta");
                    if(individuoGlicemiaDB.RespondeuDormirMadrugada == true && individuoGlicemiaModel.AntesDormirMadrugada != null) return BadRequest("Já existe um registro de glicemia para dormir / madrugada");

                    //MONTANDO O OBJETO PARA ATUALIZAR COM OS DADOS DO MODEL OU DB APÓS A VALIDAÇÃO
                    //CASO TENHA DADOS DO DB, MANTÉM O DO FORM, CASO NÃO TENHA, ATUALIZA COM OS DADOS DO FORM
                    IndividuoGlicemia individuoGlicemia = new IndividuoGlicemia
                    {
                        Id = individuoGlicemiaDB.Id,
                        IndividuoId = individuoGlicemiaDB.IndividuoId,
                        DataCadastro = individuoGlicemiaDB.DataCadastro,
                        RespondeuCafe = individuoGlicemiaDB.RespondeuCafe ?? individuoGlicemiaModel.RespondeuCafe,
                        CafeAntes = individuoGlicemiaDB.CafeAntes ?? individuoGlicemiaModel.CafeAntes,
                        CafeDepois = individuoGlicemiaDB.CafeDepois ?? individuoGlicemiaModel.CafeDepois,
                        RespondeuAlmoco = individuoGlicemiaDB.RespondeuAlmoco ?? individuoGlicemiaModel.RespondeuAlmoco,
                        AlmocoAntes = individuoGlicemiaDB.AlmocoAntes ?? individuoGlicemiaModel.AlmocoAntes,
                        AlmocoDepois = individuoGlicemiaDB.AlmocoDepois ?? individuoGlicemiaModel.AlmocoDepois,
                        RespondeuJanta = individuoGlicemiaDB.RespondeuJanta ?? individuoGlicemiaModel.RespondeuJanta,
                        JantaAntes = individuoGlicemiaDB.JantaAntes ?? individuoGlicemiaModel.JantaAntes,
                        JantaDepois = individuoGlicemiaDB.JantaDepois ?? individuoGlicemiaModel.JantaDepois,
                        RespondeuDormirMadrugada = individuoGlicemiaDB.RespondeuDormirMadrugada ?? individuoGlicemiaModel.RespondeuDormirMadrugada,
                        AntesDormirMadrugada = individuoGlicemiaDB.AntesDormirMadrugada ?? individuoGlicemiaModel.AntesDormirMadrugada,
                        Observacoes = individuoGlicemiaDB.Observacoes == individuoGlicemiaModel.Observacoes ? individuoGlicemiaDB.Observacoes : individuoGlicemiaModel.Observacoes
                    };


                    var individuoGlicemiaId = _individuoGlicemiaApplication.Update(individuoGlicemia.Id, individuoGlicemia);
                    return Ok(individuoGlicemiaId);
                }
                else {
                    //SE NÃO EXISTIR REGISTRO, CRIA UM NOVO
                    //if(individuoGlicemiaModel.CafeAntes == null || individuoGlicemiaModel == null) return BadRequest("Necessário informar dados da glicemia");
                   
                    var individuoGlicemiaEntitie = _mapper.Map<IndividuoGlicemia>(individuoGlicemiaModel);
                    var individuoGlicemiaId = _individuoGlicemiaApplication.Add(individuoGlicemiaEntitie);
                    return Created(individuoGlicemiaId, individuoGlicemiaId);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"individuoGlicemiaPost: {individuoGlicemiaModel}");
                return BadRequest(e.Message.ToString());
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Retaguarda, Médico, MédicoAD, Individuo")]
        public IActionResult Put(string id, [FromBody] IndividuoGlicemiaModel individuoGlicemiaModel)
        {
            try
            {
                if (individuoGlicemiaModel == null) return BadRequest("Informe o registro a ser alterado");

                if (individuoGlicemiaModel.Invalid)
                {
                    return BadRequest(individuoGlicemiaModel);
                }

                var individuoGlicemiaExists = _individuoGlicemiaApplication.GetById(id);
                if (individuoGlicemiaExists == null)
                {
                    return NotFound("Registro não encontrado");
                }

                var individuoGlicemiaId = _individuoGlicemiaApplication.Update(id, _mapper.Map<IndividuoGlicemia>(individuoGlicemiaModel));

                return Ok(individuoGlicemiaId);
            }
            catch (Exception e)
            {
                _logger.LogError($"individuoGlicemiaPut: {individuoGlicemiaModel}");
                return BadRequest(e.Message.ToString());
            }
        }


        [HttpGet]
        [Authorize(Roles = "Retaguarda, Médico, MédicoAD, Individuo")]
        public async Task<IActionResult> Get([FromQuery] IndividuoGlicemiaModel individuoGlicemiaModel, string param, string sort = "ig.DataCadastro DESC",
          int skip = 1, int take = 10)
        {

            if (individuoGlicemiaModel == null) return BadRequest();
            try
            {
                var (individuoGlicemias, totalCount) = await _individuoGlicemiaApplication.GetByParam(
                    _mapper.Map<IndividuoGlicemia>(individuoGlicemiaModel),
                    sort,
                    skip,
                    take).ConfigureAwait(false);

                var totalPages = (int)Math.Ceiling((double)totalCount / take);

                if (totalCount == 0) return NoContent();

                var response = new
                {
                    items = _mapper.Map<IEnumerable<IndividuoGlicemiaModel>>(individuoGlicemias),
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
                _logger.LogError($"IndividuoGlicemiaGet: {individuoGlicemiaModel}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }


        [HttpGet("{id}")]
        [Authorize(Roles = "Retaguarda, Médico, MédicoAD, Individuo")]
        public IActionResult Get(string id)
        {
            try
            {
                var individuoGlicemiaModel = _mapper.Map<IndividuoGlicemiaModel>(_individuoGlicemiaApplication.GetById(id));

                if (individuoGlicemiaModel == null) return NotFound();

                return Ok(individuoGlicemiaModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"IndividuoGlicemiaGetById: {DateTime.Now}, {id}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }
    }
}
