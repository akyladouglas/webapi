using AtendTeleMedicina.Domain.Entities.Base;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class Pais : EntidadeBase
    {

        #region Construtores
        public Pais()
        {
        }
        #endregion
        public int Id { get; set; }
        public string Nome { get; set; }
    }
}