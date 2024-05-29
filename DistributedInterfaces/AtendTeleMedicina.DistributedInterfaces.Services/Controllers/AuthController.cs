using System;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Linq;
using AtendTeleMedicina.DistributedInterfaces.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using AtendTeleMedicina.Application.Contracts.Security;
using AutoMapper;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Security;
using AtendTeleMedicina.Domain.Entities.Security;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo;
using Microsoft.Extensions.Logging;
using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.Domain.Contracts.Repositories.Nucleo;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class AuthController : Controller
    {
        private readonly IUserApplication _userApplication;
        private readonly IProfissionalApplication _profissionalApplication;
        private readonly IUser _user;
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public AuthController(IConfiguration configuration,
                             IUserApplication userApplication,
                             IProfissionalApplication profissionalApplication,
                             IUser user,
                             IMapper mapper,
                             ILogger<AuthController> logger)
        {
            _config = configuration;
            _userApplication = userApplication;
            _profissionalApplication = profissionalApplication;
            _user = user;
            _mapper = mapper;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginModel loginForm)
        {
            if (loginForm == null) return BadRequest("Acesso Inválido");

            loginForm.Username = Base64Helper.FromBase64String(loginForm.Username);
            loginForm.Password = Base64Helper.FromBase64String(loginForm.Password);
            loginForm.Audience = Base64Helper.FromBase64String(loginForm.Audience);
            loginForm.Scope = Base64Helper.FromBase64String(loginForm.Scope);

            if (!_config["Tokens:Audience"].Contains(loginForm.Audience)) return BadRequest("Audience is invalid.");
            if (loginForm.Scope != "Usuario" && loginForm.Scope != "Profissional") return BadRequest("Scope is invalid.");

            try
            {
                UserModel user;
                //Login pelo scope Usuario
                if (loginForm.Scope == "Usuario")
                {
                    user = _mapper.Map<UserModel>(_userApplication.Login(loginForm.Username, loginForm.Password));
                    if (user == null)
                    {

                        var objRecuperado = _mapper.Map<UserModel>(_userApplication.GetByUserName(loginForm.Username));
                        if (objRecuperado != null)
                        {
                            if (objRecuperado.Ativo == false && objRecuperado.TentativaLogin >= 9)
                            {
                                return BadRequest("Usuário desativado devido a quantidade de tentativas de login!");
                            }
                            else
                            {
                                return BadRequest("Erro Verifique Os Dados De Login");
                            }
                        }
                        else {
                            return BadRequest("Erro Verifique Os Dados De Login");
                        }

                    }
                    else
                    {
                        //updateTentativas para zerar o contador no banco caso o usuario acerte a senha no meio do processo de login
                        _userApplication.UpdateTentativas(user.Id);
                    }
                    if (user.Ativo == false) return BadRequest("Erro no Login, Procure o seu Administrador");
                    user.Scope = loginForm.Scope;
                    return GetUserToken(user);
                }
                // Login pelo scope Profissional
                else {
                    user = _mapper.Map<UserModel>(_profissionalApplication.Login(loginForm.Username, loginForm.Password));
                    if (user == null)
                    {

                        var objRecuperado = _mapper.Map<UserModel>(_profissionalApplication.GetByUserName(loginForm.Username));
                        if (objRecuperado != null)
                        {
                            if (objRecuperado.Ativo == false && objRecuperado.TentativaLogin >= 9)
                            {
                                return BadRequest("Usuário desativado devido a quantidade de tentativas de login!");
                            }
                            else
                            {
                                return BadRequest("Erro Verifique Os Dados De Login");
                            }
                        }
                        else {
                            return BadRequest("Erro Verifique Os Dados De Login");
                        }
                        

                    }
                    else {
                        //updateTentativas para zerar o contador no banco caso o profissional acerte a senha no meio do processo de login
                        _profissionalApplication.UpdateTentativas(user.Id);
                    }

                    if (user.Ativo == false) return BadRequest("Erro no Login, Procure o seu Administrador");
                    user.Scope = loginForm.Scope;

                    return GetUserToken(user);
                }

            }
            catch (Exception e)
            {
                _logger.LogError($"AuthLogin: {loginForm}, {e}");
                return BadRequest(e.Message);
            }

        }

        [HttpGet("Refresh"), HttpPost("Refresh")]
        [AllowAnonymous]
        public IActionResult Refresh()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return BadRequest("Token expirado faça o Login novamente.");
            }
            var scope = _user.GetUserOrigem();
            UserModel user;

            if (scope == "Usuario")
            {
                user = _mapper.Map<UserModel>(_userApplication.GetById(_user.GetUserId()));
                if (user == null) return BadRequest("Erro Verifique Os Dados De Login");
                user.Scope = scope;
                return GetUserToken(user);
            }

            user = _mapper.Map<UserModel>(_profissionalApplication.GetById(_user.GetUserId()));
            if (user == null) return BadRequest("Erro Verifique Os Dados De Login");
            user.Scope = scope;

            return GetUserToken(user);
        }

        private IActionResult GetUserToken(UserModel user)
        {
            if (user != null)
            {
                var dynClaims = new ClaimsIdentity();

                if (user.Scope == "Usuario")
                {
                    dynClaims = new ClaimsIdentity(new GenericIdentity(user.Cpf, "Token"),
                    new[]
                    {
                            new Claim("uid", user.Id.ToString()),
                            new Claim("estabelecimentoId", !string.IsNullOrEmpty(user.EstabelecimentoId) ? user.EstabelecimentoId : ""),
                            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                            new Claim(JwtRegisteredClaimNames.GivenName, user.Nome),
                            new Claim(JwtRegisteredClaimNames.Email, user.Email != null ? user.Email : "atualizar@novetech.com.br"),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64)
                    });

                    if (!string.IsNullOrEmpty(user.EstabelecimentoId)) dynClaims.AddClaim(new Claim("estabelecimentoId", user.EstabelecimentoId.ToString()));
                }
                else {

                    dynClaims = new ClaimsIdentity(new GenericIdentity(user.Cpf, "Token"),
                    new[]
                    {
                            new Claim("uid", user.Id.ToString()),
                            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                            new Claim(JwtRegisteredClaimNames.GivenName, user.Nome),
                            new Claim(JwtRegisteredClaimNames.Email, user.Email != null ? user.Email : "atualizar@novetech.com.br"),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64)
                    });
                }
                

                dynClaims.AddClaim(new Claim("scope", user.Scope));
               
                if (user.CidadeId != 0) dynClaims.AddClaim(new Claim("cidadeId", user.CidadeId.ToString()));
                if (!string.IsNullOrEmpty(user.UltimoPerfilSelecionado)) dynClaims.AddClaim(new Claim("ultimoPerfilSelecionado", user.UltimoPerfilSelecionado.ToString()));
                if (user.Uf != null) dynClaims.AddClaim(new Claim("uf", user.Uf));
                if (user.UfAbreviado != null) dynClaims.AddClaim(new Claim("uf", user.UfAbreviado));
                if (user.SenhaAlterada == true)
                {
                    dynClaims.AddClaim(new Claim("senhaAlterada", "S"));
                }
                else
                {
                    //return BadRequest("entrou no primeiro else");
                    dynClaims.AddClaim(new Claim("senhaAlterada", "F"));
                }
                if(user.Imagem != null)
                {
                    dynClaims.AddClaim(new Claim("imagem", user.Imagem));
                }
               


                #region ROLES
                if (user.UserClaims.Count > 0)
                {
                    // Roles
                    if (user.Scope == "Profissional") dynClaims.AddClaim(new Claim(ClaimTypes.Role, "Profissional"));
                    if (user.Scope == "Usuario") dynClaims.AddClaim(new Claim(ClaimTypes.Role, "Usuario"));
                    dynClaims.AddClaim(new Claim(ClaimTypes.Role, "Retaguarda")); // claim padrão para diferenciar app e retaguarda

                    foreach (var role in user.UserClaims.Where(c => c.ClaimType == "Role"))
                    {
                        dynClaims.AddClaim(new Claim(ClaimTypes.Role, role.ClaimValue));
                    }

                    // Outras Claims
                    foreach (var role in user.UserClaims.Where(c => c.ClaimType != "Role"))
                    {
                        dynClaims.AddClaim(new Claim(role.ClaimType, role.ClaimValue));
                    }
                }
                #endregion

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var jwt = new JwtSecurityToken(_config["Tokens:Issuer"],
                _config["Tokens:Audience"],
                dynClaims.Claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(int.Parse(_config["Tokens:ValidMinutes"]))),
                signingCredentials: creds);




                var refreshClaims = new ClaimsIdentity();

                if (user.Scope == "Usuario")
                {
                    refreshClaims = new ClaimsIdentity(new GenericIdentity(user.Cpf, "RefreshToken"),
                    new[]
                    {
                        new Claim("uid", user.Id.ToString()),
                        new Claim("estabelecimentoId", !string.IsNullOrEmpty(user.EstabelecimentoId) ? user.EstabelecimentoId : ""),
                        new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64),
                    });

                    if (!string.IsNullOrEmpty(user.EstabelecimentoId)) dynClaims.AddClaim(new Claim("estabelecimentoId", user.EstabelecimentoId.ToString()));
                }
                else {
                    refreshClaims = new ClaimsIdentity(new GenericIdentity(user.Cpf, "RefreshToken"),
                    new[]
                    {
                        new Claim("uid", user.Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64),
                    });
                }
                

                refreshClaims.AddClaim(new Claim("scope", user.Scope));

                if (user.CidadeId != 0) refreshClaims.AddClaim(new Claim("cidadeId", user.CidadeId.ToString()));
                if (!string.IsNullOrEmpty(user.UltimoPerfilSelecionado)) refreshClaims.AddClaim(new Claim("ultimoPerfilSelecionado", user.UltimoPerfilSelecionado.ToString()));
                if (user.Uf != null) refreshClaims.AddClaim(new Claim("uf", user.Uf));
                if (user.UfAbreviado != null) refreshClaims.AddClaim(new Claim("uf", user.UfAbreviado));
                if (user.SenhaAlterada == true)
                {
                    refreshClaims.AddClaim(new Claim("senhaAlterada", "S"));
                }
                else
                {
                    refreshClaims.AddClaim(new Claim("senhaAlterada", "F"));
                }
                


                #region ROLES
                if (user.UserClaims.Count > 0)
                {
                    // Roles
                    if (user.Scope == "Profissional") refreshClaims.AddClaim(new Claim(ClaimTypes.Role, "Profissional"));
                    if (user.Scope == "Usuario") refreshClaims.AddClaim(new Claim(ClaimTypes.Role, "Usuario"));
                    refreshClaims.AddClaim(new Claim(ClaimTypes.Role, "Retaguarda")); // claim padrão para diferenciar app e retaguarda

                    foreach (var role in user.UserClaims.Where(c => c.ClaimType == "Role"))
                    {
                        refreshClaims.AddClaim(new Claim(ClaimTypes.Role, role.ClaimValue));
                    }

                    // Outras Claims
                    foreach (var role in user.UserClaims.Where(c => c.ClaimType != "Role"))
                    {
                        refreshClaims.AddClaim(new Claim(role.ClaimType, role.ClaimValue));
                    }
                }
                #endregion

                var jwtRefresh = new JwtSecurityToken(_config["Tokens:Issuer"],
                _config["Tokens:Audience"],
                refreshClaims.Claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(int.Parse(_config["Tokens:ValidMinutes"]))).AddMinutes(10),
                signingCredentials: creds);

                return Ok(new
                {
                    access_token = new JwtSecurityTokenHandler().WriteToken(jwt),
                    expires_in = new DateTimeOffset(jwt.ValidTo).ToUnixTimeMilliseconds(),
                    refresh_token = new JwtSecurityTokenHandler().WriteToken(jwtRefresh),
                });
            }

            return BadRequest("Não foi possível gerar o token");
        }

        [HttpGet("UserInfo")]
        public IActionResult UserInfo()
        {
            var permissoes = User.FindAll(ClaimTypes.Role).Select(role => role.Value).ToList();
            var scope = User.FindFirstValue("scope");

            if (scope == "Individuo")
            {
                var individuo = new IndividuoModel()
                {
                    Id = User.FindFirstValue("uid"),
                    
                    Username = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Audience = User.FindFirstValue("aud"),
                    NomeCompleto = User.FindFirstValue(ClaimTypes.GivenName),
                    Email = User.FindFirstValue(ClaimTypes.Email),
                    CidadeId = User.FindFirstValue("cidadeId"),
                    UfAbreviado = User.FindFirstValue("uf"),
                    Roles = permissoes
                };

                return new OkObjectResult(individuo);
            }

            if (scope == "Usuario") {
                var usuario = new UserModel()
                {
                    Id = User.FindFirstValue("uid"),
                    Username = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Audience = User.FindFirstValue("aud"),
                    Nome = User.FindFirstValue(ClaimTypes.GivenName),
                    UltimoPerfilSelecionado = User.FindFirstValue("ultimoPerfilSelecionado"),
                    Email = User.FindFirstValue(ClaimTypes.Email),
                    CidadeId = int.Parse(User.FindFirstValue("cidadeId")),
                    EstabelecimentoId = User.FindFirstValue("estabelecimentoId"),
                    Uf = User.FindFirstValue("uf"),
                    SenhaAlterada = User.FindFirstValue("senhaAlterada") == "S" ? true : false,
                    Roles = permissoes,
                    Imagem = User.FindFirstValue("imagem")
                };

                return new OkObjectResult(usuario);
            }

            var profissional = new UserModel()
            {
                Id = User.FindFirstValue("uid"),
                Username = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Audience = User.FindFirstValue("aud"),
                UltimoPerfilSelecionado = User.FindFirstValue("ultimoPerfilSelecionado"),
                Nome = User.FindFirstValue(ClaimTypes.GivenName),
                Email = User.FindFirstValue(ClaimTypes.Email),
                CidadeId = int.Parse(User.FindFirstValue("cidadeId")),
                Uf = User.FindFirstValue("uf"),
                SenhaAlterada = User.FindFirstValue("senhaAlterada") == "S" ? true : false,
                Roles = permissoes,
                Imagem = User.FindFirstValue("imagem")
            };

            return new OkObjectResult(profissional);

        }

        private static long ToUnixEpochDate(DateTime date)
        {
            return (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
        }

    }
}
