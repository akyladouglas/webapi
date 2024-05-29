using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using System;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class AuditoriaModel : BaseModel
    {
        #region Propriedades
        public string Id { get; set; }
        public string Tabela { get; set; }
        public string TabelaId { get; set; }
        public string Acao { get; set; }
        public DateTime DataHora { get; set; }
        public string Origem { get; set; }
        public string AtorId { get; set; }
        public string Ip { get; set; }
        public string Justificativa { get; set; }

        public virtual string Usuario { get; set; }
        public virtual string Profissional { get; set; }
        public virtual string Individuo { get; set; }

        #endregion

        public void ValidarGet()
        {
            ValidarString();
        }

        public void ValidarString()
        {
            //Descricao = ValidateText(Descricao, 120);
            //Codigo = ValidateText(Codigo, 120);
        }

        //public void ValidarAdicionar()
        //{
        //    ValidarString();
        //    AddNotifications(new Contract()
        //        .Requires()
        //        .IsNotNullOrEmpty(Descricao, "Acompanhamento.Descricao", "Descrição Inválida")
        //        .IsNotNullOrEmpty(Descricao, "Acompanhamento.Codigo", "Código Inválida"));
        //}

        //public void ValidarEditar()
        //{
        //    ValidarString();
        //    AddNotifications(new Contract()
        //        .Requires()
        //        .IsNotNullOrEmpty(Descricao, "Acompanhamento.Descricao", "Descrição Inválida")
        //        .IsNotNullOrEmpty(Descricao, "Acompanhamento.Codigo", "Código Inválida"));
        //}
    }
}
