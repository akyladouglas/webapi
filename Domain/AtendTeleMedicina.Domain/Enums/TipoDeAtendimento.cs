using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AtendTeleMedicina.Domain.Enums
{
    /// <summary>
    /// Enum TipoDeAtendimento
    /// </summary>
    public enum TipoDeAtendimento
    {
        [Display(Name = "Consulta agendada programada / Cuidado continuado")]
        CONSULTA_AGENDADA_PROGRAMADA_CUIDADO_CONTINUADO = 1,
        [Display(Name = "Consulta agendada")]
        CONSULTA_AGENDADA = 2,
        [Display(Name = "Escuta inicial / Orientação")]
        ESCUTA_INICIAL_ORIENTACAO = 4,
        [Display(Name = "Consulta no dia")]
        CONSULTA_NO_DIA = 5,
        [Display(Name = "Atendimento de urgência")]
        ATENDIMENTO_DE_URGENCIA = 6,
        [Display(Name = "Atendimento programado")]
        ATENDIMENTO_PROGRAMADO = 7,
        [Display(Name = "Atendimento não programado")]
        ATENDIMENTO_NAO_PROGRAMADO = 8,
        [Display(Name = "Visita domiciliar pós-óbito")]
        VISITA_DOMICILIAR_POS_OBITO = 9
    }
}
