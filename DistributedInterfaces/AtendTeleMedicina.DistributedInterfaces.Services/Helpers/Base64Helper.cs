using System;
using System.Text;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Helpers
{
    public class Base64Helper
    {
        public static string FromBase64String(string s)
        {
            byte[] data = Convert.FromBase64String(s);
            return Encoding.UTF8.GetString(data);
        }

        public static string Encode(string valorDeCodificado)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(valorDeCodificado);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Decode(string valorCodificado)
        {
            if (string.IsNullOrEmpty(valorCodificado))
                return null;

            byte[] data = Convert.FromBase64String(valorCodificado);
            string valorDecodificado = Encoding.ASCII.GetString(data);
            return valorDecodificado;
        }
    }
}
