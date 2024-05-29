using AtendTeleMedicina.Domain.Entities.Base;
using AtendTeleMedicina.Domain.Enums;
using AtendTeleMedicina.Domain.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class Agendamento : EntidadeBase
    {
        #region Propriedades
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
        public virtual Individuo Individuo { get; set; }
        public virtual Profissional Profissional { get; set; }
        public virtual Estabelecimento Estabelecimento { get; set; }
        public virtual EstabelecimentoProcedimento EstabelecimentoProcedimento { get; set; }
        public virtual Procedimento Procedimento { get; set; }
        public bool? EmAndamento { get; set; }
        public bool? EmAtendimentoMedico { get; set; }
        public bool? Finalizado { get; set; }
        public bool? Cancelado { get; set; }
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
        public string NumProntuario { get; set; }
        public Guid? DadoSerializado { get; set; }
        public TipoDeAtendimento TipoDeAtendimento { get; set; }
        public string Condutas { get; private set; }
        public virtual List<CondutaEncaminhamento> ListaCondutas
        {
            get
            {
                var lista = Condutas?.Split(',').ToList();
                return lista != null ? EnumService<CondutaEncaminhamento>.ParseList(lista).ToList() : new List<CondutaEncaminhamento>();
            }
            set { Condutas = value != null ? string.Join(",", ListaCondutas) : null; }
        }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public IList<ExamesF200> ExamesF200 { get; set; }
        public IList<IndividuoCiap> IndividuoCiap { get; set; }
        public IList<IndividuoCid10> IndividuoCid10 { get; set; }
        public IList<Atendimento> Atendimentos { get; set; }

        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }

        #endregion

        #region Construtores
        public Agendamento()
        {

        }
        #endregion
    }
}
