using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.Infrastructure.CrossCutting.Notification.Validations;
using System;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class ExamesModel : BaseModel
    {
        #region Propriedades
        public string Id { get; set; }
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

        public virtual IndividuoModel Individuo { get; set; }
        public virtual TipoExameModel TipoExame { get; set; }
        public virtual ProfissionalModel Profissional { get; set; }



        #endregion
    }
}
