using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.Infrastructure.CrossCutting.Notification.Validations;
using System;
using System.Collections.Generic;
using System.Text;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class AcompanhamentoModel : BaseModel
    {
        #region Propriedades
        public string Id { get; set; }
        public string IndividuoId { get; set; }
        public DateTime Data { get; set; }
        public bool? Temperatura { get; set; }
        public bool? Tosse { get; set; }
        public bool? Coriza { get; set; }
        public bool? DorCorpo { get; set; }
        public bool? DorAbdominal { get; set; }
        public bool? Fraqueza { get; set; }
        public bool? DorGarganta { get; set; }
        public bool? NauseaVomito { get; set; }
        public bool? DorCabeca { get; set; }
        public bool? SairCasa { get; set; }
        public bool? ContatoPessoas { get; set; }
        public bool? DificuldadeRespirar { get; set; }
        public bool? Taquicardia { get; set; }
        public bool? PerdaOlfatoPaladar { get; set; }
        public bool? Diarreia { get; set; }
        public bool? TemperaturaRetornou { get; set; }
        //public bool? DiasComFebre { get; set; }
        public bool? AtendidoServicoSaude { get; set; }
        public bool? CorStatus { get; set; }
        //public string IndividuoCorona_Id { get; set; }
        public string Local_Id { get; set; }
        public string PressaoSanguinea { get; set; }
        public string BatimentoCardiaco { get; set; }
        public string OxigenacaoSanguinea { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? Deletado { get; set; }
        public bool? SintomasGripais { get; set; }
        public string AtendimentoId { get; set; }
        public string Outros { get; set; }
        public DateTime DataAtendidoServicoSaude { get; set; }

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
