using AtendTeleMedicina.Domain.Entities.Base;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class Bairro : EntidadeBase
    {
        #region Propriedades
        public string Nome { get; set; }
        public virtual Cidade Cidade { get; set; }
        public int CidadeId { get; set; }
        public bool Ativo { get; set; }
        #endregion

        #region Construtores
        public Bairro()
        {

        }
        #endregion
    }
}
