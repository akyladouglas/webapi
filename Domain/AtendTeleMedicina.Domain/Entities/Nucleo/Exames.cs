using AtendTeleMedicina.Domain.Entities.Base;
using System;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class Exames : EntidadeBase
    {
        #region Propriedades
        public string IndividuoId { get; set; }
        public string IndividuoCpf { get; set; }
        public string TipoExameId { get; set; }
        public string Formato { get; set; }
        public DateTime DataDeEnvio { get; set; }
        public string Url { get; set; }
        public string Nome { get; set; }
        public string ProfissionalId { get; set; }
        public DateTime? Solicitado { get; set; }
        public DateTime? Avaliado { get; set; }
        public bool? Resultado { get; set; }
        public string Descricao { get; set; }
        public bool? Finalizado { get; set; }
        public bool? Deletado { get; set; }


        public virtual Individuo Individuo { get; set; }
        public virtual TipoExame TipoExame { get; set; }
        public virtual Profissional Profissional { get; set; }
        #endregion

        #region Construtores
        public Exames()
        {

        }
        #endregion
    }
}
