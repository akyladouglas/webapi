using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Security;
using AtendTeleMedicina.Domain.Entities.Security;
using AtendTeleMedicina.Infrastructure.CrossCutting.Notification.Validations;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class AtendimentoModel : BaseModel
    {
        #region Propriedades
        public string Id { get; set; }
        public string TipoDaConsulta { get; set; }
        public string Subjetivo { get; set; }
        public string QueixaDoPaciente { get; set; }

        public DateTime InicioDoAtendimento { get; set; }
        public DateTime FimDoAtendimento { get; set; }
        public string TempoAtendimento { get; set; }

        public string Objetivo { get; set; }
        public string Avaliacao { get; set; }
        public string Plano { get; set; }
        public string Alergias { get; set; }
        public string Antecedentes { get; set; }

        public string CondicoesAvaliadas { get; set; }
        public string CondutaDesfecho { get; set; }
        public string ModalidadeAD { get; set; }
        public string ProcedimentosRealizados { get; set; }
        public string TipoAtendimento { get; set; }

        public string AgendamentoId { get; set; }
        public bool? AtendidoTriagem { get; set; }
        public bool? AtendidoMedico { get; set; }
        public bool? Ativo { get; set; }
        public bool? CadastroEditado { get; set; }
        public string DescricaoEditado { get; set; }
        public string UsuarioEditou { get; set; }
        public DateTime DataAlteracao { get; set; }
        public DateTime DataCadastro { get; set; }
        public string DataDoAtendimento { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public int LocalDeAtendimento { get; set;}
        public IList<IndividuoCiapModel> IndividuoCiap { get; set; }
        public virtual CiapModel Ciap { get; set; }
        public IList<IndividuoCid10Model> IndividuoCid10 { get; set; }
        public virtual CidModel Cid { get; set; }
        public IList<IndividuoProcedimentoModel> IndividuoProcedimentos { get; set; }
        public virtual IndividuoProcedimentoModel IndividuoProcedimento { get; set; }
        public ProfissionalModel Profissional { get; set; }
        public ProcedimentoModel Procedimento { get; set; }
        public AgendamentoModel Agendamento { get; set; }
        public bool Historico { get; set; }
        public string ProfissionalId { get; set; }
        public string IndividuoId { get; set; }
        public IndividuoModel Individuo { get; set; }
        public DateTime Periodo { get; set; }
        public int Total { get; set; }
        public bool? Retorno { get; set; }

        public UserModel Usuario { get; set; }
        public string UsuarioId { get; set; }


        public bool? PresencaConfirmada { get; set; }
        public bool? Finalizado { get; set; }
        public bool? Cancelado { get; set; }
        public bool? NaoCompareceu { get; set; }
        #endregion

        public void ValidarGet()
        {
            //ValidarString();
        }

        public void ValidarString()
        {
            //Nome = ValidateText(Nome, 120);
        }

        public void ValidarAdicionar()
        {
            //ValidarString();
            //AddNotifications(new Contract()
            //    .Requires()
            //    .IsNotNullOrEmpty(Subjetivo, "Atendimento.Subjetivo", "Subjetivo inválido")
            //    .IsNotNullOrEmpty(Objetivo, "Atendimento.Objetivo", "Objetivo inválido")
            //    .IsNotNullOrEmpty(Avaliacao, "Atendimento.Avaliacao", "Avaliacao inválido")
            //    .IsNotNullOrEmpty(Plano, "Atendimento.Plano", "Plano inválido")
            //    .IsNotNullOrEmpty(AgendamentoId, "Atendimento.AgendamentoId", "Agendamento inválido")
            //    );
        }

        //public void ValidarEditar()
        //{
        //    ValidarString();
        //    AddNotifications(new Contract()
        //        .Requires()
        //        .IsNotNullOrEmpty(Nome, "Profissional.Nome", "Nome inválido"));
        //}
    }
}
