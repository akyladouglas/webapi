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

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class AgendamentoParticipanteController : Controller
    {

        private readonly IAgendamentoParticipanteApplication _agendamentoParticipanteApplication;
        private readonly IAgendamentoApplication _agendamentoApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IUser _user;

        public AgendamentoParticipanteController(
                                    IAgendamentoParticipanteApplication agendamentoParticipanteApplication,
                                    IAgendamentoApplication agendamentoApplication,
                                    IMapper mapper,
                                    ILogger<AgendamentoController> logger,
                                    IUser user)
        {
            _agendamentoParticipanteApplication = agendamentoParticipanteApplication;
            _agendamentoApplication = agendamentoApplication;
            _mapper = mapper;
            _logger = logger;
            _user = user;
        }


        //POST api/Interconsulta
        [HttpPost]
        [Authorize(Roles = "Retaguarda, Médico, Recepcionista, Triagem, Individuo")]
        public IActionResult Post([FromBody] AgendamentoModel agendamentoModel)
        {

            try
            {
                if (agendamentoModel == null) return BadRequest("Necessário informar dados do Agendamento");

                if (agendamentoModel.Invalid)
                {
                    return BadRequest(agendamentoModel);
                }

                var agendamento = _mapper.Map<Agendamento>(agendamentoModel);
                var agendamentoId = _agendamentoParticipanteApplication.Post(agendamento);

                agendamentoModel = _mapper.Map<AgendamentoModel>(agendamento);

                return Created(agendamentoId, agendamentoId);
            }
            catch (Exception e)
            {
                _logger.LogError($"AgendamentoInterconsultaPost: {agendamentoModel}");
                return BadRequest(e.Message.ToString());
            }
        }

        //PUT api/Interconsulta/AceiteInterconsulta
        [HttpPut]
        [Route("AceiteInterconsulta")]
        [Authorize(Roles = "Retaguarda, Médico, Recepcionista, Triagem, MédicoEspecialista, Individuo")]
        public IActionResult PutAceiteInterconsulta([FromBody] AgendamentoParticipanteModel agendamentoParticipanteModel)
        {
            try
            {
                if (agendamentoParticipanteModel == null) return BadRequest("Informe o registro a ser alterado");

                var agendamentoParticipanteExists = _agendamentoParticipanteApplication.GetById(agendamentoParticipanteModel.Id);

                if (agendamentoParticipanteExists == null)
                {
                    return NotFound("Registro não encontrado");
                }

                _agendamentoParticipanteApplication.Update(agendamentoParticipanteModel.Id, _mapper.Map<AgendamentoParticipante>(agendamentoParticipanteModel));

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"AceiteInterconsultaPut: {agendamentoParticipanteModel}");
                return BadRequest(e.Message.ToString());
            }
        }

        //GET api/Interconsulta
        [HttpGet]
        [Authorize(Roles = "Retaguarda, App, MedicoEspecialista, Recepcionista, Triagem, Individuo")]
        public async Task<IActionResult> Get([FromQuery] AgendamentoParticipanteModel agendamentoParticipanteModel, string param, string sort = "a.Dia DESC, a.Hora DESC",
          int skip = 1, int take = 10)
        {

            try
            {
                var (agendamentoParticipantes, totalCount) = await _agendamentoParticipanteApplication.GetByParam(
                    _mapper.Map<AgendamentoParticipante>(agendamentoParticipanteModel),
                    sort,
                    skip,
                    take).ConfigureAwait(false);

                var totalPages = (int)Math.Ceiling((double)totalCount / take);

                if (totalCount == 0) return NoContent();

                var response = new
                {
                    items = _mapper.Map<IEnumerable<AgendamentoParticipanteModel>>(agendamentoParticipantes),
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
                _logger.LogError($"AgendamentoParticipanteGet: {agendamentoParticipanteModel}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        //PUT api/Interconsulta/AvaliacaoMedico/{id}
        [HttpPut("AvaliacaoMedico/{id}")]
        [Authorize(Roles = "Retaguarda, Médico, MédicoEspecialista, MédicoAD")]
        public IActionResult Put(string id, [FromBody] AgendamentoParticipanteModel agendamentoParticipanteModel)
        {
            try
            {
                if (agendamentoParticipanteModel == null) return BadRequest("Informe o registro a ser alterado");

                var agendamentoParticipanteExists = _agendamentoParticipanteApplication.GetById(agendamentoParticipanteModel.Id);
                if (agendamentoParticipanteExists == null)
                {
                    return NotFound("Registro não encontrado");
                }

                _agendamentoParticipanteApplication.Update(id, _mapper.Map<AgendamentoParticipante>(agendamentoParticipanteModel));

                return new NoContentResult();
            }
            catch (Exception e)
            {
                _logger.LogError($"AgendamentoParticipantePut: {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        //GET api/Interconsulta/{id}
        [HttpGet("{id}")]
        [Authorize
            (Roles = "Retaguarda, Médico, MédicoEspecialista, MédicoAD, Recepcionista, Triagem, Individuo")]
        public IActionResult GetById(string id) {
            try {
                if (string.IsNullOrEmpty(id)) return BadRequest("Informe o registro a ser consultado");
                var agendamentoParticipante = _agendamentoParticipanteApplication.GetById(id);
                if (agendamentoParticipante == null) return NotFound("Registro não encontrado");
                return Ok(_mapper.Map<AgendamentoParticipanteModel>(agendamentoParticipante));
            }
            catch(Exception e)
            {
                _logger.LogError($"AgendamentoParticipanteGetById: {id}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

    }
}
