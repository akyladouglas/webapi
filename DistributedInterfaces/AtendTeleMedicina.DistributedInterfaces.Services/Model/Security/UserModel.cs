using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Security
{
    public class UserModel : BaseModel
    {
        public string Id { get; set; }

        private string _Nome { get; set; }
        public string EstabelecimentoId { get; set; }
        public string Nome { get { return _Nome; } set { _Nome = value?.ToUpper(); } }

        public string Cpf { get; set; }
        public string Cns { get; set; }

        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Username { get; set; }

        public string Senha { get; set; }
        public bool? SenhaAlterada { get; set; }
        public string Imagem { get; set; }
        public CidadeModel Cidade { get; set; }

        public int CidadeId { get; set; }

        public string Uf { get; set; }
        public string UfAbreviado { get; set; }

        public DateTime? DataCadastro { get; set; }

        public DateTime? DataAlteracao { get; set; }

        public bool? Ativo { get; set; }

        public bool? Aceite { get; set; }

        public bool? CadastroEditado { get; set; }

        public string DescricaoEditado { get; set; }

        public string UsuarioEditou { get; set; }
        public string UltimoPerfilSelecionado { get; set; }
        public int TentativaLogin { get; set; }
        public OcupacaoModel Ocupacao { get; set; }
        public int OcupacaoId { get; set; }



        public virtual string Audience { get; set; }
        public virtual string Scope { get; set; }

        public virtual IList Roles { get; set; }
        public IList<UserClaimModel> UserClaims { get; set; }

        public UserModel()
        {
            UserClaims = new List<UserClaimModel>();
        }

        public void ValidarAdicionar()
        {
            ValidarString();
        }

        public void ValidarEditar()
        {
            ValidarString();
        }

        public void ValidarString()
        {
            Username = ValidateText(Username, 120);
            Senha = ValidateText(Senha, 255);
        }

        public void ValidarGet()
        {
            ValidarString();
        }

    }
}
