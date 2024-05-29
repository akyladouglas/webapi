using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AtendTeleMedicina.Domain.Enums
{
    /// <summary>
    /// Enum GrauDeInstrucao cujo os Ids correspondem a numeração padrão do eSus AB
    /// </summary>
    public enum GrauDeInstrucao
    {
        /// <summary>
        /// Creche
        /// </summary>
        [Display(Name = "Creche")] CRECHE = 51,

        /// <summary>
        /// Pré-Escola (Exceto CA)
        /// </summary>
        [Display(Name = "Pré-Escola (Exceto CA)")]
        PRE_ESCOLA_EXCETO_CA = 52,

        /// <summary>
        /// Classe Alfabetizada - CA
        /// </summary>
        [Display(Name = "Classe de alfabetização - CA")]
        CLASSE_ALFABETIZADA_CA = 53,

        /// <summary>
        /// Ensino Fundamental 1ª a 4ª séries
        /// </summary>
        [Display(Name = "Ensino Fundamental 1ª a 4ª séries")]
        ENSINO_FUNDAMENTAL_1_4_SERIES = 54,

        /// <summary>
        /// Ensino Fundamental 5ª a 8ª séries
        /// </summary>
        [Display(Name = "Ensino Fundamental 5ª a 8ª séries")]
        ENSINO_FUNDAMENTAL_5_8_SERIES = 55,

        /// <summary>
        /// Ensino Fundamental Completo
        /// </summary>
        [Display(Name = "Ensino Fundamental Completo")]
        ENSINO_FUNDAMENTAL_COMPLETO = 56,

        /// <summary>
        /// Ensino Fundamental Especial
        /// </summary>
        [Display(Name = "Ensino Fundamental Especial")]
        ENSINO_FUNDAMENTAL_ESPECIAL = 61,

        /// <summary>
        /// Ensino Fundamental EJA - Séries Iniciais (Supletivo 1ª a 4ª)
        /// </summary>
        [Display(Name = "Ensino Fundamental EJA - Séries Iniciais (Supletivo 1ª a 4ª)")]
        ENSINO_FUNDAMENTAL_EJA_SERIES_INICIAIS_SUPLETIVO_1_4 = 58,

        /// <summary>
        /// Ensino Fundamental EJA - Séries Iniciais (Supletivo 5ª a 9ª)
        /// </summary>
        [Display(Name = "Ensino fundamental EJA - séries finais (supletivo 5ª a 8ª)")]
        ENSINO_FUNDAMENTAL_EJA_SERIES_INICIAIS_SUPLETIVO_5_8 = 59,

        /// <summary>
        /// Ensino Médio, Médio 2º Ciclo (Científico, Técnico e etc)
        /// </summary>
        [Display(Name = "Ensino Médio, Médio 2º Ciclo (Científico, Técnico e etc)")]
        ENSINO_MEDIO_MEDIO_2_CICLO = 60,

        /// <summary>
        /// Ensino Médio Especial
        /// </summary>
        [Display(Name = "Ensino Médio Especial")]
        ENSINO_MEDIO_ESPECIAL = 57,

        /// <summary>
        /// Ensino Médio EJA (Supletivo)
        /// </summary>
        [Display(Name = "Ensino Médio EJA (Supletivo)")]
        ENSINO_MEDIO_EJA_SUPLETIVO = 62,

        /// <summary>
        /// Superior, Aperfeiçoamento, Especialização, Mestrado, Doutorado
        /// </summary>
        [Display(Name = "Superior, Aperfeiçoamento, Especialização, Mestrado, Doutorado")]
        SUPERIOR_APERFEICOAMENTO_ESPECIALIZACAO_MESTRADO_DOUTORADO = 63,

        /// <summary>
        /// Alfabetização para Adultos (Mobral, etc)
        /// </summary>
        [Display(Name = "Alfabetização para Adultos (Mobral, etc)")]
        ALFABETIZACAO_PARA_ADULTROS_MOBRAL_ETC = 64,

        /// <summary>
        /// Nenhum curso frequentado
        /// </summary>
        [Display(Name = "Nenhum")]
        NENHUM = 65
    }
}
