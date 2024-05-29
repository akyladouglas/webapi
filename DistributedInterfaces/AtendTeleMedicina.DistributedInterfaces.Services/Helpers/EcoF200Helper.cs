using System;
using System.Text;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Helpers
{
    public class EcoF200Helper
    {
        #region Função para limpar dados do HL7. Exemplo: 'SDB-0710^COVID/FLU ^LN' para 'COVID/FLU'
        public static string CleanString(string input)
        {
            string[] parts = input.Split('^');
            if (parts.Length < 2)
                return input;
            return parts[1];
        }
        #endregion
    }
}
