using System;
using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using AtendTeleMedicina.Infrastructure.CrossCutting.Notification.Validations;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class CidadeModel : BaseModel
    {
        #region Propriedades
        public int Ibge { get; set; }
        public string Nome { get; set; }
        public bool Capital { get; set; }
        public string UfAbreviado { get; set; }
        public string UfExtenso { get; set; }
        public bool Ativo { get; set; }
        public string Regiao { get; set; }
        #endregion

        public void ValidarGet()
        {
            ValidarString();
        }
        public void ValidarString()
        {
            Nome = ValidateText(Nome, 72);
            UfAbreviado = ValidateText(UfAbreviado, 2);
            UfExtenso = ValidateText(UfExtenso, 255);
            Regiao = ValidateText(Regiao, 255);
        }

        public void ValidarAdicionar()
        {
            ValidarString();
            AddNotifications(new Contract()
                .Requires()
                .IsNotNullOrEmpty(UfAbreviado, "Cidade.UfAbreviado", "Abreviação UF Inválida")
                .HasLen(UfAbreviado, 2, "Cidade.UfAbreviado", "UF Abreviado deve conter 2 caracteres"));
        }   

        public void ValidarEditar()
        {
            ValidarString();
            AddNotifications(new Contract()
                .Requires()
                .IsNotNullOrEmpty(UfAbreviado, "UnidadeFederativa.UfAbreviado", "Abreviação UF Inválida")
                .HasLen(UfAbreviado, 2, "UnidadeFederativa.UfAbreviado", "UF Abreviado deve conter 2 caracteres"));
        }

    }
}
