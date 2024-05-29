using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.DistributedInterfaces.Services.Helpers;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class TokenController : Controller
    {
        private readonly IIndividuoApplication _individuoApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;


        public TokenController(IIndividuoApplication individuoApplication,
                                  IMapper mapper,
                                  ILogger<TokenController> logger, IConfiguration config)
        {
            _individuoApplication = individuoApplication;
            _mapper = mapper;
            _logger = logger;
            _config = config;
        }

        /// <summary>
        /// Autenticação do Indivíduo. Retorna um objeto com um JWT Token.
        /// </summary>
        /// <remarks>
        /// ### Observação ###
        /// <p>O objeto deve ser enviado totalmente em Base64: </p>
        /// username: MTQ3MDIyMTkwMDY= <br /> 
        /// password: MTIzNDU2 <br /> 
        /// audience: YXRlbmR0ZWxlbWVkaWNpbmE=
        /// </remarks>
        /// <param name="loginModel"></param>
        /// <response code="200">Token gerado com sucesso</response>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Post([FromBody] LoginModel loginModel)
        {
            if (loginModel == null) return Unauthorized();
            if (string.IsNullOrEmpty(loginModel.Username) || string.IsNullOrEmpty(loginModel.Password) || string.IsNullOrEmpty(loginModel.Audience))
                return Unauthorized();

            loginModel.Username = Base64Helper.FromBase64String(loginModel.Username);
            loginModel.Password = Base64Helper.FromBase64String(loginModel.Password);
            loginModel.Audience = Base64Helper.FromBase64String(loginModel.Audience);

            try
            {
                var user = _mapper.Map<IndividuoModel>(_individuoApplication.Login(loginModel.Username, loginModel.Password));

                if (user == null) return BadRequest("Verifique seu Usuário e Senha.");
                if (user.Ativo == false) return BadRequest("O paciente com o login informado está inativo, entre em contato com suporte");

                if (user.UfAbreviado == null || user.CidadeId == null) return Unauthorized("Verifique o preenchimento da Cidade e do Estado");

                user.Senha = "**********";

                return GenerateJWT(user);

            }
            catch (Exception e)
            {
                _logger.LogError($"Token: {e}");
                return NotFound();
            }
        }
        /// <summary>
        /// Autenticação do Indivíduo. Retorna um objeto com um JWT Token.
        /// </summary>
        /// <remarks>
        /// <param name="loginModel"></param>
        /// <response code="200">Token gerado com sucesso</response>
        [HttpPost("Facial")]
        [AllowAnonymous]
        public IActionResult Facial([FromBody] LoginModel loginModel)
        {
            if (loginModel == null) return Unauthorized();
            if (string.IsNullOrEmpty(loginModel.Username) || string.IsNullOrEmpty(loginModel.FaceToken) || string.IsNullOrEmpty(loginModel.Audience))
                return Unauthorized();

            loginModel.Username = Base64Helper.FromBase64String(loginModel.Username);
            loginModel.Audience = Base64Helper.FromBase64String(loginModel.Audience);
            loginModel.FaceToken = Base64Helper.FromBase64String(loginModel.FaceToken);

            var user = _mapper.Map<IndividuoModel>(_individuoApplication.GetByCpf(loginModel.Username));
            if (user.FaceToken != loginModel.FaceToken)
                return BadRequest("Token Inválido.");
            if (user.FaceTokenValidade <= DateTime.Now)
                return BadRequest("Token Facial Expirado.");

            try
            {

                if (user == null) return BadRequest("Verifique seu Usuário e Senha.");

                if (user.UfAbreviado == null || user.CidadeId == null) return Unauthorized("Verifique o preenchimento da Cidade e do Estado");

                user.Senha = "**********";

                return GenerateJWT(user);

            }
            catch (Exception e)
            {
                _logger.LogError($"Token: {e}");
                return NotFound();
            }
        }


        /// <summary>
        /// Autenticação do Indivíduo. Retorna um objeto com um JWT Token.
        /// </summary>
        /// <remarks>
        /// ### Observação ###
        /// <p>O objeto deve ser enviado totalmente em Base64: </p>
        /// username: MTQ3MDIyMTkwMDY= <br /> 
        /// password: MTIzNDU2 <br /> 
        /// audience: YXRlbmR0ZWxlbWVkaWNpbmE=
        /// </remarks>
        /// <param name="loginModel"></param>
        /// <response code="200">Token gerado com sucesso</response>
        [HttpPost]
        [Route("LoginPortal")]
        [AllowAnonymous]
        public IActionResult PostPortal([FromBody] LoginModel loginModel)
        {
            if (loginModel == null) return Unauthorized();
            if (string.IsNullOrEmpty(loginModel.Username) || string.IsNullOrEmpty(loginModel.Audience))
                return Unauthorized();

            loginModel.Username = Base64Helper.FromBase64String(loginModel.Username);
            loginModel.Audience = Base64Helper.FromBase64String(loginModel.Audience);

            try
            {
                var user = _mapper.Map<IndividuoModel>(_individuoApplication.LoginPortal(loginModel.Username));

                if (user == null) return BadRequest("Verifique seu Usuário e Senha.");
                if (user.Ativo == false) return BadRequest("O paciente com o login informado está inativo, entre em contato com suporte");

                if (user.UfAbreviado == null || user.CidadeId == null) return Unauthorized("Verifique o preenchimento da Cidade e do Estado");

                user.Senha = "**********";

                return GenerateJWT(user);

            }
            catch (Exception e)
            {
                _logger.LogError($"Token: {e}");
                return NotFound();
            }
        }


        /// <summary>
        /// Renova o Token do Indivíduo.
        /// O Request deve ser feito com o refresh_token gerado na autenticação inicial.
        /// </summary>
        /// <response code="200">Novo Access Token retornado com sucesso</response>
        [HttpGet("Refresh")]
        public IActionResult Refresh()
        {

            if (!User.Identity.IsAuthenticated)
            {
                _logger.LogInformation($"Token do Indivíduo ({User.FindFirstValue(ClaimTypes.Name)} expirado.");
                return BadRequest("Token expirado faça o Login novamente.");
            }

            var user = _mapper.Map<IndividuoModel>(_individuoApplication.GetById(User.FindFirstValue("uid")));

            if (user.UfAbreviado == null || user.CidadeId == null) return Unauthorized("Verifique o preenchimento da Cidade e do Estado");

            if (user != null)
            {
                user.Senha = "**********";
                return GenerateJWT(user);
            }
            else
            {
                return Unauthorized();
            }
        }

        private IActionResult GenerateJWT(IndividuoModel user)
        {
            if (user != null)
            {
                var token = "";

                if (user.Cns != null || user.Cpf == null) { token = user.Cns; };
                if (user.Cns == null || user.Cpf != null) { token = user.Cpf; };



                        var dynClaims = new ClaimsIdentity(new GenericIdentity(token, "Token"),
                     new[]
                    {
                                new Claim("uid", user.Id.ToString()),
                                new Claim(JwtRegisteredClaimNames.Sub, token),
                                new Claim(JwtRegisteredClaimNames.Email, user.Email != null ? user.Email : "atualizar@novetech.com.br"),
                                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64),
                    });

                        dynClaims.AddClaim(new Claim("scope", "Individuo"));
                        if (user.CidadeId != null) dynClaims.AddClaim(new Claim("cidadeId", user.CidadeId.ToString()));
                        if (user.UfAbreviado != null) dynClaims.AddClaim(new Claim("uf", user.UfAbreviado));

                        // TODO Adicionar a Claim ao Adicionar Indivíduo
                        dynClaims.AddClaim(new Claim(ClaimTypes.Role, "Individuo"));

                        #region ROLES
                        if (user.UserClaims.Count > 0)
                        {
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
                    

                        #region refresh token
                        var refreshClaims = new ClaimsIdentity(new GenericIdentity(token, "RefreshToken"),
                         new[]
                         {
                            new Claim("uid", user.Id.ToString()),
                            new Claim(JwtRegisteredClaimNames.Sub, token),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64),
                         });
                        //refreshClaims.AddClaim(new Claim("scope", "App"));
                        refreshClaims.AddClaim(new Claim("scope", "Individuo"));

                        var jwtRefresh = new JwtSecurityToken(_config["Tokens:Issuer"],
                        _config["Tokens:Audience"],
                        refreshClaims.Claims,
                        expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(int.Parse(_config["Tokens:ValidMinutes"]))).AddDays(7),
                        signingCredentials: creds);
                        #endregion

                        return Ok(new
                        {
                            access_token = new JwtSecurityTokenHandler().WriteToken(jwt),
                            expires_in = new DateTimeOffset(jwt.ValidTo).ToUnixTimeMilliseconds(),
                            refresh_token = new JwtSecurityTokenHandler().WriteToken(jwtRefresh)
                        });

                
            }
            return BadRequest("Não foi possível gerar o token");
        }

        private static long ToUnixEpochDate(DateTime date)
        {
            return (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
        }

    }
}
