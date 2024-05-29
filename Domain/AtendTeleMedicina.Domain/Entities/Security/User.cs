using System;
using System.Collections.Generic;
using AtendTeleMedicina.Domain.Entities.Base;
using AtendTeleMedicina.Domain.Entities.Nucleo;

namespace AtendTeleMedicina.Domain.Entities.Security
{
    public class User : EntidadeBase
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string EstabelecimentoId { get; set; }
        public string UltimoPerfilSelecionado { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Senha { get; set; }
        public bool? SenhaAlterada { get; set; }
        public Cidade Cidade { get; set; }
        public int CidadeId { get; set; }
        public string Uf { get; set; }
        public DateTime? DataCadastro { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public bool? Ativo { get; set; }
        public bool? Aceite { get; set; }
        public string Telefone { get; set; }
        public string Cns { get; set; }
        public string CodigoAutenticacao { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string Sexo { get; set; }
        public string Logradouro { get; set; }
        public string LogradouroNumero { get; set; }
        public string LogradouroComplemento { get; set; }
        public string LogradouroBairro { get; set; }
        public string LogradouroCep { get; set; }
        public bool? CadastroEditado { get; set; }
        public string DescricaoEditado { get; set; }
        public string UsuarioEditou { get; set; }
        public int TentativaLogin { get; set; }
        public Ocupacao Ocupacao { get; set; }
        public int OcupacaoId { get; set; }

        public IList<UserClaim> UserClaims { get; set; }

        public User()
        {
            UserClaims = new List<UserClaim>();
        }
    }
}
