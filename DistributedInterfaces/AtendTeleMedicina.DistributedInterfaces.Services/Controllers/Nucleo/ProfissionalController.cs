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
using System.Net;
using AtendTeleMedicina.DistributedInterfaces.Services.Helpers;
using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Security;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class ProfissionalController : Controller
    {
        private readonly IProfissionalApplication _profissionalApplication;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IUser _user;
        private readonly IConfiguration _config;
        private static IHostingEnvironment _environment;

        public ProfissionalController(IProfissionalApplication profissionalApplication,
            IMapper mapper,
            ILoggerFactory logger,
            IUser user,
            IConfiguration config,
            IHostingEnvironment environment
            )
        {
            _profissionalApplication = profissionalApplication;
            _mapper = mapper;
            _logger = logger.CreateLogger<ProfissionalController>();
            _user = user;
            _config = config;
            _environment = environment;
        }

        /// <summary>
        /// Retorna uma lista de Profissionals
        /// </summary>
        /// <param name="profissionalModel"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns>Retorna uma lista de Profissional(is)</returns>
        /// <response code="200">Profissional(is) encontrada(s)</response>
        /// <response code="204">Nenhum Profissional encontrado</response>
        /// <response code="404">Erro ao procurar Profissional</response>
        [HttpGet]
        [Authorize(Roles = "Retaguarda, Individuo")]
        public async Task<IActionResult> Get([FromQuery]ProfissionalModel profissionalModel, string param, string sort = "Nome",
          int skip = 1, int take = 10)
        {

            if (param != null)
            {
                byte[] data = System.Convert.FromBase64String(param);
                var decodeString = System.Text.ASCIIEncoding.ASCII.GetString(data);
                profissionalModel = JsonConvert.DeserializeObject<ProfissionalModel>(decodeString);
            }
            profissionalModel?.ValidarGet();
            try
            {
                var (profissionals, totalCount) = await _profissionalApplication.GetByParam(
                    _mapper.Map<Profissional>(profissionalModel),
                    sort,
                    skip,
                    take).ConfigureAwait(false);

                var totalPages = (int)Math.Ceiling((double)totalCount / take);
                var response = new
                {
                    items = _mapper.Map<IEnumerable<ProfissionalModel>>(profissionals),
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
                _logger.LogError($"ProfissionalList: {profissionalModel}, {e}, Usuario: ");
                return BadRequest(e.Message.ToString());
            }
        }

        /// <summary>
        /// Retorna um Profissional específico.
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id}")]
        [Authorize(Roles = "Retaguarda, Individuo, Médico")]
        public IActionResult Get(string id)
        {
            try
            {
                var profissional = _mapper.Map<ProfissionalModel>(_profissionalApplication.GetById(id));

                if (profissional == null) return NotFound();

                if(profissional.Id != _user.GetUserId())
                {
                    profissional.Senha = "***";
                }

                return Ok(profissional);
            }
            catch (Exception e)
            {
                _logger.LogError($"EstabelecimentoId: {DateTime.Now}, {id}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

       //POST api/profissional
        [HttpPost]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Post([FromBody] ProfissionalModel profissionalModel)
        {

            try
            {
                profissionalModel.DataCadastro = DateTime.Now;
                if (profissionalModel == null) return BadRequest("Necessário informar dados do Profissional");
                profissionalModel.ValidarAdicionar();
                if (profissionalModel.Invalid)
                {
                    return BadRequest(profissionalModel);
                }

                var result = _profissionalApplication.Add(_mapper.Map<Profissional>(profissionalModel));
                var profissional = _profissionalApplication.GetByCpf(profissionalModel.Cpf);
                if (profissional == null) return BadRequest("Erro ao cadastrar profissional");
                profissional.Senha = "****";

                return Ok(profissional);
            }
            catch (Exception e)
            {
                _logger.LogError($"ProfissionalPost: {profissionalModel}");
                return BadRequest(e.Message.ToString());
            }
        }

        [HttpPut]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult UpdateAtivarDesativar([FromBody] ProfissionalModel profissionalModel)
        {
            if (string.IsNullOrEmpty(profissionalModel.Id)) return BadRequest($"Dados incompletos");
            if (!profissionalModel.Ativo.HasValue) return BadRequest($"Dados incompletos");

            try
            {
                _profissionalApplication.UpdateAtivarDesativar(_mapper.Map<Profissional>(profissionalModel));
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"EstabelecimentoDelete: {profissionalModel.Id}");
                return BadRequest(e.Message.ToString());
            }
        }

        //PUT api/profissional/{id}
        [HttpPut("UpdateFoto/{id}")]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult UpdateFoto(string id, [FromBody] ProfissionalModel profissionalModel)
        {
            if (profissionalModel == null) return BadRequest("Informe o registro a ser alterado");

            // TODO Implementar update foto
            //if (!String.IsNullOrEmpty(profissionalModel.Imagem))
            //{
            //    try
            //    {
            //        byte[] data = System.Convert.FromBase64String(profissionalModel.Imagem);
            //        SaveToFile(data, $"\\Fotos\\Profissional_{profissionalModel.Id}");
            //        profissionalModel.Imagem = $"Fotos/Profissional_{profissionalModel.Id}";
            //    }
            //    catch (Exception e)
            //    {
            //        var erro = (e.InnerException != null) ? e.InnerException.Message : e.Message;
            //        return BadRequest(erro);
            //    }
            //}
            _profissionalApplication.UpdateFoto(id, _mapper.Map<Profissional>(profissionalModel));

            return Ok("Foto removida");
        }

        //PUT api/profissional/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Put(string id, [FromBody] ProfissionalModel profissionalModel)
        {
            try
            {
                if (profissionalModel == null) return BadRequest("Informe o registro a ser alterado");

                profissionalModel.ValidarEditar();
                if (profissionalModel.Invalid)
                {
                    return BadRequest(profissionalModel);
                }

                var profissionalExists = _profissionalApplication.GetById(id);
                if (profissionalExists == null)
                {
                    profissionalModel.AddNotification("profissionalModel.Id", "Registro não encontrado");
                    return NotFound();
                }

                //if(profissionalModel.Imagem == null)
                //{
                //    profissionalModel.Imagem = 

                //    profissionalModel.Imagem = "";
                //}

                if (!String.IsNullOrEmpty(profissionalModel.Imagem))
                {
                    try
                    {
                        profissionalModel.DataAlteracao = DateTime.Now;
                        byte[] data = System.Convert.FromBase64String(profissionalModel.Imagem);
                        SaveToFile(data, $"\\Fotos\\Profissional_{profissionalModel.Id}");
                        profissionalModel.Imagem = $"Fotos/Profissional_{profissionalModel.Id}";
                    }
                    catch (Exception e)
                    {
                        var erro = (e.InnerException != null) ? e.InnerException.Message : e.Message;
                        return BadRequest(erro);
                    }
                }

                _profissionalApplication.Update(id, _mapper.Map<Profissional>(profissionalModel));

                //profissionalModel = _mapper.Map<ProfissionalModel>(_profissionalApplication.GetById(id));
                return new NoContentResult();
            }
            catch (Exception e)
            {
                _logger.LogError($"ProfissionalPut: {profissionalModel}");
                return BadRequest(e.Message.ToString());
            }
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Excluir(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest($"Dados incompletos");
            try
            {
                _profissionalApplication.Delete(id);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"ProfissionalDelete: {id}");
                return BadRequest(e.Message.ToString());
            }
        }

        //PUT api/profissional/addtoken{id}
        [HttpPut("{id}/AddToken")]
        [Authorize(Roles = "Retaguarda, Individuo, Médico")]
        public IActionResult AddToken(string id, [FromQuery] string token)
        {
            try
            {
                if (id == null || token == null) return BadRequest("Erro ao inserir o token no profissional");

                //profissionalModel.ValidarEditar();
                //if (profissionalModel.Invalid)
                //{
                //    return BadRequest(profissionalModel);
                //}

                var profissionalExists = _profissionalApplication.GetById(id);
                if (profissionalExists == null)
                {
                    //profissionalModel.AddNotification("profissionalModel.Id", "Registro não encontrado");
                    return NotFound();
                }

                _profissionalApplication.AddToken(id, token);

                //profissionalModel = _mapper.Map<ProfissionalModel>(_profissionalApplication.GetById(id));
                return new NoContentResult();
            }
            catch (Exception e)
            {
                _logger.LogError($"ProfissionalPut:");
                return BadRequest(e.Message.ToString());
            }
        }

        [Authorize(Roles = "Retaguarda, Individuo, Médico")]
        public IActionResult UpdateToken(string id, string token)
        {
            try
            {
                if (id == null || token == null) return BadRequest("Erro ao Atualizar o token no profissional");

      

                var profissionalExists = _profissionalApplication.GetById(id);
                if (profissionalExists == null)
                {
                    //profissionalModel.AddNotification("profissionalModel.Id", "Registro não encontrado");
                    return NotFound();
                }

                _profissionalApplication.AddToken(id, token);

                //profissionalModel = _mapper.Map<ProfissionalModel>(_profissionalApplication.GetById(id));
                return new NoContentResult();
            }
            catch (Exception e)
            {
                _logger.LogError($"ProfissionalPut:");
                return BadRequest(e.Message.ToString());
            }
        }

        [HttpPut]
        [Route("AceiteTermosDoUso")]
        [Authorize(Roles = "Retaguarda, Individuo")]
        public IActionResult AceiteTermosDoUso([FromBody] ProfissionalModel model)
        {
            if (model == null) return BadRequest("Dados inválidos");

            //if (User.FindFirstValue("uid") != model.Id) return Unauthorized("Você só tem permissão de Editar seus dados");

            if (string.IsNullOrEmpty(model.Id)) return BadRequest("Dados inválidos");

            try
            {
                var profissional = _profissionalApplication.GetById(model.Id);
                if (profissional == null) return NotFound();

                int ret = _profissionalApplication.AceiteTermosDoUso(model.Id, model.Aceite);
                profissional.Aceite = model.Aceite;

                return Ok(profissional);
            }
            catch (Exception e)
            {

                _logger.LogError($"AceiteTermosDoUso: {model.Username}");
                return BadRequest($"Erro ao aceitar Termos do Uso: {e.InnerException.Message}");
            }

        }

        private void SaveToFile(byte[] imageBytes, string url)
        {

            if (imageBytes == null)
                return;
            var path = $"{_environment.WebRootPath}";
            var filename = path + url + ".jpg";

            try
            {
                var ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                var img = Image.FromStream(ms);
                img.Save(filename, ImageFormat.Jpeg);
            }
            catch (Exception e)
            {
                // ignored
            }
        }


    }
}
