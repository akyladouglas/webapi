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
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class AgendamentoController : Controller
    {
        private readonly IAgendamentoApplication _agendamentoApplication;
        private readonly IAgendamentoParticipanteApplication _agendamentoParticipanteApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IUser _user;
        private static IHostingEnvironment _environment;

        public AgendamentoController(IAgendamentoApplication agendamentoApplication,
                                    IAgendamentoParticipanteApplication agendamentoParticipanteApplication,
                                    IMapper mapper,
                                    ILogger<AgendamentoController> logger,
                                    IUser user,
                                    IHostingEnvironment environment)
        {
            _agendamentoApplication = agendamentoApplication;
            _agendamentoParticipanteApplication = agendamentoParticipanteApplication;
            _mapper = mapper;
            _logger = logger;
            _user = user;
            _environment = environment;
        }

        /// <summary>
        /// Retorna uma lista de Agendamentos
        /// </summary>
        /// <param name="agendamentoModel"></param>
        /// <param name="param"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="usuarioLogado"></param>
        /// <returns>Retorna uma lista de Procedimentos</returns>
        /// <response code="200">Procedimento(s) ecnontrada(s)</response>
        /// <response code="204">Nenhum Procedimento encontrado</response>
        /// <response code="404">Erro ao procurar Procedimento</response>
        [HttpGet]
        [Authorize(Roles = "Retaguarda, App, MedicoEspecialista, Recepcionista, Triagem, Individuo")]
        public async Task<IActionResult> Get([FromQuery] AgendamentoModel agendamentoModel, string param, string sort = "a.Dia DESC, a.Hora DESC", int? skip = null, int? take = null)
        {
            //
            if (agendamentoModel == null) return BadRequest();

            if (User.IsInRole("Individuo")) agendamentoModel.IndividuoId = _user.GetUserId();
            if (agendamentoModel.SinaisVitaisGrafico != true)
            {
                //if (User.IsInRole("Médico")) agendamentoModel.ProfissionalId = _user.GetUserId();
            }
            try
            {
                var (agendamentos, totalCount) = await _agendamentoApplication.GetByParam(
                    _mapper.Map<Agendamento>(agendamentoModel),
                    sort,
                    skip,
                    take).ConfigureAwait(false);

                var totalPages = 0;

                if (take.HasValue && take.Value != 0)
                {
                    totalPages = (int)Math.Ceiling((decimal)totalCount / take.Value);
                }

                if (totalCount == 0) return NoContent();

                var response = new
                {
                    items = _mapper.Map<IEnumerable<AgendamentoModel>>(agendamentos),
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
                _logger.LogError($"AgendamentoGet: {agendamentoModel}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }


        /// <summary>
        /// Retorna uma lista de Agendamentos
        /// </summary>
        /// <param name="agendamentoModel"></param>
        /// <param name="param"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="usuarioLogado"></param>
        /// <returns>Retorna uma lista de Procedimentos</returns>
        /// <response code="200">Procedimento(s) ecnontrada(s)</response>
        /// <response code="204">Nenhum Procedimento encontrado</response>
        /// <response code="404">Erro ao procurar Procedimento</response>
        [HttpGet]
        [Route("Ausentes")]
        [Authorize(Roles = "Retaguarda, App, MedicoEspecialista, Recepcionista, Triagem, Medico, Individuo")]
        public async Task<IActionResult> GetAusentes([FromQuery] AgendamentoModel agendamentoModel, string param, string sort = "a.Dia DESC, a.Hora DESC",
          int skip = 1, int take = 10)
        {

            if (agendamentoModel == null) return BadRequest();

            try
            {
                var (agendamentos, totalCount) = await _agendamentoApplication.GetAusentes(
                    _mapper.Map<Agendamento>(agendamentoModel),
                    sort,
                    skip,
                    take).ConfigureAwait(false);

                var totalPages = (int)Math.Ceiling((double)totalCount / take);

                if (totalCount == 0) return NoContent();

                var response = new
                {
                    items = _mapper.Map<IEnumerable<AgendamentoModel>>(agendamentos),
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
                _logger.LogError($"AgendamentoGet: {agendamentoModel}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }




        /// <summary>
        /// Retorna um Agendamento específico.
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id}")]
        [Authorize(Roles = "Retaguarda, App, Médico, Recepcionista, Triagem, Individuo")]
        public IActionResult Get(string id)
        {
            try
            {
                var agendamentoModel = _mapper.Map<AgendamentoModel>(_agendamentoApplication.GetById(id));

                if (agendamentoModel == null) return NotFound();

                return Ok(agendamentoModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"AgendamentoGetById: {DateTime.Now}, {id}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        //POST api/agendamento
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
                var agendamentoId = _agendamentoApplication.Add(agendamento);
                if (agendamento.RetornoAgendamentoId != null)
                {
                    var exists = _agendamentoApplication.GetById(agendamento.RetornoAgendamentoId);

                    if (exists != null)
                    {

                        exists.VinculoRetorno = true;
                        _agendamentoApplication.Update(exists.Id, exists);
                    }
                }

                agendamentoModel = _mapper.Map<AgendamentoModel>(agendamento);

                return Created(agendamentoId, agendamentoId);
            }
            catch (Exception e)
            {
                _logger.LogError($"AgendamentoPost: {agendamentoModel}");
                return BadRequest(e.Message.ToString());
            }
        }

        //PUT api/agendamento/{id}
        [HttpPut("{id}")]
        [AllowAnonymous]
        [Authorize(Roles = "Retaguarda, Médico, Recepcionista, Triagem, MédicoEspecialista, Individuo")]
        public IActionResult Put(string id, [FromBody] AgendamentoModel agendamentoModel)
        {
            try
            {
                if (agendamentoModel == null) return BadRequest("Informe o registro a ser alterado");

                if (agendamentoModel.Invalid)
                {
                    return BadRequest(agendamentoModel);
                }

                var agendamentoExists = _agendamentoApplication.GetById(id);
                if (agendamentoExists == null)
                {
                    agendamentoModel.AddNotification("Agendamento.Id", "Registro não encontrado");
                    return NotFound();
                }

                if (!string.IsNullOrEmpty(agendamentoModel.GraficoEcg))
                {
                    try
                    {
                        if (agendamentoModel.GraficoEcg.Length > 50)
                        {
                            byte[] data = System.Convert.FromBase64String(agendamentoModel.GraficoEcg);
                            SaveToFile(data, $"\\GraficosEcg\\GraficoEcg_{agendamentoModel.IndividuoId}_{agendamentoModel.Id}");
                            agendamentoModel.GraficoEcg = $"GraficosEcg/GraficoEcg_{agendamentoModel.IndividuoId}_{agendamentoModel.Id}";
                        }
                    }
                    catch (Exception e)
                    {
                        var erro = (e.InnerException != null) ? e.InnerException.Message : e.Message;
                        return BadRequest(erro);
                    }
                }

                _agendamentoApplication.Update(id, _mapper.Map<Agendamento>(agendamentoModel));

                return new NoContentResult();
            }
            catch (Exception e)
            {
                _logger.LogError($"AgendamentoPut: {agendamentoModel}");
                return BadRequest(e.Message.ToString());
            }
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "Retaguarda, Médico, Recepcionista")]
        public IActionResult Excluir(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest($"Dados incompletos");
            try
            {
                _agendamentoApplication.Delete(id);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"AgendamentoDelete: {id}");
                return BadRequest(e.Message.ToString());
            }
        }

        //PUT NOS SINAIS VITAIS AFERIDOS PELA TRIAGEM
        [HttpPut("{id}/PutFormSinaisVitais")]
        [Authorize(Roles = "Retaguarda, Médico, Recepcionista, Triagem, Individuo")]
        public IActionResult PutFormSinaisVitais(string id, [FromBody] AgendamentoModel agendamentoModel)
        {
            try
            {
                if (agendamentoModel == null) return BadRequest("Informe o registro a ser alterado");

                if (agendamentoModel.Invalid)
                {
                    return BadRequest(agendamentoModel);
                }

                var agendamentoExists = _agendamentoApplication.GetById(id);
                if (agendamentoExists == null)
                {
                    agendamentoModel.AddNotification("Agendamento.Id", "Registro não encontrado");
                    return NotFound();
                }

                _agendamentoApplication.UpdateSinaisVitais(id, _mapper.Map<Agendamento>(agendamentoModel));

                return new NoContentResult();
            }
            catch (Exception e)
            {
                _logger.LogError($"AgendamentoPut: {agendamentoModel}");
                return BadRequest(e.Message.ToString());
            }
        }


        //PUT PARA CONFIRMAR O ATENDIMENTO FEITO PELA TRIAGEM PARA QUE ELE CHEGUE AO MEDICO
        [HttpPut("{id}/PutFormConfirmarPresenca")]
        [Authorize(Roles = "Retaguarda, Médico, Recepcionista, Triagem, Individuo")]
        public IActionResult PutFormConfirmarPresenca(string id, [FromBody] AgendamentoModel agendamentoModel)
        {
            try
            {
                if (agendamentoModel == null) return BadRequest("Informe o registro a ser alterado");

                if (agendamentoModel.Invalid)
                {
                    return BadRequest(agendamentoModel);
                }

                var agendamentoExists = _agendamentoApplication.GetById(id);
                if (agendamentoExists == null)
                {
                    agendamentoModel.AddNotification("Agendamento.Id", "Registro não encontrado");
                    return NotFound();
                }

                _agendamentoApplication.UpdateConfirmarPresenca(id, _mapper.Map<Agendamento>(agendamentoModel));

                return new NoContentResult();
            }
            catch (Exception e)
            {
                _logger.LogError($"AgendamentoPut: {agendamentoModel}");
                return BadRequest(e.Message.ToString());
            }
        }

        //PUT PARA FINALIZAR O ATENDIMENTO FEITO PELO MEDICO
        [HttpPut("{id}/PutFormFinalizarMedico")]
        [Authorize(Roles = "Retaguarda, Médico, Recepcionista, Triagem, Individuo")]
        public IActionResult PutFormFinalizarMedico(string id, [FromBody] AgendamentoModel agendamentoModel)
        {
            try
            {
                if (agendamentoModel == null) return BadRequest("Informe o registro a ser alterado");

                if (agendamentoModel.Invalid)
                {
                    return BadRequest(agendamentoModel);
                }

                var agendamentoExists = _agendamentoApplication.GetById(id);
                if (agendamentoExists == null)
                {
                    agendamentoModel.AddNotification("Agendamento.Id", "Registro não encontrado");
                    return NotFound();
                }

                _agendamentoApplication.UpdateFinalizarMedico(id, _mapper.Map<Agendamento>(agendamentoModel));

                return new NoContentResult();
            }
            catch (Exception e)
            {
                _logger.LogError($"AgendamentoPut: {agendamentoModel}");
                return BadRequest(e.Message.ToString());
            }
        }

        //PUT PARA TRUE NO BOOLEAN EM ATENDIMENTO MEDICO
        [HttpPut("{id}/PutEmAtendimentoMedico")]
        [Authorize(Roles = "Retaguarda, Médico, Recepcionista, Triagem, Individuo")]
        public IActionResult PutEmAtendimentoMedico(string id, [FromBody] AgendamentoModel agendamentoModel)
        {
            try
            {
                if (agendamentoModel == null) return BadRequest("Informe o registro a ser alterado");

                if (agendamentoModel.Invalid)
                {
                    return BadRequest(agendamentoModel);
                }

                var agendamentoExists = _agendamentoApplication.GetById(id);
                if (agendamentoExists == null)
                {
                    agendamentoModel.AddNotification("Agendamento.Id", "Registro não encontrado");
                    return NotFound();
                }

                _agendamentoApplication.UpdateEmAtendimentoMedico(id, _mapper.Map<Agendamento>(agendamentoModel));

                return new NoContentResult();
            }
            catch (Exception e)
            {
                _logger.LogError($"AgendamentoPut: {agendamentoModel}");
                return BadRequest(e.Message.ToString());
            }
        }



        //PUT PARA FINALIZAR O ATENDIMENTO FEITO PELA TRIAGEM QUANDO O GRAU DE RISCO FOR MAIOR QUE 3 PARA O PACIENTE SER ENCAMINHADO DIRETAMENTE PARA O MEDICO DA UNIDADE
        [HttpPut("{id}/PutFormFinalizarTriagem")]
        [Authorize(Roles = "Retaguarda, Médico, Recepcionista, Triagem, Individuo")]
        public IActionResult PutFormFinalizarTriagem(string id, [FromBody] AgendamentoModel agendamentoModel)
        {
            try
            {
                if (agendamentoModel == null) return BadRequest("Informe o registro a ser alterado");

                if (agendamentoModel.Invalid)
                {
                    return BadRequest(agendamentoModel);
                }

                var agendamentoExists = _agendamentoApplication.GetById(id);
                if (agendamentoExists == null)
                {
                    agendamentoModel.AddNotification("Agendamento.Id", "Registro não encontrado");
                    return NotFound();
                }

                _agendamentoApplication.UpdateFinalizarTriagem(id, _mapper.Map<Agendamento>(agendamentoModel));

                return new NoContentResult();
            }
            catch (Exception e)
            {
                _logger.LogError($"AgendamentoPut: {agendamentoModel}");
                return BadRequest(e.Message.ToString());
            }
        }


        [HttpPut("{id}/PutFormRecepcao")]
        [Authorize(Roles = "Retaguarda, Médico, Recepcionista, Individuo")]
        public IActionResult PutFormRecepcao(string id, [FromBody] AgendamentoModel agendamentoModel)
        {
            try
            {
                if (agendamentoModel == null) return BadRequest("Informe o registro a ser alterado");

                if (agendamentoModel.Invalid)
                {
                    return BadRequest(agendamentoModel);
                }

                var agendamentoExists = _agendamentoApplication.GetById(id);
                if (agendamentoExists == null)
                {
                    agendamentoModel.AddNotification("Agendamento.Id", "Registro não encontrado");
                    return NotFound();
                }

                _agendamentoApplication.UpdateRecepcao(id, _mapper.Map<Agendamento>(agendamentoModel));

                return new NoContentResult();
            }
            catch (Exception e)
            {
                _logger.LogError($"AgendamentoPut: {agendamentoModel}");
                return BadRequest(e.Message.ToString());
            }
        }

        [HttpPut("{id}/PutFormTriagem")]
        [Authorize(Roles = "Retaguarda, Médico, Recepcionista, Individuo")]
        public IActionResult PutFormTriagem(string id, [FromBody] AgendamentoModel agendamentoModel)
        {
            try
            {
                if (agendamentoModel == null) return BadRequest("Informe o registro a ser alterado");

                if (agendamentoModel.Invalid)
                {
                    return BadRequest(agendamentoModel);
                }

                var agendamentoExists = _agendamentoApplication.GetById(id);
                if (agendamentoExists == null)
                {
                    agendamentoModel.AddNotification("Agendamento.Id", "Registro não encontrado");
                    return NotFound();
                }

                _agendamentoApplication.UpdateTriagem(id, _mapper.Map<Agendamento>(agendamentoModel));

                return new NoContentResult();
            }
            catch (Exception e)
            {
                _logger.LogError($"AgendamentoPut: {agendamentoModel}");
                return BadRequest(e.Message.ToString());
            }
        }



        private void SaveToFile(byte[] file, string url)
        {

            if (file == null) return;
            var path = $"{_environment.WebRootPath}";
            var filename = path + url + ".png";
            try
            {
                using (var fs = new FileStream(filename, FileMode.Create))
                {
                    fs.Write(file);
                    fs.Close();
                }

            }
            catch (Exception e)
            {
                // ignored
            }
        }

    }
}
