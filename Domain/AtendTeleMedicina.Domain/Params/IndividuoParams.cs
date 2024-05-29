using System;
using System.Collections.Generic;
using System.Text;

namespace AtendTeleMedicina.Domain.Params
{
    public class IndividuoParams
    {
        public string NomeCompleto { get; set; }
        public string Email { get; set; }
        public int? Sexo { get; set; }
        public int? Monitoramento { get; set; }
        public string ResultadoExame { get; set; }
        public int? Pesquisa { get; set; }
        public int? CorStatus { get; set; }
        public int? Agravamento { get; set; }
        public DateTime? DataInternacao { get; set; }
        public int? DiasDeInternacao { get; set; }
        public string Cpf { get; set; }
        public string CartaoSUS { get; set; }
        public int? Confirmado { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public DateTime? DataUltimoAtendimento { get; set; }
        public DateTime? DataMedicao { get; set; }
        public string Cidade_Id { get; set; }
        public string Uf { get; set; }
        public int? Usuario_Id { get; set; }
        public string CorStatusIn { get; set; }
        public bool? Atendimento { get; set; }
        public int? EstaEmIsolamento { get; set; }
        public bool? ExcluirCidade { get; set; }
        public int? GrauDePrioridade { get; set; }
        public int? Horas { get; set; }
        // Comorbidades
        public int? Hipertenso { get; set; }
        public int? Asma { get; set; }
        public int? Diabetes { get; set; }
        public int? DoencaCoracao { get; set; }
        public int? DoencaPulmao { get; set; }
        public int? DoencaRins { get; set; }
        public int? DoencaFigado { get; set; }
        public int? DoencaCancer { get; set; }
        public int? Transplantado { get; set; }
        public int? Fumante { get; set; }
        public int? DoencaComprometeImunidade { get; set; }

    }
}
