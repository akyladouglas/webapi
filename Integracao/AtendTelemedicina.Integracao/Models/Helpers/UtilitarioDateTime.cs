using System;
using System.Diagnostics;

namespace AtendTelemedicina.Integracao.Models.Helpers
{
    public static class UtilitarioDateTime
    {
        /// <summary>
        /// Calcula a idade baseado na diferença entre a data de nascimento e a data de referência.
        /// </summary>
        /// <param name="dataReferencia">A data de referência.</param>
        /// <param name="dataNascimento">A data nascimento.</param>
        /// <returns>System.Int32.</returns>
        public static int CalcularIdade(DateTime dataReferencia, DateTime dataNascimento)
        {
            var idade = dataReferencia.Year - dataNascimento.Year;
            if (dataReferencia < dataNascimento.AddYears(idade)) idade--;
            return idade;
        }

        /// <summary>
        /// Calcula a idade baseado na diferença entre a data de nascimento e a data de referência. Retorna nula caso a data de nascimento seja nula.
        /// </summary>
        /// <param name="dataReferencia">A data referência.</param>
        /// <param name="dataNascimento">A data nascimento.</param>
        /// <returns>System.Nullable&lt;System.Int32&gt;.</returns>
        public static int? CalcularIdade(DateTime dataReferencia, DateTime? dataNascimento)
        {
            var idade = dataNascimento.HasValue ? (int?)(DateTime.Today.Subtract(dataNascimento.Value.Date).TotalDays / 365.25) : null;
            return idade;
        }

        /// <summary>
        /// Calcula e obtém como resultado a diferença em meses entre duas datas.
        /// </summary>
        /// <param name="dataInicial">A data inicial.</param>
        /// <param name="dataFinal">A data final.</param>
        /// <returns>System.Int32.</returns>
        public static int DiferencaEntreDatasEmMeses(DateTime dataInicial, DateTime dataFinal)
        {
            var diffMeses = 12 * (dataInicial.Year - dataFinal.Year) + dataInicial.Month - dataFinal.Month;
            return Math.Abs(diffMeses);
        }


        public static FormFaixaEtaria ConsultarFaixaEtaria(string faixaTipo, int? idadeInicial, int? idadeFinal)
        {
            var form = new FormFaixaEtaria();
            if (!idadeInicial.HasValue && !idadeFinal.HasValue) return form;

            form.IsFaixaEtaria = false;
            int idadeInicio;
            int idadeFim;

            switch (faixaTipo)
            {
                case "ANOS":

                    Debug.Assert(idadeInicial != null, "idadeInicial != null");
                    idadeInicio = idadeInicial.Value < 0 ? 0 : (idadeInicial.Value > 150 ? 150 : idadeInicial.Value);
                    Debug.Assert(idadeFinal != null, "idadeFinal != null");
                    idadeFim = idadeFinal.Value < 0 ? 0 : (idadeFinal.Value > 150 ? 150 : idadeFinal.Value);
                    form.IsFaixaEtaria = true;
                    form.DataMax = DateTime.Today.AddYears(-idadeInicio);
                    form.DataMin = DateTime.Today.AddYears(-idadeFim - 1);
                    break;

                case "MESES":
                    Debug.Assert(idadeInicial != null, "idadeInicial != null");
                    idadeInicio = idadeInicial.Value < 0 ? 0 : (idadeInicial.Value > 12 ? 12 : idadeInicial.Value);
                    Debug.Assert(idadeFinal != null, "idadeFinal != null");
                    idadeFim = idadeFinal.Value < 0 ? 0 : (idadeFinal.Value > 12 ? 12 : idadeFinal.Value);
                    form.IsFaixaEtaria = true;
                    form.DataMax = DateTime.Today.AddMonths(-idadeInicio);
                    form.DataMin = DateTime.Today.AddMonths(-idadeFim - 1);
                    break;
            }

            return form;
        }

        /// <summary>
        /// Converte um valor unix time para o formato DateTime.
        /// </summary>
        /// <param name="unixTime">O unix time.</param>
        /// <returns>DateTime.</returns>
        public static DateTime UnixTimeParaDateTime(this long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        /// <summary>
        /// Converte uma data para o formato unix time.
        /// </summary>
        /// <param name="date">A data.</param>
        /// <returns>System.Int64.</returns>
        public static long DateTimeParaUnixTime(this DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalMilliseconds);
        }

    }

    /// <summary>
    /// Classe auxiliar FormFaixaEtaria.
    /// </summary>
    public class FormFaixaEtaria
    {
        public bool IsFaixaEtaria { get; set; }
        public DateTime DataMax { get; set; }
        public DateTime DataMin { get; set; }
    }
}
