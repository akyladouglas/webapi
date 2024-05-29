using System;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.Infrastructure.CrossCutting.Notification.Validations;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class UnidadeFederativaModel : BaseModel
    {
        public int Id { get; set; }
        public string UfAbreviado { get; set; }
        public string UfExtenso { get; set; }
        public bool Ativo { get; set; }

        public void ValidarAdicionar()
        {
            ValidarString();
            AddNotifications(new Contract()
                .Requires()
                .IsNotNullOrEmpty(UfAbreviado, "UnidadeFederativa.UfAbreviado", "Abreviação UF Inválida")
                .HasLen(UfAbreviado, 2, "UnidadeFederativa.UfAbreviado", "UF Abreviado deve conter 2 caracteres"));
        }

        public void ValidarEditar()
        {
            ValidarString();
            AddNotifications(new Contract()
                .Requires()
                .IsNotNullOrEmpty(UfAbreviado, "UnidadeFederativa.UfAbreviado", "Abreviação UF Inválida")
                .HasLen(UfAbreviado, 2, "UnidadeFederativa.UfAbreviado", "UF Abreviado deve conter 2 caracteres"));
        }

        public void ValidarString()
        {
            UfAbreviado = ValidateText(UfAbreviado, 2);
            UfExtenso = ValidateText(UfExtenso, 120);
        }

        public void ValidarGet()
        {
            ValidarString();

            AddNotifications(new Contract()
                .Requires()
                .HasLen(UfAbreviado, 2, "UnidadeFederativa.UfAbreviado", "Uf deve conter 2 caracteres")
            );
        }
    }
}
