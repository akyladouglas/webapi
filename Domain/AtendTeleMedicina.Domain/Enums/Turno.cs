using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AtendTeleMedicina.Domain.Enums
{
    /// <summary>
    /// Enum Turno
    /// </summary>
    public enum Turno
    {
        /// <summary>
        /// Manhã
        /// </summary>
        [Display(Name = "Manhã")]
        MANHA = 1,

        /// <summary>
        /// Tarde
        /// </summary>
        [Display(Name = "Tarde")]
        TARDE = 2,

        /// <summary>
        /// Noite
        /// </summary>
        [Display(Name = "Noite")]
        NOITE = 3

    }
}
