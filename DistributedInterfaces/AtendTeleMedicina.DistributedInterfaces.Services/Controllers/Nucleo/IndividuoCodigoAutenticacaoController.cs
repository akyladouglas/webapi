using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using Microsoft.AspNetCore.Authorization;
using AtendTeleMedicina.Application.Contracts.Nucleo;
using AtendTeleMedicina.DistributedInterfaces.Services.Helpers;
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
    [AllowAnonymous]
    public class IndividuoCodigoAutenticacaoController : Controller
    {
        private readonly IIndividuoApplication _individuoApplication;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public IndividuoCodigoAutenticacaoController(IIndividuoApplication individuoApplication,
                                  IMapper mapper,
                                  ILogger<IndividuoCodigoAutenticacaoController> logger,
                                  IConfiguration config)
        {
            _individuoApplication = individuoApplication;
            _mapper = mapper;
            _logger = logger;
            _config = config;
        }

        /// <summary>
        /// Recupera o código de Autenticação do Indivíduo
        /// </summary>
        /// <param name="individuoId"></param>
        /// <param name="metodo">Email, SMS, WhatsApp</param>
        [HttpGet("{individuoId}/CodigoAutenticacao")]
        [AllowAnonymous]
        public IActionResult Get(string individuoId, string metodo)
        {
            try
            {
                if (individuoId == null && string.IsNullOrEmpty(metodo)) return BadRequest();
                var individuo = _individuoApplication.GetById(individuoId);
                individuo.CodigoAutenticacao = _individuoApplication.UpdateCodigoAutenticacao(individuo.Id);

                if (individuo == null) return BadRequest("Indivíduo não encontrado");

                individuo.CodigoAutenticacao = _individuoApplication.UpdateCodigoAutenticacao(individuo.Id);

                switch (metodo)
                {
                    case "Email":
                        ConfirmarPorEmail(individuo);
                        break;
                    case "SMS":
                        // CÓDIGO PARA SMS
                        SendSms(individuo);
                        break;
                    case "WhatsApp":
                        // CÓDIGO PARA WHATSAPP
                        SendWhatsapp(individuo);
                        break;
                    default:
                        return BadRequest("Método de Recuperação de Código Obrigatório");
                }

                return Ok("Código enviado com sucesso.");

            }
            catch (Exception e)
            {
                _logger.LogError($"CodigoAutenticacao: {DateTime.Now}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }

        /// <summary>
        /// Confirma o Cadastro do Indivíduo pelo Código de Autenticação
        /// </summary>
        /// <param name="individuoId"></param>
        /// <param name="codigo"></param>
        [HttpPut("{individuoId}/CodigoAutenticacao")]
        [AllowAnonymous]
        public IActionResult Put(string individuoId, string codigo)
        {
            try
            {
                if (individuoId == null && string.IsNullOrEmpty(codigo)) return BadRequest();

                var individuo = _individuoApplication.GetById(individuoId);

                if (individuo.CodigoAutenticacao != codigo) return BadRequest("Código de Autenticação inválido.");

                individuo.Ativo = true;
                _individuoApplication.Update(individuoId, individuo);

                return Ok("Indivíduo Autenticado com sucesso.");

            }
            catch (Exception e)
            {
                _logger.LogError($"CodigoAutenticacao: {DateTime.Now}, {e}");
                return BadRequest(e.Message.ToString());
            }
        }


        private void ConfirmarPorEmail(Individuo individuo)
        {
            var emailCorpo = individuo.NomeCompleto + ", você está recebendo o código de autenticação para acesso ao Portal<br />" +
                             "<p>Quando solicitado, utilize o código abaixo: " +
                             "<p><strong>" + individuo.CodigoAutenticacao + "</strong></p>";

            var emailAssunto = "Bem vindo(a) ao Portal";

            try
            {
                SendEmail.EnviarEmail(individuo.Email, emailAssunto, emailCorpo);
            }
            catch (Exception e)
            {
                if (e.InnerException != null) throw e.InnerException;
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
                to: new Twilio.Types.PhoneNumber("+55" + individuo.TelefoneCelular)
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
                new PhoneNumber("whatsapp:+55" + numeroIndividuo));
            messageOptions.From = new PhoneNumber("whatsapp:+12073864717");
            messageOptions.Body = "Olá! Segue seu código para autenticação via WhatsApp. " + individuo.CodigoAutenticacao;

            var message = MessageResource.Create(messageOptions);
            Console.WriteLine(message.Body);


        }


    }
}
