using AtendTeleMedicina.Domain.Entities.Nucleo;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtendTeleMedicina.Domain.Entities.Integracao
{
    public class Lote
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string Usuario_Id { get; set; }
        public int Mes { get; set; }
        public int Ano { get; set; }
        public string Estabelecimento_Id { get; set; }
        public int Validos { get; set; }
        public int Erros { get; set; }

        public virtual Estabelecimento Estabelecimento { get; set; }
        public virtual List<LoteIntegracao> LoteIntegracao { get; set; }

        public Lote(int id, DateTime data, string usuario_Id, int mes, int ano, string estabalecimentoId)
        {
            Id = id;
            Data = data;
            Usuario_Id = usuario_Id;
            Mes = mes;
            Ano = ano;
            if (!string.IsNullOrEmpty(estabalecimentoId))
            Estabelecimento_Id = estabalecimentoId;
            LoteIntegracao = new List<LoteIntegracao>();
        }

        public Lote(int id)
        {
            Id = id;
            LoteIntegracao = new List<LoteIntegracao>();
        }

        protected Lote()
        {
            LoteIntegracao = new List<LoteIntegracao>();
        }

        public void AdicionarItems(ICollection<LoteIntegracao> Items)
        {
            LoteIntegracao.AddRange(Items);
        }
    }
}
