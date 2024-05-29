using AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using Microsoft.Extensions.Configuration;
using System;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Helpers
{
    public class SendCode
    {
        IConfiguration _config;
        
        public SendCode(IConfiguration configuration)
        {
            _config = configuration;
        }

        public string SendCodeSmsOrEmail(string method, string email, string cellphone, string authenticationCode) {

            if(string.IsNullOrEmpty(method)) return "Método de envio não informado!";
            if(string.IsNullOrEmpty(email) && string.IsNullOrEmpty(cellphone)) return "O email ou celular não foi informado!";
            if(string.IsNullOrEmpty(authenticationCode)) return "O código de autenticação não foi informado!";

            switch (method)
            {
                case "Email":
                    RecoverToEmail(authenticationCode, email);
                    return "O código foi enviado para seu email de cadastro.";
                case "SMS":
                    // CÓDIGO PARA SMS
                    SendSms(authenticationCode, cellphone);
                    return "O código foi enviado para seu telefone de cadastro.";
                //case "WhatsApp":
                //    // CÓDIGO PARA WHATSAPP
                //    SendWhatsapp(individuoModel);
                //    break;
                default:
                    return "Escolha uma opção válida para o envio do código";
            }
        }

        private void RecoverToEmail(string authenticationCode, string email)
        {
            try
            {
                string emailBody = "Recebemos uma solicitação para redefinição da sua senha de acesso ao Portal. <br />" +
                             "<p>Para dar continuidade utilize o código abaixo: " +
                             "<p><strong>" + authenticationCode + "</strong></p>";

                string emailSubject = "Bem vindo(a) ao Portal";

            
                SendEmail.EnviarEmail(email, emailSubject, emailBody);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private void SendSms(string authenticationCode, string cellphone)
        {
            // Find your Account SID and Auth Token at twilio.com/console
            // and set the environment variables. See http://twil.io/secure
            string accountSid = _config["Twilio:AccountSid"];
            string authToken = _config["Twilio:AuthToken"];

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                body: "Olá! Segue seu código para autenticação via SMS. " + authenticationCode,
                from: new Twilio.Types.PhoneNumber("+12073864717"),
                to: new Twilio.Types.PhoneNumber("+55" + cellphone)
            );

            Console.WriteLine(message.Sid);
        }

        //private void SendWhatsapp(Individuo individuo)
        //{
        //    String numeroFone = individuo.TelefoneCelular;

        //    var numeroIndividuo = numeroFone.Remove(2, 1);

        //    var accountSid = _config["Twilio:AccountSid"];
        //    var authToken = _config["Twilio:AuthToken"];
        //    TwilioClient.Init(accountSid, authToken);

        //    var messageOptions = new CreateMessageOptions(
        //        new PhoneNumber("whatsapp:+55" + numeroIndividuo));
        //    messageOptions.From = new PhoneNumber("whatsapp:+12073864717");
        //    messageOptions.Body = "Olá! Segue seu código para autenticação via WhatsApp. " + individuo.CodigoAutenticacao;

        //    var message = MessageResource.Create(messageOptions);
        //    Console.WriteLine(message.Body);
        //}
    }
}
