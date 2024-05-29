using AtendTeleMedicina.Domain.Entities.Base;
using AtendTeleMedicina.Domain.Entities.Security;
using System;
using System.Collections.Generic;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class Profissional : EntidadeBase
    {
        #region Propriedades

        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Cns { get; set; }
        public DateTime? DataNascimento { get; set; }
        public bool? CadastroEditado { get; set; }
        public string DescricaoEditado { get; set; }
        public string UsuarioEditou { get; set; }

        public int? Sexo { get; set; }
        public string Crm { get; set; }
        public string CrmUF { get; set; }
        public string Conselho { get; set; }
        public string Email { get; set; }
        public string CodigoAutenticacao { get; set; }
        public string Username { get; set; }
        public string Senha { get; set; }
        public bool? SenhaAlterada { get; set; }
        public string Telefone { get; set; }
        public string Logradouro { get; set; }
        public string LogradouroNumero { get; set; }
        public string LogradouroComplemento { get; set; }
        public string LogradouroBairro { get; set; }
        public string LogradouroCep { get; set; }
        public Cidade Cidade { get; set; }
        public int? CidadeId { get; set; }
        public string UfAbreviado { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataAlteracao { get; set; }
        public bool? Ativo { get; set; }
        public bool? Aceite { get; set; }
        public int TentativaLogin { get; set; }
        public string Imagem { get; set; }
        public string Token { get; set; }
        public EstabelecimentoProcedimentoHorario Procedimento { get; set; }
        public string ProcedimentoId { get; set; }
        public Ocupacao Ocupacao { get; set; }
        public int OcupacaoId { get; set; }
        public string EquipeId { get; set; }

        public IList<UserClaim> UserClaims { get; set; }
        public string UltimoPerfilSelecionado { get; set; }
        public virtual IList<Estabelecimento> Estabelecimentos { get; set; }
        public virtual IList<Procedimento> Procedimentos { get; set; }

        #endregion

        #region Construtores
        public Profissional()
        {

        }
        #endregion
    }
}
