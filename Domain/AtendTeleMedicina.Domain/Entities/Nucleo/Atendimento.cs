using AtendTeleMedicina.Domain.Entities.Base;
using AtendTeleMedicina.Domain.Entities.Security;
using AtendTeleMedicina.Domain.Enums;
using AtendTeleMedicina.Domain.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class Atendimento : EntidadeBase
    {
        #region Propriedades
        public string AgendamentoId { get; set; }
        public string TipoDaConsulta { get; set; }
        public string Subjetivo { get; set; }
        public string QueixaDoPaciente { get; set; }

        public DateTime InicioDoAtendimento { get; set; }
        public DateTime FimDoAtendimento { get; set; }
        public string TempoAtendimento { get; set; }

        public string Alergias { get; set; }
        public string Antecedentes { get; set; }
        public string Objetivo { get; set; }
        public string Avaliacao { get; set; }
        public string Plano { get; set; }
        public string CondicoesAvaliadas { get; set; }
        public string CondutaDesfecho { get; set; }
        public string ModalidadeAD { get; set; }
        public string ProcedimentosRealizados { get; set; }
        public string TipoAtendimento { get; set; }
        public Guid? DadoSerializado { get; set; }
        public int? LoteIntegracaoId { get; set; }
        public bool AtendidoTriagem { get; set; }
        public bool AtendidoMedico { get; set; }
        public DateTime DataCadastro { get; set; }
        public string DataDoAtendimento { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public bool? Ativo { get; set; }
        public bool? CadastroEditado { get; set; }
        public string DescricaoEditado { get; set; }
        public string UsuarioEditou { get; set; }
        public DateTime DataAlteracao { get; set; }
        public IList<IndividuoCiap> IndividuoCiap { get; set; }
        public virtual Ciap Ciap { get; set; }
        public IList<IndividuoCid10> IndividuoCid10 { get; set; }
        public virtual Cid Cid { get; set; }
        public IList<IndividuoProcedimento> IndividuoProcedimentos { get; set; }
        public virtual IndividuoProcedimento IndividuoProcedimento { get; set; }

        public virtual Procedimento Procedimento { get; set; }
        public virtual Profissional Profissional { get; set; }
        public virtual Agendamento Agendamento { get; set; }

        
        public bool Historico { get; set; }
        public string ProfissionalId { get; set; }
        public string IndividuoId { get; set; }
        public Individuo Individuo { get; set; }
        public DateTime Periodo { get; set; }
        public int Total { get; set; }
        public bool? Retorno { get; set; }
        public string NumProntuario { get; set; }
        public User Usuario { get; set; }
        public string UsuarioId { get; set; }
        public LocalDeAtendimento LocalDeAtendimento { get; set; }

        public bool? PresencaConfirmada { get; set; }
        public bool? Finalizado { get; set; }
        public bool? Cancelado { get; set; }
        public bool? NaoCompareceu { get; set; }

        #endregion

        #region Construtores
        public Atendimento()
        {

        }
        #endregion
    }
}
