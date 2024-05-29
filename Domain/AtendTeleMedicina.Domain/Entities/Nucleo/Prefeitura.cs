using AtendTeleMedicina.Domain.Entities.Base;
using System;

namespace AtendTeleMedicina.Domain.Entities.Nucleo
{
    public class Prefeitura : EntidadeBase
    {
        #region Propriedades
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
        public virtual Cidade Cidade { get; set; }
        public int CidadeId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataAlteracao { get; set; }
        #endregion

        #region Construtores
        public Prefeitura()
        {

        }
        #endregion
    }
}
