using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AtendTeleMedicina.Domain.Enums
{
    /// <summary>
    /// Enum Conduta Encaminhamento
    /// </summary>
    public enum CondutaEncaminhamento
    {
        [Display(Name = "Retorno para consulta agendada")]
        RETORNO_PARA_CONSULTA_AGENDADA = 1,

        [Display(Name = "Retorno para cuidado continuado / programado")]
        RETORNO_PARA_CUIDADO_CONTINUADO_PROGRAMADO = 2,

        [Display(Name = "Agendamento para grupos")]
        AGENDAMENTO_PARA_GRUPOS = 12,

        [Display(Name = "Agendamento para NASF")]
        AGENDAMENTO_PARA_NASF = 3,

        [Display(Name = "Alta do episódio")]
        ALTA_DO_EPISODIO = 9,

        [Display(Name = "Encaminhamento interno no dia")]
        ENCAMINHAMENTO_INTERNO_NO_DIA = 11,

        [Display(Name = "Encaminhamento para serviço especializado")]
        ENCAMINHAMENTO_PARA_SERVICO_ESPECIALIZADO = 4,

        [Display(Name = "Encaminhamento para CAPS")]
        ENCAMINHAMENTO_PARA_CAPS = 5,

        [Display(Name = "Encaminhamento para internação hospitalar")]
        ENCAMINHAMENTO_PARA_INTERNACAO_HOSPITALAR = 6,

        [Display(Name = "Encaminhamento para urgência")]
        ENCAMINHAMENTO_PARA_URGENCIA = 7,

        [Display(Name = "Encaminhamento para serviço de atenção domiciliar")]
        ENCAMINHAMENTO_PARA_SERVICO_DE_ATENCAO_DOMICILIAR = 8,

        [Display(Name = "Encaminhamento intersetorial")]
        ENCAMINHAMENTO_INTERSETORIAL = 10
    }
}
