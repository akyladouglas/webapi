using AtendTeleMedicina.Domain.Entities.Base;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class Ciap : EntidadeBase
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public string Sexo { get; set; }
        public bool? AD { get; set; }
        public bool? AI { get; set; }
    }
}
