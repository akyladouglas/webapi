using System.Net;
using System.Net.Mail;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Helpers
{
    public class SendEmail
    {
        // public static string EmailSenha = "NcgTrh9Kw8q4Mp3P";
        //public static string EmailCovid = "AKIAVEER3D7W223MUFO5";
        public static string EmailFrom = "ti.novetech@gmail.com";
        public static string UserSES = "AKIAVEER3D7WUBHJDRL2";
        public static string PasswordSES = "BMyw7t09nI3VGRwiWQkx1fsIevXltSXAfoxJZ/iTExJU";

        public static void EnviarEmail(string destinatario, string assunto, string msg)
        {
            SmtpClient client = new SmtpClient("email-smtp.sa-east-1.amazonaws.com", 587);
            var oEmail = new MailMessage(EmailFrom, destinatario); // from, para
            oEmail.Body = msg;
            oEmail.Subject = assunto;
            oEmail.IsBodyHtml = true;

            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new NetworkCredential(UserSES, PasswordSES);
            client.Send(oEmail);
        }
    }

}
