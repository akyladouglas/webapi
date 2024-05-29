using System;
using AtendTeleMedicina.Domain.Entities.Base;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class Notificacao : EntidadeBase
    {
        #region Propriedades
        public string Titulo { get; set; }

        public string Descricao { get; set; }

        public DateTime DataCadastro { get; set; }

        public DateTime DataAlteracao { get; set; }

        public bool? Deletado { get; set; }
        public string[] IndividuoIds { get; set; }


        public string IndividuoId { get; set; }

        public Individuo Individuo { get; set; }
        #endregion

        #region Construtores
        public Notificacao()
        {

        }
        #endregion
    }
}
