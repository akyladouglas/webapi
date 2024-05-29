using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.DistributedInterfaces.Services.Helpers;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Security;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Microsoft.Extensions.Configuration;
using AtendTeleMedicina.Application.Contracts.Security;
using AtendTeleMedicina.Domain.Entities.Security;
using AtendTeleMedicina.Domain.Services.Helpers;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Usuario")]
    [Produces("application/json")]
    [Authorize]
    public class UsuarioSenhaController : Controller
    {
        private readonly IUserApplication _usuarioApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public UsuarioSenhaController(IUserApplication usuarioApplication,
                                  IMapper mapper,
                                  ILogger<UsuarioSenhaController> logger,
                                  IConfiguration config)
        {
            _usuarioApplication = usuarioApplication;
            _mapper = mapper;
            _logger = logger;
            _config = config;
        }

        /// <summary>
        /// Individuo cadastra sua primeira senha
        /// </summary>
        /// <param name="usuarioId"></param>
        /// <param name="senhaModel"></param>
        /// <remarks>
        /// ### Observação ###
        /// <p>O objeto deve ser enviado totalmente em Base64: </p>
        /// senha: MTIzNDU2 <br /> 
        /// confirmacao: MTIzNDU2 <br /> 
        /// </remarks>
        [HttpPost("{usuarioId}/Senha")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UsuarioSenhaModel), (int)HttpStatusCode.OK)]
        public IActionResult Post(string usuarioId, [FromBody] UsuarioSenhaModel senhaModel)
        {
            try
            {
                if (senhaModel == null) return BadRequest();
                if (!senhaModel.Senha.Equals(senhaModel.Confirmacao)) return BadRequest("A confirmação da senha não confere com a senha digitada");

                var usuario = _usuarioApplication.GetById(usuarioId);

                if (usuario.Ativo == false) return BadRequest("Indivíduo pendente de Ativação.");

                _usuarioApplication.UpdateSenha(usuarioId, Base64Helper.Decode(senhaModel.Senha));

                return Ok("Senha criada com sucesso.");

            }
            catch (Exception e)
            {
                _logger.LogError($"CodigoAutenticacao: {DateTime.Now}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        /// <summary>
        /// Individuo solicita recuperação de senha
        /// </summary>
        /// <param name="cpf"></param>
        /// <param name="metodo">email, sms, whatsapp</param>
        [HttpGet]
        [Route("Senha/Recuperar")]
        [AllowAnonymous]
        public IActionResult Recuperar(string cpf, string metodo)
        {
            try
            {
                if (string.IsNullOrEmpty(cpf)) return BadRequest("Informa o CPF para iniciar o processo de recuperação de senha.");

                var usuarioModel = _usuarioApplication.GetByCpf(cpf);
                if (usuarioModel == null) return BadRequest("Usuario não encontrado.");
                if (usuarioModel.Ativo == false) return BadRequest("Usuario pendente de Ativação.");

                usuarioModel.CodigoAutenticacao = _usuarioApplication.UpdateCodigoAutenticacao(usuarioModel.Id);

                switch (metodo)
                {
                    case "Email":
                        RecuperarPorEmail(usuarioModel);
                        break;
                    case "SMS":
                        // CÓDIGO PARA SMS
                        SendSms(usuarioModel);
                        break;
                    case "WhatsApp":
                        // CÓDIGO PARA WHATSAPP
                        // NÃO IMPLEMENTADO
                        SendWhatsapp(usuarioModel);
                        break;
                    default:
                        return BadRequest("Método de Redefinição de Senha Obrigatório");
                }

                return Ok(usuarioModel);

            }
            catch (Exception e)
            {
                _logger.LogError($"GetRecuperarSenha: {DateTime.Now}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        /// <summary>
        /// Usuario Redefine a senha
        /// </summary>
        /// <remarks>
        /// ### Observação ###
        /// <p>O objeto deve ser enviado totalmente em Base64: </p>
        /// cpf: MTIzNDU2XRlb <br />
        /// codigoAutenticacao: YXRlbmR0Z <br /> 
        /// senha: MTIzNDU2 <br /> 
        /// confirmacao: MTIzNDU2
        /// </remarks>
        /// <param name="senhaModel"></param>
        [HttpPut]
        [AllowAnonymous]
        [Route("Senha/Redefinir")]
        [ProducesResponseType(typeof(UsuarioSenhaModel), (int)HttpStatusCode.OK)]
        public IActionResult Redefinir([FromBody] UsuarioSenhaModel senhaModel)
        {
            if (senhaModel == null) return BadRequest("Dados inválidos");
            if (string.IsNullOrEmpty(senhaModel.CodigoAutenticacao)) return BadRequest("Codigo de Autenticação não informado.");

            var hashSenha = CryptoHashService.GenerateSha1Hash(senhaModel.Senha);
            var usuarioModel = _usuarioApplication.GetByCpf(senhaModel.Cpf);
            if (usuarioModel == null) return NotFound();
            if (usuarioModel.CodigoAutenticacao != senhaModel.CodigoAutenticacao) return BadRequest("Código de redefinição de senha expirado");
            //if (!senhaModel.Senha.Equals(senhaModel.Confirmacao)) return BadRequest("A confirmação da senha não confere com a senha digitada.");
            if (hashSenha == usuarioModel.Senha) return BadRequest("Senha Digitada Igual A Do Usuario");


            //if (individuoModel.Senha != senhaModel.SenhaAntiga) return BadRequest("Senha antiga nao confere.");

            try
            {
                int ret = _usuarioApplication.UpdateSenha(usuarioModel.Id, senhaModel.Senha);
                return Ok("Senha alterada com sucesso.");
            }
            catch (Exception e)
            {
                return BadRequest("Erro ao alterar a senha, tente novamente.");
            }
        }

       private static void RecuperarPorEmail(User usuario)
        {
            var emailCorpo = "Recebemos uma solicitação para redefinição da sua senha de acesso ao Portal. <br />" +
                             "<p>Para dar continuidade utilize o código abaixo: " +
                             "<p><strong>" + usuario.CodigoAutenticacao + "</strong></p>";

            var emailAssunto = "Bem vindo(a) ao Portal";

            try
            {
                SendEmail.EnviarEmail(usuario.Email, emailAssunto, emailCorpo);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void SendSms(User usuario)
        {
            // Find your Account SID and Auth Token at twilio.com/console
            // and set the environment variables. See http://twil.io/secure
            string accountSid = _config["Twilio:AccountSid"];
            string authToken = _config["Twilio:AuthToken"];


            //String numeroFone = individuo.TelefoneCelular;
            //var numeroIndividuo = numeroFone.Remove(2, 1);

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                body: "Olá! Segue seu código para autenticação via SMS. " + usuario.CodigoAutenticacao,
                from: new Twilio.Types.PhoneNumber("+12073864717"),
                to: new Twilio.Types.PhoneNumber("+55" + usuario.Telefone)
            );

            Console.WriteLine(message.Sid);
        }

        //não implementado
        private void SendWhatsapp(User usuario)
        {
            String numeroFone = usuario.Telefone;

            var numeroIndividuo = numeroFone.Remove(2, 1);

            var accountSid = _config["Twilio:AccountSid"];
            var authToken = _config["Twilio:AuthToken"];
            TwilioClient.Init(accountSid, authToken);

            var messageOptions = new CreateMessageOptions(
                new PhoneNumber("whatsapp:+55" + numeroIndividuo));
            messageOptions.From = new PhoneNumber("whatsapp:+12073864717");
            messageOptions.Body = "Olá! Segue seu código para autenticação via WhatsApp. " + usuario.CodigoAutenticacao;

            var message = MessageResource.Create(messageOptions);
            Console.WriteLine(message.Body);


        }

        

    }
}
