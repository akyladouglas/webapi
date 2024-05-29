using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AtendTeleMedicina.Domain.Enums
{
    /// <summary>
    /// Enum Nacionalidade
    /// </summary>
    public enum Nacionalidade
    {
        // Na integração adicionar um valor a mais ex: Brasileira=0 + 1 = 1 TB_NACIONALIDADE

        /// <summary>
        /// Nacionalidade brasileira
        /// </summary>
        [Display(Name = "Brasileiro")]
        BRASILEIRA = 1,

        /// <summary>
        /// Nacionalidade naturalizada
        /// </summary>
        [Display(Name = "Naturalizado")]
        NATURALIZADO = 2,

        /// <summary>
        /// Nacionalidade estrangeira
        /// </summary>
        [Display(Name = "Estrangeiro")]
        ESTRANGEIRO = 3
    }
}
