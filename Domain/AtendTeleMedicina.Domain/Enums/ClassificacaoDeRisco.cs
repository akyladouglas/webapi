using System.ComponentModel.DataAnnotations;

namespace AtendTeleMedicina.Domain.Enums
{
    public enum ClassificacaoDeRisco
    {
        [Display(Name = "Baixa")]
        Baixa = 1,

        [Display(Name = "Média")]
        Média = 2,

        [Display(Name = "Alta")]
        Alta = 3,

        [Display(Name = "Muito Alta")]
        MuitoAlta = 4
    }
}
