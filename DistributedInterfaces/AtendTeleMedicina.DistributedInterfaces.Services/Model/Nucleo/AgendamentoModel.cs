using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.Domain.Entities.Nucleo;
using AtendTeleMedicina.Domain.Enums;
using AtendTeleMedicina.Infrastructure.CrossCutting.Notification.Validations;
using System;
using System.Collections.Generic;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class AgendamentoModel : BaseModel
    {
        #region Propriedades
        public string Id { get; set; }
        public string IndividuoId { get; set; }
        public string ProfissionalId { get; set; }
        public string EstabelecimentoId { get; set; }
        public string EstabelecimentoProcedimentoId { get; set; }
        public string ProcedimentoId { get; set; }
        public DateTime? Dia { get; set; }
        public TimeSpan? Hora { get; set; }
        public string TipoDaConsulta { get; set; }

        //Interconsulta
        public bool? Interconsulta { get; set; }
        public bool? PacientePresente { get; set; }
        public IList<ProfissionaisInterconsulta> ProfissionaisInterconsulta { get; set; }
        public string Motivo { get; set; }

        public string Observacoes { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataAlteracao { get; set; }
        public virtual IndividuoModel Individuo { get; set; }
        public virtual ProfissionalModel Profissional { get; set; }
        public virtual EstabelecimentoModel Estabelecimento { get; set; }
        public virtual EstabelecimentoProcedimentoModel EstabelecimentoProcedimentoModel { get; set; }
        public virtual ProcedimentoModel Procedimento { get; set; }
        public bool? EmAndamento { get; set; }
        public bool? Finalizado { get; set; }
        public bool? Cancelado { get; set; }
        public bool? EmAtendimentoMedico { get; set; }
        public bool? Retorno { get; set; }
        public bool? SinaisVitaisGrafico { get; set; }
        public string RetornoAgendamentoId { get; set; }
        public bool? VinculoRetorno { get; set; }
        public bool? PresencaConfirmada { get; set; }
        public bool? NaoCompareceu { get; set; }
        public string PressaoSanguinea { get; set; }
        public string OxigenacaoSanguinea { get; set; }
        public string BatimentoCardiaco { get; set; }
        public string Altura { get; set; }
        public string Peso { get; set; }
        public string Temperatura { get; set; }
        public string CorStatusTriagem { get; set; }
        public string GraficoEcg { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }

        public string NumProntuario { get; set; }
        public string Condutas { get; set; }
        public TipoDeAtendimento TipoDeAtendimento { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        #endregion
    }
}
