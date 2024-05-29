using AtendTeleMedicina.DistributedInterfaces.Services.Model.Base;
using System;

namespace AtendTeleMedicina.DistributedInterfaces.Services.Model.Nucleo
{
    public class PrefeituraModel : BaseModel
    {
        #region Propriedades
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Cnpj { get; set; }
        public string Prefeito { get; set; }
        public string Logradouro { get; set; }
        public string LogradouroNumero { get; set; }
        public string LogradouroComplemento { get; set; }
        public string LogradouroBairro { get; set; }
        public string LogradouroCep { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public virtual CidadeModel Cidade { get; set; }
        public int CidadeId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataAlteracao { get; set; }
        #endregion

        public void ValidarGet()
        {
            ValidarString();
        }

        public void ValidarString()
        {
            Nome = ValidateText(Nome, 100);
            Cnpj = ValidateText(Cnpj, 18);
            Prefeito = ValidateText(Prefeito, 50);
            Logradouro = ValidateText(Logradouro, 100);
            LogradouroNumero = ValidateText(LogradouroNumero, 10);
            LogradouroComplemento = ValidateText(LogradouroComplemento, 50);
            LogradouroBairro = ValidateText(LogradouroBairro, 100);
            LogradouroCep = ValidateText(LogradouroCep, 8);
            Email = ValidateText(Email, 50);
            Telefone = ValidateText(Telefone, 20);
            Latitude = ValidateText(Latitude, 20);
            Longitude = ValidateText(Longitude, 20);

        }
    }
}
