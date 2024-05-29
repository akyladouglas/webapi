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

namespace AtendTeleMedicina.DistributedInterfaces.Services.Controllers.Nucleo
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Individuo")]
    [Produces("application/json")]
    [Authorize]
    public class IndividuoSenhaController : Controller
    {
        private readonly IIndividuoApplication _individuoApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public IndividuoSenhaController(IIndividuoApplication individuoApplication,
                                  IMapper mapper,
                                  ILogger<IndividuoSenhaController> logger,
                                  IConfiguration config)
        {
            _individuoApplication = individuoApplication;
            _mapper = mapper;
            _logger = logger;
            _config = config;
        }

        /// <summary>
        /// Individuo cadastra sua primeira senha
        /// </summary>
        /// <param name="individuoId"></param>
        /// <param name="senhaModel"></param>
        /// <remarks>
        /// ### Observação ###
        /// <p>O objeto deve ser enviado totalmente em Base64: </p>
        /// senha: MTIzNDU2 <br /> 
        /// confirmacao: MTIzNDU2 <br /> 
        /// </remarks>
        [HttpPost("{individuoId}/Senha")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IndividuoSenhaModel), (int)HttpStatusCode.OK)]
        public IActionResult Post(string individuoId, [FromBody] IndividuoSenhaModel senhaModel)
        {
            try
            {
                if (senhaModel == null) return BadRequest();
                if (!senhaModel.Senha.Equals(senhaModel.Confirmacao)) return BadRequest("A confirmação da senha não confere com a senha digitada");

                var individuo = _individuoApplication.GetById(individuoId);

                if (individuo.Ativo == false) return BadRequest("Indivíduo pendente de Ativação.");

                _individuoApplication.UpdateSenha(individuoId, Base64Helper.Decode(senhaModel.Senha), "cadastro");

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

                var individuoModel = _individuoApplication.GetByCpf(cpf);
                if (individuoModel == null) return BadRequest("Indivíduo não encontrado.");
                if (individuoModel.Ativo == false) return BadRequest("Indivíduo pendente de Ativação.");

                individuoModel.CodigoAutenticacao = _individuoApplication.UpdateCodigoAutenticacao(individuoModel.Id);

                switch (metodo)
                {
                    case "Email":
                        RecuperarPorEmail(individuoModel);
                        break;
                    case "SMS":
                        // CÓDIGO PARA SMS
                        SendSms(individuoModel);
                        break;
                    case "WhatsApp":
                        // CÓDIGO PARA WHATSAPP
                        SendWhatsapp(individuoModel);
                        break;
                    default:
                        return BadRequest("Método de Redefinição de Senha Obrigatório");
                }

                return Ok("Solicitação de Redefinição de senha realizada com sucesso.");

            }
            catch (Exception e)
            {
                _logger.LogError($"GetRecuperarSenha: {DateTime.Now}, {e}");
                if (e.Message.Contains("is not a mobile number")) return BadRequest("O número informado não é um número de celular");
                return BadRequest(e.Message.ToString());
            }
        }

        /// <summary>
        /// Individuo Redefine a senha
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
        [ProducesResponseType(typeof(IndividuoSenhaModel), (int)HttpStatusCode.OK)]
        public IActionResult Redefinir([FromBody] IndividuoSenhaModel senhaModel)
        {
            if (senhaModel == null) return BadRequest("Dados inválidos");
            if (string.IsNullOrEmpty(senhaModel.CodigoAutenticacao)) return BadRequest("Codigo de Autenticação não informado.");

            senhaModel.Cpf = Base64Helper.Decode(senhaModel.Cpf);
            senhaModel.CodigoAutenticacao = Base64Helper.Decode(senhaModel.CodigoAutenticacao);
            senhaModel.Senha = Base64Helper.Decode(senhaModel.Senha);
            senhaModel.Confirmacao = Base64Helper.Decode(senhaModel.Confirmacao);
            if (!senhaModel.Senha.Equals(senhaModel.Confirmacao)) return BadRequest("A confirmação da senha não confere com a senha digitada.");

            var individuoModel = _individuoApplication.GetByCpf(senhaModel.Cpf);
            if (individuoModel == null) return NotFound();
            if (individuoModel.CodigoAutenticacao != senhaModel.CodigoAutenticacao) return BadRequest("Código de redefinição de senha expirado");
            //if (individuoModel.Senha != senhaModel.SenhaAntiga) return BadRequest("Senha antiga nao confere.");

            try
            {
                int ret = _individuoApplication.UpdateSenha(individuoModel.Id, senhaModel.Senha, "redefinição");
                return Ok("Senha alterada com sucesso.");
            }
            catch (Exception e)
            {
                return BadRequest("Erro ao alterar a senha, tente novamente.");
            }
        }

        private static void RecuperarPorEmail(Individuo individuo)
        {
            var emailCorpo = "Recebemos uma solicitação para redefinição da sua senha de acesso ao Portal. <br />" +
                             "<p>Para dar continuidade utilize o código abaixo: " +
                             "<p><strong>" + individuo.CodigoAutenticacao + "</strong></p>";

            var emailAssunto = "Bem vindo(a) ao Portal";

            try
            {
                SendEmail.EnviarEmail(individuo.Email, emailAssunto, emailCorpo);
            }
            catch (Exception e)
            {
                throw e;
            }
        }



        private void SendSms(Individuo individuo)
        {
            // Find your Account SID and Auth Token at twilio.com/console
            // and set the environment variables. See http://twil.io/secure
            string accountSid = _config["Twilio:AccountSid"];
            string authToken = _config["Twilio:AuthToken"];


            //String numeroFone = individuo.TelefoneCelular;
            //var numeroIndividuo = numeroFone.Remove(2, 1);

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                body: "Olá! Segue seu código para autenticação via SMS. " + individuo.CodigoAutenticacao,
                from: new Twilio.Types.PhoneNumber("+12073864717"),
                to: new Twilio.Types.PhoneNumber("+55" + individuo.TelefoneCelular )
            );

            Console.WriteLine(message.Sid);
        }


        private void SendWhatsapp(Individuo individuo)
        {
            String numeroFone = individuo.TelefoneCelular;

            var numeroIndividuo = numeroFone.Remove(2, 1);

            var accountSid = _config["Twilio:AccountSid"];
            var authToken = _config["Twilio:AuthToken"];
            TwilioClient.Init(accountSid, authToken);

            var messageOptions = new CreateMessageOptions(
                new PhoneNumber("whatsapp:+55"+ numeroIndividuo));
            messageOptions.From = new PhoneNumber("whatsapp:+12073864717");
            messageOptions.Body = "Olá! Segue seu código para autenticação via WhatsApp. " + individuo.CodigoAutenticacao;

            var message = MessageResource.Create(messageOptions);
            Console.WriteLine(message.Body);


        }

        

    }
}
