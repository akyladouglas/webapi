using System;

namespace AtendTeleMedicina.Domain.Params
{
    public class UsuarioParams
    {
        public int? Id { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Username { get; set; }
        public string Cidade_Id { get; set; }
        public string Uf { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public string Individuo_Id { get; set; }
    }
}
