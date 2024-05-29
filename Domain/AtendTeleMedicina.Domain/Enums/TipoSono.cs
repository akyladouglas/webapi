using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AtendTeleMedicina.Domain.Enums
{
    /// <summary>
    /// Enum Tipo de Sono
    /// </summary>
    public enum TipoSono
    {
        /// <summary>
        /// Tipo de sono: Sono leve
        /// </summary>
        [Display(Name = "Sono leve")]
        SONOLEVE = 1,

        /// <summary>
        /// Tipo de sono: Sono profundo
        /// </summary>
        [Display(Name = "Sono profundo")]
        SONOPROFUNDO = 2,
    }
}
