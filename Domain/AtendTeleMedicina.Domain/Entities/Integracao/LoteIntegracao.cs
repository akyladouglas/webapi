using System;
using System.Collections.Generic;
using System.Text;

namespace AtendTeleMedicina.Domain.Entities.Integracao
{
    public class LoteIntegracao
    {
        public int Lote_Id { get; set; }
        public Guid Cadastro_Id { get; set; }
        public DateTime DataGeracao { get; set; }
        public string DadosXml { get; set; }
        public string Erros { get; set; }
        public string Tipo { get; set; }
        public bool Status { get; set; }

        protected LoteIntegracao() { }

        public LoteIntegracao(int id, Guid cadastro_Id, string tipo, bool status, string dadosXml, string erros, DateTime data)
        {
            Lote_Id = id;
            Cadastro_Id = cadastro_Id;
            Tipo = tipo;
            Status = status;
            DadosXml = dadosXml;
            Erros = erros;
            DataGeracao = data;
        }
    }
}
