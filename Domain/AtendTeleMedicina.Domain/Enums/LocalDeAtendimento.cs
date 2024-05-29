using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AtendTeleMedicina.Domain.Enums
{
    /// <summary>
    /// Enum Local de Atendimento
    /// </summary>
    public enum LocalDeAtendimento
    {
        [Display(Name = "UBS")]
        UBS = 1,
        [Display(Name = "Unidade móvel")]
        UNIDADE_MOVEL = 2,
        [Display(Name = "Rua")]
        RUA = 3,
        [Display(Name = "Domicílio")]
        DOMICILIO = 4,
        [Display(Name = "Escola / Creche")]
        ESCOLA_CRECHE = 5,
        [Display(Name = "Outros")]
        OUTROS = 6,
        [Display(Name = "Polo (academia da saúde)")]
        POLO = 7,
        [Display(Name = "Instituição / Abrigo")]
        INSTITUICAO_ABRIGO = 8,
        [Display(Name = "Unidade prisional ou congêneres")]
        UNIDADE_PRISIONAL_OU_CONGENERES = 9,
        [Display(Name = "Unidade socioeducativa")]
        UNIDADE_SOCIOEDUCATIVA = 10,
        [Display(Name = "Hospital")]
        HOSPITAL = 11,
        [Display(Name = "Unidade de pronto atendimento")]
        UNIDADE_DE_PRONTO_ATENDIMENTO = 12,
        [Display(Name = "CACON / UNACON")]
        CACON_UNACON = 13
    }
}
