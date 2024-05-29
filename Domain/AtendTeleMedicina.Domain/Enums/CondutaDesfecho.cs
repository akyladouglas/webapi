using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AtendTeleMedicina.Domain.Enums
{
    /// <summary>
    /// Enum Conduta Encaminhamento
    /// </summary>
    public enum CondutaDesfecho
    {
        [Display(Name = "Permanência")]
        PERMANENCIA = 7,

        [Display(Name = "Alta administrativa")]
        ALTA_ADMINISTRATIVA = 3,

        [Display(Name = "Alta clínica")]
        ALTA_CLINICA = 1,

        [Display(Name = "Óbito")]
        AGENDAMENTO_PARA_NASF = 9,

        [Display(Name = "Atenção Básica (AD1)")]
        ATENCAO_BASISCA_AD1 = 2,

        [Display(Name = "Serviço de urgência e emergência")]
        SERVICO_DE_URGENCIA_E_EMERGENCIA = 4,

        [Display(Name = "Serviço de internação hospitalar")]
        SERVICO_DE_INTERNACAO_HOSPITALAR = 5
    }
}
