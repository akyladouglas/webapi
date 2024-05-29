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
using Microsoft.AspNetCore.Http;
using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using AtendTeleMedicina.Domain.Entities.Dashboard;
using AtendTeleMedicina.Application.Services.Nucleo;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class AtendimentoController : Controller
    {
        private readonly IAtendimentoApplication _atendimentoApplication;
        private readonly IIndividuoCiapApplication _individuoCiapApplication;
        private readonly IIndividuoCid10Application _individuoCid10Application;
        private readonly IIndividuoProcedimentoApplication _individuoProcedimentoApplication;
        private readonly IAgendamentoApplication _agendamentoApplication;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private readonly IUser _user;
        private readonly IMapper _mapper;
        private static IHostingEnvironment _environment;

        public AtendimentoController(IAtendimentoApplication atendimentoApplication, IIndividuoCiapApplication individuoCiapApplication, IIndividuoCid10Application individuoCid10Application, IIndividuoProcedimentoApplication individuoProcedimentoApplication, IAgendamentoApplication agendamentoApplication, IUser user, IMapper mapper, ILoggerFactory logger)
        {
            _atendimentoApplication = atendimentoApplication;
            _individuoCiapApplication = individuoCiapApplication;
            _individuoCid10Application = individuoCid10Application;
            _individuoProcedimentoApplication = individuoProcedimentoApplication;
            _agendamentoApplication = agendamentoApplication;
            _user = user;
            _mapper = mapper;
            _logger = logger.CreateLogger<AtendimentoController>();
        }

        /// <summary>
        /// Retorna uma lista de Atendimento
        /// </summary>
        /// <param name="atendimentoModel"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns>Retorna uma lista de Atendimento(s)</returns>
        /// <response code="200">Atendimento(s) encontrada(s)</response>
        /// <response code="204">Nenhum Atendimento encontrado</response>
        /// <response code="404">Erro ao procurar Atendimento</response>
        [HttpGet]
        [Authorize(Roles = "Retaguarda, Individuo, Médico, MédicoEspecialista")]
        public async Task<IActionResult> Get([FromQuery] AtendimentoModel atendimentoModel, string param, string sort = "DataCadastro",
          int? skip = null, int? take = null)
        {           
            try
            {
                var (atendimentos, totalCount) = await _atendimentoApplication.GetByParam(
                    _mapper.Map<Atendimento>(atendimentoModel),
                    sort,
                    skip,
                    take).ConfigureAwait(false);

                var totalPages = 0;

                if (take.HasValue && take.Value != 0)
                {
                    totalPages = (int)Math.Ceiling((decimal)totalCount / take.Value);
                }

                var response = new
                {
                    items = _mapper.Map<IEnumerable<AtendimentoModel>>(atendimentos),
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
                _logger.LogError($"AtendimentoList: {atendimentoModel}, {e}, Usuario: ");
                return BadRequest(e.Message.ToString());
            }
        }


        /// <summary>
        /// Retorna uma lista de Historicode atendimento
        /// </summary>
        /// <param name="atendimentoModel"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns>Retorna uma lista de Atendimento(s)</returns>
        /// <response code="200">Atendimento(s) encontrada(s)</response>
        /// <response code="204">Nenhum Atendimento encontrado</response>
        /// <response code="404">Erro ao procurar Atendimento</response>
        [HttpGet]
        [Route("GetHistorico")]
        [Authorize(Roles = "Retaguarda, Individuo, Médico, MédicoEspecialista")]
        public async Task<IActionResult> GetHistorico([FromQuery] AtendimentoModel atendimentoModel, string param, string sort = "DataCadastro",
          int skip = 1, int take = 10)
        {

            if (param != null)
            {
                byte[] data = System.Convert.FromBase64String(param);
                var decodeString = System.Text.ASCIIEncoding.ASCII.GetString(data);
                atendimentoModel = JsonConvert.DeserializeObject<AtendimentoModel>(decodeString);
            }
            //atendimentoModel?.ValidarGet();
            atendimentoModel?.ValidarAdicionar();
            try
            {
                var (atendimentos, totalCount) = await _atendimentoApplication.GetByParam(
                    _mapper.Map<Atendimento>(atendimentoModel),
                    sort,
                    skip,
                    take).ConfigureAwait(false);

                var totalPages = (int)Math.Ceiling((double)totalCount / take);
                var response = new
                {
                    items = _mapper.Map<IEnumerable<AtendimentoModel>>(atendimentos),
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
                _logger.LogError($"AtendimentoList: {atendimentoModel}, {e}, Usuario: ");
                return BadRequest(e.Message.ToString());
            }
        }

        /// <summary>
        /// Retorna um Atendimento específico.
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id}")]
        [Authorize(Roles = "Retaguarda, Individuo, Médico, MédicoEspecialista")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var atendimento = await _atendimentoApplication.GetById(id).ConfigureAwait(true);
                var atendimentoModel = _mapper.Map<AtendimentoModel>(atendimento);

                if (atendimentoModel == null) return NotFound();

                return Ok(atendimentoModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"AtendimentoId: {DateTime.Now}, {id}, {e}");
                return BadRequest(e.Message);
            }
        }


        

        /// <summary>
        /// Retorna o ultimo Atendimento específico do Individuo.
        /// </summary>
        /// <param name="id"></param>
        [HttpGet]
        [Route("GetMaxAtendimentoByIndividuoId/{id}")]
        [Authorize(Roles = "Retaguarda, Individuo, Médico, MédicoEspecialista")]
        public IActionResult GetMaxAtendimentoByIndividuoId(string id)
        {
            try
            {
                var atendimentoModel = _mapper.Map<AtendimentoModel>(_atendimentoApplication.GetMaxAtendimentoByIndividuoId(id));

                if (atendimentoModel == null) return NotFound();

                return Ok(atendimentoModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"AtendimentoId: {DateTime.Now}, {id}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }


        // <summary>
        /// Retorna um Atendimento específico.
        /// </summary>
        /// <param name="id"></param>
        [HttpGet]
        [Route("GetAgendamentoById/{id}")]
        [Authorize(Roles = "Retaguarda, Individuo, Médico, MédicoEspecialista")]
        public IActionResult GetAgendamentoById(string id)
        {
            try
            {
                var atendimentoModel = _mapper.Map<AtendimentoModel>(_atendimentoApplication.GetAgendamentoById(id));

                if (atendimentoModel == null) return NoContent();

                return Ok(atendimentoModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"AtendimentoId: {DateTime.Now}, {id}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }


        // USADO PELA TRIAGEM
        [HttpPost]
        [Authorize(Roles = "Retaguarda, Médico, Recepcionista")]
        public IActionResult Post([FromBody] AtendimentoModel atendimentoModel)
        {
            try
            {
                if (atendimentoModel == null) return BadRequest("Necessário informar dados do Atendimento");
                atendimentoModel.ValidarAdicionar();
                if (atendimentoModel.Invalid)
                {
                    return BadRequest(atendimentoModel);
                }

                atendimentoModel.UsuarioId = _user.GetUserId();

                var atendimento = _mapper.Map<Atendimento>(atendimentoModel);

                if (atendimento.IndividuoCiap.Count > 0)
                {
                    try
                    {
                        _individuoCiapApplication.AddTriagem(atendimento.IndividuoCiap);

                    }
                    catch (Exception e)
                    {

                        return BadRequest(e);
                    }
                }

                if (atendimento.IndividuoCid10.Count > 0) {
                    try {

                        _individuoCid10Application.AddTriagem(atendimento.IndividuoCid10);

                    }
                    catch (Exception e)
                    {

                        return BadRequest(e);

                    }
                    
                }

                 _atendimentoApplication.Add(atendimento);

                atendimentoModel = _mapper.Map<AtendimentoModel>(atendimento);

                return Created($"api/atendimento/{atendimentoModel.Id}", atendimentoModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"AtendimentoPost: {atendimentoModel}");
                return BadRequest(e.Message.ToString());
            }
        }

        //USADO PELO MEDICO
        [HttpPost]
        [Route("PostMedico")]
        [Authorize(Roles = "Retaguarda, Médico, MédicoEspecialista")]
        public IActionResult PostMedico([FromBody] AtendimentoModel atendimentoModel)
        {

            try
            {
                if (atendimentoModel == null) return BadRequest("Necessário informar dados do Atendimento");
                atendimentoModel.ValidarAdicionar();
                if (atendimentoModel.Invalid)
                {
                    return BadRequest(atendimentoModel);
                }

                var atendimento = _mapper.Map<Atendimento>(atendimentoModel);

                if (atendimento.IndividuoCiap.Count > 0)
                {
                    try
                    {
                        _individuoCiapApplication.AddTriagem(atendimento.IndividuoCiap);

                    }
                    catch (Exception e)
                    {

                        return BadRequest(e);
                    }

                }

                if (atendimento.IndividuoCid10.Count > 0)
                {
                    try
                    {

                        _individuoCid10Application.AddTriagem(atendimento.IndividuoCid10);

                    }
                    catch (Exception e)
                    {

                        return BadRequest(e);

                    }

                }

                if (atendimento.IndividuoProcedimentos.Count > 0)
                {
                    try
                    {

                        _individuoProcedimentoApplication.AddTriagem(atendimento.IndividuoProcedimentos);

                    }
                    catch (Exception e)
                    {

                        return BadRequest(e);

                    }

                }

                _atendimentoApplication.AddMedico(atendimento);





                atendimentoModel = _mapper.Map<AtendimentoModel>(atendimento);

                return Created($"api/atendimento/{atendimentoModel.Id}", atendimentoModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"AtendimentoPost: {atendimentoModel}");
                return BadRequest(e.Message.ToString());
            }
        }


        //PUT api/atendimento/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Retaguarda, Médico, MédicoEspecialista")]
        public IActionResult Put(string id, [FromBody] AtendimentoModel atendimentoModel)
        {
            try
            {
                if (atendimentoModel == null) return BadRequest("Informe o registro a ser alterado");

                //atendimentoModel.ValidarEditar();
                if (atendimentoModel.Invalid)
                {
                    return BadRequest(atendimentoModel);
                }

                var atendimentoExists = _atendimentoApplication.GetById(id);
                if (atendimentoExists == null)
                {
                    atendimentoModel.AddNotification("atendimentoModel.Id", "Registro não encontrado");
                    return NotFound();
                }

                _atendimentoApplication.Update(id, _mapper.Map<Atendimento>(atendimentoModel));

                //profissionalModel = _mapper.Map<ProfissionalModel>(_profissionalApplication.GetById(id));
                return new NoContentResult();
            }
            catch (Exception e)
            {
                _logger.LogError($"AtendimentoPut: {atendimentoModel}");
                return BadRequest(e.Message.ToString());
            }
        }

        //PUT api/atendimento/{id}
        [HttpPut]
        [Route("PutInativar/{id}")]
        [Authorize(Roles = "Retaguarda, Médico, MédicoEspecialista")]
        public IActionResult PutInativar(string id, [FromBody] AtendimentoModel atendimentoModel)
        {
            try
            {
                if (atendimentoModel == null) return BadRequest("Informe o registro a ser alterado");

                //atendimentoModel.ValidarEditar();
                if (atendimentoModel.Invalid)
                {
                    return BadRequest(atendimentoModel);
                }

                var atendimentoExists = _atendimentoApplication.GetById(id);
                if (atendimentoExists == null)
                {
                    atendimentoModel.AddNotification("atendimentoModel.Id", "Registro não encontrado");
                    return NotFound();
                }

                _atendimentoApplication.UpdateInativar(id, _mapper.Map<Atendimento>(atendimentoModel));

                //profissionalModel = _mapper.Map<ProfissionalModel>(_profissionalApplication.GetById(id));
                return new NoContentResult();
            }
            catch (Exception e)
            {
                _logger.LogError($"AtendimentoPut: {atendimentoModel}");
                return BadRequest(e.Message.ToString());
            }
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "Retaguarda, Médico, MédicoEspecialista")]
        public IActionResult Excluir(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest($"Dados incompletos");
            try
            {
                _atendimentoApplication.Delete(id);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"AtendimentoDelete: {id}");
                return BadRequest(e.Message.ToString());
            }
        }

        [HttpGet]
        [Route("ContadorTotalTipoAtendimento")]
        [Authorize(Roles = "Retaguarda")]
        public async Task<IActionResult> GetTotalTipoAtendimento([FromQuery] DashboardParams dashParams)
        {

            try
            {
                var dashboard = _atendimentoApplication.GetTotalTipoAtendimento(dashParams);


                return Ok(dashboard);

            }
            catch (Exception e)
            {
                _logger.LogError($"ContadorAtividades: {dashParams}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }
    }
}
