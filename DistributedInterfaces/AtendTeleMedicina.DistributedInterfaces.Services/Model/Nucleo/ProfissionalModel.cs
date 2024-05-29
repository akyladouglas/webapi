using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.Domain.Entities.Security;
using AtendTeleMedicina.Infrastructure.CrossCutting.Notification.Validations;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class ProfissionalModel : BaseModel
    {
        #region Propriedades
        public string Id { get; set; }
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
        public string Username { get; set; }
        public string Senha { get; set; }
        public bool? SenhaAlterada { get; set; }
        public string Telefone { get; set; }
        public string Logradouro { get; set; }
        public string LogradouroNumero { get; set; }
        public string LogradouroComplemento { get; set; }
        public string LogradouroBairro { get; set; }
        public string LogradouroCep { get; set; }
        public CidadeModel Cidade { get; set; }
        public int? CidadeId { get; set; }
        public string UfAbreviado { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataAlteracao { get; set; }
        public bool? Ativo { get; set; }
        public bool? Aceite { get; set; }
        public int TentativaLogin { get; set; }

        public string Imagem { get; set; }
        public string Token { get; set; }
        public EstabelecimentoProcedimentoHorarioModel Procedimento { get; set; }
        public string ProcedimentoId { get; set; }
        public OcupacaoModel Ocupacao { get; set; }
        public int OcupacaoId { get; set; }
        public int EquipeId { get; set; }

        public IList<UserClaim> UserClaims { get; set; }
        public string UltimoPerfilSelecionado { get; set; }
        public virtual IList<EstabelecimentoModel> Estabelecimentos { get; set; }
        public virtual IList<ProcedimentoModel> Procedimentos { get; set; }
        #endregion

        public void ValidarGet()
        {
            ValidarString();
        }

        public void ValidarString()
        {
            Nome = ValidateText(Nome, 120);
        }

        public void ValidarAdicionar()
        {
            ValidarString();
            AddNotifications(new Contract()
                .Requires()
                .IsNotNullOrEmpty(Nome, "Profissional.Nome", "Nome inválido"));
        }

        public void ValidarEditar()
        {
            ValidarString();
            AddNotifications(new Contract()
                .Requires()
                .IsNotNullOrEmpty(Nome, "Profissional.Nome", "Nome inválido"));
        }
    }
}
