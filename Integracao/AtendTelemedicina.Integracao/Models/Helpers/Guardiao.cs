using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AtendTelemedicina.Integracao.Models.Helpers
{

    /// <summary>
    /// Classe Guardiao.
    /// Contém validações genéricas que são utilizadas em projeto
    /// </summary>
    public static class Guardiao
    {
        /// <summary>
        /// Verifica se o valor em string é um numero inteiro.
        /// </summary>
        /// <param name="valor">O valor.</param>
        /// <param name="mensagem">A mensagem de erro.</param>
        /// <returns><c>verdadeiro</c> se for número inteiro, senão <c>falso</c>.</returns>
        /// <exception cref="System.Exception"></exception>
        public static bool EhNumeroInteiro(string valor, string mensagem = null)
        {
            if (string.IsNullOrEmpty(valor))
                return false;

            var match = Regex.Match(valor, @"^[0-9]+?(.|,[0-9]+)$");
            if (match.Success)
                return true;

            throw new Exception(mensagem ?? "O valor não é um número inteiro.");
        }

        /// <summary>
        /// Verifica e retorna o valor caso não contenha caracteres especiais. Senão, uma exceção é gerada.
        /// </summary>
        /// <param name="valor">O valor.</param>
        /// <param name="mensagem">A mensagem de erro.</param>
        /// <returns><c>verdadeiro</c> se não conter caracteres especiais, senão<c>falso</c> otherwise.</returns>
        /// <exception cref="System.Exception"></exception>
        public static string VerificarCaracteresEspeciais(string valor, string mensagem = null)
        {
            if (string.IsNullOrEmpty(valor))
                return null;

            var match = Regex.Match(valor.Trim(), "[^a-z0-9]", RegexOptions.IgnoreCase);

            if (!match.Success)
                return valor;

            throw new Exception(mensagem ?? "O valor não pode conter caracteres especiais.");
        }

        /// <summary>
        /// Remove os caracteres especiais de uma valaor string.
        /// </summary>
        /// <param name="valor">O valor.</param>
        /// <param name="mensagem">A mensagem de erro.</param>
        /// <returns>System.String.</returns>
        public static string RemoverCaracteresEspeciais(string valor, string mensagem = null)
        {
            if (string.IsNullOrEmpty(valor))
                return null;

            var saida = Regex.Replace(valor, @"[^0-9a-zA-Z]+", "");
            return saida;
        }

        /// <summary>
        /// Verifica se uma string contém apenas números inteiros e retorna o valor (Apenas números).
        /// </summary>
        /// <param name="valor">O valor.</param>
        /// <param name="mensagem">A mensagem.</param>
        /// <returns>System.String.</returns>
        public static string RetornarApenasNumeroInteiro(string valor, string mensagem = null)
        {
            if (string.IsNullOrEmpty(valor))
                return null;

            const string pattern = "[^0-9]";
            const string saida = "";

            var match = Regex.Replace(valor, pattern, saida);

            //if (match.Success)
            //    return valor;

            return match;
        }

        /// <summary>
        /// Remove os acentos de uma string.
        /// </summary>
        /// <param name="str">A string.</param>
        /// <returns>System.String.</returns>
        public static string RemoverAcentos(this string str)
        {
            if (str == null) return null;
            var chars =
                from c in str.Normalize(NormalizationForm.FormD).ToCharArray()
                let uc = CharUnicodeInfo.GetUnicodeCategory(c)
                where uc != UnicodeCategory.NonSpacingMark
                select c;

            var outStr = new string(chars.ToArray()).Normalize(NormalizationForm.FormC);

            var normalized = outStr.Normalize(NormalizationForm.FormKD);

            var removal = Encoding.GetEncoding(Encoding.ASCII.CodePage,
                                                    new EncoderReplacementFallback(""),
                                                    new DecoderReplacementFallback(""));
            var bytes = removal.GetBytes(normalized);

            var cleanStr = Encoding.ASCII.GetString(bytes).ToUpper();

            return cleanStr;
        }

        /// <summary>
        /// Verifica se um determinado IP está no padrão IPv4 válido.
        /// </summary>
        /// <param name="ipString">O ip.</param>
        /// <returns><c>verdadeiro</c> if válido, senão<c>falso</c>.</returns>
        public static bool ValidarIPv4(string ipString)
        {
            if (string.IsNullOrWhiteSpace(ipString))
                return false;

            var splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
                return false;

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }

        /// <summary>
        /// Verifica se o email é válido.
        /// </summary>
        /// <param name="email">O email.</param>
        /// <returns><c>verdadeiro</c> if válido, senão<c>falso</c>.</returns>
        public static bool ValidaEmail(string email)
        {
            return Regex.IsMatch(email, @"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$");
        }


    }
}
