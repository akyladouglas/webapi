using System;
using System.Collections.Generic;
using System.Text;

namespace AtendTeleMedicina.Domain.Helpers
{
    /// <summary>
    /// Class Idade
    /// </summary>
    public class Idade
    {
        /// <summary>
        /// Os dias
        /// </summary>
        public int Dias;

        /// <summary>
        /// Os meses
        /// </summary>
        public int Meses;

        /// <summary>
        /// Os anos
        /// </summary>
        public int Anos;

        protected Idade() { }

        public Idade(DateTime dataNascimento, DateTime? dataReferencia = null)
        {
            Calcular(dataNascimento, dataReferencia);
        }

        /// <summary>
        /// Calcula a idade usando a data de nascimento e data de referência específicada.
        /// </summary>
        /// <param name="dataNascimento">A data de nascimento.</param>
        /// <param name="dataReferencia">A data de referência.</param>
        /// <returns>Idade.</returns>
        /// <exception cref="System.ArgumentException">A data de nascimento não pode ser maior que a data de referência</exception>
        public Idade Calcular(DateTime dataNascimento, DateTime? dataReferencia = null)
        {

            if (!dataReferencia.HasValue)
                dataReferencia = DateTime.Today;

            if (dataReferencia.Value.Year - dataNascimento.Year > 0 ||
                ((dataReferencia.Value.Year - dataNascimento.Year == 0) && ((dataNascimento.Month < dataReferencia.Value.Month) ||
                                                                        ((dataNascimento.Month == dataReferencia.Value.Month) &&
                                                                         (dataNascimento.Day <= dataReferencia.Value.Day)))))
            {
                var diasNoMesDeAniversario = DateTime.DaysInMonth(dataNascimento.Year, dataNascimento.Month);
                var diasRestando = dataReferencia.Value.Day + (diasNoMesDeAniversario - dataNascimento.Day);

                if (dataReferencia.Value.Month > dataNascimento.Month)
                {
                    Anos = dataReferencia.Value.Year - dataNascimento.Year;
                    Meses = dataReferencia.Value.Month - (dataNascimento.Month + 1) + Math.Abs(diasRestando / diasNoMesDeAniversario);
                    Dias = (diasRestando % diasNoMesDeAniversario + diasNoMesDeAniversario) % diasNoMesDeAniversario;
                }
                else if (dataReferencia.Value.Month == dataNascimento.Month)
                {
                    if (dataReferencia.Value.Day >= dataNascimento.Day)
                    {
                        Anos = dataReferencia.Value.Year - dataNascimento.Year;
                        Meses = 0;
                        Dias = dataReferencia.Value.Day - dataNascimento.Day;
                    }
                    else
                    {
                        Anos = (dataReferencia.Value.Year - 1) - dataNascimento.Year;
                        Meses = 11;
                        Dias = DateTime.DaysInMonth(dataNascimento.Year, dataNascimento.Month) -
                               (dataNascimento.Day - dataReferencia.Value.Day);
                    }
                }
                else
                {
                    Anos = (dataReferencia.Value.Year - 1) - dataNascimento.Year;
                    Meses = dataReferencia.Value.Month + (11 - dataNascimento.Month) + Math.Abs(diasRestando / diasNoMesDeAniversario);
                    Dias = (diasRestando % diasNoMesDeAniversario + diasNoMesDeAniversario) % diasNoMesDeAniversario;
                }
            }
            else
            {
                return new Idade();
                //throw new ArgumentException("A data de nascimento deve ser menor que a data atual.");
            }
            return this;
        }

        /// <summary>
        /// Retorna a idade condicionalmente em anos ou meses.
        /// </summary>
        /// <returns>System.String.</returns>
        public string RetornarIdade()
        {
            return Anos > 0 ? $"{Anos} ano{(Anos == 1 ? "" : "s")}" : $"{Meses} mes{(Meses == 1 ? "" : "es")}";
        }

        /// <summary>
        /// Retorna uma <see cref="System.String" /> com a idade completa (anos, meses e dias).
        /// </summary>
        /// <returns>Uma <see cref="System.String" /> que representa esta instância.</returns>
        public override string ToString()
        {
            return
                $"{Anos} ano{(Anos == 1 ? "" : "s")}, {Meses} mes{(Meses == 1 ? "" : "es")} e {Dias} dia{(Dias == 1 ? "" : "s")}";
        }
    }
}
