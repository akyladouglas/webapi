using System.ComponentModel.DataAnnotations;

namespace AtendTeleMedicina.Domain.Enums
{
    /// <summary>
    /// Enum Etnia
    /// </summary>
    public enum RacaOuCor
    {
        /// <summary>
        /// Etnia branca
        /// </summary>
        [Display(Name = "Branca")]
        BRANCA = 1,

        /// <summary>
        /// Etnia preta / negra
        /// </summary>
        [Display(Name = "Preta")]
        PRETA = 2,

        /// <summary>
        /// Etnia parda
        /// </summary>
        [Display(Name = "Parda")]
        PARDA = 4,

        /// <summary>
        /// Etnia amarela
        /// </summary>
        [Display(Name = "Amarela")]
        AMARELA = 3,

        /// <summary>
        /// Etnia indigena
        /// </summary>
        [Display(Name = "Indígena")]
        INDIGENA = 5,

        /// <summary>
        /// Etnia Sem Informação
        /// </summary>
        [Display(Name = "Sem Informação")]
        SEM_INFORMACAO = 6
    }
}
