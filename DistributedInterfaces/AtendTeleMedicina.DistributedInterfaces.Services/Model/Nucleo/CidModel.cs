using System;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.Infrastructure.CrossCutting.Notification.Validations;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class CidModel : BaseModel
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public string Sexo { get; set; }

        //public void ValidarAdicionar()
        //{
        //    ValidarString();
        //    AddNotifications(new Contract()
        //        .Requires()
        //        .IsNotNullOrEmpty(Codigo, "Cid10.Codigo", "Código Inválido"));
        //        //.HasLen(UfAbreviado, 2, "UnidadeFederativa.UfAbreviado", "UF Abreviado deve conter 2 caracteres"));
        //}

        //public void ValidarEditar()
        //{
        //    ValidarString();
        //    AddNotifications(new Contract()
        //        .Requires()
        //        .IsNotNullOrEmpty(Codigo, "Cid10.Codigo", "Codigo Inválido"));

        //}

        //public void ValidarString()
        //{
        //    Codigo = ValidateText(Codigo, 3);
        //    Descricao = ValidateText(Descricao, 255);
        //}

        //public void ValidarGet()
        //{
        //    ValidarString();

        //    AddNotifications(new Contract()
        //        .Requires()
        //        .HasLen(Codigo, 3, "Cid10.Codigo", "Codigo deve conter 3 caracteres")
        //    );
        //}
    }
}
