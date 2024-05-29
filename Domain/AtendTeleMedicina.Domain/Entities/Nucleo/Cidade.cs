
namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class Cidade
    {
        #region Propriedades
        public int Ibge { get; set; }
        public string Nome { get; set; }
        public string UfAbreviado { get; set; }
        public string UfExtenso { get; set; }
        public bool Capital { get; set; }
        public bool Ativo { get; set; }
        public string Regiao { get; set; }
        #endregion

        #region Construtores
        public Cidade()
        {

        }
        #endregion
    }
}
