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
using AtendTeleMedicina.Domain.Services.Helpers;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Profissional")]
    [Produces("application/json")]
    [Authorize]
    public class ProfissionalSenhaController : Controller
    {
        private readonly IProfissionalApplication _profissionalApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public ProfissionalSenhaController(IProfissionalApplication profissionalApplication,
                                  IMapper mapper,
                                  ILogger<ProfissionalSenhaController> logger,
                                  IConfiguration config)
        {
            _profissionalApplication = profissionalApplication;
            _mapper = mapper;
            _logger = logger;
            _config = config;
        }

        /// <summary>
        /// Individuo cadastra sua primeira senha
        /// </summary>
        /// <param name="profissionalId"></param>
        /// <param name="senhaModel"></param>
        /// <remarks>
        /// ### Observação ###
        /// <p>O objeto deve ser enviado totalmente em Base64: </p>
        /// senha: MTIzNDU2 <br /> 
        /// confirmacao: MTIzNDU2 <br /> 
        /// </remarks>
        [HttpPost("{profissionalId}/Senha")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ProfissionalSenhaModel), (int)HttpStatusCode.OK)]
        public IActionResult Post(string profissionalId, [FromBody] ProfissionalSenhaModel senhaModel)
        {
            try
            {
                if (senhaModel == null) return BadRequest();
                if (!senhaModel.Senha.Equals(senhaModel.Confirmacao)) return BadRequest("A confirmação da senha não confere com a senha digitada");

                var profissional = _profissionalApplication.GetById(profissionalId);

                if (profissional.Ativo == false) return BadRequest("Indivíduo pendente de Ativação.");

                _profissionalApplication.UpdateSenha(profissionalId, Base64Helper.Decode(senhaModel.Senha));

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

                var profissionalModel = _profissionalApplication.GetByCpf(cpf);
                if (profissionalModel == null) return BadRequest("Profissional não encontrado.");
                if (profissionalModel.Ativo == false) return BadRequest("Profissional pendente de Ativação.");

                profissionalModel.CodigoAutenticacao = _profissionalApplication.UpdateCodigoAutenticacao(profissionalModel.Id);

                switch (metodo)
                {
                    case "Email":
                        RecuperarPorEmail(profissionalModel);
                        break;
                    case "SMS":
                        // CÓDIGO PARA SMS
                        SendSms(profissionalModel);
                        break;
                    case "WhatsApp":
                        // CÓDIGO PARA WHATSAPP
                        // NÃO IMPLEMENTADO
                        SendWhatsapp(profissionalModel);
                        break;
                    default:
                        return BadRequest("Método de Redefinição de Senha Obrigatório");
                }

                return Ok(profissionalModel);

            }
            catch (Exception e)
            {
                _logger.LogError($"GetRecuperarSenha: {DateTime.Now}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        /// <summary>
        /// Profissional Redefine a senha
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
        [ProducesResponseType(typeof(ProfissionalSenhaModel), (int)HttpStatusCode.OK)]
        public IActionResult Redefinir([FromBody] ProfissionalSenhaModel senhaModel)
        {
            if (senhaModel == null) return BadRequest("Dados inválidos");
            if (string.IsNullOrEmpty(senhaModel.CodigoAutenticacao)) return BadRequest("Codigo de Autenticação não informado.");

            var hashSenha = CryptoHashService.GenerateSha1Hash(senhaModel.Senha);
            var profissionalModel = _profissionalApplication.GetByCpf(senhaModel.Cpf);
            if (profissionalModel == null) return NotFound();
            if (profissionalModel.CodigoAutenticacao != senhaModel.CodigoAutenticacao) return BadRequest("Código de redefinição de senha expirado");
            //if (!senhaModel.Senha.Equals(senhaModel.Confirmacao)) return BadRequest("A confirmação da senha não confere com a senha digitada.");
            if (hashSenha == profissionalModel.Senha) return BadRequest("Senha Digitada Igual A Do Usuario");


            // if (individuoModel.Senha != senhaModel.SenhaAntiga) return BadRequest("Senha antiga nao confere.");

            

            try
            {

                int ret = _profissionalApplication.UpdateSenha(profissionalModel.Id, senhaModel.Senha);
                return Ok("Senha alterada com sucesso.");
            }
            catch (Exception e)
            {
                return BadRequest("Erro ao alterar a senha, tente novamente.");
            }
        }

       private static void RecuperarPorEmail(Profissional profissional)
        {
            var emailCorpo = "Recebemos uma solicitação para redefinição da sua senha de acesso ao Portal. <br />" +
                             "<p>Para dar continuidade utilize o código abaixo: " +
                             "<p><strong>" + profissional.CodigoAutenticacao + "</strong></p>";

            var emailAssunto = "Bem vindo(a) ao Portal";

            try
            {
                SendEmail.EnviarEmail(profissional.Email, emailAssunto, emailCorpo);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void SendSms(Profissional profissional)
        {
            // Find your Account SID and Auth Token at twilio.com/console
            // and set the environment variables. See http://twil.io/secure
            string accountSid = _config["Twilio:AccountSid"];
            string authToken = _config["Twilio:AuthToken"];


            //String numeroFone = individuo.TelefoneCelular;
            //var numeroIndividuo = numeroFone.Remove(2, 1);

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                body: "Olá! Segue seu código para autenticação via SMS. " + profissional.CodigoAutenticacao,
                from: new Twilio.Types.PhoneNumber("+12073864717"),
                to: new Twilio.Types.PhoneNumber("+55" + profissional.Telefone)
            );

            Console.WriteLine(message.Sid);
        }

        //não implementado
        private void SendWhatsapp(Profissional profissional)
        {
            String numeroFone = profissional.Telefone;

            var numeroIndividuo = numeroFone.Remove(2, 1);

            var accountSid = _config["Twilio:AccountSid"];
            var authToken = _config["Twilio:AuthToken"];
            TwilioClient.Init(accountSid, authToken);

            var messageOptions = new CreateMessageOptions(
                new PhoneNumber("whatsapp:+55" + numeroIndividuo));
            messageOptions.From = new PhoneNumber("whatsapp:+12073864717");
            messageOptions.Body = "Olá! Segue seu código para autenticação via WhatsApp. " + profissional.CodigoAutenticacao;

            var message = MessageResource.Create(messageOptions);
            Console.WriteLine(message.Body);


        }

        

    }
}
