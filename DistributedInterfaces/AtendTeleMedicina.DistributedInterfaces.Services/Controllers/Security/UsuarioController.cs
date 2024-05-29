using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AtendTeleMedicina.Application.Contracts.Security;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AtendTeleMedicina.Domain.Entities.Security;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Security
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class UsuarioController : Controller
    {
        private readonly IUserApplication _userApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public UsuarioController(IUserApplication UserApplication,
                                IMapper mapper,
                                ILogger<UsuarioController> logger)
        {
            _userApplication = UserApplication;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Retorna uma lista de User
        /// </summary>
        /// <param name="UserModel"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns>Retorna uma lista de Usuarios</returns>
        /// <response code="200">User(s) ecnontrada(s)</response>
        /// <response code="204">Nenhuma User Encontrada</response>
        /// <response code="404">Erro ao procurar User</response>
        [HttpGet]
        [Authorize(Roles = "Administrador, GestorEstado, AdmMunicipio")]
        public async Task<IActionResult> Get([FromQuery]UserModel userModel, string param, string filters, string sort = "Nome",
          int skip = 1, int take = 10)
        {
            if (!User.IsInRole("Administrador"))
            {
                userModel.Uf = User.FindFirstValue("uf");
                if (!User.IsInRole("GestorEstado"))
                    userModel.CidadeId = int.Parse(User.FindFirstValue("cidadeId"));
            }

            if (param != null)
            {
                byte[] data = System.Convert.FromBase64String(param);
                var decodeString = System.Text.ASCIIEncoding.ASCII.GetString(data);
                userModel = JsonConvert.DeserializeObject<UserModel>(decodeString);
            }
            userModel?.ValidarGet();
            try
            {
                var (usuarios, totalCount) = await _userApplication.GetByParam(
                    _mapper.Map<User>(userModel), sort, skip, take).ConfigureAwait(false);

                if (usuarios == null) return NoContent();
                if (!usuarios.Any()) return NoContent();

                var totalPages = (int)Math.Ceiling((double)totalCount / take);
                var response = new
                {
                    itens = _mapper.Map<IEnumerable<UserModel>>(usuarios),
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
                _logger.LogError($"UsuariosGet: {userModel}, {e}");
                return NotFound();
            }
        }


        /// <summary>
        /// Retorna uma lista de User
        /// </summary>
        /// <param name="UserModel"></param>
        /// <param name="sort"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns>Retorna uma lista de Usuarios</returns>
        /// <response code="200">User(s) ecnontrada(s)</response>
        /// <response code="204">Nenhuma User Encontrada</response>
        /// <response code="404">Erro ao procurar User</response>
        [HttpGet("GetSimplified")]
        [Authorize(Roles = "Retaguarda")]
        public async Task<IActionResult> GetSimplified([FromQuery] UserModel userModel, string param, string filters, string sort = "Nome",
          int skip = 1, int take = 10)
        {
            //if (!User.IsInRole("Administrador"))
            //{
            //    userModel.Uf = User.FindFirstValue("uf");
            //    if (!User.IsInRole("GestorEstado"))
            //        userModel.CidadeId = int.Parse(User.FindFirstValue("cidadeId"));
            //}

            if (param != null)
            {
                byte[] data = System.Convert.FromBase64String(param);
                var decodeString = System.Text.ASCIIEncoding.ASCII.GetString(data);
                userModel = JsonConvert.DeserializeObject<UserModel>(decodeString);
            }
            userModel?.ValidarGet();
            try
            {
                var (usuarios, totalCount) = await _userApplication.GetByParam(
                    _mapper.Map<User>(userModel), sort, skip, take).ConfigureAwait(false);

                if (usuarios == null) return NoContent();
                if (!usuarios.Any()) return NoContent();

                var itens = usuarios.Select(x => new
                {
                    Id = x.Id,
                    username = x.Username,
                    nome = x.Nome,
                    claims = x.UserClaims
                }).ToList();

                var totalPages = (int)Math.Ceiling((double)totalCount / take);
                var response = new
                {
                    itens = itens,
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
                _logger.LogError($"UsuariosGet: {userModel}, {e}");
                return NotFound();
            }
        }

        /// <summary>
        /// Pega uma User específica.
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id}", Name = "UsuariosId")]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Get(string id)
        {
            try
            {
                var UserModel = _mapper.Map<UserModel>(_userApplication.GetById(id));

                if (UserModel == null) return NoContent();

                return Ok(UserModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"UserId: {DateTime.Now}, {id}, {e}");
                Debug.WriteLine(e);
                return NotFound();
            }

        }

        //POST api/Usuarios
        [HttpPost]
        [Authorize(Roles = "Administrador, GestorEstado, AdmMunicipio")]
        public IActionResult Post([FromBody] UserModel userModel)
        {
            var userId = User.FindFirstValue("uid");

            try
            {
                if (userModel == null) return BadRequest("Necessário informar dados da User");
                userModel.ValidarAdicionar();
                if (userModel.Invalid)
                {
                    return BadRequest(userModel);
                }

                var User = _mapper.Map<User>(userModel);

                _userApplication.Add(User, userId);

                userModel = _mapper.Map<UserModel>(User);

                return Created($"api/Usuarios/{userModel.Id}", userModel);
            }
            catch (Exception e)
            {
                // TODO Refatorar as exceptions em local mais adequado: possivelmente dentro do presentation model
                _logger.LogError($"UsuariosPost: {userModel}");
                if (e.Message.Contains("PK_User")) userModel?.AddNotification("User.Id", "Erro ao salvar. Tente novamente");

                return BadRequest(e.Message.ToString());
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(string id)
        {
            var userId = User.FindFirstValue("uid");

            if (string.IsNullOrEmpty(id)) return BadRequest("Informe o registro a ser alterado");

            var userExists = _userApplication.GetById(id);
            if (userExists == null) return NotFound("Usuário não Encontrado");

            try
            {
                _userApplication.Delete(id, userId);
                return Ok("Usuário desativado com Sucesso");
            }
            catch (Exception e)
            {
                _logger.LogError($"UsuarioDelete: {id}");
                return BadRequest(e.Message.ToString());
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult Put(string id, [FromBody] UserModel userModel)
        {
            if (userModel == null) return BadRequest("Informe o registro a ser alterado");

            try
            {
                userModel.ValidarEditar();
                if (userModel.Invalid)
                {
                    Debug.WriteLine(userModel);
                    return BadRequest(userModel);
                }

                var userExists = _userApplication.GetById(id);
                if (userExists == null)
                {
                    userModel.AddNotification("UserModel.Id", "Registro não encontrado");
                    return NoContent();
                }

                _userApplication.Update(_mapper.Map<User>(userModel), id);

                userModel = _mapper.Map<UserModel>(_userApplication.GetById(id));
                return Ok(userModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"UsuariosPut: {userModel}");
                if (e.Message.Contains("PK_User")) userModel?.AddNotification("User.Id", "Erro ao salvar. Tente novamente");
                return BadRequest(userModel);
            }
        }

        [HttpPut]
        [Route("AtivarUsuario")]
        [Authorize(Roles = "Retaguarda")]
        public IActionResult AtivarUsuario([FromBody] UserModel userModel)
        {
            if (userModel == null) return BadRequest("Informe o registro a ser alterado");

            try
            {
                userModel.ValidarEditar();
                if (userModel.Invalid)
                {
                    Debug.WriteLine(userModel);
                    return BadRequest(userModel);
                }

                var userExists = _userApplication.GetById(userModel.Id);
                if (userExists == null)
                {
                    userModel.AddNotification("UserModel.Id", "Registro não encontrado");
                    return NoContent();
                }

                _userApplication.UpdateAtivo(_mapper.Map<User>(userModel));

                userModel = _mapper.Map<UserModel>(_userApplication.GetById(userModel.Id));
                return Ok(userModel);
            }
            catch (Exception e)
            {
                _logger.LogError($"UsuariosPut: {userModel}");
                if (e.Message.Contains("PK_User")) userModel?.AddNotification("User.Id", "Erro ao salvar. Tente novamente");
                return BadRequest(userModel);
            }
        }

        [HttpPut]
        [Route("AceiteTermosDoUso")]
        [Authorize(Roles = "Retaguarda, Individuo")]
        public IActionResult AceiteTermosDoUso([FromBody] UserModel model)
        {
            if (model == null) return BadRequest("Dados inválidos");

            if (User.FindFirstValue("uid") != model.Id) return Unauthorized("Você só tem permissão de Editar seus dados");

            if (string.IsNullOrEmpty(model.Id)) return BadRequest("Dados inválidos");

            try
            {
                var usuario = _userApplication.GetById(model.Id);
                if (usuario == null) return NotFound();

                int ret = _userApplication.AceiteTermosDoUso(model.Id, model.Aceite);
                usuario.Aceite = model.Aceite;

                return Ok(usuario);
            }
            catch (Exception e)
            {

                _logger.LogError($"AceiteTermosDoUso: {model.Username}");
                return BadRequest($"Erro ao aceitar Termos do Uso: {e.InnerException.Message}");
            }
            
        }
    }
}
