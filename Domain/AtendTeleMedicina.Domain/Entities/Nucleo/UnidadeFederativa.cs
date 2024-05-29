using AtendTeleMedicina.Domain.Entities.Base;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class UnidadeFederativa : EntidadeBase
    {
        public int Id { get; set; }
        public string UfAbreviado { get; set; }
        public string UfExtenso { get; set; }
        public bool Ativo { get; set; }
    }
}
